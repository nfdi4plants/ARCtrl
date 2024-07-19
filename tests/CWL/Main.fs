module ARCtrl.CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    ()
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
