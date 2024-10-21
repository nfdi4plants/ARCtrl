module Tests.CWLObject

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open ARCtrl.CWL.Inputs
open ARCtrl.CWL.Outputs
open TestingUtils
open DynamicObj

let decodeCWLToolDescription: CWLToolDescription =
    TestObjects.CWL.CommandLineTool.cwlFile
    |> Decode.decodeCommandLineTool

let decodeCWLToolDescriptionMetadata: CWLToolDescription =
    TestObjects.CWL.CommandLineToolMetadata.cwlFile
    |> Decode.decodeCommandLineTool


let testCWLToolDescription =
    testList "Decode" [
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLToolDescription.CWLVersion
            Expect.equal actual expected ""
        testCase "baseCommand" <| fun _ ->
            let expected = Some (ResizeArray [|"dotnet"; "fsi"; "script.fsx"|])
            let actual = decodeCWLToolDescription.BaseCommand
            Expect.sequenceEqual actual.Value expected.Value ""
        testList "Hints" [
            let hintsItem = decodeCWLToolDescription.Hints
            testCase "DockerRequirement" <| fun _ ->
                let expected = DockerRequirement {DockerPull = Some "mcr.microsoft.com/dotnet/sdk:6.0"; DockerFile = None; DockerImageId = None}
                let actual = hintsItem.Value.[0]
                Expect.equal actual expected ""
        ]
        testList "Requirements" [
            let requirementsItem = decodeCWLToolDescription.Requirements
            testCase "InitialWorkDirRequirement" <| fun _ ->
                let expected = InitialWorkDirRequirement (ResizeArray [|Dirent {Entry = "$include: script.fsx"; Entryname = Some "script.fsx"; Writable = None }|])
                let actual = requirementsItem.Value.[0]
                match actual, expected with
                | InitialWorkDirRequirement actualType, InitialWorkDirRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be InitialWorkDirRequirement"
            testCase "EnvVarRequirement" <| fun _ ->
                let expected = EnvVarRequirement (ResizeArray [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}|])
                let actual = requirementsItem.Value.[1]
                match actual, expected with
                | EnvVarRequirement actualType, EnvVarRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be EnvVarRequirement"
            testCase "NetworkAccessRequirement" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = requirementsItem.Value.[2]
                Expect.equal actual expected ""
        ]
        testList "Inputs" [
            let inputsItem = decodeCWLToolDescription.Inputs.Value
            testCase "Length" <| fun _ ->
                let expected = 2
                let actual = inputsItem.Count
                Expect.equal actual expected ""
            testList "File" [
                let fileItem = inputsItem.[0]
                testCase "Name" <| fun _ ->
                    let expected = "firstArg"
                    let actual = fileItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = File (FileInstance())
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "InputBinding" <| fun _ ->
                    let expected = Some {Position = Some 1; Prefix = None; ItemSeparator = None; Separate = None}
                    let actual = fileItem.InputBinding
                    Expect.equal actual expected ""
            ]
            testList "String" [
                let stringItem = inputsItem.[1]
                testCase "Name" <| fun _ ->
                    let expected = "secondArg"
                    let actual = stringItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = String
                    let actual = stringItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "InputBinding" <| fun _ ->
                    let expected = Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = None}
                    let actual = stringItem.InputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testList "Outputs" [
            let outputsItem = decodeCWLToolDescription.Outputs
            testCase "Length" <| fun _ ->
                let expected = 2
                let actual = outputsItem.Count
                Expect.equal actual expected ""
            testList "Directory" [
                let directoryItem = outputsItem.[0]
                testCase "Name" <| fun _ ->
                    let expected = "output"
                    let actual = directoryItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = Directory (DirectoryInstance())
                    let actual = directoryItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some {Glob = Some "$(runtime.outdir)/.nuget"}
                    let actual = directoryItem.OutputBinding
                    Expect.equal actual expected ""
            ]
            testList "File" [
                let fileItem = outputsItem.[1]
                testCase "Name" <| fun _ ->
                    let expected = "output2"
                    let actual = fileItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = File (FileInstance())
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some {Glob = Some "$(runtime.outdir)/*.csv"}
                    let actual = fileItem.OutputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testCase "Metadata" <| fun _ ->
            let expected = None
            let actual = decodeCWLToolDescription.Metadata
            Expect.equal actual expected ""
    ]

let testCWLToolDescriptionMetadata =
    testList "Decode with Metadata" [
        testCase "Class" <| fun _ ->
            let expected = CWLClass.CommandLineTool
            let actual = decodeCWLToolDescriptionMetadata.Class
            Expect.equal actual expected ""
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLToolDescriptionMetadata.CWLVersion
            Expect.equal actual expected ""
        testCase "baseCommand" <| fun _ ->
            let expected = Some (ResizeArray [|"dotnet"; "fsi"; "script.fsx"|])
            let actual = decodeCWLToolDescriptionMetadata.BaseCommand
            Expect.sequenceEqual actual.Value expected.Value ""
        testList "Hints" [
            let hintsItem = decodeCWLToolDescriptionMetadata.Hints
            testCase "DockerRequirement" <| fun _ ->
                let expected = DockerRequirement {DockerPull = Some "mcr.microsoft.com/dotnet/sdk:6.0"; DockerFile = None; DockerImageId = None}
                let actual = hintsItem.Value.[0]
                Expect.equal actual expected ""
        ]
        testList "Requirements" [
            let requirementsItem = decodeCWLToolDescriptionMetadata.Requirements
            testCase "InitialWorkDirRequirement" <| fun _ ->
                let expected = InitialWorkDirRequirement (ResizeArray [|Dirent {Entry = "$include: script.fsx"; Entryname = Some "script.fsx"; Writable = None }|])
                let actual = requirementsItem.Value.[0]
                match actual, expected with
                | InitialWorkDirRequirement actualType, InitialWorkDirRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be InitialWorkDirRequirement"
            testCase "EnvVarRequirement" <| fun _ ->
                let expected = EnvVarRequirement (ResizeArray [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}|])
                let actual = requirementsItem.Value.[1]
                match actual, expected with
                | EnvVarRequirement actualType, EnvVarRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be EnvVarRequirement"
            testCase "NetworkAccessRequirement" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = requirementsItem.Value.[2]
                Expect.equal actual expected ""
        ]
        testList "Inputs" [
            let inputsItem = decodeCWLToolDescriptionMetadata.Inputs.Value
            testCase "Length" <| fun _ ->
                let expected = 2
                let actual = inputsItem.Count
                Expect.equal actual expected ""
            testList "File" [
                let fileItem = inputsItem.[0]
                testCase "Name" <| fun _ ->
                    let expected = "firstArg"
                    let actual = fileItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = File (FileInstance())
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "InputBinding" <| fun _ ->
                    let expected = Some {Position = Some 1; Prefix = None; ItemSeparator = None; Separate = None}
                    let actual = fileItem.InputBinding
                    Expect.equal actual expected ""
            ]
            testList "String" [
                let stringItem = inputsItem.[1]
                testCase "Name" <| fun _ ->
                    let expected = "secondArg"
                    let actual = stringItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = String
                    let actual = stringItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "InputBinding" <| fun _ ->
                    let expected = Some {Position = Some 2; Prefix = None; ItemSeparator = None; Separate = None}
                    let actual = stringItem.InputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testList "Outputs" [
            let outputsItem = decodeCWLToolDescriptionMetadata.Outputs
            testCase "Length" <| fun _ ->
                let expected = 2
                let actual = outputsItem.Count
                Expect.equal actual expected ""
            testList "Directory" [
                let directoryItem = outputsItem.[0]
                testCase "Name" <| fun _ ->
                    let expected = "output"
                    let actual = directoryItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = Directory (DirectoryInstance())
                    let actual = directoryItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some {Glob = Some "$(runtime.outdir)/.nuget"}
                    let actual = directoryItem.OutputBinding
                    Expect.equal actual expected ""
            ]
            testList "File" [
                let fileItem = outputsItem.[1]
                testCase "Name" <| fun _ ->
                    let expected = "output2"
                    let actual = fileItem.Name
                    Expect.equal actual expected ""
                testCase "Type" <| fun _ ->
                    let expected = File (FileInstance())
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some {Glob = Some "$(runtime.outdir)/*.csv"}
                    let actual = fileItem.OutputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testCase "Metadata" <| fun _ ->
            Expect.isSome decodeCWLToolDescriptionMetadata.Metadata $"Expected {decodeCWLToolDescriptionMetadata.Metadata} to be Some"
            let expected = TestObjects.CWL.CommandLineToolMetadata.expectedMetadataString.Trim().Replace("\r\n", "\n")
            let actual = (decodeCWLToolDescriptionMetadata.Metadata.Value |> DynObj.format).Trim().Replace("\r\n", "\n")
            Expect.equal actual expected ""
    ]

let main = 
    testList "CWLToolDescription" [
        testCWLToolDescription
        testCWLToolDescriptionMetadata
    ]