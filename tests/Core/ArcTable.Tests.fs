module ArcTable.Tests

open ARCtrl
open ARCtrl.Helper
open TestingUtils

let TableName = "Test"
/// "species", "GO", "GO:0123456"
let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
/// "Chlamy", "NCBI", "NCBI:0123456"
let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
/// "instrument model", "MS", "MS:0123456"
let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
/// "SCIEX instrument model", "MS", "MS:654321"
let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
/// "temperature","NCIT","NCIT:0123210"
let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")
/// "degreeCelsius","UO","UO:0000027"
let oa_degreeCelsius = OntologyAnnotation("degreeCelsius","UO","UO:0000027")


/// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
let tableValues_printable (table:ArcTable) = 
    [
        for KeyValue((c,r),v) in table.Values do
            yield $"({c},{r}) {v}"
    ]

let createCells_FreeText pretext (count) = ResizeArray.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
let createCells_Term (count) = ResizeArray.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
let createCells_Unitized (count) = ResizeArray.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation()))
let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 5)
let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 5)
let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 5)
let column_param = CompositeColumn.create(CompositeHeader.Parameter (OntologyAnnotation()), createCells_Unitized 5)
/// Valid TestTable with 5 columns, 5 rows: 
///
/// Input [Source] -> 5 cells: [Source_1; Source_2..]
///
/// Output [Sample] -> 5 cells: [Sample_1; Sample_2..]
///
/// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]    
///
/// Component [instrument model] -> 5 cells: [SCIEX instrument model; SCIEX instrument model; ..]
///
/// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]   
let create_testTable() = 
    let t = ArcTable.init(TableName)
    let columns = [|
        column_input
        column_output
        column_param
        column_component
        column_param
    |]
    t.AddColumns(columns)
    t

let private tests_member = 
    testList "Member" [
        let table = ArcTable.init(TableName)
        testCase "ColumnCount empty" (fun () ->
            Expect.equal table.ColumnCount 0 "ColumnCount = 0"
        )
        testCase "RowCount empty" (fun () ->
            Expect.equal table.RowCount 0 "RowCount = 0"
        )
        testCase "ColumnCount" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 ""
        )
        testCase "ColumnCount Static member" (fun () ->
            let table = create_testTable()
            Expect.equal (ArcTable.columnCount table) 5 ""
        )
        testCase "RowCount" (fun () ->
            let table = create_testTable()
            Expect.equal table.RowCount 5 ""
        )
        testCase "RowCount Static member" (fun () ->
            let table = create_testTable()
            Expect.equal (table.RowCount) 5 ""
        )
        testCase "Custom equality" (fun () ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            Expect.equal table1 table2 "equal"
        )
    ]

let private tests_GetHashCode = testList "GetHashCode" [
    testCase "GetHashCode" <| fun _ ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            let hash1 = table1.GetHashCode()
            let hash2 = table2.GetHashCode()
            Expect.equal table1 table2 "Table"
            Expect.equal hash1 hash2 "HashCode"
    testCase "equal, column-add-order does not matter" <| fun _ ->
        let column_inputHeader, column_inputValues = CompositeHeader.Input IOType.Source, [|for i in 0 .. 20 do yield (0,i), sprintf "Source_%i" i |> CompositeCell.createFreeText|]
        let column_outputHeader, column_outputValues = CompositeHeader.Output IOType.Data, [|for i in 0 .. 20 do yield (1,i), sprintf "File_%i" i |> CompositeCell.createFreeText|]
        let createTable(reverse:bool) =
            let t = ArcTable.init("MyTable")
            let shuffled = if reverse then [|yield! column_outputValues; yield! column_inputValues|] else [|yield! column_inputValues; yield! column_outputValues|]
            t.Headers.AddRange([|column_inputHeader; column_outputHeader|])
            for ((colI,rowI),value) in shuffled do
                t.SetCellAt(colI, rowI, value)
            Expect.isTrue (t.Validate()) "is valid!"
            t
        let table1 = createTable(false)
        let table2 = createTable(true)
        let v1 = table1.Values |> Array.ofSeq |> Array.map (function | KeyValue (k,v) -> [box k;box v])
        let v2 = table2.Values |> Array.ofSeq |> Array.map (function | KeyValue (k,v) -> [box k;box v])
        // IEnumerable of table was changed, now includes actual order of columns and rows, therefore these values are equal now
        //Expect.notEqual v1 v2 "seq not equal, proofs that in fact the table was built in different order. But because we use dictionary with index keys, order is irrelevant."
        Expect.equal table1 table2 "table equal"
        Expect.equal (table1.GetHashCode()) (table2.GetHashCode()) "Hash"
    testCase "notEqual table-order" <| fun _ ->
        let actual = ArcTable.init("My Assay")
        let notActual = create_testTable()
        Expect.notEqual actual notActual "equal"
        Expect.notEqual (actual.GetHashCode()) (notActual.GetHashCode()) "Hash"
    testCase "equal? sparse vs constant column" <| fun _ ->
        let cell = CompositeCell.createFreeText "Value"
        let valueMap = System.Collections.Generic.Dictionary<int, CompositeCell>()
        valueMap.Add(cell.GetHashCode(), cell)

        let sparseDict = System.Collections.Generic.Dictionary<int, int>()
        sparseDict.Add(0, cell.GetHashCode())
        sparseDict.Add(1, cell.GetHashCode())
        let sparseColumn = ArcTableAux.Sparse sparseDict
        let sparseColumns = System.Collections.Generic.Dictionary<int, ArcTableAux.ColumnValueRefs>()
        sparseColumns.Add(0, sparseColumn)
        let sparseTable = ArcTableAux.ArcTableValues(sparseColumns, valueMap, 2)

        let constantColumn = ArcTableAux.Constant (cell.GetHashCode())
        let constantColumns = System.Collections.Generic.Dictionary<int, ArcTableAux.ColumnValueRefs>()
        constantColumns.Add(0, constantColumn)
        let constantTable = ArcTableAux.ArcTableValues(constantColumns, valueMap, 2)

        Expect.equal (sparseTable.GetHashCode()) (constantTable.GetHashCode()) "HashCodes should be equal for sparse and constant column with same values"

    ptestCase "notequal? sparse vs constant column" <| fun _ ->
        let cell = CompositeCell.createFreeText "Value"
        let valueMap = System.Collections.Generic.Dictionary<int, CompositeCell>()
        valueMap.Add(cell.GetHashCode(), cell)

        let sparseDict = System.Collections.Generic.Dictionary<int, int>()
        sparseDict.Add(0, cell.GetHashCode())
        sparseDict.Add(1, cell.GetHashCode())
        let sparseColumn = ArcTableAux.Sparse sparseDict
        let sparseColumns = System.Collections.Generic.Dictionary<int, ArcTableAux.ColumnValueRefs>()
        sparseColumns.Add(0, sparseColumn)
        let sparseTable = ArcTableAux.ArcTableValues(sparseColumns, valueMap, 2)

        let constantColumn = ArcTableAux.Constant (cell.GetHashCode())
        let constantColumns = System.Collections.Generic.Dictionary<int, ArcTableAux.ColumnValueRefs>()
        constantColumns.Add(0, constantColumn)
        let constantTable = ArcTableAux.ArcTableValues(constantColumns, valueMap, 2)

        Expect.notEqual (sparseTable.GetHashCode()) (constantTable.GetHashCode()) "HashCodes should not be equal for sparse and constant column with same values"
]

let private tests_validate = 
    testList "Validate" [
        testCase "empty table" (fun () ->
            let table = ArcTable.init(TableName)
            let isValid = table.Validate()
            Expect.isTrue isValid ""
        )
        testCase "table" (fun () ->
            let table = create_testTable()
            let isValid = table.Validate()
            Expect.isTrue isValid ""
        )
        testCase "empty table, raise not" (fun () ->
            let table = ArcTable.init(TableName)
            let isValid = table.Validate(true)
            Expect.isTrue isValid ""
        )
        testCase "table, raise not" (fun () ->
            let table = create_testTable()
            let isValid = table.Validate(true)
            Expect.isTrue isValid ""
        )
    ]

let private tests_SanityChecks = testList "SanityChecks" [
    testList "ValidateHeaders" [
        let headers_valid = [
            CompositeHeader.Input IOType.Sample
            CompositeHeader.FreeText "Any"
            CompositeHeader.Component (OntologyAnnotation())
            CompositeHeader.ProtocolREF
            CompositeHeader.ProtocolType
            CompositeHeader.Output IOType.Data
        ]
        testCase "valid headers" (fun () ->
            let columns = headers_valid |> Seq.map (fun x -> CompositeColumn.create(x))
            let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
            eval()
            Expect.isTrue true "We canot eval for 'not throw'"
        )
        testCase "valid headers, multiplied non unique" (fun () ->
            let header_valid2 = [CompositeHeader.FreeText "Any"; CompositeHeader.Component (OntologyAnnotation()); CompositeHeader.Component (OntologyAnnotation())]@headers_valid
            let columns = header_valid2 |> Seq.map (fun x -> CompositeColumn.create(x))
            let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
            eval()
            Expect.isTrue true "We canot eval for 'not throw'"
        )
        testCase "invalid headers, duplicate input" (fun () ->
            let header_invalid = CompositeHeader.Input IOType.Source::headers_valid
            let columns = header_invalid |> Seq.map (fun x -> CompositeColumn.create(x))
            let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
            Expect.throws eval ""
        )
        testCase "invalid headers, duplicate output" (fun () ->
            let header_invalid = CompositeHeader.Output IOType.Source::headers_valid
            let columns = header_invalid |> Seq.map (fun x -> CompositeColumn.create(x))
            let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
            Expect.throws eval ""
        )
        testCase "invalid headers, duplicate ProtocolREF" (fun () ->
            let header_invalid = CompositeHeader.ProtocolREF::headers_valid
            let columns = header_invalid |> Seq.map (fun x -> CompositeColumn.create(x))
            let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
            Expect.throws eval ""
        )
        testCase "invalid headers, duplicate ProtocolType" (fun () ->
            let header_invalid = CompositeHeader.ProtocolREF::headers_valid
            let columns = header_invalid |> Seq.map (fun x -> CompositeColumn.create(x))
            let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
            Expect.throws eval ""
        )
    ]
    testList "ValidateIndex" [
        testCase "column, append index, append unallowed" (fun () ->
            let index = 5
            let count = 5
            let eval() = ArcTableAux.SanityChecks.validateColumnIndex index count false 
            Expect.throws eval ""
        )
        testCase "column, append index, append allowed" (fun () ->
            let index = 5
            let count = 5
            let eval() = ArcTableAux.SanityChecks.validateColumnIndex index count true
            eval()
            Expect.isTrue true ""
        )
        testCase "row, append index, append unallowed" (fun () ->
            let index = 5
            let count = 5
            let eval() = ArcTableAux.SanityChecks.validateRowIndex index count false 
            Expect.throws eval ""
        )
        testCase "row, append index, append allowed" (fun () ->
            let index = 5
            let count = 5
            let eval() = ArcTableAux.SanityChecks.validateRowIndex index count true
            eval()
            Expect.isTrue true ""
        )
        testCase "col, negative index" (fun () ->
            let index = -1
            let count = 5
            let eval() = ArcTableAux.SanityChecks.validateColumnIndex index count false 
            let eval2() = ArcTableAux.SanityChecks.validateColumnIndex index count true 
            Expect.throws eval ""
            Expect.throws eval2 ""
        )
        testCase "row, negative index" (fun () ->
            let index = -1
            let count = 5
            let eval() = ArcTableAux.SanityChecks.validateRowIndex index count false 
            let eval2() = ArcTableAux.SanityChecks.validateRowIndex index count true 
            Expect.throws eval ""
            Expect.throws eval2 ""
        )
    ]
]

