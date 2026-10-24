module Tests.WorkflowSteps

open ARCtrl.CWL
open DynamicObj
open YAMLicious
open YAMLicious
open TestingUtils

let decodeWorkflowStep =
    TestObjects.CWL.WorkflowSteps.workflowStepsFileContent
    |> Decode.readYaml
    |> Decode.stepsDecoder
    |> ListEntry.values

let tryYamlScalarString (y: RawYaml) =
    match y.Node.Kind with
    | YamlNodeKind.Scalar scalar -> Some scalar.Value
    | _ -> None

let mkStepInput id source defaultValue valueFrom linkMerge =
    StepInput.create(
        id,
        ?source = source,
        ?defaultValue = defaultValue,
        ?valueFrom = valueFrom,
        ?linkMerge = linkMerge
    )

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
                let actual = WorkflowStep.getInputs decodeWorkflowStep.[0]
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
                        mkStepInput "parallelismLevel" None (Some { Node = YamlNode.plain "8" }) None None
                        mkStepInput "outputDirectory" None None (Some (SchemaSaladString.Literal "output")) None
                    |]
                let actual = WorkflowStep.getInputs decodeWorkflowStep.[1]
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
        testCase "explicit empty in and out forms decode as empty collections" <| fun _ ->
            let validSteps = """steps:
  step1:
    run: ./tool.cwl
    in: {}
    out: []"""
            let decoded =
                validSteps
                |> Decode.readYaml
                |> Decode.stepsDecoder
                |> ListEntry.values
            let step = decoded.[0]
            Expect.equal step.In.Count 0 "Explicit empty object for `in` should decode to an empty input collection."
            Expect.equal step.Out.Count 0 "Explicit empty list for `out` should decode to an empty output collection."
        testCase "missing in field fails decode" <| fun _ ->
            let invalidSteps = """steps:
  step1:
    run: ./tool.cwl
    out: [out]"""
            let decodeInvalid () =
                invalidSteps
                |> Decode.readYaml
                |> Decode.stepsDecoder
                |> ListEntry.values
                |> ignore
            Expect.throws decodeInvalid "Missing required `in` field should fail decoding."
        testCase "missing out field fails decode" <| fun _ ->
            let invalidSteps = """steps:
  step1:
    run: ./tool.cwl
    in: {}"""
            let decodeInvalid () =
                invalidSteps
                |> Decode.readYaml
                |> Decode.stepsDecoder
                |> ListEntry.values
                |> ignore
            Expect.throws decodeInvalid "Missing required `out` field should fail decoding."
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
                |> Decode.readYaml
                |> Decode.stepsDecoder
                |> ListEntry.values
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
                |> Decode.readYaml
                |> Decode.stepsDecoder
                |> ListEntry.values
                |> ignore
            Expect.throws decodeInvalid "Step output record without id should fail decoding"
        testCase "step output parameter known fields are typed fields, not dynamic overflow" <| fun _ ->
            let output = StepOutputParameter.create "out"
            Expect.sequenceEqual StepOutputParameter.KnownFieldNames (Set [| "id" |]) "Step output known fields should be declared on the type."
            Expect.isEmpty (output |> DynamicObjHelpers.dynamicPropertiesSnapshot) "Step output known fields should not be stored as dynamic properties."
            DynObj.setProperty "arc:note" "keep overflow" output
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:note" output) (Some "keep overflow") "Unknown fields should still use DynamicObj overflow."
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
            Expect.throws (fun _ -> Encode.encodeWorkflowStepRun false run |> ignore) "Encoding invalid RunCommandLineTool payload should fail"
        testCase "encodeWorkflowStepRun raises for wrong obj in RunWorkflow" <| fun _ ->
            let run = RunWorkflow (box "bad")
            Expect.throws (fun _ -> Encode.encodeWorkflowStepRun false run |> ignore) "Encoding invalid RunWorkflow payload should fail"
        testCase "tryGetExpressionTool returns Some for correct payload" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray [], expression = SchemaSaladString.Literal "$(null)")
            let run = RunExpressionTool (box expressionTool)
            let actual = WorkflowStepRunOps.tryGetExpressionTool run
            Expect.isSome actual "Should extract expression tool payload"
            Expect.equal actual.Value.Expression (SchemaSaladString.Literal "$(null)") ""
        testCase "tryGetExpressionTool returns None for wrong obj type" <| fun _ ->
            let run = RunExpressionTool (box "not an expression tool")
            let actual = WorkflowStepRunOps.tryGetExpressionTool run
            Expect.isNone actual "Should not extract non-ExpressionTool payload"
        testCase "tryGetTool and tryGetWorkflow return None for RunExpressionTool" <| fun _ ->
            let expressionTool = CWLExpressionToolDescription(outputs = ResizeArray [], expression = SchemaSaladString.Literal "$(null)")
            let run = RunExpressionTool (box expressionTool)
            Expect.isNone (WorkflowStepRunOps.tryGetTool run) "RunExpressionTool should not decode as tool"
            Expect.isNone (WorkflowStepRunOps.tryGetWorkflow run) "RunExpressionTool should not decode as workflow"
        testCase "encodeWorkflowStepRun raises for wrong obj in RunExpressionTool" <| fun _ ->
            let run = RunExpressionTool (box 42)
            Expect.throws (fun _ -> Encode.encodeWorkflowStepRun false run |> ignore) "Encoding invalid RunExpressionTool payload should fail"
    ]

