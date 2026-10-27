namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

/// How a step input merges the values of several sources. A directive stands for the name of the symbol, and the decoder
/// reads no file, so it stays as written. An $import and a $mixin take the data of a file, and an $include takes its
/// text, which is the name of the symbol.
[<AttachMembers>]
type LinkMergeMethod =
    | MergeNested
    | MergeFlattened
    | ImportedLinkMerge of string
    | IncludedLinkMerge of string
    /// A mapping with a $mixin directive, alone or beside other fields, kept as it was written.
    | MixedLinkMerge of RawYaml
    with
    /// The name of the symbol as a document spells it, and for a directive the text with the directive in front of the
    /// path. The text of a directive serves for display, because the encoder writes the directive as a mapping.
    member this.AsCwlString =
        match this with
        | MergeNested -> "merge_nested"
        | MergeFlattened -> "merge_flattened"
        | ImportedLinkMerge path -> (SchemaSaladString.Import path).AsDirectiveString
        | IncludedLinkMerge path -> (SchemaSaladString.Include path).AsDirectiveString
        | MixedLinkMerge raw -> (SchemaSaladString.Mixed raw).AsDirectiveString

    static member tryParse(value: string) =
        match value with
        | "merge_nested" -> Some MergeNested
        | "merge_flattened" -> Some MergeFlattened
        | _ -> None

/// Which of the values of several sources a step input or a workflow output picks. A directive stands for the name of
/// the symbol, as in LinkMergeMethod.
[<AttachMembers>]
type PickValueMethod =
    | FirstNonNull
    | TheOnlyNonNull
    | AllNonNull
    | ImportedPickValue of string
    | IncludedPickValue of string
    /// A mapping with a $mixin directive, alone or beside other fields, kept as it was written.
    | MixedPickValue of RawYaml
    with
    /// The name of the symbol as a document spells it, and for a directive the text with the directive in front of the
    /// path. The text of a directive serves for display, because the encoder writes the directive as a mapping.
    member this.AsCwlString =
        match this with
        | FirstNonNull -> "first_non_null"
        | TheOnlyNonNull -> "the_only_non_null"
        | AllNonNull -> "all_non_null"
        | ImportedPickValue path -> (SchemaSaladString.Import path).AsDirectiveString
        | IncludedPickValue path -> (SchemaSaladString.Include path).AsDirectiveString
        | MixedPickValue raw -> (SchemaSaladString.Mixed raw).AsDirectiveString

    static member tryParse(value: string) =
        match value with
        | "first_non_null" -> Some FirstNonNull
        | "the_only_non_null" -> Some TheOnlyNonNull
        | "all_non_null" -> Some AllNonNull
        | _ -> None

/// How a scattered step combines its inputs. A directive stands for the name of the symbol, as in LinkMergeMethod.
[<AttachMembers>]
type ScatterMethod =
    | DotProduct
    | NestedCrossProduct
    | FlatCrossProduct
    | ImportedScatterMethod of string
    | IncludedScatterMethod of string
    /// A mapping with a $mixin directive, alone or beside other fields, kept as it was written.
    | MixedScatterMethod of RawYaml
    with
    /// The name of the symbol as a document spells it, and for a directive the text with the directive in front of the
    /// path. The text of a directive serves for display, because the encoder writes the directive as a mapping.
    member this.AsCwlString =
        match this with
        | DotProduct -> "dotproduct"
        | NestedCrossProduct -> "nested_crossproduct"
        | FlatCrossProduct -> "flat_crossproduct"
        | ImportedScatterMethod path -> (SchemaSaladString.Import path).AsDirectiveString
        | IncludedScatterMethod path -> (SchemaSaladString.Include path).AsDirectiveString
        | MixedScatterMethod raw -> (SchemaSaladString.Mixed raw).AsDirectiveString

    static member tryParse(value: string) =
        match value with
        | "dotproduct" -> Some DotProduct
        | "nested_crossproduct" -> Some NestedCrossProduct
        | "flat_crossproduct" -> Some FlatCrossProduct
        | _ -> None