let private tests_ArcTableAux =
    testList "ArcTableAux" [
        testList "tryFindDuplicateUnique" [
            let headers = [
                CompositeHeader.Input IOType.Sample
                CompositeHeader.FreeText "Any"
                CompositeHeader.Component (OntologyAnnotation())
                CompositeHeader.ProtocolREF
                CompositeHeader.ProtocolType
                CompositeHeader.Output IOType.Data
            ]
            testCase "No duplicate, component" (fun () ->
                let header = CompositeHeader.Component (OntologyAnnotation())
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, parameter" (fun () ->
                let header = CompositeHeader.Parameter (OntologyAnnotation())
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, factor" (fun () ->
                let header = CompositeHeader.Factor (OntologyAnnotation())
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, chara" (fun () ->
                let header = CompositeHeader.Characteristic (OntologyAnnotation())
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, freetext" (fun () ->
                let header = CompositeHeader.FreeText "Test"
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "duplicate input" (fun () ->
                let header = CompositeHeader.Input IOType.Sample
                let expected = Some 0
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isSome hasDuplicate "Some/None"
                Expect.equal hasDuplicate expected "index"
            )
            testCase "duplicate input2" (fun () ->
                // IOType should not(!) matter
                let header = CompositeHeader.Input <| IOType.FreeText "SomeInput"
                let expected = Some 0
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isSome hasDuplicate "Some/None"
                Expect.equal hasDuplicate expected "index"
            )
            testCase "duplicate output" (fun () ->
                let header = CompositeHeader.Output IOType.Sample
                let expected = Some 5
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isSome hasDuplicate "Some/None"
                Expect.equal hasDuplicate expected "index"
            )
            testCase "duplicate ProtocolREF" (fun () ->
                let header = CompositeHeader.ProtocolREF
                let expected = Some 3
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isSome hasDuplicate "Some/None"
                Expect.equal hasDuplicate expected "index"
            )
            testCase "duplicate ProtocolType" (fun () ->
                let header = CompositeHeader.ProtocolType
                let expected = Some 4
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isSome hasDuplicate "Some/None"
                Expect.equal hasDuplicate expected "index"
            )
        ]
    ]

let private tests_GetCellAt =
    testList "GetCellAt" [
        testCase "InsideBoundaries" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
                
            Expect.equal (table.GetCellAt(0,0)) (CompositeCell.FreeText "Source_0") "Wrong at 0,0"
            Expect.equal (table.GetCellAt(0,1)) (CompositeCell.FreeText "Source_1") "Wrong at 0,1"
            Expect.equal (table.GetCellAt(1,0)) (CompositeCell.FreeText "Sample_0") "Wrong at 1,0"
            Expect.equal (table.GetCellAt(1,1)) (CompositeCell.FreeText "Sample_1") "Wrong at 1,1"       
        )
        testCase "Outside Boundaries" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
            let f = fun () -> table.GetCellAt(2,0) |> ignore

            Expect.throws f "Should have failed"
        )       
    ]

let private tests_TryGetCellAt =
    testList "TryGetCellAt" [
        testCase "InsideBoundaries" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
                
            let cell1 = Expect.wantSome (table.TryGetCellAt(0,0)) "Should have found cell at 0,0"
            let cell2 = Expect.wantSome (table.TryGetCellAt(0,1)) "Should have found cell at 0,1"
            let cell3 = Expect.wantSome (table.TryGetCellAt(1,0)) "Should have found cell at 1,0"
            let cell4 = Expect.wantSome (table.TryGetCellAt(1,1)) "Should have found cell at 1,1"

            Expect.equal cell1 (CompositeCell.FreeText "Source_0") "Wrong at 0,0"
            Expect.equal cell2 (CompositeCell.FreeText "Source_1") "Wrong at 0,1"
            Expect.equal cell3 (CompositeCell.FreeText "Sample_0") "Wrong at 1,0"
            Expect.equal cell4 (CompositeCell.FreeText "Sample_1") "Wrong at 1,1"
    
        )
        testCase "Outside Boundaries" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
            let cell = table.TryGetCellAt(2,0)
            Expect.isNone None "Should have failed"
        )       
    


    ]

let private tests_Columns =
    testList "Columns" [
        testCase "Empty" (fun () ->
            let table = ArcTable.init "MyTable"
            let cols = table.Columns
            Expect.hasLength cols 0 "Columns should be empty"
        )
        testCase "OnlyHeaders" (fun () ->
            let table = ArcTable.init "MyTable"
            table.AddColumn(CompositeHeader.Input IOType.Source)
            table.AddColumn(CompositeHeader.Output IOType.Sample)
            let cols = table.Columns
            Expect.hasLength cols 2 "Columns should have 2 headers only"
            Expect.equal cols.[0].Header (CompositeHeader.Input IOType.Source) "Column 0 header should be Input"
            Expect.hasLength cols.[0].Cells 0 "Column 0 should have no cells"
            Expect.equal cols.[1].Header (CompositeHeader.Output IOType.Sample) "Column 1 header should be Output"
            Expect.hasLength cols.[1].Cells 0 "Column 1 should have no cells"
        )
        testCase "OnlyHeaders_Constructor" (fun () ->
            let table = ArcTable.create(
                "jointable",
                ResizeArray([CompositeHeader.Input IOType.Data]),
                ResizeArray()
            )
            let cols = table.Columns
            Expect.hasLength cols 1 "Columns should have 1 header only"
            Expect.equal cols.[0].Header (CompositeHeader.Input IOType.Data) "Column 0 header should be Input"
            Expect.hasLength cols.[0].Cells 0 "Column 0 should have no cells"

        )
        testCase "Simple" (fun () ->
            let table = create_testTable()
            let cols = table.Columns
            Expect.hasLength cols 5 "Columns should have 5 headers"
            Expect.equal cols.[0].Header column_input.Header "Column 0 header should be Input"
            Expect.equal cols.[0].Cells.[0] (CompositeCell.FreeText "Source_0") "Column 0 first cell should be Source_0"
            Expect.hasLength cols.[0].Cells 5 "Column 0 should have 5 cells"
            Expect.equal cols.[1].Header column_output.Header "Column 1 header should be Output"
            Expect.hasLength cols.[1].Cells 5 "Column 1 should have 5 cells"
            Expect.equal cols.[2].Header column_param.Header "Column 2 header should be Parameter"
            Expect.hasLength cols.[2].Cells 5 "Column 2 should have 5 cells"
            Expect.equal cols.[3].Header column_component.Header "Column 3 header should be Component"
            Expect.hasLength cols.[3].Cells 5 "Column 3 should have 5 cells"
            Expect.equal cols.[4].Header column_param.Header "Column 4 header should be Parameter"
            Expect.hasLength cols.[4].Cells 5 "Column 4 should have 5 cells"
            Expect.equal cols.[4].Cells.[4] (CompositeCell.Unitized("4",OntologyAnnotation())) "Column 4 last cell should be unitized with value 4"
        )
    ]
    

let private tests_UpdateHeader = 
    testList "UpdateHeader" [
        testCase "ensure test table" (fun () ->
            let testTable = create_testTable()
            Expect.equal testTable.RowCount 5 "RowCount"
            Expect.equal testTable.ColumnCount 5 "ColumnCount"
            Expect.equal testTable.Headers.[0] column_input.Header "header0"
            Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal testTable.Headers.[1] column_output.Header "header1"
            Expect.equal testTable.Headers.[2] column_param.Header "header3"
            Expect.equal testTable.Headers.[3] column_component.Header "header2"
            Expect.equal testTable.Headers.[4] column_param.Header "header4"
            Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set outside of range" (fun () ->
            let testTable = create_testTable()
            let table() = testTable.UpdateHeader(12, CompositeHeader.Characteristic (OntologyAnnotation()))
            Expect.throws table ""
        )
        testCase "set outside of range negative" (fun () ->
            let testTable = create_testTable()
            let table() = testTable.UpdateHeader(-12, CompositeHeader.Characteristic (OntologyAnnotation()))
            Expect.throws table ""
        )
        testCase "set unique at different index" (fun () ->
            let testTable = create_testTable()
            let table() = testTable.UpdateHeader(2, CompositeHeader.Input IOType.Data)
            Expect.throws (table >> ignore) ""
        )
        testCase "set unique at same index" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Input IOType.Data
            table.UpdateHeader(0, newHeader)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] newHeader "header0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header3"
            Expect.equal table.Headers.[3] column_component.Header "header2"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set invalid" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Factor oa_temperature
            let eval() = table.UpdateHeader(0, newHeader)
            Expect.throws eval ""
        )
        testCase "set invalid, force convert" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Factor oa_temperature
            table.UpdateHeader(0, newHeader, true)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] newHeader "header0"
            Expect.equal table.Values.[0,0] (CompositeCell.createTermFromString "Source_0") "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header3"
            Expect.equal table.Headers.[3] column_component.Header "header2"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set valid" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Factor oa_temperature
            table.UpdateHeader(3, newHeader)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] column_input.Header "header0"
            Expect.equal table.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header2"
            Expect.equal table.Values.[2,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 2,4"
            Expect.equal table.Headers.[3] newHeader "header3"
            Expect.equal table.Values.[3,4] (CompositeCell.createTerm oa_SCIEXInstrumentModel) "cell 3,4"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        // force convert should never do anything in valid case
        testCase "set valid, force convert, term cells" (fun () ->
            let table = create_testTable()
            let table2 = table.Copy()
            let newHeader = CompositeHeader.Factor oa_temperature
            table.UpdateHeader(3, newHeader)
            table2.UpdateHeader(3, newHeader, true)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] column_input.Header "header0"
            Expect.equal table.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header2"
            Expect.equal table.Values.[2,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 2,4"
            Expect.equal table.Headers.[3] newHeader "header3"
            Expect.equal table.Values.[3,4] (CompositeCell.createTerm oa_SCIEXInstrumentModel) "cell 3,4"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
            TestingUtils.Expect.sequenceEqual table.Headers table2.Headers "equal table headers"
            TestingUtils.Expect.sequenceEqual table.Values table2.Values "equal table values"
        )
        testCase "set valid, forceConvert, unitized cells" <| fun _ ->
            let table = create_testTable()
            let table2 = table.Copy()
            let newHeader = CompositeHeader.Factor oa_temperature
            table.UpdateHeader(4, newHeader)
            table2.UpdateHeader(4, newHeader, true)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] column_input.Header "header0"
            Expect.equal table.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header2"
            Expect.equal table.Values.[2,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 2,4"
            Expect.equal table.Headers.[3] column_component.Header "header3"
            Expect.equal table.Values.[3,4] (CompositeCell.createTerm oa_SCIEXInstrumentModel) "cell 3,4"
            Expect.equal table.Headers.[4] newHeader "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
            TestingUtils.Expect.sequenceEqual table.Headers table2.Headers "equal table headers"
            TestingUtils.Expect.sequenceEqual table.Values table2.Values "equal table values"
    ]

let private tests_UpdateCellAt =
    testList "UpdateCellAt" [
        testCase "ensure test table" (fun () ->
            let testTable = create_testTable()
            Expect.equal testTable.RowCount 5 "RowCount"
            Expect.equal testTable.ColumnCount 5 "ColumnCount"
            Expect.equal testTable.Headers.[0] column_input.Header "header0"
            Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal testTable.Headers.[1] column_output.Header "header1"
            Expect.equal testTable.Headers.[2] column_param.Header "header2"
            Expect.equal testTable.Headers.[3] column_component.Header "header3"
            Expect.equal testTable.Headers.[4] column_param.Header "header4"
            Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set valid" (fun () ->
            let table = create_testTable()
            let cell = CompositeCell.createFreeText "MYNEWCELL42"
            table.UpdateCellAt(0,0,cell)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] column_input.Header "header0"
            Expect.equal table.Values.[0,0] cell "cell 0,0"
            Expect.equal table.Values.[0,1] (CompositeCell.FreeText "Source_1") "cell 0,1"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header2"
            Expect.equal table.Headers.[3] column_component.Header "header3"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set valid, at invalid indices" (fun () ->
            let table = create_testTable()
            let cell = CompositeCell.createFreeText "MYNEWCELL42"
            let eval_colCount() = table.UpdateCellAt(table.ColumnCount,0,cell)
            let eval_rowCount() = table.UpdateCellAt(0,table.RowCount,cell)
            let eval_negRow() = table.UpdateCellAt(0,-2,cell)
            let eval_negCol() = table.UpdateCellAt(-2,0,cell)
            Expect.throws eval_colCount "ColumnCount"
            Expect.throws eval_rowCount "RowCount"
            Expect.throws eval_negRow "negative row"
            Expect.throws eval_negCol "negative col"
        )
        testCase "set invalid" (fun () ->
            let table = create_testTable()
            let cell = CompositeCell.createTerm (OntologyAnnotation())
            let eval() = table.UpdateCellAt(0,0,cell)
            Expect.throws eval ""
        )
    ]

let private tests_SetCellAt =
    testList "SetCellAt" [
        testCase "Valid" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
            table.SetCellAt(0,1,CompositeCell.createFreeText "NewCell")
                
            Expect.equal (table.GetCellAt(0,0)) (CompositeCell.FreeText "Source_0") "Wrong at 0,0"
            Expect.equal (table.GetCellAt(0,1)) (CompositeCell.FreeText "NewCell") "Wrong at 0,1"
            Expect.equal (table.GetCellAt(1,0)) (CompositeCell.FreeText "Sample_0") "Wrong at 1,0"
            Expect.equal (table.GetCellAt(1,1)) (CompositeCell.FreeText "Sample_1") "Wrong at 1,1"       
        )
        testCase "Outside Row boundary (Valid)" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
            table.SetCellAt(0,3,CompositeCell.createFreeText "NewCell")
                
            Expect.equal (table.GetCellAt(0,0)) (CompositeCell.FreeText "Source_0") "Wrong at 0,0"
            Expect.equal (table.GetCellAt(0,1)) (CompositeCell.FreeText "Source_1") "Wrong at 0,1"
            Expect.equal (table.GetCellAt(1,0)) (CompositeCell.FreeText "Sample_0") "Wrong at 1,0"
            Expect.equal (table.GetCellAt(1,1)) (CompositeCell.FreeText "Sample_1") "Wrong at 1,1"
            Expect.equal (table.GetCellAt(0,3)) (CompositeCell.FreeText "NewCell") "Wrong at 0,3"   
        )
        testCase "Outside column boundary (Invalid)" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])

            let f = fun () ->table.SetCellAt(3,0,CompositeCell.createFreeText "NewCell")

            Expect.throws f "Should have failed"
        )
    ]