let testWorkflowStepOps =
    testList "WorkflowStep Operations" [
        testCase "StepInput.updateAt throws for out-of-range index" <| fun _ ->
            let inputs = ListEntry.ofValues [ StepInput.create("input1") ]
            let act () = StepInput.updateAt 5 id inputs
            Expect.throws act "Out-of-range StepInput index should raise invalidArg."

        testCase "updateInputAt updates immutable StepInput in-place collection" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ListEntry.ofValues [ StepInput.create("input1", source = ResizeArray [| "old" |]) ],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.updateInputAt 0 (fun i ->
                i.Source <- Some (ResizeArray [| "new" |])
                i
            ) step
            Expect.sequenceEqual (WorkflowStep.getInputs step).[0].Source.Value (ResizeArray [| "new" |]) "Input source should be updated."

        testCase "updateInputById updates only matching input" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ =
                        ListEntry.ofValues [
                            StepInput.create("first", source = ResizeArray [| "a" |])
                            StepInput.create("second", source = ResizeArray [| "b" |])
                        ],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.updateInputById "second" (fun i ->
                i.ValueFrom <- Some (SchemaSaladString.Literal "$(self)")
                i
            ) step
            Expect.isNone (WorkflowStep.getInputs step).[0].ValueFrom "First input should remain unchanged."
            Expect.equal (WorkflowStep.getInputs step).[1].ValueFrom (Some (SchemaSaladString.Literal "$(self)")) "Second input should be updated."

        testCase "updateInputById with missing id is a no-op" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ListEntry.ofValues [ StepInput.create("first"); StepInput.create("second") ],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.updateInputById "missing" (fun i ->
                i.ValueFrom <- Some (SchemaSaladString.Literal "$(self)")
                i
            ) step
            Expect.isNone (WorkflowStep.getInputs step).[0].ValueFrom "First input should remain unchanged."
            Expect.isNone (WorkflowStep.getInputs step).[1].ValueFrom "Second input should remain unchanged."

        testCase "addInput appends new input" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ListEntry.ofValues [ StepInput.create("first") ],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.addInput (StepInput.create("second", valueFrom = (SchemaSaladString.Literal "$(self)"))) step
            Expect.equal step.In.Count 2 "A new input should be appended."
            Expect.equal (WorkflowStep.getInputs step).[1].Id "second" "Appended input should be the second entry."
            Expect.equal (WorkflowStep.getInputs step).[1].ValueFrom (Some (SchemaSaladString.Literal "$(self)")) "Appended input content should be preserved."

        testCase "removeInputsById removes matching entries" <| fun _ ->
            let step =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ =
                        ListEntry.ofValues [
                            StepInput.create("dup")
                            StepInput.create("keep")
                            StepInput.create("dup")
                        ],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )

            WorkflowStep.removeInputsById "dup" step
            Expect.equal step.In.Count 1 "Only one input should remain."
            Expect.equal (WorkflowStep.getInputs step).[0].Id "keep" "Non-matching input should remain."

        testCase "the helpers pass over an import among the step inputs and keep it" <| fun _ ->
            let importedFirst () =
                WorkflowStep.fromRunPath(
                    id = "step1",
                    in_ = ResizeArray [ ListEntry.Imported "more.yml"; ListEntry.Value (StepInput.create("dup")); ListEntry.Value (StepInput.create("keep")) ],
                    out_ = ResizeArray [| StepOutputString "out" |],
                    runPath = "./tool.cwl"
                )
            let step = importedFirst ()
            WorkflowStep.removeInputsById "dup" step
            Expect.equal (List.ofSeq step.In |> List.map (ListEntry.map (fun input -> input.Id))) [ ListEntry.Imported "more.yml"; ListEntry.Value "keep" ] "The import stays, and the matching input goes."
            let updated = importedFirst ()
            WorkflowStep.updateInputById "keep" (fun i -> i.ValueFrom <- Some (SchemaSaladString.Literal "$(self)"); i) updated
            Expect.equal (WorkflowStep.getInputs updated).[1].ValueFrom (Some (SchemaSaladString.Literal "$(self)")) "The input behind the import is updated."
            Expect.equal updated.In.[0] (ListEntry.Imported "more.yml") "The import is as it was."
            Expect.throws (fun () -> WorkflowStep.updateInputAt 0 id (importedFirst ())) "An import has no input to update."
            let added = importedFirst ()
            WorkflowStep.addInput (StepInput.create("last")) added
            Expect.equal added.In.[3] (ListEntry.Value (StepInput.create("last"))) "A new input is a value entry at the end."

        testCase "StepOutput.tryId gives the id of an id and of an object and none for a directive" <| fun _ ->
            Expect.equal (StepOutput.tryId (StepOutputString "dir")) (Some "dir") "The id of a string."
            Expect.equal (StepOutput.tryId (StepOutputRecord (StepOutputParameter.create "dir"))) (Some "dir") "The id of an object."
            Expect.equal (StepOutput.tryId (StepOutputInclude "id.txt")) None "The id is in the file."
            Expect.equal (StepOutput.tryId (StepOutputImport "outputs.yml")) None "The outputs are in the file."
    ]

