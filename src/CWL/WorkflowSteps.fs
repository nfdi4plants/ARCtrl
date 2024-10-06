namespace ARCtrl.CWL

open CWLTypes
open Outputs.Workflow
open Inputs.Workflow
open Requirements
open DynamicObj

module WorkflowSteps =

    type WorkflowStep (
        id: string,
        in_: StepInput [],
        out_: StepOutput,
        run: string,
        ?requirements: Requirement [],
        ?hints: Requirement []
    ) =
        inherit DynamicObj ()

        let mutable _id: string = id
        let mutable _in: StepInput [] = in_
        let mutable _out: StepOutput = out_
        let mutable _run: string = run
        let mutable _requirements: Requirement [] option = requirements
        let mutable _hints: Requirement [] option = hints

        member this.Id
            with get() = _id
            and set(id) = _id <- id

        member this.In
            with get() = _in
            and set(in_) = _in <- in_

        member this.Out
            with get() = _out
            and set(out_) = _out <- out_

        member this.Run
            with get() = _run
            and set(run) = _run <- run

        member this.Requirements
            with get() = _requirements
            and set(requirements) = _requirements <- requirements

        member this.Hints
            with get() = _hints
            and set(hints) = _hints <- hints
