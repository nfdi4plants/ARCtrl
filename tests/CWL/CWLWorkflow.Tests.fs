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
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None None None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"inputMzML"|])) None None None
                        mkStepInput "params" (Some (ResizeArray [|"paramsMzML"|])) None None None
                        mkStepInput "outputDirectory" (Some (ResizeArray [|"outputMzML"|])) None None None
                        mkStepInput "parallelismLevel" (Some (ResizeArray [|"cores"|])) None None None
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
                        mkStepInput "stageDirectory" (Some (ResizeArray [|"stage"|])) None None None
                        mkStepInput "inputDirectory" (Some (ResizeArray [|"MzMLToMzlite/dir1"; "MzMLToMzlite/dir2"|])) None None (Some MergeFlattened)
                        mkStepInput "database" (Some (ResizeArray [|"db"|])) None None None
                        mkStepInput "params" (Some (ResizeArray [|"paramsPSM"|])) None None None
                        mkStepInput "outputDirectory" (Some (ResizeArray [|"outputPSM"|])) None None None
                        mkStepInput "parallelismLevel" (Some (ResizeArray [|"cores"|])) None None None
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
                CWLOutput("mzlite", CWLType.Directory (DirectoryInstance()), outputSource = "MzMLToMzlite/dir")
                CWLOutput("psm", CWLType.Directory (DirectoryInstance()), outputSource = "PeptideSpectrumMatching/dir")
            |]
            let actual = decodeCWLWorkflowDescription.Outputs
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].OutputBinding expected.[i].OutputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
                Expect.equal actual.[i].OutputSource expected.[i].OutputSource ""
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
        testCase "invalid scatterMethod fails decode" <| fun _ ->
            let invalidScatter = TestObjects.CWL.Workflow.workflowWithInvalidScatterMethodFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidScatter |> ignore) "Invalid scatterMethod should fail"
        testCase "unsupported inline run class fails decode" <| fun _ ->
            let invalidInlineRun = TestObjects.CWL.Workflow.workflowWithUnsupportedInlineRunClassFile
            Expect.throws (fun _ -> Decode.decodeWorkflow invalidInlineRun |> ignore) "Inline ExpressionTool should fail until supported"
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
                original.Hints <- Some (ResizeArray [StepInputExpressionRequirement])
                let encoded = Encode.encodeWorkflowDescription original
                let decoded = Decode.decodeWorkflow encoded
                Expect.stringContains encoded "hints:" "Workflow hints should be present in encoded output"
                let hints = Expect.wantSome decoded.Hints "Workflow hints should survive roundtrip"
                Expect.equal hints.[0] StepInputExpressionRequirement ""
        ]
    ]

let main = 
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescriptionDecode
        testCWLWorkflowDescriptionEncode
    ]
