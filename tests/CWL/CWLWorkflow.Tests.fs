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

let mkStepInput id (source: ResizeArray<string> option) linkMerge =
    StepInput.create(id, ?source = (source |> Option.map sourceOf), ?linkMerge = linkMerge)

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
            let actual = (ListEntry.values decodeCWLWorkflowDescription.Inputs)
            // Check the simple inputs (first 8)
            for i = 0 to 7 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].InputBinding expected.[i].InputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
            // Check that sampleRecord exists
            Expect.equal actual.Count 9 "Should have 9 inputs including sampleRecord"
            Expect.equal actual.[8].Name "sampleRecord" ""
        testCase "sampleRecord complex type" <| fun _ ->
            let sampleRecordInput = (ListEntry.values decodeCWLWorkflowDescription.Inputs).[8]
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
            let workflowSteps = (ListEntry.values decodeCWLWorkflowDescription.Steps)
            testList "IDs" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = Some "MzMLToMzlite"
                    let actual = workflowSteps.[0].Id
                    Expect.equal actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = Some "PeptideSpectrumMatching"
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
                    let actual = WorkflowStep.getInputValues workflowSteps.[0]
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.equal actual.Source expected.Source ""
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
                    let actual = WorkflowStep.getInputValues workflowSteps.[1]
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.equal actual.Source expected.Source ""
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
                CWLOutput("mzlite", CWLType.Directory, outputSource = OutputSource.Single (SchemaSaladString.Literal "MzMLToMzlite/dir"))
                CWLOutput("psm", CWLType.Directory, outputSource = OutputSource.Single (SchemaSaladString.Literal "PeptideSpectrumMatching/dir"))
            |]
            let actual = (ListEntry.values decodeCWLWorkflowDescription.Outputs)
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].OutputBinding expected.[i].OutputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
                Expect.equal actual.[i].OutputSource expected.[i].OutputSource ""
        testCase "workflow intent decodes as typed field, not metadata overflow" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithIntentFile
            let intent = Expect.wantSome decoded.Intent "Intent should decode on Workflow."
            Expect.sequenceEqual intent (literals [ "primary-analysis"; "quality-control" ]) ""
            Expect.isNone decoded.Metadata "Intent should not be captured as overflow metadata."
        testCase "extended step fields decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithExtendedStepFile
            let step = (ListEntry.values decoded.Steps).[0]
            Expect.equal step.Label (Some (SchemaSaladString.Literal "Example step")) ""
            Expect.equal step.Doc (Some (DocText.literal "Step docs")) ""
            Expect.sequenceEqual step.Scatter.Value (literals [ "input1" ]) ""
            Expect.equal step.ScatterMethod (Some DotProduct) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].LoadContents (Some (SchemaSaladBool.Value true)) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].LoadListing (Some LoadListingEnum.DeepListing) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].Label (Some (SchemaSaladString.Literal "Input label")) ""
            Expect.equal (WorkflowStep.getInputValues step).[0].LinkMerge (Some MergeNested) ""
            let expectedOut = ResizeArray [| StepOutputRecord (StepOutputParameter.create "out") |]
            Expect.sequenceEqual step.Out expectedOut ""
        testCase "inline run commandline tool decode" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineRunCommandLineToolFile
            match (ListEntry.values decoded.Steps).[0].Run with
            | RunCommandLineTool toolObj ->
                match toolObj with
                | :? CWLToolDescription as tool ->
                    Expect.equal tool.CWLVersion "v1.2" ""
                | _ ->
                    failwithf "Expected CWLToolDescription payload but got %A" toolObj
            | other ->
                failwithf "Expected RunCommandLineTool but got %A" other
        testCase "an inline run takes the version of the enclosing process and ignores its own cwlVersion" <| fun _ ->
            let workflowFile =
                Kit.workflow
                    [ "requirements:"
                      "  - class: SubworkflowFeatureRequirement"
                      "inputs: {}"
                      "outputs: {}"
                      "steps:"
                      "  plain:"
                      "    run:"
                      "      class: CommandLineTool"
                      "      baseCommand: echo"
                      "      inputs: {}"
                      "      outputs: {}"
                      "    in: {}"
                      "    out: []"
                      "  own_version:"
                      "    run:"
                      "      class: CommandLineTool"
                      "      cwlVersion: v1.0"
                      "      baseCommand: echo"
                      "      inputs: {}"
                      "      outputs: {}"
                      "    in: {}"
                      "    out: []"
                      "  nested:"
                      "    run:"
                      "      class: Workflow"
                      "      inputs: {}"
                      "      outputs: {}"
                      "      steps:"
                      "        deep:"
                      "          run:"
                      "            class: CommandLineTool"
                      "            baseCommand: echo"
                      "            inputs: {}"
                      "            outputs: {}"
                      "          in: {}"
                      "          out: []"
                      "    in: {}"
                      "    out: []" ]
            let decoded = Decode.decodeWorkflow workflowFile
            let step (id: string) = (ListEntry.values decoded.Steps) |> Seq.find (fun s -> s.Id = Some id)
            let tool (run: WorkflowStepRun) = Expect.wantSome (WorkflowStepRunOps.tryGetTool run) "Expected an inline tool"
            Expect.equal (tool (step "plain").Run).CWLVersion "v1.2" "A run without a version takes the version of the workflow."
            Expect.equal (tool (step "own_version").Run).CWLVersion "v1.2" "A run that names v1.0 uses the version of the workflow."
            let nested = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow (step "nested").Run) "Expected an inline workflow"
            Expect.equal nested.CWLVersion "v1.2" "An inline workflow without a version takes the version of the enclosing one."
            Expect.equal (tool (ListEntry.values nested.Steps).[0].Run).CWLVersion "v1.2" "The process in it takes that version."
        testCase "a process without a cwlVersion is a decode error when no process encloses it" <| fun _ ->
            let yaml = """class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}"""
            match Kit.decodeError (fun () -> Decode.decodeCommandLineTool yaml) with
            | Some error ->
                Expect.stringContains error.Message "cwlVersion" "The error names the missing field."
                Expect.isSome error.Span "The error has the place in the text."
            | None -> failwith "A process at the root needs a cwlVersion."
        testCase "the cwlVersion at the root is a decode error at the field when it has no value or holds a number, a bool, a null, a list or a mapping" <| fun _ ->
            let tool (field: string) = "cwlVersion:" + field + "\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}"
            let cases =
                [ "a list", " [v1.2]", None
                  "a mapping", " {version: v1.2}", None
                  "a float", " 1.2", Some "Expected a string, found the plain scalar 1.2"
                  "an int", " 1", Some "Expected a string, found the plain scalar 1"
                  "a bool", " true", Some "Expected a string, found the plain scalar true"
                  "a null", " ~", Some "Expected a value, found"
                  "no value", "", Some "Expected a value, found" ]
            for (name, field, message) in cases do
                let actual = Kit.decodeFailsWith name (fun () -> Decode.decodeCWLProcessingUnit (tool field)) "cwlVersion" ""
                message |> Option.iter (fun text -> Expect.stringContains actual text $"{name}: the error should say that the value is missing.")
        testCase "the cwlVersion at the root is read as written and every document decodes under the rules of v1.2" <| fun _ ->
            // A NetworkAccess requirement exists from v1.1, so the documents of v1.0 and v1.3 show that the version selects no rule.
            for version in [ "v1.0"; "v1.1"; "v1.2"; "v1.2.0-dev2"; "v1.3"; "v1.3.0-dev1"; "banana" ] do
                let tool = "cwlVersion: " + version + "\nclass: CommandLineTool\nrequirements:\n  - class: NetworkAccess\n    networkAccess: true\nbaseCommand: echo\ninputs: {}\noutputs: {}"
                let decodedTool = Decode.decodeCommandLineTool tool
                Expect.equal decodedTool.CWLVersion version $"A process decodes with {version}."
                Expect.isSome decodedTool.Requirements $"A process of {version} keeps its NetworkAccess requirement."
                Expect.stringContains (Encode.encodeToolDescription decodedTool) ("cwlVersion: " + version) $"A process writes {version} back."
        testCase "a quoted number at the root is a text that is read as written, and the plain number is a decode error" <| fun _ ->
            let tool (field: string) = "cwlVersion: " + field + "\nclass: CommandLineTool\nbaseCommand: echo\ninputs: {}\noutputs: {}"
            let decoded = Decode.decodeCommandLineTool (tool "\"1.2\"")
            Expect.equal decoded.CWLVersion "1.2" "The quoted number is a text."
            Expect.stringContains (Encode.encodeToolDescription decoded) "cwlVersion: \"1.2\"" "The text is written in quotes, so that it reads back as a text."
            Expect.equal (Decode.decodeCommandLineTool (Encode.encodeToolDescription decoded)).CWLVersion "1.2" "The text reads back as the same version."
            match Kit.decodeError (fun () -> Decode.decodeCommandLineTool (tool "1.2")) with
            | Some error -> Expect.stringContains error.Message "Expected a string, found the plain scalar 1.2" "The plain number is no string."
            | None -> failwith "A plain number at the root must fail."
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
                let tool = Expect.wantSome (WorkflowStepRunOps.tryGetTool (ListEntry.values decoded.Steps).[0].Run) "The step runs an inline tool."
                Expect.equal tool.CWLVersion "v1.2" $"{name}: the inline run takes the version of the workflow."
        testCase "the cwlVersion of an inline run is a decode error unless it is a string or a null" <| fun _ ->
            let run (field: string) =
                String.concat "\n" [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"; "steps:"; "  s:"; "    run:"; "      class: CommandLineTool"; "      cwlVersion:" + field; "      baseCommand: echo"; "      inputs: {}"; "      outputs: {}"; "    in: {}"; "    out: []" ]
            for (name, field, message) in
                [ "a float", " 1.2", "Expected a string, found the plain scalar 1.2"
                  "an int", " 1", "Expected a string, found the plain scalar 1"
                  "a bool", " true", "Expected a string, found the plain scalar true"
                  "a list", " [v1.2]", "Expected a string, found a sequence"
                  "a mapping", " {version: v1.2}", "Expected a string, found a mapping" ] do
                Kit.processFails (name + " in a run") (run field) "steps.s.run.cwlVersion" message
            for field in [ ""; " ~"; " \"1.2\""; " v1.0" ] do
                Decode.decodeCWLProcessingUnit (run field) |> ignore
        testCase "step input array form decode" <| fun _ ->
            let workflowWithInputArrayStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      - id: in1
        source: input1
        label: Input in array syntax
    out: [out]"""
            let decoded = Decode.decodeWorkflow workflowWithInputArrayStepFile
            let stepInput = (WorkflowStep.getInputValues (ListEntry.values decoded.Steps).[0]).[0]
            Expect.equal stepInput.Id "in1" ""
            Expect.equal stepInput.Source (Some (sourceOf [ "input1" ])) ""
            Expect.equal stepInput.Label (Some (SchemaSaladString.Literal "Input in array syntax")) ""
        testCase "workflow with no steps decodes to empty" <| fun _ ->
            let yaml = TestObjects.CWL.Workflow.workflowWithNoStepsFile
            let decoded = Decode.decodeWorkflow yaml
            Expect.equal (ListEntry.values decoded.Steps).Count 0 ""
        testCase "sequence-form workflow ports with comments decode" <| fun _ ->
            let workflowWithSequencePortsAndCommentsFile = """#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: Workflow
doc: |
  Workflow doc line.
  # This line is doc text, not a YAML comment.
inputs:
# Primary input
- id: input_data
  type: File
  doc: A file input for processing
- id: param_value
  type: double
  default: 0.5
outputs:
# Primary output
- id: result_data
  type: File
  outputSource: process_step/result_data
- id: merged_output
  type: File
  outputSource: combine_step/merged_output
steps:
# First step
- id: process_step
  run: ./tools/process_step.cwl
  in: []
  out: [result_data]
# Second step
- id: combine_step
  run: ./tools/combine_step.cwl
  in:
  - id: component_files
    source:
    - process_step/result_data
  - id: action
    valueFrom: "merge_properties"
  out: [merged_output]"""
            let decoded = Decode.decodeWorkflow workflowWithSequencePortsAndCommentsFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 2 "Sequence-form inputs should decode."
            Expect.equal (ListEntry.values decoded.Outputs).Count 2 "Sequence-form outputs should decode."
            Expect.equal (ListEntry.values decoded.Steps).Count 2 "Sequence-form steps should decode after comments."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "input_data")) "input_data should decode."
            Expect.isTrue ((ListEntry.values decoded.Outputs) |> Seq.exists (fun output -> output.Name = "result_data")) "result_data output should decode."
            Expect.equal decoded.Doc (Some (DocText.literal "Workflow doc line.\n# This line is doc text, not a YAML comment.\n")) "Block scalar comment-looking text should be preserved."
        testCase "sequence-form workflow fields with blank separator lines decode" <| fun _ ->
            let workflowWithBlankSeparatedSequenceFieldsFile = """cwlVersion: v1.2
class: Workflow
inputs:


- id: trial_id
  type: string

- id: sensorthingsapi_url
  type: string
outputs:

- id: result
  type: File
  outputSource: collect/result
steps:

- id: collect
  run: ./collect.cwl
  in:

  - id: trial_id
    source: trial_id

  - id: sensorthingsapi_url
    source: sensorthingsapi_url
  out: [result]"""
            let decoded = Decode.decodeWorkflow workflowWithBlankSeparatedSequenceFieldsFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 2 "Blank lines before and between sequence-form inputs should decode."
            Expect.equal (ListEntry.values decoded.Outputs).Count 1 "Blank lines before sequence-form outputs should decode."
            Expect.equal (ListEntry.values decoded.Steps).Count 1 "Blank lines before sequence-form steps should decode."
            Expect.equal (ListEntry.values decoded.Steps).[0].In.Count 2 "Blank lines before and between nested step inputs should decode."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "trial_id")) "trial_id should decode."
            Expect.equal (ListEntry.values decoded.Steps).[0].Id (Some "collect") "The step following blank separators should stay mapped under steps."
        testCase "comment-separated map-form workflow inputs decode" <| fun _ ->
            let workflowWithLowerIndentedCommentMapInputsFile = """cwlVersion: v1.2
class: Workflow
inputs:
  first:
    type: string
# group comment between map entries
  second:
    type: string
outputs: []
steps: {}"""
            let decoded = Decode.decodeWorkflow workflowWithLowerIndentedCommentMapInputsFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 2 "Comment-separated map-form inputs should decode."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "first")) "first input should decode."
            Expect.isTrue ((ListEntry.values decoded.Inputs) |> Seq.exists (fun input -> input.Name = "second")) "second input should decode."
        testCase "comment before sequence-form workflow inputs decodes" <| fun _ ->
            let workflowWithLowerIndentedCommentBeforeSequenceInputFile = """cwlVersion: v1.2
class: Workflow
inputs:
# group comment before sequence item
  - id: first
    type: string
outputs: []
steps: {}"""
            let decoded = Decode.decodeWorkflow workflowWithLowerIndentedCommentBeforeSequenceInputFile
            Expect.equal (ListEntry.values decoded.Inputs).Count 1 "Comment before sequence-form inputs should decode."
            Expect.equal (ListEntry.values decoded.Inputs).[0].Name "first" "first input should decode."
        testCase "multiline valueFrom expression decodes as one string" <| fun _ ->
            let workflowWithMultilineValueFromFile = """cwlVersion: v1.2
class: Workflow
inputs:
  source_reads:
    type: File?
outputs: []
steps:
  expr_step:
    run:
      class: ExpressionTool
      inputs:
        reads:
          type: File?
      outputs:
        out:
          type: File[]?
      expression: |
        ${ return {"out": inputs.reads ? [inputs.reads] : null}; }
    in:
      reads:
        source: source_reads
        valueFrom:
          ${
            var reads = null;
            if (self !== null) { reads = [self]; }
            return reads;
          }
    out: [out]"""
            let decoded = Decode.decodeWorkflow workflowWithMultilineValueFromFile
            let step = (ListEntry.values decoded.Steps) |> Seq.find (fun step -> step.Id = Some "expr_step")
            let stepInput = WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = "reads")
            let valueFrom = (Expect.wantSome stepInput.ValueFrom "valueFrom should decode.").Value
            Expect.stringContains valueFrom "${" "valueFrom should include the expression opener."
            Expect.stringContains valueFrom "return reads;" "valueFrom should include the expression body."
            Expect.stringContains valueFrom "}" "valueFrom should include the expression closer."
        testCase "optional array shorthand input types decode" <| fun _ ->
            let workflowWithOptionalArrayInputTypesFile = """cwlVersion: v1.2
class: Workflow
inputs:
  optional_files:
    type: File[]?
  optional_dirs:
    type: Directory[]?
outputs: []
steps: {}"""
            let decoded = Decode.decodeWorkflow workflowWithOptionalArrayInputTypesFile
            let assertOptionalArray name expectedItem =
                let input = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = name)
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
            let workflowWithInlineSchemaUnionTypesFile = """cwlVersion: v1.2
class: Workflow
inputs:
  mode:
    type:
      - "null"
      - type: enum
        name: Mode
        symbols: [fast, stringent]
  record_payload:
    type:
      - "null"
      - type: record
        name: Payload
        fields:
          sample_id: string
          files:
            type: File[]
outputs: []
steps: {}"""
            let decoded = Decode.decodeWorkflow workflowWithInlineSchemaUnionTypesFile
            let mode = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = "mode")
            match mode.Type_ with
            | Some (Union types) ->
                Expect.isTrue (types |> Seq.exists ((=) Null)) "mode should include null in the union."
                match types |> Seq.choose (function | Enum schema -> Some schema | _ -> None) |> Seq.toList with
                | [enumSchema] ->
                    Expect.equal enumSchema.Name (Some "Mode") "Inline enum name should decode."
                    Expect.sequenceEqual (InputEnumSchema.getSymbolValues enumSchema) (ResizeArray [| "fast"; "stringent" |]) "Inline enum symbols should decode."
                | other -> failwithf "Expected one inline enum schema, got %A" other
            | other -> failwithf "Expected mode to decode as a union, got %A" other
            let recordPayload = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = "record_payload")
            match recordPayload.Type_ with
            | Some (Union types) ->
                Expect.isTrue (types |> Seq.exists ((=) Null)) "record_payload should include null in the union."
                match types |> Seq.choose (function | Record schema -> Some schema | _ -> None) |> Seq.toList with
                | [recordSchema] ->
                    Expect.equal recordSchema.Name (Some "Payload") "Inline record name should decode."
                    let fields = Expect.wantSome recordSchema.Fields "Inline record fields should decode." |> ListEntry.values
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
        testCase "decodeWorkflowWithWarnings keeps a step without an id, because cwltool accepts the step" <| fun _ ->
            let workflowWithUnnamedStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
- id: valid_input
  type: string
outputs:
- id: valid_output
  type: File
  outputSource: step1/out
steps:
- run: ./unnamed.cwl # about it
  in: []
  out: []
- id: step1
  run: ./tool.cwl
  in:
  - id: in1
    source: valid_input
  out: [out]"""
            let result = Decode.decodeWorkflowWithWarnings workflowWithUnnamedStepFile
            Expect.equal (ListEntry.values result.Value.Inputs).Count 1 "The input should decode."
            Expect.equal (ListEntry.values result.Value.Outputs).Count 1 "The output should decode."
            let steps = ListEntry.values result.Value.Steps
            Expect.equal (steps |> Seq.map (fun step -> step.Id) |> List.ofSeq) [ None; Some "step1" ] "Both steps decode, the one without an id first."
            Expect.equal steps.[0].Run (RunString "./unnamed.cwl") "The step without an id has its run."
            Expect.equal steps.[0].In.Count 0 "The step without an id has its inputs."
            Expect.equal steps.[0].Out.Count 0 "The step without an id has its outputs."
            Expect.equal result.Warnings.Count 0 "No step is left out, so there is no warning."
        testCase "malformed named input still fails" <| fun _ ->
            let workflowWithMalformedNamedInputFile = """cwlVersion: v1.2
class: Workflow
inputs:
- id: broken_input
  doc: Missing required type.
outputs:
- id: result
  type: File
  outputSource: step1/out
steps:
- id: step1
  run: ./tool.cwl
  in: []
  out: [out]"""
            Expect.throws
                (fun _ -> Decode.decodeWorkflow workflowWithMalformedNamedInputFile |> ignore)
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
            let input = (ListEntry.values decoded.Inputs) |> Seq.find (fun input -> input.Name = "sample")
            let output = (ListEntry.values decoded.Outputs) |> Seq.find (fun output -> output.Name = "result")
            let step = (ListEntry.values decoded.Steps) |> Seq.find (fun step -> step.Id = Some "step1")
            let stepInput = WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = "sample")
            let inputBinding = inputBindingOf input
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
            let step = (ListEntry.values decoded.Steps).[0]
            let stepInput = (WorkflowStep.getInputValues step).[0]
            Expect.equal step.Id (Some "step1") ""
            Expect.equal step.When_ (Some (SchemaSaladString.Literal "$(inputs.input1 != null)")) ""
            Expect.equal stepInput.PickValue (Some FirstNonNull) ""
            Expect.equal stepInput.Doc (Some (DocText.literal "Input docs")) ""
            let defaultValue =
                stepInput.DefaultValue
                |> Option.bind tryYamlScalarString
            Expect.equal defaultValue None "Default is structured object and should not collapse to scalar"
        testCase "inline run workflow decode" <| fun _ ->
            let yaml = TestObjects.CWL.Workflow.workflowWithInlineRunWorkflowFile
            let decoded = Decode.decodeWorkflow yaml
            let runValue = (ListEntry.values decoded.Steps).[0].Run
            let isRunWorkflow =
                match runValue with
                | RunWorkflow _ -> true
                | _ -> false
            Expect.isTrue isRunWorkflow $"Expected RunWorkflow but got %A{runValue}"
            let workflow = Expect.wantSome (WorkflowStepRunOps.tryGetWorkflow runValue) "Expected inline workflow payload"
            Expect.equal (ListEntry.values workflow.Steps).Count 1 ""
            Expect.equal (ListEntry.values workflow.Steps).[0].Id (Some "inner") ""
        testCase "workflow outputSource array form decodes and roundtrips" <| fun _ ->
            let workflowWithOutputSourceArrayFile = """cwlVersion: v1.2
class: Workflow
requirements:
  - class: MultipleInputFeatureRequirement
inputs: {}
outputs:
  merged:
    type: string
    outputSource:
      - step1/out
      - step2/out
steps:
  step1:
    run: ./tool-a.cwl
    in: {}
    out: [out]
  step2:
    run: ./tool-b.cwl
    in: {}
    out: [out]"""
            let decoded = Decode.decodeWorkflow workflowWithOutputSourceArrayFile
            let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
            Expect.stringContains encoded "outputSource:" "Array-form outputSource should be emitted."
            let mergedOutput =
                (ListEntry.values roundTripped.Outputs)
                |> Seq.find (fun output -> output.Name = "merged")
            let outputSources = mergedOutput.GetOutputSources()
            Expect.sequenceEqual outputSources (ResizeArray [| "step1/out"; "step2/out" |]) "outputSource entries should survive decode/encode/decode."
        testCase "workflow step run Operation class decodes and roundtrips" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithInlineRunOperationFile
            let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
            Expect.stringContains encoded "class: Operation" "Operation run should roundtrip with class marker."
            Expect.equal (ListEntry.values roundTripped.Steps).[0].Id (Some "op") "Operation-backed step should survive roundtrip."
        testCase "invalid pickValue fails decode" <| fun _ ->
            let workflowWithInvalidPickValueFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1:
        source: input1
        pickValue: invalid_pick
    out: [out]"""
            let invalidPickValue = workflowWithInvalidPickValueFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidPickValue |> ignore) "Invalid pickValue should fail"
        testCase "invalid scatterMethod fails decode" <| fun _ ->
            let workflowWithInvalidScatterMethodFile = """cwlVersion: v1.2
