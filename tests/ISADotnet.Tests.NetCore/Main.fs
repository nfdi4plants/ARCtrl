module ISADotnet.Tests.NetCore

open Expecto

[<EntryPoint>]
let main argv =

    //ISADotnet core tests
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXTests.testInvestigationFile |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXTests.testSparseMatrix |> ignore

    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv APITests.testUpdate |> ignore
    0