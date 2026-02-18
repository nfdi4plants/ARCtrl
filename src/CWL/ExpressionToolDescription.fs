namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLExpressionToolDescription (
        outputs: ResizeArray<CWLOutput>,
        expression: string,
        ?cwlVersion: string,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<HintEntry>,
        ?intent: ResizeArray<string>,
        ?inputs: ResizeArray<CWLInput>,
        ?metadata: DynamicObj,
        ?label: string,
        ?doc: string
    ) =
    inherit DynamicObj ()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _outputs: ResizeArray<CWLOutput> = outputs
    let mutable _expression: string = expression
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _intent: ResizeArray<string> option = intent
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

    member this.Expression
        with get() = _expression
        and set(expression) = _expression <- expression

    member this.Requirements
        with get() = _requirements
        and set(requirements) = _requirements <- requirements

    member this.Hints
        with get() = _hints
        and set(hints) = _hints <- hints

    member this.Intent
        with get() = _intent
        and set(intent) = _intent <- intent

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

    /// Returns the expression tool's inputs or an empty ResizeArray if None.
    static member getInputsOrEmpty (tool: CWLExpressionToolDescription) =
        tool.Inputs |> Option.defaultValue (ResizeArray())

    /// Returns expression tool outputs.
    static member getOutputs (tool: CWLExpressionToolDescription) =
        tool.Outputs

    /// Returns the expression tool's inputs, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateInputs (tool: CWLExpressionToolDescription) =
        match tool.Inputs with
        | Some inputs -> inputs
        | None ->
            let inputs = ResizeArray()
            tool.Inputs <- Some inputs
            inputs

    /// Returns the expression tool's requirements or an empty ResizeArray if None.
    static member getRequirementsOrEmpty (tool: CWLExpressionToolDescription) =
        tool.Requirements |> Option.defaultValue (ResizeArray())

    /// Returns the expression tool's hints or an empty ResizeArray if None.
    static member getHintsOrEmpty (tool: CWLExpressionToolDescription) =
        tool.Hints |> Option.defaultValue (ResizeArray())

    /// Returns the expression tool's intent or an empty ResizeArray if None.
    static member getIntentOrEmpty (tool: CWLExpressionToolDescription) =
        tool.Intent |> Option.defaultValue (ResizeArray())

    /// Returns the expression tool's hints, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateHints (tool: CWLExpressionToolDescription) =
        match tool.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            tool.Hints <- Some hints
            hints

    /// Returns the expression tool's intent, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateIntent (tool: CWLExpressionToolDescription) =
        match tool.Intent with
        | Some intent -> intent
        | None ->
            let intent = ResizeArray()
            tool.Intent <- Some intent
            intent
