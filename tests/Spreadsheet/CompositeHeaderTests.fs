module CompositeHeaderTests

open TestingUtils

open ARCtrl
open ARCtrl.Spreadsheet
open FsSpreadsheet

let deprecatedDataHeaders =
    testList "DeprecatedIOHeaders" [
        testCase "Raw Data File" <| fun _ ->
            let cells = [|"Input [Raw Data File]"|]
            let header,_ = CompositeHeader.fromStringCells cells
            Expect.equal header (CompositeHeader.Input IOType.Data) "Should be Input [Data]"
        testCase "Derived Data File" <| fun _ ->
            let cells = [|"Output [Derived Data File]"|]
            let header,_ = CompositeHeader.fromStringCells cells
            Expect.equal header (CompositeHeader.Output IOType.Data) "Should be Output [Data]"
        testCase "Image File" <| fun _ ->
            let cells = [|"Output [Image File]"|]
            let header,_ = CompositeHeader.fromStringCells cells
            Expect.equal header (CompositeHeader.Output IOType.Data) "Should be Output [Image]" 
    ]

let parameter_Tests =
   testList "Parameter" [
        testCase "onlyName WriteRead" <| fun _ ->
            let oa = OntologyAnnotation ("Name")
            let header = CompositeHeader.Parameter oa
            let out = CompositeHeader.toStringCells false header
            let inAgain,_ = CompositeHeader.fromStringCells out
            Expect.equal header inAgain "Should be the same"
    ]


let main = 
    testList "CompositeHeader" [
        deprecatedDataHeaders
        parameter_Tests
    ]