namespace ARCtrl.CWL

open ARCtrl.CWL
open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLWorkflowDescription(
    steps: ResizeArray<WorkflowStep>,
    inputs: ResizeArray<CWLInput>,
    outputs: ResizeArray<CWLOutput>,
    ?cwlVersion: string,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<Requirement>,
    ?metadata: DynamicObj
) =
    inherit DynamicObj()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _steps: ResizeArray<WorkflowStep> = steps
    let mutable _inputs: ResizeArray<CWLInput> = inputs
    let mutable _outputs: ResizeArray<CWLOutput> = outputs
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<Requirement> option = hints
    let mutable _metadata: DynamicObj option = metadata

    member this.CWLVersion
        with get() = _cwlVersion
        and set(version) = _cwlVersion <- version

    member this.Steps
        with get() = _steps
        and set(steps) = _steps <- steps

    member this.Inputs
        with get() = _inputs
        and set(inputs) = _inputs <- inputs

    member this.Outputs
        with get() = _outputs
        and set(outputs) = _outputs <- outputs

    member this.Requirements
        with get() = _requirements
        and set(requirements) = _requirements <- requirements

    member this.Hints
        with get() = _hints
        and set(hints) = _hints <- hints

    member this.Metadata
        with get() = _metadata
        and set(metadata) = _metadata <- metadata