module ROCrate.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ROCrate" []

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
