namespace ARCtrl.CWL

open ARCtrl.CWL
open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLWorkflowDescription(
    steps: ResizeArray<ListEntry<WorkflowStep>>,
    inputs: ResizeArray<ListEntry<CWLInput>>,
    outputs: ResizeArray<ListEntry<CWLOutput>>,
    ?cwlVersion: string,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<HintEntry>,
    ?intent: ResizeArray<SchemaSaladString>,
    ?metadata: DynamicObj,
    ?label: SchemaSaladString,
    ?doc: DocText,
    ?id: string
) =
    inherit DynamicObj()

    let mutable _id: string option = id
    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _steps: ResizeArray<ListEntry<WorkflowStep>> = steps
    let mutable _inputs: ResizeArray<ListEntry<CWLInput>> = inputs
    let mutable _outputs: ResizeArray<ListEntry<CWLOutput>> = outputs
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _intent: ResizeArray<SchemaSaladString> option = intent
    let mutable _metadata: DynamicObj option = metadata
    let mutable _label: SchemaSaladString option = label
    let mutable _doc: DocText option = doc
    let mutable _sourceNodes = SourceNodes.Empty

    member this.Id
        with get() = _id
        and set(id) = _id <- id

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

    member this.Intent
        with get() = _intent
        and set(intent) = _intent <- intent

    member this.Metadata
        with get() = _metadata
        and set(metadata) = _metadata <- metadata

    member this.Label
        with get() = _label
        and set(label) = _label <- label

    member this.Doc
        with get() = _doc
        and set(doc) = _doc <- doc

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    static member KnownFieldNames =
        CWLKnownFieldNames.workflowDescription

    /// Returns a new list of the inputs of the workflow that are objects. The imports and the mixins are left out, so a
    /// change of the list changes nothing in the workflow. The entries of the workflow are in Inputs.
    static member getInputValues (workflow: CWLWorkflowDescription) : ResizeArray<CWLInput> =
        ListEntry.values workflow.Inputs

    /// Returns a new list of the outputs of the workflow that are objects. The imports and the mixins are left out, so a
    /// change of the list changes nothing in the workflow. The entries of the workflow are in Outputs.
    static member getOutputValues (workflow: CWLWorkflowDescription) : ResizeArray<CWLOutput> =
        ListEntry.values workflow.Outputs

    /// Returns a new list of the steps of the workflow that are objects. The imports and the mixins are left out, so a
    /// change of the list changes nothing in the workflow. The entries of the workflow are in Steps.
    static member getStepValues (workflow: CWLWorkflowDescription) : ResizeArray<WorkflowStep> =
        ListEntry.values workflow.Steps

    /// Returns workflow requirements or an empty ResizeArray if None.
    static member getRequirementsOrEmpty (workflow: CWLWorkflowDescription) =
        workflow.Requirements |> Option.defaultValue (ResizeArray())

    /// Returns workflow hints or an empty ResizeArray if None.
    static member getHintsOrEmpty (workflow: CWLWorkflowDescription) =
        workflow.Hints |> Option.defaultValue (ResizeArray())

    /// Returns workflow intent or an empty ResizeArray if None.
    static member getIntentOrEmpty (workflow: CWLWorkflowDescription) =
        workflow.Intent |> Option.defaultValue (ResizeArray())

    /// Returns the workflow's hints, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateHints (workflow: CWLWorkflowDescription) =
        match workflow.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            workflow.Hints <- Some hints
            hints

    /// Returns the workflow's intent, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateIntent (workflow: CWLWorkflowDescription) =
        match workflow.Intent with
        | Some intent -> intent
        | None ->
            let intent = ResizeArray()
            workflow.Intent <- Some intent
            intent
