module ARCtrl.ISA.Json.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

[<Tests>]
#endif
let all = testSequenced <| testList "ISA.JSON" [
    Json.Tests.main
    JsonSchema.Tests.main
    Tests.ArcTypes.Main
]


let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER
    Mocha.runTests all
    #else
    Tests.runTestsWithCLIArgs [] argv all
    #endif