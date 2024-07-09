module ARCtrl.FileSystem.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ValidationPackages" []

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
