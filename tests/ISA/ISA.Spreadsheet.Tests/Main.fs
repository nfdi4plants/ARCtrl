module ISA.XLSX.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

[<Tests>]
#endif
let all = testSequenced <| testList "All" [
    ArcInvestigationTests.main
    CompositeColumnTests.main
    ArcAssayTests.main
    SparseTableTests.main
]

let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER
    Mocha.runTests all
    #else
    Tests.runTestsWithCLIArgs [] argv all
    #endif