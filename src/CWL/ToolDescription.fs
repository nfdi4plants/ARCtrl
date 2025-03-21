namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLToolDescription (
        outputs: ResizeArray<CWLOutput>,
        ?cwlVersion: string,
        ?baseCommand: ResizeArray<string>,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<Requirement>,
        ?inputs: ResizeArray<CWLInput>,
        ?metadata: DynamicObj
    ) =
    inherit DynamicObj ()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _outputs: ResizeArray<CWLOutput> = outputs
    let mutable _baseCommand: ResizeArray<string> option = baseCommand
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<Requirement> option = hints
    let mutable _inputs: ResizeArray<CWLInput> option = inputs
    let mutable _metadata: DynamicObj option = metadata

    member this.CWLVersion
        with get() = _cwlVersion
        and set(version) = _cwlVersion <- version

    member this.Outputs
        with get() = _outputs
        and set(outputs) = _outputs <- outputs

    member this.BaseCommand
        with get() = _baseCommand
        and set(baseCommand) = _baseCommand <- baseCommand

    member this.Requirements
        with get() = _requirements
        and set(requirements) = _requirements <- requirements

    member this.Hints
        with get() = _hints
        and set(hints) = _hints <- hints

    member this.Inputs
        with get() = _inputs
        and set(inputs) = _inputs <- inputs

    member this.Metadata
        with get() = _metadata
        and set(metadata) = _metadata <- metadata