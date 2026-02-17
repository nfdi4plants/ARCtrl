module Tests.WorkflowSteps

open ARCtrl.CWL
open YAMLicious
open YAMLicious.YAMLiciousTypes
open TestingUtils

let decodeWorkflowStep =
    TestObjects.CWL.WorkflowSteps.workflowStepsFileContent
    |> Decode.read
    |> Decode.stepsDecoder

let tryYamlScalarString (y: YAMLElement) =
    match y with
    | YAMLElement.Object [YAMLElement.Value v]
    | YAMLElement.Value v -> Some v.Value
    | _ -> None

let mkStepInput id source defaultValue valueFrom linkMerge =
    {
        Id = id
        Source = source
        DefaultValue = defaultValue
        ValueFrom = valueFrom
        LinkMerge = linkMerge
        PickValue = None
        Doc = None
        LoadContents = None
        LoadListing = None
        Label = None
    }

let testWorkflowStep =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 2 decodeWorkflowStep.Count ""
        testList "IDs" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = "MzMLToMzlite"
                let actual = decodeWorkflowStep.[0].Id
                Expect.equal actual expected ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "PeptideSpectrumMatching"
                let actual = decodeWorkflowStep.[1].Id
                Expect.equal actual expected ""
        ]
        testList "Run" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = "./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl"
                let actual = decodeWorkflowStep.[0].Run
                match actual with
                | RunString runPath -> Expect.equal runPath expected ""
                | _ -> failwithf "Expected RunString but got %A" actual
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                let actual = decodeWorkflowStep.[1].Run
                match actual with
                | RunString runPath -> Expect.equal runPath expected ""
                | _ -> failwithf "Expected RunString but got %A" actual
        ]
        testList "In" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected =
                    ResizeArray [|
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None None None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"inputMzML"|])) None None None 
                    |]
                let actual = decodeWorkflowStep.[0].In
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
                let expected =
                    ResizeArray [|
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None None None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"MzMLToMzlite/dir1"; "MzMLToMzlite/dir2"|])) None None (Some MergeFlattened)
                        mkStepInput "parallelismLevel" None (Some (YAMLElement.Object [YAMLElement.Value {Value = "8"; Comment = None}])) None None
                        mkStepInput "outputDirectory" None None (Some "output") None
                    |]
                let actual = decodeWorkflowStep.[1].In
                Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                    Expect.equal actual.Id expected.Id ""
                    if expected.Source.IsSome then
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
                let actual = decodeWorkflowStep.[0].Out
                Expect.sequenceEqual actual expected ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = ResizeArray [| StepOutputString "dir1"; StepOutputString "dir2" |]
                let actual = decodeWorkflowStep.[1].Out
                Expect.sequenceEqual actual expected ""
        ]
        testCase "invalid linkMerge fails decode" <| fun _ ->
            let invalidSteps = """steps:
  step1:
    run: ./tool.cwl
    in:
      input1:
        source: in1
        linkMerge: invalid_merge
    out: [out]"""
            let decodeInvalid () =
                invalidSteps
                |> Decode.read
                |> Decode.stepsDecoder
                |> ignore
            Expect.throws decodeInvalid "Invalid linkMerge should fail decoding"
        testCase "step output record requires id" <| fun _ ->
            let invalidSteps = """steps:
  step1:
    run: ./tool.cwl
    in: {}
    out:
      - type: string"""
            let decodeInvalid () =
                invalidSteps
                |> Decode.read
                |> Decode.stepsDecoder
                |> ignore
            Expect.throws decodeInvalid "Step output record without id should fail decoding"
        testCase "tryGetTool returns None for wrong obj type" <| fun _ ->
            let run = RunCommandLineTool (box "not a tool")
            let actual = WorkflowStepRunOps.tryGetTool run
            Expect.isNone actual "RunCommandLineTool with non-tool payload should not decode as tool"
        testCase "tryGetWorkflow returns None for wrong obj type" <| fun _ ->
            let run = RunWorkflow (box 42)
            let actual = WorkflowStepRunOps.tryGetWorkflow run
            Expect.isNone actual "RunWorkflow with non-workflow payload should not decode as workflow"
        testCase "encodeWorkflowStepRun raises for wrong obj in RunCommandLineTool" <| fun _ ->
            let run = RunCommandLineTool (box "bad")
            Expect.throws (fun _ -> Encode.encodeWorkflowStepRun run |> ignore) "Encoding invalid RunCommandLineTool payload should fail"
        testCase "encodeWorkflowStepRun raises for wrong obj in RunWorkflow" <| fun _ ->
            let run = RunWorkflow (box "bad")
            Expect.throws (fun _ -> Encode.encodeWorkflowStepRun run |> ignore) "Encoding invalid RunWorkflow payload should fail"
        testCase "tryGetExpressionTool returns Some for correct payload" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray [], expression = "$(null)")
            let run = RunExpressionTool (box expressionTool)
            let actual = WorkflowStepRunOps.tryGetExpressionTool run
            Expect.isSome actual "Should extract expression tool payload"
            Expect.equal actual.Value.Expression "$(null)" ""
        testCase "tryGetExpressionTool returns None for wrong obj type" <| fun _ ->
            let run = RunExpressionTool (box "not an expression tool")
            let actual = WorkflowStepRunOps.tryGetExpressionTool run
            Expect.isNone actual "Should not extract non-ExpressionTool payload"
        testCase "tryGetTool and tryGetWorkflow return None for RunExpressionTool" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray [], expression = "$(null)")
            let run = RunExpressionTool (box expressionTool)
            Expect.isNone (WorkflowStepRunOps.tryGetTool run) "RunExpressionTool should not decode as tool"
            Expect.isNone (WorkflowStepRunOps.tryGetWorkflow run) "RunExpressionTool should not decode as workflow"
        testCase "encodeWorkflowStepRun raises for wrong obj in RunExpressionTool" <| fun _ ->
            let run = RunExpressionTool (box 42)
            Expect.throws (fun _ -> Encode.encodeWorkflowStepRun run |> ignore) "Encoding invalid RunExpressionTool payload should fail"
    ]

