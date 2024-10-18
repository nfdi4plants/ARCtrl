namespace ARCtrl.CWL

open CWLTypes
open Outputs.Workflow
open DynamicObj
open Fable.Core

module Inputs =

    type InputBinding = {
        Prefix: string option
        Position: int option
        ItemSeparator: string option
        Separate: bool option
    }

    [<AttachMembers>]
    type Input (
        name: string,
        ?type_: CWLType,
        ?inputBinding: InputBinding
    ) as this =
        inherit DynamicObj ()
        do
            DynObj.setOptionalProperty ("type") type_ this
            DynObj.setOptionalProperty ("inputBinding") inputBinding this
        member this.Name = name
        member this.Type_ = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
        member this.InputBinding = DynObj.tryGetTypedPropertyValue<InputBinding> ("inputBinding") this

    module Workflow =

        type StepInput = {
            Id: string
            Source: string option
            DefaultValue: string option
            ValueFrom: string option
        }
