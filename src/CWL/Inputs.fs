namespace ARCtrl.CWL

open CWLTypes
open Outputs.Workflow
open DynamicObj

module Inputs =

    type InputBinding = {
        Prefix: string option
        Position: int option
        ItemSeparator: string option
        Separate: bool option
    }

    type Input (
        name: string,
        ?type_: CWLType,
        ?inputBinding: InputBinding
    ) as this =
        inherit DynamicObj ()
        do
            DynObj.setValueOpt this ("type") type_
            DynObj.setValueOpt this ("inputBinding") inputBinding
        member this.Name = name
        member this.Type = DynObj.tryGetTypedValue<CWLType> ("type") this
        member this.InputBinding = DynObj.tryGetTypedValue<InputBinding> ("inputBinding") this

    module Workflow =

        type StepInput = {
            Id: string
            Source: StepOutput option
            linkMerge: string option
            defaultValue: string option
            valueFrom: string option
        }
