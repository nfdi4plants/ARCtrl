module ISADotnet.Tests

open Expecto

[<EntryPoint>]
let main argv =

    //XLSX IO Test
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXInvestigationTests.testStringConversions |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXInvestigationTests.testSparseMatrix |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv ISAXLSXInvestigationTests.testInvestigationFile |> ignore

    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testColumnHeaderFunctions |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testNodeGetterFunctions |> ignore
    //Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testHeaderSplittingFunctions |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testProcessGetter |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testProcessComparisonFunctions |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testMetaDataFunctions |> ignore
    Tests.runTestsWithCLIArgs [Tests.CLIArguments.Sequenced] argv AssayFileTests.testAssayFileReader |> ignore

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