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
            Expect.equal (ListEntry.values decoded.Outputs).Count 0 ""
        testCase "decode allows whitespace-only separator lines" <| fun _ ->
            let withWhitespaceOnlyLine = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withWhitespaceOnlyLine
            let decoded = Decode.decodeCommandLineTool withWhitespaceOnlyLine
            let inputs = ListEntry.values (Expect.wantSome decoded.Inputs "Inputs should decode when whitespace-only separator lines are present.")
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
            Expect.equal decoded.Doc (Some (DocText.literal expectedDoc)) "Plain multiline scalars should decode unchanged."
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
            let expected = Some (ResizeArray [|(SchemaSaladString.Literal "dotnet"); (SchemaSaladString.Literal "fsi"); (SchemaSaladString.Literal "script.fsx")|])
            let actual = decodeCWLToolDescription.BaseCommand
            Expect.sequenceEqual actual.Value expected.Value ""
        testCase "intent decodes as typed field, not metadata overflow" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFileWithIntent
            let intent = Expect.wantSome decoded.Intent "Intent should decode on CommandLineTool."
            Expect.sequenceEqual intent (literals [ "classification"; "quality-control" ]) ""
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
                    KnownHint (Requirement.DockerRequirement (DockerRequirement.create(dockerPull = (SchemaSaladString.Literal "mcr.microsoft.com/dotnet/sdk:6.0"))))
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
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef(SchemaSaladString.Literal "DOTNET_NOLOGO", SchemaSaladString.Literal "true") |])
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
            let inputsItem = (ListEntry.values decodeCWLToolDescription.Inputs.Value)
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
                    let expected = Some (ListEntry.Value (InputBinding.create(position = BindingPosition.Number 1)))
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
                    let expected = Some (ListEntry.Value (InputBinding.create(position = BindingPosition.Number 2)))
                    let actual = stringItem.InputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testList "Outputs" [
            let outputsItem = (ListEntry.values decodeCWLToolDescription.Outputs)
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
                    let expected = Some (ListEntry.Value (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/.nuget"))))
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
                    let expected = Some (ListEntry.Value (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/*.csv"))))
                    let actual = fileItem.OutputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testCase "Metadata" <| fun _ ->
            let expected = None
            let actual = decodeCWLToolDescription.Metadata
            Expect.equal actual expected ""
    ]

let testToolOutputBindingExtensions =
    let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs:
  result:
    type: File
    outputBinding:
      glob: result.txt
      arc:binding note: keep output binding note
    arc:note: keep output note
"""
    testList "Output binding extensions" [
        testCase "unknown fields of the output binding of a tool output stay on the binding and round trip" <| fun _ ->
            let check (tool: CWLToolDescription) =
                let output = (ListEntry.values tool.Outputs).[0]
                let outputBinding = outputBindingOf output
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" output) (Some "keep output note") "Unknown output fields should be preserved."
                Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" output) "Unknown output binding fields should not be flattened onto the output."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" outputBinding) (Some "keep output binding note") "Unknown output binding fields should be preserved on the binding."
            let decoded = Decode.decodeCommandLineTool yaml
            check decoded
            check (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded))
    ]

let testCWLToolDescriptionMetadata =
    testList "Decode with Metadata" [
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLToolDescriptionMetadata.CWLVersion
            Expect.equal actual expected ""
        testCase "baseCommand" <| fun _ ->
            let expected = Some (ResizeArray [|(SchemaSaladString.Literal "dotnet"); (SchemaSaladString.Literal "fsi"); (SchemaSaladString.Literal "script.fsx")|])
            let actual = decodeCWLToolDescriptionMetadata.BaseCommand
            Expect.sequenceEqual actual.Value expected.Value ""
        testList "Hints" [
            let hintsItem = decodeCWLToolDescriptionMetadata.Hints
            testCase "DockerRequirement" <| fun _ ->
                let expected =
                    KnownHint (Requirement.DockerRequirement (DockerRequirement.create(dockerPull = (SchemaSaladString.Literal "mcr.microsoft.com/dotnet/sdk:6.0"))))
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
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef(SchemaSaladString.Literal "DOTNET_NOLOGO", SchemaSaladString.Literal "true") |])
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
            let inputsItem = (ListEntry.values decodeCWLToolDescriptionMetadata.Inputs.Value)
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
                    let expected = Some (ListEntry.Value (InputBinding.create(position = BindingPosition.Number 1)))
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
                    let expected = Some (ListEntry.Value (InputBinding.create(position = BindingPosition.Number 2)))
                    let actual = stringItem.InputBinding
                    Expect.equal actual expected ""
            ]
        ]
        testList "Outputs" [
            let outputsItem = (ListEntry.values decodeCWLToolDescriptionMetadata.Outputs)
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
                    let expected = Some (ListEntry.Value (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/.nuget"))))
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
                    let expected = Some (ListEntry.Value (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/*.csv"))))
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
                Expect.sequenceEqual intent (literals [ "classification"; "quality-control" ]) ""
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
                Expect.equal decoded.Stdin (Some (SchemaSaladString.Literal "stdin.txt")) "stdin should decode to a typed field."
                Expect.equal decoded.Stdout (Some (SchemaSaladString.Literal "stdout.txt")) "stdout should decode to a typed field."
                Expect.equal decoded.Stderr (Some (SchemaSaladString.Literal "stderr.txt")) "stderr should decode to a typed field."
                Expect.sequenceEqual (ListEntry.values decoded.SuccessCodes.Value) (ResizeArray [| 0 |]) "successCodes should decode to a typed field."
                Expect.sequenceEqual (ListEntry.values decoded.TemporaryFailCodes.Value) (ResizeArray [| 75 |]) "temporaryFailCodes should decode to a typed field."
                Expect.sequenceEqual (ListEntry.values decoded.PermanentFailCodes.Value) (ResizeArray [| 1; 2 |]) "permanentFailCodes should decode to a typed field."
                Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "arguments" decoded) "Known fields should not be direct overflow."
                let metadata = Expect.wantSome decoded.Metadata "Extension field should decode as metadata."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" metadata) (Some "keep overflow") "Extension field should stay in metadata."

                let encoded = Encode.encodeToolDescription decoded
                let roundTripped = Decode.decodeCommandLineTool encoded
                Expect.isSome roundTripped.Arguments "arguments should survive roundtrip."
                Expect.equal roundTripped.Stdout (Some (SchemaSaladString.Literal "stdout.txt")) "stdout should survive roundtrip."
        ]
    ]

let testNestedArrayDecoding =
    testList "Nested Array Decoding" [
        testCase "Decode nested array (array of arrays)" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.NestedArray.cwlFile
            let inputs = (ListEntry.values decoded.Inputs.Value)
            
            // Check IndexInput is File[]
            let indexInput = inputs |> Seq.find (fun i -> i.Name = "IndexInput")
            Expect.isTrue (
                match indexInput.Type_.Value with
                | Array arraySchema -> 
                    match arraySchema.Items with
                    | File -> true
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
                        | File -> true
                        | _ -> false
                    | _ -> false
                | _ -> false
            ) "sampleRecordFiles should be File[][]"
    ]

let testUnionArrayItemDecoding =
    let expectedMixedInputType =
        Array (InputArraySchema(Union (ResizeArray [String; File])))

    let assertMixedInputType (decoded: CWLToolDescription) =
        let inputs = ListEntry.values (Expect.wantSome decoded.Inputs "Union-array-items fixture should decode inputs.")
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
                Expect.equal et.Expression (SchemaSaladString.Literal "$(null)") "Expression should be parsed"
                Expect.equal et.CWLVersion "v1.2" ""
            | other ->
                Expect.isTrue false $"Expected ExpressionTool but got %A{other}"
        testCase "decodeExpressionTool with requirements/inputs/outputs" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithRequirementsFile
            Expect.equal et.CWLVersion "v1.2" ""
            let requirements = Expect.wantSome et.Requirements "Requirements should be present"
            Expect.equal requirements.[0] Requirement.defaultInlineJavascriptRequirement ""
            let inputs = ListEntry.values (Expect.wantSome et.Inputs "Inputs should be present")
            Expect.isTrue (inputs.Count > 0) "Should have at least one input"
            Expect.isTrue ((ListEntry.values et.Outputs).Count > 0) "Should have at least one output"
            Expect.isTrue (et.Expression.Value.Length > 0) "Expression should be non-empty"
        testCase "ExpressionTool intent decodes as typed field and roundtrips" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithIntentFile
            let intent = Expect.wantSome et.Intent "Intent should decode on ExpressionTool."
            Expect.sequenceEqual intent (literals [ "feature-generation"; "post-processing" ]) ""
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
            Expect.equal (ListEntry.values d1.Outputs).Count (ListEntry.values d2.Outputs).Count ""
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
            let inputs = ListEntry.values (Expect.wantSome et.Inputs "")
            let inp = inputs.[0]
            Expect.equal inp.Type_ (Some CWLType.Int) "Input should be int"
            let outp = (ListEntry.values et.Outputs).[0]
            match outp.Type_ with
            | Some (Array arraySchema) ->
                Expect.equal arraySchema.Items CWLType.Int "Expected int[] output type"
            | other ->
                Expect.isTrue false $"Expected Array type but got %A{other}"
            Expect.stringContains et.Expression.Value "Array.apply" "Expression should use Array.apply"
        testCase "default input ExpressionTool conformance pattern" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithDefaultInputFile
            let inputs = ListEntry.values (Expect.wantSome et.Inputs "Inputs should be present")
            let inp = inputs.[0]
            Expect.equal inp.Name "i1" ""
            Expect.equal (ListEntry.values et.Outputs).[0].Type_ (Some CWLType.Int) "Output type should be int"
        testCase "loadContents ExpressionTool input conformance pattern" <| fun _ ->
            let et = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolLoadContentsFile
            let inputs = ListEntry.values (Expect.wantSome et.Inputs "Inputs should be present")
            let inp = inputs.[0]
            // Asserted via `if`/bool rather than a `match` with a unit success branch: Fable's
            // Python backend drops empty branches from a match in statement position, which would
            // leave `Expect.isTrue false` unconditional and make this assertion always fail.
            let isFile = match inp.Type_ with Some File -> true | _ -> false
            Expect.isTrue isFile $"Expected File type but got %A{inp.Type_}"
            Expect.stringContains et.Expression.Value "parseInt" "Expression should use parseInt"
        testCase "pool output ExpressionTool expression roundtrips" <| fun _ ->
            let decoded = Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolPoolOutRoundtripFile
            let encoded = Encode.encodeExpressionToolDescription decoded
            let roundTripped = Decode.decodeExpressionTool encoded
            let output = (ListEntry.values roundTripped.Outputs) |> Seq.find (fun o -> o.Name = "pool_DIR")
            Expect.equal output.Type_ (Some (Directory)) "pool_DIR output type should remain Directory."
            Expect.equal roundTripped.Expression decoded.Expression "Expression payload should roundtrip without semantic changes."
            Expect.stringContains roundTripped.Expression.Value "sanitize(entry)" "Sanitize helper should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "allowedFields" "allowedFields array should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "var outputList = []" "outputList initialization should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "inputs.directory_single" "directory_single input reference should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "inputs.file_single" "file_single input reference should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "inputs.newname" "newname input reference should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "\"Directory\"" "Directory class literal should survive roundtrip."
            Expect.stringContains roundTripped.Expression.Value "listing: outputList" "listing assignment should survive roundtrip."
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
                YAMLicious.Encode.object [ "steps", YAMLicious.Encode.object [ (let id, node = Encode.encodeWorkflowStep false step in Option.get id, node) ] ]
                |> Encode.writeYaml

            let decodedSteps =
                encodedYaml
                |> YAMLicious.Reader.readRoot
                |> Decode.stepsDecoder
                |> ListEntry.values

            Expect.equal decodedSteps.[0].In.Count 0 "Empty `in` should survive encode/decode as an empty mapping."

        testCase "an anchor and its alias are rejected with a clear error" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.DecodeEdgeCases.withAliases
            let decode () = Decode.decodeCommandLineTool yaml |> ignore
            match (try decode (); None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.stringContains (DecodeError.text error) "anchor &a" "The error should name the anchor, which stands in front of the alias."
                Expect.equal (DecodeError.pathText error) "inputs.x.default" "The error should show the anchored node."
            | None -> failwith "A CWL document with an anchor must fail."
    ]

let testOperation =
    testList "Operation" [
        testCase "decode top-level Operation via decodeCWLProcessingUnit" <| fun _ ->
            let result = Decode.decodeCWLProcessingUnit TestObjects.CWL.Operation.minimalOperationFile
            match result with
            | Operation op ->
                Expect.equal op.CWLVersion "v1.2" ""
                Expect.equal (ListEntry.values op.Inputs).Count 1 "Operation should have one input"
                Expect.equal (ListEntry.values op.Outputs).Count 1 "Operation should have one output"
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
            Expect.sequenceEqual intent (literals [ "orchestration"; "wiring" ]) ""
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
            Expect.equal (ListEntry.values d1.Inputs).Count (ListEntry.values d2.Inputs).Count ""
            Expect.equal (ListEntry.values d1.Outputs).Count (ListEntry.values d2.Outputs).Count ""
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
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)")
            DynObj.setProperty "arc:direct note" "expression note" expressionTool
            let encoded = Encode.encodeExpressionToolDescription expressionTool
            Expect.stringContains encoded "arc:direct note: expression note" "Direct DynamicObj overflow should be encoded."
            let roundTripped = Decode.decodeExpressionTool encoded
            let metadata = Expect.wantSome roundTripped.Metadata "Decoded unknown top-level fields should remain available as metadata."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "expression note") "Encoded direct overflow should decode back as top-level metadata."

        testCase "Operation direct overflow encodes and decodes as metadata" <| fun _ ->
            let operation =
                CWLOperationDescription(
                    inputs = ListEntry.ofValues [| CWLInput("input", CWLType.String) |],
                    outputs = ListEntry.ofValues [| CWLOutput("output", CWLType.String) |]
                )
            DynObj.setProperty "arc:direct note" "operation note" operation
            let encoded = Encode.encodeOperationDescription operation
            Expect.stringContains encoded "arc:direct note: operation note" "Direct DynamicObj overflow should be encoded."
            let roundTripped = Decode.decodeOperation encoded
            let metadata = Expect.wantSome roundTripped.Metadata "Decoded unknown top-level fields should remain available as metadata."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "operation note") "Encoded direct overflow should decode back as top-level metadata."
    ]

/// The text of an optional doc, and none for a doc with a directive.
let docTextOf (doc: DocText option) : string option =
    doc |> Option.bind (fun value -> value.TryText)

/// The text of an optional label, and none for a directive.
let labelTextOf (label: SchemaSaladString option) : string option =
    label |> Option.bind SchemaSaladString.tryLiteral

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
            Expect.equal roundTripped.Label (Some (SchemaSaladString.Literal "1.0")) "The label should stay a string."
            Expect.equal roundTripped.Doc (Some (DocText.literal "null")) "The doc should stay a string."

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
            let enumType = Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "true"; SchemaSaladString.Literal "a"; SchemaSaladString.Literal "x,y"; SchemaSaladString.Literal "12" ]))
            let enumText = Encode.cwlTypeToYamlString enumType
            Expect.equal enumText "{type: enum, symbols: [\"true\", a, \"x,y\", \"12\"]}" "Lookalikes and flow indicators need quotes inside the flow text."
            Expect.isTrue (CWLType.typesEqual enumType (enumText |> YAMLicious.Reader.readRoot |> Decode.cwlTypeDecoder')) "The flow text should decode to the same enum."
            let recordType =
                Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("null", CWLType.String); InputRecordField("ok", CWLType.File) ]))
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
            Expect.sequenceEqual ((ListEntry.values roundTripped.Inputs.Value) |> Seq.map (fun i -> i.Name)) [ "1"; "true"; "plain" ] "The input names should survive."

        testCase "the null type is written as a quoted string" <| fun _ ->
            Expect.equal (Encode.cwlTypeToYamlString Null) "\"null\"" "A plain null would read as a YAML null."
            Expect.equal (Encode.cwlTypeToYamlString (Union (ResizeArray [ Null; CWLType.File ]))) "[\"null\", File]" "The null member of a union must be quoted."
    ]

let testToolUnionPorts =
    let tool = TestObjects.CWL.CommandLineTool.UnionPorts.cwlFile

    let assertTypesEqual (expected: CWLType) (actual: CWLType) (message: string) =
        Expect.isTrue (CWLType.typesEqual expected actual) $"{message}: expected %A{expected} but got %A{actual}"

    let check (decoded: CWLToolDescription) =
        let inputs = ListEntry.values (Expect.wantSome decoded.Inputs "Inputs should decode.")
        let typeOf name = (Expect.wantSome (inputs |> Seq.find (fun i -> i.Name = name)).Type_ $"{name} should have a type")
        let optionalOf inner = Union (ResizeArray [ Null; inner ])
        assertTypesEqual (optionalOf (Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "alpha"; SchemaSaladString.Literal "beta" ])))) (typeOf "mode_choice") "mode_choice"
        assertTypesEqual (optionalOf (CWLType.Directory)) (typeOf "optional_dir") "optional_dir"
        assertTypesEqual (optionalOf (CWLType.File)) (typeOf "flow_union") "flow_union"
        assertTypesEqual (Union (ResizeArray [ String; Int ])) (typeOf "block_union") "block_union"
        assertTypesEqual (optionalOf (Array (InputArraySchema(Union (ResizeArray [ String; CWLType.File ]))))) (typeOf "array_or_null") "array_or_null"
        let flagOf name = (inputs |> Seq.find (fun i -> i.Name = name)).Optional
        for name in [ "mode_choice"; "optional_dir"; "flow_union"; "array_or_null" ] do
            Expect.isTrue (flagOf name) (sprintf "%s is a union of null and one type, so it is optional." name)
        Expect.isFalse (flagOf "block_union") "A union without null is not optional."
        let outputTypeOf name = Expect.wantSome ((ListEntry.values decoded.Outputs) |> Seq.find (fun o -> o.Name = name)).Type_ $"{name} should have a type"
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
                Expect.isTrue ((ListEntry.values decoded.Inputs.Value) |> Seq.find (fun i -> i.Name = name)).Optional (sprintf "%s should be optional." name)
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
  z: [File, "null"]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ String; Null ]))) "The input type keeps the order of the list."
            Expect.equal (ListEntry.values decoded.Outputs).[0].Type_ (Some (Union (ResizeArray [ CWLType.File; Null ]))) "The output type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded """x: [string, "null"]""" "The input is written as a list."
            Expect.stringContains encoded """z: [File, "null"]""" "The output is written as a list."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (ListEntry.values roundTripped.Inputs.Value).[0].Type_ (ListEntry.values decoded.Inputs.Value).[0].Type_ "The input type survives the round trip."
            Expect.equal (ListEntry.values roundTripped.Outputs).[0].Type_ (ListEntry.values decoded.Outputs).[0].Type_ "The output type survives the round trip."
        testCase "a type list with the null in front of the type is written as the shorthand" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  x: ["null", string]
outputs:
  z: ["null", File]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ Null; String ]))) "The input type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "x: string?" "The input is written as the shorthand."
            Expect.stringContains encoded "z: File?" "The output is written as the shorthand."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (ListEntry.values roundTripped.Inputs.Value).[0].Type_ (ListEntry.values decoded.Inputs.Value).[0].Type_ "The input type survives the round trip."
            Expect.equal (ListEntry.values roundTripped.Outputs).[0].Type_ (ListEntry.values decoded.Outputs).[0].Type_ "The output type survives the round trip."
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
            let input = (ListEntry.values operation.Inputs) |> Seq.find (fun i -> i.Name = name)
            Expect.equal input.Type_ (Some expected) $"The input {name} should have the type {expected}."
        for (name, expected) in expectedOutputTypes do
            let output = (ListEntry.values operation.Outputs) |> Seq.find (fun o -> o.Name = name)
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
            Expect.equal (ListEntry.values operation.Inputs).[0].Type_ (Some (Array (InputArraySchema(Array (InputArraySchema(Any)))))) "An array schema may hold an array schema of Any."
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
            Expect.equal (portType (ListEntry.values decoded.Inputs.Value) (fun i -> i.Name) (fun i -> i.Type_) "unsorted") Stdin "The input has the type stdin."
            Expect.equal (portType (ListEntry.values decoded.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "sorted") Stdout "The output has the type stdout."
            Expect.equal (portType (ListEntry.values decoded.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "log") Stderr "The output in object form has the type stderr."
            Expect.isFalse (ListEntry.values decoded.Inputs.Value).[0].Optional "A stream type is no optional type."
        testCase "a stream type in object form decodes as the whole type" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool """inputs:
  lines:
    type: stdin
    label: Lines
outputs:
  out:
    type: stdout""")
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some Stdin) "The input has the type stdin."
            Expect.equal (ListEntry.values decoded.Outputs).[0].Type_ (Some Stdout) "The output has the type stdout."
        testCase "stream types are written as type names and read back" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "unsorted: stdin" "The input is written as the type name."
            Expect.stringContains encoded "sorted: stdout" "The output is written as the type name."
            Expect.stringContains encoded "type: stderr" "The output in object form keeps its object."
            let roundTripped = Decode.decodeCommandLineTool encoded
            Expect.equal (portType (ListEntry.values roundTripped.Inputs.Value) (fun i -> i.Name) (fun i -> i.Type_) "unsorted") Stdin "The input survives the round trip."
            Expect.equal (portType (ListEntry.values roundTripped.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "sorted") Stdout "The stdout output survives the round trip."
            Expect.equal (portType (ListEntry.values roundTripped.Outputs) (fun o -> o.Name) (fun o -> o.Type_) "log") Stderr "The stderr output survives the round trip."
        testCase "the inline command line tool of a workflow admits stream types" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.CommandLineTool.StreamTypes.workflowWithInlineTool
            let inlineTool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values decoded.Steps).[0].Run) "The step runs an inline tool."
            Expect.equal (ListEntry.values inlineTool.Outputs).[0].Type_ (Some Stdout) "The output has the type stdout."
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
            Expect.equal (Decode.decodeCommandLineTool withoutInput).Stdin (Some (SchemaSaladString.Literal "input.txt")) "The tool field alone is valid."
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
            Expect.equal (decodeLabel "|" "  a\n") (Some (SchemaSaladString.Literal "a\n")) "Clip keeps one line break."
            Expect.equal (decodeLabel "|-" "  a\n") (Some (SchemaSaladString.Literal "a")) "Strip keeps none."
            Expect.equal (decodeLabel "|+" "  a\n\n") (Some (SchemaSaladString.Literal "a\n\n")) "Keep keeps all of them."

        testCase "strings with trailing line breaks roundtrip" <| fun _ ->
            let texts = [ "a\n"; "a"; "a\n\n"; "a\n\n\n"; "line one\nline two\n"; "line one\nline two"; "line one\n\nline three\n"; "line one\n\n"; "$(inputs.x)\n" ]
            for text in texts do
                let tool = Decode.decodeCommandLineTool minimalTool
                tool.Label <- Some (SchemaSaladString.Literal text)
                let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription tool)
                Expect.equal roundTripped.Label (Some (SchemaSaladString.Literal text)) (sprintf "The label %A should keep its line breaks." text)

        testCase "a keep-chomped block at the end of a section is not extended by the section separator" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.ReaderAndWriter.withKeptBlockBeforeSection
            let valueFrom (tool: CWLToolDescription) =
                tool.Inputs |> Option.map ListEntry.values |> Option.bind Seq.tryHead |> Option.bind (fun i -> i.InputBinding) |> Option.bind ListEntry.tryValue |> Option.bind (fun b -> b.ValueFrom)
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (valueFrom decoded) (Some (SchemaSaladString.Literal "$(self)\n\n")) "The keep header should keep both line breaks."
            let roundTripped = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.equal (valueFrom roundTripped) (Some (SchemaSaladString.Literal "$(self)\n\n")) "The blank line between sections must not become part of the scalar."

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
            Expect.sequenceEqual (ListEntry.values packages |> Seq.map (fun p -> p.Package)) [ SchemaSaladString.Literal "samtools"; SchemaSaladString.Literal "bwa" ] "Both packages should decode."
            Expect.isTrue (ListEntry.values packages |> Seq.forall (fun p -> p.Specs.IsNone)) "Neither package has specs."

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
            Expect.equal (Decode.decodeExpressionTool (expressionTool "")).Expression (SchemaSaladString.Literal "${return 1;}") "The end of the text takes the place of the line feed."
            Expect.equal (Decode.decodeExpressionTool (expressionTool "\n")).Expression (SchemaSaladString.Literal "${return 1;}\n") "Clip chomping keeps the one line feed."

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
                  "steps.s.in.x.source[1]", steps [ "    in:"; "      - id: x"; "        source:"; "          - a"; "          -"; "    out: []" ]
                  "steps.s.out[0].id", steps [ "    in: {}"; "    out:"; "      - id:" ] ]
            for (path, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCWLProcessingUnit yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "The error should show this path:\n%s" yaml)
                    Expect.stringContains error.Message "Expected a value, found" "The error should say that a value is missing."
                | None -> failwith (sprintf "An empty value at %s must fail:\n%s" path yaml)
            // a quoted null is a string, and the empty list of out is not an empty value
            let quoted = requirement [ "  - class: EnvVarRequirement"; "    envDef:"; "      - envName: A"; "        envValue: \"null\"" ]
            Expect.isSome (Decode.decodeCommandLineTool quoted).Requirements "A quoted null is the string null."
            let decoded = Decode.decodeWorkflow (workflow [ "    run: t.cwl"; "    in: {}"; "    out: []" ])
            Expect.equal (ListEntry.values decoded.Steps).[0].Out.Count 0 "out: [] has no outputs."

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
            Expect.equal (ListEntry.values first.Inputs.Value).[0] (ListEntry.values second.Inputs.Value).[0] "Two decodes of the same port are equal."
            Expect.equal ((ListEntry.values first.Inputs.Value).[0].GetHashCode()) ((ListEntry.values second.Inputs.Value).[0].GetHashCode()) "Equal ports have the same hash."
            Expect.isTrue (DynamicObjHelpers.dynamicPropertiesEqual first.Metadata.Value second.Metadata.Value) "A preserved null compares by its node."
            Expect.equal (DynamicObjHelpers.hashDynamicProperties first.Metadata.Value) (DynamicObjHelpers.hashDynamicProperties second.Metadata.Value) "Equal extension fields have the same hash."
            Expect.equal first second "Two decodes of the same text are equal tools."
            let changed = Decode.decodeCommandLineTool (yaml.Replace("default: one", "default: two"))
            Expect.notEqual (ListEntry.values first.Inputs.Value).[1] (ListEntry.values changed.Inputs.Value).[1] "A different default value is a different port."
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
                    Expect.equal decoded.Expression (SchemaSaladString.Literal expected) (sprintf "The header %s should give this value." header)
                    let encoded = Encode.encodeExpressionToolDescription decoded
                    let roundTripped = Decode.decodeExpressionTool encoded
                    Expect.equal roundTripped.Expression (SchemaSaladString.Literal expected) (sprintf "The expression with %s and %d blank lines should survive a write and a read, written as:\n%s" header blankLines.Length encoded)
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
            Expect.stringContains encoded "# trailing\n\n  - z" "The blank line between the two arguments should be written back."

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

        testCase "keys that the loader of cwltool takes for one key are a repeated key, and keys of another type are different keys" <| fun _ ->
            let tool (keys: string) =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  x:"; "    type: Any"; "    default: " + keys; "outputs: []"; "" ]
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            // The loader compares keys as Python does: 1 equals 0x1, 1.0 and true, and every null is one key.
            // The message names both spellings when they differ, and the key once when they are equal.
            for (keys, first, second) in
                [ "{null: a, ~: b}", "null", "~"; "{null: a, Null: b}", "null", "Null"; "{true: a, True: b}", "true", "True"; "{1: a, 0x1: b}", "1", "0x1"
                  "{1: a, 01: b}", "1", "01"; "{1: a, +1: b}", "1", "+1"; "{1: a, 1.0: b}", "1", "1.0"; "{true: a, 1: b}", "true", "1"
                  "{false: a, 0: b}", "false", "0"; "{1.5: a, 15e-1: b}", "1.5", "15e-1"; "{.inf: a, .Inf: b}", ".inf", ".Inf"
                  "{.nan: a, .NaN: b}", ".nan", ".NaN"; "{8: a, 0o10: b}", "8", "0o10"
                  "{a: 1, \"a\": 2}", "a", "a"; "{\"1\": a, '1': b}", "1", "1"
                  "{12345678901234567890: a, 12345678901234567890: b}", "12345678901234567890", "12345678901234567890" ] do
                match decodeError (tool keys) with
                | Some error ->
                    Expect.isTrue ((DecodeError.pathText error).StartsWith "inputs.x.default.") (sprintf "%s: the error should show the second key, found %s." keys (DecodeError.pathText error))
                    if first = second then
                        Expect.stringContains error.Message ("The key " + first + " occurs twice, at line ") (sprintf "%s: the error should say that the key occurs twice." keys)
                    else
                        Expect.stringContains error.Message ("The key " + second + " at line ") (sprintf "%s: the error should name the second key and its place." keys)
                        Expect.stringContains error.Message (" repeats the key " + first + " at line ") (sprintf "%s: the error should name the first key and its place." keys)
                        Expect.isFalse (error.Message.Contains "occurs twice") (sprintf "%s: the error should not use the wording for equal keys." keys)
                | None -> failwith (sprintf "The keys %s are one key for cwltool and must fail." keys)
            // A quoted key is a text, so it differs from the plain key of another type.
            for keys in
                [ "{1: a, \"1\": b}"; "{\"1\": a, 1: b}"; "{true: a, \"true\": b}"; "{null: a, \"null\": b}"; "{~: a, \"~\": b}"; "{1: a, 2: b}"; "{1: a, 1.5: b}"
                  "{.inf: a, -.inf: b}"; "{1: a, a: b}"; "{.nan: a, \".nan\": b}" ] do
                let decoded = Decode.decodeCommandLineTool (tool keys)
                let written = Encode.encodeToolDescription decoded
                Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool." keys)
                Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool written)) written (sprintf "%s: writing again should give the same text." keys)
            let written = Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool "{1: a, \"1\": b, null: c, \"null\": d}"))
            for expected in [ "1: a"; "\"1\": b"; "null: c"; "\"null\": d" ] do
                Expect.stringContains written expected (sprintf "The key keeps its spelling: %s\n%s" expected written)
        testCase "a decoder of a fragment fails at a key that repeats a key of the mapping it reads, with the error of the whole text" <| fun _ ->
            // A tree that goes straight to a decoder of a fragment has not passed the check of a document, so the decoder checks
            // the keys of each mapping that it reads entry by entry. The error equals the error of the text entry point.
            let errorOf (decode: unit -> unit) : DecodeError option =
                try
                    decode ()
                    None
                with YamlDecodeException error -> Some error
            let places : (string * (YamlNode -> unit) * (string -> string -> string)) list =
                [ "inputs", (Decode.inputArrayDecoder >> ignore), (fun a b -> a + ": string\n" + b + ": int\n")
                  "outputs", (Decode.outputArrayDecoder >> ignore), (fun a b -> a + ": stdout\n" + b + ": stderr\n")
                  "steps", (Decode.stepsDecoder >> ignore), (fun a b -> "steps:\n  " + a + ": {run: t.cwl, in: [], out: []}\n  " + b + ": {run: t.cwl, in: [], out: []}\n")
                  "step inputs", (Decode.inputStepDecoder >> ignore), (fun a b -> a + ": x\n" + b + ": y\n")
                  "hints in map form", (Decode.hintArrayDecoder >> ignore), (fun a b -> a + ": {}\n" + b + ": {}\n")
                  "the fields of a hint in map form", (Decode.hintArrayDecoder >> ignore), (fun a b -> "acme:Hint: {" + a + ": 1, " + b + ": 2}\n")
                  "the fields of a hint of an unknown class", (Decode.hintArrayDecoder >> ignore), (fun a b -> "- {class: acme:Hint, " + a + ": 1, " + b + ": 2}\n")
                  "requirements in map form", (Decode.requirementArrayDecoder >> ignore), (fun a b -> a + ": {}\n" + b + ": {}\n")
                  "envDef in map form", (Decode.requirementArrayDecoder >> ignore), (fun a b -> "- class: EnvVarRequirement\n  envDef: {" + a + ": x, " + b + ": y}\n")
                  "a record of a job file", (DecodeParameters.decodeParameterValue >> ignore), (fun a b -> "{" + a + ": 1, " + b + ": 2}\n") ]
            // An extension field holds a mapping with a key that is no text as it was written, so these places take text keys.
            let extensionPlaces : (string * (YamlNode -> unit) * (string -> string -> string)) list =
                [ "a mapping in an extension field", (Decode.inputArrayDecoder >> ignore), (fun a b -> "x: {type: string, acme:f: {" + a + ": 1, " + b + ": 2}}\n")
                  "a mapping in a list of an extension field", (Decode.inputArrayDecoder >> ignore), (fun a b -> "x: {type: string, acme:f: [{a: 1}, {g: {" + a + ": 1, " + b + ": 2}}]}\n") ]
            // Equal texts in any quoting and plain keys that the loader of cwltool takes for one key. A hint in map form drops
            // its class field, and a repeated class key fails there too.
            let textPairs = [ "x", "x"; "x", "\"x\""; "'x'", "\"x\""; "class", "class" ]
            let otherPairs = [ "1", "0x1"; "true", "1"; "null", "~" ]
            for (somePlaces, keyPairs) in [ places, textPairs @ otherPairs; extensionPlaces, textPairs ] do
                for (place, decode, text) in somePlaces do
                    for (first, second) in keyPairs do
                        let yaml = text first second
                        let expected = errorOf (fun () -> Decode.readYaml yaml |> ignore)
                        match expected with
                        | Some error ->
                            Expect.isTrue (error.Message.Contains "occurs twice" || error.Message.Contains "repeats the key") (sprintf "%s, %s and %s: the whole text fails at the repeated key, found %s" place first second error.Message)
                        | None -> failwith (sprintf "%s, %s and %s: the whole text must fail." place first second)
                        Expect.equal (errorOf (fun () -> decode (Reader.readRoot yaml))) expected (sprintf "%s, %s and %s: the decoder should fail with the error of the whole text." place first second)
            let apart = "x: {type: string, acme:f: {a: 1}}\ny: {type: string, acme:f: {a: 2}}\n"
            Expect.equal (Decode.inputArrayDecoder (Reader.readRoot apart)).Count 2 "The same key in two mappings is no repeated key."
        testCase "an extension field keeps the keys of a mapping with their types, and a field of an object needs a text as its name" <| fun _ ->
            let tool (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "$namespaces: {ex: \"http://example.org/\"}"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: []"; "outputs: []" ] @ lines @ [ "" ])
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            let decoded = Decode.decodeCommandLineTool (tool [ "ex:meta:"; "  1: a"; "  \"1\": b"; "  null: e"; "  \"null\": f"; "ex:flags:"; "  true: c"; "  \"true\": d"; "ex:list:"; "  - {1: a, \"1\": b}" ])
            let written = Encode.encodeToolDescription decoded
            for expected in [ "1: a"; "\"1\": b"; "true: c"; "\"true\": d"; "null: e"; "\"null\": f" ] do
                Expect.stringContains written expected (sprintf "The key keeps its spelling: %s\n%s" expected written)
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
            Expect.notEqual decoded (Decode.decodeCommandLineTool (tool [ "ex:meta:"; "  \"1\": a"; "  \"1x\": b"; "  null: e"; "  \"null\": f"; "ex:flags:"; "  true: c"; "  \"true\": d"; "ex:list:"; "  - {1: a, \"1\": b}" ])) "Another key makes another tool."
            for (name, key, tag) in [ "an int", "1", "!!int"; "a bool", "true", "!!bool"; "a null", "~", "!!null"; "a float", "1.5", "!!float" ] do
                match decodeError (tool [ key + ": x" ]) with
                | Some error ->
                    Expect.stringContains error.Message "Expected a string as a field name, found" (sprintf "%s as the name of a field: the error should say what is wrong." name)
                    Expect.stringContains error.Message ("which resolves to " + tag) (sprintf "%s as the name of a field: the error should name the type of the key." name)
                    Expect.isSome error.Span (sprintf "%s as the name of a field: the error should have the place in the text." name)
                | None -> failwith (sprintf "%s as the name of a field must fail." name)
            Expect.isSome (decodeError (tool [ "ex:x: 1"; "1: x" ])) "A key of another type fails also behind a valid field."
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
            for hint in [ "acme:Hint"; "ShellCommandRequirement"; "WorkReuse" ] do
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
                            let port = if isOutput then (ListEntry.values decoded.Outputs).[0].Type_ else (ListEntry.values decoded.Inputs.Value).[0].Type_
                            let recordField =
                                match port with
                                | Some (Record schema) -> (InputRecordSchema.getFieldValuesOrEmpty schema).[0]
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
            let fields = match (ListEntry.values decoded.Inputs.Value).[0].Type_ with | Some (Record schema) -> (InputRecordSchema.getFieldValuesOrEmpty schema) | other -> failwithf "Expected a record, found %A" other
            Expect.sequenceEqual (fields |> Seq.map (fun field -> field.Name)) [ "a"; "b"; "c"; "d" ] "The fields keep their order."
            Expect.isTrue (CWLType.typesEqual (CWLType.File) fields.[0].Type) "A field can be given as the type."
            Expect.isTrue (CWLType.typesEqual (Union (ResizeArray [ Null; CWLType.String ])) fields.[1].Type) "A field can be given as an optional type."
            Expect.isTrue (CWLType.typesEqual (Union (ResizeArray [ Null; CWLType.File ])) fields.[2].Type) "A field can be given as a type list."
            Expect.equal fields.[3].Doc (Some (DocText.literal "object form")) "A field object keeps its doc."
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
                Expect.isSome (ListEntry.values (Decode.decodeCommandLineTool (tool fields)).Inputs.Value).[0].Type_ (sprintf "fields: %s is a list or a mapping." fields)

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
                match (ListEntry.values decoded.Inputs.Value).[0].Type_ with
                | Some (Union members) ->
                    match members.[index] with
                    | Record schema -> (InputRecordSchema.getFieldValuesOrEmpty schema)
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
                  [ "  x: [string, \"File[]\"]"; "  z: [int, \"File?\"]" ],
                  "  x: [string, \"File[]\"]\n  z: [int, File?]\n"
                  "optional array of arrays",
                  [ "  x:"; "    type:"; "      - \"null\""; "      - type: array"; "        items:"; "          type: array"; "          items: string" ],
                  "  x:\n    - \"null\"\n    - type: array\n      items:\n        type: array\n        items: string\n"
                  "array of arrays as the type of a record field",
                  [ "  x:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type:"; "            type: array"; "            items:"; "              type: array"; "              items: string" ],
                  "        f:\n          type:\n            type: array\n            items:\n              type: array\n              items: string\n"
                  "arrays of one level keep the shorthand",
                  [ "  x: File[]"; "  w: File[]?"; "  z: string?" ],
                  "  x: File[]\n  w: File[]?\n  z: string?\n" ]
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
            let read (portLines: string list) = (ListEntry.values (Decode.decodeCommandLineTool (tool portLines)).Inputs.Value)
            let arrayOf (inner: CWLType) = CWLType.Array (InputArraySchema(inner))
            let optionalOf (inner: CWLType) = CWLType.Union (ResizeArray [ CWLType.Null; inner ])
            let members = read [ "  x: [string, \"File[]\"]"; "  y: [int, \"File?\"]"; "  z: [\"null\", \"string[]\"]" ]
            Expect.equal members.[0].Type_ (Some (CWLType.Union (ResizeArray [ CWLType.String; arrayOf (CWLType.File) ]))) "A member may be an array shorthand."
            Expect.equal members.[1].Type_ (Some (CWLType.Union (ResizeArray [ CWLType.Int; optionalOf (CWLType.File) ]))) "A member may be an optional shorthand."
            Expect.equal members.[2].Type_ (Some (optionalOf (arrayOf CWLType.String))) "A list of null and an array shorthand is an optional array."
            let fieldTypes = read [ "  r:"; "    type:"; "      type: record"; "      fields:"; "        f: File[]?"; "        g:"; "          type: string[]" ]
            match fieldTypes.[0].Type_ with
            | Some (CWLType.Record schema) ->
                Expect.equal (InputRecordSchema.getFieldValuesOrEmpty schema).[0].Type (optionalOf (arrayOf (CWLType.File))) "A record field reads File[]? as a type."
                Expect.equal (InputRecordSchema.getFieldValuesOrEmpty schema).[1].Type (arrayOf CWLType.String) "A record field reads string[] in its type field."
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
                    Expect.equal (docOf decoded) (Some (DocText.literal expected)) (sprintf "%s: the doc is found" name)
                    let encoded = Encode.encodeProcessingUnit decoded
                    let again = Decode.decodeCWLProcessingUnit encoded
                    Expect.equal (docOf again) (Some (DocText.literal expected)) (sprintf "%s: the doc is the same after a round trip\n%s" name encoded)
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
            let package (version: string) = SoftwarePackage(SchemaSaladString.Literal "tool", literals [ version ], literals [ "https://example.org/tool" ])
            Expect.equal (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) "Equal lists of equal packages are equal."
            Expect.notEqual (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (SoftwareRequirement (ListEntry.ofValues [ package "2.0" ])) "A different version is a different requirement."
            Expect.notEqual (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (SoftwareRequirement (ListEntry.ofValues [ package "1.0"; package "1.0" ])) "A different number of packages is a different requirement."
            Expect.notEqual (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ])) (EnvVarRequirement (ResizeArray [])) "Another kind of requirement is a different requirement."
            Expect.equal (hash (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ]))) (hash (SoftwareRequirement (ListEntry.ofValues [ package "1.0" ]))) "Equal requirements have the same hash."
            Expect.equal (SoftwarePackage(SchemaSaladString.Literal "tool")) (SoftwarePackage(SchemaSaladString.Literal "tool")) "Packages without a version are equal."
            Expect.notEqual (SoftwarePackage(SchemaSaladString.Literal "tool")) (SoftwarePackage(SchemaSaladString.Literal "tool", literals [ "1.0" ])) "A missing version differs from a version."
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
      Fixed "        symbols: [x, z]"
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
      Fixed "      outputs:"
      Fixed "        done: string"
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
    let inputs = (ListEntry.values workflow.Inputs) |> Seq.toList
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
    let step = (ListEntry.values workflow.Steps).[0]
    [ "workflow", box workflow ]
    @ (inputs |> List.map (fun input -> "input", box input))
    @ schemas
    @ fields
    @ ((ListEntry.values workflow.Outputs) |> Seq.map (fun output -> "output", box output) |> Seq.toList)
    @ [ "step", box step ]
    @ (WorkflowStep.getInputValues step |> Seq.map (fun input -> "step input", box input) |> Seq.toList)
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
                Expect.equal workflow.Label (Some (SchemaSaladString.Literal "first part of the workflow name second part of the workflow name")) (sprintf "Texts %s: the label is the parts joined by spaces." name)
                Expect.equal workflow.Doc (Some (DocText.literal "first line of the workflow text second line of the workflow text third line of the workflow text")) (sprintf "Texts %s: the doc is the parts joined by spaces." name)
                Expect.equal (ListEntry.values workflow.Inputs).[0].Doc (Some (DocText.literal "first line of the input text second line of the input text")) (sprintf "Texts %s: the doc of an input is the parts joined by spaces." name)

        testCase "a doc or a label that is not edited comes back in the spelling of its source" <| fun _ ->
            let cases =
                [ "a folded doc over three lines",
                  "doc: >\n  first line of the text\n  second line of the text\n  third line of the text\n",
                  (fun (tool: CWLToolDescription) -> docTextOf tool.Doc),
                  "first line of the text second line of the text third line of the text\n"
                  "a plain label over three lines",
                  "label: first part of the name\n  second part of the name\n  third part of the name\n",
                  (fun (tool: CWLToolDescription) -> labelTextOf tool.Label),
                  "first part of the name second part of the name third part of the name"
                  "a double-quoted doc with an escaped break",
                  "doc: \"first part of the text \\\n  second part of the text\"\n",
                  (fun (tool: CWLToolDescription) -> docTextOf tool.Doc),
                  "first part of the text second part of the text"
                  "a double-quoted label on one line",
                  "label: \"just one line of the name\"\n",
                  (fun (tool: CWLToolDescription) -> labelTextOf tool.Label),
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
                    docTool.Doc <- Some (DocText.literal edit)
                    (ListEntry.values docTool.Inputs.Value).[0].Doc <- Some (DocText.literal edit)
                    let encodedDoc = Encode.encodeToolDescription docTool
                    let againDoc = Decode.decodeCommandLineTool encodedDoc
                    Expect.equal againDoc.Doc (Some (DocText.literal toolDoc)) (sprintf "%s edited to %A: the doc of the tool reads back.\n%s" name edit encodedDoc)
                    Expect.equal (ListEntry.values againDoc.Inputs.Value).[0].Doc (Some (DocText.literal edit)) (sprintf "%s edited to %A: the doc of the input reads back.\n%s" name edit encodedDoc)
                    Expect.isFalse (encodedDoc.Contains "of the text") (sprintf "%s edited to %A: the old text is gone." name edit)
                    let labelTool = Decode.decodeCommandLineTool (editTool "label" lines)
                    labelTool.Label <- Some (SchemaSaladString.Literal edit)
                    (ListEntry.values labelTool.Inputs.Value).[0].Label <- Some (SchemaSaladString.Literal edit)
                    let encodedLabel = Encode.encodeToolDescription labelTool
                    let againLabel = Decode.decodeCommandLineTool encodedLabel
                    Expect.equal againLabel.Label (Some (SchemaSaladString.Literal edit)) (sprintf "%s edited to %A: the label of the tool reads back.\n%s" name edit encodedLabel)
                    Expect.equal (ListEntry.values againLabel.Inputs.Value).[0].Label (Some (SchemaSaladString.Literal edit)) (sprintf "%s edited to %A: the label of the input reads back.\n%s" name edit encodedLabel)

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
                for text in [ "\"42\""; "'true'"; "|\n  42" ] do
                    let decoded = Decode.decodeCommandLineTool (tool [ key + ": " + text; "inputs: {}"; "outputs: {}" ])
                    let isSet = if key = "doc" then decoded.Doc.IsSome else decoded.Label.IsSome
                    Expect.isTrue isSet (sprintf "The spelling %s is a string." (text.Replace("\n", "\\n")))
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

        testCase "an edit of a plain string doc or label to a bool, an int or null reads back as the same string and keeps the comment" <| fun _ ->
            let tool (field: string) =
                "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\n" + field + "inputs: {}\noutputs: {}\n"
            let sources =
                [ "a plain label with a comment", "label: abc # keep me\n", "# keep me"
                  "a quoted label with a comment", "label: \"a name\" # keep me\n", "# keep me"
                  "a literal doc with a header comment", "doc: | # keep me\n  some text\n", "# keep me" ]
            for (name, field, kept) in sources do
                for edit in [ "true"; "42"; "1.5"; "null" ] do
                    let decoded = Decode.decodeCommandLineTool (tool field)
                    if field.StartsWith "doc" then decoded.Doc <- Some (DocText.literal edit) else decoded.Label <- Some (SchemaSaladString.Literal edit)
                    let encoded = Encode.encodeToolDescription decoded
                    let again = Decode.decodeCommandLineTool encoded
                    Expect.equal (if field.StartsWith "doc" then docTextOf again.Doc else labelTextOf again.Label) (Some edit) (sprintf "%s edited to %s: the text reads back as the string.\n%s" name edit encoded)
                    Expect.stringContains encoded kept (sprintf "%s edited to %s: %s stays.\n%s" name edit kept encoded)

        testCase "a doc that is a list of strings reads as their concatenation, comes back unchanged and is a string after an edit" <| fun _ ->
            let tool (field: string) = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: [echo]\n" + field + "inputs:\n  x:\n    type: string\n    doc: [one, two]\noutputs: {}\n"
            let lists =
                [ "a flow list", "doc: [first part, second part]\n"
                  "a block list", "doc:\n  - first part\n  - second part\n"
                  "a block list with a comment", "doc:\n  - first part # note\n  - second part\n" ]
            for (name, lines) in lists do
                let decoded = Decode.decodeCommandLineTool (tool lines)
                Expect.equal decoded.Doc (Some (DocText.literal "first partsecond part")) (sprintf "%s: the doc is the items joined without a separator." name)
                Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Doc (Some (DocText.literal "onetwo")) (sprintf "%s: the doc of an input is a list too." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded lines (sprintf "%s: the list comes back as written.\n%s" name encoded)
                Expect.stringContains encoded "    doc: [one, two]\n" (sprintf "%s: the list of the input comes back as written.\n%s" name encoded)
                Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: a second pass should write the same text." name)
                // the same text as a string and the list are equal, and the same text set again keeps the list
                Expect.equal decoded (Decode.decodeCommandLineTool (tool "doc: first partsecond part\n")) (sprintf "%s: a list and its concatenation are equal." name)
                decoded.Doc <- Some (DocText.literal "first partsecond part")
                Expect.stringContains (Encode.encodeToolDescription decoded) lines (sprintf "%s: the same text keeps the list." name)
                // an edit writes a string
                decoded.Doc <- Some (DocText.literal "one text")
                (ListEntry.values decoded.Inputs.Value).[0].Doc <- Some (DocText.literal "another text")
                let edited = Encode.encodeToolDescription decoded
                Expect.stringContains edited "doc: one text\n" (sprintf "%s: an edited doc is a string.\n%s" name edited)
                Expect.stringContains edited "doc: another text\n" (sprintf "%s: an edited doc of an input is a string.\n%s" name edited)
                Expect.isFalse (edited.Contains "first part" || edited.Contains "[one, two]") (sprintf "%s: the list is gone.\n%s" name edited)
                Expect.equal (Decode.decodeCommandLineTool edited).Doc (Some (DocText.literal "one text")) (sprintf "%s: the edited doc reads back." name)

        testCase "the label is a string only and the items of a doc list are strings" <| fun _ ->
            let tool (field: string) = "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: [echo]\n" + field + "inputs: {}\noutputs: {}\n"
            let decodeError (yaml: string) =
                try
                    Decode.decodeCommandLineTool yaml |> ignore
                    None
                with YamlDecodeException error -> Some error
            for (field, path, message) in [ "label: [a, b]\n", "label", "Expected a string"; "doc: [a, 42]\n", "doc[1]", "Expected a string, found the plain scalar 42"; "doc: [a, true]\n", "doc[1]", "a bool"; "doc: [a, {b: c}]\n", "doc[1]", "Expected a string"; "doc:\n  - a\n  -\n", "doc[1]", "Expected a value, found" ] do
                match decodeError (tool field) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the field." field)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what CWL expects." field)
                | None -> failwith (sprintf "%s must fail" field)
            Expect.equal (Decode.decodeCommandLineTool (tool "doc: [\"42\", 'true']\n")).Doc (Some (DocText.literal "42true")) "Quoted items are strings."

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
            tool.Doc <- Some (DocText.literal "other line of the text second line of the text\n")
            let sameLengths = Encode.encodeToolDescription tool
            Expect.stringContains sameLengths "doc: >\n  other line of the text\n  second line of the text\n" "The lines of the author should stay for a text with lines of the same lengths."
            tool.Doc <- Some (DocText.literal "first line of the text, now a longer one second line of the text\n")
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
                    ListEntry.ofValues (ResizeArray<CWLOutput>()),
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ],
                    inputs = ListEntry.ofValues (ResizeArray<CWLInput>()),
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
            Expect.equal ((ListEntry.values decoded.Inputs.Value).[0].GetProperties(false) |> Seq.length) 0 "The nodes are not dynamic properties of an input."
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
    | Workflow workflow -> (ListEntry.values workflow.Steps).[0]
    | other -> failwith (sprintf "Expected a workflow but got %A" other)

let private stepInputsOf (text: string) : StepInput list = WorkflowStep.getInputValues (stepOf text) |> Seq.toList

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
            Expect.equal firstInputs.[0].Source (Some (OutputSource.Single (SchemaSaladString.Literal "a"))) "A string is a single source."
            Expect.equal firstInputs.[1].Source (Some (OutputSource.Multiple (literals [ "a"; "b" ]))) "A list of sources keeps its order."
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
                | Workflow workflow -> (ListEntry.values workflow.Outputs) |> Seq.toList
                | other -> failwith (sprintf "Expected a workflow but got %A" other)
            let text = outputs "[a/x, b/y]"
            List.iter2 (fun (a: CWLOutput) (b: CWLOutput) -> expectSame (sprintf "the output %s" a.Name) (box a) (box b)) (outputsOf text) (outputsOf text)
            expectSame "multiple output sources" (box (OutputSource.Multiple (literals [ "a"; "b" ]))) (box (OutputSource.Multiple (literals [ "a"; "b" ])))
            expectSame "a single output source" (box (OutputSource.Single (SchemaSaladString.Literal "a"))) (box (OutputSource.Single (SchemaSaladString.Literal "a")))
            Expect.notEqual (OutputSource.Multiple (literals [ "a"; "b" ])) (OutputSource.Multiple (literals [ "b"; "a" ])) "Another order is another list of sources."
            Expect.notEqual (OutputSource.Multiple (literals [ "a"; "b" ])) (OutputSource.Multiple (literals [ "a" ])) "Another number of sources is another list."
            Expect.equal (outputsOf (outputs "[a/x]")).[0].OutputSource (Some (OutputSource.Multiple (literals [ "a/x" ]))) "A list of one source is a list."
            Expect.equal (outputsOf (outputs "a/x")).[0].OutputSource (Some (OutputSource.Single (SchemaSaladString.Literal "a/x"))) "A string is a single source."
            Expect.notEqual (outputsOf (outputs "[a/x]")).[0] (outputsOf (outputs "a/x")).[0] "A list of one source stays different from a single source."
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
            expectSame "the input" (box (ListEntry.values first.Inputs.Value).[0]) (box (ListEntry.values second.Inputs.Value).[0])
            expectSame "the input binding" (box (ListEntry.values first.Inputs.Value).[0].InputBinding.Value) (box (ListEntry.values second.Inputs.Value).[0].InputBinding.Value)
            let changed = decode (tool "[[1, 2], [4]]")
            Expect.notEqual (ListEntry.values changed.Inputs.Value).[0] (ListEntry.values first.Inputs.Value).[0] "A changed element in a nested list makes the inputs unequal."
            Expect.notEqual (ListEntry.values (decode (tool "[[1, 2]]")).Inputs.Value).[0] (ListEntry.values first.Inputs.Value).[0] "A shorter nested list makes the inputs unequal."
            Expect.notEqual (ListEntry.values (decode (text.Replace("ext-list: [a, b]", "ext-list: [b, a]"))).Inputs.Value).[0] (ListEntry.values first.Inputs.Value).[0] "Another order of the elements makes the inputs unequal."
    ]

let testPortSchemaNesting =
    let tool (portLines: string list) =
        "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n" + String.concat "\n" portLines + "\noutputs: []\n"
    let enumSchema = Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "fast"; SchemaSaladString.Literal "slow" ]))

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
                    ListEntry.ofValues (ResizeArray [ CWLOutput("record", Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("a", CWLType.String) ]))) ]),
                    inputs = ListEntry.ofValues [ CWLInput("mode", enumSchema) ],
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]
                )
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "  mode:\n    type:\n      type: enum\n      symbols: [fast, slow]\n" "The enum input is a port object with the schema under type."
            Expect.stringContains encoded "  record:\n    type:\n      type: record\n      fields:\n        a:\n          type: string\n" "The record output is a port object with the schema under type."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The hand-built tool should read back as it was built."
        testCase "a hand-built enum input with a binding reads back equal" <| fun _ ->
            let hand =
                CWLToolDescription(
                    ResizeArray [],
                    inputs = ListEntry.ofValues [ CWLInput("mode", enumSchema, inputBinding = ListEntry.Value (InputBinding.create(prefix = (SchemaSaladString.Literal "--mode")))) ],
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]
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
    let inputOf (name: string) (tool: CWLToolDescription) = (ListEntry.values tool.Inputs.Value) |> Seq.find (fun input -> input.Name = name)

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
                    Expect.equal schema.Label (Some (SchemaSaladString.Literal "names")) "The label is typed."
                    Expect.isTrue (schema.TryGetPropertyValue "inputBinding" |> Option.isSome) "The binding stays on the schema."
                | other -> failwithf "Expected an array schema, found %A" other
            | other -> failwithf "Expected a type list, found %A" other
        testCase "a hand-built array with a label is not written as the shorthand" <| fun _ ->
            let labelled = InputArraySchema(CWLType.String, label = (SchemaSaladString.Literal "names"))
            let hand =
                CWLToolDescription(
                    ResizeArray [],
                    inputs = ListEntry.ofValues (ResizeArray [ CWLInput("a", Array labelled); CWLInput("b", Union (ResizeArray [ Null; Array labelled ])); CWLInput("c", Array (InputArraySchema(CWLType.String))) ]),
                    baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ]
                )
            let encoded = Encode.encodeToolDescription hand
            Expect.stringContains encoded "  a:\n    type:\n      type: array\n      items: string\n      label: names\n" "The array as the whole type keeps its label."
            Expect.stringContains encoded "  b:\n    - \"null\"\n    - type: array\n      items: string\n      label: names\n" "The optional array keeps its label."
            Expect.stringContains encoded "  c: string[]\n" "The plain array is the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) hand "The hand-built tool should read back as it was built."
        testCase "the text of an additional type needs a schema for an array with a label" <| fun _ ->
            let labelled = Array (InputArraySchema(CWLType.File, label = (SchemaSaladString.Literal "files")))
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
            let reads = (ListEntry.values decoded.Inputs.Value).[0]
            Expect.equal reads.Type_ (Some (Union (ResizeArray [ Null; CWLType.File ]))) "The type is an optional File."
            Expect.equal reads.Format (Some (InputFormat.Single (SchemaSaladString.Literal "edam:format_2330"))) "The format belongs to the port."
            Expect.isSome reads.SecondaryFiles "The secondaryFiles belong to the port."
            Expect.equal (ListEntry.values decoded.Inputs.Value).[1].Type_ (Some (Union (ResizeArray [ Null; CWLType.Directory ]))) "The type is an optional Directory."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "  reads:\n    type: File?\n    secondaryFiles: .bai\n    format: edam:format_2330\n  dir: Directory?\n" "The port keeps its fields and the types are the shorthand."
            Expect.equal (Decode.decodeCommandLineTool encoded) decoded "The written text should decode to the same tool."
        testCase "a File and a Directory are equal to themselves and to nothing else" <| fun _ ->
            Expect.equal CWLType.File CWLType.File "A File equals a File."
            Expect.equal CWLType.Directory CWLType.Directory "A Directory equals a Directory."
            Expect.notEqual CWLType.File CWLType.Directory "A File is no Directory."
            Expect.equal (hash CWLType.File) (hash CWLType.File) "Equal types hash alike."
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

let testSchemaSaladDirectivesInDocuments =
    let expressionTool (expressionLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: ExpressionTool"; "inputs: {}"; "outputs: {}" ] @ expressionLines) + "\n"

    testList "Schema Salad directives in documents" [
        testCase "the expression of an ExpressionTool keeps an include and writes it as a mapping" <| fun _ ->
            let decoded = Decode.decodeExpressionTool (expressionTool [ "expression:"; "  $include: expr.js" ])
            Expect.equal decoded.Expression (SchemaSaladString.Include "expr.js") "The expression is the include."
            let encoded = Encode.encodeExpressionToolDescription decoded
            Expect.stringContains encoded "expression: {$include: expr.js}\n" "The include is written as a flow mapping."
            Expect.equal (Decode.decodeExpressionTool encoded) decoded "The written text should decode to the same tool."
        testCase "a type may be an import wherever a type can stand, is written as the directive and reads back equal" <| fun _ ->
            let tool (portLines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  a:" ] @ (portLines |> List.map (fun line -> "    " + line)) @ [ "outputs: {}"; "" ])
            let imported = ImportedType "t.yml"
            let cases =
                [ "the type of a port", [ "type: {$import: t.yml}" ], imported, "type: {$import: t.yml}"
                  "a member of a type list", [ "type: [\"null\", {$import: t.yml}]" ], Union (ResizeArray [ Null; imported ]), "- {$import: t.yml}"
                  "the items of an array", [ "type:"; "  type: array"; "  items: {$import: t.yml}" ], Array (InputArraySchema(imported)), "items: {$import: t.yml}"
                  "a member of the items", [ "type:"; "  type: array"; "  items: [string, {$import: t.yml}]" ], Array (InputArraySchema(Union (ResizeArray [ String; imported ]))), "- {$import: t.yml}"
                  "the type of a record field", [ "type:"; "  type: record"; "  fields:"; "    f:"; "      type: {$import: t.yml}" ], Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("f", imported) ])), "type: {$import: t.yml}" ]
            for (name, lines, expected, written) in cases do
                let decoded = Decode.decodeCommandLineTool (tool lines)
                Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some expected) (sprintf "%s: the type is the import." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded written (sprintf "%s: the import is written as a flow mapping.\n%s" name encoded)
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should decode to the same tool." name)
            Expect.notEqual imported (Reference "t.yml") "An import differs from a reference."
            Expect.notEqual imported (ImportedType "u.yml") "Imports of two files differ."
            Expect.equal (hash imported) (hash (ImportedType "t.yml")) "Equal imports hash alike."
        testCase "a type may be an include wherever a type can stand, because the text of the file can name the type" <| fun _ ->
            let tool (portLines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  a:" ] @ (portLines |> List.map (fun line -> "    " + line)) @ [ "outputs: {}"; "" ])
            let included = IncludedType "t.txt"
            let cases =
                [ "the type of a port", [ "type: {$include: t.txt}" ], included, "type: {$include: t.txt}"
                  "a member of a type list", [ "type: [\"null\", {$include: t.txt}]" ], Union (ResizeArray [ Null; included ]), "- {$include: t.txt}"
                  "the items of an array", [ "type:"; "  type: array"; "  items: {$include: t.txt}" ], Array (InputArraySchema(included)), "items: {$include: t.txt}"
                  "the type of a record field", [ "type:"; "  type: record"; "  fields:"; "    f:"; "      type: {$include: t.txt}" ], Record (InputRecordSchema(fields = ListEntry.ofValues [ InputRecordField("f", included) ])), "type: {$include: t.txt}" ]
            for (name, lines, expected, written) in cases do
                let decoded = Decode.decodeCommandLineTool (tool lines)
                Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some expected) (sprintf "%s: the type is the include." name)
                let encoded = Encode.encodeToolDescription decoded
                Expect.stringContains encoded written (sprintf "%s: the include is written as a flow mapping.\n%s" name encoded)
                Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should decode to the same tool." name)
            Expect.notEqual included (ImportedType "t.txt") "An include differs from an import of the same path."
            Expect.notEqual included (Reference "t.txt") "An include differs from a reference."
            Expect.equal (hash included) (hash (IncludedType "t.txt")) "Equal includes hash alike."
        testCase "a directive with another field cannot stand for a type, and an include still cannot stand for the value of a map entry" <| fun _ ->
            let tool (typeLines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  a:" ] @ (typeLines |> List.map (fun line -> "    " + line)) @ [ "outputs: {}"; "" ])
            for (name, lines, path, message) in
                [ "an import with another field", [ "type: {$import: t.yml, items: string}" ], "inputs.a.type", "A directive has exactly one field"
                  "an include with another field", [ "type: {$include: t.yml, items: string}" ], "inputs.a.type", "A directive has exactly one field"
                  "an include as the value of a field", [ "type:"; "  type: record"; "  fields:"; "    f: {$include: t.yml}" ], "inputs.a.type.fields.f", "A directive cannot stand for the value of an entry of the fields field of a record" ] do
                match (try Decode.decodeCommandLineTool (tool lines) |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the type" name)
                    Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
                | None -> failwith (sprintf "%s must fail" name)
        testCase "the expression of an ExpressionTool keeps its literal layout" <| fun _ ->
            let decoded = Decode.decodeExpressionTool (expressionTool [ "expression: $(1 + 1)" ])
            Expect.equal decoded.Expression (SchemaSaladString.Literal "$(1 + 1)") "The expression is the literal."
            Expect.stringContains (Encode.encodeExpressionToolDescription decoded) "expression: \"$(1 + 1)\"" "A literal expression is double-quoted."
        testCase "a mapping that is no directive as the expression is a decode error at the expression" <| fun _ ->
            match (try Decode.decodeExpressionTool (expressionTool [ "expression:"; "  foo: bar" ]) |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "expression" "The error shows the field."
                Expect.stringContains error.Message "Expected a string or a directive, found a mapping" "The error says what is wrong."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A mapping without a directive must fail."
        testCase "the when of a step keeps an include" <| fun _ ->
            let yaml = String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: tool.cwl"; "    in: {}"; "    out: []"; "    when:"; "      $include: when.txt"; "" ]
            let decoded = Decode.decodeWorkflow yaml
            Expect.equal (ListEntry.values decoded.Steps).[0].When_ (Some (SchemaSaladString.Include "when.txt")) "The when is the include."
            let encoded = Encode.encodeWorkflowDescription decoded
            Expect.stringContains encoded "when: {$include: when.txt}\n" "The include is written as a flow mapping."
            Expect.equal (ListEntry.values (Decode.decodeWorkflow encoded).Steps).[0].When_ (ListEntry.values decoded.Steps).[0].When_ "The written text should decode to the same when."
    ]

let testInvalidDocuments =
    let decodeError (decode: unit -> unit) =
        try
            decode ()
            None
        with YamlDecodeException error -> Some error
    let expectError (name: string) (path: string) (message: string) (decode: unit -> unit) =
        match decodeError decode with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
    let workflow (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow" ] @ lines) + "\n"
    let stepWith (stepLines: string list) =
        workflow ([ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl" ] @ stepLines)

    testList "Invalid documents" [
        testCase "out of a step is a list, and neither out: {} nor out: o is one" <| fun _ ->
            expectError "an empty mapping" "steps.s.out" "Expected a list of outputs, found a mapping" (fun () -> Decode.decodeWorkflow (stepWith [ "    in: {}"; "    out: {}" ]) |> ignore)
            expectError "a scalar" "steps.s.out" "Expected a list of outputs, found the text \"o\"" (fun () -> Decode.decodeWorkflow (stepWith [ "    in: {}"; "    out: o" ]) |> ignore)
            let listed = Decode.decodeWorkflow (stepWith [ "    in: {}"; "    out: [o]" ])
            Expect.equal (ListEntry.values listed.Steps).[0].Out.Count 1 "A list of one output is valid."
        testCase "a baseCommand that is a mapping is a decode error at the field" <| fun _ ->
            expectError "a mapping" "baseCommand" "Expected a string or a list of strings for baseCommand, found a mapping" (fun () ->
                Decode.decodeCommandLineTool (String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand:"; "  a: b"; "inputs: {}"; "outputs: {}"; "" ]) |> ignore)
        testCase "the inputBinding of a workflow or an expression tool input holds loadContents only and an operation input has none" <| fun _ ->
            let withBinding (header: string list) (binding: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2" ] @ header @ [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:" ] @ (binding |> List.map (fun line -> "      " + line)) @ [ "outputs: {}" ] @ (if header.Head = "class: Workflow" then [ "steps: {}" ] elif header.Head = "class: ExpressionTool" then [ "expression: $(1)" ] else []) @ [ "" ])
            let workflowHeader = [ "class: Workflow" ]
            let expressionHeader = [ "class: ExpressionTool" ]
            let operationHeader = [ "class: Operation" ]
            let commandLineHeader = [ "class: CommandLineTool"; "baseCommand: echo" ]
            for field in [ "position: 1"; "prefix: -x"; "itemSeparator: \",\""; "separate: false"; "valueFrom: $(self)"; "shellQuote: false" ] do
                let key = field.Substring(0, field.IndexOf ':')
                expectError ("workflow, " + key) ("inputs.x.inputBinding." + key) "has loadContents only" (fun () -> Decode.decodeWorkflow (withBinding workflowHeader [ field ]) |> ignore)
                expectError ("expression tool, " + key) ("inputs.x.inputBinding." + key) "has loadContents only" (fun () -> Decode.decodeExpressionTool (withBinding expressionHeader [ field ]) |> ignore)
            expectError "operation" "inputs.x.inputBinding" "An input of an operation has no inputBinding" (fun () -> Decode.decodeOperation (withBinding operationHeader [ "loadContents: true" ]) |> ignore)
            Expect.equal (inputBindingOf (ListEntry.values (Decode.decodeWorkflow (withBinding workflowHeader [ "loadContents: true" ])).Inputs).[0]).LoadContents (Some (SchemaSaladBool.Value true)) "A workflow input may load contents."
            Expect.equal (inputBindingOf (ListEntry.values (Decode.decodeExpressionTool (withBinding expressionHeader [ "loadContents: true" ])).Inputs.Value).[0]).LoadContents (Some (SchemaSaladBool.Value true)) "An expression tool input may load contents."
            Expect.equal (inputBindingOf (ListEntry.values (Decode.decodeCommandLineTool (withBinding commandLineHeader [ "position: 1"; "prefix: -x" ])).Inputs.Value).[0]).Position (Some (BindingPosition.Number 1)) "A command line tool input has the full binding."
        testCase "the loadListing of a step input is a symbol of CWL" <| fun _ ->
            let step (loadListing: string) = stepWith [ "    in:"; "      x:"; "        default: 1"; "        loadListing: " + loadListing; "    out: []" ]
            expectError "a word" "steps.s.in.x.loadListing" "Expected one of no_listing, shallow_listing and deep_listing" (fun () -> Decode.decodeWorkflow (step "foo") |> ignore)
            Expect.equal (WorkflowStep.getInputValues (ListEntry.values (Decode.decodeWorkflow (step "deep_listing")).Steps).[0]).[0].LoadListing (Some LoadListingEnum.DeepListing) "A symbol is typed."
        testCase "outputSource belongs to the output of a workflow only" <| fun _ ->
            let toolOutput = tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: s/o" ]
            expectError "a tool" "outputs.o.outputSource" "An output of a command line tool has no outputSource" (fun () -> Decode.decodeCommandLineTool toolOutput |> ignore)
            expectError "an expression tool" "outputs.o.outputSource" "An output of an expression tool has no outputSource" (fun () ->
                Decode.decodeExpressionTool (String.concat "\n" [ "cwlVersion: v1.2"; "class: ExpressionTool"; "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: s/o"; "expression: $(1)"; "" ]) |> ignore)
            let decoded = Decode.decodeWorkflow (workflow [ "inputs:"; "  x: string"; "outputs:"; "  o:"; "    type: string"; "    outputSource: x"; "steps: {}" ])
            Expect.equal (ListEntry.values decoded.Outputs).[0].OutputSource (Some (OutputSource.Single (SchemaSaladString.Literal "x"))) "A workflow output names its source."
        testCase "outputBinding belongs to the output of a command line tool only" <| fun _ ->
            let output (extra: string) = [ "outputs:"; "  o:"; "    type: string"; "    " + extra ]
            let expressionTool (extra: string) = String.concat "\n" ([ "cwlVersion: v1.2"; "class: ExpressionTool"; "inputs: {}" ] @ output extra @ [ "expression: $(1)"; "" ])
            let operation (extra: string) = String.concat "\n" ([ "cwlVersion: v1.2"; "class: Operation"; "inputs: {}" ] @ output extra @ [ "" ])
            let binding = "outputBinding: {glob: a}"
            expectError "a workflow" "outputs.o.outputBinding" "An output of a workflow has no outputBinding. Only an output of a command line tool binds files." (fun () ->
                Decode.decodeWorkflow (workflow ([ "inputs: {}"; "steps: {}" ] @ output binding)) |> ignore)
            expectError "an expression tool" "outputs.o.outputBinding" "An output of an expression tool has no outputBinding" (fun () -> Decode.decodeExpressionTool (expressionTool binding) |> ignore)
            expectError "an operation" "outputs.o.outputBinding" "An output of an operation has no outputBinding" (fun () -> Decode.decodeOperation (operation binding) |> ignore)
            // The rules are those of v1.2 whatever cwlVersion says, so an older or a later version changes nothing.
            for version in [ "v1.0"; "v1.3" ] do
                expectError ("a workflow of " + version) "outputs.o.outputBinding" "An output of a workflow has no outputBinding" (fun () ->
                    Decode.decodeWorkflow ((workflow ([ "inputs: {}"; "steps: {}" ] @ output binding)).Replace("cwlVersion: v1.2", "cwlVersion: " + version)) |> ignore)
            let decoded = Decode.decodeCommandLineTool (tool ([ "inputs: {}" ] @ output binding))
            Expect.equal (ListEntry.values decoded.Outputs).[0].OutputBinding (Some (ListEntry.Value (OutputBinding(glob = OutputGlob.Single (SchemaSaladString.Literal "a"))))) "A tool output binds files."
        testCase "linkMerge and pickValue belong to the output of a workflow only" <| fun _ ->
            let output (extra: string) = [ "outputs:"; "  o:"; "    type: string"; "    " + extra ]
            let expressionTool (extra: string) = String.concat "\n" ([ "cwlVersion: v1.2"; "class: ExpressionTool"; "inputs: {}" ] @ output extra @ [ "expression: $(1)"; "" ])
            let operation (extra: string) = String.concat "\n" ([ "cwlVersion: v1.2"; "class: Operation"; "inputs: {}" ] @ output extra @ [ "" ])
            for field in [ "linkMerge: merge_nested"; "pickValue: first_non_null" ] do
                let key = field.Substring(0, field.IndexOf ':')
                let path = "outputs.o." + key
                expectError ("a tool, " + key) path ("An output of a command line tool has no " + key) (fun () -> Decode.decodeCommandLineTool (tool ([ "inputs: {}" ] @ output field)) |> ignore)
                expectError ("an expression tool, " + key) path ("An output of an expression tool has no " + key) (fun () -> Decode.decodeExpressionTool (expressionTool field) |> ignore)
                expectError ("an operation, " + key) path ("An output of an operation has no " + key) (fun () -> Decode.decodeOperation (operation field) |> ignore)
            for version in [ "v1.0"; "v1.2" ] do
                expectError ("a tool of " + version + ", linkMerge") "outputs.o.linkMerge" "An output of a command line tool has no linkMerge" (fun () ->
                    Decode.decodeCommandLineTool ((tool ([ "inputs: {}" ] @ output "linkMerge: merge_nested")).Replace("cwlVersion: v1.2", "cwlVersion: " + version)) |> ignore)
            let yaml = workflow [ "inputs:"; "  x: string"; "steps: {}"; "outputs:"; "  o:"; "    type: string[]"; "    outputSource: [x, x]"; "    linkMerge: merge_flattened"; "    pickValue: all_non_null" ]
            let decoded = Decode.decodeWorkflow yaml
            Expect.equal (ListEntry.values decoded.Outputs).[0].OutputSource (Some (OutputSource.Multiple (literals [ "x"; "x" ]))) "A workflow output names its sources."
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "linkMerge: merge_flattened" "The workflow output keeps its linkMerge."
            Expect.stringContains written "pickValue: all_non_null" "The workflow output keeps its pickValue."
            Expect.equal (Decode.decodeWorkflow written) decoded "The written text should decode to the same workflow."
        testCase "an id that occurs twice in the list form of step inputs is a warning and both entries are written back as a list" <| fun _ ->
            let yaml = workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      - id: a"; "        source: x"; "      - id: b"; "        source: y"; "      - id: a"; "        source: [y, z]"; "    out: []" ]
            let decoded = Decode.decodeWorkflowWithWarnings yaml
            Expect.equal (WorkflowStep.getInputValues (ListEntry.values decoded.Value.Steps).[0] |> Seq.map (fun input -> input.Id) |> List.ofSeq) [ "a"; "b"; "a" ] "Every entry stays in the list, in order."
            Expect.equal decoded.Warnings.Count 1 "The repeated id is one warning."
            Expect.equal decoded.Warnings.[0].Path "steps.s.in[2]" "The warning shows the repeated entry."
            Expect.stringContains decoded.Warnings.[0].Message "The id a belongs to an earlier entry of the list too" "The warning names the id."
            Expect.isSome decoded.Warnings.[0].Raw "The warning shows the entry."
            let written = Encode.encodeWorkflowDescription decoded.Value
            Expect.stringContains written "- id: a" "The inputs are written as a list of objects with their ids."
            Expect.equal (Decode.decodeWorkflow written) decoded.Value "The written text should decode to the same workflow."
            let plain = Decode.decodeWorkflow (workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a: x"; "      b: [y, z]"; "    out: []" ])
            Expect.stringContains (Encode.encodeWorkflowDescription plain) "a: x" "Distinct ids are written as a map."
        testCase "the out field of a step lists outputs of the inline process that the step runs" <| fun _ ->
            let inlineStep (run: string list) (outLines: string list) =
                workflow ([ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run:" ] @ (run |> List.map (fun line -> "      " + line)) @ [ "    in: {}" ] @ outLines)
            let toolRun = [ "class: CommandLineTool"; "baseCommand: echo"; "inputs: {}"; "outputs:"; "  r: string"; "  q: string" ]
            let workflowRun = [ "class: Workflow"; "inputs: {}"; "steps: {}"; "outputs:"; "  w:"; "    type: string"; "    outputSource: x" ]
            let expressionRun = [ "class: ExpressionTool"; "expression: $(1)"; "inputs: {}"; "outputs:"; "  e: int" ]
            let operationRun = [ "class: Operation"; "inputs: {}"; "outputs:"; "  o: string" ]
            let message = "The out field lists the output nope, and the process that the step runs has no output of that name."
            expectError "a tool" "steps.s.out[1]" (message + " The process has the outputs r, q.") (fun () ->
                Decode.decodeWorkflow (inlineStep toolRun [ "    out: [r, nope]" ]) |> ignore)
            expectError "a workflow" "steps.s.out[0]" (message + " The process has the outputs w.") (fun () ->
                Decode.decodeWorkflow (inlineStep workflowRun [ "    out: [nope]" ]) |> ignore)
            expectError "an expression tool" "steps.s.out[0]" (message + " The process has the outputs e.") (fun () ->
                Decode.decodeWorkflow (inlineStep expressionRun [ "    out: [nope]" ]) |> ignore)
            expectError "an operation" "steps.s.out[0]" (message + " The process has the outputs o.") (fun () ->
                Decode.decodeWorkflow (inlineStep operationRun [ "    out: [nope]" ]) |> ignore)
            expectError "an entry in the object form" "steps.s.out[0].id" message (fun () ->
                Decode.decodeWorkflow (inlineStep toolRun [ "    out:"; "      - id: nope" ]) |> ignore)
            expectError "a process without outputs" "steps.s.out[0]" "The process has no outputs." (fun () ->
                Decode.decodeWorkflow (inlineStep [ "class: CommandLineTool"; "baseCommand: echo"; "inputs: {}"; "outputs: {}" ] [ "    out: [nope]" ]) |> ignore)
            expectError "a step in a list" "steps.s.out[0]" message (fun () ->
                Decode.decodeWorkflow (workflow ([ "inputs: {}"; "outputs: {}"; "steps:"; "  - id: s"; "    run:" ] @ (toolRun |> List.map (fun line -> "      " + line)) @ [ "    in: {}"; "    out: [nope]" ])) |> ignore)
            // cwltool compares the short names of the ids, so a name with a hash or with a step in front of it names the output too.
            for name in [ "r"; "\"#r\""; "s/r"; "\"#q\"" ] do
                let decoded = Decode.decodeWorkflow (inlineStep toolRun [ "    out: [" + name + "]" ])
                Expect.equal (ListEntry.values decoded.Steps).[0].Out.Count 1 (sprintf "The name %s lists an output of the tool." name)
            Expect.equal (ListEntry.values (Decode.decodeWorkflow (inlineStep toolRun [ "    out: []" ])).Steps).[0].Out.Count 0 "A step lists no output at all."
            Expect.equal (ListEntry.values (Decode.decodeWorkflow (inlineStep toolRun [ "    out: [r, r]" ])).Steps).[0].Out.Count 2 "A step may list an output twice, as cwltool accepts it."
        testCase "the out field of a step that runs a process of a packed document by #id lists outputs of that process and a file is not read" <| fun _ ->
            let packed (outEntries: string) (stepFirst: bool) =
                let mainProcess = [ "  - id: main"; "    class: Workflow"; "    inputs: {}"; "    outputs: {}"; "    steps:"; "      s:"; "        run: \"#tool\""; "        in: {}"; "        out: [" + outEntries + "]" ]
                let toolProcess = [ "  - id: tool"; "    class: CommandLineTool"; "    baseCommand: echo"; "    inputs: {}"; "    outputs:"; "      r: string" ]
                String.concat "\n" ([ "cwlVersion: v1.2"; "$graph:" ] @ (if stepFirst then mainProcess @ toolProcess else toolProcess @ mainProcess)) + "\n"
            for (name, path, stepFirst) in [ "a process behind the step", "$graph[0].steps.s.out[1]", true; "a process before the step", "$graph[1].steps.s.out[1]", false ] do
                expectError name path "The out field lists the output nope, and the process that the step runs has no output of that name. The process has the outputs r." (fun () ->
                    Decode.decodeCWLDocument (packed "r, nope" stepFirst) |> ignore)
            for name in [ "r"; "\"#r\""; "tool/r" ] do
                Expect.isTrue (match Decode.decodeCWLDocument (packed name true) with CWLDocument.Graph graph -> graph.Processes.Count = 2 | _ -> false) (sprintf "The name %s lists the output of the tool." name)
            let viaFile = workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: tool.cwl"; "    in: {}"; "    out: [anything]" ]
            Expect.equal (ListEntry.values (Decode.decodeWorkflow viaFile).Steps).[0].Out.Count 1 "The decoder reads no file, so the out field of a step that runs a file stays as written."
        testCase "an id that occurs twice in the list form of inputs, outputs or steps is a warning at the second and the list is written back" <| fun _ ->
            let inputs = Decode.decodeCommandLineToolWithWarnings (tool [ "inputs:"; "  - id: x"; "    type: string"; "  - id: y"; "    type: int"; "  - id: x"; "    type: int"; "outputs: []" ])
            Expect.equal ((ListEntry.values inputs.Value.Inputs.Value) |> Seq.map (fun input -> input.Name, input.Type_) |> List.ofSeq) [ "x", Some String; "y", Some Int; "x", Some Int ] "Every input stays in the list, in order."
            Expect.equal (inputs.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ "inputs[2]" ] "The warning shows the repeated entry."
            Expect.stringContains inputs.Warnings.[0].Message "The id x belongs to an earlier entry of the list too" "The warning names the id."
            let hashed = Decode.decodeCommandLineToolWithWarnings (tool [ "inputs:"; "  - id: x"; "    type: string"; "  - id: \"#x\""; "    type: int"; "outputs: []" ])
            Expect.equal (hashed.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ "inputs[1]" ] "An id with a hash is the id without the hash."
            let outputs = Decode.decodeCommandLineToolWithWarnings (tool [ "inputs: []"; "outputs:"; "  - id: o"; "    type: string"; "  - id: o"; "    type: int" ])
            Expect.equal ((ListEntry.values outputs.Value.Outputs) |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "o"; "o" ] "Both outputs stay in the list."
            Expect.equal (outputs.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ "outputs[1]" ] "The warning shows the repeated entry."
            let steps = Decode.decodeWorkflowWithWarnings (workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  - id: s"; "    run: t.cwl"; "    in: {}"; "    out: []"; "  - id: s"; "    run: u.cwl"; "    in: {}"; "    out: []" ])
            Expect.equal ((ListEntry.values steps.Value.Steps) |> Seq.map (fun step -> step.Id, step.Run) |> List.ofSeq) [ Some "s", RunString "t.cwl"; Some "s", RunString "u.cwl" ] "Both steps stay in the list."
            Expect.equal (steps.Warnings |> Seq.map (fun warning -> warning.Path) |> List.ofSeq) [ "steps[1]" ] "The warning shows the repeated entry."
            // A mapping holds a key once, so the lists with a repeated id are written as lists and read back equal.
            let writtenInputs = Encode.encodeToolDescription inputs.Value
            Expect.stringContains writtenInputs "- id: x" "The inputs are written as a list of objects with their ids."
            Expect.equal (Decode.decodeCommandLineTool writtenInputs) inputs.Value "The written inputs should decode to the same tool."
            let writtenOutputs = Encode.encodeToolDescription outputs.Value
            Expect.equal (Decode.decodeCommandLineTool writtenOutputs) outputs.Value "The written outputs should decode to the same tool."
            let writtenSteps = Encode.encodeWorkflowDescription steps.Value
            Expect.stringContains writtenSteps "- id: s" "The steps are written as a list of objects with their ids."
            Expect.equal (Decode.decodeWorkflow writtenSteps) steps.Value "The written steps should decode to the same workflow."
            Expect.isFalse ((Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool [ "inputs:"; "  x: string"; "  y: int"; "outputs: []" ]))).Contains "- id:") "Distinct ids are written as a map."
            Expect.equal (Decode.decodeCommandLineTool (tool [ "inputs:"; "  - id: x"; "    type: string"; "  - id: y"; "    type: int"; "outputs: []" ])).Inputs.Value.Count 2 "Distinct ids are valid."
        testCase "a value of an enum field that CWL does not know, an unknown class and a run of another shape are decode errors with a place" <| fun _ ->
            let step (lines: string list) = stepWith ([ "    in:"; "      x:"; "        source: a" ] @ lines @ [ "    out: []" ])
            expectError "linkMerge" "steps.s.in.x.linkMerge" "Invalid linkMerge value: merge_all" (fun () ->
                Decode.decodeWorkflow (stepWith [ "    in:"; "      x:"; "        source: [a, b]"; "        linkMerge: merge_all"; "    out: []" ]) |> ignore)
            expectError "pickValue" "steps.s.in.x.pickValue" "Invalid pickValue value: any" (fun () ->
                Decode.decodeWorkflow (stepWith [ "    in:"; "      x:"; "        source: [a, b]"; "        pickValue: any"; "    out: []" ]) |> ignore)
            expectError "scatterMethod" "steps.s.scatterMethod" "Invalid scatterMethod value: cross" (fun () ->
                Decode.decodeWorkflow (step [ "    scatter: x"; "    scatterMethod: cross" ]) |> ignore)
            expectError "class" "class" "Invalid or unsupported CWL class: Pipeline" (fun () ->
                Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: Pipeline\ninputs: {}\noutputs: {}\n" |> ignore)
            expectError "run" "steps.s.run" "Expected a path or a process for the run field of a step, found a list" (fun () ->
                Decode.decodeWorkflow (workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: [a]"; "    in: {}"; "    out: []" ]) |> ignore)
        testCase "the fields of a record that are no list and no mapping are reported without a dump of the node" <| fun _ ->
            let yaml = tool [ "inputs:"; "  r:"; "    type:"; "      type: record"; "      fields: abc"; "outputs: []" ]
            expectError "fields" "inputs.r.type.fields" "Expected a list of fields or a mapping of fields, found the text \"abc\"." (fun () -> Decode.decodeCommandLineTool yaml |> ignore)
        testCase "the encoder writes a packed document with a member without an id or with a repeated id and it reads back" <| fun _ ->
            let member' (id: string option) =
                CWLProcessingUnit.CommandLineTool (CWLToolDescription(ResizeArray [], baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ], inputs = ResizeArray [], ?id = id))
            for (name, members) in [ "two members with one id", [ member' (Some "t"); member' (Some "t") ]
                                     "a repeated id with a hash", [ member' (Some "t"); member' (Some "#t") ]
                                     "a member without an id", [ member' None ]
                                     "a member with an empty id", [ member' (Some "") ]
                                     "distinct ids", [ member' (Some "a"); member' (Some "b") ] ] do
                let graph = CWLGraph(ResizeArray members, cwlVersion = "v1.2")
                let written = Encode.encodeCWLGraph graph
                match Decode.decodeCWLDocument written with
                | CWLDocument.Graph again -> Expect.equal again graph (sprintf "%s: the written graph should decode to the same graph." name)
                | other -> failwith (sprintf "%s: expected a packed document but got %A" name other)
    ]

let testStrictStrings =
    let decodeError (yaml: string) =
        try
            Decode.decodeCWLDocument yaml |> ignore
            None
        with YamlDecodeException error -> Some error
    let toolHead = "cwlVersion: v1.2\nclass: CommandLineTool\n"
    let tool (lines: string list) = toolHead + String.concat "\n" lines + "\n"
    let workflow (lines: string list) = "cwlVersion: v1.2\nclass: Workflow\n" + String.concat "\n" lines + "\n"
    let step (stepLines: string list) =
        workflow ([ "inputs: {}"; "outputs: {}"; "steps:"; "  s:" ] @ (stepLines |> List.map (fun line -> "    " + line)))

    testList "Strict string fields" [
        testCase "a plain int, bool or float in a string field is a decode error at the field and a quoted spelling is a string" <| fun _ ->
            let port (field: string) = tool [ "baseCommand: cat"; "inputs:"; "  x:"; "    type: string"; "    " + field; "outputs: {}" ]
            let output (field: string) = tool [ "baseCommand: cat"; "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    " + field ]
            let requirement (lines: string list) = tool ([ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "requirements:" ] @ lines)
            // The path of the error, the document with {0} where the scalar stands, and whether the quoted spelling decodes. A position that
            // CWL types as an expression refuses a text without one, so its quoted spelling does not decode.
            let cases =
                [ "outputs.o.outputBinding.glob", output "outputBinding: {glob: {0}}", true
                  "outputs.o.outputBinding.glob[1]", output "outputBinding: {glob: [a, {0}]}", true
                  "outputs.o.outputBinding.outputEval", output "outputBinding: {outputEval: {0}}", false
                  "outputs.o.format", output "format: {0}", true
                  "inputs.x.inputBinding.prefix", port "inputBinding: {prefix: {0}}", true
                  "inputs.x.inputBinding.itemSeparator", port "inputBinding: {itemSeparator: {0}}", true
                  "inputs.x.inputBinding.valueFrom", port "inputBinding: {valueFrom: {0}}", true
                  "inputs.x.format", port "format: {0}", true
                  "inputs.x.format[1]", port "format: [a, {0}]", true
                  "baseCommand", tool [ "baseCommand: {0}"; "inputs: {}"; "outputs: {}" ], true
                  "baseCommand[1]", tool [ "baseCommand: [echo, {0}]"; "inputs: {}"; "outputs: {}" ], true
                  "stdin", tool [ "baseCommand: cat"; "stdin: {0}"; "inputs: {}"; "outputs: {}" ], true
                  "stdout", tool [ "baseCommand: cat"; "stdout: {0}"; "inputs: {}"; "outputs: {}" ], true
                  "stderr", tool [ "baseCommand: cat"; "stderr: {0}"; "inputs: {}"; "outputs: {}" ], true
                  "intent[1]", tool [ "baseCommand: cat"; "intent: [a, {0}]"; "inputs: {}"; "outputs: {}" ], true
                  "id", tool [ "id: {0}"; "baseCommand: cat"; "inputs: {}"; "outputs: {}" ], true
                  "class", "cwlVersion: v1.2\nclass: {0}\ninputs: {}\noutputs: {}\n", false
                  "requirements[0].class", requirement [ "  - {class: {0}}" ], false
                  "requirements[0].dockerPull", requirement [ "  - {class: DockerRequirement, dockerPull: {0}}" ], true
                  "requirements[0].dockerImageId", requirement [ "  - {class: DockerRequirement, dockerImageId: {0}}" ], true
                  "requirements[0].envDef[0].envName", requirement [ "  - class: EnvVarRequirement"; "    envDef: [{envName: {0}, envValue: a}]" ], true
                  "requirements[0].envDef[0].envValue", requirement [ "  - class: EnvVarRequirement"; "    envDef: [{envName: A, envValue: {0}}]" ], true
                  "requirements[0].packages[0].package", requirement [ "  - class: SoftwareRequirement"; "    packages: [{package: {0}}]" ], true
                  "requirements[0].packages[0].version[0]", requirement [ "  - class: SoftwareRequirement"; "    packages: [{package: p, version: [{0}]}]" ], true
                  "requirements[0].packages[0].specs[0]", requirement [ "  - class: SoftwareRequirement"; "    packages: [{package: p, specs: [{0}]}]" ], true
                  "requirements[0].expressionLib[0]", requirement [ "  - {class: InlineJavascriptRequirement, expressionLib: [{0}]}" ], true
                  "steps.s.scatter", step [ "run: t.cwl"; "in: {}"; "out: []"; "scatter: {0}" ], true
                  "steps.s.scatter[0]", step [ "run: t.cwl"; "in: {}"; "out: []"; "scatter: [{0}]" ], true
                  "steps.s.when", step [ "run: t.cwl"; "in: {}"; "out: []"; "when: {0}" ], false
                  "steps.s.out[0]", step [ "run: t.cwl"; "in: {}"; "out: [{0}]" ], true
                  "steps.s.run", step [ "run: {0}"; "in: {}"; "out: []" ], true
                  "steps.s.in.x", step [ "run: t.cwl"; "in: {x: {0}}"; "out: []" ], true
                  "steps.s.in.x.source", step [ "run: t.cwl"; "in: {x: {source: {0}}}"; "out: []" ], true
                  "steps.s.in.x.source[1]", step [ "run: t.cwl"; "in: {x: {source: [a, {0}]}}"; "out: []" ], true
                  "steps.s.in.x.valueFrom", step [ "run: t.cwl"; "in: {x: {valueFrom: {0}}}"; "out: []" ], true
                  "outputs.o.outputSource", workflow [ "inputs: {}"; "steps: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: {0}" ], true ]
            for (path, template, quotedDecodes) in cases do
                for text in [ "5"; "true"; "1.5"; ".5e3"; "+.5e3"; "017"; "0o17"; "0x1F" ] do
                    match decodeError (template.Replace("{0}", text)) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "The error for %s should show the field:\n%s" text template)
                        let expected = if path.EndsWith "outputEval" || path.EndsWith ".when" then "Expected an expression, found" else "Expected a string, found the plain scalar"
                        Expect.stringContains error.Message expected (sprintf "The error for %s at %s should say what CWL expects." text path)
                        Expect.isSome error.Span (sprintf "The error for %s at %s should have the place in the text." text path)
                    | None -> failwith (sprintf "A plain %s at %s must fail:\n%s" text path template)
                if quotedDecodes then
                    Expect.isNone (decodeError (template.Replace("{0}", "\"5\""))) (sprintf "A quoted 5 at %s is a string." path)
                    // The core schema of YAML 1.2.2 reads these plain spellings as strings.
                    for text in [ "1_000"; "0b101"; "1_0.5"; "yes"; "0X1F"; "2001-12-14"; "1:30"; "="; "<<"; "._" ] do
                        Expect.isNone (decodeError (template.Replace("{0}", text))) (sprintf "A plain %s at %s is a string." text path)
        testCase "a plain scalar key of an id map is a decode error at the key and a quoted key is a name" <| fun _ ->
            let cases =
                [ "inputs", tool [ "baseCommand: cat"; "inputs:"; "  {0}: string"; "outputs: {}" ]
                  "outputs", tool [ "baseCommand: cat"; "inputs: {}"; "outputs:"; "  {0}: string" ]
                  "requirements", tool [ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "requirements:"; "  {0}: {}" ]
                  "hints", tool [ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "hints:"; "  {0}: {}" ]
                  "requirements[0].envDef", tool [ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "requirements:"; "  - class: EnvVarRequirement"; "    envDef:"; "      {0}: a" ]
                  "requirements[0].packages", tool [ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "requirements:"; "  - class: SoftwareRequirement"; "    packages:"; "      {0}: []" ]
                  "inputs.x.type.fields", tool [ "baseCommand: cat"; "inputs:"; "  x:"; "    type:"; "      type: record"; "      fields:"; "        {0}: string"; "outputs: {}" ]
                  "steps", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  {0}:"; "    run: t.cwl"; "    in: {}"; "    out: []" ]
                  "steps.s.in", step [ "run: t.cwl"; "in:"; "  {0}: a"; "out: []" ] ]
            for (path, template) in cases do
                for text in [ "5"; "true"; "1.5"; "null" ] do
                    match decodeError (template.Replace("{0}", text)) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "The error for the key %s should show the map:\n%s" text template)
                        Expect.stringContains error.Message "Expected a string, found" (sprintf "The error for the key %s at %s should say what CWL expects." text path)
                    | None -> failwith (sprintf "A plain %s as a key at %s must fail:\n%s" text path template)
                // A quoted key of requirements is a class name, and no class is called 5.
                if path <> "requirements" then
                    Expect.isNone (decodeError (template.Replace("{0}", "\"5\""))) (sprintf "A quoted 5 as a key at %s is a name." path)
        testCase "a plain bool where CWL asks for a number is a decode error at the field" <| fun _ ->
            let cases =
                [ "requirements[0].timelimit", [ "  - class: ToolTimeLimit"; "    timelimit: true" ]
                  "requirements[0].coresMin", [ "  - class: ResourceRequirement"; "    coresMin: true" ]
                  "requirements[0].ramMin", [ "  - class: ResourceRequirement"; "    ramMin: false" ] ]
            for (path, lines) in cases do
                match decodeError (tool ([ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "requirements:" ] @ lines)) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path "The error should show the field."
                    Expect.isSome error.Span "The error should have the place in the text."
                | None -> failwith (sprintf "A plain bool at %s must fail." path)
        testCase "a float, a quoted number or a bool in a list of codes is a decode error at the code" <| fun _ ->
            for (path, yaml) in [ "successCodes[0]", tool [ "baseCommand: cat"; "successCodes: [1.5]"; "inputs: {}"; "outputs: {}" ]
                                  "successCodes[0]", tool [ "baseCommand: cat"; "successCodes: [\"0\"]"; "inputs: {}"; "outputs: {}" ]
                                  "successCodes[0]", tool [ "baseCommand: cat"; "successCodes: [true, 0, 3]"; "inputs: {}"; "outputs: {}" ]
                                  "temporaryFailCodes[0]", tool [ "baseCommand: cat"; "temporaryFailCodes: [false]"; "inputs: {}"; "outputs: {}" ] ] do
                match decodeError yaml with
                | Some error -> Expect.equal (DecodeError.pathText error) path "A float, a quoted number or a bool is no int."
                | None -> failwith (sprintf "%s must fail:\n%s" path yaml)
        testCase "a plain int or float where CWL asks for a bool or an expression is a decode error" <| fun _ ->
            for (path, template) in [ "requirements[0].enableReuse", "  - {class: WorkReuse, enableReuse: {0}}"; "requirements[0].networkAccess", "  - {class: NetworkAccess, networkAccess: {0}}" ] do
                for text in [ "5"; "1.5" ] do
                    let yaml = tool [ "baseCommand: cat"; "inputs: {}"; "outputs: {}"; "requirements:"; template.Replace("{0}", text) ]
                    match decodeError yaml with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "The error for %s should show the field." text)
                        Expect.stringContains error.Message "Expected a bool or an expression, found" (sprintf "The error for %s at %s should say what CWL expects." text path)
                        Expect.isFalse (error.Message.Contains "quotes") (sprintf "The error for %s at %s should not advise quotes." text path)
                    | None -> failwith (sprintf "A plain %s at %s must fail." text path)
    ]

let testDirectivesInStringFields =
    let tool (lines: string list) =
        "cwlVersion: v1.2\nclass: CommandLineTool\n" + String.concat "\n" lines + "\n"
    let toolWith (lines: string list) = tool ([ "baseCommand: echo"; "inputs: {}"; "outputs: {}" ] @ lines)
    let port (portLines: string list) = tool ([ "baseCommand: echo"; "inputs:"; "  x:"; "    type: string" ] @ (portLines |> List.map (fun line -> "    " + line)) @ [ "outputs: {}" ])
    let output (portLines: string list) = tool ([ "baseCommand: echo"; "inputs: {}"; "outputs:"; "  o:"; "    type: File" ] @ (portLines |> List.map (fun line -> "    " + line)))
    let workflow (lines: string list) = "cwlVersion: v1.2\nclass: Workflow\n" + String.concat "\n" lines + "\n"
    let step (stepLines: string list) =
        workflow ([ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in: {}"; "    out: []" ] @ (stepLines |> List.map (fun line -> "    " + line)))
    let requirement (lines: string list) = toolWith ([ "requirements:" ] @ lines)
    let processOf (yaml: string) =
        match Decode.decodeCWLDocument yaml with
        | CWLDocument.Process processingUnit -> processingUnit
        | other -> failwith (sprintf "Expected a process but got %A" other)
    let toolOf (yaml: string) =
        match processOf yaml with
        | CommandLineTool tool -> tool
        | other -> failwith (sprintf "Expected a tool but got %A" other)
    let textOf (doc: DocText option) =
        match doc with
        | Some (DocText.Text text) -> Some text
        | _ -> None
    let inputOf (yaml: string) = (ListEntry.values (toolOf yaml).Inputs.Value).[0]
    let outputOf (yaml: string) = (ListEntry.values (toolOf yaml).Outputs).[0]
    let stepOf (yaml: string) =
        match processOf yaml with
        | Workflow workflow -> (ListEntry.values workflow.Steps).[0]
        | other -> failwith (sprintf "Expected a workflow but got %A" other)
    let dockerOf (yaml: string) =
        match (toolOf yaml).Requirements.Value.[0] with
        | DockerRequirement docker -> docker
        | other -> failwith (sprintf "Expected a DockerRequirement but got %A" other)
    let schemaOf (yaml: string) =
        (inputOf yaml).Type_.Value

    // The name of the field, the document with {0} where the directive stands, and the string that the document holds there.
    let cases : (string * string * (string -> SchemaSaladString option)) list =
        [ "the doc of a tool", toolWith [ "doc: {0}" ], (fun yaml -> textOf (toolOf yaml).Doc)
          "the label of a tool", toolWith [ "label: {0}" ], (fun yaml -> (toolOf yaml).Label)
          "the doc of a workflow", workflow [ "inputs: {}"; "outputs: {}"; "steps: {}"; "doc: {0}" ], (fun yaml -> match processOf yaml with Workflow w -> textOf w.Doc | _ -> None)
          "the label of a workflow", workflow [ "inputs: {}"; "outputs: {}"; "steps: {}"; "label: {0}" ], (fun yaml -> match processOf yaml with Workflow w -> w.Label | _ -> None)
          "the doc of an expression tool", "cwlVersion: v1.2\nclass: ExpressionTool\ninputs: {}\noutputs: {}\nexpression: $(1)\ndoc: {0}\n", (fun yaml -> match processOf yaml with ExpressionTool e -> textOf e.Doc | _ -> None)
          "the label of an operation", "cwlVersion: v1.2\nclass: Operation\ninputs: {}\noutputs: {}\nlabel: {0}\n", (fun yaml -> match processOf yaml with Operation o -> o.Label | _ -> None)
          "the doc of an input", port [ "doc: {0}" ], (fun yaml -> textOf (inputOf yaml).Doc)
          "the label of an input", port [ "label: {0}" ], (fun yaml -> (inputOf yaml).Label)
          "the doc of an output", output [ "doc: {0}" ], (fun yaml -> textOf (outputOf yaml).Doc)
          "the label of an output", output [ "label: {0}" ], (fun yaml -> (outputOf yaml).Label)
          "the doc of a step", step [ "doc: {0}" ], (fun yaml -> textOf (stepOf yaml).Doc)
          "the label of a step", step [ "label: {0}" ], (fun yaml -> (stepOf yaml).Label)
          "the doc of a step input", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        source: x"; "        doc: {0}"; "    out: []" ], (fun yaml -> textOf (WorkflowStep.getInputValues (stepOf yaml)).[0].Doc)
          "the label of a step input", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        source: x"; "        label: {0}"; "    out: []" ], (fun yaml -> (WorkflowStep.getInputValues (stepOf yaml)).[0].Label)
          "the doc of a record field", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type: string"; "          doc: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Record r -> textOf (InputRecordSchema.getFieldValuesOrEmpty r).[0].Doc | _ -> None)
          "the label of a record", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: record"; "      fields: {}"; "      label: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Record r -> r.Label | _ -> None)
          "the doc of an enum", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: enum"; "      symbols: [a]"; "      doc: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Enum e -> textOf e.Doc | _ -> None)
          "the label of an array", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: array"; "      items: string"; "      label: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Array a -> a.Label | _ -> None)
          "the stdin of a tool", toolWith [ "stdin: {0}" ], (fun yaml -> (toolOf yaml).Stdin)
          "the stdout of a tool", toolWith [ "stdout: {0}" ], (fun yaml -> (toolOf yaml).Stdout)
          "the stderr of a tool", toolWith [ "stderr: {0}" ], (fun yaml -> (toolOf yaml).Stderr)
          "a baseCommand", tool [ "baseCommand: {0}"; "inputs: {}"; "outputs: {}" ], (fun yaml -> (toolOf yaml).BaseCommand.Value |> Seq.tryHead)
          "an entry of the baseCommand", tool [ "baseCommand: [echo, {0}]"; "inputs: {}"; "outputs: {}" ], (fun yaml -> Some (toolOf yaml).BaseCommand.Value.[1])
          "the prefix of an input binding", port [ "inputBinding: {prefix: {0}}" ], (fun yaml -> (inputBindingOf (inputOf yaml)).Prefix)
          "the itemSeparator of an input binding", port [ "inputBinding: {itemSeparator: {0}}" ], (fun yaml -> (inputBindingOf (inputOf yaml)).ItemSeparator)
          "the valueFrom of an input binding", port [ "inputBinding: {valueFrom: {0}}" ], (fun yaml -> (inputBindingOf (inputOf yaml)).ValueFrom)
          "the valueFrom of a step input", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        valueFrom: {0}"; "    out: []" ], (fun yaml -> (WorkflowStep.getInputValues (stepOf yaml)).[0].ValueFrom)
          "the format of an input", port [ "format: {0}" ], (fun yaml -> match (inputOf yaml).Format with Some (InputFormat.Single format) -> Some format | _ -> None)
          "a format in the list of an input", port [ "format: [{0}]" ], (fun yaml -> match (inputOf yaml).Format with Some (InputFormat.Multiple formats) -> Some formats.[0] | _ -> None)
          "the format of an output", output [ "format: {0}" ], (fun yaml -> (outputOf yaml).Format)
          "the outputEval of an output binding", output [ "outputBinding: {glob: a, outputEval: {0}}" ], (fun yaml -> (outputBindingOf (outputOf yaml)).OutputEval)
          "the glob of an output binding", output [ "outputBinding: {glob: {0}}" ], (fun yaml -> match (outputBindingOf (outputOf yaml)).Glob with Some (OutputGlob.Single glob) -> Some glob | _ -> None)
          "a glob in the list of an output binding", output [ "outputBinding: {glob: [a, {0}]}" ], (fun yaml -> match (outputBindingOf (outputOf yaml)).Glob with Some (OutputGlob.Multiple globs) -> Some globs.[1] | _ -> None)
          "the dockerPull", requirement [ "  - {class: DockerRequirement, dockerPull: {0}}" ], (fun yaml -> (dockerOf yaml).DockerPull)
          "the dockerImageId", requirement [ "  - {class: DockerRequirement, dockerImageId: {0}}" ], (fun yaml -> (dockerOf yaml).DockerImageId)
          "the dockerLoad", requirement [ "  - {class: DockerRequirement, dockerLoad: {0}}" ], (fun yaml -> (dockerOf yaml).DockerLoad)
          "the dockerImport", requirement [ "  - {class: DockerRequirement, dockerImport: {0}}" ], (fun yaml -> (dockerOf yaml).DockerImport)
          "the dockerOutputDirectory", requirement [ "  - {class: DockerRequirement, dockerOutputDirectory: {0}}" ], (fun yaml -> (dockerOf yaml).DockerOutputDirectory)
          "the listing of an InitialWorkDirRequirement", requirement [ "  - {class: InitialWorkDirRequirement, listing: {0}}" ], (fun yaml -> match (toolOf yaml).Requirements.Value.[0] with InitialWorkDirExpressionRequirement expression -> Some expression | _ -> None) ]

    testList "Directives in string fields" [
        testCase "an include or an import stands for the string in each string field, is written as a mapping and reads back equal" <| fun _ ->
            for (name, template, read) in cases do
                for (directive, expected) in [ "{$include: notes.md}", SchemaSaladString.Include "notes.md"; "{$import: other.yml}", SchemaSaladString.Import "other.yml" ] do
                    let yaml = template.Replace("{0}", directive)
                    Expect.equal (read yaml) (Some expected) (sprintf "%s: the directive is the string." name)
                    let written = Encode.encodeCWLDocument (Decode.decodeCWLDocument yaml)
                    Expect.stringContains written directive (sprintf "%s: the directive is written as a flow mapping.\n%s" name written)
                    Expect.equal (processOf written) (processOf yaml) (sprintf "%s: the written text should decode to the same process." name)
                    Expect.equal (read written) (Some expected) (sprintf "%s: the written directive is the string." name)
        testCase "a mixin beside another field stands for the string in each string field, is written back as it was and reads back equal" <| fun _ ->
            let directive = "{$mixin: notes.md, extra: 1}"
            for (name, template, read) in cases do
                let yaml = template.Replace("{0}", directive)
                match read yaml with
                | Some (SchemaSaladString.Mixed _ as mixed) -> Expect.equal mixed.Value "notes.md" (sprintf "%s: the value of the mixin is its path." name)
                | other -> failwith (sprintf "%s: expected a mixin beside a field, found %A" name other)
                let written = Encode.encodeCWLDocument (Decode.decodeCWLDocument yaml)
                Expect.stringContains written directive (sprintf "%s: the mixin and its field are written as they were.\n%s" name written)
                Expect.equal (processOf written) (processOf yaml) (sprintf "%s: the written text should decode to the same process." name)
                let changed = template.Replace("{0}", "{$mixin: notes.md, extra: 2}")
                Expect.notEqual (processOf changed) (processOf yaml) (sprintf "%s: another field beside the mixin makes another process." name)
        testCase "a plain number or bool and a mapping that is no directive are decode errors in each string field" <| fun _ ->
            for (name, template, _) in cases do
                if name <> "the listing of an InitialWorkDirRequirement" then
                    for text in [ "42"; "true"; "{a: b}" ] do
                        match (try Decode.decodeCWLDocument (template.Replace("{0}", text)) |> ignore; None with YamlDecodeException error -> Some error) with
                        | Some error -> Expect.isSome error.Span (sprintf "%s with %s: the error should have the place in the text." name text)
                        | None -> failwith (sprintf "%s: %s must fail" name text)
        testCase "a doc list with a directive item keeps its parts and a doc list of plain strings is one text" <| fun _ ->
            let withDoc (doc: string list) = toolWith ([ "doc:" ] @ (doc |> List.map (fun line -> "  " + line)))
            let parts = Decode.decodeCommandLineTool (withDoc [ "- first"; "- {$include: notes.md}"; "- last" ])
            match parts.Doc with
            | Some (DocText.Parts items) ->
                Expect.sequenceEqual items [ SchemaSaladString.Literal "first"; SchemaSaladString.Include "notes.md"; SchemaSaladString.Literal "last" ] "The items stay apart, in order."
            | other -> failwith (sprintf "Expected the parts of a doc but got %A" other)
            let written = Encode.encodeToolDescription parts
            Expect.stringContains written "doc:\n  - first\n  - {$include: notes.md}\n  - last\n" "The doc is written as a list."
            Expect.equal (Decode.decodeCommandLineTool written) parts "The written text should decode to the same tool."
            let single = Decode.decodeCommandLineTool (withDoc [ "- {$import: other.yml}" ])
            Expect.equal single.Doc (Some (DocText.Parts (ResizeArray [ SchemaSaladString.Import "other.yml" ]))) "A list of one directive stays a list."
            Expect.notEqual single.Doc (Some (DocText.Text (SchemaSaladString.Import "other.yml"))) "A list of one directive differs from the directive."
            let plain = Decode.decodeCommandLineTool (withDoc [ "- \"first \""; "- last" ])
            Expect.equal plain.Doc (Some (DocText.literal "first last")) "The items of a list of plain strings join into one text."
            let inPort = Decode.decodeCommandLineTool (port [ "doc: [a, {$include: notes.md}]" ])
            Expect.equal (ListEntry.values inPort.Inputs.Value).[0].Doc (Some (DocText.Parts (ResizeArray [ SchemaSaladString.Literal "a"; SchemaSaladString.Include "notes.md" ]))) "The doc of a port keeps its parts too."
        testCase "a directive replaces the layout of a doc or a label that is edited into text, and text replaces a directive" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (toolWith [ "doc: {$include: notes.md}"; "label: {$import: l.yml}" ])
            decoded.Doc <- Some (DocText.literal "now text")
            decoded.Label <- Some (SchemaSaladString.Literal "now a label")
            let again = Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)
            Expect.equal again.Doc (Some (DocText.literal "now text")) "The edited doc is text."
            Expect.equal again.Label (Some (SchemaSaladString.Literal "now a label")) "The edited label is text."
            let text = Decode.decodeCommandLineTool (toolWith [ "doc: some text"; "label: a label" ])
            text.Doc <- Some (DocText.Text (SchemaSaladString.Include "notes.md"))
            text.Label <- Some (SchemaSaladString.Import "l.yml")
            let written = Encode.encodeToolDescription text
            Expect.stringContains written "doc: {$include: notes.md}" "The doc is the include."
            Expect.stringContains written "label: {$import: l.yml}" "The label is the import."
            Expect.equal (Decode.decodeCommandLineTool written).Doc text.Doc "The include reads back."
    ]

let testImportedSchemaEntries =
    let tool (typeLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  a:" ] @ (typeLines |> List.map (fun line -> "    " + line)) @ [ "outputs: {}"; "" ])
    let recordOf (text: string) =
        match (ListEntry.values (Decode.decodeCommandLineTool text).Inputs.Value).[0].Type_ with
        | Some (Record schema) -> schema
        | other -> failwithf "Expected a record, found %A" other
    let enumOf (text: string) =
        match (ListEntry.values (Decode.decodeCommandLineTool text).Inputs.Value).[0].Type_ with
        | Some (Enum schema) -> schema
        | other -> failwithf "Expected an enum, found %A" other
    let mixedRecord = tool [ "type:"; "  type: record"; "  fields:"; "    - {$import: more-fields.yml}"; "    - name: f"; "      type: string"; "    - {$import: last.yml}" ]
    let mixedEnum = tool [ "type:"; "  type: enum"; "  symbols:"; "    - a"; "    - {$import: more.yml}"; "    - {$include: sym.txt}"; "    - b" ]
    let failsAt (name: string) (text: string) (path: string) (message: string) =
        match (try Decode.decodeCommandLineTool text |> ignore; None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the entry" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Imported schema entries" [
        testCase "an import stands in its place among the fields of a record, is written back there and reads back equal" <| fun _ ->
            let schema = recordOf mixedRecord
            Expect.equal (schema.Fields.Value |> Seq.map (ListEntry.map (fun field -> field.Name)) |> List.ofSeq) [ ListEntry.Imported "more-fields.yml"; ListEntry.Value "f"; ListEntry.Imported "last.yml" ] "The imports stand around the field."
            let decoded = Decode.decodeCommandLineTool mixedRecord
            let written = Encode.encodeToolDescription decoded
            for position in [ "- {$import: more-fields.yml}"; "- name: f"; "- {$import: last.yml}" ] do
                Expect.stringContains written position (sprintf "%s is written as an entry.\n%s" position written)
            Expect.isTrue (written.IndexOf "more-fields.yml" < written.IndexOf "name: f" && written.IndexOf "name: f" < written.IndexOf "last.yml") "The entries keep their order."
            let reread = Decode.decodeCommandLineTool written
            Expect.equal reread decoded "The written text should decode to the same tool."
            Expect.equal (Encode.encodeToolDescription reread) written "Writing again should give the same text."
        testCase "an import or an include stands in its place among the symbols of an enum, is written back there and reads back equal" <| fun _ ->
            let schema = enumOf mixedEnum
            Expect.equal (List.ofSeq schema.Symbols) [ SchemaSaladString.Literal "a"; SchemaSaladString.Import "more.yml"; SchemaSaladString.Include "sym.txt"; SchemaSaladString.Literal "b" ] "A symbol is a string or a directive."
            let decoded = Decode.decodeCommandLineTool mixedEnum
            let written = Encode.encodeToolDescription decoded
            for position in [ "- a"; "- {$import: more.yml}"; "- {$include: sym.txt}"; "- b" ] do
                Expect.stringContains written position (sprintf "%s is written as an entry.\n%s" position written)
            let reread = Decode.decodeCommandLineTool written
            Expect.equal reread decoded "The written text should decode to the same tool."
            Expect.equal (Encode.encodeToolDescription reread) written "Writing again should give the same text."
        testCase "the getters give the fields and the symbols that the document writes out and leave the directives out" <| fun _ ->
            Expect.equal (InputRecordSchema.getFieldValuesOrEmpty (recordOf mixedRecord) |> Seq.map (fun field -> field.Name) |> List.ofSeq) [ "f" ] "Only the field is an object."
            Expect.equal (List.ofSeq (InputEnumSchema.getSymbolValues (enumOf mixedEnum))) [ "a"; "b" ] "Only the plain symbols are strings."
            Expect.equal (InputRecordSchema.getFieldValuesOrEmpty (InputRecordSchema())).Count 0 "A record without fields has none."
            let fields = InputRecordSchema.getFieldValuesOrEmpty (recordOf mixedRecord)
            fields.Clear()
            Expect.equal (recordOf mixedRecord).Fields.Value.Count 3 "The list is a new one."
            Expect.equal (InputRecordSchema.tryGetFieldType "f" (recordOf mixedRecord)) (Some CWLType.String) "The field is found behind the import."
            Expect.isNone (InputRecordSchema.tryGetFieldType "g" (recordOf mixedRecord)) "A field of a file is unknown."
        testCase "a field that is one directive keeps that spelling, and the list of that one import is equal" <| fun _ ->
            let wholeRecord = tool [ "type:"; "  type: record"; "  fields: {$import: fields.yml}" ]
            let listedRecord = tool [ "type:"; "  type: record"; "  fields: [{$import: fields.yml}]" ]
            let wholeEnum = tool [ "type:"; "  type: enum"; "  symbols: {$import: symbols.yml}" ]
            let listedEnum = tool [ "type:"; "  type: enum"; "  symbols:"; "    - {$import: symbols.yml}" ]
            Expect.equal (List.ofSeq (recordOf wholeRecord).Fields.Value) [ ListEntry.Imported "fields.yml" ] "The fields are the import."
            Expect.equal (List.ofSeq (enumOf wholeEnum).Symbols) [ SchemaSaladString.Import "symbols.yml" ] "The symbols are the import."
            Expect.equal (Decode.decodeCommandLineTool wholeRecord) (Decode.decodeCommandLineTool listedRecord) "The spelling is layout, so the tools are equal."
            Expect.equal (Decode.decodeCommandLineTool wholeEnum) (Decode.decodeCommandLineTool listedEnum) "The spelling is layout, so the tools are equal."
            for (text, spelling) in [ wholeRecord, "fields: {$import: fields.yml}"; wholeEnum, "symbols: {$import: symbols.yml}" ] do
                let decoded = Decode.decodeCommandLineTool text
                let written = Encode.encodeToolDescription decoded
                Expect.stringContains written spelling (sprintf "%s keeps its spelling.\n%s" spelling written)
                Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
            let writtenListed = Encode.encodeToolDescription (Decode.decodeCommandLineTool listedRecord)
            Expect.isFalse (writtenListed.Contains "fields: {$import") "The listed spelling is written as a list."
            Expect.stringContains writtenListed "- {$import: fields.yml}" "The listed import is an entry."
        testCase "a record with a repeated field name is written as a list and reads back" <| fun _ ->
            let text = tool [ "type:"; "  type: record"; "  fields:"; "    - {name: f, type: string}"; "    - {name: f, type: int}" ]
            let decoded = Decode.decodeCommandLineTool text
            let written = Encode.encodeToolDescription decoded
            Expect.equal (written.Split([| "name: f" |], System.StringSplitOptions.None).Length - 1) 2 "Both fields are written."
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
        testCase "the flow text of a type writes the imports of a record and an enum in their places" <| fun _ ->
            let recordType = Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Imported "f.yml"; ListEntry.Value (InputRecordField("ok", CWLType.File)) ]))
            let enumType = Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "a"; SchemaSaladString.Import "s.yml"; SchemaSaladString.Include "t.txt" ]))
            let recordText = Encode.cwlTypeToYamlString recordType
            let enumText = Encode.cwlTypeToYamlString enumType
            Expect.equal recordText "{type: record, fields: [{$import: f.yml}, {name: ok, type: File}]}" "The import stands in the list."
            Expect.equal enumText "{type: enum, symbols: [a, {$import: s.yml}, {$include: t.txt}]}" "The directives stand in the list."
            Expect.equal (recordText |> YAMLicious.Reader.readRoot |> Decode.cwlTypeDecoder') recordType "The flow text should decode to the same record."
            Expect.equal (enumText |> YAMLicious.Reader.readRoot |> Decode.cwlTypeDecoder') enumType "The flow text should decode to the same enum."
        testCase "an import and a field of the same path are two entries, and a symbol is a literal, an include or an import" <| fun _ ->
            Expect.notEqual (CWLType.Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Imported "a.yml" ]))) (CWLType.Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Imported "b.yml" ]))) "Imports of two files differ."
            Expect.isFalse (CWLType.typesEqual (CWLType.Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Imported "a.yml" ]))) (CWLType.Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Value (InputRecordField("a", CWLType.String)) ])))) "An import is no field."
            Expect.notEqual (Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Import "a" ]))) (Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Include "a" ]))) "An import is no include."
            Expect.notEqual (Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Import "a" ]))) (Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "a" ]))) "An import is no literal."
        testCase "a directive cannot stand for the value of a field of a record" <| fun _ ->
            for (name, line) in [ "an import", "f: {$import: p.yml}"; "an include", "f: {$include: p.yml}" ] do
                failsAt name (tool [ "type:"; "  type: record"; "  fields:"; "    " + line ]) "inputs.a.type.fields.f" "A directive cannot stand for the value of an entry of the fields field of a record"
        testCase "an include or a directive with another field cannot stand for a field or a symbol" <| fun _ ->
            let cases =
                [ "an include among the fields", tool [ "type:"; "  type: record"; "  fields: [{$include: p.yml}]" ], "inputs.a.type.fields[0]", "An $include yields the text of a file, but an entry of the fields field of a record takes an object."
                  "an import with another field among the fields", tool [ "type:"; "  type: record"; "  fields: [{$import: p.yml, name: f}]" ], "inputs.a.type.fields[0]", "A directive has exactly one field"
                  "an include as the whole fields", tool [ "type:"; "  type: record"; "  fields: {$include: p.yml}" ], "inputs.a.type.fields", "An $include yields the text of a file, but the fields field of a record takes an object."
                  "an include as the whole symbols", tool [ "type:"; "  type: enum"; "  symbols: {$include: p.yml}" ], "inputs.a.type.symbols", "An $include yields the text of a file, but the symbols field of an enum takes a list of strings."
                  "an import with another field among the symbols", tool [ "type:"; "  type: enum"; "  symbols: [{$import: p.yml, extra: 1}]" ], "inputs.a.type.symbols[0]", "A directive has exactly one field"
                  "a number among the symbols", tool [ "type:"; "  type: enum"; "  symbols: [a, 1]" ], "inputs.a.type.symbols[1]", "Expected a string" ]
            for (name, text, path, message) in cases do
                failsAt name text path message
    ]

/// The hash of an object, which the process classes take from DynamicObj and the ports from their own override.
let private hashOf (value: obj) = value.GetHashCode()

/// Edits the text of a fixture once for each variant and expects the decoded objects to differ from the decoded
/// original in equality and in hash. Two decodes of the original are equal and hash alike.
let private expectVariants (name: string) (fixture: string) (decode: string -> 'T) (variants: (string * string * string) list) =
    let text = fixture.Replace("\r\n", "\n")
    let original = decode text
    expectSame (sprintf "%s: a second decode" name) (box original) (box (decode text))
    for (what, oldText, newText) in variants do
        Expect.isTrue (text.Contains oldText) (sprintf "%s: the fixture should hold %s" name oldText)
        let changed = decode (text.Replace(oldText, newText))
        Expect.notEqual (box changed) (box original) (sprintf "%s: another %s should make the objects unequal." name what)
        Expect.notEqual (hashOf (box changed)) (hashOf (box original)) (sprintf "%s: another %s should change the hash." name what)

let testEqualityAndHashOfPorts =
    let inputOf (text: string) = (ListEntry.values (Decode.decodeCommandLineTool text).Inputs.Value).[0]
    let outputOf (text: string) = (ListEntry.values (Decode.decodeCommandLineTool text).Outputs).[0]
    let workflowOutputOf (text: string) = (ListEntry.values (Decode.decodeWorkflow text).Outputs).[0]
    let stepInputOf (text: string) = (WorkflowStep.getInputValues (ListEntry.values (Decode.decodeWorkflow text).Steps).[0]).[0]
    let inputVariants =
        [ "name", "  x:\n", "  y:\n"
          "type", "type: string", "type: int"
          "imported type", "type: string", "type: {$import: t.yml}"
          "binding prefix", "prefix: -a", "prefix: -b"
          "binding position", "position: 1", "position: $(inputs.n)"
          "label", "label: lab", "label: other"
          "secondaryFiles", "secondaryFiles: .idx", "secondaryFiles: .bai"
          "streamable", "streamable: true", "streamable: false"
          "doc", "doc: first", "doc: second"
          "format", "format: edam:format_1", "format: edam:format_2"
          "loadContents", "loadContents: false", "loadContents: true"
          "loadListing", "loadListing: no_listing", "loadListing: deep_listing"
          "default", "default: dflt", "default: other"
          "extension field", "arc:note: n1", "arc:note: n2" ]
    let outputVariants =
        [ "name", "  o:\n", "  p:\n"
          "type", "type: File", "type: Directory"
          "imported type", "type: File", "type: {$import: t.yml}"
          "glob", "glob: \"*.txt\"", "glob: \"*.csv\""
          "label", "label: lab", "label: other"
          "secondaryFiles", "secondaryFiles: .idx", "secondaryFiles: .bai"
          "streamable", "streamable: true", "streamable: false"
          "doc", "doc: first", "doc: second"
          "format", "format: edam:format_1", "format: edam:format_2"
          "extension field", "arc:note: n1", "arc:note: n2" ]
    let stepInputVariants =
        [ "id", "      x:\n", "      y:\n"
          "source", "source: [a, b]", "source: [a, c]"
          "default", "default: dflt", "default: other"
          "valueFrom", "valueFrom: $(self)", "valueFrom: $(self.x)"
          "linkMerge", "linkMerge: merge_flattened", "linkMerge: merge_nested"
          "pickValue", "pickValue: first_non_null", "pickValue: all_non_null"
          "doc", "doc: first", "doc: second"
          "loadContents", "loadContents: false", "loadContents: true"
          "loadListing", "loadListing: no_listing", "loadListing: deep_listing"
          "label", "label: lab", "label: other"
          "extension field", "arc:note: n1", "arc:note: n2" ]
    testList "Equality and hash of ports and files" [
        testCase "an input differs in equality and hash in each of its fields" <| fun _ ->
            expectVariants "input" TestObjects.CWL.Inputs.AllFields.toolFile inputOf inputVariants
        testCase "an output of a tool differs in equality and hash in each of its fields" <| fun _ ->
            expectVariants "tool output" TestObjects.CWL.Outputs.AllFields.toolFile outputOf outputVariants
        testCase "the output source of a workflow output and each field of a step input differ in equality and hash" <| fun _ ->
            expectVariants "workflow output" TestObjects.CWL.Outputs.AllFields.workflowFile workflowOutputOf [ "output source", "outputSource: step/out", "outputSource: step/other" ]
            expectVariants "step input" TestObjects.CWL.WorkflowSteps.AllFields.workflowFile stepInputOf stepInputVariants
        testCase "a file differs in equality and hash in each of its fields" <| fun _ ->
            let file () =
                let file = FileInstance(location = SchemaSaladString.Literal "l", path = SchemaSaladString.Literal "p", basename = SchemaSaladString.Literal "b", dirname = SchemaSaladString.Literal "d", nameroot = SchemaSaladString.Literal "r", nameext = SchemaSaladString.Literal ".e", checksum = SchemaSaladString.Literal "c", size = FileSize.Bytes 1L, secondaryFiles = { Node = YAMLicious.Reader.readRoot "s" }, format = SchemaSaladString.Literal "f", contents = SchemaSaladString.Literal "t")
                DynObj.setProperty "arc:note" (box "n1") file
                file
            let edits : (string * (FileInstance -> unit)) list =
                [ "location", fun f -> f.Location <- Some (SchemaSaladString.Literal "l2")
                  "path", fun f -> f.Path <- Some (SchemaSaladString.Literal "p2")
                  "basename", fun f -> f.Basename <- Some (SchemaSaladString.Literal "b2")
                  "basename as a directive", fun f -> f.Basename <- Some (SchemaSaladString.Import "b")
                  "dirname", fun f -> f.Dirname <- Some (SchemaSaladString.Literal "d2")
                  "nameroot", fun f -> f.Nameroot <- Some (SchemaSaladString.Literal "r2")
                  "nameext", fun f -> f.Nameext <- Some (SchemaSaladString.Literal ".e2")
                  "checksum", fun f -> f.Checksum <- Some (SchemaSaladString.Literal "c2")
                  "size", fun f -> f.Size <- Some (FileSize.Bytes 2L)
                  "secondaryFiles", fun f -> f.SecondaryFiles <- Some { Node = YAMLicious.Reader.readRoot "s2" }
                  "format", fun f -> f.Format <- Some (SchemaSaladString.Literal "f2")
                  "contents", fun f -> f.Contents <- Some (SchemaSaladString.Literal "t2")
                  "extension field", fun f -> DynObj.setProperty "arc:note" (box "n2") f ]
            expectSame "a file" (box (file ())) (box (file ()))
            for (what, edit) in edits do
                let changed = file ()
                edit changed
                Expect.notEqual (box changed) (box (file ())) (sprintf "Another %s should make the files unequal." what)
                Expect.notEqual (hashOf (box changed)) (hashOf (box (file ()))) (sprintf "Another %s should change the hash." what)
        testCase "the entries of a list differ in equality and hash when the value or the path differs" <| fun _ ->
            Expect.notEqual (hashOf (box (CWLInput("x", CWLType.String)))) (hashOf (box (CWLInput("y", CWLType.Int)))) "Two small inputs hash differently."
            Expect.notEqual (hashOf (box (ListEntry.Value (CWLInput("x", CWLType.String))))) (hashOf (box (ListEntry.Value (CWLInput("y", CWLType.Int))))) "The entries of two inputs hash differently."
            Expect.notEqual (hashOf (box (ListEntry.Value (CWLOutput("o", CWLType.File))))) (hashOf (box (ListEntry.Value (CWLOutput("p", CWLType.File))))) "The entries of two outputs hash differently."
            Expect.notEqual (hashOf (box (ListEntry.Imported "a.yml" : ListEntry<CWLInput>))) (hashOf (box (ListEntry.Imported "b.yml" : ListEntry<CWLInput>))) "Imports of two files hash differently."
            Expect.notEqual (ListEntry.Value (CWLInput("x", CWLType.String))) (ListEntry.Value (CWLInput("x", CWLType.Int))) "The entries of two types differ."
            Expect.equal (hashOf (box (ListEntry.Value (CWLInput("x", CWLType.String))))) (hashOf (box (ListEntry.Value (CWLInput("x", CWLType.String))))) "Equal entries hash alike."
    ]

let testEqualityAndHashOfProcesses =
    let tool (inputLines: string list) (outputLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ inputLines @ outputLines @ [ "" ])
    let process' (cls: string) (extra: string list) (inputLines: string list) (outputLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: " + cls ] @ extra @ inputLines @ outputLines @ [ "" ])
    let inputs = [ "inputs:"; "  x:"; "    type: string"; "    doc: in-doc"; "    arc:note: in-note" ]
    let outputs = [ "outputs:"; "  o:"; "    type: string"; "    doc: out-doc"; "    arc:note: out-note" ]
    let variants =
        [ "input name", "  x:\n", "  y:\n"
          "input type", "    type: string\n    doc: in-doc", "    type: int\n    doc: in-doc"
          "imported input type", "    type: string\n    doc: in-doc", "    type: {$import: t.yml}\n    doc: in-doc"
          "input doc", "doc: in-doc", "doc: other"
          "input extension field", "in-note", "other"
          "output name", "  o:\n", "  p:\n"
          "output type", "    type: string\n    doc: out-doc", "    type: int\n    doc: out-doc"
          "output doc", "doc: out-doc", "doc: other"
          "output extension field", "out-note", "other" ]
    let kinds =
        [ "tool", Decode.decodeCommandLineTool >> box, tool inputs outputs
          "expression tool", Decode.decodeExpressionTool >> box, process' "ExpressionTool" [ "expression: $(null)" ] inputs outputs
          "operation", Decode.decodeOperation >> box, process' "Operation" [] inputs outputs
          "workflow", Decode.decodeWorkflow >> box, process' "Workflow" [ "steps: {}" ] inputs outputs ]
    testList "Equality and hash of processes" [
        testCase "processes that differ in one field of an input or an output are unequal and hash differently" <| fun _ ->
            for (name, decode, text) in kinds do
                expectVariants name text decode variants
        testCase "a tool that differs in the binding of an input is unequal and hashes differently" <| fun _ ->
            let binding (prefix: string) (position: string) =
                tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      prefix: " + prefix; "      position: " + position ] [ "outputs: {}" ]
            let text = binding "-a" "1"
            expectVariants "tool" text Decode.decodeCommandLineTool [ "binding prefix", "prefix: -a", "prefix: -b"; "binding position", "position: 1", "position: $(inputs.n)" ]
        testCase "a tool that differs in the path of an imported type or of an imported input is unequal and hashes differently" <| fun _ ->
            let imported = tool [ "inputs:"; "  x:"; "    type: {$import: a.yml}" ] [ "outputs: {}" ]
            expectVariants "tool" imported Decode.decodeCommandLineTool [ "path of the imported type", "a.yml", "b.yml" ]
            let importedInput = tool [ "inputs:"; "  - {$import: ports-a.yml}" ] [ "outputs: {}" ]
            expectVariants "tool" importedInput Decode.decodeCommandLineTool [ "path of the imported inputs", "ports-a.yml", "ports-b.yml" ]
    ]

let testMixinDirectives =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
    let workflow (stepLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:" ] @ stepLines) + "\n"
    let inputsOf (text: string) = (Decode.decodeCommandLineTool text).Inputs.Value
    let stepsOf (text: string) = (Decode.decodeWorkflow text).Steps
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    /// The tool must read back equal from its written text, and the written text must hold the mixin as it was.
    let expectRoundTrip (name: string) (text: string) (written: string list) =
        let decoded = Decode.decodeCommandLineTool text
        let encoded = Encode.encodeToolDescription decoded
        for expected in written do
            Expect.stringContains encoded expected (sprintf "%s: %s is written.\n%s" name expected encoded)
        Expect.isFalse (encoded.Contains "$import") (sprintf "%s: the mixin is no import.\n%s" name encoded)
        Expect.equal (Decode.decodeCommandLineTool encoded) decoded (sprintf "%s: the written text should decode to the same tool." name)
        Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool encoded)) encoded (sprintf "%s: writing again should give the same text." name)
    testList "Mixin directives" [
        testCase "the mixin path of a raw value is the path of the directive, and the empty text for any other node" <| fun _ ->
            let raw (yaml: string) : RawYaml = { Node = Reader.readRoot yaml }
            Expect.equal (raw "{$mixin: tool.yml}").MixinPath "tool.yml" "A mixin on its own has its path."
            Expect.equal (raw "{$mixin: tool.yml, label: x}").MixinPath "tool.yml" "A mixin beside a field has its path."
            Expect.equal (raw "{label: x}").MixinPath "" "A mapping without a mixin has none."
            Expect.equal (raw "{$import: tool.yml}").MixinPath "" "An import is no mixin."
            Expect.equal (raw "[a]").MixinPath "" "A list has none."
            Expect.equal (raw "x").MixinPath "" "A scalar has none."
        testCase "a port with a mixin is a mixed entry of its list, with an id or without, in a list and in a map" <| fun _ ->
            let listed = tool [ "inputs: [{id: x, $mixin: mix.yml}, {id: y, type: int, $mixin: typed.yml}]"; "outputs: [{id: o, $mixin: out.yml}]" ]
            let mapped = tool [ "inputs:"; "  x: {$mixin: mix.yml}"; "  y: {type: int, $mixin: typed.yml}"; "outputs:"; "  o: {$mixin: out.yml}" ]
            for (name, text) in [ "list", listed; "map", mapped ] do
                let decoded = Decode.decodeCommandLineTool text
                Expect.equal decoded.Inputs.Value.Count 2 (sprintf "%s: both inputs stay" name)
                for entry in Seq.append (decoded.Inputs.Value |> Seq.map (ListEntry.map ignore)) (decoded.Outputs |> Seq.map (ListEntry.map ignore)) do
                    match entry with
                    | ListEntry.Mixed _ -> ()
                    | other -> failwith (sprintf "%s: expected a mixed entry, found %A" name other)
                Expect.isEmpty (ListEntry.values decoded.Inputs.Value) (sprintf "%s: a mixed port is no typed port" name)
                Expect.isEmpty (ListEntry.values decoded.Outputs) (sprintf "%s: a mixed output is no typed output" name)
                expectRoundTrip name text [ "$mixin: mix.yml"; "$mixin: typed.yml"; "$mixin: out.yml"; "type: int" ]
        testCase "the key of a map entry replaces the field of the same name that the mixed value holds" <| fun _ ->
            let text = tool [ "inputs:"; "  x: {id: other, $mixin: mix.yml}"; "outputs: []" ]
            match (Decode.decodeCommandLineTool text).Inputs.Value.[0] with
            | ListEntry.Mixed raw ->
                let written = Encode.encodeToolDescription (Decode.decodeCommandLineTool text)
                Expect.stringContains written "id: x" "The key is the id."
                Expect.isFalse (written.Contains "other") "The field of the value gives way to the key."
            | other -> failwith (sprintf "Expected a mixed entry, found %A" other)
            expectRoundTrip "replaced" text [ "id: x" ]
        testCase "a mixin on its own or without an id is an entry of its own in a list of objects" <| fun _ ->
            let inputs = inputsOf (tool [ "inputs: [{$mixin: port.yml}, {type: int, $mixin: other.yml}, {id: a, type: int}]"; "outputs: []" ])
            Expect.equal inputs.Count 3 "Every entry stays."
            match inputs.[0], inputs.[1], inputs.[2] with
            | ListEntry.Mixed first, ListEntry.Mixed second, ListEntry.Value value ->
                Expect.isFalse (first.Equals second) "The mappings differ."
                Expect.equal value.Name "a" "The third entry is the input."
            | other -> failwithf "Expected two mixins and an input, found %A" other
            expectRoundTrip "inputs" (tool [ "inputs: [{$mixin: port.yml}, {type: int, $mixin: other.yml}, {id: a, type: int}]"; "outputs: []" ]) [ "$mixin: port.yml"; "$mixin: other.yml"; "id: a" ]
            let steps = stepsOf (workflow [ "  - $mixin: step.yml"; "  - {id: s, $mixin: body.yml}" ])
            Expect.equal steps.Count 2 "Every step stays."
            for step in steps do
                match step with
                | ListEntry.Mixed _ -> ()
                | other -> failwithf "Expected a mixin, found %A" other
            let written = Encode.encodeWorkflowDescription (Decode.decodeWorkflow (workflow [ "  - $mixin: step.yml"; "  - {id: s, $mixin: body.yml}" ]))
            Expect.stringContains written "$mixin: step.yml" "The mixin is written back as a mixin."
            Expect.isFalse (written.Contains "$import") "The mixin is no import, because the ids of an import and of a mixin resolve against other files."
            let mapped = Decode.decodeWorkflow (workflow [ "  s: {$mixin: body.yml}" ])
            let mappedWritten = Encode.encodeWorkflowDescription mapped
            Expect.stringContains mappedWritten "id: s" "The key of the map entry is the id of the loaded object."
            Expect.equal (Decode.decodeWorkflow mappedWritten) mapped "The written text should decode to the same workflow."
        testCase "a mixin stands for a run, a type, a string, a requirement and a hint, and reads back from its written text" <| fun _ ->
            let run = Decode.decodeWorkflow (workflow [ "  s:"; "    run: {$mixin: tool.cwl}"; "    in: {}"; "    out: []" ])
            match (ListEntry.values run.Steps).[0].Run with
            | RunMixed _ -> ()
            | other -> failwithf "Expected a mixed run, found %A" other
            Expect.stringContains (Encode.encodeWorkflowDescription run) "run: {$mixin: tool.cwl}" "The run is written back as a mixin."
            Expect.equal (Decode.decodeWorkflow (Encode.encodeWorkflowDescription run)) run "The written text should decode to the same workflow."
            let types = tool [ "inputs:"; "  a:"; "    type: {$mixin: t.yml}"; "  b:"; "    type: [\"null\", {$mixin: t.yml}]"; "outputs: []" ]
            let portTypes = ListEntry.values (inputsOf types) |> Seq.map (fun input -> input.Type_) |> List.ofSeq
            Expect.isTrue (match portTypes.[0] with Some (MixedType _) -> true | _ -> false) "The type of the first input is the mixin."
            Expect.isTrue (match portTypes.[1] with Some (Union members) -> members.Count = 2 && (match members.[1] with MixedType _ -> true | _ -> false) | _ -> false) "The second member of the union is the mixin."
            expectRoundTrip "types" types [ "type: {$mixin: t.yml}"; "{$mixin: t.yml}" ]
            let strings = tool [ "doc: {$mixin: d.md}"; "label: {$mixin: l.yml}"; "inputs: []"; "outputs: []" ]
            let decodedStrings = Decode.decodeCommandLineTool strings
            Expect.equal decodedStrings.Doc (Some (DocText.Text (SchemaSaladString.Mixin "d.md"))) "The doc is the mixin."
            Expect.equal decodedStrings.Label (Some (SchemaSaladString.Mixin "l.yml")) "The label is the mixin."
            expectRoundTrip "strings" strings [ "doc: {$mixin: d.md}"; "label: {$mixin: l.yml}" ]
            expectRoundTrip "requirements" (tool [ "requirements:"; "  - {$mixin: req.yml}"; "  - {$mixin: body.yml, class: NetworkAccess}"; "hints:"; "  - {$mixin: hint.yml}"; "inputs: []"; "outputs: []" ]) [ "$mixin: req.yml"; "$mixin: body.yml"; "$mixin: hint.yml" ]
            let mapped = Decode.decodeCommandLineTool (tool [ "requirements:"; "  NetworkAccess: {$mixin: body.yml}"; "hints:"; "  NetworkAccess: {$mixin: body.yml}"; "inputs: []"; "outputs: []" ])
            Expect.isTrue (match mapped.Requirements.Value.[0] with MixedRequirement _ -> true | _ -> false) "The map-form requirement is a mixin."
            Expect.isTrue (match mapped.Hints.Value.[0] with MixedHint _ -> true | _ -> false) "The map-form hint is a mixin."
        testCase "a mixin stands for a field of a record, a symbol, an environment variable, a step input and an output of a step" <| fun _ ->
            expectRoundTrip "fields" (tool [ "inputs:"; "  a:"; "    type:"; "      type: record"; "      fields: [{$mixin: field.yml}, {name: f, $mixin: mix.yml}]"; "outputs: []" ]) [ "$mixin: field.yml"; "name: f" ]
            expectRoundTrip "symbols" (tool [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: [x, {$mixin: sym.yml}]"; "outputs: []" ]) [ "{$mixin: sym.yml}" ]
            expectRoundTrip "envDef" (tool [ "requirements:"; "  EnvVarRequirement:"; "    envDef: [{$mixin: env.yml}, {envName: A, $mixin: value.yml}]"; "inputs: []"; "outputs: []" ]) [ "$mixin: env.yml"; "envName: A" ]
            let step = workflow [ "  s:"; "    run: t.cwl"; "    in: [{$mixin: in.yml}, {id: a, $mixin: more.yml}]"; "    out: [{$mixin: out.yml}, {id: o, $mixin: more.yml}]" ]
            let decoded = Decode.decodeWorkflow step
            let first = (ListEntry.values decoded.Steps).[0]
            Expect.equal first.In.Count 2 "Both step inputs stay."
            Expect.isEmpty (WorkflowStep.getInputValues first) "A step input with a mixin is no typed step input, with an id or without."
            Expect.equal (first.Out |> Seq.map StepOutput.tryId |> List.ofSeq) [ None; None ] "An output with a mixin has no id of its own, with an id or without."
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "$mixin: in.yml" "The step input mixin is written back."
            Expect.stringContains written "$mixin: out.yml" "The output mixin is written back."
            Expect.equal (Decode.decodeWorkflow written) decoded "The written text should decode to the same workflow."
        testCase "a mixin cannot stand for the whole of a list, because it completes an object" <| fun _ ->
            failsAt "inputs" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: {$mixin: ports.yml}"; "outputs: []" ]) |> ignore) "inputs" "A $mixin completes an object with the fields beside it, and the inputs field is no object."
            failsAt "requirements" (fun () -> Decode.decodeCommandLineTool (tool [ "requirements: {$mixin: reqs.yml}"; "inputs: []"; "outputs: []" ]) |> ignore) "requirements" "A $mixin completes an object with the fields beside it, and the requirements field is no object."
            failsAt "steps" (fun () -> Decode.decodeWorkflow (workflow [ "  $mixin: steps.yml" ]) |> ignore) "steps" "A $mixin completes an object with the fields beside it, and the steps field is no object."
        testCase "a malformed mixin is a decode error in the step outputs and in a map of ports, as everywhere else" <| fun _ ->
            let stepOutputs (line: string) = workflow [ "  s:"; "    run: t.cwl"; "    in: {}"; "    out: " + line ]
            failsAt "an empty path" (fun () -> Decode.decodeWorkflow (stepOutputs "[{id: o, $mixin: \"\"}]") |> ignore) "steps.s.out[0].$mixin" "The path of the $mixin directive is empty."
            failsAt "an import beside a mixin" (fun () -> Decode.decodeWorkflow (stepOutputs "[{$mixin: somix2.yml, $import: outs.yml}]") |> ignore) "steps.s.out[0]" "A directive has exactly one field"
            failsAt "a list as the path" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: {x: {$mixin: [a]}}"; "outputs: []" ]) |> ignore) "inputs.x.$mixin" "Expected the path of the $mixin directive to be a string, found a list."
            failsAt "a list as the path of an output" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: []"; "outputs: {o: {$mixin: [a]}}" ]) |> ignore) "outputs.o.$mixin" "Expected the path of the $mixin directive to be a string, found a list."
            failsAt "a list as the path of a step input" (fun () -> Decode.decodeWorkflow (workflow [ "  s:"; "    run: t.cwl"; "    in: {a: {$mixin: [a]}}"; "    out: []" ]) |> ignore) "steps.s.in.a.$mixin" "Expected the path of the $mixin directive to be a string, found a list."
        testCase "a mixin with a bad path and a mixin beside an import are decode errors" <| fun _ ->
            failsAt "a number" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: [{id: x, $mixin: 42}]"; "outputs: []" ]) |> ignore) "inputs[0].$mixin" "Expected the path of the $mixin directive to be a string"
            failsAt "an import beside a mixin" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: [{$import: a.yml, $mixin: b.yml}]"; "outputs: []" ]) |> ignore) "inputs[0]" "A directive has exactly one field"
    ]

let testScalarsThatCwltoolReadsAsAnotherType =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool" ] @ lines @ [ "" ])
    let bigBinary = "0b" + String.replicate 70 "1"
    let bigHex = "0x_FFFF_FFFF_FFFF_FFFF_FFFF"
    // Strings that the core schema reads as strings and the loader of cwltool reads as other types.
    let typed = [ "="; "<<"; bigBinary; bigHex; "1_000"; "0b101" ]
    // Strings that the loader of cwltool reads as strings. The writer quotes them for the readers of YAML 1.1, which read a bool and a number.
    let quotedForYaml11 = [ "yes"; "1:30" ]
    // Strings that quotes keep apart from an int or a float of the core schema.
    let quotedOnly = [ "0o" + String.replicate 30 "7"; "017"; ".5e3" ]
    // Strings with the pattern of a float or an int that the loader cannot convert.
    let unreadable = [ "._"; "-._"; "+._"; "._e+5"; ".__"; "0b_" ]
    testList "Strings that cwltool reads as another type" [
        testCase "a quoted text that cwltool would read as another type is written in quotes in a list of strings, an environment variable, a doc and a default" <| fun _ ->
            for text in typed @ quotedForYaml11 @ unreadable @ quotedOnly do
                let documents =
                    [ "a list of strings", tool [ "baseCommand: [echo, \"" + text + "\"]"; "inputs: []"; "outputs: []" ]
                      "an environment variable", tool [ "baseCommand: env"; "requirements:"; "  EnvVarRequirement:"; "    envDef:"; "      A: \"" + text + "\""; "inputs: []"; "outputs: []" ]
                      "a doc and a default", tool [ "doc: \"" + text + "\""; "baseCommand: echo"; "inputs:"; "  x:"; "    type: string"; "    default: \"" + text + "\""; "outputs: []" ] ]
                for (place, yaml) in documents do
                    let decoded = Decode.decodeCWLDocument yaml
                    let written = Encode.encodeCWLDocument decoded
                    Expect.stringContains written ("\"" + text + "\"") (sprintf "%s %s: the text is written in quotes.\n%s" place text written)
                    Expect.equal (Decode.decodeCWLDocument written) decoded (sprintf "%s %s: the written text should decode to the same document." place text)
        testCase "a plain text that the core schema reads as a string decodes as a string and is written in quotes" <| fun _ ->
            for text in typed @ quotedForYaml11 @ unreadable do
                let yaml = tool [ "label: " + text; "baseCommand: echo"; "inputs: []"; "outputs: []" ]
                let decoded = Decode.decodeCommandLineTool yaml
                Expect.equal decoded.Label (Some (SchemaSaladString.Literal text)) (sprintf "The plain label %s is a string." text)
                let written = Encode.encodeToolDescription decoded
                Expect.stringContains written ("label: \"" + text + "\"") (sprintf "%s: the label is written in quotes.\n%s" text written)
                Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool." text)
        testCase "a text that cwltool reads as another string is written in quotes, in a block and in a flow collection" <| fun _ ->
            // The loader fails on a tab inside a plain scalar, folds a NEL and drops a line separator at the ends, and reads a flow item that starts with ? as a mapping.
            // The first column is the text as the source writes it in quotes, the second the decoded text and the third the text as the writer spells it.
            for (what, source, text, written) in
                [ "a tab inside", "a\\tb", "a\tb", "a\tb"
                  "a NEL inside", "a\\Nb", "a\u0085b", "a\\Nb"
                  "a line separator at the end", "a\\L", "a\u2028", "a\\L"
                  "a paragraph separator at the start", "\\Pa", "\u2029a", "\\Pa"
                  "a question mark at the start", "?x", "?x", "?x"
                  "two question marks", "??", "??", "??" ] do
                let decoded = Decode.decodeCommandLineTool (tool [ "label: \"" + source + "\""; "baseCommand: [echo, \"" + source + "\"]"; "inputs: []"; "outputs: []" ])
                Expect.equal decoded.Label (Some (SchemaSaladString.Literal text)) (sprintf "%s: the label is the text." what)
                let output = Encode.encodeToolDescription decoded
                Expect.stringContains output ("label: \"" + written + "\"") (sprintf "%s: the label is written in quotes.\n%s" what output)
                Expect.stringContains output ("baseCommand: [echo, \"" + written + "\"]") (sprintf "%s: the item of the flow list is written in quotes.\n%s" what output)
                Expect.equal (Decode.decodeCommandLineTool output) decoded (sprintf "%s: the written text should decode to the same tool." what)
            // A text that the loader reads as written stays plain.
            let plain = Encode.encodeToolDescription (Decode.decodeCommandLineTool (tool [ "label: a-?b"; "baseCommand: [echo, \"-x\", \":y\"]"; "inputs: []"; "outputs: []" ]))
            Expect.stringContains plain "label: a-?b" "A question mark inside a text needs no quotes."
            Expect.stringContains plain "baseCommand: [echo, -x, :y]" "A dash or a colon at the start of a flow item needs no quotes."
        testCase "a text with several lines is a literal block, and it is written in double quotes when it holds a NEL, a line separator or a paragraph separator" <| fun _ ->
            // The loader reads those characters as line breaks in a block scalar too. The writer escapes them in double quotes.
            let built (text: string) =
                CWLToolDescription(
                    ResizeArray(),
                    baseCommand = ResizeArray [ SchemaSaladString.Literal "echo" ],
                    requirements = ResizeArray [ Requirement.EnvVarRequirement (ResizeArray [ ListEntry.Value (EnvironmentDef(SchemaSaladString.Literal "V", SchemaSaladString.Literal text)) ]) ],
                    inputs = ResizeArray(),
                    label = SchemaSaladString.Literal text,
                    doc = DocText.Text (SchemaSaladString.Literal text))
            for (what, text, written) in
                [ "a NEL inside", "a\u0085b\nc", "a\\Nb\\nc"
                  "a NEL at the end", "a\nb\u0085", "a\\nb\\N"
                  "a NEL at the start", "\u0085a\nb", "\\Na\\nb"
                  "a line separator inside", "a\u2028b\nc", "a\\Lb\\nc"
                  "a line separator at the end", "a\nb\u2028", "a\\nb\\L"
                  "a paragraph separator inside", "a\u2029b\nc", "a\\Pb\\nc"
                  "a paragraph separator at the end", "a\nb\u2029", "a\\nb\\P"
                  "the three together", "a\u0085b\u2028c\u2029d\ne", "a\\Nb\\Lc\\Pd\\ne" ] do
                let output = Encode.encodeToolDescription (built text)
                for field in [ "label"; "doc"; "envValue" ] do
                    Expect.stringContains output (field + ": \"" + written + "\"") (sprintf "%s: the %s is written in double quotes.\n%s" what field output)
                Expect.isFalse (output.Contains "|-") (sprintf "%s: no block scalar is written.\n%s" what output)
                let again = Decode.decodeCommandLineTool output
                Expect.equal again.Label (Some (SchemaSaladString.Literal text)) (sprintf "%s: the label reads back." what)
                Expect.equal again.Doc (Some (DocText.Text (SchemaSaladString.Literal text))) (sprintf "%s: the doc reads back." what)
                Expect.equal (Encode.encodeToolDescription again) output (sprintf "%s: writing again should give the same text." what)
            for text in [ "a\nb"; "x\ty\n\tz"; "a\n\nb"; "a b\nc d" ] do
                let output = Encode.encodeToolDescription (built text)
                for field in [ "label"; "doc"; "envValue" ] do
                    Expect.stringContains output (field + ": |-") (sprintf "%s: the %s stays a literal block.\n%s" (text.Replace("\n", "\\n")) field output)
                Expect.equal (Decode.decodeCommandLineTool output).Label (Some (SchemaSaladString.Literal text)) "The label reads back."
            // A label that the source wrote as a block scalar follows the rule when its text holds one of these characters.
            let fromBlock = Decode.decodeCommandLineTool (tool [ "label: |-"; "  a\u0085b"; "baseCommand: echo"; "inputs: []"; "outputs: []" ])
            Expect.equal fromBlock.Label (Some (SchemaSaladString.Literal "a\u0085b")) "The block scalar holds the NEL as a character."
            Expect.stringContains (Encode.encodeToolDescription fromBlock) "label: \"a\\Nb\"" "The label is written in double quotes."
            // The text of an expression follows the same rule.
            let expressionTool (expression: string) =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: ExpressionTool"; "requirements: [{class: InlineJavascriptRequirement}]"; "inputs: []"; "outputs: [{id: o, type: int}]"; "expression: \"" + expression + "\""; "" ]
            let quoted = Encode.encodeExpressionToolDescription (Decode.decodeExpressionTool (expressionTool "${\\n  return {o: 1};\\N\\n}"))
            Expect.stringContains quoted "expression: \"${\\n  return {o: 1};\\N\\n}\"" "An expression with a NEL is written in double quotes."
            let block = Encode.encodeExpressionToolDescription (Decode.decodeExpressionTool (expressionTool "${\\n  return {o: 1};\\n}"))
            Expect.stringContains block "expression: |-" "An expression without one stays a literal block."
        testCase "the comment behind the header of a block scalar stays behind the double-quoted scalar that replaces it" <| fun _ ->
            // A literal or folded doc or label whose text holds a NEL, a line separator or a paragraph separator is written in double quotes.
            // A double-quoted scalar has no header, so its comment moves to the end of its line, and it stays there when the text is read again.
            for (what, field, header, lines, text, written) in
                [ "a literal doc", "doc", "|  # keep this", [ "  first \u2028"; "  second" ], "first \u2028\nsecond\n", "\"first \\L\\nsecond\\n\" # keep this"
                  "a folded label", "label", ">-  # note", [ "  folded\u0085"; "  text" ], "folded\u0085 text", "\"folded\\N text\" # note"
                  "a literal label", "label", "|-  # c", [ "  a\u2029b"; "  c" ], "a\u2029b\nc", "\"a\\Pb\\nc\" # c"
                  "a folded doc", "doc", ">  # d", [ "  x\u0085y"; "  z" ], "x\u0085y z\n", "\"x\\Ny z\\n\" # d"
                  "a literal doc that keeps its final lines", "doc", "|+  # k", [ "  a\u2028b"; ""; "  c"; "" ], "a\u2028b\n\nc\n\n", "\"a\\Lb\\n\\nc\\n\\n\" # k" ] do
                let yaml = tool ([ field + ": " + header ] @ lines @ [ "baseCommand: echo"; "inputs: []"; "outputs: []" ])
                let decoded = Decode.decodeCommandLineTool yaml
                let textOf (tool: CWLToolDescription) =
                    if field = "doc" then tool.Doc |> Option.bind (fun doc -> doc.TryText) else tool.Label |> Option.bind SchemaSaladString.tryLiteral
                Expect.equal (textOf decoded) (Some text) (sprintf "%s: the block scalar holds the text." what)
                let output = Encode.encodeToolDescription decoded
                Expect.stringContains output (field + ": " + written) (sprintf "%s: the comment stands behind the quoted scalar.\n%s" what output)
                let again = Decode.decodeCommandLineTool output
                Expect.equal (textOf again) (Some text) (sprintf "%s: the text reads back." what)
                Expect.equal again decoded (sprintf "%s: the written text should decode to the same tool." what)
                Expect.equal (Encode.encodeToolDescription again) output (sprintf "%s: the comment is still there when the text is written again." what)
            // A plain scalar on several lines that gets quotes keeps its lines.
            let plain = Decode.decodeCommandLineTool (tool [ "label: ?x"; "  second line"; "baseCommand: echo"; "inputs: []"; "outputs: []" ])
            Expect.equal plain.Label (Some (SchemaSaladString.Literal "?x second line")) "The plain scalar holds both lines."
            let plainOutput = Encode.encodeToolDescription plain
            Expect.stringContains plainOutput "label: \"?x" "The label is written in double quotes."
            Expect.equal (Decode.decodeCommandLineTool plainOutput) plain "The written text should decode to the same tool."
        testCase "a quoted text that cwltool would read as another type is written in quotes in a job file and reads back" <| fun _ ->
            for text in typed @ quotedForYaml11 @ unreadable @ quotedOnly do
                let references = DecodeParameters.decodeYAMLParameterFile ("b: \"" + text + "\"\n")
                let written = Encode.encodeYAMLParameterFile references
                Expect.stringContains written ("b: \"" + text + "\"") (sprintf "%s: the value is written in quotes.\n%s" text written)
                Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references (sprintf "%s: the written file should decode to the same references." text)
        testCase "a raw value is written in the form that cwltool reads as the same data, and a read of the written text is equal" <| fun _ ->
            let yaml = tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type: Any"; "    default:"; "      plain: 1_000"; "      list: [yes, 0b101, .5e3, +.5E3, -.5e+3]"; "      1_000: key"; "outputs: []" ]
            let decoded = Decode.decodeCommandLineTool yaml
            let written = Encode.encodeToolDescription decoded
            for expected in [ "plain: \"1_000\""; "\"yes\""; "\"0b101\""; "0.5e3"; "+0.5E3"; "-.5e+3"; "\"1_000\": key" ] do
                Expect.stringContains written expected (sprintf "%s is in the written text.\n%s" expected written)
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
            Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool written)) written "Writing again should give the same text."
        testCase "an extension field that holds a marker is a string, written in quotes" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool [ "$namespaces: {ex: http://example.org/}"; "baseCommand: echo"; "inputs: []"; "outputs: []"; "ex:marker: =" ])
            let written = Encode.encodeToolDescription decoded
            Expect.stringContains written "ex:marker: \"=\"" "The extension field is written in quotes."
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
    ]

let testDirectivesInExitCodes =
    let tool (line: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: {}"; "outputs: {}"; line; "" ]
    let fields : (string * (CWLToolDescription -> ResizeArray<ListEntry<int>> option)) list =
        [ "successCodes", (fun tool -> tool.SuccessCodes)
          "temporaryFailCodes", (fun tool -> tool.TemporaryFailCodes)
          "permanentFailCodes", (fun tool -> tool.PermanentFailCodes) ]
    let failsAt (name: string) (text: string) (path: string) (message: string) =
        match (try Decode.decodeCommandLineTool text |> ignore; None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Directives in the exit codes" [
        testCase "an import stands among the codes, is written back there and reads back equal" <| fun _ ->
            for (field, read) in fields do
                let text = tool (field + ": [0, {$import: more.yml}, 3]")
                let decoded = Decode.decodeCommandLineTool text
                Expect.equal (List.ofSeq (read decoded).Value) [ ListEntry.Value 0; ListEntry.Imported "more.yml"; ListEntry.Value 3 ] (sprintf "%s: the import stands in its place." field)
                let written = Encode.encodeToolDescription decoded
                Expect.stringContains written (field + ":\n  - 0\n  - {$import: more.yml}\n  - 3\n") (sprintf "%s: the import is written as an entry.\n%s" field written)
                let reread = Decode.decodeCommandLineTool written
                Expect.equal reread decoded (sprintf "%s: the written text should decode to the same tool." field)
                Expect.equal (Encode.encodeToolDescription reread) written (sprintf "%s: writing again should give the same text." field)
        testCase "a mixin stands among the codes and keeps the mapping as it was written" <| fun _ ->
            for (field, read) in fields do
                let decoded = Decode.decodeCommandLineTool (tool (field + ": [{$mixin: m.yml}, {$mixin: n.yml, extra: 1}]"))
                for entry in read decoded |> Option.get do
                    match entry with
                    | ListEntry.Mixed _ -> ()
                    | other -> failwith (sprintf "%s: expected a mixin, found %A" field other)
                let written = Encode.encodeToolDescription decoded
                Expect.stringContains written "{$mixin: m.yml}" (sprintf "%s: the mixin is written back.\n%s" field written)
                Expect.stringContains written "{$mixin: n.yml, extra: 1}" (sprintf "%s: the field beside the mixin is written back." field)
                Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool." field)
        testCase "a field that is one directive keeps that spelling, and the list of that one import is equal" <| fun _ ->
            for (field, read) in fields do
                let whole = Decode.decodeCommandLineTool (tool (field + ": {$import: codes.yml}"))
                let listed = Decode.decodeCommandLineTool (tool (field + ": [{$import: codes.yml}]"))
                Expect.equal (List.ofSeq (read whole).Value) [ ListEntry.Imported "codes.yml" ] (sprintf "%s: the codes are the import." field)
                Expect.equal whole listed (sprintf "%s: the spelling is layout, so the tools are equal." field)
                Expect.stringContains (Encode.encodeToolDescription whole) (field + ": {$import: codes.yml}") (sprintf "%s: the whole field keeps its spelling." field)
                Expect.isFalse ((Encode.encodeToolDescription listed).Contains (field + ": {$import")) (sprintf "%s: the listed spelling is written as a list." field)
                let mixed = Decode.decodeCommandLineTool (tool (field + ": {$mixin: codes.yml, extra: 1}"))
                Expect.stringContains (Encode.encodeToolDescription mixed) (field + ": {$mixin: codes.yml, extra: 1}") (sprintf "%s: a mixin as the whole field keeps its spelling." field)
                Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription mixed)) mixed (sprintf "%s: the written text should decode to the same tool." field)
        testCase "an include, a text and a mapping that is no directive are decode errors in the codes" <| fun _ ->
            for (field, _) in fields do
                failsAt (field + " include") (tool (field + ": [{$include: c.txt}]")) (field + "[0]") ("An $include yields the text of a file, but an entry of the " + field + " field takes a number.")
                failsAt (field + " whole include") (tool (field + ": {$include: c.txt}")) field ("An $include yields the text of a file, but the " + field + " field takes a list of numbers.")
                failsAt (field + " text") (tool (field + ": 0")) field "Expected a list of numbers, found the plain scalar 0, which the core schema reads as an int."
                failsAt (field + " mapping") (tool (field + ": {a: b}")) field "Expected a list of numbers, found a mapping."
                failsAt (field + " import beside a field") (tool (field + ": {$import: c.yml, extra: 1}")) field "A directive has exactly one field"
        testCase "the plain codes still read as numbers and a quoted number is still an error" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool "successCodes: [0, 1]")
            Expect.equal (List.ofSeq (ListEntry.values decoded.SuccessCodes.Value)) [ 0; 1 ] "The numbers are the values."
            failsAt "a quoted number" (tool "successCodes: [\"0\"]") "successCodes[0]" "Expected an int"
    ]

let testDirectivesInSoftwareAndEnvironment =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs: {}"; "outputs: {}" ] @ lines @ [ "" ])
    let software (packageLines: string list) =
        tool ([ "hints:"; "  - class: SoftwareRequirement" ] @ (packageLines |> List.map (fun line -> "    " + line)))
    let packagesOf (text: string) =
        match (Decode.decodeCommandLineTool text).Hints.Value.[0] with
        | KnownHint (SoftwareRequirement packages) -> packages
        | other -> failwith (sprintf "Expected a SoftwareRequirement but got %A" other)
    let literal (text: string) = SchemaSaladString.Literal text
    let roundTrip (name: string) (text: string) =
        let decoded = Decode.decodeCommandLineTool text
        let written = Encode.encodeToolDescription decoded
        Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool.\n%s" name written)
        Expect.equal (Encode.encodeToolDescription (Decode.decodeCommandLineTool written)) written (sprintf "%s: writing again should give the same text." name)
        written
    let failsAt (name: string) (text: string) (path: string) (message: string) =
        match (try Decode.decodeCommandLineTool text |> ignore; None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Directives in software packages and environment definitions" [
        testCase "an import or a mixin stands among the packages, is written back there and reads back equal" <| fun _ ->
            let text = software [ "packages:"; "  - {$import: more.yml}"; "  - package: git"; "  - {$mixin: m.yml, package: curl}" ]
            let packages = packagesOf text
            Expect.equal packages.Count 3 "Every entry stays."
            Expect.equal packages.[0] (ListEntry.Imported "more.yml") "The import comes first."
            Expect.equal (ListEntry.values packages |> Seq.map (fun package -> package.Package) |> List.ofSeq) [ literal "git" ] "Only the package is an object."
            match packages.[2] with
            | ListEntry.Mixed _ -> ()
            | other -> failwith (sprintf "Expected a mixin, found %A" other)
            let written = roundTrip "packages" text
            Expect.stringContains written "- {$import: more.yml}" "The import is written as an entry."
            Expect.stringContains written "{$mixin: m.yml, package: curl}" "The mixin is written as it was."
        testCase "the packages as a whole may be an import, which is written as a list, and an include or a mixin there is an error" <| fun _ ->
            let whole = software [ "packages: {$import: packages.yml}" ]
            Expect.equal (List.ofSeq (packagesOf whole)) [ ListEntry.Imported "packages.yml" ] "The packages are the import, and not a package called $import."
            Expect.equal (Decode.decodeCommandLineTool whole) (Decode.decodeCommandLineTool (software [ "packages: [{$import: packages.yml}]" ])) "The spelling is layout, so the tools are equal."
            Expect.stringContains (roundTrip "whole" whole) "- {$import: packages.yml}" "The import is written as an entry."
            failsAt "an include" (software [ "packages: {$include: p.txt}" ]) "hints[0].packages" "An $include yields the text of a file, but the packages field takes an object."
            failsAt "a mixin" (software [ "packages: {$mixin: p.yml}" ]) "hints[0].packages" "A $mixin completes an object with the fields beside it"
            failsAt "an include entry" (software [ "packages: [{$include: p.txt}]" ]) "hints[0].packages[0]" "An $include yields the text of a file, but an entry of the packages field takes an object."
        testCase "a directive cannot stand for the value of a package in map form, and a mixin there gets the package name beside it" <| fun _ ->
            failsAt "an import" (software [ "packages:"; "  git: {$import: p.yml}" ]) "hints[0].packages.git" "A directive cannot stand for the value of an entry of the packages field"
            let mixed = software [ "packages:"; "  git: {$mixin: p.yml}" ]
            match (packagesOf mixed).[0] with
            | ListEntry.Mixed _ -> ()
            | other -> failwith (sprintf "Expected a mixin, found %A" other)
            Expect.stringContains (roundTrip "mixed" mixed) "package: git" "The key of the map entry is the package of the loaded object."
        testCase "the package, the version and the specs take directives and a list of specs may be an import or a mixin as a whole" <| fun _ ->
            let text = software [ "packages:"; "  - package: {$include: name.txt}"; "    version: [\"1.0\", {$import: v.yml}]"; "    specs: {$import: specs.yml}"; "  - package: {$mixin: n.yml, extra: 1}"; "    specs: {$mixin: s.yml}" ]
            let packages = ListEntry.values (packagesOf text)
            Expect.equal packages.[0].Package (SchemaSaladString.Include "name.txt") "The package is the include."
            Expect.equal (List.ofSeq packages.[0].Version.Value) [ literal "1.0"; SchemaSaladString.Import "v.yml" ] "The version holds a directive among the strings."
            Expect.equal (List.ofSeq packages.[0].Specs.Value) [ SchemaSaladString.Import "specs.yml" ] "The specs are the import."
            Expect.equal (List.ofSeq packages.[1].Specs.Value) [ SchemaSaladString.Mixin "s.yml" ] "The specs are the mixin."
            let written = roundTrip "fields" text
            Expect.stringContains written "package: {$include: name.txt}" "The include is written back."
            Expect.stringContains written "{$mixin: n.yml, extra: 1}" "The mixin beside a field is written back."
            let mapped = software [ "packages:"; "  git: [a, {$import: s.yml}]"; "  curl: {version: [{$include: v.txt}], specs: [{$import: s.yml}]}" ]
            let mappedPackages = ListEntry.values (packagesOf mapped)
            Expect.equal (List.ofSeq mappedPackages.[0].Specs.Value) [ literal "a"; SchemaSaladString.Import "s.yml" ] "The specs shorthand takes a directive."
            Expect.equal (List.ofSeq mappedPackages.[1].Version.Value) [ SchemaSaladString.Include "v.txt" ] "The version takes an include."
            roundTrip "mapped" mapped |> ignore
            failsAt "an include as the whole specs" (software [ "packages:"; "  - package: git"; "    specs: {$include: s.txt}" ]) "hints[0].packages[0].specs" "An $include yields the text of a file, but the specs field of a package takes a list of strings."
            failsAt "a text as the version" (software [ "packages:"; "  - package: git"; "    version: v1" ]) "hints[0].packages[0].version" "Expected a list of strings"
        testCase "the envName takes a directive" <| fun _ ->
            let text = tool [ "requirements:"; "  EnvVarRequirement:"; "    envDef:"; "      - envName: {$include: name.txt}"; "        envValue: v"; "      - envName: {$mixin: n.yml, extra: 1}"; "        envValue: w" ]
            let envs =
                match (Decode.decodeCommandLineTool text).Requirements.Value.[0] with
                | EnvVarRequirement envs -> ListEntry.values envs
                | other -> failwith (sprintf "Expected an EnvVarRequirement but got %A" other)
            Expect.equal envs.[0].EnvName (SchemaSaladString.Include "name.txt") "The name is the include."
            Expect.stringContains (roundTrip "envName" text) "envName: {$mixin: n.yml, extra: 1}" "The mixin beside a field is written back."
        testCase "the expressionLib of InlineJavascriptRequirement may be an import or a mixin as a whole, which is written as a list" <| fun _ ->
            let lib (value: string) = tool [ "requirements:"; "  - class: InlineJavascriptRequirement"; "    expressionLib: " + value ]
            let libOf (text: string) =
                match (Decode.decodeCommandLineTool text).Requirements.Value.[0] with
                | InlineJavascriptRequirement value -> List.ofSeq value.ExpressionLib.Value
                | other -> failwith (sprintf "Expected an InlineJavascriptRequirement but got %A" other)
            Expect.equal (libOf (lib "{$import: lib.yml}")) [ SchemaSaladString.Import "lib.yml" ] "The library is the import."
            Expect.equal (libOf (lib "{$mixin: lib.yml}")) [ SchemaSaladString.Mixin "lib.yml" ] "The library is the mixin."
            Expect.equal (Decode.decodeCommandLineTool (lib "{$import: lib.yml}")) (Decode.decodeCommandLineTool (lib "[{$import: lib.yml}]")) "The spelling is layout."
            Expect.stringContains (roundTrip "import" (lib "{$import: lib.yml}")) "- {$import: lib.yml}" "The import is written as an entry."
            failsAt "an include" (lib "{$include: lib.js}") "requirements[0].expressionLib" "An $include yields the text of a file, but the expressionLib field takes a list of strings."
            failsAt "a text" (lib "var a = 1;") "requirements[0].expressionLib" "Expected a list of strings"
    ]

let testRepeatedEnumSymbols =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let other (cls: string) (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: " + cls ] @ lines @ [ "" ])
    // The place of an enum, the document with {0} where the symbols stand.
    let places =
        [ "the type of an input", tool [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: []" ]
          "the type of an output", tool [ "inputs: []"; "outputs:"; "  o:"; "    type:"; "      type: enum"; "      symbols: {0}"; "    outputBinding: {glob: o}" ]
          "a definition that no port uses", tool [ "requirements:"; "  SchemaDefRequirement:"; "    types:"; "      - type: enum"; "        name: E"; "        symbols: {0}"; "inputs: []"; "outputs: []" ]
          "a definition in the hints", tool [ "hints:"; "  SchemaDefRequirement:"; "    types:"; "      - type: enum"; "        name: E"; "        symbols: {0}"; "inputs: []"; "outputs: []" ]
          "the type of a field of a record", tool [ "inputs:"; "  a:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type:"; "            type: enum"; "            symbols: {0}"; "outputs: []" ]
          "the items of an array", tool [ "inputs:"; "  a:"; "    type:"; "      type: array"; "      items:"; "        type: enum"; "        symbols: {0}"; "outputs: []" ]
          "a member of a union", tool [ "inputs:"; "  a:"; "    type:"; "      - 'null'"; "      - type: enum"; "        symbols: {0}"; "outputs: []" ]
          "the input of a workflow", other "Workflow" [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: {}"; "steps: {}" ]
          "the input of an expression tool", other "ExpressionTool" [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: {}"; "expression: $(1)" ]
          "the input of an operation", other "Operation" [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: {0}"; "outputs: {}" ] ]
    // Spellings of symbols that cwltool reads as the same symbol, with the index of the symbol that repeats.
    let repeated =
        [ "[a, b, a]", 2
          "[a, a]", 1
          "[a, \"#a\"]", 1
          "[x/a, y/a]", 1
          "[x#a, a]", 1
          "[p:a, q:a]", 1
          "[./a, a]", 1
          "[\"http://example.org/a\", a]", 1
          "['a/', 'b/']", 1
          "['', '']", 1
          "[\"a#\", a]", 1
          "[\"x/#\", \"y/#\"]", 1
          "[\"#\", \"#\"]", 1
          "[\"p:a#\", a]", 1
          "[\"a?q#\", a]", 1
          "[\"x/a#\", \"y/a\"]", 1
          "[a, {$import: s.yml}, a]", 2 ]
    // Spellings that cwltool reads as different symbols, or that the decoder cannot compare.
    let distinct =
        [ "[a, A]"; "[x#y#a, a]"; "['', a]"; "[a, b, c]"; "[x/a, x/b]"; "[\"a#\", \"b#\"]"; "[\"http://x.org/a#\", \"http://x.org/b#\"]"; "[\"#\", a]"; "[\"#a\", \"b#\"]"; "[a, {$import: s.yml}]"; "[{$import: s.yml}, {$import: s.yml}]"; "[{$include: t.txt}, {$include: t.txt}]"; "{$import: s.yml}" ]
    testList "Repeated enum symbols" [
        testCase "an enum that repeats a symbol is a decode error at the repeat, whichever way the symbols are spelled" <| fun _ ->
            for (place, template) in places do
                for (symbols, index) in repeated do
                    match (try Decode.decodeCWLDocument (template.Replace("{0}", symbols)) |> ignore; None with YamlDecodeException error -> Some error) with
                    | Some error ->
                        Expect.isTrue ((DecodeError.pathText error).EndsWith (sprintf "symbols[%d]" index)) (sprintf "%s %s: the error should show the repeat, found %s" place symbols (DecodeError.pathText error))
                        Expect.stringContains error.Message "repeats the symbol at line" (sprintf "%s %s: the error should say what is wrong" place symbols)
                        Expect.isSome error.Span (sprintf "%s %s: the error should have the place in the text" place symbols)
                    | None -> failwith (sprintf "%s %s must fail" place symbols)
        testCase "symbols that differ, and directives that stand for symbols of files, decode" <| fun _ ->
            for (place, template) in places do
                for symbols in distinct do
                    Decode.decodeCWLDocument (template.Replace("{0}", symbols)) |> ignore
        testCase "the rule holds in a document of v1.0" <| fun _ ->
            let text = "cwlVersion: v1.0\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  a:\n    type:\n      type: enum\n      symbols: [a, a]\noutputs: []\n"
            match (try Decode.decodeCWLDocument text |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error -> Expect.equal (DecodeError.pathText error) "inputs.a.type.symbols[1]" "The error shows the repeat."
            | None -> failwith "A repeated symbol must fail."
    ]

let testSchemasField =
    let tool (schemas: string) =
        "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: []\noutputs: []\n$schemas:" + schemas + "\n"
    let packed (schemas: string) =
        "cwlVersion: v1.2\n$graph:\n  - class: CommandLineTool\n    id: main\n    baseCommand: echo\n    inputs: []\n    outputs: []\n$schemas:" + schemas + "\n"
    testList "The $schemas field" [
        testCase "a path or a list of paths is kept as an extension field and written back" <| fun _ ->
            for schemas in [ " []"; " schema.rdf"; " [schema.rdf]"; " [a.rdf, b.rdf]"; " \"2\"" ] do
                let decoded = Decode.decodeCommandLineTool (tool schemas)
                Expect.isSome decoded.Metadata (sprintf "%s: the field is an extension field." schemas)
                Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded (sprintf "%s: the written text should decode to the same tool." schemas)
                Decode.decodeCWLDocument (packed schemas) |> ignore
        testCase "a directive, a mapping, a nested list, a number and null are decode errors, as cwltool refuses them" <| fun _ ->
            let cases =
                [ "an import in a list", " [{$import: a.yml}]", "$schemas[0]", "Expected the path of a schema file as a string, found a mapping."
                  "an include in a list", " [{$include: a.txt}]", "$schemas[0]", "Expected the path of a schema file as a string, found a mapping."
                  "an import as the whole field", " {$import: a.yml}", "$schemas", "Expected the path of a schema file or a list of paths, found a mapping."
                  "a mapping", " {a: b}", "$schemas", "Expected the path of a schema file or a list of paths, found a mapping."
                  "a nested list", " [[a]]", "$schemas[0]", "Expected the path of a schema file as a string, found a list."
                  "a number in a list", " [1]", "$schemas[0]", "Expected a string, found the plain scalar 1"
                  "null", "", "$schemas", "Expected a string, found null" ]
            for (name, schemas, path, message) in cases do
                for (kind, text) in [ "tool", tool schemas; "packed", packed schemas ] do
                    match (try Decode.decodeCWLDocument text |> ignore; None with YamlDecodeException error -> Some error) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "%s, %s: the error should show the field" name kind)
                        Expect.stringContains error.Message message (sprintf "%s, %s: the error should say what is wrong" name kind)
                        Expect.isSome error.Span (sprintf "%s, %s: the error should have the place in the text" name kind)
                    | None -> failwith (sprintf "%s, %s must fail" name kind)
    ]

let testKeysAndIdsThatCwltoolReadsAsAnotherType =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let workflow (stepLines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs:"; "  a: string"; "outputs: {}"; "steps:" ] @ stepLines @ [ "" ])
    // The place of a key or an id, and the document with {0} where the quoted text stands.
    let places =
        [ "the key of an input", tool [ "inputs:"; "  \"{0}\": string"; "outputs: []" ]
          "the key of an output", tool [ "inputs: []"; "outputs:"; "  \"{0}\": {type: File, outputBinding: {glob: x}}" ]
          "the id of an input beside an import", tool [ "inputs:"; "  - $import: ins.yml"; "  - id: \"{0}\""; "    type: string"; "outputs: []" ]
          "the key of a field of a record", tool [ "inputs:"; "  r:"; "    type:"; "      type: record"; "      fields:"; "        \"{0}\": string"; "outputs: []" ]
          "the name of a field of a record in a list", tool [ "inputs:"; "  r:"; "    type:"; "      type: record"; "      fields:"; "        - {name: \"{0}\", type: string}"; "outputs: []" ]
          "the key of an environment variable", tool [ "requirements:"; "  EnvVarRequirement:"; "    envDef:"; "      \"{0}\": v"; "inputs: []"; "outputs: []" ]
          "the key of a package", tool [ "hints:"; "  SoftwareRequirement:"; "    packages:"; "      \"{0}\": {}"; "inputs: []"; "outputs: []" ]
          "the key of a hint of an unknown class", tool [ "hints:"; "  \"{0}\": {x: 1}"; "inputs: []"; "outputs: []" ]
          "the key of a step", workflow [ "  \"{0}\":"; "    run: t.cwl"; "    in: {x: a}"; "    out: [o]" ]
          "the key of a step input", workflow [ "  s:"; "    run: t.cwl"; "    in: {\"{0}\": a}"; "    out: [o]" ]
          "the key of a step with a mixin", workflow [ "  \"{0}\":"; "    $mixin: stepmix.yml" ] ]
    let keys = [ "1_000"; "0b1"; "1_0"; "0x1F"; "1e3"; "1_0.5"; "0o7"; "="; "<<"; "yes"; "017"; "._" ]
    testList "Keys and ids that cwltool reads as another type" [
        testCase "a key or an id that cwltool would read as another type is written in quotes, and the written text reads back equal" <| fun _ ->
            for (place, template) in places do
                for key in keys do
                    let text = template.Replace("{0}", key)
                    let decoded = Decode.decodeCWLDocument text
                    let written = Encode.encodeCWLDocument decoded
                    Expect.stringContains written ("\"" + key + "\"") (sprintf "%s %s: the text is written in quotes.\n%s" place key written)
                    let reread = Decode.decodeCWLDocument written
                    Expect.equal reread decoded (sprintf "%s %s: the written text should decode to the same document." place key)
                    Expect.equal (Encode.encodeCWLDocument reread) written (sprintf "%s %s: writing again should give the same text." place key)
        testCase "a key that is plain text stays plain" <| fun _ ->
            let written = Encode.encodeCWLDocument (Decode.decodeCWLDocument (tool [ "inputs:"; "  input_1: string"; "outputs: []" ]))
            Expect.stringContains written "  input_1: string" "A key that no reader takes for another type needs no quotes."
        testCase "the key of an entry of a job file is written in quotes when cwltool would read another type" <| fun _ ->
            for key in keys do
                let references = DecodeParameters.decodeYAMLParameterFile ("\"" + key + "\": a\n")
                let written = Encode.encodeYAMLParameterFile references
                Expect.stringContains written ("\"" + key + "\": a") (sprintf "%s: the key is written in quotes.\n%s" key written)
                Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references (sprintf "%s: the written file should decode to the same references." key)
    ]

/// A place in a document that holds one field: the document with a given text as the value of the field, the way to read
/// the document and write it, and the way to read the field from the decoded object.
type private FieldPosition<'T> =
    { Name: string
      /// The path of the field in the errors.
      Path: string
      Document: string -> string
      Decode: string -> obj
      Encode: obj -> string
      Read: obj -> 'T option }

let testDirectivesInBoolIntAndEnumFields =
    let toolOf (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let workflowOf (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow" ] @ lines @ [ "" ])
    let stepWorkflow (stepLines: string list) (inputLines: string list) =
        workflowOf (
            [ "inputs:"; "  x: string"; "outputs: {}"; "steps:"; "  s:"; "    run:"; "      class: CommandLineTool"; "      baseCommand: echo"; "      inputs:"; "        a: string"; "      outputs: []" ]
            @ (stepLines |> List.map (fun line -> "    " + line))
            @ [ "    in:"; "      a:" ]
            @ (inputLines |> List.map (fun line -> "        " + line))
            @ [ "    out: []" ])
    let toolPosition (name: string) (path: string) (lines: string -> string list) (read: CWLToolDescription -> 'T option) : FieldPosition<'T> =
        { Name = name
          Path = path
          Document = fun value -> toolOf (lines value)
          Decode = fun text -> box (Decode.decodeCommandLineTool text)
          Encode = fun decoded -> Encode.encodeToolDescription (unbox<CWLToolDescription> decoded)
          Read = fun decoded -> read (unbox<CWLToolDescription> decoded) }
    let workflowPosition (name: string) (path: string) (lines: string -> string list) (read: CWLWorkflowDescription -> 'T option) : FieldPosition<'T> =
        { Name = name
          Path = path
          Document = fun value -> workflowOf (lines value)
          Decode = fun text -> box (Decode.decodeWorkflow text)
          Encode = fun decoded -> Encode.encodeWorkflowDescription (unbox<CWLWorkflowDescription> decoded)
          Read = fun decoded -> read (unbox<CWLWorkflowDescription> decoded) }
    let stepPosition (name: string) (path: string) (document: string -> string) (read: WorkflowStep -> 'T option) : FieldPosition<'T> =
        { Name = name
          Path = path
          Document = document
          Decode = fun text -> box (Decode.decodeWorkflow text)
          Encode = fun decoded -> Encode.encodeWorkflowDescription (unbox<CWLWorkflowDescription> decoded)
          Read = fun decoded -> read (ListEntry.values (unbox<CWLWorkflowDescription> decoded).Steps).[0] }
    let firstInput (tool: CWLToolDescription) = (ListEntry.values tool.Inputs.Value).[0]
    let firstOutput (tool: CWLToolDescription) = (ListEntry.values tool.Outputs).[0]
    let firstStepInput (step: WorkflowStep) = (WorkflowStep.getInputValues step).[0]
    let mixed (text: string) = { Node = YAMLicious.Reader.readRoot text }
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let fieldOf (position: FieldPosition<'T>) = position.Path.Substring(position.Path.LastIndexOf '.' + 1)
    let expectDirectives (position: FieldPosition<'T>) (variants: (string * 'T) list) =
        for (value, expected) in variants do
            let decoded = position.Decode (position.Document value)
            Expect.equal (position.Read decoded) (Some expected) (sprintf "%s: %s should decode to a directive of its own." position.Name value)
            let written = position.Encode decoded
            if value.StartsWith "{" then
                Expect.stringContains written value (sprintf "%s: %s should be written back as it was.\n%s" position.Name value written)
            let reread = position.Decode written
            Expect.equal reread decoded (sprintf "%s: the written text of %s should decode to the same object." position.Name value)
            Expect.equal (position.Encode reread) written (sprintf "%s: writing again should give the same text." position.Name)
    let expectFailures (position: FieldPosition<'T>) (failures: (string * string) list) =
        for (value, message) in failures do
            failsAt (position.Name + " " + value) (fun () -> position.Decode (position.Document value) |> ignore) position.Path message

    let boolPositions : FieldPosition<SchemaSaladBool> list =
        let binding (field: string) (itype: string) (value: string) =
            [ "inputs:"; "  x:"; "    type: " + itype; "    inputBinding: {" + field + ": " + value + "}"; "outputs: {}" ]
        let port (field: string) (value: string) =
            [ "inputs:"; "  x:"; "    type: File"; "    " + field + ": " + value; "outputs: {}" ]
        [ toolPosition "the loadContents of an inputBinding" "inputs.x.inputBinding.loadContents" (binding "loadContents" "File") (fun tool -> (inputBindingOf (firstInput tool)).LoadContents)
          toolPosition "the separate of an inputBinding" "inputs.x.inputBinding.separate" (binding "separate" "string") (fun tool -> (inputBindingOf (firstInput tool)).Separate)
          toolPosition "the shellQuote of an inputBinding" "inputs.x.inputBinding.shellQuote" (binding "shellQuote" "string") (fun tool -> (inputBindingOf (firstInput tool)).ShellQuote)
          toolPosition "the loadContents of an input" "inputs.x.loadContents" (port "loadContents") (fun tool -> (firstInput tool).LoadContents)
          toolPosition "the streamable of an input" "inputs.x.streamable" (port "streamable") (fun tool -> (firstInput tool).Streamable)
          toolPosition "the loadContents of an outputBinding" "outputs.o.outputBinding.loadContents"
              (fun value -> [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    outputBinding: {glob: o.txt, loadContents: " + value + "}" ])
              (fun tool -> (outputBindingOf (firstOutput tool)).LoadContents)
          toolPosition "the streamable of an output" "outputs.o.streamable"
              (fun value -> [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    streamable: " + value ])
              (fun tool -> (firstOutput tool).Streamable)
          toolPosition "the inplaceUpdate of the requirement" "requirements[0].inplaceUpdate"
              (fun value -> [ "requirements:"; "  - class: InplaceUpdateRequirement"; "    inplaceUpdate: " + value; "inputs: {}"; "outputs: {}" ])
              (fun tool ->
                  match tool.Requirements.Value.[0] with
                  | InplaceUpdateRequirement requirement -> Some requirement.InplaceUpdate
                  | _ -> None)
          toolPosition "the writable of a Dirent" "requirements[0].listing[0].writable"
              (fun value -> [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - entryname: a.txt"; "        entry: x"; "        writable: " + value; "inputs: {}"; "outputs: {}" ])
              (fun tool ->
                  match tool.Requirements.Value.[0] with
                  | InitialWorkDirRequirement entries ->
                      match entries.[0] with
                      | DirentEntry dirent -> dirent.Writable
                      | _ -> None
                  | _ -> None)
          workflowPosition "the streamable of a workflow input" "inputs.x.streamable"
              (fun value -> [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + value; "outputs: {}"; "steps: {}" ])
              (fun workflow -> (ListEntry.values workflow.Inputs).[0].Streamable)
          stepPosition "the loadContents of a step input" "steps.s.in.a.loadContents"
              (fun value -> stepWorkflow [] [ "source: x"; "loadContents: " + value ])
              (fun step -> (firstStepInput step).LoadContents) ]

    let loadListingPositions : FieldPosition<LoadListingEnum> list =
        [ toolPosition "the loadListing of an input" "inputs.x.loadListing"
              (fun value -> [ "inputs:"; "  x:"; "    type: Directory"; "    loadListing: " + value; "outputs: {}" ])
              (fun tool -> (firstInput tool).LoadListing)
          toolPosition "the loadListing of an outputBinding" "outputs.o.outputBinding.loadListing"
              (fun value -> [ "inputs: {}"; "outputs:"; "  o:"; "    type: Directory"; "    outputBinding: {glob: o, loadListing: " + value + "}" ])
              (fun tool -> (outputBindingOf (firstOutput tool)).LoadListing)
          toolPosition "the loadListing of the requirement" "requirements[0].loadListing"
              (fun value -> [ "requirements:"; "  - class: LoadListingRequirement"; "    loadListing: " + value; "inputs: {}"; "outputs: {}" ])
              (fun tool ->
                  match tool.Requirements.Value.[0] with
                  | LoadListingRequirement requirement -> Some requirement.LoadListing
                  | _ -> None)
          stepPosition "the loadListing of a step input" "steps.s.in.a.loadListing"
              (fun value -> stepWorkflow [] [ "source: x"; "loadListing: " + value ])
              (fun step -> (firstStepInput step).LoadListing) ]
    let linkMergePosition =
        stepPosition "the linkMerge of a step input" "steps.s.in.a.linkMerge"
            (fun value -> stepWorkflow [] [ "source: [x, x]"; "linkMerge: " + value ])
            (fun step -> (firstStepInput step).LinkMerge)
    let pickValuePosition =
        stepPosition "the pickValue of a step input" "steps.s.in.a.pickValue"
            (fun value -> stepWorkflow [] [ "source: [x, x]"; "pickValue: " + value ])
            (fun step -> (firstStepInput step).PickValue)
    let scatterMethodPosition =
        stepPosition "the scatterMethod of a step" "steps.s.scatterMethod"
            (fun value -> stepWorkflow [ "scatter: a"; "scatterMethod: " + value ] [ "source: x" ])
            (fun step -> step.ScatterMethod)
    let positionPosition =
        toolPosition "the position of an inputBinding" "inputs.x.inputBinding.position"
            (fun value -> [ "inputs:"; "  x:"; "    type: string"; "    inputBinding: {position: " + value + "}"; "outputs: {}" ])
            (fun tool -> (inputBindingOf (firstInput tool)).Position)
    let sizePosition =
        toolPosition "the size of a File in a listing" "requirements[0].listing[0].size"
            (fun value -> [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - class: File"; "        location: f.txt"; "        size: " + value; "inputs: {}"; "outputs: {}" ])
            (fun tool ->
                match tool.Requirements.Value.[0] with
                | InitialWorkDirRequirement entries ->
                    match entries.[0] with
                    | FileEntry file -> file.Size
                    | _ -> None
                | _ -> None)

    testList "Directives in the bool, int and enum fields" [
        testCase "an import or a mixin stands for the bool of every bool field, is written back as it was and reads back equal" <| fun _ ->
            for position in boolPositions do
                expectDirectives position [ "{$import: b.yml}", SchemaSaladBool.Imported "b.yml"
                                            "{$mixin: b.yml}", SchemaSaladBool.Mixed (mixed "{$mixin: b.yml}")
                                            "{$mixin: b.yml, extra: 1}", SchemaSaladBool.Mixed (mixed "{$mixin: b.yml, extra: 1}") ]
        testCase "a plain bool is still a value, and an include, a text or a mapping that is no directive are decode errors" <| fun _ ->
            for position in boolPositions do
                expectDirectives position [ "true", SchemaSaladBool.Value true; "false", SchemaSaladBool.Value false ]
                expectFailures position [ "{$include: b.txt}", "An $include yields the text of a file, but the " + fieldOf position + " field takes a boolean. Write an $import for a file that holds the boolean."
                                          "{$import: b.yml, extra: 1}", "A directive has exactly one field"
                                          "{a: b}", "Expected a bool, found a mapping."
                                          "\"true\"", "Expected a bool" ]
        testCase "a directive is no bool value: it differs from true, from false and from a missing field" <| fun _ ->
            let position = List.head boolPositions
            let withValue (value: string) = position.Decode (position.Document value)
            Expect.notEqual (withValue "{$import: b.yml}") (withValue "true") "An import is no true."
            Expect.notEqual (withValue "{$import: b.yml}") (withValue "false") "An import is no false."
            Expect.notEqual (withValue "{$import: b.yml}") (withValue "{$import: c.yml}") "Two imports differ by path."
            Expect.notEqual (withValue "{$import: b.yml}") (withValue "{$mixin: b.yml}") "An import and a mixin differ."
            Expect.equal (SchemaSaladBool.tryValue (SchemaSaladBool.Value false)) (Some false) "A value gives its bool."
            Expect.isNone (SchemaSaladBool.tryValue (SchemaSaladBool.Imported "b.yml")) "An import gives no bool."
            Expect.isNone (SchemaSaladBool.tryValue (SchemaSaladBool.Mixed (mixed "{$mixin: b.yml}"))) "A mixin gives no bool."
            let missing = Decode.decodeCommandLineTool (toolOf [ "inputs:"; "  x:"; "    type: File"; "outputs: {}" ])
            Expect.isNone (firstInput missing).LoadContents "A missing field is none."
        testCase "the position of an inputBinding may be a directive, and an include stands for an expression" <| fun _ ->
            expectDirectives positionPosition [ "{$import: p.yml}", BindingPosition.Imported "p.yml"
                                                "{$include: p.txt}", BindingPosition.Included "p.txt"
                                                "{$mixin: p.yml}", BindingPosition.Mixed (mixed "{$mixin: p.yml}")
                                                "{$mixin: p.yml, extra: 1}", BindingPosition.Mixed (mixed "{$mixin: p.yml, extra: 1}")
                                                "3", BindingPosition.Number 3
                                                "\"$(1 + 1)\"", BindingPosition.Expression "$(1 + 1)" ]
            expectFailures positionPosition [ "{$import: p.yml, extra: 1}", "A directive has exactly one field"
                                              "{a: b}", "Expected an int or an expression, found a mapping."
                                              "\"abc\"", "Expected an int or an expression" ]
            Expect.notEqual (BindingPosition.Imported "p.yml") (BindingPosition.Included "p.yml") "An import and an include differ."
            Expect.notEqual (BindingPosition.Imported "p.yml") (BindingPosition.Expression "p.yml") "An import is no expression."
        testCase "the loadListing of an input, an outputBinding, the requirement and a step input may be a directive" <| fun _ ->
            for position in loadListingPositions do
                expectDirectives position [ "{$import: l.yml}", ImportedLoadListing "l.yml"
                                            "{$include: l.txt}", IncludedLoadListing "l.txt"
                                            "{$mixin: l.yml}", MixedLoadListing (mixed "{$mixin: l.yml}")
                                            "{$mixin: l.yml, extra: 1}", MixedLoadListing (mixed "{$mixin: l.yml, extra: 1}")
                                            "deep_listing", DeepListing ]
                expectFailures position [ "{$import: l.yml, extra: 1}", "A directive has exactly one field"
                                          "{a: b}", "Expected a string, found a mapping."
                                          "foo", "Expected one of no_listing, shallow_listing and deep_listing" ]
        testCase "the linkMerge, the pickValue and the scatterMethod may be a directive" <| fun _ ->
            expectDirectives linkMergePosition [ "{$import: m.yml}", ImportedLinkMerge "m.yml"
                                                 "{$include: m.txt}", IncludedLinkMerge "m.txt"
                                                 "{$mixin: m.yml, extra: 1}", MixedLinkMerge (mixed "{$mixin: m.yml, extra: 1}")
                                                 "merge_flattened", MergeFlattened ]
            expectDirectives pickValuePosition [ "{$import: m.yml}", ImportedPickValue "m.yml"
                                                 "{$include: m.txt}", IncludedPickValue "m.txt"
                                                 "{$mixin: m.yml}", MixedPickValue (mixed "{$mixin: m.yml}")
                                                 "all_non_null", AllNonNull ]
            expectDirectives scatterMethodPosition [ "{$import: m.yml}", ImportedScatterMethod "m.yml"
                                                     "{$include: m.txt}", IncludedScatterMethod "m.txt"
                                                     "{$mixin: m.yml, extra: 1}", MixedScatterMethod (mixed "{$mixin: m.yml, extra: 1}")
                                                     "flat_crossproduct", FlatCrossProduct ]
            expectFailures linkMergePosition [ "{$import: m.yml, extra: 1}", "A directive has exactly one field"; "merge_all", "Invalid linkMerge value: merge_all" ]
            expectFailures pickValuePosition [ "{$import: m.yml, extra: 1}", "A directive has exactly one field"; "any", "Invalid pickValue value: any" ]
            expectFailures scatterMethodPosition [ "{$import: m.yml, extra: 1}", "A directive has exactly one field"; "cross", "Invalid scatterMethod value: cross" ]
        testCase "a directive in an enum shows as the directive in the text of the symbol and differs from every symbol" <| fun _ ->
            Expect.equal (LoadListingEnum.toCwlString (ImportedLoadListing "l.yml")) "$import: l.yml" "The import shows with its directive."
            Expect.equal (LoadListingEnum.toCwlString (IncludedLoadListing "l.txt")) "$include: l.txt" "The include shows with its directive."
            Expect.equal (LoadListingEnum.toCwlString (MixedLoadListing (mixed "{$mixin: l.yml, extra: 1}"))) "$mixin: l.yml" "The mixin shows with its path."
            Expect.equal (ImportedScatterMethod "m.yml").AsCwlString "$import: m.yml" "The scatterMethod shows the import."
            Expect.equal (IncludedPickValue "m.txt").AsCwlString "$include: m.txt" "The pickValue shows the include."
            Expect.equal (ImportedLinkMerge "m.yml").AsCwlString "$import: m.yml" "The linkMerge shows the import."
            Expect.notEqual (ImportedLoadListing "deep_listing") DeepListing "An import of a file called deep_listing is no deep listing."
            Expect.notEqual (ImportedLoadListing "l.yml") (IncludedLoadListing "l.yml") "An import and an include differ."
            Expect.isNone (LoadListingEnum.tryParse "$import: l.yml") "The text of a directive is no symbol."
        testCase "the size of a File in a listing may be a directive" <| fun _ ->
            expectDirectives sizePosition [ "{$import: s.yml}", FileSize.Imported "s.yml"
                                            "{$mixin: s.yml}", FileSize.Mixed (mixed "{$mixin: s.yml}")
                                            "{$mixin: s.yml, extra: 1}", FileSize.Mixed (mixed "{$mixin: s.yml, extra: 1}")
                                            "42", FileSize.Bytes 42L ]
            expectFailures sizePosition [ "{$include: s.txt}", "An $include yields the text of a file, but the size field takes a number. Write an $import for a file that holds the number."
                                          "{$import: s.yml, extra: 1}", "A directive has exactly one field"
                                          "{a: b}", "Expected an int"
                                          "\"42\"", "Expected an int" ]
            Expect.equal (FileSize.tryBytes (FileSize.Bytes 3L)) (Some 3L) "A size gives its bytes."
            Expect.isNone (FileSize.tryBytes (FileSize.Imported "s.yml")) "An import gives no bytes."
            Expect.notEqual (FileSize.Imported "s.yml") (FileSize.Bytes 0L) "An import is no size of zero."
    ]

let testSmallRejectionsOfCwltool =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines) + "\n"
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Small rejections of cwltool" [
        testCase "a $namespaces that is no mapping is a decode error at the field, wherever an object of the document holds it" <| fun _ ->
            let decodeTool (text: string) = Decode.decodeCommandLineTool text |> ignore
            let message = "The field $namespaces takes a mapping of prefixes to IRIs"
            failsAt "a list at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: [ex]" ])) "$namespaces" (message + ", found a list.")
            failsAt "a text at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: x" ])) "$namespaces" (message + ", found the text")
            failsAt "a number at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: 5" ])) "$namespaces" message
            failsAt "null at the root" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces:" ])) "$namespaces" message
            failsAt "a list of mappings" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: [{ex: 'http://example.org/'}]" ])) "$namespaces" message
            failsAt "an empty list" (fun () -> decodeTool (tool [ "inputs: []"; "outputs: []"; "$namespaces: []" ])) "$namespaces" message
            failsAt "an input" (fun () -> decodeTool (tool [ "inputs:"; "  x:"; "    type: string"; "    $namespaces: [ex]"; "outputs: []" ])) "inputs.x.$namespaces" message
            failsAt "a requirement" (fun () -> decodeTool (tool [ "requirements:"; "  - class: ShellCommandRequirement"; "    $namespaces: [ex]"; "inputs: []"; "outputs: []" ])) "requirements[0].$namespaces" message
            failsAt "a default value" (fun () -> decodeTool (tool [ "inputs:"; "  x:"; "    type: Any"; "    default:"; "      a:"; "        - $namespaces: x"; "outputs: []" ])) "inputs.x.default.a[0].$namespaces" message
            failsAt "an unknown hint" (fun () -> decodeTool (tool [ "hints:"; "  - class: ex:Hint"; "    payload:"; "      $namespaces: [ex]"; "inputs: []"; "outputs: []" ])) "hints[0].payload.$namespaces" message
            failsAt "an inline run" (fun () ->
                Decode.decodeWorkflow (String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run:"; "      $namespaces: [ex]"; "      class: CommandLineTool"; "      baseCommand: echo"; "      inputs: []"; "      outputs: []"; "    in: {}"; "    out: []"; "" ]) |> ignore) "steps.s.run.$namespaces" message
            failsAt "a process of a packed document" (fun () ->
                Decode.decodeCWLDocument (String.concat "\n" [ "cwlVersion: v1.2"; "$graph:"; "  - id: main"; "    $namespaces: [ex]"; "    class: CommandLineTool"; "    baseCommand: echo"; "    inputs: []"; "    outputs: []"; "" ]) |> ignore) "$graph[0].$namespaces" message
            failsAt "a job file" (fun () -> DecodeParameters.decodeYAMLParameterFile "x: a\n$namespaces: [ex]\n" |> ignore) "$namespaces" message
        testCase "a $namespaces that is a mapping or a directive decodes, whatever its values are" <| fun _ ->
            for value in [ "{ex: 'http://example.org/'}"; "{}"; "{ex: 1}"; "{ex: true}"; "{ex: }"; "{ex: [a]}"; "{ex: {a: b}}"; "{1: 'http://example.org/'}"; "{$import: a.yml}"; "{$include: a.yml}"; "{$mixin: a.yml}" ] do
                let text = tool [ "inputs: []"; "outputs: []"; "$namespaces: " + value ]
                let decoded = Decode.decodeCommandLineTool text
                let written = Encode.encodeToolDescription decoded
                Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool." value)
            Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: string"; "    $namespaces: {ex: 'http://example.org/'}"; "outputs: []" ]) |> ignore
            DecodeParameters.decodeYAMLParameterFile "x: a\n$namespaces: {ex: 'http://example.org/'}\n" |> ignore
        testCase "a $namespaces beside a $mixin or below it is no error, because the fields drop out when the file holds a scalar" <| fun _ ->
            let withStreamable (value: string) = tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + value; "outputs: []" ]
            for value in [ "{$mixin: t.yml, $namespaces: 5}"; "{$mixin: t.yml, $namespaces: [ex]}"; "{$mixin: t.yml, $namespaces:}"; "{$mixin: t.yml, a: {$namespaces: 5}}"; "{$mixin: t.yml, a: {b: [{$namespaces: [ex]}]}}" ] do
                let decoded = Decode.decodeCommandLineTool (withStreamable value)
                match (ListEntry.values decoded.Inputs.Value).[0].Streamable with
                | Some (SchemaSaladBool.Mixed raw) -> Expect.equal raw.MixinPath "t.yml" (sprintf "%s: the mixin keeps its path." value)
                | other -> failwith (sprintf "%s: expected a mixin, found %A" value other)
                let written = Encode.encodeToolDescription decoded
                Expect.stringContains written value (sprintf "%s: the mapping is written as it was." value)
                Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool." value)
            let label = Decode.decodeCommandLineTool (tool [ "label: {$mixin: l.yml, $namespaces: 5}"; "inputs: []"; "outputs: []" ])
            Expect.equal label.Label (Some (SchemaSaladString.Mixed { Node = Reader.readRoot "{$mixin: l.yml, $namespaces: 5}" })) "A string with a mixin keeps the mapping."
        testCase "a $namespaces outside of a mixin stays an error, and so do the anchors and repeated keys below a mixin" <| fun _ ->
            let withStreamable (value: string) = tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: " + value; "outputs: []" ]
            failsAt "a binding without a mixin" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding: {prefix: -x, $namespaces: 5}"; "outputs: []" ]) |> ignore) "inputs.x.inputBinding.$namespaces" "The field $namespaces takes a mapping"
            failsAt "a field next to the mixin" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: {$mixin: t.yml}"; "    ex:note: {$namespaces: 5}"; "outputs: []" ]) |> ignore) "inputs.x.ex:note.$namespaces" "The field $namespaces takes a mapping"
            failsAt "an import beside the field" (fun () -> Decode.decodeCommandLineTool (withStreamable "{$import: t.yml, $namespaces: 5}") |> ignore) "inputs.x.streamable.$namespaces" "The field $namespaces takes a mapping"
            failsAt "a repeated key below a mixin" (fun () -> Decode.decodeCommandLineTool (withStreamable "{$mixin: t.yml, a: 1, a: 2}") |> ignore) "inputs.x.streamable.a" "The key a occurs twice"
            failsAt "an anchor below a mixin" (fun () -> Decode.decodeCommandLineTool (withStreamable "{$mixin: t.yml, a: &anc 1, b: *anc}") |> ignore) "inputs.x.streamable.a" "The anchor &anc is not allowed"
        testCase "an $include where an inputBinding or an outputBinding stands is a decode error, and an import and a mixin are not" <| fun _ ->
            let withInputBinding (value: string) = tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding: " + value; "outputs: []" ]
            let withOutputBinding (value: string) = tool [ "inputs: []"; "outputs:"; "  o:"; "    type: string"; "    outputBinding: " + value ]
            failsAt "an inputBinding include" (fun () -> Decode.decodeCommandLineTool (withInputBinding "{$include: f.txt}") |> ignore) "inputs.x.inputBinding" "An $include yields the text of a file, but the inputBinding field takes an object. Write an $import for a file that holds the object."
            failsAt "an outputBinding include" (fun () -> Decode.decodeCommandLineTool (withOutputBinding "{$include: f.txt}") |> ignore) "outputs.o.outputBinding" "An $include yields the text of a file, but the outputBinding field takes an object. Write an $import for a file that holds the object."
            failsAt "an inputBinding include beside a field" (fun () -> Decode.decodeCommandLineTool (withInputBinding "{$include: f.txt, prefix: -x}") |> ignore) "inputs.x.inputBinding" "A directive has exactly one field"
            failsAt "an outputBinding import beside a field" (fun () -> Decode.decodeCommandLineTool (withOutputBinding "{$import: f.yml, glob: o.txt}") |> ignore) "outputs.o.outputBinding" "A directive has exactly one field"
            for (value, written) in [ "{$import: f.yml}", "$import: f.yml"; "{$mixin: f.yml}", "$mixin: f.yml"; "{$mixin: f.yml, prefix: -x}", "$mixin: f.yml" ] do
                let decoded = Decode.decodeCommandLineTool (withInputBinding value)
                Expect.isSome (ListEntry.values decoded.Inputs.Value).[0].InputBinding (sprintf "%s: the binding stays." value)
                Expect.stringContains (Encode.encodeToolDescription decoded) written (sprintf "%s: the directive is written back." value)
            for (value, written) in [ "{$import: f.yml}", "$import: f.yml"; "{$mixin: f.yml}", "$mixin: f.yml"; "{$mixin: f.yml, glob: o.txt}", "$mixin: f.yml" ] do
                let decoded = Decode.decodeCommandLineTool (withOutputBinding value)
                Expect.isSome (ListEntry.values decoded.Outputs).[0].OutputBinding (sprintf "%s: the binding stays." value)
                Expect.stringContains (Encode.encodeToolDescription decoded) written (sprintf "%s: the directive is written back." value)
    ]

let testIntsBeyond64BitsInNumberPositions =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let resource (field: string) (value: string) =
        tool [ "requirements:"; "  - class: ResourceRequirement"; "    " + field + ": " + value; "inputs: []"; "outputs: []" ]
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let coresMinOf (tool: CWLToolDescription) : obj option =
        match tool.Requirements.Value.[0] with
        | ResourceRequirement resources -> resources.CoresMin
        | other -> failwith (sprintf "Expected a ResourceRequirement, found %A" other)
    let powerOfTwo (exponent: int) =
        let mutable value = 1.0
        for _ in 1 .. exponent do
            value <- value * 2.0
        value
    let bigDecimal = "100000000000000000000000000000"
    testList "Ints beyond 64 bits in the number positions" [
        testCase "an int beyond 64 bits is the nearest float in a resource field, in every base of the core schema, and it is written back as a float" <| fun _ ->
            for (text, expected) in
                [ bigDecimal, 1e29
                  "-" + bigDecimal, -1e29
                  "+9223372036854775808", 9223372036854775808.0
                  "-9223372036854775809", -9223372036854775808.0
                  "0x8000000000000000", powerOfTwo 63
                  "0x400000000000000000", powerOfTwo 70
                  "0x10000000000000800", 18446744073709551616.0
                  "0x10000000000000801", 18446744073709555712.0
                  "0x10000000000001800", 18446744073709559808.0
                  "0o7777777777777777777777777", powerOfTwo 75 ] do
                let decoded = Decode.decodeCommandLineTool (resource "coresMin" text)
                Expect.equal (coresMinOf decoded) (Some (box expected)) (sprintf "%s: the int is the float that it converts to." text)
                let written = Encode.encodeToolDescription decoded
                Expect.isFalse (written.Contains "0x" || written.Contains "0o") (sprintf "%s: the float is written as a float.\n%s" text written)
                let reread = Decode.decodeCommandLineTool written
                Expect.equal reread decoded (sprintf "%s: the written text should decode to the same tool." text)
                Expect.equal (Encode.encodeToolDescription reread) written (sprintf "%s: writing again should give the same text." text)
        testCase "an int beyond 64 bits is the float in every resource field, in a hint and in the map form" <| fun _ ->
            for field in [ "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" ] do
                Decode.decodeCommandLineTool (resource field bigDecimal) |> ignore
            for lines in [ [ "hints:"; "  - class: ResourceRequirement"; "    ramMin: " + bigDecimal ]; [ "hints:"; "  ResourceRequirement:"; "    ramMin: " + bigDecimal ]; [ "requirements:"; "  ResourceRequirement:"; "    ramMin: " + bigDecimal ] ] do
                let decoded = Decode.decodeCommandLineTool (tool (lines @ [ "inputs: []"; "outputs: []" ]))
                Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded "The written text should decode to the same tool."
        testCase "an int beyond the largest float is a decode error at the resource field, and the largest long stays a long" <| fun _ ->
            let beyondFloat = "1" + String.replicate 400 "0"
            failsAt "a decimal beyond the float range" (fun () -> Decode.decodeCommandLineTool (resource "coresMin" beyondFloat) |> ignore) "requirements[0].coresMin" "is beyond the largest float"
            failsAt "a hexadecimal beyond the float range" (fun () -> Decode.decodeCommandLineTool (resource "ramMax" ("0x1" + String.replicate 300 "0")) |> ignore) "requirements[0].ramMax" "is beyond the largest float"
            Expect.equal (coresMinOf (Decode.decodeCommandLineTool (resource "coresMin" "0x7FFFFFFFFFFFFFFF"))) (Some (box System.Int64.MaxValue)) "The largest long is a long."
            Expect.equal (coresMinOf (Decode.decodeCommandLineTool (resource "coresMin" "9223372036854775807"))) (Some (box System.Int64.MaxValue)) "The largest long in decimal is a long."
        testCase "the other number positions refuse an int beyond 64 bits and a float" <| fun _ ->
            for text in [ bigDecimal; "0x8000000000000000"; "1.5" ] do
                failsAt ("timelimit " + text) (fun () -> Decode.decodeCommandLineTool (tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + text; "inputs: []"; "outputs: []" ]) |> ignore) "requirements[0].timelimit" "Expected an int or an expression, found"
                failsAt ("position " + text) (fun () -> Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding: {position: " + text + "}"; "outputs: []" ]) |> ignore) "inputs.x.inputBinding.position" "Expected an int or an expression"
                failsAt ("success code " + text) (fun () -> Decode.decodeCommandLineTool (tool [ "successCodes: [" + text + "]"; "inputs: []"; "outputs: []" ]) |> ignore) "successCodes[0]" "Expected an int"
                failsAt ("file size " + text) (fun () -> Decode.decodeCommandLineTool (tool [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - class: File"; "        location: f.txt"; "        size: " + text; "inputs: []"; "outputs: []" ]) |> ignore) "requirements[0].listing[0].size" "Expected an int64"
            failsAt "an int beyond 32 bits" (fun () -> Decode.decodeCommandLineTool (tool [ "successCodes: [2147483648]"; "inputs: []"; "outputs: []" ]) |> ignore) "successCodes[0]" "Expected an int"
        testCase "the message for an int beyond its range names the range" <| fun _ ->
            let position (text: string) = tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding: {position: " + text + "}"; "outputs: []" ]
            let successCode (text: string) = tool [ "successCodes: [" + text + "]"; "inputs: []"; "outputs: []" ]
            let timeLimit (text: string) = tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + text; "inputs: []"; "outputs: []" ]
            let fileSize (text: string) = tool [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - class: File"; "        location: f.txt"; "        size: " + text; "inputs: []"; "outputs: []" ]
            for text in [ "2147483648"; "-2147483649"; "9223372036854775807" ] do
                failsAt ("position " + text) (fun () -> Decode.decodeCommandLineTool (position text) |> ignore) "inputs.x.inputBinding.position" ("Expected an int, found the plain scalar " + text + ", which is an int beyond the 32-bit range.")
                failsAt ("success code " + text) (fun () -> Decode.decodeCommandLineTool (successCode text) |> ignore) "successCodes[0]" ("Expected an int, found the plain scalar " + text + ", which is an int beyond the 32-bit range.")
            failsAt "a success code beyond 64 bits" (fun () -> Decode.decodeCommandLineTool (successCode bigDecimal) |> ignore) "successCodes[0]" ("Expected an int, found the plain scalar " + bigDecimal + ", which is an int beyond the 32-bit range.")
            failsAt "a position beyond 64 bits" (fun () -> Decode.decodeCommandLineTool (position bigDecimal) |> ignore) "inputs.x.inputBinding.position" ("found the plain scalar " + bigDecimal + ", which the core schema reads as an int beyond the 64-bit range.")
            failsAt "a time limit beyond 64 bits" (fun () -> Decode.decodeCommandLineTool (timeLimit bigDecimal) |> ignore) "requirements[0].timelimit" ("found the plain scalar " + bigDecimal + ", which the core schema reads as an int beyond the 64-bit range.")
            failsAt "a size beyond 64 bits" (fun () -> Decode.decodeCommandLineTool (fileSize bigDecimal) |> ignore) "requirements[0].listing[0].size" ("Expected an int64, found the plain scalar " + bigDecimal + ", which is an int beyond the 64-bit range.")
            // The ends of the ranges stay valid.
            let decoded = Decode.decodeCommandLineTool (tool [ "successCodes: [2147483647, -2147483648]"; "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: 9223372036854775807"; "inputs:"; "  x:"; "    type: string"; "    inputBinding: {position: 2147483647}"; "outputs: []" ])
            Expect.sequenceEqual (ListEntry.values decoded.SuccessCodes.Value) [ 2147483647; -2147483648 ] "The ends of the 32-bit range are valid."
            Expect.equal (inputBindingOf (ListEntry.values decoded.Inputs.Value).[0]).Position (Some (BindingPosition.Number 2147483647)) "The highest int is a valid position."
        testCase "an int of the core schema is a number in every number position, and a spelling that only YAML 1.1 reads as an int is a string" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool [ "successCodes: [0o17, 0x1F, 017, 5]"; "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: 0o17"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - {class: File, location: f.txt, size: 0x10}"; "inputs:"; "  x:"; "    type: string"; "    inputBinding: {position: 0o11}"; "outputs: []" ])
            Expect.sequenceEqual (ListEntry.values decoded.SuccessCodes.Value) [ 15; 31; 17; 5 ] "The success codes are the numbers."
            Expect.equal (inputBindingOf (ListEntry.values decoded.Inputs.Value).[0]).Position (Some (BindingPosition.Number 9)) "The position is the number."
            match decoded.Requirements.Value.[0] with
            | ToolTimeLimitRequirement (ToolTimeLimitSeconds seconds) -> Expect.equal seconds 15L "The time limit is the number."
            | other -> failwith (sprintf "Expected a ToolTimeLimit, found %A" other)
            match decoded.Requirements.Value.[1] with
            | InitialWorkDirRequirement entries ->
                match entries.[0] with
                | FileEntry file -> Expect.equal file.Size (Some (FileSize.Bytes 16L)) "The size is the number."
                | other -> failwith (sprintf "Expected a file entry, found %A" other)
            | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded "The written text should decode to the same tool."
            for text in [ "0b101"; "1_000"; "-0x10"; "0b_" ] do
                failsAt ("success code " + text) (fun () -> Decode.decodeCommandLineTool (tool [ "successCodes: [" + text + "]"; "inputs: []"; "outputs: []" ]) |> ignore) "successCodes[0]" "Expected an int"
        testCase "a job file refuses an int beyond 64 bits, and the markers = and << are text" <| fun _ ->
            for text in [ bigDecimal; "0x8000000000000000"; "-9223372036854775809" ] do
                match (try DecodeParameters.decodeYAMLParameterFile ("x: " + text + "\n") |> ignore; None with YamlDecodeException error -> Some error) with
                | Some error ->
                    Expect.stringContains error.Message "beyond 64 bits" (sprintf "%s: the error should say what is wrong" text)
                    Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" text)
                | None -> failwith (sprintf "The job file with %s must fail" text)
            let references = DecodeParameters.decodeYAMLParameterFile "z: =\nw: <<\nv: 9223372036854775807\nu: -9223372036854775808\n"
            Expect.equal references.[0].Value (Some (CWLParameterValue.String "=")) "The marker is a text."
            Expect.equal references.[1].Value (Some (CWLParameterValue.String "<<")) "The marker is a text."
            Expect.equal references.[2].Value (Some (CWLParameterValue.Int System.Int64.MaxValue)) "The largest long is a long."
            Expect.equal references.[3].Value (Some (CWLParameterValue.Int System.Int64.MinValue)) "The lowest long is a long."
            let written = Encode.encodeYAMLParameterFile references
            Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references "The written file should decode to the same references."
        testCase "an extension field and a default keep an int beyond 64 bits as it was written" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (tool [ "$namespaces: {ex: http://example.org/}"; "ex:big: " + bigDecimal; "inputs:"; "  x:"; "    type: Any"; "    default: 0x8000000000000000"; "  y:"; "    type: Any"; "    default: -" + bigDecimal; "outputs: []" ])
            let written = Encode.encodeToolDescription decoded
            Expect.stringContains written ("ex:big: " + bigDecimal) "The extension field is written as it was, not as a float."
            Expect.stringContains written "default: 0x8000000000000000" "The default is written as it was."
            Expect.stringContains written ("default: -" + bigDecimal) "The decimal default is written as it was."
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
    ]

let testDirectivesInFileAndDirectoryStrings =
    let toolWith (listing: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:" ] @ (listing |> List.map (fun line -> "      " + line)) @ [ "inputs: []"; "outputs: []"; "" ])
    let firstEntry (tool: CWLToolDescription) =
        match tool.Requirements.Value.[0] with
        | InitialWorkDirRequirement entries -> entries.[0]
        | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
    let fileOf (tool: CWLToolDescription) =
        match firstEntry tool with
        | FileEntry file -> file
        | other -> failwith (sprintf "Expected a File, found %A" other)
    let directoryOf (tool: CWLToolDescription) =
        match firstEntry tool with
        | DirectoryEntry directory -> directory
        | other -> failwith (sprintf "Expected a Directory, found %A" other)
    let filePosition (field: string) (read: FileInstance -> SchemaSaladString option) =
        let location = if field = "location" then [] else [ "  location: f.txt" ]
        "the " + field + " of a File", "requirements[0].listing[0]." + field, (fun (value: string) -> toolWith ([ "- class: File" ] @ location @ [ "  " + field + ": " + value ])), (fun (tool: CWLToolDescription) -> read (fileOf tool))
    let directoryPosition (field: string) (read: DirectoryInstance -> SchemaSaladString option) =
        let location = if field = "location" then [] else [ "  location: d" ]
        "the " + field + " of a Directory", "requirements[0].listing[0]." + field, (fun (value: string) -> toolWith ([ "- class: Directory" ] @ location @ [ "  " + field + ": " + value ])), (fun (tool: CWLToolDescription) -> read (directoryOf tool))
    // The name, the path of the field in the errors, the document with the given text as the value of the field, and the way to read the field.
    let positions =
        [ filePosition "location" (fun file -> file.Location)
          filePosition "path" (fun file -> file.Path)
          filePosition "basename" (fun file -> file.Basename)
          filePosition "dirname" (fun file -> file.Dirname)
          filePosition "nameroot" (fun file -> file.Nameroot)
          filePosition "nameext" (fun file -> file.Nameext)
          filePosition "checksum" (fun file -> file.Checksum)
          filePosition "contents" (fun file -> file.Contents)
          filePosition "format" (fun file -> file.Format)
          directoryPosition "location" (fun directory -> directory.Location)
          directoryPosition "path" (fun directory -> directory.Path)
          directoryPosition "basename" (fun directory -> directory.Basename) ]
    let mixed (text: string) = { Node = YAMLicious.Reader.readRoot text }
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Directives in the strings of Files and Directories" [
        testCase "an import, an include or a mixin stands for the string of each string field of a File and a Directory, is written back as it was and reads back equal" <| fun _ ->
            for (name, _, document, read) in positions do
                for (text, expected) in
                    [ "{$import: s.yml}", SchemaSaladString.Import "s.yml"
                      "{$include: s.txt}", SchemaSaladString.Include "s.txt"
                      "{$mixin: s.yml}", SchemaSaladString.Mixin "s.yml"
                      "{$mixin: s.yml, extra: 1}", SchemaSaladString.Mixed (mixed "{$mixin: s.yml, extra: 1}") ] do
                    let decoded = Decode.decodeCommandLineTool (document text)
                    Expect.equal (read decoded) (Some expected) (sprintf "%s: %s should decode to a directive of its own." name text)
                    let written = Encode.encodeToolDescription decoded
                    Expect.stringContains written text (sprintf "%s: %s should be written back as it was.\n%s" name text written)
                    let reread = Decode.decodeCommandLineTool written
                    Expect.equal reread decoded (sprintf "%s: the written text of %s should decode to the same tool." name text)
                    Expect.equal (Encode.encodeToolDescription reread) written (sprintf "%s: writing again should give the same text." name)
        testCase "a plain text is still the literal, and a number, a bool and a mapping that is no directive are decode errors at the field" <| fun _ ->
            for (name, path, document, read) in positions do
                Expect.equal (read (Decode.decodeCommandLineTool (document "text"))) (Some (SchemaSaladString.Literal "text")) (sprintf "%s: a text is the literal." name)
                for (text, message) in
                    [ "42", "Expected a string, found the plain scalar"
                      "true", "Expected a string, found the plain scalar"
                      "{a: b}", "Expected a string or a directive, found a mapping."
                      "{$import: s.yml, extra: 1}", "A directive has exactly one field" ] do
                    failsAt (name + " " + text) (fun () -> Decode.decodeCommandLineTool (document text) |> ignore) path message
        testCase "the location and the path of a File and a Directory in an array of the listing take a directive too" <| fun _ ->
            for cls in [ "File"; "Directory" ] do
                for field in [ "location"; "path" ] do
                    let document = toolWith [ "- - class: " + cls; "    " + field + ": {$include: abs.txt}" ]
                    let tool = Decode.decodeCommandLineTool document
                    Expect.stringContains (Encode.encodeToolDescription tool) (field + ": {$include: abs.txt}") (sprintf "The %s of a %s should be written back as it was." field cls)
                    Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription tool)) tool (sprintf "The written %s of a %s should decode to the same tool." field cls)
        testCase "the path or the location of a value is the text that the model holds, and a directive counts as absent" <| fun _ ->
            let literal (text: string) = SchemaSaladString.Literal text
            let directive = SchemaSaladString.Import "p.yml"
            Expect.equal (FileInstance.pathOrLocation (FileInstance(path = literal "p", location = literal "l"))) "p" "The path comes first."
            Expect.equal (FileInstance.pathOrLocation (FileInstance(location = literal "l"))) "l" "The location stands in for a missing path."
            Expect.equal (FileInstance.pathOrLocation (FileInstance(path = directive, location = literal "l"))) "l" "A path that is a directive has no text, so the location counts."
            Expect.equal (FileInstance.pathOrLocation (FileInstance(path = directive, location = directive))) "" "Two directives hold no text."
            Expect.equal (FileInstance.pathOrLocation (FileInstance())) "" "A File without a path and a location holds no text."
            Expect.equal (DirectoryInstance.pathOrLocation (DirectoryInstance(path = directive, location = literal "d"))) "d" "A Directory reads the same way."
            Expect.equal (DirectoryInstance.pathOrLocation (DirectoryInstance(location = directive))) "" "A directive holds no text in a Directory either."
        testCase "a directive is no literal of the same text: it differs in equality and in the hash" <| fun _ ->
            let file (basename: SchemaSaladString) = FileInstance(location = SchemaSaladString.Literal "f.txt", basename = basename)
            Expect.notEqual (file (SchemaSaladString.Import "a")) (file (SchemaSaladString.Literal "a")) "An import of a is no literal a."
            Expect.notEqual (file (SchemaSaladString.Import "a")) (file (SchemaSaladString.Include "a")) "An import and an include differ."
            Expect.notEqual ((file (SchemaSaladString.Import "a")).GetHashCode()) ((file (SchemaSaladString.Literal "a")).GetHashCode()) "The hash tells an import from a literal."
            Expect.equal (DirectoryInstance(basename = SchemaSaladString.Import "a")) (DirectoryInstance(basename = SchemaSaladString.Import "a")) "Two imports of the same file are equal."
            Expect.notEqual (DirectoryInstance(basename = SchemaSaladString.Import "a")) (DirectoryInstance(basename = SchemaSaladString.Literal "a")) "An import of a is no literal a in a Directory."
        testCase "the File and Directory values of a job file take the same directives" <| fun _ ->
            let references = DecodeParameters.decodeYAMLParameterFile "f:\n  class: File\n  path: {$include: a.txt}\n  format: {$import: f.yml}\n  checksum: {$include: c.txt}\nd:\n  class: Directory\n  location: {$import: b.yml}\n  basename: {$import: n.yml}\n"
            match references.[0].Value with
            | Some (CWLParameterValue.File file) ->
                Expect.equal file.Path (Some (SchemaSaladString.Include "a.txt")) "The path is the include."
                Expect.equal file.Format (Some (SchemaSaladString.Import "f.yml")) "The format is the import."
                Expect.equal file.Checksum (Some (SchemaSaladString.Include "c.txt")) "The checksum is the include."
            | other -> failwith (sprintf "Expected a File, found %A" other)
            match references.[1].Value with
            | Some (CWLParameterValue.Directory directory) ->
                Expect.equal directory.Location (Some (SchemaSaladString.Import "b.yml")) "The location is the import."
                Expect.equal directory.Basename (Some (SchemaSaladString.Import "n.yml")) "The basename is the import."
            | other -> failwith (sprintf "Expected a Directory, found %A" other)
            let written = Encode.encodeYAMLParameterFile references
            Expect.stringContains written "format: {$import: f.yml}" "The import is written as a flow mapping."
            Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references "The written file should decode to the same references."
    ]

let testDirectivesAsWholeBindings =
    let toolOf (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let toolPosition (name: string) (path: string) (lines: string -> string list) (read: CWLToolDescription -> 'T option) : FieldPosition<'T> =
        { Name = name
          Path = path
          Document = fun value -> toolOf (lines value)
          Decode = fun text -> box (Decode.decodeCommandLineTool text)
          Encode = fun decoded -> Encode.encodeToolDescription (unbox<CWLToolDescription> decoded)
          Read = fun decoded -> read (unbox<CWLToolDescription> decoded) }
    let workflowPosition (name: string) (path: string) (lines: string -> string list) (read: CWLWorkflowDescription -> 'T option) : FieldPosition<'T> =
        { Name = name
          Path = path
          Document = fun value -> String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow" ] @ lines value @ [ "" ])
          Decode = fun text -> box (Decode.decodeWorkflow text)
          Encode = fun decoded -> Encode.encodeWorkflowDescription (unbox<CWLWorkflowDescription> decoded)
          Read = fun decoded -> read (unbox<CWLWorkflowDescription> decoded) }
    let expressionToolPosition (name: string) (path: string) (lines: string -> string list) (read: CWLExpressionToolDescription -> 'T option) : FieldPosition<'T> =
        { Name = name
          Path = path
          Document = fun value -> String.concat "\n" ([ "cwlVersion: v1.2"; "class: ExpressionTool"; "expression: $(1)" ] @ lines value @ [ "" ])
          Decode = fun text -> box (Decode.decodeExpressionTool text)
          Encode = fun decoded -> Encode.encodeExpressionToolDescription (unbox<CWLExpressionToolDescription> decoded)
          Read = fun decoded -> read (unbox<CWLExpressionToolDescription> decoded) }
    let firstInput (inputs: ResizeArray<ListEntry<CWLInput>>) = (ListEntry.values inputs).[0]
    let firstOutput (outputs: ResizeArray<ListEntry<CWLOutput>>) = (ListEntry.values outputs).[0]
    let inputLines (value: string) = [ "inputs:"; "  x:"; "    type: File"; "    inputBinding: " + value; "outputs: {}" ]
    let listInputLines (value: string) = [ "inputs:"; "  - id: x"; "    type: File"; "    inputBinding: " + value; "outputs: {}" ]
    let outputLines (value: string) = [ "inputs: {}"; "outputs:"; "  o:"; "    type: File"; "    outputBinding: " + value ]
    let listOutputLines (value: string) = [ "inputs: {}"; "outputs:"; "  - id: o"; "    type: File"; "    outputBinding: " + value ]
    let inputPositions : FieldPosition<ListEntry<InputBinding>> list =
        [ toolPosition "the inputBinding of a tool input" "inputs.x.inputBinding" inputLines (fun tool -> (firstInput tool.Inputs.Value).InputBinding)
          toolPosition "the inputBinding of a tool input in list form" "inputs.x.inputBinding" listInputLines (fun tool -> (firstInput tool.Inputs.Value).InputBinding)
          workflowPosition "the inputBinding of a workflow input" "inputs.x.inputBinding" (fun value -> inputLines value @ [ "steps: {}" ]) (fun workflow -> (firstInput workflow.Inputs).InputBinding)
          expressionToolPosition "the inputBinding of an expression tool input" "inputs.x.inputBinding" inputLines (fun tool -> (firstInput tool.Inputs.Value).InputBinding) ]
    let outputPositions : FieldPosition<ListEntry<OutputBinding>> list =
        [ toolPosition "the outputBinding of a tool output" "outputs.o.outputBinding" outputLines (fun tool -> (firstOutput tool.Outputs).OutputBinding)
          toolPosition "the outputBinding of a tool output in list form" "outputs.o.outputBinding" listOutputLines (fun tool -> (firstOutput tool.Outputs).OutputBinding) ]
    let mixed (text: string) = { Node = YAMLicious.Reader.readRoot text }
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let expectDirectives (position: FieldPosition<ListEntry<'T>>) =
        for (text, expected) in
            [ "{$import: b.yml}", ListEntry.Imported "b.yml"
              "{$mixin: b.yml}", ListEntry.Mixed (mixed "{$mixin: b.yml}")
              "{$mixin: b.yml, loadContents: true}", ListEntry.Mixed (mixed "{$mixin: b.yml, loadContents: true}") ] do
            let decoded = position.Decode (position.Document text)
            Expect.equal (position.Read decoded) (Some expected) (sprintf "%s: %s should decode to a directive of its own." position.Name text)
            let written = position.Encode decoded
            Expect.stringContains written text (sprintf "%s: %s should be written back as it was.\n%s" position.Name text written)
            let reread = position.Decode written
            Expect.equal reread decoded (sprintf "%s: the written text of %s should decode to the same object." position.Name text)
            Expect.equal (position.Encode reread) written (sprintf "%s: writing again should give the same text." position.Name)
    let expectFailures (position: FieldPosition<ListEntry<'T>>) (failures: (string * string * string) list) =
        for (text, path, message) in failures do
            failsAt (position.Name + " " + text) (fun () -> position.Decode (position.Document text) |> ignore) path message
    testList "Directives as a whole binding" [
        testCase "an import or a mixin stands for the whole inputBinding of an input, is written back as it was and reads back equal" <| fun _ ->
            for position in inputPositions do
                expectDirectives position
        testCase "an import or a mixin stands for the whole outputBinding of an output, is written back as it was and reads back equal" <| fun _ ->
            for position in outputPositions do
                expectDirectives position
        testCase "a binding of its own is still the binding, and a directive is no binding without fields" <| fun _ ->
            let inputBinding (text: string) = (List.head inputPositions).Read ((List.head inputPositions).Decode ((List.head inputPositions).Document text))
            let outputBinding (text: string) = (List.head outputPositions).Read ((List.head outputPositions).Decode ((List.head outputPositions).Document text))
            Expect.equal (inputBinding "{prefix: -x}") (Some (ListEntry.Value (InputBinding.create(prefix = SchemaSaladString.Literal "-x")))) "A binding with a field is the value."
            Expect.equal (inputBinding "{}") (Some (ListEntry.Value (InputBinding.create()))) "A binding without fields is the value too."
            Expect.equal (outputBinding "{glob: o.txt}") (Some (ListEntry.Value (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "o.txt"))))) "An output binding with a field is the value."
            Expect.notEqual (inputBinding "{$import: b.yml}") (inputBinding "{}") "An import differs from a binding without fields."
            Expect.notEqual (outputBinding "{$import: b.yml}") (outputBinding "{}") "An import differs from an output binding without fields."
            Expect.notEqual (inputBinding "{$import: b.yml}") (inputBinding "{$import: c.yml}") "Two imports differ by path."
            Expect.notEqual (inputBinding "{$import: b.yml}") (inputBinding "{$mixin: b.yml}") "An import and a mixin differ."
            Expect.isNone ((List.head inputPositions).Read ((List.head inputPositions).Decode (toolOf [ "inputs:"; "  x: File"; "outputs: {}" ]))) "A port without a binding has none."
            Expect.equal (ListEntry.tryValue (ListEntry.Value (InputBinding.create()))) (Some (InputBinding.create())) "A binding gives its value."
            Expect.isNone (ListEntry.tryValue (ListEntry.Imported "b.yml" : ListEntry<InputBinding>)) "An import gives no binding."
            Expect.isNone (ListEntry.tryValue (ListEntry.Mixed (mixed "{$mixin: b.yml}") : ListEntry<OutputBinding>)) "A mixin gives no binding."
        testCase "an input and an output differ in equality and in the hash by the directive of their binding" <| fun _ ->
            let input (binding: ListEntry<InputBinding> option) = CWLInput("x", CWLType.String, ?inputBinding = binding)
            let output (binding: ListEntry<OutputBinding> option) = CWLOutput("o", CWLType.String, ?outputBinding = binding)
            let inputs = [ input None; input (Some (ListEntry.Value (InputBinding.create()))); input (Some (ListEntry.Imported "b.yml")); input (Some (ListEntry.Imported "c.yml")); input (Some (ListEntry.Mixed (mixed "{$mixin: b.yml}"))) ]
            let outputs = [ output None; output (Some (ListEntry.Value (OutputBinding.create()))); output (Some (ListEntry.Imported "b.yml")); output (Some (ListEntry.Imported "c.yml")); output (Some (ListEntry.Mixed (mixed "{$mixin: b.yml}"))) ]
            // An object without a binding and an object with an empty binding may hash alike on a target, because the empty binding
            // has no field to hash. The test compares their hashes only when a directive is among the two.
            let numberedInputs = List.indexed inputs
            for (index, left) in numberedInputs do
                for (other, right) in numberedInputs do
                    if index <> other then
                        Expect.notEqual left right (sprintf "The inputs %d and %d differ." index other)
                        if index >= 2 || other >= 2 then
                            Expect.notEqual (left.GetHashCode()) (right.GetHashCode()) (sprintf "The inputs %d and %d hash differently." index other)
            let numberedOutputs = List.indexed outputs
            for (index, left) in numberedOutputs do
                for (other, right) in numberedOutputs do
                    if index <> other then
                        Expect.notEqual left right (sprintf "The outputs %d and %d differ." index other)
                        if index >= 2 || other >= 2 then
                            Expect.notEqual (left.GetHashCode()) (right.GetHashCode()) (sprintf "The outputs %d and %d hash differently." index other)
        testCase "an include, a field beside an import, a field of the wrong type beside a mixin and a text are decode errors at the binding" <| fun _ ->
            for position in inputPositions do
                expectFailures position [ "{$include: b.txt}", position.Path, "An $include yields the text of a file, but the inputBinding field takes an object. Write an $import for a file that holds the object."
                                          "{$import: b.yml, loadContents: true}", position.Path, "A directive has exactly one field"
                                          "{$mixin: b.yml, loadContents: abc}", position.Path + ".loadContents", "Expected a bool"
                                          "42", position.Path, "Expected" ]
            for position in outputPositions do
                expectFailures position [ "{$include: b.txt}", position.Path, "An $include yields the text of a file, but the outputBinding field takes an object. Write an $import for a file that holds the object."
                                          "{$import: b.yml, glob: o.txt}", position.Path, "A directive has exactly one field"
                                          "{$mixin: b.yml, loadContents: abc}", position.Path + ".loadContents", "Expected a bool"
                                          "42", position.Path, "Expected" ]
        testCase "a directive binding keeps the rules of the process and the type: stdin and stdout take none, and a workflow, an expression tool and an operation have no outputBinding" <| fun _ ->
            failsAt "stdin" (fun () -> Decode.decodeCommandLineTool (toolOf [ "inputs:"; "  x:"; "    type: stdin"; "    inputBinding: {$import: b.yml}"; "outputs: {}" ]) |> ignore) "inputs.x" "The type stdin is valid only for an input without an inputBinding."
            failsAt "stdout" (fun () -> Decode.decodeCommandLineTool (toolOf [ "inputs: {}"; "outputs:"; "  o:"; "    type: stdout"; "    outputBinding: {$import: b.yml}" ]) |> ignore) "outputs.o" "The type stdout is valid only for an output without an outputBinding."
            let workflow = "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs:\n  o:\n    type: File\n    outputSource: s/o\n    outputBinding: {$import: b.yml}\nsteps: {}\n"
            failsAt "a workflow output" (fun () -> Decode.decodeWorkflow workflow |> ignore) "outputs.o.outputBinding" "An output of a workflow has no outputBinding."
            let operation = "cwlVersion: v1.2\nclass: Operation\ninputs:\n  x:\n    type: File\n    inputBinding: {$mixin: b.yml}\noutputs: {}\n"
            failsAt "an operation input" (fun () -> Decode.decodeOperation operation |> ignore) "inputs.x.inputBinding" "An input of an operation has no inputBinding."
    ]

let testDirectivesAsClassOrType =
    let toolDocument (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let workflowDocument (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow" ] @ lines @ [ "" ])
    let mixed (text: string) = { Node = YAMLicious.Reader.readRoot text }
    let forms = [ "{$import: s.yml}"; "{$include: s.txt}"; "{$mixin: s.yml}"; "{$mixin: s.yml, extra: 1}" ]
    let inputsOf (tool: CWLToolDescription) = ListEntry.values tool.Inputs.Value
    let outputsOf (tool: CWLToolDescription) = ListEntry.values tool.Outputs
    let schemaDefsOf (tool: CWLToolDescription) =
        match tool.Requirements.Value.[0] with
        | SchemaDefRequirement entries -> entries
        | other -> failwith (sprintf "Expected a SchemaDefRequirement, found %A" other)
    let listingOf (tool: CWLToolDescription) =
        match tool.Requirements.Value.[0] with
        | InitialWorkDirRequirement entries -> entries
        | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
    let stepRunOf (workflow: CWLWorkflowDescription) = (ListEntry.values workflow.Steps).[0].Run
    // The name, the entry that holds the directive with {0} in its place, the document around the entry, the way to read the
    // entry from the decoded document and the value that the entry becomes. The value is boxed, because the positions differ.
    let toolPositions : (string * string * (string -> string) * (string -> obj) * (RawYaml -> obj)) list =
        let tool (text: string) = Decode.decodeCommandLineTool text
        [ "the class of a requirement", "{class: {0}}", (fun entry -> toolDocument [ "requirements:"; "  - " + entry; "inputs: []"; "outputs: []" ]), (fun text -> box (tool text).Requirements.Value.[0]), (fun raw -> box (RawRequirement raw))
          "the class of a hint", "{class: {0}}", (fun entry -> toolDocument [ "hints:"; "  - " + entry; "inputs: []"; "outputs: []" ]), (fun text -> box (tool text).Hints.Value.[0]), (fun raw -> box (RawHint raw))
          "the type of an entry of the SchemaDefRequirement", "{type: {0}, name: R, fields: []}", (fun entry -> toolDocument [ "requirements:"; "  - class: SchemaDefRequirement"; "    types:"; "      - " + entry; "inputs: []"; "outputs: []" ]), (fun text -> box (schemaDefsOf (tool text)).[0]), (fun raw -> box (SchemaDefEntry.Raw raw))
          "the type of the schema of an input", "{type: {0}, symbols: [a]}", (fun entry -> toolDocument [ "inputs: {x: {type: " + entry + "}}"; "outputs: []" ]), (fun text -> box (inputsOf (tool text)).[0].Type_.Value), (fun raw -> box (RawType raw))
          "the type of the schema of an output", "{type: {0}, fields: {f: string}}", (fun entry -> toolDocument [ "inputs: []"; "outputs: {o: {type: " + entry + ", outputBinding: {glob: o}}}" ]), (fun text -> box (outputsOf (tool text)).[0].Type_.Value), (fun raw -> box (RawType raw))
          "the type of the schema in the items of an array", "{type: {0}, symbols: [a]}", (fun entry -> toolDocument [ "inputs: {x: {type: {type: array, items: " + entry + "}}}"; "outputs: []" ]), (fun text -> match (inputsOf (tool text)).[0].Type_.Value with Array array -> box array.Items | other -> failwith (sprintf "Expected an array, found %A" other)), (fun raw -> box (RawType raw))
          "the type of the schema in a union", "{type: {0}, symbols: [a]}", (fun entry -> toolDocument [ "inputs: {x: {type: [\"null\", " + entry + "]}}"; "outputs: []" ]), (fun text -> match (inputsOf (tool text)).[0].Type_.Value with Union members -> box members.[1] | other -> failwith (sprintf "Expected a union, found %A" other)), (fun raw -> box (RawType raw))
          "the type of the schema of a record field", "{type: {0}, symbols: [a]}", (fun entry -> toolDocument [ "inputs: {x: {type: {type: record, fields: {f: {type: " + entry + "}}}}}"; "outputs: []" ]), (fun text -> match (inputsOf (tool text)).[0].Type_.Value with Record record -> box (InputRecordSchema.getFieldValuesOrEmpty record).[0].Type | other -> failwith (sprintf "Expected a record, found %A" other)), (fun raw -> box (RawType raw))
          "the class of a File in the listing", "{class: {0}, location: f.txt}", (fun entry -> toolDocument [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - " + entry; "inputs: []"; "outputs: []" ]), (fun text -> box (listingOf (tool text)).[0]), (fun raw -> box (RawEntry raw))
          "an array of the listing that holds a File with a class that is a directive", "[{class: File, location: a.txt}, {class: {0}, location: b.txt}]", (fun entry -> toolDocument [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - " + entry; "inputs: []"; "outputs: []" ]), (fun text -> box (listingOf (tool text)).[0]), (fun raw -> match raw.Node.Kind with YamlNodeKind.Sequence collection -> box (FilesEntry (ResizeArray [ FileItem (FileInstance(location = SchemaSaladString.Literal "a.txt")); RawItem { Node = collection.Items.[1].Node } ])) | other -> failwith (sprintf "Expected a list, found %A" other)) ]
    let stepPosition : string * string * (string -> string) * (string -> obj) * (RawYaml -> obj) =
        "the class of the process that a step runs", "{class: {0}, baseCommand: echo, inputs: [], outputs: []}", (fun entry -> workflowDocument [ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run: " + entry; "    in: []"; "    out: []" ]), (fun text -> box (stepRunOf (Decode.decodeWorkflow text))), (fun raw -> box (RunRaw raw))
    let allPositions = toolPositions @ [ stepPosition ]
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Directives as the class or the type of an object" [
        testCase "an object whose class or type is a directive stays as written in each position, is written back as it was and reads back equal" <| fun _ ->
            for (name, entryTemplate, document, read, make) in allPositions do
                for form in forms do
                    let entry = entryTemplate.Replace("{0}", form)
                    let text = document entry
                    Expect.equal (read text) (make (mixed entry)) (sprintf "%s: %s should stay as written." name form)
                    let written =
                        if name = "the class of the process that a step runs" then
                            Encode.encodeWorkflowDescription (Decode.decodeWorkflow text)
                        else
                            Encode.encodeToolDescription (Decode.decodeCommandLineTool text)
                    Expect.stringContains written form (sprintf "%s: %s should be written back as it was.\n%s" name form written)
                    Expect.equal (read written) (read text) (sprintf "%s: the written text of %s should decode to the same value." name form)
        testCase "an object with another entry or another type is another value" <| fun _ ->
            for (name, entryTemplate, document, read, _) in allPositions do
                let withForm (form: string) = read (document (entryTemplate.Replace("{0}", form)))
                Expect.notEqual (withForm "{$import: s.yml}") (withForm "{$import: t.yml}") (sprintf "%s: two paths differ." name)
                Expect.notEqual (withForm "{$import: s.yml}") (withForm "{$include: s.yml}") (sprintf "%s: an import and an include differ." name)
        testCase "a class or a type that is a plain name is still read as the typed object" <| fun _ ->
            let tool = Decode.decodeCommandLineTool (toolDocument [ "requirements:"; "  - class: ShellCommandRequirement"; "hints:"; "  - class: ShellCommandRequirement"; "inputs: {x: {type: {type: enum, symbols: [a]}}}"; "outputs: []" ])
            Expect.equal tool.Requirements.Value.[0] ShellCommandRequirement "The class of a requirement is the requirement."
            Expect.equal tool.Hints.Value.[0] (KnownHint ShellCommandRequirement) "The class of a hint is the known hint."
            Expect.isTrue (match (inputsOf tool).[0].Type_ with Some (Enum _) -> true | _ -> false) "The type of a schema is the enum."
        testCase "the types that a requirement, a hint or an entry defines beside a directive count for the type names of the ports" <| fun _ ->
            let definition = [ "      - name: R"; "        type: record"; "        fields:"; "          - name: f"; "            type: string" ]
            let input = [ "inputs:"; "  x: R"; "outputs: []" ]
            for form in forms do
                for holder in [ "requirements"; "hints" ] do
                    let tool = Decode.decodeCommandLineTool (toolDocument ([ holder + ":"; "  - class: " + form; "    types:" ] @ definition @ input))
                    Expect.equal (inputsOf tool).[0].Type_ (Some (CWLType.Reference "R")) (sprintf "%s %s: the name is the type that the requirement defines." holder form)
                let entry = Decode.decodeCommandLineTool (toolDocument ([ "requirements:"; "  SchemaDefRequirement:"; "    types:"; "      - type: " + form; "        name: R"; "        fields: []" ] @ input))
                Expect.equal (inputsOf entry).[0].Type_ (Some (CWLType.Reference "R")) (sprintf "%s: the name is the type that the entry defines." form)
        testCase "a malformed directive and a mapping that is no directive are decode errors at the class or the type" <| fun _ ->
            failsAt "a requirement" (fun () -> Decode.decodeCommandLineTool (toolDocument [ "requirements:"; "  - class: {$import: 42}"; "inputs: []"; "outputs: []" ]) |> ignore) "requirements[0].class.$import" "Expected the path of the $import directive to be a string"
            failsAt "a hint" (fun () -> Decode.decodeCommandLineTool (toolDocument [ "hints:"; "  - class: {$import: s.yml, extra: 1}"; "inputs: []"; "outputs: []" ]) |> ignore) "hints[0].class" "A directive has exactly one field"
            failsAt "a requirement mapping" (fun () -> Decode.decodeCommandLineTool (toolDocument [ "requirements:"; "  - class: {a: b}"; "inputs: []"; "outputs: []" ]) |> ignore) "requirements[0].class" "Expected a string, found a mapping."
            failsAt "a schema" (fun () -> Decode.decodeCommandLineTool (toolDocument [ "inputs: {x: {type: {type: {$mixin: 42}, symbols: [a]}}}"; "outputs: []" ]) |> ignore) "inputs.x.type.type.$mixin" "Expected the path of the $mixin directive to be a string"
            failsAt "a schema mapping" (fun () -> Decode.decodeCommandLineTool (toolDocument [ "inputs: {x: {type: {type: {a: b}, symbols: [a]}}}"; "outputs: []" ]) |> ignore) "inputs.x.type.type" "Expected record, enum or array in the type field of a schema, found a mapping."
            failsAt "a File" (fun () -> Decode.decodeCommandLineTool (toolDocument [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - {class: {$import: 42}, location: f.txt}"; "inputs: []"; "outputs: []" ]) |> ignore) "requirements[0].listing[0].class.$import" "Expected the path of the $import directive to be a string"
    ]

let testWordsInNumberPositions =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "inputs: []"; "outputs: []"; "" ])
    let resource (field: string) (value: string) =
        tool [ "requirements:"; "  - class: ResourceRequirement"; "    " + field + ": " + value ]
    let timeLimit (value: string) =
        tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + value ]
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let resourceFields = [ "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" ]
    let words = [ "abc", "found the text \"abc\""; "'5'", "found the text \"5\""; "''", "found the text \"\""; "'true'", "found the text \"true\"" ]
    let coresMinOf (tool: CWLToolDescription) : obj option =
        match tool.Requirements.Value.[0] with
        | ResourceRequirement resources -> resources.CoresMin
        | other -> failwith (sprintf "Expected a ResourceRequirement, found %A" other)
    testList "Words in the number positions" [
        testCase "a text without an expression is a decode error in a resource field and a time limit" <| fun _ ->
            for (text, found) in words do
                for field in resourceFields do
                    failsAt (sprintf "%s %s" field text) (fun () -> Decode.decodeCommandLineTool (resource field text) |> ignore) ("requirements[0]." + field) ("Expected a number or an expression, " + found + ". An expression holds $( or ${.")
                failsAt (sprintf "timelimit %s" text) (fun () -> Decode.decodeCommandLineTool (timeLimit text) |> ignore) "requirements[0].timelimit" ("Expected an int or an expression, " + found + ". An expression holds $( or ${.")
            failsAt "hint" (fun () -> Decode.decodeCommandLineTool (tool [ "hints:"; "  - class: ResourceRequirement"; "    ramMin: abc" ]) |> ignore) "hints[0].ramMin" "Expected a number or an expression"
            failsAt "map form" (fun () -> Decode.decodeCommandLineTool (tool [ "requirements:"; "  ResourceRequirement:"; "    ramMin: abc" ]) |> ignore) "requirements.ResourceRequirement.ramMin" "Expected a number or an expression"
            failsAt "time limit hint" (fun () -> Decode.decodeCommandLineTool (tool [ "hints:"; "  ToolTimeLimit:"; "    timelimit: abc" ]) |> ignore) "hints.ToolTimeLimit.timelimit" "Expected an int or an expression"
        testCase "a number, an expression and a directive stay valid in a resource field and a time limit" <| fun _ ->
            Expect.equal (coresMinOf (Decode.decodeCommandLineTool (resource "coresMin" "5"))) (Some (box 5L)) "A number is the long."
            Expect.equal (coresMinOf (Decode.decodeCommandLineTool (resource "coresMin" "$(inputs.n)"))) (Some (box (SchemaSaladString.Literal "$(inputs.n)"))) "A parameter reference is the expression."
            Expect.equal (coresMinOf (Decode.decodeCommandLineTool (resource "coresMin" "'${return 1;}'"))) (Some (box (SchemaSaladString.Literal "${return 1;}"))) "A function body is the expression."
            Expect.equal (coresMinOf (Decode.decodeCommandLineTool (resource "coresMin" "{$import: n.yml}"))) (Some (box (SchemaSaladString.Import "n.yml"))) "A directive stands for the number."
            Expect.equal (Decode.decodeCommandLineTool (timeLimit "'size: $(1)'")).Requirements.Value.[0] (ToolTimeLimitRequirement (ToolTimeLimitExpression (SchemaSaladString.Literal "size: $(1)"))) "A text with an expression is the expression."
        testCase "the position of an inputBinding stays an int or an expression" <| fun _ ->
            for text in [ "abc"; "'5'"; "''" ] do
                failsAt (sprintf "position %s" text) (fun () -> Decode.decodeCommandLineTool (String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "inputs:"; "  x:"; "    type: string"; "    inputBinding: {position: " + text + "}"; "outputs: []"; "" ]) |> ignore) "inputs.x.inputBinding.position" "Expected an int or an expression"
    ]

let testSecondaryFileRequiredField =
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let inputWith (secondaryFiles: string list) =
        tool ([ "inputs:"; "  x:"; "    type: File"; "    secondaryFiles:" ] @ (secondaryFiles |> List.map (fun line -> "      " + line)) @ [ "outputs: []" ])
    let outputWith (secondaryFiles: string list) =
        tool ([ "inputs: []"; "outputs:"; "  o:"; "    type: File"; "    outputBinding: {glob: o}"; "    secondaryFiles:" ] @ (secondaryFiles |> List.map (fun line -> "      " + line)))
    let valid = [ "true"; "false"; "null"; "~"; "'$(inputs.x.size > 0)'"; "'${return true;}'"; "$(inputs.x.size)"; "{$import: r.yml}"; "{$include: r.txt}"; "{$mixin: r.yml}"; "{$mixin: r.yml, extra: 1}" ]
    let invalid = [ "1"; "0"; "1.5"; "[1]"; "[true]"; "[]"; "{a: b}"; "abc"; "'true'"; "''"; "yes" ]
    testList "The required field of a secondary file" [
        testCase "the required field of a secondary file is a bool, an expression or a directive, and null counts as missing" <| fun _ ->
            for value in valid do
                for (place, document, path) in
                    [ "an input", inputWith [ "- pattern: .idx"; "  required: " + value ], "inputs.x.secondaryFiles[0].required"
                      "an input with one object", inputWith [ "pattern: .idx"; "required: " + value ], "inputs.x.secondaryFiles.required"
                      "an output", outputWith [ "- pattern: .idx"; "  required: " + value ], "outputs.o.secondaryFiles[0].required" ] do
                    let decoded = Decode.decodeCommandLineTool document
                    Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)) decoded (sprintf "%s %s: the written text should decode to the same tool." place value)
        testCase "a required field that is no bool and no expression is a decode error at the field" <| fun _ ->
            for value in invalid do
                failsAt (sprintf "input %s" value) (fun () -> Decode.decodeCommandLineTool (inputWith [ "- pattern: .idx"; "  required: " + value ]) |> ignore) "inputs.x.secondaryFiles[0].required" "Expected a bool or an expression, found"
                failsAt (sprintf "one object %s" value) (fun () -> Decode.decodeCommandLineTool (inputWith [ "pattern: .idx"; "required: " + value ]) |> ignore) "inputs.x.secondaryFiles.required" "Expected a bool or an expression, found"
                failsAt (sprintf "output %s" value) (fun () -> Decode.decodeCommandLineTool (outputWith [ "- pattern: .idx"; "  required: " + value ]) |> ignore) "outputs.o.secondaryFiles[0].required" "Expected a bool or an expression, found"
            failsAt "second entry" (fun () -> Decode.decodeCommandLineTool (inputWith [ "- .bai"; "- pattern: .idx"; "  required: [1]" ]) |> ignore) "inputs.x.secondaryFiles[1].required" "Expected a bool or an expression, found"
            failsAt "a mixin beside the field" (fun () -> Decode.decodeCommandLineTool (inputWith [ "- {$mixin: r.yml, required: 1}" ]) |> ignore) "inputs.x.secondaryFiles[0].required" "Expected a bool or an expression, found"
            failsAt "a malformed directive" (fun () -> Decode.decodeCommandLineTool (inputWith [ "- pattern: .idx"; "  required: {$import: 42}" ]) |> ignore) "inputs.x.secondaryFiles[0].required.$import" "Expected the path of the $import directive to be a string"
        testCase "a string and an expression as a secondary file have no required field to check" <| fun _ ->
            Decode.decodeCommandLineTool (inputWith [ "- .idx"; "- '$(self.basename).bai'"; "- pattern: .tbi" ]) |> ignore
            Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: File"; "    secondaryFiles: .idx"; "outputs: []" ]) |> ignore
    ]

let testLowestLong =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "inputs: []"; "outputs: []"; "" ])
    let resource (value: string) =
        tool [ "requirements:"; "  - class: ResourceRequirement"; "    ramMin: " + value ]
    let timeLimit (value: string) =
        tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + value ]
    let fileSize (value: string) =
        tool [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:"; "      - {class: File, location: f.txt, size: " + value + "}" ]
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let ramMinOf (tool: CWLToolDescription) : obj option =
        match tool.Requirements.Value.[0] with
        | ResourceRequirement resources -> resources.RamMin
        | other -> failwith (sprintf "Expected a ResourceRequirement, found %A" other)
    // Only the decimal spelling of -2^63 is an int. The prefixed spellings take no sign in the core schema, so -0x8000000000000000 is a string.
    let lowest = "-9223372036854775808"
    testList "The lowest long" [
        testCase "-2^63 is the lowest long in a resource field, and it reads back" <| fun _ ->
            let decoded = Decode.decodeCommandLineTool (resource lowest)
            Expect.equal (ramMinOf decoded) (Some (box System.Int64.MinValue)) "The number is the lowest long."
            let written = Encode.encodeToolDescription decoded
            Expect.stringContains written ("ramMin: " + lowest) (sprintf "The long is written in decimal.\n%s" written)
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
        testCase "the lowest long is the number of a time limit and the size of a File" <| fun _ ->
            let limit = Decode.decodeCommandLineTool (timeLimit lowest)
            Expect.equal limit.Requirements.Value.[0] (ToolTimeLimitRequirement (ToolTimeLimitSeconds System.Int64.MinValue)) "The time limit is the lowest long."
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription limit)) limit "The written time limit should decode to the same tool."
            let sized = Decode.decodeCommandLineTool (fileSize lowest)
            match sized.Requirements.Value.[0] with
            | InitialWorkDirRequirement entries ->
                match entries.[0] with
                | FileEntry file -> Expect.equal file.Size (Some (FileSize.Bytes System.Int64.MinValue)) "The size is the lowest long."
                | other -> failwith (sprintf "Expected a File, found %A" other)
            | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription sized)) sized "The written size should decode to the same tool."
        testCase "a job file holds the lowest long as an int, and it reads back" <| fun _ ->
            let references = DecodeParameters.decodeYAMLParameterFile ("x: " + lowest + "\n")
            Expect.equal references.[0].Value (Some (CWLParameterValue.Int System.Int64.MinValue)) "The value is the lowest long."
            let written = Encode.encodeYAMLParameterFile references
            Expect.stringContains written ("x: " + lowest) (sprintf "The long is written in decimal.\n%s" written)
            Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references "The written file should decode to the same references."
        testCase "a signed hexadecimal or octal spelling is a string, so a number position refuses it" <| fun _ ->
            for text in [ "-0x8000000000000000"; "-0o1000000000000000000000" ] do
                failsAt ("a resource " + text) (fun () -> Decode.decodeCommandLineTool (resource text) |> ignore) "requirements[0].ramMin" "Expected a number or an expression"
                failsAt ("a size " + text) (fun () -> Decode.decodeCommandLineTool (fileSize text) |> ignore) "requirements[0].listing[0].size" "Expected an int64"
                failsAt ("a time limit " + text) (fun () -> Decode.decodeCommandLineTool (timeLimit text) |> ignore) "requirements[0].timelimit" "Expected an int or an expression"
                Expect.equal (DecodeParameters.decodeYAMLParameterFile ("x: " + text + "\n")).[0].Value (Some (CWLParameterValue.String text)) (sprintf "%s: a job file holds the text." text)
        testCase "a number that is one beyond the lowest or the highest long stays beyond 64 bits" <| fun _ ->
            for text in [ "-9223372036854775809"; "9223372036854775808"; "+9223372036854775808" ] do
                failsAt ("a size " + text) (fun () -> Decode.decodeCommandLineTool (fileSize text) |> ignore) "requirements[0].listing[0].size" "Expected an int64"
                failsAt ("a time limit " + text) (fun () -> Decode.decodeCommandLineTool (timeLimit text) |> ignore) "requirements[0].timelimit" "Expected an int or an expression, found"
            Expect.equal (ramMinOf (Decode.decodeCommandLineTool (resource "+9223372036854775808"))) (Some (box 9223372036854775808.0)) "The highest long plus one is the float in a resource field."
            Expect.equal (ramMinOf (Decode.decodeCommandLineTool (resource "-9223372036854775809"))) (Some (box -9223372036854775808.0)) "The lowest long minus one is the float that it converts to."
            Expect.equal (ramMinOf (Decode.decodeCommandLineTool (resource "9223372036854775807"))) (Some (box System.Int64.MaxValue)) "The highest long stays a long."
            match (try DecodeParameters.decodeYAMLParameterFile "x: +9223372036854775808\n" |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error -> Expect.stringContains error.Message "beyond 64 bits" "A job file refuses the highest long plus one."
            | None -> failwith "The highest long plus one must fail in a job file."
            let references = DecodeParameters.decodeYAMLParameterFile "y: 9223372036854775807\n"
            Expect.equal references.[0].Value (Some (CWLParameterValue.Int System.Int64.MaxValue)) "The highest long is an int in a job file."
    ]

let testProcessWithDirectiveClass =
    let forms = [ "{$import: s.yml}"; "{$include: s.txt}"; "{$mixin: s.yml}"; "{$mixin: s.yml, extra: 1}" ]
    let documentText (classValue: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: " + classValue; "baseCommand: echo"; "inputs: []"; "outputs: []"; "" ]
    let graphText (classValue: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "$graph:"; "  - class: " + classValue; "    id: main"; "    baseCommand: echo"; "    inputs: []"; "    outputs: []"; "  - class: CommandLineTool"; "    id: tool"; "    baseCommand: cat"; "    inputs: []"; "    outputs: []"; "" ]
    let workflowGraphText (runValue: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "$graph:"; "  - class: Workflow"; "    id: main"; "    inputs: []"; "    outputs: []"; "    steps:"; "      s:"; "        run: " + runValue; "        in: []"; "        out: []"; "  - class: {$import: s.yml}"; "    id: t"; "    baseCommand: echo"; "    inputs: []"; "    outputs: []"; "" ]
    let graphOf (text: string) =
        match Decode.decodeCWLDocument text with
        | CWLDocument.Graph graph -> graph
        | other -> failwith (sprintf "Expected a packed document, found %A" other)
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "A process whose class is a directive" [
        testCase "a document of one process stays as written, reads back equal and is written back as it was" <| fun _ ->
            for form in forms do
                let text = documentText form
                let document = Decode.decodeCWLDocument text
                Expect.equal document (CWLDocument.Process (RawProcess { Node = Reader.readRoot text })) (sprintf "%s: the process should stay as written." form)
                Expect.equal (Decode.decodeCWLProcessingUnit text) (RawProcess { Node = Reader.readRoot text }) (sprintf "%s: the process decoder should keep it as written too." form)
                let written = Encode.encodeCWLDocument document
                Expect.equal written text (sprintf "%s: the document should be written back as it was." form)
                Expect.equal (Decode.decodeCWLDocument written) document (sprintf "%s: the written text should decode to the same document." form)
                Expect.equal (Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit text)) text (sprintf "%s: the process should be written back as it was." form)
        testCase "the fields that every process has are checked on a process of unknown class, in a document, a run and a packed document" <| fun _ ->
            let document (lines: string list) = String.concat "\n" ([ "cwlVersion: v1.2"; "class: {$import: c.yml}" ] @ lines @ [ "" ])
            let run (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "class: Workflow"; "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run:"; "      class: {$import: c.yml}" ] @ (lines |> List.map (fun line -> "      " + line)) @ [ "    in: []"; "    out: []"; "" ])
            let graph (lines: string list) =
                String.concat "\n" ([ "cwlVersion: v1.2"; "$graph:"; "  - class: {$import: c.yml}" ] @ (lines |> List.map (fun line -> "    " + line)) @ [ "" ])
            for (name, lines, path, message) in
                [ "a list as the label", [ "label: [1, 2]"; "inputs: []"; "outputs: []" ], "label", "Expected a string or a directive, found a list."
                  "a number as the doc", [ "doc: 5"; "inputs: []"; "outputs: []" ], "doc", "Expected a string, found the plain scalar 5"
                  "a name that is no type in the inputs", [ "inputs: {x: notatype}"; "outputs: []" ], "inputs.x", "Invalid CWL type: notatype"
                  "a name that is no type in the outputs", [ "inputs: []"; "outputs: {x: notatype}" ], "outputs.x", "Invalid CWL type: notatype"
                  "no inputs", [ "outputs: []" ], "", "Expected an inputs field, which CWL requires for a process."
                  "no outputs", [ "inputs: []" ], "", "The mapping has no field outputs."
                  "a number as the requirements", [ "requirements: 5"; "inputs: []"; "outputs: []" ], "requirements", "Expected a list or a mapping of requirements"
                  "a number as the hints", [ "hints: 5"; "inputs: []"; "outputs: []" ], "hints", "Expected a list or a mapping of hints"
                  "a number as the intent", [ "intent: 5"; "inputs: []"; "outputs: []" ], "intent", "Expected a list of strings"
                  "a number as the inputs", [ "inputs: 5"; "outputs: []" ], "inputs", "Expected a list or a mapping of inputs, found"
                  "a number as the outputs", [ "inputs: []"; "outputs: 5" ], "outputs", "Expected a list or a mapping of outputs, found"
                  "an input of the type stdout", [ "inputs: {x: stdout}"; "outputs: []" ], "inputs.x", "The type stdout is valid only as the whole type of an output"
                  "an input of the type stderr", [ "inputs: [{id: x, type: stderr}]"; "outputs: []" ], "inputs.x.type", "The type stderr is valid only as the whole type of an output"
                  "an output of the type stdin", [ "inputs: []"; "outputs: {o: stdin}" ], "outputs.o", "The type stdin is valid only as the whole type of an input"
                  "an output of the type stdin in a list", [ "inputs: []"; "outputs: [{id: o, type: stdin}]" ], "outputs.o.type", "The type stdin is valid only as the whole type of an input" ] do
                failsAt (name + " in a document") (fun () -> Decode.decodeCWLDocument (document lines) |> ignore) path message
                failsAt (name + " in a run") (fun () -> Decode.decodeCWLDocument (run lines) |> ignore) ("steps.s.run" + (if path = "" then "" else "." + path)) message
                failsAt (name + " in a packed document") (fun () -> Decode.decodeCWLDocument (graph lines) |> ignore) ("$graph[0]" + (if path = "" then "" else "." + path)) message
            // The file may hold a command line tool, so an input may have the type stdin and an output stdout or stderr.
            let streams = [ "inputs: {i: stdin}"; "outputs: {o: stdout, e: stderr}" ]
            Expect.equal (Decode.decodeCWLDocument (document streams)) (CWLDocument.Process (RawProcess { Node = Reader.readRoot (document streams) })) "A stream type that a command line tool admits leaves the process as written."
            Decode.decodeCWLDocument (run streams) |> ignore
            Decode.decodeCWLDocument (graph streams) |> ignore
            failsAt "a list as the id in a document" (fun () -> Decode.decodeCWLDocument (document [ "id: [a]"; "inputs: []"; "outputs: []" ]) |> ignore) "id" "Expected a string, found a sequence."
            failsAt "a list as the id in a run" (fun () -> Decode.decodeCWLDocument (run [ "id: [a]"; "inputs: []"; "outputs: []" ]) |> ignore) "steps.s.run.id" "Expected a string, found a sequence."
            let fieldsOfOneClass = [ "inputs: {i: {type: string, inputBinding: {position: 1}}}"; "outputs: {o: {type: string, outputBinding: {glob: o}}}"; "steps: []"; "baseCommand: echo"; "expression: x" ]
            Expect.equal (Decode.decodeCWLDocument (document fieldsOfOneClass)) (CWLDocument.Process (RawProcess { Node = Reader.readRoot (document fieldsOfOneClass) })) "A field that only some classes have is left to the file, and the process stays as written."
        testCase "the comments and blank lines around the directive stay in the written document" <| fun _ ->
            let text = "# leading\ncwlVersion: v1.2\n\n# before the class\nclass: {$import: s.yml} # trailing\n\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            Expect.equal (Encode.encodeCWLDocument (Decode.decodeCWLDocument text)) text "The layout of the document should stay."
        testCase "a process of a packed document stays as written, beside the processes that are read" <| fun _ ->
            for form in forms do
                let text = graphText form
                let graph = graphOf text
                Expect.equal graph.Processes.Count 2 (sprintf "%s: both processes should be in the graph." form)
                match graph.Processes.[0] with
                | RawProcess raw -> Expect.equal (raw.TryScalarField "id") (Some "main") (sprintf "%s: the raw process should keep its id." form)
                | other -> failwith (sprintf "%s: expected a process of unknown class, found %A" form other)
                Expect.isTrue (match graph.Processes.[1] with CommandLineTool tool -> tool.Id = Some "tool" | _ -> false) (sprintf "%s: the second process should be read." form)
                let written = Encode.encodeCWLGraph graph
                Expect.stringContains written ("  - class: " + form + "\n    id: main\n    baseCommand: echo\n") (sprintf "%s: the process should be written back as it was." form)
                Expect.equal (graphOf written) graph (sprintf "%s: the written text should decode to the same document." form)
        testCase "a process of unknown class is found by its id, and it can be the process main" <| fun _ ->
            let document = Decode.decodeCWLDocument (graphText "{$import: s.yml}")
            let graph = match document with CWLDocument.Graph graph -> graph | other -> failwith (sprintf "Expected a packed document, found %A" other)
            Expect.equal (CWLProcessingUnit.tryGetId graph.Processes.[0]) (Some "main") "The id of the raw process is the text of its id field."
            Expect.isTrue (CWLProcessingUnit.isSameObject (CWLGraph.tryFindProcess "main" graph |> Option.get) graph.Processes.[0]) "The process is found by its id."
            Expect.isTrue (CWLProcessingUnit.isSameObject (CWLDocument.tryGetMain document |> Option.get) graph.Processes.[0]) "The process main is the raw process."
            Expect.isTrue (CWLGraph.isMain graph.Processes.[0] graph) "The raw process is the process main of the graph."
            let withoutId = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            Expect.isNone (CWLProcessingUnit.tryGetId withoutId) "A raw process without an id field has no id."
            let withEmptyId = Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: {$import: s.yml}\nid:\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            Expect.isNone (CWLProcessingUnit.tryGetId withEmptyId) "A raw process with an empty id field has no id."
        testCase "a step reaches a process of unknown class by its id and a missing id still fails" <| fun _ ->
            let graph = graphOf (workflowGraphText "'#t'")
            Expect.isSome (CWLGraph.tryFindProcess "t" graph) "The process t is in the graph."
            failsAt "a run of a missing id" (fun () -> Decode.decodeCWLDocument (workflowGraphText "'#nothere'") |> ignore) "$graph[0].steps.s.run" "which is no process of the $graph"
        testCase "the model shows no inputs, outputs, requirements, hints or intent of a process of unknown class" <| fun _ ->
            let processingUnit = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            Expect.equal (CWLProcessingUnit.getInputValues processingUnit).Count 0 "No inputs."
            Expect.equal (CWLProcessingUnit.getOutputValues processingUnit).Count 0 "No outputs."
            Expect.equal (CWLProcessingUnit.getRequirements processingUnit).Count 0 "No requirements."
            Expect.equal (CWLProcessingUnit.getHints processingUnit).Count 0 "No hints."
            Expect.equal (CWLProcessingUnit.getKnownHints processingUnit).Count 0 "No known hints."
            Expect.equal (CWLProcessingUnit.getIntent processingUnit).Count 0 "No intent."
        testCase "a step lists the outputs of a process of unknown class without a check, because the outputs are not read" <| fun _ ->
            let graph = graphOf ((workflowGraphText "'#t'").Replace("out: []", "out: [anything]"))
            Expect.equal graph.Processes.Count 2 "The document decodes."
        testCase "two processes of unknown class are equal when their documents are, and the directive decides" <| fun _ ->
            let read (form: string) = Decode.decodeCWLDocument (documentText form)
            Expect.equal (read "{$import: s.yml}") (read "{$import: s.yml}") "The same directive gives the same document."
            Expect.notEqual (read "{$import: s.yml}") (read "{$import: t.yml}") "Two paths differ."
            Expect.notEqual (read "{$import: s.yml}") (read "{$include: s.yml}") "An import and an include differ."
            let one = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            let two = Decode.decodeCWLProcessingUnit (documentText "{$import: s.yml}")
            Expect.isFalse (CWLProcessingUnit.isSameObject one two) "Two decoded processes are two objects."
            Expect.isTrue (CWLProcessingUnit.isSameObject one one) "A process is the same object as itself."
        testCase "the cwlVersion of the document is still required and is read as written" <| fun _ ->
            let versioned (header: string) = header + "class: {$import: s.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            failsAt "a missing version" (fun () -> Decode.decodeCWLDocument (versioned "") |> ignore) "" "cwlVersion"
            for version in [ "v1.0"; "v1.1"; "v1.2"; "v1.3"; "v1.5" ] do
                Expect.isTrue (match Decode.decodeCWLDocument (versioned ("cwlVersion: " + version + "\n")) with CWLDocument.Process (RawProcess _) -> true | _ -> false) (sprintf "%s: the document should be read." version)
        testCase "the cwlVersion of a process of a packed document changes nothing when its class is a directive" <| fun _ ->
            let withVersion (version: string) = (graphText "{$import: s.yml}").Replace("    id: main\n", "    id: main\n    cwlVersion: " + version + "\n")
            for version in [ "v1.1"; "v1.5" ] do
                Expect.equal (graphOf (withVersion version)).Processes.Count 2 (sprintf "%s: a version of the process is ignored." version)
        testCase "the cwlVersion of an inline run whose class is a directive changes nothing" <| fun _ ->
            let withVersion (version: string) =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run:"; "      cwlVersion: " + version; "      class: {$import: s.yml}"; "      baseCommand: echo"; "      inputs: []"; "      outputs: []"; "    in: []"; "    out: []"; "" ]
            for version in [ "v1.1"; "v1.5" ] do
                let workflow = Decode.decodeWorkflow (withVersion version)
                Expect.isTrue (match (ListEntry.values workflow.Steps).[0].Run with RunRaw _ -> true | _ -> false) (sprintf "%s: the run stays as written." version)
        testCase "a malformed directive as the class of a process is a decode error at the class" <| fun _ ->
            let fails (name: string) (classValue: string) (path: string) (message: string) =
                failsAt name (fun () -> Decode.decodeCWLDocument (documentText classValue) |> ignore) path message
            fails "a path that is no string" "{$import: 42}" "class.$import" "Expected the path of the $import directive to be a string"
            fails "a second field in a directive" "{$import: s.yml, extra: 1}" "class" "A directive has exactly one field"
            fails "a mapping that is no directive" "{a: b}" "class" "Expected a string, found a mapping."
            fails "a class that is a list" "[CommandLineTool]" "class" "Expected a string"
    ]

let testClassFieldOfMapEntries =
    let toolWith (field: string) (entry: string) =
        String.concat "\n" [ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; field + ":"; entry; "inputs: []"; "outputs: []"; "" ]
    // Schema Salad gives the object the key of the entry as its class and drops the class field of the value unread.
    let classValues =
        [ "the key", "class: ShellCommandRequirement"
          "another requirement", "class: DockerRequirement"
          "an unknown class", "class: Bogus"
          "a number", "class: 5"
          "a list", "class: [ShellCommandRequirement]"
          "a mapping that is no directive", "class: {a: b}"
          "an import", "class: {$import: s.yml}"
          "an include", "class: {$include: s.txt}"
          "a mixin", "class: {$mixin: s.yml}"
          "a mixin beside a field", "class: {$mixin: s.yml, extra: 1}"
          "a directive that is malformed", "class: {$import: 42}" ]
    testList "The class field of a map-form requirement or hint" [
        testCase "a requirement is the requirement of its key whatever its class field holds" <| fun _ ->
            let plain = Decode.decodeCommandLineTool (toolWith "requirements" "  ShellCommandRequirement: {}")
            for (name, classField) in classValues do
                let tool = Decode.decodeCommandLineTool (toolWith "requirements" ("  ShellCommandRequirement:\n    " + classField))
                Expect.equal tool plain (sprintf "%s: the class field should change nothing." name)
                Expect.equal tool.Requirements.Value.[0] ShellCommandRequirement (sprintf "%s: the requirement should be the one of the key." name)
        testCase "a hint is the hint of its key whatever its class field holds" <| fun _ ->
            let plain = Decode.decodeCommandLineTool (toolWith "hints" "  ShellCommandRequirement: {}")
            for (name, classField) in classValues do
                let tool = Decode.decodeCommandLineTool (toolWith "hints" ("  ShellCommandRequirement:\n    " + classField))
                Expect.equal tool plain (sprintf "%s: the class field should change nothing." name)
                Expect.equal tool.Hints.Value.[0] (KnownHint ShellCommandRequirement) (sprintf "%s: the hint should be the one of the key." name)
        testCase "a hint of an unknown class keeps the key as its class and its other fields" <| fun _ ->
            for (name, classField) in classValues do
                let tool = Decode.decodeCommandLineTool (toolWith "hints" ("  acme:Hint:\n    " + classField + "\n    a: 1"))
                match tool.Hints.Value.[0] with
                | UnknownHint value ->
                    Expect.equal value.Class (Some "acme:Hint") (sprintf "%s: the class should be the key." name)
                    Expect.equal value.Raw (Reader.readRoot "class: acme:Hint\na: 1\n" |> fun node -> { Node = node }) (sprintf "%s: the hint should hold the key as its class and the other fields." name)
                | other -> failwith (sprintf "%s: expected an unknown hint, found %A" name other)
        testCase "the written hint and requirement have the class of the key and no class field of the value" <| fun _ ->
            for (name, classField) in classValues do
                for field in [ "requirements"; "hints" ] do
                    let tool = Decode.decodeCommandLineTool (toolWith field ("  ShellCommandRequirement:\n    " + classField))
                    let written = Encode.encodeToolDescription tool
                    Expect.stringContains written "class: ShellCommandRequirement" (sprintf "%s in %s: the written text should hold the class of the key." name field)
                    Expect.isFalse (written.Contains "$import" || written.Contains "$include" || written.Contains "$mixin" || written.Contains "Bogus") (sprintf "%s in %s: the field of the value is gone." name field)
                    Expect.equal (Decode.decodeCommandLineTool written) tool (sprintf "%s in %s: the written text should decode to the same tool." name field)
        testCase "a class field that holds the key stays where it stands in a hint" <| fun _ ->
            let tool = Decode.decodeCommandLineTool (toolWith "hints" "  acme:Hint:\n    a: 1\n    class: acme:Hint\n    b: 2")
            Expect.stringContains (Encode.encodeToolDescription tool) "  - a: 1\n    class: acme:Hint\n    b: 2\n" "The hint is written as it stands."
    ]

let testSecondaryFilePatternsAndNumberPositions =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    // The secondary files of an input, as a list or as one object, and of an output, each with the entry in place.
    let inputWith (entry: string) =
        tool [ "inputs:"; "  x:"; "    type: File"; "    secondaryFiles:"; "      - " + entry; "outputs: []" ]
    let oneObjectWith (entry: string) =
        tool [ "inputs:"; "  x:"; "    type: File"; "    secondaryFiles: " + entry; "outputs: []" ]
    let outputWith (entry: string) =
        tool [ "inputs: []"; "outputs:"; "  o:"; "    type: File"; "    outputBinding: {glob: o}"; "    secondaryFiles:"; "      - " + entry ]
    let resource (field: string) (value: string) =
        tool [ "requirements:"; "  - class: ResourceRequirement"; "    " + field + ": " + value; "inputs: []"; "outputs: []" ]
    let binding (position: string) =
        tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      position: " + position; "outputs: []" ]
    let positionOf (tool: CWLToolDescription) : BindingPosition option =
        match ListEntry.tryValue (ListEntry.values tool.Inputs.Value |> Seq.head |> fun input -> input.InputBinding.Value) with
        | Some inputBinding -> inputBinding.Position
        | None -> None
    testList "The small rejections of cwltool" [
        testCase "the pattern of a secondary file object is a string, an expression or a directive" <| fun _ ->
            for pattern in [ ".bai"; "\".bai\""; "\"1\""; "\"true\""; "\"\""; "2001-12-14"; "1_000"; "'$(self.basename).bai'"; "'${return self.basename;}'"; "{$import: p.yml}"; "{$include: p.txt}"; "{$mixin: p.yml}"; "{$mixin: p.yml, extra: 1}" ] do
                Decode.decodeCommandLineTool (inputWith ("pattern: " + pattern)) |> ignore
                Decode.decodeCommandLineTool (oneObjectWith ("{pattern: " + pattern + "}")) |> ignore
                Decode.decodeCommandLineTool (outputWith ("pattern: " + pattern)) |> ignore
            Decode.decodeCommandLineTool (inputWith "pattern: |\n          .bai") |> ignore
        testCase "a message names the type that the core schema gives a plain scalar and the number that a field takes" <| fun _ ->
            failsAt "a float as the time limit" (fun () -> Decode.decodeCommandLineTool (tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: 1.5"; "inputs: []"; "outputs: []" ]) |> ignore) "requirements[0].timelimit" "Expected an int or an expression, found the plain scalar 1.5, which the core schema reads as a float. An expression holds $( or ${."
            failsAt "a text as the position" (fun () -> Decode.decodeCommandLineTool (binding "\"1\"") |> ignore) "inputs.x.inputBinding.position" "Expected an int or an expression, found the text \"1\"."
            failsAt "null as a pattern" (fun () -> Decode.decodeCommandLineTool (inputWith "pattern: null") |> ignore) "inputs.x.secondaryFiles[0].pattern" "Expected a string or an expression as a pattern, found null."
            failsAt "a text in a resource field" (fun () -> Decode.decodeCommandLineTool (resource "ramMin" "abc") |> ignore) "requirements[0].ramMin" "Expected a number or an expression, found the text \"abc\"."
        testCase "a pattern that is a number, a boolean, null, a list or a mapping is a decode error at the pattern" <| fun _ ->
            for (name, pattern, found) in [ "an int", "1", "the plain scalar 1, which the core schema reads as an int"; "a float", "1.5", "the plain scalar 1.5, which the core schema reads as a float"; "infinity", ".inf", "the plain scalar .inf, which the core schema reads as a float"; "a boolean", "true", "the plain scalar true, which the core schema reads as a bool"
                                            "null", "~", "null"; "a list", "[a]", "a list"; "a mapping", "{a: b}", "a mapping" ] do
                failsAt (sprintf "%s in an input" name) (fun () -> Decode.decodeCommandLineTool (inputWith ("pattern: " + pattern)) |> ignore) "inputs.x.secondaryFiles[0].pattern" ("Expected a string or an expression as a pattern, found " + found)
                failsAt (sprintf "%s in one object" name) (fun () -> Decode.decodeCommandLineTool (oneObjectWith ("{pattern: " + pattern + "}")) |> ignore) "inputs.x.secondaryFiles.pattern" "Expected a string or an expression as a pattern"
                failsAt (sprintf "%s in an output" name) (fun () -> Decode.decodeCommandLineTool (outputWith ("pattern: " + pattern)) |> ignore) "outputs.o.secondaryFiles[0].pattern" "Expected a string or an expression as a pattern"
        testCase "a scalar that is no text and a list in the secondary files are decode errors at the entry, and null as the whole field is missing" <| fun _ ->
            for (name, entry) in [ "an int", "5"; "null", "null"; "a boolean", "true"; "a float", "1.5"; "an int beyond 64 bits", "123456789012345678901234567890"; "a list", "[a]" ] do
                failsAt (sprintf "%s in a list of an input" name) (fun () -> Decode.decodeCommandLineTool (inputWith entry) |> ignore) "inputs.x.secondaryFiles[0]" "Expected a string or an object as a secondary file, found"
                failsAt (sprintf "%s in a list of an output" name) (fun () -> Decode.decodeCommandLineTool (outputWith entry) |> ignore) "outputs.o.secondaryFiles[0]" "Expected a string or an object as a secondary file, found"
            for (name, entry) in [ "an int", "5"; "a boolean", "true"; "a float", "1.5" ] do
                failsAt (sprintf "%s as the whole field" name) (fun () -> Decode.decodeCommandLineTool (oneObjectWith entry) |> ignore) "inputs.x.secondaryFiles" "Expected a string or an object as a secondary file, found"
            for entry in [ ".bai"; "'5'"; "'null'"; "''"; "'^.idx'"; "null"; "[.bai, \"^.idx\"]"; "[]" ] do
                Decode.decodeCommandLineTool (oneObjectWith entry) |> ignore
            for entry in [ ".bai"; "'5'"; "'null'"; "''"; "'$(self)'" ] do
                Decode.decodeCommandLineTool (inputWith entry) |> ignore
                Decode.decodeCommandLineTool (outputWith entry) |> ignore
            for entry in [ "{$import: s.yml}"; "{$include: s.txt}"; "{$mixin: s.yml}" ] do
                Decode.decodeCommandLineTool (inputWith entry) |> ignore
                Decode.decodeCommandLineTool (oneObjectWith entry) |> ignore
        testCase "a secondary file object without a pattern, an empty one included, is a decode error at the object" <| fun _ ->
            failsAt "required only" (fun () -> Decode.decodeCommandLineTool (inputWith "required: true") |> ignore) "inputs.x.secondaryFiles[0]" "Expected a pattern in an object as a secondary file"
            failsAt "empty object" (fun () -> Decode.decodeCommandLineTool (inputWith "{}") |> ignore) "inputs.x.secondaryFiles[0]" "Expected a pattern in an object as a secondary file"
            failsAt "empty one object" (fun () -> Decode.decodeCommandLineTool (oneObjectWith "{}") |> ignore) "inputs.x.secondaryFiles" "Expected a pattern in an object as a secondary file"
            failsAt "missing value" (fun () -> Decode.decodeCommandLineTool (inputWith "pattern:") |> ignore) "inputs.x.secondaryFiles[0].pattern" "Expected a string or an expression as a pattern"
            failsAt "the second entry" (fun () -> Decode.decodeCommandLineTool (inputWith "pattern: .bai\n      - pattern: 1") |> ignore) "inputs.x.secondaryFiles[1].pattern" "Expected a string or an expression as a pattern"
        testCase "a malformed directive as a pattern is a decode error, and a text, an empty list and a directive as a secondary file are no objects to check" <| fun _ ->
            failsAt "a malformed directive" (fun () -> Decode.decodeCommandLineTool (inputWith "pattern: {$import: 42}") |> ignore) "inputs.x.secondaryFiles[0].pattern.$import" "Expected the path of the $import directive to be a string"
            Decode.decodeCommandLineTool (inputWith ".bai") |> ignore
            Decode.decodeCommandLineTool (inputWith "{$import: f.yml}") |> ignore
            Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: File"; "    secondaryFiles: []"; "outputs: []" ]) |> ignore
            Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: File"; "    secondaryFiles: [.bai]"; "outputs: []" ]) |> ignore
        testCase "the position of an input binding is an int, an expression or a directive" <| fun _ ->
            for expression in [ "'$(1)'"; "'${return 1;}'" ] do
                Expect.equal (positionOf (Decode.decodeCommandLineTool (binding expression))) (Some (BindingPosition.Expression (expression.Trim '\''))) (sprintf "%s: the position should be the expression." expression)
            Expect.equal (positionOf (Decode.decodeCommandLineTool (binding "3"))) (Some (BindingPosition.Number 3)) "A number is a position."
            Expect.equal (positionOf (Decode.decodeCommandLineTool (binding "{$import: p.yml}"))) (Some (BindingPosition.Imported "p.yml")) "A directive stands for the position of a file."
        testCase "a float and an int beyond 64 bits are the number of every resource field, in a requirement and in a hint" <| fun _ ->
            for field in [ "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" ] do
                for (value, expected) in [ "1.5", box 1.5; "4.0", box 4.0; ".inf", box infinity; "100000000000000000000000000000", box 1e29; "5", box 5L ] do
                    match (Decode.decodeCommandLineTool (resource field value)).Requirements.Value.[0] with
                    | ResourceRequirement resources ->
                        let held = resources.KnownFieldValues |> Seq.pick (fun (name, held) -> if name = field then held else None)
                        Expect.equal held expected (sprintf "%s %s: the field should hold the number." field value)
                    | other -> failwith (sprintf "Expected a ResourceRequirement, found %A" other)
                    Decode.decodeCommandLineTool (tool [ "hints:"; "  ResourceRequirement:"; "    " + field + ": " + value; "inputs: []"; "outputs: []" ]) |> ignore
    ]

let testDirectivesAsItemsOfAListingArray =
    let toolWith (listing: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:" ] @ (listing |> List.map (fun line -> "      " + line)) @ [ "inputs: []"; "outputs: []"; "" ])
    let itemsOf (tool: CWLToolDescription) : ResizeArray<FileOrDirectory> =
        match tool.Requirements.Value.[0] with
        | InitialWorkDirRequirement entries ->
            match entries.[0] with
            | FilesEntry items -> items
            | other -> failwith (sprintf "Expected an array of Files and Directories, found %A" other)
        | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
    let mixed (text: string) = { Node = Reader.readRoot text }
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    testList "Directives as items of an array in the listing" [
        testCase "an import stands for the File, the Directory or the list of them that a file holds, in the place of the item" <| fun _ ->
            let tool = Decode.decodeCommandLineTool (toolWith [ "- - {$import: file.yml}"; "  - {class: File, location: a.txt}"; "  - {$import: list.yml}" ])
            Expect.sequenceEqual (itemsOf tool) (ResizeArray [ ImportedItem "file.yml"; FileItem (FileInstance(location = SchemaSaladString.Literal "a.txt")); ImportedItem "list.yml" ]) "The imports are items of their own, in their places."
            let written = Encode.encodeToolDescription tool
            Expect.stringContains written "- {$import: file.yml}" "The first import is written as it was."
            Expect.stringContains written "- {$import: list.yml}" "The second import is written as it was."
            Expect.equal (Decode.decodeCommandLineTool written) tool "The written text should decode to the same tool."
        testCase "a mixin, alone or beside fields, is an item of its own and stays as it was written" <| fun _ ->
            for text in [ "{$mixin: file.yml}"; "{$mixin: file.yml, basename: z.txt}"; "{$mixin: file.yml, class: File}" ] do
                let tool = Decode.decodeCommandLineTool (toolWith [ "- - " + text ])
                Expect.sequenceEqual (itemsOf tool) (ResizeArray [ MixedItem (mixed text) ]) (sprintf "%s: the mixin should stay as written." text)
                let written = Encode.encodeToolDescription tool
                Expect.stringContains written text (sprintf "%s: the mixin should be written back as it was." text)
                Expect.equal (Decode.decodeCommandLineTool written) tool (sprintf "%s: the written text should decode to the same tool." text)
        testCase "two items with different paths or kinds of directive differ" <| fun _ ->
            Expect.notEqual (ImportedItem "a.yml") (ImportedItem "b.yml") "Two paths differ."
            Expect.notEqual (ImportedItem "a.yml") (MixedItem (mixed "{$mixin: a.yml}")) "An import and a mixin differ."
            Expect.equal (MixedItem (mixed "{$mixin: a.yml, basename: z}")) (MixedItem (mixed "{$mixin: a.yml, basename: z}")) "The same mixin is equal."
        testCase "an include, a malformed import and an import with a second field are decode errors at the item" <| fun _ ->
            failsAt "an include" (fun () -> Decode.decodeCommandLineTool (toolWith [ "- - {$include: c.txt}" ]) |> ignore) "requirements[0].listing[0][0]" "An $include yields the text of a file, but an item of an array in the listing takes an object."
            failsAt "an import of a number" (fun () -> Decode.decodeCommandLineTool (toolWith [ "- - {$import: 42}" ]) |> ignore) "requirements[0].listing[0][0].$import" "Expected the path of the $import directive to be a string"
            failsAt "an import with a field" (fun () -> Decode.decodeCommandLineTool (toolWith [ "- - {$import: file.yml, extra: 1}" ]) |> ignore) "requirements[0].listing[0][0]" "A directive has exactly one field"
            failsAt "a text" (fun () -> Decode.decodeCommandLineTool (toolWith [ "- - abc" ]) |> ignore) "requirements[0].listing[0][0]" "Expected a File or a Directory with a class field in an array of the listing"
    ]

let testSchemaDefinitionsWithMixins =
    let tool (field: string) (items: string list) (inputs: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; field + ":" ] @ items @ [ "inputs:" ] @ (inputs |> List.map (fun line -> "  " + line)) @ [ "outputs: []"; "" ])
    let schemaDef (entries: string list) =
        [ "  - class: SchemaDefRequirement"; "    types:" ] @ (entries |> List.map (fun entry -> "      - " + entry))
    let typeOf (decoded: CWLToolDescription) (name: string) : CWLType option =
        ListEntry.values decoded.Inputs.Value |> Seq.find (fun input -> input.Name = name) |> fun input -> input.Type_
    let fails (text: string) (message: string) =
        match (try Decode.decodeCommandLineTool text |> ignore; None with YamlDecodeException error -> Some error) with
        | Some error -> Expect.stringContains error.Message message "The error should name the type that nothing defines."
        | None -> failwith ("The document must fail:\n" + text)
    testList "SchemaDefRequirements with a mixin" [
        testCase "a requirement with a mixin and a types field defines those types, as a requirement or as a hint" <| fun _ ->
            for field in [ "requirements"; "hints" ] do
                let decoded = Decode.decodeCommandLineTool (tool field [ "  - {$mixin: sdclass.yml, types: [{name: Colour, type: enum, symbols: [red, blue]}]}" ] [ "c: Colour" ])
                Expect.equal (typeOf decoded "c") (Some (CWLType.Reference "Colour")) (sprintf "%s: the name is the type that the mixin item defines." field)
                fails (tool field [ "  - {$mixin: sdclass.yml, types: [{name: Other, type: enum, symbols: [red]}]}" ] [ "c: Colour" ]) "Invalid CWL type: Colour"
        testCase "a requirement with a mixin and no types field may bring types, so every well-formed name counts" <| fun _ ->
            for field in [ "requirements"; "hints" ] do
                for item in [ "{$mixin: sdfull.yml}"; "{$mixin: sdfull.yml, extra: 1}"; "{$mixin: sdfull.yml, class: SchemaDefRequirement}" ] do
                    let decoded = Decode.decodeCommandLineTool (tool field [ "  - " + item ] [ "c: Colour" ])
                    Expect.equal (typeOf decoded "c") (Some (CWLType.Reference "Colour")) (sprintf "%s %s: the file of the mixin may define the type." field item)
        testCase "an open requirement lets a type behind a path through, because the file of the mixin may import any file" <| fun _ ->
            let viaRequirement = Decode.decodeCommandLineTool (tool "requirements" [ "  - $mixin: sd.yml" ] [ "c: types.yml#Colour" ])
            Expect.equal (typeOf viaRequirement "c") (Some (CWLType.Reference "types.yml#Colour")) "A requirement that holds a mixin only may import the file."
            let viaEntry = Decode.decodeCommandLineTool (tool "requirements" (schemaDef [ "{$mixin: sdentry.yml}" ]) [ "c: types.yml#Colour" ])
            Expect.equal (typeOf viaEntry "c") (Some (CWLType.Reference "types.yml#Colour")) "An entry of the types field that holds a mixin only may import the file."
            fails (tool "requirements" [ "  - {$mixin: nn.yml, class: NetworkAccess, networkAccess: true}" ] [ "c: types.yml#Colour" ]) "The file types.yml is not imported by a SchemaDefRequirement of the document."
        testCase "a mixin that names another class cannot bring types, and a plain document still fails for a type that nothing defines" <| fun _ ->
            fails (tool "requirements" [ "  - {$mixin: nn.yml, class: NetworkAccess, networkAccess: true}" ] [ "c: Colour" ]) "Invalid CWL type: Colour"
            fails (tool "requirements" [ "  - class: InlineJavascriptRequirement" ] [ "c: Colour" ]) "Invalid CWL type: Colour"
            fails (tool "requirements" [ "  - {$import: sdfull.yml}" ] [ "c: Colour" ]) "Invalid CWL type: Colour"
        testCase "an entry of the types field with a mixin and a name defines that name, with the fields and items that it shows" <| fun _ ->
            let named = Decode.decodeCommandLineTool (tool "requirements" (schemaDef [ "{$mixin: enumbody.yml, name: Colour}" ]) [ "c: Colour" ])
            Expect.equal (typeOf named "c") (Some (CWLType.Reference "Colour")) "The name beside the mixin is the type."
            let record = Decode.decodeCommandLineTool (tool "requirements" (schemaDef [ "{$mixin: recbody.yml, name: Rec, fields: [{name: x, type: {name: Inner, type: enum, symbols: [a]}}]}" ]) [ "c: Rec"; "i: Rec/x/Inner" ])
            Expect.equal (typeOf record "c") (Some (CWLType.Reference "Rec")) "The record is a type."
            Expect.equal (typeOf record "i") (Some (CWLType.Reference "Rec/x/Inner")) "The schema in a field of the record is a type below it."
            fails (tool "requirements" (schemaDef [ "{$mixin: enumbody.yml, name: Other}" ]) [ "c: Colour" ]) "Invalid CWL type: Colour"
        testCase "an entry of the types field with a mixin and no name may name its type, so every well-formed name counts" <| fun _ ->
            for entry in [ "{$mixin: named.yml}"; "{$mixin: named.yml, type: enum}"; "{$mixin: enumbody.yml}" ] do
                let decoded = Decode.decodeCommandLineTool (tool "requirements" (schemaDef [ entry ]) [ "c: Colour" ])
                Expect.equal (typeOf decoded "c") (Some (CWLType.Reference "Colour")) (sprintf "%s: the file of the mixin may name the type." entry)
            let later = Decode.decodeCommandLineTool (tool "requirements" (schemaDef [ "{$mixin: named.yml}"; "{name: Inner, type: enum, symbols: [a]}" ]) [ "c: Colour"; "i: Inner" ])
            Expect.equal (typeOf later "i") (Some (CWLType.Reference "Inner")) "The entry that follows defines its own type."
        testCase "an open requirement leaves the types of the requirement of an inner process alone" <| fun _ ->
            let workflow =
                String.concat "\n"
                    [ "cwlVersion: v1.2"; "class: Workflow"; "requirements:"; "  - {$mixin: sdfull.yml}"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    in: {}"; "    out: []"
                      "    run:"; "      class: CommandLineTool"; "      baseCommand: echo"; "      requirements:"; "        - class: SchemaDefRequirement"; "          types:"; "            - {name: Other, type: enum, symbols: [a]}"
                      "      inputs:"; "        c: Colour"; "      outputs: []"; "" ]
            match (try Decode.decodeWorkflow workflow |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error -> Expect.stringContains error.Message "Invalid CWL type: Colour" "The requirement of the tool replaces the open one."
            | None -> failwith "The tool defines Other only, so Colour must fail."
    ]

let testRawObjectsAndMixinObjects =
    let toolWith (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let raw (text: string) = { Node = Reader.readRoot text }
    let listing (entries: string list) =
        toolWith ([ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:" ] @ (entries |> List.map (fun entry -> "      " + entry)) @ [ "inputs: []"; "outputs: []" ])
    let listingOf (tool: CWLToolDescription) : ResizeArray<InitialWorkDirEntry> =
        match tool.Requirements.Value.[0] with
        | InitialWorkDirRequirement entries -> entries
        | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
    testList "A raw case holds an object without a mixin and a Mixed case holds an object with one" [
        testCase "an object whose class or type is a directive is a raw case in each position" <| fun _ ->
            let classText = "{$import: c.yml}"
            let tool = Decode.decodeCommandLineTool (toolWith [ "requirements:"; "  - class: " + classText; "hints:"; "  - class: " + classText; "inputs: {x: {type: {type: " + classText + ", symbols: [a]}}}"; "outputs: []" ])
            Expect.equal tool.Requirements.Value.[0] (RawRequirement (raw "{class: {$import: c.yml}}")) "A requirement with a directive as its class is a raw requirement."
            Expect.equal tool.Hints.Value.[0] (RawHint (raw "{class: {$import: c.yml}}")) "A hint with a directive as its class is a raw hint."
            Expect.equal (ListEntry.values tool.Inputs.Value).[0].Type_ (Some (RawType (raw "{type: {$import: c.yml}, symbols: [a]}"))) "A schema with a directive as its type is a raw type."
            let schemaDef = Decode.decodeCommandLineTool (toolWith [ "requirements:"; "  - class: SchemaDefRequirement"; "    types:"; "      - {type: " + classText + ", name: R, fields: []}"; "inputs: []"; "outputs: []" ])
            Expect.equal schemaDef.Requirements.Value.[0] (SchemaDefRequirement (ResizeArray [ SchemaDefEntry.Raw (raw "{type: {$import: c.yml}, name: R, fields: []}") ])) "An entry with a directive as its type is a raw entry."
            let entries = listingOf (Decode.decodeCommandLineTool (listing [ "- {class: " + classText + ", location: a.txt}" ]))
            Expect.sequenceEqual entries (ResizeArray [ RawEntry (raw "{class: {$import: c.yml}, location: a.txt}") ]) "A File or Directory with a directive as its class is a raw entry."
            let workflow = Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps:\n  s:\n    run: {class: {$import: c.yml}, baseCommand: echo, inputs: [], outputs: []}\n    in: []\n    out: []\n"
            Expect.equal (ListEntry.values workflow.Steps).[0].Run (RunRaw (raw "{class: {$import: c.yml}, baseCommand: echo, inputs: [], outputs: []}")) "A run with a directive as its class is a raw run."
            Expect.equal (Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: {$import: c.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n") (RawProcess (raw "{cwlVersion: v1.2, class: {$import: c.yml}, baseCommand: echo, inputs: [], outputs: []}")) "A process with a directive as its class is a raw process."
        testCase "an object with a mixin stays a Mixed case, and a raw case differs from the Mixed case of the same text" <| fun _ ->
            let mixinText = "{$mixin: f.yml, class: NetworkAccess}"
            let tool = Decode.decodeCommandLineTool (toolWith [ "requirements:"; "  - " + mixinText; "hints:"; "  - " + mixinText; "inputs: {x: {type: {$mixin: t.yml}}}"; "outputs: []" ])
            Expect.equal tool.Requirements.Value.[0] (MixedRequirement (raw mixinText)) "A requirement with a mixin is a mixed requirement."
            Expect.equal tool.Hints.Value.[0] (MixedHint (raw mixinText)) "A hint with a mixin is a mixed hint."
            Expect.equal (ListEntry.values tool.Inputs.Value).[0].Type_ (Some (MixedType (raw "{$mixin: t.yml}"))) "A type with a mixin is a mixed type."
            let schemaDef = Decode.decodeCommandLineTool (toolWith [ "requirements:"; "  - class: SchemaDefRequirement"; "    types:"; "      - {$mixin: t.yml, name: R}"; "inputs: []"; "outputs: []" ])
            Expect.equal schemaDef.Requirements.Value.[0] (SchemaDefRequirement (ResizeArray [ SchemaDefEntry.Mixed (raw "{$mixin: t.yml, name: R}") ])) "An entry with a mixin is a mixed entry."
            let entries = listingOf (Decode.decodeCommandLineTool (listing [ "- {$mixin: f.yml, basename: z.txt}"; "- [{$mixin: f.yml, basename: z.txt}]" ]))
            Expect.equal entries.[0] (StringEntry (SchemaSaladString.Mixed (raw "{$mixin: f.yml, basename: z.txt}"))) "A listing entry with a mixin is a mixed string entry."
            Expect.equal entries.[1] (FilesEntry (ResizeArray [ MixedItem (raw "{$mixin: f.yml, basename: z.txt}") ])) "An item of an array with a mixin is a mixed item."
            let workflow = Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps:\n  s:\n    run: {$mixin: tool.cwl}\n    in: []\n    out: []\n"
            Expect.equal (ListEntry.values workflow.Steps).[0].Run (RunMixed (raw "{$mixin: tool.cwl}")) "A run with a mixin is a mixed run."
            Expect.notEqual (RawRequirement (raw "{class: {$import: c.yml}}")) (MixedRequirement (raw "{class: {$import: c.yml}}")) "The cases differ whatever the text."
            Expect.notEqual (RunRaw (raw "{a: 1}")) (RunMixed (raw "{a: 1}")) "The cases of a run differ."
            Expect.notEqual (RawEntry (raw "{a: 1}")) (StringEntry (SchemaSaladString.Mixed (raw "{a: 1}"))) "The cases of a listing entry differ."
        testCase "an array of the listing keeps the Files and Directories that it shows beside an object with a directive as its class" <| fun _ ->
            let entries = listingOf (Decode.decodeCommandLineTool (listing [ "- - {class: File, location: a.txt}"; "  - {class: {$include: c.txt}, location: b.txt}"; "  - {class: Directory, location: d}" ]))
            Expect.sequenceEqual entries (ResizeArray [ FilesEntry (ResizeArray [ FileItem (FileInstance(location = SchemaSaladString.Literal "a.txt")); RawItem (raw "{class: {$include: c.txt}, location: b.txt}"); DirectoryItem (DirectoryInstance(location = SchemaSaladString.Literal "d")) ]) ]) "The array holds its three items."
            let tool = Decode.decodeCommandLineTool (listing [ "- - {class: File, location: a.txt}"; "  - {class: {$include: c.txt}, location: b.txt}" ])
            let written = Encode.encodeToolDescription tool
            Expect.stringContains written "{class: {$include: c.txt}, location: b.txt}" "The object is written as it was."
            Expect.equal (Decode.decodeCommandLineTool written) tool "The written text should decode to the same tool."
    ]

let testDirentFieldsBesideAClass =
    let listing (entries: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo"; "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing:" ] @ (entries |> List.map (fun entry -> "      " + entry)) @ [ "inputs: []"; "outputs: []"; "" ])
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    let classes = [ "{$include: c.txt}"; "{$import: c.yml}"; "{$mixin: c.yml}"; "{$mixin: c.yml, extra: 1}" ]
    testList "The fields of a Dirent beside a class" [
        testCase "the fields entry, entryname and writable are decode errors beside a class that is a directive, in an entry and in an array" <| fun _ ->
            for cls in classes do
                for field in [ "entry: foo"; "entryname: x.txt"; "writable: true" ] do
                    let name = field.Split(':').[0]
                    failsAt (sprintf "%s with %s" cls field) (fun () -> Decode.decodeCommandLineTool (listing [ "- {class: " + cls + ", location: a.txt, " + field + "}" ]) |> ignore) ("requirements[0].listing[0]." + name) "belong to a Dirent"
                    failsAt (sprintf "%s with %s in an array" cls field) (fun () -> Decode.decodeCommandLineTool (listing [ "- [{class: " + cls + ", location: a.txt, " + field + "}]" ]) |> ignore) ("requirements[0].listing[0][0]." + name) "belong to a Dirent"
        testCase "a class beside the fields of a Dirent is a decode error at the class" <| fun _ ->
            for cls in [ "Dirent"; "Foo" ] do
                failsAt ("the class " + cls) (fun () -> Decode.decodeCommandLineTool (listing [ "- {class: " + cls + ", entry: foo, entryname: x.txt}" ]) |> ignore) "requirements[0].listing[0].class" ("A Dirent has no class field, found the class " + cls)
        testCase "a Dirent without a class and an object with a directive as its class and no field of a Dirent still decode" <| fun _ ->
            let tool = Decode.decodeCommandLineTool (listing [ "- {entry: foo, entryname: x.txt}"; "- {class: {$include: c.txt}, location: a.txt}"; "- [{class: {$include: c.txt}, location: a.txt}]" ])
            match tool.Requirements.Value.[0] with
            | InitialWorkDirRequirement entries ->
                Expect.isTrue (match entries.[0] with DirentEntry _ -> true | _ -> false) "The first entry is the Dirent."
                Expect.isTrue (match entries.[1] with RawEntry _ -> true | _ -> false) "The second entry is the raw object."
                Expect.isTrue (match entries.[2] with FilesEntry items -> (match items.[0] with RawItem _ -> true | _ -> false) | _ -> false) "The third entry is the array with a raw item."
            | other -> failwith (sprintf "Expected an InitialWorkDirRequirement, found %A" other)
    ]

let testTextWithoutAnExpressionInExpressionPositions =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
        | None -> failwith (sprintf "%s must fail" name)
    // The name of the position, the path of its error, the document with the value in place, and what the field takes.
    let positions : (string * string * (string -> string) * string) list =
        [ "position of an input binding", "inputs.x.inputBinding.position", (fun value -> tool [ "inputs:"; "  x:"; "    type: string"; "    inputBinding:"; "      position: " + value; "outputs: []" ]), "an int or an expression"
          "listing of an InitialWorkDirRequirement", "requirements[0].listing", (fun value -> tool [ "requirements:"; "  - class: InitialWorkDirRequirement"; "    listing: " + value; "inputs: []"; "outputs: []" ]), "a list of entries or an expression for the listing"
          "networkAccess of NetworkAccess", "requirements[0].networkAccess", (fun value -> tool [ "requirements:"; "  - class: NetworkAccess"; "    networkAccess: " + value; "inputs: []"; "outputs: []" ]), "a bool or an expression"
          "networkAccess of a hint", "hints[0].networkAccess", (fun value -> tool [ "hints:"; "  - class: NetworkAccess"; "    networkAccess: " + value; "inputs: []"; "outputs: []" ]), "a bool or an expression"
          "enableReuse of WorkReuse", "requirements[0].enableReuse", (fun value -> tool [ "requirements:"; "  - class: WorkReuse"; "    enableReuse: " + value; "inputs: []"; "outputs: []" ]), "a bool or an expression"
          "enableReuse of a hint", "hints[0].enableReuse", (fun value -> tool [ "hints:"; "  - class: WorkReuse"; "    enableReuse: " + value; "inputs: []"; "outputs: []" ]), "a bool or an expression"
          "outputEval of an output binding", "outputs.o.outputBinding.outputEval", (fun value -> tool [ "inputs: []"; "outputs:"; "  o:"; "    type: string"; "    outputBinding:"; "      outputEval: " + value ]), "an expression"
          "expression of an ExpressionTool", "expression", (fun value -> "cwlVersion: v1.2\nclass: ExpressionTool\ninputs: []\noutputs: []\nexpression: " + value + "\n"), "an expression"
          "when of a step", "steps.s.when", (fun value -> "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps:\n  s:\n    run: {class: CommandLineTool, baseCommand: echo, inputs: [], outputs: []}\n    in: []\n    out: []\n    when: " + value + "\n"), "an expression"
          "timelimit of ToolTimeLimit", "requirements[0].timelimit", (fun value -> tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + value; "inputs: []"; "outputs: []" ]), "an int or an expression"
          "ramMin of ResourceRequirement", "requirements[0].ramMin", (fun value -> tool [ "requirements:"; "  - class: ResourceRequirement"; "    ramMin: " + value; "inputs: []"; "outputs: []" ]), "a number or an expression" ]
    testList "A text without an expression in the positions that CWL types as an expression" [
        testCase "a text without $( or ${ is a decode error in every position" <| fun _ ->
            for (name, path, document, expected) in positions do
                for text in [ "abc"; "\"5\""; "\"true\""; "\"\"" ] do
                    failsAt (sprintf "%s with %s" name text) (fun () -> Decode.decodeCWLDocument (document text) |> ignore) path ("Expected " + expected + ", found")
        testCase "an expression and a directive are read in every position" <| fun _ ->
            for (name, _, document, _) in positions do
                for text in [ "$(1)"; "{$include: e.txt}"; "{$import: e.yml}" ] do
                    Decode.decodeCWLDocument (document text) |> ignore
            let documentOf (name: string) = positions |> List.pick (fun (positionName, _, document, _) -> if positionName = name then Some document else None)
            Decode.decodeCWLDocument (documentOf "networkAccess of NetworkAccess" "true") |> ignore
            Decode.decodeCWLDocument (documentOf "enableReuse of WorkReuse" "false") |> ignore
            Decode.decodeCWLDocument (documentOf "timelimit of ToolTimeLimit" "60") |> ignore
            Decode.decodeCWLDocument (documentOf "outputEval of an output binding" "'${return 1;}'") |> ignore
    ]

let testAdviceForNonTextScalarsInExpressionPositions =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "inputs: []"; "outputs: []"; "" ])
    let failsAt (name: string) (decode: unit -> unit) (path: string) =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
            error.Message
        | None -> failwith (sprintf "%s must fail" name)
    testList "The advice for a non-text scalar where CWL types an expression" [
        testCase "a float, an int or a bool where a number or an expression stands is reported without the advice to quote it" <| fun _ ->
            for (field, value) in [ "timelimit", "1.5"; "timelimit", ".inf"; "timelimit", "100000000000000000000000000000"; "coresMin", "true"; "timelimit", ".nan"; "timelimit", "0x8000000000000000" ] do
                let requirement = if field = "timelimit" then [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: " + value ] else [ "requirements:"; "  - class: ResourceRequirement"; "    " + field + ": " + value ]
                let message = failsAt (field + ": " + value) (fun () -> Decode.decodeCommandLineTool (tool requirement) |> ignore) ("requirements[0]." + field)
                Expect.stringContains message (if field = "timelimit" then "Expected an int or an expression, found" else "Expected a number or an expression, found") (sprintf "%s: %s should say what the field takes." field value)
                Expect.isFalse (message.Contains "quotes") (sprintf "%s: %s should not advise quotes, because a quoted text is no expression." field value)
        testCase "the quoted spelling of such a scalar is an error that says an expression holds $( or ${" <| fun _ ->
            let message = failsAt "a quoted float" (fun () -> Decode.decodeCommandLineTool (tool [ "requirements:"; "  - class: ToolTimeLimit"; "    timelimit: \"1.5\"" ]) |> ignore) "requirements[0].timelimit"
            Expect.stringContains message "Expected an int or an expression, found the text \"1.5\". An expression holds $( or ${." "The quoted text names the rule."
        testCase "a number where the symbol of an enum stands is reported as an unknown symbol, without the advice to quote it" <| fun _ ->
            for (lines, path, expected) in
                [ [ "inputs: {x: {type: Directory, loadListing: 5}}"; "outputs: []" ], "inputs.x.loadListing", "Expected one of no_listing, shallow_listing and deep_listing, found"
                  [ "inputs: {x: {type: Directory, loadListing: 1.5}}"; "outputs: []" ], "inputs.x.loadListing", "Expected one of no_listing, shallow_listing and deep_listing, found"
                  [ "inputs: {x: {type: Directory, loadListing: true}}"; "outputs: []" ], "inputs.x.loadListing", "Expected one of no_listing, shallow_listing and deep_listing, found" ] do
                let message = failsAt path (fun () -> Decode.decodeCommandLineTool (String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])) |> ignore) path
                Expect.stringContains message expected (sprintf "%s: the field should name its symbols." path)
                Expect.isFalse (message.Contains "quotes") (sprintf "%s: the error should not advise quotes." path)
            let workflow (field: string) (value: string) =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in: {x: {source: [a, b], " + field + ": " + value + "}}"; "    out: []"; "" ]
            for field in [ "linkMerge"; "pickValue" ] do
                let message = failsAt field (fun () -> Decode.decodeWorkflow (workflow field "5") |> ignore) ("steps.s.in.x." + field)
                Expect.stringContains message ("Invalid " + field + " value: 5") (sprintf "%s: the field should name the value." field)
        testCase "a number where a bool or an expression stands and a number where an expression stands name what the field takes" <| fun _ ->
            let document (lines: string list) = String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
            for (lines, path, expected) in
                [ [ "requirements:"; "  - class: NetworkAccess"; "    networkAccess: 5"; "inputs: []"; "outputs: []" ], "requirements[0].networkAccess", "Expected a bool or an expression, found"
                  [ "requirements:"; "  - class: WorkReuse"; "    enableReuse: 1.5"; "inputs: []"; "outputs: []" ], "requirements[0].enableReuse", "Expected a bool or an expression, found"
                  [ "inputs: []"; "outputs:"; "  o:"; "    type: string"; "    outputBinding: {outputEval: 5}" ], "outputs.o.outputBinding.outputEval", "Expected an expression, found" ] do
                let message = failsAt path (fun () -> Decode.decodeCommandLineTool (document lines) |> ignore) path
                Expect.stringContains message expected (sprintf "%s: the field should say what it takes." path)
                Expect.isFalse (message.Contains "quotes") (sprintf "%s: the error should not advise quotes." path)
    ]

let testOneWriterForTheEntriesOfAList =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    testList "One writer for the entries of a list" [
        testCase "the flow text of a record writes a mixed field as it was written" <| fun _ ->
            let mixed = { Node = YAMLicious.Reader.readRoot "{$mixin: f.yml, name: x}" }
            let recordType = Record (InputRecordSchema(fields = ResizeArray [ ListEntry.Mixed mixed; ListEntry.Imported "g.yml" ]))
            Expect.equal (Encode.cwlTypeToYamlString recordType) "{type: record, fields: [{$mixin: f.yml, name: x}, {$import: g.yml}]}" "The mixin and the import stand in the list."
        testCase "the success codes, the packages and the environment definitions write an import and a mixin in their places" <| fun _ ->
            let text =
                tool [ "successCodes: [0, {$import: codes.yml}, {$mixin: code.yml}]"
                       "requirements:"
                       "  - {class: SoftwareRequirement, packages: [{package: git}, {$import: packages.yml}, {$mixin: package.yml, package: curl}]}"
                       "  - {class: EnvVarRequirement, envDef: [{envName: A, envValue: a}, {$import: env.yml}, {$mixin: e.yml, envName: B}]}"
                       "inputs: []"
                       "outputs: []" ]
            let decoded = Decode.decodeCommandLineTool text
            let written = Encode.encodeToolDescription decoded
            for expected in [ "{$import: codes.yml}"; "{$mixin: code.yml}"; "{$import: packages.yml}"; "{$mixin: package.yml, package: curl}"; "{$import: env.yml}"; "{$mixin: e.yml, envName: B}" ] do
                Expect.stringContains written expected (sprintf "%s should be written back as it was." expected)
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
        testCase "the inputs, the outputs and the steps write an import and a mixin in their places in a list with an import" <| fun _ ->
            let text = tool [ "inputs: [{$import: ports.yml}, {id: a, type: string}, {$mixin: port.yml, id: b}]"; "outputs: [{id: o, type: string, outputBinding: {glob: o}}, {$import: outs.yml}]" ]
            let decoded = Decode.decodeCommandLineTool text
            let written = Encode.encodeToolDescription decoded
            for expected in [ "{$import: ports.yml}"; "{$mixin: port.yml, id: b}"; "{$import: outs.yml}" ] do
                Expect.stringContains written expected (sprintf "%s should be written back as it was." expected)
            Expect.equal (Decode.decodeCommandLineTool written) decoded "The written text should decode to the same tool."
    ]

let testProhibitedYamlFeatures =
    let tool (lines: string list) =
        String.concat "\n" ([ "cwlVersion: v1.2"; "class: CommandLineTool"; "baseCommand: echo" ] @ lines @ [ "" ])
    /// The error of a decode that has to fail, with the place checks that every error has.
    let errorOf (name: string) (decode: unit -> unit) : DecodeError =
        match (try decode (); None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
            error
        | None -> failwith (sprintf "%s must fail" name)
    let failsAt (name: string) (decode: unit -> unit) (path: string) (message: string) =
        let error = errorOf name decode
        Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
        Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
    let startOf (error: DecodeError) = error.Span |> Option.map (fun span -> span.Start.Line, span.Start.Column)
    /// The tree without the anchors, so that only an alias is left to meet.
    let rec withoutAnchors (node: YamlNode) : YamlNode =
        let kind =
            match node.Kind with
            | YamlNodeKind.Mapping collection -> YamlNodeKind.Mapping { collection with Items = collection.Items |> List.map (fun entry -> { entry with Key = withoutAnchors entry.Key; Value = withoutAnchors entry.Value }) }
            | YamlNodeKind.Sequence collection -> YamlNodeKind.Sequence { collection with Items = collection.Items |> List.map (fun item -> { item with Node = withoutAnchors item.Node }) }
            | other -> other
        { node with Kind = kind; Properties = { node.Properties with Anchor = None } }
    testList "Anchors, aliases, explicit tags, directives and merge keys" [
        testCase "an anchor is a decode error at the anchored node with the place of the anchor, also when no alias names it" <| fun _ ->
            // The tool has three lines in front of the lines below.
            for (name, lines, path, anchor, line, column) in
                [ "a scalar", [ "doc: &d greeting" ], "doc", "&d", 4, 6
                  "a mapping", [ "ex:meta: &m {a: 1}" ], "ex:meta", "&m", 4, 10
                  "a sequence", [ "arguments: &c [echo]" ], "arguments", "&c", 4, 12
                  "an item of a list", [ "arguments: [&i a]" ], "arguments[0]", "&i", 4, 13
                  "a key", [ "ex:x: {&k a: 1}" ], "ex:x.a", "&k", 4, 8
                  "a node below a later field", [ "label: x"; "ex:y: {b: &z 1}" ], "ex:y.b", "&z", 5, 11 ] do
                let error = errorOf name (fun () -> Decode.decodeCommandLineTool (tool (lines @ [ "inputs: []"; "outputs: []" ])) |> ignore)
                Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the anchored node" name)
                Expect.stringContains error.Message ("The anchor " + anchor + " is not allowed. Schema Salad v1.2 prohibits anchors and aliases in a document.") (sprintf "%s: the error should name the anchor and the rule" name)
                Expect.equal (startOf error) (Some (line, column)) (sprintf "%s: the error should stand at the anchor" name)
        testCase "an alias after its anchor is rejected at the anchor, and an alias in a tree without the anchor is rejected at the alias" <| fun _ ->
            let yaml = tool [ "doc: &d greeting"; "label: *d"; "inputs: []"; "outputs: []" ]
            failsAt "the anchor comes first" (fun () -> Decode.decodeCommandLineTool yaml |> ignore) "doc" "The anchor &d is not allowed"
            let tree = withoutAnchors (Reader.readRoot yaml)
            failsAt "the alias of a tree" (fun () -> Decode.decodeCWLProcessingUnitElement tree |> ignore) "label" "The alias *d is not allowed. Schema Salad v1.2 prohibits anchors and aliases in a document."
        testCase "an explicit tag is a decode error at the tagged node with the place of the tag, the non-specific tag included" <| fun _ ->
            for (name, lines, path, tag, line, column) in
                [ "a core tag on a string", [ "doc: !!str greeting" ], "doc", "!!str", 4, 6
                  "a core tag on an int", [ "ex:n: !!int 5" ], "ex:n", "!!int", 4, 7
                  "a core tag on a quoted string", [ "doc: !!str \"greeting\"" ], "doc", "!!str", 4, 6
                  "a local tag", [ "ex:n: !custom {a: 1}" ], "ex:n", "!custom", 4, 7
                  "a verbatim tag", [ "ex:n: !<tag:x,2000:y> a" ], "ex:n", "!<tag:x,2000:y>", 4, 7
                  "the non-specific tag", [ "doc: ! greeting" ], "doc", "!", 4, 6
                  "a tag on a key", [ "ex:x: {!!str a: 1}" ], "ex:x.a", "!!str", 4, 8 ] do
                let error = errorOf name (fun () -> Decode.decodeCommandLineTool (tool (lines @ [ "inputs: []"; "outputs: []" ])) |> ignore)
                Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the tagged node" name)
                Expect.stringContains error.Message ("The tag " + tag + " is not allowed. Schema Salad v1.2 prohibits explicit node tags in a document.") (sprintf "%s: the error should name the tag and the rule" name)
                Expect.equal (startOf error) (Some (line, column)) (sprintf "%s: the error should stand at the tag" name)
        testCase "a %YAML or %TAG directive is a decode error at the root with the place of the directive, and a reserved directive is read" <| fun _ ->
            let body = tool [ "inputs: []"; "outputs: []" ]
            let yamlDirective = errorOf "a %YAML directive" (fun () -> Decode.decodeCommandLineTool ("%YAML 1.2\n---\n" + body) |> ignore)
            Expect.equal (DecodeError.pathText yamlDirective) "" "The error stands at the root."
            Expect.stringContains yamlDirective.Message "The directive %YAML 1.2 is not allowed. Schema Salad v1.2 prohibits %YAML and %TAG directives in a document." "The error names the directive and the rule."
            Expect.equal (startOf yamlDirective) (Some (1, 1)) "The error stands at the directive."
            let tagDirective = errorOf "a %TAG directive" (fun () -> Decode.decodeCommandLineTool ("# before\n%TAG ! tag:x,2000:\n---\n" + body) |> ignore)
            Expect.stringContains tagDirective.Message "The directive %TAG ! tag:x,2000: is not allowed" "The error names the directive."
            Expect.equal (startOf tagDirective) (Some (2, 1)) "The error stands at the directive."
            Expect.equal (Decode.decodeCommandLineTool ("%FOO bar\n---\n" + body)).CWLVersion "v1.2" "A reserved directive is read."
        testCase "an alias without an anchor fails in the reader, so its error stands at the root without the line of the reader" <| fun _ ->
            let error = errorOf "an alias without an anchor" (fun () -> Decode.decodeCommandLineTool (tool [ "doc: *nothere"; "inputs: []"; "outputs: []" ]) |> ignore)
            Expect.equal (DecodeError.pathText error) "" "The error stands at the root."
            Expect.stringContains error.Message "*nothere" "The error should name the alias."
            Expect.isFalse (error.Message.StartsWith "Line") "The place is in the span, so the message holds no line."
            Expect.equal (startOf error) (Some (4, 6)) "The error stands at the alias."
        testCase "the features are rejected in a packed document, in a nested run, below a mixin and in the text of an RO-Crate type" <| fun _ ->
            let packed = String.concat "\n" [ "cwlVersion: v1.2"; "$graph:"; "  - class: CommandLineTool"; "    id: main"; "    baseCommand: echo"; "    doc: &d x"; "    inputs: []"; "    outputs: []"; "" ]
            failsAt "a packed document" (fun () -> Decode.decodeCWLDocument packed |> ignore) "$graph[0].doc" "The anchor &d is not allowed"
            failsAt "a tag in a packed document" (fun () -> Decode.decodeCWLDocument (packed.Replace("&d x", "!!str x")) |> ignore) "$graph[0].doc" "The tag !!str is not allowed"
            let nested = String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run:"; "      class: CommandLineTool"; "      baseCommand: echo"; "      inputs: []"; "      outputs: []"; "      doc: !!str x"; "    in: []"; "    out: []"; "" ]
            failsAt "a nested run" (fun () -> Decode.decodeWorkflow nested |> ignore) "steps.s.run.doc" "The tag !!str is not allowed"
            failsAt "a tag below a mixin" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: {$mixin: t.yml, a: !!int 1}"; "outputs: []" ]) |> ignore) "inputs.x.streamable.a" "The tag !!int is not allowed"
            failsAt "a directive and a mixin" (fun () -> Decode.decodeCommandLineTool ("%YAML 1.2\n---\n" + tool [ "inputs:"; "  x:"; "    type: File"; "    streamable: {$mixin: t.yml}"; "outputs: []" ]) |> ignore) "" "The directive %YAML 1.2 is not allowed"
        testCase "the entry points that take a tree apply the same check as the entry points that take a text" <| fun _ ->
            let yaml = tool [ "doc: !!str greeting"; "inputs: []"; "outputs: []" ]
            failsAt "a process" (fun () -> Decode.decodeCWLProcessingUnitElement (Reader.readRoot yaml) |> ignore) "doc" "The tag !!str is not allowed"
            let packed = String.concat "\n" [ "cwlVersion: v1.2"; "$graph:"; "  - class: CommandLineTool"; "    id: main"; "    baseCommand: echo"; "    doc: &d x"; "    inputs: []"; "    outputs: []"; "" ]
            failsAt "a packed document" (fun () -> Decode.decodeCWLGraphElementWithWarnings None (Reader.readRoot packed) |> ignore) "$graph[0].doc" "The anchor &d is not allowed"
            failsAt "a job file" (fun () -> DecodeParameters.cwlparameterReferenceArrayDecoder (Reader.readRoot "x: &v a\n") |> ignore) "x" "The anchor &v is not allowed"
            let clean = Reader.readRoot (tool [ "inputs: []"; "outputs: []" ])
            Expect.equal (Decode.decodeCWLProcessingUnitElement clean) (Decode.decodeCWLProcessingUnit (tool [ "inputs: []"; "outputs: []" ])) "A tree without a prohibited feature decodes as its text."
        testCase "the characters of the features are legal where they are no features" <| fun _ ->
            let accepted (name: string) (lines: string list) =
                try Decode.decodeCommandLineTool (tool (lines @ [ "inputs: []"; "outputs: []" ])) |> ignore
                with YamlDecodeException error -> failwith (sprintf "%s must decode, found %s" name (DecodeError.text error))
            accepted "quoted characters" [ "doc: \"!x &x *x %YAML 1.2\""; "label: '&x'"; "ex:q: \"*x\"" ]
            accepted "plain scalars with the characters inside" [ "doc: a&b"; "label: a*b"; "ex:p: a!b %c" ]
            accepted "a block scalar with lines that look like features" [ "doc: |"; "  &x"; "  *y"; "  !z"; "  %YAML 1.2"; "label: x" ]
            accepted "a document with a shebang and the markers of one document" [ "label: x" ]
            Expect.equal (Decode.decodeCommandLineTool ("#!/usr/bin/env cwl-runner\n---\n" + tool [ "inputs: []"; "outputs: []"; "..." ])).CWLVersion "v1.2" "A shebang and the markers of a single document are read."
            accepted "the keys of the directives of Schema Salad" [ "$namespaces: {ex: http://example.org/}"; "$schemas: [s.rdf]"; "ex:r: {$import: r.yml}" ]
        testCase "a << key is an ordinary key, plain or quoted, and its entry is an extension field that round trips" <| fun _ ->
            for key in [ "<<"; "\"<<\"" ] do
                let decoded = Decode.decodeCommandLineTool (tool [ "$namespaces: {ex: http://example.org/}"; key + ": {a: 1}"; "ex:marker: <<"; "inputs: []"; "outputs: []" ])
                let written = Encode.encodeToolDescription decoded
                Expect.stringContains written "\"<<\":" (sprintf "%s: the key is written in quotes.\n%s" key written)
                Expect.stringContains written "ex:marker: \"<<\"" (sprintf "%s: the value is a string written in quotes." key)
                Expect.equal (Decode.decodeCommandLineTool written) decoded (sprintf "%s: the written text should decode to the same tool." key)
            let mapKey = Decode.decodeCommandLineTool (tool [ "inputs:"; "  <<: string"; "outputs: []" ])
            Expect.equal (ListEntry.values mapKey.Inputs.Value).[0].Name "<<" "A << key of an id map is the name of the entry."
    ]

let testEqualityAndHashOfRawAndImportedCases =
    let raw (text: string) = { Node = Reader.readRoot text }
    // Two values of one case from equal nodes are equal and hash alike, and a changed node makes another value.
    let check (name: string) (make: RawYaml -> 'T) =
        let first = make (raw "{class: {$import: c.yml}, a: [1, 2]}")
        let second = make (raw "{class: {$import: c.yml}, a: [1, 2]}")
        Expect.equal first second (sprintf "%s: equal nodes should make equal values." name)
        Expect.equal (hash first) (hash second) (sprintf "%s: equal values should hash alike." name)
        for (change, text) in [ "a scalar", "{class: {$import: c.yml}, a: [1, 3]}"; "a key", "{class: {$import: c.yml}, b: [1, 2]}"; "a quote", "{class: {$import: c.yml}, a: [1, \"2\"]}"; "the directive", "{class: {$import: d.yml}, a: [1, 2]}" ] do
            Expect.notEqual first (make (raw text)) (sprintf "%s: %s should change the value." name change)
    testList "The raw and imported cases compare and hash by their nodes and paths" [
        testCase "a process whose class is a directive, in a document and in a run" <| fun _ ->
            check "RawProcess" (fun node -> RawProcess node)
            check "RunRaw" (fun node -> RunRaw node)
        testCase "a type whose type is a directive" <| fun _ ->
            check "RawType" (fun node -> CWLType.RawType node)
        testCase "an item and an entry of the listing" <| fun _ ->
            check "RawItem" (fun node -> RawItem node)
            check "MixedItem" (fun node -> MixedItem node)
            check "RawEntry" (fun node -> RawEntry node)
            Expect.equal (ImportedItem "a.yml") (ImportedItem "a.yml") "Equal paths should make equal items."
            Expect.equal (hash (ImportedItem "a.yml")) (hash (ImportedItem "a.yml")) "Equal items should hash alike."
            Expect.notEqual (ImportedItem "a.yml") (ImportedItem "b.yml") "Another path should make another item."
            Expect.notEqual (ImportedItem "a.yml") (MixedItem (raw "a.yml")) "The cases of an item differ whatever the text."
        testCase "a requirement and a hint whose class is a directive" <| fun _ ->
            check "RawRequirement" (fun node -> RawRequirement node)
            check "RawHint" (fun node -> RawHint node)
    ]

let main =
    testList "CWLToolDescription" [
        testCWLToolDescriptionDecode
        testCWLToolDescriptionEncode
        testEncodeNormalizeEdgeCases
        testCWLToolDescriptionMetadata
        testToolOutputBindingExtensions
        testNestedArrayDecoding
        testUnionArrayItemDecoding
        testStringScalars
        testToolUnionPorts
        testAnyType
        testStreamTypes
        testYamlReaderAndWriter
        testDocAndLabelLayout
        testStrictStrings
        testScalarsThatCwltoolReadsAsAnotherType
        testDirectivesInStringFields
        testEqualityOfDecodedObjects
        testExpressionTool
        testOperation
        testTopLevelDirectDynamicOverflow
        testPortSchemaNesting
        testArrayShorthand
        testFileAndDirectoryTypes
        testDirentIsNoType
        testSchemaSaladDirectivesInDocuments
        testImportedSchemaEntries
        testEqualityAndHashOfPorts
        testEqualityAndHashOfProcesses
        testMixinDirectives
        testDirectivesInExitCodes
        testDirectivesInSoftwareAndEnvironment
        testRepeatedEnumSymbols
        testSchemasField
        testKeysAndIdsThatCwltoolReadsAsAnotherType
        testDirectivesInBoolIntAndEnumFields
        testSmallRejectionsOfCwltool
        testIntsBeyond64BitsInNumberPositions
        testDirectivesInFileAndDirectoryStrings
        testDirectivesAsWholeBindings
        testDirectivesAsClassOrType
        testProcessWithDirectiveClass
        testClassFieldOfMapEntries
        testRawObjectsAndMixinObjects
        testDirentFieldsBesideAClass
        testTextWithoutAnExpressionInExpressionPositions
        testProhibitedYamlFeatures
        testAdviceForNonTextScalarsInExpressionPositions
        testOneWriterForTheEntriesOfAList
        testSecondaryFilePatternsAndNumberPositions
        testDirectivesAsItemsOfAListingArray
        testSchemaDefinitionsWithMixins
        testWordsInNumberPositions
        testSecondaryFileRequiredField
        testLowestLong
        testEqualityAndHashOfRawAndImportedCases
        testInvalidDocuments
    ]