let private tests_UpdateCellsBy =
    testList "UpdateCellsBy" [
        testCase "InputAndOutput" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            let c2 = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            table.AddColumns([|c1;c2|])
            table.UpdateCellsBy(fun _ _ c ->
                match c with
                | CompositeCell.FreeText s -> CompositeCell.createFreeText (s + "_new")
                | _ -> c
             )
            Expect.equal (table.GetCellAt(0,0)) (CompositeCell.FreeText "Source_0_new") "Wrong at 0,0"
            Expect.equal (table.GetCellAt(0,1)) (CompositeCell.FreeText "Source_1_new") "Wrong at 0,1"
            Expect.equal (table.GetCellAt(1,0)) (CompositeCell.FreeText "Sample_0_new") "Wrong at 1,0"
            Expect.equal (table.GetCellAt(1,1)) (CompositeCell.FreeText "Sample_1_new") "Wrong at 1,1"           
        )
        testCase "Fail For Wrong CellType" (fun () ->
            let table = ArcTable.init "MyTable"
            let c1 = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            table.AddColumns([|c1|])
            let f = fun () ->
                table.UpdateCellsBy(fun _ _ c ->
                    c.ToTermCell()
                 )
            Expect.throws f "Should have failed"
        )
    ]

let private tests_UpdateColumn = 
    testList "UpdateColumn" [
        testCase "ensure test table" (fun () ->
            let testTable = create_testTable()
            Expect.equal testTable.RowCount 5 "RowCount"
            Expect.equal testTable.ColumnCount 5 "ColumnCount"
            Expect.equal testTable.Headers.[0] column_input.Header "header0"
            Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal testTable.Headers.[1] column_output.Header "header1"
            Expect.equal testTable.Headers.[2] column_param.Header "header3"
            Expect.equal testTable.Headers.[3] column_component.Header "header2"
            Expect.equal testTable.Headers.[4] column_param.Header "header4"
            Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set valid, at invalid index = columnCount" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Component <| OntologyAnnotation(name="TestTerm")
            let cells = createCells_Term 5
            let eval() = table.UpdateColumn(table.ColumnCount, h, cells)
            Expect.throws eval ""
        )
        testCase "set valid, at invalid negative index" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Component <| OntologyAnnotation(name="TestTerm")
            let cells = createCells_Term 5
            let eval() = table.UpdateColumn(-1, h, cells)
            Expect.throws eval ""
        )
        testCase "set valid" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Component <| OntologyAnnotation(name="TestTerm")
            let cells = createCells_Term 5
            table.UpdateColumn(0, h, cells)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] h "header0"
            Expect.equal table.Values.[0,0] cells.[0] "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header3"
            Expect.equal table.Headers.[3] column_component.Header "header2"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set unique duplicate, different index" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Input IOType.Data
            let cells = createCells_FreeText "NEWSOURCE" 5
            let column = CompositeColumn.create(h, cells)
            let eval() = table.UpdateColumn(1, h, cells)
            Expect.throws eval ""
        )
        testCase "set unique duplicate, same index" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Input IOType.Data
            let cells = createCells_FreeText "NEWSOURCE" 5
            table.UpdateColumn(0, h, cells)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] h "header0"
            Expect.equal table.Values.[0,0] cells.[0] "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header3"
            Expect.equal table.Headers.[3] column_component.Header "header2"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set valid, less rows" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Component <| OntologyAnnotation(name="TestTerm")
            let cells = createCells_Term 2
            table.UpdateColumn(0, h, cells)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] h "header0"
            Expect.equal (table.GetCellAt(0,0)) cells.[0] "cell 0,0"
            Expect.equal (table.GetCellAt(0,cells.Count)) (CompositeCell.emptyTerm) "cell 0,cells.Count"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header3"
            Expect.equal table.Headers.[3] column_component.Header "header2"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal (table.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
        )
        testCase "set valid, more rows" (fun () ->
            let table = create_testTable()
            let h = CompositeHeader.Component <| OntologyAnnotation(name="TestTerm")
            let cells = createCells_Term 7
            table.UpdateColumn(0, h, cells)
            Expect.equal table.RowCount 7 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] h "header0"
            Expect.equal (table.GetCellAt(0,0)) cells.[0] "cell 0,0"
            Expect.equal (table.GetCellAt(0,cells.Count - 1)) (cells.[cells.Count-1]) "cell 0,cells.Count-1"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_param.Header "header3"
            Expect.equal table.Headers.[3] column_component.Header "header2"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal (table.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
            Expect.equal (table.GetCellAt(4,5)) (CompositeCell.emptyUnitized) "cell 4,5"
        )
    ]

let private tests_AddColumn_Mutable =
    let header_input = CompositeHeader.Input IOType.Source
    let header_chara = CompositeHeader.Characteristic oa_species
    let createCells_chara (count) = ResizeArray.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
    let createCells_freetext pretext (count) = ResizeArray.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    testList "AddColumn" [
        testList "New Table" [
            let create_table() = ArcTable.init(TableName)
            testCase "IO column, no cells" (fun () ->
                let table = create_table()
                let header = header_input
                table.AddColumn(header)
                Expect.equal table.RowCount 0 "RowCount = 0"
                Expect.equal table.ColumnCount 1 "ColumnCount = 1"
                Expect.equal table.Headers.[0] header "header"
            )
            testCase "term column, no cells" (fun () ->
                let table = create_table()
                let header = header_chara
                table.AddColumn(header)
                Expect.equal table.RowCount 0 "RowCount = 0"
                Expect.equal table.ColumnCount 1 "ColumnCount = 1"
                Expect.equal table.Headers.[0] header "header"
            )
            testCase "IO column, with cells" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                table.AddColumn(header, cells)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.Headers.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in ResizeArray.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.Expect.sequenceEqual table.Values expected "values"
            )
            testCase "term column, with cells" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = ResizeArray.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                table.AddColumn(header, cells)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.Headers.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in ResizeArray.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.Expect.sequenceEqual table.Values expected "values"
            )
            testCase "IO column, with wrong cells" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = ResizeArray.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable() = table.AddColumn(header, cells)
                Expect.throws newTable ""
            )
            testCase "term column, with wrong cells" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable() = table.AddColumn(header, cells)
                Expect.throws newTable ""
            )
            testCase "IO column, with cells at index" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                table.AddColumn(header, cells, 0)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
            )
            testCase "term column, with cells at index" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = ResizeArray.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                table.AddColumn(header, cells, 0)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
            )
            testCase "IO column, with cells at outside index" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable() = table.AddColumn(header, cells, 1)
                Expect.throws newTable ""
            )
            testCase "term column, with cells at outside index" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = ResizeArray.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable() = table.AddColumn(header, cells, 1)
                Expect.throws newTable ""
            )
        ]
        testList "Existing Table" [
            /// Table contains 5 rows and 1 column Input [Source] with cells "Source_id"
            let create_table() = 
                let io_cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let table = ArcTable.init(TableName)
                table.AddColumn(header_input, io_cells)
                table
            testCase "Ensure base table" (fun () ->
                let table = create_table()
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
            )
            testCase "add equal rows" (fun () ->
                let table = create_table()
                table.AddColumn(header_chara, createCells_chara 5)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.Headers.[0] header_input "Header 0"
                Expect.equal table.Headers.[1] header_chara "Header 1"
            )
            testCase "add less rows" (fun () ->
                let table = create_table()
                let cells = createCells_chara 2
                table.AddColumn(header_chara, cells)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.Headers.[0] header_input "Header 0"
                Expect.equal table.Headers.[1] header_chara "Header 1"
                let expected = 
                    ResizeArray.init 2 (fun i -> 
                        let c = CompositeCell.createTerm oa_chlamy
                        System.Collections.Generic.KeyValuePair((1,i), c) 
                    )
                let actual = Array.ofSeq table.Values
                Expect.containsAll actual expected "extendedValues"
            )
            testCase "add more rows" (fun () ->
                let table = create_table()
                let cells = createCells_chara 8
                table.AddColumn(header_chara, cells)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 8 "RowCount"
                Expect.equal table.Headers.[0] header_input "Header 0"
                Expect.equal table.Headers.[1] header_chara "Header 1"
                let expected_chara = ResizeArray.init 8 (fun i -> System.Collections.Generic.KeyValuePair((1,i), CompositeCell.createTerm oa_chlamy))
                let expected_io = 
                    ResizeArray.init 5 (fun i -> 
                        let c = CompositeCell.createFreeText $"Source_{i}" 
                        System.Collections.Generic.KeyValuePair((0,i), c) 
                    )
                let actual = Array.ofSeq table.Values
                Expect.containsAll actual expected_chara "extendedValues chara"
                Expect.containsAll actual expected_io "extendedValues io"
            )
            testCase "add equal rows, insert at" (fun () ->
                let table = create_table()
                let cells = createCells_chara 5
                table.AddColumn(header_chara, cells, 0)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.Headers.[0] header_chara "Header chara"
                Expect.equal table.Headers.[1] header_input "Header io"
            )
            testCase "add less rows, insert at" (fun () ->
                let table = create_table()
                let cells = createCells_chara 2
                table.AddColumn(header_chara, cells, 0)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.Headers.[0] header_chara "Header chara"
                Expect.equal table.Headers.[1] header_input "Header io"
                let expected = 
                    ResizeArray.init 2 (fun i -> 
                        let c = CompositeCell.createTerm oa_chlamy
                        System.Collections.Generic.KeyValuePair((0,i), c) 
                    )
                let actual = Array.ofSeq table.Values
                Expect.containsAll actual expected "extendedValues"
            )
            testCase "add more rows, insert at" (fun () ->
                let table = create_table()
                let cells = createCells_chara 8
                table.AddColumn(header_chara, cells, 0)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 8 "RowCount"
                Expect.equal table.Headers.[0] header_chara "Header chara"
                Expect.equal table.Headers.[1] header_input "Header io"
                let expected_chara = ResizeArray.init 8 (fun i -> System.Collections.Generic.KeyValuePair((0,i), CompositeCell.createTerm oa_chlamy))
                let expected_io = 
                    ResizeArray.init 5 (fun i -> 
                        let c = CompositeCell.createFreeText $"Source_{i}"
                        System.Collections.Generic.KeyValuePair((1,i), c) 
                    )
                let actual = Array.ofSeq table.Values
                Expect.containsAll actual expected_chara "extendedValues chara"
                Expect.containsAll actual expected_io "extendedValues io"
            )
            testCase "add equal rows, replace input, throw" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                let table() = table.AddColumn(newHeader, createCells_freetext "NewInput" 5)
                Expect.throws table ""
            )
            testCase "add equal rows, replace input, replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                table.AddColumn(newHeader, createCells_freetext "NewInput" 5, forceReplace=true)
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.Headers.[0] newHeader "Header"
            )
            testCase "add more rows, replace input, force replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                table.AddColumn(newHeader, createCells_freetext "NewInput" 8, forceReplace=true)
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 8 "RowCount"
                Expect.equal table.Headers.[0] newHeader "Header"
            )
            testCase "add less rows, replace input, force replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                table.AddColumn(newHeader, createCells_freetext "NewInput" 2, forceReplace=true)
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 2 "RowCount, if this rowCount is higher, previous cells might not get deleted"
                Expect.equal table.Headers.[0] newHeader "Header"
                Expect.equal table.Values.[0,0] (CompositeCell.createFreeText "NewInput_0") "0,0"
            )
        ]
    ]

