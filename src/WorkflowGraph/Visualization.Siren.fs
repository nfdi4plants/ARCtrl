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
        let mermaidSpecials = [| '/'; '\\'; '['; ']'; '{'; '}'; '('; ')'; '>'; '<'; '|' |]
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
        flowchart.node(nodeId, label)

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
        let nodeIdsBy (nodes: WorkflowGraphNode []) =
            nodes
            |> Array.map (fun n -> sanitizeMermaidId n.Id)

        let processingIds = nodeIdsBy processingUnitNodes
        let inputIds = nodeIdsBy rootInputNodes
        let outputIds = nodeIdsBy rootOutputNodes
        let unresolvedIds =
            processingUnitNodes
            |> Array.filter (fun n ->
                n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference
                || n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference
            )
            |> nodeIdsBy

        elements.Add(flowchart.classDef("wg_processing", [ "fill", "#eaf2ff"; "stroke", "#1f4b8f"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_initial_input", [ "fill", "#e9f7df"; "stroke", "#2f7d32"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_final_output", [ "fill", "#fff3d6"; "stroke", "#b06f00"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_unresolved", [ "fill", "#ffe3e3"; "stroke", "#b3261e"; "stroke-width", "2px" ]))

        if processingIds.Length > 0 then
            elements.Add(flowchart.``class``(processingIds, "wg_processing"))
        if inputIds.Length > 0 then
            elements.Add(flowchart.``class``(inputIds, "wg_initial_input"))
        if outputIds.Length > 0 then
            elements.Add(flowchart.``class``(outputIds, "wg_final_output"))
        if unresolvedIds.Length > 0 then
            elements.Add(flowchart.``class``(unresolvedIds, "wg_unresolved"))

    let fromGraphWithOptions (options: WorkflowGraphVisualizationOptions) (graph: WorkflowGraph) =
        let elements = ResizeArray<FlowchartElement>()
        let edgeKeys = ResizeArray<string>()

        let addGroupedNodes (groupId: string) (groupLabel: string) (nodes: WorkflowGraphNode []) =
            if nodes.Length > 0 then
                let grouped = ResizeArray<FlowchartElement>()
                nodes
                |> Array.iter (fun node -> grouped.Add(nodeToElement node))
                elements.Add(flowchart.subgraphNamed(groupId, quoteMermaidLabel groupLabel, grouped))

        let addRenderedEdge (sourceNodeId: WorkflowGraphNodeId) (targetNodeId: WorkflowGraphNodeId) (label: string option) =
            let labelKey = defaultArg label ""
            let key = $"{sourceNodeId}::{targetNodeId}::{labelKey}"
            if edgeKeys.Contains key |> not then
                edgeKeys.Add key
                let src = sanitizeMermaidId sourceNodeId
                let tgt = sanitizeMermaidId targetNodeId
                let link =
                    match label with
                    | Some l when System.String.IsNullOrWhiteSpace l |> not -> flowchart.linkArrow(src, tgt, quoteMermaidLabel l)
                    | _ -> flowchart.linkArrow(src, tgt)
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

        let stepInputPorts =
            graph.Nodes
            |> Seq.filter (fun n ->
                n.Kind = NodeKind.PortNode PortDirection.Input
                && (n.OwnerNodeId |> Option.exists (fun ownerStepId ->
                    graph.Nodes |> Seq.exists (fun x -> x.Id = ownerStepId && x.Kind = NodeKind.StepNode)
                ))
            )
            |> Seq.map (fun n -> n.Id, n)
            |> Map.ofSeq

        let stepOutputPorts =
            graph.Nodes
            |> Seq.filter (fun n ->
                n.Kind = NodeKind.PortNode PortDirection.Output
                && (n.OwnerNodeId |> Option.exists (fun ownerStepId ->
                    graph.Nodes |> Seq.exists (fun x -> x.Id = ownerStepId && x.Kind = NodeKind.StepNode)
                ))
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
        addGroupedNodes "wg_final_outputs" "Workflow Outputs" rootOutputNodes

        if hasCalls |> not && processingUnitNodeIds.Contains graph.RootProcessingUnitNodeId then
            for inputNode in rootInputNodes do
                addRenderedEdge inputNode.Id graph.RootProcessingUnitNodeId (Some inputNode.Label)
            for outputNode in rootOutputNodes do
                addRenderedEdge graph.RootProcessingUnitNodeId outputNode.Id None

        // Re-add workflow-to-step connectivity in PU-centric view:
        // workflow --contains/calls--> step run processing unit.
        containsStepEdges
        |> Array.iter (fun e ->
            if processingUnitNodeIds.Contains e.SourceNodeId then
                match tryGetRunOfStep e.TargetNodeId with
                | Some runNodeId ->
                    addRenderedEdge e.SourceNodeId runNodeId None
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
                        addRenderedEdge edge.SourceNodeId consumerRunId (Some targetPort.Label)
                    | None ->
                        ()
                | None ->
                    ()
            | EdgeKind.DataFlow ->
                match stepOutputPorts |> Map.tryFind edge.SourceNodeId, stepInputPorts |> Map.tryFind edge.TargetNodeId with
                | Some sourcePort, Some targetPort ->
                    match sourcePort.OwnerNodeId |> Option.bind tryGetRunOfStep, targetPort.OwnerNodeId |> Option.bind tryGetRunOfStep with
                    | Some producerRunId, Some consumerRunId ->
                        addRenderedEdge producerRunId consumerRunId (Some targetPort.Label)
                    | _ ->
                        ()
                | _ ->
                    ()
            | EdgeKind.BindsWorkflowOutput when rootOutputNodeIds.Contains edge.TargetNodeId ->
                match stepOutputPorts |> Map.tryFind edge.SourceNodeId with
                | Some sourcePort ->
                    match sourcePort.OwnerNodeId |> Option.bind tryGetRunOfStep with
                    | Some producerRunId ->
                        addRenderedEdge producerRunId edge.TargetNodeId None
                    | None ->
                        ()
                | None ->
                    if rootInputNodeIds.Contains edge.SourceNodeId then
                        addRenderedEdge edge.SourceNodeId edge.TargetNodeId None
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
