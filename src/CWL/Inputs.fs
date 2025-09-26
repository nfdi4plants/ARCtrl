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
    member this.Type_ = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
    member this.InputBinding = DynObj.tryGetTypedPropertyValue<InputBinding> ("inputBinding") this
    member this.Optional = DynObj.tryGetTypedPropertyValue<bool> ("optional") this
