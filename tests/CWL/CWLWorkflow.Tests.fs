module Tests.CWLWorkflow

open ARCtrl.CWL
open DynamicObj
open TestingUtils
open TestingUtils.CWL
open YAMLicious

let decodeCWLWorkflowDescription: CWLWorkflowDescription =
    TestObjects.CWL.Workflow.workflowFile
    |> Decode.decodeWorkflow

let tryYamlScalarString (y: RawYaml) =
    match y.Node.Kind with
    | YamlNodeKind.Scalar scalar -> Some scalar.Value
    | _ -> None

let tryYamlScalarSequence (y: RawYaml) =
    match y.Node.Kind with
    | YamlNodeKind.Sequence collection ->
        collection.Items
        |> List.choose (fun item ->
            match item.Node.Kind with
            | YamlNodeKind.Scalar scalar -> Some scalar.Value
            | _ -> None)
        |> ResizeArray
        |> Some
    | _ -> None

let renderYamlElement (element: YamlNode) =
    Encode.writeYaml element

let renderYamlPair pair =
    YAMLicious.Encode.object [ pair ]
    |> Encode.writeYaml

let mkStepInput id source linkMerge =
    StepInput.create(id, ?source = source, ?linkMerge = linkMerge)

/// The error that a decode raises, and none when it succeeds.
let decodeError (decode: unit -> unit) =
    try
        decode ()
        None
    with YamlDecodeException error -> Some error