class: Workflow
inputs: { input1: string }
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: input1
    scatterMethod: invalid_scatter
    in:
      in1: input1
    out: [out]"""
            let invalidScatter = workflowWithInvalidScatterMethodFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidScatter |> ignore) "Invalid scatterMethod should fail"
        testCase "inline ExpressionTool run decode succeeds" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnsupportedInlineRunClassFile
            let runValue = (ListEntry.values decoded.Steps).[0].Run
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
            Expect.equal (ListEntry.values decoded.Steps).Count 3 "Should have 3 steps"
            for i in 0..2 do
                let step = (ListEntry.values decoded.Steps).[i]
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool step.Run)
                        $"Step {i} should be ExpressionTool"
                Expect.isTrue (expressionTool.Expression.Value.Length > 0) $"Step {i} expression should be non-empty"
        testCase "loadContents on inline ExpressionTool input decoded" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithLoadContentsExpressionToolFile
            let step = (ListEntry.values decoded.Steps).[0]
            let expressionTool =
                Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool step.Run) "Step run should be inline ExpressionTool"
            let hasLoadContentsOnStepInput =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.LoadContents = Some (SchemaSaladBool.Value true))
            let hasValueFromOnStepInput =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.ValueFrom.IsSome)
            Expect.isTrue hasLoadContentsOnStepInput "Step input should have loadContents"
            Expect.isTrue hasValueFromOnStepInput "Step input should have valueFrom"
            let expressionToolInputs = expressionTool.Inputs |> Option.defaultValue (ResizeArray())
            Expect.equal expressionToolInputs.Count 1 "Inline expression tool should have one input"
        testCase "mixed workflow with CommandLineTool and ExpressionTool steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
            Expect.equal (ListEntry.values decoded.Steps).Count 2 ""
            match (ListEntry.values decoded.Steps).[0].Run with
            | RunString _ -> ()
            | other -> Expect.isTrue false $"Step 0 should be RunString but got %A{other}"
            let isExpressionToolStep =
                match (ListEntry.values decoded.Steps).[1].Run with
                | RunExpressionTool _ -> true
                | _ -> false
            Expect.isTrue isExpressionToolStep "Step 1 should be RunExpressionTool"
        testCase "chained ExpressionTool array pipeline type verification" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
            let step1 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values decoded.Steps).[0].Run) "Step 1 should be ExpressionTool"
            let step2 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values decoded.Steps).[1].Run) "Step 2 should be ExpressionTool"
            let step3 = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values decoded.Steps).[2].Run) "Step 3 should be ExpressionTool"

            Expect.equal (ListEntry.values step1.Inputs.Value).[0].Type_ (Some CWLType.Int) "step1 input should be int"
            match (ListEntry.values step1.Outputs).[0].Type_ with
            | Some (Array arraySchema) ->
                Expect.equal arraySchema.Items CWLType.Int "step1 output should be int[]"
            | other ->
                Expect.isTrue false $"Expected int[] but got %A{other}"

            // Asserted via `if`/bool rather than a `match` with a unit success branch: Fable's
            // Python backend drops empty branches from a match in statement position, which would
            // leave `Expect.isTrue false` unconditional and make this assertion always fail.
            let step2InputType = (ListEntry.values step2.Inputs.Value).[0].Type_
            let isArray = match step2InputType with Some (Array _) -> true | _ -> false
            Expect.isTrue isArray $"step2 input should be int[] but got %A{step2InputType}"

            Expect.equal (ListEntry.values step3.Outputs).[0].Type_ (Some CWLType.Int) "step3 output should be int"
        testCase "step-level loadContents with valueFrom on ExpressionTool" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithLoadContentsExpressionToolFile
            let step = (ListEntry.values decoded.Steps).[0]
            let stepInputHasLoadContents =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.LoadContents = Some (SchemaSaladBool.Value true))
            let stepInputHasValueFrom =
                WorkflowStep.getInputValues step |> Seq.exists (fun si -> si.ValueFrom.IsSome)
            Expect.isTrue stepInputHasLoadContents "Step input should have loadContents"
            Expect.isTrue stepInputHasValueFrom "Step input should have valueFrom"
        testCase "advanced workflow fixture decodes and preserves full content" <| fun _ ->
            let advancedWorkflowFile = """#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: Workflow
