namespace ARCtrl.CWL

open YAMLicious
open YAMLicious.YAMLiciousTypes
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

    type private WarningSink = ResizeArray<DecodeWarning> option

    let private addWarning (warnings: WarningSink) (path: string) (message: string) (raw: YAMLElement option) =
        match warnings with
        | Some warningList ->
            warningList.Add({
                Path = path
                Message = message
                Raw = raw |> Option.map (sprintf "%A")
            })
        | None ->
            ()

    let countLeadingSpaces (line: string) =
        line |> Seq.takeWhile (fun c -> c = ' ') |> Seq.length

    let isBlankLine (line: string) =
        line.Trim().Length = 0

    let normalizeLineEndings (yaml: string) =
        if isNull yaml then "" else yaml.Replace("\r\n", "\n")

    let stripLeadingShebang (yaml: string) =
        let normalized = normalizeLineEndings yaml
        let lines = normalized.Split('\n')
        if lines.Length > 0 && lines.[0].StartsWith("#!") then
            lines.[1..] |> String.concat "\n"
        else
            normalized

    let tryParseBlockScalarHeader (line: string) : int option =
        if isBlankLine line then
            None
        else
            let trimmed = line.TrimEnd()
            // Match common block scalar headers:
            //   key: |
            //   key: >-
            //   - |
            //   - >+
            // and preserve surrounding comments.
            let isBlockScalarHeader =
                System.Text.RegularExpressions.Regex.IsMatch(
                    trimmed,
                    @"^(?:.+:\s*[|>][1-9]?[+-]?\s*(?:#.*)?|-\s*[|>][1-9]?[+-]?\s*(?:#.*)?)$"
                )
            if isBlockScalarHeader then Some (countLeadingSpaces line) else None

    let normalizeYamlInput (yaml: string) =
        let normalized = stripLeadingShebang yaml
        let lines = normalized.Split('\n')

        let filtered = ResizeArray<string>()
        let mutable blockScalarIndent : int option = None

        let rec processLine (line: string) =
            match blockScalarIndent with
            | Some indent ->
                if isBlankLine line then
                    // Preserve whitespace-only blank content lines in block scalars.
                    filtered.Add line
                else
                    let currentIndent = countLeadingSpaces line
                    if currentIndent > indent then
                        filtered.Add line
                    else
                        // End of block scalar; re-process this line in normal mode.
                        blockScalarIndent <- None
                        processLine line
            | None ->
                match tryParseBlockScalarHeader line with
                | Some indent ->
                    blockScalarIndent <- Some indent
                    filtered.Add line
                | None ->
                    if line = "" || line.Trim().Length > 0 then
                        filtered.Add line

        lines |> Array.iter processLine

        filtered
        |> Seq.toArray
        |> String.concat "\n"
        |> fun text -> text.TrimEnd()

    let removeFullLineComments (yaml: string) =
        yaml.Split('\n')
        |> Array.filter (fun line -> line.TrimStart().StartsWith("#") |> not)
        |> String.concat "\n"

    let rec removeYamlComments (yamlElement: YAMLElement) : YAMLElement =
        match yamlElement with
        | YAMLElement.Object elements ->
            elements
            |> List.choose (fun element ->
                match element with
                | YAMLElement.Comment _ -> None
                | other -> Some (removeYamlComments other)
            )
            |> YAMLElement.Object
        | YAMLElement.Sequence elements ->
            elements
            |> List.choose (fun element ->
                match removeYamlComments element with
                | YAMLElement.Comment _ -> None
                // YAML comments inside sequences can be represented as empty objects.
                // Remove these placeholders to keep sequence decoders stable.
                | YAMLElement.Object [] -> None
                | other -> Some other
            )
            |> YAMLElement.Sequence
        | YAMLElement.Mapping (key, value) ->
            YAMLElement.Mapping (key, removeYamlComments value)
        | other ->
            other

    /// Determines if an exception represents a recoverable decoding error.
    /// Returns true for schema mismatches; false for system errors that should propagate.
    let isRecoverableDecodingError (ex: exn) : bool =
        match ex with
        // Type-based matching for known exception types
        | :? System.Collections.Generic.KeyNotFoundException -> true
        | :? System.ArgumentException -> true
        | :? System.FormatException -> true
        | :? System.InvalidOperationException
            when ex.Message.Contains("decode") -> true
        // Message-based fallback for library-specific exceptions
        | _ when ex.Message.Contains("Expected") -> true
        | _ when ex.Message.Contains("Required") -> true
        // All other exceptions (including system-critical) should propagate
        | _ -> false

    let readSanitizedYaml (yaml: string) =
        let prepared = stripLeadingShebang yaml
        let tryRead text =
            text
            |> Decode.read
            |> removeYamlComments
        try
            tryRead prepared
        with ex when isRecoverableDecodingError ex ->
            let normalized = normalizeYamlInput prepared
            try
                tryRead normalized
            with ex2 when isRecoverableDecodingError ex2 ->
                normalized
                |> removeFullLineComments
                |> tryRead

    /// Decode key value pairs into a dynamic object, while preserving their tree structure.
    let rec overflowDecoder (dynObj: DynamicObj) (dict: System.Collections.Generic.Dictionary<string,YAMLElement>) =
        let rec decodeOverflowValue (value: YAMLElement) : obj =
            match value with
            | YAMLElement.Value v
            | YAMLElement.Object [YAMLElement.Value v] ->
                box v.Value
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                let decodedItems =
                    items
                    |> List.map decodeOverflowValue
                    |> ResizeArray
                if decodedItems.Count = 1 then
                    decodedItems.[0]
                else
                    box decodedItems
            | YAMLElement.Object _ ->
                let nested = DynamicObj()
                value
                |> Decode.object (fun get -> get.Overflow.FieldList [])
                |> overflowDecoder nested
                |> box
            | other ->
                box other

        for e in dict do
            DynObj.setProperty e.Key (decodeOverflowValue e.Value) dynObj
        dynObj

    let private isIgnorableYamlNoise (value: YAMLElement) =
        match value with
        | YAMLElement.Comment _ -> true
        | YAMLElement.Object [] -> true
        | _ -> false

    let private tryGetStringField (fieldName: string) (value: YAMLElement) =
        try
            Decode.object (fun get -> get.Optional.Field fieldName Decode.string) value
        with ex when isRecoverableDecodingError ex ->
            None

    let private tryGetBoolField (fieldName: string) (value: YAMLElement) =
        try
            Decode.object (fun get -> get.Optional.Field fieldName Decode.bool) value
        with ex when isRecoverableDecodingError ex ->
            None

    let private tryGetYamlField (fieldName: string) (value: YAMLElement) =
        try
            Decode.object (fun get -> get.Optional.Field fieldName id) value
        with ex when isRecoverableDecodingError ex ->
            None

    let private tryGetIntArrayField (fieldName: string) (value: YAMLElement) =
        try
            Decode.object (fun get -> get.Optional.Field fieldName (Decode.resizearray Decode.int)) value
        with ex when isRecoverableDecodingError ex ->
            None

    let private tryGetInt64Field (fieldName: string) (value: YAMLElement) =
        let decodeInt64 = function
            | YAMLElement.Value scalar
            | YAMLElement.Object [YAMLElement.Value scalar] ->
                match System.Int64.TryParse(scalar.Value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with
                | true, parsed -> parsed
                | false, _ -> raise (System.ArgumentException($"Invalid int64 value for {fieldName}: {scalar.Value}"))
            | other -> raise (System.ArgumentException($"Invalid int64 value for {fieldName}: {other}"))
        try
            Decode.object (fun get -> get.Optional.Field fieldName decodeInt64) value
        with ex when isRecoverableDecodingError ex ->
            None

    let private tryGetLoadListingField (fieldName: string) (value: YAMLElement) =
        tryGetStringField fieldName value
        |> Option.map (fun loadListingValue ->
            match LoadListingEnum.tryParse loadListingValue with
            | Some parsed -> parsed
            | None -> raise (System.ArgumentException($"Invalid loadListing value '{loadListingValue}'. Expected one of: no_listing, shallow_listing, deep_listing.")))

    let overflowIntoDynamicObj (dynObj: DynamicObj) (knownFields: string list) (value: YAMLElement) =
        match value with
        | YAMLElement.Object _ ->
            value
            |> Decode.object (fun get -> overflowDecoder dynObj (get.Overflow.FieldList knownFields))
            |> ignore
        | _ ->
            ()
        dynObj

    let private decodeFileInstanceFields (element: YAMLElement) =
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

    let private decodeDirectoryInstanceFields (element: YAMLElement) =
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
    let decodeSchemaSaladString (yEle:YAMLElement) : SchemaSaladString =
        match yEle with
        | YAMLElement.Value v
        | YAMLElement.Object [YAMLElement.Value v] ->
            SchemaSaladString.Literal v.Value
        | YAMLElement.Object [YAMLElement.Mapping (c, YAMLElement.Value v)]
        | YAMLElement.Object [YAMLElement.Mapping (c, YAMLElement.Object [YAMLElement.Value v])] ->
            match c.Value with
            | "$include" -> SchemaSaladString.Include v.Value
            | "$import" -> SchemaSaladString.Import v.Value
            | _ -> SchemaSaladString.Literal (sprintf "%s: %s" c.Value v.Value)
        | _ -> raise (System.ArgumentException($"Unexpected YAMLElement format in decodeSchemaSaladString: %A{yEle}"))

    /// Decode a YAMLElement which is either a string or expression into a string.
    /// Directive objects such as {$include: path} are represented using legacy string form.
    let decodeStringOrExpression (yEle:YAMLElement) =
        decodeSchemaSaladString yEle
        |> SchemaSaladString.toDirectiveString

    /// Decode a YAMLElement into a glob search pattern for output binding
    let outputBindingGlobDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding) =
        fun value ->
            Decode.object (fun get ->
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

    /// Decode a YAMLElement into an OutputBinding
    let outputBindingDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding option) =
        Decode.object(fun get ->
            let outputBinding = get.Optional.Field "outputBinding" outputBindingGlobDecoder
            outputBinding
        )

    let decodeStringArrayOrScalar (value: YAMLElement) : ResizeArray<string> =
        match value with
        | YAMLElement.Object [YAMLElement.Sequence items]
        | YAMLElement.Sequence items ->
            items
            |> List.map decodeStringOrExpression
            |> ResizeArray
        | _ ->
            ResizeArray [| decodeStringOrExpression value |]

    let outputSourceDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<string> option) =
        Decode.object(fun get ->
            get.Optional.Field "outputSource" decodeStringArrayOrScalar
        )

    /// Decode a YAMLElement into a Dirent
    let direntDecoder: (YAMLiciousTypes.YAMLElement -> CWLType) =
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
    let initialWorkDirEntryDecoder: (YAMLiciousTypes.YAMLElement -> InitialWorkDirEntry) =
        fun value ->
            let decodeFileInstance (element: YAMLElement) =
                decodeFileInstanceFields element

            let decodeDirectoryInstance (element: YAMLElement) =
                decodeDirectoryInstanceFields element

            match value with
            | YAMLElement.Object mappings ->
                let hasEntryField =
                    mappings
                    |> List.exists (function
                        | YAMLElement.Mapping (k, _) when k.Value = "entry" -> true
                        | _ -> false
                    )

                if hasEntryField then
                    match direntDecoder value with
                    | Dirent dirent -> DirentEntry dirent
                    | _ -> raise (System.ArgumentException("Unexpected InitialWorkDir Dirent decoding result."))
                else
                    let classValue =
                        mappings
                        |> List.tryPick (function
                            | YAMLElement.Mapping (k, YAMLElement.Object [YAMLElement.Value v]) when k.Value = "class" -> Some v.Value
                            | YAMLElement.Mapping (k, YAMLElement.Value v) when k.Value = "class" -> Some v.Value
                            | _ -> None
                        )
                    match classValue with
                    | Some "File" ->
                        FileEntry (decodeFileInstance value)
                    | Some "Directory" ->
                        DirectoryEntry (decodeDirectoryInstance value)
                    | _ ->
                        StringEntry (decodeSchemaSaladString value)
            | YAMLElement.Value _
            | YAMLElement.Object [YAMLElement.Value _] ->
                StringEntry (decodeSchemaSaladString value)
            | _ ->
                raise (System.ArgumentException($"Invalid InitialWorkDir listing entry: %A{value}"))

    /// Decode the contained type of a CWL Array
    let rec cwlSimpleTypeFromString (s: string) =
        match s with
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
        | _ -> raise (System.ArgumentException($"Invalid CWL simple type: {s}"))


    /// Recursively parse array shorthand notation (File[][], string[][][], etc.)
    let rec parseArrayShorthand (typeStr: string) : CWLType option =
        if typeStr.EndsWith("[]") then
            let innerType = typeStr.Substring(0, typeStr.Length - 2)
            // Try to parse the inner type recursively
            match parseArrayShorthand innerType with
            | Some innerCwlType ->
                // Nested array
                Some (Array (InputArraySchema(innerCwlType)))
            | None ->
                // Base type with array suffix
                try
                    let baseType = cwlSimpleTypeFromString innerType
                    Some (Array (InputArraySchema(baseType)))
                with ex when isRecoverableDecodingError ex -> None
        else
            None

    /// Decode an InputArraySchema from a YAMLElement
    let rec inputArraySchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputArraySchema) =
        fun value ->
        Decode.object (fun get ->
            // Decode items - can be string or complex type
            let itemsValue = get.Required.Field "items" id
            let decodedItems = cwlTypeDecoder' itemsValue
            
            let schema =
                InputArraySchema(
                    decodedItems,
                    ?label = get.Optional.Field "label" Decode.string,
                    ?doc = get.Optional.Field "doc" Decode.string,
                    ?name = get.Optional.Field "name" Decode.string
                )
            overflowIntoDynamicObj schema (InputArraySchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value
    /// Decode an InputRecordField from a YAMLElement
    and inputRecordFieldDecoder: (YAMLiciousTypes.YAMLElement -> InputRecordField) =
        fun value ->
        Decode.object (fun get ->
            let name = get.Required.Field "name" Decode.string
            
            // Decode the type field (can be string or complex type)
            let typeValue = get.Required.Field "type" id
            let decodedType = cwlTypeDecoder' typeValue
            
            let field =
                InputRecordField(
                    name,
                    decodedType,
                    ?doc = get.Optional.Field "doc" Decode.string,
                    ?label = get.Optional.Field "label" Decode.string
                )
            overflowIntoDynamicObj field (InputRecordField.KnownFieldNames |> Seq.toList) value |> ignore
            field
        ) value

    /// Attempt to decode fields as flow-style array: [{name: x, type: y}]
    and tryDecodeFieldsAsArray (element: YAMLElement) : ResizeArray<InputRecordField> option =
        try
            Decode.resizearray inputRecordFieldDecoder element |> Some
        with ex when isRecoverableDecodingError ex -> None

    /// Attempt to decode fields as map-style: {fieldName: type}
    and tryDecodeFieldsAsMap (element: YAMLElement) : ResizeArray<InputRecordField> option =
        try
            let dict = Decode.object (fun get2 -> get2.Overflow.FieldList []) element
            let fields = ResizeArray<InputRecordField>()
            for kvp in dict do
                let fieldType = cwlTypeDecoder' kvp.Value
                let field =
                    InputRecordField(
                        kvp.Key,
                        fieldType,
                        ?doc = tryGetStringField "doc" kvp.Value,
                        ?label = tryGetStringField "label" kvp.Value
                    )
                overflowIntoDynamicObj field (InputRecordField.KnownFieldNames |> Seq.toList) kvp.Value |> ignore
                fields.Add(field)
            Some fields
        with ex when isRecoverableDecodingError ex -> None

    /// Decode an InputRecordSchema from a YAMLElement
    and inputRecordSchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputRecordSchema) =
        fun value ->
        Decode.object (fun get ->
            // Try to decode fields as an array (flow-style) or as a map (block-style)
            let decodedFields =
                // Get the fields element directly
                let fieldsElement = get.Optional.Field "fields" id
                
                match fieldsElement with
                | Some (YAMLElement.Object []) ->
                    // Empty array case: fields: []
                    Some (ResizeArray<InputRecordField>())
                | Some element ->
                    // Try flow-style first, then fall back to map-style
                    match tryDecodeFieldsAsArray element with
                    | Some fields -> Some fields
                    | None -> tryDecodeFieldsAsMap element
                | None -> None
            
            let schema =
                InputRecordSchema(
                    ?fields = decodedFields,
                    ?label = get.Optional.Field "label" Decode.string,
                    ?doc = get.Optional.Field "doc" Decode.string,
                    ?name = get.Optional.Field "name" Decode.string
                )
            overflowIntoDynamicObj schema (InputRecordSchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value

    /// Decode an InputEnumSchema from a YAMLElement
    and inputEnumSchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputEnumSchema) =
        fun value ->
        Decode.object (fun get ->
            let symbols = get.Required.Field "symbols" (Decode.resizearray Decode.string)
            
            let schema =
                InputEnumSchema(
                    symbols,
                    ?label = get.Optional.Field "label" Decode.string,
                    ?doc = get.Optional.Field "doc" Decode.string,
                    ?name = get.Optional.Field "name" Decode.string
                )
            overflowIntoDynamicObj schema (InputEnumSchema.KnownFieldNames |> Seq.toList) value |> ignore
            schema
        ) value

    /// Decode a CWLType from a YAMLElement (handles all types including complex schemas)
    and cwlTypeDecoder' (element: YAMLiciousTypes.YAMLElement): CWLType =
        let parseTypeString (typeStr: string) =
            // Handle optional suffix
            let stripped, isOptional = 
                if typeStr.EndsWith("?") then
                    typeStr.Replace("?", ""), true
                else
                    typeStr, false
            
            // Try to parse as array shorthand (handles arbitrary nesting recursively)
            let baseType = 
                match parseArrayShorthand stripped with
                | Some arrayType -> arrayType
                | None -> cwlSimpleTypeFromString stripped
            
            // Wrap in Union if optional
            if isOptional then
                Union (ResizeArray [Null; baseType])
            else
                baseType

        let parseTypeObjectString (typeStr: string) =
            let stripped, isOptional =
                if typeStr.EndsWith("?") then
                    typeStr.Replace("?", ""), true
                else
                    typeStr, false

            match stripped with
            | "File" ->
                let file = decodeFileInstanceFields element
                let baseType = File file
                if isOptional then Union (ResizeArray [Null; baseType]) else baseType
            | "Directory" ->
                let directory = decodeDirectoryInstanceFields element
                let baseType = Directory directory
                if isOptional then Union (ResizeArray [Null; baseType]) else baseType
            | _ -> parseTypeString typeStr
        
        match element with
        | YAMLElement.Value v | YAMLElement.Object [YAMLElement.Value v] ->
            // Simple type string
            parseTypeString v.Value
        | YAMLElement.Sequence items
        | YAMLElement.Object [YAMLElement.Sequence items] ->
            // Union type. Mapping values may arrive wrapped as Object [Sequence ...].
            let types = items |> List.map cwlTypeDecoder' |> ResizeArray
            Union types
        | YAMLElement.Object _ ->
            // Complex type - check for type field
            Decode.object (fun get ->
                let typeField = get.Optional.Field "type" id
                match typeField with
                | Some (YAMLElement.Object [YAMLElement.Value v]) ->
                    match v.Value with
                    | "record" -> Record (inputRecordSchemaDecoder element)
                    | "enum" -> Enum (inputEnumSchemaDecoder element)
                    | "array" -> Array (inputArraySchemaDecoder element)
                    | simpleType -> parseTypeObjectString simpleType
                | Some (YAMLElement.Object _) ->
                    // Nested complex type
                    cwlTypeDecoder' (get.Required.Field "type" id)
                | _ -> raise (System.ArgumentException("Unexpected type format in cwlTypeDecoder'"))
            ) element
        | _ -> raise (System.ArgumentException("Unexpected YAMLElement in cwlTypeDecoder'"))
    /// Match the input string to the possible CWL types and checks if it is optional
    let cwlTypeStringMatcher (t: string) (get: Decode.IGetters) =
        let optional, newT =
            if t.EndsWith("?") then
                true, t.Replace("?", "")
            else
                false, t
        
        // Try to parse as array shorthand (handles arbitrary nesting recursively)
        let cwlType =
            match parseArrayShorthand newT with
            | Some arrayType -> arrayType
            | None ->
                // Not an array, check for simple types or Dirent
                match newT with
                | "File" -> File (FileInstance ())
                | "Directory" -> Directory (DirectoryInstance ())
                | "Dirent" -> (get.Required.Field "listing" direntDecoder)
                | "string" -> String
                | "int" -> Int
                | "long" -> Long
                | "float" -> Float
                | "double" -> Double
                | "boolean" -> Boolean
                | "stdout" -> Stdout
                | "null" -> Null
                | _ -> failwith "Invalid CWL type"
        
        // Wrap in Union if optional
        let finalType = 
            if optional then
                Union (ResizeArray [Null; cwlType])
            else
                cwlType
        finalType, optional

    /// Access the type field and decode a YAMLElement into a CWLType
    let cwlTypeDecoder: (YAMLiciousTypes.YAMLElement -> CWLType*bool) =
        Decode.object (fun get ->
            let cwlType = 
                get.Required.Field 
                    "type" 
                    (
                        fun value ->
                            match value with
                            | YAMLElement.Value v | YAMLElement.Object [YAMLElement.Value v] -> Some v.Value
                            | YAMLElement.Object o -> None
                            | YAMLElement.Sequence _ -> None
                            | _ -> raise (System.ArgumentException("Unexpected YAMLElement in cwlTypeDecoder"))
                    )
            match cwlType with
            | Some t ->
                cwlTypeStringMatcher t get
            | None -> 
                let cwlType = get.Required.Field "type" cwlTypeDecoder'
                cwlType, false
        )

    let private decodeNamedOutput (name: string) (value: YAMLElement) =
        let outputBinding = outputBindingDecoder value
        let outputSourceValues = outputSourceDecoder value
        let cwlType =
            match value with
            | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value (Unchecked.defaultof<Decode.IGetters>) |> fst
            | _ -> cwlTypeDecoder value |> fst
        let output =
            CWLOutput(
                name,
                cwlType,
                ?label = tryGetStringField "label" value,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" value,
                ?streamable = tryGetBoolField "streamable" value,
                ?doc = tryGetStringField "doc" value,
                ?format = tryGetStringField "format" value
            )
        output.OutputBinding <- outputBinding
        match outputSourceValues with
        | Some values when values.Count > 1 -> output.OutputSource <- Some (OutputSource.Multiple values)
        | Some values when values.Count = 1 -> output.OutputSource <- Some (OutputSource.Single values.[0])
        | _ -> ()
        overflowIntoDynamicObj output (CWLOutput.KnownFieldNames |> Seq.toList) value |> ignore
        output

    let private decodeOutputSequenceItem (warnings: WarningSink) (path: string) (index: int) (item: YAMLElement) =
        match tryGetStringField "id" item with
        | Some id -> Some (decodeNamedOutput id item)
        | None when isIgnorableYamlNoise item -> None
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL output entry." (Some item)
            None

    /// Decode a YAMLElement into an Output Array
    let outputArrayDecoderWithWarnings (warnings: WarningSink) (path: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLOutput>) =
        fun value ->
            match value with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                items
                |> List.mapi (decodeOutputSequenceItem warnings path)
                |> List.choose id
                |> ResizeArray
            | _ ->
                let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
                [| for key in dict.Keys do decodeNamedOutput key dict.[key] |] |> ResizeArray

    let outputArrayDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLOutput>) =
        outputArrayDecoderWithWarnings None "outputs"

    /// Access the outputs field and decode a YAMLElement into an Output Array
    let outputsDecoderWithWarnings (warnings: WarningSink) : (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLOutput>) =
        Decode.object (fun get ->
            get.Required.Field "outputs" (outputArrayDecoderWithWarnings warnings "outputs")
        )

    let outputsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLOutput>) =
        outputsDecoderWithWarnings None

    /// Decode a YAMLElement into a DockerRequirement
    let dockerRequirementDecoder (get: Decode.IGetters): DockerRequirement =
        let dockerFile =
            get.Optional.Field "dockerFile" id
            |> Option.map decodeSchemaSaladString

        DockerRequirement.create(
            ?dockerPull = get.Optional.Field "dockerPull" Decode.string,
            ?dockerFileReference = dockerFile,
            ?dockerImageId = get.Optional.Field "dockerImageId" Decode.string,
            ?dockerLoad = get.Optional.Field "dockerLoad" Decode.string,
            ?dockerImport = get.Optional.Field "dockerImport" Decode.string,
            ?dockerOutputDirectory = get.Optional.Field "dockerOutputDirectory" Decode.string,
            ?dockerRunOptions = get.Optional.Field "cwltool:dockerRunOptions" decodeStringArrayOrScalar
        )

    /// Decode a YAMLElement into an EnvVarRequirement array.
    /// Supports both array form and map shorthand form (envName -> envValue).
    let envVarRequirementDecoder (get: Decode.IGetters): ResizeArray<EnvironmentDef> =
        let normalizeCollectionElement = function
            | YAMLElement.Object [YAMLElement.Sequence sequence] -> YAMLElement.Sequence sequence
            | YAMLElement.Object [YAMLElement.Object mappings] -> YAMLElement.Object mappings
            | other -> other

        let decodeEnvValue = function
            | YAMLElement.Value value
            | YAMLElement.Object [YAMLElement.Value value] ->
                value.Value.Trim('"')
            | other ->
                decodeStringOrExpression other

        let envDefElement = get.Required.Field "envDef" id |> normalizeCollectionElement

        match envDefElement with
        | YAMLElement.Sequence _ ->
            Decode.resizearray
                (fun value ->
                    Decode.object (fun get2 ->
                        let env =
                            EnvironmentDef(
                                get2.Required.Field "envName" Decode.string,
                                get2.Required.Field "envValue" Decode.string
                            )
                        overflowIntoDynamicObj env (EnvironmentDef.KnownFieldNames |> Seq.toList) value |> ignore
                        env
                    ) value)
                envDefElement
        | YAMLElement.Object mappings ->
            mappings
            |> List.choose (function
                | YAMLElement.Mapping (key, value) ->
                    Some (EnvironmentDef(key.Value, decodeEnvValue value))
                | _ -> None)
            |> ResizeArray
        | _ ->
            raise (System.ArgumentException("Invalid envDef format. Expected array or map."))

    /// Decode a YAMLElement into a SoftwareRequirement array.
    /// Supports both array form and map shorthand form.
    let softwareRequirementDecoder (get: Decode.IGetters): ResizeArray<SoftwarePackage> =
        let normalizeCollectionElement = function
            | YAMLElement.Object [YAMLElement.Sequence sequence] -> YAMLElement.Sequence sequence
            | YAMLElement.Object [YAMLElement.Object mappings] -> YAMLElement.Object mappings
            | other -> other

        let packagesElement = get.Required.Field "packages" id |> normalizeCollectionElement

        let decodeSpecsArray (element: YAMLElement) =
            let normalized = normalizeCollectionElement element
            Decode.resizearray decodeStringOrExpression normalized

        let decodePackageFromMapEntry packageName packageValue =
            let normalizedPackageValue = normalizeCollectionElement packageValue
            match normalizedPackageValue with
            | YAMLElement.Object [] ->
                SoftwarePackage(packageName)
            | YAMLElement.Sequence _ ->
                SoftwarePackage(packageName, specs = decodeSpecsArray normalizedPackageValue)
            | YAMLElement.Object mappings ->
                let version =
                    mappings
                    |> List.tryPick (function
                        | YAMLElement.Mapping (k, v) when k.Value = "version" -> Some (decodeSpecsArray v)
                        | _ -> None)
                let specs =
                    mappings
                    |> List.tryPick (function
                        | YAMLElement.Mapping (k, v) when k.Value = "specs" -> Some (decodeSpecsArray v)
                        | _ -> None)
                let package = SoftwarePackage(packageName, ?version = version, ?specs = specs)
                overflowIntoDynamicObj package (SoftwarePackage.KnownFieldNames |> Seq.toList) normalizedPackageValue |> ignore
                package
            | _ ->
                SoftwarePackage(packageName, specs = ResizeArray [| decodeStringOrExpression packageValue |])

        match packagesElement with
        | YAMLElement.Sequence _ ->
            Decode.resizearray
                (fun value ->
                    Decode.object (fun get2 ->
                        let package =
                            SoftwarePackage(
                                get2.Required.Field "package" Decode.string,
                                ?version = get2.Optional.Field "version" (Decode.resizearray Decode.string),
                                ?specs = get2.Optional.Field "specs" (Decode.resizearray Decode.string)
                            )
                        overflowIntoDynamicObj package (SoftwarePackage.KnownFieldNames |> Seq.toList) value |> ignore
                        package
                    ) value)
                packagesElement
        | YAMLElement.Object mappings ->
            mappings
            |> List.choose (function
                | YAMLElement.Mapping (key, value) -> Some (decodePackageFromMapEntry key.Value value)
                | _ -> None)
            |> ResizeArray
        | _ ->
            raise (System.ArgumentException("Invalid packages format. Expected array or map."))

    /// Decode a YAMLElement into an InitialWorkDirRequirement array.
    /// Supports both string/expression and Dirent listing items.
    let initialWorkDirRequirementDecoder (get: Decode.IGetters): ResizeArray<InitialWorkDirEntry> =
        let listingElement = get.Required.Field "listing" id
        match listingElement with
        | YAMLElement.Object [YAMLElement.Sequence _]
        | YAMLElement.Sequence _ ->
            Decode.resizearray initialWorkDirEntryDecoder listingElement
        | _ ->
            ResizeArray [| initialWorkDirEntryDecoder listingElement |]

    let loadListingRequirementDecoder (get: Decode.IGetters): LoadListingRequirementValue =
        let loadListingValue =
            get.Optional.Field "loadListing" Decode.string
            |> Option.defaultValue "no_listing"
        let loadListing =
            match LoadListingEnum.tryParse loadListingValue with
            | Some parsed -> parsed
            | None -> raise (System.ArgumentException($"Invalid loadListing value '{loadListingValue}'. Expected one of: no_listing, shallow_listing, deep_listing."))
        LoadListingRequirementValue(loadListing)

    let decodeResourceScalar (element: YAMLElement) : obj =
        let tryGetScalarString = function
            | YAMLElement.Value value
            | YAMLElement.Object [YAMLElement.Value value] ->
                Some value.Value
            | _ ->
                None

        match tryGetScalarString element with
        | Some scalarValue ->
            match System.Int64.TryParse(scalarValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with
            | true, intValue -> box intValue
            | false, _ ->
                match System.Double.TryParse(scalarValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
                | true, floatValue -> box floatValue
                | false, _ -> box (decodeStringOrExpression element)
        | None ->
            box (decodeStringOrExpression element)

    let optionalResourceField (get: Decode.IGetters) (fieldName: string) : obj option =
        get.Optional.Field fieldName id
        |> Option.map decodeResourceScalar

    /// Decode a YAMLElement into a ResourceRequirementInstance
    let resourceRequirementDecoder (get: Decode.IGetters): ResourceRequirementInstance =
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
        
    let schemaDefRequirementTypeDecoder (value: YAMLElement) : SchemaDefRequirementType =
        let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
        let schemaDefKnownFields =
            [
                yield! SchemaDefRequirementType.KnownFieldNames
                yield! InputRecordSchema.KnownFieldNames
                yield! InputArraySchema.KnownFieldNames
                yield! InputEnumSchema.KnownFieldNames
            ]
            |> Seq.distinct
            |> Seq.toList
        if dict.ContainsKey "name" then
            let schema = SchemaDefRequirementType(decodeStringOrExpression dict.["name"], cwlTypeDecoder' value)
            overflowIntoDynamicObj schema schemaDefKnownFields value |> ignore
            schema
        else
            if dict.Count = 0 then
                raise (System.ArgumentException("SchemaDefRequirement entry cannot be empty."))
            let kv = dict |> Seq.head
            let schema = SchemaDefRequirementType(kv.Key, cwlTypeDecoder' kv.Value)
            overflowIntoDynamicObj schema (kv.Key :: schemaDefKnownFields) value |> ignore
            schema

    /// Decode a YAMLElement into a SchemaDefRequirementType array
    let schemaDefRequirementDecoder (get: Decode.IGetters): ResizeArray<SchemaDefRequirementType> =
        get.Required.Field "types" (Decode.resizearray schemaDefRequirementTypeDecoder)

    let tryDecodeBoolScalar (element: YAMLElement) : bool option =
        match element with
        | YAMLElement.Value value
        | YAMLElement.Object [YAMLElement.Value value] ->
            match value.Value.Trim().ToLowerInvariant() with
            | "true" -> Some true
            | "false" -> Some false
            | _ -> None
        | _ ->
            None

    let workReuseRequirementDecoder (get: Decode.IGetters): Requirement =
        match get.Optional.Field "enableReuse" id with
        | None ->
            WorkReuseRequirement (WorkReuseRequirementValue(true))
        | Some value ->
            match tryDecodeBoolScalar value with
            | Some boolValue ->
                WorkReuseRequirement (WorkReuseRequirementValue(boolValue))
            | None ->
                WorkReuseExpressionRequirement (decodeStringOrExpression value)

    let networkAccessRequirementDecoder (get: Decode.IGetters): Requirement =
        match get.Optional.Field "networkAccess" id with
        | None ->
            NetworkAccessRequirement (NetworkAccessRequirementValue(true))
        | Some value ->
            match tryDecodeBoolScalar value with
            | Some boolValue ->
                NetworkAccessRequirement (NetworkAccessRequirementValue(boolValue))
            | None ->
                NetworkAccessExpressionRequirement (decodeStringOrExpression value)

    let inplaceUpdateRequirementDecoder (get: Decode.IGetters): InplaceUpdateRequirementValue =
        InplaceUpdateRequirementValue(
            get.Optional.Field "inplaceUpdate" Decode.bool
            |> Option.defaultValue true
        )

    /// Decode a YAMLElement into a ToolTimeLimitRequirement value
    let toolTimeLimitRequirementDecoder (get: Decode.IGetters): ToolTimeLimitValue =
        let timeLimitElement = get.Required.Field "timelimit" id
        let tryGetScalarString =
            match timeLimitElement with
            | YAMLElement.Value value
            | YAMLElement.Object [YAMLElement.Value value] ->
                Some value.Value
            | _ ->
                None

        match tryGetScalarString with
        | Some scalarValue ->
            match System.Int64.TryParse(scalarValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with
            | true, intValue when intValue >= 0L -> ToolTimeLimitSeconds intValue
            | true, _ -> raise (System.ArgumentException("ToolTimeLimit timelimit must be non-negative."))
            | false, _ -> ToolTimeLimitExpression (decodeStringOrExpression timeLimitElement)
        | None ->
            ToolTimeLimitExpression (decodeStringOrExpression timeLimitElement)

    let inlineJavascriptRequirementDecoder (get: Decode.IGetters): InlineJavascriptRequirementValue =
        InlineJavascriptRequirementValue(?expressionLib = get.Optional.Field "expressionLib" decodeStringArrayOrScalar)

    /// Decode all YAMLElements matching the Requirement type into a ResizeArray of Requirement
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

    let private addRequirementPayloadOverflow (element: YAMLElement) (requirement: Requirement) =
        let knownWithClass knownFields = "class" :: (knownFields |> Seq.toList)
        match requirement with
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

    let requirementArrayDecoder : YAMLElement -> ResizeArray<Requirement> =
        fun yEle ->
            // helper: decode a single requirement object that contain 'class' field
            let decodeSingleRequirementObject (ele: YAMLElement) : Requirement =
                Decode.object (fun get ->
                    let cls = get.Required.Field "class" Decode.string
                    requirementFromTypeName cls get
                    |> addRequirementPayloadOverflow ele
                ) ele

            match yEle with
            // I: ARRAY SYNTAX
                // requirements:
                //   - class: DockerRequirement
            | YAMLElement.Object [YAMLElement.Sequence items] ->
                items
                |> List.map decodeSingleRequirementObject
                |> ResizeArray

            // II:  OBJECT/MAP SYNTAX (also covers flow/JSON-style mapping)
                // requirements:
                //   DockerRequirement: { ... }
            | YAMLElement.Object _ ->
                Decode.object (fun get ->
                    get.Overflow.FieldList []
                    |> Seq.map (fun kv ->
                        Decode.object (requirementFromTypeName kv.Key) kv.Value
                        |> addRequirementPayloadOverflow kv.Value
                    )
                    |> ResizeArray
                ) yEle
            // INVALID CWL REQUIREMENTS  
            | other -> raise (System.ArgumentException($"Invalid CWL requirements syntax: {other}"))

    let tryDecodeKnownRequirementFromElement (element: YAMLElement) : Requirement option =
        try
            Some (Decode.object (fun get ->
                let cls = get.Required.Field "class" Decode.string
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

    let decodeHintElement (element: YAMLElement) : HintEntry =
        match tryDecodeKnownRequirementFromElement element with
        | Some requirement -> KnownHint requirement
        | None ->
            let hintClass =
                try
                    Decode.object (fun get -> get.Optional.Field "class" Decode.string) element
                with _ ->
                    None
            UnknownHint (HintUnknownValue(hintClass, element))

    let hintArrayDecoder : YAMLElement -> ResizeArray<HintEntry> =
        fun yEle ->
            match yEle with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                items
                |> List.map decodeHintElement
                |> ResizeArray
            | YAMLElement.Object _ ->
                Decode.object (fun get ->
                    get.Overflow.FieldList []
                    |> Seq.map (fun kv ->
                        let valueWithClass =
                            match kv.Value with
                            | YAMLElement.Object mappings ->
                                let hasClass =
                                    mappings
                                    |> List.exists (function
                                        | YAMLElement.Mapping (k, _) when k.Value = "class" -> true
                                        | _ -> false
                                    )
                                if hasClass then kv.Value
                                else
                                    let clsKey = YAMLContent.create "class"
                                    let clsValue = YAMLElement.Object [YAMLElement.Value (YAMLContent.create kv.Key)]
                                    YAMLElement.Object (YAMLElement.Mapping (clsKey, clsValue) :: mappings)
                            | other ->
                                let clsKey = YAMLContent.create "class"
                                let clsValue = YAMLElement.Object [YAMLElement.Value (YAMLContent.create kv.Key)]
                                let valueKey = YAMLContent.create "value"
                                YAMLElement.Object [
                                    YAMLElement.Mapping (clsKey, clsValue)
                                    YAMLElement.Mapping (valueKey, other)
                                ]
                        decodeHintElement valueWithClass
                    )
                    |> ResizeArray
                ) yEle
            | other ->
                raise (System.ArgumentException($"Invalid CWL hints syntax: {other}"))

    /// Access the requirements field and decode the YAMLElements into a Requirement array
    let requirementsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<Requirement> option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "requirements" requirementArrayDecoder
            requirements
        )

    /// Access the hints field and decode the YAMLElements into a HintEntry array
    let hintsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<HintEntry> option) =
        Decode.object (fun get ->
            let hints = get.Optional.Field "hints" hintArrayDecoder
            hints
        )

    /// Decode a YAMLElement into an InputBinding
    let inputBindingDecoder: (YAMLiciousTypes.YAMLElement -> InputBinding option) =
        Decode.object(fun get ->
            let outputBinding = 
                get.Optional.Field 
                    "inputBinding" 
                    (
                        fun value ->
                            Decode.object (fun get' ->
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
                    )
            outputBinding
        )

    let private decodeNamedInput (name: string) (value: YAMLElement) =
        let inputBinding = inputBindingDecoder value
        let cwlType, optional =
            match value with
            | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value (Unchecked.defaultof<Decode.IGetters>)
            | _ -> cwlTypeDecoder value
        let input =
            CWLInput(
                name,
                cwlType,
                ?label = tryGetStringField "label" value,
                ?secondaryFiles = tryGetYamlField "secondaryFiles" value,
                ?streamable = tryGetBoolField "streamable" value,
                ?doc = tryGetStringField "doc" value,
                ?format = tryGetStringField "format" value,
                ?loadContents = tryGetBoolField "loadContents" value,
                ?loadListing = tryGetLoadListingField "loadListing" value,
                ?defaultValue = tryGetYamlField "default" value
            )
        if optional then input.Optional <- Some true
        input.InputBinding <- inputBinding
        overflowIntoDynamicObj input (CWLInput.KnownFieldNames |> Seq.toList) value |> ignore
        input

    let private decodeInputSequenceItem (warnings: WarningSink) (path: string) (index: int) (item: YAMLElement) =
        match tryGetStringField "id" item with
        | Some id -> Some (decodeNamedInput id item)
        | None when isIgnorableYamlNoise item -> None
        | None ->
            addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL input entry." (Some item)
            None

    /// Decode a YAMLElement into an Input array
    let inputArrayDecoderWithWarnings (warnings: WarningSink) (path: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLInput>) =
        fun value ->
            match value with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                items
                |> List.mapi (decodeInputSequenceItem warnings path)
                |> List.choose id
                |> ResizeArray
            | _ ->
                let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
                [| for key in dict.Keys do decodeNamedInput key dict.[key] |] |> ResizeArray

    let inputArrayDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLInput>) =
        inputArrayDecoderWithWarnings None "inputs"

    /// Access the inputs field and decode the YAMLElements into an Input array
    let inputsDecoderWithWarnings (warnings: WarningSink) : (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLInput> option) =
        Decode.object (fun get ->
            get.Optional.Field "inputs" (inputArrayDecoderWithWarnings warnings "inputs")
        )

    let inputsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLInput> option) =
        inputsDecoderWithWarnings None

    let baseCommandDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<string> option) =
        Decode.object (fun get ->
            let baseCommandField = get.Optional.Field "baseCommand" id
            match baseCommandField with
            | Some (YAMLElement.Object [YAMLElement.Value v]) ->
                // Single string value
                Some (ResizeArray([v.Value]))
            | Some (YAMLElement.Value v) ->
                // Single string value (unwrapped)
                Some (ResizeArray([v.Value]))
            | Some (YAMLElement.Object [YAMLElement.Sequence s]) ->
                // Array of strings wrapped
                Some (Decode.resizearray Decode.string (YAMLElement.Sequence s))
            | Some (YAMLElement.Sequence s) ->
                // Array of strings unwrapped
                Some (Decode.resizearray Decode.string (YAMLElement.Sequence s))
            | None -> None
            | _ -> None
        )

    let versionDecoder: (YAMLiciousTypes.YAMLElement -> string) =
        Decode.object (fun get -> get.Required.Field "cwlVersion" Decode.string)

    let classDecoder: (YAMLiciousTypes.YAMLElement -> string) =
        Decode.object (fun get ->
            get.Required.Field "class" Decode.string 
        )
    let stringOptionFieldDecoder field : (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object(fun get ->
            let fieldValue = get.Optional.Field field Decode.string
            fieldValue
        )

    let boolOptionFieldDecoder field : (YAMLiciousTypes.YAMLElement -> bool option) =
        Decode.object(fun get ->
            let fieldValue = get.Optional.Field field Decode.bool
            fieldValue
        )

    let yamlElementOptionFieldDecoder field : (YAMLiciousTypes.YAMLElement -> YAMLElement option) =
        Decode.object(fun get ->
            let fieldValue = get.Optional.Field field id
            fieldValue
        )

    let stringFieldDecoder field : (YAMLiciousTypes.YAMLElement -> string) =
        Decode.object(fun get ->
            let fieldValue = get.Required.Field field Decode.string
            fieldValue
        )

    /// Decode a YAMLElement that may be a single string or an array of strings.
    let stringOrStringArrayDecoder (value: YAMLElement) : ResizeArray<string> option =
        match value with
        | YAMLElement.Object [YAMLElement.Value v]
        | YAMLElement.Value v ->
            Some (ResizeArray [ v.Value ])
        | YAMLElement.Object [YAMLElement.Sequence s]
        | YAMLElement.Sequence s ->
            Some (Decode.resizearray Decode.string (YAMLElement.Sequence s))
        | _ -> None

    /// Decode a YAMLElement into a ResizeArray<string> option for the source field
    /// Handles both single string values and arrays of strings
    let sourceArrayFieldDecoder field : (YAMLiciousTypes.YAMLElement -> ResizeArray<string> option) =
        Decode.object(fun get ->
            get.Optional.Field field id
            |> Option.bind stringOrStringArrayDecoder
        )

    let linkMergeFieldDecoder field : (YAMLiciousTypes.YAMLElement -> LinkMergeMethod option) =
        Decode.object(fun get ->
            let linkMergeField = get.Optional.Field field Decode.string
            match linkMergeField with
            | Some linkMergeString ->
                match LinkMergeMethod.tryParse linkMergeString with
                | Some linkMerge -> Some linkMerge
                | None -> raise (System.ArgumentException($"Invalid linkMerge value: {linkMergeString}"))
            | None -> None
        )

    let pickValueFieldDecoder field : (YAMLiciousTypes.YAMLElement -> PickValueMethod option) =
        Decode.object(fun get ->
            let pickValueField = get.Optional.Field field Decode.string
            match pickValueField with
            | Some pickValueString ->
                match PickValueMethod.tryParse pickValueString with
                | Some pickValue -> Some pickValue
                | None -> raise (System.ArgumentException($"Invalid pickValue value: {pickValueString}"))
            | None -> None
        )

    let scatterFieldDecoder field : (YAMLiciousTypes.YAMLElement -> ResizeArray<string> option) =
        Decode.object(fun get ->
            get.Optional.Field field id
            |> Option.bind stringOrStringArrayDecoder
        )

    let scatterMethodFieldDecoder field : (YAMLiciousTypes.YAMLElement -> ScatterMethod option) =
        Decode.object(fun get ->
            let scatterMethodField = get.Optional.Field field Decode.string
            match scatterMethodField with
            | Some scatterMethodString ->
                match ScatterMethod.tryParse scatterMethodString with
                | Some scatterMethod -> Some scatterMethod
                | None -> raise (System.ArgumentException($"Invalid scatterMethod value: {scatterMethodString}"))
            | None -> None
        )

    let expressionStringOptionFieldDecoder field : (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object(fun get ->
            get.Optional.Field field id
            |> Option.map decodeStringOrExpression
        )

    let decodeStepInputFromValue (id: string) (value: YAMLElement) (allowScalarSource: bool) : StepInput =
        let scalarSource =
            if allowScalarSource then
                stringOrStringArrayDecoder value
            else
                None
        let fieldSource = sourceArrayFieldDecoder "source" value
        let source =
            match scalarSource, fieldSource with
            | Some s, _ -> Some s
            | _, Some s -> Some s
            | _ -> None
        let stepInput =
            StepInput.create(
                id,
                ?source = source,
                ?defaultValue = yamlElementOptionFieldDecoder "default" value,
                ?valueFrom = stringOptionFieldDecoder "valueFrom" value,
                ?linkMerge = linkMergeFieldDecoder "linkMerge" value,
                ?pickValue = pickValueFieldDecoder "pickValue" value,
                ?doc = stringOptionFieldDecoder "doc" value,
                ?loadContents = boolOptionFieldDecoder "loadContents" value,
                ?loadListing = stringOptionFieldDecoder "loadListing" value,
                ?label = stringOptionFieldDecoder "label" value
            )
        overflowIntoDynamicObj
            stepInput
            (StepInput.KnownFieldNames |> Seq.toList)
            value
        |> ignore
        stepInput

    let decodeStepInputsFromMap (value: YAMLElement) : ResizeArray<StepInput> =
        let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
        [|
            for key in dict.Keys do
                decodeStepInputFromValue key dict.[key] true
        |]
        |> ResizeArray

    let decodeStepInputFromArrayItem (item: YAMLElement) : StepInput =
        let id = stringFieldDecoder "id" item
        decodeStepInputFromValue id item false

    let decodeStepInputsFromArrayWithWarnings (warnings: WarningSink) (path: string) (items: YAMLElement list) : ResizeArray<StepInput> =
        items
        |> List.mapi (fun index item ->
            match tryGetStringField "id" item with
            | Some _ -> Some (decodeStepInputFromArrayItem item)
            | None when isIgnorableYamlNoise item -> None
            | None ->
                addWarning warnings $"{path}[{index}]" "Skipped malformed unnamed CWL step input entry." (Some item)
                None)
        |> List.choose id
        |> ResizeArray

    let decodeStepInputsFromArray (items: YAMLElement list) : ResizeArray<StepInput> =
        decodeStepInputsFromArrayWithWarnings None "in" items

    let inputStepDecoderWithWarnings (warnings: WarningSink) (path: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<StepInput>) =
        fun value ->
            match value with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                decodeStepInputsFromArrayWithWarnings warnings path items
            | _ ->
                decodeStepInputsFromMap value

    let inputStepDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<StepInput>) =
        inputStepDecoderWithWarnings None "in"

    let decodeStepOutputItem (value: YAMLElement) : StepOutput =
        match value with
        | YAMLElement.Object [YAMLElement.Value v]
        | YAMLElement.Value v ->
            StepOutputString v.Value
        | _ ->
            let id = stringFieldDecoder "id" value
            let output = StepOutputParameter.create id
            overflowIntoDynamicObj output (StepOutputParameter.KnownFieldNames |> Seq.toList) value |> ignore
            StepOutputRecord output

    let outputStepsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<StepOutput>) =
        Decode.object (fun get ->
            let outField = get.Required.Field "out" id
            match outField with
            | YAMLElement.Object [] ->
                ResizeArray()
            | YAMLElement.Object [YAMLElement.Value v] when v.Value = "[]" ->
                ResizeArray()
            | YAMLElement.Object [YAMLElement.Sequence []] | YAMLElement.Sequence [] ->
                ResizeArray()
            | YAMLElement.Object [YAMLElement.Sequence outputs]
            | YAMLElement.Sequence outputs ->
                outputs
                |> List.map decodeStepOutputItem
                |> ResizeArray
            | value ->
                ResizeArray [ decodeStepOutputItem value ]
        )

    let docDecoder =
        Decode.object (fun get -> get.Optional.Field "doc" Decode.string)

    let labelDecoder: (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object (fun get -> get.Optional.Field "label" Decode.string)

    let idDecoder: (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object (fun get -> get.Optional.Field "id" Decode.string)

    let intentDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<string> option) =
        Decode.object (fun get ->
            get.Optional.Field "intent" id
            |> Option.bind stringOrStringArrayDecoder
        )
    
    let hasField (fieldName: string) (yamlElement: YAMLElement) : bool =
        match yamlElement with
        | YAMLElement.Object fields ->
            fields
            |> List.exists (function
                | YAMLElement.Mapping (k, _) when k.Value = fieldName -> true
                | _ -> false
            )
        | _ -> false

    let withDefaultCwlVersion (defaultCwlVersion: string) (yamlElement: YAMLElement) : YAMLElement =
        match yamlElement with
        | YAMLElement.Object fields when hasField "cwlVersion" yamlElement ->
            yamlElement
        | YAMLElement.Object fields ->
            let key = YAMLContent.create "cwlVersion"
            let value = YAMLElement.Object [YAMLElement.Value (YAMLContent.create defaultCwlVersion)]
            YAMLElement.Object (YAMLElement.Mapping (key, value) :: fields)
        | _ ->
            yamlElement

    let rec workflowStepRunDecoder (warnings: WarningSink) (defaultCwlVersion: string) (runValue: YAMLElement) : WorkflowStepRun =
        match runValue with
        | YAMLElement.Object [YAMLElement.Value v]
        | YAMLElement.Value v ->
            RunString v.Value
        | YAMLElement.Object _ ->
            let normalizedRun = withDefaultCwlVersion defaultCwlVersion runValue
            match decodeCWLProcessingUnitElementWithWarnings warnings normalizedRun with
            | CommandLineTool tool -> WorkflowStepRunOps.fromTool tool
            | Workflow workflow -> WorkflowStepRunOps.fromWorkflow workflow
            | ExpressionTool expressionTool -> WorkflowStepRunOps.fromExpressionTool expressionTool
            | Operation operation -> WorkflowStepRunOps.fromOperation operation
        | _ ->
            raise (System.ArgumentException($"Unsupported run value for workflow step: %A{runValue}"))

    and decodeWorkflowStepFromValueWithId (warnings: WarningSink) (defaultCwlVersion: string) (path: string) (stepId: string) (value: YAMLElement) : WorkflowStep =
        let runValue = Decode.object (fun get' -> get'.Required.Field "run" id) value
        let run = workflowStepRunDecoder warnings defaultCwlVersion runValue
        let inputs =
            Decode.object (fun get' ->
                get'.Required.Field "in" (inputStepDecoderWithWarnings warnings $"{path}.in")
            ) value
        let outputs = outputStepsDecoder value
        let requirements = requirementsDecoder value
        let hints = hintsDecoder value
        let doc = docDecoder value
        let label = labelDecoder value
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
        if requirements.IsSome then
            wfStep.Requirements <- requirements
        if hints.IsSome then
            wfStep.Hints <- hints
        overflowIntoDynamicObj
            wfStep
            (WorkflowStep.KnownFieldNames |> Seq.toList)
            value
        |> ignore
        wfStep

    and decodeWorkflowStepFromArrayItem (defaultCwlVersion: string) (item: YAMLElement) : WorkflowStep =
        let stepId = stringFieldDecoder "id" item
        decodeWorkflowStepFromValueWithId None defaultCwlVersion "steps[]" stepId item

    and decodeWorkflowStepFromArrayItemWithWarnings (warnings: WarningSink) (defaultCwlVersion: string) (index: int) (item: YAMLElement) =
        match tryGetStringField "id" item with
        | Some stepId -> Some (decodeWorkflowStepFromValueWithId warnings defaultCwlVersion $"steps[{index}]" stepId item)
        | None when isIgnorableYamlNoise item -> None
        | None ->
            addWarning warnings $"steps[{index}]" "Skipped malformed unnamed CWL workflow step entry." (Some item)
            None

    and stepArrayDecoderWithVersion (warnings: WarningSink) (defaultCwlVersion: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<WorkflowStep>) =
        fun value ->
            match value with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                items
                |> List.mapi (decodeWorkflowStepFromArrayItemWithWarnings warnings defaultCwlVersion)
                |> List.choose id
                |> ResizeArray
            | _ ->
                let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
                [|
                    for key in dict.Keys do
                        decodeWorkflowStepFromValueWithId warnings defaultCwlVersion $"steps.{key}" key dict.[key]
                |]
                |> ResizeArray

    and stepsDecoderWithVersion (warnings: WarningSink) (defaultCwlVersion: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<WorkflowStep>) =
        Decode.object (fun get ->
            get.Required.Field "steps" (stepArrayDecoderWithVersion warnings defaultCwlVersion)
        )

    and commandLineToolDecoder (warnings: WarningSink) (yamlCWL : YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs = inputsDecoderWithWarnings warnings yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let baseCommand = baseCommandDecoder yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
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
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (get.Overflow.FieldList (CWLToolDescription.KnownFieldNames |> Seq.toList))
            ) |> ignore
            md
        if inputs.IsSome then
            description.Inputs <- inputs
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
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and expressionToolDecoder (warnings: WarningSink) (yamlCWL: YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs = inputsDecoderWithWarnings warnings yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
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
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (get.Overflow.FieldList (CWLExpressionToolDescription.KnownFieldNames |> Seq.toList))
            ) |> ignore
            md
        if inputs.IsSome then
            description.Inputs <- inputs
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
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and operationDecoder (warnings: WarningSink) (yamlCWL: YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs =
            match inputsDecoderWithWarnings warnings yamlCWL with
            | Some i -> i
            | None -> raise (System.InvalidOperationException("Inputs are required for an operation"))
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
        let description =
            CWLOperationDescription(
                inputs,
                outputs,
                ?cwlVersion = Some cwlVersion,
                ?id = idDecoder yamlCWL
            )
        let metadata =
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
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and workflowDecoder (warnings: WarningSink) (yamlCWL: YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoderWithWarnings warnings yamlCWL
        let inputs =
            match inputsDecoderWithWarnings warnings yamlCWL with
            | Some i -> i
            | None -> raise (System.InvalidOperationException("Inputs are required for a workflow"))
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let intent = intentDecoder yamlCWL
        let steps = stepsDecoderWithVersion warnings cwlVersion yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
        let description =
            CWLWorkflowDescription(
                steps,
                inputs,
                outputs,
                ?cwlVersion = Some cwlVersion,
                ?id = idDecoder yamlCWL
            )
        let metadata =
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
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and decodeCWLProcessingUnitElementWithWarnings (warnings: WarningSink) (yamlCWL: YAMLElement) =
        let cls = classDecoder yamlCWL
        match cls with
        | "CommandLineTool" -> CommandLineTool (commandLineToolDecoder warnings yamlCWL)
        | "Workflow" -> Workflow (workflowDecoder warnings yamlCWL)
        | "ExpressionTool" -> ExpressionTool (expressionToolDecoder warnings yamlCWL)
        | "Operation" -> Operation (operationDecoder warnings yamlCWL)
        | _ -> raise (System.ArgumentException($"Invalid or unsupported CWL class: {cls}"))

    let decodeCWLProcessingUnitElement (yamlCWL: YAMLElement) =
        decodeCWLProcessingUnitElementWithWarnings None yamlCWL

    let stepArrayDecoder = stepArrayDecoderWithVersion None "v1.2"

    let stepsDecoder = stepsDecoderWithVersion None "v1.2"

    /// Decode a CWL file string written in the YAML format into a CWLToolDescription
    let decodeCommandLineToolWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readSanitizedYaml cwl
        { Value = commandLineToolDecoder (Some warnings) yamlCWL; Warnings = warnings }

    let decodeCommandLineTool (cwl: string) =
        (decodeCommandLineToolWithWarnings cwl).Value

    /// Decode a CWL file string written in the YAML format into a CWLWorkflowDescription
    let decodeWorkflowWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readSanitizedYaml cwl
        { Value = workflowDecoder (Some warnings) yamlCWL; Warnings = warnings }

    let decodeWorkflow (cwl: string) =
        (decodeWorkflowWithWarnings cwl).Value

    /// Decode a CWL file string written in the YAML format into a CWLExpressionToolDescription
    let decodeExpressionToolWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readSanitizedYaml cwl
        { Value = expressionToolDecoder (Some warnings) yamlCWL; Warnings = warnings }

    let decodeExpressionTool (cwl: string) =
        (decodeExpressionToolWithWarnings cwl).Value

    /// Decode a CWL file string written in the YAML format into a CWLOperationDescription
    let decodeOperationWithWarnings (cwl: string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readSanitizedYaml cwl
        { Value = operationDecoder (Some warnings) yamlCWL; Warnings = warnings }

    let decodeOperation (cwl: string) =
        (decodeOperationWithWarnings cwl).Value

    let decodeCWLProcessingUnitWithWarnings (cwl:string) =
        let warnings = ResizeArray<DecodeWarning>()
        let yamlCWL = readSanitizedYaml cwl
        { Value = decodeCWLProcessingUnitElementWithWarnings (Some warnings) yamlCWL; Warnings = warnings }

    let decodeCWLProcessingUnit (cwl:string) =
        (decodeCWLProcessingUnitWithWarnings cwl).Value

module DecodeParameters =

    let cwlParameterReferenceDecoder (get : Decode.IGetters) (key: string) (yEle: YAMLElement): CWLParameterReference =
        let tryScalarString = function
            | YAMLElement.Value v
            | YAMLElement.Object [YAMLElement.Value v] -> Some v.Value
            | _ -> None

        let tryField fieldName value =
            try
                Decode.object (fun get -> get.Optional.Field fieldName id) value
            with ex when Decode.isRecoverableDecodingError ex ->
                None

        let tryStringField fieldName value =
            tryField fieldName value
            |> Option.bind tryScalarString

        let withOverflow (reference: CWLParameterReference) value =
            Decode.overflowIntoDynamicObj reference (CWLParameterReference.KnownFieldNames |> Seq.toList) value |> ignore
            reference

        let fileOrDirectoryType className =
            match className with
            | "File" -> Some (CWLType.file())
            | "Directory" -> Some (CWLType.directory())
            | _ -> None

        let pathValue value =
            tryStringField "path" value
            |> Option.orElseWith (fun () -> tryStringField "location" value)

        let decodeObjectParameter value =
            match tryStringField "class" value, tryStringField "type" value with
            | Some className, _ ->
                let cwlType = fileOrDirectoryType className
                let values =
                    pathValue value
                    |> Option.map (fun p -> ResizeArray [| p |])
                    |> Option.defaultValue (ResizeArray())
                let reference = CWLParameterReference(key = key, values = values, ?type_ = cwlType)
                withOverflow reference value
            | None, Some typeName ->
                let cwlType, _ = Decode.cwlTypeStringMatcher typeName get
                let values =
                    match tryField "value" value with
                    | Some (YAMLElement.Object [YAMLElement.Sequence _] as sequenceValue)
                    | Some (YAMLElement.Sequence _ as sequenceValue) ->
                        Decode.resizearray Decode.string sequenceValue
                    | Some scalarOrObject ->
                        match tryScalarString scalarOrObject with
                        | Some scalar -> ResizeArray [| scalar |]
                        | None -> ResizeArray()
                    | None -> ResizeArray()
                let reference = CWLParameterReference(key = key, values = values, type_ = cwlType)
                withOverflow reference value
            | None, None ->
                let reference = CWLParameterReference(key = key, values = ResizeArray())
                withOverflow reference value

        let decodeSequenceParameter (items: YAMLElement list) =
            match items |> List.tryHead with
            | Some first ->
                match tryStringField "class" first with
                | Some className when className = "File" || className = "Directory" ->
                    let paths =
                        items
                        |> List.choose pathValue
                        |> ResizeArray
                    let itemType =
                        match className with
                        | "Directory" -> Directory (DirectoryInstance())
                        | _ -> File (FileInstance())
                    CWLParameterReference(
                        key = key,
                        values = paths,
                        type_ = Array (InputArraySchema(itemType))
                    )
                | _ ->
                    let values =
                        items
                        |> List.choose tryScalarString
                        |> ResizeArray
                    CWLParameterReference(key = key, values = values)
            | None ->
                CWLParameterReference(key = key, values = ResizeArray())

        match yEle with
        | YAMLElement.Value v
        | YAMLElement.Object [YAMLElement.Value v] ->
            CWLParameterReference(
                key = key,
                values = ResizeArray [v.Value]
            )
        | YAMLElement.Object [YAMLElement.Sequence s]
        | YAMLElement.Sequence s ->
            decodeSequenceParameter s
        | YAMLElement.Object _ ->
            decodeObjectParameter yEle
        | _ -> raise (System.ArgumentException($"Unexpected YAMLElement format in cwlParameterReferenceDecoder: %A{yEle}"))

    let cwlparameterReferenceArrayDecoder: YAMLElement -> ResizeArray<CWLParameterReference> =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for ele in dict do
                    cwlParameterReferenceDecoder get ele.Key ele.Value
            |]
            |> ResizeArray
        )

    let decodeYAMLParameterFile (yaml: string) =
        let yEle = Decode.read yaml
        cwlparameterReferenceArrayDecoder yEle

