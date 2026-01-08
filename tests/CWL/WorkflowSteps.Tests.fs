module Tests.WorkflowSteps

open ARCtrl.CWL
open YAMLicious
open TestingUtils

let decodeWorkflowStep =
    TestObjects.CWL.WorkflowSteps.workflowStepsFileContent
    |> Decode.read
    |> Decode.stepsDecoder

let testWorkflowStep =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 2  decodeWorkflowStep.Count ""
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
                Expect.equal actual expected ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                let actual = decodeWorkflowStep.[1].Run
                Expect.equal actual expected ""
        ]
        testList "In" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = ResizeArray [|
                    {Id = "stageDirectory"; Source = Some (ResizeArray [|"stage"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None};
                    {Id = "inputDirectory"; Source = Some (ResizeArray [|"inputMzML"|]); DefaultValue = None; ValueFrom = None; LinkMerge = None}
                |]
                let actual = decodeWorkflowStep.[0].In
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
                    {Id = "parallelismLevel"; Source = None; DefaultValue = Some "8"; ValueFrom = None; LinkMerge = None};
                    {Id = "outputDirectory"; Source = None; DefaultValue = None; ValueFrom = Some "output"; LinkMerge = None}|]
                let actual = decodeWorkflowStep.[1].In
                Seq.iter2 (fun (expected: StepInput) (actual: StepInput) ->
                    Expect.equal actual.Id expected.Id ""
                    if expected.Source.IsSome then
                        Expect.sequenceEqual actual.Source.Value expected.Source.Value ""
                    Expect.equal actual.DefaultValue expected.DefaultValue ""
                    Expect.equal actual.ValueFrom expected.ValueFrom ""
                    Expect.equal actual.LinkMerge expected.LinkMerge ""
                ) expected actual
        ]
        testList "Out" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = {Id = ResizeArray [|"dir"|]}
                let actual = decodeWorkflowStep.[0].Out
                Expect.sequenceEqual actual.Id expected.Id ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = {Id = ResizeArray [|"dir1";"dir2"|]}
                let actual = decodeWorkflowStep.[1].Out
                Expect.sequenceEqual actual.Id expected.Id ""
        ]
    ]

let main = 
    testList "WorkflowStep" [
        testWorkflowStep
    ]