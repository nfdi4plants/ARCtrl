namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

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
type StepInput = {
    Id: string
    Source: ResizeArray<string> option
    DefaultValue: string option
    ValueFrom: string option
    LinkMerge: LinkMergeMethod option
    LoadContents: bool option
    LoadListing: string option
    Label: string option
}

    with
    static member create(
        id: string,
        ?source: ResizeArray<string>,
        ?defaultValue: string,
        ?valueFrom: string,
        ?linkMerge: LinkMergeMethod,
        ?loadContents: bool,
        ?loadListing: string,
        ?label: string
    ) =
        { Id = id
          Source = source
          DefaultValue = defaultValue
          ValueFrom = valueFrom
          LinkMerge = linkMerge
          LoadContents = loadContents
          LoadListing = loadListing
          Label = label }

[<AttachMembers>]
type StepOutputParameter = {
    Id: string option
}

    with
    static member create(?id: string) =
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

[<AttachMembers>]
type WorkflowStep (
    id: string,
    in_: ResizeArray<StepInput>,
    out_: ResizeArray<StepOutput>,
    run: WorkflowStepRun,
    ?label: string,
    ?doc: string,
    ?scatter: ResizeArray<string>,
    ?scatterMethod: string,
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
    let mutable _scatterMethod: string option = scatterMethod
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
        ?scatterMethod: string,
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

    member this.Requirements
        with get() = _requirements
        and set(requirements) = _requirements <- requirements

    member this.Hints
        with get() = _hints
        and set(hints) = _hints <- hints


