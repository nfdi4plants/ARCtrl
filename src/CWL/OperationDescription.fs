namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLOperationDescription(
    inputs: ResizeArray<CWLInput>,
    outputs: ResizeArray<CWLOutput>,
    ?cwlVersion: string,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<HintEntry>,
    ?intent: ResizeArray<string>,
    ?metadata: DynamicObj,
    ?label: string,
    ?doc: string
) =
    inherit DynamicObj()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _inputs: ResizeArray<CWLInput> = inputs
    let mutable _outputs: ResizeArray<CWLOutput> = outputs
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _intent: ResizeArray<string> option = intent
    let mutable _metadata: DynamicObj option = metadata
    let mutable _label: string option = label
    let mutable _doc: string option = doc

    member this.CWLVersion
        with get() = _cwlVersion
        and set(value) = _cwlVersion <- value

    member this.Inputs
        with get() = _inputs
        and set(value) = _inputs <- value

    member this.Outputs
        with get() = _outputs
        and set(value) = _outputs <- value

    member this.Requirements
        with get() = _requirements
        and set(value) = _requirements <- value

    member this.Hints
        with get() = _hints
        and set(value) = _hints <- value

    member this.Intent
        with get() = _intent
        and set(value) = _intent <- value

    member this.Metadata
        with get() = _metadata
        and set(value) = _metadata <- value

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    static member getInputs (operation: CWLOperationDescription) =
        operation.Inputs

    static member getOutputs (operation: CWLOperationDescription) =
        operation.Outputs

    static member getRequirementsOrEmpty (operation: CWLOperationDescription) =
        operation.Requirements |> Option.defaultValue (ResizeArray())

    static member getHintsOrEmpty (operation: CWLOperationDescription) =
        operation.Hints |> Option.defaultValue (ResizeArray())

    static member getIntentOrEmpty (operation: CWLOperationDescription) =
        operation.Intent |> Option.defaultValue (ResizeArray())

    static member getOrCreateHints (operation: CWLOperationDescription) =
        match operation.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            operation.Hints <- Some hints
            hints

    static member getOrCreateIntent (operation: CWLOperationDescription) =
        match operation.Intent with
        | Some intent -> intent
        | None ->
            let intent = ResizeArray()
            operation.Intent <- Some intent
            intent
