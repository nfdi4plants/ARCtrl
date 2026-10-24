namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLToolDescription (
        outputs: ResizeArray<ListEntry<CWLOutput>>,
        ?cwlVersion: string,
        ?baseCommand: ResizeArray<SchemaSaladString>,
        ?arguments: RawYaml,
        ?stdin: SchemaSaladString,
        ?stderr: SchemaSaladString,
        ?stdout: SchemaSaladString,
        ?successCodes: ResizeArray<int>,
        ?temporaryFailCodes: ResizeArray<int>,
        ?permanentFailCodes: ResizeArray<int>,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<HintEntry>,
        ?intent: ResizeArray<string>,
        ?inputs: ResizeArray<ListEntry<CWLInput>>,
        ?metadata: DynamicObj,
        ?label: SchemaSaladString,
        ?doc: DocText,
        ?id: string
    ) =
    inherit DynamicObj ()

    let mutable _id: string option = id
    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _outputs: ResizeArray<ListEntry<CWLOutput>> = outputs
    let mutable _baseCommand: ResizeArray<SchemaSaladString> option = baseCommand
    let mutable _arguments: RawYaml option = arguments
    let mutable _stdin: SchemaSaladString option = stdin
    let mutable _stderr: SchemaSaladString option = stderr
    let mutable _stdout: SchemaSaladString option = stdout
    let mutable _successCodes: ResizeArray<int> option = successCodes
    let mutable _temporaryFailCodes: ResizeArray<int> option = temporaryFailCodes
    let mutable _permanentFailCodes: ResizeArray<int> option = permanentFailCodes
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _intent: ResizeArray<string> option = intent
    let mutable _inputs: ResizeArray<ListEntry<CWLInput>> option = inputs
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

    member this.Outputs
        with get() = _outputs
        and set(outputs) = _outputs <- outputs

    member this.BaseCommand
        with get() = _baseCommand
        and set(baseCommand) = _baseCommand <- baseCommand

    member this.Arguments
        with get() = _arguments
        and set(value) = _arguments <- value

    member this.Stdin
        with get() = _stdin
        and set(value) = _stdin <- value

    member this.Stderr
        with get() = _stderr
        and set(value) = _stderr <- value

    member this.Stdout
        with get() = _stdout
        and set(value) = _stdout <- value

    member this.SuccessCodes
        with get() = _successCodes
        and set(value) = _successCodes <- value

    member this.TemporaryFailCodes
        with get() = _temporaryFailCodes
        and set(value) = _temporaryFailCodes <- value

    member this.PermanentFailCodes
        with get() = _permanentFailCodes
        and set(value) = _permanentFailCodes <- value

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

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    static member KnownFieldNames =
        CWLKnownFieldNames.toolDescription

    /// Returns the inputs of the tool, or an empty list. The imports are left out, and the list is a new one.
    static member getInputsOrEmpty (tool: CWLToolDescription) : ResizeArray<CWLInput> =
        tool.Inputs |> Option.map ListEntry.values |> Option.defaultValue (ResizeArray())

    /// Returns the outputs of the tool. The imports are left out, and the list is a new one.
    static member getOutputs (tool: CWLToolDescription) : ResizeArray<CWLOutput> =
        ListEntry.values tool.Outputs

    /// Returns the entries of the tool's inputs, creating and assigning a new empty ResizeArray if None.
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

    /// Returns the tool's intent or an empty ResizeArray if None.
    static member getIntentOrEmpty (tool: CWLToolDescription) =
        tool.Intent |> Option.defaultValue (ResizeArray())

    /// Returns the tool's hints, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateHints (tool: CWLToolDescription) =
        match tool.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            tool.Hints <- Some hints
            hints

    /// Returns the tool's intent, creating and assigning a new empty ResizeArray if None.
    static member getOrCreateIntent (tool: CWLToolDescription) =
        match tool.Intent with
        | Some intent -> intent
        | None ->
            let intent = ResizeArray()
            tool.Intent <- Some intent
            intent
