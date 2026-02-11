module Tests.Adapters

open ARCtrl
open ARCtrl.CWL
open ARCtrl.FileSystem
open ARCtrl.WorkflowGraph
open TestingUtils
open CrossAsync

let private workflowFixturePath =
    ArcPathHelper.combineMany [|
        TestObjects.IO.testSimpleARCWithCWL
        "workflows"
        "ProteomIQon"
        "workflow.cwl"
    |]

let private runFixturePath =
    ArcPathHelper.combineMany [|
        TestObjects.IO.testSimpleARCWithCWL
        "runs"
        "tests"
        "run.cwl"
    |]

let private loadProcessingUnitFromPath (path: string) =
    crossAsync {
        let! content = FileSystemHelper.readFileTextAsync path
        return Decode.decodeCWLProcessingUnit content
    }

let tests_adapters =
    testList "Adapters" [
        testCaseCrossAsync "ofWorkflow returns graph for workflow with CWL description" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let workflow = ArcWorkflow.init "ProteomIQon"
            workflow.CWLDescription <- Some processingUnit
            let result = Adapters.ofWorkflow workflow
            match result with
            | Ok graph ->
                let stepNodes =
                    graph.Nodes
                    |> Seq.filter (fun n -> n.Kind = NodeKind.StepNode)
                    |> Seq.length
                Expect.equal stepNodes 12 ""
                Expect.equal graph.EdgeCount 93 ""
            | Error issue ->
                Expect.isTrue false $"Expected Ok graph but got Error: {issue.Message}"
        })

        testCaseCrossAsync "ofRun resolves nested workflow when investigation lookup exists" (crossAsync {
            let! workflowCwl = loadProcessingUnitFromPath workflowFixturePath
            let! runCwl = loadProcessingUnitFromPath runFixturePath

            let inv = ArcInvestigation.init "AdapterInv"
            let workflow = ArcWorkflow.init "ProteomIQon"
            workflow.CWLDescription <- Some workflowCwl
            inv.AddWorkflow workflow

            let run = ArcRun.init "tests"
            run.CWLDescription <- Some runCwl
            inv.AddRun run

            let result = Adapters.ofRun run
            match result with
            | Ok graph ->
                let nestedStepExists =
                    graph.Nodes
                    |> Seq.exists (fun n ->
                        n.Kind = NodeKind.StepNode
                        && n.Id = GraphId.stepNodeId "tests/Workflow/run" "PeptideSpectrumMatching"
                    )
                Expect.isTrue nestedStepExists ""
                Expect.equal graph.EdgeCount 131 ""
            | Error issue ->
                Expect.isTrue false $"Expected Ok graph but got Error: {issue.Message}"
        })

        testCaseCrossAsync "ofInvestigation returns one workflow graph and one run graph" (crossAsync {
            let! workflowCwl = loadProcessingUnitFromPath workflowFixturePath
            let! runCwl = loadProcessingUnitFromPath runFixturePath

            let inv = ArcInvestigation.init "AdapterInv2"
            let workflow = ArcWorkflow.init "ProteomIQon"
            workflow.CWLDescription <- Some workflowCwl
            inv.AddWorkflow workflow

            let run = ArcRun.init "tests"
            run.CWLDescription <- Some runCwl
            inv.AddRun run

            let index = Adapters.ofInvestigation inv
            Expect.equal index.WorkflowGraphs.Count 1 ""
            Expect.equal index.RunGraphs.Count 1 ""
            match snd index.WorkflowGraphs.[0], snd index.RunGraphs.[0] with
            | Ok workflowGraph, Ok runGraph ->
                Expect.equal workflowGraph.EdgeCount 93 ""
                Expect.equal runGraph.EdgeCount 131 ""
            | _ ->
                Expect.isTrue false "Expected both workflow and run graph results to be Ok"
        })

        testCase "ofWorkflow returns error when CWLDescription is missing" <| fun () ->
            let workflow = ArcWorkflow.init "NoCWLWorkflow"
            workflow.CWLDescription <- None
            let result = Adapters.ofWorkflow workflow
            match result with
            | Ok _ ->
                Expect.isTrue false "Expected Error but got Ok"
            | Error issue ->
                Expect.equal issue.Kind GraphIssueKind.MissingCwlDescription ""

        testCase "ofRun returns error when CWLDescription is missing" <| fun () ->
            let run = ArcRun.init "NoCWLRun"
            run.CWLDescription <- None
            let result = Adapters.ofRun run
            match result with
            | Ok _ ->
                Expect.isTrue false "Expected Error but got Ok"
            | Error issue ->
                Expect.equal issue.Kind GraphIssueKind.MissingCwlDescription ""
    ]

let main = testList "Adapters" [ tests_adapters ]