let testWorkflowStepOps =
    testList "WorkflowStepOps" [
        testCase "StepInput.updateAt throws for out-of-range index" <| fun _ ->
            let inputs = ResizeArray [| StepInput.create("input1") |]
            let act () = StepInput.updateAt 5 id inputs
            Expect.throws act "Out-of-range StepInput index should raise invalidArg."

        testCase "updateInputAt updates immutable StepInput in-place collection" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ResizeArray [| StepInput.create("input1", source = ResizeArray [| "old" |]) |],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.updateInputAt 0 (fun i -> { i with Source = Some (ResizeArray [| "new" |]) }) step
            Expect.sequenceEqual step.In.[0].Source.Value (ResizeArray [| "new" |]) "Input source should be updated."

        testCase "updateInputById updates only matching input" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ =
                        ResizeArray [|
                            StepInput.create("first", source = ResizeArray [| "a" |])
                            StepInput.create("second", source = ResizeArray [| "b" |])
                        |],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.updateInputById "second" (fun i -> { i with ValueFrom = Some "$(self)" }) step
            Expect.isNone step.In.[0].ValueFrom "First input should remain unchanged."
            Expect.equal step.In.[1].ValueFrom (Some "$(self)") "Second input should be updated."

        testCase "updateInputById with missing id is a no-op" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ResizeArray [| StepInput.create("first"); StepInput.create("second") |],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.updateInputById "missing" (fun i -> { i with ValueFrom = Some "$(self)" }) step
            Expect.isNone step.In.[0].ValueFrom "First input should remain unchanged."
            Expect.isNone step.In.[1].ValueFrom "Second input should remain unchanged."

        testCase "addInput appends new input" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ResizeArray [| StepInput.create("first") |],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.addInput (StepInput.create("second", valueFrom = "$(self)")) step
            Expect.equal step.In.Count 2 "A new input should be appended."
            Expect.equal step.In.[1].Id "second" "Appended input should be the second entry."
            Expect.equal step.In.[1].ValueFrom (Some "$(self)") "Appended input content should be preserved."

        testCase "removeInputsById removes matching entries" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ =
                        ResizeArray [|
                            StepInput.create("dup")
                            StepInput.create("keep")
                            StepInput.create("dup")
                        |],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.removeInputsById "dup" step
            Expect.equal step.In.Count 1 "Only one input should remain."
            Expect.equal step.In.[0].Id "keep" "Non-matching input should remain."
    ]

let main = 
    testList "WorkflowStep" [
        testWorkflowStep
        testWorkflowStepOps
    ]
