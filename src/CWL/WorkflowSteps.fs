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
type StepInput = {
    Id: string
    Source: ResizeArray<string> option
    DefaultValue: YAMLElement option
    ValueFrom: string option
    LinkMerge: LinkMergeMethod option
    PickValue: PickValueMethod option
    Doc: string option
    LoadContents: bool option
    LoadListing: string option
    Label: string option
}

    with
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
        { Id = id
          Source = source
          DefaultValue = defaultValue
          ValueFrom = valueFrom
          LinkMerge = linkMerge
          PickValue = pickValue
          Doc = doc
          LoadContents = loadContents
          LoadListing = loadListing
          Label = label }

[<AttachMembers>]
type StepOutputParameter = {
    Id: string
}

    with
    static member create(id: string) =
        { Id = id }

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
    ?hints: ResizeArray<Requirement>
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
    let mutable _hints: ResizeArray<Requirement> option = hints

    new(
        id: string,
        in_: ResizeArray<StepInput>,
        out_: ResizeArray<StepOutput>,
        run: string,
        ?label: string,
        ?doc: string,
        ?scatter: ResizeArray<string>,
        ?scatterMethod: ScatterMethod,
        ?when_: string,
        ?requirements: ResizeArray<Requirement>,
        ?hints: ResizeArray<Requirement>
    ) =
        WorkflowStep(
            id,
            in_,
            out_,
            RunString run,
            ?label = label,
            ?doc = doc,
            ?scatter = scatter,
            ?scatterMethod = scatterMethod,
            ?when_ = when_,
            ?requirements = requirements,
            ?hints = hints
        )

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


