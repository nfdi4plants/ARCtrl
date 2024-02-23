module ARCtrl.ISADotnet.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ISA" [
    Update.Tests.main
    DataModel.Tests.main
    Regex.Tests.main
    Person.Tests.main
    CompositeHeader.Tests.main
    CompositeCell.Tests.main
    CompositeColumn.Tests.main
    ArcTables.Tests.main
    ArcTable.Tests.main
    ArcAssay.Tests.main
    ArcStudy.Tests.main
    ArcInvestigation.Tests.main
    ArcJsonConversion.Tests.main
    Identifier.Tests.main
    Fable.Tests.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all