/// Exemplary tests to check non mutable implementation of mutability bases member function.
let private tests_addColumn =
    let header_input = CompositeHeader.Input IOType.Source
    let header_chara = CompositeHeader.Characteristic oa_species
    let createCells_chara (count) = ResizeArray.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
    let createCells_freetext pretext (count) = ResizeArray.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    testList "addColumn" [
        testList "New Table" [
            let create_table() = ArcTable.init(TableName)
            testCase "IO column, no cells" (fun () ->
                let table = create_table()
                let header = header_input
                let table_rep = create_table()
                let updatedTable =
                    table 
                    |> ArcTable.addColumn(header)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.RowCount 0 "RowCount = 0"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount = 1"
                Expect.equal updatedTable.Headers.[0] header "header"
            )
            testCase "term column, no cells" (fun () ->
                let table = create_table()
                let header = header_chara
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.RowCount 0 "RowCount = 0"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount = 1"
                Expect.equal updatedTable.Headers.[0] header "header"
            )
            testCase "IO column, with cells" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header, cells)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
                Expect.equal updatedTable.Headers.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in ResizeArray.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.Expect.sequenceEqual updatedTable.Values expected "values"
            )
            testCase "term column, with cells" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = ResizeArray.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header, cells)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
                Expect.equal updatedTable.Headers.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in ResizeArray.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.Expect.sequenceEqual updatedTable.Values expected "values"
            )
            testCase "IO column, with cells at index" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header, cells, 0)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
            )
            testCase "term column, with cells at index" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = ResizeArray.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header, cells, 0)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
            )
        ]
        testList "Existing Table" [
            /// Table contains 5 rows and 1 column Input [Source] with cells "Source_id"
            let create_table() = 
                let io_cells = ResizeArray.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let table = ArcTable.init(TableName)
                table.AddColumn(header_input, io_cells)
                table
            testCase "Ensure base table" (fun () ->
                let table = create_table()
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
            )
            testCase "add equal rows" (fun () ->
                let table = create_table()
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header_chara, createCells_chara 5)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 2 "ColumnCount"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.Headers.[0] header_input "Header 0"
                Expect.equal updatedTable.Headers.[1] header_chara "Header 1"
            )
            testCase "add less rows" (fun () ->
                let table = create_table()
                let cells = createCells_chara 2
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header_chara, cells)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 2 "ColumnCount"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.Headers.[0] header_input "Header 0"
                Expect.equal updatedTable.Headers.[1] header_chara "Header 1"
                let expected = 
                    ResizeArray.init 2 (fun i -> 
                        let c = CompositeCell.createTerm oa_chlamy
                        System.Collections.Generic.KeyValuePair((1,i), c) 
                    )
                let actual = Array.ofSeq updatedTable.Values
                Expect.containsAll actual expected "extendedValues"
            )
            testCase "add more rows" (fun () ->
                let table = create_table()
                let cells = createCells_chara 8
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header_chara, cells)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 2 "ColumnCount"
                Expect.equal updatedTable.RowCount 8 "RowCount"
                Expect.equal updatedTable.Headers.[0] header_input "Header 0"
                Expect.equal updatedTable.Headers.[1] header_chara "Header 1"
                let expected_chara = ResizeArray.init 8 (fun i -> System.Collections.Generic.KeyValuePair((1,i), CompositeCell.createTerm oa_chlamy))
                let expected_io = 
                    ResizeArray.init 5 (fun i -> 
                        let c = CompositeCell.createFreeText $"Source_{i}"
                        System.Collections.Generic.KeyValuePair((0,i), c) 
                    )
                let actual = Array.ofSeq updatedTable.Values
                Expect.containsAll actual expected_chara "extendedValues chara"
                Expect.containsAll actual expected_io "extendedValues io"
            )
            testCase "add equal rows, insert at" (fun () ->
                let table = create_table()
                let cells = createCells_chara 5
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header_chara, cells, 0)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 2 "ColumnCount"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.Headers.[0] header_chara "Header chara"
                Expect.equal updatedTable.Headers.[1] header_input "Header io"
            )
            testCase "add less rows, insert at" (fun () ->
                let table = create_table()
                let cells = createCells_chara 2
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header_chara, cells, 0)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 2 "ColumnCount"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.Headers.[0] header_chara "Header chara"
                Expect.equal updatedTable.Headers.[1] header_input "Header io"
                let expected = 
                    ResizeArray.init 2 (fun i -> 
                        let c = CompositeCell.createTerm oa_chlamy
                        System.Collections.Generic.KeyValuePair((0,i), c) 
                    )
                let actual = Array.ofSeq updatedTable.Values
                Expect.containsAll actual expected "extendedValues"
            )
            testCase "add more rows, insert at" (fun () ->
                let table = create_table()
                let cells = createCells_chara 8
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(header_chara, cells, 0)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 2 "ColumnCount"
                Expect.equal updatedTable.RowCount 8 "RowCount"
                Expect.equal updatedTable.Headers.[0] header_chara "Header chara"
                Expect.equal updatedTable.Headers.[1] header_input "Header io"
                let expected_chara = ResizeArray.init 8 (fun i -> System.Collections.Generic.KeyValuePair((0,i), CompositeCell.createTerm oa_chlamy))
                let expected_io = 
                    ResizeArray.init 5 (fun i -> 
                        let c = CompositeCell.createFreeText $"Source_{i}" 
                        System.Collections.Generic.KeyValuePair((1,i), c) 
                    )
                let actual = Array.ofSeq updatedTable.Values
                Expect.containsAll actual expected_chara "extendedValues chara"
                Expect.containsAll actual expected_io "extendedValues io"
            )
            testCase "add equal rows, replace input, replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(newHeader, createCells_freetext "NewInput" 5, forceReplace=true)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
                Expect.equal updatedTable.RowCount 5 "RowCount"
                Expect.equal updatedTable.Headers.[0] newHeader "Header"
            )
            testCase "add more rows, replace input, force replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(newHeader, createCells_freetext "NewInput" 8, forceReplace=true)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
                Expect.equal updatedTable.RowCount 8 "RowCount"
                Expect.equal updatedTable.Headers.[0] newHeader "Header"
            )
            testCase "add less rows, replace input, force replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                let table_rep = create_table()
                let updatedTable =
                    table |> ArcTable.addColumn(newHeader, createCells_freetext "NewInput" 2, forceReplace=true)
                Expect.equal table table_rep "origin table must be unchanged!"
                Expect.equal updatedTable.ColumnCount 1 "ColumnCount"
                Expect.equal updatedTable.RowCount 2 "RowCount, if this rowCount is higher, previous cells might not get deleted"
                Expect.equal updatedTable.Headers.[0] newHeader "Header"
                Expect.equal updatedTable.Values.[0,0] (CompositeCell.createFreeText "NewInput_0") "0,0"
            )
        ]
    ]

