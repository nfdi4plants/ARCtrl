namespace ARCtrl.WorkflowGraph

open ARCtrl
open DynamicObj

type WorkflowGraphNodeId = string
type WorkflowGraphEdgeId = string

[<RequireQualifiedAccess>]
type ProcessingUnitKind =
    | Workflow
    | CommandLineTool
    | ExpressionTool
    | Operation
    | ExternalReference
    | UnresolvedReference

[<RequireQualifiedAccess>]
type PortDirection =
    | Input
    | Output

[<RequireQualifiedAccess>]
type NodeKind =
    | ProcessingUnitNode of ProcessingUnitKind
    | StepNode
    | PortNode of PortDirection

[<RequireQualifiedAccess>]
type EdgeKind =
    | Contains
    | Calls
    | BindsWorkflowInput
    | DataFlow
    | BindsWorkflowOutput

[<RequireQualifiedAccess>]
type GraphIssueKind =
    | MissingReference
    | InvalidReference
    | ResolutionFailed
    | CycleDetected
    | MissingCwlDescription
    | UnexpectedRuntimeType

type GraphBuildIssue = {
    Kind: GraphIssueKind
    Message: string
    Scope: string option
    Reference: string option
}
with
    static member create (kind: GraphIssueKind, message: string, ?scope: string, ?reference: string) =
        {
            Kind = kind
            Message = message
            Scope = scope
            Reference = reference
        }

type WorkflowGraphNode = {
    Id: WorkflowGraphNodeId
    Kind: NodeKind
    Label: string
    OwnerNodeId: WorkflowGraphNodeId option
    Reference: string option
    Metadata: DynamicObj option
}
with
    static member create
        (id: WorkflowGraphNodeId, kind: NodeKind, label: string, ?ownerNodeId: WorkflowGraphNodeId, ?reference: string, ?metadata: DynamicObj)
        =
        {
            Id = id
            Kind = kind
            Label = label
            OwnerNodeId = ownerNodeId
            Reference = reference
            Metadata = metadata
        }

type WorkflowGraphEdge = {
    Id: WorkflowGraphEdgeId
    SourceNodeId: WorkflowGraphNodeId
    TargetNodeId: WorkflowGraphNodeId
    Kind: EdgeKind
    Label: string option
}
with
    static member create
        (id: WorkflowGraphEdgeId, sourceNodeId: WorkflowGraphNodeId, targetNodeId: WorkflowGraphNodeId, kind: EdgeKind, ?label: string)
        =
        {
            Id = id
            SourceNodeId = sourceNodeId
            TargetNodeId = targetNodeId
            Kind = kind
            Label = label
        }

type WorkflowGraph = {
    RootProcessingUnitNodeId: WorkflowGraphNodeId
    Nodes: ResizeArray<WorkflowGraphNode>
    Edges: ResizeArray<WorkflowGraphEdge>
    Diagnostics: ResizeArray<GraphBuildIssue>
}
with
    static member create
        (rootProcessingUnitNodeId: WorkflowGraphNodeId, ?nodes: ResizeArray<WorkflowGraphNode>, ?edges: ResizeArray<WorkflowGraphEdge>, ?diagnostics: ResizeArray<GraphBuildIssue>)
        =
        {
            RootProcessingUnitNodeId = rootProcessingUnitNodeId
            Nodes = defaultArg nodes (ResizeArray())
            Edges = defaultArg edges (ResizeArray())
            Diagnostics = defaultArg diagnostics (ResizeArray())
        }

    member this.NodeCount = this.Nodes.Count
    member this.EdgeCount = this.Edges.Count

type WorkflowGraphIndexEntry = string * Result<WorkflowGraph, GraphBuildIssue>

type WorkflowGraphIndex = {
    WorkflowGraphs: ResizeArray<WorkflowGraphIndexEntry>
    RunGraphs: ResizeArray<WorkflowGraphIndexEntry>
}
with
    static member create
        (?workflowGraphs: ResizeArray<WorkflowGraphIndexEntry>, ?runGraphs: ResizeArray<WorkflowGraphIndexEntry>)
        =
        {
            WorkflowGraphs = defaultArg workflowGraphs (ResizeArray())
            RunGraphs = defaultArg runGraphs (ResizeArray())
        }

[<RequireQualifiedAccess>]
module EdgeKind =

    /// Returns a string key representation for an EdgeKind.
    let asKey kind =
        match kind with
        | EdgeKind.Contains -> "contains"
        | EdgeKind.Calls -> "calls"
        | EdgeKind.BindsWorkflowInput -> "binds_workflow_input"
        | EdgeKind.DataFlow -> "dataflow"
        | EdgeKind.BindsWorkflowOutput -> "binds_workflow_output"

[<RequireQualifiedAccess>]
module PortDirection =

    /// Returns "in" for Input and "out" for Output.
    let asKey direction =
        match direction with
        | PortDirection.Input -> "in"
        | PortDirection.Output -> "out"

[<RequireQualifiedAccess>]
module GraphId =

    /// Trims and normalizes a path segment for use in graph IDs.
    let normalizeSegment (value: string) =
        if System.String.IsNullOrWhiteSpace value then
            ""
        else
            value.Trim()
            |> ArcPathHelper.normalizePathKey

    /// Combines a parent scope and child into a normalized hierarchical path.
    let childScope (scope: string) (child: string) =
        let normalizedScope = normalizeSegment scope
        let normalizedChild = normalizeSegment child
        if System.String.IsNullOrWhiteSpace normalizedScope then
            normalizedChild
        elif System.String.IsNullOrWhiteSpace normalizedChild then
            normalizedScope
        else
            ArcPathHelper.combine normalizedScope normalizedChild
            |> ArcPathHelper.normalizePathKey

    /// Creates a processing unit node ID with the format "unit:{scope}".
    let unitNodeId (scope: string) : WorkflowGraphNodeId =
        $"unit:{normalizeSegment scope}"

    /// Creates a step node ID with the format "step:{scope}/{stepId}".
    let stepNodeId (scope: string) (stepId: string) : WorkflowGraphNodeId =
        let normalizedStepId = normalizeSegment stepId
        $"step:{normalizeSegment scope}/{normalizedStepId}"

    /// Creates a port node ID with the format "port:{ownerNodeId}/{in|out}/{portId}".
    let portNodeId (ownerNodeId: WorkflowGraphNodeId) (direction: PortDirection) (portId: string) : WorkflowGraphNodeId =
        let normalizedPortId = normalizeSegment portId
        $"port:{ownerNodeId}/{PortDirection.asKey direction}/{normalizedPortId}"

    /// Creates an edge ID with the format "edge:{kind}:{source}->{target}".
    let edgeId (kind: EdgeKind) (sourceNodeId: WorkflowGraphNodeId) (targetNodeId: WorkflowGraphNodeId) : WorkflowGraphEdgeId =
        $"edge:{EdgeKind.asKey kind}:{sourceNodeId}->{targetNodeId}"
