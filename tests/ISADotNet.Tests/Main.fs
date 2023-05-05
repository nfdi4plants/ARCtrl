module ISADotnet.Tests

open ISADotNet
open Expecto

[<EntryPoint>]
let main argv =

    // API functionality Tests
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv APITests.testUpdate |> ignore
    0