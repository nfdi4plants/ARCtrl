module Main.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

[<Tests>]
#endif
let all = testSequenced <| testList "All" [
    ARCtrl.Contracts.Tests.main
    ARCtrl.WebRequest.Tests.main
    ARCtrl.SemVer.Tests.main
    ARCtrl.Template.Tests.main
    ARCtrl.Tests.main
]

let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER
    Mocha.runTests all
    #else
    Tests.runTestsWithCLIArgs [] argv all
    #endif