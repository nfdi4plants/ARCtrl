module CWL.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "CWL" [
    Tests.CWLObject.main

]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
