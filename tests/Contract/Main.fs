module ARCtrl.Contract.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ValidationPackages" [
    Tests.ValidationPackagesConfig.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
