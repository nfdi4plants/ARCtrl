module ARCtrl.ISA.XLSX.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

[<Tests>]
#endif
let all = testSequenced <| testList "ISA.Spreadsheet" [
    FableTests.main
    ArcInvestigationTests.main
    CompositeColumnTests.main
    ArcTableTests.main
    ArcAssayTests.main
    ArcStudyTests.main
    SparseTableTests.main
    IdentifierTests.main
]

let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER
    Mocha.runTests all
    #else
    Tests.runTestsWithCLIArgs [] argv all
    #endif