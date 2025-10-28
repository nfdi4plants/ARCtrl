module Tests.CWLWorkflow

open ARCtrl.CWL
open TestingUtils

let decodeCWLWorkflowDescription: CWLWorkflowDescription =
    TestObjects.CWL.Workflow.workflowFile
    |> Decode.decodeWorkflow

let testCWLWorkflowDescription =
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

let main = 
    testList "CWLWorkflowDescription" [
        testCWLWorkflowDescription
    ]