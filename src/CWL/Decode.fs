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

    /// Schema Salad v1.2 (section 2.2, Syntax) restricts a document to the JSON-compatible subset of YAML 1.2. It prohibits explicit
    /// node tags, anchors, aliases and the %YAML and %TAG directives, and a merge key is no YAML 1.2 feature, so << is an ordinary
    /// key. cwltool reads the prohibited features, and ARCtrl follows the specification. The specification names the documents
    /// only, and ARCtrl applies the rule to a job file too.
    let private prohibitedMessage (feature: string) (rule: string) : string =
        feature + " is not allowed. Schema Salad v1.2 prohibits " + rule + " in a document."

    /// Fail with a decode error at the place of a prohibited feature.
    let private failProhibited (path: string list) (span: SourceSpan option) (message: string) : 'T =
        raise (YamlDecodeException { Message = message; Path = path; Span = span })

    /// Fail at an alias. The decoders that match every kind of node reach this arm only for a tree that did not pass the
    /// check of rejectWhatTheSpecProhibits.
    let private failAliasAt (path: string list) (name: string) (node: YamlNode) : 'T =
        failProhibited path node.Span (prohibitedMessage ("The alias *" + name) "anchors and aliases")

    let internal failAlias (name: string) (node: YamlNode) : 'T =
        failAliasAt [] name node

    /// Fail with a decode error at the place of a node. The callers that read the node inside a field add the path of the
    /// field with atField.
    let private failAt (node: YamlNode) (message: string) : 'T =
        raise (YamlDecodeException { Message = message; Path = []; Span = node.Span })

    /// The place of a node in the text, as it stands in the message of an error.
    let private placeOf (node: YamlNode) : string =
        match node.Span with
        | Some span -> "line " + string span.Start.Line + ", column " + string span.Start.Column
        | None -> "an unknown place"

    /// The type that the YAML 1.2 core schema gives a scalar node. This is the rule behind the quoting of
    /// Encode.string, so a decoder has to read the plain spellings the same way. Quoted and block scalars
    /// are text, and an explicit tag fixes the type. A node that is no scalar counts as text. The core schema sets no
    /// size limit for an int, so an int beyond 64 bits is a case of its own that holds the text of the scalar, and the
    /// field that takes it converts it (CoreSchema.integerToFloat).
    let internal (|CoreNull|CoreBool|CoreInt|CoreBigInt|CoreFloat|CoreText|) (node: YamlNode) =
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
                    // tryParseInt64 gives none for an int beyond 64 bits as it does for a text that is no int, and
                    // tryParseFloat reads only a decimal int beyond 64 bits, so resolveKind tells them apart.
                    | None when tag.IsNone && scalar.Style = ScalarStyle.Plain && CoreSchema.resolveKind scalar.Value = CoreKind.Integer ->
                        CoreBigInt scalar.Value
                    | None ->
                        match CoreSchema.tryParseFloat tag scalar with
                        | Some parsed -> CoreFloat parsed
                        | None -> CoreText
        | _ -> CoreText

    /// What a plain scalar is when the core schema reads it as null, a bool, an int or a float, as it stands in a message.
    /// A quoted scalar, a block scalar and a scalar under !!str are strings, and so is any node that is no scalar.
    let private notAString (node: YamlNode) : string option =
        let plain (typeName: string) = Some ("the plain scalar " + Decode.string node + ", which the core schema reads as " + typeName)
        match node with
        | CoreNull -> Some "null"
        | CoreBool _ -> plain "a bool"
        | CoreInt _
        | CoreBigInt _ -> plain "an int"
        | CoreFloat _ -> plain "a float"
        | CoreText -> None

    /// What a node is, as it stands in the message of an error: a scalar by the type that the core schema gives it, so a plain
    /// float is no text, and any other node by its kind.
    let private describeNode (node: YamlNode) : string =
        match node.Kind with
        | YamlNodeKind.Scalar scalar ->
            match notAString node with
            | Some found -> found
            | None -> "the text \"" + scalar.Value + "\""
        | YamlNodeKind.Sequence _ -> "a list"
        | YamlNodeKind.Mapping _ -> "a mapping"
        | YamlNodeKind.Alias name -> "the alias *" + name

    /// The text of a tag as a document writes it: !! for a core tag, ! for a local tag and !<...> for a verbatim tag.
    let private tagText (tag: YamlTag) : string =
        match tag with
        | YamlTag.NonSpecific -> "!"
        | YamlTag.Named name ->
            if name.Prefix = YamlTag.coreNamespace then "!!" + name.Suffix
            elif name.Prefix = "!" then "!" + name.Suffix
            else "!<" + name.Prefix + name.Suffix + ">"

    /// The place of the tag or the anchor of a node in the text, or the place of the node when the tree does not know it.
    let private propertySpan (node: YamlNode) (property: PropertyName) : SourceSpan option =
        node.PropertyLayout
        |> List.tryPick (fun placement -> if placement.Property = property then placement.Span else None)
        |> Option.orElse node.Span

    /// Fail with the first anchor, alias, explicit tag, repeated scalar key and $namespaces field that holds no mapping in the
    /// tree. Schema Salad v1.2 prohibits the first three (see prohibitedMessage). Every anchor counts, also one that no alias
    /// names, and so does every explicit tag, also the non-specific tag !. Raw values keep their entries as they were written,
    /// so a repeated key would be written back twice, and YAML 1.2 asks for unique keys. cwltool refuses a $namespaces that is
    /// no mapping in any object of a document, and in a job file too. A mapping with a $mixin and everything below it is exempt
    /// from the $namespaces check. Schema Salad merges the fields beside a $mixin into the data of the file only when that
    /// data is a mapping, so the fields drop out when the file holds a scalar or a list, and the verdict depends on the file.
    /// The other checks apply there. The walk keeps its own stack, so a raw value of any depth is safe. It checks the keys and
    /// the values of a mapping and the items of a sequence.
    let internal rejectWhatTheSpecProhibits (root: YamlNode) : unit =
        // The nodes that are still to visit, each with the path to it and the flag that tells whether a $mixin is above
        // the node. The path is built backwards.
        let pending = ResizeArray<YamlNode * string list * bool>()
        pending.Add((root, [], false))
        while pending.Count > 0 do
            let (node, reversedPath, belowMixin) = pending.[pending.Count - 1]
            pending.RemoveAt(pending.Count - 1)
            match node.Properties.Anchor with
            | Some name -> failProhibited (List.rev reversedPath) (propertySpan node PropertyName.Anchor) (prohibitedMessage ("The anchor &" + name) "anchors and aliases")
            | None -> ()
            match node.Properties.Tag with
            | Some tag -> failProhibited (List.rev reversedPath) (propertySpan node PropertyName.Tag) (prohibitedMessage ("The tag " + tagText tag) "explicit node tags")
            | None -> ()
            match node.Kind with
            | YamlNodeKind.Alias name -> failAliasAt (List.rev reversedPath) name node
            | YamlNodeKind.Scalar _ -> ()
            | YamlNodeKind.Sequence collection ->
                // The last item goes in first, so the first item comes out first.
                collection.Items
                |> List.mapi (fun index item -> item.Node, ("[" + string index + "]") :: reversedPath, belowMixin)
                |> List.rev
                |> List.iter pending.Add
            | YamlNodeKind.Mapping collection ->
                let seen = System.Collections.Generic.Dictionary<string, YamlNode>()
                let children = ResizeArray<YamlNode * string list * bool>()
                let holdsMixin =
                    collection.Items
                    |> List.exists (fun entry ->
                        match entry.Key.Kind with
                        | YamlNodeKind.Scalar scalar -> scalar.Value = "$mixin"
                        | _ -> false)
                let exemptFromNamespaces = belowMixin || holdsMixin
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
                                if scalar.Value = "$namespaces" && not exemptFromNamespaces then
                                    match entry.Value.Kind with
                                    | YamlNodeKind.Mapping _
                                    | YamlNodeKind.Alias _ -> ()
                                    | _ ->
                                        raise (
                                            YamlDecodeException
                                                { Message = "The field $namespaces takes a mapping of prefixes to IRIs, found " + describeNode entry.Value + "."
                                                  Path = List.rev (scalar.Value :: reversedPath)
                                                  Span = entry.Value.Span }
                                        )
                                scalar.Value
                        | _ -> "?" + string index
                    children.Add((entry.Key, name :: reversedPath, exemptFromNamespaces))
                    children.Add((entry.Value, name :: reversedPath, exemptFromNamespaces)))
                for index in children.Count - 1 .. -1 .. 0 do
                    pending.Add children.[index]

    /// Fail with the first %YAML or %TAG directive of the document, which Schema Salad prohibits. A reserved directive stays
    /// legal, because the specification names no other directive. The reader keeps the directives in the prelude of the
    /// document, and Reader.readRoot drops the prelude, so the text entry point reads the whole document.
    let private rejectDirectives (document: YamlDocument) : unit =
        for item in document.Prelude do
            match item with
            | PreludeItem.Directive directive ->
                match directive.Kind with
                | DirectiveKind.YamlVersion (major, minor) ->
                    failProhibited [] directive.Span (prohibitedMessage ("The directive %YAML " + string major + "." + string minor) "%YAML and %TAG directives")
                | DirectiveKind.TagDirective (handle, prefix) ->
                    failProhibited [] directive.Span (prohibitedMessage ("The directive %TAG " + handle + " " + prefix) "%YAML and %TAG directives")
                | DirectiveKind.ReservedDirective _ -> ()
            | PreludeItem.PreludeTrivia _ -> ()

    /// The message of an error of the reader without the line that it starts with, because the span of the decode error shows
    /// the place.
    let private withoutLinePrefix (message: string) : string =
        System.Text.RegularExpressions.Regex.Replace(message, "^Line [0-9]+: ", "")

    /// True for the errors of the reader that name an alias, such as an alias without an anchor in front of it.
    let private isAliasError (code: YamlErrorCode) : bool =
        match code with
        | YamlErrorCode.UndefinedAlias
        | YamlErrorCode.EmptyAlias
        | YamlErrorCode.AliasWithProperties -> true
        | _ -> false

    /// Read the root node of a CWL document. The text must hold one document that follows the restrictions of Schema Salad: no
    /// %YAML or %TAG directive, no explicit tag, no anchor, no alias and no repeated key (see rejectWhatTheSpecProhibits). A
    /// merge key is no YAML 1.2 feature, so << is an ordinary key. Every text entry point reads through this function, also the
    /// one of the job files and of the additional type of an RO-Crate. A decoder that takes a tree has its own entry point, which
    /// applies rejectWhatTheSpecProhibits to the tree. An alias without an anchor fails in the reader before a tree exists, so
    /// its error stands at the root of the document.
    let readYaml (yaml: string) : YamlNode =
        let document =
            match Reader.tryRead yaml with
            | Ok document -> document
            | Error error when isAliasError error.Code -> failProhibited [] (Some error.Span) (withoutLinePrefix error.Message)
            | Error error -> raise (YamlParseException error)
        rejectDirectives document
        rejectWhatTheSpecProhibits document.Root
        document.Root

    /// Box int64 values in a way that preserves 64-bit numeric intent across Fable targets.
    let boxOverflowInt64 (value: int64) : obj =
#if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyExpr value "int64($0)"
#else
        box value
