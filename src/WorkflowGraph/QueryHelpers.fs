namespace ARCtrl.WorkflowGraph

[<RequireQualifiedAccess>]
module WorkflowGraphQueries =

    let tryFindNode (nodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphNode option =
        graph.Nodes
        |> Seq.tryFind (fun node -> node.Id = nodeId)

    let getNodes (kind: NodeKind) (graph: WorkflowGraph) : WorkflowGraphNode list =
        graph.Nodes
        |> Seq.filter (fun node -> node.Kind = kind)
        |> List.ofSeq

    let getEdgesFrom (nodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphEdge list =
        graph.Edges
        |> Seq.filter (fun edge -> edge.SourceNodeId = nodeId)
        |> List.ofSeq

    let getEdgesTo (nodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphEdge list =
        graph.Edges
        |> Seq.filter (fun edge -> edge.TargetNodeId = nodeId)
        |> List.ofSeq

    let getEdgesByKind (kind: EdgeKind) (graph: WorkflowGraph) : WorkflowGraphEdge list =
        graph.Edges
        |> Seq.filter (fun edge -> edge.Kind = kind)
        |> List.ofSeq

    let getInputPorts (ownerNodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphNode list =
        graph.Nodes
        |> Seq.filter (fun node ->
            node.OwnerNodeId = Some ownerNodeId
            && node.Kind = NodeKind.PortNode PortDirection.Input
        )
        |> List.ofSeq

    let getOutputPorts (ownerNodeId: WorkflowGraphNodeId) (graph: WorkflowGraph) : WorkflowGraphNode list =
        graph.Nodes
        |> Seq.filter (fun node ->
            node.OwnerNodeId = Some ownerNodeId
            && node.Kind = NodeKind.PortNode PortDirection.Output
        )
        |> List.ofSeq

    let nodeCount (graph: WorkflowGraph) = graph.NodeCount
    let edgeCount (graph: WorkflowGraph) = graph.EdgeCount
