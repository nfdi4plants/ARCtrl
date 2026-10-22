namespace ARCtrl.CWL

open YAMLicious
open DynamicObj

module ResizeArray =

    let map  f (a : ResizeArray<_>) =
        let b = ResizeArray<_>()
        for i in a do
            b.Add(f i)
        b

module Decode =

    type DecodeWarning = {
        Path: string
        Message: string
        Raw: string option
    }

    type DecodeResult<'T> = {
        Value: 'T
        Warnings: ResizeArray<DecodeWarning>
    }

    type WarningSink = ResizeArray<DecodeWarning> option

    /// Add a recoverable decode warning only when the caller requested warning collection.
    let addWarning (warnings: WarningSink) (path: string) (message: string) (raw: YamlNode option) =
        match warnings with
        // Public non-warning APIs pass None, so tolerance paths can share this helper
        // without allocating a collection or changing old call behavior.
        | Some warningList ->
            warningList.Add({
                Path = path
                Message = message
                // The warning shows the skipped entry as the YAML that it was written in.
                Raw = raw |> Option.map (fun node -> (Writer.writeNode WriterOptions.defaults node).TrimEnd('\n'))
            })
        | None ->
            ()

    /// An alias is an error in a CWL document. YAMLicious keeps aliases symbolic, and a raw value that kept one
    /// would point at an anchor that the written document no longer holds.
    let private failAliasAt (path: string list) (name: string) (node: YamlNode) : 'T =
        raise (
            YamlDecodeException
                { Message = "The alias *" + name + " is not supported, because CWL documents do not use YAML aliases."
                  Path = path
                  Span = node.Span }
        )

    let internal failAlias (name: string) (node: YamlNode) : 'T =
        failAliasAt [] name node

    /// The place of a node in the text, as it stands in the message of an error.
    let private placeOf (node: YamlNode) : string =
        match node.Span with
        | Some span -> "line " + string span.Start.Line + ", column " + string span.Start.Column
        | None -> "an unknown place"

    /// Fail with the first alias or the first repeated scalar key in the tree. Raw values keep their entries as they
    /// were written, so a repeated key would be written back twice, and YAML 1.2 asks for unique keys. The walk
    /// keeps its own stack, so a raw value of any depth is safe.
    let private rejectAliasesAndRepeatedKeys (root: YamlNode) : unit =
        // The nodes that are still to visit, each with the path to it. The path is built backwards.
        let pending = ResizeArray<YamlNode * string list>()
        pending.Add((root, []))
        while pending.Count > 0 do
            let (node, reversedPath) = pending.[pending.Count - 1]
            pending.RemoveAt(pending.Count - 1)
            match node.Kind with
            | YamlNodeKind.Alias name -> failAliasAt (List.rev reversedPath) name node
            | YamlNodeKind.Scalar _ -> ()
            | YamlNodeKind.Sequence collection ->
                // The last item goes in first, so the first item comes out first.
                collection.Items
                |> List.mapi (fun index item -> item.Node, ("[" + string index + "]") :: reversedPath)
                |> List.rev
                |> List.iter pending.Add
            | YamlNodeKind.Mapping collection ->
                let seen = System.Collections.Generic.Dictionary<string, YamlNode>()
                let children = ResizeArray<YamlNode * string list>()
                collection.Items
                |> List.iteri (fun index entry ->
                    let name =
                        match entry.Key.Kind with
                        | YamlNodeKind.Scalar scalar ->
                            match seen.TryGetValue scalar.Value with
                            | true, first ->
                                raise (
                                    YamlDecodeException
                                        { Message =
                                            "The key " + scalar.Value + " occurs twice, at " + placeOf first
                                            + " and at " + placeOf entry.Key + "."
                                          Path = List.rev (scalar.Value :: reversedPath)
                                          Span = entry.Key.Span }
                                )
                            | _ ->
                                seen.[scalar.Value] <- entry.Key
                                scalar.Value
                        | _ -> "?" + string index
                    children.Add((entry.Key, name :: reversedPath))
                    children.Add((entry.Value, name :: reversedPath)))
                for index in children.Count - 1 .. -1 .. 0 do
                    pending.Add children.[index]

    /// Read the root node of a CWL document. The text must hold one document without aliases or repeated keys.
    let readYaml (yaml: string) : YamlNode =
        let root = Reader.readRoot yaml
        rejectAliasesAndRepeatedKeys root
        root

    /// Determines if an exception represents a recoverable decoding error.
    /// Returns true for schema mismatches; false for system errors that should propagate.
    let isRecoverableDecodingError (ex: exn) : bool =
        match ex with
        | YamlDecodeException _ -> true
        // Type-based matching for known exception types
        | :? System.Collections.Generic.KeyNotFoundException -> true
        | :? System.ArgumentException -> true
        | :? System.FormatException -> true
        | :? System.InvalidOperationException
            when ex.Message.Contains("decode") -> true
        // All other exceptions (including system-critical) should propagate
        | _ -> false

    /// Box int64 values in a way that preserves 64-bit numeric intent across Fable targets.
    let boxOverflowInt64 (value: int64) : obj =
#if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyExpr value "int64($0)"
#else
        box value
