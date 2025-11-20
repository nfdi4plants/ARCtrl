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
    member this.Type_ = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
    member this.OutputBinding = DynObj.tryGetTypedPropertyValue<OutputBinding> ("outputBinding") this
    member this.OutputSource = DynObj.tryGetTypedPropertyValue<string> ("outputSource") this