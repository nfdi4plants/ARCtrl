module Tests.CWLWorkflow

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open ARCtrl.CWL.Inputs
open ARCtrl.CWL.Outputs
open ARCtrl.CWL.Outputs.Workflow
open ARCtrl.CWL.Inputs.Workflow
open TestingUtils

let decodeCWLWorkflowDescription =
    TestObjects.CWL.Workflow.workflowFile
    |> Decode.decodeWorkflow

let testCWLWorkflowDescription =
    testList "Decode" [
        testCase "Class" <| fun _ ->
            let expected = CWLClass.Workflow
            let actual = decodeCWLWorkflowDescription.Class
            Expect.equal actual expected ""
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
                Input("cores", CWLType.Int);
                Input("db", CWLType.File (FileInstance()));
                Input ("stage", CWLType.Directory (DirectoryInstance()));
                Input ("outputMzML", CWLType.Directory (DirectoryInstance()));
                Input ("outputPSM", CWLType.Directory (DirectoryInstance()));
                Input ("inputMzML", CWLType.Directory (DirectoryInstance()));
                Input ("paramsMzML", CWLType.File (FileInstance()));
                Input ("paramsPSM", CWLType.File (FileInstance()))
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
                        {Id = "stageDirectory"; Source = Some "stage"; DefaultValue = None; ValueFrom = None};
                        {Id = "inputDirectory"; Source = Some "inputMzML"; DefaultValue = None; ValueFrom = None};
                        {Id = "params"; Source = Some "paramsMzML"; DefaultValue = None; ValueFrom = None};
                        {Id = "outputDirectory"; Source = Some "outputMzML"; DefaultValue = None; ValueFrom = None};
                        {Id = "parallelismLevel"; Source = Some "cores"; DefaultValue = None; ValueFrom = None}
                    |]
                    let actual = workflowSteps.[0].In
                    Expect.sequenceEqual actual expected ""
                testCase "PeptideSpectrumMatching" <| fun _ ->
                    let expected = ResizeArray [|
                        {Id = "stageDirectory"; Source = Some "stage"; DefaultValue = None; ValueFrom = None};
                        {Id = "inputDirectory"; Source = Some "MzMLToMzlite/dir"; DefaultValue = None; ValueFrom = None };
                        {Id = "database"; Source = Some "db"; DefaultValue = None; ValueFrom = None};
                        {Id = "params"; Source = Some "paramsPSM"; DefaultValue = None; ValueFrom = None};
                        {Id = "outputDirectory"; Source = Some "outputPSM"; DefaultValue = None; ValueFrom = None}
                        {Id = "parallelismLevel"; Source = Some "cores"; DefaultValue = None; ValueFrom = None};
                    |]
                    let actual = workflowSteps.[1].In
                    Expect.sequenceEqual actual expected ""
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
                Output("mzlite", CWLType.Directory (DirectoryInstance()), outputSource = "MzMLToMzlite/dir");
                Output("psm", CWLType.Directory (DirectoryInstance()), outputSource = "PeptideSpectrumMatching/dir")
            |]
            let actual = decodeCWLWorkflowDescription.Outputs
            for i = 0 to actual.Count - 1 do
                Expect.equal actual.[i].Name expected.[i].Name ""
                Expect.equal actual.[i].OutputBinding expected.[i].OutputBinding ""
                Expect.equal actual.[i].Type_ expected.[i].Type_ ""
    ]

let main = 
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescription
    ]