let testCWLWorkflowDescriptionDecode =
    testList "Decode" [
        testCase "CWLVersion" <| fun _ ->
            let expected = "v1.2"
            let actual = decodeCWLWorkflowDescription.CWLVersion
            Expect.equal actual expected ""
        testCase "MultipleInputFeatureRequirement" <| fun _ ->
            let requirementsItem = decodeCWLWorkflowDescription.Requirements
            let expected = MultipleInputFeatureRequirement
            let actual = requirementsItem.Value.[0]
            Expect.equal actual expected ""
        testCase "inputs" <| fun _ ->
            let expected = ResizeArray [|
                CWLInput("cores", CWLType.Int)
                CWLInput("db", CWLType.File)
                CWLInput ("stage", CWLType.Directory)
                CWLInput ("outputMzML", CWLType.Directory)
                CWLInput ("outputPSM", CWLType.Directory)
                CWLInput ("inputMzML", CWLType.Directory)
                CWLInput ("paramsMzML", CWLType.File)
                CWLInput ("paramsPSM", CWLType.File)
                // Note: sampleRecord has complex type and will be checked separately
            |]
            let actual = decodeCWLWorkflowDescription.Inputs
            // Check the simple inputs (first 8)
            for i = 0 to 7 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].InputBinding expected.[i].InputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
            // Check that sampleRecord exists
            Expect.equal actual.Count 9 "Should have 9 inputs including sampleRecord"
            Expect.equal actual.[8].Name "sampleRecord" ""
        testCase "sampleRecord complex type" <| fun _ ->
            let sampleRecordInput = decodeCWLWorkflowDescription.Inputs.[8]
            Expect.equal sampleRecordInput.Name "sampleRecord" ""
            Expect.isSome sampleRecordInput.Type_ "sampleRecord should have a type"
            match sampleRecordInput.Type_ with
            | Some (Array arraySchema) ->
                match arraySchema.Items with
                | Record recordSchema ->
                    Expect.isSome recordSchema.Fields "record should have fields"
                    Expect.equal recordSchema.Fields.Value.Count 2 "Should have 2 fields: readsOfOneSample and sampleName"
                | _ -> failwithf "Expected Record for array items but got: %A" arraySchema.Items
            | _ -> failwithf "Expected Array for sampleRecord type but got: %A" sampleRecordInput.Type_
        testList "steps" [
            let workflowSteps = decodeCWLWorkflowDescription.Steps
            testList "IDs" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = "MzMLToMzlite"
                    let actual = workflowSteps.[0].Id
                    Expect.equal actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = "PeptideSpectrumMatching"
                    let actual = workflowSteps.[1].Id
                    Expect.equal actual expected ""
            ]
            testList "Run" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = "./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl"
                    let actual = workflowSteps.[0].Run
                    match actual with
                    | RunString runPath -> Expect.equal runPath expected ""
                    | _ -> failwithf "Expected RunString but got %A" actual
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                    let actual = workflowSteps.[1].Run
                    match actual with
                    | RunString runPath -> Expect.equal runPath expected ""
                    | _ -> failwithf "Expected RunString but got %A" actual
            ]
            testList "In" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = ResizeArray [|
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"inputMzML"|])) None
                        mkStepInput "params" (Some (ResizeArray [|"paramsMzML"|])) None
                        mkStepInput "outputDirectory" (Some (ResizeArray [|"outputMzML"|])) None
                        mkStepInput "parallelismLevel" (Some (ResizeArray [|"cores"|])) None
                    |]
                    let actual = workflowSteps.[0].In
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                        Expect.equal
                            (actual.DefaultValue |> Option.bind tryYamlScalarString)
                            (expected.DefaultValue |> Option.bind tryYamlScalarString)
                            ""
                        Expect.equal actual.ValueFrom expected.ValueFrom ""
                        Expect.equal actual.LinkMerge expected.LinkMerge ""
                        Expect.equal actual.PickValue expected.PickValue ""
                        Expect.equal actual.Doc expected.Doc ""
                        Expect.equal actual.LoadContents expected.LoadContents ""
                        Expect.equal actual.LoadListing expected.LoadListing ""
                        Expect.equal actual.Label expected.Label ""
                    ) expected actual
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = ResizeArray [|
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"MzMLToMzlite/dir1"; "MzMLToMzlite/dir2"|])) (Some MergeFlattened)
                        mkStepInput "database" (Some (ResizeArray [|"db"|])) None
                        mkStepInput "params" (Some (ResizeArray [|"paramsPSM"|])) None
                        mkStepInput "outputDirectory" (Some (ResizeArray [|"outputPSM"|])) None
                        mkStepInput "parallelismLevel" (Some (ResizeArray [|"cores"|])) None
                    |]
                    let actual = workflowSteps.[1].In
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                        Expect.equal
                            (actual.DefaultValue |> Option.bind tryYamlScalarString)
                            (expected.DefaultValue |> Option.bind tryYamlScalarString)
                            ""
                        Expect.equal actual.ValueFrom expected.ValueFrom ""
                        Expect.equal actual.LinkMerge expected.LinkMerge ""
                        Expect.equal actual.PickValue expected.PickValue ""
                        Expect.equal actual.Doc expected.Doc ""
                        Expect.equal actual.LoadContents expected.LoadContents ""
                        Expect.equal actual.LoadListing expected.LoadListing ""
                        Expect.equal actual.Label expected.Label ""
                    ) expected actual
            ]
            testList "Out" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = ResizeArray [| StepOutputString "dir" |]
                    let actual = workflowSteps.[0].Out
                    Expect.sequenceEqual actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = ResizeArray [| StepOutputString "dir" |]
                    let actual = workflowSteps.[1].Out
                    Expect.sequenceEqual actual expected ""
            ]
        ]
        testCase "outputs" <| fun _ ->
            let expected = ResizeArray [|
                CWLOutput("mzlite", CWLType.Directory, outputSource = OutputSource.Single "MzMLToMzlite/dir")
                CWLOutput("psm", CWLType.Directory, outputSource = OutputSource.Single "PeptideSpectrumMatching/dir")
            |]
            let actual = decodeCWLWorkflowDescription.Outputs
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].OutputBinding expected.[i].OutputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
                Expect.equal actual.[i].OutputSource expected.[i].OutputSource ""
        testCase "workflow intent decodes as typed field, not metadata overflow" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithIntentFile
            let intent = Expect.wantSome decoded.Intent "Intent should decode on Workflow."
            Expect.sequenceEqual intent (ResizeArray [|"primary-analysis"; "quality-control"|]) ""
            Expect.isNone decoded.Metadata "Intent should not be captured as overflow metadata."
        testCase "extended step fields decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithExtendedStepFile
            let step = decoded.Steps.[0]
            Expect.equal step.Label (Some "Example step") ""
            Expect.equal step.Doc (Some "Step docs") ""
            Expect.sequenceEqual step.Scatter.Value (ResizeArray [|"input1"|]) ""
            Expect.equal step.ScatterMethod (Some DotProduct) ""
            Expect.equal step.In.[0].LoadContents (Some true) ""
            Expect.equal step.In.[0].LoadListing (Some LoadListingEnum.DeepListing) ""
            Expect.equal step.In.[0].Label (Some "Input label") ""
            Expect.equal step.In.[0].LinkMerge (Some MergeNested) ""
            let expectedOut = ResizeArray [| StepOutputRecord (StepOutputParameter.create "out") |]
            Expect.sequenceEqual step.Out expectedOut ""
        testCase "inline run commandline tool decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineRunCommandLineToolFile
            match decoded.Steps.[0].Run with
            | RunCommandLineTool toolObj ->
                match toolObj with
                | :? CWLToolDescription as tool ->
                    Expect.equal tool.CWLVersion "v1.2" ""
                | _ ->
                    failwithf "Expected CWLToolDescription payload but got %A" toolObj
            | other ->
                failwithf "Expected RunCommandLineTool but got %A" other
        testCase "an inline run takes the version of the enclosing process and ignores its own cwlVersion" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.InlineRunVersions.workflowFile
            let step (id: string) = decoded.Steps |> Seq.find (fun s -> s.Id = id)
            let tool (run: WorkflowStepRun) = Expect.wantSome (WorkflowStepRunOps.tryGetTool run) "Expected an inline tool"
            Expect.equal (tool (step "plain").Run).CWLVersion "v1.2" "A run without a version takes the version of the workflow."
            Expect.equal (tool (step "own_version").Run).CWLVersion "v1.2" "A run that names v1.0 uses the version of the workflow."
            let nested = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow (step "nested").Run) "Expected an inline workflow"
            Expect.equal nested.CWLVersion "v1.2" "An inline workflow without a version takes the version of the enclosing one."
            Expect.equal (tool nested.Steps.[0].Run).CWLVersion "v1.2" "The process in it takes that version."
        testCase "a process without a cwlVersion is a decode error when no process encloses it" <| fun _ ->
            let yaml = """class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}"""
            match (try Decode.decodeCommandLineTool yaml |> ignore; None with YamlDecodeException error -> Some error) with
            | Some error ->
                Expect.stringContains error.Message "cwlVersion" "The error names the missing field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A process at the root needs a cwlVersion."
        testCase "the cwlVersion at the root is a decode error at the field when it has no value or holds a list or a mapping" <| fun _ ->
            let tool (field: string) = "cwlVersion:" + field + "\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}"
            let graph (field: string) = "cwlVersion:" + field + "\n$graph:\n  - id: main\n    class: CommandLineTool\n    baseCommand: echo\n    inputs: {}\n    outputs: {}"
            let cases =
                [ "a list", " [v1.2]", None
                  "a mapping", " {version: v1.2}", None
                  "no value", "", Some "Expected a value, found" ]
            for (name, field, message) in cases do
                for (kind, decode) in [ "a process", (fun () -> Decode.decodeCWLDocument (tool field) |> ignore); "a packed document", (fun () -> Decode.decodeCWLDocument (graph field) |> ignore) ] do
                    match decodeError decode with
                    | Some error ->
                        Expect.equal (DecodeError.pathText error) "cwlVersion" $"{name} in {kind}: the error should show the field."
                        message |> Option.iter (fun text -> Expect.stringContains error.Message text $"{name} in {kind}: the error should say that the value is missing.")
                        Expect.isSome error.Span $"{name} in {kind}: the error should have the place in the text."
                    | None -> failwith $"{name} in {kind} must fail"
        testCase "the cwlVersion at the root is read as written and every document decodes under the rules of v1.2" <| fun _ ->
            // A NetworkAccess requirement exists from v1.1, so the documents of v1.0 and v1.3 show that the version selects no rule.
            for version in [ "v1.0"; "v1.1"; "v1.2"; "v1.2.0-dev2"; "v1.3"; "v1.3.0-dev1"; "banana" ] do
                let tool = "cwlVersion: " + version + "\nclass: CommandLineTool\nrequirements:\n  - class: NetworkAccess\n    networkAccess: true\nbaseCommand: echo\ninputs: {}\noutputs: {}"
                let graph = "cwlVersion: " + version + "\n$graph:\n  - id: main\n    class: CommandLineTool\n    requirements:\n      - class: NetworkAccess\n        networkAccess: true\n    baseCommand: echo\n    inputs: {}\n    outputs: {}"
                let decodedTool = Decode.decodeCommandLineTool tool
                Expect.equal decodedTool.CWLVersion version $"A process decodes with {version}."
                Expect.isSome decodedTool.Requirements $"A process of {version} keeps its NetworkAccess requirement."
                Expect.stringContains (Encode.encodeToolDescription decodedTool) ("cwlVersion: " + version) $"A process writes {version} back."
                match Decode.decodeCWLDocument graph with
                | CWLDocument.Graph decoded ->
                    Expect.equal decoded.CWLVersion version $"A packed document decodes with {version}."
                    Expect.stringContains (Encode.encodeCWLDocument (CWLDocument.Graph decoded)) ("cwlVersion: " + version) $"A packed document writes {version} back."
                | other -> failwith $"Expected a packed document but got %A{other}"
        testCase "a number at the root is read as the text of its spelling and written back" <| fun _ ->
            let tool = "cwlVersion: 1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}"
            let decoded = Decode.decodeCommandLineTool tool
            Expect.equal decoded.CWLVersion "1.2" "The number is read as its text."
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)).CWLVersion "1.2" "The text reads back as the same version."
        testCase "the cwlVersion of an inline run changes nothing whatever it holds" <| fun _ ->
            let head = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  s:
    run:
      class: CommandLineTool
      cwlVersion:"""
            let tail = """
      baseCommand: echo
      inputs: {}
      outputs: {}
    in: {}
    out: []"""
            for (name, field) in [ "no value", ""; "an older version", " v1.0"; "a draft", " draft-3"; "the version itself", " v1.2"; "a newer version", " v1.3"; "a text that is no version", " banana" ] do
                let decoded = Decode.decodeWorkflow (head + field + tail)
                let tool = Expect.wantSome (WorkflowStepRunOps.tryGetTool decoded.Steps.[0].Run) "The step runs an inline tool."
                Expect.equal tool.CWLVersion "v1.2" $"{name}: the inline run takes the version of the workflow."
        testCase "step input array form decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInputArrayStepFile
            let stepInput = decoded.Steps.[0].In.[0]
            Expect.equal stepInput.Id "in1" ""
            Expect.sequenceEqual stepInput.Source.Value (ResizeArray [|"input1"|]) ""
            Expect.equal stepInput.Label (Some "Input in array syntax") ""
        testCase "workflow with no steps decodes to empty" <| fun _ ->
            let yaml = TestObjects.CWL.Workflow.workflowWithNoStepsFile
            let decoded = Decode.decodeWorkflow yaml
            Expect.equal decoded.Steps.Count 0 ""
        testCase "sequence-form workflow ports with comments decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithSequencePortsAndCommentsFile
            Expect.equal decoded.Inputs.Count 2 "Sequence-form inputs should decode."
            Expect.equal decoded.Outputs.Count 2 "Sequence-form outputs should decode."
            Expect.equal decoded.Steps.Count 2 "Sequence-form steps should decode after comments."
            Expect.isTrue (decoded.Inputs |> Seq.exists (fun input -> input.Name = "input_data")) "input_data should decode."
            Expect.isTrue (decoded.Outputs |> Seq.exists (fun output -> output.Name = "result_data")) "result_data output should decode."
            Expect.equal decoded.Doc (Some "Workflow doc line.\n# This line is doc text, not a YAML comment.\n") "Block scalar comment-looking text should be preserved."
        testCase "sequence-form workflow fields with blank separator lines decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithBlankSeparatedSequenceFieldsFile
            Expect.equal decoded.Inputs.Count 2 "Blank lines before and between sequence-form inputs should decode."
            Expect.equal decoded.Outputs.Count 1 "Blank lines before sequence-form outputs should decode."
            Expect.equal decoded.Steps.Count 1 "Blank lines before sequence-form steps should decode."
            Expect.equal decoded.Steps.[0].In.Count 2 "Blank lines before and between nested step inputs should decode."
            Expect.isTrue (decoded.Inputs |> Seq.exists (fun input -> input.Name = "trial_id")) "trial_id should decode."
            Expect.equal decoded.Steps.[0].Id "collect" "The step following blank separators should stay mapped under steps."
        testCase "comment-separated map-form workflow inputs decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithLowerIndentedCommentMapInputsFile
            Expect.equal decoded.Inputs.Count 2 "Comment-separated map-form inputs should decode."
            Expect.isTrue (decoded.Inputs |> Seq.exists (fun input -> input.Name = "first")) "first input should decode."
            Expect.isTrue (decoded.Inputs |> Seq.exists (fun input -> input.Name = "second")) "second input should decode."
        testCase "comment before sequence-form workflow inputs decodes" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithLowerIndentedCommentBeforeSequenceInputFile
            Expect.equal decoded.Inputs.Count 1 "Comment before sequence-form inputs should decode."
            Expect.equal decoded.Inputs.[0].Name "first" "first input should decode."
        testCase "multiline valueFrom expression decodes as one string" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithMultilineValueFromFile
            let step = decoded.Steps |> Seq.find (fun step -> step.Id = "expr_step")
            let stepInput = step.In |> Seq.find (fun input -> input.Id = "reads")
            let valueFrom = Expect.wantSome stepInput.ValueFrom "valueFrom should decode."
            Expect.stringContains valueFrom "${" "valueFrom should include the expression opener."
            Expect.stringContains valueFrom "return reads;" "valueFrom should include the expression body."
            Expect.stringContains valueFrom "}" "valueFrom should include the expression closer."
        testCase "optional array shorthand input types decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithOptionalArrayInputTypesFile
            let assertOptionalArray name expectedItem =
                let input = decoded.Inputs |> Seq.find (fun input -> input.Name = name)
                match input.Type_ with
                | Some (Union types) ->
                    Expect.isTrue (types |> Seq.exists ((=) Null)) $"{name} should include null in the union."
                    let arrayItems =
                        types
                        |> Seq.choose (function | Array arraySchema -> Some arraySchema.Items | _ -> None)
                        |> Seq.toList
                    Expect.equal arrayItems [expectedItem] $"{name} should include exactly one array type."
                | other -> failwithf "Expected optional array type for %s, got %A" name other
            assertOptionalArray "optional_files" (File)
            assertOptionalArray "optional_dirs" (Directory)
        testCase "inline enum and record schema union types decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineSchemaUnionTypesFile
            let mode = decoded.Inputs |> Seq.find (fun input -> input.Name = "mode")
            match mode.Type_ with
            | Some (Union types) ->
                Expect.isTrue (types |> Seq.exists ((=) Null)) "mode should include null in the union."
                match types |> Seq.choose (function | Enum schema -> Some schema | _ -> None) |> Seq.toList with
                | [enumSchema] ->
                    Expect.equal enumSchema.Name (Some "Mode") "Inline enum name should decode."
                    Expect.sequenceEqual enumSchema.Symbols (ResizeArray [| "fast"; "stringent" |]) "Inline enum symbols should decode."
                | other -> failwithf "Expected one inline enum schema, got %A" other
            | other -> failwithf "Expected mode to decode as a union, got %A" other
            let recordPayload = decoded.Inputs |> Seq.find (fun input -> input.Name = "record_payload")
            match recordPayload.Type_ with
            | Some (Union types) ->
                Expect.isTrue (types |> Seq.exists ((=) Null)) "record_payload should include null in the union."
                match types |> Seq.choose (function | Record schema -> Some schema | _ -> None) |> Seq.toList with
                | [recordSchema] ->
                    Expect.equal recordSchema.Name (Some "Payload") "Inline record name should decode."
                    let fields = Expect.wantSome recordSchema.Fields "Inline record fields should decode."
                    let sampleId = fields |> Seq.find (fun field -> field.Name = "sample_id")
                    Expect.equal sampleId.Type String "sample_id type should decode."
                    let files = fields |> Seq.find (fun field -> field.Name = "files")
                    match files.Type with
                    | Array arraySchema -> Expect.equal arraySchema.Items (File) "files should decode as File array."
                    | other -> failwithf "Expected files to decode as an array, got %A" other
                | other -> failwithf "Expected one inline record schema, got %A" other
            | other -> failwithf "Expected record_payload to decode as a union, got %A" other
            Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "symbols" decoded) "Nested enum fields should not become workflow overflow."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "fields" decoded) "Nested record fields should not become workflow overflow."
        testCase "decodeWorkflowWithWarnings skips malformed unnamed entries" <| fun _ ->
            let result = Decode.decodeWorkflowWithWarnings TestObjects.CWL.Workflow.workflowWithUnnamedMalformedEntriesFile
            Expect.equal result.Value.Inputs.Count 1 "Only valid input should decode."
            Expect.equal result.Value.Outputs.Count 1 "Only valid output should decode."
            Expect.equal result.Value.Steps.Count 1 "Only valid step should decode."
            Expect.equal result.Value.Steps.[0].In.Count 1 "Only valid step input should decode."
            Expect.equal result.Warnings.Count 4 "Each malformed unnamed entry should produce one warning."
            Expect.sequenceEqual
                (result.Warnings |> Seq.map (fun w -> w.Path) |> Seq.sort |> ResizeArray)
                (ResizeArray [| "inputs[1]"; "outputs[1]"; "steps[0]"; "steps[1].in[1]" |] |> Seq.sort |> ResizeArray)
                "Warning paths should identify skipped entries."
        testCase "the warning of a skipped entry holds the YAML that the entry was written in" <| fun _ ->
            let result = Decode.decodeWorkflowWithWarnings TestObjects.CWL.Workflow.workflowWithUnnamedMalformedEntriesFile
            let rawOf (path: string) = (result.Warnings |> Seq.find (fun w -> w.Path = path)).Raw
            Expect.equal (rawOf "inputs[1]") (Some "not_a_cwl_port: true") "A skipped mapping shows its entries."
            Expect.equal (rawOf "outputs[1]") (Some "[not, a, cwl, output]") "A skipped flow sequence keeps its flow style."
            Expect.equal (rawOf "steps[0]") (Some "\"not a workflow step\"") "A skipped scalar keeps its quotes."
            Expect.equal (rawOf "steps[1].in[1]") (Some "not_a_step_input: true") "A skipped step input shows its entries."
            let withComment = """cwlVersion: v1.2