id: "CWL_advanced_layout_workflow"

$namespaces:
  cwltool: "http://commonwl.org/cwltool#"
  arc: "https://github.com/nfdi4plants/ARC_ontology"

$schemas:
  - "https://raw.githubusercontent.com/nfdi4plants/ARC_ontology/main/ARC_v2.0.owl"
  - "https://raw.githubusercontent.com/common-workflow-language/cwltool/main/cwltool/extensions.yml"

hints:
  - class: DockerRequirement
    dockerImageId: "devcontainer"
    dockerFile:
      $include: "./Dockerfile"
    cwltool:dockerRunOptions:
      - "--gpus=all"
      # - "--memory=120g"
      # - "--memory-swap=128g"
  - class: cwltool:CUDARequirement
    cudaComputeCapability: '8.9'
    cudaDeviceCountMin: 1
    cudaVersionMin: "12.3"
  - class: EnvVarRequirement
    envDef:
      NVIDIA_VISIBLE_DEVICES: "all"
      NVIDIA_DRIVER_CAPABILITIES: "compute,utility"
      VIRTUAL_ENV: /workspace/venv
      PATH: /workspace/venv/bin:/usr/local/nvidia/bin:/usr/local/cuda/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin

requirements:
  # - class: ResourceRequirement
  #   ramMin: 120000   # in megabytes
  #   coresMin: 4
  - class: MultipleInputFeatureRequirement
  - class: StepInputExpressionRequirement
  - class: InitialWorkDirRequirement
    listing:
      - entryname: arc
        #entry: "/workspaces/example-layout
        entry: $(inputs.rootDir)
        writable: true
  - class: NetworkAccess
    networkAccess: true

