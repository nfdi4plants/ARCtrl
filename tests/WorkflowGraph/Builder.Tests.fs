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

        testCase "Operation root graph" <| fun () ->
            let processingUnit = decodeProcessingUnit TestObjects.CWL.Operation.minimalOperationFile
            let graph = Builder.build processingUnit
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Operation) graph) 1 ""
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

        testCase "an imported run resolved to commandlinetool node like a run string" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile.Replace("run: ./tool.cwl", "run: {$import: ./tool.cwl}")
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

        testCase "imports among the inputs, outputs and steps stand for nothing the graph can draw" <| fun () ->
            let yaml =
                String.concat "
"
                    [ "cwlVersion: v1.2"; "class: Workflow"
                      "inputs:"; "  - {$import: ports.yml}"; "  - id: a"; "    type: string"
                      "outputs:"; "  - id: o"; "    type: string"; "    outputSource: s/out"; "  - {$import: outputs.yml}"
                      "steps:"; "  - {$import: steps.yml}"; "  - id: s"; "    run: ./tool.cwl"; "    in: {}"; "    out: [out]"; "" ]
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 1 "Only the step that the document holds is a node."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.PortNode PortDirection.Input) graph) 1 "Only the input that the document holds is a port."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.PortNode PortDirection.Output) graph) 2 "Only the output of the workflow that the document holds and the output of its step are ports."

        testCase "imports among the step inputs and outputs get no port and the other entries do" <| fun () ->
            let yaml =
                String.concat "\n"
                    [ "cwlVersion: v1.2"; "class: Workflow"; "inputs:"; "  a: string"; "outputs: {}"
                      "steps:"; "  s:"; "    run: ./tool.cwl"
                      "    in:"; "      - {$import: more-in.yml}"; "      - id: x"; "        source: a"
                      "    out:"; "      - r"; "      - {$import: more-out.yml}"; "      - {$include: id.txt}"; "" ]
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            let stepNodeId = GraphId.stepNodeId "root" "s"
            Expect.isTrue (hasNode (GraphId.portNodeId stepNodeId PortDirection.Input "x") graph) "The step input x is a port."
            Expect.isTrue (hasNode (GraphId.portNodeId stepNodeId PortDirection.Output "r") graph) "The output r is a port."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.PortNode PortDirection.Input) graph) 2 "The workflow input and the step input are the ports of inputs."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.PortNode PortDirection.Output) graph) 1 "Only the output r is a port of outputs."
            Expect.equal (countEdges EdgeKind.BindsWorkflowInput graph) 1 "The step input x reads the workflow input."

        testCase "a run that a mixin completes is an external reference that the graph cannot follow" <| fun () ->
            let yaml =
                String.concat "\n"
                    [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"
                      "steps:"; "  s:"; "    run: {$mixin: tool.cwl}"; "    in: {}"; "    out: []"; "" ]
            let graph = yaml |> decodeProcessingUnit |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) graph) 1 "The mixin is one external reference."
            Expect.equal (countEdges EdgeKind.Calls graph) 1 "The step calls it."

        testCase "a run whose class is a directive is an external reference that the graph cannot follow" <| fun () ->
            let workflowWith (run: string) =
                String.concat "\n"
                    [ "cwlVersion: v1.2"; "class: Workflow"; "inputs: {}"; "outputs: {}"
                      "steps:"; "  s:"; "    run: " + run; "    in: {}"; "    out: []"; "" ]
            let reference (graph: WorkflowGraph) = graph.Nodes |> Seq.find (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference)
            let withId = workflowWith "{class: {$import: s.yml}, id: shell, baseCommand: echo, inputs: [], outputs: []}" |> decodeProcessingUnit |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) withId) 1 "The run is one external reference."
            Expect.equal (reference withId).Label "shell" "The label is the id of the process."
            Expect.equal (countEdges EdgeKind.Calls withId) 1 "The step calls it."
            let withoutId = workflowWith "{class: {$import: s.yml}, baseCommand: echo, inputs: [], outputs: []}" |> decodeProcessingUnit |> Builder.build
            Expect.equal (reference withoutId).Label "Process with a directive as its class" "A process without an id has a plain label."

        testCase "a process whose class is a directive is one external reference node without ports" <| fun () ->
            let withId = decodeProcessingUnit "cwlVersion: v1.2\nclass: {$import: s.yml}\nid: main\nbaseCommand: echo\ninputs: {a: string}\noutputs: []\n"
            let graph = Builder.build withId
            Expect.equal graph.Nodes.Count 1 "The graph shows the process as one node."
            Expect.equal graph.Nodes.[0].Kind (NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) "The graph cannot follow the file that decides the class."
            Expect.equal graph.Nodes.[0].Label "main" "The label is the id of the process."
            Expect.equal graph.Diagnostics.Count 0 "The node is no failure."
            let withoutId = decodeProcessingUnit "cwlVersion: v1.2\nclass: {$import: s.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            Expect.equal (Builder.build withoutId).Nodes.[0].Label "Process with a directive as its class" "A process without an id has a plain label."

        testCase "a run whose file holds a process of unknown class is an unresolved reference" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile
            let unknown = decodeProcessingUnit "cwlVersion: v1.2\nclass: {$import: s.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n"
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun path -> if path.EndsWith("tool.cwl") then Some unknown else None))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference) graph) 1 "A step has no run for such a process, so the reference stays unresolved."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) graph) 0 "The process is not drawn."
            Expect.isTrue (graph.Diagnostics |> Seq.exists (fun d -> d.Kind = GraphIssueKind.ResolutionFailed)) "The failure to resolve is reported."

        testCase "RunString resolved to operation node" <| fun () ->
            let yaml = TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile
            let resolvedOperation = Decode.decodeOperation TestObjects.CWL.Operation.minimalOperationFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun path ->
                    if path.EndsWith("tool.cwl") then Some (Operation resolvedOperation) else None
                ))
            let graph = yaml |> decodeProcessingUnit |> Builder.buildWith options
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Operation) graph) 1 ""
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

        testCase "a step with a resolved run builds the node of that process" <| fun () ->
            let workflow =
                match decodeProcessingUnit TestObjects.CWL.WorkflowGraph.singleStepToolRunWorkflowFile with
                | Workflow workflow -> workflow
                | other -> failwith $"Expected a workflow but got %A{other}"
            let tool =
                match decodeProcessingUnit TestObjects.CWL.CommandLineTool.cwlFile with
                | CommandLineTool tool -> tool
                | other -> failwith $"Expected a tool but got %A{other}"
            (ListEntry.values workflow.Steps).[0].ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromTool tool) }
            let graph = Builder.build (Workflow workflow)
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool) graph) 1 "The tool is a node."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference) graph) 0 "The reference is not left over."
            Expect.equal (countEdges EdgeKind.Calls graph) 1 "The step calls the tool."

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

        testCase "inline operation run creates Operation processing unit node" <| fun () ->
            let graph =
                TestObjects.CWL.Workflow.workflowWithInlineRunOperationFile
                |> decodeProcessingUnit
                |> Builder.build
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Operation) graph) 1 ""
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow) graph) 1 ""
            Expect.equal (countEdges EdgeKind.Calls graph) 1 ""
    ]