class: Workflow
inputs:
- not_a_port: true # about it
  other: [1, 2]
outputs: {}
steps: {}
"""
            let commented = Decode.decodeWorkflowWithWarnings withComment
            Expect.equal commented.Warnings.[0].Raw (Some "not_a_port: true # about it\nother: [1, 2]") "A skipped entry keeps its comment and its flow sequence."
        testCase "malformed named input still fails" <| fun _ ->
            Expect.throws
                (fun _ -> Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithMalformedNamedInputFile |> ignore)
                "Named CWL inputs with missing type should fail."
        testCase "malformed known field on named input fails" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
- id: named
  type: string
  label: [not, a, string]
outputs: {}
steps: {}"""
            Expect.throws
                (fun _ -> Decode.decodeWorkflow yaml |> ignore)
                "Named CWL inputs with malformed known fields should fail."
        testCase "malformed id-bearing sequence entries fail instead of becoming warnings" <| fun _ ->
            let malformedInputId = """cwlVersion: v1.2
class: Workflow
inputs:
- id: [not, a, string]
  type: string
outputs: {}
steps: {}"""
            let malformedOutputId = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
- id: [not, a, string]
  type: string
steps: {}"""
            let malformedStepId = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
- id: [not, a, string]
  run: ./tool.cwl
  in: []
  out: []"""
            let malformedStepInputId = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