let private tests_AddColumns =
    testList "AddColumns" [
        testList "New Table" [
            let create_table() = ArcTable.init(TableName)
            testCase "IO columns, no cells" (fun () ->
                let table = create_table()
                let column_0 = CompositeColumn.create(CompositeHeader.Input IOType.Source)
                let column_1 = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
                let columns = [|
                    column_0
                    column_1
                |]
                table.AddColumns(columns)
                Expect.equal table.RowCount 0 "RowCount"
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.Headers.[0] column_0.Header "header0"
                Expect.equal table.Headers.[1] column_1.Header "header1"
            )
            testCase "multiple columns, no cells" (fun () ->
                let table = create_table()
                let column_0 = CompositeColumn.create(CompositeHeader.Input IOType.Source)
                let column_1 = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
                let column_2 = CompositeColumn.create(CompositeHeader.Component (OntologyAnnotation()))
                let column_3 = CompositeColumn.create(CompositeHeader.Parameter (OntologyAnnotation()))
                let columns = [|
                    column_0
                    column_1
                    column_2
                    column_3
                    column_3
                |]
                table.AddColumns(columns)
                Expect.equal table.RowCount 0 "RowCount"
                Expect.equal table.ColumnCount 5 "ColumnCount"
                Expect.equal table.Headers.[0] column_0.Header "header0"
                Expect.equal table.Headers.[1] column_1.Header "header1"
                Expect.equal table.Headers.[2] column_2.Header "header2"
                Expect.equal table.Headers.[3] column_3.Header "header3"
                Expect.equal table.Headers.[4] column_3.Header "header4"
            )
            testCase "multiple input, no cells, should throw" (fun () ->
                let table = create_table()
                let column_0 = CompositeColumn.create(CompositeHeader.Input IOType.Source)
                let column_1 = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
                let column_2 = CompositeColumn.create(CompositeHeader.Component (OntologyAnnotation()))
                let columns = [|
                    column_0
                    column_1
                    column_2
                    column_0
                |]
                let newTable() = table.AddColumns(columns)
                Expect.throws newTable ""
            )
            testCase "multiple ProtocolType, no cells, should throw" (fun () ->
                let table = create_table()
                let column_0 = CompositeColumn.create(CompositeHeader.Input IOType.Source)
                let column_1 = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
                let column_2 = CompositeColumn.create(CompositeHeader.ProtocolType)
                let columns = [|
                    column_0
                    column_1
                    column_2
                    column_2
                |]
                let newTable() = table.AddColumns(columns)
                Expect.throws newTable ""
            )
            testCase "multiple ProtocolREF, no cells, should throw" (fun () ->
                let table = create_table()
                let column_0 = CompositeColumn.create(CompositeHeader.Input IOType.Source)
                let column_1 = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
                let column_2 = CompositeColumn.create(CompositeHeader.ProtocolREF)
                let columns = [|
                    column_0
                    column_1
                    column_2
                    column_2
                |]
                let newTable() = table.AddColumns(columns) |> ignore
                Expect.throws newTable ""
            )
            testCase "multiple columns, no cells, index 0" (fun () ->
                let table = create_table()
                let column_0 = CompositeColumn.create(CompositeHeader.Input IOType.Source)
                let column_1 = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
                let column_2 = CompositeColumn.create(CompositeHeader.Component (OntologyAnnotation()))
                let column_3 = CompositeColumn.create(CompositeHeader.Parameter (OntologyAnnotation()))
                let columns = [|
                    column_0
                    column_1
                    column_2
                    column_3
                    column_3
                |]
                table.AddColumns(columns, 0)
                Expect.equal table.RowCount 0 "RowCount"
                Expect.equal table.ColumnCount 5 "ColumnCount"
                Expect.equal table.Headers.[0] column_0.Header "header0"
                Expect.equal table.Headers.[1] column_1.Header "header1"
                Expect.equal table.Headers.[2] column_2.Header "header2"
                Expect.equal table.Headers.[3] column_3.Header "header3"
                Expect.equal table.Headers.[4] column_3.Header "header4"
            )
        ]
        testList "To Table, no cells" [
            let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source)
            let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample)
            let column_component = CompositeColumn.create(CompositeHeader.Component (OntologyAnnotation()))
            let column_param = CompositeColumn.create(CompositeHeader.Parameter (OntologyAnnotation()))
            /// Valid TestTable with 5 columns, no cells: Input [Source] - >Output [Sample] -> Component [instrument model] -> Parameter [empty] -> Parameter [empty]
            let create_testTable() = 
                let t = ArcTable.init(TableName)
                let columns = [|
                    column_input
                    column_output
                    column_component
                    column_param
                    column_param
                |]
                t.AddColumns(columns)
                t
            testCase "ensure test table" (fun () ->
                let testTable = create_testTable() 
                Expect.equal testTable.RowCount 0 "RowCount"
                Expect.equal testTable.ColumnCount 5 "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
            )
            testCase "multiple columns" (fun () ->
                let table = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_param
                |]
                table.AddColumns(columns)
                let expected_RowCount = table.RowCount
                let expected_ColumnCount = 5 + 4
                Expect.equal table.RowCount expected_RowCount "RowCount"
                Expect.equal table.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal table.Headers.[0] column_input.Header "header0"
                Expect.equal table.Headers.[1] column_output.Header "header1"
                Expect.equal table.Headers.[2] column_component.Header "header2"
                Expect.equal table.Headers.[3] column_param.Header "header3"
                Expect.equal table.Headers.[4] column_param.Header "header4"
                Expect.equal table.Headers.[5] column_param.Header "header4"
                Expect.equal table.Headers.[6] column_param.Header "header4"
                Expect.equal table.Headers.[7] column_param.Header "header4"
                Expect.equal table.Headers.[8] column_param.Header "header4"
            )
            testCase "multiple columns, duplicate input, throws" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_input
                |]
                let newTable() = testTable.AddColumns(columns)
                Expect.throws newTable ""
            )
            testCase "multiple columns, duplicate output, throws" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_input
                |]
                let newTable() = testTable.AddColumns(columns)
                Expect.throws newTable ""
            )
            testCase "multiple columns, duplicate input, force replace" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_input
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + 3
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
                Expect.equal testTable.Headers.[6] column_param.Header "header4"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
            )
            testCase "multiple columns, duplicate output, force replace" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_input
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + 3
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
                Expect.equal testTable.Headers.[6] column_param.Header "header4"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
            )
            testCase "multiple columns, insert at index" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 0)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + 4
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[4] column_input.Header "header0"
                Expect.equal testTable.Headers.[5] column_output.Header "header1"
                Expect.equal testTable.Headers.[6] column_component.Header "header2"
                Expect.equal testTable.Headers.[7] column_param.Header "header3"
                Expect.equal testTable.Headers.[8] column_param.Header "header4"
                Expect.equal testTable.Headers.[0] column_param.Header "header4"
                Expect.equal testTable.Headers.[1] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
            )
            testCase "multiple columns, insert at index2" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 2)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + 4
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[6] column_component.Header "header2"
                Expect.equal testTable.Headers.[7] column_param.Header "header3"
                Expect.equal testTable.Headers.[8] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
            )
            testCase "multiple columns, insert at index, duplicate output, force replace" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_input
                    column_param
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 0, true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + 3
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[3] column_input.Header "header0"
                Expect.equal testTable.Headers.[4] column_output.Header "header1"
                Expect.equal testTable.Headers.[5] column_component.Header "header2"
                Expect.equal testTable.Headers.[6] column_param.Header "header3"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Headers.[0] column_param.Header "header4"
                Expect.equal testTable.Headers.[1] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
            )
            testCase "multiple columns, insert at index2, duplicate output, force replace" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_input
                    column_param
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 2, true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + 3
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[5] column_component.Header "header2"
                Expect.equal testTable.Headers.[6] column_param.Header "header3"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
            )
        ]
        testList "To Table with cells" [
            testCase "ensure test table" (fun () ->
                let testTable = create_testTable() 
                Expect.equal testTable.RowCount 5 "RowCount"
                Expect.equal testTable.ColumnCount 5 "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
            )
            testCase "multiple columns, same rowCount" (fun () ->
                let testTable = create_testTable() 
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
                Expect.equal testTable.Headers.[6] column_param.Header "header4"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Headers.[8] column_param.Header "header4"
                Expect.equal testTable.Values.[8,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 8,4"
            )
            testCase "multiple columns, same rowCount, insert at" (fun () ->
                let testTable = create_testTable()
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 0)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[4] column_input.Header "header0"
                Expect.equal testTable.Headers.[5] column_output.Header "header1"
                Expect.equal testTable.Headers.[6] column_param.Header "header2"
                Expect.equal testTable.Headers.[7] column_component.Header "header3"
                Expect.equal testTable.Headers.[8] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(4,0)) (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal (testTable.GetCellAt(8,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal testTable.Headers.[0] column_param.Header "header4"
                Expect.equal testTable.Headers.[1] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(3,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 8,4"
            )
            testCase "multiple columns, same rowCount, duplicate throw" (fun () ->
                let testTable = create_testTable()
                let columns = [|
                    column_param
                    column_param
                    column_param
                    column_input
                |]
                let newTable() = testTable.AddColumns(columns)
                Expect.throws newTable ""
            )
            testCase "multiple columns, same rowCount, duplicate replace" (fun () ->
                let testTable = create_testTable()
                let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "NEW" 5)
                let columns = [|
                    column_param
                    newInputCol
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInputCol.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(0,0)) (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal (testTable.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
                Expect.equal testTable.Headers.[6] column_param.Header "header4"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(7,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 7,4"
            )
            testCase "multiple columns, same rowCount, duplicate replace, insert at" (fun () ->
                let testTable = create_testTable()
                let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "NEW" 5)
                let columns = [|
                    column_param
                    newInputCol
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 0, true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[3] newInputCol.Header "header0"
                Expect.equal (testTable.GetCellAt(3,0)) (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Headers.[4] column_output.Header "header1"
                Expect.equal testTable.Headers.[5] column_param.Header "header2"
                Expect.equal testTable.Headers.[6] column_component.Header "header3"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(7,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal testTable.Headers.[0] column_param.Header "header4"
                Expect.equal testTable.Headers.[1] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(2,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 7,4"
            )
            testCase "multiple columns, same rowCount, duplicate replace, insert at2" (fun () ->
                let testTable = create_testTable()
                let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "NEW" 5)
                let columns = [|
                    column_param
                    newInputCol
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 2, true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInputCol.Header "header0"
                Expect.equal (testTable.GetCellAt(0,0)) (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[5] column_param.Header "header2"
                Expect.equal testTable.Headers.[6] column_component.Header "header3"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(7,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 7,4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
            )
            testCase "multiple columns, less rowCount" (fun () ->
                let testTable = create_testTable()
                let newColumn = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 2)
                let columns = [|
                    newColumn
                    newColumn
                    newColumn
                    newColumn
                |]
                testTable.AddColumns(columns)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = 5 + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(0,0)) (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal (testTable.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal testTable.Headers.[8] newColumn.Header "header8"
                Expect.equal (testTable.GetCellAt(8,4)) (CompositeCell.emptyTerm) "cell 8,4"
            )
            testCase "multiple columns, more rowCount" (fun () ->
                let testTable = create_testTable()
                let newColumn = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 8)
                let columns = [|
                    newColumn
                    newColumn
                    newColumn
                    newColumn
                |]
                testTable.AddColumns(columns)
                let expected_RowCount = newColumn.Cells.Count
                let expected_ColumnCount = 5 + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal (testTable.GetCellAt(0,0)) (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal (testTable.GetCellAt(0,7)) (CompositeCell.emptyFreeText) "cell 0,7"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal (testTable.GetCellAt(4,7)) (CompositeCell.emptyUnitized) "cell 4,7"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal testTable.Headers.[8] newColumn.Header "header8"
                Expect.equal (testTable.GetCellAt(8,7)) (newColumn.Cells.[0]) "cell 8,7"
            )
            testCase "multiple columns, more rowCount, duplicate replace" (fun () ->
                let testTable = create_testTable()
                let newInput = CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "NEW" 8)
                let newColumn = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 8)
                let columns = [|
                    newColumn
                    newInput
                    newColumn
                    newColumn
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = newColumn.Cells.Count
                let expected_ColumnCount = 5 + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInput.Header "header0"
                Expect.equal (testTable.GetCellAt(0,0)) (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal (testTable.GetCellAt(0,7)) (CompositeCell.FreeText "NEW_7") "cell 0,7"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal (testTable.GetCellAt(4,7)) (CompositeCell.emptyUnitized) "cell 4,7"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal (testTable.GetCellAt(7,7)) (newColumn.Cells.[0]) "cell 7,7"
            )
            testCase "multiple columns, different rowCount, duplicate replace" (fun () ->
                let testTable = create_testTable()
                let newInput = CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "NEW" 2)
                let newColumn = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 8)
                let columns = [|
                    newColumn
                    newInput
                    newColumn
                    newColumn
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = newColumn.Cells.Count
                let expected_ColumnCount = 5 + 4 - 1 //5 base table, 4 new, 1 replace instead of add
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInput.Header "header0"
                Expect.equal (testTable.GetCellAt(0,0)) (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal (testTable.GetCellAt(0,7)) (CompositeCell.emptyFreeText) "cell 0,7"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_param.Header "header2"
                Expect.equal testTable.Headers.[3] column_component.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal (testTable.GetCellAt(4,4)) (CompositeCell.createUnitized (string 4,OntologyAnnotation())) "cell 4,4"
                Expect.equal (testTable.GetCellAt(4,7)) (CompositeCell.emptyUnitized) "cell 4,7"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal (testTable.GetCellAt(7,7)) (newColumn.Cells.[0]) "cell 7,7"
            )
        ]
    ]


//Test for AddColumnFill
let private tests_AddColumnFill = 
    testList "AddColumnFill" [ 
        let table = create_testTable()
        let inputHeader = CompositeHeader.Parameter (OntologyAnnotation("chlamy_r"))
        let cellU = CompositeCell.createUnitizedFromString("1")
        testCase "addColumnFill to preexisting table with rows" (fun () -> 
            table.AddColumnFill(inputHeader, cellU)
            Expect.equal table.ColumnCount 6 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            let columnIndex = table.ColumnCount - 1
            for ri in 0 .. table.RowCount - 1 do
                let value = table.Values.[(columnIndex, ri)]
                Expect.equal value cellU $"Value at column {columnIndex}, row {ri} should be equal to the input cellU"
        )
        // Obsolete? I guess having one row after calling this function would be fine?
        ptestCase "addColumnFill to empty table" (fun () -> 
            let emptyTable = ArcTable.init(name = "EmptyTable")
            emptyTable.AddColumnFill(inputHeader, cellU) 
            Expect.equal emptyTable.ColumnCount 1 "ColumnCount"
            Expect.equal emptyTable.RowCount 0 "RowCount"            
            let content = emptyTable.Values 
            Expect.isEmpty content "Cannot add column to empty table"
        )
        // Obsolete? I guess having one row after calling this function would be fine?
        testCase "addColumnFill to empty table" (fun () -> 
            let emptyTable = ArcTable.init(name = "EmptyTable")
            emptyTable.AddColumnFill(inputHeader, cellU) 
            Expect.equal emptyTable.ColumnCount 1 "ColumnCount"
            Expect.equal emptyTable.RowCount 1 "RowCount"
            match IntDictionary.tryFind 0 emptyTable.Values.Columns with
            | Some (ArcTableAux.Constant _) -> 
                Expect.isTrue true ""
            | Some _ -> 
                Expect.isTrue false "Should be constant"
            | None -> 
                Expect.isTrue false "Should have a column with index 0"
        )
        ptestCase "mutability test_obsolete?" (fun () ->  
            let cell = CompositeCell.createTermFromString("1")
            table.AddColumnFill(inputHeader, cell, index = 2)
            let c = table.GetCellAt(2, 0)
            c.AsTerm.Name <- Some "2"
            table.SetCellAt(2, 0, cell)
            table.Values.[2,0].AsTerm.Name <- Some "2" 
            let changedCell = table.Values.[(2, 0)]
            let unaffectedCell = [ for ri in 1 .. table.RowCount - 1 -> table.Values.[(2, ri)] ]
            for el in unaffectedCell do
                Expect.notEqual el changedCell "Other cells should remain unchanged"
        )
        testCase "mutability test" (fun () ->  
            let cell = CompositeCell.createTermFromString("1")
            table.AddColumnFill(inputHeader, cell, index = 2)
            let c = table.GetCellAt(2, 0).Copy()
            c.AsTerm.Name <- Some "2"
            table.SetCellAt(2, 0, c)
            let changedCell = table.Values.[(2, 0)]
            let unaffectedCell = [ for ri in 1 .. table.RowCount - 1 -> table.Values.[(2, ri)] ]
            for el in unaffectedCell do
                Expect.notEqual el changedCell "Other cells should remain unchanged"
        )
        testCase "checkBoundaries" (fun () ->  
            let newTable() = table.AddColumnFill(inputHeader, cellU, index = 10)
            Expect.throws newTable "Should fail with index 10"
        )
    ]


let private tests_RemoveColumn = 
    testList "RemoveColumn" [
        testCase "ensure table" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
        )
        testCase "remove middle" (fun () ->
            let table = create_testTable()
            table.RemoveColumn(2)
            Expect.equal table.ColumnCount 4 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString ("4")) "3,4"
        )
        testCase "remove first" (fun () ->
            let table = create_testTable()
            table.RemoveColumn(0)
            Expect.equal table.ColumnCount 4 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Sample_0") "0,0"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString ("4")) "3,4"
        )
        testCase "remove last" (fun () ->
            let table = create_testTable()
            table.RemoveColumn(table.ColumnCount-1)
            Expect.equal table.ColumnCount 4 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createTerm oa_SCIEXInstrumentModel) "3,4"
        )
    ]

let private tests_RemoveColumns = 
    testList "RemoveColumns" [
        testCase "ensure table" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
        )
        testCase "remove middle" (fun () ->
            let table = create_testTable()
            table.RemoveColumns([|1;2|])
            Expect.equal table.ColumnCount 3 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString ("4")) "table.ColumnCount-1,table.RowCount-1"
        )
        testCase "remove first 2" (fun () ->
            let table = create_testTable()
            table.RemoveColumns([|0;1|])
            Expect.equal table.ColumnCount 3 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitized ("4", OntologyAnnotation())) "table.ColumnCount-1,table.RowCount-1"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString ("4")) "3,4"
        )
        testCase "remove last 2" (fun () ->
            let table = create_testTable()
            table.RemoveColumns([|3;4|])
            Expect.equal table.ColumnCount 3 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitized ("4", OntologyAnnotation())) "table.ColumnCount-1,table.RowCount-1"
        )
        // if indices are NOT managed correctly, the first one will be removed shifting column index -1. 
        // This will result in an raised error trying to remove the last column, from an index which does not exist anymore
        testCase "remove head first then last" (fun () ->
            let table = create_testTable()
            table.RemoveColumns([|0;table.ColumnCount-1|])
            Expect.equal table.ColumnCount 3 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Sample_0") "0,0"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createTerm oa_SCIEXInstrumentModel) "table.ColumnCount-1,table.RowCount-1"
        )
    ]

let private tests_TryGetColumnByHeaderBy =
    testList "TryGetColumnByHeaderBy" [                 
        testCase "Empty column" (fun () ->
            let emptyTable = ArcTable.init("empty table")
            let column_species = CompositeColumn.create(CompositeHeader.Characteristic oa_species)
            emptyTable.AddColumns[|column_species|]
            let colOption = emptyTable.TryGetColumnByHeaderBy (fun (header:CompositeHeader) -> 
                match header with 
                | CompositeHeader.Characteristic oa -> oa = oa_species
                | _ -> false )        
            let col = Expect.wantSome colOption "should have found col but returned None"
            // Expect.sequenceEqual col.Cells column_component.Cells "cells did not match"
            Expect.hasLength col.Cells 0 "cells did not match"
        )
        testCase "Column with values" (fun () ->
            let table = create_testTable()
            let column_species = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 8)
            table.AddColumns[|column_species|]
            let colOption = table.TryGetColumnByHeaderBy (fun (header:CompositeHeader) -> 
                match header with
                | CompositeHeader.Characteristic oa -> oa = oa_species
                | _ -> false )
            let col = Expect.wantSome colOption "should have found col but returned None"
            // Expect.hasLength col.Cells 8 "cells did not match"
            Expect.sequenceEqual col.Cells column_species.Cells "cells did not match"
        )
        testCase "Non-existing column" (fun () -> 
            let table = create_testTable()
            let colOption = table.TryGetColumnByHeaderBy (fun (header:CompositeHeader) -> 
                match header with 
                | CompositeHeader.Parameter oa -> oa = oa_temperature
                | _ -> false ) 
            Expect.isNone colOption "fails to find col therefore returns none"
        )
    ]


