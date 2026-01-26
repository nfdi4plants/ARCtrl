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

    /// Decode a YAMLElement which is either a string or expression into a string
    let decodeStringOrExpression (yEle:YAMLElement) =
        match yEle with
        | YAMLElement.Value v | YAMLElement.Object [YAMLElement.Value v] -> v.Value
        | YAMLElement.Object [YAMLElement.Mapping (c,YAMLElement.Object [YAMLElement.Value v])] -> sprintf "%s: %s" c.Value v.Value
        | _ -> failwithf "%A" yEle

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
                    // BUG: Entry Requires an Entryname to be present when it's an expression
                    Entry = get.Required.Field "entry" decodeStringOrExpression
                    Entryname = get.Optional.Field "entryname" decodeStringOrExpression
                    Writable = get.Optional.Field "writable" Decode.bool
                }
        )

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
        | _ -> failwith $"Invalid CWL simple type: {s}"

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
                with _ -> None
        else
            None

    /// Decode an InputArraySchema from a YAMLElement
    and inputArraySchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputArraySchema) =
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

    /// Decode an InputRecordSchema from a YAMLElement
    and inputRecordSchemaDecoder: (YAMLiciousTypes.YAMLElement -> InputRecordSchema) =
        Decode.object (fun get ->
            // Decode fields using Overflow to get the map form
            let fieldsDict = 
                get.Optional.Field 
                    "fields" 
                    (Decode.object (fun get2 -> get2.Overflow.FieldList []))
            
            let decodedFields =
                match fieldsDict with
                | Some dict ->
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
                | _ -> failwith "Unexpected type format in cwlTypeDecoder'"
            ) element
        | _ -> failwith "Unexpected YAMLElement in cwlTypeDecoder'"
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
                            | _ -> failwith "Unexpected YAMLElement"
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


    /// Decode a YAMLElement into a DockerRequirement
    let dockerRequirementDecoder (get: Decode.IGetters): DockerRequirement =
        let dockerReq = {
            DockerPull = get.Optional.Field "dockerPull" Decode.string
            DockerFile = get.Optional.Field "dockerFile" (Decode.map id Decode.string )
            DockerImageId = get.Optional.Field "dockerImageId" Decode.string
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

    /// Decode a YAMLElement into a InitialWorkDirRequirement array
    let initialWorkDirRequirementDecoder (get: Decode.IGetters): ResizeArray<CWLType> =
        let initialWorkDir =
            //TODO: Support more than dirent
            get.Required.Field
                "listing"
                (Decode.resizearray direntDecoder)
        initialWorkDir

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
        
    /// Decode a YAMLElement into a SchemaDefRequirementType array
    let schemaDefRequirementDecoder (get: Decode.IGetters): ResizeArray<SchemaDefRequirementType> =
        let schemaDef =
            get.Required.Field 
                "types" 
                (
                    Decode.resizearray
                        (
                            Decode.map id Decode.string
                        )
                )
                |> ResizeArray.map (fun m -> SchemaDefRequirementType(m.Keys |> Seq.item 0, m.Values |> Seq.item 0))
        schemaDef

    /// Decode a YAMLElement into a ToolTimeLimitRequirement
    let toolTimeLimitRequirementDecoder (get: Decode.IGetters): float =
        get.Required.Field "timelimit" Decode.float

    /// Decode all YAMLElements matching the Requirement type into a ResizeArray of Requirement
    let requirementArrayDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<Requirement>) =
        Decode.resizearray 
            (
                Decode.object (fun get ->
                    let cls = get.Required.Field "class" Decode.string
                    match cls with
                    | "InlineJavascriptRequirement" -> InlineJavascriptRequirement
                    | "SchemaDefRequirement" -> SchemaDefRequirement (schemaDefRequirementDecoder get)
                    | "DockerRequirement" -> DockerRequirement (dockerRequirementDecoder get)
                    | "SoftwareRequirement" -> SoftwareRequirement (softwareRequirementDecoder get)
                    | "InitialWorkDirRequirement" -> InitialWorkDirRequirement (initialWorkDirRequirementDecoder get)
                    | "EnvVarRequirement" -> EnvVarRequirement (envVarRequirementDecoder get)
                    | "ShellCommandRequirement" -> ShellCommandRequirement
                    | "ResourceRequirement" -> ResourceRequirement (resourceRequirementDecoder get)
                    | "WorkReuse" -> WorkReuseRequirement
                    | "NetworkAccess" -> NetworkAccessRequirement
                    | "InplaceUpdateRequirement" -> InplaceUpdateRequirement
                    | "ToolTimeLimit" -> ToolTimeLimitRequirement (toolTimeLimitRequirementDecoder get)
                    | "SubworkflowFeatureRequirement" -> SubworkflowFeatureRequirement
                    | "ScatterFeatureRequirement" -> ScatterFeatureRequirement
                    | "MultipleInputFeatureRequirement" -> MultipleInputFeatureRequirement
                    | "StepInputExpressionRequirement" -> StepInputExpressionRequirement
                    | _ -> failwith "Invalid requirement"
                )
            )

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
                                | _ -> failwith "Unexpected input format without type field"
                        | _ -> failwith "Unexpected input format in inputArrayDecoder"
                    
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

    let stringFieldDecoder field : (YAMLiciousTypes.YAMLElement -> string) =
        Decode.object(fun get ->
            let fieldValue = get.Required.Field field Decode.string
            fieldValue
        )

    /// Decode a YAMLElement into a ResizeArray<string> option for the source field
    /// Handles both single string values and arrays of strings
    let sourceArrayFieldDecoder field : (YAMLiciousTypes.YAMLElement -> ResizeArray<string> option) =
        Decode.object(fun get ->
            let sourceField = get.Optional.Field field id
            match sourceField with
            | Some sourceValue ->
                match sourceValue with
                | YAMLElement.Object [YAMLElement.Value v] -> 
                    // Single string value
                    Some (ResizeArray([v.Value]))
                | YAMLElement.Value v ->
                    // Single string value (unwrapped)
                    Some (ResizeArray([v.Value]))
                | YAMLElement.Object [YAMLElement.Sequence s] ->
                    // Array of strings wrapped
                    Some (Decode.resizearray Decode.string (YAMLElement.Sequence s))
                | YAMLElement.Sequence s ->
                    // Array of strings unwrapped
                    Some (Decode.resizearray Decode.string (YAMLElement.Sequence s))
                | _ -> None
            | None -> None
        )

    let inputStepDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<StepInput>) =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let source =
                        let s1 =
                            match value with
                            | YAMLElement.Object [YAMLElement.Value v] -> Some (ResizeArray([v.Value]))
                            | _ -> None
                        let s2 = sourceArrayFieldDecoder "source" value
                        match s1,s2 with
                        | Some s1, _ -> Some s1
                        | _, Some s2 -> Some s2
                        | _ -> None
                    let defaultValue = stringOptionFieldDecoder "default" value
                    let valueFrom = stringOptionFieldDecoder "valueFrom" value
                    let linkMerge = stringOptionFieldDecoder "linkMerge" value
                    { Id = key; Source = source; DefaultValue = defaultValue; ValueFrom = valueFrom; LinkMerge = linkMerge }
            |]
            |> ResizeArray
        )

    let outputStepsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<string>) =
        Decode.object (fun get ->
            let outField = get.Optional.Field "out" id
            match outField with
            | Some (YAMLElement.Object [YAMLElement.Value v]) when v.Value = "[]" ->
                // Empty array represented as string "[]"
                ResizeArray()
            | Some (YAMLElement.Object [YAMLElement.Sequence []]) | Some (YAMLElement.Sequence []) ->
                // Empty sequence
                ResizeArray()
            | Some value ->
                // Non-empty array - decode normally
                Decode.resizearray Decode.string value
            | None ->
                // Field not present
                ResizeArray()
        )

    let stepArrayDecoder =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let run = stringFieldDecoder "run" value
                    let inputs = Decode.object (fun get -> get.Required.Field "in" inputStepDecoder) value
                    let outputs = {Id = outputStepsDecoder value}
                    let requirements = requirementsDecoder value
                    let hints = hintsDecoder value
                    let wfStep =
                        WorkflowStep(
                            key,
                            inputs,
                            outputs,
                            run
                        )
                    if requirements.IsSome then
                        wfStep.Requirements <- requirements
                    if hints.IsSome then
                        wfStep.Hints <- hints
                    wfStep
            |]
            |> ResizeArray
        )

    let docDecoder =
        Decode.object (fun get -> get.Optional.Field "doc" Decode.string)

    let labelDecoder: (YAMLiciousTypes.YAMLElement -> string option) =
        Decode.object (fun get -> get.Optional.Field "label" Decode.string)
    
    let stepsDecoder =
        Decode.object (fun get ->
            let steps = get.Required.Field "steps" stepArrayDecoder
            steps
        )

    let commandLineToolDecoder (yamlCWL : YAMLElement) =
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


    /// Decode a CWL file string written in the YAML format into a CWLToolDescription
    let decodeCommandLineTool (cwl: string) =
        let yamlCWL = Decode.read cwl
        commandLineToolDecoder yamlCWL

    let workflowDecoder (yamlCWL: YAMLElement) =
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoder yamlCWL
        let inputs =
            match inputsDecoder yamlCWL with
            | Some i -> i
            | None -> failwith "Inputs are required for a workflow"
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let steps = stepsDecoder yamlCWL
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


    /// Decode a CWL file string written in the YAML format into a CWLWorkflowDescription
    let decodeWorkflow (cwl: string) =
        let yamlCWL = Decode.read cwl
        workflowDecoder yamlCWL

    let decodeCWLProcessingUnit (cwl:string) =
        let yamlCWL = Decode.read cwl
        let cls = classDecoder yamlCWL
        match cls with
        | "CommandLineTool" -> CommandLineTool (commandLineToolDecoder yamlCWL)
        | "Workflow" -> Workflow (workflowDecoder yamlCWL)
        | _ -> failwithf "Invalid CWL class: %s" cls

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
        | _ -> failwith $"{yEle}"

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