module ARCtrl.FileSystem.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ValidationPackages" [
    Tests.ValidationPackage.main
    Tests.ValidationPackagesConfig.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
