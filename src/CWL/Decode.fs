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
    let cwlArrayTypeDecoder: (YAMLiciousTypes.YAMLElement -> CWLType) =
        Decode.object (fun get ->
            let items = get.Required.Field "items" Decode.string
            match items with
            | "File" -> Array (File (FileInstance ()))
            | "Directory" -> Array (Directory (DirectoryInstance ()))
            | "Dirent" -> Array (get.Required.Field "listing" direntDecoder)
            | "string" -> Array String
            | "int" -> Array Int
            | "long" -> Array Long
            | "float" -> Array Float
            | "double" -> Array Double
            | "boolean" -> Array Boolean
            | _ -> failwith "Invalid CWL type"
        )

    /// Match the input string to the possible CWL types and checks if it is optional
    let cwlTypeStringMatcher (t: string) (get: Decode.IGetters) =
        let optional, newT =
            if t.EndsWith("?") then
                true, t.Replace("?", "")
            else
                false, t
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
        | "File[]" -> Array (File (FileInstance ()))
        | "Directory[]" -> Array (Directory (DirectoryInstance ()))
        | "Dirent[]" -> Array (get.Required.Field "listing" direntDecoder)
        | "string[]" -> Array String
        | "int[]" -> Array Int
        | "long[]" -> Array Long
        | "float[]" -> Array Float
        | "double[]" -> Array Double
        | "boolean[]" -> Array Boolean
        | "stdout" -> Stdout
        | "null" -> Null
        | _ -> failwith "Invalid CWL type"
        , optional

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
                let cwlTypeArray = get.Required.Field "type" cwlArrayTypeDecoder
                cwlTypeArray, false
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
                    let cwlType,optional = 
                        match value with
                        | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value get
                        | _ -> cwlTypeDecoder value
                    let input =
                        CWLInput(
                            key,
                            cwlType
                        )
                    if inputBinding.IsSome then
                        DynObj.setOptionalProperty "inputBinding" inputBinding input
                    if optional then
                        DynObj.setOptionalProperty "optional" (Some true) input
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
            get.Optional.Field "baseCommand" (Decode.resizearray Decode.string)
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

    let inputStepDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<StepInput>) =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let source =
                        let s1 =
                            match value with
                            | YAMLElement.Object [YAMLElement.Value v] -> Some v.Value
                            | _ -> None
                        let s2 = stringOptionFieldDecoder "source" value
                        match s1,s2 with
                        | Some s1, _ -> Some s1
                        | _, Some s2 -> Some s2
                        | _ -> None
                    let defaultValue = stringOptionFieldDecoder "default" value
                    let valueFrom = stringOptionFieldDecoder "valueFrom" value
                    { Id = key; Source = source; DefaultValue = defaultValue; ValueFrom = valueFrom }
            |]
            |> ResizeArray
        )

    let outputStepsDecoder: (YAMLiciousTypes.YAMLElement -> ResizeArray<string>) =
        Decode.object (fun get ->
            let outputs = get.Required.Field "out" (Decode.resizearray Decode.string)
            outputs
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
                        "label";
                        "doc";
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
                            "class";
                            "steps";
                            "id";
                            "label";
                            "doc";
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
                        "label";
                        "doc"
                    ]
                )
        ) |> ignore
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
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
            {
                Key = key
                Values = ResizeArray [v.Value]
                Type = None
            }
        | YAMLElement.Object[YAMLElement.Mapping (_,YAMLElement.Object [YAMLElement.Value v1]) ; YAMLElement.Mapping (_,YAMLElement.Object [YAMLElement.Value v2])] ->
            let t,b = Decode.cwlTypeStringMatcher v1.Value get
            {
                Key = key
                Values = ResizeArray [v2.Value]
                Type = Some t
            }
        | YAMLElement.Object[YAMLElement.Sequence s] ->
            {
                Key = key
                Values = Decode.resizearray Decode.string (YAMLElement.Sequence s)
                Type = None
            }
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