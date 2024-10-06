namespace ARCtrl.CWL

open YAMLicious
open YAMLicious.YAMLiciousTypes
open CWLTypes
open Requirements
open Inputs
open Inputs.Workflow
open Outputs
open Outputs.Workflow
open WorkflowSteps
open DynamicObj

module Decode =
    
    let outputBindingGlobDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding) =
        Decode.object (fun get ->
            let glob = get.Optional.Field "glob" Decode.string
            { Glob = glob }
        )

    let outputBindingDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding option) =
        Decode.object(fun get ->
            let outputBinding = get.Optional.Field "outputBinding" outputBindingGlobDecoder
            outputBinding
        )

    let cwlArrayTypeDecoder: (YAMLiciousTypes.YAMLElement -> CWLType) =
        Decode.object (fun get ->
            let items = get.Required.Field "items" Decode.string
            match items with
            | "File" -> Array (File (FileInstance ()))
            | "Directory" -> Array (Directory (DirectoryInstance ()))
            | "Dirent" -> Array (Dirent { Entry = ""; Entryname = None; Writable = None })
            | "string" -> Array String
            | "int" -> Array Int
            | "long" -> Array Long
            | "float" -> Array Float
            | "double" -> Array Double
            | "boolean" -> Array Boolean
            | _ -> failwith "Invalid CWL type"
        )

    let cwlTypeStringMatcher t =
        match t with
        | "File" -> File (FileInstance ())
        | "Directory" -> Directory (DirectoryInstance ())
        | "Dirent" -> Dirent { Entry = ""; Entryname = None; Writable = None }
        | "string" -> String
        | "int" -> Int
        | "long" -> Long
        | "float" -> Float
        | "double" -> Double
        | "boolean" -> Boolean
        | "File[]" -> Array (File (FileInstance ()))
        | "Directory[]" -> Array (Directory (DirectoryInstance ()))
        | "Dirent[]" -> Array (Dirent { Entry = ""; Entryname = None; Writable = None })
        | "string[]" -> Array String
        | "int[]" -> Array Int
        | "long[]" -> Array Long
        | "float[]" -> Array Float
        | "double[]" -> Array Double
        | "boolean[]" -> Array Boolean
        | "stdout" -> Stdout
        | "null" -> Null
        | _ -> failwith "Invalid CWL type"

    let cwlTypeDecoder: (YAMLiciousTypes.YAMLElement -> CWLType) =
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
                cwlTypeStringMatcher t
            | None -> 
                let cwlTypeArray = get.Required.Field "type" cwlArrayTypeDecoder
                cwlTypeArray
        )

    let outputArrayDecoder: (YAMLiciousTypes.YAMLElement -> Output[]) =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let outputBinding = outputBindingDecoder value
                    let cwlType = 
                        match value with
                        | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value
                        | _ -> cwlTypeDecoder value
                    let output =
                        Output(
                            key,
                            cwlType
                        )
                    if outputBinding.IsSome then
                        DynObj.setValueOpt output "outputBinding" outputBinding
                    output
            |]     
        )
    
    let outputsDecoder: (YAMLiciousTypes.YAMLElement -> Output[]) =
        Decode.object (fun get ->
            let outputs = get.Required.Field "outputs" outputArrayDecoder
            outputs
        )

    let dockerRequirementDecoder (get: Decode.IGetters): DockerRequirement =
        let dockerReq = {
            DockerPull = get.Optional.Field "dockerPull" Decode.string
            DockerFile = get.Optional.Field "dockerFile" (Decode.map id Decode.string )
            DockerImageId = get.Optional.Field "dockerImageId" Decode.string
        }
        dockerReq

    let envVarRequirementDecoder (get: Decode.IGetters): EnvironmentDef[] =
        let envDef = 
            get.Required.Field
                "envDef"
                (
                    Decode.array 
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

    let softwareRequirementDecoder (get: Decode.IGetters): SoftwarePackage[] =
        let envDef = 
            get.Required.Field
                "packages"
                (
                    Decode.array 
                        (
                            Decode.object (fun get2 ->
                                {
                                    Package = get2.Required.Field "package" Decode.string
                                    Version = get2.Optional.Field "version" (Decode.array Decode.string)
                                    Specs = get2.Optional.Field "specs" (Decode.array Decode.string)
                                }
                            )
                        )
                )
        envDef

    let decodeStringOrExpression (yEle:YAMLElement) =
        match yEle with
        | YAMLElement.Value v | YAMLElement.Object [YAMLElement.Value v] -> v.Value
        | YAMLElement.Object [YAMLElement.Mapping (c,YAMLElement.Object [YAMLElement.Value v])] -> sprintf "%s: %s" c.Value v.Value
        | _ -> failwithf "%A" yEle

    let initialWorkDirRequirementDecoder (get: Decode.IGetters): CWLType[] =
        let initialWorkDir =
            //TODO: Support more than dirent
            get.Required.Field
                "listing"
                (
                    Decode.array 
                        (
                            Decode.object (fun get2 ->
                                Dirent
                                    {
                                        // BUG: Entry Requires an Entryname to be present when it's an expression
                                        Entry = get2.Required.Field "entry" decodeStringOrExpression
                                        Entryname = get2.Optional.Field "entryname" decodeStringOrExpression
                                        Writable = get2.Optional.Field "writable" Decode.bool
                                    }
                            )
                        )
                    )
        initialWorkDir

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
        

    let schemaDefRequirementDecoder (get: Decode.IGetters): SchemaDefRequirementType[] =
        let schemaDef =
            get.Required.Field 
                "types" 
                (
                    Decode.array
                        (
                            Decode.map id Decode.string
                        )
                )
                |> Array.map (fun m -> SchemaDefRequirementType(m.Keys |> Seq.item 0, m.Values |> Seq.item 0))
        schemaDef


    let requirementArrayDecoder: (YAMLiciousTypes.YAMLElement -> Requirement[]) =
        Decode.array 
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
                    | "NetworkAccess" -> NetworkAccessRequirement
                )
            )

    let requirementsDecoder: (YAMLiciousTypes.YAMLElement -> Requirement[] option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "requirements" requirementArrayDecoder
            requirements
        )

    let hintsDecoder: (YAMLiciousTypes.YAMLElement -> Requirement[] option) =
        Decode.object (fun get ->
            let requirements = get.Optional.Field "hints" requirementArrayDecoder
            requirements
        )

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

    let inputArrayDecoder: (YAMLiciousTypes.YAMLElement -> Input[]) =
        Decode.object (fun get ->
            let dict = get.Overflow.FieldList []
            [|
                for key in dict.Keys do
                    let value = dict.[key]
                    let inputBinding = inputBindingDecoder value
                    let cwlType = 
                        match value with
                        | YAMLElement.Object [YAMLElement.Value v] -> cwlTypeStringMatcher v.Value
                        | _ -> cwlTypeDecoder value
                    let input =
                        Input(
                            key,
                            cwlType
                        )
                    if inputBinding.IsSome then
                        DynObj.setValueOpt input "inputBinding" inputBinding
                    input
            |]     
        )
    
    let inputsDecoder: (YAMLiciousTypes.YAMLElement -> Input[] option) =
        Decode.object (fun get ->
            let outputs = get.Optional.Field "inputs" inputArrayDecoder
            outputs
        )

    let baseCommandDecoder: (YAMLiciousTypes.YAMLElement -> string [] option) =
        Decode.object (fun get ->
            get.Optional.Field "baseCommand" (Decode.array Decode.string)
        )

    let versionDecoder: (YAMLiciousTypes.YAMLElement -> string) =
        Decode.object (fun get -> get.Required.Field "cwlVersion" Decode.string)

    let classDecoder: (YAMLiciousTypes.YAMLElement -> Class) =
        Decode.object (fun get ->
            match get.Required.Field "class" Decode.string with
            | "Workflow" -> Workflow
            | "CommandLineTool" -> CommandLineTool
            | "ExpressionTool" -> ExpressionTool
            | _ -> failwith "Invalid class"
        )

    let decodeCommandLineTool (cwl: string) =
        let yamlCWL = Decode.read cwl
        let cwlVersion = versionDecoder yamlCWL
        let outputs = outputsDecoder yamlCWL
        let inputs = inputsDecoder yamlCWL
        let requirements = requirementsDecoder yamlCWL
        let hints = hintsDecoder yamlCWL
        let baseCommand = baseCommandDecoder yamlCWL
        let description =
            CWLProcessingUnits.CWLToolDescription(
                cwlVersion,
                CommandLineTool,
                outputs
            )
        if inputs.IsSome then
            description.Inputs <- inputs
        if requirements.IsSome then
            description.Requirements <- requirements
        if hints.IsSome then
            description.Hints <- hints
        if baseCommand.IsSome then
            description.BaseCommand <- baseCommand
        description

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

    let inputStepDecoder: (YAMLiciousTypes.YAMLElement -> StepInput []) =
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
        )

    let outputStepsDecoder: (YAMLiciousTypes.YAMLElement -> string []) =
        Decode.object (fun get ->
            let outputs = get.Required.Field "out" (Decode.array Decode.string)
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
        )
    
    let stepsDecoder =
        Decode.object (fun get ->
            let steps = get.Required.Field "steps" stepArrayDecoder
            steps
        )

    let rec overflowDecoder (dynObj: DynamicObj) (dict: System.Collections.Generic.Dictionary<string,YAMLElement>) =
        for e in dict do
            match e.Value with
            | YAMLElement.Object [YAMLElement.Value v] -> 
                DynObj.setValue dynObj e.Key v.Value
            | YAMLElement.Object [YAMLElement.Sequence s] ->
                let newDynObj = new DynamicObj ()
                (s |> List.map ((Decode.object (fun get ->  (get.Overflow.FieldList []))) >> overflowDecoder newDynObj))
                |> List.iter (fun x ->
                    DynObj.setValue
                        dynObj
                        e.Key
                        x
                )
        dynObj
