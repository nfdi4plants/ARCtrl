module Tests.CWLWorkflow

open ARCtrl.CWL
open DynamicObj
open TestingUtils
open TestingUtils.CWL
open YAMLicious.YAMLiciousTypes

let decodeCWLWorkflowDescription: CWLWorkflowDescription =
    TestObjects.CWL.Workflow.workflowFile
    |> Decode.decodeWorkflow

let tryYamlScalarString (y: YAMLElement) =
    match y with
    | YAMLElement.Object [YAMLElement.Value v]
    | YAMLElement.Value v -> Some v.Value
    | _ -> None

let tryYamlScalarSequence (y: YAMLElement) =
    let tryScalarValue element =
        match element with
        | YAMLElement.Object [YAMLElement.Value v]
        | YAMLElement.Value v -> Some v.Value
        | _ -> None
    match y with
    | YAMLElement.Object [YAMLElement.Sequence values]
    | YAMLElement.Sequence values ->
        values
        |> List.choose tryScalarValue
        |> ResizeArray
        |> Some
    | _ -> None

let mkStepInput id source linkMerge =
    {
        Id = id
        Source = source
        DefaultValue = None
        ValueFrom = None
        LinkMerge = linkMerge
        PickValue = None
        Doc = None
        LoadContents = None
        LoadListing = None
        Label = None
    }

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
                CWLInput("db", CWLType.File (FileInstance()))
                CWLInput ("stage", CWLType.Directory (DirectoryInstance()))
                CWLInput ("outputMzML", CWLType.Directory (DirectoryInstance()))
                CWLInput ("outputPSM", CWLType.Directory (DirectoryInstance()))
                CWLInput ("inputMzML", CWLType.Directory (DirectoryInstance()))
                CWLInput ("paramsMzML", CWLType.File (FileInstance()))
                CWLInput ("paramsPSM", CWLType.File (FileInstance()))
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
                CWLOutput("mzlite", CWLType.Directory (DirectoryInstance()), outputSource = OutputSource.Single "MzMLToMzlite/dir")
                CWLOutput("psm", CWLType.Directory (DirectoryInstance()), outputSource = OutputSource.Single "PeptideSpectrumMatching/dir")
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
            Expect.equal step.In.[0].LoadListing (Some "deep_listing") ""
            Expect.equal step.In.[0].Label (Some "Input label") ""
            Expect.equal step.In.[0].LinkMerge (Some MergeNested) ""
            let expectedOut = ResizeArray [| StepOutputRecord { Id = "out" } |]
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
        testCase "steps array form decode with when/pickValue/doc/default" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithStepsArrayFile
            let step = decoded.Steps.[0]
            let stepInput = step.In.[0]
            Expect.equal step.Id "step1" ""
            Expect.equal step.When_ (Some "$(inputs.input1 != null)") ""
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
            Expect.equal expressionTool.Expression "$(null)" ""
        testCase "inline ExpressionTool chain decodes all steps" <| fun _ ->
            let decoded = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithInlineExpressionToolChainFile
            Expect.equal decoded.Steps.Count 3 "Should have 3 steps"
            for i in 0..2 do
                let step = decoded.Steps.[i]
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool step.Run)
                        $"Step {i} should be ExpressionTool"
                Expect.isTrue (expressionTool.Expression.Length > 0) $"Step {i} expression should be non-empty"
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

            match step2.Inputs.Value.[0].Type_ with
            | Some (Array _) -> ()
            | other ->
                Expect.isTrue false $"step2 input should be int[] but got %A{other}"

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
                Expect.equal step.When_ (Some "$(inputs.input1 != null)") ""
                Expect.equal stepInput.PickValue (Some FirstNonNull) ""
                Expect.equal stepInput.Doc (Some "Input docs") ""
                let defaultEntry =
                    Expect.wantSome stepInput.DefaultValue "Step input default should remain present as structured YAML"
                match defaultEntry with
                | YAMLElement.Object _
                | YAMLElement.Sequence _ ->
                    Expect.isTrue true ""
                | _ ->
                    Expect.isTrue false (sprintf "Expected structured default, got %A" defaultEntry)
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
                Expect.equal decoded.Steps.[0].When_ (Some expression) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                Expect.equal roundTripped.Steps.[0].When_ (Some expression) ""
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
                Expect.equal decodedRequirements.[0] (NetworkAccessRequirement { NetworkAccess = true }) ""
                let encoded = Encode.encodeWorkflowDescription decoded
                let roundTripped = Decode.decodeWorkflow encoded
                let roundTrippedStep = roundTripped.Steps.[0]
                let roundTrippedHints = Expect.wantSome roundTrippedStep.Hints "Step hints should survive roundtrip"
                let roundTrippedRequirements = Expect.wantSome roundTrippedStep.Requirements "Step requirements should survive roundtrip"
                Expect.equal roundTrippedHints.[0] (KnownHint StepInputExpressionRequirement) ""
                Expect.equal roundTrippedRequirements.[0] (NetworkAccessRequirement { NetworkAccess = true }) ""
            testCase "inline ExpressionTool roundtrip preserves expression" <| fun _ ->
                let original = Decode.decodeWorkflow TestObjects.CWL.ExpressionTool.workflowWithMixedToolAndExpressionStepFile
                let encoded = Encode.encodeWorkflowDescription original
                let roundTripped = Decode.decodeWorkflow encoded
                let expressionTool =
                    Expect.wantSome
                        (WorkflowStepRunOps.tryGetExpressionTool roundTripped.Steps.[1].Run)
                        "Should roundtrip ExpressionTool run"
                Expect.equal expressionTool.Expression "$({'out': inputs.y})" "Expression preserved"
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

                Expect.stringContains expressionTool.Expression "return" "Expression should decode after roundtrip."
                Expect.stringContains expressionTool.Expression "var name = \"arc\"" "Variable assignment should survive roundtrip."
                Expect.stringContains expressionTool.Expression "{\"out\": name}" "Return object should survive roundtrip."
        ]
    ]

let main = 
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescriptionDecode
        testCWLWorkflowDescriptionEncode
    ]
