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
    ?hints: ResizeArray<HintEntry>,
    ?metadata: DynamicObj,
    ?label: string,
    ?doc: string
) =
    inherit DynamicObj()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _steps: ResizeArray<WorkflowStep> = steps
    let mutable _inputs: ResizeArray<CWLInput> = inputs
    let mutable _outputs: ResizeArray<CWLOutput> = outputs
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _metadata: DynamicObj option = metadata
    let mutable _label: string option = label
    let mutable _doc: string option = doc

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

    member this.Label
        with get() = _label
        and set(label) = _label <- label

    member this.Doc
        with get() = _doc
        and set(doc) = _doc <- doc

    /// Returns workflow inputs.
    static member getInputs (workflow: CWLWorkflowDescription) =
        workflow.Inputs

    /// Returns workflow outputs.
    static member getOutputs (workflow: CWLWorkflowDescription) =
        workflow.Outputs

    /// Returns workflow requirements or an empty ResizeArray if None.
    static member getRequirementsOrEmpty (workflow: CWLWorkflowDescription) =
        workflow.Requirements |> Option.defaultValue (ResizeArray())

    /// Returns workflow hints or an empty ResizeArray if None.
    static member getHintsOrEmpty (workflow: CWLWorkflowDescription) =
        workflow.Hints |> Option.defaultValue (ResizeArray())

    /// Returns the workflow's hints, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateHints (workflow: CWLWorkflowDescription) =
        match workflow.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            workflow.Hints <- Some hints
            hints
