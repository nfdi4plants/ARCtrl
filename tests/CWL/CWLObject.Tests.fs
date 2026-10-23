module Tests.CWLObject

open ARCtrl.CWL
open TestingUtils
open DynamicObj
open TestingUtils.CWL
open YAMLicious

let decodeCWLToolDescription: CWLToolDescription =
    TestObjects.CWL.CommandLineTool.cwlFile
    |> Decode.decodeCommandLineTool

let decodeCWLToolDescriptionMetadata: CWLToolDescription =
    TestObjects.CWL.CommandLineToolMetadata.cwlFile
    |> Decode.decodeCommandLineTool


let testCWLToolDescriptionDecode =
    testList "Decode" [
        testCase "decode allows shebang and full-line comments" <| fun _ ->
            let withShebangAndComments = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withShebangAndComments
            let decoded = Decode.decodeCommandLineTool withShebangAndComments
            Expect.equal decoded.CWLVersion "v1.2" ""
            Expect.equal decoded.Outputs.Count 0 ""
        testCase "decode allows whitespace-only separator lines" <| fun _ ->
            let withWhitespaceOnlyLine = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withWhitespaceOnlyLine
            let decoded = Decode.decodeCommandLineTool withWhitespaceOnlyLine
            let inputs = Expect.wantSome decoded.Inputs "Inputs should decode when whitespace-only separator lines are present."
            Expect.equal inputs.Count 1 ""
            Expect.equal inputs.[0].Name "sample" ""
        testCase "decode preserves plain multiline scalar semantics" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
doc: First paragraph line
  continues here

  Second paragraph
baseCommand: echo
inputs: {}
outputs: {}
"""
            let expectedDoc =
                yaml
                |> YAMLicious.Reader.readRoot
                |> YAMLicious.Decode.object (fun get -> get.Required.Field "doc" YAMLicious.Decode.string)
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal decoded.Doc (Some expectedDoc) "Plain multiline scalars should decode unchanged."
        testCase "decode does not hide malformed yaml errors" <| fun _ ->
            let malformed = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.malformedYaml
            let decodeMalformed () = Decode.decodeCWLProcessingUnit malformed |> ignore
            Expect.throws decodeMalformed "Malformed YAML should fail decoding"
        testCase "decode propagates non-recoverable exception type" <| fun _ ->
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
        testCase "typed id decodes and roundtrips for command line tool" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
id: tool-1
baseCommand: echo
inputs: {}
outputs: {}"""
            let decoded = Decode.decodeCommandLineTool yaml
            let encoded = Encode.encodeToolDescription decoded
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal decoded.Id (Some "tool-1") "Tool id should decode as typed field."
            Expect.equal roundTripped.Id decoded.Id "Tool id should roundtrip."
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
                        ResizeArray [| DirentEntry (DirentInstance(SchemaSaladString.Include "script.fsx", entryname = SchemaSaladString.Literal "script.fsx")) |]
                    )
                let actual = requirementsItem.Value.[0]
                match actual, expected with
                | InitialWorkDirRequirement actualType, InitialWorkDirRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be InitialWorkDirRequirement"
            testCase "EnvVarRequirement" <| fun _ ->
                let expected = EnvVarRequirement (ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true") |])
                let actual = requirementsItem.Value.[1]
                match actual, expected with
                | EnvVarRequirement actualType, EnvVarRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be EnvVarRequirement"
            testCase "NetworkAccessRequirement" <| fun _ ->
                let expected = NetworkAccessRequirement (NetworkAccessRequirementValue(true))
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
                    let expected = File
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "InputBinding" <| fun _ ->
                    let expected = Some (InputBinding.create(position = 1))
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
                    let expected = Some (InputBinding.create(position = 2))
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
                    let expected = Directory
                    let actual = directoryItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some (OutputBinding.create(glob = "$(runtime.outdir)/.nuget"))
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
                    let expected = File
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some (OutputBinding.create(glob = "$(runtime.outdir)/*.csv"))
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
                        ResizeArray [| DirentEntry (DirentInstance(SchemaSaladString.Include "script.fsx", entryname = SchemaSaladString.Literal "script.fsx")) |]
                    )
                let actual = requirementsItem.Value.[0]
                match actual, expected with
                | InitialWorkDirRequirement actualType, InitialWorkDirRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be InitialWorkDirRequirement"
            testCase "EnvVarRequirement" <| fun _ ->
                let expected = EnvVarRequirement (ResizeArray [| EnvironmentDef("DOTNET_NOLOGO", "true") |])
                let actual = requirementsItem.Value.[1]
                match actual, expected with
                | EnvVarRequirement actualType, EnvVarRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be EnvVarRequirement"
            testCase "NetworkAccessRequirement" <| fun _ ->
                let expected = NetworkAccessRequirement (NetworkAccessRequirementValue(true))
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
                    let expected = File
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "InputBinding" <| fun _ ->
                    let expected = Some (InputBinding.create(position = 1))
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
                    let expected = Some (InputBinding.create(position = 2))
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
                    let expected = Directory
                    let actual = directoryItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some (OutputBinding.create(glob = "$(runtime.outdir)/.nuget"))
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
                    let expected = File
                    let actual = fileItem.Type_.Value
                    Expect.equal actual expected ""
                testCase "OutputBinding" <| fun _ ->
                    let expected = Some (OutputBinding.create(glob = "$(runtime.outdir)/*.csv"))
                    let actual = fileItem.OutputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testCase "Metadata" <| fun _ ->
            let metadata = Expect.wantSome decodeCWLToolDescriptionMetadata.Metadata "Metadata should decode."
            let singletonObject fieldName (parent: DynamicObj) =
                let values =
                    Expect.wantSome
                        (DynamicObjHelpers.tryGetTypedPropertyValueAsResizeArray<obj> fieldName parent)
                        $"{fieldName} should remain a sequence."
                Expect.equal values.Count 1 $"{fieldName} should have one value."
                values.[0] :?> DynamicObj

            let technologyType = singletonObject "arc:has technology type" metadata
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "class" technologyType) (Some "arc:technology type") ""
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:annotation value" technologyType) (Some "Fsharp Devcontainer") ""
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:technology platform" metadata) (Some ".NET") ""

            let performer = singletonObject "arc:performer" metadata
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:first name" performer) (Some "Timo") ""
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:last name" performer) (Some "Mühlhaus") ""
            let role = singletonObject "arc:has role" performer
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:annotation value" role) (Some "Formal analysis") ""

            let processSequence = singletonObject "arc:has process sequence" metadata
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:name" processSequence) (Some "script.fsx") ""
            let processInput = singletonObject "arc:has input" processSequence
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:name" processInput)
                (Some "./arc/assays/measurement1/dataset/table.csv")
                ""
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
                let elementEncoded = decoded |> Encode.encodeToolDescriptionElement true |> Encode.writeYaml

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
            testCase "CommandLineTool command fields decode as typed members and roundtrip" <| fun _ ->
                let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.DecodeEdgeCases.commandFieldsOverflowFile

                Expect.isSome decoded.Arguments "arguments should decode to a typed field."
                Expect.equal decoded.Stdin (Some "stdin.txt") "stdin should decode to a typed field."
                Expect.equal decoded.Stdout (Some "stdout.txt") "stdout should decode to a typed field."
                Expect.equal decoded.Stderr (Some "stderr.txt") "stderr should decode to a typed field."
                Expect.sequenceEqual decoded.SuccessCodes.Value (ResizeArray [| 0 |]) "successCodes should decode to a typed field."
                Expect.sequenceEqual decoded.TemporaryFailCodes.Value (ResizeArray [| 75 |]) "temporaryFailCodes should decode to a typed field."
                Expect.sequenceEqual decoded.PermanentFailCodes.Value (ResizeArray [| 1; 2 |]) "permanentFailCodes should decode to a typed field."
                Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "arguments" decoded) "Known fields should not be direct overflow."
                let metadata = Expect.wantSome decoded.Metadata "Extension field should decode as metadata."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" metadata) (Some "keep overflow") "Extension field should stay in metadata."

                let encoded = Encode.encodeToolDescription decoded
                let roundTripped = Decode.decodeCommandLineTool encoded
                Expect.isSome roundTripped.Arguments "arguments should survive roundtrip."
                Expect.equal roundTripped.Stdout (Some "stdout.txt") "stdout should survive roundtrip."
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

let testUnionArrayItemDecoding =
    let expectedMixedInputType =
        Array (InputArraySchema(Union (ResizeArray [String; File])))

    let assertMixedInputType (decoded: CWLToolDescription) =
        let inputs = Expect.wantSome decoded.Inputs "Union-array-items fixture should decode inputs."
        let mixedInput = inputs |> Seq.find (fun input -> input.Name = "mixed_input")
        Expect.equal mixedInput.Type_.Value expectedMixedInputType "mixed_input should decode as array of [string, File]."

    testList "Union Array Items" [
        testCase "Decode array items union in flow style" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.UnionArrayItems.flowStyleCwlFile
            assertMixedInputType decoded
        testCase "Decode array items union in block style" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.UnionArrayItems.blockStyleCwlFile
            assertMixedInputType decoded
        testCase "array items union roundtrips through encoder" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.UnionArrayItems.blockStyleCwlFile
            let encoded = Encode.encodeToolDescription decoded
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.stringContains encoded "items: [string, File]" "Encoder should normalize union-valued items to flow style."
            assertMixedInputType roundTripped
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
        testCase "typed id decodes and roundtrips for ExpressionTool" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: ExpressionTool
id: expr-1
inputs: {}
outputs: {}
expression: $(null)"""
            let decoded = Decode.decodeExpressionTool yaml
            let encoded = Encode.encodeExpressionToolDescription decoded
            let roundTripped = Decode.decodeExpressionTool encoded
            Expect.equal decoded.Id (Some "expr-1") "ExpressionTool id should decode as typed field."
            Expect.equal roundTripped.Id decoded.Id "ExpressionTool id should roundtrip."
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
            // Asserted via `if`/bool rather than a `match` with a unit success branch: Fable's
            // Python backend drops empty branches from a match in statement position, which would
            // leave `Expect.isTrue false` unconditional and make this assertion always fail.
            let isFile = match inp.Type_ with Some (File _) -> true | _ -> false
            Expect.isTrue isFile $"Expected File type but got %A{inp.Type_}"
            Expect.stringContains et.Expression "parseInt" "Expression should use parseInt"
        testCase "pool output ExpressionTool expression roundtrips" <| fun _ ->
            let decoded = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolPoolOutRoundtripFile
            let encoded = Encode.encodeExpressionToolDescription decoded
            let roundTripped = Decode.decodeExpressionTool encoded
            let output = roundTripped.Outputs |> Seq.find (fun o -> o.Name = "pool_DIR")
            Expect.equal output.Type_ (Some (Directory)) "pool_DIR output type should remain Directory."
            Expect.equal roundTripped.Expression decoded.Expression "Expression payload should roundtrip without semantic changes."
            Expect.stringContains roundTripped.Expression "sanitize(entry)" "Sanitize helper should survive roundtrip."
            Expect.stringContains roundTripped.Expression "allowedFields" "allowedFields array should survive roundtrip."
            Expect.stringContains roundTripped.Expression "var outputList = []" "outputList initialization should survive roundtrip."
            Expect.stringContains roundTripped.Expression "inputs.directory_single" "directory_single input reference should survive roundtrip."
            Expect.stringContains roundTripped.Expression "inputs.file_single" "file_single input reference should survive roundtrip."
            Expect.stringContains roundTripped.Expression "inputs.newname" "newname input reference should survive roundtrip."
            Expect.stringContains roundTripped.Expression "\"Directory\"" "Directory class literal should survive roundtrip."
            Expect.stringContains roundTripped.Expression "listing: outputList" "listing assignment should survive roundtrip."
    ]

let testEncodeNormalizeEdgeCases =
    testList "Encode Normalize Edge Cases" [
        testCase "empty object value roundtrips as mapping, not string" <| fun _ ->
            let encodedYaml =
                YAMLicious.Encode.object [ "root", YAMLicious.Encode.object [] ]
                |> Encode.writeYaml

            let parsedRoot =
                encodedYaml
                |> YAMLicious.Reader.readRoot
                |> YAMLicious.Decode.object (fun get -> get.Required.Field "root" YAMLicious.Decode.node)

            match parsedRoot.Kind with
            | YamlNodeKind.Mapping collection when collection.Items.IsEmpty -> ()
            | YamlNodeKind.Scalar scalar when scalar.Value = "{}" ->
                failwith "Expected empty mapping for `{}`, but got scalar \"{}\"."
            | _ ->
                failwith $"Expected empty mapping for `{{}}`, got {YamlNode.show parsedRoot}"

        testCase "workflow step empty in re-decodes to empty input collection" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "s1",
                    in_ = ResizeArray(),
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            let encodedYaml =
                YAMLicious.Encode.object [ "steps", YAMLicious.Encode.object [ Encode.encodeWorkflowStep step ] ]
                |> Encode.writeYaml

            let decodedSteps =
                encodedYaml
                |> YAMLicious.Reader.readRoot
                |> Decode.stepsDecoder

            Expect.equal decodedSteps.[0].In.Count 0 "Empty `in` should survive encode/decode as an empty mapping."

        testCase "aliases are rejected with a clear error" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withAliases
            let decode () = Decode.decodeCommandLineTool yaml |> ignore
            match (try decode (); None with YamlDecodeException error -> Some error) with
            | Some error -> Expect.stringContains (DecodeError.text error) "alias *a" "The error should name the alias."
            | None -> failwith "A CWL document with an alias must fail."
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
        testCase "typed id decodes and roundtrips for Operation" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Operation
id: op-1
inputs:
  i: string
outputs:
  o: string"""
            let decoded = Decode.decodeOperation yaml
            let encoded = Encode.encodeOperationDescription decoded
            let roundTripped = Decode.decodeOperation encoded
            Expect.equal decoded.Id (Some "op-1") "Operation id should decode as typed field."
            Expect.equal roundTripped.Id decoded.Id "Operation id should roundtrip."
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

let testTopLevelDirectDynamicOverflow =
    testList "Top-level direct DynamicObj overflow" [
        testCase "CommandLineTool direct overflow encodes and decodes as metadata" <| fun _ ->
            let tool = CWLToolDescription(outputs = ResizeArray())
            DynObj.setProperty "arc:direct note" "tool note" tool
            let encoded = Encode.encodeToolDescription tool
            Expect.stringContains encoded "arc:direct note: tool note" "Direct DynamicObj overflow should be encoded."
            let roundTripped = Decode.decodeCommandLineTool encoded
            let metadata = Expect.wantSome roundTripped.Metadata "Decoded unknown top-level fields should remain available as metadata."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "tool note") "Encoded direct overflow should decode back as top-level metadata."

        testCase "ExpressionTool direct overflow encodes and decodes as metadata" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = "$(null)")
            DynObj.setProperty "arc:direct note" "expression note" expressionTool
            let encoded = Encode.encodeExpressionToolDescription expressionTool
            Expect.stringContains encoded "arc:direct note: expression note" "Direct DynamicObj overflow should be encoded."
            let roundTripped = Decode.decodeExpressionTool encoded
            let metadata = Expect.wantSome roundTripped.Metadata "Decoded unknown top-level fields should remain available as metadata."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "expression note") "Encoded direct overflow should decode back as top-level metadata."

        testCase "Operation direct overflow encodes and decodes as metadata" <| fun _ ->
            let operation =
                CWLOperationDescription(
                    inputs = ResizeArray [| CWLInput("input", CWLType.String) |],
                    outputs = ResizeArray [| CWLOutput("output", CWLType.String) |]
                )
            DynObj.setProperty "arc:direct note" "operation note" operation
            let encoded = Encode.encodeOperationDescription operation
            Expect.stringContains encoded "arc:direct note: operation note" "Direct DynamicObj overflow should be encoded."
            let roundTripped = Decode.decodeOperation encoded
            let metadata = Expect.wantSome roundTripped.Metadata "Decoded unknown top-level fields should remain available as metadata."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "operation note") "Encoded direct overflow should decode back as top-level metadata."
    ]

