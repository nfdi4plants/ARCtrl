namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type StepInput = {
    Id: string
    Source: string option
    DefaultValue: string option
    ValueFrom: string option
}

    with
    static member create(id: string, ?source: string, ?defaultValue: string, ?valueFrom: string) =
        { Id = id
          Source = source
          DefaultValue = defaultValue
          ValueFrom = valueFrom }

[<AttachMembers>]
type StepOutput = {
    Id: ResizeArray<string>
}

    with
    static member create(id: ResizeArray<string>) =
        { Id = id }

[<AttachMembers>]
type WorkflowStep (
    id: string,
    in_: ResizeArray<StepInput>,
    out_: StepOutput,
    run: string,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<Requirement>
) =
    inherit DynamicObj ()

    let mutable _id: string = id
    let mutable _in: ResizeArray<StepInput> = in_
    let mutable _out: StepOutput = out_
    let mutable _run: string = run
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<Requirement> option = hints

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