inputs:

  rootDir:
    type: Directory
    doc: "Root directory of the working area containing all inputs and outputs."

  auxDir:
    type: Directory
    doc: "Directory containing auxiliary runtime resources."

  inputDirA:
    type: Directory
    doc: "Input directory containing source files for processing."

  inputFileA:
    type: File
    doc: "Input table describing the source files to be processed."

  outputDirA:
    type: string
    doc: "Output directory for the first generated directory payload."

  outputDirB:
    type: string
    doc: "Output directory for the second generated directory payload."

  supportDirA:
    type: Directory
    doc: "Directory containing supporting model or configuration files."

  scalarA:
    type: float
    doc: "Threshold value controlling a decision boundary."

  integerA:
    type: int
    doc: "Primary integer setting preserved for scalar layout coverage."

  integerB:
    type: int
    doc: "Secondary integer setting preserved for scalar layout coverage."

  integerC:
    type: int
    doc: "Tertiary integer setting preserved for scalar layout coverage."

  integerD:
    type: int
    doc: "Fourth integer setting preserved for scalar layout coverage."

  integerE:
    type: int
    doc: "Fifth integer setting preserved for scalar layout coverage."

  integerF:
    type: int
    doc: "Sixth integer setting preserved for scalar layout coverage."

  integerArrayA:
    type: int[]
    doc: "Integer array setting preserved to exercise array input layout."

  outputFileA:
    type: string
    doc: "Path where the first generated file payload will be stored."

  outputFileB:
    type: string
    doc: "Path where the second generated file payload will be stored."

  outputDirC:
    type: string
    doc: "Output directory for the third generated directory payload."

  integerG:
    type: int
    doc: "Seventh integer setting preserved for scalar layout coverage."

  integerH:
    type: int
    doc: "Eighth integer setting preserved for scalar layout coverage."

  outputDirD:
    type: string
    doc: "Output directory for the fourth generated directory payload."

  inputDirB:
    type: string
    doc: "Additional input directory used in a later stage."

  outputDirE:
    type: string
    doc: "Output directory for the fifth generated directory payload."

  outputDirF:
    type: string
    doc: "Final output directory name consumed by downstream stages."


steps:
  step_alpha:
    run: ./cwl_tools/step_alpha.cwl
    in:
      rootDir:                       rootDir
      inputDirA:                     inputDirA
      inputFileA:                    inputFileA
      outputDirA:                    outputDirA
    out:
      - out_dir_alpha
      # - out_dir_alpha_children
      # - out_dir_alpha_files
      - out_mount

  step_beta:
    run: ./cwl_tools/step_beta.cwl
    in:
      rootDir:                       step_alpha/out_mount
      outputDirA:                    step_alpha/out_dir_alpha
      inputFileA:                    inputFileA
      outputDirB:                    outputDirB
      auxDir:                        auxDir
      supportDirA:                   supportDirA
      scalarA:                       scalarA
      integerA:                      integerA
      integerB:                      integerB
      integerC:                      integerC
      integerD:                      integerD
      integerE:                      integerE
      integerF:                      integerF
      integerArrayA:                 integerArrayA
    out:
      - out_dir_beta
      # - out_dir_beta_children
      # - out_dir_beta_files
      - out_mount

  step_gamma:
    run: ./cwl_tools/step_gamma.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_beta/out_mount
      outputDirA:                    step_alpha/out_dir_alpha
      inputFileA:                    inputFileA
      outputDirB:                    step_beta/out_dir_beta
      outputFileA:                   outputFileA
    out:
      - out_file_alpha
      - out_mount

  step_delta:
    run: ./cwl_tools/step_delta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_gamma/out_mount
      inputFileB: step_gamma/out_file_alpha
      outputDirC: outputDirC
    out:
      - out_dir_gamma
      # - out_dir_gamma_plot
      # - out_dir_gamma_table
      - out_mount

  step_epsilon:
    run: ./cwl_tools/step_epsilon.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_delta/out_mount
      outputDirA:                    step_alpha/out_dir_alpha
      inputFileA:                    inputFileA
      outputDirB:                    step_beta/out_dir_beta
      outputFileB:                   outputFileB
    out:
      - out_file_beta
      - out_mount

  step_zeta:
    run: ./cwl_tools/step_zeta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_epsilon/out_mount
      inputFileC: step_gamma/out_file_alpha
      integerG:                     integerG
      integerH:                     integerH
      outputDirD:                   outputDirD
    out:
      - out_dir_delta
      # - ot_distance_matrix
      # - out_file_pairwise
      # - out_file_global
      # - out_file_condition
      # - permanova_results
      # - mds_eigenvalues
      # - mds_eigenvalues_summary
      # - mds_2d
      - out_mount

  step_eta:
    run: ./cwl_tools/step_eta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_zeta/out_mount
      inputDirB:                    inputDirB
      outputDirE :                  outputDirE
    out:
      - out_dir_epsilon
      # - ot_distance_matrix_html
      # - permanova_pvalues_log_html
      # - ot_mds_scatter_html
      # - out_file_global_html
      # - out_file_condition_html
      - out_mount

  step_theta:
    run: ./cwl_tools/step_theta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_eta/out_mount
      inputFileD:                    step_gamma/out_file_alpha
      outputDirF:                    outputDirF
    out:
      - out_file_gamma
      - out_file_delta
      - out_file_epsilon
      - out_mount

  step_iota:
    run: ./cwl_tools/step_iota.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_theta/out_mount
      inputFileE:                    step_gamma/out_file_alpha
      outputDirF:                    outputDirF
    out:
      - out_file_zeta
      - out_mount

  step_kappa:
    run: ./cwl_tools/step_kappa.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_iota/out_mount
      inputFileF:                    step_gamma/out_file_alpha
      outputDirF:                    outputDirF
    out:
      - out_file_eta
      - out_mount

  step_lambda:
    run: ./cwl_tools/step_lambda.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      in_mount:
        source:
          - step_iota/out_mount # step_kappa/out_mount
      in_dirs:
        # linkMerge: merge_nested
        source:
          - step_alpha/out_dir_alpha
          - step_beta/out_dir_beta
          - step_delta/out_dir_gamma
          - step_zeta/out_dir_delta
          - step_eta/out_dir_epsilon
      in_files:
        linkMerge: merge_flattened
        source:
          - step_gamma/out_file_alpha
          - step_epsilon/out_file_beta
          - step_theta/out_file_gamma
          - step_theta/out_file_delta
          - step_theta/out_file_epsilon
          - step_iota/out_file_zeta
          - step_kappa/out_file_eta
      in_name:
        valueFrom: $(inputs.outputDirF)


    out:
      - final_dir

outputs:
  final_dir:
    type: Directory
    doc: "Folder containing all final results"
    outputSource: step_lambda/final_dir

arc:has technology type:
  - class: arc:technology type
    arc:annotation value: "Docker Container"

arc:technology platform: "Python"

