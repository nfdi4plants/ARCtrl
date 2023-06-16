module ArcTable.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let private TableName = "Test"
let private oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
let private oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
let private oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
let private oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
let private oa_temperature = OntologyAnnotation.fromString("temperature","NCIT","NCIT:0123210")

/// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
let private tableValues_printable (table:ArcTable) = 
    [
        for KeyValue((c,r),v) in table.Values do
            yield $"({c},{r}) {v}"
    ]

let private tests_member = 
    let TableName = "Test"
    let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    let createCells_Term (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
    let createCells_Unitized (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation.empty))
    let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 7)
    let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 7)
    let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 7)
    let column_param = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty, createCells_Unitized 7)
    /// Valid TestTable with 5 columns, 7 rows: 
    ///
    /// Input [Source] -> 5 cells: [Source_1; Source_2..]
    ///
    /// Output [Sample] -> 5 cells: [Sample_1; Sample_2..]
    ///
    /// Component [empty] -> 5 cells: [SCIEX instrument model; SCIEX instrument model; ..]
    ///
    /// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]    
    ///
    /// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]   
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
    ]

let private tests_ArcTableAux =
    testList "ArcTableAux" [
        testList "SanityChecks" [
            let headers_valid = [
                CompositeHeader.Input IOType.Sample
                CompositeHeader.FreeText "Any"
                CompositeHeader.Component OntologyAnnotation.empty
                CompositeHeader.ProtocolREF
                CompositeHeader.ProtocolType
                CompositeHeader.Output IOType.DerivedDataFile
            ]
            testCase "valid headers" (fun () ->
                let columns = headers_valid |> Seq.map (fun x -> CompositeColumn.create(x))
                let eval() = ArcTableAux.SanityChecks.validateNoDuplicateUniqueColumns columns
                eval()
                Expect.isTrue true "We canot eval for 'not throw'"
            )
            testCase "valid headers, multiplied non unique" (fun () ->
                let header_valid2 = [CompositeHeader.FreeText "Any"; CompositeHeader.Component OntologyAnnotation.empty; CompositeHeader.Component OntologyAnnotation.empty]@headers_valid
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
        testList "tryFindDuplicateUnique" [
            let headers = [
                CompositeHeader.Input IOType.Sample
                CompositeHeader.FreeText "Any"
                CompositeHeader.Component OntologyAnnotation.empty
                CompositeHeader.ProtocolREF
                CompositeHeader.ProtocolType
                CompositeHeader.Output IOType.DerivedDataFile
            ]
            testCase "No duplicate, component" (fun () ->
                let header = CompositeHeader.Component OntologyAnnotation.empty
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, parameter" (fun () ->
                let header = CompositeHeader.Parameter OntologyAnnotation.empty
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, factor" (fun () ->
                let header = CompositeHeader.Factor OntologyAnnotation.empty
                let hasDuplicate = headers |> ArcTableAux.tryFindDuplicateUnique header
                Expect.isNone hasDuplicate "Some/None"
            )
            testCase "No duplicate, chara" (fun () ->
                let header = CompositeHeader.Characteristic OntologyAnnotation.empty
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

let private tests_addColumn =
    let header_input = CompositeHeader.Input IOType.Source
    let header_chara = CompositeHeader.Characteristic oa_species
    let createCells_chara (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
    let createCells_freetext pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    testList "addColumn" [
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
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                table.AddColumn(header, cells)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.Headers.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in Array.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.mySequenceEqual table.Values expected "values"
            )
            testCase "term column, with cells" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                table.AddColumn(header, cells)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.Headers.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in Array.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.mySequenceEqual table.Values expected "values"
            )
            testCase "IO column, with wrong cells" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable() = table.AddColumn(header, cells)
                Expect.throws newTable ""
            )
            testCase "term column, with wrong cells" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable() = table.AddColumn(header, cells)
                Expect.throws newTable ""
            )
            testCase "IO column, with cells at index" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                table.AddColumn(header, cells, 0)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
            )
            testCase "term column, with cells at index" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                table.AddColumn(header, cells, 0)
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ColumnCount 1 "ColumnCount"
            )
            testCase "IO column, with cells at outside index" (fun () ->
                let table = create_table()
                let header = header_input
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable() = table.AddColumn(header, cells, 1)
                Expect.throws newTable ""
            )
            testCase "term column, with cells at outside index" (fun () ->
                let table = create_table()
                let header = header_chara
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable() = table.AddColumn(header, cells, 1)
                Expect.throws newTable ""
            )
        ]
        testList "Existing Table" [
            /// Table contains 5 rows and 1 column Input [Source] with cells "Source_id"
            let create_table() = 
                let io_cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
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
                    Array.init 5 (fun i -> 
                        let c = if i <= 1 then CompositeCell.createTerm oa_chlamy else CompositeCell.emptyTerm
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
                let expected_chara = Array.init 8 (fun i -> System.Collections.Generic.KeyValuePair((1,i), CompositeCell.createTerm oa_chlamy))
                let expected_io = 
                    Array.init 8 (fun i -> 
                        let c = if i <= 4 then CompositeCell.createFreeText $"Source_{i}" else CompositeCell.emptyFreeText
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
                let printable = table |> tableValues_printable
                System.IO.File.WriteAllLines(@"C:\Users\Kevin\Desktop\test.txt", printable)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.Headers.[0] header_chara "Header chara"
                Expect.equal table.Headers.[1] header_input "Header io"
                let expected = 
                    Array.init 5 (fun i -> 
                        let c = if i <= 1 then CompositeCell.createTerm oa_chlamy else CompositeCell.emptyTerm
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
                let expected_chara = Array.init 8 (fun i -> System.Collections.Generic.KeyValuePair((0,i), CompositeCell.createTerm oa_chlamy))
                let expected_io = 
                    Array.init 8 (fun i -> 
                        let c = if i <= 4 then CompositeCell.createFreeText $"Source_{i}" else CompositeCell.emptyFreeText
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
            testCase "add more rows, replace input, replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                table.AddColumn(newHeader, createCells_freetext "NewInput" 8, forceReplace=true)
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 8 "RowCount"
                Expect.equal table.Headers.[0] newHeader "Header"
            )
            testCase "add less rows, replace input, replace" (fun () ->
                let table = create_table()
                let newHeader = CompositeHeader.Input IOType.Sample
                table.AddColumn(newHeader, createCells_freetext "NewInput" 2, forceReplace=true)
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 2 "RowCount"
                Expect.equal table.Headers.[0] newHeader "Header"
            )
        ]
    ]

let private test_addColumns =
    testList "addColumns" [
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
                let column_2 = CompositeColumn.create(CompositeHeader.Component OntologyAnnotation.empty)
                let column_3 = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty)
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
                let column_2 = CompositeColumn.create(CompositeHeader.Component OntologyAnnotation.empty)
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
                let column_2 = CompositeColumn.create(CompositeHeader.Component OntologyAnnotation.empty)
                let column_3 = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty)
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
            let column_component = CompositeColumn.create(CompositeHeader.Component OntologyAnnotation.empty)
            let column_param = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty)
            /// Valid TestTable with 5 columns, no cells: Input [Source] - >Output [Sample] -> Component [empty] -> Parameter [empty] -> Parameter [empty]
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
                let expected_ColumnCount = table.ColumnCount + 4
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
                let expected_ColumnCount = testTable.ColumnCount + 3
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
                let expected_ColumnCount = testTable.ColumnCount + 3
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
                let expected_ColumnCount = testTable.ColumnCount + 4
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
                let expected_ColumnCount = testTable.ColumnCount + 4
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
                let expected_ColumnCount = testTable.ColumnCount + 3
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
                let expected_ColumnCount = testTable.ColumnCount + 3
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
            let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
            let createCells_Term (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
            let createCells_Unitized (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation.empty))
            let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 5)
            let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 5)
            let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 5)
            let column_param = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty, createCells_Unitized 5)
            /// Valid TestTable with 5 columns, no cells: 
            ///
            /// Input [Source] -> 5 cells: [Source_1; Source_2..]
            ///
            /// Output [Sample] -> 5 cells: [Sample_1; Sample_2..]
            ///
            /// Component [empty] -> 5 cells: [SCIEX instrument model; SCIEX instrument model; ..]
            ///
            /// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]    
            ///
            /// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]   
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
                Expect.equal testTable.RowCount 5 "RowCount"
                Expect.equal testTable.ColumnCount 5 "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
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
                let expected_ColumnCount = testTable.ColumnCount + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
                Expect.equal testTable.Headers.[6] column_param.Header "header4"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Headers.[8] column_param.Header "header4"
                Expect.equal testTable.Values.[8,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 8,4"
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
                let expected_ColumnCount = testTable.ColumnCount + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[4] column_input.Header "header0"
                Expect.equal testTable.Headers.[5] column_output.Header "header1"
                Expect.equal testTable.Headers.[6] column_component.Header "header2"
                Expect.equal testTable.Headers.[7] column_param.Header "header3"
                Expect.equal testTable.Headers.[8] column_param.Header "header4"
                Expect.equal testTable.Values.[4,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[8,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Headers.[0] column_param.Header "header4"
                Expect.equal testTable.Headers.[1] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
                Expect.equal testTable.Values.[3,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 8,4"
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
                let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "NEW" 5)
                let columns = [|
                    column_param
                    newInputCol
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = testTable.ColumnCount + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInputCol.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Headers.[5] column_param.Header "header4"
                Expect.equal testTable.Headers.[6] column_param.Header "header4"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Values.[7,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 7,4"
            )
            testCase "multiple columns, same rowCount, duplicate replace, insert at" (fun () ->
                let testTable = create_testTable()
                let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "NEW" 5)
                let columns = [|
                    column_param
                    newInputCol
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 0, true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = testTable.ColumnCount + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[3] newInputCol.Header "header0"
                Expect.equal testTable.Headers.[4] column_output.Header "header1"
                Expect.equal testTable.Headers.[5] column_component.Header "header2"
                Expect.equal testTable.Headers.[6] column_param.Header "header3"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Values.[3,0] (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Values.[7,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Headers.[0] column_param.Header "header4"
                Expect.equal testTable.Headers.[1] column_param.Header "header4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Values.[2,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 7,4"
            )
            testCase "multiple columns, same rowCount, duplicate replace, insert at2" (fun () ->
                let testTable = create_testTable()
                let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "NEW" 5)
                let columns = [|
                    column_param
                    newInputCol
                    column_param
                    column_param
                |]
                testTable.AddColumns(columns, 2, true)
                let expected_RowCount = testTable.RowCount
                let expected_ColumnCount = testTable.ColumnCount + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInputCol.Header "header0"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[5] column_component.Header "header2"
                Expect.equal testTable.Headers.[6] column_param.Header "header3"
                Expect.equal testTable.Headers.[7] column_param.Header "header4"
                Expect.equal testTable.Values.[7,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 7,4"
                Expect.equal testTable.Headers.[2] column_param.Header "header4"
                Expect.equal testTable.Headers.[3] column_param.Header "header4"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
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
                let expected_ColumnCount = testTable.ColumnCount + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal testTable.Headers.[8] newColumn.Header "header8"
                Expect.equal testTable.Values.[8,4] (CompositeCell.emptyTerm) "cell 8,4"
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
                let expected_RowCount = newColumn.Cells.Length
                let expected_ColumnCount = testTable.ColumnCount + columns.Length
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] column_input.Header "header0"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
                Expect.equal testTable.Values.[0,7] (CompositeCell.emptyFreeText) "cell 0,7"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Values.[4,7] (CompositeCell.emptyUnitized) "cell 4,7"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal testTable.Headers.[8] newColumn.Header "header8"
                Expect.equal testTable.Values.[8,7] (newColumn.Cells.[0]) "cell 8,7"
            )
            testCase "multiple columns, more rowCount, duplicate replace" (fun () ->
                let testTable = create_testTable()
                let newInput = CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "NEW" 8)
                let newColumn = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 8)
                let columns = [|
                    newColumn
                    newInput
                    newColumn
                    newColumn
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = newColumn.Cells.Length
                let expected_ColumnCount = testTable.ColumnCount + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInput.Header "header0"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Values.[0,7] (CompositeCell.FreeText "NEW_7") "cell 0,7"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Values.[4,7] (CompositeCell.emptyUnitized) "cell 4,7"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal testTable.Values.[7,7] (newColumn.Cells.[0]) "cell 7,7"
            )
            testCase "multiple columns, different rowCount, duplicate replace" (fun () ->
                let testTable = create_testTable()
                let newInput = CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "NEW" 2)
                let newColumn = CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_Term 8)
                let columns = [|
                    newColumn
                    newInput
                    newColumn
                    newColumn
                |]
                testTable.AddColumns(columns, forceReplace=true)
                let expected_RowCount = newColumn.Cells.Length
                let expected_ColumnCount = testTable.ColumnCount + columns.Length - 1
                Expect.equal testTable.RowCount expected_RowCount "RowCount"
                Expect.equal testTable.ColumnCount expected_ColumnCount "ColumnCount"
                Expect.equal testTable.Headers.[0] newInput.Header "header0"
                Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "NEW_0") "cell 0,0"
                Expect.equal testTable.Values.[0,7] (CompositeCell.emptyFreeText) "cell 0,7"
                Expect.equal testTable.Headers.[1] column_output.Header "header1"
                Expect.equal testTable.Headers.[2] column_component.Header "header2"
                Expect.equal testTable.Headers.[3] column_param.Header "header3"
                Expect.equal testTable.Headers.[4] column_param.Header "header4"
                Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
                Expect.equal testTable.Values.[4,7] (CompositeCell.emptyUnitized) "cell 4,7"
                Expect.equal testTable.Headers.[5] newColumn.Header "header5"
                Expect.equal testTable.Headers.[6] newColumn.Header "header6"
                Expect.equal testTable.Headers.[7] newColumn.Header "header7"
                Expect.equal testTable.Values.[7,7] (newColumn.Cells.[0]) "cell 7,7"
            )
        ]
    ]

