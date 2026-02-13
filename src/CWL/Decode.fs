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

    let private normalizeYamlInput (yaml: string) =
        let normalized =
            if isNull yaml then "" else yaml.Replace("\r\n", "\n")
        let lines = normalized.Split('\n')
        let withoutShebang =
            if lines.Length > 0 && lines.[0].StartsWith("#!") then
                lines.[1..]
            else
                lines
        withoutShebang
        |> String.concat "\n"
        |> fun text -> text.TrimEnd()

    let private removeFullLineComments (yaml: string) =
        yaml.Split('\n')
        |> Array.filter (fun line -> line.TrimStart().StartsWith("#") |> not)
        |> String.concat "\n"

    let rec private removeYamlComments (yamlElement: YAMLElement) : YAMLElement =
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
    let private isRecoverableDecodingError (ex: exn) : bool =
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
        let normalized = normalizeYamlInput yaml
        let tryRead text =
            text
            |> Decode.read
            |> removeYamlComments
        try
            tryRead normalized
        with ex when isRecoverableDecodingError ex ->
            normalized
            |> removeFullLineComments
            |> tryRead

    /// Decode key value pairs into a dynamic object, while preserving their tree structure
    let rec overflowDecoder (dynObj: DynamicObj) (dict: System.Collections.Generic.Dictionary<string,YAMLElement>) =
        for e in dict do
            match e.Value with
            | YAMLElement.Object [YAMLElement.Value v] -> 
                DynObj.setProperty e.Key v.Value dynObj
            | YAMLElement.Object [YAMLElement.Sequence s] ->
                let newDynObj = new DynamicObj ()
                (s |> List.map ((Decode.object (fun get ->  (get.Overflow.FieldList []))) >> overflowDecoder newDynObj))
                |> List.iter (fun x ->
                    DynObj.setProperty
                        e.Key
                        x
                        dynObj
                )
            | _ -> DynObj.setProperty e.Key e.Value dynObj
        dynObj

    /// Decode scalar schema-salad string fields.
    /// Recognized directive wrappers are `$include` and `$import`.
    /// Unknown single-key mappings are intentionally coerced to a legacy literal `key: value` string.
    let decodeSchemaSaladString (yEle:YAMLElement) : SchemaSaladString =
        match yEle with
        | YAMLElement.Value v
        | YAMLElement.Object [YAMLElement.Value v] ->
            Literal v.Value
        | YAMLElement.Object [YAMLElement.Mapping (c, YAMLElement.Value v)]
        | YAMLElement.Object [YAMLElement.Mapping (c, YAMLElement.Object [YAMLElement.Value v])] ->
            match c.Value with
            | "$include" -> Include v.Value
            | "$import" -> Import v.Value
            | _ -> Literal (sprintf "%s: %s" c.Value v.Value)
        | _ -> raise (System.ArgumentException($"Unexpected YAMLElement format in decodeSchemaSaladString: %A{yEle}"))

    /// Decode a YAMLElement which is either a string or expression into a string.
    /// Directive objects such as {$include: path} are represented using legacy string form.
    let decodeStringOrExpression (yEle:YAMLElement) =
        decodeSchemaSaladString yEle
        |> SchemaSaladString.toDirectiveString

    /// Decode a YAMLElement into a glob search pattern for output binding
    let outputBindingGlobDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding) =
        Decode.object (fun get ->
            let glob = get.Optional.Field "glob" Decode.string
            { Glob = glob }
        )

    /// Decode a YAMLElement into an OutputBinding
    let outputBindingDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding option) =
        Decode.object(fun get ->
            let outputBinding = get.Optional.Field "outputBinding" outputBindingGlobDecoder
            outputBinding
        )

    let outputSourceDecoder: (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object(fun get ->
            let outputSource = get.Optional.Field "outputSource" Decode.string
            outputSource
        )

    /// Decode a YAMLElement into a Dirent
    let direntDecoder: (YAMLiciousTypes.YAMLElement -> CWLType) =
        Decode.object (fun get ->
            Dirent
                {
                    Entry = get.Required.Field "entry" decodeSchemaSaladString
                    Entryname = get.Optional.Field "entryname" decodeSchemaSaladString
                    Writable = get.Optional.Field "writable" Decode.bool
                }
        )

    /// Decode a listing entry of InitialWorkDirRequirement.
    /// Supports both Dirent object form and string/expression form.
    let initialWorkDirEntryDecoder: (YAMLiciousTypes.YAMLElement -> InitialWorkDirEntry) =
        fun value ->
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
                Some (Array { Items = innerCwlType; Label = None; Doc = None; Name = None })
            | None ->
                // Base type with array suffix
                try
                    let baseType = cwlSimpleTypeFromString innerType
                    Some (Array { Items = baseType; Label = None; Doc = None; Name = None })
                with ex when isRecoverableDecodingError ex -> None
        else
            None

    /// Decode an InputArraySchema from a YAMLElement
    let rec inputArraySchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputArraySchema) =
        Decode.object (fun get ->
            // Decode items - can be string or complex type
            let itemsValue = get.Required.Field "items" id
            let decodedItems = cwlTypeDecoder' itemsValue
            
            {
                Items = decodedItems
                Label = get.Optional.Field "label" Decode.string
                Doc = get.Optional.Field "doc" Decode.string
                Name = get.Optional.Field "name" Decode.string
            }
        )
    /// Decode an InputRecordField from a YAMLElement
    and inputRecordFieldDecoder: (YAMLiciousTypes.YAMLElement -> InputRecordField) =
        Decode.object (fun get ->
            let name = get.Required.Field "name" Decode.string
            
            // Decode the type field (can be string or complex type)
            let typeValue = get.Required.Field "type" id
            let decodedType = cwlTypeDecoder' typeValue
            
            {
                Name = name
                Type = decodedType
                Doc = get.Optional.Field "doc" Decode.string
                Label = get.Optional.Field "label" Decode.string
            }
        )

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
                fields.Add({
                    Name = kvp.Key
                    Type = fieldType
                    Doc = None
                    Label = None
                })
            Some fields
        with ex when isRecoverableDecodingError ex -> None

    /// Decode an InputRecordSchema from a YAMLElement
    and inputRecordSchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputRecordSchema) =
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
            
            {
                Fields = decodedFields
                Label = get.Optional.Field "label" Decode.string
                Doc = get.Optional.Field "doc" Decode.string
                Name = get.Optional.Field "name" Decode.string
            }
        )

    /// Decode an InputEnumSchema from a YAMLElement
    and inputEnumSchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputEnumSchema) =
        Decode.object (fun get ->
            let symbols = get.Required.Field "symbols" (Decode.resizearray Decode.string)
            
            {
                Symbols = symbols
                Label = get.Optional.Field "label" Decode.string
                Doc = get.Optional.Field "doc" Decode.string
                Name = get.Optional.Field "name" Decode.string
            }
        )

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
        
        match element with
        | YAMLElement.Value v | YAMLElement.Object [YAMLElement.Value v] ->
            // Simple type string
            parseTypeString v.Value
        | YAMLElement.Sequence items ->
            // Union type
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
                    | simpleType -> parseTypeString simpleType
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
                            | _ -> raise (System.ArgumentException("Unexpected YAMLElement in cwlTypeDecoder"))
                    )
            match cwlType with
            | Some t ->
                cwlTypeStringMatcher t get
            | None -> 
                let cwlType = get.Required.Field "type" cwlTypeDecoder'
                cwlType, false
        )

    /// Decode a YAMLElement into an Output Array
    let outputArrayDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLOutput>) =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let outputBinding = outputBindingDecoder value
                    let outputSource = outputSourceDecoder value
                    let cwlType =
                        match value with
                        | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value get |> fst
                        | _ -> cwlTypeDecoder value |> fst
                    let output = CWLOutput(key, cwlType)
                    if outputBinding.IsSome then DynObj.setOptionalProperty "outputBinding" outputBinding output
                    if outputSource.IsSome then DynObj.setOptionalProperty "outputSource" outputSource output
                    output
            |] |> ResizeArray)

    /// Access the outputs field and decode a YAMLElement into an Output Array
    let outputsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLOutput>) =
        Decode.object (fun get ->
            let outputs = get.Required.Field "outputs" outputArrayDecoder
            outputs
        )


    let private tryDecodeLegacyDockerFileMap (value: YAMLElement) : SchemaSaladString option =
        try
            let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
            if dict.Count = 0 then
                None
            elif dict.ContainsKey "$include" then
                // `$include` and `$import` are mutually exclusive; prefer `$include` when both are present.
                Some (Include (Decode.string dict.["$include"]))
            elif dict.ContainsKey "$import" then
                Some (Import (Decode.string dict.["$import"]))
            else
                // Legacy fallback: keep only the first map value and drop extra keys.
                dict.Values
                |> Seq.tryHead
                |> Option.map (fun v -> Literal (Decode.string v))
        with ex when isRecoverableDecodingError ex ->
            None

    /// Decode a YAMLElement into a DockerRequirement
    let dockerRequirementDecoder (get: Decode.IGetters): DockerRequirement =
        let dockerFile =
            get.Optional.Field "dockerFile" id
            |> Option.map (fun value ->
                match tryDecodeLegacyDockerFileMap value with
                | Some legacyValue -> legacyValue
                | None -> decodeSchemaSaladString value
            )

        let dockerReq = {
            DockerPull = get.Optional.Field "dockerPull" Decode.string
            DockerFile = dockerFile
            DockerImageId = get.Optional.Field "dockerImageId" Decode.string
            DockerLoad = get.Optional.Field "dockerLoad" Decode.string
            DockerImport = get.Optional.Field "dockerImport" Decode.string
            DockerOutputDirectory = get.Optional.Field "dockerOutputDirectory" Decode.string
        }
        dockerReq

    /// Decode a YAMLElement into an EnvVarRequirement array
    let envVarRequirementDecoder (get: Decode.IGetters): ResizeArray<EnvironmentDef> =
        let envDef = 
            get.Required.Field
                "envDef"
                (
                    Decode.resizearray 
                        (
                            Decode.object (fun get2 ->
                                {
                                    EnvName = get2.Required.Field "envName" Decode.string
                                    EnvValue = get2.Required.Field "envValue" Decode.string
                                }
                            )
                        )
                )
        envDef

    /// Decode a YAMLElement into a SoftwareRequirement array
    let softwareRequirementDecoder (get: Decode.IGetters): ResizeArray<SoftwarePackage> =
        let envDef = 
            get.Required.Field
                "packages"
                (
                    Decode.resizearray 
                        (
                            Decode.object (fun get2 ->
                                {
                                    Package = get2.Required.Field "package" Decode.string
                                    Version = get2.Optional.Field "version" (Decode.resizearray Decode.string)
                                    Specs = get2.Optional.Field "specs" (Decode.resizearray Decode.string)
                                }
                            )
                        )
                )
        envDef

    /// Decode a YAMLElement into an InitialWorkDirRequirement array.
    /// Supports both string/expression and Dirent listing items.
    let initialWorkDirRequirementDecoder (get: Decode.IGetters): ResizeArray<InitialWorkDirEntry> =
        let initialWorkDir =
            get.Required.Field
                "listing"
                (Decode.resizearray initialWorkDirEntryDecoder)
        initialWorkDir

    let loadListingRequirementDecoder (get: Decode.IGetters): LoadListingRequirementValue =
        let loadListing =
            get.Optional.Field "loadListing" Decode.string
            |> Option.defaultValue "no_listing"
        { LoadListing = loadListing }

    /// Decode a YAMLElement into a ResourceRequirementInstance
    let resourceRequirementDecoder (get: Decode.IGetters): ResourceRequirementInstance =
        ResourceRequirementInstance(
            get.Optional.Field "coresMin" id,
            get.Optional.Field "coresMax" id,
            get.Optional.Field "ramMin" id,
            get.Optional.Field "ramMax" id,
            get.Optional.Field "tmpdirMin" id,
            get.Optional.Field "tmpdirMax" id,
            get.Optional.Field "outdirMin" id,
            get.Optional.Field "outdirMax" id
        )
        
    let private schemaDefRequirementTypeDecoder (value: YAMLElement) : SchemaDefRequirementType =
        let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
        if dict.ContainsKey "name" then
            {
                Name = decodeStringOrExpression dict.["name"]
                Type_ = cwlTypeDecoder' value
            }
        else
            if dict.Count = 0 then
                raise (System.ArgumentException("SchemaDefRequirement entry cannot be empty."))
            let kv = dict |> Seq.head
            {
                Name = kv.Key
                Type_ = cwlTypeDecoder' kv.Value
            }

    /// Decode a YAMLElement into a SchemaDefRequirementType array
    let schemaDefRequirementDecoder (get: Decode.IGetters): ResizeArray<SchemaDefRequirementType> =
        get.Required.Field "types" (Decode.resizearray schemaDefRequirementTypeDecoder)

    let workReuseRequirementDecoder (get: Decode.IGetters): WorkReuseRequirementValue =
        {
            EnableReuse =
                get.Optional.Field "enableReuse" Decode.bool
                |> Option.defaultValue true
        }

    let networkAccessRequirementDecoder (get: Decode.IGetters): NetworkAccessRequirementValue =
        {
            NetworkAccess =
                get.Optional.Field "networkAccess" Decode.bool
                |> Option.defaultValue true
        }

    let inplaceUpdateRequirementDecoder (get: Decode.IGetters): InplaceUpdateRequirementValue =
        {
            InplaceUpdate =
                get.Optional.Field "inplaceUpdate" Decode.bool
                |> Option.defaultValue true
        }

    /// Decode a YAMLElement into a ToolTimeLimitRequirement value
    let toolTimeLimitRequirementDecoder (get: Decode.IGetters): ToolTimeLimitValue =
        let timeLimitElement = get.Required.Field "timelimit" id
        let tryParseNumber () =
            try
                // Numeric scalars should decode to the numeric form.
                // Non-numeric scalars (including expressions) fall back to expression form.
                Some (Decode.float timeLimitElement)
            with _ ->
                None
        match tryParseNumber () with
        | Some value -> ToolTimeLimitSeconds value
        | None -> ToolTimeLimitExpression (decodeStringOrExpression timeLimitElement)

    /// Decode all YAMLElements matching the Requirement type into a ResizeArray of Requirement
    let requirementFromTypeName cls get =
        match cls with
        | "InlineJavascriptRequirement" -> InlineJavascriptRequirement
        | "SchemaDefRequirement" -> SchemaDefRequirement (schemaDefRequirementDecoder get)
        | "DockerRequirement" -> DockerRequirement (dockerRequirementDecoder get)
        | "SoftwareRequirement" -> SoftwareRequirement (softwareRequirementDecoder get)
        | "LoadListingRequirement" -> LoadListingRequirement (loadListingRequirementDecoder get)
        | "InitialWorkDirRequirement" -> InitialWorkDirRequirement (initialWorkDirRequirementDecoder get)
        | "EnvVarRequirement" -> EnvVarRequirement (envVarRequirementDecoder get)
        | "ShellCommandRequirement" -> ShellCommandRequirement
        | "ResourceRequirement" -> ResourceRequirement (resourceRequirementDecoder get)
        | "WorkReuse"
        | "WorkReuseRequirement" -> WorkReuseRequirement (workReuseRequirementDecoder get)
        | "NetworkAccess"
        | "NetworkAccessRequirement" -> NetworkAccessRequirement (networkAccessRequirementDecoder get)
        | "InplaceUpdateRequirement"
        | "InplaceUpdate" -> InplaceUpdateRequirement (inplaceUpdateRequirementDecoder get)
        | "ToolTimeLimit"
        | "ToolTimeLimitRequirement" -> ToolTimeLimitRequirement (toolTimeLimitRequirementDecoder get)
        | "SubworkflowFeatureRequirement" -> SubworkflowFeatureRequirement
        | "ScatterFeatureRequirement" -> ScatterFeatureRequirement
        | "MultipleInputFeatureRequirement" -> MultipleInputFeatureRequirement
        | "StepInputExpressionRequirement" -> StepInputExpressionRequirement
        | _ -> raise (System.ArgumentException($"Invalid or unsupported requirement class: {cls}"))

    let requirementArrayDecoder : YAMLElement -> ResizeArray<Requirement> =
        fun yEle ->
            // helper: decode a single requirement object that contain 'class' field
            let decodeSingleRequirementObject (ele: YAMLElement) : Requirement =
                Decode.object (fun get ->
                    let cls = get.Required.Field "class" Decode.string
                    requirementFromTypeName cls get
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
                    )
                    |> ResizeArray
                ) yEle
            // INVALID CWL REQUIREMENTS  
            | other -> raise (System.ArgumentException($"Invalid CWL requirements syntax: {other}"))

    /// Access the requirements field and decode the YAMLElements into a Requirement array
    let requirementsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<Requirement> option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "requirements" requirementArrayDecoder
            requirements
        )

    /// Access the hints field and decode the YAMLElements into a Requirement array
    let hintsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<Requirement> option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "hints" requirementArrayDecoder
            requirements
        )

    /// Decode a YAMLElement into an InputBinding
    let inputBindingDecoder: (YAMLiciousTypes.YAMLElement -> InputBinding option) =
        Decode.object(fun get ->
            let outputBinding = 
                get.Optional.Field 
                    "inputBinding" 
                    (
                        Decode.object (fun get' ->
                            {
                                Prefix = get'.Optional.Field "prefix" Decode.string
                                Position = get'.Optional.Field "position" Decode.int
                                ItemSeparator = get'.Optional.Field "itemSeparator" Decode.string
                                Separate = get'.Optional.Field "separate" Decode.bool
                            }
                        )         
                    )
            outputBinding
        )

    /// Decode a YAMLElement into an Input array
    let inputArrayDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLInput>) =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let inputBinding = inputBindingDecoder value
                    
                    // Decode using unified cwlTypeDecoder'
                    let cwlType, optional = 
                        match value with
                        | YAMLElement.Object [YAMLElement.Value v] -> 
                            cwlTypeStringMatcher v.Value get
                        | YAMLElement.Object mappings ->
                            // Check if this has a "type" field
                            let hasTypeField =
                                mappings |> List.exists (fun m ->
                                    match m with
                                    | YAMLElement.Mapping (k, _) when k.Value = "type" -> true
                                    | _ -> false
                                )
                            if hasTypeField then
                                cwlTypeDecoder value
                            else
                                // No type field, treat as type string
                                match value with
                                | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value get
                                | _ -> raise (System.ArgumentException("Unexpected input format without type field"))
                        | _ -> raise (System.ArgumentException("Unexpected input format in inputArrayDecoder"))
                    
                    let input = CWLInput(key, cwlType)
                    if optional then
                        DynObj.setOptionalProperty "optional" (Some true) input
                    
                    if inputBinding.IsSome then
                        DynObj.setOptionalProperty "inputBinding" inputBinding input
                    input
            |]
            |> ResizeArray
        )

    /// Access the inputs field and decode the YAMLElements into an Input array
    let inputsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<CWLInput> option) =
        Decode.object (fun get ->
            let outputs = get.Optional.Field "inputs" inputArrayDecoder
            outputs
        )

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

    let private decodeStepInputFromValue (id: string) (value: YAMLElement) (allowScalarSource: bool) : StepInput =
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
        {
            Id = id
            Source = source
            DefaultValue = yamlElementOptionFieldDecoder "default" value
            ValueFrom = stringOptionFieldDecoder "valueFrom" value
            LinkMerge = linkMergeFieldDecoder "linkMerge" value
            PickValue = pickValueFieldDecoder "pickValue" value
            Doc = stringOptionFieldDecoder "doc" value
            LoadContents = boolOptionFieldDecoder "loadContents" value
            LoadListing = stringOptionFieldDecoder "loadListing" value
            Label = stringOptionFieldDecoder "label" value
        }

    let private decodeStepInputsFromMap (value: YAMLElement) : ResizeArray<StepInput> =
        let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
        [|
            for key in dict.Keys do
                decodeStepInputFromValue key dict.[key] true
        |]
        |> ResizeArray

    let private decodeStepInputFromArrayItem (item: YAMLElement) : StepInput =
        let id = stringFieldDecoder "id" item
        decodeStepInputFromValue id item false

    let private decodeStepInputsFromArray (items: YAMLElement list) : ResizeArray<StepInput> =
        items
        |> List.map decodeStepInputFromArrayItem
        |> ResizeArray

    let inputStepDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<StepInput>) =
        fun value ->
            match value with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                decodeStepInputsFromArray items
            | _ ->
                decodeStepInputsFromMap value

    let private decodeStepOutputItem (value: YAMLElement) : StepOutput =
        match value with
        | YAMLElement.Object [YAMLElement.Value v]
        | YAMLElement.Value v ->
            StepOutputString v.Value
        | _ ->
            let id = stringFieldDecoder "id" value
            StepOutputRecord { Id = id }

    let outputStepsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<StepOutput>) =
        Decode.object (fun get ->
            let outField = get.Optional.Field "out" id
            match outField with
            | Some (YAMLElement.Object []) ->
                ResizeArray()
            | Some (YAMLElement.Object [YAMLElement.Value v]) when v.Value = "[]" ->
                ResizeArray()
            | Some (YAMLElement.Object [YAMLElement.Sequence []]) | Some (YAMLElement.Sequence []) ->
                ResizeArray()
            | Some (YAMLElement.Object [YAMLElement.Sequence outputs])
            | Some (YAMLElement.Sequence outputs) ->
                outputs
                |> List.map decodeStepOutputItem
                |> ResizeArray
            | Some value ->
                ResizeArray [ decodeStepOutputItem value ]
            | None ->
                ResizeArray()
        )

    let docDecoder =
        Decode.object (fun get -> get.Optional.Field "doc" Decode.string)

    let labelDecoder: (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object (fun get -> get.Optional.Field "label" Decode.string)
    
    let private hasField (fieldName: string) (yamlElement: YAMLElement) : bool =
        match yamlElement with
        | YAMLElement.Object fields ->
            fields
            |> List.exists (function
                | YAMLElement.Mapping (k, _) when k.Value = fieldName -> true
                | _ -> false
            )
        | _ -> false

    let private withDefaultCwlVersion (defaultCwlVersion: string) (yamlElement: YAMLElement) : YAMLElement =
        match yamlElement with
        | YAMLElement.Object fields when hasField "cwlVersion" yamlElement ->
            yamlElement
        | YAMLElement.Object fields ->
            let key = { Value = "cwlVersion"; Comment = None }
            let value = YAMLElement.Object [YAMLElement.Value { Value = defaultCwlVersion; Comment = None }]
            YAMLElement.Object (YAMLElement.Mapping (key, value) :: fields)
        | _ ->
            yamlElement

    let rec workflowStepRunDecoder (defaultCwlVersion: string) (runValue: YAMLElement) : WorkflowStepRun =
        match runValue with
        | YAMLElement.Object [YAMLElement.Value v]
        | YAMLElement.Value v ->
            RunString v.Value
        | YAMLElement.Object _ ->
            let normalizedRun = withDefaultCwlVersion defaultCwlVersion runValue
            match decodeCWLProcessingUnitElement normalizedRun with
            | CommandLineTool tool -> WorkflowStepRunOps.fromTool tool
            | Workflow workflow -> WorkflowStepRunOps.fromWorkflow workflow
            | ExpressionTool expressionTool -> WorkflowStepRunOps.fromExpressionTool expressionTool
        | _ ->
            raise (System.ArgumentException($"Unsupported run value for workflow step: %A{runValue}"))

    and private decodeWorkflowStepFromValueWithId (defaultCwlVersion: string) (stepId: string) (value: YAMLElement) : WorkflowStep =
        let runValue = Decode.object (fun get' -> get'.Required.Field "run" id) value
        let run = workflowStepRunDecoder defaultCwlVersion runValue
        let inputs =
            Decode.object (fun get' ->
                get'.Optional.Field "in" inputStepDecoder
                |> Option.defaultValue (ResizeArray())
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
        wfStep

    and private decodeWorkflowStepFromArrayItem (defaultCwlVersion: string) (item: YAMLElement) : WorkflowStep =
        let stepId = stringFieldDecoder "id" item
        decodeWorkflowStepFromValueWithId defaultCwlVersion stepId item

    and stepArrayDecoderWithVersion (defaultCwlVersion: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<WorkflowStep>) =
        fun value ->
            match value with
            | YAMLElement.Object [YAMLElement.Sequence items]
            | YAMLElement.Sequence items ->
                items
                |> List.map (decodeWorkflowStepFromArrayItem defaultCwlVersion)
                |> ResizeArray
            | _ ->
                let dict = Decode.object (fun get -> get.Overflow.FieldList []) value
                [|
                    for key in dict.Keys do
                        decodeWorkflowStepFromValueWithId defaultCwlVersion key dict.[key]
                |]
                |> ResizeArray

    and stepsDecoderWithVersion (defaultCwlVersion: string) : (YAMLiciousTypes.YAMLElement -> ResizeArray<WorkflowStep>) =
        Decode.object (fun get ->
            get.Required.Field "steps" (stepArrayDecoderWithVersion defaultCwlVersion)
        )

    and commandLineToolDecoder (yamlCWL : YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoder yamlCWL
        let inputs = inputsDecoder yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let baseCommand = baseCommandDecoder yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
        let description =
            CWLToolDescription(
                outputs,
                cwlVersion
            )
        let metadata =
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (
                        get.Overflow.FieldList [
                            "inputs";
                            "outputs";
                            "class";
                            "id";
                            "label";
                            "doc";
                            "requirements";
                            "hints";
                            "cwlVersion";
                            "baseCommand";
                            "arguments";
                            "stdin";
                            "stderr";
                            "stdout";
                            "successCodes";
                            "temporaryFailCodes";
                            "permanentFailCodes"
                        ]
                    )
            ) |> ignore
            md
        yamlCWL
        |> Decode.object (fun get ->
            overflowDecoder
                description
                (
                    get.MultipleOptional.FieldList [
                        "id";
                        "arguments";
                        "stdin";
                        "stderr";
                        "stdout";
                        "successCodes";
                        "temporaryFailCodes";
                        "permanentFailCodes"
                    ]
                )
        ) |> ignore
        if inputs.IsSome then
            description.Inputs <- inputs
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if baseCommand.IsSome then
            description.BaseCommand <- baseCommand
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and expressionToolDecoder (yamlCWL: YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoder yamlCWL
        let inputs = inputsDecoder yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
        let expression =
            Decode.object (fun get -> get.Required.Field "expression" decodeStringOrExpression) yamlCWL
        let description =
            CWLExpressionToolDescription(
                outputs,
                expression,
                cwlVersion
            )
        let metadata =
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (
                        get.Overflow.FieldList [
                            "inputs";
                            "outputs";
                            "class";
                            "id";
                            "label";
                            "doc";
                            "requirements";
                            "hints";
                            "cwlVersion";
                            "expression";
                        ]
                    )
            ) |> ignore
            md
        yamlCWL
        |> Decode.object (fun get ->
            overflowDecoder
                description
                (
                    get.MultipleOptional.FieldList [
                        "id";
                    ]
                )
        ) |> ignore
        if inputs.IsSome then
            description.Inputs <- inputs
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and workflowDecoder (yamlCWL: YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoder yamlCWL
        let inputs =
            match inputsDecoder yamlCWL with
            | Some i -> i
            | None -> raise (System.InvalidOperationException("Inputs are required for a workflow"))
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let steps = stepsDecoderWithVersion cwlVersion yamlCWL
        let doc = docDecoder yamlCWL
        let label = labelDecoder yamlCWL
        let description =
            CWLWorkflowDescription(
                steps,
                inputs,
                outputs,
                cwlVersion
            )
        let metadata =
            let md = new DynamicObj ()
            yamlCWL
            |> Decode.object (fun get ->
                overflowDecoder
                    md
                    (
                        get.Overflow.FieldList [
                            "inputs";
                            "outputs";
                            "label";
                            "doc";
                            "class";
                            "steps";
                            "id";
                            "requirements";
                            "hints";
                            "cwlVersion";
                        ]
                    )
            ) |> ignore
            md
        yamlCWL
        |> Decode.object (fun get ->
            overflowDecoder
                description
                (
                    get.MultipleOptional.FieldList [
                        "id";
                    ]
                )
        ) |> ignore
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if doc.IsSome then
            description.Doc <- doc
        if label.IsSome then
            description.Label <- label
        if metadata.GetProperties(false) |> Seq.length > 0 then
            description.Metadata <- Some metadata
        description

    and decodeCWLProcessingUnitElement (yamlCWL: YAMLElement) =
        let cls = classDecoder yamlCWL
        match cls with
        | "CommandLineTool" -> CommandLineTool (commandLineToolDecoder yamlCWL)
        | "Workflow" -> Workflow (workflowDecoder yamlCWL)
        | "ExpressionTool" -> ExpressionTool (expressionToolDecoder yamlCWL)
        | _ -> raise (System.ArgumentException($"Invalid or unsupported CWL class: {cls}"))

    let stepArrayDecoder = stepArrayDecoderWithVersion "v1.2"

    let stepsDecoder = stepsDecoderWithVersion "v1.2"

    /// Decode a CWL file string written in the YAML format into a CWLToolDescription
    let decodeCommandLineTool (cwl: string) =
        let yamlCWL = readSanitizedYaml cwl
        commandLineToolDecoder yamlCWL

    /// Decode a CWL file string written in the YAML format into a CWLWorkflowDescription
    let decodeWorkflow (cwl: string) =
        let yamlCWL = readSanitizedYaml cwl
        workflowDecoder yamlCWL

    /// Decode a CWL file string written in the YAML format into a CWLExpressionToolDescription
    let decodeExpressionTool (cwl: string) =
        let yamlCWL = readSanitizedYaml cwl
        expressionToolDecoder yamlCWL

    let decodeCWLProcessingUnit (cwl:string) =
        let yamlCWL = readSanitizedYaml cwl
        decodeCWLProcessingUnitElement yamlCWL

module DecodeParameters =

    let cwlParameterReferenceDecoder (get : Decode.IGetters) (key: string) (yEle: YAMLElement): CWLParameterReference =
        match yEle with
        | YAMLElement.Object[YAMLElement.Value v] ->
            CWLParameterReference(
                key = key,
                values = ResizeArray [v.Value]
            )
        | YAMLElement.Object[YAMLElement.Mapping (_,YAMLElement.Object [YAMLElement.Value v1]) ; YAMLElement.Mapping (_,YAMLElement.Object [YAMLElement.Value v2])] ->
            let t,b = Decode.cwlTypeStringMatcher v1.Value get
            CWLParameterReference(
                key = key,
                values = ResizeArray [v2.Value],
                type_ = t
            )
        | YAMLElement.Object[YAMLElement.Sequence s] ->
            // Check what type of sequence this is by examining the first element
            match s |> List.tryHead with
            | Some (YAMLElement.Object mappings) ->
                // Check if mappings actually contains Mapping elements (not just Value)
                let hasMappings = mappings |> List.exists (function
                    | YAMLElement.Mapping _ -> true
                    | _ -> false)
                
                if not hasMappings then
                    // Not actually mappings, just wrapped values - use default decoder
                    CWLParameterReference(
                        key = key,
                        values = Decode.resizearray Decode.string (YAMLElement.Sequence s)
                    )
                else
                    // Check if it's a File/Directory object with a "class" field
                    let hasClassField = mappings |> List.exists (function
                        | YAMLElement.Mapping (k, _) when k.Value = "class" -> true
                        | _ -> false)
                    
                    if hasClassField then
                        // Sequence of File/Directory objects - extract paths
                        let paths = ResizeArray<string>()
                        for item in s do
                            match item with
                            | YAMLElement.Object itemMappings ->
                                for mapping in itemMappings do
                                    match mapping with
                                    | YAMLElement.Mapping (k, YAMLElement.Object [YAMLElement.Value v]) when k.Value = "path" ->
                                        paths.Add(v.Value)
                                    | _ -> ()
                            | _ -> ()
                        // Since this is a sequence of Files, the type should be File[] (Array)
                        CWLParameterReference(
                            key = key,
                            values = paths,
                            type_ = Array { Items = File (FileInstance()); Label = None; Doc = None; Name = None }
                        )
                    else
                        // Sequence of complex record objects (no "class" field)
                        // Return empty placeholder
                        CWLParameterReference(key = key, values = ResizeArray())
            | _ ->
                // Simple values sequence (strings, numbers, etc.) or empty
                // Use original decoder logic
                CWLParameterReference(
                    key = key,
                    values = Decode.resizearray Decode.string (YAMLElement.Sequence s)
                )
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