[<AttachMembers>]
type StepInput (
    id: string,
    ?source: OutputSource,
    ?defaultValue: RawYaml,
    ?valueFrom: SchemaSaladString,
    ?linkMerge: LinkMergeMethod,
    ?pickValue: PickValueMethod,
    ?doc: DocText,
    ?loadContents: SchemaSaladBool,
    ?loadListing: LoadListingEnum,
    ?label: SchemaSaladString
) =
    inherit DynamicObj ()

    let mutable _id = id
    let mutable _source = source
    let mutable _defaultValue = defaultValue
    let mutable _valueFrom = valueFrom
    let mutable _linkMerge = linkMerge
    let mutable _pickValue = pickValue
    let mutable _doc = doc
    let mutable _loadContents = loadContents
    let mutable _loadListing = loadListing
    let mutable _label = label
    let mutable _sourceNodes = SourceNodes.Empty

    member this.Id
        with get() = _id
        and set(value) = _id <- value

    member this.Source
        with get() = _source
        and set(value) = _source <- value

    member this.DefaultValue
        with get() = _defaultValue
        and set(value) = _defaultValue <- value

    member this.ValueFrom
        with get() = _valueFrom
        and set(value) = _valueFrom <- value

    member this.LinkMerge
        with get() = _linkMerge
        and set(value) = _linkMerge <- value

    member this.PickValue
        with get() = _pickValue
        and set(value) = _pickValue <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    member this.LoadContents
        with get() = _loadContents
        and set(value) = _loadContents <- value

    member this.LoadListing
        with get() = _loadListing
        and set(value) = _loadListing <- value

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    override this.Equals(o: obj) =
        match o with
        | :? StepInput as other ->
            this.Id = other.Id &&
            this.Source = other.Source &&
            this.DefaultValue = other.DefaultValue &&
            this.ValueFrom = other.ValueFrom &&
            this.LinkMerge = other.LinkMerge &&
            this.PickValue = other.PickValue &&
            this.Doc = other.Doc &&
            this.LoadContents = other.LoadContents &&
            this.LoadListing = other.LoadListing &&
            this.Label = other.Label &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        HashHelpers.hashFields [
            box this.Id
            box this.Source
            box this.DefaultValue
            box this.ValueFrom
            box this.LinkMerge
            box this.PickValue
            box this.Doc
            box this.LoadContents
            box this.LoadListing
            box this.Label
            box (DynamicObjHelpers.hashDynamicProperties this)
        ]

    static member create(
        id: string,
        ?source: OutputSource,
        ?defaultValue: RawYaml,
        ?valueFrom: SchemaSaladString,
        ?linkMerge: LinkMergeMethod,
        ?pickValue: PickValueMethod,
        ?doc: DocText,
        ?loadContents: SchemaSaladBool,
        ?loadListing: LoadListingEnum,
        ?label: SchemaSaladString
    ) =
        StepInput(
            id,
            ?source = source,
            ?defaultValue = defaultValue,
            ?valueFrom = valueFrom,
            ?linkMerge = linkMerge,
            ?pickValue = pickValue,
            ?doc = doc,
            ?loadContents = loadContents,
            ?loadListing = loadListing,
            ?label = label
        )

    static member KnownFieldNames =
        CWLKnownFieldNames.stepInput

    /// Updates the StepInput at the given index of the entries. An import and a mixin have no StepInput to update.
    static member updateAt (index: int) (f: StepInput -> StepInput) (inputs: ResizeArray<ListEntry<StepInput>>) =
        if index < 0 || index >= inputs.Count then
            invalidArg (nameof index) $"StepInput index {index} is out of range."
        match inputs.[index] with
        | ListEntry.Value input -> inputs.[index] <- ListEntry.Value (f input)
        | ListEntry.Imported _
        | ListEntry.Mixed _ -> invalidArg (nameof index) $"StepInput index {index} is an import or a mixin."

[<AttachMembers>]
type StepOutputParameter (id: string) =
    inherit DynamicObj ()

    let mutable _id = id

    member this.Id
        with get() = _id
        and set(value) = _id <- value

    override this.Equals(o: obj) =
        match o with
        | :? StepOutputParameter as other ->
            this.Id = other.Id &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Id, DynamicObjHelpers.hashDynamicProperties this)

    static member create(id: string) =
        StepOutputParameter(id)

    static member KnownFieldNames =
        CWLKnownFieldNames.stepOutputParameter

