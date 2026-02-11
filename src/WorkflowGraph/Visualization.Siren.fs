namespace ARCtrl.WorkflowGraph

open ARCtrl.CWL
open Siren

type WorkflowGraphVisualizationOptions = {
    Direction: Direction
    EnableStyling: bool
    RenderContainsLinks: bool
}

[<RequireQualifiedAccess>]
module WorkflowGraphVisualizationOptions =

    let defaultOptions =
        {
            Direction = Direction.TD
            EnableStyling = true
            RenderContainsLinks = false
        }

[<RequireQualifiedAccess>]
module WorkflowGraphSiren =

    let private mermaidSpecials = [| '/'; '\\'; '['; ']'; '{'; '}'; '('; ')'; '>'; '<'; '|' |]

    let sanitizeMermaidId (id: string) =
        if System.String.IsNullOrWhiteSpace id then
            "node"
        else
            let normalized =
                let chars = ResizeArray<char>()
                id.ToCharArray()
                |> Array.iter (fun c ->
                    if System.Char.IsLetterOrDigit c || c = '_' then
                        chars.Add c
                    elif c = '/' || c = '\\' then
                        chars.Add '_'
                        chars.Add '_'
                    else
                        chars.Add '_'
                )
                chars.ToArray() |> System.String
            if normalized.Length > 0 && System.Char.IsDigit normalized.[0] then
                "_" + normalized
            else
                normalized

    /// Wraps labels in Mermaid double quotes when they contain characters that
    /// can be interpreted as Mermaid shape/link syntax. Double quotes are escaped as #quot;.
    let private quoteMermaidLabel (label: string) =
        if System.String.IsNullOrWhiteSpace label then
            label
        elif label.IndexOfAny(mermaidSpecials) >= 0 || label.Contains("\"") then
            let escaped = label.Replace("\"", "#quot;")
            $"\"{escaped}\""
        else
            label

    let private nodeToElement (node: WorkflowGraphNode) =
        let nodeId = sanitizeMermaidId node.Id
        let label = quoteMermaidLabel node.Label
        match node.Kind with
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow ->
            flowchart.nodeSubroutine(nodeId, label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool ->
            flowchart.nodeHexagon(nodeId, label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.ExpressionTool ->
            flowchart.nodeRhombus(nodeId, label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference ->
            flowchart.node(nodeId, label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference ->
            flowchart.node(nodeId, label)
        | NodeKind.StepNode ->
            flowchart.node(nodeId, label)
        | NodeKind.PortNode PortDirection.Input ->
            flowchart.nodeRound(nodeId, label)
        | NodeKind.PortNode PortDirection.Output ->
            flowchart.nodeStadium(nodeId, label)

    let private tryGetStepRunLookup (graph: WorkflowGraph) =
        graph.Edges
        |> Seq.filter (fun e -> e.Kind = EdgeKind.Calls)
        |> Seq.map (fun e -> e.SourceNodeId, e.TargetNodeId)
        |> Map.ofSeq

    let private addStyles
        (elements: ResizeArray<FlowchartElement>)
        (processingUnitNodes: WorkflowGraphNode [])
        (rootInputNodes: WorkflowGraphNode [])
        (rootOutputNodes: WorkflowGraphNode [])
        =
        let nodeIdsByPredicate (predicate: WorkflowGraphNode -> bool) (nodes: WorkflowGraphNode []) =
            nodes
            |> Array.filter predicate
            |> Array.map (fun n -> sanitizeMermaidId n.Id)

        let workflowIds =
            nodeIdsByPredicate (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow) processingUnitNodes
        let toolIds =
            nodeIdsByPredicate (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool) processingUnitNodes
        let expressionIds =
            nodeIdsByPredicate (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExpressionTool) processingUnitNodes
        let unresolvedIds =
            nodeIdsByPredicate
                (fun n ->
                    n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference
                    || n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference
                )
                processingUnitNodes
        let inputIds = rootInputNodes |> Array.map (fun n -> sanitizeMermaidId n.Id)
        let outputIds = rootOutputNodes |> Array.map (fun n -> sanitizeMermaidId n.Id)

        elements.Add(flowchart.classDef("wg_workflow", [ "fill", "#dff4ff"; "stroke", "#246fa8"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_tool", [ "fill", "#e8f5e9"; "stroke", "#2e7d32"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_expression", [ "fill", "#fff3e0"; "stroke", "#e65100"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_unresolved", [ "fill", "#ffebee"; "stroke", "#c62828"; "stroke-width", "2px"; "stroke-dasharray", "5 5" ]))
        elements.Add(flowchart.classDef("wg_initial_input", [ "fill", "#e1f5fe"; "stroke", "#0288d1"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_final_output", [ "fill", "#f3e5f5"; "stroke", "#7b1fa2"; "stroke-width", "2px" ]))

        if workflowIds.Length > 0 then
            elements.Add(flowchart.``class``(workflowIds, "wg_workflow"))
        if toolIds.Length > 0 then
            elements.Add(flowchart.``class``(toolIds, "wg_tool"))
        if expressionIds.Length > 0 then
            elements.Add(flowchart.``class``(expressionIds, "wg_expression"))
        if unresolvedIds.Length > 0 then
            elements.Add(flowchart.``class``(unresolvedIds, "wg_unresolved"))
        if inputIds.Length > 0 then
            elements.Add(flowchart.``class``(inputIds, "wg_initial_input"))
        if outputIds.Length > 0 then
            elements.Add(flowchart.``class``(outputIds, "wg_final_output"))

    let fromGraphWithOptions (options: WorkflowGraphVisualizationOptions) (graph: WorkflowGraph) =
        let elements = ResizeArray<FlowchartElement>()
        let mutable edgeKeys = Set.empty<string>

        let addGroupedNodes (groupId: string) (groupLabel: string) (nodes: WorkflowGraphNode []) =
            if nodes.Length > 0 then
                let grouped = ResizeArray<FlowchartElement>()
                nodes
                |> Array.iter (fun node -> grouped.Add(nodeToElement node))
                elements.Add(flowchart.subgraphNamed(groupId, quoteMermaidLabel groupLabel, grouped))

        let addRenderedEdge (kind: EdgeKind) (sourceNodeId: WorkflowGraphNodeId) (targetNodeId: WorkflowGraphNodeId) (label: string option) =
            let labelKey = defaultArg label ""
            let key = $"{EdgeKind.asKey kind}::{sourceNodeId}::{targetNodeId}::{labelKey}"
            if edgeKeys.Contains key |> not then
                edgeKeys <- edgeKeys.Add key
                let src = sanitizeMermaidId sourceNodeId
                let tgt = sanitizeMermaidId targetNodeId
                let link =
                    match kind, label with
                    | EdgeKind.DataFlow, Some l when System.String.IsNullOrWhiteSpace l |> not ->
                        flowchart.linkThickArrow(src, tgt, quoteMermaidLabel l)
                    | EdgeKind.DataFlow, _ ->
                        flowchart.linkThickArrow(src, tgt)
                    | EdgeKind.Contains, Some l when System.String.IsNullOrWhiteSpace l |> not ->
                        flowchart.linkDottedArrow(src, tgt, quoteMermaidLabel l)
                    | EdgeKind.Contains, _ ->
                        flowchart.linkDottedArrow(src, tgt)
                    | _, Some l when System.String.IsNullOrWhiteSpace l |> not ->
                        flowchart.linkArrow(src, tgt, quoteMermaidLabel l)
                    | _ ->
                        flowchart.linkArrow(src, tgt)
                elements.Add(link)

        let hasCalls =
            graph.Edges
            |> Seq.exists (fun e -> e.Kind = EdgeKind.Calls)

        let processingUnitNodes =
            graph.Nodes
            |> Seq.filter (fun n -> match n.Kind with | NodeKind.ProcessingUnitNode _ -> true | _ -> false)
            |> Seq.filter (fun n -> if hasCalls then n.Id <> graph.RootProcessingUnitNodeId else true)
            |> Seq.sortBy (fun n -> n.Id)
            |> Seq.toArray

        let rootInputNodes =
            graph.Nodes
            |> Seq.filter (fun n ->
                n.Kind = NodeKind.PortNode PortDirection.Input
                && n.OwnerNodeId = Some graph.RootProcessingUnitNodeId
            )
            |> Seq.sortBy (fun n -> n.Id)
            |> Seq.toArray

        let rootOutputNodes =
            graph.Nodes
            |> Seq.filter (fun n ->
                n.Kind = NodeKind.PortNode PortDirection.Output
                && n.OwnerNodeId = Some graph.RootProcessingUnitNodeId
            )
            |> Seq.sortBy (fun n -> n.Id)
            |> Seq.toArray

        let processingUnitNodeIds =
            processingUnitNodes
            |> Array.map (fun n -> n.Id)
            |> Set.ofArray

        let rootInputNodeIds = rootInputNodes |> Array.map (fun n -> n.Id) |> Set.ofArray
        let rootOutputNodeIds = rootOutputNodes |> Array.map (fun n -> n.Id) |> Set.ofArray

        let stepNodeIds =
            graph.Nodes
            |> Seq.choose (fun n ->
                if n.Kind = NodeKind.StepNode then Some n.Id else None
            )
            |> Set.ofSeq

        let stepInputPorts =
            graph.Nodes
            |> Seq.filter (fun n ->
                n.Kind = NodeKind.PortNode PortDirection.Input
                && (n.OwnerNodeId |> Option.exists stepNodeIds.Contains)
            )
            |> Seq.map (fun n -> n.Id, n)
            |> Map.ofSeq

        let stepOutputPorts =
            graph.Nodes
            |> Seq.filter (fun n ->
                n.Kind = NodeKind.PortNode PortDirection.Output
                && (n.OwnerNodeId |> Option.exists stepNodeIds.Contains)
            )
            |> Seq.map (fun n -> n.Id, n)
            |> Map.ofSeq

        let stepToRun = tryGetStepRunLookup graph
        let containsStepEdges =
            graph.Edges
            |> Seq.filter (fun e -> e.Kind = EdgeKind.Contains)
            |> Seq.toArray
        let tryGetRunOfStep (stepNodeId: WorkflowGraphNodeId) =
            stepToRun
            |> Map.tryFind stepNodeId
            |> Option.filter processingUnitNodeIds.Contains

        addGroupedNodes "wg_initial_inputs" "Initial Inputs" rootInputNodes
        processingUnitNodes |> Array.iter (nodeToElement >> elements.Add)
        addGroupedNodes "wg_final_outputs" "Final Outputs" rootOutputNodes

        if hasCalls |> not && processingUnitNodeIds.Contains graph.RootProcessingUnitNodeId then
            for inputNode in rootInputNodes do
                addRenderedEdge EdgeKind.BindsWorkflowInput inputNode.Id graph.RootProcessingUnitNodeId (Some inputNode.Label)
            for outputNode in rootOutputNodes do
                addRenderedEdge EdgeKind.BindsWorkflowOutput graph.RootProcessingUnitNodeId outputNode.Id None

        // Re-add workflow-to-step connectivity in PU-centric view:
        // workflow --contains/calls--> step run processing unit.
        containsStepEdges
        |> Array.iter (fun e ->
            if processingUnitNodeIds.Contains e.SourceNodeId then
                match tryGetRunOfStep e.TargetNodeId with
                | Some runNodeId ->
                    addRenderedEdge EdgeKind.Contains e.SourceNodeId runNodeId None
                | None ->
                    ()
        )

        graph.Edges
        |> Seq.sortBy (fun e -> e.Id)
        |> Seq.iter (fun edge ->
            match edge.Kind with
            | EdgeKind.BindsWorkflowInput when rootInputNodeIds.Contains edge.SourceNodeId ->
                match stepInputPorts |> Map.tryFind edge.TargetNodeId with
                | Some targetPort ->
                    match targetPort.OwnerNodeId |> Option.bind tryGetRunOfStep with
                    | Some consumerRunId ->
                        addRenderedEdge EdgeKind.BindsWorkflowInput edge.SourceNodeId consumerRunId (Some targetPort.Label)
                    | None ->
                        ()
                | None ->
                    ()
            | EdgeKind.DataFlow ->
                match stepOutputPorts |> Map.tryFind edge.SourceNodeId, stepInputPorts |> Map.tryFind edge.TargetNodeId with
                | Some sourcePort, Some targetPort ->
                    match sourcePort.OwnerNodeId |> Option.bind tryGetRunOfStep, targetPort.OwnerNodeId |> Option.bind tryGetRunOfStep with
                    | Some producerRunId, Some consumerRunId ->
                        addRenderedEdge EdgeKind.DataFlow producerRunId consumerRunId (Some targetPort.Label)
                    | _ ->
                        ()
                | _ ->
                    ()
            | EdgeKind.BindsWorkflowOutput when rootOutputNodeIds.Contains edge.TargetNodeId ->
                match stepOutputPorts |> Map.tryFind edge.SourceNodeId with
                | Some sourcePort ->
                    match sourcePort.OwnerNodeId |> Option.bind tryGetRunOfStep with
                    | Some producerRunId ->
                        addRenderedEdge EdgeKind.BindsWorkflowOutput producerRunId edge.TargetNodeId None
                    | None ->
                        ()
                | None ->
                    if rootInputNodeIds.Contains edge.SourceNodeId then
                        addRenderedEdge EdgeKind.BindsWorkflowOutput edge.SourceNodeId edge.TargetNodeId None
            | _ ->
                ()
        )

        if options.EnableStyling then
            addStyles elements processingUnitNodes rootInputNodes rootOutputNodes

        siren.flowchart(options.Direction, elements)

    let fromGraph (graph: WorkflowGraph) =
        fromGraphWithOptions WorkflowGraphVisualizationOptions.defaultOptions graph

    let fromProcessingUnit (processingUnit: CWLProcessingUnit) =
        processingUnit
        |> Builder.build
        |> fromGraph

    let fromProcessingUnitResolved (buildOptions: WorkflowGraphBuildOptions) (processingUnit: CWLProcessingUnit) =
        processingUnit
        |> Builder.buildWith buildOptions
        |> fromGraph

    let toMermaidWithOptions (options: WorkflowGraphVisualizationOptions) (graph: WorkflowGraph) =
        graph
        |> fromGraphWithOptions options
        |> siren.write

    let toMermaid (graph: WorkflowGraph) =
        toMermaidWithOptions WorkflowGraphVisualizationOptions.defaultOptions graph

    let toMarkdownWithOptions (options: WorkflowGraphVisualizationOptions) (graph: WorkflowGraph) =
        let mermaid = toMermaidWithOptions options graph
        $"```mermaid\n{mermaid}\n```"

    let toMarkdown (graph: WorkflowGraph) =
        toMarkdownWithOptions WorkflowGraphVisualizationOptions.defaultOptions graph
