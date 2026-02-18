module Tests.Integration

open ARCtrl
open ARCtrl.CWL
open ARCtrl.WorkflowGraph
open CrossAsync
open TestingUtils
open Tests.WorkflowGraphTestHelpers

let private countEdges kind (graph: WorkflowGraph) =
    graph.Edges
    |> Seq.filter (fun e -> e.Kind = kind)
    |> Seq.length

let private countNodes predicate (graph: WorkflowGraph) =
    graph.Nodes
    |> Seq.filter predicate
    |> Seq.length

let private toFixtureFullPath (fixtureRoot: string) (path: string) =
    let normalizedRelative = ArcPathHelper.normalizePathKey path
    ArcPathHelper.combine fixtureRoot normalizedRelative

let private getFileNameWithoutExtension (path: string) =
    let fileName = ArcPathHelper.getFileName path
    let extensionIndex = fileName.LastIndexOf('.')
    if extensionIndex <= 0 then
        fileName
    else
        fileName.Substring(0, extensionIndex)

let private getDirectoryPath (path: string) =
    let segments = ArcPathHelper.split path
    if segments.Length <= 1 then
        ""
    else
        segments.[0 .. segments.Length - 2]
        |> ArcPathHelper.combineMany

let private buildFixtureResolver (fixtureRoot: string) =
    crossAsync {
        let! relativePaths = FileSystemHelper.getAllFilePathsAsync fixtureRoot
        let cwlRelativePaths =
            relativePaths
            |> Array.filter (fun p -> p.EndsWith(".cwl"))

        let! lookupEntries =
            cwlRelativePaths
            |> Array.map (fun relativePath ->
                crossAsync {
                    let fullPath = ArcPathHelper.combine fixtureRoot relativePath
                    let! content = FileSystemHelper.readFileTextAsync fullPath
                    let processingUnit = Decode.decodeCWLProcessingUnit content
                    let normalizedRelativePath = ArcPathHelper.normalizePathKey relativePath
                    let normalizedFullPath = ArcPathHelper.normalizePathKey fullPath
                    let fileNameOnly = ArcPathHelper.normalizePathKey (ArcPathHelper.getFileName relativePath)
                    return [
                        normalizedRelativePath, processingUnit
                        normalizedFullPath, processingUnit
                        fileNameOnly, processingUnit
                    ]
                }
            )
            |> CrossAsync.all

        let lookupMap =
            lookupEntries
            |> Seq.collect id
            |> Seq.distinctBy fst
            |> Map.ofSeq

        return fun (path: string) ->
            lookupMap
            |> Map.tryFind (ArcPathHelper.normalizePathKey path)
    }

