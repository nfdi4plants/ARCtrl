namespace ARCtrl.CWL

open CWLTypes
open DynamicObj
open Fable.Core

module Outputs =

    type OutputBinding = {
        Glob: string option
    }

    [<AttachMembers>]
    type Output (
        name: string,
        ?type_: CWLType,
        ?outputBinding: OutputBinding,
        ?outputSource: string
    ) as this =
        inherit DynamicObj ()
        do
            DynObj.setOptionalProperty ("type") type_ this
            DynObj.setOptionalProperty ("outputBinding") outputBinding this
        member this.Name = name
        member this.Type_ = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
        member this.OutputBinding = DynObj.tryGetTypedPropertyValue<OutputBinding> ("outputBinding") this
        member this.OutputSource = DynObj.tryGetTypedPropertyValue<string> ("outputSource") this

    module Workflow =

        type StepOutput = {
            Id: string []
        }