let tests_fullIds =
    let built = TestObjects.CWL.WorkflowGraph.fullIdsWorkflowFile |> decodeProcessingUnit |> Builder.build
    testList "Full ids" [
        testCase "the ids of the steps, the ports and the references are read against the id of the workflow" <| fun () ->
            Expect.isTrue (hasNode "step:root/first" built) "The step #main/first is first."
            Expect.isTrue (hasNode "step:root/second" built) "The step #main/second is second."
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) built) 2 ""
            Expect.equal (countEdges EdgeKind.BindsWorkflowInput built) 1 "The input #main/msg feeds #main/first/msg."
            Expect.equal (countEdges EdgeKind.DataFlow built) 1 "The output #main/first/out feeds #main/second/msg."
            Expect.equal (countEdges EdgeKind.BindsWorkflowOutput built) 1 "The output #main/result takes #main/second/out."
            Expect.equal built.Edges.Count 7 "Each step adds a Contains and a Calls edge to the three bindings."
            Expect.isEmpty (built.Diagnostics |> Seq.toList) "No reference is left unwired."
    ]

let tests_stepsWithoutAnId =
    let built = TestObjects.CWL.WorkflowGraph.stepsWithoutIdWorkflowFile |> decodeProcessingUnit |> Builder.build
    let stepNodes = built.Nodes |> Seq.filter (fun n -> n.Kind = NodeKind.StepNode) |> Seq.toList
    testList "Steps without an id" [
        testCase "every step is a node of its own, and a step without an id is named by its position" <| fun () ->
            Expect.equal (stepNodes |> List.map (fun n -> n.Id) |> List.sort) (List.sort [ "anonymous-step:root/0"; "step:root/stepB"; "anonymous-step:root/2" ]) "The nodes stand apart from each other."
            Expect.equal (stepNodes |> List.map (fun n -> n.Label) |> List.sort) (List.sort [ "steps[0]"; "stepB"; "steps[2]" ]) "The label of a step without an id is its position."
            Expect.equal (countEdges EdgeKind.Contains built) 3 "The workflow contains the three steps."
            Expect.equal (countEdges EdgeKind.Calls built) 3 "Each step calls its run."
        testCase "the ports and the sources of a step without an id are wired like those of the other steps" <| fun () ->
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.PortNode PortDirection.Input && n.OwnerNodeId.IsSome && (n.OwnerNodeId.Value.StartsWith "anonymous-step:" || n.OwnerNodeId.Value.StartsWith "step:")) built) 3 "Each step has its input port."
            Expect.isTrue (hasNode "port:anonymous-step:root/0/in/in1" built) "The first step without an id has its input port."
            Expect.isTrue (hasNode "port:anonymous-step:root/2/out/out" built) "The second step without an id has its output port."
            Expect.equal (countEdges EdgeKind.BindsWorkflowInput built) 3 "The workflow input feeds the input port of each step."
            Expect.isEmpty (built.Diagnostics |> Seq.toList) "Nothing is left unwired."
    ]

