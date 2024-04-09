module ARCtrl.NET.Tests

open Expecto


let all = testSequenced <| testList "All" [
        Path.Tests.main
        Contract.Tests.main
        Arc.Tests.main
    ]

[<EntryPoint>]
let main argv =

    Tests.runTestsWithCLIArgs [] argv all
