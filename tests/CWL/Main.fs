module ARCtrl.CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    Tests.CWLWorkflow.main
    Tests.CWLObject.main
    Tests.Metadata.main
    Tests.Outputs.main
    Tests.Inputs.main
    Tests.Requirements.main
    Tests.WorkflowSteps.main
    Tests.YAMLParameterFile.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
