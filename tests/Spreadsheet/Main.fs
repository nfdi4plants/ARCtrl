module Spreadsheet.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ISA.Spreadsheet" [
    FableTests.main
    RegexTests.main
    ArcInvestigationTests.main
    CompositeColumnTests.main
    CompositeHeaderTests.main
    ArcTableTests.main
    ArcAssayTests.main
    ArcStudyTests.main
    SparseTableTests.main
    IdentifierTests.main
    TemplateTests.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
