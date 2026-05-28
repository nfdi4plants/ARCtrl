namespace ARCtrl.CWL

open DynamicObj
open Fable.Core
open YAMLicious.YAMLiciousTypes

[<AttachMembers>]
type LinkMergeMethod =
    | MergeNested
    | MergeFlattened
    with
    member this.AsCwlString =
        match this with
        | MergeNested -> "merge_nested"
        | MergeFlattened -> "merge_flattened"

    static member tryParse(value: string) =
        match value with
        | "merge_nested" -> Some MergeNested
        | "merge_flattened" -> Some MergeFlattened
        | _ -> None

[<AttachMembers>]
type PickValueMethod =
    | FirstNonNull
    | TheOnlyNonNull
    | AllNonNull
    with
    member this.AsCwlString =
        match this with
        | FirstNonNull -> "first_non_null"
        | TheOnlyNonNull -> "the_only_non_null"
        | AllNonNull -> "all_non_null"

    static member tryParse(value: string) =
        match value with
        | "first_non_null" -> Some FirstNonNull
        | "the_only_non_null" -> Some TheOnlyNonNull
        | "all_non_null" -> Some AllNonNull
        | _ -> None

[<AttachMembers>]
type ScatterMethod =
    | DotProduct
    | NestedCrossProduct
    | FlatCrossProduct
    with
    member this.AsCwlString =
        match this with
        | DotProduct -> "dotproduct"
        | NestedCrossProduct -> "nested_crossproduct"
        | FlatCrossProduct -> "flat_crossproduct"

    static member tryParse(value: string) =
        match value with
        | "dotproduct" -> Some DotProduct
        | "nested_crossproduct" -> Some NestedCrossProduct
        | "flat_crossproduct" -> Some FlatCrossProduct
        | _ -> None

[<AttachMembers>]
type StepInput (
    id: string,
    ?source: ResizeArray<string>,
    ?defaultValue: YAMLElement,
    ?valueFrom: string,
    ?linkMerge: LinkMergeMethod,
    ?pickValue: PickValueMethod,
    ?doc: string,
    ?loadContents: bool,
    ?loadListing: string,
    ?label: string
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
        hash (
            this.Id,
            this.Source,
            this.DefaultValue,
            this.ValueFrom,
            this.LinkMerge,
            this.PickValue,
            this.Doc,
            this.LoadContents,
            this.LoadListing,
            this.Label,
            DynamicObjHelpers.hashDynamicProperties this
        )

    static member create(
        id: string,
        ?source: ResizeArray<string>,
        ?defaultValue: YAMLElement,
        ?valueFrom: string,
        ?linkMerge: LinkMergeMethod,
        ?pickValue: PickValueMethod,
        ?doc: string,
        ?loadContents: bool,
        ?loadListing: string,
        ?label: string
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
        Set [|
            "id"
            "source"
            "default"
            "valueFrom"
            "linkMerge"
            "pickValue"
            "doc"
            "loadContents"
            "loadListing"
            "label"
        |]

    /// Updates a StepInput at the given index.
    static member updateAt (index: int) (f: StepInput -> StepInput) (inputs: ResizeArray<StepInput>) =
        if index < 0 || index >= inputs.Count then
            invalidArg (nameof index) $"StepInput index {index} is out of range."
        inputs.[index] <- f inputs.[index]

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
        Set [| "id" |]

[<AttachMembers>]
type StepOutput =
    | StepOutputString of string
    | StepOutputRecord of StepOutputParameter

[<AttachMembers>]
type WorkflowStepRun =
    | RunString of string
    | RunCommandLineTool of obj
    | RunWorkflow of obj
    | RunExpressionTool of obj
    | RunOperation of obj

[<AttachMembers>]
type WorkflowStep (
    id: string,
    in_: ResizeArray<StepInput>,
    out_: ResizeArray<StepOutput>,
    run: WorkflowStepRun,
    ?label: string,
    ?doc: string,
    ?scatter: ResizeArray<string>,
    ?scatterMethod: ScatterMethod,
    ?when_: string,
    ?requirements: ResizeArray<Requirement>,
    ?hints: ResizeArray<HintEntry>
) =
    inherit DynamicObj ()

    let mutable _id: string = id
    let mutable _in: ResizeArray<StepInput> = in_
    let mutable _out: ResizeArray<StepOutput> = out_
    let mutable _run: WorkflowStepRun = run
    let mutable _label: string option = label
    let mutable _doc: string option = doc
    let mutable _scatter: ResizeArray<string> option = scatter
    let mutable _scatterMethod: ScatterMethod option = scatterMethod
    let mutable _when: string option = when_
    let mutable _requirements: ResizeArray<Requirement> option = requirements
    let mutable _hints: ResizeArray<HintEntry> option = hints

    member this.Id
        with get() = _id
        and set(id) = _id <- id

    member this.In
        with get() = _in
        and set(in_) = _in <- in_

    member this.Out
        with get() = _out
        and set(out_) = _out <- out_

    member this.Run
        with get() = _run
        and set(run) = _run <- run

    member this.Label
        with get() = _label
        and set(label) = _label <- label

    member this.Doc
        with get() = _doc
        and set(doc) = _doc <- doc

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

    static member fromRunPath(
        id: string,
        in_: ResizeArray<StepInput>,
        out_: ResizeArray<StepOutput>,
        runPath: string,
        ?label: string,
        ?doc: string,
        ?scatter: ResizeArray<string>,
        ?scatterMethod: ScatterMethod,
        ?when_: string,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<HintEntry>
    ) =
        WorkflowStep(
            id,
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
        Set [|
            "id"
            "run"
            "in"
            "out"
            "requirements"
            "hints"
            "label"
            "doc"
            "scatter"
            "scatterMethod"
            "when"
        |]

    /// Updates a workflow step input by index.
    static member updateInputAt (index: int) (f: StepInput -> StepInput) (step: WorkflowStep) =
        StepInput.updateAt index f step.In

    /// Adds a new StepInput to a workflow step.
    static member addInput (input: StepInput) (step: WorkflowStep) =
        step.In.Add input

    /// Removes all StepInputs matching the provided id.
    /// Reassigns step.In to a new filtered ResizeArray.
    static member removeInputsById (id: string) (step: WorkflowStep) =
        step.In
        |> Seq.filter (fun i -> i.Id <> id)
        |> ResizeArray
        |> fun remaining -> step.In <- remaining

    /// Updates the first StepInput matching the provided id.
    static member updateInputById (id: string) (f: StepInput -> StepInput) (step: WorkflowStep) =
        step.In
        |> Seq.tryFindIndex (fun i -> i.Id = id)
        |> Option.iter (fun i -> StepInput.updateAt i f step.In)


