module ARCtrl.ISADotnet.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

[<Tests>]
#endif
let all = testSequenced <| testList "All" [
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

let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER
    Mocha.runTests all
    #else
    Tests.runTestsWithCLIArgs [] argv all
    #endif