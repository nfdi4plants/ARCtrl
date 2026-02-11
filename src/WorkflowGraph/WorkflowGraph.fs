namespace ARCtrl.WorkflowGraph

open ARCtrl
open ARCtrl.CWL

[<RequireQualifiedAccess>]
module WorkflowGraphApi =

    let build = Builder.build
    let buildWith = Builder.buildWith

    let ofWorkflow = Adapters.ofWorkflow
    let ofRun = Adapters.ofRun
    let ofInvestigation = Adapters.ofInvestigation

    let tryFindNode = WorkflowGraphQueries.tryFindNode
    let getNodes = WorkflowGraphQueries.getNodes
    let getEdgesFrom = WorkflowGraphQueries.getEdgesFrom
    let getEdgesTo = WorkflowGraphQueries.getEdgesTo
    let getEdgesByKind = WorkflowGraphQueries.getEdgesByKind
    let getInputPorts = WorkflowGraphQueries.getInputPorts
    let getOutputPorts = WorkflowGraphQueries.getOutputPorts

    let fromGraphToSiren = WorkflowGraphSiren.fromGraph
    let toMermaid = WorkflowGraphSiren.toMermaid
    let toMarkdown = WorkflowGraphSiren.toMarkdown
    let fromProcessingUnitToSiren = WorkflowGraphSiren.fromProcessingUnit
    let fromProcessingUnitToSirenResolved = WorkflowGraphSiren.fromProcessingUnitResolved

    let fromArcWorkflow = ofWorkflow
    let fromArcRun = ofRun
    let fromArcInvestigation = ofInvestigation
