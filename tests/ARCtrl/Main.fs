module Main.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ARCtrl" [
    ARCtrl.Contracts.Tests.main
    ARCtrl.WebRequest.Tests.main
    ARCtrl.SemVer.Tests.main
    ARCtrl.Template.Tests.main
    ARCtrl.Tests.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all