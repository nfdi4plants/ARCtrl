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
                    {Id = "stageDirectory"; Source = Some "stage"; DefaultValue = None; ValueFrom = None};
                    {Id = "inputDirectory"; Source = Some "inputMzML"; DefaultValue = None; ValueFrom = None}
                |]
                let actual = decodeWorkflowStep.[0].In
                Expect.sequenceEqual actual expected ""
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = ResizeArray [|
                    {Id = "stageDirectory"; Source = Some "stage"; DefaultValue = None; ValueFrom = None};
                    {Id = "inputDirectory"; Source = Some "MzMLToMzlite/dir"; DefaultValue = None; ValueFrom = None };
                    {Id = "parallelismLevel"; Source = None; DefaultValue = Some "8"; ValueFrom = None};
                    {Id = "outputDirectory"; Source = None; DefaultValue = None; ValueFrom = Some "output"}|]
                let actual = decodeWorkflowStep.[1].In
                Expect.sequenceEqual actual expected ""
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