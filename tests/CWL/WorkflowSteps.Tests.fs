module Tests.WorkflowSteps

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.WorkflowSteps
open ARCtrl.CWL.Outputs.Workflow
open ARCtrl.CWL.Inputs.Workflow
open YAMLicious
open TestingUtils

let decodeWorkflowStep =
    TestObjects.CWL.WorkflowSteps.workflowStepsFileContent
    |> Decode.read
    |> Decode.stepsDecoder

let testWorkflowStep =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.isTrue (2 = decodeWorkflowStep.Length) "Length of WorkflowSteps is not 2"
        testList "IDs" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = "MzMLToMzlite"
                let actual = decodeWorkflowStep.[0].Id
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "PeptideSpectrumMatching"
                let actual = decodeWorkflowStep.[1].Id
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "Run" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = "./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl"
                let actual = decodeWorkflowStep.[0].Run
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = "./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl"
                let actual = decodeWorkflowStep.[1].Run
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "In" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = [|
                    {Id = "stageDirectory"; Source = Some "stage"; DefaultValue = None; ValueFrom = None};
                    {Id = "inputDirectory"; Source = Some "inputMzML"; DefaultValue = None; ValueFrom = None}
                |]
                let actual = decodeWorkflowStep.[0].In
                Expect.isTrue
                    (expected = actual)
                    $"Expected:\n{expected.[0]}\n{expected.[1]}\nActual:\n{actual.[0]}\n{actual.[1]}"
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = [|
                    {Id = "stageDirectory"; Source = Some "stage"; DefaultValue = None; ValueFrom = None};
                    {Id = "inputDirectory"; Source = Some "MzMLToMzlite/dir"; DefaultValue = None; ValueFrom = None };
                    {Id = "parallelismLevel"; Source = None; DefaultValue = Some "8"; ValueFrom = None};
                    {Id = "outputDirectory"; Source = None; DefaultValue = None; ValueFrom = Some "output"}|]
                let actual = decodeWorkflowStep.[1].In
                Expect.isTrue
                    (expected = actual)
                    $"Expected:\n{expected.[0]}\n{expected.[1]}\n{expected.[2]}\n{expected.[3]}\nActual:\n{actual.[0]}\n{actual.[1]}\n{actual.[2]}\n{actual.[3]}"
        ]
        testList "Out" [
            testCase "MzMLToMzlite" <| fun _ ->
                let expected = {Id = [|"dir"|]}
                let actual = decodeWorkflowStep.[0].Out
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
            testCase "PeptideSpectrumMatching" <| fun _ ->
                let expected = {Id = [|"dir1";"dir2"|]}
                let actual = decodeWorkflowStep.[1].Out
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
    ]

let main = 
    testList "WorkflowStep" [
        testWorkflowStep
    ]