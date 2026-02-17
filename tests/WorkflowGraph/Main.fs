module ARCtrl.WorkflowGraph.Tests

open Fable.Pyxpecto

let all =
    testSequenced <|
        testList "WorkflowGraph" [
            Tests.ReferenceParsing.main
            Tests.Builder.main
            Tests.Integration.main
            Tests.Adapters.main
            Tests.EdgeCases.main
            Tests.VisualizationSiren.main
            Tests.WorkflowGraph.main
        ]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