let private tests_MoveColumn = 
    testList "MoveColumn" [
        testCase "CheckBoundaries" (fun () ->
            let table = create_testTable()
            let startTooLow() = table.MoveColumn(-1, 1)
            let startTooHigh() = table.MoveColumn(table.ColumnCount, 1)
            let endTooLow() = table.MoveColumn(1, -1)
            let endTooHigh() = table.MoveColumn(1, table.ColumnCount)
            Expect.throws startTooLow "Should fail for start Index -1"
            Expect.throws startTooHigh "Should fail for start Index ColumnCount"
            Expect.throws endTooLow "Should fail for end Index -1"
            Expect.throws endTooHigh "Should fail for end Index ColumnCount"
        )
        testCase "DoesNothingForSameIndex" (fun () ->
            let table = create_testTable()
            table.MoveColumn(1, 1)
            let expected = create_testTable()
            Expect.arcTableEqual table expected "Table should not have changed"
        )
        testCase "MoveStartToEnd" (fun () ->
            let table = create_testTable()
            table.MoveColumn(0, 4)
            let expected = 
                let t = ArcTable.init(TableName)
                let columns = [|
                    column_output
                    column_param
                    column_component
                    column_param
                    column_input
                |]
                t.AddColumns(columns)
                t
            Expect.arcTableEqual table expected "Table should have moved column 0 to 4"
        )
        testCase "MoveEndToStart" (fun () ->
            let table = create_testTable()
            table.MoveColumn(4, 0)
            let expected = 
                let t = ArcTable.init(TableName)
                let columns = [|
                    column_param
                    column_input
                    column_output
                    column_param
                    column_component
                |]
                t.AddColumns(columns)
                t
            Expect.arcTableEqual table expected "Table should have moved column 4 to 0"
        )
        testCase "MoveOneAheadInMiddle" (fun () ->
            let table = create_testTable()
            table.MoveColumn(1, 2)
            let expected = 
                let t = ArcTable.init(TableName)
                let columns = [|
                    column_input
                    column_param
                    column_output
                    column_component
                    column_param
                |]
                t.AddColumns(columns)
                t
            Expect.arcTableEqual table expected "Table should have moved column 1 to 2"
        )
    ]


let private tests_RemoveRow = 
    testList "RemoveRow" [
        testCase "ensure table" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
        )
        testCase "remove middle" (fun () ->
            let table = create_testTable()
            table.RemoveRow(2)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 4 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_1") "0,1"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_3") "0,2"
            Expect.equal table.Values.[(0,3)] (CompositeCell.createFreeText "Source_4") "0,3"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString ("4")) "4,3"
        )
        testCase "remove first" (fun () ->
            let table = create_testTable()
            table.RemoveRow(0)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 4 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_1") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_2") "0,1"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_3") "0,2"
            Expect.equal table.Values.[(0,3)] (CompositeCell.createFreeText "Source_4") "0,3"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString "4") "4,3"
        )
        testCase "remove last" (fun () ->
            let table = create_testTable()
            table.RemoveRow(table.RowCount-1)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 4 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_1") "0,1"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_2") "0,2"
            Expect.equal table.Values.[(0,3)] (CompositeCell.createFreeText "Source_3") "0,3"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString "3") "4,3"
        )
    ]

let private tests_RemoveRows = 
    testList "RemoveRows" [
        testCase "ensure table" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
        )
        testCase "remove middle" (fun () ->
            let table = create_testTable()
            table.RemoveRows([|1;2|])
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 3 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_3") "0,2"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_4") "0,3"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString ("4")) "4,2"
        )
        testCase "remove first 2" (fun () ->
            let table = create_testTable()
            table.RemoveRows([|0;1|])
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 3 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_2") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_3") "0,1"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_4") "0,2"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString "4") "4,2"
        )
        testCase "remove last 2" (fun () ->
            let table = create_testTable()
            table.RemoveRows([|3;4|])
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 3 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_0") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_1") "0,1"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_2") "0,2"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString "2") "4,2"
        )
        testCase "remove head first then last" (fun () ->
            let table = create_testTable()
            table.RemoveRows([|0;table.RowCount-1|])
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 3 "RowCount"
            Expect.equal table.Values.[(0,0)] (CompositeCell.createFreeText "Source_1") "0,0"
            Expect.equal table.Values.[(0,1)] (CompositeCell.createFreeText "Source_2") "0,1"
            Expect.equal table.Values.[(0,2)] (CompositeCell.createFreeText "Source_3") "0,2"
            Expect.equal table.Values.[(table.ColumnCount-1,table.RowCount-1)] (CompositeCell.createUnitizedFromString "3") "4,2"
        )
    ]

let private tests_AddRow = 
    testList "AddRow" [
        testCase "ensure table" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
        )
        let row_default = ResizeArray [|
            CompositeCell.FreeText "NewSource"
            CompositeCell.FreeText "NewSample"
            CompositeCell.createTerm (OntologyAnnotation())
            CompositeCell.createTerm oa_SCIEXInstrumentModel
            CompositeCell.createTerm (OntologyAnnotation())
        |]
        let row_empty = ResizeArray [|
            CompositeCell.emptyFreeText
            CompositeCell.emptyFreeText
            CompositeCell.emptyUnitized
            CompositeCell.emptyTerm
            CompositeCell.emptyUnitized
        |]
        let row_wrong = ResizeArray.rev row_default
        testCase "append row, empty table, throw" (fun () ->
            let table = ArcTable.init(TableName)
            let eval() = table.AddRow(row_default)
            Expect.throws eval ""
        )
        testCase "append row, less cols, throw" (fun () ->
            let table = create_testTable()
            let row = ResizeArray.take 4 row_default
            let eval() = table.AddRow(row)
            Expect.isTrue (row.Count < table.RowCount) "must be less than"
            Expect.throws eval ""
        )
        testCase "append row, more cols, throw" (fun () ->
            let table = create_testTable()
            let row = ResizeArray.append row_default row_default
            let eval() = table.AddRow(row)
            Expect.isTrue (row.Count > table.RowCount) "must be more than"
            Expect.throws eval ""
        )
        testCase "append row, wrong cells for column, throw" (fun () ->
            let table = create_testTable()
            let eval() = table.AddRow(row_wrong)
            Expect.equal table.ColumnCount row_wrong.Count "This MUST be correct"
            Expect.throws eval ""
        )
        testCase "append row" (fun () ->
            let table = create_testTable()
            let index = 5
            table.AddRow(row_default)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 6 "RowCount"
            let newTable = create_testTable()
            // Test full table
            for columnIndex in 0 .. (table.ColumnCount-1) do
                for rowIndex in 0 .. (table.RowCount-1) do
                    if rowIndex < index then
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex] $"Cell,rowIndex < index, {columnIndex},{rowIndex}"
                    elif rowIndex = index then
                        Expect.equal table.Values.[columnIndex, rowIndex] row_default.[columnIndex] $"Cell,rowIndex = index, {columnIndex},{rowIndex}"
                    else
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex-1] $"Cell {columnIndex},{rowIndex}"
        )
        testCase "insert row" (fun () ->
            let table = create_testTable()
            let index = 3
            table.AddRow(row_default, index)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 6 "RowCount"
            let newTable = create_testTable()
            // Test full table
            for columnIndex in 0 .. (table.ColumnCount-1) do
                for rowIndex in 0 .. (table.RowCount-1) do
                    if rowIndex < index then
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex] $"Cell,rowIndex < index, {columnIndex},{rowIndex}"
                    elif rowIndex = index then
                        Expect.equal table.Values.[columnIndex, rowIndex] row_default.[columnIndex] $"Cell,rowIndex = index, {columnIndex},{rowIndex}"
                    else
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex-1] $"Cell {columnIndex},{rowIndex}"
        )
        testCase "append empty row" (fun () ->
            let table = create_testTable()
            let index = 5
            table.AddRow()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 6 "RowCount"
            let newTable = create_testTable()
            // Test full table
            for columnIndex in 0 .. (table.ColumnCount-1) do
                for rowIndex in 0 .. (table.RowCount-1) do
                    if rowIndex < index then
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex] $"Cell,rowIndex < index, {columnIndex},{rowIndex}"
                    elif rowIndex = index then
                        // The last row is empty, except when the column is constant
                        if columnIndex = 3 then
                            let cell = Expect.wantSome (table.TryGetCellAt(columnIndex, rowIndex)) $"Last row ({columnIndex}, {rowIndex}) should not be empty for constant column"
                            Expect.equal cell (CompositeCell.createTerm oa_SCIEXInstrumentModel) $"Last row ({columnIndex}, {rowIndex}) should be correct for constant column"
                        else
                            Expect.isNone (table.TryGetCellAt(columnIndex, rowIndex)) $"Last row ({columnIndex}, {rowIndex}) should be empty"
                    else
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex-1] $"Cell {columnIndex},{rowIndex}"
        )
        testCase "insert empty row" (fun () ->
            let table = create_testTable()
            let index = 3
            table.AddRow(index=index)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 6 "RowCount"
            let newTable = create_testTable()
            // Test full table
            for columnIndex in 0 .. (table.ColumnCount-1) do
                for rowIndex in 0 .. (table.RowCount-1) do
                    if rowIndex < index then
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex] $"Cell,rowIndex < index, {columnIndex},{rowIndex}"
                    elif rowIndex = index then
                        // The third row is empty, except when the column is constant
                        if columnIndex = 3 then
                            let cell = Expect.wantSome (table.TryGetCellAt(columnIndex, rowIndex)) $"Last row ({columnIndex}, {rowIndex}) should not be empty for constant column"
                            Expect.equal cell (CompositeCell.createTerm oa_SCIEXInstrumentModel) $"Last row ({columnIndex}, {rowIndex}) should be correct for constant column"
                        else
                            Expect.isNone (table.TryGetCellAt(columnIndex, rowIndex)) $"Last row ({columnIndex}, {rowIndex}) should be empty"
                    else
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex-1] $"Cell {columnIndex},{rowIndex}"
        )
        testCase "row to empty table, constant" (fun () ->
            let headers = ResizeArray [|
                CompositeHeader.Input IOType.Sample
                CompositeHeader.Output IOType.Data
                CompositeHeader.Parameter oa_temperature
                CompositeHeader.Characteristic oa_species
                CompositeHeader.Parameter oa_SCIEXInstrumentModel
            |]
            let table = ArcTable(TableName, headers = headers)
            table.AddRow(row_default)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 1 "RowCount"
            let c1 = Expect.wantSome (IntDictionary.tryFind 0 table.Values.Columns) "Column 0 should be present"
            match c1 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 0 should be constant"
            | _ -> Expect.isTrue false "Column 0 should be constant"
            let c2 = Expect.wantSome (IntDictionary.tryFind 1 table.Values.Columns) "Column 1 should be present"
            match c2 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 1 should be constant"
            | _ -> Expect.isTrue false "Column 1 should be constant"
            let c3 = Expect.wantSome (IntDictionary.tryFind 2 table.Values.Columns) "Column 2 should be present"
            match c3 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 2 should be constant"
            | _ -> Expect.isTrue false "Column 2 should be constant"
            let c4 = Expect.wantSome (IntDictionary.tryFind 3 table.Values.Columns) "Column 3 should be present"
            match c4 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 3 should be constant"
            | _ -> Expect.isTrue false "Column 3 should be constant"
            let c5 = Expect.wantSome (IntDictionary.tryFind 4 table.Values.Columns) "Column 4 should be present"
            match c5 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 4 should be constant"
            | _ -> Expect.isTrue false "Column 4 should be constant"
        )
        testCase "same row to table, constant" (fun () ->
            let headers = ResizeArray [|
                CompositeHeader.Input IOType.Sample
                CompositeHeader.Output IOType.Data
                CompositeHeader.Parameter oa_temperature
                CompositeHeader.Characteristic oa_species
                CompositeHeader.Parameter oa_SCIEXInstrumentModel
            |]
            let table = ArcTable(TableName, headers = headers)
            table.AddRow(row_default)
            table.AddRow(row_default)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 2 "RowCount"
            let c1 = Expect.wantSome (IntDictionary.tryFind 0 table.Values.Columns) "Column 0 should be present"
            match c1 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 0 should be constant"
            | _ -> Expect.isTrue false "Column 0 should be constant"
            let c2 = Expect.wantSome (IntDictionary.tryFind 1 table.Values.Columns) "Column 1 should be present"
            match c2 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 1 should be constant"
            | _ -> Expect.isTrue false "Column 1 should be constant"
            let c3 = Expect.wantSome (IntDictionary.tryFind 2 table.Values.Columns) "Column 2 should be present"
            match c3 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 2 should be constant"
            | _ -> Expect.isTrue false "Column 2 should be constant"
            let c4 = Expect.wantSome (IntDictionary.tryFind 3 table.Values.Columns) "Column 3 should be present"
            match c4 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 3 should be constant"
            | _ -> Expect.isTrue false "Column 3 should be constant"
            let c5 = Expect.wantSome (IntDictionary.tryFind 4 table.Values.Columns) "Column 4 should be present"
            match c5 with
            | ArcTableAux.Constant _ -> Expect.isTrue true "Column 4 should be constant"
            | _ -> Expect.isTrue false "Column 4 should be constant"
        )
    ]