arc:performer:
  - class: arc:Person
    arc:first name: "Example"
    arc:last name: "Researcher"
    arc:email: "workflow.author@example.org"
    arc:affiliation: "Example Research Institute"
    arc:has role:
      - class: arc:role
        arc:term accession: "https://credit.niso.org/contributor-roles/formal-analysis/"
        arc:annotation value: "Formal analysis" """
            let original = advancedWorkflowFile
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
                Expect.equal dockerRequirement.DockerImageId (Some (SchemaSaladString.Literal "devcontainer")) "Docker hint should keep dockerImageId."
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

            let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription workflow

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
                |> Seq.map (Encode.encodeWorkflowStep >> (fun (id, node) -> Option.get id, node) >> renderYamlPair)
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
            Expect.sequenceEqual (encodeInputs (ListEntry.values roundTripped.Inputs)) (encodeInputs (ListEntry.values workflow.Inputs)) "Workflow inputs should roundtrip."
            Expect.sequenceEqual (encodeSteps (ListEntry.values roundTripped.Steps)) (encodeSteps (ListEntry.values workflow.Steps)) "Workflow steps should roundtrip."
            Expect.sequenceEqual (encodeOutputs (ListEntry.values roundTripped.Outputs)) (encodeOutputs (ListEntry.values workflow.Outputs)) "Workflow outputs should roundtrip."
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
                let missing = (ListEntry.values decodedModified.Outputs) |> Seq.filter (fun o -> o.OutputSource.IsNone) |> Seq.toList
                Expect.isTrue (missing.Length > 0) "At least one output should be missing outputSource after manual removal"
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
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                let step = (ListEntry.values roundTripped.Steps).[0]
                let stepInput = (WorkflowStep.getInputValues step).[0]
                Expect.equal step.When_ (Some (SchemaSaladString.Literal "$(inputs.input1 != null)")) ""
                Expect.equal stepInput.PickValue (Some FirstNonNull) ""
                Expect.equal stepInput.Doc (Some (DocText.literal "Input docs")) ""
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
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                Expect.stringContains encoded "intent:" "Workflow intent should be present in encoded output"
                let intent = Expect.wantSome roundTripped.Intent "Workflow intent should survive roundtrip"
                Expect.sequenceEqual intent (literals [ "primary-analysis"; "quality-control" ]) ""
            testCase "workflow direct overflow and nested port overflow roundtrip" <| fun _ ->
                let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnknownPortFieldsFile
                DynObj.setProperty "arc:direct note" "workflow note" decoded
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let metadata = Expect.wantSome roundTripped.Metadata "Decoded direct overflow should be available as metadata."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:direct note" metadata) (Some "workflow note") "Workflow direct DynamicObj overflow should encode."

                let input = (ListEntry.values roundTripped.Inputs) |> Seq.find (fun input -> input.Name = "sample")
                let output = (ListEntry.values roundTripped.Outputs) |> Seq.find (fun output -> output.Name = "result")
                let step = (ListEntry.values roundTripped.Steps) |> Seq.find (fun step -> step.Id = Some "step1")
                let stepInput = WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = "sample")
                let inputBinding = inputBindingOf input

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" input) (Some "keep input note") "Input overflow should roundtrip."
                Expect.equal input.Type_ (Some CWLType.File) "The input type should roundtrip."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:binding note" inputBinding) (Some "keep input binding note") "Input binding overflow should roundtrip."

                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" output) (Some "keep output note") "Output overflow should roundtrip."
                Expect.equal output.Type_ (Some CWLType.File) "The output type should roundtrip."

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
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                let input = (ListEntry.values roundTripped.Inputs) |> Seq.find (fun input -> input.Name = "sample")
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
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                let input = (ListEntry.values roundTripped.Inputs) |> Seq.find (fun input -> input.Name = "sample")
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
                        let workflowWithPickValueMethodFile (pickValue: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1:
        source: input1
        pickValue: {pickValue}
    out: [out]"""
                        let yaml = workflowWithPickValueMethodFile cwlString
                        let decoded = Decode.decodeWorkflow yaml
                        Expect.equal (WorkflowStep.getInputValues (ListEntry.values decoded.Steps).[0]).[0].PickValue (Some pickValueMethod) ""
                        let encoded = Encode.encodeWorkflowDescription decoded
                        Expect.stringContains encoded $"pickValue: {cwlString}" ""
                        let roundTripped = Decode.decodeWorkflow encoded
                        Expect.equal (WorkflowStep.getInputValues (ListEntry.values roundTripped.Steps).[0]).[0].PickValue (Some pickValueMethod) ""
            ]
            testList "ScatterMethod roundtrip" [
                for (scatterMethod, cwlString) in [
                    DotProduct, "dotproduct"
                    NestedCrossProduct, "nested_crossproduct"
                    FlatCrossProduct, "flat_crossproduct"
                ] do
                    testCase cwlString <| fun _ ->
                        let workflowWithScatterMethodFile (scatterMethod: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: in1
    scatterMethod: {scatterMethod}
    in:
      in1:
        source: input1
    out: [out]"""
                        let yaml = workflowWithScatterMethodFile cwlString
                        let decoded = Decode.decodeWorkflow yaml
                        Expect.equal (ListEntry.values decoded.Steps).[0].ScatterMethod (Some scatterMethod) ""
                        let encoded = Encode.encodeWorkflowDescription decoded
                        Expect.stringContains encoded $"scatterMethod: {cwlString}" ""
                        let roundTripped = Decode.decodeWorkflow encoded
                        Expect.equal (ListEntry.values roundTripped.Steps).[0].ScatterMethod (Some scatterMethod) ""
            ]
            testCase "structured default with array value roundtrips" <| fun _ ->
                let workflowWithStructuredArrayDefaultFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1:
        source: input1
        default: [1, 2, 3]
    out: [out]"""
                let yaml = workflowWithStructuredArrayDefaultFile
                let decoded = Decode.decodeWorkflow yaml
                let decodedDefault =
                    (WorkflowStep.getInputValues (ListEntry.values decoded.Steps).[0]).[0].DefaultValue
                    |> Option.bind tryYamlScalarSequence
                    |> Option.defaultValue (ResizeArray())
                Expect.sequenceEqual decodedDefault (ResizeArray [|"1"; "2"; "3"|]) ""
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                let roundTrippedDefault =
                    (WorkflowStep.getInputValues (ListEntry.values roundTripped.Steps).[0]).[0].DefaultValue
                    |> Option.bind tryYamlScalarSequence
                    |> Option.defaultValue (ResizeArray())
                Expect.sequenceEqual roundTrippedDefault (ResizeArray [|"1"; "2"; "3"|]) ""
            testCase "when expression with embedded quotes roundtrips" <| fun _ ->
                let expression = "$(inputs.name == 'test')"
                let workflowWithWhenExpressionFile (whenExpression: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  name: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    when: "{whenExpression}"
    in:
      in1: name
    out: [out]"""
                let yaml = workflowWithWhenExpressionFile expression
                let decoded = Decode.decodeWorkflow yaml
                Expect.equal (ListEntry.values decoded.Steps).[0].When_ (Some (SchemaSaladString.Literal expression)) ""
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                Expect.equal (ListEntry.values roundTripped.Steps).[0].When_ (Some (SchemaSaladString.Literal expression)) ""
            testCase "scatter + scatterMethod together roundtrip" <| fun _ ->
                let yaml = TestObjects.CWL.Workflow.workflowWithScatterAndScatterMethodFile
                let decoded = Decode.decodeWorkflow yaml
                Expect.sequenceEqual (ListEntry.values decoded.Steps).[0].Scatter.Value (literals [ "in1"; "in2" ]) ""
                Expect.equal (ListEntry.values decoded.Steps).[0].ScatterMethod (Some FlatCrossProduct) ""
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                Expect.sequenceEqual (ListEntry.values roundTripped.Steps).[0].Scatter.Value (literals [ "in1"; "in2" ]) ""
                Expect.equal (ListEntry.values roundTripped.Steps).[0].ScatterMethod (Some FlatCrossProduct) ""
            testCase "step-level requirements and hints roundtrip" <| fun _ ->
                let workflowWithStepLevelRequirementsAndHintsFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: input1
    out: [out]
    hints:
      - class: StepInputExpressionRequirement
    requirements:
      - class: NetworkAccess
        networkAccess: true"""
                let yaml = workflowWithStepLevelRequirementsAndHintsFile
                let decoded = Decode.decodeWorkflow yaml
                let decodedStep = (ListEntry.values decoded.Steps).[0]
                let decodedHints = Expect.wantSome decodedStep.Hints "Step hints should decode"
                let decodedRequirements = Expect.wantSome decodedStep.Requirements "Step requirements should decode"
                Expect.equal decodedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal decodedRequirements.[0] (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) ""
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                let roundTrippedStep = (ListEntry.values roundTripped.Steps).[0]
                let roundTrippedHints = Expect.wantSome roundTrippedStep.Hints "Step hints should survive roundtrip"
                let roundTrippedRequirements = Expect.wantSome roundTrippedStep.Requirements "Step requirements should survive roundtrip"
                Expect.equal roundTrippedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal roundTrippedRequirements.[0] (NetworkAccessRequirement (NetworkAccessRequirementValue(true))) ""
            testCase "inline ExpressionTool roundtrip preserves expression" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription original
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values roundTripped.Steps).[1].Run)
                        "Should roundtrip ExpressionTool run"
                Expect.equal expressionTool.Expression (SchemaSaladString.Literal "$({'out': inputs.y})") "Expression preserved"
            testCase "inline ExpressionTool chain roundtrip" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription original
                Expect.equal (ListEntry.values roundTripped.Steps).Count 3 ""
                for i in 0..2 do
                    let originalTool =
                        Expect.wantSome
                            (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values original.Steps).[i].Run)
                            $"Original step {i} should be ExpressionTool"
                    let roundTrippedTool =
                        Expect.wantSome
                            (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values roundTripped.Steps).[i].Run)
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
                let roundTripped, encoded = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded

                Expect.stringContains encoded "expression: |" "Expression should encode as block scalar in nested workflow runs."

                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values roundTripped.Steps).[0].Run)
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
        let input = CWLWorkflowDescription.getInputValues workflow |> Seq.find (fun i -> i.Name = name)
        Expect.wantSome input.Type_ $"Input {name} should have a type."

    let outputTypeOf (workflow: CWLWorkflowDescription) (name: string) =
        let output = CWLWorkflowDescription.getOutputValues workflow |> Seq.find (fun o -> o.Name = name)
        Expect.wantSome output.Type_ $"Output {name} should have a type."

    let enumOf (symbols: string list) = Enum (InputEnumSchema(ResizeArray (List.map SchemaSaladString.Literal symbols)))

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
                        let fields = Expect.wantSome schema.Fields "The record should keep its fields." |> ListEntry.values
                        Expect.sequenceEqual (fields |> Seq.map (fun f -> f.Name)) [ "data_folder"; "db_file" ] "Record fields should keep their order."
                    | other -> failwithf "Expected a record member but got %A" other
                | other -> failwithf "Expected a union but got %A" other
                let flagOf name = (CWLWorkflowDescription.getInputValues workflow |> Seq.find (fun i -> i.Name = name)).Optional
                for name in [ "mode_choice"; "annotation_directory"; "sample_files"; "reference_dbs" ] do
                    Expect.isTrue (flagOf name) (sprintf "%s is a union of null and one type, so it is optional." name)
                Expect.isFalse (flagOf "threshold") "A union without null is not optional."
                assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "chosen") "chosen"
                assertTypesEqual (Union (ResizeArray [ CWLType.File; CWLType.Directory ])) (outputTypeOf workflow "summary") "summary"
                let expressionTool = Expect.wantSome (WorkflowStepRunOps.tryGetExpressionTool (ListEntry.values workflow.Steps).[0].Run) "The inline ExpressionTool should decode."
                let toolInput = (Expect.wantSome expressionTool.Inputs "The inline tool should keep its inputs.") |> ListEntry.values |> Seq.find (fun i -> i.Name = "mode_string")
                assertTypesEqual (optionalOf (enumOf [ "alpha"; "delta"; "beta"; "gamma" ])) (Expect.wantSome toolInput.Type_ "Inline input type") "inline mode_string"
            let typeListWorkflow =
                Kit.workflow
                    [ "requirements:"
                      "  InlineJavascriptRequirement: {}"
                      "  StepInputExpressionRequirement: {}"
                      "inputs:"
                      "  mode_choice:"
                      "    type:"
                      "      - \"null\""
                      "      - type: enum"
                      "        symbols: [\"alpha\", \"beta\", \"gamma\", \"delta\"]"
                      "    label: Mode choice"
                      "    doc: |"
                      "      Choice of the mode. Optional."
                      "  reference_dbs:"
                      "    type:"
                      "      - 'null'"
                      "      - type: record"
                      "        name: reference_dbs"
                      "        fields:"
                      "          data_folder:"
                      "            type: Directory?"
                      "            doc: Folder with the data files of the reference database."
                      "          db_file:"
                      "            type: File?"
                      "            doc: Reference database file"
                      "    label: Reference databases"
                      "  annotation_directory:"
                      "    type: Directory?"
                      "    label: Annotation directory"
                      "  sample_files:"
                      "    type:"
                      "      - \"null\""
                      "      - type: array"
                      "        items: File"
                      "  threshold:"
                      "    type: [int, float]"
                      "  first_result: File?"
                      "steps:"
                      "  select_result:"
                      "    label: Mode choice"
                      "    run:"
                      "      class: ExpressionTool"
                      "      requirements:"
                      "        InlineJavascriptRequirement: {}"
                      "      inputs:"
                      "        mode_string:"
                      "          type:"
                      "            - \"null\""
                      "            - type: enum"
                      "              symbols: [\"alpha\", \"delta\", \"beta\", \"gamma\"]"
                      "        first_result: File?"
                      "      outputs:"
                      "        result: File"
                      "      expression: |"
                      "        ${ return {\"result\": inputs.first_result}; }"
                      "    in:"
                      "      mode_string: mode_choice"
                      "      first_result: first_result"
                      "    out: [result]"
                      "outputs:"
                      "  chosen:"
                      "    type:"
                      "      - \"null\""
                      "      - File"
                      "    outputSource: select_result/result"
                      "  summary: [File, Directory]" ]
            let decoded = Decode.decodeWorkflow typeListWorkflow
            check decoded
            check (fst (Kit.rewrite "unions of null" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded))

        testCase "array form ports with unions roundtrip" <| fun _ ->
            let check (workflow: CWLWorkflowDescription) =
                assertTypesEqual CWLType.String (inputTypeOf workflow "NAME_PATTERN") "NAME_PATTERN"
                assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "INDEX_FILES") "INDEX_FILES"
                assertTypesEqual (optionalOf (Array (InputArraySchema(Float)))) (inputTypeOf workflow "LIMITS") "LIMITS"
                assertTypesEqual (Union (ResizeArray [ String; Int; Boolean ])) (inputTypeOf workflow "CHOICE") "CHOICE"
                assertTypesEqual (optionalOf (CWLType.File)) (inputTypeOf workflow "MAYBE_LIST") "MAYBE_LIST"
                assertTypesEqual (CWLType.File) (outputTypeOf workflow "RESULT_LIST") "RESULT_LIST"
                assertTypesEqual (optionalOf (CWLType.File)) (outputTypeOf workflow "OPTIONAL_RESULT") "OPTIONAL_RESULT"
            let arrayFormWorkflow =
                Kit.workflow
                    [ "id: array_form_ports"
                      "inputs:"
                      "  - id: NAME_PATTERN"
                      "    label: \"name pattern\""
                      "    doc: \"pattern for the input files, default *.txt\""
                      "    type: string"
                      "    default: \"*.txt\""
                      "  - id: INDEX_FILES"
                      "    label: \"index files\""
                      "    type:"
                      "      - \"null\""
                      "      - File"
                      "    secondaryFiles:"
                      "      - .idx"
                      "      - .meta"
                      "  - id: LIMITS"
                      "    type: float[]?"
                      "  - id: CHOICE"
                      "    type: [string, int, boolean]"
                      "  - id: MAYBE_LIST"
                      "    type: [\"null\", File]"
                      "steps:"
                      "  - id: run_tool"
                      "    run: tool.cwl"
                      "    in:"
                      "      pattern: NAME_PATTERN"
                      "    out: [result]"
                      "outputs:"
                      "  - id: RESULT_LIST"
                      "    type: File"
                      "    outputSource: run_tool/result"
                      "  - id: OPTIONAL_RESULT"
                      "    type:"
                      "      - \"null\""
                      "      - File"
                      "    outputSource: run_tool/result" ]
            let decoded = Decode.decodeWorkflow arrayFormWorkflow
            check decoded
            check (fst (Kit.rewrite "array form ports" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded))
    ]

