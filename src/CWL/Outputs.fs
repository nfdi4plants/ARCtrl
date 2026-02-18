namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

type OutputBinding = {
    Glob: string option
    }

    with static member create(?glob) = {Glob = glob}

[<AttachMembers>]
type OutputSource =
    | Single of string
    | Multiple of ResizeArray<string>
    with
    member this.AsValues() =
        match this with
        | Single value -> ResizeArray [| value |]
        | Multiple values -> values

[<AttachMembers>]
type CWLOutput (
    name: string,
    ?type_: CWLType,
    ?outputBinding: OutputBinding,
    ?outputSource: OutputSource
) as this =
    inherit DynamicObj ()
    do
        DynObj.setOptionalProperty ("type") type_ this
        DynObj.setOptionalProperty ("outputBinding") outputBinding this
        DynObj.setOptionalProperty ("outputSource") outputSource this
    member this.Name = name
    member this.Type_
        with get() = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
        and set(value: CWLType option) =
            match value with
            | Some v -> DynObj.setProperty "type" v this
            | None -> DynObj.removeProperty "type" this
    member this.OutputBinding
        with get() = DynObj.tryGetTypedPropertyValue<OutputBinding> ("outputBinding") this
        and set(value: OutputBinding option) =
            match value with
            | Some v -> DynObj.setProperty "outputBinding" v this
            | None -> DynObj.removeProperty "outputBinding" this
    member this.OutputSource
        with get() = DynObj.tryGetTypedPropertyValue<OutputSource> ("outputSource") this
        and set(value: OutputSource option) =
            match value with
            | Some (Multiple values) when values.Count = 0 -> DynObj.removeProperty "outputSource" this
            | Some v -> DynObj.setProperty "outputSource" v this
            | None -> DynObj.removeProperty "outputSource" this

    member this.GetOutputSources() =
        match this.OutputSource with
        | Some outputSource -> outputSource.AsValues()
        | _ ->
            ResizeArray()
