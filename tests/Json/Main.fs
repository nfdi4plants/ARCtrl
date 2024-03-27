module ARCtrl.Json.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "Json" [
    Tests.Decoder.main
    Tests.OntologyAnnotation.main
    Tests.CompositeCell.main
    Tests.IOType.main
    Tests.CompositeHeader.main
    Tests.ArcTable.main
    Tests.Assay.main
    Tests.Study.main
    Tests.Investigation.main
    Tests.Template.main
    //Tests.ArcTypes.main
    //Json.Tests.main
    //JsonSchema.Tests.main
    //Tests.ROCrate.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all