#endif

    /// The type that the YAML 1.2 core schema gives a scalar node. This is the rule behind the quoting of
    /// Encode.string, so a decoder has to read the plain spellings the same way. Quoted and block scalars
    /// are text, and an explicit tag fixes the type. A node that is no scalar counts as text.
    let internal (|CoreNull|CoreBool|CoreInt|CoreFloat|CoreText|) (node: YamlNode) =
        match node.Kind with
        | YamlNodeKind.Scalar scalar ->
            let tag = node.Properties.Tag
            if CoreSchema.isNull tag scalar then
                CoreNull
            else
                match CoreSchema.tryParseBool tag scalar with
                | Some parsed -> CoreBool parsed
                | None ->
                    match CoreSchema.tryParseInt64 tag scalar with
                    | Some parsed -> CoreInt parsed
                    | None ->
                        match CoreSchema.tryParseFloat tag scalar with
                        | Some parsed -> CoreFloat parsed
                        | None -> CoreText
        | _ -> CoreText

    /// The entries of a mapping in source order. The keys must be scalars, and a key must not occur twice.
    let mappingEntries (node: YamlNode) : MappingEntry list =
        node |> Decode.object (fun get -> get.Entries)

    /// The text of the key of an entry.
    let entryName (entry: MappingEntry) : string =
        Decode.string entry.Key

    /// The text of a type. The text null is the CWL type null, so only a missing value fails. CWL requires the
    /// other values of this file with Decode.nonNull, which fails on a missing value and on a plain null.
    let typeText (node: YamlNode) : string =
        if YamlNode.isMissingValue node then
            Decode.nonNull Decode.string node
        else
            Decode.string node

    /// Run a decoder for the value of a map entry and put the name of the entry in the path of its error.
    let atField (name: string) (decode: unit -> 'T) : 'T =
        try
            decode ()
        with YamlDecodeException error ->
            raise (YamlDecodeException { error with Path = name :: error.Path })

    /// Decode key value pairs into a dynamic object, while preserving their tree structure.
    let rec overflowDecoder (dynObj: DynamicObj) (entries: MappingEntry list) =
        let decodeOverflowScalar (node: YamlNode) : obj =
            match node with
            | CoreBool parsed -> box parsed
            | CoreInt parsed -> boxOverflowInt64 parsed
            | CoreFloat parsed -> box parsed
            | CoreNull ->
                // A null has no value to hold in the object. The YAML node keeps the spelling and the type,
                // and the encoder writes it back as it was.
                box { Node = node }
            | CoreText -> box (Decode.string node)

        let rec decodeOverflowValue (node: YamlNode) : obj =
            match node.Kind with
            | YamlNodeKind.Scalar _ ->
                decodeOverflowScalar node
            | YamlNodeKind.Sequence collection ->
                // Preserve lists recursively as ResizeArray<obj> to match the rest of
                // the CWL object model.
                collection.Items
                |> List.map (fun item -> decodeOverflowValue item.Node)
                |> ResizeArray
                |> box
            | YamlNodeKind.Mapping _ ->
                // Nested mappings become nested DynamicObj instances instead of
                // flattened string values.
                let nested = DynamicObj()
                node
                |> mappingEntries
                |> overflowDecoder nested
                |> box
            | YamlNodeKind.Alias name ->
                failAlias name node

        for entry in entries do
            // The caller already filtered known fields with FieldList, so every
            // entry here is dynamic extension metadata.
            DynObj.setProperty (entryName entry) (decodeOverflowValue entry.Value) dynObj
        dynObj

    /// Decode an optional field of a node that may be a scalar shorthand. A node that is no mapping has no
    /// fields, and a field with a null value counts as missing.
    let tryGetPresentField (fieldName: string) (decoder: YamlNode -> 'T) (node: YamlNode) : 'T option =
        match node.Kind with
        | YamlNodeKind.Mapping _ -> node |> Decode.object (fun get -> get.Optional.Field fieldName decoder)
        | _ -> None

    let tryGetStringField (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName Decode.string value

    let tryGetBoolField (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName Decode.bool value

    let tryGetYamlField (fieldName: string) (value: YamlNode) : RawYaml option =
        tryGetPresentField fieldName (fun node -> { Node = node }) value

    /// Decode the doc and the label of a mapping, each with the node that it was read from. The texts go to the
    /// Doc and Label properties of an object, and the nodes go to its SourceNodes, so that the encoder can write the
    /// layout of the author again. A field with a null value counts as missing, as it does for the other fields.
    /// A doc and a label are strings in CWL, so a plain scalar that the core schema reads as a number or a bool is a
    /// decode error at the field. Every other scalar is a string, for example a quoted one or one tagged !!str. A doc
    /// may also be a list of strings, which Schema Salad concatenates without a separator, and its node is the
    /// sequence.
    let internal docAndLabelDecoder (value: YamlNode) : string option * string option * SourceNodes =
        let stringText (node: YamlNode) : string =
            let typeName =
                match node with
                | CoreNull -> Some "null"
                | CoreBool _ -> Some "a bool"
                | CoreInt _ -> Some "an int"
                | CoreFloat _ -> Some "a float"
                | CoreText -> None
            match typeName with
            | Some name ->
                let found = if name = "null" then "null" else "the plain scalar " + Decode.string node + ", which the core schema reads as " + name
                raise (
                    YamlDecodeException
                        { Message = "Expected a string, found " + found + ". Write it in quotes to make it a string."
                          Path = []
                          Span = node.Span }
                )
            | None -> Decode.string node
        let label (node: YamlNode) = stringText node, node
        let doc (node: YamlNode) =
            match node.Kind with
            | YamlNodeKind.Sequence collection ->
                let parts = collection.Items |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> stringText item.Node))
                String.concat "" parts, node
            | _ -> stringText node, node
        let docField = tryGetPresentField "doc" doc value
        let labelField = tryGetPresentField "label" label value
        Option.map fst docField, Option.map fst labelField, { Doc = Option.map snd docField; Label = Option.map snd labelField }

    let tryGetIntArrayField (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName (Decode.resizearray Decode.int) value

    /// Decode int64 fields through the core schema so File sizes do not narrow to int and a quoted size
    /// fails like the other typed fields.
    let tryGetInt64Field (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName Decode.int64 value

    /// Decode and validate CWL loadListing enum fields from their YAML string form.
    let tryGetLoadListingField (fieldName: string) (value: YamlNode) =
        tryGetStringField fieldName value
        |> Option.map (fun loadListingValue ->
            match LoadListingEnum.tryParse loadListingValue with
            | Some parsed -> parsed
            | None -> raise (System.ArgumentException($"Invalid loadListing value '{loadListingValue}'. Expected one of: no_listing, shallow_listing, deep_listing.")))

    /// Preserve unknown object fields on DynamicObj-backed CWL objects.
    let overflowIntoDynamicObj (dynObj: DynamicObj) (knownFields: string list) (value: YamlNode) =
        match value.Kind with
        | YamlNodeKind.Mapping _ ->
            // get.Overflow.FieldList removes fields consumed by the typed decoder;
            // overflowDecoder stores the remaining fields without flattening them.
            value
            |> Decode.object (fun get -> overflowDecoder dynObj (get.Overflow.FieldList knownFields))
            |> ignore
        | _ ->
            // Scalar shorthand has no field-level overflow to preserve.
            ()
        dynObj

    /// Decode File object fields while preserving unrecognized extension metadata.
    let decodeFileInstanceFields (element: YamlNode) =
        let file =
            FileInstance(
                ?location = tryGetStringField "location" element,
                ?path = tryGetStringField "path" element,
                ?basename = tryGetStringField "basename" element,
                ?dirname = tryGetStringField "dirname" element,
                ?nameroot = tryGetStringField "nameroot" element,
                ?nameext = tryGetStringField "nameext" element,
                ?checksum = tryGetStringField "checksum" element,
                ?size = tryGetInt64Field "size" element,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" element,
                ?format = tryGetStringField "format" element,
                ?contents = tryGetStringField "contents" element
            )
        overflowIntoDynamicObj file (FileInstance.KnownFieldNames |> Seq.toList) element |> ignore
        file

    /// Decode Directory object fields while preserving unrecognized extension metadata.
    let decodeDirectoryInstanceFields (element: YamlNode) =
        let directory =
            DirectoryInstance(
                ?location = tryGetStringField "location" element,
                ?path = tryGetStringField "path" element,
                ?basename = tryGetStringField "basename" element,
                ?listing = tryGetYamlField "listing" element
            )
        overflowIntoDynamicObj directory (DirectoryInstance.KnownFieldNames |> Seq.toList) element |> ignore
        directory

    /// Decode scalar schema-salad string fields.
    /// Recognized directive wrappers are `$include` and `$import`.
    /// Unknown single-key mappings are intentionally coerced to a legacy literal `key: value` string.
    let decodeSchemaSaladString (node: YamlNode) : SchemaSaladString =
        let unexpected () =
            raise (System.ArgumentException($"Unexpected YAML node in decodeSchemaSaladString: {YamlNode.show node}"))
        match node.Kind with
        | YamlNodeKind.Scalar _ ->
            // Common path: scalar values become literal schema-salad strings. YAMLicious folds the
            // line breaks of a multi-line plain scalar into spaces, as YAML 1.2.2 requires.
            SchemaSaladString.Literal (Decode.nonNull Decode.string node)
        | YamlNodeKind.Mapping collection ->
            match collection.Items with
            | [ entry ] when (YamlNode.tryScalar entry.Value).IsSome ->
                // Directive forms keep their directive kind so encode can write them
                // back as mappings instead of lossy literal text.
                let key = entryName entry
                let text = Decode.nonNull Decode.string entry.Value
                match key with
                | "$include" -> SchemaSaladString.Include text
                | "$import" -> SchemaSaladString.Import text
                | _ -> SchemaSaladString.Literal (sprintf "%s: %s" key text)
            | _ -> unexpected ()
        | _ -> unexpected ()

    /// Decode a YamlNode which is either a string or expression into a string.
    /// Directive objects such as {$include: path} are represented using legacy string form.
    let decodeStringOrExpression (node: YamlNode) =
        decodeSchemaSaladString node
        |> SchemaSaladString.toDirectiveString

    /// Decode a YamlNode into a glob search pattern for output binding
    let outputBindingGlobDecoder: (YamlNode -> OutputBinding) =
        fun value ->
            Decode.object (fun get ->
                // Read all known outputBinding fields first; any implementation
                // extension fields are attached below through overflowIntoDynamicObj.
                let glob = get.Optional.Field "glob" Decode.string
                let binding =
                    OutputBinding(
                        ?glob = glob,
                        ?loadContents = get.Optional.Field "loadContents" Decode.bool,
                        ?loadListing = tryGetLoadListingField "loadListing" value,
                        ?outputEval = get.Optional.Field "outputEval" Decode.string
                    )
                overflowIntoDynamicObj binding (OutputBinding.KnownFieldNames |> Seq.toList) value |> ignore
                binding
            ) value

    /// Decode a YamlNode into an OutputBinding. A port in scalar shorthand has none.
    let outputBindingDecoder: (YamlNode -> OutputBinding option) =
        tryGetPresentField "outputBinding" outputBindingGlobDecoder

    /// Decode schema-salad string-or-array fields into a uniform ResizeArray.
    let decodeStringArrayOrScalar (value: YamlNode) : ResizeArray<string> =
        match value.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.map (fun item -> decodeStringOrExpression item.Node)
            |> ResizeArray
        | _ ->
            ResizeArray [| decodeStringOrExpression value |]

    /// Decode CWL outputSource in either scalar or list form.
    let outputSourceDecoder: (YamlNode -> ResizeArray<string> option) =
        tryGetPresentField "outputSource" decodeStringArrayOrScalar

    /// Decode a YamlNode into a Dirent
    let direntDecoder: (YamlNode -> CWLType) =
        fun value ->
            Decode.object (fun get ->
                let dirent =
                    DirentInstance(
                        get.Required.Field "entry" decodeSchemaSaladString,
                        ?entryname = get.Optional.Field "entryname" decodeSchemaSaladString,
                        ?writable = get.Optional.Field "writable" Decode.bool
                    )
                overflowIntoDynamicObj dirent (DirentInstance.KnownFieldNames |> Seq.toList) value |> ignore
                Dirent dirent
            ) value

    /// Decode a listing entry of InitialWorkDirRequirement.
    /// Supports both Dirent object form and string/expression form.
    let initialWorkDirEntryDecoder: (YamlNode -> InitialWorkDirEntry) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Mapping collection ->
                // Objects with entry are Dirent entries even if they also contain
                // optional entryname/writable fields.
                let hasEntryField =
                    collection.Items
                    |> List.exists (fun entry -> entryName entry = "entry")

                if hasEntryField then
                    match direntDecoder value with
                    | Dirent dirent -> DirentEntry dirent
                    | _ -> raise (System.ArgumentException("Unexpected InitialWorkDir Dirent decoding result."))
                else
                    // File and Directory entries are discriminated by their class field;
                    // all other objects are treated as schema-salad string directives.
                    match tryGetStringField "class" value with
                    | Some "File" ->
                        // File and Directory object decoding is shared with CWL type decoding so
                        // listing entries preserve the same extension metadata.
                        FileEntry (decodeFileInstanceFields value)
                    | Some "Directory" ->
                        DirectoryEntry (decodeDirectoryInstanceFields value)
                    | _ ->
                        StringEntry (decodeSchemaSaladString value)
            | YamlNodeKind.Scalar _ ->
                // Scalar listing entries are allowed by CWL and represent generated
                // file content or expressions.
                StringEntry (decodeSchemaSaladString value)
            | _ ->
                raise (System.ArgumentException($"Invalid InitialWorkDir listing entry: {YamlNode.show value}"))

    /// The type that a type name spells. The node is the scalar that holds the name, and an unknown name is a decode
    /// error at its place.
    let cwlSimpleTypeFromString (node: YamlNode) (name: string) : CWLType =
        match name with
        | "File" -> File (FileInstance ())
        | "Directory" -> Directory (DirectoryInstance ())
        | "string" -> String
        | "int" -> Int
        | "long" -> Long
        | "float" -> Float
        | "double" -> Double
        | "boolean" -> Boolean
        | "stdout" -> Stdout
        | "null" -> Null
        | "record" | "enum" | "array" ->
            raise (
                YamlDecodeException
                    { Message =
                        "Expected a type name, found \"" + name + "\". A schema is a mapping, so a schema belongs under type."
                      Path = []
                      Span = node.Span }
            )
        | _ -> raise (YamlDecodeException { Message = "Invalid CWL type: " + name; Path = []; Span = node.Span })

    /// Read a type name with the shorthand of a CWL document. T[] is an array of T and T? is null or T, so T[]? is an
    /// optional array. A document reads one [] only, because CWL reads a nested array as a schema. The flag tells
    /// whether the place of the node reads the shorthand, which a type under items does not. The second result tells
    /// whether the text ended in a question mark.
    let cwlTypeFromShorthand (shorthand: bool) (node: YamlNode) (direntListing: unit -> CWLType) : CWLType * bool =
        let fail (message: string) : 'T =
            raise (YamlDecodeException { Message = message; Path = []; Span = node.Span })
        let text = typeText node
        let optional = text.EndsWith "?"
        let withoutOptional = if optional then text.Substring(0, text.Length - 1) else text
        let isArray = withoutOptional.EndsWith "[]"
        let name = if isArray then withoutOptional.Substring(0, withoutOptional.Length - 2) else withoutOptional
        if (optional || isArray) && not shorthand then
            fail (
                "Expected a type name or a schema under items, found " + text + ". CWL reads the shorthand [] and ? as a type and "
                + "as a member of a type list. Write the array as a schema with type: array and items."
            )
        elif name.EndsWith "[]" then
            fail ("Expected one [] behind a type name, found " + text + ". CWL reads a nested array as a schema with type: array and items.")
        else
            let baseType = if name = "Dirent" && not isArray then direntListing () else cwlSimpleTypeFromString node name
            let arrayType = if isArray then Array (InputArraySchema(baseType)) else baseType
            (if optional then Union (ResizeArray [ Null; arrayType ]) else arrayType), optional

    /// Decode an InputArraySchema from a YamlNode
    let rec inputArraySchemaDecoder: (YamlNode -> InputArraySchema) =
        fun value ->
        Decode.object (fun get ->
            // The items are a type name or a schema, with no shorthand.
            let decodedItems = get.Required.Field "items" itemsTypeDecoder

            // Array schema metadata is optional, but unknown fields still need to
            // remain on the schema object for lossless roundtrips.
            let doc, label, sourceNodes = docAndLabelDecoder value
            let schema =
                InputArraySchema(
                    decodedItems,
                    ?label = label,
                    ?doc = doc,
                    ?name = get.Optional.Field "name" Decode.string
                )
            schema.SourceNodes <- sourceNodes
            overflowIntoDynamicObj schema (InputArraySchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value
    /// Decode a record field object under its name. The type field is the type of the field. The doc and the label
    /// are typed, and every other property of the field, such as inputBinding or secondaryFiles, stays on the field.
    and inputRecordFieldObject (name: string) (value: YamlNode) : InputRecordField =
        let decodedType = value |> Decode.object (fun get -> get.Required.Field "type" cwlTypeDecoder')
        let doc, label, sourceNodes = docAndLabelDecoder value
        let field = InputRecordField(name, decodedType, ?doc = doc, ?label = label)
        field.SourceNodes <- sourceNodes
        overflowIntoDynamicObj field (InputRecordField.KnownFieldNames |> Seq.toList) value |> ignore
        field

    /// Decode an InputRecordField from a YamlNode
    and inputRecordFieldDecoder: (YamlNode -> InputRecordField) =
        fun value ->
            // Array-style record fields carry their own name field.
            let name = value |> Decode.object (fun get -> get.Required.Field "name" (Decode.nonNull Decode.string))
            atField name (fun () -> inputRecordFieldObject name value)

    /// Decode fields as an array: [{name: x, type: y}]
    and decodeFieldsAsArray (element: YamlNode) : ResizeArray<InputRecordField> =
        Decode.resizearray inputRecordFieldDecoder element

    /// Decode fields as a map: {fieldName: type} or {fieldName: {type: ..., inputBinding: ...}}
    and decodeFieldsAsMap (element: YamlNode) : ResizeArray<InputRecordField> =
        let fields = ResizeArray<InputRecordField>()
        for entry in mappingEntries element do
            // Map-style record fields use the mapping key as the field name. A mapping is the field object, with the
            // type and the other properties of the field. Any other value is the type itself.
            let name = entryName entry
            let field =
                atField name (fun () ->
                    match entry.Value.Kind with
                    | YamlNodeKind.Mapping _ -> inputRecordFieldObject name entry.Value
                    | _ -> InputRecordField(name, cwlTypeDecoder' entry.Value))
            fields.Add(field)
        fields

    /// Decode an InputRecordSchema from a YamlNode
    and inputRecordSchemaDecoder: (YamlNode -> InputRecordSchema) =
        fun value ->
        Decode.object (fun get ->
            // Try to decode fields as an array (flow-style) or as a map (block-style)
            let decodedFields =
                // Get the fields element directly
                let fieldsElement = get.Optional.Field "fields" Decode.node

                match fieldsElement with
                | Some element when (YamlNode.tryMapping element |> Option.exists (fun mapping -> mapping.Items.IsEmpty)) ->
                    // An empty mapping, fields: {}, has no fields. An empty list, fields: [], takes the array path below.
                    Some (ResizeArray<InputRecordField>())
                | Some element ->
                    // A list holds field objects with their names, and a mapping holds the fields under their names.
                    atField "fields" (fun () ->
                        match element.Kind with
                        | YamlNodeKind.Sequence _ -> Some (decodeFieldsAsArray element)
                        | YamlNodeKind.Mapping _ -> Some (decodeFieldsAsMap element)
                        | _ ->
                            raise (
                                YamlDecodeException
                                    { Message = "Expected a list of fields or a mapping of fields, found " + YamlNode.show element + "."
                                      Path = []
                                      Span = element.Span }
                            ))
                | None -> None

            // A record schema may be incomplete in some authoring contexts; keep
            // whatever fields were available and preserve additional schema keys.
            let doc, label, sourceNodes = docAndLabelDecoder value
            let schema =
                InputRecordSchema(
                    ?fields = decodedFields,
                    ?label = label,
                    ?doc = doc,
                    ?name = get.Optional.Field "name" Decode.string
                )
            schema.SourceNodes <- sourceNodes
            overflowIntoDynamicObj schema (InputRecordSchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value

    /// Decode an InputEnumSchema from a YamlNode
    and inputEnumSchemaDecoder: (YamlNode -> InputEnumSchema) =
        fun value ->
        Decode.object (fun get ->
            let symbols = get.Required.Field "symbols" (Decode.resizearray (Decode.nonNull Decode.string))

            let doc, label, sourceNodes = docAndLabelDecoder value
            let schema =
                InputEnumSchema(
                    symbols,
                    ?label = label,
                    ?doc = doc,
                    ?name = get.Optional.Field "name" Decode.string
                )
            schema.SourceNodes <- sourceNodes
            overflowIntoDynamicObj schema (InputEnumSchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value

    /// Decode a CWLType from a YamlNode (handles all types including complex schemas). A document reads the shorthand
    /// File[] and File? as the whole value of a type field and as a member of a type list.
    and cwlTypeDecoder' (element: YamlNode): CWLType =
        cwlTypeDecoderAt true element

    /// Decode the type under items. It is a name or a schema, and a list of those, with no shorthand.
    and itemsTypeDecoder (element: YamlNode): CWLType =
        cwlTypeDecoderAt false element

    /// Decode a CWLType from a YamlNode. The flag tells whether the place reads the shorthand of a type name.
    and cwlTypeDecoderAt (shorthand: bool) (element: YamlNode): CWLType =
        let noListing () = cwlSimpleTypeFromString element "Dirent"

        let parseTypeObjectString (typeNode: YamlNode) =
            // Object forms such as {type: File, secondaryFiles: ...} need the
            // surrounding object to populate File/Directory metadata. The name of the type
            // goes through the reader of type names, which decides whether this place reads the shorthand.
            // The path of an error shows the type field for the name only, because the other fields of the
            // object are read from the object itself.
            let parsed, isOptional =
                atField "type" (fun () ->
                    cwlTypeFromShorthand shorthand typeNode (fun () -> cwlSimpleTypeFromString typeNode "Dirent"))
            let typeStr = typeText typeNode
            let stripped = if isOptional then typeStr.Substring(0, typeStr.Length - 1) else typeStr
            let optionalOf (baseType: CWLType) = if isOptional then Union (ResizeArray [Null; baseType]) else baseType

            match stripped with
            | "File" -> optionalOf (File (decodeFileInstanceFields element))
            | "Directory" -> optionalOf (Directory (decodeDirectoryInstanceFields element))
            | _ -> parsed

        match element.Kind with
        | YamlNodeKind.Scalar _ ->
            // Simple type string
            cwlTypeFromShorthand shorthand element noListing |> fst
        | YamlNodeKind.Sequence collection ->
            // Union type. Each member is a type name with the shorthand of its place, an array schema, a record or an enum.
            let types =
                collection.Items
                |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> cwlTypeDecoderAt shorthand item.Node))
                |> ResizeArray
            Union types
        | YamlNodeKind.Mapping _ ->
            // Complex type - check for type field
            Decode.object (fun get ->
                match get.Optional.Field "type" Decode.node with
                | Some typeNode ->
                    match typeNode.Kind with
                    | YamlNodeKind.Scalar _ ->
                        // Schema objects advertise their kind in the type field. Simple
                        // File/Directory object types are handled separately to keep metadata.
                        match Decode.string typeNode with
                        | "record" -> Record (inputRecordSchemaDecoder element)
                        | "enum" -> Enum (inputEnumSchemaDecoder element)
                        | "array" -> Array (inputArraySchemaDecoder element)
                        | _ -> parseTypeObjectString typeNode
                    | _ ->
                        // Nested complex type, for example type: { type: array, ... }, or a union list.
                        atField "type" (fun () -> cwlTypeDecoderAt shorthand typeNode)
                | None ->
                    raise (
                        YamlDecodeException
                            { Message = "Expected a type name or a schema, found a mapping without a type field."
                              Path = []
                              Span = element.Span }
                    )
            ) element
        | YamlNodeKind.Alias name -> failAlias name element

    /// Decode the type of a type that stands without a surrounding mapping. Such a type has no listing field, so the
    /// Dirent shorthand is an error.
    let cwlTypeStringDecoder (node: YamlNode) =
        cwlTypeFromShorthand
            true
            node
            (fun () -> raise (YamlDecodeException { Message = "The Dirent type needs a listing field."; Path = []; Span = node.Span }))

    /// Decode the type text of the type field of a mapping and check if it is optional.
    let cwlTypeStringMatcher (node: YamlNode) (get: Decode.IGetters) =
        cwlTypeFromShorthand true node (fun () -> get.Required.Field "listing" direntDecoder)

    /// Decode the text of the additional type of an RO-Crate entity. The text is YAML. A scalar is a type name such as
    /// File[][]? and may be quoted, as the null type is written ("null"), or carry a comment. The shorthand nests
    /// arrays there, which a CWL document does not read. Anything else is a schema or a list of types.
    let cwlTypeFromAdditionalType (text: string) : CWLType =
        let node = readYaml text
        match node.Kind with
        | YamlNodeKind.Scalar _ ->
            let typeName = typeText node
            let optional = typeName.EndsWith "?"
            let stripped = if optional then typeName.Substring(0, typeName.Length - 1) else typeName
            // Any number of [] stands behind the name.
            let mutable name = stripped
            let mutable depth = 0
            while name.EndsWith "[]" do
                name <- name.Substring(0, name.Length - 2)
                depth <- depth + 1
            let mutable arrayType = cwlSimpleTypeFromString node name
            for _ in 1 .. depth do
                arrayType <- Array (InputArraySchema(arrayType))
            if optional then Union (ResizeArray [ Null; arrayType ]) else arrayType
        | _ -> cwlTypeDecoder' node

    /// A type list of exactly null and one other type is the optional type, as the shorthand File? is. The encoder
    /// writes it as that shorthand, so the flag has to be set when the list is read.
    let private isOptionalUnion (cwlType: CWLType) : bool =
        match cwlType with
        | Union types ->
            types.Count = 2 && types |> Seq.exists (function Null -> true | _ -> false)
        | _ -> false

    /// Access the type field and decode a YamlNode into a CWLType
    let cwlTypeDecoder: (YamlNode -> CWLType*bool) =
        Decode.object (fun get ->
            let typeNode = get.Required.Field "type" Decode.node
            match typeNode.Kind with
            | YamlNodeKind.Scalar _ ->
                // Scalar type field, including shorthand such as File[]?.
                atField "type" (fun () -> cwlTypeStringMatcher typeNode get)
            | _ ->
                // Complex type field, for example inline record/enum/array schema.
                let cwlType = get.Required.Field "type" cwlTypeDecoder'
                cwlType, isOptionalUnion cwlType
        )

    /// Decode the type of a port. CWL lets a map entry hold just the type, written as a type string
    /// (File?) or as a list of types (a union such as [null, File]). Any other value is the port object.
    let portTypeDecoder (value: YamlNode) : CWLType * bool =
        match value.Kind with
        | YamlNodeKind.Scalar _ ->
            cwlTypeStringDecoder value
        | YamlNodeKind.Sequence _ ->
            let cwlType = cwlTypeDecoder' value
            cwlType, isOptionalUnion cwlType
        | _ -> cwlTypeDecoder value

    /// Decode a named output from either map shorthand or full object form.
    let decodeNamedOutput (name: string) (value: YamlNode) =
        // Decode dependent fields before constructing the output so both shorthand
        // and object forms share the same object initialization path.
        let outputBinding = outputBindingDecoder value
        let outputSourceValues = outputSourceDecoder value
        let cwlType = portTypeDecoder value |> fst
        let doc, label, sourceNodes = docAndLabelDecoder value
        // Optional presentation and behavior fields are read directly from the
        // object form; shorthand values simply leave them absent.
        let output =
            CWLOutput(
                name,
                cwlType,
                ?label = label,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" value,
                ?streamable = tryGetBoolField "streamable" value,
                ?doc = doc,
                ?format = tryGetStringField "format" value
        )
        output.SourceNodes <- sourceNodes
        output.OutputBinding <- outputBinding
        match outputSourceValues with
        // Preserve CWL's scalar-versus-array distinction as a discriminated union.
        | Some values when values.Count > 1 -> output.OutputSource <- Some (OutputSource.Multiple values)
        | Some values when values.Count = 1 -> output.OutputSource <- Some (OutputSource.Single values.[0])
        | _ -> ()
        overflowIntoDynamicObj output (CWLOutput.KnownFieldNames |> Seq.toList) value |> ignore
        output

    /// Decode one sequence-form output, warning only for malformed unnamed entries.
    let decodeOutputSequenceItem (warnings: WarningSink) (path: string) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        | Some id -> Some (atField id (fun () -> decodeNamedOutput id item))
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL output entry." (Some item)
            None

    /// Decode a YamlNode into an Output Array
    let outputArrayDecoderWithWarnings (warnings: WarningSink) (path: string) : (YamlNode -> ResizeArray<CWLOutput>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form ports must declare id on each item. Unnamed malformed
                // items are the tolerant path and produce warnings.
                collection.Items
                |> List.mapi (fun index item -> decodeOutputSequenceItem warnings path index item.Node)
                |> List.choose id
                |> ResizeArray
            | _ ->
                // Map-form ports use the map key as the port name.
                [|
                    for entry in mappingEntries value do
                        let name = entryName entry
                        atField name (fun () -> decodeNamedOutput name entry.Value)
                |]
                |> ResizeArray

    let outputArrayDecoder: (YamlNode -> ResizeArray<CWLOutput>) =
        outputArrayDecoderWithWarnings None "outputs"

    /// Access the outputs field and decode a YamlNode into an Output Array
    let outputsDecoderWithWarnings (warnings: WarningSink) : (YamlNode -> ResizeArray<CWLOutput>) =
        Decode.object (fun get ->
            get.Required.Field "outputs" (outputArrayDecoderWithWarnings warnings "outputs")
        )

    let outputsDecoder: (YamlNode -> ResizeArray<CWLOutput>) =
        outputsDecoderWithWarnings None

    /// Decode a YamlNode into a DockerRequirement
    let dockerRequirementDecoder (get: Decode.IGetters): DockerRequirement =
        // dockerFile can be a plain string or schema-salad include/import directive.
        let dockerFile =
            get.Optional.Field "dockerFile" Decode.node
            |> Option.map decodeSchemaSaladString

        // Constructor normalization gives dockerFileReference precedence over the
        // legacy plain dockerFile argument.
        DockerRequirement.create(
            ?dockerPull = get.Optional.Field "dockerPull" Decode.string,
            ?dockerFileReference = dockerFile,
            ?dockerImageId = get.Optional.Field "dockerImageId" Decode.string,
            ?dockerLoad = get.Optional.Field "dockerLoad" Decode.string,
            ?dockerImport = get.Optional.Field "dockerImport" Decode.string,
            ?dockerOutputDirectory = get.Optional.Field "dockerOutputDirectory" Decode.string,
            ?dockerRunOptions = get.Optional.Field "cwltool:dockerRunOptions" decodeStringArrayOrScalar
        )

    /// Decode a YamlNode into an EnvVarRequirement array.
    /// Supports both array form and map shorthand form (envName -> envValue).
    let envVarRequirementDecoder (get: Decode.IGetters): ResizeArray<EnvironmentDef> =
        // Env var values are strings by CWL semantics. The scalar text is already unquoted, so
        // a quoted "true" and a plain true both decode to the string true.
        let decodeEnvValue = decodeStringOrExpression

        get.Required.Field "envDef" (fun envDefElement ->
            match envDefElement.Kind with
            | YamlNodeKind.Sequence _ ->
                // Standard CWL form: envDef is an array of {envName, envValue} objects.
                Decode.resizearray
                    (fun value ->
                        Decode.object (fun get2 ->
                            let env =
                                EnvironmentDef(
                                    get2.Required.Field "envName" (Decode.nonNull Decode.string),
                                    get2.Required.Field "envValue" (Decode.nonNull Decode.string)
                                )
                            overflowIntoDynamicObj env (EnvironmentDef.KnownFieldNames |> Seq.toList) value |> ignore
                            env
                        ) value)
                    envDefElement
            | YamlNodeKind.Mapping _ ->
                // Shorthand form: envDef is a map from variable name to value.
                mappingEntries envDefElement
                |> List.map (fun entry ->
                    let name = entryName entry
                    EnvironmentDef(name, atField name (fun () -> decodeEnvValue entry.Value)))
                |> ResizeArray
            | _ ->
                raise (System.ArgumentException("Invalid envDef format. Expected array or map.")))

    /// Decode a YamlNode into a SoftwareRequirement array.
    /// Supports both array form and map shorthand form.
    let softwareRequirementDecoder (get: Decode.IGetters): ResizeArray<SoftwarePackage> =
        let decodeSpecsArray (element: YamlNode) =
            // Version and specs can be scalar-or-array schema-salad strings.
            Decode.resizearray decodeStringOrExpression element

        let decodePackageFromMapEntry packageName (packageValue: YamlNode) =
            // packages: { samtools: {} } and packages: { samtools: } both name a package without specs.
            match Decode.option Decode.node packageValue with
            | None ->
                SoftwarePackage(packageName)
            | Some value ->
                match value.Kind with
                | YamlNodeKind.Sequence _ ->
                    // packages: { samtools: [spec1, spec2] } means specs shorthand.
                    SoftwarePackage(packageName, specs = decodeSpecsArray value)
                | YamlNodeKind.Mapping collection when collection.Items.IsEmpty ->
                    SoftwarePackage(packageName)
                | YamlNodeKind.Mapping _ ->
                    // Full map shorthand can include version/specs plus extension fields.
                    let version = tryGetPresentField "version" decodeSpecsArray value
                    let specs = tryGetPresentField "specs" decodeSpecsArray value
                    let package = SoftwarePackage(packageName, ?version = version, ?specs = specs)
                    overflowIntoDynamicObj package (SoftwarePackage.KnownFieldNames |> Seq.toList) value |> ignore
                    package
                | _ ->
                    // Scalar map value is treated as one specs entry.
                    SoftwarePackage(packageName, specs = ResizeArray [| decodeStringOrExpression value |])

        get.Required.Field "packages" (fun packagesElement ->
            match packagesElement.Kind with
            | YamlNodeKind.Sequence _ ->
                // Standard CWL form: packages is an array of package objects.
                Decode.resizearray
                    (fun value ->
                        Decode.object (fun get2 ->
                            let package =
                                SoftwarePackage(
                                    get2.Required.Field "package" (Decode.nonNull Decode.string),
                                    ?version = get2.Optional.Field "version" (Decode.resizearray (Decode.nonNull Decode.string)),
                                    ?specs = get2.Optional.Field "specs" (Decode.resizearray (Decode.nonNull Decode.string))
                                )
                            overflowIntoDynamicObj package (SoftwarePackage.KnownFieldNames |> Seq.toList) value |> ignore
                            package
                        ) value)
                    packagesElement
            | YamlNodeKind.Mapping _ ->
                // Compact CWL form: package names are map keys.
                mappingEntries packagesElement
                |> List.map (fun entry ->
                    let name = entryName entry
                    atField name (fun () -> decodePackageFromMapEntry name entry.Value))
                |> ResizeArray
            | _ ->
                raise (System.ArgumentException("Invalid packages format. Expected array or map.")))

    /// Decode a YamlNode into an InitialWorkDirRequirement array.
    /// Supports both string/expression and Dirent listing items.
    let initialWorkDirRequirementDecoder (get: Decode.IGetters): ResizeArray<InitialWorkDirEntry> =
        get.Required.Field "listing" (fun element ->
            let listingElement = Decode.nonNull Decode.node element
            match listingElement.Kind with
            | YamlNodeKind.Sequence _ ->
                // Normal case: listing is a sequence of entries.
                Decode.resizearray initialWorkDirEntryDecoder listingElement
            | _ ->
                // CWL also permits a single listing entry without an enclosing array.
                ResizeArray [| initialWorkDirEntryDecoder listingElement |])

    /// Decode LoadListingRequirement, applying CWL's default no_listing behavior.
    let loadListingRequirementDecoder (get: Decode.IGetters): LoadListingRequirementValue =
        let loadListingValue =
            get.Optional.Field "loadListing" Decode.string
            |> Option.defaultValue "no_listing"
        let loadListing =
            match LoadListingEnum.tryParse loadListingValue with
            | Some parsed -> parsed
            | None -> raise (System.ArgumentException($"Invalid loadListing value '{loadListingValue}'. Expected one of: no_listing, shallow_listing, deep_listing."))
        LoadListingRequirementValue(loadListing)

    /// Decode resource scalar fields as int64, float, or expression string without losing numeric shape.
    /// CWL types these fields as long, float or string, so the core schema decides. A plain number keeps its
    /// numeric shape and a quoted number is a string.
    let decodeResourceScalar (element: YamlNode) : obj =
        match element with
        | CoreInt parsed -> box parsed
        | CoreFloat parsed -> box parsed
        | _ -> box (decodeStringOrExpression element)

    /// Decode an optional resource field through the resource scalar coercion rules.
    let optionalResourceField (get: Decode.IGetters) (fieldName: string) : obj option =
        get.Optional.Field fieldName Decode.node
        |> Option.map decodeResourceScalar

    /// Decode a YamlNode into a ResourceRequirementInstance
    let resourceRequirementDecoder (get: Decode.IGetters): ResourceRequirementInstance =
        // Resource fields may be integer, float, or expression string. Keep them as
        // obj so the original scalar category can be re-encoded.
        ResourceRequirementInstance(
            ?coresMin = optionalResourceField get "coresMin",
            ?coresMax = optionalResourceField get "coresMax",
            ?ramMin = optionalResourceField get "ramMin",
            ?ramMax = optionalResourceField get "ramMax",
            ?tmpdirMin = optionalResourceField get "tmpdirMin",
            ?tmpdirMax = optionalResourceField get "tmpdirMax",
            ?outdirMin = optionalResourceField get "outdirMin",
            ?outdirMax = optionalResourceField get "outdirMax"
        )

    /// Decode schema definition entries in either named object or map-entry shorthand form.
    let schemaDefRequirementTypeDecoder (value: YamlNode) : SchemaDefRequirementType =
        let entries = mappingEntries value
        let schemaDefKnownFields =
            // SchemaDefRequirement entries combine the wrapper fields with the
            // schema fields of the referenced record/array/enum definition.
            Set.unionMany
                [
                    SchemaDefRequirementType.KnownFieldNames
                    InputRecordSchema.KnownFieldNames
                    InputArraySchema.KnownFieldNames
                    InputEnumSchema.KnownFieldNames
                ]
            |> Seq.toList
        match entries |> List.tryFind (fun entry -> entryName entry = "name") with
        | Some nameEntry ->
            // Explicit object form: {name: X, type: ...}
            let schema = SchemaDefRequirementType(decodeStringOrExpression nameEntry.Value, cwlTypeDecoder' value)
            overflowIntoDynamicObj schema schemaDefKnownFields value |> ignore
            schema
        | None ->
            match entries with
            | [] ->
                raise (System.ArgumentException("SchemaDefRequirement entry cannot be empty."))
            | first :: _ ->
                // Map shorthand form: {X: {type: record, ...}}
                let firstName = entryName first
                let schema = SchemaDefRequirementType(firstName, cwlTypeDecoder' first.Value)
                overflowIntoDynamicObj schema (firstName :: schemaDefKnownFields) value |> ignore
                schema

    /// Decode a YamlNode into a SchemaDefRequirementType array
    let schemaDefRequirementDecoder (get: Decode.IGetters): ResizeArray<SchemaDefRequirementType> =
        get.Required.Field "types" (Decode.resizearray schemaDefRequirementTypeDecoder)

    /// Decode only plain boolean scalars, as the core schema reads them, and leave every other node for the
    /// requirement to take as an expression. A quoted "false" is a string.
    let tryDecodeBoolScalar (element: YamlNode) : bool option =
        match element with
        | CoreBool parsed -> Some parsed
        | _ -> None

    /// Decode WorkReuse as either a concrete bool requirement or an expression payload.
    let workReuseRequirementDecoder (get: Decode.IGetters): Requirement =
        match get.Optional.Field "enableReuse" Decode.node with
        | None ->
            WorkReuseRequirement (WorkReuseRequirementValue(true))
        | Some value ->
            match tryDecodeBoolScalar value with
            | Some boolValue ->
                WorkReuseRequirement (WorkReuseRequirementValue(boolValue))
            | None ->
                WorkReuseExpressionRequirement (decodeStringOrExpression value)

    /// Decode NetworkAccess as either a concrete bool requirement or an expression payload.
    let networkAccessRequirementDecoder (get: Decode.IGetters): Requirement =
        match get.Optional.Field "networkAccess" Decode.node with
        | None ->
            NetworkAccessRequirement (NetworkAccessRequirementValue(true))
        | Some value ->
            match tryDecodeBoolScalar value with
            | Some boolValue ->
                NetworkAccessRequirement (NetworkAccessRequirementValue(boolValue))
            | None ->
                NetworkAccessExpressionRequirement (decodeStringOrExpression value)

    /// Decode InplaceUpdateRequirement, defaulting missing inplaceUpdate to true.
    let inplaceUpdateRequirementDecoder (get: Decode.IGetters): InplaceUpdateRequirementValue =
        InplaceUpdateRequirementValue(
            get.Optional.Field "inplaceUpdate" Decode.bool
            |> Option.defaultValue true
        )

    /// Decode a YamlNode into a ToolTimeLimitRequirement value
    let toolTimeLimitRequirementDecoder (get: Decode.IGetters): ToolTimeLimitValue =
        let timeLimitElement = get.Required.Field "timelimit" (Decode.nonNull Decode.node)
        // CWL types the time limit as long or string, so a plain int is the number of seconds and
        // every other value is an expression.
        match timeLimitElement with
        | CoreInt seconds when seconds >= 0L -> ToolTimeLimitSeconds seconds
        | CoreInt _ -> raise (System.ArgumentException("ToolTimeLimit timelimit must be non-negative."))
        | _ -> ToolTimeLimitExpression (decodeStringOrExpression timeLimitElement)

    /// Decode InlineJavascriptRequirement expressionLib in scalar or array form.
    let inlineJavascriptRequirementDecoder (get: Decode.IGetters): InlineJavascriptRequirementValue =
        InlineJavascriptRequirementValue(?expressionLib = get.Optional.Field "expressionLib" decodeStringArrayOrScalar)

    /// Dispatch a known CWL requirement class to its typed decoder.
    let requirementFromTypeName cls get =
        match cls with
        | "InlineJavascriptRequirement" -> InlineJavascriptRequirement (inlineJavascriptRequirementDecoder get)
        | "SchemaDefRequirement" -> SchemaDefRequirement (schemaDefRequirementDecoder get)
        | "DockerRequirement" -> DockerRequirement (dockerRequirementDecoder get)
        | "SoftwareRequirement" -> SoftwareRequirement (softwareRequirementDecoder get)
        | "LoadListingRequirement" -> LoadListingRequirement (loadListingRequirementDecoder get)
        | "InitialWorkDirRequirement" -> InitialWorkDirRequirement (initialWorkDirRequirementDecoder get)
        | "EnvVarRequirement" -> EnvVarRequirement (envVarRequirementDecoder get)
        | "ShellCommandRequirement" -> ShellCommandRequirement
        | "ResourceRequirement" -> ResourceRequirement (resourceRequirementDecoder get)
        | "WorkReuse"
        | "WorkReuseRequirement" -> workReuseRequirementDecoder get
        | "NetworkAccess"
        | "NetworkAccessRequirement" -> networkAccessRequirementDecoder get
        | "InplaceUpdateRequirement"
        | "InplaceUpdate" -> InplaceUpdateRequirement (inplaceUpdateRequirementDecoder get)
        | "ToolTimeLimit"
        | "ToolTimeLimitRequirement" -> ToolTimeLimitRequirement (toolTimeLimitRequirementDecoder get)
        | "SubworkflowFeatureRequirement" -> SubworkflowFeatureRequirement
        | "ScatterFeatureRequirement" -> ScatterFeatureRequirement
        | "MultipleInputFeatureRequirement" -> MultipleInputFeatureRequirement
        | "StepInputExpressionRequirement" -> StepInputExpressionRequirement
        | _ -> raise (System.ArgumentException($"Invalid or unsupported requirement class: {cls}"))

    /// Attach unknown payload fields to DynamicObj-backed requirement values after class dispatch.
    let addRequirementPayloadOverflow (element: YamlNode) (requirement: Requirement) =
        let knownWithClass knownFields = "class" :: (knownFields |> Seq.toList)
        match requirement with
        // Only payload records/classes that inherit DynamicObj can receive overflow.
        // Collection requirements preserve overflow at each item decoder instead.
        | InlineJavascriptRequirement value ->
            overflowIntoDynamicObj value (knownWithClass InlineJavascriptRequirementValue.KnownFieldNames) element |> ignore
        | DockerRequirement value ->
            overflowIntoDynamicObj value (knownWithClass DockerRequirement.KnownFieldNames) element |> ignore
        | LoadListingRequirement value ->
            overflowIntoDynamicObj value (knownWithClass LoadListingRequirementValue.KnownFieldNames) element |> ignore
        | EnvVarRequirement _ ->
            ()
        | SoftwareRequirement _ ->
            ()
        | ResourceRequirement value ->
            overflowIntoDynamicObj value (knownWithClass ResourceRequirementInstance.KnownFieldNames) element |> ignore
        | WorkReuseRequirement value ->
            overflowIntoDynamicObj value (knownWithClass WorkReuseRequirementValue.KnownFieldNames) element |> ignore
        | NetworkAccessRequirement value ->
            overflowIntoDynamicObj value (knownWithClass NetworkAccessRequirementValue.KnownFieldNames) element |> ignore
        | InplaceUpdateRequirement value ->
            overflowIntoDynamicObj value (knownWithClass InplaceUpdateRequirementValue.KnownFieldNames) element |> ignore
        | _ ->
            ()
        requirement

    /// Decode requirements from either sequence syntax or class-name map syntax.
    let requirementArrayDecoder : YamlNode -> ResizeArray<Requirement> =
        fun yEle ->
            // helper: decode a single requirement object that contain 'class' field
            let decodeSingleRequirementObject (ele: YamlNode) : Requirement =
                Decode.object (fun get ->
                    // Sequence syntax carries class inside each item.
                    let cls = get.Required.Field "class" (Decode.nonNull Decode.string)
                    requirementFromTypeName cls get
                    |> addRequirementPayloadOverflow ele
                ) ele

            match yEle.Kind with
            // I: ARRAY SYNTAX
                // requirements:
                //   - class: DockerRequirement
            | YamlNodeKind.Sequence collection ->
                collection.Items
                |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeSingleRequirementObject item.Node))
                |> ResizeArray

            // II:  OBJECT/MAP SYNTAX (also covers flow/JSON-style mapping)
                // requirements:
                //   DockerRequirement: { ... }
            | YamlNodeKind.Mapping _ ->
                mappingEntries yEle
                |> List.map (fun entry ->
                    // Map syntax uses the mapping key as the requirement class
                    // and the mapping value as that requirement's payload.
                    let name = entryName entry
                    atField name (fun () ->
                        Decode.object (requirementFromTypeName name) entry.Value
                        |> addRequirementPayloadOverflow entry.Value)
                )
                |> ResizeArray
            // INVALID CWL REQUIREMENTS
            | _ -> raise (System.ArgumentException($"Invalid CWL requirements syntax: {YamlNode.show yEle}"))

    /// Try known requirement decoding for hints, falling back to UnknownHint on decode failure.
    let tryDecodeKnownRequirementFromElement (element: YamlNode) : Requirement option =
        try
            Some (Decode.object (fun get ->
                let cls = get.Required.Field "class" (Decode.nonNull Decode.string)
                requirementFromTypeName cls get
                |> addRequirementPayloadOverflow element
            ) element)
        with ex ->
            let hintClass =
                try
                    Decode.object (fun get -> get.Optional.Field "class" Decode.string) element
                with _ ->
                    None
            match hintClass with
            | Some knownClass ->
                System.Diagnostics.Debug.WriteLine($"Hint decode fallback to UnknownHint for class '{knownClass}': {ex.Message}")
            | None ->
                ()
            None

    /// Decode one hint as a known requirement when possible, otherwise preserve the raw hint.
    let decodeHintElement (element: YamlNode) : HintEntry =
        match tryDecodeKnownRequirementFromElement element with
        | Some requirement -> KnownHint requirement
        | None ->
            let hintClass =
                try
                    Decode.object (fun get -> get.Optional.Field "class" Decode.string) element
                with _ ->
                    None
            UnknownHint (HintUnknownValue(hintClass, { Node = element }))

    /// Decode hints from sequence syntax or class-name map syntax while preserving unknown hints.
    let hintArrayDecoder : YamlNode -> ResizeArray<HintEntry> =
        fun yEle ->
            match yEle.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence hints can be known requirement payloads or arbitrary
                // extension hints.
                collection.Items
                |> List.map (fun item -> decodeHintElement item.Node)
                |> ResizeArray
            | YamlNodeKind.Mapping _ ->
                mappingEntries yEle
                |> List.map (fun entry ->
                    let hintName = entryName entry
                    let classEntry = YamlNode.entry (YamlNode.plain "class") (Encode.string hintName)
                    // A hint without a payload has the same meaning as a hint with an empty one.
                    let payload = if YamlNode.isMissingValue entry.Value then YamlNode.flowMapping [] else entry.Value
                    let valueWithClass =
                        match payload.Kind with
                        | YamlNodeKind.Mapping collection ->
                            let hasClass =
                                // If map-style hint payload already has class,
                                // preserve it instead of overwriting with the key.
                                collection.Items
                                |> List.exists (fun payloadEntry -> entryName payloadEntry = "class")
                            if hasClass then payload
                            else
                                // Map-style hints use the key as synthetic class
                                // for compatibility with sequence-form decoding.
                                { payload with Kind = YamlNodeKind.Mapping { collection with Items = classEntry :: collection.Items } }
                        | _ ->
                            // Scalar map-style hints are wrapped as {class, value}
                            // so decodeHintElement can still classify them.
                            YamlNode.mapping [ classEntry; YamlNode.entry (YamlNode.plain "value") payload ]
                    decodeHintElement valueWithClass
                )
                |> ResizeArray
            | _ ->
                raise (System.ArgumentException($"Invalid CWL hints syntax: {YamlNode.show yEle}"))

    /// Access the requirements field and decode the YamlNodes into a Requirement array
    let requirementsDecoder: (YamlNode -> ResizeArray<Requirement> option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "requirements" requirementArrayDecoder
            requirements
        )

    /// Access the hints field and decode the YamlNodes into a HintEntry array
    let hintsDecoder: (YamlNode -> ResizeArray<HintEntry> option) =
        Decode.object (fun get ->
            let hints = get.Optional.Field "hints" hintArrayDecoder
            hints
        )

    /// Decode a YamlNode into an InputBinding
    let inputBindingDecoder: (YamlNode -> InputBinding option) =
        let decodeBinding (value: YamlNode) =
            Decode.object (fun get' ->
                // InputBinding has several optional command-line
                // behavior fields. Absent fields remain None.
                let binding =
                    InputBinding(
                        ?prefix = get'.Optional.Field "prefix" Decode.string,
                        ?position = get'.Optional.Field "position" Decode.int,
                        ?itemSeparator = get'.Optional.Field "itemSeparator" Decode.string,
                        ?separate = get'.Optional.Field "separate" Decode.bool,
                        ?loadContents = get'.Optional.Field "loadContents" Decode.bool,
                        ?valueFrom = get'.Optional.Field "valueFrom" Decode.string,
                        ?shellQuote = get'.Optional.Field "shellQuote" Decode.bool
                    )
                overflowIntoDynamicObj binding (InputBinding.KnownFieldNames |> Seq.toList) value |> ignore
                binding
            ) value
        // A port in scalar shorthand has no input binding.
        tryGetPresentField "inputBinding" decodeBinding

    /// Decode a named input from map shorthand or full object form, preserving optionality and extensions.
    let decodeNamedInput (name: string) (value: YamlNode) =
        // inputBinding is nested and decoded separately so both shorthand and object
        // input definitions can share the same construction flow.
        let inputBinding = inputBindingDecoder value
        let cwlType, optional = portTypeDecoder value
        let doc, label, sourceNodes = docAndLabelDecoder value
        // Known optional fields are copied into typed properties; unrecognized fields
        // are attached below as DynamicObj overflow.
        let input =
            CWLInput(
                name,
                cwlType,
                ?label = label,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" value,
                ?streamable = tryGetBoolField "streamable" value,
                ?doc = doc,
                ?format = tryGetStringField "format" value,
                ?loadContents = tryGetBoolField "loadContents" value,
                ?loadListing = tryGetLoadListingField "loadListing" value,
                ?defaultValue = tryGetYamlField "default" value
            )
        input.SourceNodes <- sourceNodes
        if optional then input.Optional <- Some true
        input.InputBinding <- inputBinding
        overflowIntoDynamicObj input (CWLInput.KnownFieldNames |> Seq.toList) value |> ignore
        input

    /// Decode one sequence-form input, warning only for malformed unnamed entries.
    let decodeInputSequenceItem (warnings: WarningSink) (path: string) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        | Some id -> Some (atField id (fun () -> decodeNamedInput id item))
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL input entry." (Some item)
            None

    /// Decode a YamlNode into an Input array
    let inputArrayDecoderWithWarnings (warnings: WarningSink) (path: string) : (YamlNode -> ResizeArray<CWLInput>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form inputs must carry an id on each object item.
                collection.Items
                |> List.mapi (fun index item -> decodeInputSequenceItem warnings path index item.Node)
                |> List.choose id
                |> ResizeArray
            | _ ->
                // Map-form inputs use the map key as the input name.
                [|
                    for entry in mappingEntries value do
                        let name = entryName entry
                        atField name (fun () -> decodeNamedInput name entry.Value)
                |]
                |> ResizeArray

    let inputArrayDecoder: (YamlNode -> ResizeArray<CWLInput>) =
        inputArrayDecoderWithWarnings None "inputs"

    /// Access the inputs field and decode the YamlNodes into an Input array
    let inputsDecoderWithWarnings (warnings: WarningSink) : (YamlNode -> ResizeArray<CWLInput> option) =
        Decode.object (fun get ->
            get.Optional.Field "inputs" (inputArrayDecoderWithWarnings warnings "inputs")
        )

    let inputsDecoder: (YamlNode -> ResizeArray<CWLInput> option) =
        inputsDecoderWithWarnings None

    /// Decode the inputs of a process. CWL requires them for every class, so a missing inputs field is a decode error
    /// at the place of the process.
    let private requiredInputsDecoder (warnings: WarningSink) (what: string) (yamlCWL: YamlNode) : ResizeArray<CWLInput> =
        match inputsDecoderWithWarnings warnings yamlCWL with
        | Some inputs -> inputs
        | None ->
            raise (
                YamlDecodeException
                    { Message = "Expected an inputs field, which CWL requires for " + what + "."
                      Path = []
                      Span = yamlCWL.Span }
            )

    /// Decode baseCommand from CWL's scalar or array forms.
    let baseCommandDecoder: (YamlNode -> ResizeArray<string> option) =
        Decode.object (fun get ->
            match get.Optional.Field "baseCommand" Decode.node with
            | Some baseCommandField ->
                match baseCommandField.Kind with
                | YamlNodeKind.Scalar _ ->
                    // Single string value
                    Some (ResizeArray([Decode.string baseCommandField]))
                | YamlNodeKind.Sequence _ ->
                    // Array of strings
                    Some (atField "baseCommand" (fun () -> Decode.resizearray (Decode.nonNull Decode.string) baseCommandField))
                | _ ->
                    // Malformed baseCommand is ignored for backward compatibility with
                    // the previous permissive decoder.
                    None
            | None -> None
        )

    let versionDecoder: (YamlNode -> string) =
        Decode.object (fun get -> get.Required.Field "cwlVersion" (Decode.nonNull Decode.string))

    let classDecoder: (YamlNode -> string) =
        Decode.object (fun get ->
            get.Required.Field "class" (Decode.nonNull Decode.string)
        )
    let stringOptionFieldDecoder field : (YamlNode -> string option) =
        tryGetPresentField field Decode.string

    let boolOptionFieldDecoder field : (YamlNode -> bool option) =
        tryGetPresentField field Decode.bool

    let rawYamlOptionFieldDecoder field : (YamlNode -> RawYaml option) =
        tryGetPresentField field (fun node -> { Node = node })

    let stringFieldDecoder field : (YamlNode -> string) =
        Decode.object(fun get ->
            let fieldValue = get.Required.Field field (Decode.nonNull Decode.string)
            fieldValue
        )

    /// Decode a YamlNode that may be a single string or an array of strings.
    let stringOrStringArrayDecoder (value: YamlNode) : ResizeArray<string> option =
        match value.Kind with
        | YamlNodeKind.Scalar _ ->
            // CWL shorthand: source: input_id
            Some (ResizeArray [ Decode.nonNull Decode.string value ])
        | YamlNodeKind.Sequence _ ->
            // Full form: source: [a, b]
            Some (Decode.resizearray (Decode.nonNull Decode.string) value)
        | _ -> None

    /// Decode a YamlNode into a ResizeArray<string> option for the source field
    /// Handles both single string values and arrays of strings
    let sourceArrayFieldDecoder field : (YamlNode -> ResizeArray<string> option) =
        fun value ->
            tryGetPresentField field Decode.node value
            |> Option.bind stringOrStringArrayDecoder

    /// Decode and validate linkMerge enum fields.
    let linkMergeFieldDecoder field : (YamlNode -> LinkMergeMethod option) =
        fun value ->
            match tryGetPresentField field Decode.string value with
            | Some linkMergeString ->
                match LinkMergeMethod.tryParse linkMergeString with
                | Some linkMerge -> Some linkMerge
                | None -> raise (System.ArgumentException($"Invalid linkMerge value: {linkMergeString}"))
            | None -> None

    /// Decode and validate pickValue enum fields.
    let pickValueFieldDecoder field : (YamlNode -> PickValueMethod option) =
        fun value ->
            match tryGetPresentField field Decode.string value with
            | Some pickValueString ->
                match PickValueMethod.tryParse pickValueString with
                | Some pickValue -> Some pickValue
                | None -> raise (System.ArgumentException($"Invalid pickValue value: {pickValueString}"))
            | None -> None

    let scatterFieldDecoder field : (YamlNode -> ResizeArray<string> option) =
        fun value ->
            tryGetPresentField field Decode.node value
            |> Option.bind stringOrStringArrayDecoder

    /// Decode and validate scatterMethod enum fields.
    let scatterMethodFieldDecoder field : (YamlNode -> ScatterMethod option) =
        fun value ->
            match tryGetPresentField field Decode.string value with
            | Some scatterMethodString ->
                match ScatterMethod.tryParse scatterMethodString with
                | Some scatterMethod -> Some scatterMethod
                | None -> raise (System.ArgumentException($"Invalid scatterMethod value: {scatterMethodString}"))
            | None -> None

    /// Decode optional expression-like fields, including schema-salad directive wrappers.
    let expressionStringOptionFieldDecoder field : (YamlNode -> string option) =
        fun value ->
            tryGetPresentField field Decode.node value
            |> Option.map decodeStringOrExpression

    /// Decode a step input from map or scalar shorthand and preserve unknown step-input fields.
    let decodeStepInputFromValue (id: string) (value: YamlNode) (allowScalarSource: bool) : StepInput =
        let scalarSource =
            if allowScalarSource then
                // Map-form step inputs may be `in: { target: source }`, where the
                // value itself is the source field.
                stringOrStringArrayDecoder value
            else
                // Sequence-form items use explicit fields and should not treat the
                // whole item as a source scalar.
                None
        let fieldSource = sourceArrayFieldDecoder "source" value
        let source =
            match scalarSource, fieldSource with
            // Prefer scalar shorthand when present; otherwise use the source field.
            | Some s, _ -> Some s
            | _, Some s -> Some s
            | _ -> None
        let doc, label, sourceNodes = docAndLabelDecoder value
        // Decode all optional step-input behavior fields before preserving overflow.
        let stepInput =
            StepInput.create(
                id,
                ?source = source,
                ?defaultValue = rawYamlOptionFieldDecoder "default" value,
                ?valueFrom = stringOptionFieldDecoder "valueFrom" value,
                ?linkMerge = linkMergeFieldDecoder "linkMerge" value,
                ?pickValue = pickValueFieldDecoder "pickValue" value,
                ?doc = doc,
                ?loadContents = boolOptionFieldDecoder "loadContents" value,
                ?loadListing = stringOptionFieldDecoder "loadListing" value,
                ?label = label
            )
        stepInput.SourceNodes <- sourceNodes
        overflowIntoDynamicObj
            stepInput
            (StepInput.KnownFieldNames |> Seq.toList)
            value
        |> ignore
        stepInput

    /// Decode map-form step inputs where the map key supplies the step input id.
    let decodeStepInputsFromMap (value: YamlNode) : ResizeArray<StepInput> =
        [|
            for entry in mappingEntries value do
                // A step input in map form needs a source or a mapping. A key without a value has neither.
                let name = entryName entry
                atField name (fun () -> decodeStepInputFromValue name (Decode.nonNull Decode.node entry.Value) true)
        |]
        |> ResizeArray

    /// Decode sequence-form step inputs where the id must be present in the item.
    let decodeStepInputFromArrayItem (item: YamlNode) : StepInput =
        let id = stringFieldDecoder "id" item
        decodeStepInputFromValue id item false

    /// Decode sequence-form step inputs, warning and skipping only malformed unnamed entries.
    let decodeStepInputsFromArrayWithWarnings (warnings: WarningSink) (path: string) (items: YamlNode list) : ResizeArray<StepInput> =
        items
        |> List.mapi (fun index item ->
            match tryGetStringField "id" item with
            // Items with ids are considered intended CWL step inputs and decode strictly.
            | Some id -> Some (atField id (fun () -> decodeStepInputFromArrayItem item))
            | None ->
                // Unnamed malformed entries are public-workflow tolerance cases.
                addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL step input entry." (Some item)
                None)
        |> List.choose id
        |> ResizeArray

    let decodeStepInputsFromArray (items: YamlNode list) : ResizeArray<StepInput> =
        decodeStepInputsFromArrayWithWarnings None "in" items

    /// Select the step input decoder based on CWL map-form versus sequence-form syntax.
    let inputStepDecoderWithWarnings (warnings: WarningSink) (path: string) : (YamlNode -> ResizeArray<StepInput>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                decodeStepInputsFromArrayWithWarnings warnings path (collection.Items |> List.map (fun item -> item.Node))
            | _ ->
                decodeStepInputsFromMap value

    let inputStepDecoder: (YamlNode -> ResizeArray<StepInput>) =
        inputStepDecoderWithWarnings None "in"

    /// Decode step outputs from scalar shorthand or record form with extension fields.
    let decodeStepOutputItem (value: YamlNode) : StepOutput =
        match value.Kind with
        | YamlNodeKind.Scalar _ ->
            StepOutputString (Decode.nonNull Decode.string value)
        | _ ->
            let id = stringFieldDecoder "id" value
            let output = StepOutputParameter.create id
            overflowIntoDynamicObj output (StepOutputParameter.KnownFieldNames |> Seq.toList) value |> ignore
            StepOutputRecord output

    /// Decode workflow step out values, treating CWL empty forms as an empty output list.
    let outputStepsDecoder: (YamlNode -> ResizeArray<StepOutput>) =
        Decode.object (fun get ->
            // CWL requires out. `out: []` and `out: {}` are empty lists, and a key without a value is an error.
            let outField = get.Required.Field "out" (Decode.nonNull Decode.node)
            atField "out" (fun () ->
                match outField.Kind with
                | YamlNodeKind.Mapping collection when collection.Items.IsEmpty ->
                    // An empty flow mapping, out: {}, has no outputs either.
                    ResizeArray()
                | YamlNodeKind.Sequence collection ->
                    // Normal multi-output sequence. An empty sequence has no outputs.
                    collection.Items
                    |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeStepOutputItem item.Node))
                    |> ResizeArray
                | _ ->
                    // Single scalar or single record output.
                    ResizeArray [ decodeStepOutputItem outField ])
        )

    let idDecoder: (YamlNode -> string option) =
        Decode.object (fun get -> get.Optional.Field "id" Decode.string)

    let intentDecoder: (YamlNode -> ResizeArray<string> option) =
        Decode.object (fun get ->
            get.Optional.Field "intent" Decode.node
            |> Option.bind stringOrStringArrayDecoder
        )

    /// Test object mappings for an explicit field without invoking a typed decoder.
    let hasField (fieldName: string) (node: YamlNode) : bool =
        match node.Kind with
        | YamlNodeKind.Mapping collection ->
            collection.Items
            |> List.exists (fun entry -> entryName entry = fieldName)
        | _ -> false

    /// Inject the parent workflow cwlVersion into inline run objects that omit it.
    let withDefaultCwlVersion (defaultCwlVersion: string) (node: YamlNode) : YamlNode =
        match node.Kind with
        | YamlNodeKind.Mapping collection when not (hasField "cwlVersion" node) ->
            // Inline run objects commonly omit cwlVersion and inherit the workflow's
            // version for decoding purposes.
            let versionEntry = YamlNode.entry (YamlNode.plain "cwlVersion") (Encode.string defaultCwlVersion)
            { node with Kind = YamlNodeKind.Mapping { collection with Items = versionEntry :: collection.Items } }
        | _ ->
            // Inline tools can declare their own version, and non-object run values are path strings.
            // Neither needs a version injection.
            node

    /// Decode a workflow step run as either a path string or an inline CWL processing unit.
    let rec workflowStepRunDecoder (warnings: WarningSink) (defaultCwlVersion: string) (runValue: YamlNode) : WorkflowStepRun =
        match runValue.Kind with
        | YamlNodeKind.Scalar _ ->
            // Most workflow steps reference an external CWL file by path.
            RunString (Decode.nonNull Decode.string runValue)
        | YamlNodeKind.Mapping _ ->
            let normalizedRun = withDefaultCwlVersion defaultCwlVersion runValue
            // Inline run objects are full CWL processing units and can themselves
            // contain tolerant ports/steps, so the same warning sink is threaded in.
            match decodeCWLProcessingUnitElementWithWarnings warnings normalizedRun with
            | CommandLineTool tool -> WorkflowStepRunOps.fromTool tool
            | Workflow workflow -> WorkflowStepRunOps.fromWorkflow workflow
            | ExpressionTool expressionTool -> WorkflowStepRunOps.fromExpressionTool expressionTool
            | Operation operation -> WorkflowStepRunOps.fromOperation operation
        | _ ->
            raise (System.ArgumentException($"Unsupported run value for workflow step: {YamlNode.show runValue}"))

    /// Decode a named workflow step and thread warnings into nested step inputs and inline runs.
    and decodeWorkflowStepFromValueWithId (warnings: WarningSink) (defaultCwlVersion: string) (path: string) (stepId: string) (value: YamlNode) : WorkflowStep =
        // Required fields are decoded strictly because the step has an explicit id.
        let runValue = Decode.object (fun get' -> get'.Required.Field "run" (Decode.nonNull Decode.node)) value
        let run = atField "run" (fun () -> workflowStepRunDecoder warnings defaultCwlVersion runValue)
        let inputs =
            Decode.object (fun get' ->
                get'.Required.Field "in" (inputStepDecoderWithWarnings warnings $"{path}.in")
            ) value
        // Optional fields are decoded independently so missing fields do not block
        // preservation of other step metadata.
        let outputs = outputStepsDecoder value
        let requirements = requirementsDecoder value
        let hints = hintsDecoder value
        let doc, label, sourceNodes = docAndLabelDecoder value
        let scatter = scatterFieldDecoder "scatter" value
        let scatterMethod = scatterMethodFieldDecoder "scatterMethod" value
        let when_ = expressionStringOptionFieldDecoder "when" value
        let wfStep =
            WorkflowStep(
                stepId,
                inputs,
                outputs,
                run,
                ?label = label,
                ?doc = doc,
                ?scatter = scatter,
                ?scatterMethod = scatterMethod,
                ?when_ = when_
            )
        wfStep.SourceNodes <- sourceNodes
        if requirements.IsSome then
            wfStep.Requirements <- requirements
        if hints.IsSome then
            wfStep.Hints <- hints
        // Preserve step-level extension fields such as vendor-specific scheduling hints.
        overflowIntoDynamicObj
            wfStep
            (WorkflowStep.KnownFieldNames |> Seq.toList)
            value
        |> ignore
        wfStep

    and decodeWorkflowStepFromArrayItem (defaultCwlVersion: string) (item: YamlNode) : WorkflowStep =
        let stepId = stringFieldDecoder "id" item
        decodeWorkflowStepFromValueWithId None defaultCwlVersion "steps[]" stepId item

    /// Decode one sequence-form workflow step, warning only for malformed unnamed entries.
    and decodeWorkflowStepFromArrayItemWithWarnings (warnings: WarningSink) (defaultCwlVersion: string) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        // An id marks the item as an intended step, so malformed required fields
        // should still fail instead of becoming warnings.
        | Some stepId -> Some (atField stepId (fun () -> decodeWorkflowStepFromValueWithId warnings defaultCwlVersion $"steps[{index}]" stepId item))
        | None ->
            // Unnamed malformed sequence entries are skipped with location-aware warnings.
            addWarning warnings $"steps[{index}]" "Skipped malformed unnamed CWL workflow step entry." (Some item)
            None

    /// Decode workflow steps from sequence-form or map-form syntax with version-aware inline runs.
    and stepArrayDecoderWithVersion (warnings: WarningSink) (defaultCwlVersion: string) : (YamlNode -> ResizeArray<WorkflowStep>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form steps carry id in each item.
                collection.Items
                |> List.mapi (fun index item -> decodeWorkflowStepFromArrayItemWithWarnings warnings defaultCwlVersion index item.Node)
                |> List.choose id
                |> ResizeArray
            | _ ->
                // Map-form steps use the map key as the step id.
                [|
                    for entry in mappingEntries value do
                        let key = entryName entry
                        atField key (fun () -> decodeWorkflowStepFromValueWithId warnings defaultCwlVersion $"steps.{key}" key entry.Value)
                |]
                |> ResizeArray

    and stepsDecoderWithVersion (warnings: WarningSink) (defaultCwlVersion: string) : (YamlNode -> ResizeArray<WorkflowStep>) =
        Decode.object (fun get ->
            get.Required.Field "steps" (stepArrayDecoderWithVersion warnings defaultCwlVersion)
        )

    /// Decode a CommandLineTool and preserve unknown top-level metadata fields.
    and commandLineToolDecoder (warnings: WarningSink) (yamlCWL : YamlNode) =
        // Decode standard sections first. Inputs and outputs are required.
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs = requiredInputsDecoder warnings "a command line tool" yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let baseCommand = baseCommandDecoder yamlCWL
        let doc, label, sourceNodes = docAndLabelDecoder yamlCWL
        // Command-line execution fields are kept as typed properties because they
        // are common CWL fields and should not fall through to metadata overflow.
        let description =
            CWLToolDescription(
                outputs,
                ?cwlVersion = Some cwlVersion,
                ?id = idDecoder yamlCWL,
                ?arguments = tryGetYamlField "arguments" yamlCWL,
                ?stdin = tryGetStringField "stdin" yamlCWL,
                ?stderr = tryGetStringField "stderr" yamlCWL,
                ?stdout = tryGetStringField "stdout" yamlCWL,
                ?successCodes = tryGetIntArrayField "successCodes" yamlCWL,
                ?temporaryFailCodes = tryGetIntArrayField "temporaryFailCodes" yamlCWL,
                ?permanentFailCodes = tryGetIntArrayField "permanentFailCodes" yamlCWL
            )
        let metadata =
            // Anything not in KnownFieldNames is top-level metadata/extension data.
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (get.Overflow.FieldList (CWLToolDescription.KnownFieldNames |> Seq.toList))
            ) |> ignore
            md
        // Apply optional sections only when present to preserve constructor defaults.
        description.Inputs <- Some inputs
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if intent.IsSome then
            description.Intent <- intent
        if baseCommand.IsSome then
            description.BaseCommand <- baseCommand
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        description.SourceNodes <- sourceNodes
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Decode an ExpressionTool and preserve unknown top-level metadata fields.
    and expressionToolDecoder (warnings: WarningSink) (yamlCWL: YamlNode) =
        // ExpressionTool shares port/requirement metadata decoding with other
        // processing units, then decodes its required expression body.
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs = requiredInputsDecoder warnings "an expression tool" yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let doc, label, sourceNodes = docAndLabelDecoder yamlCWL
        let expression =
            Decode.object (fun get -> get.Required.Field "expression" decodeStringOrExpression) yamlCWL
        let description =
            CWLExpressionToolDescription(
                outputs,
                expression,
                ?cwlVersion = Some cwlVersion,
                ?id = idDecoder yamlCWL
            )
        let metadata =
            // Preserve top-level fields that are not part of the typed ExpressionTool model.
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (get.Overflow.FieldList (CWLExpressionToolDescription.KnownFieldNames |> Seq.toList))
            ) |> ignore
            md
        description.Inputs <- Some inputs
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if intent.IsSome then
            description.Intent <- intent
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        description.SourceNodes <- sourceNodes
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Decode an Operation and enforce its required inputs collection.
    and operationDecoder (warnings: WarningSink) (yamlCWL: YamlNode) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs = requiredInputsDecoder warnings "an operation" yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let doc, label, sourceNodes = docAndLabelDecoder yamlCWL
        let description =
            CWLOperationDescription(
                inputs,
                outputs,
                ?cwlVersion = Some cwlVersion,
                ?id = idDecoder yamlCWL
            )
        let metadata =
            // Preserve top-level operation extensions separately from typed fields.
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (get.Overflow.FieldList (CWLOperationDescription.KnownFieldNames |> Seq.toList))
            ) |> ignore
            md
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if intent.IsSome then
            description.Intent <- intent
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        description.SourceNodes <- sourceNodes
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Decode a Workflow, including version-aware steps and preserved metadata fields.
    and workflowDecoder (warnings: WarningSink) (yamlCWL: YamlNode) =
        // Workflow inputs and steps are required by this model; outputs and steps
        // can still use tolerant collection decoders internally.
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs = requiredInputsDecoder warnings "a workflow" yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let steps = stepsDecoderWithVersion warnings cwlVersion yamlCWL
        let doc, label, sourceNodes = docAndLabelDecoder yamlCWL
        let description =
            CWLWorkflowDescription(
                steps,
                inputs,
                outputs,
                ?cwlVersion = Some cwlVersion,
                ?id = idDecoder yamlCWL
            )
        let metadata =
            // Preserve top-level workflow extensions after removing known workflow fields.
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (get.Overflow.FieldList (CWLWorkflowDescription.KnownFieldNames |> Seq.toList))
            ) |> ignore
            md
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if intent.IsSome then
            description.Intent <- intent
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        description.SourceNodes <- sourceNodes
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Dispatch a parsed CWL document to the processing-unit decoder declared by its class field.
    and decodeCWLProcessingUnitElementWithWarnings (warnings: WarningSink) (yamlCWL: YamlNode) =
        let cls = classDecoder yamlCWL
        match cls with
        // The class field is the only discriminator at this layer; individual
        // decoders handle version, ports, requirements, and metadata.
        | "CommandLineTool" -> CommandLineTool (commandLineToolDecoder warnings yamlCWL)
        | "Workflow" -> Workflow (workflowDecoder warnings yamlCWL)
        | "ExpressionTool" -> ExpressionTool (expressionToolDecoder warnings yamlCWL)
        | "Operation" -> Operation (operationDecoder warnings yamlCWL)
        | _ -> raise (System.ArgumentException($"Invalid or unsupported CWL class: {cls}"))

    /// Decode an already-parsed CWL element without warning collection.
    let decodeCWLProcessingUnitElement (yamlCWL: YamlNode) =
        decodeCWLProcessingUnitElementWithWarnings None yamlCWL

    let stepArrayDecoder = stepArrayDecoderWithVersion None "v1.2"

    let stepsDecoder = stepsDecoderWithVersion None "v1.2"

    /// Decode a CWL CommandLineTool YAML string and return recoverable warnings.
    let decodeCommandLineToolWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = commandLineToolDecoder (Some warnings) yamlCWL; Warnings = warnings }

    /// Decode a CWL CommandLineTool and discard recoverable warnings.
    let decodeCommandLineTool (cwl: string) =
        (decodeCommandLineToolWithWarnings cwl).Value

    /// Decode a CWL Workflow YAML string and return recoverable warnings.
    let decodeWorkflowWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = workflowDecoder (Some warnings) yamlCWL; Warnings = warnings }

    /// Decode a CWL Workflow and discard recoverable warnings.
    let decodeWorkflow (cwl: string) =
        (decodeWorkflowWithWarnings cwl).Value

    /// Decode a CWL ExpressionTool YAML string and return recoverable warnings.
    let decodeExpressionToolWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = expressionToolDecoder (Some warnings) yamlCWL; Warnings = warnings }

    /// Decode a CWL ExpressionTool and discard recoverable warnings.
    let decodeExpressionTool (cwl: string) =
        (decodeExpressionToolWithWarnings cwl).Value

    /// Decode a CWL Operation YAML string and return recoverable warnings.
    let decodeOperationWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = operationDecoder (Some warnings) yamlCWL; Warnings = warnings }

    /// Decode a CWL Operation and discard recoverable warnings.
    let decodeOperation (cwl: string) =
        (decodeOperationWithWarnings cwl).Value

    /// Decode any CWL processing unit from a YAML string and return recoverable warnings.
    let decodeCWLProcessingUnitWithWarnings (cwl:string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = decodeCWLProcessingUnitElementWithWarnings (Some warnings) yamlCWL; Warnings = warnings }

    /// Decode any CWL processing unit and discard recoverable warnings.
    let decodeCWLProcessingUnit (cwl:string) =
        (decodeCWLProcessingUnitWithWarnings cwl).Value

module DecodeParameters =

    let private tryField (fieldName: string) (node: YamlNode) =
        match node.Kind with
        | YamlNodeKind.Mapping _ ->
            Decode.mappingEntries node
            |> List.tryPick (fun entry ->
                if Decode.entryName entry = fieldName then Some entry.Value else None
            )
        | _ -> None

    let private tryScalarString (node: YamlNode) =
        match node.Kind with
        | YamlNodeKind.Scalar _ -> Some (YAMLicious.Decode.string node)
        | _ -> None

    let private decodeFileInstance (node: YamlNode) =
        Decode.decodeFileInstanceFields node

    let private decodeDirectoryInstance (node: YamlNode) =
        Decode.decodeDirectoryInstanceFields node

    let rec decodeParameterValue (value: YamlNode) : CWLParameterValue =
        match value.Kind with
        | YamlNodeKind.Scalar _ ->
            // The core schema decides the type, so a quoted "true" is a string and a plain true is a boolean.
            // Encode.string relies on this to write the string "true" as a quoted scalar. A key or a sequence
            // item without a value, as in `k:` or `-`, holds null.
            match value with
            | Decode.CoreNull -> CWLParameterValue.Null
            | Decode.CoreBool parsed -> CWLParameterValue.Boolean parsed
            | Decode.CoreInt parsed -> CWLParameterValue.Int parsed
            | Decode.CoreFloat parsed -> CWLParameterValue.Float parsed
            | Decode.CoreText -> CWLParameterValue.String (YAMLicious.Decode.string value)
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.map (fun item -> decodeParameterValue item.Node)
            |> ResizeArray
            |> CWLParameterValue.Array
        | YamlNodeKind.Mapping _ ->
            match tryField "class" value |> Option.bind tryScalarString with
            | Some "File" -> decodeFileInstance value |> CWLParameterValue.File
            | Some "Directory" -> decodeDirectoryInstance value |> CWLParameterValue.Directory
            | _ ->
                Decode.mappingEntries value
                |> List.map (fun entry -> CWLParameterRecordField(Decode.entryName entry, decodeParameterValue entry.Value))
                |> ResizeArray
                |> CWLParameterValue.Record
        | YamlNodeKind.Alias name ->
            Decode.failAlias name value

    let cwlParameterReferenceDecoder (key: string) (yEle: YamlNode): CWLParameterReference =
        let makeReference (value: CWLParameterValue) (explicitType: CWLType option) =
            let type_ =
                explicitType
                |> Option.orElseWith (fun () -> CWLParameterValue.tryInferType value)

            CWLParameterReference(key = key, value = value, ?type_ = type_)

        let withOverflow (reference: CWLParameterReference) =
            Decode.overflowIntoDynamicObj
                reference
                (CWLParameterReference.KnownFieldNames |> Seq.toList)
                yEle
            |> ignore
            reference

        let decodeType (typeElement: YamlNode) =
            match typeElement.Kind with
            | YamlNodeKind.Scalar _ -> Decode.cwlTypeStringDecoder typeElement |> fst
            | _ -> Decode.cwlTypeDecoder' typeElement

        match tryField "type" yEle, tryField "value" yEle with
        | Some typeElement, Some valueElement ->
            makeReference (decodeParameterValue valueElement) (Some (decodeType typeElement))
            |> withOverflow
        | None, Some valueElement ->
            makeReference (decodeParameterValue valueElement) None
            |> withOverflow
        | Some typeElement, None ->
            makeReference CWLParameterValue.Null (Some (decodeType typeElement))
            |> withOverflow
        | _ ->
            makeReference (decodeParameterValue yEle) None
            |> withOverflow

    /// Decode every top-level parameter entry into CWLParameterReference values.
    let cwlparameterReferenceArrayDecoder: YamlNode -> ResizeArray<CWLParameterReference> =
        let decodeReferences =
            Decode.object (fun get ->
                [|
                    for entry in get.Entries do
                        cwlParameterReferenceDecoder (Decode.entryName entry) entry.Value
                |]
                |> ResizeArray
            )
        fun root ->
            // A parameter file without content, such as an empty file or a file with comments only, holds no references.
            match Decode.option decodeReferences root with
            | Some references -> references
            | None -> ResizeArray()

    /// Decode a YAML parameter file string into parameter references.
    let decodeYAMLParameterFile (yaml: string) =
        let yEle = Decode.readYaml yaml
        cwlparameterReferenceArrayDecoder yEle
