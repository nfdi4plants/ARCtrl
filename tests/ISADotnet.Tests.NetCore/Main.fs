module ISADotnet.Tests.NetCore

open Expecto

[<EntryPoint>]
let main argv =

    //XLSX IO Test
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXTests.testStringConversions |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXTests.testSparseMatrix |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXTests.testInvestigationFile |> ignore

    // Json IO Tests
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv JsonExtensionsTests.testAnyOf |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv JsonExtensionsTests.testStringEnum |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAJsonTests.testProtocolFile |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAJsonTests.testProcessFile |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAJsonTests.testAssayFile |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAJsonTests.testInvestigationFile |> ignore

    // API functionality Tests
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv APITests.testUpdate |> ignore
    0