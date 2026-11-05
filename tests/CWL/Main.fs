module ARCtrl.CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    Tests.CWLWorkflow.main
    Tests.CWLObject.main
    Tests.Types.main
    Tests.Layout.main
    Tests.Equality.main
    Tests.SchemaSalad.main
    Tests.Descriptions.main
    Tests.Metadata.main
    Tests.Outputs.main
    Tests.Inputs.main
    Tests.Requirements.main
    Tests.WorkflowSteps.main
    Tests.Directives.main
    Tests.Scalars.main
    Tests.Messages.main
    Tests.Quoting.main
    Tests.Shapes.main
    Tests.Keys.main
    Tests.YAMLParameterFile.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