/// An entry of the out field of a step. An entry is the id of an output or an object with the id. A document can write
/// a directive in its place, and Schema Salad replaces the directive with the file. An $include yields the text of the
/// file, which is an id. An $import yields the data of the file, which is an output or a list of outputs. The decoder
/// reads no file, so a directive stays an entry of its own, in its place.
[<AttachMembers>]
type StepOutput =
    | StepOutputString of string
    | StepOutputRecord of StepOutputParameter
    /// The text of a file as the id of an output, as in out: [{$include: path}].
    | StepOutputInclude of string
    /// The data of a file as an output or a list of outputs, as in out: [{$import: path}].
    | StepOutputImport of string
    /// A mapping with a $mixin directive, kept as it was written. The file may supply the id.
    | StepOutputMixed of RawYaml

    /// The id of an output that the document writes out, and none for an entry that names a file, because the id is in
    /// the file.
    static member tryId (output: StepOutput) : string option =
        match output with
        | StepOutputString id -> Some id
        | StepOutputRecord record -> Some record.Id
        | StepOutputInclude _
        | StepOutputImport _
        | StepOutputMixed _ -> None

[<AttachMembers>]
type WorkflowStepRun =
    | RunString of string
    /// A process that the document imports, as run: {$import: path}. The path names the file like a RunString does, and
    /// the decoder reads no file.
    | RunImport of string
    /// A process that a file completes through $mixin, as run: {$mixin: path}. The mapping is kept as it was written, and the
    /// decoder reads no file.
    | RunMixed of RawYaml
    /// A process whose class is a directive, as in run: {class: {$import: path}, baseCommand: echo}, kept as it was written.
    /// The file decides which process it is, and the decoder reads no file.
    | RunRaw of RawYaml
    | RunCommandLineTool of obj
    | RunWorkflow of obj
    | RunExpressionTool of obj
    | RunOperation of obj

/// The process that the run reference of a step resolved to. The run holds the reference as it is written, and this
/// holds what it stands for, one of the four cases of a run that carry a process. The reader of an ARC sets it from the
/// other files of the ARC, and a step that was not read from an ARC has none.
///
/// The resolved process is no part of the value of the step, so every ResolvedRun is equal to every other one and hashes
/// to 0. DynamicObj compares and hashes all properties of an object, and the resolved process is a whole workflow with
/// resolved steps of its own.
[<CustomEquality; NoComparison>]
type ResolvedRun =
    { Run: WorkflowStepRun option }

    override this.Equals(o: obj) =
        match o with
        | :? ResolvedRun -> true
        | _ -> false

    override this.GetHashCode() = 0

    static member Empty : ResolvedRun = { Run = None }

