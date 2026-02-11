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
                id.ToCharArray()
                |> Array.map (fun c ->
                    if System.Char.IsLetterOrDigit c || c = '_' then c else '_'
                )
                |> System.String
            if normalized.Length > 0 && System.Char.IsDigit normalized.[0] then
                "_" + normalized
            else
                normalized

    let private nodeToElement (node: WorkflowGraphNode) =
        let nodeId = sanitizeMermaidId node.Id
        match node.Kind with
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow ->
            flowchart.nodeSubroutine(nodeId, node.Label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool ->
            flowchart.nodeHexagon(nodeId, node.Label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.ExpressionTool ->
            flowchart.nodeRhombus(nodeId, node.Label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference ->
            flowchart.node(nodeId, node.Label)
        | NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference ->
            flowchart.node(nodeId, node.Label)
        | NodeKind.StepNode ->
            flowchart.node(nodeId, node.Label)
        | NodeKind.PortNode PortDirection.Input ->
            flowchart.nodeRound(nodeId, node.Label)
        | NodeKind.PortNode PortDirection.Output ->
            flowchart.nodeStadium(nodeId, node.Label)

    let private edgeToElement (options: WorkflowGraphVisualizationOptions) (edge: WorkflowGraphEdge) =
        let sourceId = sanitizeMermaidId edge.SourceNodeId
        let targetId = sanitizeMermaidId edge.TargetNodeId
        match edge.Kind with
        | EdgeKind.DataFlow ->
            Some (flowchart.linkThickArrow(sourceId, targetId))
        | EdgeKind.Calls ->
            Some (flowchart.linkArrow(sourceId, targetId, defaultArg edge.Label "calls"))
        | EdgeKind.BindsWorkflowInput
        | EdgeKind.BindsWorkflowOutput ->
            Some (flowchart.linkArrow(sourceId, targetId))
        | EdgeKind.Contains ->
            if options.RenderContainsLinks then
                Some (flowchart.linkArrow(sourceId, targetId, "contains"))
            else
                None

    let private sortedChildren (graph: WorkflowGraph) (ownerNodeId: WorkflowGraphNodeId) =
        graph.Nodes
        |> Seq.filter (fun n -> n.OwnerNodeId = Some ownerNodeId)
        |> Seq.sortBy (fun n -> n.Id)
        |> List.ofSeq

    let rec private nodeTreeToElement (graph: WorkflowGraph) (node: WorkflowGraphNode) =
        let children = sortedChildren graph node.Id
        if List.isEmpty children then
            nodeToElement node
        else
            let childElements = ResizeArray<FlowchartElement>()
            childElements.Add(nodeToElement node)
            children
            |> List.iter (fun child ->
                childElements.Add(nodeTreeToElement graph child)
            )
            let subgraphId = sanitizeMermaidId $"{node.Id}_subgraph"
            flowchart.subgraphNamed(subgraphId, node.Label, childElements)

    let private addStyles (elements: ResizeArray<FlowchartElement>) (graph: WorkflowGraph) =
        let idsBy predicate =
            graph.Nodes
            |> Seq.filter predicate
            |> Seq.map (fun n -> sanitizeMermaidId n.Id)
            |> Seq.toArray

        let workflowIds =
            idsBy (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.Workflow)
        let toolIds =
            idsBy (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool)
        let expressionIds =
            idsBy (fun n -> n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExpressionTool)
        let unresolvedIds =
            idsBy (fun n ->
                n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.UnresolvedReference
                || n.Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.ExternalReference
            )
        let stepIds = idsBy (fun n -> n.Kind = NodeKind.StepNode)
        let inPortIds = idsBy (fun n -> n.Kind = NodeKind.PortNode PortDirection.Input)
        let outPortIds = idsBy (fun n -> n.Kind = NodeKind.PortNode PortDirection.Output)

        elements.Add(flowchart.classDef("wg_workflow", [ "fill", "#dff4ff"; "stroke", "#246fa8"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_tool", [ "fill", "#e9f7df"; "stroke", "#2f7d32"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_expression", [ "fill", "#fff3d6"; "stroke", "#b06f00"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_unresolved", [ "fill", "#ffe3e3"; "stroke", "#b3261e"; "stroke-width", "2px" ]))
        elements.Add(flowchart.classDef("wg_step", [ "fill", "#f0ecff"; "stroke", "#5b4aa3"; "stroke-width", "1px" ]))
        elements.Add(flowchart.classDef("wg_port_in", [ "fill", "#f5f9ff"; "stroke", "#6d7f95"; "stroke-width", "1px" ]))
        elements.Add(flowchart.classDef("wg_port_out", [ "fill", "#f4fff4"; "stroke", "#4b8352"; "stroke-width", "1px" ]))

        if workflowIds.Length > 0 then
            elements.Add(flowchart.``class``(workflowIds, "wg_workflow"))
        if toolIds.Length > 0 then
            elements.Add(flowchart.``class``(toolIds, "wg_tool"))
        if expressionIds.Length > 0 then
            elements.Add(flowchart.``class``(expressionIds, "wg_expression"))
        if unresolvedIds.Length > 0 then
            elements.Add(flowchart.``class``(unresolvedIds, "wg_unresolved"))
        if stepIds.Length > 0 then
            elements.Add(flowchart.``class``(stepIds, "wg_step"))
        if inPortIds.Length > 0 then
            elements.Add(flowchart.``class``(inPortIds, "wg_port_in"))
        if outPortIds.Length > 0 then
            elements.Add(flowchart.``class``(outPortIds, "wg_port_out"))

    let fromGraphWithOptions (options: WorkflowGraphVisualizationOptions) (graph: WorkflowGraph) =
        let elements = ResizeArray<FlowchartElement>()

        let roots =
            graph.Nodes
            |> Seq.filter (fun n -> n.OwnerNodeId.IsNone)
            |> Seq.sortBy (fun n -> n.Id)
            |> Seq.toArray

        roots
        |> Array.iter (fun root ->
            elements.Add(nodeTreeToElement graph root)
        )

        graph.Edges
        |> Seq.sortBy (fun e -> e.Id)
        |> Seq.choose (edgeToElement options)
        |> Seq.iter elements.Add

        if options.EnableStyling then
            addStyles elements graph

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

    let toSvgWith (renderMermaidToSvg: string -> string option) (graph: WorkflowGraph) =
        graph |> toMermaid |> renderMermaidToSvg

    let toPngBytesWith (renderMermaidToPng: string -> byte[] option) (graph: WorkflowGraph) =
        graph |> toMermaid |> renderMermaidToPng

#if !FABLE_COMPILER
    open System.IO

    let saveMermaid (filePath: string) (graph: WorkflowGraph) =
        File.WriteAllText(filePath, toMermaid graph)

    let saveMarkdown (filePath: string) (graph: WorkflowGraph) =
        File.WriteAllText(filePath, toMarkdown graph)

    let saveSvgWith (renderMermaidToSvg: string -> string option) (filePath: string) (graph: WorkflowGraph) =
        match toSvgWith renderMermaidToSvg graph with
        | Some svg ->
            File.WriteAllText(filePath, svg)
            Ok ()
        | None ->
            Error "Failed to render Mermaid to SVG."

    let savePngWith (renderMermaidToPng: string -> byte[] option) (filePath: string) (graph: WorkflowGraph) =
        match toPngBytesWith renderMermaidToPng graph with
        | Some png ->
            File.WriteAllBytes(filePath, png)
            Ok ()
        | None ->
            Error "Failed to render Mermaid to PNG."
#endif