- id: step1
  run: ./tool.cwl
  in:
  - id: [not, a, string]
    source: input
  out: []"""
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedInputId |> ignore)
                "Malformed input ids should fail rather than become skipped unnamed inputs."
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedOutputId |> ignore)
                "Malformed output ids should fail rather than become skipped unnamed outputs."
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedStepId |> ignore)
                "Malformed step ids should fail rather than become skipped unnamed steps."
            Expect.throws
                (fun _ -> Decode.decodeWorkflowWithWarnings malformedStepInputId |> ignore)
                "Malformed step input ids should fail rather than become skipped unnamed step inputs."
        testCase "unknown fields on named ports and steps are preserved as dynamic properties" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnknownPortFieldsFile
            let input = decoded.Inputs |> Seq.find (fun input -> input.Name = "sample")
            let output = decoded.Outputs |> Seq.find (fun output -> output.Name = "result")
            let step = decoded.Steps |> Seq.find (fun step -> step.Id = "step1")
            let stepInput = step.In |> Seq.find (fun input -> input.Id = "sample")
            let inputBinding = Expect.wantSome input.InputBinding "Input binding should decode."
            let outputBinding = Expect.wantSome output.OutputBinding "Output binding should decode."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:note" input)
                (Some "keep input note")
                "Unknown input fields should be preserved."
            Expect.equal input.Type_ (Some CWLType.File) "The sample input should decode as File."
            Expect.isNone
                (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" input)
                "Unknown input binding fields should not be flattened onto the input."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" inputBinding)
                (Some "keep input binding note")
                "Unknown input binding fields should be preserved on the binding."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:note" output)
                (Some "keep output note")
                "Unknown output fields should be preserved."
            Expect.equal output.Type_ (Some CWLType.File) "The result output should decode as File."
            Expect.isNone
                (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" output)
                "Unknown output binding fields should not be flattened onto the output."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" outputBinding)
                (Some "keep output binding note")
                "Unknown output binding fields should be preserved on the binding."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:step note" step)
                (Some "keep step note")
                "Unknown workflow step fields should be preserved."
            Expect.isNone
                (DynObj.tryGetTypedPropertyValue<string> "arc:step input note" step)
                "Unknown step input fields should not be flattened onto the step."
            Expect.equal
                (DynObj.tryGetTypedPropertyValue<string> "arc:step input note" stepInput)
                (Some "keep step input note")
                "Unknown step input fields should be preserved on the step input."
        testCase "steps array form decode with when/pickValue/doc/default" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithStepsArrayFile
            let step = decoded.Steps.[0]
            let stepInput = step.In.[0]
            Expect.equal step.Id "step1" ""
            Expect.equal step.When_ (Some (SchemaSaladString.Literal "$(inputs.input1 != null)")) ""
            Expect.equal stepInput.PickValue (Some FirstNonNull) ""
            Expect.equal stepInput.Doc (Some "Input docs") ""
            let defaultValue =
                stepInput.DefaultValue
                |> Option.bind tryYamlScalarString
            Expect.equal defaultValue None "Default is structured object and should not collapse to scalar"
        testCase "inline run workflow decode" <| fun _ ->
            let yaml = TestObjects.CWL.Workflow.workflowWithInlineRunWorkflowFile
            let decoded = Decode.decodeWorkflow yaml
            let runValue = decoded.Steps.[0].Run
            let isRunWorkflow =
                match runValue with
                | RunWorkflow _ -> true
                | _ -> false
            Expect.isTrue isRunWorkflow $"Expected RunWorkflow but got %A{runValue}"
            let workflow = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow runValue) "Expected inline workflow payload"
            Expect.equal workflow.Steps.Count 1 ""
            Expect.equal workflow.Steps.[0].Id "inner" ""
        testCase "workflow outputSource array form decodes and roundtrips" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithOutputSourceArrayFile
            let encoded = Encode.encodeWorkflowDescription decoded
            let roundTripped = Decode.decodeWorkflow encoded
            Expect.stringContains encoded "outputSource:" "Array-form outputSource should be emitted."
            let mergedOutput =
                roundTripped.Outputs
                |> Seq.find (fun output -> output.Name = "merged")
            let outputSources = mergedOutput.GetOutputSources()
            Expect.sequenceEqual outputSources (ResizeArray [| "step1/out"; "step2/out" |]) "outputSource entries should survive decode/encode/decode."
        testCase "workflow step run Operation class decodes and roundtrips" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineRunOperationFile
            let encoded = Encode.encodeWorkflowDescription decoded
            let roundTripped = Decode.decodeWorkflow encoded
            Expect.stringContains encoded "class: Operation" "Operation run should roundtrip with class marker."
            Expect.equal roundTripped.Steps.[0].Id "op" "Operation-backed step should survive roundtrip."
        testCase "invalid pickValue fails decode" <| fun _ ->
            let invalidPickValue = TestObjects.CWL.Workflow.workflowWithInvalidPickValueFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidPickValue |> ignore) "Invalid pickValue should fail"
        testCase "invalid scatterMethod fails decode" <| fun _ ->
            let invalidScatter = TestObjects.CWL.Workflow.workflowWithInvalidScatterMethodFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidScatter |> ignore) "Invalid scatterMethod should fail"
        testCase "inline ExpressionTool run decode succeeds" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnsupportedInlineRunClassFile
            let runValue = decoded.Steps.[0].Run
            let isRunExpressionTool =
                match runValue with
                | RunExpressionTool _ -> true
                | _ -> false
            Expect.isTrue isRunExpressionTool $"Expected RunExpressionTool but got %A{runValue}"
            let expressionTool =
                Expect.wantSome
                    (WorkflowStepRunOps.tryGetExpressionTool runValue)
                    "Should extract ExpressionTool payload"
            Expect.equal expressionTool.Expression (SchemaSaladString.Literal "$(null)") ""
        testCase "inline ExpressionTool chain decodes all steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
            Expect.equal decoded.Steps.Count 3 "Should have 3 steps"
            for i in 0..2 do
                let step = decoded.Steps.[i]
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool step.Run)
                        $"Step {i} should be ExpressionTool"
                Expect.isTrue (expressionTool.Expression.Value.Length > 0) $"Step {i} expression should be non-empty"
        testCase "loadContents on inline ExpressionTool input decoded" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithLoadContentsExpressionToolFile
            let step = decoded.Steps.[0]
            let expressionTool =
                Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool step.Run) "Step run should be inline ExpressionTool"
            let hasLoadContentsOnStepInput =
                step.In |> Seq.exists (fun si -> si.LoadContents = Some true)
            let hasValueFromOnStepInput =
                step.In |> Seq.exists (fun si -> si.ValueFrom.IsSome)
            Expect.isTrue hasLoadContentsOnStepInput "Step input should have loadContents"
            Expect.isTrue hasValueFromOnStepInput "Step input should have valueFrom"
            let expressionToolInputs = expressionTool.Inputs |> Option.defaultValue (ResizeArray())
            Expect.equal expressionToolInputs.Count 1 "Inline expression tool should have one input"
        testCase "mixed workflow with CommandLineTool and ExpressionTool steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
            Expect.equal decoded.Steps.Count 2 ""
            match decoded.Steps.[0].Run with
            | RunString _ -> ()
            | other -> Expect.isTrue false $"Step 0 should be RunString but got %A{other}"
            let isExpressionToolStep =
                match decoded.Steps.[1].Run with
                | RunExpressionTool _ -> true
                | _ -> false
            Expect.isTrue isExpressionToolStep "Step 1 should be RunExpressionTool"
        testCase "chained ExpressionTool array pipeline type verification" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
            let step1 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool decoded.Steps.[0].Run) "Step 1 should be ExpressionTool"
            let step2 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool decoded.Steps.[1].Run) "Step 2 should be ExpressionTool"
            let step3 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool decoded.Steps.[2].Run) "Step 3 should be ExpressionTool"

            Expect.equal step1.Inputs.Value.[0].Type_ (Some CWLType.Int) "step1 input should be int"
            match step1.Outputs.[0].Type_ with
            | Some (Array arraySchema) ->
                Expect.equal arraySchema.Items CWLType.Int "step1 output should be int[]"
            | other ->
                Expect.isTrue false $"Expected int[] but got %A{other}"

            // Asserted via `if`/bool rather than a `match` with a unit success branch: Fable's
            // Python backend drops empty branches from a match in statement position, which would
            // leave `Expect.isTrue false` unconditional and make this assertion always fail.
            let step2InputType = step2.Inputs.Value.[0].Type_
            let isArray = match step2InputType with Some (Array _) -> true | _ -> false
            Expect.isTrue isArray $"step2 input should be int[] but got %A{step2InputType}"

            Expect.equal step3.Outputs.[0].Type_ (Some CWLType.Int) "step3 output should be int"
        testCase "step-level loadContents with valueFrom on ExpressionTool" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithLoadContentsExpressionToolFile
            let step = decoded.Steps.[0]
            let stepInputHasLoadContents =
                step.In |> Seq.exists (fun si -> si.LoadContents = Some true)
            let stepInputHasValueFrom =
                step.In |> Seq.exists (fun si -> si.ValueFrom.IsSome)
            Expect.isTrue stepInputHasLoadContents "Step input should have loadContents"
            Expect.isTrue stepInputHasValueFrom "Step input should have valueFrom"
        testCase "advanced workflow fixture decodes and preserves full content" <| fun _ ->
            let original = TestObjects.CWL.Workflow.advancedWorkflowFile
            let processingUnit = Decode.decodeCWLProcessingUnit original
            let workflow =
                match processingUnit with
                | Workflow workflow -> workflow
                | other -> failwithf "Expected Workflow but got %A" other

            Expect.equal workflow.CWLVersion "v1.2" ""
            Expect.equal workflow.Inputs.Count 24 "All workflow inputs should decode."
            Expect.equal workflow.Steps.Count 11 "All workflow steps should decode."
            Expect.equal workflow.Outputs.Count 1 "Workflow output should decode."
            Expect.equal workflow.Id (Some "CWL_advanced_layout_workflow") "Workflow id should be preserved."

            let hints = Expect.wantSome workflow.Hints "Workflow hints should decode."
            Expect.equal hints.Count 3 "All workflow hints should decode."
            match hints.[0] with
            | KnownHint (DockerRequirement dockerRequirement) ->
                Expect.equal dockerRequirement.DockerImageId (Some "devcontainer") "Docker hint should keep dockerImageId."
                Expect.equal dockerRequirement.DockerFile (Some (SchemaSaladString.Include "./Dockerfile")) "Docker hint should keep dockerFile include."
                Expect.sequenceEqual
                    (dockerRequirement.DockerRunOptions |> Option.defaultValue (ResizeArray()))
                    (ResizeArray [| "--gpus=all" |])
                    "Docker hint should keep cwltool docker run options."
            | other ->
                failwithf "Expected first hint to decode as DockerRequirement but got %A" other

            match hints.[1] with
            | UnknownHint unknownHint ->
                Expect.equal unknownHint.Class (Some "cwltool:CUDARequirement") "Unknown CUDA hint should be preserved."
            | other ->
                failwithf "Expected second hint to decode as UnknownHint but got %A" other

            let requirements = Expect.wantSome workflow.Requirements "Workflow requirements should decode."
            Expect.equal requirements.Count 4 "All workflow requirements should decode."

            let encoded = Encode.encodeWorkflowDescription workflow
            let roundTripped = Decode.decodeWorkflow encoded

            let encodeInputs (inputs: ResizeArray<CWLInput>) =
                inputs
                |> Seq.map (Encode.encodeCWLInput >> renderYamlPair)
                |> ResizeArray

            let encodeOutputs (outputs: ResizeArray<CWLOutput>) =
                outputs
                |> Seq.map (Encode.encodeCWLOutput >> renderYamlPair)
                |> ResizeArray

            let encodeSteps (steps: ResizeArray<WorkflowStep>) =
                steps
                |> Seq.map (Encode.encodeWorkflowStep >> renderYamlPair)
                |> ResizeArray

            let encodeRequirements (requirements: ResizeArray<Requirement>) =
                requirements
                |> Seq.map (Encode.encodeRequirement >> renderYamlElement)
                |> ResizeArray

            let encodeHints (hints: ResizeArray<HintEntry>) =
                hints
                |> Seq.map (Encode.encodeHintEntry >> renderYamlElement)
                |> ResizeArray

            Expect.equal roundTripped.CWLVersion workflow.CWLVersion "Workflow version should roundtrip."
            Expect.equal roundTripped.Id workflow.Id "Workflow id should roundtrip."
            Expect.sequenceEqual (encodeHints (Expect.wantSome roundTripped.Hints "Roundtripped workflow hints should exist.")) (encodeHints hints) "Workflow hints should roundtrip."
            Expect.sequenceEqual (encodeRequirements (Expect.wantSome roundTripped.Requirements "Roundtripped workflow requirements should exist.")) (encodeRequirements requirements) "Workflow requirements should roundtrip."
            Expect.sequenceEqual (encodeInputs roundTripped.Inputs) (encodeInputs workflow.Inputs) "Workflow inputs should roundtrip."
            Expect.sequenceEqual (encodeSteps roundTripped.Steps) (encodeSteps workflow.Steps) "Workflow steps should roundtrip."
            Expect.sequenceEqual (encodeOutputs roundTripped.Outputs) (encodeOutputs workflow.Outputs) "Workflow outputs should roundtrip."
    ]

let testCWLWorkflowDescriptionEncode =
    testList "Encode" [
        testList "Workflow negative tests" [
            testCase "missing outputSource detected" <| fun _ ->
                let original = TestObjects.CWL.Workflow.workflowFile
                let decoded = Decode.decodeWorkflow original
                let encoded = Encode.encodeWorkflowDescription decoded
                // Simulate user editing and deleting outputSource lines
                let modified =
                    encoded.Split('\n')
                    |> Array.filter (fun l -> not (l.TrimStart().StartsWith("outputSource:")))
                    |> String.concat "\n"
                let decodedModified = Decode.decodeWorkflow modified
                let missing = decodedModified.Outputs |> Seq.filter (fun o -> o.OutputSource.IsNone) |> Seq.toList
                Expect.isTrue (missing.Length > 0) "At least one output should be missing outputSource after manual removal"
        ]
        testList "Process elements" [
            testCase "the process element encoders write the cwlVersion only when asked to" <| fun _ ->
                let elements (includeCwlVersion: bool) =
                    [ Encode.encodeToolDescriptionElement includeCwlVersion (CWLToolDescription(outputs = ResizeArray()))
                      Encode.encodeWorkflowDescriptionElement includeCwlVersion (CWLWorkflowDescription(steps = ResizeArray(), inputs = ResizeArray(), outputs = ResizeArray()))
                      Encode.encodeExpressionToolDescriptionElement includeCwlVersion (CWLExpressionToolDescription(outputs = ResizeArray(), expression = SchemaSaladString.Literal "$(null)"))
                      Encode.encodeOperationDescriptionElement includeCwlVersion (CWLOperationDescription(inputs = ResizeArray(), outputs = ResizeArray())) ]
                    |> List.map Encode.writeYaml
                for text in elements true do
                    Expect.stringContains text "cwlVersion: v1.2" "The element writes its version."
                for text in elements false do
                    Expect.isFalse (text.Contains "cwlVersion") "The element takes the version of its document."
                    Expect.stringContains text "class:" "The element still writes its class."
        ]
        testList "Workflow Encode RoundTrip" [
            testCase "workflow encode/decode deterministic & outputSource preserved" <| fun _ ->
                let original = TestObjects.CWL.Workflow.workflowFile
                let (encoded1, d1, d2) = assertDeterministic Encode.encodeWorkflowDescription Decode.decodeWorkflow "Workflow" original
                // Verify outputs have outputSource in both decoded versions
                assertAllOutputsHaveSource d1
                assertAllOutputsHaveSource d2
            testCase "workflow encode/decode preserves step array extended fields" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithStepsArrayFile
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let step = roundTripped.Steps.[0]
                let stepInput = step.In.[0]
                Expect.equal step.When_ (Some (SchemaSaladString.Literal "$(inputs.input1 != null)")) ""
                Expect.equal stepInput.PickValue (Some FirstNonNull) ""
                Expect.equal stepInput.Doc (Some "Input docs") ""
                let defaultEntry =
                    Expect.wantSome stepInput.DefaultValue "Step input default should remain present as structured YAML"
                match defaultEntry.Node.Kind with
                | YamlNodeKind.Mapping _
                | YamlNodeKind.Sequence _ ->
                    Expect.isTrue true ""
                | _ ->
                    Expect.isTrue false (sprintf "Expected structured default, got %s" (YamlNode.show defaultEntry.Node))
            testCase "workflow hints are encoded and preserved" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowFile
                original.Hints <- Some (ResizeArray [KnownHint StepInputExpressionRequirement])
                let encoded = Encode.encodeWorkflowDescription original
                let decoded = Decode.decodeWorkflow encoded
                Expect.stringContains encoded "hints:" "Workflow hints should be present in encoded output"
                let hints = Expect.wantSome decoded.Hints "Workflow hints should survive roundtrip"
                Expect.equal hints.[0] (KnownHint StepInputExpressionRequirement) ""
            testCase "workflow intent is encoded and preserved" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithIntentFile
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.stringContains encoded "intent:" "Workflow intent should be present in encoded output"
                let intent = Expect.wantSome roundTripped.Intent "Workflow intent should survive roundtrip"
                Expect.sequenceEqual intent (ResizeArray [|"primary-analysis"; "quality-control"|]) ""
            testCase "workflow direct overflow and nested port overflow roundtrip" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnknownPortFieldsFile
                DynObj.setProperty "arc:direct note" "workflow note" decoded
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let metadata = Expect.wantSome roundTripped.Metadata "Decoded direct overflow should be available as metadata."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "workflow note") "Workflow direct DynamicObj overflow should encode."

                let input = roundTripped.Inputs |> Seq.find (fun input -> input.Name = "sample")
                let output = roundTripped.Outputs |> Seq.find (fun output -> output.Name = "result")
                let step = roundTripped.Steps |> Seq.find (fun step -> step.Id = "step1")
                let stepInput = step.In |> Seq.find (fun input -> input.Id = "sample")
                let inputBinding = Expect.wantSome input.InputBinding "Input binding should survive roundtrip."
                let outputBinding = Expect.wantSome output.OutputBinding "Output binding should survive roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" input) (Some "keep input note") "Input overflow should roundtrip."
                Expect.equal input.Type_ (Some CWLType.File) "The input type should roundtrip."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" inputBinding) (Some "keep input binding note") "Input binding overflow should roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" output) (Some "keep output note") "Output overflow should roundtrip."
                Expect.equal output.Type_ (Some CWLType.File) "The output type should roundtrip."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" outputBinding) (Some "keep output binding note") "Output binding overflow should roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:step note" step) (Some "keep step note") "Workflow step overflow should roundtrip."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:step input note" stepInput) (Some "keep step input note") "Step input overflow should roundtrip."
            testCase "workflow decimal overflow roundtrips" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  sample:
    type: string
    arc:threshold: 2.5
outputs: {}
steps: {}"""
                let decoded = Decode.decodeWorkflow yaml
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let input = roundTripped.Inputs |> Seq.find (fun input -> input.Name = "sample")
                Expect.equal (DynObj.tryGetTypedPropertyValue<float> "arc:threshold" input) (Some 2.5) "Decimal overflow should roundtrip as a float."
            testCase "workflow singleton sequence overflow roundtrips as a sequence" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  sample:
    type: string
    arc:list: [only]
