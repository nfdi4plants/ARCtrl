module ARCtrl.Json.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "Json" [
    Tests.Decoder.Main
    Tests.OntologyAnnotation.Main
    Tests.CompositeCell.Main
    Tests.IOType.Main
    Tests.CompositeHeader.Main
    Tests.ArcTable.Main
    Tests.Assay.Main
    //Tests.ArcTypes.Main
    //Json.Tests.main
    //JsonSchema.Tests.main
    //Tests.ROCrate.Main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all