let testInlineRunsThatHoldTheirWorkflow =
    let text = "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: other.cwl\n    in: {}\n    out: []\n"
    testList "Inline runs that hold their workflow" [
        testCase "a workflow whose step holds the workflow inline, directly or further down, cannot be written" <| fun _ ->
            let direct = Decode.decodeWorkflow text
            (ListEntry.values direct.Steps).[0].Run <- WorkflowStepRunOps.fromWorkflow direct
            Expect.throws (fun () -> Encode.encodeWorkflowDescription direct |> ignore) "A workflow that holds itself has no text."
            let outer = Decode.decodeWorkflow text
            let inner = Decode.decodeWorkflow text
            (ListEntry.values outer.Steps).[0].Run <- WorkflowStepRunOps.fromWorkflow inner
            (ListEntry.values inner.Steps).[0].Run <- WorkflowStepRunOps.fromWorkflow outer
            Expect.throws (fun () -> Encode.encodeWorkflowDescription outer |> ignore) "A workflow that holds itself further down has no text."
            Expect.throws (fun () -> Encode.encodeWorkflowStep (ListEntry.values outer.Steps).[0] |> ignore) "The step encoder stops at the cycle too."
        testCase "one inline workflow in two steps is written twice and reads back" <| fun _ ->
            let shared = Decode.decodeWorkflow text
            let holder = Decode.decodeWorkflow (text.Replace("steps:\n", "steps:\n  t:\n    run: other.cwl\n    in: {}\n    out: []\n"))
            for step in ListEntry.values holder.Steps do
                step.Run <- WorkflowStepRunOps.fromWorkflow shared
            Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription holder |> ignore
    ]

