module ARCtrl.ValidationPackages.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ValidationPackages" [
    Tests.ValidationPackage.main
    Tests.ValidationPackagesConfig.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