let tests_workflowFixture =
    testList "workflow.cwl fixture" [
        testCaseCrossAsync "builds without diagnostics" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            Expect.equal graph.Diagnostics.Count 0 ""
        })

        testCaseCrossAsync "root is workflow and has 12 steps" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let workflowNodes =
                graph.Nodes
                |> Seq.filter (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow)
                |> Seq.length
            let stepNodes =
                graph.Nodes
                |> Seq.filter (fun n -> n.Kind = NodeKind.StepNode)
                |> Seq.length
            Expect.equal workflowNodes 1 ""
            Expect.equal stepNodes 12 ""
        })

        testCaseCrossAsync "all known step labels are present" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let expected =
                [
                    "MzMLToMzlite"
                    "PeptideDB"
                    "PeptideSpectrumMatching"
                    "PSMStatistics"
                    "PSMBasedQuantification"
                    "ProteinInference"
                    "QuantBasedAlignment"
                    "AlignmentBasedQuantification"
                    "AlignmentBasedQuantStatistics"
                    "AddDeducedPeptides"
                    "JoinQuantPepIonsWithProteins"
                    "LabeledProteinQuantification"
                ]
            expected
            |> List.iter (fun stepId ->
                let stepNodeId = GraphId.stepNodeId "root" stepId
                let exists =
                    graph.Nodes
                    |> Seq.exists (fun n -> n.Id = stepNodeId && n.Kind = NodeKind.StepNode)
                Expect.isTrue exists $"Missing step node: {stepId}"
            )
        })

        testCaseCrossAsync "workflow root has 24 inputs and 12 outputs" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let inputPortCount =
                WorkflowGraphQueries.getInputPorts graph.RootProcessingUnitNodeId graph
                |> List.length
            let outputPortCount =
                WorkflowGraphQueries.getOutputPorts graph.RootProcessingUnitNodeId graph
                |> List.length
            Expect.equal inputPortCount 24 ""
            Expect.equal outputPortCount 12 ""
        })

        testCaseCrossAsync "edge counts match expected fixture structure" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            Expect.equal (countEdges EdgeKind.Contains graph) 12 ""
            Expect.equal (countEdges EdgeKind.Calls graph) 12 ""
            Expect.equal (countEdges EdgeKind.DataFlow graph) 27 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowInput graph) 30 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowOutput graph) 12 ""
            Expect.equal graph.EdgeCount 93 ""
        })

        testCaseCrossAsync "fan-out PeptideDB/db has at least four outgoing dataflow edges" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let peptideDbOutputPortId =
                let stepNodeId = GraphId.stepNodeId "root" "PeptideDB"
                GraphId.portNodeId stepNodeId PortDirection.Output "db"
            let outgoing =
                graph.Edges
                |> Seq.filter (fun e -> e.Kind = EdgeKind.DataFlow && e.SourceNodeId = peptideDbOutputPortId)
                |> Seq.length
            Expect.isTrue (outgoing >= 4) ""
        })

        testCaseCrossAsync "fan-out MzMLToMzlite/dir has three outgoing dataflow edges" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let mzMlOutputPortId =
                let stepNodeId = GraphId.stepNodeId "root" "MzMLToMzlite"
                GraphId.portNodeId stepNodeId PortDirection.Output "dir"
            let outgoing =
                graph.Edges
                |> Seq.filter (fun e -> e.Kind = EdgeKind.DataFlow && e.SourceNodeId = mzMlOutputPortId)
                |> Seq.length
            Expect.equal outgoing 3 ""
        })

        testCaseCrossAsync "fan-out PSMBasedQuantification/dir has at least three outgoing dataflow edges" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let quantOutputPortId =
                let stepNodeId = GraphId.stepNodeId "root" "PSMBasedQuantification"
                GraphId.portNodeId stepNodeId PortDirection.Output "dir"
            let outgoing =
                graph.Edges
                |> Seq.filter (fun e -> e.Kind = EdgeKind.DataFlow && e.SourceNodeId = quantOutputPortId)
                |> Seq.length
            Expect.isTrue (outgoing >= 3) ""
        })

        testCaseCrossAsync "PeptideDB has two outputs and all others one output" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath workflowFixturePath
            let graph = Builder.build processingUnit
            let expectedStepIds =
                [
                    "MzMLToMzlite"
                    "PeptideDB"
                    "PeptideSpectrumMatching"
                    "PSMStatistics"
                    "PSMBasedQuantification"
                    "ProteinInference"
                    "QuantBasedAlignment"
                    "AlignmentBasedQuantification"
                    "AlignmentBasedQuantStatistics"
                    "AddDeducedPeptides"
                    "JoinQuantPepIonsWithProteins"
                    "LabeledProteinQuantification"
                ]
            expectedStepIds
            |> List.iter (fun stepId ->
                let stepNodeId = GraphId.stepNodeId "root" stepId
                let outputCount = WorkflowGraphQueries.getOutputPorts stepNodeId graph |> List.length
                if stepId = "PeptideDB" then
                    Expect.equal outputCount 2 "Expected PeptideDB to have two outputs"
                else
                    Expect.equal outputCount 1 $"Expected one output for step {stepId}"
            )
        })
    ]

