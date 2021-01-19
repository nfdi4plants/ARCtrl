module ISADotnet.Tests.NetCore

open Expecto

[<EntryPoint>]
let main argv =

    //ISADotnet core tests
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXTests.testISAXLSXIO |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv UpdateTests.testUpdate |> ignore
    0