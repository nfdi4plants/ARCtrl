module Tests.CWLObject

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL

let decodeCWLToolDescription: CWLToolDescription =
    TestObjects.CWL.CommandLineTool.cwlFile
    |> Decode.decodeCommandLineTool

let decodeCWLToolDescriptionMetadata: CWLToolDescription =
    TestObjects.CWL.CommandLineToolMetadata.cwlFile
    |> Decode.decodeCommandLineTool


let testCWLToolDescriptionDecode =
    testList "Decode" [
        testCase "sanitize allows shebang and full-line comments" <| fun _ ->
            let withShebangAndComments = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withShebangAndComments
            let decoded = Decode.decodeCommandLineTool withShebangAndComments
            Expect.equal decoded.CWLVersion "v1.2" ""
            Expect.equal decoded.Outputs.Count 0 ""
        testCase "sanitize does not hide malformed yaml errors" <| fun _ ->
            let malformed = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.malformedYaml
            let decodeMalformed () = Decode.decodeCWLProcessingUnit malformed |> ignore
            Expect.throws decodeMalformed "Malformed YAML should fail decoding"
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

let testCWLToolDescriptionEncode =
    testList "Encode" [
        testList "Tool Encode RoundTrip" [
            testCase "tool encode/decode deterministic & extended requirements" <| fun _ ->
                let original = TestObjects.CWL.CommandLineTool.cwlFile
                let (encoded1, _, _) = TestingUtils.CWL.assertDeterministic Encode.encodeToolDescription Decode.decodeCommandLineTool "CommandLineTool" original
                TestingUtils.CWL.assertRequirementsExtended encoded1
            testCase "hints emitted before requirements for both encoder variants" <| fun _ ->
                let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
                let topLevelEncoded = Encode.encodeToolDescription decoded
                let elementEncoded = decoded |> Encode.encodeToolDescriptionElement |> Encode.writeYaml

                let topReqIndex = topLevelEncoded.IndexOf("requirements:")
                let topHintIndex = topLevelEncoded.IndexOf("hints:")
                let elementReqIndex = elementEncoded.IndexOf("requirements:")
                let elementHintIndex = elementEncoded.IndexOf("hints:")

                Expect.isTrue (topReqIndex > -1) "Top-level encoding should include requirements"
                Expect.isTrue (topHintIndex > -1) "Top-level encoding should include hints"
                Expect.isTrue (elementReqIndex > -1) "Element encoding should include requirements"
                Expect.isTrue (elementHintIndex > -1) "Element encoding should include hints"
                Expect.isTrue (topHintIndex < topReqIndex) "Top-level encoding should place hints before requirements"
                Expect.isTrue (elementHintIndex < elementReqIndex) "Element encoding should place hints before requirements"
        ]
    ]

let testNestedArrayDecoding =
    testList "Nested Array Decoding" [
        testCase "Decode nested array (array of arrays)" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.NestedArray.cwlFile
            let inputs = decoded.Inputs.Value
            
            // Check IndexInput is File[]
            let indexInput = inputs |> Seq.find (fun i -> i.Name = "IndexInput")
            Expect.isTrue (
                match indexInput.Type_.Value with
                | Array arraySchema -> 
                    match arraySchema.Items with
                    | File _ -> true
                    | _ -> false
                | _ -> false
            ) "IndexInput should be File[]"
            
            // Check sampleRecordFiles is File[][] - nested Array with Array items
            let sampleRecordFiles = inputs |> Seq.find (fun i -> i.Name = "sampleRecordFiles")
            Expect.isTrue (
                match sampleRecordFiles.Type_.Value with
                | Array outerArraySchema ->
                    match outerArraySchema.Items with
                    | Array innerArraySchema ->
                        match innerArraySchema.Items with
                        | File _ -> true
                        | _ -> false
                    | _ -> false
                | _ -> false
            ) "sampleRecordFiles should be File[][]"
    ]

let main = 
    testList "CWLToolDescription" [
        testCWLToolDescriptionDecode
        testCWLToolDescriptionEncode
        testCWLToolDescriptionMetadata
        testNestedArrayDecoding
    ]
