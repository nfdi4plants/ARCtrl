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
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef("DOTNET_NOLOGO", SchemaSaladString.Literal "true") |])
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
                    let expected = Some (InputBinding.create(position = BindingPosition.Number 1))
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
                    let expected = Some (InputBinding.create(position = BindingPosition.Number 2))
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
                    let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/.nuget")))
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
                    let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/*.csv")))
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
                let outputBinding = Expect.wantSome output.OutputBinding "Output binding should decode."
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
                let expected = EnvVarRequirement (ListEntry.ofValues [| EnvironmentDef("DOTNET_NOLOGO", SchemaSaladString.Literal "true") |])
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
                    let expected = Some (InputBinding.create(position = BindingPosition.Number 1))
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
                    let expected = Some (InputBinding.create(position = BindingPosition.Number 2))
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
                    let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/.nuget")))
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
                    let expected = Some (OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "$(runtime.outdir)/*.csv")))
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
                Expect.equal decoded.Stdin (Some (SchemaSaladString.Literal "stdin.txt")) "stdin should decode to a typed field."
                Expect.equal decoded.Stdout (Some (SchemaSaladString.Literal "stdout.txt")) "stdout should decode to a typed field."
                Expect.equal decoded.Stderr (Some (SchemaSaladString.Literal "stderr.txt")) "stderr should decode to a typed field."
                Expect.sequenceEqual decoded.SuccessCodes.Value (ResizeArray [| 0 |]) "successCodes should decode to a typed field."
                Expect.sequenceEqual decoded.TemporaryFailCodes.Value (ResizeArray [| 75 |]) "temporaryFailCodes should decode to a typed field."
                Expect.sequenceEqual decoded.PermanentFailCodes.Value (ResizeArray [| 1; 2 |]) "permanentFailCodes should decode to a typed field."
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
                YAMLicious.Encode.object [ "steps", YAMLicious.Encode.object [ Encode.encodeWorkflowStep false step ] ]
                |> Encode.writeYaml

            let decodedSteps =
                encodedYaml
                |> YAMLicious.Reader.readRoot
                |> Decode.stepsDecoder
                |> ListEntry.values

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
  y: [File, "null"]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ String; Null ]))) "The input type keeps the order of the list."
            Expect.equal (ListEntry.values decoded.Outputs).[0].Type_ (Some (Union (ResizeArray [ CWLType.File; Null ]))) "The output type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded """x: [string, "null"]""" "The input is written as a list."
            Expect.stringContains encoded """y: [File, "null"]""" "The output is written as a list."
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
  y: ["null", File]
"""
            let decoded = Decode.decodeCommandLineTool yaml
            Expect.equal (ListEntry.values decoded.Inputs.Value).[0].Type_ (Some (Union (ResizeArray [ Null; String ]))) "The input type keeps the order of the list."
            let encoded = Encode.encodeToolDescription decoded
            Expect.stringContains encoded "x: string?" "The input is written as the shorthand."
            Expect.stringContains encoded "y: File?" "The output is written as the shorthand."
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
                tool.Inputs |> Option.map ListEntry.values |> Option.bind Seq.tryHead |> Option.bind (fun i -> i.InputBinding) |> Option.bind (fun b -> b.ValueFrom)
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
                                | Some (Record schema) -> (InputRecordSchema.getFieldsOrEmpty schema).[0]
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
            let fields = match (ListEntry.values decoded.Inputs.Value).[0].Type_ with | Some (Record schema) -> (InputRecordSchema.getFieldsOrEmpty schema) | other -> failwithf "Expected a record, found %A" other
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
                    | Record schema -> (InputRecordSchema.getFieldsOrEmpty schema)
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
                Expect.equal (InputRecordSchema.getFieldsOrEmpty schema).[0].Type (optionalOf (arrayOf (CWLType.File))) "A record field reads File[]? as a type."
                Expect.equal (InputRecordSchema.getFieldsOrEmpty schema).[1].Type (arrayOf CWLType.String) "A record field reads string[] in its type field."
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
    @ (WorkflowStep.getInputs step |> Seq.map (fun input -> "step input", box input) |> Seq.toList)
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
                  "a doc under !!str that reads as a bool without the tag",
                  "doc: !!str true\n",
                  (fun (tool: CWLToolDescription) -> docTextOf tool.Doc),
                  "true"
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
                for text in [ "\"42\""; "'true'"; "!!str 1.5"; "|\n  42" ] do
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

let private stepInputsOf (text: string) : StepInput list = WorkflowStep.getInputs (stepOf text) |> Seq.toList

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
                | Workflow workflow -> (ListEntry.values workflow.Outputs) |> Seq.toList
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
                    inputs = ListEntry.ofValues [ CWLInput("mode", enumSchema, inputBinding = InputBinding.create(prefix = (SchemaSaladString.Literal "--mode"))) ],
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
                    inputs = ListEntry.ofValues (ResizeArray [ CWLInput("a", Array labelled); CWLInput("b", Union (ResizeArray [ Null; Array labelled ]), optional = true); CWLInput("c", Array (InputArraySchema(CWLType.String))) ]),
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
                Expect.stringContains error.Message "Expected a string or a mapping with the single key $include or $import, found a mapping" "The error says what is wrong."
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
            Expect.equal (ListEntry.values (Decode.decodeWorkflow (withBinding workflowHeader [ "loadContents: true" ])).Inputs).[0].InputBinding.Value.LoadContents (Some true) "A workflow input may load contents."
            Expect.equal (ListEntry.values (Decode.decodeExpressionTool (withBinding expressionHeader [ "loadContents: true" ])).Inputs.Value).[0].InputBinding.Value.LoadContents (Some true) "An expression tool input may load contents."
            Expect.equal (ListEntry.values (Decode.decodeCommandLineTool (withBinding commandLineHeader [ "position: 1"; "prefix: -x" ])).Inputs.Value).[0].InputBinding.Value.Position (Some (BindingPosition.Number 1)) "A command line tool input has the full binding."
        testCase "the loadListing of a step input is a symbol of CWL" <| fun _ ->
            let step (loadListing: string) = stepWith [ "    in:"; "      x:"; "        default: 1"; "        loadListing: " + loadListing; "    out: []" ]
            expectError "a word" "steps.s.in.x.loadListing" "Expected one of no_listing, shallow_listing and deep_listing" (fun () -> Decode.decodeWorkflow (step "foo") |> ignore)
            Expect.equal (WorkflowStep.getInputs (ListEntry.values (Decode.decodeWorkflow (step "deep_listing")).Steps).[0]).[0].LoadListing (Some LoadListingEnum.DeepListing) "A symbol is typed."
        testCase "outputSource belongs to the output of a workflow only" <| fun _ ->
            let toolOutput = tool [ "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: s/o" ]
            expectError "a tool" "outputs.o.outputSource" "An output of a command line tool has no outputSource" (fun () -> Decode.decodeCommandLineTool toolOutput |> ignore)
            expectError "an expression tool" "outputs.o.outputSource" "An output of an expression tool has no outputSource" (fun () ->
                Decode.decodeExpressionTool (String.concat "\n" [ "cwlVersion: v1.2"; "class: ExpressionTool"; "inputs: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: s/o"; "expression: $(1)"; "" ]) |> ignore)
            let decoded = Decode.decodeWorkflow (workflow [ "inputs:"; "  x: string"; "outputs:"; "  o:"; "    type: string"; "    outputSource: x"; "steps: {}" ])
            Expect.equal (ListEntry.values decoded.Outputs).[0].OutputSource (Some (OutputSource.Single "x")) "A workflow output names its source."
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
            Expect.equal (ListEntry.values decoded.Outputs).[0].OutputBinding (Some (OutputBinding(glob = OutputGlob.Single (SchemaSaladString.Literal "a")))) "A tool output binds files."
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
            Expect.equal (ListEntry.values decoded.Outputs).[0].OutputSource (Some (OutputSource.Multiple (ResizeArray [ "x"; "x" ]))) "A workflow output names its sources."
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "linkMerge: merge_flattened" "The workflow output keeps its linkMerge."
            Expect.stringContains written "pickValue: all_non_null" "The workflow output keeps its pickValue."
            Expect.equal (Decode.decodeWorkflow written) decoded "The written text should decode to the same workflow."
        testCase "an id that occurs twice in the list form of step inputs is a warning and both entries are written back as a list" <| fun _ ->
            let yaml = workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      - id: a"; "        source: x"; "      - id: b"; "        source: y"; "      - id: a"; "        source: [y, z]"; "    out: []" ]
            let decoded = Decode.decodeWorkflowWithWarnings yaml
            Expect.equal (WorkflowStep.getInputs (ListEntry.values decoded.Value.Steps).[0] |> Seq.map (fun input -> input.Id) |> List.ofSeq) [ "a"; "b"; "a" ] "Every entry stays in the list, in order."
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
            Expect.equal ((ListEntry.values steps.Value.Steps) |> Seq.map (fun step -> step.Id, step.Run) |> List.ofSeq) [ "s", RunString "t.cwl"; "s", RunString "u.cwl" ] "Both steps stay in the list."
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
            // The path of the error, the document with {0} where the scalar stands, and whether the quoted spelling decodes.
            let cases =
                [ "outputs.o.outputBinding.glob", output "outputBinding: {glob: {0}}", true
                  "outputs.o.outputBinding.glob[1]", output "outputBinding: {glob: [a, {0}]}", true
                  "outputs.o.outputBinding.outputEval", output "outputBinding: {outputEval: {0}}", true
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
                  "steps.s.when", step [ "run: t.cwl"; "in: {}"; "out: []"; "when: {0}" ], true
                  "steps.s.out[0]", step [ "run: t.cwl"; "in: {}"; "out: [{0}]" ], true
                  "steps.s.run", step [ "run: {0}"; "in: {}"; "out: []" ], true
                  "steps.s.in.x", step [ "run: t.cwl"; "in: {x: {0}}"; "out: []" ], true
                  "steps.s.in.x.source", step [ "run: t.cwl"; "in: {x: {source: {0}}}"; "out: []" ], true
                  "steps.s.in.x.source[1]", step [ "run: t.cwl"; "in: {x: {source: [a, {0}]}}"; "out: []" ], true
                  "steps.s.in.x.valueFrom", step [ "run: t.cwl"; "in: {x: {valueFrom: {0}}}"; "out: []" ], true
                  "outputs.o.outputSource", workflow [ "inputs: {}"; "steps: {}"; "outputs:"; "  o:"; "    type: string"; "    outputSource: {0}" ], true ]
            for (path, template, quotedDecodes) in cases do
                for text in [ "5"; "true"; "1.5" ] do
                    match decodeError (template.Replace("{0}", text)) with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) path (sprintf "The error for %s should show the field:\n%s" text template)
                        Expect.stringContains error.Message "Expected a string, found the plain scalar" (sprintf "The error for %s at %s should say what CWL expects." text path)
                        Expect.isSome error.Span (sprintf "The error for %s at %s should have the place in the text." text path)
                    | None -> failwith (sprintf "A plain %s at %s must fail:\n%s" text path template)
                if quotedDecodes then
                    Expect.isNone (decodeError (template.Replace("{0}", "\"5\""))) (sprintf "A quoted 5 at %s is a string." path)
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
                        Expect.stringContains error.Message "Expected a string, found the plain scalar" (sprintf "The error for %s at %s should say what CWL expects." text path)
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
          "the doc of a step input", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        source: x"; "        doc: {0}"; "    out: []" ], (fun yaml -> textOf (WorkflowStep.getInputs (stepOf yaml)).[0].Doc)
          "the label of a step input", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        source: x"; "        label: {0}"; "    out: []" ], (fun yaml -> (WorkflowStep.getInputs (stepOf yaml)).[0].Label)
          "the doc of a record field", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: record"; "      fields:"; "        f:"; "          type: string"; "          doc: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Record r -> textOf (InputRecordSchema.getFieldsOrEmpty r).[0].Doc | _ -> None)
          "the label of a record", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: record"; "      fields: {}"; "      label: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Record r -> r.Label | _ -> None)
          "the doc of an enum", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: enum"; "      symbols: [a]"; "      doc: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Enum e -> textOf e.Doc | _ -> None)
          "the label of an array", tool [ "baseCommand: echo"; "inputs:"; "  x:"; "    type:"; "      type: array"; "      items: string"; "      label: {0}"; "outputs: {}" ], (fun yaml -> match schemaOf yaml with Array a -> a.Label | _ -> None)
          "the stdin of a tool", toolWith [ "stdin: {0}" ], (fun yaml -> (toolOf yaml).Stdin)
          "the stdout of a tool", toolWith [ "stdout: {0}" ], (fun yaml -> (toolOf yaml).Stdout)
          "the stderr of a tool", toolWith [ "stderr: {0}" ], (fun yaml -> (toolOf yaml).Stderr)
          "a baseCommand", tool [ "baseCommand: {0}"; "inputs: {}"; "outputs: {}" ], (fun yaml -> (toolOf yaml).BaseCommand.Value |> Seq.tryHead)
          "an entry of the baseCommand", tool [ "baseCommand: [echo, {0}]"; "inputs: {}"; "outputs: {}" ], (fun yaml -> Some (toolOf yaml).BaseCommand.Value.[1])
          "the prefix of an input binding", port [ "inputBinding: {prefix: {0}}" ], (fun yaml -> (inputOf yaml).InputBinding.Value.Prefix)
          "the itemSeparator of an input binding", port [ "inputBinding: {itemSeparator: {0}}" ], (fun yaml -> (inputOf yaml).InputBinding.Value.ItemSeparator)
          "the valueFrom of an input binding", port [ "inputBinding: {valueFrom: {0}}" ], (fun yaml -> (inputOf yaml).InputBinding.Value.ValueFrom)
          "the valueFrom of a step input", workflow [ "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run: t.cwl"; "    in:"; "      a:"; "        valueFrom: {0}"; "    out: []" ], (fun yaml -> (WorkflowStep.getInputs (stepOf yaml)).[0].ValueFrom)
          "the format of an input", port [ "format: {0}" ], (fun yaml -> match (inputOf yaml).Format with Some (InputFormat.Single format) -> Some format | _ -> None)
          "a format in the list of an input", port [ "format: [{0}]" ], (fun yaml -> match (inputOf yaml).Format with Some (InputFormat.Multiple formats) -> Some formats.[0] | _ -> None)
          "the format of an output", output [ "format: {0}" ], (fun yaml -> (outputOf yaml).Format)
          "the outputEval of an output binding", output [ "outputBinding: {glob: a, outputEval: {0}}" ], (fun yaml -> (outputOf yaml).OutputBinding.Value.OutputEval)
          "the glob of an output binding", output [ "outputBinding: {glob: {0}}" ], (fun yaml -> match (outputOf yaml).OutputBinding.Value.Glob with Some (OutputGlob.Single glob) -> Some glob | _ -> None)
          "a glob in the list of an output binding", output [ "outputBinding: {glob: [a, {0}]}" ], (fun yaml -> match (outputOf yaml).OutputBinding.Value.Glob with Some (OutputGlob.Multiple globs) -> Some globs.[1] | _ -> None)
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
            Expect.equal (InputRecordSchema.getFieldsOrEmpty (recordOf mixedRecord) |> Seq.map (fun field -> field.Name) |> List.ofSeq) [ "f" ] "Only the field is an object."
            Expect.equal (List.ofSeq (InputEnumSchema.getSymbols (enumOf mixedEnum))) [ "a"; "b" ] "Only the plain symbols are strings."
            Expect.equal (InputRecordSchema.getFieldsOrEmpty (InputRecordSchema())).Count 0 "A record without fields has none."
            let fields = InputRecordSchema.getFieldsOrEmpty (recordOf mixedRecord)
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
                  "an include as the whole symbols", tool [ "type:"; "  type: enum"; "  symbols: {$include: p.yml}" ], "inputs.a.type.symbols", "An $include yields the text of a file, but the symbols field of an enum takes an object."
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
    let stepInputOf (text: string) = (WorkflowStep.getInputs (ListEntry.values (Decode.decodeWorkflow text).Steps).[0]).[0]
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
                let file = FileInstance(location = "l", path = "p", basename = "b", dirname = "d", nameroot = "r", nameext = ".e", checksum = "c", size = 1L, secondaryFiles = { Node = YAMLicious.Reader.readRoot "s" }, format = "f", contents = "t")
                DynObj.setProperty "arc:note" (box "n1") file
                file
            let edits : (string * (FileInstance -> unit)) list =
                [ "location", fun f -> f.Location <- Some "l2"
                  "path", fun f -> f.Path <- Some "p2"
                  "basename", fun f -> f.Basename <- Some "b2"
                  "dirname", fun f -> f.Dirname <- Some "d2"
                  "nameroot", fun f -> f.Nameroot <- Some "r2"
                  "nameext", fun f -> f.Nameext <- Some ".e2"
                  "checksum", fun f -> f.Checksum <- Some "c2"
                  "size", fun f -> f.Size <- Some 2L
                  "secondaryFiles", fun f -> f.SecondaryFiles <- Some { Node = YAMLicious.Reader.readRoot "s2" }
                  "format", fun f -> f.Format <- Some "f2"
                  "contents", fun f -> f.Contents <- Some "t2"
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
        testCase "a port with an id and a mixin is a port without the fields that the file supplies, in a list and in a map" <| fun _ ->
            let listed = tool [ "inputs: [{id: x, $mixin: mix.yml}]"; "outputs: [{id: o, $mixin: out.yml}]" ]
            let mapped = tool [ "inputs:"; "  x: {$mixin: mix.yml}"; "outputs:"; "  o: {$mixin: out.yml}" ]
            for (name, text) in [ "list", listed; "map", mapped ] do
                let decoded = Decode.decodeCommandLineTool text
                let input = (ListEntry.values decoded.Inputs.Value).[0]
                Expect.equal input.Name "x" (sprintf "%s: the input keeps its id" name)
                Expect.isNone input.Type_ (sprintf "%s: the file supplies the type" name)
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "$mixin" input) (Some "mix.yml") (sprintf "%s: the mixin stays an extension field of the object" name)
                Expect.isNone (ListEntry.values decoded.Outputs).[0].Type_ (sprintf "%s: the output has no type either" name)
                expectRoundTrip name text [ "$mixin: mix.yml"; "$mixin: out.yml" ]
            let typed = Decode.decodeCommandLineTool (tool [ "inputs: [{id: x, type: int, $mixin: mix.yml}]"; "outputs: []" ])
            Expect.equal (ListEntry.values typed.Inputs.Value).[0].Type_ (Some CWLType.Int) "The type beside the mixin stays."
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
            match (ListEntry.values (inputsOf types)).[0].Type_, (ListEntry.values (inputsOf types)).[1].Type_ with
            | Some (MixedType _), Some (Union members) when members.Count = 2 && (match members.[1] with MixedType _ -> true | _ -> false) -> ()
            | other -> failwithf "Expected mixed types, found %A" other
            expectRoundTrip "types" types [ "type: {$mixin: t.yml}"; "{$mixin: t.yml}" ]
            let strings = tool [ "doc: {$mixin: d.md}"; "label: {$mixin: l.yml}"; "inputs: []"; "outputs: []" ]
            let decodedStrings = Decode.decodeCommandLineTool strings
            Expect.equal decodedStrings.Doc (Some (DocText.Text (SchemaSaladString.Mixin "d.md"))) "The doc is the mixin."
            Expect.equal decodedStrings.Label (Some (SchemaSaladString.Mixin "l.yml")) "The label is the mixin."
            expectRoundTrip "strings" strings [ "doc: {$mixin: d.md}"; "label: {$mixin: l.yml}" ]
            let requirements = tool [ "requirements:"; "  - {$mixin: req.yml}"; "  - {$mixin: body.yml, class: NetworkAccess}"; "hints:"; "  - {$mixin: hint.yml}"; "  NetworkAccess: {$mixin: body.yml}"; "inputs: []"; "outputs: []" ]
            expectRoundTrip "requirements" (tool [ "requirements:"; "  - {$mixin: req.yml}"; "  - {$mixin: body.yml, class: NetworkAccess}"; "hints:"; "  - {$mixin: hint.yml}"; "inputs: []"; "outputs: []" ]) [ "$mixin: req.yml"; "$mixin: body.yml"; "$mixin: hint.yml" ]
            let mapped = Decode.decodeCommandLineTool (tool [ "requirements:"; "  NetworkAccess: {$mixin: body.yml}"; "hints:"; "  NetworkAccess: {$mixin: body.yml}"; "inputs: []"; "outputs: []" ])
            match mapped.Requirements.Value.[0], mapped.Hints.Value.[0] with
            | MixedRequirement _, MixedHint _ -> ()
            | other -> failwithf "Expected a mixed requirement and a mixed hint, found %A" other
            ignore requirements
        testCase "a mixin stands for a field of a record, a symbol, an environment variable, a step input and an output of a step" <| fun _ ->
            expectRoundTrip "fields" (tool [ "inputs:"; "  a:"; "    type:"; "      type: record"; "      fields: [{$mixin: field.yml}, {name: f, $mixin: mix.yml}]"; "outputs: []" ]) [ "$mixin: field.yml"; "name: f" ]
            expectRoundTrip "symbols" (tool [ "inputs:"; "  a:"; "    type:"; "      type: enum"; "      symbols: [x, {$mixin: sym.yml}]"; "outputs: []" ]) [ "{$mixin: sym.yml}" ]
            expectRoundTrip "envDef" (tool [ "requirements:"; "  EnvVarRequirement:"; "    envDef: [{$mixin: env.yml}, {envName: A, $mixin: value.yml}]"; "inputs: []"; "outputs: []" ]) [ "$mixin: env.yml"; "envName: A" ]
            let step = workflow [ "  s:"; "    run: t.cwl"; "    in: [{$mixin: in.yml}, {id: a, $mixin: more.yml}]"; "    out: [{$mixin: out.yml}, {id: o, $mixin: more.yml}]" ]
            let decoded = Decode.decodeWorkflow step
            let first = (ListEntry.values decoded.Steps).[0]
            Expect.equal first.In.Count 2 "Both step inputs stay."
            Expect.equal (WorkflowStep.getInputs first |> Seq.map (fun input -> input.Id) |> List.ofSeq) [ "a" ] "The step input with an id is typed."
            Expect.equal (first.Out |> Seq.map StepOutput.tryId |> List.ofSeq) [ None; Some "o" ] "The output with an id is typed."
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "$mixin: in.yml" "The step input mixin is written back."
            Expect.stringContains written "$mixin: out.yml" "The output mixin is written back."
            Expect.equal (Decode.decodeWorkflow written) decoded "The written text should decode to the same workflow."
        testCase "a mixin cannot stand for the whole of a list, because it completes an object" <| fun _ ->
            failsAt "inputs" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: {$mixin: ports.yml}"; "outputs: []" ]) |> ignore) "inputs" "A $mixin completes an object with the fields beside it, and the inputs field is no object."
            failsAt "requirements" (fun () -> Decode.decodeCommandLineTool (tool [ "requirements: {$mixin: reqs.yml}"; "inputs: []"; "outputs: []" ]) |> ignore) "requirements" "A $mixin completes an object with the fields beside it, and the requirements field is no object."
            failsAt "steps" (fun () -> Decode.decodeWorkflow (workflow [ "  $mixin: steps.yml" ]) |> ignore) "steps" "A $mixin completes an object with the fields beside it, and the steps field is no object."
        testCase "a mixin beside a field of a string and a mixin with a bad path are decode errors" <| fun _ ->
            failsAt "a string" (fun () -> Decode.decodeCommandLineTool (tool [ "doc: {$mixin: d.md, extra: 1}"; "inputs: []"; "outputs: []" ]) |> ignore) "doc" "A $mixin beside other fields completes an object, but this place takes a string."
            failsAt "a number" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: [{id: x, $mixin: 42}]"; "outputs: []" ]) |> ignore) "inputs[0].$mixin" "Expected the path of the $mixin directive to be a string"
            failsAt "an import beside a mixin" (fun () -> Decode.decodeCommandLineTool (tool [ "inputs: [{$import: a.yml, $mixin: b.yml}]"; "outputs: []" ]) |> ignore) "inputs[0]" "A directive has exactly one field"
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
        testInvalidDocuments
    ]

