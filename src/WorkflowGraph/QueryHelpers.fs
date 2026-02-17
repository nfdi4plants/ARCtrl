namespace ARCtrl.WorkflowGraph

/// Read-only query functions for inspecting a built WorkflowGraph.
[<RequireQualifiedAccess>]
module WorkflowGraphQueries =

    /// <summary>
    /// Finds a node by its ID in the graph. Returns None if not found.
    /// </summary>
    /// <param name="nodeId">The unique node identifier to search for.</param>
    /// <param name="graph">The workflow graph to search.</param>
    let tryFindNode (nodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphNode option =
        graph.Nodes
        |> Seq.tryFind (fun node -> node.Id = nodeId)

    /// <summary>
    /// Returns all nodes of a given NodeKind from the graph.
    /// </summary>
    /// <param name="kind">The kind of nodes to filter for (e.g., ProcessingUnitNode, StepNode, PortNode).</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getNodes (kind: NodeKind) (graph: WorkflowGraph) : WorkflowGraphNode list =
        graph.Nodes
        |> Seq.filter (fun node -> node.Kind = kind)
        |> List.ofSeq

    /// <summary>
    /// Returns all edges originating from a given node.
    /// </summary>
    /// <param name="nodeId">The source node identifier.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getEdgesFrom (nodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphEdge list =
        graph.Edges
        |> Seq.filter (fun edge -> edge.SourceNodeId = nodeId)
        |> List.ofSeq

    /// <summary>
    /// Returns all edges targeting a given node.
    /// </summary>
    /// <param name="nodeId">The target node identifier.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getEdgesTo (nodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphEdge list =
        graph.Edges
        |> Seq.filter (fun edge -> edge.TargetNodeId = nodeId)
        |> List.ofSeq

    /// <summary>
    /// Returns all edges of a given EdgeKind from the graph.
    /// </summary>
    /// <param name="kind">The kind of edges to filter for (e.g., DataFlow, Contains, Calls).</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getEdgesByKind (kind: EdgeKind) (graph: WorkflowGraph) : WorkflowGraphEdge list =
        graph.Edges
        |> Seq.filter (fun edge -> edge.Kind = kind)
        |> List.ofSeq

    /// <summary>
    /// Returns all input port nodes owned by a given node.
    /// </summary>
    /// <param name="ownerNodeId">The owner node identifier whose input ports to retrieve.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getInputPorts (ownerNodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphNode list =
        graph.Nodes
        |> Seq.filter (fun node ->
            node.OwnerNodeId = Some ownerNodeId
            && node.Kind = NodeKind.PortNode PortDirection.Input
        )
        |> List.ofSeq

    /// <summary>
    /// Returns all output port nodes owned by a given node.
    /// </summary>
    /// <param name="ownerNodeId">The owner node identifier whose output ports to retrieve.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getOutputPorts (ownerNodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphNode list =
        graph.Nodes
        |> Seq.filter (fun node ->
            node.OwnerNodeId = Some ownerNodeId
            && node.Kind = NodeKind.PortNode PortDirection.Output
        )
        |> List.ofSeq

    /// Returns the number of nodes in the graph.
    let nodeCount (graph: WorkflowGraph) = graph.NodeCount

    /// Returns the number of edges in the graph.
    let edgeCount (graph: WorkflowGraph) = graph.EdgeCount