let testStringScalars =
    testList "String Scalars" [
        testCase "YAMLicious.Encode.string double-quotes text that a YAML reader would take for another type" <| fun _ ->
            let encoded =
                YAMLicious.Encode.object [
                    "bool", YAMLicious.Encode.string "true"
                    "number", YAMLicious.Encode.string "1.0"
                    "nothing", YAMLicious.Encode.string "null"
                    "empty", YAMLicious.Encode.string ""
                    "text", YAMLicious.Encode.string "plain text"
                ]
                |> Encode.writeYaml
            Expect.stringContains encoded "bool: \"true\"" "The string true needs quotes."
            Expect.stringContains encoded "number: \"1.0\"" "The string 1.0 needs quotes."
            Expect.stringContains encoded "nothing: \"null\"" "The string null needs quotes."
            Expect.stringContains encoded "empty: \"\"" "The empty string needs quotes."
            Expect.stringContains encoded "text: plain text" "Ordinary text stays plain."
            Expect.isFalse (encoded.Contains("\\\"")) "The quotes come from the scalar style, so nothing may be escaped."

        testCase "CWL string fields keep lookalike strings quoted through a roundtrip" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
label: "1.0"
doc: "null"
baseCommand: echo
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "label: \"1.0\"" "A label that reads as a float needs quotes."
            Expect.stringContains encoded "doc: \"null\"" "A doc that reads as null needs quotes."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal roundTripped.Label (Some "1.0") "The label should stay a string."
            Expect.equal roundTripped.Doc (Some "null") "The doc should stay a string."

        testCase "dynamic string values keep their type through a roundtrip" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.StringScalars.withDynamicStringValues
            let decoded = Decode.decodeCommandLineTool yaml
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "arc:quoted: \"true\"" "A quoted true is a string and must stay quoted."
            Expect.stringContains encoded "arc:plain: true" "A plain true is a boolean and stays plain."
            Expect.stringContains encoded "arc:number string: \"12\"" "A quoted number is a string and must stay quoted."
            Expect.stringContains encoded "arc:number: 12" "A plain number stays plain."
            Expect.stringContains encoded "arc:null: null" "A plain null is not a string and stays plain."
            Expect.stringContains encoded "arc:tilde: ~" "A plain tilde is a null and stays as written."
            Expect.stringContains encoded "arc:capital bool: true" "A plain True is a boolean and is written as one."
            Expect.isFalse (encoded.Contains("arc:hex: \"0x1F\"")) "A plain hexadecimal number is not a string, so it must not be quoted."
            Expect.stringContains encoded "arc:word: NaN" "YAML 1.2 reads NaN as a string, which needs no quotes."
            Expect.stringContains encoded "arc:whole float: 2.0" "A whole number written as a float must not turn into an int."
            let metadata = Expect.wantSome (Decode.decodeCommandLineTool encoded).Metadata "Dynamic fields should survive."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:quoted" metadata) (Some "true") "The string true should stay a string."
            Expect.equal (DynObj.tryGetTypedPropertyValue<bool> "arc:plain" metadata) (Some true) "The boolean should stay a boolean."
            Expect.equal (DynObj.tryGetTypedPropertyValue<bool> "arc:capital bool" metadata) (Some true) "True is a boolean of the core schema."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:word" metadata) (Some "NaN") "NaN is a string in YAML 1.2."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "arc:null" metadata) "A plain null is not a string."

        testCase "flow text for additionalType quotes enum symbols and record field names" <| fun _ ->
            let enumType = Enum (InputEnumSchema(ResizeArray [ "true"; "a"; "x,y"; "12" ]))
            let enumText = Encode.cwlTypeToYamlString enumType
            Expect.equal enumText "{type: enum, symbols: [\"true\", a, \"x,y\", \"12\"]}" "Lookalikes and flow indicators need quotes inside the flow text."
            Expect.isTrue (CWLType.typesEqual enumType (enumText |> YAMLicious.Reader.readRoot |> Decode.cwlTypeDecoder')) "The flow text should decode to the same enum."
            let recordType =
                Record (InputRecordSchema(fields = ResizeArray [ InputRecordField("null", CWLType.String); InputRecordField("ok", CWLType.File) ]))
            let recordText = Encode.cwlTypeToYamlString recordType
            Expect.stringContains recordText "{name: \"null\", type: string}" "A field named null needs quotes."
            Expect.stringContains recordText "{name: ok, type: File}" "An ordinary field name stays plain."
            Expect.isTrue (CWLType.typesEqual recordType (recordText |> YAMLicious.Reader.readRoot |> Decode.cwlTypeDecoder')) "The flow text should decode to the same record."

        testCase "mapping keys that look like another type are quoted" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.StringScalars.withLookalikeKeys
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            Expect.stringContains encoded "\"1\": string" "A key that reads as a number needs quotes."
            Expect.stringContains encoded "\"true\": int" "A key that reads as a bool needs quotes."
            Expect.stringContains encoded "\"null\": File" "A key that reads as null needs quotes."
            Expect.stringContains encoded "plain: File" "An ordinary key stays plain."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.sequenceEqual (roundTripped.Inputs.Value |> Seq.map (fun i -> i.Name)) [ "1"; "true"; "plain" ] "The input names should survive."

        testCase "the null type is written as a quoted string" <| fun _ ->
            Expect.equal (Encode.cwlTypeToYamlString Null) "\"null\"" "A plain null would read as a YAML null."
            Expect.equal (Encode.cwlTypeToYamlString (Union (ResizeArray [ Null; CWLType.File ]))) "[\"null\", File]" "The null member of a union must be quoted."
    ]

let testToolUnionPorts =
    let tool = TestObjects.CWL.CommandLineTool.UnionPorts.cwlFile

    let assertTypesEqual (expected: CWLType) (actual: CWLType) (message: string) =
        Expect.isTrue (CWLType.typesEqual expected actual) $"{message}: expected %A{expected} but got %A{actual}"

    let check (decoded: CWLToolDescription) =
        let inputs = Expect.wantSome decoded.Inputs "Inputs should decode."
        let typeOf name = (Expect.wantSome (inputs |> Seq.find (fun i -> i.Name = name)).Type_ $"{name} should have a type")
        let optionalOf inner = Union (ResizeArray [ Null; inner ])
        assertTypesEqual (optionalOf (Enum (InputEnumSchema(ResizeArray [ "alpha"; "beta" ])))) (typeOf "mode_choice") "mode_choice"
        assertTypesEqual (optionalOf (CWLType.Directory)) (typeOf "optional_dir") "optional_dir"
        assertTypesEqual (optionalOf (CWLType.File)) (typeOf "flow_union") "flow_union"
        assertTypesEqual (Union (ResizeArray [ String; Int ])) (typeOf "block_union") "block_union"
        assertTypesEqual (optionalOf (Array (InputArraySchema(Union (ResizeArray [ String; CWLType.File ]))))) (typeOf "array_or_null") "array_or_null"
        let flagOf name = (inputs |> Seq.find (fun i -> i.Name = name)).Optional
        for name in [ "mode_choice"; "optional_dir"; "flow_union"; "array_or_null" ] do
            Expect.equal (flagOf name) (Some true) (sprintf "%s is a union of null and one type, so it is optional." name)
        Expect.equal (flagOf "block_union") None "A union without null is not optional."
        let outputTypeOf name = Expect.wantSome (decoded.Outputs |> Seq.find (fun o -> o.Name = name)).Type_ $"{name} should have a type"
        assertTypesEqual (Union (ResizeArray [ CWLType.File; CWLType.Directory ])) (outputTypeOf "result") "result"
        assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf "maybe") "maybe"

    testList "Union port types" [
        testCase "unions in object form and in bare list form decode" <| fun _ ->
            check (Decode.decodeCommandLineTool tool)
        testCase "unions roundtrip through the encoder" <| fun _ ->
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool tool)
            check (Decode.decodeCommandLineTool encoded)
        testCase "a port with a null and a file in a type list is the same port after a round trip" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x: ["null", File]
  y:
    type: ["null", File]
    label: y
outputs: []
"""
            let decoded = Decode.decodeCommandLineTool yaml
            for name in [ "x"; "y" ] do
                Expect.equal (decoded.Inputs.Value |> Seq.find (fun i -> i.Name = name)).Optional (Some true) (sprintf "%s should be optional." name)
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded "A round trip should keep the optional flag."
        testCase "unions in the encoder output use only quoted null members" <| fun _ ->
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool tool)
            Expect.isFalse (encoded.Contains("[null") || encoded.Contains("- null") || encoded.Contains(", null")) "A plain null member reads as a YAML null, which CWL rejects."
        testCase "a type list with the null behind the type keeps its order" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x: [string, "null"]
outputs:
  y: [File, "null"]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal decoded.Inputs.Value.[0].Type_ (Some (Union (ResizeArray [ String; Null ]))) "The input type keeps the order of the list."
            Expect.equal decoded.Outputs.[0].Type_ (Some (Union (ResizeArray [ CWLType.File; Null ]))) "The output type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded """x: [string, "null"]""" "The input is written as a list."
            Expect.stringContains encoded """y: [File, "null"]""" "The output is written as a list."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal roundTripped.Inputs.Value.[0].Type_ decoded.Inputs.Value.[0].Type_ "The input type survives the round trip."
            Expect.equal roundTripped.Outputs.[0].Type_ decoded.Outputs.[0].Type_ "The output type survives the round trip."
        testCase "a type list with the null in front of the type is written as the shorthand" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x: ["null", string]
outputs:
  y: ["null", File]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal decoded.Inputs.Value.[0].Type_ (Some (Union (ResizeArray [ Null; String ]))) "The input type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "x: string?" "The input is written as the shorthand."
            Expect.stringContains encoded "y: File?" "The output is written as the shorthand."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal roundTripped.Inputs.Value.[0].Type_ decoded.Inputs.Value.[0].Type_ "The input type survives the round trip."
            Expect.equal roundTripped.Outputs.[0].Type_ decoded.Outputs.[0].Type_ "The output type survives the round trip."
    ]

let testAnyType =
    let expectedInputTypes =
        [ "anything", Any
          "maybe_anything", Union (ResizeArray [ Null; Any ])
          "anything_list", Array (InputArraySchema(Any))
          "anything_or_text", Union (ResizeArray [ Any; String ])
          "listed", Array (InputArraySchema(Any)) ]
    let expectedOutputTypes =
        [ "anything_out", Any
          "maybe_out", Union (ResizeArray [ Null; Any ])
          "list_out", Array (InputArraySchema(Any)) ]
    let expectTypes (operation: CWLOperationDescription) =
        for (name, expected) in expectedInputTypes do
            let input = operation.Inputs |> Seq.find (fun i -> i.Name = name)
            Expect.equal input.Type_ (Some expected) $"The input {name} should have the type {expected}."
        for (name, expected) in expectedOutputTypes do
            let output = operation.Outputs |> Seq.find (fun o -> o.Name = name)
            Expect.equal output.Type_ (Some expected) $"The output {name} should have the type {expected}."

    testList "Any type" [
        testCase "Any decodes as a type name, in an optional, in an array, in a union and under items" <| fun _ ->
            expectTypes (Decode.decodeOperation TestObjects.CWL.Operation.AnyType.cwlFile)
        testCase "Any is written as the shorthand and reads back" <| fun _ ->
            let encoded = Encode.encodeOperationDescription (Decode.decodeOperation TestObjects.CWL.Operation.AnyType.cwlFile)
            Expect.stringContains encoded "maybe_anything: Any?" "An optional Any is written as the shorthand."
            Expect.stringContains encoded "anything_list: Any[]" "An array of Any is written as the shorthand."
            Expect.stringContains encoded "anything_or_text: [Any, string]" "A union with Any is a list."
            Expect.stringContains encoded "listed: Any[]" "An array schema with Any items is written as the shorthand."
            expectTypes (Decode.decodeOperation encoded)
        testCase "Any equals Any and differs from the other types" <| fun _ ->
            Expect.equal Any Any "Any is equal to itself."
            Expect.equal (hash Any) (hash Any) "Any hashes to the same value each time."
            Expect.notEqual Any String "Any is no string."
            Expect.notEqual (Union (ResizeArray [ Null; Any ])) (Union (ResizeArray [ Null; String ])) "The members of a union count."
        testCase "Any decodes under the items of an array schema that stands under items" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Operation
inputs:
  nested:
    type:
      type: array
      items:
        type: array
        items: Any
outputs: {}
"""
            let operation = Decode.decodeOperation yaml
            Expect.equal operation.Inputs.[0].Type_ (Some (Array (InputArraySchema(Array (InputArraySchema(Any)))))) "An array schema may hold an array schema of Any."
    ]

let testStreamTypes =
    let toolHead = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: cat
"""
    let tool (ports: string) = toolHead + ports
    let decodeError (decode: unit -> unit) =
        try
            decode ()
            None
        with YamlDecodeException error -> Some error
    let portType (ports: ResizeArray<'port>) (nameOf: 'port -> string) (typeOf: 'port -> CWLType option) (name: string) =
        Expect.wantSome (typeOf (ports |> Seq.find (fun port -> nameOf port = name))) $"{name} should have a type."

    testList "Stream types" [
        testCase "stdout, stderr and stdin decode as the whole type of a port of a command line tool" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            Expect.equal (portType decoded.Inputs.Value (fun i -> i.Name) (fun i -> i.Type_) "unsorted") Stdin "The input has the type stdin."
            Expect.equal (portType decoded.Outputs (fun o -> o.Name) (fun o -> o.Type_) "sorted") Stdout "The output has the type stdout."
            Expect.equal (portType decoded.Outputs (fun o -> o.Name) (fun o -> o.Type_) "log") Stderr "The output in object form has the type stderr."
            Expect.isNone decoded.Inputs.Value.[0].Optional "A stream type is no optional type."
        testCase "a stream type in object form decodes as the whole type" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool """inputs:
  lines:
    type: stdin
    label: Lines
outputs:
  out:
    type: stdout""")
            Expect.equal decoded.Inputs.Value.[0].Type_ (Some Stdin) "The input has the type stdin."
            Expect.equal decoded.Outputs.[0].Type_ (Some Stdout) "The output has the type stdout."
        testCase "stream types are written as type names and read back" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "unsorted: stdin" "The input is written as the type name."
            Expect.stringContains encoded "sorted: stdout" "The output is written as the type name."
            Expect.stringContains encoded "type: stderr" "The output in object form keeps its object."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (portType roundTripped.Inputs.Value (fun i -> i.Name) (fun i -> i.Type_) "unsorted") Stdin "The input survives the round trip."
            Expect.equal (portType roundTripped.Outputs (fun o -> o.Name) (fun o -> o.Type_) "sorted") Stdout "The stdout output survives the round trip."
            Expect.equal (portType roundTripped.Outputs (fun o -> o.Name) (fun o -> o.Type_) "log") Stderr "The stderr output survives the round trip."
        testCase "the inline command line tool of a workflow admits stream types" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.CommandLineTool.StreamTypes.workflowWithInlineTool
            let inlineTool = Expect.wantSome (WorkflowStepRunOps.tryGetTool decoded.Steps.[0].Run) "The step runs an inline tool."
            Expect.equal inlineTool.Outputs.[0].Type_ (Some Stdout) "The output has the type stdout."
        testCase "a stream type anywhere but as the whole type of a port is a decode error at its place" <| fun _ ->
            let cases =
                [ "an optional stdout", "outputs.x", tool """inputs: {}
outputs:
  x: stdout?"""
                  "an array of stderr", "outputs.x", tool """inputs: {}
outputs:
  x: stderr[]"""
                  "stdout in a type list", "outputs.x[0]", tool """inputs: {}
outputs:
  x: [stdout, "null"]"""
                  "an optional stdout in a type field", "outputs.x.type", tool """inputs: {}
outputs:
  x:
    type: stdout?"""
                  "stdout under items", "outputs.x.type.items", tool """inputs: {}
outputs:
  x:
    type:
      type: array
      items: stdout"""
                  "stdout as the type of a record field", "outputs.x.type.fields.f", tool """inputs: {}
outputs:
  x:
    type:
      type: record
      fields:
        f: stdout"""
                  "stdin as the type of an output", "outputs.x", tool """inputs: {}
outputs:
  x: stdin"""
                  "stdout as the type of an input", "inputs.x", tool """inputs:
  x: stdout
outputs: {}"""
                  "an optional stdin", "inputs.x", tool """inputs:
  x: stdin?
outputs: {}"""
                  "an array of stdin", "inputs.x", tool """inputs:
  x: stdin[]
outputs: {}"""
                  "stdin in a type list", "inputs.x[0]", tool """inputs:
  x: [stdin, string]
outputs: {}""" ]
            for (name, path, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the place of the type."
                    Expect.stringContains error.Message "is valid only as the whole type of" $"{name}: the error should say where the type is valid."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "a stream type with a binding is a decode error at the port" <| fun _ ->
            let cases =
                [ "stdout with an outputBinding", "outputs.x", "The type stdout is valid only for an output without an outputBinding.", tool """inputs: {}
outputs:
  x:
    type: stdout
    outputBinding:
      glob: out.txt"""
                  "stderr with an outputBinding", "outputs.x", "The type stderr is valid only for an output without an outputBinding.", tool """inputs: {}
outputs:
  x:
    type: stderr
    outputBinding:
      glob: err.txt"""
                  "stdin with an inputBinding", "inputs.x", "The type stdin is valid only for an input without an inputBinding.", tool """inputs:
  x:
    type: stdin
    inputBinding:
      position: 1
outputs: {}""" ]
            for (name, path, message, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the port."
                    Expect.equal error.Message message $"{name}: the error should say what is wrong."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "a tool that sets stdin and has an input of the type stdin is a decode error at stdin" <| fun _ ->
            let yaml = tool """stdin: input.txt
inputs:
  x: stdin
outputs: {}"""
            match decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "stdin" "The error shows the field of the tool."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "Both ways to read the standard input must fail."
            let withoutInput = tool """stdin: input.txt
inputs: {}
outputs: {}"""
            Expect.equal (Decode.decodeCommandLineTool withoutInput).Stdin (Some "input.txt") "The tool field alone is valid."
        testCase "a workflow, an expression tool and an operation admit no stream type" <| fun _ ->
            let cases =
                [ "a workflow output", "outputs.x", """cwlVersion: v1.2
class: Workflow
inputs: {}
steps: {}
outputs:
  x: stdout"""
                  "a workflow input", "inputs.x", """cwlVersion: v1.2
class: Workflow
inputs:
  x: stdin
steps: {}
outputs: {}"""
                  "an expression tool output", "outputs.x", """cwlVersion: v1.2
class: ExpressionTool
inputs: {}
outputs:
  x: stdout
expression: '${ return {}; }'"""
                  "an operation output", "outputs.x", """cwlVersion: v1.2
class: Operation
inputs: {}
outputs:
  x: stderr""" ]
            for (name, path, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCWLProcessingUnit yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the port."
                    Expect.stringContains error.Message "is valid only as the whole type of" $"{name}: the error should say where the type is valid."
                | None -> failwith $"{name} must fail"
    ]

let testYamlReaderAndWriter =
    let minimalTool = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
"""
    testList "YAMLicious Reader and Writer" [
        testCase "multi-line plain scalars fold into one line in schema-salad strings" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.ReaderAndWriter.withMultiLinePlainScalar
            let decoded = Decode.decodeCommandLineTool yaml
            let listing =
                decoded.Requirements
                |> Option.get
                |> Seq.pick (function InitialWorkDirRequirement entries -> Some entries | _ -> None)
            match Seq.exactlyOne listing with
            | DirentEntry dirent ->
                Expect.equal dirent.Entry (SchemaSaladString.Literal "first line second line") "A plain scalar over two lines is one line of text."
            | other -> failwithf "Expected a Dirent entry but got %A" other

        testCase "block scalars keep their trailing line breaks" <| fun _ ->
            let decodeLabel (header: string) (body: string) =
                (Decode.decodeCommandLineTool ("label: " + header + "\n" + body + minimalTool)).Label
            Expect.equal (decodeLabel "|" "  a\n") (Some "a\n") "Clip keeps one line break."
            Expect.equal (decodeLabel "|-" "  a\n") (Some "a") "Strip keeps none."
            Expect.equal (decodeLabel "|+" "  a\n\n") (Some "a\n\n") "Keep keeps all of them."

        testCase "strings with trailing line breaks roundtrip" <| fun _ ->
            let texts = [ "a\n"; "a"; "a\n\n"; "a\n\n\n"; "line one\nline two\n"; "line one\nline two"; "line one\n\nline three\n"; "line one\n\n"; "$(inputs.x)\n" ]
            for text in texts do
                let tool = Decode.decodeCommandLineTool minimalTool
                tool.Label <- Some text
                let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription tool)
                Expect.equal roundTripped.Label (Some text) (sprintf "The label %A should keep its line breaks." text)

        testCase "a keep-chomped block at the end of a section is not extended by the section separator" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.ReaderAndWriter.withKeptBlockBeforeSection
            let valueFrom (tool: CWLToolDescription) =
                tool.Inputs |> Option.bind Seq.tryHead |> Option.bind (fun i -> i.InputBinding) |> Option.bind (fun b -> b.ValueFrom)
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (valueFrom decoded) (Some "$(self)\n\n") "The keep header should keep both line breaks."
            let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.equal (valueFrom roundTripped) (Some "$(self)\n\n") "The blank line between sections must not become part of the scalar."

        testCase "a keep-chomped block at the end of the document ends with a line break" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
arguments:
  - valueFrom: |+
      $(self)

"""
            let rec scalarsOf (node: YamlNode) =
                match node.Kind with
                | YamlNodeKind.Scalar scalar -> [ scalar.Value ]
                | YamlNodeKind.Mapping collection -> collection.Items |> List.collect (fun entry -> scalarsOf entry.Value)
                | YamlNodeKind.Sequence collection -> collection.Items |> List.collect (fun item -> scalarsOf item.Node)
                | YamlNodeKind.Alias _ -> []
            let argumentScalars (tool: CWLToolDescription) = scalarsOf (Expect.wantSome tool.Arguments "Arguments should decode.").Node
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.isTrue (List.contains "$(self)\n\n" (argumentScalars decoded)) "The keep header should keep both line breaks."
            let encoded = Encode.encodeToolDescription decoded
            Expect.isTrue (encoded.EndsWith "\n") "The last blank line of the scalar needs a line break behind it, or other YAML readers drop it."
            Expect.isTrue (List.contains "$(self)\n\n" (argumentScalars (Decode.decodeCommandLineTool encoded))) "The scalar should read back unchanged."

        testCase "empty values in extension fields are written back as they were" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.ReaderAndWriter.withEmptyExtensionValues
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            let lines = encoded.Replace("\r", "").Split('\n') |> Array.map (fun line -> line.TrimEnd())
            Expect.isTrue (Array.contains "arc:note:" lines) "A key without a value stays without a value."
            let listLines = lines |> Array.skipWhile (fun line -> line <> "arc:list:") |> Array.skip 1 |> Array.take 3
            Expect.sequenceEqual listLines [ "  - a"; "  -"; "  - b" ] "An empty item is written as a dash and not as an empty mapping."
            Expect.isTrue (Array.contains "arc:flow: {}" lines) "An empty flow mapping stays one."

        testCase "a package in the map form without a value has no specs" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SoftwareRequirement
    packages:
      samtools:
      bwa: {}
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            let packages =
                decoded.Requirements
                |> Option.get
                |> Seq.pick (function SoftwareRequirement packages -> Some packages | _ -> None)
            Expect.sequenceEqual (packages |> Seq.map (fun p -> p.Package)) [ "samtools"; "bwa" ] "Both packages should decode."
            Expect.isTrue (packages |> Seq.forall (fun p -> p.Specs.IsNone)) "Neither package has specs."

        testCase "quoted booleans and integers in typed fields fail like schema validation" <| fun _ ->
            let tool (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
            let inputBinding field = tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      " + field; "outputs: {}" ]
            let inputPort field = tool [ "inputs:"; "  x:"; "    type: string"; "    " + field; "outputs: {}" ]
            let outputPort field = tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    " + field ]
            let outputBinding field = tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    outputBinding:"; "      glob: a"; "      " + field ]
            let dirent field = tool [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - entryname: a"; "        entry: b"; "        " + field; "inputs: {}"; "outputs: {}" ]
            let inplaceUpdate field = tool [ "requirements:"; "  - class: InplaceUpdateRequirement"; "    " + field; "inputs: {}"; "outputs: {}" ]
            let exitCodes field = tool [ field; "inputs: {}"; "outputs: {}" ]
            let workflowStep field =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs:"; "  a:"; "    type: File"; "outputs: {}"; "steps:"; "  s:"; "    run: other.cwl"; "    in:"; "      x:"; "        source: a"; "        " + field; "    out: []" ] + "\n"
            let decodeTool (yaml: string) = Decode.decodeCommandLineTool yaml |> ignore
            let decodeWorkflow (yaml: string) = Decode.decodeWorkflow yaml |> ignore
            let decodeParameters (yaml: string) = DecodeParameters.decodeYAMLParameterFile yaml |> ignore
            let fileWithSize = """f:
  class: File
  path: a.txt
  size: 42
"""
            let fileWithQuotedSize = """f:
  class: File
  path: a.txt
  size: "42"
"""
            let cases =
                [ "inputBinding position", decodeTool, inputBinding "position: 1", inputBinding "position: \"1\""
                  "inputBinding separate", decodeTool, inputBinding "separate: false", inputBinding "separate: \"false\""
                  "inputBinding loadContents", decodeTool, inputBinding "loadContents: true", inputBinding "loadContents: \"true\""
                  "inputBinding shellQuote", decodeTool, inputBinding "shellQuote: false", inputBinding "shellQuote: \"false\""
                  "input streamable", decodeTool, inputPort "streamable: true", inputPort "streamable: \"true\""
                  "output streamable", decodeTool, outputPort "streamable: true", outputPort "streamable: \"true\""
                  "outputBinding loadContents", decodeTool, outputBinding "loadContents: true", outputBinding "loadContents: \"true\""
                  "Dirent writable", decodeTool, dirent "writable: true", dirent "writable: \"true\""
                  "inplaceUpdate", decodeTool, inplaceUpdate "inplaceUpdate: true", inplaceUpdate "inplaceUpdate: \"true\""
                  "successCodes", decodeTool, exitCodes "successCodes: [0, 1]", exitCodes "successCodes: [\"0\", \"1\"]"
                  "step input loadContents", decodeWorkflow, workflowStep "loadContents: true", workflowStep "loadContents: \"true\""
                  "File size", decodeParameters, fileWithSize, fileWithQuotedSize ]
            for (name, decode, plain, quoted) in cases do
                decode plain
                Expect.throws (fun () -> decode quoted) (sprintf "A quoted value for %s is a string and must fail." name)
            // The core schema also reads these plain spellings as a bool and an int.
            decodeTool (inputBinding "shellQuote: False")
            decodeTool (inputBinding "position: 0x2")

        testCase "$namespaces and $schemas are kept as mapping and list through decode and encode" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.ReaderAndWriter.withNamespacesAndSchemas
            let check (tool: CWLToolDescription) =
                let metadata = Expect.wantSome tool.Metadata "The extension fields should be kept."
                let namespaces = Expect.wantSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "$namespaces" metadata) "$namespaces should be a mapping."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "cwltool" namespaces) (Some "http://commonwl.org/cwltool#") "The cwltool prefix should be kept."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc" namespaces) (Some "https://github.com/nfdi4plants/ARC_ontology") "The arc prefix should be kept."
                Expect.isSome (DynObj.tryGetTypedPropertyValue<obj> "$schemas" metadata) "$schemas should be kept."
            let decoded = Decode.decodeCommandLineTool yaml
            check decoded
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "$namespaces:" "The namespaces should be written as a mapping key."
            Expect.stringContains encoded "cwltool: http://commonwl.org/cwltool#" "The prefixes should be written as nested keys."
            Expect.stringContains encoded "$schemas:" "The schemas should be written as a key."
            Expect.stringContains encoded "https://example.org/a.owl" "The first schema should be written."
            Expect.stringContains encoded "https://example.org/b.yml" "The second schema should be written."
            check (Decode.decodeCommandLineTool encoded)
            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded "A second pass should write the same text."

        testCase "a field that occurs twice is a decode error" <| fun _ ->
            let yaml = minimalTool + "label: first\nlabel: second\n"
            let decodeError (decode: unit -> unit) =
                try
                    decode ()
                    None
                with YamlDecodeException error -> Some error
            let error = decodeError (fun () -> Decode.decodeCommandLineTool yaml |> ignore)
            Expect.isSome error "A mapping with the field label twice must fail."
            Expect.stringContains (DecodeError.text error.Value) "label" "The error should name the field."
            let repeatedKey = """a: 1
a: 2
"""
            let parameterError = decodeError (fun () -> DecodeParameters.decodeYAMLParameterFile repeatedKey |> ignore)
            Expect.isSome parameterError "A parameter file with the key a twice must fail."

        testCase "a text with two documents is a parse error" <| fun _ ->
            let yaml = minimalTool + "---\nfoo: 1\n"
            let parseError =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlParseException error -> Some error
            Expect.isSome parseError "A CWL file holds one document."

        testCase "a parameter file without content has no references" <| fun _ ->
            let onlyAComment = """# only a comment
"""
            let onlyADocumentMarker = """---
"""
            for text in [ ""; onlyAComment; onlyADocumentMarker ] do
                let references = DecodeParameters.decodeYAMLParameterFile text
                Expect.equal references.Count 0 (sprintf "The text %A holds no parameter." text)

        testCase "a block scalar at the end of the text without a line break has no final line break" <| fun _ ->
            let expressionTool (ending: string) =
                "cwlVersion: v1.2\nclass: ExpressionTool\ninputs: {}\noutputs: {}\nexpression: >\n  ${return 1;}" + ending
            Expect.equal (Decode.decodeExpressionTool (expressionTool "")).Expression "${return 1;}" "The end of the text takes the place of the line feed."
            Expect.equal (Decode.decodeExpressionTool (expressionTool "\n")).Expression "${return 1;}\n" "Clip chomping keeps the one line feed."

        testCase "an empty value where CWL requires a value is a decode error with the full path" <| fun _ ->
            let tool (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
            let requirement (lines: string list) = tool ([ "requirements:" ] @ lines @ [ "inputs: {}"; "outputs: {}" ])
            let workflow (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:" ] @ lines) + "\n"
            let steps (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  - id: s"; "    run: t.cwl" ] @ lines) + "\n"
            let portsWorkflow (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow" ] @ lines) + "\n"
            let decodeError (decode: unit -> unit) =
                try
                    decode ()
                    None
                with YamlDecodeException error -> Some error
            let emptyCwlVersion = """cwlVersion:
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
"""
            let emptyClass = """cwlVersion: v1.2
class:
inputs: {}
outputs: {}
"""
            let emptyBaseCommandItem = """cwlVersion: v1.2
class: CommandLineTool
baseCommand:
  - echo
  -
inputs: {}
outputs: {}
"""
            let emptyExpression = """cwlVersion: v1.2
class: ExpressionTool
inputs: {}
outputs: {}
expression:
"""
            let emptyStepRun = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  - id: s
    run:
    in: {}
    out: []
"""
            // the path that the error must show, and a document that holds an empty value there
            let cases =
                [ "cwlVersion", emptyCwlVersion
                  "class", emptyClass
                  "baseCommand[1]", emptyBaseCommandItem
                  // ports in map form and in sequence form
                  "inputs.x", tool [ "inputs:"; "  x:"; "outputs: {}" ]
                  "outputs.o", tool [ "inputs: {}"; "outputs:"; "  o:" ]
                  "inputs.x.type", tool [ "inputs:"; "  - id: x"; "    type:"; "outputs: {}" ]
                  "inputs.x.type[1]", tool [ "inputs:"; "  - id: x"; "    type:"; "      - string"; "      -"; "outputs: {}" ]
                  "outputs.o.type", tool [ "inputs: {}"; "outputs:"; "  - id: o"; "    type:" ]
                  "inputs.a.type", portsWorkflow [ "inputs:"; "  - id: a"; "    type:"; "outputs: {}"; "steps: {}" ]
                  "outputs.o.type", portsWorkflow [ "inputs: {}"; "outputs:"; "  - id: o"; "    type:"; "steps: {}" ]
                  "inputs.x.type.symbols[1]", tool [ "inputs:"; "  x:"; "    type:"; "      type: enum"; "      symbols:"; "        - a"; "        -"; "outputs: {}" ]
                  // requirements
                  "requirements[0].envDef[0].envName", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName:"; "        envValue: a" ]
                  "requirements[0].envDef[0].envValue", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue:" ]
                  "requirements[0].envDef[0].envValue", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: null" ]
                  "requirements[0].envDef.A", requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      A:" ]
                  "requirements.EnvVarRequirement.envDef.A", tool [ "requirements:"; "  EnvVarRequirement:"; "    envDef:"; "      A:"; "inputs: {}"; "outputs: {}" ]
                  "requirements[0].packages[0].package", requirement [ "  - class: SoftwareRequirement"; "    packages:"; "      - package:" ]
                  "requirements[0].packages[0].specs[0]", requirement [ "  - class: SoftwareRequirement"; "    packages:"; "      - package: a"; "        specs:"; "          -" ]
                  "requirements[0].packages.samtools.specs[0]", requirement [ "  - class: SoftwareRequirement"; "    packages:"; "      samtools:"; "        specs:"; "          -" ]
                  "requirements[0].listing", requirement [ "  - class: InitialWorkDirRequirement"; "    listing:" ]
                  "requirements[0].listing[0].entry", requirement [ "  - class: InitialWorkDirRequirement"; "    listing:"; "      - entryname: a"; "        entry:" ]
                  "requirements[0].timelimit", requirement [ "  - class: ToolTimeLimit"; "    timelimit:" ]
                  "requirements[0].class", requirement [ "  - class:" ]
                  "expression", emptyExpression
                  // steps in map form and in sequence form
                  "steps.s.run", workflow [ "    run:"; "    in: {}"; "    out: []" ]
                  "steps.s.in.x", workflow [ "    run: t.cwl"; "    in:"; "      x:"; "    out: []" ]
                  "steps.s.out", workflow [ "    run: t.cwl"; "    in: {}"; "    out:" ]
                  "steps.s.out[1]", workflow [ "    run: t.cwl"; "    in: {}"; "    out:"; "      - a"; "      -" ]
                  "steps.s.run", emptyStepRun
                  "steps.s.out", steps [ "    in: {}"; "    out:" ]
                  "steps.s.in.x", steps [ "    in:"; "      x:"; "    out: []" ]
                  "steps.s.in.x[1]", steps [ "    in:"; "      - id: x"; "        source:"; "          - a"; "          -"; "    out: []" ]
                  "steps.s.out[0].id", steps [ "    in: {}"; "    out:"; "      - id:" ] ]
            for (path, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCWLProcessingUnit yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "The error should show this path:\n%s" yaml)
                    Expect.stringContains error.Message "Expected a value, found" "The error should say that a value is missing."
                | None -> failwith (sprintf "An empty value at %s must fail:\n%s" path yaml)
            // a quoted null is a string, and the empty forms of out are not empty values
            let quoted = requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: \"null\"" ]
            Expect.isSome (Decode.decodeCommandLineTool quoted).Requirements "A quoted null is the string null."
            for out in [ "[]"; "{}" ] do
                let decoded = Decode.decodeWorkflow (workflow [ "    run: t.cwl"; "    in: {}"; "    out: " + out ])
                Expect.equal decoded.Steps.[0].Out.Count 0 (sprintf "out: %s has no outputs." out)

        testCase "encoded CWL has compact list items, inline scalar lists and line feeds only" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: [dotnet, fsi, script.fsx]
requirements:
  - class: InlineJavascriptRequirement
  - class: NetworkAccess
    networkAccess: true
inputs: {}
outputs: {}
"""
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            Expect.stringContains encoded "baseCommand: [dotnet, fsi, script.fsx]" "A list of single-line scalars stays inline."
            Expect.stringContains encoded "  - class: InlineJavascriptRequirement" "A list item that holds a mapping starts on the line of its dash."
            Expect.stringContains encoded "  - class: NetworkAccess" "Every item is written this way."
            Expect.isFalse (encoded.Contains "\r") "The writer ends every line with a line feed."

        testCase "raw YAML kept in a CWL object compares and hashes by value" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.RawValues.withDefaultsAndExtensionField
            let first = Decode.decodeCommandLineTool yaml
            let second = Decode.decodeCommandLineTool yaml
            Expect.equal first.Inputs.Value.[0] second.Inputs.Value.[0] "Two decodes of the same port are equal."
            Expect.equal (first.Inputs.Value.[0].GetHashCode()) (second.Inputs.Value.[0].GetHashCode()) "Equal ports have the same hash."
            Expect.isTrue (DynamicObjHelpers.dynamicPropertiesEqual first.Metadata.Value second.Metadata.Value) "A preserved null compares by its node."
            Expect.equal (DynamicObjHelpers.hashDynamicProperties first.Metadata.Value) (DynamicObjHelpers.hashDynamicProperties second.Metadata.Value) "Equal extension fields have the same hash."
            Expect.equal first second "Two decodes of the same text are equal tools."
            let changed = Decode.decodeCommandLineTool (yaml.Replace("default: one", "default: two"))
            Expect.notEqual first.Inputs.Value.[1] changed.Inputs.Value.[1] "A different default value is a different port."
            Expect.notEqual first changed "A different default value is a different tool."

        testCase "raw YAML with comments is equal after a roundtrip" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.RawValues.withComments
            let decoded = Decode.decodeCommandLineTool yaml
            let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.equal roundTripped decoded "The data of the raw values survives a write and a read."
            Expect.equal (roundTripped.GetHashCode()) (decoded.GetHashCode()) "Equal tools have the same hash."

        testCase "an expression keeps its line feeds in front of a metadata section" <| fun _ ->
            let expressionTool (header: string) (blankLines: string) (metadata: string) =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: ExpressionTool"; "inputs: {}"; "outputs: {}"; "expression: " + header; "  ${"; "    return {};"; "  }" ] + "\n" + blankLines + metadata
            // the header, the blank lines behind the body and the value that they give
            let cases =
                [ "|-", "", "${\n  return {};\n}"
                  "|", "", "${\n  return {};\n}\n"
                  "|+", "\n", "${\n  return {};\n}\n\n"
                  "|+", "\n\n", "${\n  return {};\n}\n\n\n" ]
            for (header, blankLines, expected) in cases do
                for metadata in [ "s:author: me\n"; "" ] do
                    let decoded = Decode.decodeExpressionTool (expressionTool header blankLines metadata)
                    Expect.equal decoded.Expression expected (sprintf "The header %s should give this value." header)
                    let encoded = Encode.encodeExpressionToolDescription decoded
                    let roundTripped = Decode.decodeExpressionTool encoded
                    Expect.equal roundTripped.Expression expected (sprintf "The expression with %s and %d blank lines should survive a write and a read, written as:\n%s" header blankLines.Length encoded)
                    Expect.isTrue (encoded.EndsWith "\n") "The text ends with a line feed."
                    if metadata <> "" then
                        let author = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "s:author" roundTripped.Metadata.Value) "The metadata should survive."
                        Expect.equal author "me" "The metadata value should be kept."
                        // The line feeds between the closing brace and the next key are the end of the body line and one blank
                        // line, or the line feeds that the value keeps when there are more of them.
                        let bodyEnd = encoded.IndexOf "  }" + 3
                        let between = encoded.Substring(bodyEnd, encoded.IndexOf "s:author" - bodyEnd)
                        let keptLineFeeds = expected.Length - expected.TrimEnd('\n').Length
                        Expect.isTrue (between |> Seq.forall (fun c -> c = '\n')) "Only line feeds stand between the sections."
                        Expect.equal between.Length (max 2 keptLineFeeds) "One blank line separates the sections, and a kept blank line is that separator."

        testCase "raw YAML values keep the comments and the blank lines of their source" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.RawValues.withCommentsAndBlankLines
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            for comment in [ "# hint comment"; "# leading comment of a field"; "# item comment"; "# about files"; "# index"; "# class comment"; "# inside arguments"; "# trailing" ] do
                Expect.stringContains encoded comment (sprintf "The comment %s belongs to a raw value and should be written back." comment)
            Expect.stringContains encoded "# trailing\n\n  - y" "The blank line between the two arguments should be written back."

        testCase "a deeply nested raw value decodes, compares, hashes and writes without a stack overflow" <| fun _ ->
            let depth = 100000
            let tool (value: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x:\n    type: string\n    default: " + value + "\noutputs: {}\n"
            let flow = String.replicate depth "[" + "x" + String.replicate depth "]"
            let block = "\n      " + String.replicate depth "- " + "x"
            for value in [ flow; block ] do
                let first = Decode.decodeCommandLineTool (tool value)
                let second = Decode.decodeCommandLineTool (tool value)
                Expect.equal first second "Two decodes of the same text should be equal."
                Expect.equal (first.GetHashCode()) (second.GetHashCode()) "Two decodes of the same text should have the same hash."
                let encoded = Encode.encodeToolDescription first
                Expect.equal (Decode.decodeCommandLineTool encoded) first "The written text should decode to the same tool."

        testCase "a key that occurs twice, in a raw value or anywhere else, is a decode error with the path of the key" <| fun _ ->
            let tool (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            let cases =
                [ "inputs.x.default.a", tool [ "inputs:"; "  x:"; "    type: string"; "    default: {a: 1, a: 2}"; "outputs: []" ]
                  "inputs.x.default.a", tool [ "inputs:"; "  x:"; "    type: string"; "    default:"; "      a: 1"; "      a: 2"; "outputs: []" ]
                  "inputs.x.default.b[1].k", tool [ "inputs:"; "  x:"; "    type: string"; "    default:"; "      b:"; "        - {k: 1}"; "        - {k: 1, k: 2}"; "outputs: []" ]
                  "hints[0].list[0].k", tool [ "hints:"; "  - class: acme:Hint"; "    list:"; "      - {k: 1, \"k\": 2}"; "inputs: []"; "outputs: []" ]
                  "outputs.o.secondaryFiles[0].k", tool [ "inputs: []"; "outputs:"; "  o:"; "    type: File"; "    secondaryFiles:"; "      - {k: 1, k: 2}" ]
                  "baseCommand", tool [ "baseCommand: ls"; "inputs: []"; "outputs: []" ] ]
            for (path, yaml) in cases do
                match decodeError yaml with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "The error should show this path:\n%s" yaml)
                    Expect.stringContains error.Message "occurs twice" "The error should say that the key occurs twice."
                | None -> failwith (sprintf "A key that occurs twice at %s must fail:\n%s" path yaml)
            let sameKeyApart = tool [ "inputs:"; "  x:"; "    type: string"; "    default: {a: {a: 1}, b: {a: 2}}"; "outputs: []" ]
            Expect.isNone (decodeError sameKeyApart) "The same key in different mappings is no repeated key."

        testCase "a raw hint that starts with a comment or a blank line keeps it in the hint and is written behind a dash with a line break" <| fun _ ->
            let tool (hints: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: []"; "outputs: []"; "hints:" ] @ hints) + "\n"
            let commentLines (text: string) = text.Split('\n') |> Array.filter (fun line -> line.TrimStart().StartsWith "#") |> Array.length
            let cases =
                [ "array form, comment in front of the first field",
                  tool [ "  -"; "    # about the hint"; "    class: acme:Hint"; "    a: 1" ],
                  "hints:\n  -\n    # about the hint\n    class: acme:Hint\n    a: 1\n"
                  "array form, blank line and comment in front of the first field",
                  tool [ "  - class: acme:Other"; "  -"; ""; "    # about the hint"; "    class: acme:Hint"; "    a: 1" ],
                  "  - class: acme:Other\n  -\n\n    # about the hint\n    class: acme:Hint\n    a: 1\n"
                  "map form with the class in the payload",
                  tool [ "  acme:Hint:"; "    # about the hint"; "    class: acme:Hint"; "    a: 1" ],
                  "hints:\n  -\n    # about the hint\n    class: acme:Hint\n    a: 1\n"
                  "map form without the class in the payload",
                  tool [ "  acme:Hint:"; "    # about the hint"; "    a: 1" ],
                  "hints:\n  - class: acme:Hint\n    # about the hint\n    a: 1\n" ]
            for (name, yaml, expected) in cases do
                let decoded = Decode.decodeCommandLineTool yaml
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "%s: the comment should stay in front of the first field." name)
                Expect.equal (commentLines encoded) (commentLines yaml) (sprintf "%s: the comment should be written once." name)
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should decode to the same tool." name)
                Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: a second pass should write the same text." name)

        testCase "a comment at the end of a raw value moves in front of its last entry when the value is no longer the last node of the document" <| fun _ ->
            let header = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n"
            let cases =
                [ // the name, the document and the text that the written document holds
                  "hints in array form, with blank lines at the end of the document",
                  header + "inputs: []\noutputs: []\nhints:\n  - class: acme:Unknown\n    foo: bar\n    # baz: qux\n\n\n",
                  "  - class: acme:Unknown\n    # baz: qux\n    foo: bar\n\nbaseCommand: [echo]\n"
                  "hints in map form",
                  header + "inputs: []\noutputs: []\nhints:\n  acme:Unknown:\n    foo: bar\n    # baz: qux\n",
                  "  - class: acme:Unknown\n    # baz: qux\n    foo: bar\n\nbaseCommand: [echo]\n"
                  "arguments in front of extension metadata",
                  header + "inputs: []\noutputs: []\ns:author: me\narguments:\n  - valueFrom: x\n    # tail\n",
                  "  -\n    # tail\n    valueFrom: x\ns:author: me\n"
                  "input default as a mapping",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a: 1\n      # tail\n",
                  "      # tail\n      a: 1\n\noutputs: {}\n"
                  "input default as a sequence",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      - a\n      - b\n      # tail\n",
                  "      - a\n      # tail\n      - b\n\noutputs: {}\n"
                  "input default in front of a field of the same input",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    inputBinding:\n      prefix: -x\n    default:\n      a: 1\n      # tail\n",
                  "      # tail\n      a: 1\n    inputBinding:\n"
                  "nested collections that each end with a comment",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a:\n        b:\n          - 1\n          # deep\n        # middle\n      # outer\n",
                  "      # outer\n      a:\n        # middle\n        b:\n          # deep\n          - 1\n\noutputs: {}\n"
                  "secondaryFiles of an input",
                  header + "outputs: []\ninputs:\n  x:\n    type: File\n    secondaryFiles:\n      - pattern: .bai\n        # tail\n",
                  "      -\n        # tail\n        pattern: .bai\n\noutputs: {}\n"
                  "default of a step input in map form",
                  TestObjects.CWL.Workflow.TrailingComments.stepInputDefaultInMapForm,
                  "          # tail\n          k: v\n    out: []\n"
                  "default of a step input in sequence form",
                  TestObjects.CWL.Workflow.TrailingComments.stepInputDefaultInSequenceForm,
                  "          - 1\n          # tail\n          - 2\n    out: []\n"
                  "a kept block scalar before the comment",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a: |+\n        abc\n\n      # tail\n",
                  "      # tail\n      a: |+\n        abc\n\noutputs: {}\n" ]
            let commentLines (text: string) = text.Split('\n') |> Array.filter (fun line -> line.TrimStart().StartsWith "#") |> Array.length
            for (name, yaml, expected) in cases do
                let decoded = Decode.decodeCWLProcessingUnit yaml
                let encoded = Encode.encodeProcessingUnit decoded
                Expect.stringContains encoded expected (sprintf "%s: the comment should stand in front of the last entry." name)
                Expect.equal (commentLines encoded) (commentLines yaml) (sprintf "%s: every comment should be written once." name)
                Expect.equal (Decode.decodeCWLProcessingUnit encoded) decoded (sprintf "%s: the written text should decode to the same document." name)
                Expect.equal (Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit encoded)) encoded (sprintf "%s: a second pass should write the same text." name)

        testCase "a comment at the end of a raw value of depth 100000 moves without a stack overflow" <| fun _ ->
            let depth = 100000
            let yaml =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\noutputs: []\ninputs:\n  x:\n    type: string\n    default:\n      "
                + String.replicate depth "- " + "x\n      # tail\n"
            let decoded = Decode.decodeCommandLineTool yaml
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "# tail\n" "The comment should be written."
            Expect.equal (encoded.Split("# tail").Length) 2 "The comment should be written once."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded "A second pass should write the same text."

        testCase "a map-form hint without a payload is the hint with an empty payload" <| fun _ ->
            let tool (hints: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: []"; "outputs: []"; "hints:" ] @ hints) + "\n"
            for hint in [ "acme:Hint"; "NetworkAccess"; "WorkReuse" ] do
                let withoutPayload = Decode.decodeCommandLineTool (tool [ "  " + hint + ":" ])
                let emptyPayload = Decode.decodeCommandLineTool (tool [ "  " + hint + ": {}" ])
                Expect.equal withoutPayload emptyPayload (sprintf "%s without a payload should equal %s with {}." hint hint)
                let encoded = Encode.encodeToolDescription withoutPayload
                Expect.isFalse (encoded.Contains "value:") (sprintf "%s: the hint has no value field.\n%s" hint encoded)
                Expect.equal (Decode.decodeCommandLineTool encoded) withoutPayload (sprintf "%s: a round trip should keep the hint." hint)
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool [ "  acme:Hint:" ]))
            Expect.stringContains encoded "class: acme:Hint" "The unknown hint should keep its class."

        testCase "a raw block scalar that keeps its line feeds is written back with them in front of a section" <| fun _ ->
            let tool (scalar: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x:\n    type: string\n    default: " + scalar + "outputs: []\n"
            let cases =
                [ // the source, the text that follows default: in the written document
                  "|+\n", "|+\noutputs: {}\n"
                  "|+\n      abc\n", "|+\n      abc\noutputs: {}\n"
                  "|+\n      abc\n\n", "|+\n      abc\n\noutputs: {}\n"
                  "|+\n      abc\n\n\n", "|+\n      abc\n\n\noutputs: {}\n"
                  "|\n      abc\n", "|\n      abc\n\noutputs: {}\n"
                  "|\n", "|\n\noutputs: {}\n"
                  "|-\n      abc\n", "|-\n      abc\n\noutputs: {}\n"
                  "|-\n", "|-\n\noutputs: {}\n" ]
            for (source, expected) in cases do
                let decoded = Decode.decodeCommandLineTool (tool source)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "The scalar %s should be written back with its line feeds." (source.Replace("\n", "\\n")))
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "The scalar %s should read back as the same tool." (source.Replace("\n", "\\n")))

        testCase "kept block scalars inside a raw value stay in front of the next section and of a comment" <| fun _ ->
            let header = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n"
            let cases =
                [ // the document and the text that follows the raw value in the written document
                  "a kept scalar, then a comment at the end of the document",
                  header + "outputs: []\ninputs:\n  x:\n    type: string\n    default:\n      a: |+\n        abc\n\n      # tail\n",
                  "      # tail\n      a: |+\n        abc\n\noutputs: {}\n"
                  "kept scalars at the end of nested mappings",
                  header + "inputs:\n  x:\n    type: string\n    default:\n      a: |+\n        abc\n\n      b:\n        c: |+\n          q\n\noutputs: []\n",
                  "          q\n\noutputs: {}\n"
                  "a kept scalar as the last item of a sequence",
                  header + "inputs:\n  x:\n    type: string\n    default:\n      - a\n      - |+\n        abc\n\noutputs: []\n",
                  "        abc\n\noutputs: {}\n" ]
            for (name, yaml, expected) in cases do
                let decoded = Decode.decodeCommandLineTool yaml
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "%s: the lines of the scalar should come first." name)
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should read back as the same tool." name)

        testCase "a record field keeps its type and its other properties apart, in map and array form, for input and output records" <| fun _ ->
            // the lines of the type of the field, and the type that the decoder must find (None where the text is the check)
            let types : (string * string list * CWLType option) list =
                [ "File", [ "type: File" ], Some (CWLType.File)
                  "Directory", [ "type: Directory" ], Some (CWLType.Directory)
                  "string", [ "type: string" ], Some CWLType.String
                  "int", [ "type: int" ], Some CWLType.Int
                  "long", [ "type: long" ], Some CWLType.Long
                  "float", [ "type: float" ], Some CWLType.Float
                  "double", [ "type: double" ], Some CWLType.Double
                  "boolean", [ "type: boolean" ], Some CWLType.Boolean
                  "optional File", [ "type: File?" ], Some (Union (ResizeArray [ Null; CWLType.File ]))
                  "File array", [ "type: File[]" ], Some (Array (InputArraySchema(CWLType.File)))
                  "union list", [ "type:"; "  - \"null\""; "  - File" ], Some (Union (ResizeArray [ Null; CWLType.File ]))
                  "union of two types", [ "type:"; "  - string"; "  - int" ], Some (Union (ResizeArray [ CWLType.String; CWLType.Int ]))
                  "array schema", [ "type:"; "  type: array"; "  items: File" ], Some (Array (InputArraySchema(CWLType.File)))
                  "nested array schema", [ "type:"; "  type: array"; "  items:"; "    type: array"; "    items: string" ], None
                  "array of records", [ "type:"; "  type: array"; "  items:"; "    type: record"; "    fields:"; "      g:"; "        type: File"; "        inputBinding:"; "          prefix: --g" ], None
                  "enum", [ "type:"; "  type: enum"; "  symbols: [a, b]" ], None
                  "nested record", [ "type:"; "  type: record"; "  fields:"; "    g:"; "      type: File"; "      inputBinding:"; "        prefix: --g"; "    h: string" ], None ]
            let indent (count: int) (lines: string list) = lines |> List.map (fun line -> System.String(' ', count) + line)
            for isOutput in [ false; true ] do
                let binding = if isOutput then "outputBinding" else "inputBinding"
                let bindingLines = if isOutput then [ "outputBinding:"; "  glob: x" ] else [ "inputBinding:"; "  prefix: --f" ]
                let propertySets : (string * string list * string list) list =
                    [ "no other property", [], []
                      "a binding", bindingLines, [ binding ]
                      "all properties", [ "doc: the field"; "label: The field"; "secondaryFiles: .bai"; "streamable: true" ] @ bindingLines, [ "doc"; "label"; "secondaryFiles"; "streamable"; binding ] ]
                for isArrayForm in [ false; true ] do
                    for (typeName, typeLines, expectedType) in types do
                        for (propertyName, propertyLines, propertyKeys) in propertySets do
                            let field = (if isArrayForm then [ "        - name: f" ] else [ "        f:" ]) @ indent 10 (typeLines @ propertyLines)
                            let head = [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ]
                            let record = [ "      type: record"; "      fields:" ]
                            let lines =
                                if isOutput then head @ [ "inputs:"; "  x: string"; "outputs:"; "  o:"; "    type:" ] @ record @ field
                                else head @ [ "inputs:"; "  r:"; "    type:" ] @ record @ field @ [ "outputs: []" ]
                            let yaml = String.concat "\n" lines + "\n"
                            let name = sprintf "%s, %s form, %s, %s" typeName (if isArrayForm then "array" else "map") propertyName (if isOutput then "output" else "input")
                            let decoded = Decode.decodeCommandLineTool yaml
                            let port = if isOutput then decoded.Outputs.[0].Type_ else decoded.Inputs.Value.[0].Type_
                            let recordField =
                                match port with
                                | Some (Record schema) -> schema.Fields.Value.[0]
                                | other -> failwithf "%s: expected a record, found %A" name other
                            Expect.equal recordField.Name "f" (sprintf "%s: the field name" name)
                            match expectedType with
                            | Some expected -> Expect.isTrue (CWLType.typesEqual expected recordField.Type) (sprintf "%s: the type of the field is %A, found %A" name expected recordField.Type)
                            | None -> ()
                            let fieldKeys = recordField.GetProperties(false) |> Seq.map (fun property -> property.Key) |> Seq.toList
                            for key in propertyKeys do
                                if key <> "doc" && key <> "label" then
                                    Expect.isTrue (List.contains key fieldKeys) (sprintf "%s: the property %s stays on the field" name key)
                            let encoded = Encode.encodeToolDescription decoded
                            for key in propertyKeys do
                                Expect.equal (encoded.Split(key + ":").Length - 1) (yaml.Split(key + ":").Length - 1) (sprintf "%s: %s is written once\n%s" name key encoded)
                            Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should decode to the same tool\n%s" name encoded)
                            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: a second pass should write the same text" name)

        testCase "a record field in map form can be the type itself or a type list" <| fun _ ->
            let tool (fields: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  r:"; "    type:"; "      type: record"; "      fields:" ] @ fields @ [ "outputs: []" ]) + "\n"
            let yaml =
                tool [ "        a: File"
                       "        b: string?"
                       "        c: [\"null\", File]"
                       "        d:"
                       "          type: File"
                       "          doc: object form" ]
            let decoded = Decode.decodeCommandLineTool yaml
            let fields = match decoded.Inputs.Value.[0].Type_ with | Some (Record schema) -> schema.Fields.Value | other -> failwithf "Expected a record, found %A" other
            Expect.sequenceEqual (fields |> Seq.map (fun field -> field.Name)) [ "a"; "b"; "c"; "d" ] "The fields keep their order."
            Expect.isTrue (CWLType.typesEqual (CWLType.File) fields.[0].Type) "A field can be given as the type."
            Expect.isTrue (CWLType.typesEqual (Union (ResizeArray [ Null; CWLType.String ])) fields.[1].Type) "A field can be given as an optional type."
            Expect.isTrue (CWLType.typesEqual (Union (ResizeArray [ Null; CWLType.File ])) fields.[2].Type) "A field can be given as a type list."
            Expect.equal fields.[3].Doc (Some "object form") "A field object keeps its doc."
            let encoded = Encode.encodeToolDescription decoded
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded "A second pass should write the same text."

        testCase "an invalid type name is a decode error at its place in every type position" <| fun _ ->
            let tool (portLines: string list) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            let invalid = "Invalid CWL type: foo"
            let bare (word: string) = "Expected a type name, found \"" + word + "\". A schema is a mapping, so a schema belongs under type."
            let record (fieldLines: string list) = [ "  r:"; "    type:"; "      type: record"; "      fields:" ] @ fieldLines
            let cases =
                [ "a port type", "inputs.x", invalid, [ "  x: foo" ]
                  "a port type with the shorthand", "inputs.x", invalid, [ "  x: foo[]?" ]
                  "the type field of a port", "inputs.x.type", invalid, [ "  x:"; "    type: foo" ]
                  "a member of a type list", "inputs.x[1]", invalid, [ "  x: [string, foo]" ]
                  "a member of the type field of a port", "inputs.x.type[1]", invalid, [ "  x:"; "    type: [string, foo]" ]
                  "a bare array as a member of a type list", "inputs.x[1]", bare "array", [ "  x: [\"null\", array]" ]
                  "a bare record as the type of a port", "inputs.x", bare "record", [ "  x: record" ]
                  "the items of an array", "inputs.x.type.items", invalid, [ "  x:"; "    type:"; "      type: array"; "      items: foo" ]
                  "a member of the items of an array", "inputs.x.type.items[1]", invalid, [ "  x:"; "    type:"; "      type: array"; "      items: [string, foo]" ]
                  "items that are a mapping without a type", "inputs.x.type.items", "a mapping without a type field", [ "  x:"; "    type:"; "      type: array"; "      items: {doc: a}" ]
                  "a record field in map form", "inputs.r.type.fields.f", invalid, record [ "        f: foo" ]
                  "a bare array as a record field in map form", "inputs.r.type.fields.f", bare "array", record [ "        f: array" ]
                  "the type field of a record field", "inputs.r.type.fields.f.type", invalid, record [ "        f:"; "          type: foo" ]
                  "a type list in a record field", "inputs.r.type.fields.f.type[1]", bare "array", record [ "        f:"; "          type: [\"null\", array]" ] ]
            for (name, path, message, portLines) in cases do
                match decodeError (tool portLines) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the path of the type" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
            let output = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: []
outputs:
  o: foo
"""
            match decodeError output with
            | Some error -> Expect.equal (DecodeError.pathText error) "outputs.o" "The error of an output type should show the output."
            | None -> failwith "An invalid output type must fail"

        testCase "a type object with a type name is a decode error at its type field in every type position" <| fun _ ->
            let tool (portLines: string list) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"
            let items (lines: string list) = [ "  x:"; "    type:"; "      type: array" ] @ lines
            let cases =
                [ "File under items", "inputs.x.type.items.type", "File", items [ "      items: {type: File}" ]
                  "File? under items", "inputs.x.type.items.type", "File?", items [ "      items: {type: \"File?\"}" ]
                  "Directory? in a list under items", "inputs.x.type.items[1].type", "Directory?", items [ "      items: [string, {type: \"Directory?\"}]" ]
                  "File with a format under items", "inputs.x.type.items.type", "File", items [ "      items: {type: File, format: \"http://edamontology.org/format_2330\"}" ]
                  "optional File with a size in the type field of a port", "inputs.x.type.type", "File?", [ "  x:"; "    type:"; "      type: \"File?\""; "      size: 5" ]
                  "File with a format in a type list", "inputs.x[1].type", "File", [ "  x: [\"null\", {type: File, format: \"http://edamontology.org/format_2330\"}]" ]
                  "a stream type", "inputs.x.type.items.type", "stdout", items [ "      items: {type: stdout}" ]
                  "a name that is no type", "inputs.x.type.items.type", "foo", items [ "      items: {type: foo}" ] ]
            for (name, path, word, portLines) in cases do
                match (try Decode.decodeCommandLineTool (tool portLines) |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the path of the type" name)
                    Expect.stringContains error.Message ("Expected record, enum or array in the type field of a schema, found the text \"" + word + "\"") (sprintf "%s: the error should say what CWL reads" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)

        testCase "the fields of a record that are neither a list nor a mapping are a decode error at fields" <| fun _ ->
            let tool (fields: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  r:\n    type:\n      type: record\n      fields: " + fields + "\noutputs: []\n"
            for fields in [ "abc"; "42"; "true"; "\"x\"" ] do
                match (try Decode.decodeCommandLineTool (tool fields) |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "inputs.r.type.fields" (sprintf "fields: %s should fail at the field." fields)
                    Expect.stringContains error.Message "Expected a list of fields or a mapping of fields" (sprintf "fields: %s should say what CWL expects." fields)
                | None -> failwith (sprintf "fields: %s must fail" fields)
            for fields in [ "[]"; "{}"; "[{name: f, type: string}]"; "{f: string}" ] do
                Expect.isSome (Decode.decodeCommandLineTool (tool fields)).Inputs.Value.[0].Type_ (sprintf "fields: %s is a list or a mapping." fields)

        testCase "a schema in place of the field object of a record field is a decode error that names the field" <| fun _ ->
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            let tool (fields: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  r:"; "    type:"; "      type: record"; "      fields:" ] @ fields @ [ "outputs: []" ]) + "\n"
            let cases =
                [ "an array schema in map form", "array", "inputs.r.type.fields.f.type", tool [ "        f:"; "          type: array"; "          items: string" ]
                  "a record schema in map form", "record", "inputs.r.type.fields.f.type", tool [ "        f:"; "          type: record"; "          fields:"; "            g: string" ]
                  "an enum schema in map form", "enum", "inputs.r.type.fields.f.type", tool [ "        f:"; "          type: enum"; "          symbols: [a, b]" ]
                  "an array schema in array form", "array", "inputs.r.type.fields[0].f.type", tool [ "        - name: f"; "          type: array"; "          items: string" ] ]
            for (name, word, path, yaml) in cases do
                match decodeError yaml with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the path of the field" name)
                    Expect.stringContains error.Message ("found \"" + word + "\"") (sprintf "%s: the error should say what it found" name)
                    Expect.stringContains error.Message "a schema belongs under type" (sprintf "%s: the error should say where the schema belongs" name)
                | None -> failwith (sprintf "%s: a schema in place of the field object must fail" name)
            // the same schema under type is the field object that CWL reads
            let valid = tool [ "        f:"; "          type:"; "            type: array"; "            items: string" ]
            Expect.isNone (decodeError valid) "A schema under type is valid."

        testCase "a union of null and records keeps the order of the fields and the binding of each field" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.UnionPorts.withRecordMembers
            let decoded = Decode.decodeCommandLineTool yaml
            let recordFields (index: int) =
                match decoded.Inputs.Value.[0].Type_ with
                | Some (Union members) ->
                    match members.[index] with
                    | Record schema -> schema.Fields.Value
                    | other -> failwithf "Expected a record, found %A" other
                | other -> failwithf "Expected a union, found %A" other
            let withFile = recordFields 1
            let withFlags = recordFields 2
            Expect.sequenceEqual (withFile |> Seq.map (fun field -> field.Name)) [ "source"; "title" ] "The fields of the first record keep their order."
            Expect.sequenceEqual (withFlags |> Seq.map (fun field -> field.Name)) [ "verbose"; "quiet" ] "The fields of the second record keep their order."
            Expect.equal withFile.[1].Type CWLType.String "title is a string."
            Expect.equal withFlags.[0].Type CWLType.Boolean "verbose is a boolean."
            Expect.equal withFlags.[1].Type CWLType.Boolean "quiet is a boolean."
            Expect.equal withFile.[0].Type CWLType.File "source is a File."
            for field in Seq.append withFile withFlags do
                Expect.isTrue (field.GetProperties(false) |> Seq.exists (fun property -> property.Key = "inputBinding")) (sprintf "%s keeps its inputBinding." field.Name)
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "        source:\n          type: File\n          inputBinding:\n            prefix: --source\n        title:\n          type: string\n          inputBinding:\n            prefix: --title\n" "A File field is the type File and its binding."
            for prefix in [ "--source"; "--title"; "--verbose"; "--quiet" ] do
                Expect.equal (encoded.Split("prefix: " + prefix).Length) 2 (sprintf "The binding %s is written once." prefix)
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded "A second pass should write the same text."

        testCase "an array of arrays, array items and the members of a type list are written as schemas and lists in a CWL document" <| fun _ ->
            let tool (portLines: string list) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"
            let cases =
                [ // the name, the lines of the input x and the text that the written document holds for it
                  "array of arrays",
                  [ "  x:"; "    type:"; "      type: array"; "      items:"; "        type: array"; "        items: File" ],
                  "  x:\n    type:\n      type: array\n      items:\n        type: array\n        items: File\n"
                  "optional items",
                  [ "  x:"; "    type:"; "      type: array"; "      items: [\"null\", File]" ],
                  "      items: [\"null\", File]\n"
                  "union with an array schema as a member",
                  [ "  x: [string, {type: array, items: File}]" ],
                  "  x: [string, \"File[]\"]\n"
                  "type list members with the shorthand",
                  [ "  x: [string, \"File[]\"]"; "  y: [int, \"File?\"]" ],
                  "  x: [string, \"File[]\"]\n  y: [int, File?]\n"
                  "optional array of arrays",
                  [ "  x:"; "    type:"; "      - \"null\""; "      - type: array"; "        items:"; "          type: array"; "          items: string" ],
                  "  x:\n    - \"null\"\n    - type: array\n      items:\n        type: array\n        items: string\n"
                  "array of arrays as the type of a record field",
                  [ "  x:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type:"; "            type: array"; "            items:"; "              type: array"; "              items: string" ],
                  "        f:\n          type:\n            type: array\n            items:\n              type: array\n              items: string\n"
                  "arrays of one level keep the shorthand",
                  [ "  x: File[]"; "  y: File[]?"; "  z: string?" ],
                  "  x: File[]\n  y: File[]?\n  z: string?\n" ]
            for (name, portLines, expected) in cases do
                let decoded = Decode.decodeCommandLineTool (tool portLines)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded expected (sprintf "%s: the type should be written as CWL reads it." name)
                Expect.isFalse (encoded.Contains "[][]") (sprintf "%s: CWL reads one [] after a name." name)
                Expect.isFalse (encoded.Contains "items: File[]" || encoded.Contains "items: File?") (sprintf "%s: CWL reads no shorthand in items." name)
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should decode to the same tool." name)
                Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: a second pass should write the same text." name)

        testCase "a nested array and any shorthand under items are decode errors at the field" <| fun _ ->
            let tool (portLines: string list) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            let nested = "Expected one [] behind a type name"
            let underItems = "Expected a type name or a schema under items"
            let cases =
                [ "a nested array as a port type", "inputs.x", nested, [ "  x: File[][]" ]
                  "a nested optional array as a port type", "inputs.x", nested, [ "  x: File[][]?" ]
                  "a nested array in the type field of a port", "inputs.x.type", nested, [ "  x:"; "    type: File[][]" ]
                  "a nested array as a member of a type list", "inputs.x[1]", nested, [ "  x: [string, \"File[][]\"]" ]
                  "a nested array as the type of a record field", "inputs.r.type.fields.f", nested, [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f: File[][]" ]
                  "a nested array in the type field of a record field", "inputs.r.type.fields.f.type", nested, [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type: File[][]" ]
                  "an array as the items of an array", "inputs.x.type.items", underItems, [ "  x:"; "    type:"; "      type: array"; "      items: File[]" ]
                  "an optional type as the items of an array", "inputs.x.type.items", underItems, [ "  x:"; "    type:"; "      type: array"; "      items: File?" ]
                  "an optional member in the items of an array", "inputs.x.type.items[1]", underItems, [ "  x:"; "    type:"; "      type: array"; "      items: [string, \"File?\"]" ] ]
            for (name, path, message, portLines) in cases do
                match decodeError (tool portLines) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the path of the type" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what CWL reads" name)
                | None -> failwith (sprintf "%s must fail" name)
            // one level of shorthand is read as a type, as a member of a type list and as the type of a record field
            let read (portLines: string list) = (Decode.decodeCommandLineTool (tool portLines)).Inputs.Value
            let arrayOf (inner: CWLType) = CWLType.Array (InputArraySchema(inner))
            let optionalOf (inner: CWLType) = CWLType.Union (ResizeArray [ CWLType.Null; inner ])
            let members = read [ "  x: [string, \"File[]\"]"; "  y: [int, \"File?\"]"; "  z: [\"null\", \"string[]\"]" ]
            Expect.equal members.[0].Type_ (Some (CWLType.Union (ResizeArray [ CWLType.String; arrayOf (CWLType.File) ]))) "A member may be an array shorthand."
            Expect.equal members.[1].Type_ (Some (CWLType.Union (ResizeArray [ CWLType.Int; optionalOf (CWLType.File) ]))) "A member may be an optional shorthand."
            Expect.equal members.[2].Type_ (Some (optionalOf (arrayOf CWLType.String))) "A list of null and an array shorthand is an optional array."
            let fieldTypes = read [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f: File[]?"; "        g:"; "          type: string[]" ]
            match fieldTypes.[0].Type_ with
            | Some (CWLType.Record schema) ->
                Expect.equal schema.Fields.Value.[0].Type (optionalOf (arrayOf (CWLType.File))) "A record field reads File[]? as a type."
                Expect.equal schema.Fields.Value.[1].Type (arrayOf CWLType.String) "A record field reads string[] in its type field."
            | other -> failwith (sprintf "Expected a record but got %A" other)
            let items = read [ "  x:"; "    type:"; "      type: array"; "      items: [string, File]" ]
            Expect.equal items.[0].Type_ (Some (arrayOf (CWLType.Union (ResizeArray [ CWLType.String; CWLType.File ])))) "A name under items needs no shorthand."

        testCase "the shorthand text of a type nests arrays and the type of a CWL document does not" <| fun _ ->
            let nested = Array (InputArraySchema(Array (InputArraySchema(CWLType.File))))
            Expect.equal ((Encode.encodeCWLTypeShorthand nested |> Encode.writeYaml).Trim()) "File[][]" "The shorthand text nests the arrays."
            Expect.equal ((Encode.encodeCWLType nested |> Encode.writeYaml).Trim()) "type: array\nitems:\n  type: array\n  items: File" "A CWL document writes the schema."
            Expect.equal ((Encode.encodeCWLType (Array (InputArraySchema(CWLType.File))) |> Encode.writeYaml).Trim()) "File[]" "An array of one level is the shorthand."

        testCase "a process without inputs writes the empty inputs that CWL requires and reads back equal" <| fun _ ->
            let tool (inputs: string) = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n" + inputs + "outputs: []\n"
            let expression (inputs: string) =
                "cwlVersion: v1.2\nclass: ExpressionTool\n" + inputs + "outputs: []\nexpression: \"${ return {}; }\"\nrequirements:\n  - class: InlineJavascriptRequirement\n"
            let operation (inputs: string) = "cwlVersion: v1.2\nclass: Operation\n" + inputs + "outputs: []\n"
            let workflow (inputs: string) (steps: string) = "cwlVersion: v1.2\nclass: Workflow\n" + inputs + "outputs: []\n" + steps
            let step (toolInputs: string) (inOut: string) =
                "steps:\n  s:\n    run:\n      class: CommandLineTool\n      baseCommand: echo\n" + toolInputs + "      outputs: []\n" + inOut
            let cases =
                [ "a tool with an empty list", tool "inputs: []\n"
                  "a tool with an empty map", tool "inputs: {}\n"
                  "an expression tool with an empty list", expression "inputs: []\n"
                  "an operation with an empty list", operation "inputs: []\n"
                  "a workflow with an empty list", workflow "inputs: []\n" "steps: []\n"
                  "a workflow with an inline tool with empty inputs", workflow "inputs: {}\n" (step "      inputs: {}\n" "    in: {}\n    out: []\n")
                  "a workflow with empty lists of step inputs and step outputs", workflow "inputs: {}\n" (step "      inputs: []\n" "    in: []\n    out: []\n") ]
            for (name, yaml) in cases do
                let decoded = Decode.decodeCWLProcessingUnit yaml
                let encoded = Encode.encodeProcessingUnit decoded
                Expect.stringContains encoded "inputs: {}\n" (sprintf "%s: the inputs are written\n%s" name encoded)
                Expect.stringContains encoded "outputs: {}\n" (sprintf "%s: the outputs are written\n%s" name encoded)
                Expect.equal (Decode.decodeCWLProcessingUnit encoded) decoded (sprintf "%s: the written text should decode to the same document" name)
                Expect.equal (Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit encoded)) encoded (sprintf "%s: a second pass should write the same text" name)
            let workflowText = Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit (workflow "inputs: {}\n" (step "      inputs: []\n" "    in: {}\n    out: []\n")))
            Expect.stringContains workflowText "      inputs: {}\n" "The inline tool of a step writes its inputs too."
            Expect.stringContains workflowText "    in: {}\n    out: []\n" "The step writes in and out."

        testCase "a document without inputs is a decode error at the place of the process for every class" <| fun _ ->
            let step = TestObjects.CWL.Workflow.WithoutInputs.toolInStepMapForm
            let stepList = TestObjects.CWL.Workflow.WithoutInputs.toolInStepListForm
            let toolWithoutInputs = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
outputs: []
"""
            let expressionToolWithoutInputs = """cwlVersion: v1.2
class: ExpressionTool
outputs: []
expression: "${ return {}; }"
"""
            let operationWithoutInputs = """cwlVersion: v1.2
class: Operation
outputs: []
"""
            let workflowWithoutInputs = """cwlVersion: v1.2
class: Workflow
outputs: []
steps: []
"""
            // the document, the process that needs inputs, the path of the error and the line where the process starts
            let documents =
                [ "a tool", toolWithoutInputs, "a command line tool", "", 1
                  "an expression tool", expressionToolWithoutInputs, "an expression tool", "", 1
                  "an operation", operationWithoutInputs, "an operation", "", 1
                  "a workflow", workflowWithoutInputs, "a workflow", "", 1
                  "a tool inside a step in map form", step, "a command line tool", "steps.s.run", 8
                  "a tool inside a step in list form", stepList, "a command line tool", "steps.s.run", 8 ]
            for (name, yaml, what, path, line) in documents do
                match (try Decode.decodeCWLProcessingUnit yaml |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal error.Message ("Expected an inputs field, which CWL requires for " + what + ".") (sprintf "%s: the message should say what is missing." name)
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the path should show the place of the process." name)
                    match error.Span with
                    | Some span -> Expect.equal span.Start.Line line (sprintf "%s: the span should start where the process starts." name)
                    | None -> failwith (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s without inputs should fail." name)

        testCase "a doc keeps its final line feeds through a write and a read" <| fun _ ->
            let tool (doc: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ndoc: " + doc + "inputs: []\noutputs: []\n"
            let expression (doc: string) =
                "cwlVersion: v1.2\nclass: ExpressionTool\ndoc: " + doc + "inputs: []\noutputs: []\nexpression: \"${ return {}; }\"\nrequirements:\n  - class: InlineJavascriptRequirement\n"
            // the YAML of the doc and the text that the decoder finds
            let docs =
                [ "|\n  first line\n  second line\n", "first line\nsecond line\n"
                  "|-\n  first line\n  second line\n", "first line\nsecond line"
                  "|+\n  first line\n  second line\n\n", "first line\nsecond line\n\n"
                  "|\n\n  first line\n\n  second line\n\n", "\nfirst line\n\nsecond line\n"
                  "one line\n", "one line" ]
            for (yaml, expected) in docs do
                for (kind, make) in [ "tool", tool; "expression tool", expression ] do
                    let name = sprintf "%s with the doc %s" kind (yaml.Replace("\n", "\\n"))
                    let decoded = Decode.decodeCWLProcessingUnit (make yaml)
                    let docOf unit =
                        match unit with
                        | CommandLineTool tool -> tool.Doc
                        | ExpressionTool expressionTool -> expressionTool.Doc
                        | _ -> None
                    Expect.equal (docOf decoded) (Some expected) (sprintf "%s: the doc is found" name)
                    let encoded = Encode.encodeProcessingUnit decoded
                    let again = Decode.decodeCWLProcessingUnit encoded
                    Expect.equal (docOf again) (Some expected) (sprintf "%s: the doc is the same after a round trip\n%s" name encoded)
                    Expect.equal again decoded (sprintf "%s: the document is the same after a round trip" name)
                    Expect.equal (Encode.encodeProcessingUnit again) encoded (sprintf "%s: a second pass should write the same text" name)

        testCase "requirements and hints with lists are equal to a second decode of the same text and unequal after a change" <| fun _ ->
            let head = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            let software (version: string) =
                "  - class: SoftwareRequirement\n    packages:\n      - package: tool\n        version: [ \"" + version + "\" ]\n        specs: [ https://example.org/tool ]\n"
            let cases =
                [ "software packages with a version and specs", software "1.0", software "2.0"
                  "environment variables",
                  "  - class: EnvVarRequirement\n    envDef:\n      - envName: A\n        envValue: one\n",
                  "  - class: EnvVarRequirement\n    envDef:\n      - envName: A\n        envValue: two\n"
                  "files to create",
                  "  - class: InitialWorkDirRequirement\n    listing:\n      - entryname: a\n        entry: one\n",
                  "  - class: InitialWorkDirRequirement\n    listing:\n      - entryname: a\n        entry: two\n"
                  "schema definitions",
                  "  - class: SchemaDefRequirement\n    types:\n      - name: t\n        type: enum\n        symbols: [a]\n",
                  "  - class: SchemaDefRequirement\n    types:\n      - name: t\n        type: enum\n        symbols: [b]\n"
                  "a library of expressions",
                  "  - class: InlineJavascriptRequirement\n    expressionLib:\n      - \"var a = 1;\"\n",
                  "  - class: InlineJavascriptRequirement\n    expressionLib:\n      - \"var a = 2;\"\n"
                  "container options",
                  "  - class: DockerRequirement\n    dockerPull: image\n    cwltool:dockerRunOptions: [ --one ]\n",
                  "  - class: DockerRequirement\n    dockerPull: image\n    cwltool:dockerRunOptions: [ --two ]\n" ]
            for (name, requirement, changed) in cases do
                for section in [ "requirements"; "hints" ] do
                    let yaml = head + section + ":\n" + requirement
                    let first = Decode.decodeCommandLineTool yaml
                    let second = Decode.decodeCommandLineTool yaml
                    Expect.equal first second (sprintf "%s in %s: the same text decodes to equal tools" name section)
                    Expect.equal (first.GetHashCode()) (second.GetHashCode()) (sprintf "%s in %s: the same text decodes to tools with the same hash" name section)
                    let encoded = Encode.encodeToolDescription first
                    Expect.equal (Decode.decodeCommandLineTool encoded) first (sprintf "%s in %s: the written text decodes to the same tool" name section)
                    let other = Decode.decodeCommandLineTool (head + section + ":\n" + changed)
                    Expect.notEqual first other (sprintf "%s in %s: a changed list makes the tools unequal" name section)

        testCase "requirement values compare their lists by their elements" <| fun _ ->
            let package (version: string) = SoftwarePackage("tool", ResizeArray [ version ], ResizeArray [ "https://example.org/tool" ])
            Expect.equal (SoftwareRequirement (ResizeArray [ package "1.0" ])) (SoftwareRequirement (ResizeArray [ package "1.0" ])) "Equal lists of equal packages are equal."
            Expect.notEqual (SoftwareRequirement (ResizeArray [ package "1.0" ])) (SoftwareRequirement (ResizeArray [ package "2.0" ])) "A different version is a different requirement."
            Expect.notEqual (SoftwareRequirement (ResizeArray [ package "1.0" ])) (SoftwareRequirement (ResizeArray [ package "1.0"; package "1.0" ])) "A different number of packages is a different requirement."
            Expect.notEqual (SoftwareRequirement (ResizeArray [ package "1.0" ])) (EnvVarRequirement (ResizeArray [])) "Another kind of requirement is a different requirement."
            Expect.equal (hash (SoftwareRequirement (ResizeArray [ package "1.0" ]))) (hash (SoftwareRequirement (ResizeArray [ package "1.0" ]))) "Equal requirements have the same hash."
            Expect.equal (SoftwarePackage("tool")) (SoftwarePackage("tool")) "Packages without a version are equal."
            Expect.notEqual (SoftwarePackage("tool")) (SoftwarePackage("tool", ResizeArray [ "1.0" ])) "A missing version differs from a version."
    ]

/// A piece of a workflow document. A spelled piece is a doc or a label that a test writes in different layouts.
type private Piece =
    | Fixed of string
    | Spelled of indent: string * key: string * parts: string list

/// A workflow with a doc or a label at every place that has one. Each text has two or three parts, one for each line
/// of its multi-line layouts. The lines of the pieces are in the order and the form that the encoder writes.
let private layoutPieces =
    [ Fixed "cwlVersion: v1.2"
      Fixed "class: Workflow"
      Spelled("", "label", [ "first part of the workflow name"; "second part of the workflow name" ])
      Spelled("", "doc", [ "first line of the workflow text"; "second line of the workflow text"; "third line of the workflow text" ])
      Fixed ""
      Fixed "inputs:"
      Fixed "  sample:"
      Fixed "    type: string"
      Spelled("    ", "label", [ "first part of the input name"; "second part of the input name" ])
      Spelled("    ", "doc", [ "first line of the input text"; "second line of the input text" ])
      Fixed "  options:"
      Fixed "    type:"
      Fixed "      type: record"
      Fixed "      fields:"
      Fixed "        size:"
      Fixed "          type: int"
      Spelled("          ", "doc", [ "first line of the field text"; "second line of the field text" ])
      Spelled("      ", "label", [ "first part of the record name"; "second part of the record name" ])
      Spelled("      ", "doc", [ "first line of the record text"; "second line of the record text" ])
      Fixed "  choice:"
      Fixed "    type:"
      Fixed "      type: enum"
      Fixed "      symbols: [a, b]"
      Spelled("      ", "doc", [ "first line of the enum text"; "second line of the enum text" ])
      Fixed "  list:"
      Fixed "    type:"
      Fixed "      type: array"
      Fixed "      items:"
      Fixed "        type: enum"
      Fixed "        symbols: [x, y]"
      Spelled("      ", "label", [ "first part of the array name"; "second part of the array name" ])
      Fixed ""
      Fixed "steps:"
      Fixed "  work:"
      Fixed "    run:"
      Fixed "      cwlVersion: v1.2"
      Fixed "      class: CommandLineTool"
      Spelled("      ", "doc", [ "first line of the tool text"; "second line of the tool text" ])
      Fixed "      baseCommand: [echo]"
      Fixed "      inputs: {}"
      Fixed "      outputs: {}"
      Fixed "    in:"
      Fixed "      value:"
      Fixed "        source:"
      Fixed "          - sample"
      Fixed "          - options"
      Spelled("        ", "doc", [ "first line of the step input text"; "second line of the step input text" ])
      Fixed "    out: [done]"
      Spelled("    ", "label", [ "first part of the step name"; "second part of the step name" ])
      Spelled("    ", "doc", [ "first line of the step text"; "second line of the step text" ])
      Fixed ""
      Fixed "outputs:"
      Fixed "  result:"
      Fixed "    type: string"
      Spelled("    ", "label", [ "first part of the output name"; "second part of the output name" ])
      Spelled("    ", "doc", [ "first line of the output text"; "second line of the output text" ])
      Fixed "    outputSource: work/done" ]

/// The ways to write a text of several parts. Every way reads as the parts joined by single spaces.
let private layoutSpellings : (string * (string -> string -> string list -> string list)) list =
    let quoted (opening: string) (closing: string) (lineEnd: string) indent key (parts: string list) =
        let last = List.length parts - 1
        parts
        |> List.mapi (fun index part ->
            (if index = 0 then indent + key + ": " + opening else indent + "  ")
            + part
            + (if index = last then closing else lineEnd))
    [ "on one line", (fun indent key parts -> [ indent + key + ": " + String.concat " " parts ])
      "as plain lines",
      (fun indent key parts -> (indent + key + ": " + List.head parts) :: (List.tail parts |> List.map (fun part -> indent + "  " + part)))
      "as single-quoted lines", quoted "'" "'" ""
      "as double-quoted lines", quoted "\"" "\"" ""
      "as double-quoted lines with escaped breaks", quoted "\"" "\"" " \\"
      "as folded lines", (fun indent key parts -> (indent + key + ": >-") :: (parts |> List.map (fun part -> indent + "  " + part))) ]

let private layoutWorkflow (spell: string -> string -> string list -> string list) : string =
    layoutPieces
    |> List.collect (fun piece ->
        match piece with
        | Fixed line -> [ line ]
        | Spelled(indent, key, parts) -> spell indent key parts)
    |> String.concat "\n"
    |> fun text -> text + "\n"

let private layoutWorkflowOf (text: string) : CWLWorkflowDescription =
    match Decode.decodeCWLProcessingUnit text with
    | Workflow workflow -> workflow
    | other -> failwith (sprintf "Expected a workflow but got %A" other)

/// The objects of a decoded layout workflow that carry a doc or a label, each one with its own equality.
let private layoutObjects (workflow: CWLWorkflowDescription) : (string * obj) list =
    let inputs = workflow.Inputs |> Seq.toList
    let schemas =
        inputs
        |> List.choose (fun input ->
            match input.Type_ with
            | Some (CWLType.Record schema) -> Some ("record schema", box schema)
            | Some (CWLType.Enum schema) -> Some ("enum schema", box schema)
            | Some (CWLType.Array schema) -> Some ("array schema", box schema)
            | _ -> None)
    let fields =
        inputs
        |> List.collect (fun input ->
            match input.Type_ with
            | Some (CWLType.Record schema) -> schema.Fields |> Option.map (Seq.map (fun field -> "record field", box field) >> Seq.toList) |> Option.defaultValue []
            | _ -> [])
    let step = workflow.Steps.[0]
    [ "workflow", box workflow ]
    @ (inputs |> List.map (fun input -> "input", box input))
    @ schemas
    @ fields
    @ (workflow.Outputs |> Seq.map (fun output -> "output", box output) |> Seq.toList)
    @ [ "step", box step ]
    @ (step.In |> Seq.map (fun input -> "step input", box input) |> Seq.toList)
    @ (WorkflowStepRunOps.tryGetTool step.Run |> Option.map (fun tool -> "tool", box tool) |> Option.toList)

/// The ways to write a doc or a label of a tool and of its first input, as a function of the indent and the key.
let private editSources : (string * (string -> string -> string list)) list =
    [ "a folded doc", (fun indent key -> [ indent + key + ": >"; indent + "  first line of the text"; indent + "  second line of the text" ])
      "a literal doc", (fun indent key -> [ indent + key + ": |"; indent + "  first line of the text"; indent + "  second line of the text" ])
      "a plain doc over two lines", (fun indent key -> [ indent + key + ": first part of the text"; indent + "  second part of the text" ])
      "a single-quoted doc over two lines", (fun indent key -> [ indent + key + ": 'first part of the text"; indent + "  second part of the text'" ])
      "a double-quoted doc with an escaped break", (fun indent key -> [ indent + key + ": \"first part of the text \\"; indent + "  second part of the text\"" ])
      "a doc on one line", (fun indent key -> [ indent + key + ": one line of the text" ]) ]

let private editTool (key: string) (lines: string -> string -> string list) : string =
    [ [ "cwlVersion: v1.2"; "class: CommandLineTool" ]
      lines "" key
      [ "baseCommand: [echo]"; "inputs:"; "  x:"; "    type: string" ]
      lines "    " key
      [ "outputs: {}" ] ]
    |> List.concat
    |> String.concat "\n"
    |> fun text -> text + "\n"

let testDocAndLabelLayout =
    testList "Doc and label layout" [
        testCase "a document comes back with the layout of every doc and label as it was written" <| fun _ ->
            for (name, spell) in layoutSpellings do
                let text = layoutWorkflow spell
                let workflow = layoutWorkflowOf text
                let encoded = Encode.encodeProcessingUnit (Workflow workflow)
                Expect.equal encoded text (sprintf "Texts %s should come back as they were written." name)
                Expect.equal (Encode.encodeProcessingUnit (Workflow (layoutWorkflowOf encoded))) encoded (sprintf "Texts %s: a second pass should write the same text." name)
                Expect.equal workflow.Label (Some "first part of the workflow name second part of the workflow name") (sprintf "Texts %s: the label is the parts joined by spaces." name)
                Expect.equal workflow.Doc (Some "first line of the workflow text second line of the workflow text third line of the workflow text") (sprintf "Texts %s: the doc is the parts joined by spaces." name)
                Expect.equal workflow.Inputs.[0].Doc (Some "first line of the input text second line of the input text") (sprintf "Texts %s: the doc of an input is the parts joined by spaces." name)

        testCase "a doc or a label that is not edited comes back in the spelling of its source" <| fun _ ->
            let cases =
                [ "a folded doc over three lines",
                  "doc: >\n  first line of the text\n  second line of the text\n  third line of the text\n",
                  (fun (tool: CWLToolDescription) -> tool.Doc),
                  "first line of the text second line of the text third line of the text\n"
                  "a plain label over three lines",
                  "label: first part of the name\n  second part of the name\n  third part of the name\n",
                  (fun (tool: CWLToolDescription) -> tool.Label),
                  "first part of the name second part of the name third part of the name"
                  "a double-quoted doc with an escaped break",
                  "doc: \"first part of the text \\\n  second part of the text\"\n",
                  (fun (tool: CWLToolDescription) -> tool.Doc),
                  "first part of the text second part of the text"
                  "a doc under !!str that reads as a bool without the tag",
                  "doc: !!str true\n",
                  (fun (tool: CWLToolDescription) -> tool.Doc),
                  "true"
                  "a double-quoted label on one line",
                  "label: \"just one line of the name\"\n",
                  (fun (tool: CWLToolDescription) -> tool.Label),
                  "just one line of the name" ]
            for (name, lines, read, expected) in cases do
                let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\n" + lines + "baseCommand: [echo]\ninputs: {}\noutputs: {}\n"
                let decoded = Decode.decodeCommandLineTool yaml
                Expect.equal (read decoded) (Some expected) (sprintf "%s: the text is found." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded lines (sprintf "%s: the lines come back as written.\n%s" name encoded)

        testCase "the label and the doc of an expression tool and of an operation come back as written" <| fun _ ->
            let texts = "label: first part of the name\n  second part of the name\ndoc: >\n  first line of the text\n  second line of the text\n"
            let documents =
                [ "an expression tool",
                  "cwlVersion: v1.2\nclass: ExpressionTool\n" + texts + "inputs: {}\noutputs: {}\nexpression: \"${ return {}; }\"\nrequirements:\n  - class: InlineJavascriptRequirement\n"
                  "an operation", "cwlVersion: v1.2\nclass: Operation\n" + texts + "inputs: {}\noutputs: {}\n" ]
            for (name, yaml) in documents do
                let encoded = Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit yaml)
                Expect.stringContains encoded texts (sprintf "%s: the label and the doc come back as written.\n%s" name encoded)

        testCase "documents that differ only in the layout of their texts are equal and have the same hash" <| fun _ ->
            let decoded = layoutSpellings |> List.map (fun (name, spell) -> name, layoutWorkflowOf (layoutWorkflow spell))
            for (name, workflow) in decoded do
                for (otherName, other) in decoded do
                    let first = layoutObjects workflow
                    let second = layoutObjects other
                    Expect.equal first.Length second.Length "The same objects should carry a doc or a label."
                    List.iter2
                        (fun (kind: string, a: obj) (_: string, b: obj) ->
                            Expect.equal a b (sprintf "Texts %s and %s: the %s objects should be equal." name otherName kind)
                            Expect.equal (a.GetHashCode()) (b.GetHashCode()) (sprintf "Texts %s and %s: the %s objects should have the same hash." name otherName kind))
                        first
                        second
            let text = layoutWorkflow (snd (List.last layoutSpellings))
            let changed = layoutWorkflowOf (text.Replace("first line of the tool text", "first line of the other text"))
            let original = layoutWorkflowOf text
            Expect.notEqual (List.zip (layoutObjects original) (layoutObjects changed) |> List.filter (fun ((_, a), (_, b)) -> a <> b) |> List.length) 0 "A changed text should still make an object unequal."

        testCase "an edited doc or label is written for the new text and reads back exactly" <| fun _ ->
            let edits =
                [ ""; "short"; "true"; "null"; "42"; " leading blank"; "trailing blank "; "key: value # comment"; "- item"
                  "two\nlines"; "two\nlines\n"; "\ntext after a blank line"; "tab\there"; "café über"; "line one\r\nline two" ]
            for (name, lines) in editSources do
                for edit in edits do
                    // The doc of a tool reads its line breaks as line feeds. The other texts keep the characters that they have.
                    let toolDoc = edit.Replace("\r\n", "\n")
                    let docTool = Decode.decodeCommandLineTool (editTool "doc" lines)
                    docTool.Doc <- Some edit
                    docTool.Inputs.Value.[0].Doc <- Some edit
                    let encodedDoc = Encode.encodeToolDescription docTool
                    let againDoc = Decode.decodeCommandLineTool encodedDoc
                    Expect.equal againDoc.Doc (Some toolDoc) (sprintf "%s edited to %A: the doc of the tool reads back.\n%s" name edit encodedDoc)
                    Expect.equal againDoc.Inputs.Value.[0].Doc (Some edit) (sprintf "%s edited to %A: the doc of the input reads back.\n%s" name edit encodedDoc)
                    Expect.isFalse (encodedDoc.Contains "of the text") (sprintf "%s edited to %A: the old text is gone." name edit)
                    let labelTool = Decode.decodeCommandLineTool (editTool "label" lines)
                    labelTool.Label <- Some edit
                    labelTool.Inputs.Value.[0].Label <- Some edit
                    let encodedLabel = Encode.encodeToolDescription labelTool
                    let againLabel = Decode.decodeCommandLineTool encodedLabel
                    Expect.equal againLabel.Label (Some edit) (sprintf "%s edited to %A: the label of the tool reads back.\n%s" name edit encodedLabel)
                    Expect.equal againLabel.Inputs.Value.[0].Label (Some edit) (sprintf "%s edited to %A: the label of the input reads back.\n%s" name edit encodedLabel)

        testCase "a doc or a label that is a plain bool, int or float is a decode error at the field and a string spelling is not" <| fun _ ->
            let tool (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            for key in [ "doc"; "label" ] do
                for text in [ "true"; "42"; "1.5"; ".inf"; "0x1F" ] do
                    let cases =
                        [ key, tool [ key + ": " + text; "inputs: {}"; "outputs: {}" ]
                          "inputs.x." + key, tool [ "inputs:"; "  x:"; "    type: string"; "    " + key + ": " + text; "outputs: {}" ]
                          "outputs.o." + key, tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    " + key + ": " + text ] ]
                    for (path, yaml) in cases do
                        match decodeError yaml with
                        | Some error ->
                            Expect.equal (DecodeError.pathText error) path (sprintf "The error for %s should show the field:\n%s" text yaml)
                            Expect.stringContains error.Message "Expected a string" (sprintf "The error for %s should say what CWL expects." text)
                        | None -> failwith (sprintf "A plain %s as %s must fail:\n%s" text key yaml)
                for text in [ "\"42\""; "'true'"; "!!str 1.5"; "|\n  42" ] do
                    let decoded = Decode.decodeCommandLineTool (tool [ key + ": " + text; "inputs: {}"; "outputs: {}" ])
                    let value = if key = "doc" then decoded.Doc else decoded.Label
                    Expect.isSome value (sprintf "The spelling %s is a string." (text.Replace("\n", "\\n")))
            let step = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  s:
    run: t.cwl
    in: {}
    out: []
    label: 42
"""
            match (try Decode.decodeWorkflow step |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error -> Expect.equal (DecodeError.pathText error) "steps.s.label" "The error should show the label of the step."
            | None -> failwith "A plain int as the label of a step must fail."

        testCase "an edit of a plain string doc or label to a bool, an int or null reads back as the same string and keeps the comment, the anchor and the tag" <| fun _ ->
            let tool (field: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n" + field + "inputs: {}\noutputs: {}\n"
            let sources =
                [ "a plain label with a comment", "label: abc # keep me\n", "# keep me"
                  "a plain label with an anchor", "label: &name abc\n", "&name"
                  "a plain label with a tag", "label: !!str abc\n", "!!str"
                  "a quoted label with a comment", "label: \"a name\" # keep me\n", "# keep me"
                  "a literal doc with a header comment", "doc: | # keep me\n  some text\n", "# keep me" ]
            for (name, field, kept) in sources do
                for edit in [ "true"; "42"; "1.5"; "null" ] do
                    let decoded = Decode.decodeCommandLineTool (tool field)
                    if field.StartsWith "doc" then decoded.Doc <- Some edit else decoded.Label <- Some edit
                    let encoded = Encode.encodeToolDescription decoded
                    let again = Decode.decodeCommandLineTool encoded
                    Expect.equal (if field.StartsWith "doc" then again.Doc else again.Label) (Some edit) (sprintf "%s edited to %s: the text reads back as the string.\n%s" name edit encoded)
                    Expect.stringContains encoded kept (sprintf "%s edited to %s: %s stays.\n%s" name edit kept encoded)

        testCase "a doc that is a list of strings reads as their concatenation, comes back unchanged and is a string after an edit" <| fun _ ->
            let tool (field: string) = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: [echo]\n" + field + "inputs:\n  x:\n    type: string\n    doc: [one, two]\noutputs: {}\n"
            let lists =
                [ "a flow list", "doc: [first part, second part]\n"
                  "a block list", "doc:\n  - first part\n  - second part\n"
                  "a block list with a comment", "doc:\n  - first part # note\n  - second part\n" ]
            for (name, lines) in lists do
                let decoded = Decode.decodeCommandLineTool (tool lines)
                Expect.equal decoded.Doc (Some "first partsecond part") (sprintf "%s: the doc is the items joined without a separator." name)
                Expect.equal decoded.Inputs.Value.[0].Doc (Some "onetwo") (sprintf "%s: the doc of an input is a list too." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded lines (sprintf "%s: the list comes back as written.\n%s" name encoded)
                Expect.stringContains encoded "    doc: [one, two]\n" (sprintf "%s: the list of the input comes back as written.\n%s" name encoded)
                Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: a second pass should write the same text." name)
                // the same text as a string and the list are equal, and the same text set again keeps the list
                Expect.equal decoded (Decode.decodeCommandLineTool (tool "doc: first partsecond part\n")) (sprintf "%s: a list and its concatenation are equal." name)
                decoded.Doc <- Some "first partsecond part"
                Expect.stringContains (Encode.encodeToolDescription decoded) lines (sprintf "%s: the same text keeps the list." name)
                // an edit writes a string
                decoded.Doc <- Some "one text"
                decoded.Inputs.Value.[0].Doc <- Some "another text"
                let edited = Encode.encodeToolDescription decoded
                Expect.stringContains edited "doc: one text\n" (sprintf "%s: an edited doc is a string.\n%s" name edited)
                Expect.stringContains edited "doc: another text\n" (sprintf "%s: an edited doc of an input is a string.\n%s" name edited)
                Expect.isFalse (edited.Contains "first part" || edited.Contains "[one, two]") (sprintf "%s: the list is gone.\n%s" name edited)
                Expect.equal (Decode.decodeCommandLineTool edited).Doc (Some "one text") (sprintf "%s: the edited doc reads back." name)

        testCase "the label is a string only and the items of a doc list are strings" <| fun _ ->
            let tool (field: string) = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: [echo]\n" + field + "inputs: {}\noutputs: {}\n"
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            for (field, path, message) in [ "label: [a, b]\n", "label", "Expected a string"; "doc: [a, 42]\n", "doc[1]", "Expected a string, found the plain scalar 42"; "doc: [a, true]\n", "doc[1]", "a bool"; "doc: [a, {b: c}]\n", "doc[1]", "Expected a string"; "doc:\n  - a\n  -\n", "doc[1]", "found null" ] do
                match decodeError (tool field) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the field." field)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what CWL expects." field)
                | None -> failwith (sprintf "%s must fail" field)
            Expect.equal (Decode.decodeCommandLineTool (tool "doc: [\"42\", 'true']\n")).Doc (Some "42true") "Quoted items are strings."

        testCase "an edit that keeps the length of every line keeps the lines and a longer edit writes the new text" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
doc: >
  first line of the text
  second line of the text
baseCommand: [echo]
inputs: {}
outputs: {}
"""
            let tool = Decode.decodeCommandLineTool yaml
            tool.Doc <- Some "other line of the text second line of the text\n"
            let sameLengths = Encode.encodeToolDescription tool
            Expect.stringContains sameLengths "doc: >\n  other line of the text\n  second line of the text\n" "The lines of the author should stay for a text with lines of the same lengths."
            tool.Doc <- Some "first line of the text, now a longer one second line of the text\n"
            let longer = Encode.encodeToolDescription tool
            Expect.equal (Decode.decodeCommandLineTool longer).Doc tool.Doc "The longer text should read back exactly."
            Expect.isFalse (longer.Contains "first line of the text\n") "The old line should be gone."

        testCase "a tool that is built from the texts of a decoded one writes them in the plain spelling" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
label: first part of the name
  second part of the name
doc: >
  first line of the text
  second line of the text
baseCommand: [echo]
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            let rebuilt =
                CWLToolDescription(
                    ResizeArray<CWLOutput>(),
                    baseCommand = ResizeArray [ "echo" ],
                    inputs = ResizeArray<CWLInput>(),
                    ?label = decoded.Label,
                    ?doc = decoded.Doc
                )
            let encoded = Encode.encodeToolDescription rebuilt
            Expect.stringContains encoded "label: first part of the name second part of the name\n" "The label has no layout of the author."
            Expect.stringContains encoded "doc: |\n  first line of the text second line of the text\n" "The doc has no layout of the author."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The tools are equal whatever their layouts."

        testCase "the nodes of the texts stay out of the dynamic properties and out of the written document" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (editTool "doc" (snd editSources.[0]))
            Expect.isNone decoded.Metadata "The nodes are not metadata."
            Expect.equal (decoded.GetProperties(false) |> Seq.length) 0 "The nodes are not dynamic properties."
            Expect.equal (decoded.Inputs.Value.[0].GetProperties(false) |> Seq.length) 0 "The nodes are not dynamic properties of an input."
            let encoded = Encode.encodeToolDescription decoded
            Expect.isFalse (encoded.ToLowerInvariant().Contains "sourcenodes") "The nodes are not written."
    ]

/// Two objects of the CWL model are the same when they are equal and their hashes agree.
let private expectSame (name: string) (left: obj) (right: obj) =
    Expect.equal left right (sprintf "%s: the objects should be equal." name)
    Expect.equal (left.GetHashCode()) (right.GetHashCode()) (sprintf "%s: equal objects should have the same hash." name)

/// A workflow with one step. Its step inputs hold a single source, a list of sources with every other field of a step
/// input, and a source with a default value.
let private stepInputWorkflow (sources: string) (linkMerge: string) (pickValue: string) (valueFrom: string) : string =
    String.concat "\n" [
        "cwlVersion: v1.2"
        "class: Workflow"
        "inputs:"
        "  a: string"
        "  b: string"
        "  c: string"
        "outputs: {}"
        "steps:"
        "  first:"
        "    run: tool.cwl"
        "    in:"
        "      single: a"
        "      list:"
        "        source: " + sources
        "        linkMerge: " + linkMerge
        "        pickValue: " + pickValue
        "        valueFrom: " + valueFrom
        "      withDefault:"
        "        source: b"
        "        default: 1"
        "    out: [done]"
        "" ]

let private stepOf (text: string) : WorkflowStep =
    match Decode.decodeCWLProcessingUnit text with
    | Workflow workflow -> workflow.Steps.[0]
    | other -> failwith (sprintf "Expected a workflow but got %A" other)

let private stepInputsOf (text: string) : StepInput list = (stepOf text).In |> Seq.toList

let testEqualityOfDecodedObjects =
    testList "Equality of decoded objects" [
        testCase "two decodes of a workflow are equal in the step and in every step input" <| fun _ ->
            let text = stepInputWorkflow "[a, b]" "merge_flattened" "first_non_null" "$(self)"
            let first = Decode.decodeCWLProcessingUnit text
            let second = Decode.decodeCWLProcessingUnit text
            expectSame "the workflow" (box first) (box second)
            let firstInputs = stepInputsOf text
            let secondInputs = stepInputsOf text
            Expect.equal firstInputs.Length 3 "The step has three inputs."
            List.iter2 (fun (a: StepInput) (b: StepInput) -> expectSame (sprintf "the step input %s" a.Id) (box a) (box b)) firstInputs secondInputs
            Expect.equal (firstInputs.[0].Source |> Option.map Seq.toList) (Some [ "a" ]) "A single source is a list of one."
            Expect.equal (firstInputs.[1].Source |> Option.map Seq.toList) (Some [ "a"; "b" ]) "A list of sources keeps its order."
            expectSame "the step" (box (stepOf text)) (box (stepOf text))

        testCase "a changed source, list order, link merge, pick value or value expression makes a step input unequal" <| fun _ ->
            let baseText = stepInputWorkflow "[a, b]" "merge_flattened" "first_non_null" "$(self)"
            let baseInput = (stepInputsOf baseText).[1]
            let variants =
                [ "another source in the list", stepInputWorkflow "[a, c]" "merge_flattened" "first_non_null" "$(self)"
                  "the sources in another order", stepInputWorkflow "[b, a]" "merge_flattened" "first_non_null" "$(self)"
                  "one more source", stepInputWorkflow "[a, b, c]" "merge_flattened" "first_non_null" "$(self)"
                  "one source less", stepInputWorkflow "[a]" "merge_flattened" "first_non_null" "$(self)"
                  "another link merge", stepInputWorkflow "[a, b]" "merge_nested" "first_non_null" "$(self)"
                  "another pick value", stepInputWorkflow "[a, b]" "merge_flattened" "all_non_null" "$(self)"
                  "another value expression", stepInputWorkflow "[a, b]" "merge_flattened" "first_non_null" "$(self.x)" ]
            for (name, text) in variants do
                Expect.notEqual (stepInputsOf text).[1] baseInput (sprintf "%s: the step inputs should differ." name)
            let single = (stepInputsOf baseText).[0]
            let otherSingle = (stepInputsOf (baseText.Replace("single: a", "single: c"))).[0]
            Expect.notEqual otherSingle single "Another single source should make the step inputs unequal."
            Expect.equal (stepInputsOf baseText).[1] baseInput "A second decode of the same text is equal."

        testCase "outputs with several sources are equal to a second decode and differ when a source changes" <| fun _ ->
            let outputs (sources: string) =
                "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs:\n  many:\n    type: File[]\n    outputSource: " + sources + "\n  one:\n    type: File\n    outputSource: a/x\nsteps: {}\n"
            let outputsOf (text: string) =
                match Decode.decodeCWLProcessingUnit text with
                | Workflow workflow -> workflow.Outputs |> Seq.toList
                | other -> failwith (sprintf "Expected a workflow but got %A" other)
            let text = outputs "[a/x, b/y]"
            List.iter2 (fun (a: CWLOutput) (b: CWLOutput) -> expectSame (sprintf "the output %s" a.Name) (box a) (box b)) (outputsOf text) (outputsOf text)
            expectSame "multiple output sources" (box (OutputSource.Multiple (ResizeArray [ "a"; "b" ]))) (box (OutputSource.Multiple (ResizeArray [ "a"; "b" ])))
            expectSame "a single output source" (box (OutputSource.Single "a")) (box (OutputSource.Single "a"))
            Expect.notEqual (OutputSource.Multiple (ResizeArray [ "a"; "b" ])) (OutputSource.Multiple (ResizeArray [ "b"; "a" ])) "Another order is another list of sources."
            Expect.notEqual (OutputSource.Multiple (ResizeArray [ "a"; "b" ])) (OutputSource.Multiple (ResizeArray [ "a" ])) "Another number of sources is another list."
            Expect.notEqual (OutputSource.Single "a") (OutputSource.Multiple (ResizeArray [ "a" ])) "A single source stays different from a list of one source."
            Expect.notEqual (outputsOf text).[0] (outputsOf (outputs "[a/x, c/z]")).[0] "A changed source makes the outputs unequal."

        testCase "lists of any element type in dynamic properties compare and hash by their elements" <| fun _ ->
            let input (value: obj) =
                let input = CWLInput("x", CWLType.String)
                DynObj.setProperty "ext" value input
                input
            let strings (items: string list) = box (ResizeArray items)
            let objects (items: obj list) = box (ResizeArray<obj> items)
            let cases =
                [ "a list of strings", strings [ "a"; "b" ], strings [ "a"; "b" ], strings [ "a"; "c" ]
                  "a list of strings in another order", strings [ "a"; "b" ], strings [ "a"; "b" ], strings [ "b"; "a" ]
                  "a shorter list of strings", strings [ "a"; "b" ], strings [ "a"; "b" ], strings [ "a" ]
                  "a list of objects", objects [ box "a"; box 1 ], objects [ box "a"; box 1 ], objects [ box "a"; box 2 ]
                  "lists in a list", objects [ strings [ "a" ]; objects [ box "b" ] ], objects [ strings [ "a" ]; objects [ box "b" ] ], objects [ strings [ "a" ]; objects [ box "c" ] ] ]
            for (name, value, same, changed) in cases do
                expectSame name (box (input value)) (box (input same))
                Expect.notEqual (input value) (input changed) (sprintf "%s: a changed element makes the objects unequal." name)
            Expect.notEqual (input (strings [ "a" ])) (input (box "a")) "A list differs from a string."

        testCase "lists in extension fields compare by their elements" <| fun _ ->
            let tool (nested: string) =
                String.concat "\n" [
                    "cwlVersion: v1.2"
                    "class: CommandLineTool"
                    "baseCommand: echo"
                    "inputs:"
                    "  x:"
                    "    type: string"
                    "    inputBinding:"
                    "      position: 1"
                    "      ext-binding: [p, q]"
                    "    ext-list: [a, b]"
                    "    ext-nested: " + nested
                    "    ext-map:"
                    "      k: [v, w]"
                    "outputs: {}"
                    "ext-top: [p, q]"
                    "" ]
            let decode (text: string) = Decode.decodeCommandLineTool text
            let text = tool "[[1, 2], [3]]"
            let first = decode text
            let second = decode text
            expectSame "the tool" (box first) (box second)
            expectSame "the input" (box first.Inputs.Value.[0]) (box second.Inputs.Value.[0])
            expectSame "the input binding" (box first.Inputs.Value.[0].InputBinding.Value) (box second.Inputs.Value.[0].InputBinding.Value)
            let changed = decode (tool "[[1, 2], [4]]")
            Expect.notEqual changed.Inputs.Value.[0] first.Inputs.Value.[0] "A changed element in a nested list makes the inputs unequal."
            Expect.notEqual (decode (tool "[[1, 2]]")).Inputs.Value.[0] first.Inputs.Value.[0] "A shorter nested list makes the inputs unequal."
            Expect.notEqual (decode (text.Replace("ext-list: [a, b]", "ext-list: [b, a]"))).Inputs.Value.[0] first.Inputs.Value.[0] "Another order of the elements makes the inputs unequal."
    ]

let testPortSchemaNesting =
    let tool (portLines: string list) =
        "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"
    let enumSchema = Enum (InputEnumSchema(ResizeArray [ "fast"; "slow" ]))

    testList "Port schema nesting" [
        testCase "the schema of a port with other fields stands directly under type" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.PortSchemas.cwlFile
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  mode:\n    type:\n      type: enum\n      symbols: [fast, slow]\n    inputBinding:\n      prefix: --mode\n" "The enum input keeps its schema under type and its binding beside it."
            Expect.stringContains encoded "  summary:\n    type:\n      type: record\n      fields:\n        count:\n          type: int\n    doc: the summary\n" "The record output keeps its schema under type and its doc beside it."
            Expect.stringContains encoded "  samples:\n    type:\n      type: array\n      items:\n        type: record\n" "The array of records keeps its schema under type."
            Expect.isFalse (encoded.Contains "type:\n      type:\n        type:") "No port writes a schema in a second mapping under type."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
        testCase "a port with a schema and no other field writes the schema under type" <| fun _ ->
            let hand =
                CWLToolDescription(
                    ResizeArray [ CWLOutput("record", Record (InputRecordSchema(fields = ResizeArray [ InputRecordField("a", CWLType.String) ]))) ],
                    inputs = ResizeArray [ CWLInput("mode", enumSchema) ],
                    baseCommand = ResizeArray [ "echo" ]
                )
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "  mode:\n    type:\n      type: enum\n      symbols: [fast, slow]\n" "The enum input is a port object with the schema under type."
            Expect.stringContains encoded "  record:\n    type:\n      type: record\n      fields:\n        a:\n          type: string\n" "The record output is a port object with the schema under type."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The hand-built tool should read back as it was built."
        testCase "a hand-built enum input with a binding reads back equal" <| fun _ ->
            let hand =
                CWLToolDescription(
                    ResizeArray [],
                    inputs = ResizeArray [ CWLInput("mode", enumSchema, inputBinding = InputBinding.create(prefix = "--mode")) ],
                    baseCommand = ResizeArray [ "echo" ]
                )
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "    type:\n      type: enum\n" "The schema stands under type."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The written text should read back as the same tool."
        testCase "a schema under a schema under type is a decode error" <| fun _ ->
            let yaml = tool [ "  mode:"; "    type:"; "      type:"; "        type: enum"; "        symbols: [fast, slow]"; "    inputBinding:"; "      prefix: --mode" ]
            match (try Decode.decodeCommandLineTool yaml |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "inputs.mode.type.type" "The error should show the misplaced level."
                Expect.stringContains error.Message "Expected record, enum or array in the type field of a schema" "The error should say what a schema has under type."
                Expect.isSome error.Span "The error should have the place in the text."
            | None -> failwith "A schema in the type of a schema must fail."
        testCase "a type list under the type of a schema is a decode error" <| fun _ ->
            let yaml = tool [ "  x:"; "    type:"; "      type: [string, int]" ]
            match (try Decode.decodeCommandLineTool yaml |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error -> Expect.equal (DecodeError.pathText error) "inputs.x.type.type" "The error should show the misplaced level."
            | None -> failwith "A type list in the type of a schema must fail."
    ]

let testArrayShorthand =
    let inputOf (name: string) (tool: CWLToolDescription) = tool.Inputs.Value |> Seq.find (fun input -> input.Name = name)

    testList "Array shorthand" [
        testCase "an array with an input binding keeps its binding and the binding of its port" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.ArraySchemas.withBinding
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "    type:\n      type: array\n      items: File\n      inputBinding:\n        prefix: -I\n    inputBinding:\n      position: 1\n" "The array stays a schema with its binding, and the port keeps its own."
            Expect.isFalse (encoded.Contains "File[]") "The shorthand would drop the binding of the array."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
        testCase "an array with a label, a doc or a name is written as a schema, as a type or in a type list" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.ArraySchemas.withMetadata
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  names:\n    - \"null\"\n    - type: array\n      items: string\n      label: names\n      inputBinding:\n        itemSeparator: \",\"\n" "The optional array with a label keeps its schema."
            Expect.stringContains encoded "  counts:\n    type:\n      type: array\n      items: int\n      doc: the counts\n" "The array with a doc keeps its schema."
            Expect.stringContains encoded "  tags:\n    - string\n    - type: array\n      items: string\n      name: Tags\n" "The array with a name keeps its schema in a type list."
            Expect.stringContains encoded "  plain: string[]\n" "A plain array stays the shorthand."
            Expect.stringContains encoded "  maybe: int[]?\n" "A plain optional array stays the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
        testCase "the schema of an array keeps the fields that the shorthand has no place for" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.ArraySchemas.withMetadata
            match (inputOf "names" decoded).Type_ with
            | Some (Union members) ->
                match members.[1] with
                | Array schema ->
                    Expect.equal schema.Label (Some "names") "The label is typed."
                    Expect.isTrue (schema.TryGetPropertyValue "inputBinding" |> Option.isSome) "The binding stays on the schema."
                | other -> failwithf "Expected an array schema, found %A" other
            | other -> failwithf "Expected a type list, found %A" other
        testCase "a hand-built array with a label is not written as the shorthand" <| fun _ ->
            let labelled = InputArraySchema(CWLType.String, label = "names")
            let hand =
                CWLToolDescription(
                    ResizeArray [],
                    inputs = ResizeArray [ CWLInput("a", Array labelled); CWLInput("b", Union (ResizeArray [ Null; Array labelled ]), optional = true); CWLInput("c", Array (InputArraySchema(CWLType.String))) ],
                    baseCommand = ResizeArray [ "echo" ]
                )
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "  a:\n    type:\n      type: array\n      items: string\n      label: names\n" "The array as the whole type keeps its label."
            Expect.stringContains encoded "  b:\n    - \"null\"\n    - type: array\n      items: string\n      label: names\n" "The optional array keeps its label."
            Expect.stringContains encoded "  c: string[]\n" "The plain array is the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The hand-built tool should read back as it was built."
        testCase "the text of an additional type needs a schema for an array with a label" <| fun _ ->
            let labelled = Array (InputArraySchema(CWLType.File, label = "files"))
            Expect.isTrue (Encode.isComplexType labelled) "An array with a label has no shorthand."
            Expect.isTrue (Encode.isComplexType (Union (ResizeArray [ Null; labelled ]))) "An optional array with a label has no shorthand."
            Expect.isFalse (Encode.isComplexType (Array (InputArraySchema(CWLType.File)))) "A plain array has the shorthand."
            Expect.equal ((Encode.encodeCWLTypeShorthand (Array (InputArraySchema(Array (InputArraySchema(CWLType.File))))) |> Encode.writeYaml).Trim()) "File[][]" "A plain array of arrays has the nested shorthand."
    ]

let testFileAndDirectoryTypes =
    let tool (portLines: string list) =
        "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"

    testList "File and Directory types" [
        testCase "the format and the secondaryFiles of an optional File stay on the port and the type round trips equal" <| fun _ ->
            let yaml = tool [ "  reads:"; "    type: File?"; "    format: edam:format_2330"; "    secondaryFiles: .bai"; "  dir: Directory?" ]
            let decoded = Decode.decodeCommandLineTool yaml
            let reads = decoded.Inputs.Value.[0]
            Expect.equal reads.Type_ (Some (Union (ResizeArray [ Null; CWLType.File ]))) "The type is an optional File."
            Expect.equal reads.Format (Some (InputFormat.Single "edam:format_2330")) "The format belongs to the port."
            Expect.isSome reads.SecondaryFiles "The secondaryFiles belong to the port."
            Expect.equal decoded.Inputs.Value.[1].Type_ (Some (Union (ResizeArray [ Null; CWLType.Directory ]))) "The type is an optional Directory."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  reads:\n    type: File?\n    secondaryFiles: .bai\n    format: edam:format_2330\n  dir: Directory?\n" "The port keeps its fields and the types are the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
        testCase "a File and a Directory are equal to themselves and to nothing else" <| fun _ ->
            Expect.equal CWLType.File CWLType.File "A File equals a File."
            Expect.equal CWLType.Directory CWLType.Directory "A Directory equals a Directory."
            Expect.notEqual CWLType.File CWLType.Directory "A File is no Directory."
            Expect.equal (hash CWLType.File) (hash CWLType.File) "Equal types hash alike."
        testCase "the type of a File value is File" <| fun _ ->
            Expect.equal (CWLParameterValue.tryInferType (CWLParameterValue.File (FileInstance(path = "a.txt")))) (Some CWLType.File) "A File value has the type File."
            Expect.equal (CWLParameterValue.tryInferType (CWLParameterValue.Directory (DirectoryInstance(path = "d")))) (Some CWLType.Directory) "A Directory value has the type Directory."
    ]

let testDirentIsNoType =
    let tool (inputLines: string list) (outputLines: string list) =
        "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" inputLines + "\noutputs:\n" + String.concat "\n" outputLines + "\n"

    testList "Dirent is no type" [
        testCase "Dirent as a type name is an invalid CWL type at its place" <| fun _ ->
            let cases =
                [ "an input", "inputs.x", tool [ "  x: Dirent" ] [ "  o: string" ]
                  "an array of Dirent", "inputs.x", tool [ "  x: Dirent[]" ] [ "  o: string" ]
                  "an optional Dirent", "inputs.x", tool [ "  x: Dirent?" ] [ "  o: string" ]
                  "the type field of an input", "inputs.x.type", tool [ "  x:"; "    type: Dirent"; "    listing: []" ] [ "  o: string" ]
                  "a member of a type list", "inputs.x[1]", tool [ "  x: [string, Dirent]" ] [ "  o: string" ]
                  "the items of an array", "inputs.x.type.items", tool [ "  x:"; "    type:"; "      type: array"; "      items: Dirent" ] [ "  o: string" ]
                  "an output", "outputs.o", tool [ "  x: string" ] [ "  o: Dirent" ] ]
            for (name, path, yaml) in cases do
                match (try Decode.decodeCommandLineTool yaml |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place of the type" name)
                    Expect.equal error.Message "Invalid CWL type: Dirent" (sprintf "%s: the error should name the type" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "a Dirent stays an entry of the listing of an InitialWorkDirRequirement" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: a.txt
        entry: hello
inputs: {}
outputs: {}
"""
            let decoded = Decode.decodeCommandLineTool yaml
            match decoded.Requirements.Value.[0] with
            | InitialWorkDirRequirement listing ->
                Expect.equal listing.[0] (DirentEntry (DirentInstance(SchemaSaladString.Literal "hello", entryname = SchemaSaladString.Literal "a.txt"))) "The entry is a Dirent."
            | other -> failwithf "Expected an InitialWorkDirRequirement, found %A" other
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded "The written text should decode to the same tool."
    ]

let main =
    testList "CWLToolDescription" [
        testCWLToolDescriptionDecode
        testCWLToolDescriptionEncode
        testEncodeNormalizeEdgeCases
        testCWLToolDescriptionMetadata
        testNestedArrayDecoding
        testUnionArrayItemDecoding
        testStringScalars
        testToolUnionPorts
        testAnyType
        testStreamTypes
        testYamlReaderAndWriter
        testDocAndLabelLayout
        testEqualityOfDecodedObjects
        testExpressionTool
        testOperation
        testTopLevelDirectDynamicOverflow
        testPortSchemaNesting
        testArrayShorthand
        testFileAndDirectoryTypes
        testDirentIsNoType
    ]