let private toolStep () =
    WorkflowStep.fromRunPath(
        id = "step1",
        in_ = ResizeArray(),
        out_ = ResizeArray [| StepOutputString "out" |],
        runPath = "./tool.cwl"
    )

let private echoTool () =
    CWLToolDescription(outputs = ResizeArray(), baseCommand = ResizeArray [ (SchemaSaladString.Literal "echo") ])

let testResolvedRun =
    testList "ResolvedRun" [
        testCase "a step has no resolved run until a process is set" <| fun _ ->
            let step = toolStep ()
            Expect.isNone step.ResolvedRun.Run "The decoder and the constructor leave the resolved run empty."
            Expect.equal (WorkflowStep.getResolvedRun step) (RunString "./tool.cwl") "Without a process the run stands for itself."
        testCase "getResolvedRun returns the process the reference resolved to" <| fun _ ->
            let step = toolStep ()
            let tool = echoTool ()
            step.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool tool) }
            Expect.equal step.Run (RunString "./tool.cwl") "The run keeps the reference."
            Expect.isSome (WorkflowStepRunOps.tryGetTool (WorkflowStep.getResolvedRun step)) "The resolved run is the tool."
        testCase "an inline run is its own resolved run" <| fun _ ->
            let tool = echoTool ()
            let step = toolStep ()
            step.Run <- WorkflowStepRunOps.fromTool tool
            Expect.isNone step.ResolvedRun.Run "An inline run has nothing to resolve."
            Expect.equal (WorkflowStep.getResolvedRun step) (WorkflowStepRunOps.fromTool tool) "The inline process is the run."
        testCase "setting the run drops the process of the previous reference" <| fun _ ->
            let step = toolStep ()
            step.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool (echoTool ())) }
            step.Run <- RunString "./other.cwl"
            Expect.isNone step.ResolvedRun.Run "The process belonged to ./tool.cwl."
            Expect.equal (WorkflowStep.getResolvedRun step) (RunString "./other.cwl") "The new reference stands for itself."
        testCase "the resolved run stays out of the equality and the hash of the step" <| fun _ ->
            let resolved = toolStep ()
            resolved.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool (echoTool ())) }
            let plain = toolStep ()
            Expect.equal resolved plain "A step with a resolved run equals the step without it."
            Expect.equal (hash resolved) (hash plain) "Both hash alike."
            let other = toolStep ()
            other.Run <- RunString "./other.cwl"
            Expect.notEqual resolved other "Another reference still makes another step."
        testCase "the encoder writes the reference of a step with a resolved run" <| fun _ ->
            let step = toolStep ()
            step.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool (echoTool ())) }
            let workflow = CWLWorkflowDescription(steps = ListEntry.ofValues [ step ], inputs = ResizeArray(), outputs = ResizeArray())
            let written = Encode.encodeWorkflowDescription workflow
            Expect.stringContains written "run: ./tool.cwl" "The file refers to the tool."
            Expect.isFalse (written.Contains "baseCommand") "The tool stays in its own file."
    ]

let main = 
    testList "WorkflowStep" [
        testWorkflowStep
        testWorkflowStepOps
        testResolvedRun
    ]