let private tests_AddRows = 
    testList "AddRows" [
        testCase "ensure table" (fun () ->
            let table = create_testTable()
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount 5 "RowCount"
        )
        let row_default prefix = ResizeArray [|
            CompositeCell.FreeText $"{prefix}Source"
            CompositeCell.FreeText $"{prefix}Sample"
            CompositeCell.createTerm (OntologyAnnotation())
            CompositeCell.createTerm oa_SCIEXInstrumentModel
            CompositeCell.createTerm (OntologyAnnotation())
        |]
        let rows_default = ResizeArray [|
            row_default "Nice"
            row_default "Awesome"
            row_default "Wonderful"
        |]
        testCase "append row, empty table, throw" (fun () ->
            let table = ArcTable.init(TableName)
            let eval() = table.AddRows(rows_default)
            Expect.throws eval ""
        )
        testCase "append row, less cols, throw" (fun () ->
            let table = create_testTable()
            let rows = rows_default |> ResizeArray.mapi (fun i arr -> if i = 0 then ResizeArray.take 3 arr else arr)
            let eval() = table.AddRows(rows)
            Expect.isTrue (rows.[0].Count < table.RowCount) "must be less than"
            Expect.throws eval ""
        )
        testCase "append row, more cols, throw" (fun () ->
            let table = create_testTable()
            let rows = rows_default |> ResizeArray.mapi (fun i arr -> if i = 0 then ResizeArray.append arr arr else arr)
            let eval() = table.AddRows(rows)
            Expect.isTrue (rows.[0].Count > table.RowCount) "must be more than"
            Expect.throws eval ""
        )
        testCase "append row, wrong cells for column, throw" (fun () ->
            let table = create_testTable()
            let rows = rows_default |> ResizeArray.mapi (fun i arr -> if i = 0 then ResizeArray.rev arr else arr)
            let eval() = table.AddRows(rows)
            Expect.equal table.ColumnCount rows.[0].Count "This MUST be correct"
            Expect.throws eval ""
        )
        testCase "append rows" (fun () ->
            let table = create_testTable()
            let index = table.RowCount
            table.AddRows(rows_default)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount (5+rows_default.Count) "RowCount"
            let newTable = create_testTable()
            let newColumnCount = rows_default.Count
            // Test full table
            for columnIndex in 0 .. (table.ColumnCount-1) do
                for rowIndex in 0 .. (table.RowCount-1) do
                    if rowIndex < index then
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex] $"Cell,rowIndex < index, {columnIndex},{rowIndex}"
                    elif rowIndex >= index && rowIndex < index + newColumnCount then
                        Expect.equal table.Values.[columnIndex, rowIndex] rows_default.[rowIndex-index].[columnIndex] $"Cell,rowIndex >= index && rowIndex < index + newColumnCount, {columnIndex},{rowIndex}"
                    else
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex-newColumnCount] $"Cell {columnIndex},{rowIndex}"
        )
        testCase "insert rows" (fun () ->
            let table = create_testTable()
            let index = 3
            table.AddRows(rows_default, index)
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.RowCount (5+rows_default.Count) "RowCount"
            let newTable = create_testTable()
            let newColumnCount = rows_default.Count
            // Test full table
            for columnIndex in 0 .. (table.ColumnCount-1) do
                for rowIndex in 0 .. (table.RowCount-1) do
                    if rowIndex < index then
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex] $"Cell,rowIndex < index, {columnIndex},{rowIndex}"
                    elif rowIndex >= index && rowIndex < index + newColumnCount then
                        Expect.equal table.Values.[columnIndex, rowIndex] rows_default.[rowIndex-index].[columnIndex] $"Cell,rowIndex >= index && rowIndex < index + newColumnCount, {columnIndex},{rowIndex}"
                    else
                        Expect.equal table.Values.[columnIndex, rowIndex] newTable.Values.[columnIndex, rowIndex-newColumnCount] $"Cell {columnIndex},{rowIndex}"
        )
    ]

let private tests_UpdateRefWithSheet =
    testList "tests_UpdateRefWithSheet" [
        testCase "singleREFAndDescription" (fun () ->
            let protocolREF = "MyProtocol"
            let protocolDescription = "MyProtocolDescription"
            let refTable = ArcTable.init("Table")
            refTable.AddProtocolNameColumn (ResizeArray [|protocolREF|])
            refTable.AddProtocolDescriptionColumn (ResizeArray [|protocolDescription|])
            let valueTable = ArcTable.init("Table")
            let columns = [|
                column_input
                column_output
                column_component
            |]            
            valueTable.AddColumns(columns)
            let expectedRowCount = valueTable.RowCount
            let expectedColumnCount = 5
            let refTable = ArcTable.updateReferenceByAnnotationTable refTable valueTable

            Expect.equal valueTable.ColumnCount 3 "ColumnCount of value table should not change after update"

            Expect.equal refTable.RowCount expectedRowCount "RowCount of reference table should be the same as value table after update"
            Expect.equal refTable.ColumnCount expectedColumnCount "ColumnCount of reference table should be the sum of value table and protocol table after update"
            
            TestingUtils.Expect.sequenceEqual 
                (refTable.GetProtocolDescriptionColumn().Cells)
                (Array.create 5 (CompositeCell.createFreeText protocolDescription))
                "ProtocolDescriptionColumn should be filled with protocol description"
            TestingUtils.Expect.sequenceEqual
                (refTable.GetColumnByHeader column_component.Header).Cells
                column_component.Cells
                "Component column should have been taken as is"
        )
        testCase "OverwriteDescription" (fun () ->
            let protocolREF = "MyProtocol"
            let protocolDescription = "MyProtocolDescription"
            let newProtocolDescription = "Improved ProtocolDescription"
            let refTable = ArcTable.init("Table")
            refTable.AddProtocolNameColumn (ResizeArray [|protocolREF|])
            refTable.AddProtocolDescriptionColumn (ResizeArray [|protocolDescription|])
            let valueTable = ArcTable.init("Table")
            let columns = [|
                column_input
                column_output
                column_component
            |]            
            valueTable.AddColumns(columns)
            valueTable.AddProtocolDescriptionColumn (ResizeArray.create 5 newProtocolDescription)
            let expectedRowCount = valueTable.RowCount
            let expectedColumnCount = 5
            let refTable = ArcTable.updateReferenceByAnnotationTable refTable valueTable

            Expect.equal valueTable.ColumnCount 4 "ColumnCount of value table should not change after update"

            Expect.equal refTable.RowCount expectedRowCount "RowCount of reference table should be the same as value table after update"
            Expect.equal refTable.ColumnCount expectedColumnCount "ColumnCount of reference table should be the sum of value table and protocol table after update"
            
            TestingUtils.Expect.sequenceEqual 
                (refTable.GetProtocolDescriptionColumn().Cells)
                (Array.create 5 (CompositeCell.createFreeText newProtocolDescription))
                "ProtocolDescriptionColumn should be filled with protocol description"
            TestingUtils.Expect.sequenceEqual
                (refTable.GetColumnByHeader column_component.Header).Cells
                column_component.Cells
                "Component column should have been taken as is"     
        )
        testCase "DropParams" (fun () ->
            let protocolREF = "MyProtocol"
            let protocolDescription = "MyProtocolDescription"
            let refTable = ArcTable.init("Table")
            refTable.AddProtocolNameColumn (ResizeArray [|protocolREF|])
            refTable.AddProtocolDescriptionColumn (ResizeArray [|protocolDescription|])
            refTable.AddColumn(CompositeHeader.Parameter oa_species, ResizeArray [|CompositeCell.createTerm oa_chlamy|])
            let valueTable = ArcTable.init("Table")
            let columns = [|
                column_input
                column_output
                column_component
            |]            
            valueTable.AddColumns(columns)
            let expectedRowCount = valueTable.RowCount
            let expectedColumnCount = 5
            let refTable = ArcTable.updateReferenceByAnnotationTable refTable valueTable

            Expect.equal refTable.RowCount expectedRowCount "RowCount of reference table should be the same as value table after update"
            Expect.equal refTable.ColumnCount expectedColumnCount "ColumnCount of reference table should be the sum of value table and protocol table after update minus the param columns"
            
        )
    ]