outputs: {}
steps: {}"""
                let decoded = Decode.decodeWorkflow yaml
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let input = roundTripped.Inputs |> Seq.find (fun input -> input.Name = "sample")
                let items = Expect.wantSome (DynamicObjHelpers.tryGetTypedPropertyValueAsResizeArray<obj> "arc:list" input) "Singleton overflow should roundtrip as a collection."
                Expect.equal items.Count 1 "Singleton overflow sequence should keep its item count."
                Expect.equal (items.[0] :?> string) "only" "Singleton overflow sequence should keep its value."
            testList "PickValueMethod roundtrip" [
                for (pickValueMethod, cwlString) in [
                    FirstNonNull, "first_non_null"
                    TheOnlyNonNull, "the_only_non_null"
                    AllNonNull, "all_non_null"
                ] do
                    testCase cwlString <| fun _ ->
                        let yaml = TestObjects.CWL.Workflow.workflowWithPickValueMethodFile cwlString
                        let decoded = Decode.decodeWorkflow yaml
                        Expect.equal decoded.Steps.[0].In.[0].PickValue (Some pickValueMethod) ""
                        let encoded = Encode.encodeWorkflowDescription decoded
                        Expect.stringContains encoded $"pickValue: {cwlString}" ""
                        let roundTripped = Decode.decodeWorkflow encoded
                        Expect.equal roundTripped.Steps.[0].In.[0].PickValue (Some pickValueMethod) ""
            ]
            testList "ScatterMethod roundtrip" [
                for (scatterMethod, cwlString) in [
                    DotProduct, "dotproduct"
                    NestedCrossProduct, "nested_crossproduct"
                    FlatCrossProduct, "flat_crossproduct"
                ] do
                    testCase cwlString <| fun _ ->
                        let yaml = TestObjects.CWL.Workflow.workflowWithScatterMethodFile cwlString
                        let decoded = Decode.decodeWorkflow yaml
                        Expect.equal decoded.Steps.[0].ScatterMethod (Some scatterMethod) ""
                        let encoded = Encode.encodeWorkflowDescription decoded
                        Expect.stringContains encoded $"scatterMethod: {cwlString}" ""
                        let roundTripped = Decode.decodeWorkflow encoded
                        Expect.equal roundTripped.Steps.[0].ScatterMethod (Some scatterMethod) ""
            ]
            testCase "structured default with array value roundtrips" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithStructuredArrayDefaultFile
                let decoded = Decode.decodeWorkflow yaml
                let decodedDefault =
                    decoded.Steps.[0].In.[0].DefaultValue
                    |> Option.bind tryYamlScalarSequence
                    |> Option.defaultValue (ResizeArray())
                Expect.sequenceEqual decodedDefault (ResizeArray [|"1"; "2"; "3"|]) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let roundTrippedDefault =
                    roundTripped.Steps.[0].In.[0].DefaultValue
                    |> Option.bind tryYamlScalarSequence
                    |> Option.defaultValue (ResizeArray())
                Expect.sequenceEqual roundTrippedDefault (ResizeArray [|"1"; "2"; "3"|]) ""
            testCase "when expression with embedded quotes roundtrips" <| fun _ ->
                let expression = "$(inputs.name == 'test')"
                let yaml = TestObjects.CWL.Workflow.workflowWithWhenExpressionFile expression
                let decoded = Decode.decodeWorkflow yaml
                Expect.equal decoded.Steps.[0].When_ (Some (SchemaSaladString.Literal expression)) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.equal roundTripped.Steps.[0].When_ (Some (SchemaSaladString.Literal expression)) ""
            testCase "scatter + scatterMethod together roundtrip" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithScatterAndScatterMethodFile
                let decoded = Decode.decodeWorkflow yaml
                Expect.sequenceEqual decoded.Steps.[0].Scatter.Value (ResizeArray [|"in1"; "in2"|]) ""
                Expect.equal decoded.Steps.[0].ScatterMethod (Some FlatCrossProduct) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.sequenceEqual roundTripped.Steps.[0].Scatter.Value (ResizeArray [|"in1"; "in2"|]) ""
                Expect.equal roundTripped.Steps.[0].ScatterMethod (Some FlatCrossProduct) ""
            testCase "step-level requirements and hints roundtrip" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithStepLevelRequirementsAndHintsFile
                let decoded = Decode.decodeWorkflow yaml
                let decodedStep = decoded.Steps.[0]
                let decodedHints = Expect.wantSome decodedStep.Hints "Step hints should decode"
                let decodedRequirements = Expect.wantSome decodedStep.Requirements "Step requirements should decode"
                Expect.equal decodedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal decodedRequirements.[0] (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let roundTrippedStep = roundTripped.Steps.[0]
                let roundTrippedHints = Expect.wantSome roundTrippedStep.Hints "Step hints should survive roundtrip"
                let roundTrippedRequirements = Expect.wantSome roundTrippedStep.Requirements "Step requirements should survive roundtrip"
                Expect.equal roundTrippedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal roundTrippedRequirements.[0] (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) ""
            testCase "inline ExpressionTool roundtrip preserves expression" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
                let encoded = Encode.encodeWorkflowDescription original
                let roundTripped = Decode.decodeWorkflow encoded
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool roundTripped.Steps.[1].Run)
                        "Should roundtrip ExpressionTool run"
                Expect.equal expressionTool.Expression (SchemaSaladString.Literal "$({'out': inputs.y})") "Expression preserved"
            testCase "inline ExpressionTool chain roundtrip" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
                let encoded = Encode.encodeWorkflowDescription original
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.equal roundTripped.Steps.Count 3 ""
                for i in 0..2 do
                    let originalTool =
                        Expect.wantSome
                            (WorkflowStepRunOps.tryGetExpressionTool original.Steps.[i].Run)
                            $"Original step {i} should be ExpressionTool"
                    let roundTrippedTool =
                        Expect.wantSome
                            (WorkflowStepRunOps.tryGetExpressionTool roundTripped.Steps.[i].Run)
                            $"Roundtripped step {i} should be ExpressionTool"
                    Expect.equal roundTrippedTool.Expression originalTool.Expression $"Step {i} expression should be preserved"
                    let originalInputs = originalTool.Inputs |> Option.defaultValue (ResizeArray())
                    let roundTrippedInputs = roundTrippedTool.Inputs |> Option.defaultValue (ResizeArray())
                    Expect.equal roundTrippedInputs.Count originalInputs.Count $"Step {i} input count should match"
                    Expect.equal roundTrippedTool.Outputs.Count originalTool.Outputs.Count $"Step {i} output count should match"
            testCase "inline ExpressionTool multiline expression in workflow step roundtrips" <| fun _ ->
                let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: expr/out
steps:
  expr:
    run:
      cwlVersion: v1.2
      class: ExpressionTool
      requirements:
        - class: InlineJavascriptRequirement
      inputs: {}
      outputs:
        out: string
      expression: |
        ${ return (function() {
          var name = "arc";
          return {"out": name};
        })(); }
    in: {}
    out: [out]"""

                let decoded = Decode.decodeWorkflow yaml
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded

                Expect.stringContains encoded "expression: |" "Expression should encode as block scalar in nested workflow runs."

                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool roundTripped.Steps.[0].Run)
                        "Should roundtrip multiline inline ExpressionTool run"

                Expect.stringContains expressionTool.Expression.Value "return" "Expression should decode after roundtrip."
                Expect.stringContains expressionTool.Expression.Value "var name = \"arc\"" "Variable assignment should survive roundtrip."
                Expect.stringContains expressionTool.Expression.Value "{\"out\": name}" "Return object should survive roundtrip."
        ]
    ]