let tests_runFixture =
    testList "run.cwl fixture" [
        testCaseCrossAsync "unresolved graph keeps external run reference and outer edge counts" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath runFixturePath
            let graph = Builder.build processingUnit
            Expect.equal (countEdges EdgeKind.Contains graph) 1 ""
            Expect.equal (countEdges EdgeKind.Calls graph) 1 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowInput graph) 24 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowOutput graph) 12 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 1 ""
            let externalReferences =
                graph.Nodes
                |> Seq.filter (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference)
                |> Seq.length
            Expect.equal externalReferences 1 ""
        })

        testCaseCrossAsync "resolved graph expands nested workflow structure" (crossAsync {
            let! processingUnit = loadProcessingUnitFromPath runFixturePath
            let! resolver = buildRunResolverFromFixtures ()
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "runs/tests/run.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some resolver)
            let graph = Builder.buildWith options processingUnit

            Expect.equal (countEdges EdgeKind.Contains graph) 13 ""
            Expect.equal (countEdges EdgeKind.Calls graph) 13 ""
            Expect.equal (countEdges EdgeKind.DataFlow graph) 27 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowInput graph) 54 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowOutput graph) 24 ""
            Expect.equal graph.EdgeCount 131 ""

            let nestedStepExists =
                graph.Nodes
                |> Seq.exists (fun n ->
                    n.Kind = NodeKind.StepNode
                    && n.Id = GraphId.stepNodeId "root/Workflow/run" "PeptideSpectrumMatching"
                )
            Expect.isTrue nestedStepExists ""
        })
    ]

let tests_expectedChartFixtures =
    testList "expected graph fixtures" [
        testCaseCrossAsync "regenerated markdown matches reviewed expected files" (crossAsync {
            let fixtureRoot = TestObjects.IO.testSimpleARCWithCWL
            let requestedCwlPaths = [
                "workflows/ProteomIQon/workflow.cwl"
                "runs/tests/run.cwl"
                "workflows/ProteomIQon/proteomiqon-peptidespectrummatching.cwl"
            ]
            let! tryResolveRunPath = buildFixtureResolver fixtureRoot

            let renderMarkdown (requestedPath: string) =
                crossAsync {
                    let fullCwlPath = toFixtureFullPath fixtureRoot requestedPath
                    let rootWorkflowPath = ArcPathHelper.normalizePathKey requestedPath
                    let! content = FileSystemHelper.readFileTextAsync fullCwlPath
                    let processingUnit = Decode.decodeCWLProcessingUnit content
                    let options =
                        WorkflowGraphBuildOptions.defaultOptions
                        |> WorkflowGraphBuildOptions.withRootScope (getFileNameWithoutExtension fullCwlPath)
                        |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some rootWorkflowPath)
                        |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some tryResolveRunPath)
                    return
                        processingUnit
                        |> Builder.buildWith options
                        |> WorkflowGraphSiren.toMarkdown
                }

            let expectedPathForRequested (requestedPath: string) =
                let fullCwlPath = toFixtureFullPath fixtureRoot requestedPath
                let directoryPath = getDirectoryPath fullCwlPath
                let fileStem = getFileNameWithoutExtension fullCwlPath
                ArcPathHelper.combine directoryPath $"{fileStem}_expected.graph.md"

            for requestedPath in requestedCwlPaths do
                let! actual = renderMarkdown requestedPath
                let expectedPath = expectedPathForRequested requestedPath
                let! expectedExists = FileSystemHelper.fileExistsAsync expectedPath
                Expect.isTrue expectedExists $"Expected graph fixture missing: {expectedPath}"
                let! expected = FileSystemHelper.readFileTextAsync expectedPath
                Expect.trimEqual actual expected $"Generated chart for '{requestedPath}' should match reviewed fixture '{expectedPath}'."
        })
    ]

let main =
    testList "Integration" [
        tests_workflowFixture
        tests_runFixture
        tests_expectedChartFixtures
    ]
