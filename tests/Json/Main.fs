module ARCtrl.Json.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "Json" [
    Tests.Decoder.main
    Tests.Comment.main
    Tests.OntologyAnnotation.main
    Tests.Data.main
    Tests.CompositeCell.main
    Tests.IOType.main
    Tests.CompositeHeader.main
    Tests.ArcTable.main
    Tests.Person.main
    Tests.Publication.main
    Tests.Assay.main
    Tests.Study.main
    Tests.Investigation.main
    Tests.Template.main
    Tests.Process.ProcessParameterValue.main
    Tests.Process.ProcessInput.main
    Tests.Process.Protocol.main
    Tests.Process.Process.main
    Tests.SchemaValidation.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all