[<AttachMembers>]
type WorkflowStep (
    id: string option,
    in_: ResizeArray<ListEntry<StepInput>>,
    out_: ResizeArray<StepOutput>,
    run: WorkflowStepRun,
    ?label: SchemaSaladString,
    ?doc: DocText,
    ?scatter: ResizeArray<SchemaSaladString>,
    ?scatterMethod: ScatterMethod,
    ?when_: SchemaSaladString,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<HintEntry>
) =
    inherit DynamicObj ()

    let mutable _id: string option = id
    let mutable _in: ResizeArray<ListEntry<StepInput>> = in_
    let mutable _out: ResizeArray<StepOutput> = out_
    let mutable _run: WorkflowStepRun = run
    let mutable _label: SchemaSaladString option = label
    let mutable _doc: DocText option = doc
    let mutable _scatter: ResizeArray<SchemaSaladString> option = scatter
    let mutable _scatterMethod: ScatterMethod option = scatterMethod
    let mutable _when: SchemaSaladString option = when_
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints
    let mutable _sourceNodes = SourceNodes.Empty
    let mutable _resolvedRun = ResolvedRun.Empty

    /// The id of the step. CWL makes the id of a step optional, so a step without an id key has none, and a step with
    /// id: "" has the empty text. The decoder and the model never make up an id.
    member this.Id
        with get() = _id
        and set(id) = _id <- id

    member this.In
        with get() = _in
        and set(in_) = _in <- in_

    member this.Out
        with get() = _out
        and set(out_) = _out <- out_

    /// The run as it is written: a reference to a file or to a process of the same packed document, or an inline
    /// process. Setting it drops the resolved process, which belonged to the previous reference.
    member this.Run
        with get() = _run
        and set(run) =
            _run <- run
            _resolvedRun <- ResolvedRun.Empty

    /// The process that a run reference resolved to. The encoder writes only Run, so resolving a reference never changes
    /// the text of the file.
    member this.ResolvedRun
        with get() = _resolvedRun
        and set(value) = _resolvedRun <- value

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

    member this.Scatter
        with get() = _scatter
        and set(scatter) = _scatter <- scatter

    member this.ScatterMethod
        with get() = _scatterMethod
        and set(scatterMethod) = _scatterMethod <- scatterMethod

    member this.When_
        with get() = _when
        and set(when_) = _when <- when_

    member this.Requirements
        with get() = _requirements
        and set(requirements) = _requirements <- requirements

    member this.Hints
        with get() = _hints
        and set(hints) = _hints <- hints

    /// A step without an id and a step with the empty id hash alike under the hash of the properties on Python, so the id
    /// is part of the equality and of the hash on its own.
    override this.Equals(o: obj) =
        match o with
        | :? WorkflowStep as other -> this.Id = other.Id && base.Equals other
        | _ -> false

    override this.GetHashCode() =
        let idHash =
            match this.Id with
            | Some id -> HashHelpers.mergeHashes 1 (hash id)
            | None -> 0
        HashHelpers.mergeHashes idHash (base.GetHashCode())

    static member fromRunPath(
        id: string,
        in_: ResizeArray<ListEntry<StepInput>>,
        out_: ResizeArray<StepOutput>,
        runPath: string,
        ?label: SchemaSaladString,
        ?doc: DocText,
        ?scatter: ResizeArray<SchemaSaladString>,
        ?scatterMethod: ScatterMethod,
        ?when_: SchemaSaladString,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<HintEntry>
    ) =
        WorkflowStep(
            Some id,
            in_,
            out_,
            RunString runPath,
            ?label = label,
            ?doc = doc,
            ?scatter = scatter,
            ?scatterMethod = scatterMethod,
            ?when_ = when_,
            ?requirements = requirements,
            ?hints = hints
        )

    static member KnownFieldNames =
        CWLKnownFieldNames.workflowStep

    /// The run that holds the process of the step: the process that the reference resolved to, or the run itself when
    /// it is an inline process or a reference that did not resolve.
    static member getResolvedRun (step: WorkflowStep) : WorkflowStepRun =
        defaultArg step.ResolvedRun.Run step.Run

    /// Returns a new list of the inputs of the step that are objects. The imports and the mixins are left out, so a change
    /// of the list changes nothing in the step. The entries of the step are in In.
    static member getInputValues (step: WorkflowStep) : ResizeArray<StepInput> =
        ListEntry.values step.In

    /// Returns a new list of the outputs of the step that the document writes out as an id or an object. The directives
    /// are left out, so a change of the list changes nothing in the step. The entries of the step are in Out.
    static member getOutputValues (step: WorkflowStep) : ResizeArray<StepOutput> =
        step.Out |> Seq.filter (fun output -> (StepOutput.tryId output).IsSome) |> ResizeArray

    /// Updates a workflow step input by the index of its entry. An import and a mixin have no input to update.
    static member updateInputAt (index: int) (f: StepInput -> StepInput) (step: WorkflowStep) =
        StepInput.updateAt index f step.In

    /// Adds a new StepInput to a workflow step.
    static member addInput (input: StepInput) (step: WorkflowStep) =
        step.In.Add (ListEntry.Value input)

    /// Removes all StepInputs matching the provided id. The imports and the mixins stay.
    /// Reassigns step.In to a new filtered ResizeArray.
    static member removeInputsById (id: string) (step: WorkflowStep) =
        step.In
        |> Seq.filter (fun entry -> ListEntry.tryValue entry |> Option.forall (fun i -> i.Id <> id))
        |> ResizeArray
        |> fun remaining -> step.In <- remaining

    /// Updates the first StepInput matching the provided id.
    static member updateInputById (id: string) (f: StepInput -> StepInput) (step: WorkflowStep) =
        step.In
        |> Seq.tryFindIndex (fun entry -> ListEntry.tryValue entry |> Option.exists (fun i -> i.Id = id))
        |> Option.iter (fun i -> StepInput.updateAt i f step.In)