let testWorkflowUnionPortRoundTrip =
    let optionalOf (inner: CWLType) = Union (ResizeArray [ Null; inner ])

    let assertTypesEqual (expected: CWLType) (actual: CWLType) (message: string) =
        Expect.isTrue (CWLType.typesEqual expected actual) $"{message}: expected %A{expected} but got %A{actual}"

    let inputTypeOf (workflow: CWLWorkflowDescription) (name: string) =
        let input = CWLWorkflowDescription.getInputs workflow |> Seq.find (fun i -> i.Name = name)
        Expect.wantSome input.Type_ $"Input {name} should have a type."

    let outputTypeOf (workflow: CWLWorkflowDescription) (name: string) =
        let output = CWLWorkflowDescription.getOutputs workflow |> Seq.find (fun o -> o.Name = name)
        Expect.wantSome output.Type_ $"Output {name} should have a type."

    let enumOf (symbols: string list) = Enum (InputEnumSchema(ResizeArray symbols))

    testList "Union port types roundtrip" [
        testCase "unions of null with an enum, a record or an array decode, encode and decode again" <| fun _ ->
            let check (workflow: CWLWorkflowDescription) =
                assertTypesEqual (optionalOf (enumOf [ "alpha"; "beta"; "gamma"; "delta" ])) (inputTypeOf workflow "mode_choice") "mode_choice"
                assertTypesEqual (optionalOf (CWLType.Directory)) (inputTypeOf workflow "annotation_directory") "annotation_directory"
                assertTypesEqual (optionalOf (Array (InputArraySchema(CWLType.File)))) (inputTypeOf workflow "sample_files") "sample_files"
                assertTypesEqual (Union (ResizeArray [ Int; Float ])) (inputTypeOf workflow "threshold") "threshold"
                match inputTypeOf workflow "reference_dbs" with
                | Union members ->
                    Expect.equal members.Count 2 "reference_dbs should be a two member union."
                    Expect.equal members.[0] Null "The first member should be null."
                    match members.[1] with
                    | Record schema ->
                        let fields = Expect.wantSome schema.Fields "The record should keep its fields."
                        Expect.sequenceEqual (fields |> Seq.map (fun f -> f.Name)) [ "data_folder"; "db_file" ] "Record fields should keep their order."
                    | other -> failwithf "Expected a record member but got %A" other
                | other -> failwithf "Expected a union but got %A" other
                let flagOf name = (CWLWorkflowDescription.getInputs workflow |> Seq.find (fun i -> i.Name = name)).Optional
                for name in [ "mode_choice"; "annotation_directory"; "sample_files"; "reference_dbs" ] do
                    Expect.isTrue (flagOf name) (sprintf "%s is a union of null and one type, so it is optional." name)
                Expect.isFalse (flagOf "threshold") "A union without null is not optional."
                assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "chosen") "chosen"
                assertTypesEqual (Union (ResizeArray [ CWLType.File; CWLType.Directory ])) (outputTypeOf workflow "summary") "summary"
                let expressionTool = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool workflow.Steps.[0].Run) "The inline ExpressionTool should decode."
                let toolInput = (Expect.wantSome expressionTool.Inputs "The inline tool should keep its inputs.") |> Seq.find (fun i -> i.Name = "mode_string")
                assertTypesEqual (optionalOf (enumOf [ "alpha"; "delta"; "beta"; "gamma" ])) (Expect.wantSome toolInput.Type_ "Inline input type") "inline mode_string"
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.UnionPorts.typeListWorkflow
            check decoded
            let encoded = Encode.encodeWorkflowDescription decoded
            check (Decode.decodeWorkflow encoded)

        testCase "the encoder writes the null type as a quoted string" <| fun _ ->
            let encoded = Encode.encodeWorkflowDescription (Decode.decodeWorkflow TestObjects.CWL.Workflow.UnionPorts.typeListWorkflow)
            Expect.stringContains encoded "\"null\"" "A plain null would read as a YAML null, so the null type must be quoted."
            Expect.isFalse (encoded.Contains("[null") || encoded.Contains("- null") || encoded.Contains(", null")) "No union may contain an unquoted null."

        testCase "array form ports with unions roundtrip" <| fun _ ->
            let check (workflow: CWLWorkflowDescription) =
                assertTypesEqual CWLType.String (inputTypeOf workflow "NAME_PATTERN") "NAME_PATTERN"
                assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "INDEX_FILES") "INDEX_FILES"
                assertTypesEqual (optionalOf (Array (InputArraySchema(Float)))) (inputTypeOf workflow "LIMITS") "LIMITS"
                assertTypesEqual (Union (ResizeArray [ String; Int; Boolean ])) (inputTypeOf workflow "CHOICE") "CHOICE"
                assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "MAYBE_LIST") "MAYBE_LIST"
                assertTypesEqual (CWLType.File) (outputTypeOf workflow "RESULT_LIST") "RESULT_LIST"
                assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "OPTIONAL_RESULT") "OPTIONAL_RESULT"
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.UnionPorts.arrayFormWorkflow
            check decoded
            check (Decode.decodeWorkflow (Encode.encodeWorkflowDescription decoded))

        testCase "a port written as a bare type list decodes as a union" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  flow_style: [null, File]
  block_style:
    - string
    - int