let testPackedDocuments =
    let packed = String.concat "\n" [ "cwlVersion: v1.2"; "$graph:"; "  - id: main"; "    class: CommandLineTool"; "    baseCommand: echo"; "    inputs: []"; "    outputs: []"; "" ]
    let expectPackedError (name: string) (decode: unit -> unit) (line: int) =
        match Kit.decodeError decode with
        | Some error ->
            Expect.equal (DecodeError.pathText error) "" $"{name}: the error stands at the root."
            Expect.equal error.Message "Packed CWL documents ($graph) are not supported." $"{name}: the error says that the document is packed."
            Expect.equal (error.Span |> Option.map (fun span -> span.Start.Line)) (Some line) $"{name}: the error points at the $graph key."
        | None -> failwith $"{name} must fail"
    testList "Packed documents" [
        testCase "a document with $graph at its root is a decode error at the key, for every decoder of a document" <| fun _ ->
            expectPackedError "a process" (fun () -> Decode.decodeCWLProcessingUnit packed |> ignore) 2
            expectPackedError "a process with warnings" (fun () -> Decode.decodeCWLProcessingUnitWithWarnings packed |> ignore) 2
            expectPackedError "a tree" (fun () -> Decode.decodeCWLProcessingUnitElement (Reader.readRoot packed) |> ignore) 2
            expectPackedError "a workflow" (fun () -> Decode.decodeWorkflow packed |> ignore) 2
            expectPackedError "a tool" (fun () -> Decode.decodeCommandLineTool packed |> ignore) 2
            expectPackedError "a class beside the graph" (fun () -> Decode.decodeCWLProcessingUnit (packed.Replace("$graph:", "class: Workflow\n$graph:")) |> ignore) 3
            expectPackedError "a graph that is no list" (fun () -> Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\n$graph: main\n" |> ignore) 2
            expectPackedError "a quoted key" (fun () -> Decode.decodeCWLProcessingUnit (packed.Replace("$graph:", "\"$graph\":")) |> ignore) 2
        testCase "a run of the form #id stays a run string as written, and the workflow reads back equal" <| fun _ ->
            // An inline run defines the id tool, and a second step names it. The decoder resolves no reference.
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  a:
    run:
      class: CommandLineTool
      id: tool
      baseCommand: echo
      inputs: []
      outputs: []
    in: {}
    out: []
  b:
    run: "#tool"
    in: {}
    out: []"""
            let decoded = Decode.decodeWorkflow yaml
            let second = (ListEntry.values decoded.Steps).[1]
            Expect.equal second.Run (RunString "#tool") "The reference is kept as written."
            let _, written = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "run: \"#tool\"" "The reference is written as it was read."
            second.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool (CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ SchemaSaladString.Literal "cat" ]))) }
            Expect.equal (Encode.encodeWorkflowDescription decoded) written "A resolved process stays out of the text."
    ]

let testListsOfOneSource =
    let workflowWithListsOfOneSourceFile = """cwlVersion: v1.2
class: Workflow
requirements: [{class: MultipleInputFeatureRequirement}]
inputs:
  file1: File
outputs:
  nested:
    type: File[]
    outputSource: [make/created]
    linkMerge: merge_nested
  single:
    type: File
    outputSource: make/created
steps:
  make:
    run: ./make.cwl
    in: {}
    out: [created]
  count:
    run: ./count.cwl
    in:
      files:
        source: [file1]
        linkMerge: merge_nested
      one:
        source: file1
      short: [file1]
      shortOne: file1
    out: [n]"""
    let text = workflowWithListsOfOneSourceFile
    let inputOf (workflow: CWLWorkflowDescription) (name: string) =
        let step = ListEntry.values workflow.Steps |> Seq.find (fun step -> step.Id = Some "count")
        WorkflowStep.getInputValues step |> Seq.find (fun input -> input.Id = name)
    let outputOf (workflow: CWLWorkflowDescription) (name: string) =
        ListEntry.values workflow.Outputs |> Seq.find (fun output -> output.Name = name)
    testList "Lists of one source" [
        testCase "a list of one source in a step input or an output stays a list, and a string stays a single source" <| fun _ ->
            let decoded = Decode.decodeWorkflow text
            Expect.equal (inputOf decoded "files").Source (Some (OutputSource.Multiple (literals [ "file1" ]))) "A source with a list of one is a list."
            Expect.equal (inputOf decoded "one").Source (Some (OutputSource.Single (SchemaSaladString.Literal "file1"))) "A string is a single source."
            Expect.equal (inputOf decoded "short").Source (Some (OutputSource.Multiple (literals [ "file1" ]))) "The shorthand with a list of one is a list."
            Expect.equal (inputOf decoded "shortOne").Source (Some (OutputSource.Single (SchemaSaladString.Literal "file1"))) "The shorthand with a string is a single source."
            Expect.equal (outputOf decoded "nested").OutputSource (Some (OutputSource.Multiple (literals [ "make/created" ]))) "An outputSource with a list of one is a list."
            Expect.equal (outputOf decoded "single").OutputSource (Some (OutputSource.Single (SchemaSaladString.Literal "make/created"))) "A string is a single outputSource."
        testCase "the written workflow keeps a list of one source as a list and reads back equal" <| fun _ ->
            let decoded = Decode.decodeWorkflow text
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "outputSource: [make/created]" "The outputSource list of one is written as a list."
            Expect.stringContains written "outputSource: make/created\n" "The single outputSource is written as a string."
            Expect.stringContains written "source:\n          - file1" "The source list of one is written as a sequence."
            Expect.stringContains written "      one: file1" "The single source is written as a string."
            Expect.stringContains written "      short:\n        - file1" "The shorthand list of one source is written as a sequence."
            let again = Decode.decodeWorkflow written
            Expect.equal again decoded "The written text should decode to the same workflow."
            Expect.equal (Encode.encodeWorkflowDescription again) written "Writing again should give the same text."
        testCase "a list of one source differs from a single source" <| fun _ ->
            let asString = text.Replace("source: [file1]", "source: file1").Replace("outputSource: [make/created]", "outputSource: make/created")
            Expect.notEqual (inputOf (Decode.decodeWorkflow asString) "files") (inputOf (Decode.decodeWorkflow text) "files") "The step inputs differ."
            Expect.notEqual (outputOf (Decode.decodeWorkflow asString) "nested") (outputOf (Decode.decodeWorkflow text) "nested") "The outputs differ."
            Expect.notEqual (Decode.decodeWorkflow asString) (Decode.decodeWorkflow text) "The workflows differ."
        testCase "an empty list of sources in a step input stays an empty list" <| fun _ ->
            let decoded = Decode.decodeWorkflow (text.Replace("source: [file1]", "source: []"))
            Expect.equal (inputOf decoded "files").Source (Some (OutputSource.Multiple (ResizeArray()))) "The list is empty."
            Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded |> ignore
    ]

let testStepsWithoutAnId =
    let ids (steps: ResizeArray<ListEntry<WorkflowStep>>) = ListEntry.values steps |> Seq.map (fun step -> step.Id) |> List.ofSeq
    let kinds (steps: ResizeArray<ListEntry<WorkflowStep>>) =
        steps
        |> Seq.map (fun entry ->
            match entry with
            | ListEntry.Value step -> "value " + (match step.Id with | Some id -> "'" + id + "'" | None -> "without an id")
            | ListEntry.Imported path -> "import " + path
            | ListEntry.Mixed _ -> "mixin")
        |> List.ofSeq
    let workflow (idLines: string list) (steps: string list) = Kit.workflow (idLines @ [ "inputs: {}"; "outputs: {}"; "steps:" ] @ steps)
    let failure (text: string) =
        match Kit.decodeError (fun () -> Decode.decodeWorkflow text) with
        | Some error -> error
        | None -> failwith ("The decode must fail:\n" + text)
    testList "Steps without an id" [
        testCase "two steps without an id stay two in their order, also when their content is equal, and read back" <| fun _ ->
            let workflowWithTwoUnnamedStepsFile = """cwlVersion: v1.2
class: Workflow
id: main
inputs:
  x: string
outputs: {}
steps:
- run: ./same.cwl
  in:
    a: x
  out: [o]
- run: ./same.cwl
  in:
    a: x
  out: [o]"""
            let decoded = Decode.decodeWorkflow workflowWithTwoUnnamedStepsFile
            let check (what: string) (workflow: CWLWorkflowDescription) =
                let steps = ListEntry.values workflow.Steps
                Expect.equal steps.Count 2 (what + ": both steps stay.")
                Expect.equal (ids workflow.Steps) [ None; None ] (what + ": neither step has an id.")
                for step in steps do
                    Expect.equal step.Run (RunString "./same.cwl") (what + ": the step has its run.")
                    Expect.equal (WorkflowStep.getInputValues step |> Seq.map (fun input -> input.Id, input.Source) |> List.ofSeq) [ "a", Some (sourceOf [ "x" ]) ] (what + ": the step has its input.")
                    Expect.equal (step.Out |> Seq.choose StepOutput.tryId |> List.ofSeq) [ "o" ] (what + ": the step has its output.")
            check "decoded" decoded
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "steps:\n  - run: ./same.cwl" "The steps are written as a list of objects without an id."
            Expect.isFalse (written.Contains "id: \"\"") "No empty id is written for a step without an id."
            let again = Decode.decodeWorkflow written
            check "read back" again
            Expect.equal again decoded "The written text should decode to the same workflow."
            Expect.equal (Encode.encodeWorkflowDescription again) written "Writing again should give the same text."
        testCase "a step without an id keeps its place beside a step with an id, an import and a mixin, and the steps are written as a list" <| fun _ ->
            let workflowWithUnnamedAndDirectiveStepsFile = """cwlVersion: v1.2
class: Workflow
id: main
inputs:
  x: string
outputs: {}
steps:
- $import: more-steps.yml
- run: ./first.cwl
  label: first
  in:
  - id: a
    source: x
  out: [o]
- id: named
  run: ./second.cwl
  in: {}
  out: []
- run: ./third.cwl
  in: {}
  out: []
- $mixin: mixed.yml
  label: mixed"""
            let decoded = Decode.decodeWorkflow workflowWithUnnamedAndDirectiveStepsFile
            let expected = [ "import more-steps.yml"; "value without an id"; "value 'named'"; "value without an id"; "mixin" ]
            Expect.equal (kinds decoded.Steps) expected "The entries keep their order and their kind."
            let first = (ListEntry.values decoded.Steps).[0]
            Expect.equal first.Run (RunString "./first.cwl") "The first step without an id has its run."
            Expect.equal first.Label (Some (SchemaSaladString.Literal "first")) "The first step without an id has its label."
            let written = Encode.encodeWorkflowDescription decoded
            Expect.stringContains written "$import: more-steps.yml" "The import is written."
            Expect.stringContains written "$mixin: mixed.yml" "The mixin is written."
            Expect.stringContains written "id: named" "The step with an id carries it."
            let again = Decode.decodeWorkflow written
            Expect.equal (kinds again.Steps) expected "The written text has the same entries."
            Expect.equal (ListEntry.values again.Steps |> Seq.map (fun step -> step.Run) |> List.ofSeq) [ RunString "./first.cwl"; RunString "./second.cwl"; RunString "./third.cwl" ] "The steps keep their runs."
            Expect.equal again decoded "The written text should decode to the same workflow."
        testCase "id: \"\" is a step with the empty id and no step without an id, in the list form and in the map form" <| fun _ ->
            let inList = workflow [ "id: main" ] [ "  - {id: \"\", run: a.cwl, in: [], out: []}"; "  - {run: b.cwl, in: [], out: []}"; "  - {id: a, run: c.cwl, in: [], out: []}" ]
            let inMap = workflow [ "id: main" ] [ "  \"\": {run: a.cwl, in: [], out: []}"; "  a: {run: c.cwl, in: [], out: []}" ]
            let fromList = Decode.decodeWorkflow inList
            Expect.equal (ids fromList.Steps) [ Some ""; None; Some "a" ] "The empty id is an id, and the missing id is none."
            let fromMap = Decode.decodeWorkflow inMap
            Expect.equal (ids fromMap.Steps) [ Some ""; Some "a" ] "The key of the map is the id."
            for (what, decoded) in [ "list", fromList; "map", fromMap ] do
                let again, written = Kit.rewrite "the round trip" Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded
                Expect.equal (ids again.Steps) (ids decoded.Steps) (what + ": the ids read back.")
                Expect.equal (ListEntry.values again.Steps |> Seq.map (fun step -> step.Run) |> List.ofSeq) (ListEntry.values decoded.Steps |> Seq.map (fun step -> step.Run) |> List.ofSeq) (what + ": the runs read back.")
                Expect.equal again decoded (what + ": the written text should decode to the same workflow.")
            Expect.stringContains (Encode.encodeWorkflowDescription fromMap) "\"\":" "The empty key is written in quotes."
        testCase "no id, the empty id and a name differ in equality and in the hash of a step" <| fun _ ->
            let step (id: string option) = WorkflowStep(id, ResizeArray(), ResizeArray(), RunString "t.cwl")
            let steps = [ "none", step None; "empty", step (Some ""); "named", step (Some "a") ]
            for (leftName, left) in steps do
                for (rightName, right) in steps do
                    if leftName = rightName then
                        Kit.expectSame (leftName + " and itself") (box left) (box right)
                    else
                        Expect.notEqual left right (leftName + " differs from " + rightName + ".")
                        Expect.notEqual (left.GetHashCode()) (right.GetHashCode()) (leftName + " hashes apart from " + rightName + ".")
            Expect.equal (step None).Id None "The id is none."
            Expect.equal (step (Some "")).Id (Some "") "The id is the empty text."
            Expect.equal (WorkflowStep.fromRunPath("a", ResizeArray(), ResizeArray(), "t.cwl")).Id (Some "a") "The convenience constructor takes the id."
        testCase "a step without an id built in code is written without an id and reads back" <| fun _ ->
            let steps =
                [ WorkflowStep(None, ResizeArray(), ResizeArray [ StepOutputString "o" ], RunString "a.cwl")
                  WorkflowStep.fromRunPath("named", ResizeArray(), ResizeArray(), "b.cwl")
                  WorkflowStep(None, ResizeArray(), ResizeArray(), RunString "c.cwl") ]
            let built = CWLWorkflowDescription(steps = ListEntry.ofValues (ResizeArray steps), inputs = ResizeArray(), outputs = ResizeArray(), id = "main")
            let again = Decode.decodeWorkflow (Encode.encodeWorkflowDescription built)
            Expect.equal (ids again.Steps) [ None; Some "named"; None ] "The ids read back."
            Expect.equal (ListEntry.values again.Steps |> Seq.map (fun step -> step.Run) |> List.ofSeq) [ RunString "a.cwl"; RunString "b.cwl"; RunString "c.cwl" ] "The runs read back."
            Expect.equal (ListEntry.values again.Steps).[0].Out.Count 1 "The output reads back."
        testCase "a step without an id decodes in full, so it fails like a step with an id, and a null id fails too" <| fun _ ->
            for (what, fields) in
                [ "no run", [ "in: []"; "out: []" ]
                  "a number as the inputs", [ "run: a.cwl"; "in: 5"; "out: []" ]
                  "a word as the scatter method", [ "run: a.cwl"; "in: []"; "out: []"; "scatterMethod: foo" ]
                  "a number as the run", [ "run: 5"; "in: []"; "out: []" ]
                  "a list as the label", [ "run: a.cwl"; "in: []"; "out: []"; "label: [a, b]" ] ] do
                let body = fields |> String.concat ", "
                let unnamed = failure (workflow [ "id: main" ] [ "  - {" + body + "}" ])
                let named = failure (workflow [ "id: main" ] [ "  - {id: s, " + body + "}" ])
                Expect.equal unnamed.Message named.Message (what + ": the step without an id fails with the message of a step with an id.")
                Expect.isTrue ((DecodeError.pathText unnamed).StartsWith "steps[0]") (what + ": the path shows the position, found " + DecodeError.pathText unnamed)
                Expect.isSome unnamed.Span (what + ": the error has the place in the text.")
            let nullId = failure (workflow [ "id: main" ] [ "  - {id: , run: a.cwl, in: [], out: []}" ])
            Expect.equal (DecodeError.pathText nullId) "steps[0].id" "A null id is an error at the id."
            Expect.stringContains nullId.Message "Expected a value, found" "A null id is no missing id."
    ]

let testEmptyIdsOfPortsAndSteps =
    let classes =
        [ "CommandLineTool", [ "baseCommand: echo" ]
          "ExpressionTool", [ "expression: $(null)" ]
          "Operation", []
          "Workflow", [ "steps: []" ] ]
    let stepWith (field: string) =
        Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run: t.cwl"; "    " + field; "    out: []" ]
    // The spellings of an id that cwltool reads as the empty one, with the id that they decode to.
    let emptyIds =
        [ "empty", "\"\"", ""
          "single quoted empty", "''", ""
          "only spaces", "\"  \"", "  "
          "a tab", "\"\\t\"", "\t"
          "a bare hash", "\"#\"", "#" ]
    let readsBack (name: string) (text: string) : CWLWorkflowDescription =
        let decoded = Decode.decodeWorkflow text
        Kit.rewrite (sprintf "%s: the written text should decode to the same workflow." name) Decode.decodeWorkflow Encode.encodeWorkflowDescription decoded |> ignore
        decoded
    testList "Empty ids of ports and steps" [
        testCase "an empty id of an input or an output is a decode error at the id or the key, in every class" <| fun _ ->
            for (cls, extra) in classes do
                for (spelling, text, _) in emptyIds do
                    Kit.processFails (cls + " with the input id " + spelling) (Kit.document cls (extra @ [ "inputs: [{id: " + text + ", type: string}]"; "outputs: []" ])) "inputs[0].id" "Expected a non-empty id for an input"
                    Kit.processFails (cls + " with the output id " + spelling) (Kit.document cls (extra @ [ "inputs: []"; "outputs: [{id: " + text + ", type: string}]" ])) "outputs[0].id" "Expected a non-empty id for an output"
                    Kit.processFails (cls + " with the input key " + spelling) (Kit.document cls (extra @ [ "inputs: {" + text + ": string}"; "outputs: []" ])) "inputs" "Expected a non-empty id for an input"
                    Kit.processFails (cls + " with the output key " + spelling) (Kit.document cls (extra @ [ "inputs: []"; "outputs: {" + text + ": {type: string}}" ])) "outputs" "Expected a non-empty id for an output"
        testCase "an empty id behind valid entries is a decode error at its own index" <| fun _ ->
            Kit.processFails "inputs" (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: [{id: a, type: string}, {id: \"\", type: string}]"; "outputs: []" ]) "inputs[1].id" "Expected a non-empty id for an input"
            Kit.processFails "outputs" (Kit.document "CommandLineTool" [ "baseCommand: echo"; "inputs: []"; "outputs: [{id: a, type: stdout}, {id: \"\", type: stderr}]" ]) "outputs[1].id" "Expected a non-empty id for an output"
            Kit.processFails "in" (stepWith "in: [{id: a, source: x}, {id: \"\", source: x}]") "steps.s.in[1].id" "Expected a non-empty id for a step input"
        testCase "an empty id of a step input is a decode error at the id or the key" <| fun _ ->
            for (spelling, text, _) in emptyIds do
                Kit.processFails ("in with the id " + spelling) (stepWith ("in: [{id: " + text + ", source: x}]")) "steps.s.in[0].id" "Expected a non-empty id for a step input"
                Kit.processFails ("in with the key " + spelling) (stepWith ("in: {" + text + ": x}")) "steps.s.in" "Expected a non-empty id for a step input"
                Kit.processFails ("in with the key " + spelling + " and an object") (stepWith ("in: {" + text + ": {source: x}}")) "steps.s.in" "Expected a non-empty id for a step input"
        testCase "an empty id of a step is a decode error in a workflow without an id, and the step of a workflow with an id reads back" <| fun _ ->
            let fields = "run: t.cwl, in: [], out: []"
            let workflow (idLines: string list) (steps: string) = Kit.document "Workflow" (idLines @ [ "inputs: []"; "outputs: []"; "steps: " + steps ])
            for (spelling, text, value) in emptyIds do
                let inList = "[{id: " + text + ", " + fields + "}]"
                let inMap = "{" + text + ": {" + fields + "}}"
                for (what, idLines) in [ "no id", []; "an empty id", [ "id: \"\"" ]; "a bare hash id", [ "id: \"#\"" ] ] do
                    Kit.processFails ("steps in a list, workflow with " + what + ", id " + spelling) (workflow idLines inList) "steps[0].id" "Expected a non-empty id for a step of a workflow without an id"
                    Kit.processFails ("steps in a map, workflow with " + what + ", key " + spelling) (workflow idLines inMap) "steps" "Expected a non-empty id for a step of a workflow without an id"
                for id in [ "main"; "\"#main\""; "a/b" ] do
                    let fromList = readsBack ("a list in the workflow " + id) (workflow [ "id: " + id ] inList)
                    Expect.equal (ListEntry.values fromList.Steps).[0].Id (Some value) "The step keeps its id."
                    let fromMap = readsBack ("a map in the workflow " + id) (workflow [ "id: " + id ] inMap)
                    Expect.equal (ListEntry.values fromMap.Steps).[0].Id (Some value) "The step keeps its id."
        testCase "an empty step id in an inline workflow reads back, because the step lives below the id of its process" <| fun _ ->
            let step = "{id: \"\", run: {class: CommandLineTool, baseCommand: echo, inputs: [], outputs: []}, in: [], out: []}"
            let nested = Kit.document "Workflow" [ "requirements: [{class: SubworkflowFeatureRequirement}]"; "inputs: []"; "outputs: []"; "steps:"; "  s:"; "    run: {class: Workflow, inputs: [], outputs: [], steps: [" + step + "]}"; "    in: []"; "    out: []" ]
            readsBack "an inline workflow" nested |> ignore
        testCase "an id that cwltool reads as a name is no empty id" <| fun _ ->
            // cwltool drops only the characters up to a space at the start of an id, so a no-break space is a name.
            // For a step, a # followed by a space is a name too, because the fragment behind the # is a space.
            for text in [ "\"\\u00a0\""; "\"\\u3000\"" ] do
                let tool = Decode.decodeCWLProcessingUnit (Kit.document "CommandLineTool" [ "baseCommand: echo"; "id: main"; "inputs: [{id: " + text + ", type: string}]"; "outputs: []" ])
                Kit.rewrite (sprintf "The tool with the input id %s should read back." text) Decode.decodeCWLProcessingUnit Encode.encodeProcessingUnit tool |> ignore
            for text in [ "\"\\u00a0\""; "\"\\u3000\""; "\"# \""; "\" # \"" ] do
                let fromSteps = readsBack (sprintf "the step id %s" text) (Kit.document "Workflow" [ "inputs: []"; "outputs: []"; "steps: [{id: " + text + ", run: t.cwl, in: [], out: []}]" ])
                Expect.equal (ListEntry.values fromSteps.Steps).Count 1 "The step is there."
    ]

let main =
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescriptionDecode
        testCWLWorkflowDescriptionEncode
        testWorkflowUnionPortRoundTrip
        testPackedDocuments
        testInlineRunsThatHoldTheirWorkflow
        testStepsWithoutAnId
        testEmptyIdsOfPortsAndSteps
        testListsOfOneSource
    ]
