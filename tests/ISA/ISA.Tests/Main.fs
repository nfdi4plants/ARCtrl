module ARCtrl.ISADotnet.Tests

#if FABLE_COMPILER_PYTHON
open Fable.Pyxpecto
#endif
#if FABLE_COMPILER_JAVASCRIPT
open Fable.Mocha
#endif
#if !FABLE_COMPILER
open Expecto

[<Tests>]
#endif
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

let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER_PYTHON
    Pyxpecto.runTests (ConfigArg.fromStrings argv) all
    #endif
    #if FABLE_COMPILER_JAVASCRIPT
    Mocha.runTests all
    #endif
    #if !FABLE_COMPILER
    Tests.runTestsWithCLIArgs [] [||] all
    #endif