let private tests_Join = testList "Join" [
    testList "index" [
        testCase "ensure default is append" <| fun _ ->
            let table1 = create_testTable()
            let columnCount = table1.ColumnCount
            let table2 = create_testTable()
            table2.RemoveColumn 0 // rmv input
            table2.RemoveColumn 0 // rmv output
            let expectedJoinCount = columnCount + table2.ColumnCount
            table1.Join(table2)
            Expect.equal table1.ColumnCount expectedJoinCount "new column count"
            Expect.equal (table1.Headers.[columnCount]) (table2.Headers.[0]) "column equal"
        testCase "ensure -1 defaults to append" <| fun _ ->
            let table1 = create_testTable()
            let columnCount = table1.ColumnCount
            let table2 = create_testTable()
            table2.RemoveColumn 0 // rmv input
            table2.RemoveColumn 0 // rmv output
            let expectedJoinCount = columnCount + table2.ColumnCount
            table1.Join(table2,-1)
            Expect.equal table1.ColumnCount expectedJoinCount "new column count"
            Expect.equal (table1.Headers.[columnCount]) (table2.Headers.[0]) "column equal"
        testCase "insert at start" <| fun _ ->
            let table1 = create_testTable()
            let columnCount = table1.ColumnCount
            let table2 = create_testTable()
            table2.RemoveColumn 0 // rmv input
            table2.RemoveColumn 0 // rmv output
            let expectedJoinCount = columnCount + table2.ColumnCount
            table1.Join(table2,0)
            Expect.equal table1.ColumnCount expectedJoinCount "new column count"
            Expect.equal (table1.Headers.[0]) (table2.Headers.[0]) "column equal"
    ]
    testList "TableJoinOption.Headers" [
        testCase "Add to empty" <| fun _ ->
            let table = ArcTable.init("MyTable")
            let joinTable = create_testTable()
            table.Join(joinTable,-1,TableJoinOptions.Headers)
            Expect.equal table.ColumnCount 5 "columnCount"
            // test headers
            Expect.equal table.Headers.[0] (CompositeHeader.Input IOType.Source) "Header input"
            Expect.equal table.Headers.[1] (CompositeHeader.Output IOType.Sample) "Header output"
            Expect.equal table.Headers.[2] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty]"
            Expect.equal table.Headers.[3] (CompositeHeader.Component oa_instrumentModel) "Header component [instrument model]"
            Expect.equal table.Headers.[4] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty] 2"
            // test rows
            Expect.equal table.RowCount 0 "rowcount"
        testCase "Add to duplicate" <| fun _ ->
            let table = create_testTable()
            let joinTable = create_testTable()
            let func = fun () -> table.Join(joinTable,-1,TableJoinOptions.Headers)
            Expect.throws func "This should fail as we try to add multiple inputs/outputs to one table"
        testCase "Add to duplicate, forceReplace" <| fun _ ->
            let table = create_testTable()
            let joinTable = create_testTable()
            table.Join(joinTable,-1,TableJoinOptions.Headers,true)
            Expect.equal table.ColumnCount 8 "We expect 8 columns as there are 5 per table with 2 unique 5 + (5-2) = 8"
            // headers
            Expect.equal table.Headers.[0] (CompositeHeader.Input IOType.Source) "Header input"
            Expect.equal table.Headers.[1] (CompositeHeader.Output IOType.Sample) "Header output"
            Expect.equal table.Headers.[2] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty]"
            Expect.equal table.Headers.[3] (CompositeHeader.Component oa_instrumentModel) "Header component [instrument model]"
            Expect.equal table.Headers.[4] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty] 2"
            Expect.equal table.Headers.[2] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty] 3"
            Expect.equal table.Headers.[3] (CompositeHeader.Component oa_instrumentModel) "Header component [instrument model] 2"
            Expect.equal table.Headers.[4] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty] 4"
            // rows should be untouched
            Expect.equal table.RowCount 5 "rows should be untouched"
        testCase "Join replace input" <| fun _ ->
            let table = create_testTable()
            let joinTable = ArcTable.create(
                "jointable",
                ResizeArray([CompositeHeader.Input IOType.Data]),
                ResizeArray()
            )
            table.Join(joinTable,-1,TableJoinOptions.Headers,true)
            Expect.equal table.ColumnCount 5 "columnCount"
            // test headers
            Expect.equal table.Headers.[0] (CompositeHeader.Input IOType.Data) "Here should be new image input"
            Expect.equal table.Headers.[1] (CompositeHeader.Output IOType.Sample) "Header output"
            Expect.equal table.Headers.[2] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty]"
            Expect.equal table.Headers.[3] (CompositeHeader.Component oa_instrumentModel) "Header component [instrument model]"
            Expect.equal table.Headers.[4] (CompositeHeader.Parameter (OntologyAnnotation())) "Header parameter [empty] 2"
            Expect.equal (table.TryGetCellAt(0,4).Value) (CompositeCell.createFreeText "Source_4") "Input cell should be unchanged, only header should be changed"
    ]
    testList "TableJoinOption.WithUnits" [ 
        testCase "Add to empty, no unit" <| fun _ ->
            let table = ArcTable.init "MyTable"
            let joinTable = ArcTable.init("jointable")
            let columns = [|
                //only term no unit
                column_component 
            |]
            joinTable.AddColumns(columns)
            table.Join(joinTable,-1,TableJoinOptions.WithUnit)
            Expect.equal table.ColumnCount 1 "column count"
            Expect.equal table.RowCount 0 "row count"
        testCase "Add to empty, with unit" <| fun _ ->
            let table = ArcTable.init "MyTable"
            let joinTable = ArcTable.init("jointable")
            let columns = [|
                //only term no unit
                column_component 
                // with unit
                CompositeColumn.create( 
                    CompositeHeader.Parameter oa_temperature,
                    ResizeArray [|for i in 0 .. 4 do yield CompositeCell.createUnitized($"{i}",oa_degreeCelsius)|]
                )
            |]
            joinTable.AddColumns(columns)
            table.Join(joinTable,-1,TableJoinOptions.WithUnit)
            Expect.equal table.ColumnCount 2 "column count"
            Expect.equal table.RowCount 5 "row count"
            Expect.equal (table.GetCellAt(0,0)) (CompositeCell.createTerm (OntologyAnnotation())) "empty term cell"
            Expect.equal (table.GetCellAt(1,0)) (CompositeCell.createUnitized("",oa_degreeCelsius)) "temperature unit cell"
            // https://github.com/nfdi4plants/ARCtrl/issues/563
            table.Columns |> ignore
    ]
    testList "TableJoinOption.WithValues" [
        testCase "Add to empty" <| fun _ ->
            let table = ArcTable.init "MyTable"
            let joinTable = ArcTable.init("jointable")
            let columns = [|
                //only term no unit
                column_component 
                // with unit
                CompositeColumn.create( 
                    CompositeHeader.Parameter oa_temperature,
                    ResizeArray [|for i in 0 .. 4 do yield CompositeCell.createUnitized($"{i}",oa_degreeCelsius)|]
                )
            |]
            joinTable.AddColumns(columns)
            table.Join(joinTable,-1,TableJoinOptions.WithValues)
            Expect.equal table.ColumnCount 2 "column count"
            Expect.equal table.RowCount 5 "row count"
            Expect.equal table.Values.[0,0] (CompositeCell.createTerm oa_SCIEXInstrumentModel) "sciex instrument model"
            Expect.equal table.Values.[1,0] (CompositeCell.createUnitized("0",oa_degreeCelsius)) "temperature unit cell"
    ]
]

let private tests_IterColumns = testList "IterColumns" [
    testCase "Replace input column header" <| fun _ ->
        let table = create_testTable()
        let expected = CompositeHeader.Input IOType.Data
        table.IteriColumns (fun i c ->
            if c.Header.isInput then
                table.UpdateHeader(i,expected)
        )
        Expect.equal table.Columns.[0].Header expected ""
]

let private tests_equality = testList "equality" [
    testList "override equality" [
        testCase "equal" <| fun _ ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            Expect.equal table1 table2 "equal"
        testCase "not equal" <| fun _ ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            table2 |> IdentifierSetters.setArcTableName "A New Name" |> ignore
            Expect.notEqual table1 table2 "not equal"
    ]
    testList "structural equality" [
        testCase "equal" <| fun _ ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            let equals = table1.StructurallyEquals(table2)
            Expect.isTrue equals "equal"
        testCase "not equal" <| fun _ ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            table2 |> IdentifierSetters.setArcTableName "A New Name" |> ignore
            let equals = table1.StructurallyEquals(table2)
            Expect.isFalse equals "not equal"
    ]
    testList "reference equality" [
        testCase "not same object" <| fun _ ->
            let table1 = create_testTable()
            let table2 = create_testTable()
            let equals = table1.ReferenceEquals(table2)
            Expect.isFalse equals ""
        testCase "same object" <| fun _ ->
            let table1 = create_testTable()
            let equals = table1.ReferenceEquals(table1)
            Expect.isTrue equals ""
    ]
]

let private tests_copy = testList "Copy" [
    testCase "DefaultEquality"<| fun _ ->
        let table = create_testTable()
        let copy = table.Copy()
        Expect.equal table copy "Tables should be equal"
    testCase "StructuralEquality" <| fun _ ->
        let table = create_testTable()
        let copy = table.Copy()
        let equals = table.StructurallyEquals(copy)
        Expect.isTrue equals "Structural equality should be true"
    testCase "ReferenceEquality" <| fun _ ->
        let table = create_testTable()
        let copy = table.Copy()
        let equals = table.ReferenceEquals(copy)
        Expect.isFalse equals "Reference equality should be false"
    testCase "UpdateInputHeader" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(CompositeHeader.Input IOType.Sample)
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        copy.UpdateHeader(0,CompositeHeader.Input IOType.Data)
        Expect.equal (table.Headers.[0]) (CompositeHeader.Input IOType.Sample) "Header of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateParameterHeader" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("OldAnnotation")))
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        copy.UpdateHeader(0,CompositeHeader.Parameter (OntologyAnnotation("NewAnnotation")))
        Expect.equal (table.Headers.[0]) (CompositeHeader.Parameter (OntologyAnnotation("OldAnnotation"))) "Header of old table should stay as is"
        Expect.notEqual copy table "Should not be equal affter change"
    testCase "UpdateAnnotationOfParameterHeader" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("OldAnnotation")))
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        match copy.Headers.[0] with
        | CompositeHeader.Parameter oa -> oa.Name <- Some "NewAnnotation"
        | _ -> Expect.isTrue false "Header not parameter?"
        Expect.equal (table.Headers.[0]) (CompositeHeader.Parameter (OntologyAnnotation("OldAnnotation"))) "Header of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateFreetextCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Input IOType.Sample,
            ResizeArray [|CompositeCell.FreeText "OldFreetext"|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        copy.SetCellAt(0,0,CompositeCell.FreeText "NewFreetext")
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.FreeText "OldFreetext") "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateDataCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Input IOType.Data,
            ResizeArray [|CompositeCell.Data (Data(name = "OldData"))|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        copy.SetCellAt(0,0,CompositeCell.Data (Data(name = "NewData")))
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.Data (Data(name = "OldData"))) "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateNameOfDataCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Input IOType.Data,
            ResizeArray [|CompositeCell.Data (Data(name = "OldData"))|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        match copy.GetCellAt(0,0) with
        | CompositeCell.Data d -> d.Name <- Some "NewData"
        | _ -> Expect.isTrue false "Cell not data?"
        copy.RescanValueMap()
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.Data (Data(name = "OldData"))) "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateTermCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Parameter (OntologyAnnotation("MyParameter")),
            ResizeArray [|CompositeCell.createTerm (OntologyAnnotation("OldAnnotation"))|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        copy.SetCellAt(0,0,CompositeCell.createTerm (OntologyAnnotation("NewAnnotation")))
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.createTerm (OntologyAnnotation("OldAnnotation"))) "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateAnnotationOfTermCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Parameter (OntologyAnnotation("MyParameter")),
            ResizeArray [|CompositeCell.createTerm (OntologyAnnotation("OldAnnotation"))|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        match copy.GetCellAt(0,0) with
        | CompositeCell.Term oa -> oa.Name <- Some "NewAnnotation"
        | _ -> Expect.isTrue false "Cell not term?"
        copy.RescanValueMap()
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.createTerm (OntologyAnnotation("OldAnnotation"))) "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateUnitCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Parameter (OntologyAnnotation("MyParameter")),
            ResizeArray [|CompositeCell.createUnitized("OldValue",(OntologyAnnotation("OldAnnotation")))|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        copy.SetCellAt(0,0,CompositeCell.createUnitized("NewValue",(OntologyAnnotation("NewAnnotation"))))        
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.createUnitized("OldValue",(OntologyAnnotation("OldAnnotation")))) "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
    testCase "UpdateAnnotationOfUnitCell" <| fun _ ->
        let table = ArcTable.init("NewTable")
        table.AddColumn(
            CompositeHeader.Parameter (OntologyAnnotation("MyParameter")),
            ResizeArray [|CompositeCell.createUnitized("OldValue",(OntologyAnnotation("OldAnnotation")))|]
            )
        let copy = table.Copy()
        Expect.equal copy table "Should be equal before change"
        match copy.GetCellAt(0,0) with
        | CompositeCell.Unitized (v,oa) -> oa.Name <- Some "NewAnnotation"
        | _ -> Expect.isTrue false "Cell not unit?"
        copy.RescanValueMap()
        Expect.equal (table.GetCellAt(0,0)) (CompositeCell.createUnitized("OldValue",(OntologyAnnotation("OldAnnotation")))) "Cell of old table should stay as is"
        Expect.notEqual copy table "Should not be equal after change"
]

let private tests_fillMissing = testList "fillMissing" [
        testCase "OntologyAnnotationCopied" <| fun _ ->
            let table = ArcTable.init "My Table"

            table.AddColumn(CompositeHeader.Input IOType.Source, ResizeArray [|for i in 0 .. 4 do CompositeCell.FreeText $"Source {i}"|])

            table.AddColumn(CompositeHeader.Component (OntologyAnnotation.create("instrument model", "MS", "MS:424242")))

            let cell = table.GetCellAt(1,0)

            match cell with
            | CompositeCell.Term oa -> 
              oa.Name <- Some "New Name"
            | _ -> failwith "Expected a term"

            let otherCell = table.GetCellAt(1,1)

            Expect.equal (table.GetCellAt(1,1)) (table.GetCellAt(1,2)) "Should be the same cell"
            Expect.notEqual cell otherCell "Should not be the same cell"        
    ]

let main = 
    testList "ArcTable" [
        tests_SanityChecks
        tests_ArcTableAux
        tests_member
        tests_GetCellAt
        tests_TryGetCellAt
        tests_Columns
        tests_UpdateHeader
        tests_UpdateCellAt
        tests_SetCellAt
        tests_UpdateCellsBy
        tests_UpdateColumn
        tests_AddColumn_Mutable
        tests_addColumn
        tests_AddColumns
        tests_AddColumnFill
        tests_RemoveColumn
        tests_RemoveColumns
        tests_TryGetColumnByHeaderBy
        tests_MoveColumn
        tests_RemoveRow
        tests_RemoveRows
        tests_AddRow
        tests_AddRows
        tests_validate
        tests_UpdateRefWithSheet
        tests_Join
        tests_IterColumns
        tests_GetHashCode
        tests_equality
        tests_copy
        tests_fillMissing
    ]