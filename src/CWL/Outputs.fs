namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

type OutputBinding = {
    Glob: string option
    }

    with static member create(?glob) = {Glob = glob}

[<AttachMembers>]
type CWLOutput (
    name: string,
    ?type_: CWLType,
    ?outputBinding: OutputBinding,
    ?outputSource: string
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
        with get() = DynObj.tryGetTypedPropertyValue<string> ("outputSource") this
        and set(value: string option) =
            match value with
            | Some v -> DynObj.setProperty "outputSource" v this
            | None -> DynObj.removeProperty "outputSource" this