let tests_cycles =
    let workflowOf (text: string) =
        match decodeProcessingUnit text with
        | Workflow workflow -> workflow
        | other -> failwith $"Expected a workflow but got %A{other}"
    let text = "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: other.cwl\n    in: {}\n    out: []\n"
    let cycles (graph: WorkflowGraph) =
        graph.Diagnostics |> Seq.filter (fun d -> d.Kind = GraphIssueKind.CycleDetected) |> Seq.length
    testList "Cycles" [
        testCase "a workflow whose step holds the workflow inline builds once and reports the cycle" <| fun () ->
            let workflow = workflowOf text
            (ListEntry.values workflow.Steps).[0].Run <- WorkflowStepRunOps.fromWorkflow workflow
            let graph = Builder.build (Workflow workflow)
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 1 "The steps are built once."
            Expect.equal (cycles graph) 1 "The cycle is a diagnostic."
        testCase "two workflows whose resolved runs lead to each other build once each and report the cycle" <| fun () ->
            let first = workflowOf text
            let second = workflowOf text
            (ListEntry.values first.Steps).[0].ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromWorkflow second) }
            (ListEntry.values second.Steps).[0].ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromWorkflow first) }
            let graph = Builder.build (Workflow first)
            Expect.equal (countNodes (fun n -> n.Kind = NodeKind.StepNode) graph) 2 "The steps of each workflow are built once."
            Expect.equal (cycles graph) 1 "The cycle is a diagnostic."
        testCase "a workflow that two steps run is no cycle" <| fun () ->
            let shared = workflowOf text
            let holder = workflowOf (text.Replace("steps:\n", "steps:\n  t:\n    run: other.cwl\n    in: {}\n    out: []\n"))
            for step in ListEntry.values holder.Steps do
                step.ResolvedRun <- { Run = Some (WorkflowStepRunOps.fromWorkflow shared) }
            let graph = Builder.build (Workflow holder)
            Expect.equal (cycles graph) 0 "A workflow that runs twice is no cycle."
    ]

let main =
    testList "Builder" [
        tests_builderCore
        tests_fullIds
        tests_stepsWithoutAnId
        tests_cycles
    ]
