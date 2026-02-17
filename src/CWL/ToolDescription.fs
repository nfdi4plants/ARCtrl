namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLToolDescription (
        outputs: ResizeArray<CWLOutput>,
        ?cwlVersion: string,
        ?baseCommand: ResizeArray<string>,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<HintEntry>,
        ?inputs: ResizeArray<CWLInput>,
        ?metadata: DynamicObj,
        ?label: string,
        ?doc: string
    ) =
    inherit DynamicObj ()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _outputs: ResizeArray<CWLOutput> = outputs
    let mutable _baseCommand: ResizeArray<string> option = baseCommand
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _inputs: ResizeArray<CWLInput> option = inputs
    let mutable _metadata: DynamicObj option = metadata
    let mutable _label: string option = label
    let mutable _doc: string option = doc

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

    member this.Label
        with get() = _label
        and set(label) = _label <- label

    member this.Doc
        with get() = _doc
        and set(doc) = _doc <- doc

    /// Returns the tool's inputs or an empty ResizeArray if None.
    static member getInputsOrEmpty (tool: CWLToolDescription) =
        tool.Inputs |> Option.defaultValue (ResizeArray())

    /// Returns the tool's outputs.
    static member getOutputs (tool: CWLToolDescription) =
        tool.Outputs

    /// Returns the tool's inputs, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateInputs (tool: CWLToolDescription) =
        match tool.Inputs with
        | Some inputs -> inputs
        | None ->
            let inputs = ResizeArray()
            tool.Inputs <- Some inputs
            inputs

    /// Returns the tool's requirements or an empty ResizeArray if None.
    static member getRequirementsOrEmpty (tool: CWLToolDescription) =
        tool.Requirements |> Option.defaultValue (ResizeArray())

    /// Returns the tool's hints or an empty ResizeArray if None.
    static member getHintsOrEmpty (tool: CWLToolDescription) =
        tool.Hints |> Option.defaultValue (ResizeArray())

    /// Returns the tool's hints, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateHints (tool: CWLToolDescription) =
        match tool.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            tool.Hints <- Some hints
            hints
