module Tests.CWLWorkflow

open ARCtrl.CWL
open TestingUtils
open TestingUtils.CWL

let decodeCWLWorkflowDescription: CWLWorkflowDescription =
    TestObjects.CWL.Workflow.workflowFile
    |> Decode.decodeWorkflow

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
                CWLInput("cores", CWLType.Int);
                CWLInput("db", CWLType.File (FileInstance()));
                CWLInput ("stage", CWLType.Directory (DirectoryInstance()));
                CWLInput ("outputMzML", CWLType.Directory (DirectoryInstance()));
                CWLInput ("outputPSM", CWLType.Directory (DirectoryInstance()));
                CWLInput ("inputMzML", CWLType.Directory (DirectoryInstance()));
                CWLInput ("paramsMzML", CWLType.File (FileInstance()));
                CWLInput ("paramsPSM", CWLType.File (FileInstance()))
            |]
            let actual = decodeCWLWorkflowDescription.Inputs
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].InputBinding expected.[i].InputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
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
                    Expect.equal actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                    let actual = workflowSteps.[1].Run
                    Expect.equal actual expected ""
            ]
            testList "In" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = ResizeArray [|
                        {Id = "stageDirectory"; Source = Some (ResizeArray [|"stage"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "inputDirectory"; Source = Some (ResizeArray [|"inputMzML"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "params"; Source = Some (ResizeArray [|"paramsMzML"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "outputDirectory"; Source = Some (ResizeArray [|"outputMzML"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "parallelismLevel"; Source = Some (ResizeArray [|"cores"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None}
                    |]
                    let actual = workflowSteps.[0].In
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                        Expect.equal actual.DefaultValue expected.DefaultValue ""
                        Expect.equal actual.ValueFrom expected.ValueFrom ""
                        Expect.equal actual.LinkMerge expected.LinkMerge ""
                    ) expected actual
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = ResizeArray [|
                        {Id = "stageDirectory"; Source = Some (ResizeArray [|"stage"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "inputDirectory"; Source = Some (ResizeArray [|"MzMLToMzlite/dir1";"MzMLToMzlite/dir2"|]); DefaultValue = None; ValueFrom = None; LinkMerge = Some "merge_flattened"};
                        {Id = "database"; Source = Some (ResizeArray [|"db"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "params"; Source = Some (ResizeArray [|"paramsPSM"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                        {Id = "outputDirectory"; Source = Some (ResizeArray [|"outputPSM"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None}
                        {Id = "parallelismLevel"; Source = Some (ResizeArray [|"cores"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                    |]
                    let actual = workflowSteps.[1].In
                    Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                        Expect.equal actual.Id expected.Id ""
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                        Expect.equal actual.DefaultValue expected.DefaultValue ""
                        Expect.equal actual.ValueFrom expected.ValueFrom ""
                        Expect.equal actual.LinkMerge expected.LinkMerge ""
                    ) expected actual
            ]
            testList "Out" [
                testCase "MzMLToMzlite" <| fun _ ->
                    let expected = {Id = ResizeArray [|"dir"|]}
                    let actual = workflowSteps.[0].Out
                    Expect.sequenceEqual actual.Id expected.Id ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = {Id = ResizeArray [|"dir"|]}
                    let actual = workflowSteps.[1].Out
                    Expect.sequenceEqual actual.Id expected.Id ""
            ]
        ]
        testCase "outputs" <| fun _ ->
            let expected = ResizeArray [|
                CWLOutput("mzlite", CWLType.Directory (DirectoryInstance()), outputSource = "MzMLToMzlite/dir");
                CWLOutput("psm", CWLType.Directory (DirectoryInstance()), outputSource = "PeptideSpectrumMatching/dir")
            |]
            let actual = decodeCWLWorkflowDescription.Outputs
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].OutputBinding expected.[i].OutputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
                Expect.equal actual.[i].OutputSource expected.[i].OutputSource ""
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
        ]
    ]
let main = 
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescriptionDecode
        testCWLWorkflowDescriptionEncode
    ]