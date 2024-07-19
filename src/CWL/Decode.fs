namespace ARCtrl.CWL

open YAMLicious

module Decode =
    
    let outputBindingGlobDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding) =
        Decode.object (fun get ->
            let glob = get.Optional.Field "glob" Decode.string
            { Glob = glob }
        )

    let outputBindingDecoder: (YAMLiciousTypes.YAMLElement -> OutputBinding) =
        Decode.object(fun get ->
            let outputBinding = get.Required.Field "outputBinding" outputBindingGlobDecoder
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
                    let cwlType = cwlTypeDecoder value
                    { Name = key; Type = cwlType; OutputBinding = Some outputBinding }
            |]     
        )
    
    let outputsDecoder: (YAMLiciousTypes.YAMLElement -> Output[]) =
        Decode.object (fun get ->
            let outputs = get.Required.Field "outputs" outputArrayDecoder
            outputs
        )

    let requirementArrayDecoder: (YAMLiciousTypes.YAMLElement -> Requirement[]) =
        Decode.array 
            (
                Decode.object (fun get ->
                    let cls = get.Required.Field "class" Decode.string
                    match cls with
                    | "InlineJavascriptRequirement" -> InlineJavascriptRequirement
                    | "SchemaDefRequirement" -> SchemaDefRequirement [||]
                    | "DockerRequirement" ->
                        let dockerReq = {
                            DockerPull = get.Optional.Field "dockerPull" Decode.string
                            DockerFile = Some ""
                            DockerImageId = get.Optional.Field "dockerImageId" Decode.string
                        }
                        DockerRequirement dockerReq
                    | "SoftwareRequirement" -> SoftwareRequirement [||]
                    | "InitialWorkDirRequirement" -> InitialWorkDirRequirement [||]
                    | "EnvVarRequirement" -> EnvVarRequirement {EnvName = ""; EnvValue = ""}
                    | "ShellCommandRequirement" -> ShellCommandRequirement
                    | "ResourceRequirement" -> ResourceRequirement (ResourceRequirementInstance())
                    | "NetworkAccess" -> NetworkAccessRequirement
 
                )
            )

    let requirementsDecoder: (YAMLiciousTypes.YAMLElement -> Requirement[]) =
        Decode.object (fun get ->
            let requirements = get.Required.Field "requirements" requirementArrayDecoder
            requirements
        )

    let hintsDecoder: (YAMLiciousTypes.YAMLElement -> Requirement[]) =
        Decode.object (fun get ->
            let requirements = get.Required.Field "hints" requirementArrayDecoder
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
                    let cwlType = cwlTypeDecoder value
                    { Name = key; Type = cwlType; InputBinding = inputBinding }
            |]     
        )
    
    let inputsDecoder: (YAMLiciousTypes.YAMLElement -> Input[]) =
        Decode.object (fun get ->
            let outputs = get.Required.Field "inputs" inputArrayDecoder
            outputs
        )

    let decodeAll =
        let yamlCWL = Decode.read exampleCWL
        CWL(
            cwlVersion = Decode.object (fun get -> get.Required.Field "cwlVersion" Decode.string) yamlCWL,
            cls = 
                Decode.object (fun get ->
                    match get.Required.Field "class" Decode.string with
                    | "Workflow" -> Workflow
                    | "CommandLineTool" -> CommandLineTool
                    | "ExpressionTool" -> ExpressionTool
                    | _ -> failwith "Invalid class"
                ) yamlCWL
                ,
            outputs = outputsDecoder yamlCWL,
            inputs = inputsDecoder yamlCWL,
            //baseCommand = Decode.object (fun get -> get.Optional.Field "baseCommand" (Decode.array Decode.string)) yamlCWL,
            requirements = requirementsDecoder yamlCWL,
            hints = hintsDecoder yamlCWL
        )