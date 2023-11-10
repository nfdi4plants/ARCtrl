module ARCtrl.FileSystem.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto

[<Tests>]
#endif
let all = testSequenced <| testList "FileSystem" [
    ARCtrl.FileSystemTree.Tests.main
]

let [<EntryPoint>] main argv = 
    #if FABLE_COMPILER
    Mocha.runTests all
    #else
    Tests.runTestsWithCLIArgs [] argv all
    #endif