steps: []
outputs:
  result: ["null", File]
"""
            let workflow = Decode.decodeWorkflow yaml
            assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "flow_style") "flow_style"
            assertTypesEqual (Union (ResizeArray [ String; Int ])) (inputTypeOf workflow "block_style") "block_style"
            let flagOf name = (CWLWorkflowDescription.getInputs workflow |> Seq.find (fun i -> i.Name = name)).Optional
            Expect.isTrue (flagOf "flow_style") "A bare list of null and a file is optional."
            Expect.isFalse (flagOf "block_style") "A bare list of two types without null is not optional."
            assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "result") "result"
    ]

let testPackedDocuments =
    let fixture = TestObjects.CWL.Workflow.PackedDocument.graphFile
    let graphOf (yaml: string) =
        match Decode.decodeCWLDocument yaml with
        | CWLDocument.Graph graph -> graph
        | other -> failwith $"Expected a packed document but got %A{other}"
    let workflowOf (processingUnit: CWLProcessingUnit) =
        match processingUnit with
        | Workflow workflow -> workflow
        | other -> failwith $"Expected a workflow but got %A{other}"
    let toolOf (processingUnit: CWLProcessingUnit) =
        match processingUnit with
        | CommandLineTool tool -> tool
        | other -> failwith $"Expected a command line tool but got %A{other}"
    testList "Packed documents" [
        testCase "a packed document decodes as a graph of its processes that use the version of the root" <| fun _ ->
            let graph = graphOf fixture
            Expect.equal graph.CWLVersion "v1.2" "The graph has the version of the root."
            Expect.equal graph.Processes.Count 2 "The graph holds two processes."
            let main = workflowOf graph.Processes.[0]
            Expect.equal main.Id (Some "main") "The workflow keeps its id."
            Expect.equal main.CWLVersion "v1.2" "The workflow uses the version of the root."
            let shout = toolOf graph.Processes.[1]
            Expect.equal shout.Id (Some "shout") "The tool keeps its id."
            Expect.equal shout.CWLVersion "v1.2" "The tool uses the version of the root."
        testCase "a step keeps the reference to a process of the graph as its run string" <| fun _ ->
            let main = workflowOf (graphOf fixture).Processes.[0]
            Expect.equal main.Steps.[0].Run (RunString "#shout") "The run field stays the reference."
        testCase "the other keys of the root stay on the graph" <| fun _ ->
            let graph = graphOf fixture
            let namespaces = Expect.wantSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "$namespaces" graph) "The graph keeps $namespaces."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "ex" namespaces) (Some "https://example.org/ns#") "The namespace keeps its IRI."
            Expect.isNone (DynObj.tryGetTypedPropertyValue<obj> "$graph" graph) "The processes are no dynamic property."
        testCase "a graph is written with the version of the root only and reads back" <| fun _ ->
            let graph = graphOf fixture
            let encoded = Encode.encodeCWLDocument (CWLDocument.Graph graph)
            Expect.stringContains encoded "$graph:" "The processes stand under $graph."
            Expect.stringContains encoded "$namespaces:" "The other keys of the root are written."
            Expect.equal (encoded.Split('\n') |> Array.filter (fun line -> line.Contains "cwlVersion") |> Array.length) 1 "Only the root writes a cwlVersion."
            let roundTripped = graphOf encoded
            Expect.equal roundTripped.CWLVersion "v1.2" "The version survives the round trip."
            Expect.equal roundTripped.Processes.Count 2 "Both processes survive the round trip."
            Expect.equal (workflowOf roundTripped.Processes.[0]).Steps.[0].Run (RunString "#shout") "The reference survives the round trip."
            Expect.equal (toolOf roundTripped.Processes.[1]).CWLVersion "v1.2" "The tool uses the version of the root again."
            Expect.isSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "$namespaces" roundTripped) "$namespaces survives the round trip."
            Expect.equal (Encode.encodeCWLDocument (CWLDocument.Graph roundTripped)) encoded "A second pass writes the same text."
        testCase "a graph compares by its version, its processes and the other keys of its root" <| fun _ ->
            let namespaced = TestObjects.CWL.Workflow.PackedDocument.namespacedFile
            Expect.equal (graphOf namespaced) (graphOf namespaced) "Two reads of one text are equal."
            Expect.equal (Decode.decodeCWLDocument namespaced) (Decode.decodeCWLDocument namespaced) "The documents are equal too."
            Expect.equal (hash (graphOf namespaced)) (hash (graphOf namespaced)) "Equal graphs hash alike."
            Expect.notEqual (graphOf namespaced) (graphOf (namespaced.Replace("baseCommand: wc", "baseCommand: cat"))) "Another process makes another graph."
            Expect.notEqual (graphOf namespaced) (graphOf (namespaced.Replace("ex:note: kept at the root", "ex:note: changed"))) "Another key of the root makes another graph."
            Expect.notEqual (graphOf namespaced) (graphOf (namespaced.Replace("cwlVersion: v1.2", "cwlVersion: v1.1"))) "Another version makes another graph."
        testCase "a step that holds a process of the graph is written as a reference, and an inline copy stays inline" <| fun _ ->
            let graph = graphOf fixture
            let main = workflowOf graph.Processes.[0]
            main.Steps.[0].Run <- WorkflowStepRunOps.fromTool (toolOf graph.Processes.[1])
            Expect.equal (graphOf (Encode.encodeCWLGraph graph)) (graphOf fixture) "The step runs the process by its id again."
            let copy = toolOf (graphOf fixture).Processes.[1]
            main.Steps.[0].Run <- WorkflowStepRunOps.fromTool copy
            let inlined = graphOf (Encode.encodeCWLGraph graph)
            Expect.equal (workflowOf inlined.Processes.[0]).Steps.[0].Run (WorkflowStepRunOps.fromTool copy) "A copy of the process is written inside the step."
        testCase "a process is the process main of a graph when the graph finds that object under the id main" <| fun _ ->
            let namespaced = TestObjects.CWL.Workflow.PackedDocument.namespacedFile
            let graph = graphOf namespaced
            let main = workflowOf graph.Processes.[0]
            Expect.isTrue (CWLGraph.isMain (Workflow main) graph) "A new union value around the object of main is the process main."
            Expect.isFalse (CWLGraph.isMain graph.Processes.[1] graph) "Another process of the graph is not."
            Expect.isFalse (CWLGraph.isMain (Workflow (workflowOf (graphOf namespaced).Processes.[0])) graph) "A process with the same content in another graph is another object."
            main.Id <- Some "renamed"
            Expect.isFalse (CWLGraph.isMain (Workflow main) graph) "The graph reads the id on each call, so a process that was renamed is not the process main."
        testCase "the process main of a graph is written with its graph and any other process is written alone" <| fun _ ->
            let namespaced = TestObjects.CWL.Workflow.PackedDocument.namespacedFile
            let graph = graphOf namespaced
            let packed = Encode.encodeProcessingUnitInGraph (Some graph) graph.Processes.[0]
            Expect.equal packed (Encode.encodeCWLGraph graph) "The process main stands for its graph."
            Expect.equal (graphOf packed) (graphOf namespaced) "The other processes and the keys of the root are written as they were read."
            let renamed (id: string option) =
                let fresh = graphOf namespaced
                (workflowOf fresh.Processes.[0]).Id <- id
                fresh, fresh.Processes.[0]
            let withoutMain = graphOf (namespaced.Replace("- id: main", "- id: first"))
            let renamedGraph, renamedMain = renamed (Some "renamed")
            let unnamedGraph, unnamedMain = renamed None
            let cases =
                [ "a new process with the id main", graph, CommandLineTool (CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ "date" ], id = "main"))
                  "another process of the graph", graph, graph.Processes.[1]
                  "the process main after a rename", renamedGraph, renamedMain
                  "the process main after its id was removed", unnamedGraph, unnamedMain
                  "a process of a graph that has no process main", withoutMain, withoutMain.Processes.[0] ]
            for (name, owner, processingUnit) in cases do
                let written = Encode.encodeProcessingUnitInGraph (Some owner) processingUnit
                Expect.equal written (Encode.encodeProcessingUnit processingUnit) $"{name}: the file holds the process alone."
                match Decode.decodeCWLDocument written with
                | CWLDocument.Process _ -> ()
                | other -> failwith $"{name}: expected a plain process but got %A{other}"
            let withoutId = CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ "date" ])
            Encode.encodeProcessingUnitInGraph (Some graph) (CommandLineTool withoutId) |> ignore
            Expect.isNone withoutId.Id "Writing does not give a process an id."
        testCase "a graph with a cycle of run references is a decode error at the run field that closes the cycle" <| fun _ ->
            let cases =
                [ "two workflows that run each other", TestObjects.CWL.Workflow.PackedDocument.cyclicFile, "$graph[1].steps.back.run", "#main, #sub, #main"
                  "a workflow that runs itself", TestObjects.CWL.Workflow.PackedDocument.cyclicFile.Replace("run: \"#sub\"", "run: \"#main\""), "$graph[0].steps.s.run", "#main, #main"
                  "a step of an inline workflow that runs main", TestObjects.CWL.Workflow.PackedDocument.cyclicThroughInlineRunFile, "$graph[0].steps.s.run.steps.t.run", "#main, #main" ]
            for (name, yaml, path, cycle) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) path $"{name}: the error should show the run field."
                    Expect.stringContains error.Message ("cycle of run references (" + cycle + ")") $"{name}: the error should name the cycle."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
            let shared = graphOf TestObjects.CWL.Workflow.PackedDocument.sharedProcessFile
            Expect.equal shared.Processes.Count 2 "Two runs of one process are no cycle."
        testCase "a run that names no process of the graph is a decode error at the run field" <| fun _ ->
            let yaml = fixture.Replace("run: \"#shout\"", "run: \"#absent\"")
            match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "$graph[0].steps.shout.run" "The error shows the run field of the step."
                Expect.stringContains error.Message "#absent" "The error names the reference."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A reference to a missing process must fail."
        testCase "ids with and without the leading # name the same process" <| fun _ ->
            let withHash = fixture.Replace("- id: main", "- id: \"#main\"").Replace("- id: shout", "- id: \"#shout\"")
            let graph = graphOf withHash
            Expect.equal (workflowOf graph.Processes.[0]).Id (Some "#main") "The id keeps the # as it is written."
            Expect.isSome (CWLGraph.tryFindProcess "main" graph) "main is found without the #."
            Expect.isSome (CWLGraph.tryFindProcess "#shout" graph) "shout is found with the #."
            Expect.isNone (CWLGraph.tryFindProcess "absent" graph) "A missing id finds nothing."
            let plain = graphOf fixture
            Expect.isSome (CWLGraph.tryFindProcess "#main" plain) "An id written without # is found with the #."
            Expect.isSome (CWLGraph.tryFindProcess "shout" plain) "An id written without # is found without it."
        testCase "a process that names another cwlVersion than the root uses the version of the root" <| fun _ ->
            let graph = graphOf TestObjects.CWL.Workflow.PackedDocument.versionsFile
            Expect.equal (workflowOf graph.Processes.[0]).CWLVersion "v1.2" "The same version is accepted."
            Expect.equal (toolOf graph.Processes.[1]).CWLVersion "v1.2" "The version of the root wins over v1.0."
        testCase "the cwlVersion of a process of a packed document changes nothing whatever it holds" <| fun _ ->
            let head = """cwlVersion: v1.2