let private test_setHeader = 
    testList "setHeader" [
        let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
        let createCells_Term (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
        let createCells_Unitized (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation.empty))
        let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 5)
        let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 5)
        let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 5)
        let column_param = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty, createCells_Unitized 5)
        /// Valid TestTable with 5 columns, no cells: 
        ///
        /// Input [Source] -> 5 cells: [Source_1; Source_2..]
        ///
        /// Output [Sample] -> 5 cells: [Sample_1; Sample_2..]
        ///
        /// Component [empty] -> 5 cells: [SCIEX instrument model; SCIEX instrument model; ..]
        ///
        /// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]    
        ///
        /// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]   
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
            Expect.equal testTable.RowCount 5 "RowCount"
            Expect.equal testTable.ColumnCount 5 "ColumnCount"
            Expect.equal testTable.Headers.[0] column_input.Header "header0"
            Expect.equal testTable.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal testTable.Headers.[1] column_output.Header "header1"
            Expect.equal testTable.Headers.[2] column_component.Header "header2"
            Expect.equal testTable.Headers.[3] column_param.Header "header3"
            Expect.equal testTable.Headers.[4] column_param.Header "header4"
            Expect.equal testTable.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
        )
        testCase "set outside of range" (fun () ->
            let testTable = create_testTable()
            let table() = testTable.SetHeader(12, CompositeHeader.Characteristic OntologyAnnotation.empty)
            Expect.throws table ""
        )
        testCase "set outside of range negative" (fun () ->
            let testTable = create_testTable()
            let table() = testTable.SetHeader(-12, CompositeHeader.Characteristic OntologyAnnotation.empty)
            Expect.throws table ""
        )
        testCase "set unique at different index" (fun () ->
            let testTable = create_testTable()
            let table() = testTable.SetHeader(2, CompositeHeader.Input IOType.DerivedDataFile)
            Expect.throws (table >> ignore) ""
        )
        testCase "set unique at same index" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Input IOType.DerivedDataFile
            table.SetHeader(0, newHeader)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] newHeader "header0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_component.Header "header2"
            Expect.equal table.Headers.[3] column_param.Header "header3"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
        )
        testCase "set invalid, force convert" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Factor oa_temperature
            table.SetHeader(0, newHeader, true)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] newHeader "header0"
            Expect.equal table.Values.[0,0] (CompositeCell.createTermFromString "Source_0") "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_component.Header "header2"
            Expect.equal table.Headers.[3] column_param.Header "header3"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
        )
        testCase "set valid" (fun () ->
            let table = create_testTable()
            let newHeader = CompositeHeader.Factor oa_temperature
            table.SetHeader(3, newHeader)
            Expect.equal table.RowCount 5 "RowCount"
            Expect.equal table.ColumnCount 5 "ColumnCount"
            Expect.equal table.Headers.[0] column_input.Header "header0"
            Expect.equal table.Values.[0,0] (CompositeCell.FreeText "Source_0") "cell 0,0"
            Expect.equal table.Headers.[1] column_output.Header "header1"
            Expect.equal table.Headers.[2] column_component.Header "header2"
            Expect.equal table.Headers.[3] newHeader "header3"
            Expect.equal table.Values.[3,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 3,4"
            Expect.equal table.Headers.[4] column_param.Header "header4"
            Expect.equal table.Values.[4,4] (CompositeCell.createUnitized (string 4,OntologyAnnotation.empty)) "cell 4,4"
        )
    ]

let main = 
    testList "ArcTable" [
        tests_ArcTableAux
        tests_member
        tests_addColumn
        test_addColumns
        test_setHeader
    ]