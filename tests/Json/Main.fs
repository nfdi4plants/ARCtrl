module ARCtrl.ISA.Json.Tests

open Fable.Pyxpecto


let all = testSequenced <| testList "ISA.JSON" [
    Json.Tests.main
    JsonSchema.Tests.main
    Tests.ROCrate.Main
    Tests.ArcTypes.Main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all