namespace ARCtrl.CWL

open CWLTypes
open DynamicObj

module Outputs =

    type OutputBinding = {
        Glob: string option
    }

    type Output (
        name: string,
        ?type_: CWLType,
        ?outputBinding: OutputBinding
    ) as this =
        inherit DynamicObj ()
        do
            DynObj.setValueOpt this ("type") type_
            DynObj.setValueOpt this ("outputBinding") outputBinding
        member this.Name = name
        member this.Type = DynObj.tryGetTypedValue<CWLType> ("type") this
        member this.OutputBinding = DynObj.tryGetTypedValue<OutputBinding> ("outputBinding") this

    module Workflow =

        type StepOutput = {
            Id: string
        }