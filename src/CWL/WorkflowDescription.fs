namespace ARCtrl.CWL

open ARCtrl.CWL
open DynamicObj
open CWLTypes
open Requirements
open Inputs
open Outputs
open WorkflowSteps
open Fable.Core

[<AttachMembers>]
type CWLWorkflowDescription(
    steps: ResizeArray<WorkflowStep>,
    inputs: ResizeArray<Input>,
    outputs: ResizeArray<Output>,
    ?cwlVersion: string,
    ?cls: CWLClass,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<Requirement>,
    ?metadata: DynamicObj
) =
    inherit DynamicObj()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _class: CWLClass = cls |> Option.defaultValue CWLClass.Workflow
    let mutable _steps: ResizeArray<WorkflowStep> = steps
    let mutable _inputs: ResizeArray<Input> = inputs
    let mutable _outputs: ResizeArray<Output> = outputs
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<Requirement> option = hints
    let mutable _metadata: DynamicObj option = metadata

    member this.CWLVersion
        with get() = _cwlVersion
        and set(version) = _cwlVersion <- version

    member this.Class
        with get() = _class
        and set(cls) = _class <- cls

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