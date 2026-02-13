namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

type InputBinding = {
    Prefix: string option
    Position: int option
    ItemSeparator: string option
    Separate: bool option
    }

    with

    static member create
        (
            ?prefix: string,
            ?position: int,
            ?itemSeparator: string,
            ?separate: bool
        ) =
        {
            Prefix = prefix
            Position = position
            ItemSeparator = itemSeparator
            Separate = separate
        }


[<AttachMembers>]
type CWLInput (
    name: string,
    ?type_: CWLType,
    ?inputBinding: InputBinding,
    ?optional: bool
) as this =
    inherit DynamicObj ()
    do
        DynObj.setOptionalProperty ("type") type_ this
        DynObj.setOptionalProperty ("inputBinding") inputBinding this
        DynObj.setOptionalProperty ("optional") optional this
    member this.Name = name
    member this.Type_
        with get() = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
        and set(value: CWLType option) =
            match value with
            | Some v -> DynObj.setProperty "type" v this
            | None -> DynObj.removeProperty "type" this
    member this.InputBinding
        with get() = DynObj.tryGetTypedPropertyValue<InputBinding> ("inputBinding") this
        and set(value: InputBinding option) =
            match value with
            | Some v -> DynObj.setProperty "inputBinding" v this
            | None -> DynObj.removeProperty "inputBinding" this
    member this.Optional
        with get() = DynObj.tryGetTypedPropertyValue<bool> ("optional") this
        and set(value: bool option) =
            match value with
            | Some v -> DynObj.setProperty "optional" v this
            | None -> DynObj.removeProperty "optional" this
