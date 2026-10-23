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

    /// What a node is, as it stands in the message of an error: the text of a scalar or the kind of any other node.
    let private describeNode (node: YamlNode) : string =
        match node.Kind with
        | YamlNodeKind.Scalar scalar -> "the text \"" + scalar.Value + "\""
        | YamlNodeKind.Sequence _ -> "a list"
        | YamlNodeKind.Mapping _ -> "a mapping"
        | YamlNodeKind.Alias name -> "the alias *" + name

    /// Fail with a decode error at the place of a node. The callers that read the node inside a field add the path of the
    /// field with atField.
    let private failAt (node: YamlNode) (message: string) : 'T =
        raise (YamlDecodeException { Message = message; Path = []; Span = node.Span })

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

    /// Decode a loadListing value from its YAML string form.
    let loadListingDecoder (node: YamlNode) : LoadListingEnum =
        let text = Decode.string node
        match LoadListingEnum.tryParse text with
        | Some parsed -> parsed
        | None -> failAt node ("Expected one of no_listing, shallow_listing and deep_listing, found " + describeNode node + ".")

    /// Decode and validate CWL loadListing enum fields from their YAML string form.
    let tryGetLoadListingField (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName loadListingDecoder value

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

    /// Decode a string field that may hold a Schema Salad directive. A scalar is the literal text. A mapping with the
    /// single key $include or $import names the file whose text or data takes the place of the mapping. The directive
    /// stays a case of the result, so the encoder writes the mapping again.
    let decodeSchemaSaladString (node: YamlNode) : SchemaSaladString =
        let unexpected () : SchemaSaladString =
            failAt node ("Expected a string or a mapping with the single key $include or $import, found " + describeNode node + ".")
        match node.Kind with
        | YamlNodeKind.Scalar _ ->
            // Common path: scalar values become literal schema-salad strings. YAMLicious folds the
            // line breaks of a multi-line plain scalar into spaces, as YAML 1.2.2 requires.
            SchemaSaladString.Literal (Decode.nonNull Decode.string node)
        | YamlNodeKind.Mapping collection ->
            match collection.Items with
            | [ entry ] when (YamlNode.tryScalar entry.Value).IsSome && entryName entry = "$include" ->
                SchemaSaladString.Include (Decode.nonNull Decode.string entry.Value)
            | [ entry ] when (YamlNode.tryScalar entry.Value).IsSome && entryName entry = "$import" ->
                SchemaSaladString.Import (Decode.nonNull Decode.string entry.Value)
            | _ -> unexpected ()
        | _ -> unexpected ()

    /// Decode a list of schema-salad strings.
    let decodeSchemaSaladStringArray (value: YamlNode) : ResizeArray<SchemaSaladString> =
        match value.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeSchemaSaladString item.Node))
            |> ResizeArray
        | _ ->
            failAt value ("Expected a list of strings, found " + describeNode value + ".")

    /// Decode the glob of an output binding: one pattern or a list of patterns, kept as it was written.
    let outputGlobDecoder (node: YamlNode) : OutputGlob =
        match node.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> Decode.nonNull Decode.string item.Node))
            |> ResizeArray
            |> OutputGlob.Multiple
        | _ -> OutputGlob.Single (Decode.string node)

    /// Decode a YamlNode into a glob search pattern for output binding
    let outputBindingGlobDecoder: (YamlNode -> OutputBinding) =
        fun value ->
            Decode.object (fun get ->
                // Read all known outputBinding fields first; any implementation
                // extension fields are attached below through overflowIntoDynamicObj.
                let glob = get.Optional.Field "glob" outputGlobDecoder
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

    /// Decode a list of strings. A single string stands for a list of one.
    let decodeStringArrayOrScalar (value: YamlNode) : ResizeArray<string> =
        match value.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> Decode.nonNull Decode.string item.Node))
            |> ResizeArray
        | _ ->
            ResizeArray [| Decode.nonNull Decode.string value |]

    /// Decode CWL outputSource in either scalar or list form.
    let outputSourceDecoder: (YamlNode -> ResizeArray<string> option) =
        tryGetPresentField "outputSource" decodeStringArrayOrScalar

    /// Decode a YamlNode into a Dirent
    let direntDecoder: (YamlNode -> DirentInstance) =
        fun value ->
            Decode.object (fun get ->
                let dirent =
                    DirentInstance(
                        get.Required.Field "entry" decodeSchemaSaladString,
                        ?entryname = get.Optional.Field "entryname" decodeSchemaSaladString,
                        ?writable = get.Optional.Field "writable" Decode.bool
                    )
                overflowIntoDynamicObj dirent (DirentInstance.KnownFieldNames |> Seq.toList) value |> ignore
                dirent
            ) value

    /// Decode a File or a Directory object, which an array inside a listing holds.
    let private fileOrDirectoryDecoder (value: YamlNode) : FileOrDirectory =
        match tryGetStringField "class" value with
        | Some "File" -> FileItem (decodeFileInstanceFields value)
        | Some "Directory" -> DirectoryItem (decodeDirectoryInstanceFields value)
        | _ -> failAt value ("Expected a File or a Directory with a class field in an array of the listing, found " + describeNode value + ".")

    /// Decode a listing entry of InitialWorkDirRequirement. CWL admits null, a Dirent, an expression, a File, a
    /// Directory and an array of Files and Directories.
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
                    DirentEntry (direntDecoder value)
                else
                    // File and Directory entries are discriminated by their class field. The only other mapping is
                    // a directive that includes or imports the text of a file.
                    match tryGetStringField "class" value, collection.Items with
                    | Some "File", _ ->
                        // File and Directory object decoding is shared with CWL type decoding so
                        // listing entries preserve the same extension metadata.
                        FileEntry (decodeFileInstanceFields value)
                    | Some "Directory", _ ->
                        DirectoryEntry (decodeDirectoryInstanceFields value)
                    | _, [ entry ] when entryName entry = "$include" || entryName entry = "$import" ->
                        StringEntry (decodeSchemaSaladString value)
                    | _ ->
                        failAt value "Expected a Dirent with an entry field, a File or a Directory with a class field, or an $include or $import directive in the listing, found a mapping."
            | YamlNodeKind.Scalar _ ->
                match value with
                | CoreNull -> NullEntry
                | _ ->
                    // Scalar listing entries are allowed by CWL and represent generated
                    // file content or expressions.
                    StringEntry (decodeSchemaSaladString value)
            | YamlNodeKind.Sequence collection ->
                collection.Items
                |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> fileOrDirectoryDecoder item.Node))
                |> ResizeArray
                |> FilesEntry
            | YamlNodeKind.Alias name -> failAlias name value

    /// Where a type may be one of the stream types stdout, stderr and stdin.
    type StreamTypes =
        /// Nowhere.
        | NoStreams
        /// As the whole type of an output of a command line tool, which admits stdout and stderr.
        | OutputStreams
        /// As the whole type of an input of a command line tool, which admits stdin.
        | InputStreams
        /// As the whole type, for any kind of port. A type text has no port around it, so it may be any of the three.
        | AnyStreams

    /// Which names a type may use for a type that a SchemaDefRequirement defines.
    type ReferenceNames =
        /// The names that the scope lists, and the names in the files that it lists as imported.
        | DefinedNames
        /// Every well-formed name that is no builtin type. A name is well-formed when it is not empty and holds no
        /// whitespace and none of the characters [ ] ?. A type text has no SchemaDefRequirement around it that could
        /// say which names exist.
        | AnyNames

    /// The types that one SchemaDefRequirement defines: their ids and the paths of the files that it imports.
    type DefinedTypes =
        { /// The ids of the types, without the leading #.
          Ids: Set<string>
          /// The paths of the imported files, with the . and .. segments resolved.
          Imports: Set<string> }

    /// The SchemaDefRequirements that one chain of enclosing processes and steps leaves at a place. A process that a step
    /// runs by #id has the chain of that step.
    type SchemaDefChain =
        { /// The types of the innermost SchemaDefRequirement in the requirements of the chain.
          Required: DefinedTypes option
          /// The types of the innermost SchemaDefRequirement in the hints of the chain. A hint applies only where no
          /// requirement of the chain defines a SchemaDefRequirement, because a requirement overrides a hint.
          Hinted: DefinedTypes option
          /// The types that apply where the chain holds no SchemaDefRequirement. A document with one process has none,
          /// and neither has the process main of a packed document or a process that its steps reach. Any other process
          /// of a packed document that no step runs has every type of the document, because a runner may start with it.
          Fallback: DefinedTypes }

    /// The class of a process. CWL gives the ports of each class their own fields.
    type ProcessClass =
        | CommandLineToolClass
        | WorkflowClass
        | ExpressionToolClass
        | OperationClass

    /// What the decoders of types, ports and steps know about the place that they read.
    type DecodeScope =
        { /// True where the place reads the shorthand of a type name (File[], File?). A type under items does not.
          Shorthand: bool
          /// The class of the process whose ports the place reads, and none outside of a process. A port list without
          /// a process around it admits the fields of every class.
          Process: ProcessClass option
          /// Where the place admits the stream types.
          StreamTypes: StreamTypes
          /// Which names the place admits for a type that a SchemaDefRequirement defines.
          References: ReferenceNames
          /// The ids of the types that the whole document defines, without the leading #. Schema Salad gives a type
          /// the id of the process or step that holds its SchemaDefRequirement, followed by its name: Name in a
          /// process without an id, and main/Name in the process main of a packed document. A step adds its own id,
          /// as in wf/move/Name for the step move of the workflow wf. A name in a port finds its id in this set, and
          /// the type applies only when InForce holds the id too.
          Definitions: Set<string>
          /// The paths of the files that the SchemaDefRequirements of the document import, with the . and .. segments
          /// resolved.
          ImportedPaths: Set<string>
          /// The chains of enclosing processes and steps that reach the place, with the innermost SchemaDefRequirement
          /// of each chain. Only the innermost one applies. The requirement of a step overrides the requirement of its
          /// workflow, and the requirement of a process overrides both. A process of a packed document has one chain
          /// for each step that runs it, and a document with one process has a single chain.
          Chains: SchemaDefChain list
          /// The types that apply at the place. A chain applies the types of its Required, else the types of its
          /// Hinted, else its Fallback, and a type applies only when every chain applies it, as cwltool checks a
          /// process once for each step that runs it. While an entry of a SchemaDefRequirement decodes, they are the
          /// types of the entries before it and the entry itself, because the requirement overrides the types around it.
          InForce: DefinedTypes
          /// The ids of the types that the place cannot use yet, because the SchemaDefRequirement that it belongs to
          /// defines them further down its list. They are not in InForce. The scope keeps them to tell a type that
          /// comes too late from a type that nothing defines.
          Later: Set<string>
          /// The id of the nearest object around the place that has one, without the leading #, and the empty text for
          /// the document itself. A type name in a port is relative to the id of the port, which Schema Salad
          /// resolves two levels up (refScope 2), so DocId is the base of every lookup of a name.
          DocId: string
          /// The ids of the processes of the packed document that the place belongs to, without a leading #, or none
          /// outside of a packed document. A step of such a document refers to a process by #id.
          GraphIds: Set<string> option
          /// The cwlVersion that a process without one of its own takes. It is the version of the enclosing process
          /// or of the packed document, and a document at the root has none.
          DefaultCwlVersion: string option }

    /// The scope of the root of a document.
    let rootScope : DecodeScope =
        { Shorthand = true
          Process = None
          StreamTypes = NoStreams
          References = DefinedNames
          Definitions = Set.empty
          ImportedPaths = Set.empty
          Chains = [ { Required = None; Hinted = None; Fallback = { Ids = Set.empty; Imports = Set.empty } } ]
          InForce = { Ids = Set.empty; Imports = Set.empty }
          Later = Set.empty
          DocId = ""
          GraphIds = None
          DefaultCwlVersion = None }

    let private scalarText (node: YamlNode) : string option =
        match node.Kind with
        | YamlNodeKind.Scalar scalar -> Some scalar.Value
        | _ -> None

    /// The entries of a mapping, and none for any other node. The readers of the raw node before the decode must not
    /// fail, because the decoder of the requirement reports a malformed node at its place.
    let private entriesOf (node: YamlNode) : MappingEntry list =
        match node.Kind with
        | YamlNodeKind.Mapping collection -> collection.Items
        | _ -> []

    let private valueOf (key: string) (node: YamlNode) : YamlNode option =
        entriesOf node |> List.tryPick (fun entry -> if scalarText entry.Key = Some key then Some entry.Value else None)

    let private itemsOf (node: YamlNode) : YamlNode list =
        match node.Kind with
        | YamlNodeKind.Sequence collection -> collection.Items |> List.map (fun item -> item.Node)
        | _ -> []

    /// The path that an entry of the types field of a SchemaDefRequirement imports, as written. The entry is a
    /// mapping with the $import directive only. The types field can be such a mapping as a whole too.
    let private importedPathOf (entry: YamlNode) : string option =
        match entriesOf entry with
        | [ single ] when scalarText single.Key = Some "$import" -> scalarText single.Value
        | _ -> None

    /// The path of an imported file with its . and .. segments resolved, so that ./shapes.yml, a/../shapes.yml and
    /// shapes.yml name one file. Schema Salad resolves the path of the import and the path in front of the # of a type
    /// name against the document in the same way. A path with a scheme stays as written.
    let private normalizedPath (path: string) : string =
        if path.Contains "://" then
            path
        else
            let segments =
                path.Split('/')
                |> Array.toList
                |> List.filter (fun segment -> segment <> "" && segment <> ".")
                |> List.fold
                    (fun resolved segment ->
                        match resolved with
                        | previous :: rest when segment = ".." && previous <> ".." -> rest
                        | _ -> segment :: resolved)
                    []
                |> List.rev
            (if path.StartsWith "/" then "/" else "") + String.concat "/" segments

    /// The entries of the types field of a SchemaDefRequirement. The field is a list. A mapping with the $import
    /// directive only is the field as a whole, and Schema Salad replaces it with the list of the imported file, so it
    /// stands for a list with one import.
    let private typeEntriesIn (types: YamlNode) : YamlNode list =
        match types.Kind with
        | YamlNodeKind.Sequence _ -> itemsOf types
        | YamlNodeKind.Mapping _ when (importedPathOf types).IsSome -> [ types ]
        | _ -> []


    /// The nodes of the SchemaDefRequirements in a requirements or hints field, in list and in map syntax.
    let private schemaDefsIn (field: YamlNode) : YamlNode list =
        match field.Kind with
        | YamlNodeKind.Sequence _ ->
            itemsOf field |> List.filter (fun item -> (valueOf "class" item |> Option.bind scalarText) = Some "SchemaDefRequirement")
        | YamlNodeKind.Mapping _ -> valueOf "SchemaDefRequirement" field |> Option.toList
        | _ -> []

    /// The id that Schema Salad gives an object, without the leading #. An id that starts with # is absolute in the
    /// document. Any other id is relative to the id of the enclosing object, and the document has the empty id.
    let private scopedId (enclosing: string) (id: string) : string =
        let id = id.Trim()
        if id.StartsWith "#" then id.Substring 1
        elif enclosing = "" then id
        else enclosing + "/" + id

    /// The id of a process, which is its id field resolved against the id of the enclosing object. A process without
    /// an id has the enclosing id when it stands at the root. An inline run of a step lives below the name of its
    /// field, run, so the caller passes that name as the segment. Its id field is relative to that segment, and a run
    /// without an id has the segment as its id.
    let private processId (enclosing: string) (segment: string) (node: YamlNode) : string =
        let baseId = if segment = "" then enclosing else scopedId enclosing segment
        match valueOf "id" node |> Option.bind scalarText with
        | Some id -> scopedId baseId id
        | None -> baseId

    /// The steps of a workflow node with their ids, in map and in list form.
    let private stepsOf (node: YamlNode) : (string * YamlNode) list =
        match valueOf "steps" node with
        | Some steps ->
            match steps.Kind with
            | YamlNodeKind.Mapping _ ->
                entriesOf steps |> List.choose (fun entry -> scalarText entry.Key |> Option.map (fun id -> id, entry.Value))
            | YamlNodeKind.Sequence _ ->
                itemsOf steps |> List.choose (fun step -> valueOf "id" step |> Option.bind scalarText |> Option.map (fun id -> id, step))
            | _ -> []
        | None -> []

    /// The fields of a record schema with the type of each one. The fields are a list of objects with a name, or a
    /// mapping from the name to the type or to an object that holds it. An object that holds the type is the field
    /// itself, as the decoder reads it.
    let private fieldTypesOf (record: YamlNode) : (string * YamlNode) list =
        match valueOf "fields" record with
        | Some fields ->
            match fields.Kind with
            | YamlNodeKind.Sequence _ ->
                itemsOf fields
                |> List.choose (fun field ->
                    match valueOf "name" field |> Option.bind scalarText, valueOf "type" field with
                    | Some name, Some fieldType -> Some (name, fieldType)
                    | _ -> None)
            | YamlNodeKind.Mapping _ ->
                entriesOf fields
                |> List.choose (fun entry ->
                    scalarText entry.Key
                    |> Option.map (fun name ->
                        match entry.Value.Kind, valueOf "type" entry.Value with
                        | YamlNodeKind.Mapping _, Some fieldType -> name, fieldType
                        | _ -> name, entry.Value))
            | _ -> []
        | None -> []

    /// The ids of the named record and enum schemas in a type, including the schema itself. A named schema has the id
    /// of its name below the given id. The fields of a record are objects with ids of their own below the id of the
    /// record, or below the given id when the record has no name, and the schemas in their types get ids below the id
    /// of the field. An array schema has no id, so the schemas in its items stand below the given id.
    let rec private namedSchemaIds (parentId: string) (node: YamlNode) : string list =
        match node.Kind with
        | YamlNodeKind.Sequence _ -> itemsOf node |> List.collect (namedSchemaIds parentId)
        | YamlNodeKind.Mapping _ ->
            let ownId = valueOf "name" node |> Option.bind scalarText |> Option.map (scopedId parentId)
            match valueOf "type" node |> Option.bind scalarText with
            | Some "record" ->
                let recordId = defaultArg ownId parentId
                Option.toList ownId
                @ (fieldTypesOf node |> List.collect (fun (name, fieldType) -> namedSchemaIds (scopedId recordId name) fieldType))
            | Some "enum" -> Option.toList ownId
            | Some "array" -> valueOf "items" node |> Option.map (namedSchemaIds parentId) |> Option.defaultValue []
            | _ -> valueOf "type" node |> Option.map (namedSchemaIds parentId) |> Option.defaultValue []
        | _ -> []

    /// The ids that an entry of the types field of a SchemaDefRequirement defines below the id of its holder: its own
    /// id and the ids of the named schemas inside it. An import and an unnamed schema add no id of their own.
    let private entryIds (holderId: string) (entry: YamlNode) : string list =
        if (importedPathOf entry).IsSome then [] else namedSchemaIds holderId entry

    /// The entries of the types field of the SchemaDefRequirements in the requirements and the hints of a process or
    /// a step.
    let private typeEntriesOf (holder: YamlNode) : YamlNode list =
        [ "requirements"; "hints" ]
        |> List.collect (fun field -> valueOf field holder |> Option.toList)
        |> List.collect schemaDefsIn
        |> List.collect (fun schemaDef -> valueOf "types" schemaDef |> Option.map typeEntriesIn |> Option.defaultValue [])

    /// The ids of the types that a document defines and the paths of the files that it imports. The walk reads the raw
    /// nodes of each process and of its steps, and it descends into the inline runs of the steps. The ports decode
    /// before the requirements of their process, and every port sees the definitions of the whole document.
    let private documentDefinitions (root: YamlNode) : Set<string> * Set<string> =
        let definitions = ResizeArray<string>()
        let imports = ResizeArray<string>()
        let addHolder (holderId: string) (holder: YamlNode) =
            for entry in typeEntriesOf holder do
                match importedPathOf entry with
                | Some path -> imports.Add (normalizedPath path)
                | None -> definitions.AddRange (entryIds holderId entry)
        let rec addProcess (id: string) (processNode: YamlNode) =
            addHolder id processNode
            for (stepName, step) in stepsOf processNode do
                let stepId = scopedId id stepName
                addHolder stepId step
                match valueOf "run" step with
                | Some run -> addProcess (processId stepId "run" run) run
                | None -> ()
        match valueOf "$graph" root with
        | Some graph -> itemsOf graph |> List.iter (fun processNode -> addProcess (processId "" "" processNode) processNode)
        | None -> addProcess (processId "" "" root) root
        Set.ofSeq definitions, Set.ofSeq imports

    /// The scope of a document, with the types that it defines, the files that it imports and the id of its root
    /// process. The definitions come from a pre-pass over the raw nodes.
    let documentScope (root: YamlNode) : DecodeScope =
        let definitions, imports = documentDefinitions root
        { rootScope with Definitions = definitions; ImportedPaths = imports; DocId = processId "" "" root }

    /// The types that a SchemaDefRequirement defines, with the ids relative to the id of the process or step that
    /// holds it.
    let private definedTypesOf (holderId: string) (schemaDef: YamlNode) : DefinedTypes =
        let entries = valueOf "types" schemaDef |> Option.map typeEntriesIn |> Option.defaultValue []
        { Ids = entries |> List.collect (entryIds holderId) |> Set.ofList
          Imports = entries |> List.choose importedPathOf |> List.map normalizedPath |> Set.ofList }

    /// The types that a chain applies: those of its requirement, else those of its hint, else its fallback.
    let private appliedBy (chain: SchemaDefChain) : DefinedTypes =
        match chain.Required, chain.Hinted with
        | Some types, _ -> types
        | None, Some types -> types
        | None, None -> chain.Fallback

    /// The scope inside a process or a step. The SchemaDefRequirement of the process or step overrides the one of the
    /// enclosing process or step in each chain, and a requirement overrides a hint of the same class. The last
    /// SchemaDefRequirement of a list is the innermost one, as in cwltool. The id of the holder is the DocId of the
    /// scope.
    let private enterHolder (holder: YamlNode) (scope: DecodeScope) : DecodeScope =
        let innermostIn (field: string) =
            valueOf field holder |> Option.bind (fun types -> schemaDefsIn types |> List.tryLast) |> Option.map (definedTypesOf scope.DocId)
        let required = innermostIn "requirements"
        let hinted = innermostIn "hints"
        let chains =
            scope.Chains
            |> List.map (fun chain -> { chain with Required = Option.orElse chain.Required required; Hinted = Option.orElse chain.Hinted hinted })
            |> List.distinct
        let inForce =
            chains
            |> List.map appliedBy
            |> List.reduce (fun left right -> { Ids = Set.intersect left.Ids right.Ids; Imports = Set.intersect left.Imports right.Imports })
        { scope with Chains = chains; InForce = inForce }

    /// The id that a link names among the given type ids, and none when it names no id. A link that starts with # is
    /// the absolute id in the document. Any other link is relative to the id of the port or of the record field that
    /// holds it, and Schema Salad drops the last two segments of that id first (refScope 2). It looks for the link
    /// behind what remains and then behind each shorter prefix, down to the root of the document.
    let private resolveLink (scope: DecodeScope) (ids: Set<string>) (link: string) : string option =
        if link.StartsWith "#" then
            Some (link.Substring 1) |> Option.filter (fun id -> Set.contains id ids)
        else
            let owner = if scope.DocId = "" then [] else scope.DocId.Split('/') |> Array.toList
            let rec lookup (prefix: string list) =
                let id = String.concat "/" (prefix @ [ link ])
                if Set.contains id ids then Some id
                elif prefix.IsEmpty then None
                else lookup (List.truncate (prefix.Length - 1) prefix)
            lookup (List.truncate (max 0 (owner.Length - 2)) owner)

    /// The path in front of the # of a type name, with the . and .. segments resolved, and none for a name without a
    /// path. A type of an imported file is named behind the path of the file (path#Name).
    let private importedPathOfName (name: string) : string option =
        let hash = name.IndexOf '#'
        if hash > 0 then Some (normalizedPath (name.Substring(0, hash))) else None

    /// True when the name spells a type that applies at the place. The name finds an id in the document, and the
    /// innermost SchemaDefRequirement defines that id. A name behind a path needs the import in that requirement.
    let private isDefinedReference (scope: DecodeScope) (name: string) : bool =
        match importedPathOfName name with
        | Some path -> Set.contains path scope.InForce.Imports
        | None -> resolveLink scope scope.Definitions name |> Option.exists (fun id -> Set.contains id scope.InForce.Ids)

    /// True when a Reference can spell the name: it is not empty, holds no whitespace and none of the characters [ ] ?
    /// that the shorthand of a type uses.
    let private isWellFormedName (name: string) : bool =
        name <> "" && not (name |> Seq.exists (fun character -> System.Char.IsWhiteSpace character || character = '[' || character = ']' || character = '?'))

    /// The type that a type name spells. The node is the scalar that holds the name, and an unknown name is a decode
    /// error at its place. A name that is no builtin type is a Reference when the scope defines it, or when the scope
    /// takes every well-formed name. The stream types are no name here. A port admits them as its whole type, which
    /// the readers of a port type check before they read a name.
    let cwlSimpleTypeFromString (scope: DecodeScope) (node: YamlNode) (name: string) : CWLType =
        match name with
        | "File" -> File
        | "Directory" -> Directory
        | "string" -> String
        | "int" -> Int
        | "long" -> Long
        | "float" -> Float
        | "double" -> Double
        | "boolean" -> Boolean
        | "Any" -> Any
        | "stdout" | "stderr" ->
            raise (
                YamlDecodeException
                    { Message =
                        "The type " + name + " is valid only as the whole type"
                        + (if scope.StreamTypes = AnyStreams then "." else " of an output of a command line tool, without an outputBinding.")
                      Path = []
                      Span = node.Span }
            )
        | "stdin" ->
            raise (
                YamlDecodeException
                    { Message =
                        "The type stdin is valid only as the whole type"
                        + (if scope.StreamTypes = AnyStreams then "." else " of an input of a command line tool, without an inputBinding.")
                      Path = []
                      Span = node.Span }
            )
        | "null" -> Null
        | "record" | "enum" | "array" ->
            raise (
                YamlDecodeException
                    { Message =
                        "Expected a type name, found \"" + name + "\". A schema is a mapping, so a schema belongs under type."
                      Path = []
                      Span = node.Span }
            )
        | _ when (scope.References = AnyNames && isWellFormedName name) || isDefinedReference scope name -> Reference name
        | _ ->
            // path#Name names a type in a file. The name inside the file stays unchecked, so only the path can be unknown.
            let hash = name.IndexOf '#'
            let overridden =
                "Invalid CWL type: " + name + ". The document defines this type in a SchemaDefRequirement that does not apply here. The innermost SchemaDefRequirement applies. It overrides the one of the enclosing process, and a requirement overrides a hint."
                + (if scope.GraphIds.IsSome then " A process of a packed document that a step runs takes the SchemaDefRequirement that applies at the step, and a process that several steps run needs the type at each of them." else "")
            let message =
                if (resolveLink scope scope.Later name).IsSome then
                    "Invalid CWL type: " + name + ". The SchemaDefRequirement defines this type further down. It processes its types in the order listed, so a type may name only the types before it and itself."
                elif hash > 0 then
                    if Set.contains (normalizedPath (name.Substring(0, hash))) scope.ImportedPaths then overridden
                    else "Invalid CWL type: " + name + ". The file " + name.Substring(0, hash) + " is not imported by a SchemaDefRequirement of the document."
                elif (resolveLink scope scope.Definitions name).IsSome then
                    overridden
                else
                    "Invalid CWL type: " + name
            raise (YamlDecodeException { Message = message; Path = []; Span = node.Span })

    /// Read a type name with the shorthand of a CWL document. T[] is an array of T and T? is null or T, so T[]? is an
    /// optional array. A document reads one [] only, because CWL reads a nested array as a schema. The scope tells
    /// whether the place of the node reads the shorthand, which a type under items does not.
    let cwlTypeFromShorthand (scope: DecodeScope) (node: YamlNode) : CWLType =
        let fail (message: string) : 'T =
            raise (YamlDecodeException { Message = message; Path = []; Span = node.Span })
        let text = typeText node
        let optional = text.EndsWith "?"
        let withoutOptional = if optional then text.Substring(0, text.Length - 1) else text
        let isArray = withoutOptional.EndsWith "[]"
        let name = if isArray then withoutOptional.Substring(0, withoutOptional.Length - 2) else withoutOptional
        if (optional || isArray) && not scope.Shorthand then
            fail (
                "Expected a type name or a schema under items, found " + text + ". CWL reads the shorthand [] and ? as a type and "
                + "as a member of a type list. Write the array as a schema with type: array and items."
            )
        elif name.EndsWith "[]" then
            fail ("Expected one [] behind a type name, found " + text + ". CWL reads a nested array as a schema with type: array and items.")
        else
            let baseType = cwlSimpleTypeFromString scope node name
            let arrayType = if isArray then Array (InputArraySchema(baseType)) else baseType
            if optional then Union (ResizeArray [ Null; arrayType ]) else arrayType

    /// The scope of a place that reads the shorthand of a type name, or of one that does not.
    let private withShorthand (shorthand: bool) (scope: DecodeScope) : DecodeScope =
        { scope with Shorthand = shorthand }

    /// The scope inside an object that has an id, such as a port or a record field. The id is the given name resolved
    /// against the id of the enclosing object, and the type names inside are relative to it.
    let private withChildId (name: string) (scope: DecodeScope) : DecodeScope =
        { scope with DocId = scopedId scope.DocId name }

    /// Decode an InputArraySchema from a YamlNode
    let rec inputArraySchemaDecoder (scope: DecodeScope) : (YamlNode -> InputArraySchema) =
        fun value ->
        Decode.object (fun get ->
            // The items are a type name or a schema, with no shorthand.
            let decodedItems = get.Required.Field "items" (itemsTypeDecoder scope)

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
    and inputRecordFieldObject (scope: DecodeScope) (name: string) (value: YamlNode) : InputRecordField =
        let decodedType =
            value |> Decode.object (fun get -> get.Required.Field "type" (cwlTypeDecoderAt (scope |> withChildId name |> withShorthand true)))
        let doc, label, sourceNodes = docAndLabelDecoder value
        let field = InputRecordField(name, decodedType, ?doc = doc, ?label = label)
        field.SourceNodes <- sourceNodes
        overflowIntoDynamicObj field (InputRecordField.KnownFieldNames |> Seq.toList) value |> ignore
        field

    /// Decode an InputRecordField from a YamlNode
    and inputRecordFieldDecoder (scope: DecodeScope) : (YamlNode -> InputRecordField) =
        fun value ->
            // Array-style record fields carry their own name field.
            let name = value |> Decode.object (fun get -> get.Required.Field "name" (Decode.nonNull Decode.string))
            atField name (fun () -> inputRecordFieldObject scope name value)

    /// Decode fields as an array: [{name: x, type: y}]
    and decodeFieldsAsArray (scope: DecodeScope) (element: YamlNode) : ResizeArray<InputRecordField> =
        Decode.resizearray (inputRecordFieldDecoder scope) element

    /// Decode fields as a map: {fieldName: type} or {fieldName: {type: ..., inputBinding: ...}}
    and decodeFieldsAsMap (scope: DecodeScope) (element: YamlNode) : ResizeArray<InputRecordField> =
        let fields = ResizeArray<InputRecordField>()
        for entry in mappingEntries element do
            // Map-style record fields use the mapping key as the field name. A mapping is the field object, with the
            // type and the other properties of the field. Any other value is the type itself.
            let name = entryName entry
            let field =
                atField name (fun () ->
                    match entry.Value.Kind with
                    | YamlNodeKind.Mapping _ -> inputRecordFieldObject scope name entry.Value
                    | _ -> InputRecordField(name, cwlTypeDecoderAt (scope |> withChildId name |> withShorthand true) entry.Value))
            fields.Add(field)
        fields

    /// Decode an InputRecordSchema from a YamlNode
    and inputRecordSchemaDecoder (scope: DecodeScope) : (YamlNode -> InputRecordSchema) =
        fun value ->
        Decode.object (fun get ->
            let recordName = get.Optional.Field "name" Decode.string
            // The fields of a named record are relative to the id of the record.
            let scope = match recordName with Some name -> withChildId name scope | None -> scope
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
                        | YamlNodeKind.Sequence _ -> Some (decodeFieldsAsArray scope element)
                        | YamlNodeKind.Mapping _ -> Some (decodeFieldsAsMap scope element)
                        | _ ->
                            raise (
                                YamlDecodeException
                                    { Message = "Expected a list of fields or a mapping of fields, found " + describeNode element + "."
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
                    ?name = recordName
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
        cwlTypeDecoderAt rootScope element

    /// Decode the type under items. It is a name or a schema, and a list of those, with no shorthand.
    and itemsTypeDecoder (scope: DecodeScope) (element: YamlNode): CWLType =
        cwlTypeDecoderAt (withShorthand false scope) element

    /// Decode a CWLType from a YamlNode. The scope tells whether the place reads the shorthand of a type name.
    and cwlTypeDecoderAt (scope: DecodeScope) (element: YamlNode): CWLType =
        match element.Kind with
        | YamlNodeKind.Scalar _ ->
            // Simple type string
            cwlTypeFromShorthand scope element
        | YamlNodeKind.Sequence collection ->
            // Union type. Each member is a type name with the shorthand of its place, an array schema, a record or an enum.
            let types =
                collection.Items
                |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> cwlTypeDecoderAt scope item.Node))
                |> ResizeArray
            Union types
        | YamlNodeKind.Mapping _ ->
            // Complex type - check for type field
            Decode.object (fun get ->
                match get.Optional.Field "type" Decode.node with
                | Some typeNode ->
                    // A mapping is a schema, and its type field names the kind of the schema. A type name stands for
                    // itself, without a mapping around it, so the fields of a File belong to the port. The schema of a
                    // port or of a record field stands under its own type field, so a schema or a list in the type
                    // field of a schema is a misplaced level.
                    match scalarText typeNode with
                    | Some "record" -> Record (inputRecordSchemaDecoder scope element)
                    | Some "enum" -> Enum (inputEnumSchemaDecoder element)
                    | Some "array" -> Array (inputArraySchemaDecoder scope element)
                    | _ ->
                        atField "type" (fun () ->
                            raise (
                                YamlDecodeException
                                    { Message = "Expected record, enum or array in the type field of a schema, found " + describeNode typeNode + "."
                                      Path = []
                                      Span = typeNode.Span }
                            ))
                | None ->
                    raise (
                        YamlDecodeException
                            { Message = "Expected a type name or a schema, found a mapping without a type field."
                              Path = []
                              Span = element.Span }
                    )
            ) element
        | YamlNodeKind.Alias name -> failAlias name element

    /// The stream type that the whole type text of a port spells, when the scope admits it. Text such as stdout? or
    /// stdout[] spells no stream type, and the name reader rejects it.
    let private tryWholeStreamType (scope: DecodeScope) (node: YamlNode) : CWLType option =
        match scope.StreamTypes, typeText node with
        | (OutputStreams | AnyStreams), "stdout" -> Some Stdout
        | (OutputStreams | AnyStreams), "stderr" -> Some Stderr
        | (InputStreams | AnyStreams), "stdin" -> Some Stdin
        | _ -> None

    /// The scope of a type that stands without a port or a schema around it: the type text of an RO-Crate entity and
    /// the type of a parameter file. No SchemaDefRequirement is in view, so a well-formed name that is no builtin type
    /// is a Reference, and the stream types are valid as the whole type only.
    let typeTextScope : DecodeScope =
        { rootScope with StreamTypes = AnyStreams; References = AnyNames }

    /// Decode the type text of a scalar, whether it stands alone or in the type field of a mapping.
    let cwlTypeStringDecoder (scope: DecodeScope) (node: YamlNode) : CWLType =
        match tryWholeStreamType scope node with
        | Some streamType -> streamType
        | None -> cwlTypeFromShorthand scope node

    /// Decode the text of the additional type of an RO-Crate entity. The text is YAML. A scalar is a type name such as
    /// File[][]? and may be quoted, as the null type is written ("null"), or carry a comment. The shorthand nests
    /// arrays there, which a CWL document does not read. Anything else is a schema or a list of types. The text has no
    /// port and no SchemaDefRequirement around it, so it reads like any type text (typeTextScope). A stream type is the
    /// whole text, and a name that is no builtin type is a Reference when it is well-formed.
    let cwlTypeFromAdditionalType (text: string) : CWLType =
        let node = readYaml text
        let scope = typeTextScope
        match node.Kind with
        | YamlNodeKind.Scalar _ ->
            match tryWholeStreamType scope node with
            | Some streamType -> streamType
            | None ->
                let typeName = typeText node
                let optional = typeName.EndsWith "?"
                let stripped = if optional then typeName.Substring(0, typeName.Length - 1) else typeName
                // Any number of [] stands behind the name.
                let mutable name = stripped
                let mutable depth = 0
                while name.EndsWith "[]" do
                    name <- name.Substring(0, name.Length - 2)
                    depth <- depth + 1
                let mutable arrayType = cwlSimpleTypeFromString scope node name
                for _ in 1 .. depth do
                    arrayType <- Array (InputArraySchema(arrayType))
                if optional then Union (ResizeArray [ Null; arrayType ]) else arrayType
        | _ -> cwlTypeDecoderAt scope node

    /// Access the type field and decode a YamlNode into a CWLType
    let cwlTypeDecoder (scope: DecodeScope) : (YamlNode -> CWLType) =
        Decode.object (fun get ->
            let typeNode = get.Required.Field "type" Decode.node
            match typeNode.Kind with
            | YamlNodeKind.Scalar _ ->
                // Scalar type field, including shorthand such as File[]?.
                atField "type" (fun () -> cwlTypeStringDecoder scope typeNode)
            | _ ->
                // Complex type field, for example inline record/enum/array schema.
                get.Required.Field "type" (cwlTypeDecoderAt scope)
        )

    /// Decode the type of a port. CWL lets a map entry hold just the type, written as a type string
    /// (File?) or as a list of types (a union such as [null, File]). Any other value is the port object.
    let portTypeDecoder (scope: DecodeScope) (value: YamlNode) : CWLType =
        match value.Kind with
        | YamlNodeKind.Scalar _ ->
            cwlTypeStringDecoder scope value
        | YamlNodeKind.Sequence _ ->
            cwlTypeDecoderAt scope value
        | _ -> cwlTypeDecoder scope value

    /// The entry of a mapping with the given key, and none for any other node.
    let private fieldEntry (key: string) (node: YamlNode) : MappingEntry option =
        entriesOf node |> List.tryFind (fun entry -> scalarText entry.Key = Some key)

    /// The name of a process class as it stands in a message, with its article.
    let private processName (processClass: ProcessClass) : string =
        match processClass with
        | CommandLineToolClass -> "a command line tool"
        | WorkflowClass -> "a workflow"
        | ExpressionToolClass -> "an expression tool"
        | OperationClass -> "an operation"

    /// Decode a named output from either map shorthand or full object form.
    let decodeNamedOutput (scope: DecodeScope) (name: string) (value: YamlNode) =
        // Only the output of a workflow takes the output of a step. A place outside of a process admits every field.
        match scope.Process, fieldEntry "outputSource" value with
        | Some processClass, Some entry when processClass <> WorkflowClass ->
            atField "outputSource" (fun () ->
                failAt entry.Key ("An output of " + processName processClass + " has no outputSource. Only an output of a workflow names the output of a step."))
        | _ -> ()
        // Decode dependent fields before constructing the output so both shorthand
        // and object forms share the same object initialization path.
        let outputBinding = outputBindingDecoder value
        let outputSourceValues = outputSourceDecoder value
        let cwlType = portTypeDecoder (withChildId name scope) value
        // The tool writes the file of a stream output itself, so a binding has nothing to bind.
        let streamWithBinding (typeName: string) : unit =
            raise (
                YamlDecodeException
                    { Message = "The type " + typeName + " is valid only for an output without an outputBinding."
                      Path = []
                      Span = value.Span }
            )
        match cwlType, outputBinding with
        | Stdout, Some _ -> streamWithBinding "stdout"
        | Stderr, Some _ -> streamWithBinding "stderr"
        | _ -> ()
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

    /// Fail at the id of an entry of a list that repeats the id of an earlier entry. The list form names its entries by
    /// their id fields, and the map form cannot repeat a key, so an id occurs once in either. An entry without an id has
    /// none to repeat.
    let private rejectRepeatedIds (items: YamlNode list) : unit =
        let seen = System.Collections.Generic.Dictionary<string, YamlNode>()
        items
        |> List.iteri (fun index item ->
            match valueOf "id" item |> Option.bind (fun idNode -> scalarText idNode |> Option.map (fun id -> id, idNode)) with
            | Some (id, idNode) ->
                match seen.TryGetValue id with
                | true, first ->
                    atField $"[{index}]" (fun () ->
                        atField "id" (fun () ->
                            failAt idNode ("The id " + id + " belongs to an earlier entry of the list too, at " + placeOf first + ". An id must be unique in the list.")))
                | _ -> seen.[id] <- idNode
            | None -> ())

    /// Decode one sequence-form output, warning only for malformed unnamed entries.
    let decodeOutputSequenceItem (warnings: WarningSink) (scope: DecodeScope) (path: string) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        | Some id -> Some (atField id (fun () -> decodeNamedOutput scope id item))
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL output entry." (Some item)
            None

    /// Decode a YamlNode into an Output Array
    let outputArrayDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) (path: string) : (YamlNode -> ResizeArray<CWLOutput>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form ports must declare id on each item. Unnamed malformed
                // items are the tolerant path and produce warnings.
                rejectRepeatedIds (collection.Items |> List.map (fun item -> item.Node))
                collection.Items
                |> List.mapi (fun index item -> decodeOutputSequenceItem warnings scope path index item.Node)
                |> List.choose id
                |> ResizeArray
            | _ ->
                // Map-form ports use the map key as the port name.
                [|
                    for entry in mappingEntries value do
                        let name = entryName entry
                        atField name (fun () -> decodeNamedOutput scope name entry.Value)
                |]
                |> ResizeArray

    let outputArrayDecoder: (YamlNode -> ResizeArray<CWLOutput>) =
        outputArrayDecoderWithWarnings None rootScope "outputs"

    /// Access the outputs field and decode a YamlNode into an Output Array
    let outputsDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<CWLOutput>) =
        Decode.object (fun get ->
            get.Required.Field "outputs" (outputArrayDecoderWithWarnings warnings scope "outputs")
        )

    let outputsDecoder: (YamlNode -> ResizeArray<CWLOutput>) =
        outputsDecoderWithWarnings None rootScope

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
        let decodeEnvValue = decodeSchemaSaladString

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
                                    get2.Required.Field "envValue" decodeEnvValue
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
                failAt envDefElement ("Expected a list or a mapping for envDef, found " + describeNode envDefElement + "."))

    /// Decode a YamlNode into a SoftwareRequirement array.
    /// Supports both array form and map shorthand form.
    let softwareRequirementDecoder (get: Decode.IGetters): ResizeArray<SoftwarePackage> =
        let decodeSpecsArray (element: YamlNode) =
            Decode.resizearray (Decode.nonNull Decode.string) element

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
                    SoftwarePackage(packageName, specs = ResizeArray [| Decode.nonNull Decode.string value |])

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
                failAt packagesElement ("Expected a list or a mapping for packages, found " + describeNode packagesElement + "."))

    /// Decode a YamlNode into an InitialWorkDirRequirement. The listing is a list of entries or an expression that
    /// evaluates to the list.
    let initialWorkDirRequirementDecoder (get: Decode.IGetters): Requirement =
        get.Required.Field "listing" (fun element ->
            let listingElement = Decode.nonNull Decode.node element
            match listingElement.Kind with
            | YamlNodeKind.Sequence _ ->
                InitialWorkDirRequirement (Decode.resizearray initialWorkDirEntryDecoder listingElement)
            | YamlNodeKind.Scalar _ ->
                InitialWorkDirExpressionRequirement (Decode.string listingElement)
            | _ ->
                failAt listingElement ("Expected a list of entries or an expression for the listing, found " + describeNode listingElement + "."))

    /// Decode LoadListingRequirement, applying CWL's default no_listing behavior.
    let loadListingRequirementDecoder (get: Decode.IGetters): LoadListingRequirementValue =
        LoadListingRequirementValue(
            get.Optional.Field "loadListing" loadListingDecoder
            |> Option.defaultValue NoListing
        )

    /// Decode resource scalar fields as int64, float, or expression string without losing numeric shape.
    /// CWL types these fields as long, float or string, so the core schema decides. A plain number keeps its
    /// numeric shape and a quoted number is a string.
    let decodeResourceScalar (element: YamlNode) : obj =
        match element with
        | CoreInt parsed -> box parsed
        | CoreFloat parsed -> box parsed
        | _ -> box (decodeSchemaSaladString element)

    /// Decode an optional resource field through the resource scalar coercion rules.
    let optionalResourceField (get: Decode.IGetters) (fieldName: string) : obj option =
        get.Optional.Field fieldName Decode.node
        |> Option.map (fun element -> atField fieldName (fun () -> decodeResourceScalar element))

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

    /// Decode an entry of the types field that defines a type. CWL defines the entry as the schema itself, with type:
    /// record or enum. Process.yml admits no other type there, so any other type is an error at the entry, such as a
    /// builtin type name or an array schema. The schema keeps its name, which is optional, and its extension fields.
    let schemaDefinitionDecoder (scope: DecodeScope) (value: YamlNode) : SchemaDefinition =
        let fail (message: string) : 'T =
            raise (YamlDecodeException { Message = message; Path = []; Span = value.Span })
        match valueOf "type" value with
        | Some typeNode ->
            match scalarText typeNode with
            | Some "record" -> SchemaDefinition.Record (inputRecordSchemaDecoder scope value)
            | Some "enum" -> SchemaDefinition.Enum (inputEnumSchemaDecoder value)
            | Some other -> fail ("A SchemaDefRequirement defines record and enum types only, found type: " + other + ".")
            | None -> fail "A SchemaDefRequirement defines record and enum types only, found a type that is no name."
        | None -> fail "Expected a type field. A SchemaDefRequirement defines a record or an enum."

    /// Decode an entry of the types field: a file to import, as {$import: path}, or a type to define.
    let schemaDefEntryDecoder (scope: DecodeScope) (value: YamlNode) : SchemaDefEntry =
        match importedPathOf value with
        | Some path -> Imported path
        | None -> Defined (schemaDefinitionDecoder scope value)

    /// Decode a YamlNode into a SchemaDefEntry array. A SchemaDefRequirement processes its types in the order listed
    /// (Process.yml), so an entry sees the entries before it, itself included. A type may name itself, which makes a
    /// recursive record. The requirement overrides the SchemaDefRequirements around it, so an entry does not see their
    /// types. The ports of the process see every entry of the innermost requirement. A types field that is an $import
    /// mapping stands for a list with one import.
    let schemaDefRequirementDecoder (scope: DecodeScope) (get: Decode.IGetters): ResizeArray<SchemaDefEntry> =
        get.Required.Field "types" (fun typesNode ->
            let isEntryList =
                match typesNode.Kind with
                | YamlNodeKind.Sequence _ -> true
                | _ -> (importedPathOf typesNode).IsSome
            if isEntryList then
                let entries = typeEntriesIn typesNode
                let ids = entries |> List.map (entryIds scope.DocId)
                let imports = entries |> List.map (fun entry -> importedPathOf entry |> Option.map normalizedPath)
                entries
                |> List.mapi (fun index entry ->
                    // An id that an earlier entry defines too stays visible.
                    let upToEntry = ids |> List.take (index + 1) |> List.concat |> Set.ofList
                    let later = ids |> List.skip (index + 1) |> List.concat |> Set.ofList |> fun later -> Set.difference later upToEntry
                    // The requirement overrides the types around it, so the entry sees its own requirement only.
                    let inForce = { Ids = upToEntry; Imports = imports |> List.take (index + 1) |> List.choose id |> Set.ofList }
                    let entryScope = { scope with InForce = inForce; Later = later }
                    atField $"[{index}]" (fun () -> schemaDefEntryDecoder entryScope entry))
                |> ResizeArray
            else
                Decode.resizearray (schemaDefEntryDecoder scope) typesNode)

    /// Decode only plain boolean scalars, as the core schema reads them, and leave every other node for the
    /// requirement to take as an expression. A quoted "false" is a string.
    let tryDecodeBoolScalar (element: YamlNode) : bool option =
        match element with
        | CoreBool parsed -> Some parsed
        | _ -> None

    /// Decode WorkReuse as either a concrete bool requirement or an expression payload. A WorkReuse without
    /// enableReuse enables the reuse.
    let workReuseRequirementDecoder (get: Decode.IGetters): Requirement =
        get.Optional.Field "enableReuse" (fun value ->
            match tryDecodeBoolScalar value with
            | Some boolValue -> WorkReuseRequirement (WorkReuseRequirementValue(boolValue))
            | None -> WorkReuseExpressionRequirement (decodeSchemaSaladString value))
        |> Option.defaultValue (WorkReuseRequirement (WorkReuseRequirementValue(true)))

    /// Decode NetworkAccess as either a concrete bool requirement or an expression payload. CWL requires the
    /// networkAccess field, and a missing one is no permission to use the network.
    let networkAccessRequirementDecoder (get: Decode.IGetters): Requirement =
        get.Required.Field "networkAccess" (fun value ->
            match tryDecodeBoolScalar value with
            | Some boolValue -> NetworkAccessRequirement (NetworkAccessRequirementValue(boolValue))
            | None -> NetworkAccessExpressionRequirement (decodeSchemaSaladString value))

    /// Decode InplaceUpdateRequirement. CWL requires the inplaceUpdate field.
    let inplaceUpdateRequirementDecoder (get: Decode.IGetters): InplaceUpdateRequirementValue =
        InplaceUpdateRequirementValue(get.Required.Field "inplaceUpdate" Decode.bool)

    /// Decode a YamlNode into a ToolTimeLimitRequirement value
    let toolTimeLimitRequirementDecoder (get: Decode.IGetters): ToolTimeLimitValue =
        let timeLimitElement = get.Required.Field "timelimit" (Decode.nonNull Decode.node)
        // CWL types the time limit as long or string, so a plain int is the number of seconds and
        // every other value is an expression.
        match timeLimitElement with
        | CoreInt seconds when seconds >= 0L -> ToolTimeLimitSeconds seconds
        | CoreInt _ -> atField "timelimit" (fun () -> failAt timeLimitElement "The timelimit of a ToolTimeLimit must not be negative.")
        | _ -> ToolTimeLimitExpression (atField "timelimit" (fun () -> decodeSchemaSaladString timeLimitElement))

    /// Decode InlineJavascriptRequirement, whose expressionLib is a list of strings.
    let inlineJavascriptRequirementDecoder (get: Decode.IGetters): InlineJavascriptRequirementValue =
        InlineJavascriptRequirementValue(?expressionLib = get.Optional.Field "expressionLib" decodeSchemaSaladStringArray)

    /// Dispatch a CWL requirement class to its typed decoder. A class that the model does not know has no decoder.
    let tryRequirementFromTypeName (scope: DecodeScope) cls get : Requirement option =
        match cls with
        | "InlineJavascriptRequirement" -> Some (InlineJavascriptRequirement (inlineJavascriptRequirementDecoder get))
        | "SchemaDefRequirement" -> Some (SchemaDefRequirement (schemaDefRequirementDecoder scope get))
        | "DockerRequirement" -> Some (DockerRequirement (dockerRequirementDecoder get))
        | "SoftwareRequirement" -> Some (SoftwareRequirement (softwareRequirementDecoder get))
        | "LoadListingRequirement" -> Some (LoadListingRequirement (loadListingRequirementDecoder get))
        | "InitialWorkDirRequirement" -> Some (initialWorkDirRequirementDecoder get)
        | "EnvVarRequirement" -> Some (EnvVarRequirement (envVarRequirementDecoder get))
        | "ShellCommandRequirement" -> Some ShellCommandRequirement
        | "ResourceRequirement" -> Some (ResourceRequirement (resourceRequirementDecoder get))
        | "WorkReuse" -> Some (workReuseRequirementDecoder get)
        | "NetworkAccess" -> Some (networkAccessRequirementDecoder get)
        | "InplaceUpdateRequirement" -> Some (InplaceUpdateRequirement (inplaceUpdateRequirementDecoder get))
        | "ToolTimeLimit" -> Some (ToolTimeLimitRequirement (toolTimeLimitRequirementDecoder get))
        | "SubworkflowFeatureRequirement" -> Some SubworkflowFeatureRequirement
        | "ScatterFeatureRequirement" -> Some ScatterFeatureRequirement
        | "MultipleInputFeatureRequirement" -> Some MultipleInputFeatureRequirement
        | "StepInputExpressionRequirement" -> Some StepInputExpressionRequirement
        | _ -> None

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
    let requirementArrayDecoderWithScope (scope: DecodeScope) : YamlNode -> ResizeArray<Requirement> =
        fun yEle ->
            // helper: decode a single requirement object that contain 'class' field
            let decodeSingleRequirementObject (ele: YamlNode) : Requirement =
                Decode.object (fun get ->
                    // Sequence syntax carries class inside each item.
                    let classNode = get.Required.Field "class" (Decode.nonNull Decode.node)
                    let cls = Decode.string classNode
                    match tryRequirementFromTypeName scope cls get with
                    | Some requirement -> addRequirementPayloadOverflow ele requirement
                    | None -> atField "class" (fun () -> failAt classNode ("Invalid or unsupported requirement class: " + cls))
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
                        Decode.object (fun get ->
                            match tryRequirementFromTypeName scope name get with
                            | Some requirement -> addRequirementPayloadOverflow entry.Value requirement
                            | None -> failAt entry.Key ("Invalid or unsupported requirement class: " + name)
                        ) entry.Value)
                )
                |> ResizeArray
            // INVALID CWL REQUIREMENTS
            | _ -> failAt yEle ("Expected a list or a mapping of requirements, found " + describeNode yEle + ".")

    /// Decode requirements from either sequence syntax or class-name map syntax, outside of any process.
    let requirementArrayDecoder : YamlNode -> ResizeArray<Requirement> =
        requirementArrayDecoderWithScope rootScope

    /// Decode one hint. A hint of a class that the model knows decodes with the decoder of the requirement of that
    /// class and fails with the same errors. A hint of any other class stays as it was written.
    let decodeHintElement (scope: DecodeScope) (element: YamlNode) : HintEntry =
        Decode.object (fun get ->
            let cls = get.Required.Field "class" (Decode.nonNull Decode.string)
            // A requirement of the same class overrides a SchemaDefRequirement hint in every chain, so no runner applies
            // its types. cwltool accepts the type names in such a hint without resolving them.
            let scope =
                if cls = "SchemaDefRequirement" && scope.Chains |> List.forall (fun chain -> chain.Required.IsSome) then
                    { scope with References = AnyNames }
                else
                    scope
            match tryRequirementFromTypeName scope cls get with
            | Some requirement -> KnownHint (addRequirementPayloadOverflow element requirement)
            | None -> UnknownHint (HintUnknownValue(Some cls, { Node = element }))
        ) element

    /// Decode hints from sequence syntax or class-name map syntax while preserving unknown hints.
    let hintArrayDecoderWithScope (scope: DecodeScope) : YamlNode -> ResizeArray<HintEntry> =
        fun yEle ->
            match yEle.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence hints can be known requirement payloads or arbitrary
                // extension hints.
                collection.Items
                |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeHintElement scope item.Node))
                |> ResizeArray
            | YamlNodeKind.Mapping _ ->
                mappingEntries yEle
                |> List.map (fun entry ->
                    let hintName = entryName entry
                    atField hintName (fun () ->
                        // A hint without a payload has the same meaning as a hint with an empty one.
                        let payload = if YamlNode.isMissingValue entry.Value then YamlNode.flowMapping [] else entry.Value
                        match payload.Kind with
                        | YamlNodeKind.Mapping collection ->
                            let hasClass =
                                // If map-style hint payload already has class,
                                // preserve it instead of overwriting with the key.
                                collection.Items
                                |> List.exists (fun payloadEntry -> entryName payloadEntry = "class")
                            let valueWithClass =
                                if hasClass then
                                    payload
                                else
                                    // Map-style hints use the key as synthetic class
                                    // for compatibility with sequence-form decoding.
                                    let classEntry = YamlNode.entry (YamlNode.plain "class") (Encode.string hintName)
                                    { payload with Kind = YamlNodeKind.Mapping { collection with Items = classEntry :: collection.Items } }
                            decodeHintElement scope valueWithClass
                        | _ ->
                            failAt payload ("Expected a mapping for the hint " + hintName + ", found " + describeNode payload + "."))
                )
                |> ResizeArray
            | _ ->
                failAt yEle ("Expected a list or a mapping of hints, found " + describeNode yEle + ".")

    /// Decode hints from sequence syntax or class-name map syntax, outside of any process.
    let hintArrayDecoder : YamlNode -> ResizeArray<HintEntry> =
        hintArrayDecoderWithScope rootScope

    /// Access the requirements field and decode the YamlNodes into a Requirement array
    let requirementsDecoderWithScope (scope: DecodeScope) : (YamlNode -> ResizeArray<Requirement> option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "requirements" (requirementArrayDecoderWithScope scope)
            requirements
        )

    /// Access the hints field and decode the YamlNodes into a HintEntry array
    let hintsDecoderWithScope (scope: DecodeScope) : (YamlNode -> ResizeArray<HintEntry> option) =
        Decode.object (fun get ->
            let hints = get.Optional.Field "hints" (hintArrayDecoderWithScope scope)
            hints
        )

    /// Decode the requirements of a node that stands alone. The types that its SchemaDefRequirements define are the
    /// only ones that its other requirements may name.
    let requirementsDecoder: (YamlNode -> ResizeArray<Requirement> option) =
        fun node -> requirementsDecoderWithScope (documentScope node) node

    /// Decode the hints of a node that stands alone, as requirementsDecoder decodes its requirements.
    let hintsDecoder: (YamlNode -> ResizeArray<HintEntry> option) =
        fun node -> hintsDecoderWithScope (documentScope node) node

    /// Decode the position of an input binding: an int, or an expression that evaluates to one. A plain scalar that the
    /// core schema reads as an int is the number, and a string with $( or ${ in it is the expression.
    let bindingPositionDecoder (node: YamlNode) : BindingPosition =
        match node, scalarText node with
        | CoreInt _, _ -> BindingPosition.Number (Decode.int node)
        | _, Some text when text.Contains "$(" || text.Contains "${" -> BindingPosition.Expression text
        | _ -> failAt node ("Expected an int or an expression, found " + describeNode node + ".")

    /// Decode a YamlNode into an InputBinding
    let inputBindingDecoder: (YamlNode -> InputBinding option) =
        let decodeBinding (value: YamlNode) =
            Decode.object (fun get' ->
                // InputBinding has several optional command-line
                // behavior fields. Absent fields remain None.
                let binding =
                    InputBinding(
                        ?prefix = get'.Optional.Field "prefix" Decode.string,
                        ?position = get'.Optional.Field "position" bindingPositionDecoder,
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

    /// Decode the format of an input: one name or a list of names. The format of an output admits one name only.
    let inputFormatDecoder (node: YamlNode) : InputFormat =
        match node.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> Decode.nonNull Decode.string item.Node))
            |> ResizeArray
            |> InputFormat.Multiple
        | _ -> InputFormat.Single (Decode.string node)

    /// Decode a named input from map shorthand or full object form, preserving optionality and extensions.
    let decodeNamedInput (scope: DecodeScope) (name: string) (value: YamlNode) =
        // The input of a command line tool has the full inputBinding. The input of a workflow or of an expression tool
        // has an inputBinding with loadContents only, and the input of an operation has none. A place outside of a
        // process admits every field.
        let bindingFieldsOfCommandLine = [ "prefix"; "position"; "itemSeparator"; "separate"; "valueFrom"; "shellQuote" ]
        match scope.Process, fieldEntry "inputBinding" value with
        | Some OperationClass, Some entry ->
            atField "inputBinding" (fun () -> failAt entry.Key "An input of an operation has no inputBinding.")
        | Some ((WorkflowClass | ExpressionToolClass) as processClass), Some entry ->
            match entriesOf entry.Value |> List.tryFind (fun field -> List.contains (scalarText field.Key |> Option.defaultValue "") bindingFieldsOfCommandLine) with
            | Some field ->
                atField "inputBinding" (fun () ->
                    atField (scalarText field.Key |> Option.defaultValue "") (fun () ->
                        failAt field.Key ("The inputBinding of an input of " + processName processClass + " has loadContents only. The other fields belong to the inputBinding of a command line tool.")))
            | None -> ()
        | _ -> ()
        // inputBinding is nested and decoded separately so both shorthand and object
        // input definitions can share the same construction flow.
        let inputBinding = inputBindingDecoder value
        let cwlType = portTypeDecoder (withChildId name scope) value
        match cwlType, inputBinding with
        | Stdin, Some _ ->
            // The tool reads a stdin input from its standard input, so a binding has nothing to bind.
            raise (
                YamlDecodeException
                    { Message = "The type stdin is valid only for an input without an inputBinding."
                      Path = []
                      Span = value.Span }
            )
        | _ -> ()
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
                ?format = tryGetPresentField "format" inputFormatDecoder value,
                ?loadContents = tryGetBoolField "loadContents" value,
                ?loadListing = tryGetLoadListingField "loadListing" value,
                ?defaultValue = tryGetYamlField "default" value
            )
        input.SourceNodes <- sourceNodes
        input.InputBinding <- inputBinding
        overflowIntoDynamicObj input (CWLInput.KnownFieldNames |> Seq.toList) value |> ignore
        input

    /// Decode one sequence-form input, warning only for malformed unnamed entries.
    let decodeInputSequenceItem (warnings: WarningSink) (scope: DecodeScope) (path: string) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        | Some id -> Some (atField id (fun () -> decodeNamedInput scope id item))
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL input entry." (Some item)
            None

    /// Decode a YamlNode into an Input array
    let inputArrayDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) (path: string) : (YamlNode -> ResizeArray<CWLInput>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form inputs must carry an id on each object item.
                rejectRepeatedIds (collection.Items |> List.map (fun item -> item.Node))
                collection.Items
                |> List.mapi (fun index item -> decodeInputSequenceItem warnings scope path index item.Node)
                |> List.choose id
                |> ResizeArray
            | _ ->
                // Map-form inputs use the map key as the input name.
                [|
                    for entry in mappingEntries value do
                        let name = entryName entry
                        atField name (fun () -> decodeNamedInput scope name entry.Value)
                |]
                |> ResizeArray

    let inputArrayDecoder: (YamlNode -> ResizeArray<CWLInput>) =
        inputArrayDecoderWithWarnings None rootScope "inputs"

    /// Access the inputs field and decode the YamlNodes into an Input array
    let inputsDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<CWLInput> option) =
        Decode.object (fun get ->
            get.Optional.Field "inputs" (inputArrayDecoderWithWarnings warnings scope "inputs")
        )

    let inputsDecoder: (YamlNode -> ResizeArray<CWLInput> option) =
        inputsDecoderWithWarnings None rootScope

    /// Decode the inputs of a process. CWL requires them for every class, so a missing inputs field is a decode error
    /// at the place of the process.
    let private requiredInputsDecoder (warnings: WarningSink) (scope: DecodeScope) (what: string) (yamlCWL: YamlNode) : ResizeArray<CWLInput> =
        match inputsDecoderWithWarnings warnings scope yamlCWL with
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
                    atField "baseCommand" (fun () ->
                        failAt baseCommandField ("Expected a string or a list of strings for baseCommand, found " + describeNode baseCommandField + "."))
            | None -> None
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
        tryGetPresentField field (fun node ->
            let linkMergeString = Decode.string node
            match LinkMergeMethod.tryParse linkMergeString with
            | Some linkMerge -> linkMerge
            | None -> failAt node ($"Invalid linkMerge value: {linkMergeString}"))

    /// Decode and validate pickValue enum fields.
    let pickValueFieldDecoder field : (YamlNode -> PickValueMethod option) =
        tryGetPresentField field (fun node ->
            let pickValueString = Decode.string node
            match PickValueMethod.tryParse pickValueString with
            | Some pickValue -> pickValue
            | None -> failAt node ($"Invalid pickValue value: {pickValueString}"))

    let scatterFieldDecoder field : (YamlNode -> ResizeArray<string> option) =
        fun value ->
            tryGetPresentField field Decode.node value
            |> Option.bind stringOrStringArrayDecoder

    /// Decode and validate scatterMethod enum fields.
    let scatterMethodFieldDecoder field : (YamlNode -> ScatterMethod option) =
        tryGetPresentField field (fun node ->
            let scatterMethodString = Decode.string node
            match ScatterMethod.tryParse scatterMethodString with
            | Some scatterMethod -> scatterMethod
            | None -> failAt node ($"Invalid scatterMethod value: {scatterMethodString}"))

    /// Decode optional expression-like fields, including schema-salad directive wrappers.
    let expressionStringOptionFieldDecoder field : (YamlNode -> SchemaSaladString option) =
        fun value ->
            tryGetPresentField field Decode.node value
            |> Option.map decodeSchemaSaladString

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
                ?loadListing = tryGetLoadListingField "loadListing" value,
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
            // CWL requires out, and out is a list. `out: []` has no outputs, and a key without a value is an error.
            let outField = get.Required.Field "out" (Decode.nonNull Decode.node)
            atField "out" (fun () ->
                match outField.Kind with
                | YamlNodeKind.Sequence collection ->
                    collection.Items
                    |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeStepOutputItem item.Node))
                    |> ResizeArray
                | _ ->
                    failAt outField ("Expected a list of outputs, found " + describeNode outField + "."))
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

    /// The cwlVersion of a process. A process below the top level takes the default of the scope, which is the version of
    /// the enclosing process or of the packed document. CWL ignores a cwlVersion field anywhere else than at the top level
    /// (concepts.md), so a field of such a process changes nothing, whatever it holds. The field of the root is required and
    /// a text.
    let versionDecoder (scope: DecodeScope) : (YamlNode -> string) =
        fun node ->
            match scope.DefaultCwlVersion with
            | Some defaultCwlVersion -> defaultCwlVersion
            | None -> Decode.object (fun get -> get.Required.Field "cwlVersion" (Decode.nonNull Decode.string)) node

    /// Decode a workflow step run as either a path string or an inline CWL processing unit.
    let rec workflowStepRunDecoder (warnings: WarningSink) (scope: DecodeScope) (runValue: YamlNode) : WorkflowStepRun =
        match runValue.Kind with
        | YamlNodeKind.Scalar _ ->
            // Most workflow steps reference an external CWL file by path. A step of a packed document may refer to a
            // process of the document by #id, and then the id must belong to one of its processes.
            let reference = Decode.nonNull Decode.string runValue
            match scope.GraphIds with
            | Some ids when reference.StartsWith "#" && not (Set.contains (LocalReference.withoutHash reference) ids) ->
                raise (
                    YamlDecodeException
                        { Message = "The run field refers to " + reference + ", which is no process of the $graph."
                          Path = []
                          Span = runValue.Span }
                )
            | _ -> RunString reference
        | YamlNodeKind.Mapping _ ->
            // Inline run objects are full CWL processing units and can themselves
            // contain tolerant ports/steps, so the same warning sink is threaded in.
            // An inline run without a cwlVersion takes the one of the scope. Its id is relative to the id of the step.
            // An inline run without an id has run as its segment.
            let runScope = { scope with DocId = processId scope.DocId "run" runValue }
            match decodeCWLProcessingUnitElementWithWarnings warnings runScope runValue with
            | CommandLineTool tool -> WorkflowStepRunOps.fromTool tool
            | Workflow workflow -> WorkflowStepRunOps.fromWorkflow workflow
            | ExpressionTool expressionTool -> WorkflowStepRunOps.fromExpressionTool expressionTool
            | Operation operation -> WorkflowStepRunOps.fromOperation operation
        | _ ->
            failAt runValue ("Expected a path or a process for the run field of a step, found " + describeNode runValue + ".")

    /// Decode a named workflow step and thread warnings into nested step inputs and inline runs.
    and decodeWorkflowStepFromValueWithId (warnings: WarningSink) (scope: DecodeScope) (path: string) (stepId: string) (value: YamlNode) : WorkflowStep =
        // Schema Salad gives the step its id behind the id of the workflow. The definitions of its requirements and
        // its inline run are relative to that id, and its SchemaDefRequirement overrides the one of the workflow.
        let scope = scope |> withChildId stepId |> enterHolder value
        // Required fields are decoded strictly because the step has an explicit id.
        let runValue = Decode.object (fun get' -> get'.Required.Field "run" (Decode.nonNull Decode.node)) value
        let run = atField "run" (fun () -> workflowStepRunDecoder warnings scope runValue)
        let inputs =
            Decode.object (fun get' ->
                get'.Required.Field "in" (inputStepDecoderWithWarnings warnings $"{path}.in")
            ) value
        // Optional fields are decoded independently so missing fields do not block
        // preservation of other step metadata.
        let outputs = outputStepsDecoder value
        let requirements = requirementsDecoderWithScope scope value
        let hints = hintsDecoderWithScope scope value
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

    and decodeWorkflowStepFromArrayItem (scope: DecodeScope) (item: YamlNode) : WorkflowStep =
        let stepId = stringFieldDecoder "id" item
        decodeWorkflowStepFromValueWithId None scope "steps[]" stepId item

    /// Decode one sequence-form workflow step, warning only for malformed unnamed entries.
    and decodeWorkflowStepFromArrayItemWithWarnings (warnings: WarningSink) (scope: DecodeScope) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        // An id marks the item as an intended step, so malformed required fields
        // should still fail instead of becoming warnings.
        | Some stepId -> Some (atField stepId (fun () -> decodeWorkflowStepFromValueWithId warnings scope $"steps[{index}]" stepId item))
        | None ->
            // Unnamed malformed sequence entries are skipped with location-aware warnings.
            addWarning warnings $"steps[{index}]" "Skipped malformed unnamed CWL workflow step entry." (Some item)
            None

    /// Decode workflow steps from sequence-form or map-form syntax with version-aware inline runs.
    and stepArrayDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<WorkflowStep>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form steps carry id in each item.
                rejectRepeatedIds (collection.Items |> List.map (fun item -> item.Node))
                collection.Items
                |> List.mapi (fun index item -> decodeWorkflowStepFromArrayItemWithWarnings warnings scope index item.Node)
                |> List.choose id
                |> ResizeArray
            | _ ->
                // Map-form steps use the map key as the step id.
                [|
                    for entry in mappingEntries value do
                        let key = entryName entry
                        atField key (fun () -> decodeWorkflowStepFromValueWithId warnings scope $"steps.{key}" key entry.Value)
                |]
                |> ResizeArray

    and stepsDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<WorkflowStep>) =
        Decode.object (fun get ->
            get.Required.Field "steps" (stepArrayDecoderWithScope warnings scope)
        )

    /// Decode a CommandLineTool and preserve unknown top-level metadata fields.
    and commandLineToolDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        // Decode standard sections first. Inputs and outputs are required.
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder yamlCWL scope with Process = Some CommandLineToolClass }
        // Only the ports of a command line tool may have a stream type.
        let outputs = outputsDecoderWithWarnings warnings { scope with StreamTypes = OutputStreams } yamlCWL
        let inputs = requiredInputsDecoder warnings { scope with StreamTypes = InputStreams } "a command line tool" yamlCWL
        // A stdin input reads the standard input, so the tool must not name a file for it as well.
        match tryGetPresentField "stdin" id yamlCWL with
        | Some stdinNode when inputs |> Seq.exists (fun input -> input.Type_ = Some Stdin) ->
            atField "stdin" (fun () ->
                raise (
                    YamlDecodeException
                        { Message = "The tool sets stdin and has an input of the type stdin. CWL admits only one of them."
                          Path = []
                          Span = stdinNode.Span }
                ))
        | _ -> ()
        let requirements = requirementsDecoderWithScope scope yamlCWL
        let hints = hintsDecoderWithScope scope yamlCWL
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
    and expressionToolDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        // ExpressionTool shares port/requirement metadata decoding with other
        // processing units, then decodes its required expression body.
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder yamlCWL scope with Process = Some ExpressionToolClass }
        let outputs = outputsDecoderWithWarnings warnings scope yamlCWL
        let inputs = requiredInputsDecoder warnings scope "an expression tool" yamlCWL
        let requirements = requirementsDecoderWithScope scope yamlCWL
        let hints = hintsDecoderWithScope scope yamlCWL
        let intent = intentDecoder yamlCWL
        let doc, label, sourceNodes = docAndLabelDecoder yamlCWL
        let expression =
            Decode.object (fun get -> get.Required.Field "expression" decodeSchemaSaladString) yamlCWL
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
    and operationDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder yamlCWL scope with Process = Some OperationClass }
        let outputs = outputsDecoderWithWarnings warnings scope yamlCWL
        let inputs = requiredInputsDecoder warnings scope "an operation" yamlCWL
        let requirements = requirementsDecoderWithScope scope yamlCWL
        let hints = hintsDecoderWithScope scope yamlCWL
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
    and workflowDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        // Workflow inputs and steps are required by this model; outputs and steps
        // can still use tolerant collection decoders internally.
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder yamlCWL scope with Process = Some WorkflowClass }
        let outputs = outputsDecoderWithWarnings warnings scope yamlCWL
        let inputs = requiredInputsDecoder warnings scope "a workflow" yamlCWL
        let requirements = requirementsDecoderWithScope scope yamlCWL
        let hints = hintsDecoderWithScope scope yamlCWL
        let intent = intentDecoder yamlCWL
        let steps = stepsDecoderWithScope warnings { scope with DefaultCwlVersion = Some cwlVersion } yamlCWL
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
    and decodeCWLProcessingUnitElementWithWarnings (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        let classNode = Decode.object (fun get -> get.Required.Field "class" (Decode.nonNull Decode.node)) yamlCWL
        let cls = Decode.string classNode
        match cls with
        // The class field is the only discriminator at this layer; individual
        // decoders handle version, ports, requirements, and metadata.
        | "CommandLineTool" -> CommandLineTool (commandLineToolDecoder warnings scope yamlCWL)
        | "Workflow" -> Workflow (workflowDecoder warnings scope yamlCWL)
        | "ExpressionTool" -> ExpressionTool (expressionToolDecoder warnings scope yamlCWL)
        | "Operation" -> Operation (operationDecoder warnings scope yamlCWL)
        | _ -> atField "class" (fun () -> failAt classNode ("Invalid or unsupported CWL class: " + cls))

    /// Decode an already-parsed CWL element without warning collection.
    let decodeCWLProcessingUnitElement (yamlCWL: YamlNode) =
        decodeCWLProcessingUnitElementWithWarnings None (documentScope yamlCWL) yamlCWL

    let stepArrayDecoder = stepArrayDecoderWithScope None { rootScope with DefaultCwlVersion = Some "v1.2" }

    let stepsDecoder = stepsDecoderWithScope None { rootScope with DefaultCwlVersion = Some "v1.2" }

    /// Decode a CWL CommandLineTool YAML string and return recoverable warnings.
    let decodeCommandLineToolWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = commandLineToolDecoder (Some warnings) (documentScope yamlCWL) yamlCWL; Warnings = warnings }

    /// Decode a CWL CommandLineTool and discard recoverable warnings.
    let decodeCommandLineTool (cwl: string) =
        (decodeCommandLineToolWithWarnings cwl).Value

    /// Decode a CWL Workflow YAML string and return recoverable warnings.
    let decodeWorkflowWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = workflowDecoder (Some warnings) (documentScope yamlCWL) yamlCWL; Warnings = warnings }

    /// Decode a CWL Workflow and discard recoverable warnings.
    let decodeWorkflow (cwl: string) =
        (decodeWorkflowWithWarnings cwl).Value

    /// Decode a CWL ExpressionTool YAML string and return recoverable warnings.
    let decodeExpressionToolWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = expressionToolDecoder (Some warnings) (documentScope yamlCWL) yamlCWL; Warnings = warnings }

    /// Decode a CWL ExpressionTool and discard recoverable warnings.
    let decodeExpressionTool (cwl: string) =
        (decodeExpressionToolWithWarnings cwl).Value

    /// Decode a CWL Operation YAML string and return recoverable warnings.
    let decodeOperationWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        { Value = operationDecoder (Some warnings) (documentScope yamlCWL) yamlCWL; Warnings = warnings }

    /// Decode a CWL Operation and discard recoverable warnings.
    let decodeOperation (cwl: string) =
        (decodeOperationWithWarnings cwl).Value

    /// Decode any CWL processing unit from a YAML string and return recoverable warnings. A packed document has no
    /// process at its root, so it is a decode error here, and decodeCWLDocument reads it.
    let decodeCWLProcessingUnitWithWarnings (cwl:string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        if hasField "$graph" yamlCWL then
            raise (
                YamlDecodeException
                    { Message = "The document is a packed document with a $graph and has no process at its root. Read it with decodeCWLDocument."
                      Path = []
                      Span = yamlCWL.Span }
            )
        { Value = decodeCWLProcessingUnitElementWithWarnings (Some warnings) (documentScope yamlCWL) yamlCWL; Warnings = warnings }

    /// Decode any CWL processing unit and discard recoverable warnings.
    let decodeCWLProcessingUnit (cwl:string) =
        (decodeCWLProcessingUnitWithWarnings cwl).Value

    /// The steps of a process that run another process of the document by #id, with the path of each run field and its
    /// node. The walk descends into the inline runs of the steps.
    let rec private runsIn (path: string list) (processNode: YamlNode) : (string list * string * YamlNode) list =
        stepsOf processNode
        |> List.collect (fun (stepId, step) ->
            match valueOf "run" step with
            | Some run ->
                let runPath = path @ [ "steps"; stepId; "run" ]
                match run.Kind with
                | YamlNodeKind.Scalar scalar when scalar.Value.StartsWith "#" -> [ runPath, LocalReference.withoutHash scalar.Value, run ]
                | YamlNodeKind.Mapping _ -> runsIn runPath run
                | _ -> []
            | None -> [])

    /// The chains of SchemaDefRequirements that reach each process of a packed document. A step passes its own chains to
    /// the process that it runs by #id, so a process has the chains of all steps that run it. The process main starts
    /// the document, so it has a chain without a fallback, as a root process has, and the processes that its steps reach
    /// inherit that. Any other process that no step runs has the fallback of every type of the document, because a
    /// runner may start with it. The walk reads the raw nodes with the same scope steps as the decoders, and it needs a
    /// document without cycles of run references.
    let private chainsOfProcesses (graphScope: DecodeScope) (processes: (string * YamlNode) list) (callers: Map<string, string list>) : Map<string, SchemaDefChain list> =
        let nodes = Map.ofList processes
        let everyType = { Ids = graphScope.Definitions; Imports = graphScope.ImportedPaths }
        let rec runsFrom (scope: DecodeScope) (processNode: YamlNode) : (string * SchemaDefChain list) list =
            let scope = enterHolder processNode scope
            stepsOf processNode
            |> List.collect (fun (stepId, step) ->
                let stepScope = scope |> withChildId stepId |> enterHolder step
                match valueOf "run" step with
                | Some run ->
                    match run.Kind with
                    | YamlNodeKind.Scalar scalar when scalar.Value.StartsWith "#" -> [ LocalReference.withoutHash scalar.Value, stepScope.Chains ]
                    | YamlNodeKind.Mapping _ -> runsFrom { stepScope with DocId = processId stepScope.DocId "run" run } run
                    | _ -> []
                | None -> [])
        let entries = System.Collections.Generic.Dictionary<string, SchemaDefChain list>()
        let sites = System.Collections.Generic.Dictionary<string, (string * SchemaDefChain list) list>()
        let rec chainsOf (id: string) : SchemaDefChain list =
            match entries.TryGetValue id with
            | true, chains -> chains
            | _ ->
                let fromRunner = if id = "main" then [ { Required = None; Hinted = None; Fallback = { Ids = Set.empty; Imports = Set.empty } } ] else []
                let fromSteps =
                    Map.tryFind id callers
                    |> Option.defaultValue []
                    |> List.collect (fun caller -> sitesOf caller |> List.filter (fun (target, _) -> target = id) |> List.collect snd)
                let chains =
                    match fromRunner @ fromSteps with
                    | [] -> [ { Required = None; Hinted = None; Fallback = everyType } ]
                    | found -> List.distinct found
                entries.[id] <- chains
                chains
        and sitesOf (id: string) : (string * SchemaDefChain list) list =
            match sites.TryGetValue id with
            | true, found -> found
            | _ ->
                let node = nodes.[id]
                let found = runsFrom { graphScope with DocId = processId "" "" node; Chains = chainsOf id } node
                sites.[id] <- found
                found
        processes |> List.map (fun (id, _) -> id, chainsOf id) |> Map.ofList

    /// Decode a packed document. Its processes use the cwlVersion of the root and ignore their own, and a step names a
    /// process of the document by #id. The scope of each process holds the ids and the type definitions of the whole
    /// document, because a workflow may run a process that stands behind it in the list and a process may name a type
    /// that another one defines by its id, as main/Point or #main/Point. Which of these types apply to a process
    /// follows from the steps that run it. A process takes the SchemaDefRequirements in force at each of those steps,
    /// and the process main takes none, as a root process does. A process that no step runs and that is not main takes
    /// every type of the document.
    let decodeCWLGraphElementWithWarnings (warnings: WarningSink) (yamlCWL: YamlNode) : CWLGraph =
        let cwlVersion = versionDecoder rootScope yamlCWL
        // The root of a packed document holds no process, so a class there contradicts the $graph.
        match entriesOf yamlCWL |> List.tryFind (fun entry -> scalarText entry.Key = Some "class") with
        | Some classEntry ->
            raise (
                YamlDecodeException
                    { Message = "The document has a $graph, so it has no process at its root and no class field."
                      Path = []
                      Span = classEntry.Key.Span }
            )
        | None -> ()
        let graphNode = Decode.object (fun get -> get.Required.Field "$graph" (Decode.nonNull Decode.node)) yamlCWL
        let processNodes =
            match graphNode.Kind with
            | YamlNodeKind.Sequence _ -> itemsOf graphNode
            | _ ->
                atField "$graph" (fun () ->
                    raise (
                        YamlDecodeException
                            { Message = "Expected a list of processes, found " + describeNode graphNode + "."
                              Path = []
                              Span = graphNode.Span }
                    ))
        // Every process needs an id that no other process of the document has, because a step names a process by its id.
        let idList =
            let seen = System.Collections.Generic.HashSet<string>()
            processNodes
            |> List.mapi (fun index node ->
                let failAt (message: string) (at: YamlNode) : 'T =
                    atField "$graph" (fun () ->
                        atField $"[{index}]" (fun () -> raise (YamlDecodeException { Message = message; Path = []; Span = at.Span })))
                match valueOf "id" node with
                | Some idNode ->
                    match scalarText idNode with
                    | Some id when id <> "" ->
                        let id = LocalReference.withoutHash id
                        if not (seen.Add id) then
                            failAt ("The id " + id + " belongs to an earlier process of the document too. An id must be unique in the document.") idNode
                        id
                    | _ -> failAt ("Expected the id of a process, found " + describeNode idNode + ".") idNode
                | None -> failAt "Expected an id field. Every process of a packed document needs one." node)
        let ids = Set.ofList idList
        // A run that leads back to a process on its way never ends. The run field that closes the cycle is the error.
        let members = List.zip idList processNodes |> List.mapi (fun index (id, node) -> id, (index, node)) |> Map.ofList
        let visiting = System.Collections.Generic.Dictionary<string, bool>()
        let rec visit (stack: string list) (id: string) : unit =
            visiting.[id] <- true
            let index, processNode = members.[id]
            for (runPath, target, runNode) in runsIn [] processNode do
                if members.ContainsKey target then
                    match visiting.TryGetValue target with
                    | true, true ->
                        let cycle = (id :: stack) |> List.rev |> List.skipWhile (fun step -> step <> target)
                        let names = (cycle @ [ target ]) |> List.map (fun name -> "#" + name) |> String.concat ", "
                        let raiseAt () =
                            raise (
                                YamlDecodeException
                                    { Message = "The run field closes a cycle of run references (" + names + "). A process cannot run itself."
                                      Path = []
                                      Span = runNode.Span }
                            )
                        (List.foldBack (fun segment (inner: unit -> unit) -> (fun () -> atField segment inner)) ([ "$graph"; $"[{index}]" ] @ runPath) raiseAt) ()
                    | true, false -> ()
                    | _ -> visit (id :: stack) target
            visiting.[id] <- false
        for id in idList do
            if not (visiting.ContainsKey id) then visit [] id
        let graphScope = { documentScope yamlCWL with DefaultCwlVersion = Some cwlVersion; GraphIds = Some ids; DocId = "" }
        // A process takes the types in force at each step that runs it, so the steps that run it are known before it decodes.
        let callers =
            idList
            |> List.collect (fun id -> runsIn [] (snd members.[id]) |> List.map (fun (_, target, _) -> target, id))
            |> List.filter (fun (target, _) -> members.ContainsKey target)
            |> List.groupBy fst
            |> List.map (fun (target, runs) -> target, runs |> List.map snd |> List.distinct)
            |> Map.ofList
        let chains = chainsOfProcesses graphScope (List.zip idList processNodes) callers
        let processes =
            List.zip idList processNodes
            |> List.mapi (fun index (id, node) ->
                atField "$graph" (fun () ->
                    atField $"[{index}]" (fun () ->
                        decodeCWLProcessingUnitElementWithWarnings warnings { graphScope with DocId = processId "" "" node; Chains = chains.[id] } node)))
            |> ResizeArray
        let graph = CWLGraph(processes, cwlVersion = cwlVersion)
        overflowIntoDynamicObj graph (CWLGraph.KnownFieldNames |> Seq.toList) yamlCWL |> ignore
        graph

    /// Decode a CWL document, which is a process or a packed document, from a YAML string and return recoverable
    /// warnings.
    let decodeCWLDocumentWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        let document =
            if hasField "$graph" yamlCWL then
                CWLDocument.Graph (decodeCWLGraphElementWithWarnings (Some warnings) yamlCWL)
            else
                CWLDocument.Process (decodeCWLProcessingUnitElementWithWarnings (Some warnings) (documentScope yamlCWL) yamlCWL)
        { Value = document; Warnings = warnings }

    /// Decode a CWL document, which is a process or a packed document, and discard recoverable warnings.
    let decodeCWLDocument (cwl: string) =
        (decodeCWLDocumentWithWarnings cwl).Value

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

        // A parameter file has no schema around its types, so they read like the type text of an RO-Crate entity.
        let decodeType (typeElement: YamlNode) =
            match typeElement.Kind with
            | YamlNodeKind.Scalar _ -> Decode.cwlTypeStringDecoder Decode.typeTextScope typeElement
            | _ -> Decode.cwlTypeDecoderAt Decode.typeTextScope typeElement

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