$graph:
  - id: main
    class: CommandLineTool
    cwlVersion:"""
            let tail = """
    baseCommand: echo
    inputs: {}
    outputs: {}"""
            for (name, field) in [ "no value", ""; "an older version", " v1.0"; "a draft", " draft-3"; "the version itself", " v1.2"; "a newer version", " v1.3"; "a text that is no version", " banana" ] do
                Expect.equal (toolOf (graphOf (head + field + tail)).Processes.[0]).CWLVersion "v1.2" $"{name}: the process uses the version of the root."
        testCase "a process of a packed document without an id is a decode error at its place" <| fun _ ->
            let yaml = """cwlVersion: v1.2
$graph:
  - id: main
    class: CommandLineTool
    baseCommand: echo
    inputs: {}
    outputs: {}
  - class: CommandLineTool
    baseCommand: cat
    inputs: {}
    outputs: {}"""
            match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "$graph[1]" "The error shows the process."
                Expect.stringContains error.Message "Expected an id field" "The error says that the id is missing."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A process without an id must fail."
        testCase "a packed document with a class at the root is a decode error at the root" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
$graph:
  - id: main
    class: CommandLineTool
    baseCommand: echo
    inputs: {}
    outputs: {}"""
            match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "" "The error stands at the root."
                Expect.stringContains error.Message "no class field" "The error names the field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A class at the root of a packed document must fail."
        testCase "two processes of a packed document with one id are a decode error at the second" <| fun _ ->
            let cases =
                [ "the same id", TestObjects.CWL.Workflow.PackedDocument.duplicateIdsFile
                  "the same id with and without the #", TestObjects.CWL.Workflow.PackedDocument.duplicateIdsWithHashFile ]
            for (name, yaml) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "$graph[1]" $"{name}: the error should show the second process."
                    Expect.stringContains error.Message "The id main" $"{name}: the error should name the id."
                    Expect.isSome error.Span $"{name}: the error should have the place in the text."
                | None -> failwith $"{name} must fail"
        testCase "a $graph that is no list is a decode error that names the kind of the value" <| fun _ ->
            let mapping = """cwlVersion: v1.2
$graph:
  id: main"""
            let text = """cwlVersion: v1.2
$graph: main"""
            let cases = [ "a mapping", mapping, "found a mapping."; "a text", text, "found the text \"main\"." ]
            for (name, yaml, message) in cases do
                match decodeError (fun () -> Decode.decodeCWLDocument yaml |> ignore) with
                | Some error ->
                    Expect.equal (DecodeError.pathText error) "$graph" $"{name}: the error should show the field."
                    Expect.stringContains error.Message message $"{name}: the error should say what it found."
                | None -> failwith $"{name} must fail"
        testCase "a type is named through the id of another process of the graph" <| fun _ ->
            let graph = graphOf TestObjects.CWL.Workflow.PackedDocument.typesFile
            let place = toolOf graph.Processes.[1]
            Expect.equal place.Inputs.Value.[0].Type_ (Some (Reference "#main/Point")) "The tool names the type of the workflow."
            let main = workflowOf graph.Processes.[0]
            Expect.equal main.Inputs.[0].Type_ (Some (Reference "#main/Point")) "The workflow names its own type the same way."
        testCase "a document without a graph decodes as a single process" <| fun _ ->
            let yaml = TestObjects.CWL.CommandLineTool.StreamTypes.cwlFile
            match Decode.decodeCWLDocument yaml with
            | CWLDocument.Process (CommandLineTool tool) -> Expect.equal tool.CWLVersion "v1.2" "The process keeps its version."
            | other -> failwith $"Expected a command line tool but got %A{other}"
            Expect.equal (Encode.encodeCWLDocument (Decode.decodeCWLDocument yaml)) (Encode.encodeProcessingUnit (Decode.decodeCWLProcessingUnit yaml)) "A process is written as before."
        testCase "a packed document is a decode error at the root when it is read as one process" <| fun _ ->
            match decodeError (fun () -> Decode.decodeCWLProcessingUnit fixture |> ignore) with
            | Some error ->
                Expect.equal (DecodeError.pathText error) "" "The error stands at the root."
                Expect.stringContains error.Message "decodeCWLDocument" "The error names the function that reads the document."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A packed document has no process at its root."
        testCase "a reference by #id in a document without a graph stays a run string" <| fun _ ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  s:
    run: "#tool"
    in: {}
    out: []"""
            Expect.equal (Decode.decodeWorkflow yaml).Steps.[0].Run (RunString "#tool") "Nothing names a process, so the reference is kept."
    ]

let main =
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescriptionDecode
        testCWLWorkflowDescriptionEncode
        testWorkflowUnionPortRoundTrip
        testPackedDocuments
    ]
