module Tests.CWLObject

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open ARCtrl.CWL.Inputs
open ARCtrl.CWL.Outputs
open YAMLicious
open TestingUtils

let decodeCWLToolDescription =
    TestObjects.CWL.CommandLineTool.cwl
    |> Decode.decodeCommandLineTool

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
                let expected = InitialWorkDirRequirement [|Dirent {Entry = "$include: script.fsx"; Entryname = Some "script.fsx"; Writable = None }|]
                let actual = requirementsItem.Value.[0]
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "EnvVarRequirement" <| fun _ ->
                let expected = EnvVarRequirement [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}|]
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
                    let actual = fileItem.Type.Value
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
                    let actual = stringItem.Type.Value
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
                    let actual = directoryItem.Type.Value
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
                    let actual = fileItem.Type.Value
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