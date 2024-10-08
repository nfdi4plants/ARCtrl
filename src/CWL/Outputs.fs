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
            DynObj.setOptionalProperty ("type") type_ this
            DynObj.setOptionalProperty ("outputBinding") outputBinding this
        member this.Name = name
        member this.Type_ = DynObj.tryGetTypedPropertyValue<CWLType> ("type") this
        member this.OutputBinding = DynObj.tryGetTypedPropertyValue<OutputBinding> ("outputBinding") this

    module Workflow =

        type StepOutput = {
            Id: string []
        }