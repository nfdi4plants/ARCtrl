module ARCtrl.CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    Tests.CWLWorkflow.testCWLWorkflowDescription
    Tests.CWLObject.testCWLToolDescription
    Tests.CWLObjectMetadata.testCWLToolDescriptionMetadata
    Tests.Metadata.testMetadata
    Tests.Outputs.testOutput
    Tests.Inputs.testInput
    Tests.Requirements.testRequirement
    Tests.WorkflowSteps.testWorkflowStep
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
