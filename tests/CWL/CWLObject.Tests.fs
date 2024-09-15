module Tests.CWLObject

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open ARCtrl.CWL.Inputs
open ARCtrl.CWL.Outputs
open YAMLicious
open TestingUtils


let decodeOutput =
    TestUtil.outputs
    |> Decode.read
    |> Decode.outputsDecoder

let decodeInput =
    TestUtil.inputs
    |> Decode.read
    |> Decode.inputsDecoder

let decodeRequirement =
    TestUtil.requirements
    |> Decode.read
    |> Decode.requirementsDecoder

let decodeCWLToolDescription =
    TestUtil.cwl
    |> Decode.decodeAll

let testOutput =
    testList "outputs with basetypes and array" [
        testCase "Length" <| fun _ -> Expect.isTrue (4 = decodeOutput.Length) "Length of outputs is not 4"
        testList "File" [
            let fileItem = decodeOutput.[0]
            testCase "Name" <| fun _ -> Expect.isTrue ("output" = fileItem.Name) "Name of output is not 'output'"
            testCase "Type" <| fun _ -> Expect.isTrue ((File (FileInstance())) = fileItem.Type) "Type of output is not File"
            testCase "OutputBinding" <| fun _ -> Expect.isTrue (Some {Glob = Some "./arc/runs/fsResult1/result.csv"} = fileItem.OutputBinding) "OutputBinding of output is not Some Pattern"
        ]
        testList "Directory" [
            let directoryItem = decodeOutput.[1]
            testCase "Name" <| fun _ -> Expect.isTrue ("example" = directoryItem.Name) "Name of output is not 'example'"
            testCase "Type" <| fun _ -> Expect.isTrue ((Directory (DirectoryInstance())) = directoryItem.Type) "Type of output is not Directory"
            testCase "OutputBinding" <| fun _ -> Expect.isTrue (Some {Glob = Some "./arc/runs/fsResult1/example.csv"} = directoryItem.OutputBinding) "OutputBinding of output is not Some Pattern"
        ]
        testList "File Array" [
            let fileArrayItem = decodeOutput.[2]
            testCase "Name" <| fun _ -> Expect.isTrue ("exampleArray1" = fileArrayItem.Name) "Name of output is not 'exampleArray1'"
            testCase "Type" <| fun _ -> Expect.isTrue ((Array (File (FileInstance()))) = fileArrayItem.Type) "Type of output is not Array File"
            testCase "OutputBinding" <| fun _ -> Expect.isTrue (Some {Glob = Some "./arc/runs/fsResult1/example.csv"} = fileArrayItem.OutputBinding) "OutputBinding of output is not Some Pattern"
        ]
        testList "File Array 2" [
            let fileArrayItem = decodeOutput.[3]
            testCase "Name" <| fun _ -> Expect.isTrue ("exampleArray2" = fileArrayItem.Name) "Name of output is not 'exampleArray2'"
            testCase "Type" <| fun _ -> Expect.isTrue ((Array (File (FileInstance()))) = fileArrayItem.Type) "Type of output is not Array File"
            testCase "OutputBinding" <| fun _ -> Expect.isTrue (Some {Glob = Some "./arc/runs/fsResult1/example.csv"} = fileArrayItem.OutputBinding) "OutputBinding of output is not Some Pattern"
        ]
    ]

let testInput =
    testList "inputs with Directory, File and string" [
        testCase "Length" <| fun _ -> Expect.isTrue (3 = decodeInput.Length) "Length of inputs is not 3"
        testList "Directory" [
            let directoryItem = decodeInput.[0]
            testCase "Name" <| fun _ -> Expect.isTrue ("arcDirectory" = directoryItem.Name) "Name of input is not 'arcDirectory'"
            testCase "Type" <| fun _ -> Expect.isTrue ((Directory (DirectoryInstance())) = directoryItem.Type) "Type of input is not Directory"
        ]
        testList "File" [
            let fileItem = decodeInput.[1]
            testCase "Name" <| fun _ -> Expect.isTrue ("firstArg" = fileItem.Name) "Name of input is not 'firstArg'"
            testCase "Type" <| fun _ -> Expect.isTrue ((File (FileInstance())) = fileItem.Type) "Type of input is not File"
            testCase "InputBinding" <| fun _ -> Expect.isTrue (Some {Position = Some 1; Prefix = Some "--example"; ItemSeparator = None; Separate = None} = fileItem.InputBinding) "InputBinding of input is not Some Pattern"
        ]
        testList "String" [
            let stringItem = decodeInput.[2]
            testCase "Name" <| fun _ -> Expect.isTrue ("secondArg" = stringItem.Name) "Name of input is not 'secondArg'"
            testCase "Type" <| fun _ -> Expect.isTrue (String = stringItem.Type) "Type of input is not String"
            testCase "InputBinding" <| fun _ -> Expect.isTrue (Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = Some false} = stringItem.InputBinding) "InputBinding of input is not Some Pattern"
        ]
    ]

