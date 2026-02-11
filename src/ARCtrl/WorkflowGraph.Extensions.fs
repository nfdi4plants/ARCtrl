module ARCtrl.WorkflowGraphExtensions

open ARCtrl.WorkflowGraph

type ARCtrl.ARC with

    member this.BuildWorkflowGraphs() =
        let graphIndex = Adapters.ofInvestigation this
        graphIndex.WorkflowGraphs

    member this.BuildRunGraphs() =
        let graphIndex = Adapters.ofInvestigation this
        graphIndex.RunGraphs

    member this.BuildAllProcessingUnitGraphs() =
        Adapters.ofInvestigation this
