module CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    Tests.CWLObject.testCWLToolDescription
    Tests.Outputs.testOutput
    Tests.Inputs.testInput
    Tests.Requirements.testRequirement
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
