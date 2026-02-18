module Main.Tests

open Fable.Pyxpecto
open Fable.Core

let all = testList "All" [  
    ARCtrl.Core.Tests.all
    ARCtrl.Json.Tests.all
    ARCtrl.Spreadsheet.Tests.all
    ARCtrl.FileSystem.Tests.all
    ARCtrl.Yaml.Tests.all
    ARCtrl.ValidationPackages.Tests.all
    ARCtrl.Contract.Tests.all
    ARCtrl.ROCrate.Tests.all
    ARCtrl.CWL.Tests.all
    ARCtrl.WorkflowGraph.Tests.all
    ARCtrl.ARC.Tests.all
]

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open TestingUtils.TSUtils

describe("index", fun () -> 
    itAsync ("add", fun () ->
        Pyxpecto.runTestsAsync [| ConfigArg.DoNotExitWithCode|] all
        |> Async.StartAsPromise
        |> Promise.map (fun (exitCode : int) -> Expect.equal exitCode 0 "Tests failed")
    )
)
#else
[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
#endif
