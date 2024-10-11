module CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    Tests.CWLObject.testCWLToolDescription
    Tests.CWLObjectMetadata.testCWLToolDescriptionMetadata
    Tests.Metadata.testMetadata
    Tests.Outputs.testOutput
    Tests.Inputs.testInput
    Tests.Requirements.testRequirement
    Tests.WorkflowSteps.testWorkflowStep
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
