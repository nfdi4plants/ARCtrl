module ARCtrl.ROCrate.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ROCrate" [
    Tests.LDContext.main
    Tests.LDNode.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
