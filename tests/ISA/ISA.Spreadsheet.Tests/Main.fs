module ARCtrl.ISA.XLSX.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ISA.Spreadsheet" [
    FableTests.main
    ArcInvestigationTests.main
    CompositeColumnTests.main
    ArcTableTests.main
    ArcAssayTests.main
    ArcStudyTests.main
    SparseTableTests.main
    IdentifierTests.main
    Tests.Performance.Main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
