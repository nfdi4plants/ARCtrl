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
    |>fun i ->i.Value

let decodeRequirement =
    TestUtil.requirements
    |> Decode.read
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

let decodeCWLToolDescription =
    TestUtil.cwl
    |> Decode.decodeCommandLineTool

let testOutput =
    testList "outputs with basetypes and array" [
        testCase "Length" <| fun _ ->
            let expected = 4
            let actual = decodeOutput.Length
            Expect.isTrue
                (expected = actual)
                $"Expected: {expected}\nActual: {actual}"
        testList "File" [
            let fileItem = decodeOutput.[0]
            testCase "Name" <| fun _ ->
                let expected = "output"
                let actual = fileItem.Name
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "Type" <| fun _ ->
                let expected = File (FileInstance())
                let actual = fileItem.Type
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/result.csv"}
                let actual = fileItem.OutputBinding
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "Directory" [
            let directoryItem = decodeOutput.[1]
            testCase "Name" <| fun _ ->
                let expected = "example"
                let actual = directoryItem.Name
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "Type" <| fun _ ->
                let expected = Directory (DirectoryInstance())
                let actual = directoryItem.Type
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = directoryItem.OutputBinding
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "File Array" [
            let fileArrayItem = decodeOutput.[2]
            testCase "Name" <| fun _ ->
                let expected = "exampleArray1"
                let actual = fileArrayItem.Name
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "Type" <| fun _ ->
                let expected = Array (File (FileInstance()))
                let actual = fileArrayItem.Type
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = fileArrayItem.OutputBinding
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "File Array 2" [
            let fileArrayItem = decodeOutput.[3]
            testCase "Name" <| fun _ ->
                let expected = "exampleArray2"
                let actual = fileArrayItem.Name
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "Type" <| fun _ ->
                let expected = Array (File (FileInstance()))
                let actual = fileArrayItem.Type
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "OutputBinding" <| fun _ ->
                let expected = Some {Glob = Some "./arc/runs/fsResult1/example.csv"}
                let actual = fileArrayItem.OutputBinding
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
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
            testCase "Type" <| fun _ ->
                let expected = String
                let actual = stringItem.Type
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "InputBinding" <| fun _ ->
                let expected = Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = Some false}
                let actual = stringItem.InputBinding
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
    ]

let testRequirement =
    testList "requirements with DockerRequirement, InitialWorkDirRequirement, EnvVarRequirement and NetworkAccess" [
        testCase "Length" <| fun _ -> Expect.isTrue (4 = decodeRequirement.Length) "Length of requirements is not 4"
        testList "DockerRequirement" [
            let dockerItem = decodeRequirement.[0]
            testCase "Class" <| fun _ ->
                let expected = DockerRequirement {DockerPull = None; DockerFile = Some "FSharpArcCapsule/Dockerfile"; DockerImageId = Some "devcontainer"}
                let actual = dockerItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "InitialWorkDirRequirement" [
            let initialWorkDirItem = decodeRequirement.[1]
            testCase "Class" <| fun _ ->
                let expected = InitialWorkDirRequirement [||]
                let actual = initialWorkDirItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "EnvVarRequirement" [
            let envVarItem = decodeRequirement.[2]
            testCase "Class" <| fun _ ->
                let expected = EnvVarRequirement {EnvName = ""; EnvValue = ""}
                let actual = envVarItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "NetworkAccess" [
            let networkAccessItem = decodeRequirement.[3]
            testCase "Class" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = networkAccessItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
    ]

let testCWLToolDescription =
    testList "CWLToolDescription" [
        testCase "Class" <| fun _ ->
            let expected = Class.CommandLineTool
            let actual = decodeCWLToolDescription.Class
            Expect.isTrue
                (expected = actual)
                $"Expected: {expected}\nActual: {actual}"
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLToolDescription.CWLVersion
            Expect.isTrue
                (expected = actual)
                $"Expected: {expected}\nActual: {actual}"
        testCase "baseCommand" <| fun _ ->
            let expected = Some [|"dotnet"; "fsi"; "script.fsx"|]
            let actual = decodeCWLToolDescription.BaseCommand
            Expect.isTrue
                (expected = actual)
                $"Expected: {expected}\nActual: {actual}"
        testList "Hints" [
            let hintsItem = decodeCWLToolDescription.Hints
            testCase "DockerRequirement" <| fun _ ->
                let expected = DockerRequirement {DockerPull = Some "mcr.microsoft.com/dotnet/sdk:6.0"; DockerFile = None; DockerImageId = None}
                let actual = hintsItem.Value.[0]
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "Requirements" [
            let requirementsItem = decodeCWLToolDescription.Requirements
            testCase "InitialWorkDirRequirement" <| fun _ ->
                let expected = InitialWorkDirRequirement [||]
                let actual = requirementsItem.Value.[0]
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "EnvVarRequirement" <| fun _ ->
                let expected = EnvVarRequirement {EnvName = ""; EnvValue = ""}
                let actual = requirementsItem.Value.[1]
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "NetworkAccessRequirement" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = requirementsItem.Value.[2]
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "Inputs" [
            let inputsItem = decodeCWLToolDescription.Inputs.Value
            testCase "Length" <| fun _ ->
                let expected = 2
                let actual = inputsItem.Length
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testList "File" [
                let fileItem = inputsItem.[0]
                testCase "Name" <| fun _ ->
                    let expected = "firstArg"
                    let actual = fileItem.Name
                    Expect.isTrue
                        ("firstArg" = fileItem.Name)
                        "Name of input is not 'firstArg'"
                testCase "Type" <| fun _ ->
                    let expected = File (FileInstance())
                    let actual = fileItem.Type
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "InputBinding" <| fun _ ->
                    let expected = Some {Position = Some 1; Prefix = None; ItemSeparator = None; Separate = None}
                    let actual = fileItem.InputBinding
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
            ]
            testList "String" [
                let stringItem = inputsItem.[1]
                testCase "Name" <| fun _ ->
                    let expected = "secondArg"
                    let actual = stringItem.Name
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "Type" <| fun _ ->
                    let expected = String
                    let actual = stringItem.Type
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "InputBinding" <| fun _ ->
                    let expected = Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = None}
                    let actual = stringItem.InputBinding
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
            ]
        ]
        testList "Outputs" [
            let outputsItem = decodeCWLToolDescription.Outputs
            testCase "Length" <| fun _ ->
                let expected = 2
                let actual = outputsItem.Length
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testList "Directory" [
                let directoryItem = outputsItem.[0]
                testCase "Name" <| fun _ ->
                    let expected = "output"
                    let actual = directoryItem.Name
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "Type" <| fun _ ->
                    let expected = Directory (DirectoryInstance())
                    let actual = directoryItem.Type
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some {Glob = Some "$(runtime.outdir)/.nuget"}
                    let actual = directoryItem.OutputBinding
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
            ]
            testList "File" [
                let fileItem = outputsItem.[1]
                testCase "Name" <| fun _ ->
                    let expected = "output2"
                    let actual = fileItem.Name
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "Type" <| fun _ ->
                    let expected = File (FileInstance())
                    let actual = fileItem.Type
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some {Glob = Some "$(runtime.outdir)/*.csv"}
                    let actual = fileItem.OutputBinding
                    Expect.isTrue
                        (expected = actual)
                        $"Expected: {expected}\nActual: {actual}"
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