let testRequirement =
    testList "requirements with DockerRequirement, InitialWorkDirRequirement, EnvVarRequirement and NetworkAccess" [
        testCase "Length" <| fun _ -> Expect.isTrue (4 = decodeRequirement.Length) "Length of requirements is not 4"
        testList "DockerRequirement" [
            let dockerItem = decodeRequirement.[0]
            testCase "Class" <| fun _ -> Expect.isTrue (DockerRequirement {DockerPull = None; DockerFile = Some "FSharpArcCapsule/Dockerfile"; DockerImageId = Some "devcontainer"} = dockerItem) "Class of requirement is not DockerRequirement"
        ]
        testList "InitialWorkDirRequirement" [
            let initialWorkDirItem = decodeRequirement.[1]
            testCase "Class" <| fun _ -> Expect.isTrue (InitialWorkDirRequirement [||] = initialWorkDirItem) "Class of requirement is not InitialWorkDirRequirement"
        ]
        testList "EnvVarRequirement" [
            let envVarItem = decodeRequirement.[2]
            testCase "Class" <| fun _ -> Expect.isTrue (EnvVarRequirement {EnvName = ""; EnvValue = ""} = envVarItem) "Class of requirement is not EnvVarRequirement"
        ]
        testList "NetworkAccess" [
            let networkAccessItem = decodeRequirement.[3]
            testCase "Class" <| fun _ -> Expect.isTrue (NetworkAccessRequirement = networkAccessItem) "Class of requirement is not NetworkAccess"
        ]
    ]

let testCWLToolDescription =
    testList "CWLToolDescription" [
        testCase "Class" <| fun _ -> Expect.isTrue (Class.CommandLineTool = decodeCWLToolDescription.Class) "Class of CWLToolDescription is not CommandLineTool"
        testCase "CWLVersion" <| fun _ -> Expect.isTrue ("v1.2" = decodeCWLToolDescription.CWLVersion) "CWLVersion of CWLToolDescription is not v1.2"
        testList "Hints" [
            let hintsItem = decodeCWLToolDescription.Hints
            testCase "DockerRequirement" <| fun _ -> Expect.isTrue (DockerRequirement {DockerPull = Some "mcr.microsoft.com/dotnet/sdk:6.0"; DockerFile = None; DockerImageId = None} = hintsItem.Value.[0]) "Class of hint is not DockerRequirement"
        ]
        testList "Requirements" [
            let requirementsItem = decodeCWLToolDescription.Requirements
            testCase "InitialWorkDirRequirement" <| fun _ -> Expect.isTrue (InitialWorkDirRequirement [||] = requirementsItem.Value.[0]) "Class of requirement is not InitialWorkDirRequirement"
            testCase "EnvVarRequirement" <| fun _ -> Expect.isTrue (EnvVarRequirement {EnvName = ""; EnvValue = ""} = requirementsItem.Value.[1]) "Class of requirement is not EnvVarRequirement"
            testCase "NetworkAccessRequirement" <| fun _ -> Expect.isTrue (NetworkAccessRequirement = requirementsItem.Value.[2]) "Class of requirement is not NetworkAccessRequirement"
        ]
        testList "Inputs" [
            let inputsItem = decodeCWLToolDescription.Inputs.Value
            testCase "Length" <| fun _ -> Expect.isTrue (2 = inputsItem.Length) "Length of inputs is not 2"
            testList "File" [
                let fileItem = inputsItem.[0]
                testCase "Name" <| fun _ -> Expect.isTrue ("firstArg" = fileItem.Name) "Name of input is not 'firstArg'"
                testCase "Type" <| fun _ -> Expect.isTrue ((File (FileInstance())) = fileItem.Type) "Type of input is not File"
                testCase "InputBinding" <| fun _ -> Expect.isTrue (Some {Position = Some 1; Prefix = None; ItemSeparator = None; Separate = None} = fileItem.InputBinding) "InputBinding of input is not Some Pattern"
            ]
            testList "String" [
                let stringItem = inputsItem.[1]
                testCase "Name" <| fun _ -> Expect.isTrue ("secondArg" = stringItem.Name) "Name of input is not 'secondArg'"
                testCase "Type" <| fun _ -> Expect.isTrue (String = stringItem.Type) "Type of input is not String"
                testCase "InputBinding" <| fun _ -> Expect.isTrue (Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = None} = stringItem.InputBinding) "InputBinding of input is not Some Pattern"
            ]
        ]
        testList "Outputs" [
            let outputsItem = decodeCWLToolDescription.Outputs
            testCase "Length" <| fun _ -> Expect.isTrue (2 = outputsItem.Length) "Length of outputs is not 2"
            testList "Directory" [
                let directoryItem = outputsItem.[0]
                testCase "Name" <| fun _ -> Expect.isTrue ("output" = directoryItem.Name) "Name of output is not 'output'"
                testCase "Type" <| fun _ -> Expect.isTrue ((Directory (DirectoryInstance())) = directoryItem.Type) "Type of output is not Directory"
                testCase "OutputBinding" <| fun _ -> Expect.isTrue (Some {Glob = Some "$(runtime.outdir)/.nuget"} = directoryItem.OutputBinding) "OutputBinding of output is not Some Pattern"
            ]
            testList "File" [
                let fileItem = outputsItem.[1]
                testCase "Name" <| fun _ -> Expect.isTrue ("output2" = fileItem.Name) "Name of output is not 'output2'"
                testCase "Type" <| fun _ -> Expect.isTrue ((File (FileInstance())) = fileItem.Type) "Type of output is not File"
                testCase "OutputBinding" <| fun _ -> Expect.isTrue (Some {Glob = Some "$(runtime.outdir)/*.csv"} = fileItem.OutputBinding) "OutputBinding of output is not Some Pattern"
            ]
        ]
    ]

let main =
    testList "CWL" [
        testOutput
        testInput
        testRequirement
        testCWLToolDescription
    ]