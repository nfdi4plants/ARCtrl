module Tests.WorkflowGraphExtensions

open ARCtrl
open ARCtrl.WorkflowGraph
open ARCtrl.WorkflowGraphExtensions
open TestingUtils
open CrossAsync
open Tests.WorkflowGraphTestHelpers

let tests_extensions =
    testList "WorkflowGraph.Extensions" [
        testCaseCrossAsync "BuildWorkflowGraphs returns workflow graph results" (crossAsync {
            let! workflowCwl = loadProcessingUnitFromPath workflowFixturePath
            let arc = ARC("ExtGraphArc1")
            let workflow = ArcWorkflow.init "ProteomIQon"
            workflow.CWLDescription <- Some workflowCwl
            arc.AddWorkflow workflow

            let results = arc.BuildWorkflowGraphs()
            Expect.equal results.Count 1 ""
            match snd results.[0] with
            | Ok graph ->
                Expect.equal graph.EdgeCount 93 ""
            | Error issue ->
                Expect.isTrue false $"Expected Ok graph but got Error: {issue.Message}"
        })

        testCaseCrossAsync "BuildRunGraphs returns resolved run graph results" (crossAsync {
            let! workflowCwl = loadProcessingUnitFromPath workflowFixturePath
            let! runCwl = loadProcessingUnitFromPath runFixturePath
            let arc = ARC("ExtGraphArc2")

            let workflow = ArcWorkflow.init "ProteomIQon"
            workflow.CWLDescription <- Some workflowCwl
            arc.AddWorkflow workflow

            let run = ArcRun.init "tests"
            run.CWLDescription <- Some runCwl
            arc.AddRun run

            let results = arc.BuildRunGraphs()
            Expect.equal results.Count 1 ""
            match snd results.[0] with
            | Ok graph ->
                Expect.equal graph.EdgeCount 131 ""
            | Error issue ->
                Expect.isTrue false $"Expected Ok graph but got Error: {issue.Message}"
        })

        testCaseCrossAsync "BuildAllProcessingUnitGraphs returns workflow and run graph indexes" (crossAsync {
            let! workflowCwl = loadProcessingUnitFromPath workflowFixturePath
            let! runCwl = loadProcessingUnitFromPath runFixturePath
            let arc = ARC("ExtGraphArc3")

            let workflow = ArcWorkflow.init "ProteomIQon"
            workflow.CWLDescription <- Some workflowCwl
            arc.AddWorkflow workflow

            let run = ArcRun.init "tests"
            run.CWLDescription <- Some runCwl
            arc.AddRun run

            let index = arc.BuildAllProcessingUnitGraphs()
            Expect.equal index.WorkflowGraphs.Count 1 ""
            Expect.equal index.RunGraphs.Count 1 ""
            match snd index.WorkflowGraphs.[0], snd index.RunGraphs.[0] with
            | Ok workflowGraph, Ok runGraph ->
                Expect.equal workflowGraph.EdgeCount 93 ""
                Expect.equal runGraph.EdgeCount 131 ""
            | _ ->
                Expect.isTrue false "Expected both workflow and run graph results to be Ok"
        })
    ]

let main = testList "WorkflowGraph.Extensions" [ tests_extensions ]