#endif

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

    /// Run a decoder with the segments of a path in front of the path of its errors.
    let private atPath (path: string list) (decode: unit -> 'T) : 'T =
        List.foldBack (fun segment inner -> (fun () -> atField segment inner)) path decode ()

    /// The text of a CWL string field. CWL types these fields as strings, so a plain scalar that the core schema reads
    /// as null, a bool, an int or a float is a decode error at the field, as cwltool reports it. Every other scalar is
    /// a string, for example a quoted one or one tagged !!str. A collection and an alias are errors too.
    let stringDecoder (node: YamlNode) : string =
        match notAString node with
        | Some found ->
            raise (
                YamlDecodeException
                    { Message = "Expected a string, found " + found + ". Write it in quotes to make it a string."
                      Path = []
                      Span = node.Span }
            )
        | None -> Decode.string node

    /// The id that the key of an entry gives its value in an id map, such as inputs, outputs, steps and requirements in map
    /// form. CWL types the id as a string, so a plain key that the core schema reads as null, a bool, an int or a float
    /// is a decode error at the key.
    let idMapKey (entry: MappingEntry) : string =
        stringDecoder entry.Key

    /// Decode key value pairs into a dynamic object, while preserving their tree structure.
    let rec overflowDecoder (dynObj: DynamicObj) (entries: MappingEntry list) =
        let decodeOverflowScalar (node: YamlNode) : obj =
            match node with
            | CoreBool parsed -> box parsed
            | CoreInt parsed -> boxOverflowInt64 parsed
            | CoreFloat parsed -> box parsed
            | CoreNull
            | CoreBigInt _ ->
                // A null has no value to hold in the object, and neither has an int beyond 64 bits. The YAML node keeps
                // the spelling and the type, and the encoder writes it back as it was.
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
        tryGetPresentField fieldName stringDecoder value

    let tryGetYamlField (fieldName: string) (value: YamlNode) : RawYaml option =
        tryGetPresentField fieldName (fun node -> { Node = node }) value

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

    /// The text of a scalar node, and none for any other node.
    let private scalarTextOf (node: YamlNode) : string option =
        match node.Kind with
        | YamlNodeKind.Scalar scalar -> Some scalar.Value
        | _ -> None

    /// What a node is at a place that reads a Schema Salad directive.
    type private DirectiveReading =
        /// A node without a directive key. The caller decides what the node is.
        | NotDirective
        /// A directive on its own, which names a file.
        | DirectiveRead of SchemaSaladString
        /// A $mixin beside other fields, with the path of its file. The file completes the object that the fields make.
        | MixinBeside of string
        /// A mapping with a directive key that is no directive: the path to the place of the problem inside the node, the
        /// message, and the node to point at.
        | Malformed of string list * string * YamlNode

    /// Read a node as a Schema Salad directive. A directive is a mapping with exactly one field, which is one of the
    /// three directive names, and the path of a file as its value. $import takes the data of the file, and $include takes
    /// its text.
    /// $mixin takes the data of the file like $import and joins it with the fields beside the directive, so it may stand
    /// beside other fields. Schema Salad resolves the ids of a mixin against the document that holds it, and those of an
    /// import against the imported file, so they are separate kinds. The reader is pure, so the scans of the raw nodes can
    /// use it.
    let private readDirective (node: YamlNode) : DirectiveReading =
        match node.Kind with
        | YamlNodeKind.Mapping collection ->
            let keyOf (entry: MappingEntry) = scalarTextOf entry.Key
            let directiveEntries = collection.Items |> List.filter (fun entry -> List.contains (keyOf entry |> Option.defaultValue "") [ "$import"; "$include"; "$mixin" ])
            let pathOf (entry: MappingEntry) (key: string) : DirectiveReading =
                match entry.Value.Kind with
                | YamlNodeKind.Scalar _ ->
                    match notAString entry.Value with
                    | Some found ->
                        Malformed ([ key ], "Expected the path of the " + key + " directive to be a string, found " + found + ". Write it in quotes to make it a string.", entry.Value)
                    | None ->
                        let path = Decode.string entry.Value
                        if path.Trim() = "" then
                            Malformed ([ key ], "The path of the " + key + " directive is empty.", entry.Value)
                        elif key = "$include" then
                            DirectiveRead (SchemaSaladString.Include path)
                        elif key = "$mixin" then
                            DirectiveRead (SchemaSaladString.Mixin path)
                        else
                            DirectiveRead (SchemaSaladString.Import path)
                | _ -> Malformed ([ key ], "Expected the path of the " + key + " directive to be a string, found " + describeNode entry.Value + ".", entry.Value)
            match directiveEntries, collection.Items with
            | [], _ -> NotDirective
            | [ entry ], [ _ ] ->
                match keyOf entry with
                | Some key -> pathOf entry key
                | None -> NotDirective
            | [ entry ], _ when keyOf entry = Some "$mixin" ->
                match pathOf entry "$mixin" with
                | DirectiveRead mixinDirective -> MixinBeside mixinDirective.Value
                | malformed -> malformed
            | _ ->
                let names = collection.Items |> List.map (fun entry -> keyOf entry |> Option.defaultValue "?") |> String.concat ", "
                Malformed ([], "A directive has exactly one field, but this mapping has the fields " + names + ".", node)
        | _ -> NotDirective

    /// Raise the problem of a malformed directive at its place.
    let private failMalformed (path: string list) (message: string) (at: YamlNode) : 'T =
        atPath path (fun () -> failAt at message)

    /// True when the node is a mapping with a directive key, whether the directive is well-formed or not. The scans of the
    /// raw nodes use it, and they leave the error of a malformed directive to the decoder of the place.
    let private hasDirective (node: YamlNode) : bool =
        match readDirective node with
        | NotDirective -> false
        | _ -> true

    /// True when a field of the node holds a directive, such as the class of a requirement or the type of a schema. The file
    /// decides what the class or the type is, so the decoder cannot build the typed object, and the caller keeps the node
    /// as it was written. A malformed directive is an error at the field.
    let private fieldIsDirective (field: string) (node: YamlNode) : bool =
        let fieldValue =
            match node.Kind with
            | YamlNodeKind.Mapping collection -> collection.Items |> List.tryPick (fun entry -> if scalarTextOf entry.Key = Some field then Some entry.Value else None)
            | _ -> None
        match fieldValue with
        | Some value ->
            atField field (fun () ->
                match readDirective value with
                | NotDirective -> false
                | Malformed (path, message, at) -> failMalformed path message at
                | _ -> true)
        | None -> false

    /// Read a directive at a place that takes a string. The directive stays a case of SchemaSaladString, so the encoder
    /// writes the mapping again. A $mixin beside other fields stays as it was written, because the file decides whether
    /// the fields count. A mapping with a directive key that is no directive is an error, and a node without a
    /// directive key is none.
    let tryDirective (node: YamlNode) : SchemaSaladString option =
        match readDirective node with
        | NotDirective -> None
        | DirectiveRead directive -> Some directive
        | MixinBeside _ -> Some (SchemaSaladString.Mixed { Node = node })
        | Malformed (path, message, at) -> failMalformed path message at

    /// Read a directive at a place that takes the whole of a list. Only $import can stand there, because an $include
    /// yields the text of a file and a $mixin completes an object with the fields beside it. The result is the path of
    /// the imported file, and none for a node without a directive key. The text names what the place takes, as in the
    /// types field.
    let importOnly (what: string) (node: YamlNode) : string option =
        match readDirective node with
        | NotDirective -> None
        | DirectiveRead (SchemaSaladString.Import path) -> Some path
        | DirectiveRead (SchemaSaladString.Mixin _)
        | MixinBeside _ -> failAt node ("A $mixin completes an object with the fields beside it, and " + what + " is no object. Write an $import for a file that holds it.")
        | DirectiveRead _ -> failAt node ("An $include yields the text of a file, but " + what + " takes an object. Write an $import for a file that holds the object.")
        | Malformed (path, message, at) -> failMalformed path message at

    /// What a mapping with a directive stands for at a place that takes an object or a scalar that is no string.
    type ValueDirective =
        /// An $import of a file that holds the value, or a list of values where the place takes an object.
        | ImportedFile of string
        /// A $mixin, alone or beside the fields that complete the object. The mapping is kept as it was written, because
        /// the file may supply fields that the model cannot leave out.
        | MixedMapping of RawYaml

    /// Read a directive at a place whose value a file can give as data or as text, such as the name of a symbol or an
    /// expression. An $import and a $mixin take the data of the file, and an $include takes its text. The constructors
    /// of the place build the result for each of the three, and the mixin keeps its mapping. The result is none for a node
    /// without a directive key.
    let private textDirective (imported: string -> 'T) (included: string -> 'T) (mixed: RawYaml -> 'T) (node: YamlNode) : 'T option =
        match readDirective node with
        | NotDirective -> None
        | DirectiveRead (SchemaSaladString.Import path) -> Some (imported path)
        | DirectiveRead (SchemaSaladString.Include path) -> Some (included path)
        | DirectiveRead _
        | MixinBeside _ -> Some (mixed { Node = node })
        | Malformed (path, message, at) -> failMalformed path message at

    /// Read a directive at a place that takes an object or a scalar that is no string. An $import and a $mixin can stand
    /// there, because an $include yields the text of a file. The result is none for a node without a directive key. The
    /// first text names the place, as in an entry of the types field. The other two name the value as the message needs
    /// them, as in an object and the object.
    let private valueDirective (what: string) (taking: string) (holding: string) (node: YamlNode) : ValueDirective option =
        textDirective
            ImportedFile
            (fun _ -> failAt node ("An $include yields the text of a file, but " + what + " takes " + taking + ". Write an $import for a file that holds " + holding + "."))
            MixedMapping
            node

    /// Read a directive at a place that takes an object, as in an entry of the types field.
    let objectDirective (what: string) (node: YamlNode) : ValueDirective option =
        valueDirective what "an object" "the object" node

    /// Decode a boolean field. A directive stands for the boolean of a file. The text names what the place is, as in the
    /// loadContents field.
    let schemaSaladBoolDecoder (what: string) (node: YamlNode) : SchemaSaladBool =
        match valueDirective what "a boolean" "the boolean" node with
        | Some (ImportedFile path) -> SchemaSaladBool.Imported path
        | Some (MixedMapping raw) -> SchemaSaladBool.Mixed raw
        | None -> SchemaSaladBool.Value (Decode.bool node)

    /// Decode the boolean field of a node that may be a scalar shorthand. A node that is no mapping has no fields, and a
    /// field with a null value counts as missing.
    let tryGetBoolField (fieldName: string) (value: YamlNode) : SchemaSaladBool option =
        tryGetPresentField fieldName (schemaSaladBoolDecoder ("the " + fieldName + " field")) value

    /// Decode the size of a File through the core schema, so that the size does not narrow to int and a quoted size
    /// fails like the other typed fields. A directive stands for the size of a file.
    let fileSizeDecoder (node: YamlNode) : FileSize =
        match valueDirective "the size field" "a number" "the number" node with
        | Some (ImportedFile path) -> FileSize.Imported path
        | Some (MixedMapping raw) -> FileSize.Mixed raw
        | None -> FileSize.Bytes (Decode.int64 node)

    /// Decode the name of a symbol of an enum, or a directive that stands for it. An $include yields the text of a file,
    /// which is the name, so it can stand there. The function tells the message for a name that is no symbol of the enum. A
    /// plain scalar that the core schema reads as something other than text is no symbol either, and quotes would
    /// not make it one, so the same message tells it.
    let private symbolDecoder
        (tryParse: string -> 'T option)
        (unknown: YamlNode -> string -> string)
        (imported: string -> 'T)
        (included: string -> 'T)
        (mixed: RawYaml -> 'T)
        (node: YamlNode)
        : 'T =
        match textDirective imported included mixed node with
        | Some directive -> directive
        | None ->
            match node.Kind, node with
            | YamlNodeKind.Scalar scalar, (CoreBool _ | CoreInt _ | CoreBigInt _ | CoreFloat _) -> failAt node (unknown node scalar.Value)
            | _ ->
                let text = stringDecoder node
                match tryParse text with
                | Some symbol -> symbol
                | None -> failAt node (unknown node text)

    /// Decode a loadListing value from its YAML string form, or the directive that stands for it.
    let loadListingDecoder : YamlNode -> LoadListingEnum =
        symbolDecoder
            LoadListingEnum.tryParse
            (fun node _ -> "Expected one of no_listing, shallow_listing and deep_listing, found " + describeNode node + ".")
            ImportedLoadListing
            IncludedLoadListing
            MixedLoadListing

    /// Decode and validate CWL loadListing enum fields from their YAML string form.
    let tryGetLoadListingField (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName loadListingDecoder value

    /// Decode a string field that may hold a Schema Salad directive. A scalar is the literal text. A mapping with a
    /// directive as its only field names the file whose text or data takes the place of the mapping.
    let decodeSchemaSaladString (node: YamlNode) : SchemaSaladString =
        let unexpected () : SchemaSaladString =
            failAt node ("Expected a string or a directive, found " + describeNode node + ".")
        match node.Kind with
        | YamlNodeKind.Scalar _ ->
            // Common path: scalar values become literal schema-salad strings. YAMLicious folds the
            // line breaks of a multi-line plain scalar into spaces, as YAML 1.2.2 requires.
            SchemaSaladString.Literal (Decode.nonNull stringDecoder node)
        | YamlNodeKind.Mapping _ ->
            match tryDirective node with
            | Some directive -> directive
            | None -> unexpected ()
        | _ -> unexpected ()

    /// Decode File object fields while preserving unrecognized extension metadata.
    let decodeFileInstanceFields (element: YamlNode) =
        let file =
            FileInstance(
                ?location = tryGetPresentField "location" decodeSchemaSaladString element,
                ?path = tryGetPresentField "path" decodeSchemaSaladString element,
                ?basename = tryGetPresentField "basename" decodeSchemaSaladString element,
                ?dirname = tryGetPresentField "dirname" decodeSchemaSaladString element,
                ?nameroot = tryGetPresentField "nameroot" decodeSchemaSaladString element,
                ?nameext = tryGetPresentField "nameext" decodeSchemaSaladString element,
                ?checksum = tryGetPresentField "checksum" decodeSchemaSaladString element,
                ?size = tryGetPresentField "size" fileSizeDecoder element,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" element,
                ?format = tryGetPresentField "format" decodeSchemaSaladString element,
                ?contents = tryGetPresentField "contents" decodeSchemaSaladString element
            )
        overflowIntoDynamicObj file (FileInstance.KnownFieldNames |> Seq.toList) element |> ignore
        file

    /// Decode Directory object fields while preserving unrecognized extension metadata.
    let decodeDirectoryInstanceFields (element: YamlNode) =
        let directory =
            DirectoryInstance(
                ?location = tryGetPresentField "location" decodeSchemaSaladString element,
                ?path = tryGetPresentField "path" decodeSchemaSaladString element,
                ?basename = tryGetPresentField "basename" decodeSchemaSaladString element,
                ?listing = tryGetYamlField "listing" element
            )
        overflowIntoDynamicObj directory (DirectoryInstance.KnownFieldNames |> Seq.toList) element |> ignore
        directory

    /// Decode a list of schema-salad strings.
    let decodeSchemaSaladStringArray (value: YamlNode) : ResizeArray<SchemaSaladString> =
        match value.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeSchemaSaladString item.Node))
            |> ResizeArray
        | _ ->
            failAt value ("Expected a list of strings, found " + describeNode value + ".")

    /// Read a directive that stands for the whole of a list of strings. An $import takes the list of a file, and a $mixin
    /// replaces the value with the content of the file, so both can stand there. An $include yields the text of a file,
    /// which is no list. The result is none for a node without a directive key.
    let private wholeListDirective (what: string) (items: string) (node: YamlNode) : SchemaSaladString option =
        match readDirective node with
        | NotDirective -> None
        | DirectiveRead (SchemaSaladString.Include _) ->
            failAt node ("An $include yields the text of a file, but " + what + " takes a list of " + items + ". Write the $include as an entry of the list.")
        | DirectiveRead directive -> Some directive
        | MixinBeside _ -> Some (SchemaSaladString.Mixed { Node = node })
        | Malformed (path, message, at) -> failMalformed path message at

    /// Decode a field that holds a list of strings and nothing else. A directive on its own stands for the whole list when
    /// it is an $import or a $mixin, and it is the one entry of the result.
    let decodeSchemaSaladStringList (what: string) (node: YamlNode) : ResizeArray<SchemaSaladString> =
        match node.Kind with
        | YamlNodeKind.Mapping _ ->
            match wholeListDirective what "strings" node with
            | Some directive -> ResizeArray [ directive ]
            | None -> failAt node ("Expected a list of strings, found " + describeNode node + ".")
        | _ -> decodeSchemaSaladStringArray node

    /// Decode an entry of a list of numbers. An $import stands for the number or the numbers of a file, and a $mixin keeps
    /// the mapping as written. An $include yields the text of a file, which is no number.
    let private intEntryDecoder (what: string) (node: YamlNode) : ListEntry<int> =
        match valueDirective what "a number" "the number" node with
        | Some (ImportedFile path) -> ListEntry.Imported path
        | Some (MixedMapping raw) -> ListEntry.Mixed raw
        | None -> ListEntry.Value (Decode.int node)

    /// Decode a field that holds a list of numbers. A directive on its own stands for the whole list when it is an $import
    /// or a $mixin, and it is the one entry of the result.
    let intListDecoder (what: string) (node: YamlNode) : ResizeArray<ListEntry<int>> =
        match node.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> intEntryDecoder ("an entry of " + what) item.Node))
            |> ResizeArray
        | YamlNodeKind.Mapping _ ->
            match wholeListDirective what "numbers" node with
            | Some (SchemaSaladString.Import path) -> ResizeArray [ ListEntry.Imported path ]
            | Some _ -> ResizeArray [ ListEntry.Mixed { Node = node } ]
            | None -> failAt node ("Expected a list of numbers, found " + describeNode node + ".")
        | _ -> failAt node ("Expected a list of numbers, found " + describeNode node + ".")

    let tryGetIntListField (fieldName: string) (value: YamlNode) =
        tryGetPresentField fieldName (intListDecoder ("the " + fieldName + " field")) value

    /// Decode a field that holds a string or a list of strings, such as the scatter. A string is one entry. A directive on
    /// its own stands for the string or the list of strings of a file, so it is one entry whichever directive it is.
    let stringsOrOneDecoder (node: YamlNode) : ResizeArray<SchemaSaladString> =
        match node.Kind with
        | YamlNodeKind.Scalar _ -> ResizeArray [ decodeSchemaSaladString node ]
        | YamlNodeKind.Sequence _ -> decodeSchemaSaladStringArray node
        | YamlNodeKind.Mapping _ ->
            match tryDirective node with
            | Some directive -> ResizeArray [ directive ]
            | None -> failAt node ("Expected a string or a list of strings, found " + describeNode node + ".")
        | YamlNodeKind.Alias name -> failAlias name node

    /// Decode the doc and the label of a mapping, each with the node that it was read from. The values go to the Doc and
    /// Label properties of an object, and the nodes go to its SourceNodes, so that the encoder can write the layout of
    /// the author again. A field with a null value counts as missing, as it does for the other fields. A doc and a label
    /// are strings in CWL, so a plain scalar that the core schema reads as a number or a bool is a decode error at the
    /// field. Every other scalar is a string, for example a quoted one or one tagged !!str, and a directive names a file
    /// with the text. A doc may also be a list of strings, which Schema Salad concatenates without a separator. A list of
    /// plain strings is one text, and a list with a directive keeps its parts, because the model does not hold the text
    /// of the file. The node of a doc may be the sequence.
    let internal docAndLabelDecoder (value: YamlNode) : DocText option * SchemaSaladString option * SourceNodes =
        let label (node: YamlNode) = decodeSchemaSaladString node, node
        let doc (node: YamlNode) =
            match node.Kind with
            | YamlNodeKind.Sequence collection ->
                let parts = collection.Items |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeSchemaSaladString item.Node))
                let texts = parts |> List.choose SchemaSaladString.tryLiteral
                if texts.Length = parts.Length then
                    DocText.Text (SchemaSaladString.Literal (String.concat "" texts)), node
                else
                    DocText.Parts (ResizeArray parts), node
            | _ -> DocText.Text (decodeSchemaSaladString node), node
        let docField = tryGetPresentField "doc" doc value
        let labelField = tryGetPresentField "label" label value
        Option.map fst docField, Option.map fst labelField, { SourceNodes.Empty with Doc = Option.map snd docField; Label = Option.map snd labelField }

    /// Decode the glob of an output binding: one pattern or a list of patterns, kept as it was written.
    let outputGlobDecoder (node: YamlNode) : OutputGlob =
        match node.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeSchemaSaladString item.Node))
            |> ResizeArray
            |> OutputGlob.Multiple
        | _ -> OutputGlob.Single (decodeSchemaSaladString node)

    /// Decode the field that holds one binding, given the decoder of the fields of a binding. An $import stands for the
    /// binding of a file, and so does a $mixin, which the file completes with the fields beside it. The decoder reads no
    /// file, so the directive stays as written. The fields beside a mixin win over the fields of the file, so they have to be
    /// valid on their own, and the decoder of the fields checks them. An $include yields the text of a file, which is no
    /// binding. The text names the field, as in the inputBinding field.
    let private bindingEntryDecoder (what: string) (decodeFields: YamlNode -> 'T) (value: YamlNode) : ListEntry<'T> =
        match objectDirective what value with
        | Some (ImportedFile path) -> ListEntry.Imported path
        | Some (MixedMapping raw) ->
            decodeFields value |> ignore
            ListEntry.Mixed raw
        | None -> ListEntry.Value (decodeFields value)

    /// Decode a YamlNode into the fields of an output binding. The argument decodes the outputEval.
    let outputBindingGlobDecoder (outputEvalDecoder: YamlNode -> SchemaSaladString) : (YamlNode -> OutputBinding) =
        fun value ->
            Decode.object (fun get ->
                // Read all known outputBinding fields first; any implementation
                // extension fields are attached below through overflowIntoDynamicObj.
                let glob = get.Optional.Field "glob" outputGlobDecoder
                let binding =
                    OutputBinding(
                        ?glob = glob,
                        ?loadContents = tryGetBoolField "loadContents" value,
                        ?loadListing = tryGetLoadListingField "loadListing" value,
                        ?outputEval = get.Optional.Field "outputEval" outputEvalDecoder
                    )
                overflowIntoDynamicObj binding (OutputBinding.KnownFieldNames |> Seq.toList) value |> ignore
                binding
            ) value

    /// Decode a YamlNode into an OutputBinding, or the directive that stands for it. A port in scalar shorthand has none.
    let outputBindingDecoder (outputEvalDecoder: YamlNode -> SchemaSaladString) : (YamlNode -> ListEntry<OutputBinding> option) =
        tryGetPresentField "outputBinding" (bindingEntryDecoder "the outputBinding field" (outputBindingGlobDecoder outputEvalDecoder))

    /// Decode a list of strings. A single string stands for a list of one.
    let decodeStringArrayOrScalar (value: YamlNode) : ResizeArray<string> =
        match value.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> Decode.nonNull stringDecoder item.Node))
            |> ResizeArray
        | _ ->
            ResizeArray [| Decode.nonNull stringDecoder value |]

    /// Decode CWL outputSource in either scalar or list form.
    let outputSourceDecoder: (YamlNode -> ResizeArray<SchemaSaladString> option) =
        tryGetPresentField "outputSource" stringsOrOneDecoder

    /// Decode a YamlNode into a Dirent
    let direntDecoder: (YamlNode -> DirentInstance) =
        fun value ->
            Decode.object (fun get ->
                let dirent =
                    DirentInstance(
                        get.Required.Field "entry" decodeSchemaSaladString,
                        ?entryname = get.Optional.Field "entryname" decodeSchemaSaladString,
                        ?writable = get.Optional.Field "writable" (schemaSaladBoolDecoder "the writable field")
                    )
                overflowIntoDynamicObj dirent (DirentInstance.KnownFieldNames |> Seq.toList) value |> ignore
                dirent
            ) value

    /// Fail at a field of a Dirent in a File or a Directory of a listing. A Dirent has the fields entry and entryname and
    /// the flag writable, and an object with a class is no Dirent, so cwltool rejects them as invalid fields of a File and of
    /// a Directory.
    let private rejectDirentFields (className: string) (value: YamlNode) : unit =
        for entry in mappingEntries value do
            let field = entryName entry
            if field = "entry" || field = "entryname" || field = "writable" then
                atField field (fun () ->
                    failAt entry.Key ("A " + className + " has no field " + field + ". The fields entry and entryname and the flag writable belong to a Dirent, which has no class."))

    /// Decode a File or a Directory object, which an array inside a listing holds, or the directive that stands for one. An
    /// $import takes the data of a file, and a $mixin completes the object with the fields beside it. An $include yields
    /// text, which is no File and no Directory. An object whose class is a directive is a File or a Directory that the file
    /// names, and it stays as it was written.
    let private fileOrDirectoryDecoder (value: YamlNode) : FileOrDirectory =
        match objectDirective "an item of an array in the listing" value with
        | Some (ImportedFile path) -> ImportedItem path
        | Some (MixedMapping raw) -> MixedItem raw
        | None when fieldIsDirective "class" value ->
            rejectDirentFields "File or Directory" value
            RawItem { Node = value }
        | None ->
        match tryGetStringField "class" value with
        | Some "File" ->
            rejectDirentFields "File" value
            FileItem (decodeFileInstanceFields value)
        | Some "Directory" ->
            rejectDirentFields "Directory" value
            DirectoryItem (decodeDirectoryInstanceFields value)
        | _ -> failAt value ("Expected a File or a Directory with a class field in an array of the listing, found " + describeNode value + ".")

    /// True when the text holds a parameter reference or a JavaScript expression, as in $(inputs.x) and ${return 1;}.
    /// CWL asks for one in the fields that it types as Expression alone, and cwltool checks the text.
    let private containsExpression (text: string) : bool =
        text.Contains "$(" || text.Contains "${"

    /// Decode a listing entry of InitialWorkDirRequirement. CWL admits null, a Dirent, an expression, a File, a
    /// Directory and an array of Files and Directories. A text is an expression, so it holds $( or ${.
    let initialWorkDirEntryDecoder: (YamlNode -> InitialWorkDirEntry) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Mapping collection ->
                // The class field makes a File or a Directory of the object, and the fields of a Dirent are errors there. An
                // object with entry and no class is a Dirent, even if it also contains optional entryname/writable fields.
                // A mapping whose class is a directive is a File or a Directory that the file names, and it stays as written.
                // The only other mapping is a directive that includes or imports the text of a file.
                if fieldIsDirective "class" value then
                    rejectDirentFields "File or Directory" value
                    RawEntry { Node = value }
                else
                match tryGetStringField "class" value, collection.Items with
                | Some "File", _ ->
                    // File and Directory object decoding is shared with CWL type decoding so
                    // listing entries preserve the same extension metadata.
                    rejectDirentFields "File" value
                    FileEntry (decodeFileInstanceFields value)
                | Some "Directory", _ ->
                    rejectDirentFields "Directory" value
                    DirectoryEntry (decodeDirectoryInstanceFields value)
                | None, items when items |> List.exists (fun entry -> entryName entry = "entry") ->
                    DirentEntry (direntDecoder value)
                | Some className, items when items |> List.exists (fun entry -> entryName entry = "entry") ->
                    atField "class" (fun () ->
                        failAt (collection.Items |> List.find (fun entry -> entryName entry = "class")).Value ("A Dirent has no class field, found the class " + className + ". Only a File and a Directory have a class."))
                | _ ->
                    match tryDirective value with
                    | Some directive -> StringEntry directive
                    | None ->
                        failAt value "Expected a Dirent with an entry field, a File or a Directory with a class field, or a directive in the listing, found a mapping."
            | YamlNodeKind.Scalar _ ->
                match value with
                | CoreNull -> NullEntry
                | _ ->
                    // A scalar entry is an expression that evaluates to a File, a Directory, a Dirent or a list of
                    // them. The Dirent holds the text of a file in its entry field.
                    match stringDecoder value with
                    | text when containsExpression text -> StringEntry (SchemaSaladString.Literal text)
                    | _ ->
                        failAt value ("Expected an expression, a Dirent, a File or a Directory as an entry of the listing, found " + describeNode value + ". An expression holds $( or ${.")
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
          Imports: Set<string>
          /// True when a $mixin may bring types that the document does not show, as in a requirement that holds a $mixin and
          /// no types field. The file of the mixin decides, and the decoder reads no file, so every well-formed type name
          /// counts as defined.
          Open: bool }

    /// The SchemaDefRequirements that one chain of enclosing processes and steps leaves at a place. A process that a step
    /// runs by #id has the chain of that step. The requirements of a chain follow cwltool. A step hands its process the
    /// requirements of its workflow, and a requirement of the step replaces the one of the workflow. When a workflow has
    /// inherited a SchemaDefRequirement and defines one itself, the first of the two goes on to its steps. So the
    /// outermost SchemaDefRequirement reaches the processes below a nested workflow, and the nested workflow keeps its
    /// own for its ports.
    type SchemaDefChain =
        { /// The types of the SchemaDefRequirement that the process around the place inherits from its step or from the
          /// workflow of its step.
          Inherited: DefinedTypes option
          /// The types of the innermost SchemaDefRequirement in the requirements of the process around the place. It
          /// replaces the inherited one for the ports of that process.
          Own: DefinedTypes option
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
          /// The chains of enclosing processes and steps that reach the place, with the SchemaDefRequirements of each
          /// chain. The requirement of a step overrides the requirement of its workflow, and the requirement of a
          /// process overrides both. A process of a packed document has one chain for each step that runs it, and a
          /// document with one process has a single chain.
          Chains: SchemaDefChain list
          /// The types that apply at the place. A chain applies the types of its own requirement, else the types of its
          /// inherited requirement, else the types of its hint, else its Fallback, and a type applies only when every
          /// chain applies it, as cwltool checks a
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
          /// The paths of the files with requirements or hints that the processes and steps around the place import, with the
          /// . and .. segments resolved. A file may hold a SchemaDefRequirement, and a type of that file is named by the path
          /// in front of a #.
          ImportedRequirements: Set<string>
          /// The ids of the named record and enum schemas that the ports of the process around the place write inline,
          /// without the leading #. Any port of the process may name them, whatever SchemaDefRequirement applies.
          PortIds: Set<string>
          /// The ids of the processes of the packed document that the place belongs to, without a leading #, or none
          /// outside of a packed document. A step of such a document refers to a process by #id.
          GraphIds: Set<string> option
          /// The cwlVersion that a process without one of its own takes. It is the version of the enclosing process
          /// or of the packed document, and a document at the root has none.
          DefaultCwlVersion: string option
          /// True where the steps are checked against the processes that they run: the out field against the outputs of an
          /// inline process, and a run of the form #id against the processes of the packed document. cwltool builds the
          /// process tree that starts at the process it runs and nothing else, so a process of a packed document that this
          /// tree does not reach meets the schema only.
          CheckSteps: bool }

    /// The scope of the root of a document.
    let rootScope : DecodeScope =
        { Shorthand = true
          Process = None
          StreamTypes = NoStreams
          References = DefinedNames
          Definitions = Set.empty
          ImportedPaths = Set.empty
          Chains = [ { Inherited = None; Own = None; Hinted = None; Fallback = { Ids = Set.empty; Imports = Set.empty; Open = false } } ]
          InForce = { Ids = Set.empty; Imports = Set.empty; Open = false }
          Later = Set.empty
          DocId = ""
          ImportedRequirements = Set.empty
          PortIds = Set.empty
          GraphIds = None
          DefaultCwlVersion = None
          CheckSteps = true }

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

    /// The path of the file that an entry of the types field of a SchemaDefRequirement brings in, as written. The entry is
    /// a mapping with the $import directive only, or a mapping with a $mixin. The types field can be an $import mapping
    /// as a whole too. A mixin names a file like an import does, and the decoder reads neither.
    let private importedPathOf (entry: YamlNode) : string option =
        match readDirective entry with
        | DirectiveRead (SchemaSaladString.Import path)
        | DirectiveRead (SchemaSaladString.Mixin path)
        | MixinBeside path -> Some path
        | _ -> None

    /// The names among the given fields that the node writes as one directive, as in requirements: {$import: path}.
    let private wholeDirectiveFields (fields: string list) (node: YamlNode) : Set<string> =
        fields
        |> List.filter (fun field ->
            match valueOf field node with
            | Some value ->
                match value.Kind with
                | YamlNodeKind.Mapping _ -> (importedPathOf value).IsSome
                | _ -> false
            | None -> false)
        |> Set.ofList

    /// Fail when the value of an entry of an id map is a directive. Schema Salad adds the key of the entry as a field of the
    /// loaded object, and an $import or an $include has no other field, so cwltool rejects the document. A list holds the
    /// directive instead.
    let private rejectDirectiveValue (what: string) (entry: MappingEntry) : unit =
        let hasDirective =
            entriesOf entry.Value
            |> List.exists (fun field -> scalarText field.Key = Some "$import" || scalarText field.Key = Some "$include")
        if hasDirective then
            failAt entry.Value ("A directive cannot stand for the value of " + what + ", because Schema Salad adds the key as a field of the loaded object. Write the field as a list.")

    /// The entry of a list for an entry of an id map whose value holds a $mixin. Schema Salad sets the key of the map
    /// entry as a field of the loaded object, so the entry is the mapping with that field in front, and it replaces a field
    /// of that name in the value. The id key names the field. The result is none for a value without a $mixin.
    let private mixedEntryOfMapValue (idKey: string) (key: string) (value: YamlNode) : RawYaml option =
        match readDirective value, value.Kind with
        | (DirectiveRead (SchemaSaladString.Mixin _) | MixinBeside _), YamlNodeKind.Mapping collection ->
            let idEntry = YamlNode.entry (YamlNode.plain idKey) (ScalarQuoting.textNode key)
            // Schema Salad sets the id field to the key, so a field of that name in the value gives way to it.
            let others = collection.Items |> List.filter (fun entry -> scalarText entry.Key <> Some idKey)
            Some { Node = { value with Kind = YamlNodeKind.Mapping { collection with Items = idEntry :: others } } }
        | Malformed (path, message, at), _ -> failMalformed path message at
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

    /// True when the node is a mapping with a $mixin, alone or beside other fields. The file of the mixin may supply the
    /// class, the name, the type and the fields that the node does not show.
    let private holdsMixin (node: YamlNode) : bool =
        match readDirective node with
        | DirectiveRead (SchemaSaladString.Mixin _)
        | MixinBeside _ -> true
        | _ -> false

    /// True when an entry of the types field is a $mixin without a name beside it, so the file of the mixin may name the
    /// type that the entry defines.
    let private entryMayNameTypes (entry: YamlNode) : bool =
        holdsMixin entry && (valueOf "name" entry).IsNone

    /// The entries of the types field of a SchemaDefRequirement. The field is a list. A mapping with the $import
    /// directive only is the field as a whole, and Schema Salad replaces it with the list of the imported file, so it
    /// stands for a list with one import.
    let private typeEntriesIn (types: YamlNode) : YamlNode list =
        match types.Kind with
        | YamlNodeKind.Sequence _ -> itemsOf types
        | YamlNodeKind.Mapping _ when (importedPathOf types).IsSome -> [ types ]
        | _ -> []

    /// The nodes of the SchemaDefRequirements in a requirements or hints field, in list and in map syntax. A requirement
    /// whose class is a directive counts when it has a types field, because no other class has one. A requirement with a
    /// $mixin and no class counts too, because the file of the mixin may give it the class SchemaDefRequirement.
    let private schemaDefsIn (field: YamlNode) : YamlNode list =
        match field.Kind with
        | YamlNodeKind.Sequence _ ->
            itemsOf field
            |> List.filter (fun item ->
                match valueOf "class" item with
                | Some classNode -> scalarText classNode = Some "SchemaDefRequirement" || (hasDirective classNode && (valueOf "types" item).IsSome)
                | None -> holdsMixin item)
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
            let ofRecord () =
                let recordId = defaultArg ownId parentId
                Option.toList ownId
                @ (fieldTypesOf node |> List.collect (fun (name, fieldType) -> namedSchemaIds (scopedId recordId name) fieldType))
            let ofArray () = valueOf "items" node |> Option.map (namedSchemaIds parentId) |> Option.defaultValue []
            match valueOf "type" node with
            | Some typeNode when hasDirective typeNode ->
                // The file decides the kind of the schema, so the ids of every kind count.
                ofRecord () @ ofArray ()
            | None when holdsMixin node ->
                // The file of the mixin gives the type, so the name beside it counts, and so do the fields and items that it shows.
                ofRecord () @ ofArray ()
            | typeNode ->
                match typeNode |> Option.bind scalarText with
                | Some "record" -> ofRecord ()
                | Some "enum" -> Option.toList ownId
                | Some "array" -> ofArray ()
                | _ -> typeNode |> Option.map (namedSchemaIds parentId) |> Option.defaultValue []
        | _ -> []

    /// The ids of the named record and enum schemas that the ports of a process write inline in their types. A port has
    /// an id below the id of the process, and a schema in its type gets an id below the id of the port. A port is an
    /// entry of the inputs or outputs map, or an item of the list with an id field. A port in the shorthand of a type
    /// holds the type itself.
    let private portSchemaIds (processId: string) (processNode: YamlNode) : string list =
        [ "inputs"; "outputs" ]
        |> List.collect (fun field ->
            let ports =
                match valueOf field processNode with
                | Some portsNode ->
                    match portsNode.Kind with
                    | YamlNodeKind.Mapping _ -> entriesOf portsNode |> List.choose (fun entry -> scalarText entry.Key |> Option.map (fun name -> name, entry.Value))
                    | YamlNodeKind.Sequence _ -> itemsOf portsNode |> List.choose (fun port -> valueOf "id" port |> Option.bind scalarText |> Option.map (fun name -> name, port))
                    | _ -> []
                | None -> []
            ports
            |> List.collect (fun (name, port) ->
                let typeNode =
                    match port.Kind with
                    | YamlNodeKind.Mapping _ -> valueOf "type" port
                    | _ -> Some port
                typeNode |> Option.map (namedSchemaIds (scopedId processId name)) |> Option.defaultValue []))

    /// The ids that an entry of the types field of a SchemaDefRequirement defines below the id of its holder: its own
    /// id and the ids of the named schemas inside it. A directive on its own and an unnamed schema add no id of their own.
    /// A $mixin beside fields adds the ids that the fields show.
    let private entryIds (holderId: string) (entry: YamlNode) : string list =
        match readDirective entry with
        | DirectiveRead _ -> []
        | _ -> namedSchemaIds holderId entry

    /// The paths of the files that the requirements and the hints of a process or a step import, as written. An entry of the
    /// list is a mapping with the $import directive only, and the field can be such a mapping as a whole.
    let private importedRequirementPathsOf (holder: YamlNode) : string list =
        [ "requirements"; "hints" ]
        |> List.collect (fun field ->
            match valueOf field holder with
            | Some entries ->
                match entries.Kind with
                | YamlNodeKind.Sequence _ -> itemsOf entries |> List.choose importedPathOf
                | YamlNodeKind.Mapping _ -> importedPathOf entries |> Option.toList
                | _ -> []
            | None -> [])

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
                importedPathOf entry |> Option.iter (fun path -> imports.Add (normalizedPath path))
                definitions.AddRange (entryIds holderId entry)
            for path in importedRequirementPathsOf holder do
                imports.Add (normalizedPath path)
        let rec addProcess (id: string) (processNode: YamlNode) =
            addHolder id processNode
            definitions.AddRange (portSchemaIds id processNode)
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

    /// Fail at a $schemas field that cwltool refuses. The field takes the path of a schema file or a list of paths, and a
    /// directive or any other node there stops cwltool before it reads the document. A path is a string, so a plain
    /// number or bool is an error too, and so is null.
    let private checkSchemasField (root: YamlNode) : unit =
        let pathOf (node: YamlNode) : unit =
            match node.Kind with
            | YamlNodeKind.Scalar _ -> stringDecoder node |> ignore
            | _ -> failAt node ("Expected the path of a schema file as a string, found " + describeNode node + ".")
        match entriesOf root |> List.tryFind (fun entry -> scalarText entry.Key = Some "$schemas") with
        | Some entry ->
            atField "$schemas" (fun () ->
                match entry.Value.Kind with
                | YamlNodeKind.Sequence collection -> collection.Items |> List.iteri (fun index item -> atField $"[{index}]" (fun () -> pathOf item.Node))
                | YamlNodeKind.Scalar _ -> pathOf entry.Value
                | _ -> failAt entry.Value ("Expected the path of a schema file or a list of paths, found " + describeNode entry.Value + "."))
        | None -> ()

    /// The scope of a document, with the types that it defines, the files that it imports and the id of its root
    /// process. The definitions come from a pre-pass over the raw nodes.
    let documentScope (root: YamlNode) : DecodeScope =
        checkSchemasField root
        let definitions, imports = documentDefinitions root
        { rootScope with Definitions = definitions; ImportedPaths = imports; DocId = processId "" "" root }

    /// The types that a SchemaDefRequirement defines, with the ids relative to the id of the process or step that
    /// holds it. A requirement with a $mixin and no types field takes the types of its file, and so does an entry of the
    /// types field that a $mixin completes without a name. The decoder reads no file, so those leave the types open.
    let private definedTypesOf (holderId: string) (schemaDef: YamlNode) : DefinedTypes =
        let entries = valueOf "types" schemaDef |> Option.map typeEntriesIn |> Option.defaultValue []
        { Ids = entries |> List.collect (entryIds holderId) |> Set.ofList
          Imports = entries |> List.choose importedPathOf |> List.map normalizedPath |> Set.ofList
          Open = (holdsMixin schemaDef && (valueOf "types" schemaDef).IsNone) || List.exists entryMayNameTypes entries }

    /// The types that both of two requirements let through. An open requirement lets through every name, so it leaves the
    /// choice of the ids to the other one.
    let private bothTypes (left: DefinedTypes) (right: DefinedTypes) : DefinedTypes =
        { Ids = (if left.Open then right.Ids elif right.Open then left.Ids else Set.intersect left.Ids right.Ids)
          Imports = Set.intersect left.Imports right.Imports
          Open = left.Open && right.Open }

    /// The types that a chain applies: those of its own requirement, else those that it inherits, else those of its hint,
    /// else its fallback.
    let private appliedBy (chain: SchemaDefChain) : DefinedTypes =
        match chain.Own, chain.Inherited, chain.Hinted with
        | Some types, _, _ -> types
        | None, Some types, _ -> types
        | None, None, Some types -> types
        | None, None, None -> chain.Fallback

    /// The SchemaDefRequirement that a chain hands to the steps of the process around the place: the one that the
    /// process inherited, else its own. cwltool keeps the first SchemaDefRequirement of the requirements of the workflow
    /// for its steps, and the inherited one stands in front of the own one.
    let private handedDown (chain: SchemaDefChain) : DefinedTypes option =
        Option.orElse chain.Own chain.Inherited

    /// What a node adds to the chains around it.
    type private HolderKind =
        /// A process. Its own requirement replaces the one that it inherited for its ports.
        | ProcessHolder
        /// A step. Its own requirement replaces the one of the workflow for the process that the step runs.
        | StepHolder

    /// The scope inside a process or a step. The last SchemaDefRequirement of a list is the innermost one, as in cwltool,
    /// and a requirement overrides a hint of the same class. The id of the holder is the DocId of the scope.
    let private enterHolder (kind: HolderKind) (holder: YamlNode) (scope: DecodeScope) : DecodeScope =
        let innermostIn (field: string) =
            valueOf field holder |> Option.bind (fun types -> schemaDefsIn types |> List.tryLast) |> Option.map (definedTypesOf scope.DocId)
        let required = innermostIn "requirements"
        let hinted = innermostIn "hints"
        let enter (chain: SchemaDefChain) : SchemaDefChain =
            let hints = Option.orElse chain.Hinted hinted
            match kind with
            | ProcessHolder -> { chain with Own = required; Hinted = hints }
            | StepHolder -> { chain with Inherited = Option.orElse (handedDown chain) required; Own = None; Hinted = hints }
        let chains = scope.Chains |> List.map enter |> List.distinct
        let inForce =
            chains
            |> List.map appliedBy
            |> List.reduce bothTypes
        let imported = importedRequirementPathsOf holder |> List.map normalizedPath |> Set.ofList
        { scope with Chains = chains; InForce = inForce; ImportedRequirements = Set.union scope.ImportedRequirements imported }

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

    /// True when a Reference can spell the name: it is not empty, holds no whitespace and none of the characters [ ] ?
    /// that the shorthand of a type uses.
    let private isWellFormedName (name: string) : bool =
        name <> "" && not (name |> Seq.exists (fun character -> System.Char.IsWhiteSpace character || character = '[' || character = ']' || character = '?'))

    /// True when the name spells a type that applies at the place. The name finds an id in the document, and the
    /// innermost SchemaDefRequirement defines that id. A name behind a path needs the import in that requirement. An open
    /// requirement lets every name through, because the file of a mixin may define the type and may import any file.
    let private isDefinedReference (scope: DecodeScope) (name: string) : bool =
        match importedPathOfName name with
        | Some path ->
            (scope.InForce.Open && isWellFormedName name)
            || Set.contains path scope.InForce.Imports || Set.contains path scope.ImportedRequirements
            // A path with a scheme, such as file:///work/shapes.yml#Name, names the file by an address that the decoder cannot
            // compare with the path of an import. It counts when the document imports a file at all.
            || (path.Contains "://" && not (Set.isEmpty scope.InForce.Imports && Set.isEmpty scope.ImportedRequirements))
        | None ->
            (scope.InForce.Open && isWellFormedName name)
            || (resolveLink scope scope.Definitions name |> Option.exists (fun id -> Set.contains id scope.InForce.Ids || Set.contains id scope.PortIds))

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
                "Invalid CWL type: " + name + ". The document defines this type in a SchemaDefRequirement that does not apply here. The innermost SchemaDefRequirement applies. It overrides the one of the enclosing process, and a requirement overrides a hint. A nested workflow hands its steps the SchemaDefRequirement that it inherited, the outermost one."
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

    /// The name that cwltool compares when it checks the symbols of an enum for repeats. Schema Salad resolves a symbol like
    /// a link, and cwltool keeps the last segment of the fragment behind a #, or of the path when there is no fragment. An
    /// empty fragment is no fragment, as urlsplit reads it, so a# names the symbol a. A scheme in front (p:a) and a query
    /// behind (a?b) belong to neither, so x/a, #a, p:a and a name the same symbol.
    let private symbolShortName (symbol: string) : string =
        let hash = symbol.IndexOf '#'
        let rest =
            if hash >= 0 && hash < symbol.Length - 1 then
                symbol.Substring(hash + 1)
            else
                let beforeFragment = if hash >= 0 then symbol.Substring(0, hash) else symbol
                let withoutQuery = match beforeFragment.IndexOf '?' with | -1 -> beforeFragment | query -> beforeFragment.Substring(0, query)
                System.Text.RegularExpressions.Regex.Replace(withoutQuery, "^[A-Za-z][A-Za-z0-9+.-]*:", "")
        rest.Substring(rest.LastIndexOf '/' + 1)

    /// Fail at the first symbol of an enum that repeats an earlier one. cwltool rejects an enum with a repeated symbol,
    /// whether a port or a definition holds it. A directive stands for a symbol of a file that the decoder does not read,
    /// so only the plain symbols are compared.
    let private rejectRepeatedSymbols (node: YamlNode) (symbols: ResizeArray<SchemaSaladString>) : unit =
        let seen = System.Collections.Generic.Dictionary<string, YamlNode>()
        let items = itemsOf node
        for index in 0 .. min (symbols.Count - 1) (items.Length - 1) do
            match SchemaSaladString.tryLiteral symbols.[index] with
            | Some text ->
                let name = symbolShortName text
                match seen.TryGetValue name with
                | true, first ->
                    atField $"[{index}]" (fun () ->
                        failAt items.[index] ("The symbol " + text + " repeats the symbol at " + placeOf first + ", because cwltool compares the last segment of a symbol (" + name + "). An enum lists each symbol once."))
                | _ -> seen.[name] <- items.[index]
            | None -> ()

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
                    ?name = get.Optional.Field "name" stringDecoder
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
            let name = value |> Decode.object (fun get -> get.Required.Field "name" (Decode.nonNull stringDecoder))
            atField name (fun () -> inputRecordFieldObject scope name value)

    /// Decode fields as an array: [{name: x, type: y}]. An $import stands for the fields of a file.
    and decodeFieldsAsArray (scope: DecodeScope) (element: YamlNode) : ResizeArray<ListEntry<InputRecordField>> =
        let decodeEntry (item: YamlNode) : ListEntry<InputRecordField> =
            match objectDirective "an entry of the fields field of a record" item with
            | Some (ImportedFile importPath) -> ListEntry.Imported importPath
            | Some (MixedMapping raw) -> ListEntry.Mixed raw
            | None -> ListEntry.Value (inputRecordFieldDecoder scope item)
        Decode.resizearray decodeEntry element

    /// Decode fields as a map: {fieldName: type} or {fieldName: {type: ..., inputBinding: ...}}
    and decodeFieldsAsMap (scope: DecodeScope) (element: YamlNode) : ResizeArray<ListEntry<InputRecordField>> =
        let fields = ResizeArray<ListEntry<InputRecordField>>()
        for entry in mappingEntries element do
            // Map-style record fields use the mapping key as the field name. A mapping is the field object, with the
            // type and the other properties of the field. Any other value is the type itself.
            let name = idMapKey entry
            let field =
                atField name (fun () ->
                    rejectDirectiveValue "an entry of the fields field of a record" entry
                    match mixedEntryOfMapValue "name" name entry.Value with
                    | Some raw -> ListEntry.Mixed raw
                    | None ->
                        match entry.Value.Kind with
                        | YamlNodeKind.Mapping _ -> ListEntry.Value (inputRecordFieldObject scope name entry.Value)
                        | _ -> ListEntry.Value (InputRecordField(name, cwlTypeDecoderAt (scope |> withChildId name |> withShorthand true) entry.Value)))
            fields.Add field
        fields

    /// Decode an InputRecordSchema from a YamlNode
    and inputRecordSchemaDecoder (scope: DecodeScope) : (YamlNode -> InputRecordSchema) =
        fun value ->
        Decode.object (fun get ->
            let recordName = get.Optional.Field "name" stringDecoder
            // The fields of a named record are relative to the id of the record.
            let scope = match recordName with Some name -> withChildId name scope | None -> scope
            // Try to decode fields as an array (flow-style) or as a map (block-style)
            let decodedFields =
                // Get the fields element directly
                let fieldsElement = get.Optional.Field "fields" Decode.node

                match fieldsElement with
                | Some element when (YamlNode.tryMapping element |> Option.exists (fun mapping -> mapping.Items.IsEmpty)) ->
                    // An empty mapping, fields: {}, has no fields. An empty list, fields: [], takes the array path below.
                    Some (ResizeArray<ListEntry<InputRecordField>>())
                | Some element ->
                    // A list holds field objects with their names, and a mapping holds the fields under their names. A
                    // mapping with the $import directive only is the field as a whole, and it stands for the fields of
                    // a file.
                    atField "fields" (fun () ->
                        match element.Kind with
                        | YamlNodeKind.Sequence _ -> Some (decodeFieldsAsArray scope element)
                        | YamlNodeKind.Mapping _ when (importOnly "the fields field of a record" element).IsSome ->
                            Some (ResizeArray [ ListEntry.Imported (importOnly "the fields field of a record" element).Value ])
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
            schema.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "fields" ] value }
            overflowIntoDynamicObj schema (InputRecordSchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value

    /// Decode an InputEnumSchema from a YamlNode
    and inputEnumSchemaDecoder: (YamlNode -> InputEnumSchema) =
        fun value ->
        Decode.object (fun get ->
            let symbols =
                get.Required.Field "symbols" (fun symbolsNode ->
                    let symbols = decodeSchemaSaladStringList "the symbols field of an enum" symbolsNode
                    rejectRepeatedSymbols symbolsNode symbols
                    symbols)

            let doc, label, sourceNodes = docAndLabelDecoder value
            let schema =
                InputEnumSchema(
                    symbols,
                    ?label = label,
                    ?doc = doc,
                    ?name = get.Optional.Field "name" stringDecoder
                )
            schema.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "symbols" ] value }
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
            // A type is a name, and the text of a file can be one, so an $include can stand in the place of a type.
            match readDirective element with
            | Malformed (path, message, at) -> failMalformed path message at
            | DirectiveRead (SchemaSaladString.Include path) -> IncludedType path
            | DirectiveRead (SchemaSaladString.Import path) -> ImportedType path
            | DirectiveRead (SchemaSaladString.Mixin _)
            | DirectiveRead (SchemaSaladString.Mixed _)
            | MixinBeside _ -> MixedType { Node = element }
            | DirectiveRead (SchemaSaladString.Literal _)
            | NotDirective ->
            // The type of a schema may be a directive, and the file decides which kind of schema it is, so the schema stays as it
            // was written.
            if fieldIsDirective "type" element then RawType { Node = element } else
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

    /// Decode a field that CWL types as an expression, alone or beside a number or a boolean: the expression of an
    /// ExpressionTool, the outputEval of an output binding, the field when of a step, the networkAccess of NetworkAccess,
    /// the enableReuse of WorkReuse, the timelimit of ToolTimeLimit and the fields of ResourceRequirement. A text with $( or
    /// ${ is the expression, and a text without one is an error. A directive stands for the text of a file. A plain scalar that
    /// the core schema reads as something other than text is no text here, and quotes would not turn it into an expression, so
    /// the error leaves out the advice to quote it. The text names what the field takes, as in a number or an expression.
    let private expressionTextDecoder (expected: string) (element: YamlNode) : SchemaSaladString =
        let unexpected () = failAt element ("Expected " + expected + ", found " + describeNode element + ". An expression holds $( or ${.")
        let isNonTextScalar =
            match element.Kind, element with
            | YamlNodeKind.Scalar _, (CoreText | CoreNull) -> false
            | YamlNodeKind.Scalar _, _ -> true
            | _ -> false
        if isNonTextScalar then
            unexpected ()
        else
            match decodeSchemaSaladString element with
            | SchemaSaladString.Literal text when not (containsExpression text) -> unexpected ()
            | value -> value

    /// Fail at the required field of an object in the secondaryFiles of a port. CWL types it as a boolean or an expression, and
    /// null counts as missing. A directive stands for the value of a file, so it passes.
    let private checkSecondaryFileRequired (node: YamlNode) : unit =
        match node with
        | CoreNull
        | CoreBool _ -> ()
        | _ ->
            match readDirective node with
            | Malformed (path, message, at) -> failMalformed path message at
            | NotDirective ->
                let isExpression =
                    match node.Kind with
                    | YamlNodeKind.Scalar _ ->
                        match node with
                        | CoreText -> containsExpression (Decode.string node)
                        | _ -> false
                    | _ -> false
                if not isExpression then
                    failAt node ("Expected a bool or an expression, found " + describeNode node + ". An expression holds $( or ${.")
            | _ -> ()

    /// Fail at the pattern of an object in the secondaryFiles of a port. CWL requires it and types it as a string or an
    /// expression. A plain scalar that the core schema reads as something other than text is no string, and neither is a
    /// list or a mapping. A directive stands for the value of a file, so it passes.
    let private checkSecondaryFilePattern (secondaryFile: YamlNode) : unit =
        match fieldEntry "pattern" secondaryFile with
        | None -> failAt secondaryFile "Expected a pattern in an object as a secondary file, found an object without one."
        | Some entry ->
            atField "pattern" (fun () ->
                let isText =
                    match entry.Value.Kind with
                    | YamlNodeKind.Scalar _ -> (match entry.Value with CoreText -> true | _ -> false)
                    | _ -> false
                if not isText then
                    match readDirective entry.Value with
                    | Malformed (path, message, at) -> failMalformed path message at
                    | NotDirective -> failAt entry.Value ("Expected a string or an expression as a pattern, found " + describeNode entry.Value + ".")
                    | _ -> ())

    /// Check the secondaryFiles of a port. A secondary file is a string or an object, and the field holds one of them or a list.
    /// A scalar that the core schema reads as something other than text is no string, and neither is a list inside the list.
    /// Null as the whole field counts as missing. A directive stands for what a file holds, so it passes. An object has a
    /// pattern, and its required field is a boolean or an expression.
    let private checkSecondaryFileObjects (port: YamlNode) : unit =
        let isDirective (node: YamlNode) = [ "$import"; "$include"; "$mixin" ] |> List.exists (fun key -> (fieldEntry key node).IsSome)
        let check (node: YamlNode) =
            match node.Kind with
            | YamlNodeKind.Mapping _ ->
                if not (isDirective node) then
                    checkSecondaryFilePattern node
                match fieldEntry "required" node with
                | Some entry -> atField "required" (fun () -> checkSecondaryFileRequired entry.Value)
                | None -> ()
            | YamlNodeKind.Alias name -> failAlias name node
            | YamlNodeKind.Scalar _ ->
                match node with
                | CoreText -> ()
                | _ -> failAt node ("Expected a string or an object as a secondary file, found " + describeNode node + ".")
            | YamlNodeKind.Sequence _ -> failAt node "Expected a string or an object as a secondary file, found a list."
        match valueOf "secondaryFiles" port with
        | Some files ->
            match files.Kind with
            | YamlNodeKind.Sequence collection ->
                collection.Items
                |> List.iteri (fun index item -> atField "secondaryFiles" (fun () -> atField $"[{index}]" (fun () -> check item.Node)))
            | YamlNodeKind.Scalar _ when (match files with CoreNull -> true | _ -> false) -> ()
            | _ -> atField "secondaryFiles" (fun () -> check files)
        | None -> ()

    /// The fields that the output of a process of this class does not have, each with the reason for the error. CWL gives
    /// the output of each class its own fields, and cwltool rejects the others. The output of a workflow takes the
    /// outputSource, the linkMerge and the pickValue and has no outputBinding. The output of a command line tool binds
    /// files. The output of an expression tool or of an operation has neither the one nor the other.
    let private refusedOutputFields (processClass: ProcessClass) : (string * string) list =
        let binding = "outputBinding", "Only an output of a command line tool binds files."
        let ofWorkflow =
            [ "outputSource", "Only an output of a workflow names the output of a step."
              "linkMerge", "Only an output of a workflow merges the values of its sources."
              "pickValue", "Only an output of a workflow picks among the values of its sources." ]
        match processClass with
        | WorkflowClass -> [ binding ]
        | CommandLineToolClass -> ofWorkflow
        | ExpressionToolClass
        | OperationClass -> binding :: ofWorkflow

    /// Decode a named output from either map shorthand or full object form.
    let decodeNamedOutput (scope: DecodeScope) (name: string) (value: YamlNode) =
        // A place outside of a process admits every field.
        match scope.Process with
        | Some processClass ->
            for (field, reason) in refusedOutputFields processClass do
                match fieldEntry field value with
                | Some entry ->
                    atField field (fun () -> failAt entry.Key ("An output of " + processName processClass + " has no " + field + ". " + reason))
                | None -> ()
        | None -> ()
        checkSecondaryFileObjects value
        // Decode dependent fields before constructing the output so both shorthand
        // and object forms share the same object initialization path.
        let outputBinding = outputBindingDecoder (expressionTextDecoder "an expression") value
        let outputSourceValues = outputSourceDecoder value
        let cwlType = Some (portTypeDecoder (withChildId name scope) value)
        // The tool writes the file of a stream output itself, so a binding has nothing to bind.
        let streamWithBinding (typeName: string) : unit =
            raise (
                YamlDecodeException
                    { Message = "The type " + typeName + " is valid only for an output without an outputBinding."
                      Path = []
                      Span = value.Span }
            )
        match cwlType, outputBinding with
        | Some Stdout, Some _ -> streamWithBinding "stdout"
        | Some Stderr, Some _ -> streamWithBinding "stderr"
        | _ -> ()
        let doc, label, sourceNodes = docAndLabelDecoder value
        // Optional presentation and behavior fields are read directly from the
        // object form; shorthand values simply leave them absent.
        let output =
            CWLOutput(
                name,
                ?type_ = cwlType,
                ?label = label,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" value,
                ?streamable = tryGetBoolField "streamable" value,
                ?doc = doc,
                ?format = tryGetPresentField "format" decodeSchemaSaladString value
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

    /// Warn at the id of an entry of a list that repeats the id of an earlier entry. cwltool accepts such a list and
    /// warns that the object id was previously defined. It checks every entry, and a reference to the id finds the last
    /// one. The list keeps every entry, so the file reads back as it was written. An id with a hash in front of it is the id without the hash.
    let private warnRepeatedIds (warnings: WarningSink) (path: string) (items: YamlNode list) : unit =
        let seen = System.Collections.Generic.Dictionary<string, YamlNode>()
        items
        |> List.iteri (fun index item ->
            match valueOf "id" item |> Option.bind (fun idNode -> scalarText idNode |> Option.map (fun id -> LocalReference.withoutHash id, idNode)) with
            | Some (id, idNode) ->
                match seen.TryGetValue id with
                | true, first ->
                    addWarning warnings $"{path}[{index}]" ("The id " + id + " belongs to an earlier entry of the list too, at " + placeOf first + ". A reference to the id finds the last entry.") (Some item)
                | _ -> seen.[id] <- idNode
            | None -> ())

    /// Decode one sequence-form output, warning only for malformed unnamed entries.
    let decodeOutputSequenceItem (warnings: WarningSink) (scope: DecodeScope) (path: string) (index: int) (item: YamlNode) =
        match tryGetStringField "id" item with
        | Some id -> Some (atField id (fun () -> decodeNamedOutput scope id item))
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL output entry." (Some item)
            None

    /// Decode a YamlNode into an Output Array. An $import stands for the outputs of a file, in the list or as the whole
    /// field.
    let outputArrayDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) (path: string) : (YamlNode -> ResizeArray<ListEntry<CWLOutput>>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form ports must declare id on each item. Unnamed malformed
                // items are the tolerant path and produce warnings.
                warnRepeatedIds warnings path (collection.Items |> List.map (fun item -> item.Node))
                collection.Items
                |> List.mapi (fun index item ->
                    match atField $"[{index}]" (fun () -> objectDirective "an entry of the outputs" item.Node) with
                    | Some (ImportedFile importPath) -> Some (ListEntry.Imported importPath)
                    | Some (MixedMapping raw) -> Some (ListEntry.Mixed raw)
                    | None -> decodeOutputSequenceItem warnings scope path index item.Node |> Option.map ListEntry.Value)
                |> List.choose id
                |> ResizeArray
            | YamlNodeKind.Mapping _ when (importOnly "the outputs field" value).IsSome ->
                ResizeArray [ ListEntry.Imported (importOnly "the outputs field" value).Value ]
            | _ ->
                // Map-form ports use the map key as the port name.
                [|
                    for entry in mappingEntries value do
                        let name = idMapKey entry
                        atField name (fun () ->
                            rejectDirectiveValue "an entry of the outputs" entry
                            match mixedEntryOfMapValue "id" name entry.Value with
                            | Some raw -> ListEntry.Mixed raw
                            | None -> ListEntry.Value (decodeNamedOutput scope name entry.Value))
                |]
                |> ResizeArray

    let outputArrayDecoder: (YamlNode -> ResizeArray<ListEntry<CWLOutput>>) =
        outputArrayDecoderWithWarnings None rootScope "outputs"

    /// Access the outputs field and decode a YamlNode into an Output Array
    let outputsDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<ListEntry<CWLOutput>>) =
        Decode.object (fun get ->
            get.Required.Field "outputs" (outputArrayDecoderWithWarnings warnings scope "outputs")
        )

    let outputsDecoder: (YamlNode -> ResizeArray<ListEntry<CWLOutput>>) =
        outputsDecoderWithWarnings None rootScope

    /// Decode a YamlNode into a DockerRequirement
    let dockerRequirementDecoder (get: Decode.IGetters): DockerRequirement =
        // dockerFile can be a plain string or schema-salad include/import directive.
        let dockerFile = get.Optional.Field "dockerFile" decodeSchemaSaladString

        // Constructor normalization gives dockerFileReference precedence over the
        // legacy plain dockerFile argument.
        DockerRequirement.create(
            ?dockerPull = get.Optional.Field "dockerPull" decodeSchemaSaladString,
            ?dockerFileReference = dockerFile,
            ?dockerImageId = get.Optional.Field "dockerImageId" decodeSchemaSaladString,
            ?dockerLoad = get.Optional.Field "dockerLoad" decodeSchemaSaladString,
            ?dockerImport = get.Optional.Field "dockerImport" decodeSchemaSaladString,
            ?dockerOutputDirectory = get.Optional.Field "dockerOutputDirectory" decodeSchemaSaladString,
            ?dockerRunOptions = get.Optional.Field "cwltool:dockerRunOptions" decodeStringArrayOrScalar
        )

    /// Decode a YamlNode into an EnvVarRequirement array.
    /// Supports both array form and map shorthand form (envName -> envValue). An $import stands for the definitions of
    /// a file, in the list and as the whole field.
    let envVarRequirementDecoder (get: Decode.IGetters): ResizeArray<ListEntry<EnvironmentDef>> =
        // Env var values are strings by CWL semantics. The scalar text is already unquoted, so
        // a quoted "true" and a plain true both decode to the string true.
        let decodeEnvValue = decodeSchemaSaladString

        get.Required.Field "envDef" (fun envDefElement ->
            match envDefElement.Kind with
            | YamlNodeKind.Sequence _ ->
                // Standard CWL form: envDef is an array of {envName, envValue} objects.
                Decode.resizearray
                    (fun value ->
                        match objectDirective "an entry of the envDef field" value with
                        | Some (ImportedFile importPath) -> ListEntry.Imported importPath
                        | Some (MixedMapping raw) -> ListEntry.Mixed raw
                        | None ->
                            Decode.object (fun get2 ->
                                let env =
                                    EnvironmentDef(
                                        get2.Required.Field "envName" decodeSchemaSaladString,
                                        get2.Required.Field "envValue" decodeEnvValue
                                    )
                                overflowIntoDynamicObj env (EnvironmentDef.KnownFieldNames |> Seq.toList) value |> ignore
                                ListEntry.Value env
                            ) value)
                    envDefElement
            | YamlNodeKind.Mapping _ when (importOnly "the envDef field" envDefElement).IsSome ->
                ResizeArray [ ListEntry.Imported (importOnly "the envDef field" envDefElement).Value ]
            | YamlNodeKind.Mapping _ ->
                // Shorthand form: envDef is a map from variable name to value.
                mappingEntries envDefElement
                |> List.map (fun entry ->
                    let name = idMapKey entry
                    atField name (fun () ->
                        rejectDirectiveValue "an entry of the envDef field" entry
                        match mixedEntryOfMapValue "envName" name entry.Value with
                        | Some raw -> ListEntry.Mixed raw
                        | None -> ListEntry.Value (EnvironmentDef(SchemaSaladString.Literal name, decodeEnvValue entry.Value))))
                |> ResizeArray
            | _ ->
                failAt envDefElement ("Expected a list or a mapping for envDef, found " + describeNode envDefElement + "."))

    /// Decode a YamlNode into a SoftwareRequirement array.
    /// Supports both array form and map shorthand form.
    let softwareRequirementDecoder (get: Decode.IGetters): ResizeArray<ListEntry<SoftwarePackage>> =
        let decodeVersion = decodeSchemaSaladStringList "the version field of a package"
        let decodeSpecs = decodeSchemaSaladStringList "the specs field of a package"

        let decodePackageFromMapEntry (packageName: string) (packageValue: YamlNode) =
            let name = SchemaSaladString.Literal packageName
            // packages: { samtools: {} } and packages: { samtools: } both name a package without specs.
            match Decode.option Decode.node packageValue with
            | None ->
                SoftwarePackage(name)
            | Some value ->
                match value.Kind with
                | YamlNodeKind.Sequence _ ->
                    // packages: { samtools: [spec1, spec2] } means specs shorthand.
                    SoftwarePackage(name, specs = decodeSpecs value)
                | YamlNodeKind.Mapping collection when collection.Items.IsEmpty ->
                    SoftwarePackage(name)
                | YamlNodeKind.Mapping _ ->
                    // Full map shorthand can include version/specs plus extension fields.
                    let version = tryGetPresentField "version" decodeVersion value
                    let specs = tryGetPresentField "specs" decodeSpecs value
                    let package = SoftwarePackage(name, ?version = version, ?specs = specs)
                    overflowIntoDynamicObj package (SoftwarePackage.KnownFieldNames |> Seq.toList) value |> ignore
                    package
                | _ ->
                    // Scalar map value is treated as one specs entry.
                    SoftwarePackage(name, specs = ResizeArray [ decodeSchemaSaladString value ])

        let decodePackageObject (value: YamlNode) =
            Decode.object (fun get2 ->
                let package =
                    SoftwarePackage(
                        get2.Required.Field "package" decodeSchemaSaladString,
                        ?version = get2.Optional.Field "version" decodeVersion,
                        ?specs = get2.Optional.Field "specs" decodeSpecs
                    )
                overflowIntoDynamicObj package (SoftwarePackage.KnownFieldNames |> Seq.toList) value |> ignore
                package
            ) value

        get.Required.Field "packages" (fun packagesElement ->
            match packagesElement.Kind with
            | YamlNodeKind.Sequence _ ->
                // Standard CWL form: packages is an array of package objects. An import stands for the packages of a file.
                Decode.resizearray
                    (fun value ->
                        match objectDirective "an entry of the packages field" value with
                        | Some (ImportedFile importPath) -> ListEntry.Imported importPath
                        | Some (MixedMapping raw) -> ListEntry.Mixed raw
                        | None -> ListEntry.Value (decodePackageObject value))
                    packagesElement
            | YamlNodeKind.Mapping _ when (importOnly "the packages field" packagesElement).IsSome ->
                ResizeArray [ ListEntry.Imported (importOnly "the packages field" packagesElement).Value ]
            | YamlNodeKind.Mapping _ ->
                // Compact CWL form: package names are map keys.
                mappingEntries packagesElement
                |> List.map (fun entry ->
                    let name = idMapKey entry
                    atField name (fun () ->
                        rejectDirectiveValue "an entry of the packages field" entry
                        match mixedEntryOfMapValue "package" name entry.Value with
                        | Some raw -> ListEntry.Mixed raw
                        | None -> ListEntry.Value (decodePackageFromMapEntry name entry.Value)))
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
                match stringDecoder listingElement with
                | text when containsExpression text -> InitialWorkDirExpressionRequirement (SchemaSaladString.Literal text)
                | _ ->
                    failAt listingElement ("Expected a list of entries or an expression for the listing, found " + describeNode listingElement + ". An expression holds $( or ${.")
            | YamlNodeKind.Mapping _ ->
                // A directive names a file with the text of the listing, and the decoder reads no file.
                match tryDirective listingElement with
                | Some directive -> InitialWorkDirExpressionRequirement directive
                | None -> failAt listingElement ("Expected a list of entries or an expression for the listing, found " + describeNode listingElement + ".")
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
    /// numeric shape and a quoted number is a string. Schema Salad accepts an int of any size for a float, and the model holds
    /// an int beyond 64 bits as the nearest float. An int beyond the largest float is a decode error.
    let decodeResourceScalar (element: YamlNode) : obj =
        match element with
        | CoreInt parsed -> box parsed
        | CoreFloat parsed -> box parsed
        | CoreBigInt text ->
            match CoreSchema.integerToFloat text with
            | Ok parsed -> box parsed
            | Error _ -> failAt element ("The int " + text + " is beyond the largest float.")
        | _ -> box (expressionTextDecoder "a number or an expression" element)

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
    /// record, enum or array. Process.yml admits no other type there, so any other type is an error at the entry, such
    /// as a builtin type name. The schema keeps its name, which is optional, and its extension fields.
    let schemaDefinitionDecoder (scope: DecodeScope) (value: YamlNode) : SchemaDefinition =
        let fail (message: string) : 'T =
            raise (YamlDecodeException { Message = message; Path = []; Span = value.Span })
        match valueOf "type" value with
        | Some typeNode ->
            match scalarText typeNode with
            | Some "record" -> SchemaDefinition.Record (inputRecordSchemaDecoder scope value)
            | Some "enum" -> SchemaDefinition.Enum (inputEnumSchemaDecoder value)
            | Some "array" -> SchemaDefinition.Array (inputArraySchemaDecoder scope value)
            | Some other -> fail ("A SchemaDefRequirement defines record, enum and array types only, found type: " + other + ".")
            | None -> fail "A SchemaDefRequirement defines record, enum and array types only, found a type that is no name."
        | None -> fail "Expected a type field. A SchemaDefRequirement defines a record or an enum."

    /// Decode an entry of the types field: a file to import, as {$import: path}, or a type to define.
    let schemaDefEntryDecoder (scope: DecodeScope) (value: YamlNode) : SchemaDefEntry =
        match objectDirective "an entry of the types field" value with
        | Some (ImportedFile path) -> Imported path
        | Some (MixedMapping raw) -> Mixed raw
        // The file decides the kind of a schema with a directive as its type, so the entry stays as it was written.
        | None when fieldIsDirective "type" value -> Raw { Node = value }
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
                | YamlNodeKind.Mapping _ -> (importOnly "the types field" typesNode).IsSome
                | _ -> false
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
                    let inForce = { Ids = upToEntry; Imports = imports |> List.take (index + 1) |> List.choose id |> Set.ofList; Open = entries |> List.take (index + 1) |> List.exists entryMayNameTypes }
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
            | None -> WorkReuseExpressionRequirement (expressionTextDecoder "a bool or an expression" value))
        |> Option.defaultValue (WorkReuseRequirement (WorkReuseRequirementValue(true)))

    /// Decode NetworkAccess as either a concrete bool requirement or an expression payload. CWL requires the
    /// networkAccess field, and a missing one is no permission to use the network.
    let networkAccessRequirementDecoder (get: Decode.IGetters): Requirement =
        get.Required.Field "networkAccess" (fun value ->
            match tryDecodeBoolScalar value with
            | Some boolValue -> NetworkAccessRequirement (NetworkAccessRequirementValue(boolValue))
            | None -> NetworkAccessExpressionRequirement (expressionTextDecoder "a bool or an expression" value))

    /// Decode InplaceUpdateRequirement. CWL requires the inplaceUpdate field.
    let inplaceUpdateRequirementDecoder (get: Decode.IGetters): InplaceUpdateRequirementValue =
        InplaceUpdateRequirementValue(get.Required.Field "inplaceUpdate" (schemaSaladBoolDecoder "the inplaceUpdate field"))

    /// Decode a YamlNode into a ToolTimeLimitRequirement value
    let toolTimeLimitRequirementDecoder (get: Decode.IGetters): ToolTimeLimitValue =
        let timeLimitElement = get.Required.Field "timelimit" (Decode.nonNull Decode.node)
        // CWL types the time limit as long or string, so a plain int is the number of seconds and
        // every other value is an expression, and a text without an expression is an error, like in a resource field.
        match timeLimitElement with
        | CoreInt seconds -> ToolTimeLimitSeconds seconds
        | _ -> ToolTimeLimitExpression (atField "timelimit" (fun () -> expressionTextDecoder "an int or an expression" timeLimitElement))

    /// Decode InlineJavascriptRequirement, whose expressionLib is a list of strings.
    let inlineJavascriptRequirementDecoder (get: Decode.IGetters): InlineJavascriptRequirementValue =
        InlineJavascriptRequirementValue(?expressionLib = get.Optional.Field "expressionLib" (decodeSchemaSaladStringList "the expressionLib field"))

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

    /// Warn about a requirement that the specification rules out and cwltool accepts when it checks the document. The
    /// path leads to the requirement from the process that holds it.
    let private warnAboutRequirement (warnings: WarningSink) (path: string) (node: YamlNode) (requirement: Requirement) : unit =
        match requirement with
        | ToolTimeLimitRequirement (ToolTimeLimitSeconds seconds) when seconds < 0L ->
            addWarning warnings (path + ".timelimit") "The CWL specification calls a negative timelimit of a ToolTimeLimit an error, and cwltool accepts it." (Some node)
        | ResourceRequirement resources ->
            // The test reads each value as the type it holds. TryGetInt64 turns any number into an int64, and the JavaScript
            // target fails on a fractional number such as 4.5.
            let isNegative (value: obj) =
                match value with
                | :? int64 as number -> number < 0L
                | :? float as number -> number < 0.0
                | _ -> false
            for (name, value) in resources.KnownFieldValues do
                if Option.exists isNegative value then
                    addWarning warnings (path + "." + name) ("The CWL specification calls a negative " + name + " of a ResourceRequirement an error, and cwltool accepts it.") (Some node)
            // The specification calls a maximum below its minimum an error as well. An expression has no number to compare.
            let number (value: obj option) : float option =
                value
                |> Option.bind (fun held ->
                    match held with
                    | :? int64 as whole -> Some (float whole)
                    | :? float as fraction -> Some fraction
                    | _ -> None)
            for (name, minimum, maximum) in
                [ "cores", resources.CoresMin, resources.CoresMax
                  "ram", resources.RamMin, resources.RamMax
                  "tmpdir", resources.TmpdirMin, resources.TmpdirMax
                  "outdir", resources.OutdirMin, resources.OutdirMax ] do
                let low = number minimum
                let high = number maximum
                if low.IsSome && high.IsSome && high.Value < low.Value then
                    addWarning warnings (path + "." + name + "Max") ("The CWL specification calls a " + name + "Max below the " + name + "Min of a ResourceRequirement an error, and cwltool accepts it.") (Some node)
        | _ -> ()

    /// Decode requirements from either sequence syntax or class-name map syntax. The field is the name of the field that
    /// holds them in the process, as it stands in the path of a warning. An $import stands for the requirements of a
    /// file, and the field may be that directive as a whole.
    let requirementArrayDecoderWithScope (warnings: WarningSink) (field: string) (scope: DecodeScope) : YamlNode -> ResizeArray<Requirement> =
        fun yEle ->
            // helper: decode a single requirement object that contain 'class' field
            let decodeSingleRequirementObject (ele: YamlNode) : Requirement =
                Decode.object (fun get ->
                    // Sequence syntax carries class inside each item.
                    let classNode = get.Required.Field "class" (Decode.nonNull Decode.node)
                    let cls = atField "class" (fun () -> stringDecoder classNode)
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
                |> List.mapi (fun index item ->
                    atField $"[{index}]" (fun () ->
                        match objectDirective "an entry of the requirements" item.Node with
                        | Some (ImportedFile path) -> ImportedRequirement path
                        | Some (MixedMapping raw) -> MixedRequirement raw
                        // The file decides the class of a requirement whose class is a directive, so it stays as written.
                        | None when fieldIsDirective "class" item.Node -> RawRequirement { Node = item.Node }
                        | None ->
                            let requirement = decodeSingleRequirementObject item.Node
                            warnAboutRequirement warnings $"{field}[{index}]" item.Node requirement
                            requirement))
                |> ResizeArray

            // II:  OBJECT/MAP SYNTAX (also covers flow/JSON-style mapping)
                // requirements:
                //   DockerRequirement: { ... }
            | YamlNodeKind.Mapping _ when (importOnly "the requirements field" yEle).IsSome ->
                // The field is one directive, as in requirements: {$import: path}.
                ResizeArray [ ImportedRequirement (importOnly "the requirements field" yEle).Value ]
            | YamlNodeKind.Mapping _ ->
                mappingEntries yEle
                |> List.map (fun entry ->
                    // Map syntax uses the mapping key as the requirement class
                    // and the mapping value as that requirement's payload.
                    let name = idMapKey entry
                    atField name (fun () ->
                        rejectDirectiveValue "an entry of the requirements" entry
                        match mixedEntryOfMapValue "class" name entry.Value with
                        | Some raw -> MixedRequirement raw
                        | None ->
                        let requirement =
                            Decode.object (fun get ->
                                match tryRequirementFromTypeName scope name get with
                                | Some requirement -> addRequirementPayloadOverflow entry.Value requirement
                                | None -> failAt entry.Key ("Invalid or unsupported requirement class: " + name)
                            ) entry.Value
                        warnAboutRequirement warnings $"{field}.{name}" entry.Value requirement
                        requirement)
                )
                |> ResizeArray
            // INVALID CWL REQUIREMENTS
            | _ -> failAt yEle ("Expected a list or a mapping of requirements, found " + describeNode yEle + ".")

    /// Decode requirements from either sequence syntax or class-name map syntax, outside of any process.
    let requirementArrayDecoder : YamlNode -> ResizeArray<Requirement> =
        requirementArrayDecoderWithScope None "requirements" rootScope

    /// Decode one hint. An $import stands for the hints of a file, and the decoder checks for it before it reads the class,
    /// because the directive has none. A hint of a class that the model knows decodes with the decoder of the requirement of
    /// that class and fails with the same errors. A hint of any other class stays as it was written.
    let decodeHintElement (scope: DecodeScope) (element: YamlNode) : HintEntry =
        match objectDirective "an entry of the hints" element with
        | Some (ImportedFile path) -> ImportedHint path
        | Some (MixedMapping raw) -> MixedHint raw
        // The file decides the class of a hint whose class is a directive, so it stays as written.
        | None when fieldIsDirective "class" element -> RawHint { Node = element }
        | None ->
        Decode.object (fun get ->
            let cls = get.Required.Field "class" (Decode.nonNull stringDecoder)
            // A requirement of the same class overrides a SchemaDefRequirement hint in every chain, so no runner applies
            // its types. cwltool accepts the type names in such a hint without resolving them.
            let scope =
                if cls = "SchemaDefRequirement" && scope.Chains |> List.forall (fun chain -> (Option.orElse chain.Inherited chain.Own).IsSome) then
                    { scope with References = AnyNames }
                else
                    scope
            match tryRequirementFromTypeName scope cls get with
            | Some requirement -> KnownHint (addRequirementPayloadOverflow element requirement)
            | None -> UnknownHint (HintUnknownValue(Some cls, { Node = element }))
        ) element

    /// Warn about the requirement of a known hint, as warnAboutRequirement does.
    let private warnAboutHint (warnings: WarningSink) (path: string) (node: YamlNode) (hint: HintEntry) : HintEntry =
        match hint with
        | KnownHint requirement -> warnAboutRequirement warnings path node requirement
        | UnknownHint _
        | ImportedHint _
        | MixedHint _
        | RawHint _ -> ()
        hint

    /// Decode hints from sequence syntax or class-name map syntax while preserving unknown hints.
    let hintArrayDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : YamlNode -> ResizeArray<HintEntry> =
        fun yEle ->
            match yEle.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence hints can be known requirement payloads or arbitrary
                // extension hints.
                collection.Items
                |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeHintElement scope item.Node |> warnAboutHint warnings $"hints[{index}]" item.Node))
                |> ResizeArray
            | YamlNodeKind.Mapping _ when (importOnly "the hints field" yEle).IsSome ->
                // The field is one directive, as in hints: {$import: path}.
                ResizeArray [ ImportedHint (importOnly "the hints field" yEle).Value ]
            | YamlNodeKind.Mapping _ ->
                mappingEntries yEle
                |> List.map (fun entry ->
                    let hintName = idMapKey entry
                    atField hintName (fun () ->
                        rejectDirectiveValue "an entry of the hints" entry
                        // A hint without a payload has the same meaning as a hint with an empty one.
                        let payload = if YamlNode.isMissingValue entry.Value then YamlNode.flowMapping [] else entry.Value
                        match mixedEntryOfMapValue "class" hintName payload, payload.Kind with
                        | Some raw, _ -> MixedHint raw
                        | None, kind ->
                        match kind with
                        | YamlNodeKind.Mapping collection ->
                            // Schema Salad sets the key of the entry as the class of the hint and drops a class field in the
                            // value before it reads that field. A class field with any other content, such as a directive,
                            // changes nothing, so only a class field that already holds the key stays where it stands.
                            let isClassField (payloadEntry: MappingEntry) = entryName payloadEntry = "class"
                            let holdsKey = collection.Items |> List.exists (fun payloadEntry -> isClassField payloadEntry && scalarText payloadEntry.Value = Some hintName)
                            let valueWithClass =
                                if holdsKey then
                                    payload
                                else
                                    let classEntry = YamlNode.entry (YamlNode.plain "class") (ScalarQuoting.textNode hintName)
                                    { payload with Kind = YamlNodeKind.Mapping { collection with Items = classEntry :: (collection.Items |> List.filter (isClassField >> not)) } }
                            decodeHintElement scope valueWithClass |> warnAboutHint warnings $"hints.{hintName}" payload
                        | _ ->
                            failAt payload ("Expected a mapping for the hint " + hintName + ", found " + describeNode payload + "."))
                )
                |> ResizeArray
            | _ ->
                failAt yEle ("Expected a list or a mapping of hints, found " + describeNode yEle + ".")

    /// Decode hints from sequence syntax or class-name map syntax, outside of any process.
    let hintArrayDecoder : YamlNode -> ResizeArray<HintEntry> =
        hintArrayDecoderWithScope None rootScope

    /// Access the requirements field and decode the YamlNodes into a Requirement array
    let requirementsDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<Requirement> option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "requirements" (requirementArrayDecoderWithScope warnings "requirements" scope)
            requirements
        )

    /// Access the hints field and decode the YamlNodes into a HintEntry array
    let hintsDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<HintEntry> option) =
        Decode.object (fun get ->
            let hints = get.Optional.Field "hints" (hintArrayDecoderWithScope warnings scope)
            hints
        )

    /// Decode the requirements of a node that stands alone. The types that its SchemaDefRequirements define are the
    /// only ones that its other requirements may name.
    let requirementsDecoder: (YamlNode -> ResizeArray<Requirement> option) =
        fun node -> requirementsDecoderWithScope None (documentScope node) node

    /// Decode the hints of a node that stands alone, as requirementsDecoder decodes its requirements.
    let hintsDecoder: (YamlNode -> ResizeArray<HintEntry> option) =
        fun node -> hintsDecoderWithScope None (documentScope node) node

    /// Decode the position of an input binding: an int, or an expression that evaluates to one. A plain scalar that the
    /// core schema reads as an int is the number, and a string with $( or ${ in it is the expression. A directive
    /// stands for the number or the expression of a file.
    let bindingPositionDecoder (node: YamlNode) : BindingPosition =
        match textDirective BindingPosition.Imported BindingPosition.Included BindingPosition.Mixed node with
        | Some directive -> directive
        | None ->
            match node, scalarText node with
            | CoreInt _, _ -> BindingPosition.Number (Decode.int node)
            | _, Some text when containsExpression text -> BindingPosition.Expression text
            | _ -> failAt node ("Expected an int or an expression, found " + describeNode node + ".")

    /// Decode a YamlNode into the fields of an InputBinding
    let private inputBindingFieldsDecoder (value: YamlNode) : InputBinding =
        Decode.object (fun get' ->
            // InputBinding has several optional command-line
            // behavior fields. Absent fields remain None.
            let binding =
                InputBinding(
                    ?prefix = get'.Optional.Field "prefix" decodeSchemaSaladString,
                    ?position = get'.Optional.Field "position" bindingPositionDecoder,
                    ?itemSeparator = get'.Optional.Field "itemSeparator" decodeSchemaSaladString,
                    ?separate = get'.Optional.Field "separate" (schemaSaladBoolDecoder "the separate field"),
                    ?loadContents = get'.Optional.Field "loadContents" (schemaSaladBoolDecoder "the loadContents field"),
                    ?valueFrom = get'.Optional.Field "valueFrom" decodeSchemaSaladString,
                    ?shellQuote = get'.Optional.Field "shellQuote" (schemaSaladBoolDecoder "the shellQuote field")
                )
            overflowIntoDynamicObj binding (InputBinding.KnownFieldNames |> Seq.toList) value |> ignore
            binding
        ) value

    /// Decode a YamlNode into an InputBinding, or the directive that stands for it. A port in scalar shorthand has none.
    let inputBindingDecoder: (YamlNode -> ListEntry<InputBinding> option) =
        tryGetPresentField "inputBinding" (bindingEntryDecoder "the inputBinding field" inputBindingFieldsDecoder)

    /// Decode the format of an input: one name or a list of names. The format of an output admits one name only.
    let inputFormatDecoder (node: YamlNode) : InputFormat =
        match node.Kind with
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> atField $"[{index}]" (fun () -> decodeSchemaSaladString item.Node))
            |> ResizeArray
            |> InputFormat.Multiple
        | _ -> InputFormat.Single (decodeSchemaSaladString node)

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
        checkSecondaryFileObjects value
        // inputBinding is nested and decoded separately so both shorthand and object
        // input definitions can share the same construction flow.
        let inputBinding = inputBindingDecoder value
        let cwlType = Some (portTypeDecoder (withChildId name scope) value)
        match cwlType, inputBinding with
        | Some Stdin, Some _ ->
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
                ?type_ = cwlType,
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

    /// Decode a YamlNode into an Input array. An $import stands for the inputs of a file, in the list or as the whole
    /// field.
    let inputArrayDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) (path: string) : (YamlNode -> ResizeArray<ListEntry<CWLInput>>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form inputs must carry an id on each object item.
                warnRepeatedIds warnings path (collection.Items |> List.map (fun item -> item.Node))
                collection.Items
                |> List.mapi (fun index item ->
                    match atField $"[{index}]" (fun () -> objectDirective "an entry of the inputs" item.Node) with
                    | Some (ImportedFile importPath) -> Some (ListEntry.Imported importPath)
                    | Some (MixedMapping raw) -> Some (ListEntry.Mixed raw)
                    | None -> decodeInputSequenceItem warnings scope path index item.Node |> Option.map ListEntry.Value)
                |> List.choose id
                |> ResizeArray
            | YamlNodeKind.Mapping _ when (importOnly "the inputs field" value).IsSome ->
                ResizeArray [ ListEntry.Imported (importOnly "the inputs field" value).Value ]
            | _ ->
                // Map-form inputs use the map key as the input name.
                [|
                    for entry in mappingEntries value do
                        let name = idMapKey entry
                        atField name (fun () ->
                            rejectDirectiveValue "an entry of the inputs" entry
                            match mixedEntryOfMapValue "id" name entry.Value with
                            | Some raw -> ListEntry.Mixed raw
                            | None -> ListEntry.Value (decodeNamedInput scope name entry.Value))
                |]
                |> ResizeArray

    let inputArrayDecoder: (YamlNode -> ResizeArray<ListEntry<CWLInput>>) =
        inputArrayDecoderWithWarnings None rootScope "inputs"

    /// Access the inputs field and decode the YamlNodes into an Input array
    let inputsDecoderWithWarnings (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<ListEntry<CWLInput>> option) =
        Decode.object (fun get ->
            get.Optional.Field "inputs" (inputArrayDecoderWithWarnings warnings scope "inputs")
        )

    let inputsDecoder: (YamlNode -> ResizeArray<ListEntry<CWLInput>> option) =
        inputsDecoderWithWarnings None rootScope

    /// Decode the inputs of a process. CWL requires them for every class, so a missing inputs field is a decode error
    /// at the place of the process.
    let private requiredInputsDecoder (warnings: WarningSink) (scope: DecodeScope) (what: string) (yamlCWL: YamlNode) : ResizeArray<ListEntry<CWLInput>> =
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
    let baseCommandDecoder: (YamlNode -> ResizeArray<SchemaSaladString> option) =
        Decode.object (fun get ->
            get.Optional.Field "baseCommand" (fun baseCommandField ->
                match baseCommandField.Kind with
                | YamlNodeKind.Scalar _ ->
                    // Single string value
                    ResizeArray([ decodeSchemaSaladString baseCommandField ])
                | YamlNodeKind.Sequence _ ->
                    // Array of strings
                    Decode.resizearray decodeSchemaSaladString baseCommandField
                | _ ->
                    // A directive yields the string, and any other mapping is none.
                    match (match baseCommandField.Kind with YamlNodeKind.Mapping _ -> tryDirective baseCommandField | _ -> None) with
                    | Some directive -> ResizeArray([ directive ])
                    | None -> failAt baseCommandField ("Expected a string or a list of strings for baseCommand, found " + describeNode baseCommandField + ".")))

    let stringOptionFieldDecoder field : (YamlNode -> string option) =
        tryGetPresentField field stringDecoder

    let rawYamlOptionFieldDecoder field : (YamlNode -> RawYaml option) =
        tryGetPresentField field (fun node -> { Node = node })

    let stringFieldDecoder field : (YamlNode -> string) =
        Decode.object(fun get ->
            let fieldValue = get.Required.Field field (Decode.nonNull stringDecoder)
            fieldValue
        )

    /// Decode a YamlNode into a ResizeArray<SchemaSaladString> option for the source field
    /// Handles both single string values and arrays of strings
    let sourceArrayFieldDecoder field : (YamlNode -> ResizeArray<SchemaSaladString> option) =
        tryGetPresentField field stringsOrOneDecoder

    /// Decode and validate linkMerge enum fields. A directive stands for the symbol of a file.
    let linkMergeFieldDecoder field : (YamlNode -> LinkMergeMethod option) =
        tryGetPresentField field (
            symbolDecoder
                LinkMergeMethod.tryParse
                (fun _ text -> $"Invalid linkMerge value: {text}")
                ImportedLinkMerge
                IncludedLinkMerge
                MixedLinkMerge)

    /// Decode and validate pickValue enum fields. A directive stands for the symbol of a file.
    let pickValueFieldDecoder field : (YamlNode -> PickValueMethod option) =
        tryGetPresentField field (
            symbolDecoder
                PickValueMethod.tryParse
                (fun _ text -> $"Invalid pickValue value: {text}")
                ImportedPickValue
                IncludedPickValue
                MixedPickValue)

    let scatterFieldDecoder field : (YamlNode -> ResizeArray<SchemaSaladString> option) =
        tryGetPresentField field stringsOrOneDecoder

    /// Decode and validate scatterMethod enum fields. A directive stands for the symbol of a file.
    let scatterMethodFieldDecoder field : (YamlNode -> ScatterMethod option) =
        tryGetPresentField field (
            symbolDecoder
                ScatterMethod.tryParse
                (fun _ text -> $"Invalid scatterMethod value: {text}")
                ImportedScatterMethod
                IncludedScatterMethod
                MixedScatterMethod)

    /// Decode optional expression-like fields, including schema-salad directive wrappers.
    let expressionStringOptionFieldDecoder field : (YamlNode -> SchemaSaladString option) =
        tryGetPresentField field decodeSchemaSaladString

    /// Decode a step input from map or scalar shorthand and preserve unknown step-input fields.
    let decodeStepInputFromValue (id: string) (value: YamlNode) (allowScalarSource: bool) : StepInput =
        let scalarSource =
            match value.Kind with
            | YamlNodeKind.Mapping _ ->
                // A mapping is the step input itself, with a source field of its own.
                None
            | _ when allowScalarSource ->
                // Map-form step inputs may be `in: { target: source }`, where the
                // value itself is the source field.
                Some (stringsOrOneDecoder value)
            | _ ->
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
                ?valueFrom = expressionStringOptionFieldDecoder "valueFrom" value,
                ?linkMerge = linkMergeFieldDecoder "linkMerge" value,
                ?pickValue = pickValueFieldDecoder "pickValue" value,
                ?doc = doc,
                ?loadContents = tryGetBoolField "loadContents" value,
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
    let decodeStepInputsFromMap (value: YamlNode) : ResizeArray<ListEntry<StepInput>> =
        [|
            for entry in mappingEntries value do
                // A step input in map form needs a source or a mapping. A key without a value has neither.
                let name = idMapKey entry
                atField name (fun () ->
                    rejectDirectiveValue "an entry of the in field" entry
                    match mixedEntryOfMapValue "id" name (Decode.nonNull Decode.node entry.Value) with
                    | Some raw -> ListEntry.Mixed raw
                    | None -> ListEntry.Value (decodeStepInputFromValue name (Decode.nonNull Decode.node entry.Value) true))
        |]
        |> ResizeArray

    /// Decode sequence-form step inputs where the id must be present in the item.
    let decodeStepInputFromArrayItem (item: YamlNode) : StepInput =
        let id = stringFieldDecoder "id" item
        decodeStepInputFromValue id item false

    /// Decode sequence-form step inputs, warning and skipping only malformed unnamed entries. An $import stands for the
    /// step inputs of a file.
    let decodeStepInputsFromArrayWithWarnings (warnings: WarningSink) (path: string) (items: YamlNode list) : ResizeArray<ListEntry<StepInput>> =
        warnRepeatedIds warnings path items
        items
        |> List.mapi (fun index item ->
            match atField $"[{index}]" (fun () -> objectDirective "an entry of the in field" item) with
            | Some (ImportedFile importPath) -> Some (ListEntry.Imported importPath)
            | Some (MixedMapping raw) -> Some (ListEntry.Mixed raw)
            | None ->
                match tryGetStringField "id" item with
                // Items with ids are considered intended CWL step inputs and decode strictly.
                | Some id -> Some (ListEntry.Value (atField id (fun () -> decodeStepInputFromArrayItem item)))
                | None ->
                    // Unnamed malformed entries are public-workflow tolerance cases.
                    addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL step input entry." (Some item)
                    None)
        |> List.choose id
        |> ResizeArray

    let decodeStepInputsFromArray (items: YamlNode list) : ResizeArray<ListEntry<StepInput>> =
        decodeStepInputsFromArrayWithWarnings None "in" items

    /// Select the step input decoder based on CWL map-form versus sequence-form syntax. An $import mapping as the whole
    /// field stands for the step inputs of a file.
    let inputStepDecoderWithWarnings (warnings: WarningSink) (path: string) : (YamlNode -> ResizeArray<ListEntry<StepInput>>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                decodeStepInputsFromArrayWithWarnings warnings path (collection.Items |> List.map (fun item -> item.Node))
            | YamlNodeKind.Mapping _ when (importOnly "the in field of a step" value).IsSome ->
                ResizeArray [ ListEntry.Imported (importOnly "the in field of a step" value).Value ]
            | _ ->
                decodeStepInputsFromMap value

    let inputStepDecoder: (YamlNode -> ResizeArray<ListEntry<StepInput>>) =
        inputStepDecoderWithWarnings None "in"

    /// Decode a step output from the scalar shorthand or from the record form with extension fields. A directive stands
    /// for a file. A record with an id and a $mixin is the output that the id names, and a $mixin without an id stands
    /// for an entry of its own.
    let decodeStepOutputItem (value: YamlNode) : StepOutput =
        let record () =
            let id = stringFieldDecoder "id" value
            let output = StepOutputParameter.create id
            overflowIntoDynamicObj output (StepOutputParameter.KnownFieldNames |> Seq.toList) value |> ignore
            StepOutputRecord output
        match value.Kind with
        | YamlNodeKind.Scalar _ ->
            StepOutputString (Decode.nonNull stringDecoder value)
        | _ ->
            match tryDirective value with
            | Some (SchemaSaladString.Include path) -> StepOutputInclude path
            | Some (SchemaSaladString.Import path) -> StepOutputImport path
            | Some (SchemaSaladString.Literal id) -> StepOutputString id
            | Some (SchemaSaladString.Mixin _)
            | Some (SchemaSaladString.Mixed _) -> StepOutputMixed { Node = value }
            | None -> record ()

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
                | YamlNodeKind.Mapping _ when (importOnly "the out field of a step" outField).IsSome ->
                    ResizeArray [ StepOutputImport (importOnly "the out field of a step" outField).Value ]
                | _ ->
                    failAt outField ("Expected a list of outputs, found " + describeNode outField + "."))
        )

    let idDecoder: (YamlNode -> string option) =
        Decode.object (fun get -> get.Optional.Field "id" stringDecoder)

    let intentDecoder: (YamlNode -> ResizeArray<SchemaSaladString> option) =
        Decode.object (fun get -> get.Optional.Field "intent" (decodeSchemaSaladStringList "the intent field"))

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

    /// The name of an id as cwltool compares the outputs of a step with the outputs of its process: the part behind the
    /// last / of the fragment. The names r, #r and s/r stand for one output.
    let private shortName (id: string) : string =
        let fragment = match id.IndexOf '#' with | -1 -> id | hash -> id.Substring(hash + 1)
        match fragment.LastIndexOf '/' with
        | -1 -> fragment
        | slash -> fragment.Substring(slash + 1)

    /// The outputs that a step lists in its out field, each with the node to point at and the path inside the entry. An
    /// entry is the id of an output or an object with an id. An entry of another shape is no output name here, and the
    /// decoder of the out field reports it.
    let private listedOutputs (step: YamlNode) : (string * YamlNode * string list) list =
        valueOf "out" step
        |> Option.map itemsOf
        |> Option.defaultValue []
        |> List.mapi (fun index item ->
            match item.Kind with
            | YamlNodeKind.Scalar scalar -> Some (scalar.Value, item, [ $"[{index}]" ])
            | YamlNodeKind.Mapping _ ->
                valueOf "id" item |> Option.bind (fun idNode -> scalarText idNode |> Option.map (fun id -> id, idNode, [ $"[{index}]"; "id" ]))
            | _ -> None)
        |> List.choose id

    /// Fail at the first entry of the out field of a step that names no output of the process that the step runs.
    /// cwltool compares the short names of the ids, and it reports the step output that does not correspond to a tool
    /// output. The caller puts the path of the step in front of the path of the error.
    let private checkStepOutputs (outputs: ResizeArray<CWLOutput>) (step: YamlNode) : unit =
        let names = outputs |> Seq.map (fun output -> output.Name) |> List.ofSeq
        let known = names |> List.map shortName |> Set.ofList
        for (id, node, entryPath) in listedOutputs step do
            if not (Set.contains (shortName id) known) then
                let available = if names.IsEmpty then "The process has no outputs." else "The process has the outputs " + String.concat ", " names + "."
                atPath ("out" :: entryPath) (fun () ->
                    failAt node ("The out field lists the output " + id + ", and the process that the step runs has no output of that name. " + available))

    /// The outputs of a process, and none when an import stands among them, because the file that it names holds outputs
    /// that the decoder does not read. A process whose class is a directive has none either, because its outputs stand in
    /// the document as raw nodes.
    let private knownOutputs (processingUnit: CWLProcessingUnit) : ResizeArray<CWLOutput> option =
        let entries =
            match processingUnit with
            | CommandLineTool tool -> Some tool.Outputs
            | Workflow workflow -> Some workflow.Outputs
            | ExpressionTool tool -> Some tool.Outputs
            | Operation operation -> Some operation.Outputs
            | RawProcess _ -> None
        entries
        |> Option.bind (fun listed -> if listed |> Seq.forall (fun entry -> (ListEntry.tryValue entry).IsSome) then Some (ListEntry.values listed) else None)

    /// The outputs of the process that a run holds inline, and none for a reference to a file or to a process of the
    /// document.
    let private inlineRunOutputs (run: WorkflowStepRun) : ResizeArray<CWLOutput> option =
        match run with
        | RunString _
        | RunImport _
        | RunMixed _
        | RunRaw _ -> None
        | RunCommandLineTool _ -> WorkflowStepRunOps.tryGetTool run |> Option.bind (fun tool -> knownOutputs (CommandLineTool tool))
        | RunWorkflow _ -> WorkflowStepRunOps.tryGetWorkflow run |> Option.bind (fun workflow -> knownOutputs (Workflow workflow))
        | RunExpressionTool _ -> WorkflowStepRunOps.tryGetExpressionTool run |> Option.bind (fun tool -> knownOutputs (ExpressionTool tool))
        | RunOperation _ -> WorkflowStepRunOps.tryGetOperation run |> Option.bind (fun operation -> knownOutputs (Operation operation))

    /// Run the decoders of the fields that every process has on a process whose class is a directive, and drop what they read.
    /// The file of the directive decides the class, and every class takes these fields in one form, so cwltool refuses what
    /// these decoders refuse. The ports admit the fields of every class, so a field that only some classes have stays
    /// unread. The model keeps the process as it was written, and nothing is skipped, so no warning is collected.
    let private checkSharedFields (scope: DecodeScope) (yamlCWL: YamlNode) : unit =
        versionDecoder scope yamlCWL |> ignore
        let scope =
            { enterHolder ProcessHolder yamlCWL scope with
                Process = None
                StreamTypes = AnyStreams
                PortIds = Set.ofList (portSchemaIds scope.DocId yamlCWL) }
        outputsDecoderWithWarnings None scope yamlCWL |> ignore
        requiredInputsDecoder None scope "a process" yamlCWL |> ignore
        requirementsDecoderWithScope None scope yamlCWL |> ignore
        hintsDecoderWithScope None scope yamlCWL |> ignore
        intentDecoder yamlCWL |> ignore
        docAndLabelDecoder yamlCWL |> ignore
        idDecoder yamlCWL |> ignore

    /// Decode a workflow step run as either a path string or an inline CWL processing unit.
    let rec workflowStepRunDecoder (warnings: WarningSink) (scope: DecodeScope) (runValue: YamlNode) : WorkflowStepRun =
        match runValue.Kind with
        | YamlNodeKind.Scalar _ ->
            // Most workflow steps reference an external CWL file by path. A step of a packed document may refer to a
            // process of the document by #id, and then the id must belong to one of its processes.
            let reference = Decode.nonNull stringDecoder runValue
            match scope.GraphIds with
            | Some ids when scope.CheckSteps && reference.StartsWith "#" && not (Set.contains (LocalReference.withoutHash reference) ids) ->
                raise (
                    YamlDecodeException
                        { Message = "The run field refers to " + reference + ", which is no process of the $graph."
                          Path = []
                          Span = runValue.Span }
                )
            | _ -> RunString reference
        | YamlNodeKind.Mapping _ ->
            match objectDirective "the run field of a step" runValue with
            | Some (ImportedFile path) -> RunImport path
            | Some (MixedMapping raw) -> RunMixed raw
            | None ->
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
            // The file decides the class of a process whose class is a directive, so the run stays as written.
            | RawProcess raw -> RunRaw raw
        | _ ->
            failAt runValue ("Expected a path or a process for the run field of a step, found " + describeNode runValue + ".")

    /// Decode a named workflow step and thread warnings into nested step inputs and inline runs.
    and decodeWorkflowStepFromValueWithId (warnings: WarningSink) (scope: DecodeScope) (path: string) (stepId: string) (value: YamlNode) : WorkflowStep =
        // Schema Salad gives the step its id behind the id of the workflow. The definitions of its requirements and
        // its inline run are relative to that id, and its SchemaDefRequirement overrides the one of the workflow.
        let scope = scope |> withChildId stepId |> enterHolder StepHolder value
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
        // A step names the outputs of the process that it runs. The decoder reads no file, so only an inline process can
        // be checked here.
        if scope.CheckSteps then
            inlineRunOutputs run |> Option.iter (fun processOutputs -> checkStepOutputs processOutputs value)
        let requirements = requirementsDecoderWithScope warnings scope value
        let hints = hintsDecoderWithScope warnings scope value
        let doc, label, sourceNodes = docAndLabelDecoder value
        let scatter = scatterFieldDecoder "scatter" value
        let scatterMethod = scatterMethodFieldDecoder "scatterMethod" value
        let when_ = tryGetPresentField "when" (expressionTextDecoder "an expression") value
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
        wfStep.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "requirements"; "hints"; "in"; "out" ] value }
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
    and stepArrayDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<ListEntry<WorkflowStep>>) =
        fun value ->
            match value.Kind with
            | YamlNodeKind.Sequence collection ->
                // Sequence-form steps carry id in each item.
                warnRepeatedIds warnings "steps" (collection.Items |> List.map (fun item -> item.Node))
                collection.Items
                |> List.mapi (fun index item ->
                    match atField $"[{index}]" (fun () -> objectDirective "an entry of the steps" item.Node) with
                    | Some (ImportedFile importPath) -> Some (ListEntry.Imported importPath)
                    | Some (MixedMapping raw) -> Some (ListEntry.Mixed raw)
                    | None -> decodeWorkflowStepFromArrayItemWithWarnings warnings scope index item.Node |> Option.map ListEntry.Value)
                |> List.choose id
                |> ResizeArray
            | YamlNodeKind.Mapping _ when (importOnly "the steps field" value).IsSome ->
                ResizeArray [ ListEntry.Imported (importOnly "the steps field" value).Value ]
            | _ ->
                // Map-form steps use the map key as the step id.
                [|
                    for entry in mappingEntries value do
                        let key = idMapKey entry
                        atField key (fun () ->
                            rejectDirectiveValue "an entry of the steps" entry
                            match mixedEntryOfMapValue "id" key entry.Value with
                            | Some raw -> ListEntry.Mixed raw
                            | None -> ListEntry.Value (decodeWorkflowStepFromValueWithId warnings scope $"steps.{key}" key entry.Value))
                |]
                |> ResizeArray

    and stepsDecoderWithScope (warnings: WarningSink) (scope: DecodeScope) : (YamlNode -> ResizeArray<ListEntry<WorkflowStep>>) =
        Decode.object (fun get ->
            get.Required.Field "steps" (stepArrayDecoderWithScope warnings scope)
        )

    /// Decode a CommandLineTool and preserve unknown top-level metadata fields.
    and commandLineToolDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        // Decode standard sections first. Inputs and outputs are required.
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder ProcessHolder yamlCWL scope with Process = Some CommandLineToolClass; PortIds = Set.ofList (portSchemaIds scope.DocId yamlCWL) }
        // Only the ports of a command line tool may have a stream type.
        let outputs = outputsDecoderWithWarnings warnings { scope with StreamTypes = OutputStreams } yamlCWL
        let inputs = requiredInputsDecoder warnings { scope with StreamTypes = InputStreams } "a command line tool" yamlCWL
        // A stdin input reads the standard input, so the tool must not name a file for it as well.
        match tryGetPresentField "stdin" id yamlCWL with
        | Some stdinNode when inputs |> Seq.exists (fun input -> ListEntry.tryValue input |> Option.exists (fun port -> port.Type_ = Some Stdin)) ->
            atField "stdin" (fun () ->
                raise (
                    YamlDecodeException
                        { Message = "The tool sets stdin and has an input of the type stdin. CWL admits only one of them."
                          Path = []
                          Span = stdinNode.Span }
                ))
        | _ -> ()
        let requirements = requirementsDecoderWithScope warnings scope yamlCWL
        let hints = hintsDecoderWithScope warnings scope yamlCWL
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
                ?stdin = tryGetPresentField "stdin" decodeSchemaSaladString yamlCWL,
                ?stderr = tryGetPresentField "stderr" decodeSchemaSaladString yamlCWL,
                ?stdout = tryGetPresentField "stdout" decodeSchemaSaladString yamlCWL,
                ?successCodes = tryGetIntListField "successCodes" yamlCWL,
                ?temporaryFailCodes = tryGetIntListField "temporaryFailCodes" yamlCWL,
                ?permanentFailCodes = tryGetIntListField "permanentFailCodes" yamlCWL
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
        description.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "requirements"; "hints"; "inputs"; "outputs"; "intent"; "successCodes"; "temporaryFailCodes"; "permanentFailCodes" ] yamlCWL }
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Decode an ExpressionTool and preserve unknown top-level metadata fields.
    and expressionToolDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        // ExpressionTool shares port/requirement metadata decoding with other
        // processing units, then decodes its required expression body.
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder ProcessHolder yamlCWL scope with Process = Some ExpressionToolClass; PortIds = Set.ofList (portSchemaIds scope.DocId yamlCWL) }
        let outputs = outputsDecoderWithWarnings warnings scope yamlCWL
        let inputs = requiredInputsDecoder warnings scope "an expression tool" yamlCWL
        let requirements = requirementsDecoderWithScope warnings scope yamlCWL
        let hints = hintsDecoderWithScope warnings scope yamlCWL
        let intent = intentDecoder yamlCWL
        let doc, label, sourceNodes = docAndLabelDecoder yamlCWL
        let expression =
            Decode.object (fun get -> get.Required.Field "expression" (expressionTextDecoder "an expression")) yamlCWL
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
        description.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "requirements"; "hints"; "inputs"; "outputs"; "intent" ] yamlCWL }
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Decode an Operation and enforce its required inputs collection.
    and operationDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder ProcessHolder yamlCWL scope with Process = Some OperationClass; PortIds = Set.ofList (portSchemaIds scope.DocId yamlCWL) }
        let outputs = outputsDecoderWithWarnings warnings scope yamlCWL
        let inputs = requiredInputsDecoder warnings scope "an operation" yamlCWL
        let requirements = requirementsDecoderWithScope warnings scope yamlCWL
        let hints = hintsDecoderWithScope warnings scope yamlCWL
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
        description.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "requirements"; "hints"; "inputs"; "outputs"; "intent" ] yamlCWL }
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Decode a Workflow, including version-aware steps and preserved metadata fields.
    and workflowDecoder (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        // Workflow inputs and steps are required by this model; outputs and steps
        // can still use tolerant collection decoders internally.
        let cwlVersion = versionDecoder scope yamlCWL
        // The SchemaDefRequirement of the process overrides the one of the enclosing process.
        let scope = { enterHolder ProcessHolder yamlCWL scope with Process = Some WorkflowClass; PortIds = Set.ofList (portSchemaIds scope.DocId yamlCWL) }
        let outputs = outputsDecoderWithWarnings warnings scope yamlCWL
        let inputs = requiredInputsDecoder warnings scope "a workflow" yamlCWL
        let requirements = requirementsDecoderWithScope warnings scope yamlCWL
        let hints = hintsDecoderWithScope warnings scope yamlCWL
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
        description.SourceNodes <- { sourceNodes with WholeFields = wholeDirectiveFields [ "requirements"; "hints"; "inputs"; "outputs"; "steps"; "intent" ] yamlCWL }
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    /// Dispatch a parsed CWL document to the processing-unit decoder declared by its class field.
    and decodeCWLProcessingUnitElementWithWarnings (warnings: WarningSink) (scope: DecodeScope) (yamlCWL: YamlNode) =
        let classNode = Decode.object (fun get -> get.Required.Field "class" (Decode.nonNull Decode.node)) yamlCWL
        // cwltool reads a process whose class is a directive. The class tells which of the four processes the model builds,
        // and the decoder reads no file, so the process stays as written. The fields that every process has are checked, as
        // for any process.
        if fieldIsDirective "class" yamlCWL then
            checkSharedFields scope yamlCWL
            RawProcess { Node = yamlCWL }
        else
        let cls = atField "class" (fun () -> stringDecoder classNode)
        match cls with
        // The class field is the only discriminator at this layer; individual
        // decoders handle version, ports, requirements, and metadata.
        | "CommandLineTool" -> CommandLineTool (commandLineToolDecoder warnings scope yamlCWL)
        | "Workflow" -> Workflow (workflowDecoder warnings scope yamlCWL)
        | "ExpressionTool" -> ExpressionTool (expressionToolDecoder warnings scope yamlCWL)
        | "Operation" -> Operation (operationDecoder warnings scope yamlCWL)
        | _ -> atField "class" (fun () -> failAt classNode ("Invalid or unsupported CWL class: " + cls))

    /// Decode an already-parsed CWL element without warning collection. The tree has to pass the same check as a text, so the
    /// function applies rejectWhatTheSpecProhibits to it.
    let decodeCWLProcessingUnitElement (yamlCWL: YamlNode) =
        rejectWhatTheSpecProhibits yamlCWL
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

    /// A step that runs another process of the packed document by #id.
    type private GraphRun =
        { /// The path to the run field of the step.
          RunPath: string list
          /// The id of the process that the step runs, without the hash.
          Target: string
          /// The node of the step.
          Step: YamlNode
          /// The node of the run field.
          Run: YamlNode }

    /// The steps of a process that run another process of the document by #id. The walk descends into the inline runs of
    /// the steps.
    let rec private runsIn (path: string list) (processNode: YamlNode) : GraphRun list =
        stepsOf processNode
        |> List.collect (fun (stepId, step) ->
            match valueOf "run" step with
            | Some run ->
                let runPath = path @ [ "steps"; stepId; "run" ]
                match run.Kind with
                | YamlNodeKind.Scalar scalar when scalar.Value.StartsWith "#" -> [ { RunPath = runPath; Target = LocalReference.withoutHash scalar.Value; Step = step; Run = run } ]
                | YamlNodeKind.Mapping _ -> runsIn runPath run
                | _ -> []
            | None -> [])

    /// The chains of SchemaDefRequirements that reach each process of a packed document. A step passes its own chains to
    /// the process that it runs by #id, so a process has the chains of all steps that run it. The process main starts
    /// the document, so it has a chain without a fallback, as a root process has, and the processes that its steps reach
    /// inherit that. Any other process that no step runs has the fallback of every type of the document, because a
    /// runner may start with it. The walk reads the raw nodes with the same scope steps as the decoders. Processes that
    /// run each other in a cycle can only be out of reach of the process main, which the decoder rejects otherwise, and
    /// they take the fallback.
    let private chainsOfProcesses (graphScope: DecodeScope) (processes: (string option * YamlNode) list) (start: int option) (callers: Map<string, int list>) : SchemaDefChain list[] =
        let members = List.toArray processes
        let everyType = { Ids = graphScope.Definitions; Imports = graphScope.ImportedPaths; Open = false }
        let rec runsFrom (scope: DecodeScope) (processNode: YamlNode) : (string * SchemaDefChain list) list =
            let scope = enterHolder ProcessHolder processNode scope
            stepsOf processNode
            |> List.collect (fun (stepId, step) ->
                let stepScope = scope |> withChildId stepId |> enterHolder StepHolder step
                match valueOf "run" step with
                | Some run ->
                    match run.Kind with
                    | YamlNodeKind.Scalar scalar when scalar.Value.StartsWith "#" -> [ LocalReference.withoutHash scalar.Value, stepScope.Chains ]
                    | YamlNodeKind.Mapping _ -> runsFrom { stepScope with DocId = processId stepScope.DocId "run" run } run
                    | _ -> []
                | None -> [])
        let entries = System.Collections.Generic.Dictionary<int, SchemaDefChain list>()
        let unfinished = System.Collections.Generic.HashSet<int>()
        let fallbackChains = [ { Inherited = None; Own = None; Hinted = None; Fallback = everyType } ]
        let sites = System.Collections.Generic.Dictionary<int, (string * SchemaDefChain list) list>()
        // A member is out of reach of the steps when it has no id and when a later member has its id, because a step reaches the last one.
        let rec chainsOf (index: int) : SchemaDefChain list =
            match entries.TryGetValue index with
            | true, chains -> chains
            | _ when unfinished.Contains index -> fallbackChains
            | _ ->
                unfinished.Add index |> ignore
                let id = fst members.[index]
                let fromRunner = if Some index = start then [ { Inherited = None; Own = None; Hinted = None; Fallback = { Ids = Set.empty; Imports = Set.empty; Open = false } } ] else []
                let fromSteps =
                    match id with
                    | Some id when GraphMembers.tryReach id (Array.map fst members) = Some index ->
                        Map.tryFind id callers
                        |> Option.defaultValue []
                        |> List.collect (fun caller -> sitesOf caller |> List.filter (fun (target, _) -> target = id) |> List.collect snd)
                    | _ -> []
                let chains =
                    match fromRunner @ fromSteps with
                    | [] -> fallbackChains
                    | found -> List.distinct found
                unfinished.Remove index |> ignore
                entries.[index] <- chains
                chains
        and sitesOf (index: int) : (string * SchemaDefChain list) list =
            match sites.TryGetValue index with
            | true, found -> found
            | _ ->
                let node = snd members.[index]
                let found = runsFrom { graphScope with DocId = processId "" "" node; Chains = chainsOf index } node
                sites.[index] <- found
                found
        members |> Array.mapi (fun index _ -> chainsOf index)

    /// Decode a packed document. Its processes use the cwlVersion of the root and ignore their own, and a step names a
    /// process of the document by #id. The scope of each process holds the ids and the type definitions of the whole
    /// document, because a workflow may run a process that stands behind it in the list and a process may name a type
    /// that another one defines by its id, as main/Point or #main/Point. Which of these types apply to a process
    /// follows from the steps that run it. A process takes the SchemaDefRequirements in force at each of those steps,
    /// and the process main takes none, as a root process does. A process that no step runs and that is not main takes
    /// every type of the document. The caller has applied rejectWhatTheSpecProhibits to the tree.
    let private decodeCWLGraph (warnings: WarningSink) (yamlCWL: YamlNode) : CWLGraph =
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
        // A step names a process by its id, so a process without an id is out of its reach. cwltool accepts such a
        // process, and it accepts two processes with one id with the warning that the id was previously defined. A
        // step then runs the last of them, so the steps are checked against every process behind an id.
        let idList : string option list =
            processNodes
            |> List.mapi (fun index node ->
                match valueOf "id" node with
                | None
                | Some CoreNull -> None
                | Some idNode ->
                    match scalarText idNode with
                    | Some id -> Some (LocalReference.withoutHash id)
                    | None ->
                        atField "$graph" (fun () ->
                            atField $"[{index}]" (fun () ->
                                raise (YamlDecodeException { Message = "Expected the id of a process, found " + describeNode idNode + "."; Path = []; Span = idNode.Span }))))
        warnRepeatedIds warnings "$graph" processNodes
        let ids = idList |> List.choose id |> Set.ofList
        let nodes = List.toArray processNodes
        let idsByIndex = List.toArray idList
        // The position of the process that a run of the form #id reaches.
        let membersBehind (target: string) : int list = GraphMembers.tryReach target idList |> Option.toList
        // cwltool runs the process main of a packed document, or the one process of a document that holds one. It builds the
        // process tree from there, and it checks the steps of that tree only. A document with several processes and no main
        // has no tree, so the steps of its processes meet the schema only.
        let roots : int list =
            match GraphMembers.tryStart idList with
            | Some start -> [ start ]
            | None -> if nodes.Length = 1 then [ 0 ] else []
        let reachable =
            let seen = System.Collections.Generic.HashSet<int>()
            let rec reach (index: int) : unit =
                if seen.Add index then
                    for run in runsIn [] nodes.[index] do
                        for targetIndex in membersBehind run.Target do
                            reach targetIndex
            roots |> List.iter reach
            Set.ofSeq seen
        // A run that leads back to a process on its way never ends. The run field that closes the cycle is the error.
        let visiting = System.Collections.Generic.Dictionary<int, bool>()
        let rec visit (stack: int list) (index: int) : unit =
            visiting.[index] <- true
            for { RunPath = runPath; Target = target; Run = runNode } in runsIn [] nodes.[index] do
                for targetIndex in membersBehind target do
                    match visiting.TryGetValue targetIndex with
                    | true, true ->
                        let cycle = (index :: stack) |> List.rev |> List.skipWhile (fun step -> step <> targetIndex)
                        let names = (cycle @ [ targetIndex ]) |> List.map (fun step -> "#" + defaultArg idsByIndex.[step] "") |> String.concat ", "
                        let raiseAt () =
                            raise (
                                YamlDecodeException
                                    { Message = "The run field closes a cycle of run references (" + names + "). A process cannot run itself."
                                      Path = []
                                      Span = runNode.Span }
                            )
                        (List.foldBack (fun segment (inner: unit -> unit) -> (fun () -> atField segment inner)) ([ "$graph"; $"[{index}]" ] @ runPath) raiseAt) ()
                    | true, false -> ()
                    | _ -> visit (index :: stack) targetIndex
            visiting.[index] <- false
        for index in roots do
            if not (visiting.ContainsKey index) then visit [] index
        let graphScope = { documentScope yamlCWL with DefaultCwlVersion = Some cwlVersion; GraphIds = Some ids; DocId = "" }
        // A process takes the types in force at each step that runs it, so the steps that run it are known before it decodes.
        let callers =
            [ for index in 0 .. nodes.Length - 1 do
                for run in runsIn [] nodes.[index] do
                    if not (membersBehind run.Target).IsEmpty then
                        yield run.Target, index ]
            |> List.groupBy fst
            |> List.map (fun (target, runs) -> target, runs |> List.map snd |> List.distinct)
            |> Map.ofList
        let chains = chainsOfProcesses graphScope (List.zip idList processNodes) (GraphMembers.tryStart idList) callers
        let processes =
            processNodes
            |> List.mapi (fun index node ->
                atField "$graph" (fun () ->
                    atField $"[{index}]" (fun () ->
                        decodeCWLProcessingUnitElementWithWarnings warnings { graphScope with DocId = processId "" "" node; Chains = chains.[index]; CheckSteps = reachable.Contains index } node)))
            |> ResizeArray
        // A step names the outputs of the process that it runs. The members are decoded now, so a step that runs one of
        // them by #id can be checked, wherever the member stands in the list.
        for index in reachable do
            for run in runsIn [] nodes.[index] do
                match membersBehind run.Target with
                | [] -> ()
                | targets ->
                    let stepPath = run.RunPath |> List.take (run.RunPath.Length - 1)
                    let known = targets |> List.map (fun target -> knownOutputs processes.[target])
                    if known |> List.forall Option.isSome then
                        let outputs = known |> Seq.collect Option.get |> ResizeArray
                        atPath ([ "$graph"; $"[{index}]" ] @ stepPath) (fun () -> checkStepOutputs outputs run.Step)
        let graph = CWLGraph(processes, cwlVersion = cwlVersion)
        overflowIntoDynamicObj graph (CWLGraph.KnownFieldNames |> Seq.toList) yamlCWL |> ignore
        graph

    /// Decode an already-parsed packed document and return the graph. The tree has to pass the same check as a text, so the
    /// function applies rejectWhatTheSpecProhibits to it.
    let decodeCWLGraphElementWithWarnings (warnings: WarningSink) (yamlCWL: YamlNode) : CWLGraph =
        rejectWhatTheSpecProhibits yamlCWL
        decodeCWLGraph warnings yamlCWL

    /// Decode a CWL document, which is a process or a packed document, from a YAML string and return recoverable
    /// warnings.
    let decodeCWLDocumentWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readYaml cwl
        let document =
            if hasField "$graph" yamlCWL then
                CWLDocument.Graph (decodeCWLGraph (Some warnings) yamlCWL)
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
            | Decode.CoreBigInt text ->
                // The model holds an int of a job file as a long, and a float would change the value without a word.
                raise (
                    YamlDecodeException
                        { Message = "The int " + text + " in a job file is beyond 64 bits, and the model holds an int as a long."
                          Path = []
                          Span = value.Span }
                )
            | Decode.CoreFloat parsed -> CWLParameterValue.Float parsed
            | Decode.CoreText -> CWLParameterValue.String (YAMLicious.Decode.string value)
        | YamlNodeKind.Sequence collection ->
            collection.Items
            |> List.mapi (fun index item -> Decode.atField $"[{index}]" (fun () -> decodeParameterValue item.Node))
            |> ResizeArray
            |> CWLParameterValue.Array
        | YamlNodeKind.Mapping _ ->
            match tryField "class" value |> Option.bind tryScalarString with
            | Some "File" -> decodeFileInstance value |> CWLParameterValue.File
            | Some "Directory" -> decodeDirectoryInstance value |> CWLParameterValue.Directory
            | _ ->
                Decode.mappingEntries value
                |> List.map (fun entry ->
                    let name = Decode.entryName entry
                    CWLParameterRecordField(name, Decode.atField name (fun () -> decodeParameterValue entry.Value)))
                |> ResizeArray
                |> CWLParameterValue.Record
        | YamlNodeKind.Alias name ->
            Decode.failAlias name value

    /// A job file gives each input a plain CWL value. An object with the fields type and value is a record with these
    /// two fields, and the type of the input comes from the process.
    let cwlParameterReferenceDecoder (key: string) (yEle: YamlNode): CWLParameterReference =
        CWLParameterReference(key = key, value = Decode.atField key (fun () -> decodeParameterValue yEle))

    /// Decode every top-level parameter entry into CWLParameterReference values. The caller has applied
    /// rejectWhatTheSpecProhibits to the tree.
    let private referencesOfRoot: YamlNode -> ResizeArray<CWLParameterReference> =
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

    /// Decode the root of an already-parsed job file into parameter references. The tree has to pass the same check as a text, so
    /// the function applies rejectWhatTheSpecProhibits to it.
    let cwlparameterReferenceArrayDecoder: YamlNode -> ResizeArray<CWLParameterReference> =
        fun root ->
            Decode.rejectWhatTheSpecProhibits root
            referencesOfRoot root

    /// Decode a YAML parameter file string into parameter references.
    let decodeYAMLParameterFile (yaml: string) =
        referencesOfRoot (Decode.readYaml yaml)
