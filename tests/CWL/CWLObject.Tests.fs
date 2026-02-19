module Tests.CWLObject

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL
open YAMLicious.YAMLiciousTypes

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
        testCase "sanitize removes whitespace-only lines" <| fun _ ->
            let withWhitespaceOnlyLine = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withWhitespaceOnlyLine
            let decoded = Decode.decodeCommandLineTool withWhitespaceOnlyLine
            let inputs = Expect.wantSome decoded.Inputs "Inputs should decode when whitespace-only separator lines are present."
            Expect.equal inputs.Count 1 ""
            Expect.equal inputs.[0].Name "sample" ""
        testCase "sanitize does not hide malformed yaml errors" <| fun _ ->
            let malformed = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.malformedYaml
            let decodeMalformed () = Decode.decodeCWLProcessingUnit malformed |> ignore
            Expect.throws decodeMalformed "Malformed YAML should fail decoding"
        testCase "sanitize propagates non-recoverable exception type" <| fun _ ->
            let nonRecoverableInput = "cwlVersion:\u0000 v1.2"
            let decodeInvalid () = Decode.decodeCWLProcessingUnit nonRecoverableInput |> ignore
            Expect.throws decodeInvalid "Non-recoverable parse exceptions should not be swallowed"
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLToolDescription.CWLVersion
            Expect.equal actual expected ""
        testCase "baseCommand" <| fun _ ->
            let expected = Some (ResizeArray [|"dotnet"; "fsi"; "script.fsx"|])
            let actual = decodeCWLToolDescription.BaseCommand
            Expect.sequenceEqual actual.Value expected.Value ""
        testCase "intent decodes as typed field, not metadata overflow" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFileWithIntent
            let intent = Expect.wantSome decoded.Intent "Intent should decode on CommandLineTool."
            Expect.sequenceEqual intent (ResizeArray [|"classification"; "quality-control"|]) ""
            Expect.isNone decoded.Metadata "Intent should not be captured as overflow metadata."
        testList "Hints" [
            let hintsItem = decodeCWLToolDescription.Hints
            testCase "DockerRequirement" <| fun _ ->
                let expected =
                    KnownHint (Requirement.DockerRequirement (DockerRequirement.create(dockerPull = "mcr.microsoft.com/dotnet/sdk:6.0")))
                let actual = hintsItem.Value.[0]
                Expect.equal actual expected ""
        ]
        testList "Requirements" [
            let requirementsItem = decodeCWLToolDescription.Requirements
            testCase "InitialWorkDirRequirement" <| fun _ ->
                let expected =
                    InitialWorkDirRequirement (
                        ResizeArray [| DirentEntry { Entry = Include "script.fsx"; Entryname = Some (Literal "script.fsx"); Writable = None } |]
                    )
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
                let expected = NetworkAccessRequirement { NetworkAccess = true }
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
                let expected =
                    KnownHint (Requirement.DockerRequirement (DockerRequirement.create(dockerPull = "mcr.microsoft.com/dotnet/sdk:6.0")))
                let actual = hintsItem.Value.[0]
                Expect.equal actual expected ""
        ]
        testList "Requirements" [
            let requirementsItem = decodeCWLToolDescriptionMetadata.Requirements
            testCase "InitialWorkDirRequirement" <| fun _ ->
                let expected =
                    InitialWorkDirRequirement (
                        ResizeArray [| DirentEntry { Entry = Include "script.fsx"; Entryname = Some (Literal "script.fsx"); Writable = None } |]
                    )
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
                let expected = NetworkAccessRequirement { NetworkAccess = true }
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
            testCase "intent is encoded and preserved for command line tools" <| fun _ ->
                let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFileWithIntent
                let encoded = Encode.encodeToolDescription decoded
                let roundTripped = Decode.decodeCommandLineTool encoded
                Expect.stringContains encoded "intent:" "Encoded tool should contain intent."
                let intent = Expect.wantSome roundTripped.Intent "Intent should survive roundtrip."
                Expect.sequenceEqual intent (ResizeArray [|"classification"; "quality-control"|]) ""
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
            testCase "tool with metadata preserves unknown keys in encoded output" <| fun _ ->
                let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineToolMetadata.cwlFile
                let encoded = Encode.encodeToolDescription decoded
                let roundTripped = Decode.decodeCommandLineTool encoded
                let metadata = Expect.wantSome roundTripped.Metadata "Metadata should survive roundtrip"
                let metadataText = metadata |> DynObj.format
                Expect.stringContains encoded "arc:technology platform" "Encoded tool should keep unknown metadata keys"
                Expect.stringContains metadataText "arc:technology platform" "Decoded metadata should still include unknown keys"
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

let testExpressionTool =
    testList "ExpressionTool" [
        testCase "decode top-level ExpressionTool via decodeCWLProcessingUnit" <| fun _ ->
            let result = Decode.decodeCWLProcessingUnit TestObjects.CWL.ExpressionTool.minimalExpressionToolFile
            match result with
            | ExpressionTool et ->
                Expect.equal et.Expression "$(null)" "Expression should be parsed"
                Expect.equal et.CWLVersion "v1.2" ""
            | other ->
                Expect.isTrue false $"Expected ExpressionTool but got %A{other}"
        testCase "decodeExpressionTool with requirements/inputs/outputs" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithRequirementsFile
            Expect.equal et.CWLVersion "v1.2" ""
            let requirements = Expect.wantSome et.Requirements "Requirements should be present"
            Expect.equal requirements.[0] Requirement.defaultInlineJavascriptRequirement ""
            let inputs = Expect.wantSome et.Inputs "Inputs should be present"
            Expect.isTrue (inputs.Count > 0) "Should have at least one input"
            Expect.isTrue (et.Outputs.Count > 0) "Should have at least one output"
            Expect.isTrue (et.Expression.Length > 0) "Expression should be non-empty"
        testCase "ExpressionTool intent decodes as typed field and roundtrips" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithIntentFile
            let intent = Expect.wantSome et.Intent "Intent should decode on ExpressionTool."
            Expect.sequenceEqual intent (ResizeArray [|"feature-generation"; "post-processing"|]) ""
            Expect.isNone et.Metadata "Intent should not be captured as overflow metadata."
            let encoded = Encode.encodeExpressionToolDescription et
            let roundTripped = Decode.decodeExpressionTool encoded
            let roundTrippedIntent = Expect.wantSome roundTripped.Intent "Intent should survive ExpressionTool roundtrip."
            Expect.sequenceEqual roundTrippedIntent intent ""
        testCase "ExpressionTool encode/decode deterministic" <| fun _ ->
            let original = TestObjects.CWL.ExpressionTool.expressionToolWithRequirementsFile
            let (_, d1, d2) =
                assertDeterministic
                    Encode.encodeExpressionToolDescription
                    Decode.decodeExpressionTool
                    "ExpressionTool"
                    original
            Expect.equal d1.Expression d2.Expression "Expression must survive roundtrip"
            Expect.equal d1.CWLVersion d2.CWLVersion ""
            Expect.equal d1.Outputs.Count d2.Outputs.Count ""
        testCase "ExpressionTool metadata survives roundtrip" <| fun _ ->
            let decoded = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithMetadataFile
            let encoded = Encode.encodeExpressionToolDescription decoded
            let roundTripped = Decode.decodeExpressionTool encoded
            let metadata = Expect.wantSome roundTripped.Metadata "Metadata should survive roundtrip"
            let metadataText = metadata |> DynObj.format
            Expect.stringContains metadataText "customKey" "Unknown metadata key should be preserved"
        testCase "ExpressionTool hints emitted before requirements" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithRequirementsFile
            et.Hints <- Some (ResizeArray [KnownHint StepInputExpressionRequirement])
            let encoded = Encode.encodeExpressionToolDescription et
            let hintIdx = encoded.IndexOf("hints:")
            let reqIdx = encoded.IndexOf("requirements:")
            Expect.isTrue (hintIdx > -1) "Hints should be present"
            Expect.isTrue (reqIdx > -1) "Requirements should be present"
            Expect.isTrue (hintIdx < reqIdx) "Hints should appear before requirements"
        testCase "missing expression field fails decode" <| fun _ ->
            Expect.throws
                (fun _ -> Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.missingExpressionFieldFile |> ignore)
                "ExpressionTool without expression field should fail decoding"
        testCase "invalid ExpressionTool class fails decodeCWLProcessingUnit" <| fun _ ->
            Expect.throws
                (fun _ -> Decode.decodeCWLProcessingUnit TestObjects.CWL.ExpressionTool.malformedExpressionToolClassFile |> ignore)
                "Unknown CWL class should fail decoding"
        testCase "array output ExpressionTool conformance pattern" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolArrayOutputFile
            let inputs = Expect.wantSome et.Inputs ""
            let inp = inputs.[0]
            Expect.equal inp.Type_ (Some CWLType.Int) "Input should be int"
            let outp = et.Outputs.[0]
            match outp.Type_ with
            | Some (Array arraySchema) ->
                Expect.equal arraySchema.Items CWLType.Int "Expected int[] output type"
            | other ->
                Expect.isTrue false $"Expected Array type but got %A{other}"
            Expect.stringContains et.Expression "Array.apply" "Expression should use Array.apply"
        testCase "default input ExpressionTool conformance pattern" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithDefaultInputFile
            let inputs = Expect.wantSome et.Inputs "Inputs should be present"
            let inp = inputs.[0]
            Expect.equal inp.Name "i1" ""
            Expect.equal et.Outputs.[0].Type_ (Some CWLType.Int) "Output type should be int"
        testCase "loadContents ExpressionTool input conformance pattern" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolLoadContentsFile
            let inputs = Expect.wantSome et.Inputs "Inputs should be present"
            let inp = inputs.[0]
            match inp.Type_ with
            | Some (File _) -> ()
            | other -> Expect.isTrue false $"Expected File type but got %A{other}"
            Expect.stringContains et.Expression "parseInt" "Expression should use parseInt"
    ]

let testEncodeNormalizeEdgeCases =
    testList "Encode Normalize Edge Cases" [
        testCase "empty object value roundtrips as mapping, not string" <| fun _ ->
            let encodedYaml =
                Encode.yMap [ "root", YAMLElement.Object [] ]
                |> Encode.writeYaml

            let parsedRoot =
                encodedYaml
                |> YAMLicious.Decode.read
                |> YAMLicious.Decode.object (fun get -> get.Required.Field "root" id)

            match parsedRoot with
            | YAMLElement.Object [] -> ()
            | YAMLElement.Object [YAMLElement.Value v] when v.Value = "{}" ->
                failwith "Expected empty mapping for `{}`, but got scalar \"{}\"."
            | YAMLElement.Value v when v.Value = "{}" ->
                failwith "Expected empty mapping for `{}`, but got scalar \"{}\"."
            | other ->
                failwith $"Expected empty mapping for `{{}}`, got %A{other}"

        testCase "workflow step empty in re-decodes to empty input collection" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "s1",
                    in_ = ResizeArray(),
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            let encodedYaml =
                Encode.yMap [ "steps", Encode.yMap [ Encode.encodeWorkflowStep step ] ]
                |> Encode.writeYaml

            let decodedSteps =
                encodedYaml
                |> YAMLicious.Decode.read
                |> Decode.stepsDecoder

            Expect.equal decodedSteps.[0].In.Count 0 "Empty `in` should survive encode/decode as an empty mapping."
    ]

let testOperation =
    testList "Operation" [
        testCase "decode top-level Operation via decodeCWLProcessingUnit" <| fun _ ->
            let result = Decode.decodeCWLProcessingUnit TestObjects.CWL.Operation.minimalOperationFile
            match result with
            | Operation op ->
                Expect.equal op.CWLVersion "v1.2" ""
                Expect.equal op.Inputs.Count 1 "Operation should have one input"
                Expect.equal op.Outputs.Count 1 "Operation should have one output"
            | other ->
                Expect.isTrue false $"Expected Operation but got %A{other}"
        testCase "decodeOperation with requirements/hints/metadata" <| fun _ ->
            let op = Decode.decodeOperation TestObjects.CWL.Operation.operationWithRequirementsAndMetadataFile
            Expect.equal op.CWLVersion "v1.2" ""
            let requirements = Expect.wantSome op.Requirements "Requirements should be present"
            let hints = Expect.wantSome op.Hints "Hints should be present"
            Expect.equal requirements.[0] Requirement.defaultInlineJavascriptRequirement ""
            Expect.equal hints.[0] (KnownHint StepInputExpressionRequirement) ""
            let metadata = Expect.wantSome op.Metadata "Metadata should be present"
            let metadataText = metadata |> DynObj.format
            Expect.stringContains metadataText "customKey" "Metadata should preserve unknown keys"
        testCase "Operation intent decodes as typed field and roundtrips" <| fun _ ->
            let op = Decode.decodeOperation TestObjects.CWL.Operation.operationWithIntentFile
            let intent = Expect.wantSome op.Intent "Intent should decode on Operation."
            Expect.sequenceEqual intent (ResizeArray [|"orchestration"; "wiring"|]) ""
            Expect.isNone op.Metadata "Intent should not be captured as overflow metadata."
            let encoded = Encode.encodeOperationDescription op
            let roundTripped = Decode.decodeOperation encoded
            let roundTrippedIntent = Expect.wantSome roundTripped.Intent "Intent should survive Operation roundtrip."
            Expect.sequenceEqual roundTrippedIntent intent ""
        testCase "Operation encode/decode deterministic" <| fun _ ->
            let original = TestObjects.CWL.Operation.minimalOperationFile
            let (_, d1, d2) =
                assertDeterministic
                    Encode.encodeOperationDescription
                    Decode.decodeOperation
                    "Operation"
                    original
            Expect.equal d1.CWLVersion d2.CWLVersion ""
            Expect.equal d1.Inputs.Count d2.Inputs.Count ""
            Expect.equal d1.Outputs.Count d2.Outputs.Count ""
    ]

let main = 
    testList "CWLToolDescription" [
        testCWLToolDescriptionDecode
        testCWLToolDescriptionEncode
        testEncodeNormalizeEdgeCases
        testCWLToolDescriptionMetadata
        testNestedArrayDecoding
        testExpressionTool
        testOperation
    ]
