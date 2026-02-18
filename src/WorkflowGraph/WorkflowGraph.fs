namespace ARCtrl.WorkflowGraph

open ARCtrl
open ARCtrl.CWL

/// <summary>
/// Public API façade for building, querying, and visualizing workflow graphs from CWL processing units and ARC domain objects.
/// </summary>
[<RequireQualifiedAccess>]
module WorkflowGraphApi =

    /// <summary>
    /// Builds a WorkflowGraph from a CWLProcessingUnit using default build options.
    /// </summary>
    /// <param name="processingUnit">The CWL processing unit (Workflow, CommandLineTool, or ExpressionTool) to build the graph from.</param>
    let build = Builder.build

    /// <summary>
    /// Builds a WorkflowGraph from a CWLProcessingUnit using the specified build options.
    /// </summary>
    /// <param name="options">Configuration controlling root scope, run resolution, nested workflow expansion, and strictness.</param>
    /// <param name="processingUnit">The CWL processing unit to build the graph from.</param>
    let buildWith = Builder.buildWith

    /// <summary>
    /// Converts an ArcWorkflow into a WorkflowGraph. Returns Error if no CWL description is available.
    /// </summary>
    /// <param name="workflow">The ArcWorkflow to convert.</param>
    let ofWorkflow = Adapters.ofWorkflow

    /// <summary>
    /// Converts an ArcRun into a WorkflowGraph. Returns Error if no CWL description is available.
    /// </summary>
    /// <param name="run">The ArcRun to convert.</param>
    let ofRun = Adapters.ofRun

    /// <summary>
    /// Builds workflow graphs for all workflows and runs in an ArcInvestigation, returning a WorkflowGraphIndex.
    /// </summary>
    /// <param name="investigation">The ArcInvestigation containing workflows and runs.</param>
    let ofInvestigation = Adapters.ofInvestigation

    /// <summary>
    /// Finds a node by its ID in the graph. Returns None if not found.
    /// </summary>
    /// <param name="nodeId">The unique node identifier to search for.</param>
    /// <param name="graph">The workflow graph to search.</param>
    let tryFindNode = WorkflowGraphQueries.tryFindNode

    /// <summary>
    /// Returns all nodes of a given NodeKind from the graph.
    /// </summary>
    /// <param name="kind">The kind of nodes to filter for.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getNodes = WorkflowGraphQueries.getNodes

    /// <summary>
    /// Returns all edges originating from a given node.
    /// </summary>
    /// <param name="nodeId">The source node identifier.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getEdgesFrom = WorkflowGraphQueries.getEdgesFrom

    /// <summary>
    /// Returns all edges targeting a given node.
    /// </summary>
    /// <param name="nodeId">The target node identifier.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getEdgesTo = WorkflowGraphQueries.getEdgesTo

    /// <summary>
    /// Returns all edges of a given EdgeKind from the graph.
    /// </summary>
    /// <param name="kind">The kind of edges to filter for.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getEdgesByKind = WorkflowGraphQueries.getEdgesByKind

    /// <summary>
    /// Returns all input port nodes owned by a given node.
    /// </summary>
    /// <param name="ownerNodeId">The owner node identifier.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getInputPorts = WorkflowGraphQueries.getInputPorts

    /// <summary>
    /// Returns all output port nodes owned by a given node.
    /// </summary>
    /// <param name="ownerNodeId">The owner node identifier.</param>
    /// <param name="graph">The workflow graph to query.</param>
    let getOutputPorts = WorkflowGraphQueries.getOutputPorts

    /// <summary>
    /// Transforms a WorkflowGraph into a Siren flowchart representation using default visualization options.
    /// </summary>
    /// <param name="graph">The workflow graph to visualize.</param>
    let fromGraphToSiren = WorkflowGraphSiren.fromGraph

    /// <summary>
    /// Converts a WorkflowGraph to a Mermaid text string using default visualization options.
    /// </summary>
    /// <param name="graph">The workflow graph to render.</param>
    let toMermaid = WorkflowGraphSiren.toMermaid

    /// <summary>
    /// Converts a WorkflowGraph to a Markdown-fenced Mermaid code block using default visualization options.
    /// </summary>
    /// <param name="graph">The workflow graph to render.</param>
    let toMarkdown = WorkflowGraphSiren.toMarkdown

    /// <summary>
    /// Builds a graph from a raw CWLProcessingUnit and converts it directly to a Siren flowchart.
    /// </summary>
    /// <param name="processingUnit">The CWL processing unit to visualize.</param>
    let fromProcessingUnitToSiren = WorkflowGraphSiren.fromProcessingUnit

    /// <summary>
    /// Builds a graph from a CWLProcessingUnit with custom build options and converts it to a Siren flowchart.
    /// </summary>
    /// <param name="buildOptions">The build options controlling graph construction.</param>
    /// <param name="processingUnit">The CWL processing unit to visualize.</param>
    let fromProcessingUnitToSirenResolved = WorkflowGraphSiren.fromProcessingUnitResolved

    /// <summary>
    /// Alias for ofWorkflow. Converts an ArcWorkflow into a WorkflowGraph.
    /// </summary>
    /// <param name="workflow">The ArcWorkflow to convert.</param>
    let fromArcWorkflow = ofWorkflow

    /// <summary>
    /// Alias for ofRun. Converts an ArcRun into a WorkflowGraph.
    /// </summary>
    /// <param name="run">The ArcRun to convert.</param>
    let fromArcRun = ofRun

    /// <summary>
    /// Alias for ofInvestigation. Builds workflow graphs for all workflows and runs in an ArcInvestigation.
    /// </summary>
    /// <param name="investigation">The ArcInvestigation to convert.</param>
    let fromArcInvestigation = ofInvestigation
