module Tests.Builder

open ARCtrl
open ARCtrl.CWL
open ARCtrl.WorkflowGraph
open TestingUtils

let private decodeProcessingUnit (text: string) =
    Decode.decodeCWLProcessingUnit text

let private countNodes (predicate: WorkflowGraphNode -> bool) (graph: WorkflowGraph) =
    graph.Nodes |> Seq.filter predicate |> Seq.length

let private countEdges (kind: EdgeKind) (graph: WorkflowGraph) =
    graph.Edges |> Seq.filter (fun e -> e.Kind = kind) |> Seq.length

let private hasNode (nodeId: string) (graph: WorkflowGraph) =
    graph.Nodes |> Seq.exists (fun n -> n.Id = nodeId)

let tests_builderCore =
    testList "Builder Core" [
        testCase "CommandLineTool root graph" <| fun () ->
            let processingUnit = decodeProcessingUnit TestObjects.CWL.CommandLineTool.cwlFile
            let graph = Builder.build processingUnit
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool) graph) 1 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 0 ""
            Expect.equal (countEdges EdgeKind.Contains graph) 0 ""
            Expect.equal (countEdges EdgeKind.DataFlow graph) 0 ""
            Expect.equal graph.Diagnostics.Count 0 ""

        testCase "ExpressionTool root graph" <| fun () ->
            let processingUnit = decodeProcessingUnit TestObjects.CWL.ExpressionTool.minimalExpressionToolFile
            let graph = Builder.build processingUnit
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExpressionTool) graph) 1 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 0 ""

        testCase "empty workflow has no step nodes" <| fun () ->
            let processingUnit = decodeProcessingUnit TestObjects.CWL.Workflow.workflowWithNoStepsFile
            let graph = Builder.build processingUnit
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow) graph) 1 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 0 ""
            Expect.equal (countEdges EdgeKind.DataFlow graph) 0 ""

        testCase "two-step workflow creates contains and dataflow edges" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.twoStepWorkflowFile
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 2 ""
            Expect.equal (countEdges EdgeKind.Contains graph) 2 ""
            Expect.equal (countEdges EdgeKind.DataFlow graph) 1 ""

        testCase "multiple source values create fan-in edges" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.fanInWorkflowFile
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            Expect.equal (countEdges EdgeKind.DataFlow graph) 2 ""

        testCase "RunString unresolved without lookup becomes external reference node" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) graph) 1 ""

        testCase "strict unresolved run references without lookup become unresolved reference nodes" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withStrictUnresolvedRunReferences true
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference) graph) 1 ""
            let resolutionFailures =
                graph.Diagnostics
                |> Seq.filter (fun d -> d.Kind = GraphIssueKind.ResolutionFailed)
                |> Seq.length
            Expect.isTrue (resolutionFailures >= 1) ""

        testCase "RunString unresolved with lookup becomes unresolved reference node" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepMissingRunWorkflowFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun _ -> None))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference) graph) 1 ""
            Expect.isTrue (graph.Diagnostics.Count > 0) ""

        testCase "RunString with cwl fragment keeps basename label for unresolved references" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepFragmentRunWorkflowFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun _ -> None))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            let unresolvedNode =
                graph.Nodes
                |> Seq.tryFind (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference)
                |> fun nodeOpt -> Expect.wantSome nodeOpt "Expected unresolved reference node"
            Expect.equal unresolvedNode.Label "my-tool" ""

        testCase "RunString with cwl query keeps basename label for unresolved references" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepQueryRunWorkflowFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun _ -> None))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            let unresolvedNode =
                graph.Nodes
                |> Seq.tryFind (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference)
                |> fun nodeOpt -> Expect.wantSome nodeOpt "Expected unresolved reference node"
            Expect.equal unresolvedNode.Label "my-tool" ""

        testCase "RunString resolved to commandlinetool node" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile
            let resolvedTool = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun path ->
                    if path.EndsWith("tool.cwl") then Some (CommandLineTool resolvedTool) else None
                ))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool) graph) 1 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) graph) 0 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference) graph) 0 ""

        testCase "shared run path across sibling steps resolves as cache-hit (no false cycle)" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.sharedRunPathWorkflowFile
            let resolvedTool = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
            let mutable resolveCalls = 0
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun path ->
                    if path.EndsWith("tool.cwl") then
                        resolveCalls <- resolveCalls + 1
                        Some (CommandLineTool resolvedTool)
                    else
                        None
                ))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            let cycleIssues =
                graph.Diagnostics
                |> Seq.filter (fun d -> d.Kind = GraphIssueKind.CycleDetected)
                |> Seq.length
            Expect.equal cycleIssues 0 "Sibling reuse of the same run path should not be treated as a cycle."
            Expect.equal resolveCalls 1 "Shared run path should be resolved once and reused via cache."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool) graph) 1 "Shared run path should yield one cached tool node."
            Expect.equal (countEdges EdgeKind.Calls graph) 2 "Both workflow steps should still call the resolved tool."

        testCase "StepOutput string and record produce output ports" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.stepOutputMixedWorkflowFile
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            let stepNodeId = GraphId.stepNodeId "root" "step1"
            let out1 = GraphId.portNodeId stepNodeId PortDirection.Output "out1"
            let out2 = GraphId.portNodeId stepNodeId PortDirection.Output "out2"
            Expect.isTrue (hasNode out1 graph) ""
            Expect.isTrue (hasNode out2 graph) ""

        testCase "cycle detection is diagnostic and graph build terminates" <| fun () ->
            let workflowA = decodeProcessingUnit TestObjects.CWL.RunWorkflowReference.CycleSafe.workflowACwlText
            let workflowB = decodeProcessingUnit TestObjects.CWL.RunWorkflowReference.CycleSafe.workflowBCwlText
            let runRoot = decodeProcessingUnit TestObjects.CWL.RunWorkflowReference.CycleSafe.runCwlText
            let resolver =
                fun (path: string) ->
                    match ArcPathHelper.normalizePathKey path with
                    | "a.cwl" -> Some workflowA
                    | "b.cwl" -> Some workflowB
                    | _ -> None
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "run.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some resolver)
            let graph = Builder.buildWith options runRoot
            let cycleIssues =
                graph.Diagnostics
                |> Seq.filter (fun d -> d.Kind = GraphIssueKind.CycleDetected)
                |> Seq.length
            Expect.isTrue (cycleIssues >= 1) ""
            Expect.isTrue (graph.NodeCount > 0) ""

        testCase "inline commandlinetool run builds called node" <| fun () ->
            let graph =
                TestObjects.CWL.Workflow.workflowWithInlineRunCommandLineToolFile
                |> decodeProcessingUnit
                |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool) graph) 1 ""
            Expect.equal (countEdges EdgeKind.Calls graph) 1 ""

        testCase "inline workflow run expands nested step graph" <| fun () ->
            let graph =
                TestObjects.CWL.Workflow.workflowWithInlineRunWorkflowFile
                |> decodeProcessingUnit
                |> Builder.build
            let innerStepExists =
                graph.Nodes
                |> Seq.exists (fun n -> n.Kind = NodeKind.StepNode && n.Label = "inner")
            Expect.isTrue innerStepExists ""
            Expect.isTrue ((countEdges EdgeKind.Contains graph) >= 2) ""
    ]

let main = 
    testList "Builder" [
        tests_builderCore
    ]
