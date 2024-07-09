module ARCtrl.Yaml.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "Yaml" [
    Tests.ValidationPackage.main
    Tests.ValidationPackagesConfig.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
