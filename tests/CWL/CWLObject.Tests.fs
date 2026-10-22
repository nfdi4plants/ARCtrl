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
                    let expected = File (FileInstance())
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
                    let expected = Directory (DirectoryInstance())
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
                    let expected = File (FileInstance())
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
                    let expected = File (FileInstance())
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
                    let expected = Directory (DirectoryInstance())
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
                    let expected = File (FileInstance())
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
        Array (InputArraySchema(Union (ResizeArray [String; File (FileInstance())])))

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
            Expect.equal output.Type_ (Some (Directory (DirectoryInstance()))) "pool_DIR output type should remain Directory."
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x:
    type: string
    default: &a value
  y:
    type: string
    default: *a
outputs: {}
"""
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
arc:quoted: "true"
arc:plain: true
arc:number string: '12'
arc:number: 12
arc:null: null
arc:tilde: ~
arc:capital bool: True
arc:hex: 0x1F
arc:word: NaN
arc:whole float: 2.0
"""
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
                Record (InputRecordSchema(fields = ResizeArray [ InputRecordField("null", CWLType.String); InputRecordField("ok", CWLType.file ()) ]))
            let recordText = Encode.cwlTypeToYamlString recordType
            Expect.stringContains recordText "{name: \"null\", type: string}" "A field named null needs quotes."
            Expect.stringContains recordText "{name: ok, type: File}" "An ordinary field name stays plain."
            Expect.isTrue (CWLType.typesEqual recordType (recordText |> YAMLicious.Reader.readRoot |> Decode.cwlTypeDecoder')) "The flow text should decode to the same record."

        testCase "mapping keys that look like another type are quoted" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  "1":
    type: string
  "true":
    type: int
  plain:
    type: File
outputs:
  "null":
    type: File
"""
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool yaml)
            Expect.stringContains encoded "\"1\": string" "A key that reads as a number needs quotes."
            Expect.stringContains encoded "\"true\": int" "A key that reads as a bool needs quotes."
            Expect.stringContains encoded "\"null\": File" "A key that reads as null needs quotes."
            Expect.stringContains encoded "plain: File" "An ordinary key stays plain."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.sequenceEqual (roundTripped.Inputs.Value |> Seq.map (fun i -> i.Name)) [ "1"; "true"; "plain" ] "The input names should survive."

        testCase "the null type is written as a quoted string" <| fun _ ->
            Expect.equal (Encode.cwlTypeToYamlString Null) "\"null\"" "A plain null would read as a YAML null."
            Expect.equal (Encode.cwlTypeToYamlString (Union (ResizeArray [ Null; CWLType.file () ]))) "[\"null\", File]" "The null member of a union must be quoted."
    ]

let testToolUnionPorts =
    let tool = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  mode_choice:
    type:
      - "null"
      - type: enum
        symbols: ["alpha", "beta"]
    label: Mode choice
  optional_dir:
    type: Directory?
  flow_union: [null, File]
  block_union:
    - string
    - int
  array_or_null:
    type:
      - "null"
      - type: array
        items: [string, File]
outputs:
  result:
    type: [File, Directory]
    outputBinding:
      glob: "*.out"
  maybe: ["null", File]
"""

    let assertTypesEqual (expected: CWLType) (actual: CWLType) (message: string) =
        Expect.isTrue (CWLType.typesEqual expected actual) $"{message}: expected %A{expected} but got %A{actual}"

    let check (decoded: CWLToolDescription) =
        let inputs = Expect.wantSome decoded.Inputs "Inputs should decode."
        let typeOf name = (Expect.wantSome (inputs |> Seq.find (fun i -> i.Name = name)).Type_ $"{name} should have a type")
        let optionalOf inner = Union (ResizeArray [ Null; inner ])
        assertTypesEqual (optionalOf (Enum (InputEnumSchema(ResizeArray [ "alpha"; "beta" ])))) (typeOf "mode_choice") "mode_choice"
        assertTypesEqual (optionalOf (CWLType.directory ())) (typeOf "optional_dir") "optional_dir"
        assertTypesEqual (optionalOf (CWLType.file ())) (typeOf "flow_union") "flow_union"
        assertTypesEqual (Union (ResizeArray [ String; Int ])) (typeOf "block_union") "block_union"
        assertTypesEqual (optionalOf (Array (InputArraySchema(Union (ResizeArray [ String; CWLType.file () ]))))) (typeOf "array_or_null") "array_or_null"
        let flagOf name = (inputs |> Seq.find (fun i -> i.Name = name)).Optional
        for name in [ "mode_choice"; "optional_dir"; "flow_union"; "array_or_null" ] do
            Expect.equal (flagOf name) (Some true) (sprintf "%s is a union of null and one type, so it is optional." name)
        Expect.equal (flagOf "block_union") None "A union without null is not optional."
        let outputTypeOf name = Expect.wantSome (decoded.Outputs |> Seq.find (fun o -> o.Name = name)).Type_ $"{name} should have a type"
        assertTypesEqual (Union (ResizeArray [ CWLType.file (); CWLType.directory () ])) (outputTypeOf "result") "result"
        assertTypesEqual (optionalOf (CWLType.file ())) (outputTypeOf "maybe") "maybe"

    testList "Union port types" [
        testCase "unions in object form and in bare list form decode" <| fun _ ->
            check (Decode.decodeCommandLineTool tool)
        testCase "unions roundtrip through the encoder" <| fun _ ->
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool tool)
            check (Decode.decodeCommandLineTool encoded)
        testCase "a port with a null and a file in a type list is the same port after a round trip" <| fun _ ->
            let yaml = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x: [\"null\", File]\n  y:\n    type: [\"null\", File]\n    label: y\noutputs: []\n"
            let decoded = Decode.decodeCommandLineTool yaml
            for name in [ "x"; "y" ] do
                Expect.equal (decoded.Inputs.Value |> Seq.find (fun i -> i.Name = name)).Optional (Some true) (sprintf "%s should be optional." name)
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded "A round trip should keep the optional flag."
        testCase "unions in the encoder output use only quoted null members" <| fun _ ->
            let encoded = Encode.encodeToolDescription (Decode.decodeCommandLineTool tool)
            Expect.isFalse (encoded.Contains("[null") || encoded.Contains("- null") || encoded.Contains(", null")) "A plain null member reads as a YAML null, which CWL rejects."
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: notes.txt
        entry: first line
          second line
inputs: {}
outputs: {}
"""
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x:
    type: string
    inputBinding:
      valueFrom: |+
        $(self)

outputs: {}
"""
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}
arc:note:
arc:list:
  - a
  -
  - b
arc:flow: {}
"""
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
                  "File size", decodeParameters, "f:\n  class: File\n  path: a.txt\n  size: 42\n", "f:\n  class: File\n  path: a.txt\n  size: \"42\"\n" ]
            for (name, decode, plain, quoted) in cases do
                decode plain
                Expect.throws (fun () -> decode quoted) (sprintf "A quoted value for %s is a string and must fail." name)
            // The core schema also reads these plain spellings as a bool and an int.
            decodeTool (inputBinding "shellQuote: False")
            decodeTool (inputBinding "position: 0x2")

        testCase "$namespaces and $schemas are kept as mapping and list through decode and encode" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
$namespaces:
  cwltool: "http://commonwl.org/cwltool#"
  arc: "https://github.com/nfdi4plants/ARC_ontology"
$schemas:
  - "https://example.org/a.owl"
  - "https://example.org/b.yml"
inputs: {}
outputs: {}
"""
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
            let parameterError = decodeError (fun () -> DecodeParameters.decodeYAMLParameterFile "a: 1\na: 2\n" |> ignore)
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
            for text in [ ""; "# only a comment\n"; "---\n" ] do
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
            // the path that the error must show, and a document that holds an empty value there
            let cases =
                [ "cwlVersion", "cwlVersion:\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}\n"
                  "class", "cwlVersion: v1.2\nclass:\ninputs: {}\noutputs: {}\n"
                  "baseCommand[1]", "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand:\n  - echo\n  -\ninputs: {}\noutputs: {}\n"
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
                  "expression", "cwlVersion: v1.2\nclass: ExpressionTool\ninputs: {}\noutputs: {}\nexpression:\n"
                  // steps in map form and in sequence form
                  "steps.s.run", workflow [ "    run:"; "    in: {}"; "    out: []" ]
                  "steps.s.in.x", workflow [ "    run: t.cwl"; "    in:"; "      x:"; "    out: []" ]
                  "steps.s.out", workflow [ "    run: t.cwl"; "    in: {}"; "    out:" ]
                  "steps.s.out[1]", workflow [ "    run: t.cwl"; "    in: {}"; "    out:"; "      - a"; "      -" ]
                  "steps.s.run", "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  - id: s\n    run:\n    in: {}\n    out: []\n"
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x:
    type: File
    secondaryFiles: [.bai, ^.idx]
    default: {class: File, path: a.txt}
  y:
    type: string
    default: one
outputs: {}
arc:note:
"""
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
hints:
  - class: acme:Hint # trailing comment
    # leading comment
    list:
      - a # first
      - b
inputs:
  x:
    type: File
    secondaryFiles:
      # about files
      - .bai # index
    default:
      class: File # the class
      path: a.txt
outputs: {}
"""
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
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
hints:
  - class: acme:Hint # hint comment
    # leading comment of a field
    list:
      - a # item comment
inputs:
  x:
    type: File
    secondaryFiles:
      # about files
      - .bai # index
    default:
      class: File # class comment
      path: a.txt
arguments:
  # inside arguments
  - valueFrom: x # trailing

  - y
outputs: {}
"""
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
                  "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps:\n  s:\n    run: t.cwl\n    out: []\n    in:\n      x:\n        default:\n          k: v\n          # tail\n",
                  "          # tail\n          k: v\n    out: []\n"
                  "default of a step input in sequence form",
                  "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps:\n  - id: s\n    run: t.cwl\n    out: []\n    in:\n      - id: x\n        default:\n          - 1\n          - 2\n          # tail\n",
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
        testYamlReaderAndWriter
        testExpressionTool
        testOperation
        testTopLevelDirectDynamicOverflow
    ]

