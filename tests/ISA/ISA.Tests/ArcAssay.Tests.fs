module ArcAssay.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

module Helper =
    let TableName = "Test"
    let oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
    let oa_temperature = OntologyAnnotation.fromString("temperature","NCIT","NCIT:0123210")

    /// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
    let tableValues_printable (table:ArcTable) = 
        [
            for KeyValue((c,r),v) in table.Values do
                yield $"({c},{r}) {v}"
        ]

    let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    let createCells_Term (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
    let createCells_Unitized (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation.empty))
    /// Input [Source] --> Source_0 .. Source_4
    let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 5)
    let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 5)
    let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 5)
    let column_param = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty, createCells_Unitized 5)
    /// Valid TestTable "Test" with 5 columns, 5 rows: 
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

    /// Creates 5 empty tables
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleTables() = Array.init 5 (fun i -> ArcTable.init($"New Table {i}"))

    /// Valid TestAssay with empty tables: 
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleAssay() =
        let assay = ArcAssay.create()
        let sheets = create_exampleTables()
        sheets |> Array.iter (fun table -> assay.AddTable(table))
        assay

open Helper

let private tests_AddTable = 
    testList "AddTable" [
        testCase "append, default table" (fun () ->
            let assay = ArcAssay.create()
            assay.AddTable()
            Expect.equal assay.SheetCount 1 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table" "Sheet Name"
        )
        testCase "append, default table, iter 5x" (fun () ->
            let assay = ArcAssay.create()
            for i in 1 .. 5 do assay.AddTable()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table" "Sheet Name 0"
            Expect.equal assay.SheetNames.[4] "New Table" "Sheet Name 4"
        )
        testCase "append, table" (fun () ->
            let assay = ArcAssay.create()
            let sheet = ArcTable.init($"MY NICE TABLE")
            assay.AddTable(sheet)
            Expect.equal assay.SheetCount 1 "SheetCount"
            Expect.equal assay.SheetNames.[0] "MY NICE TABLE" "Sheet Name 0"
        )
        testCase "append, tables, iter 5x" (fun () ->
            let assay = ArcAssay.create()
            let sheets = Array.init 5 (fun i -> ArcTable.init($"New Table {i}"))
            sheets |> Array.iter (fun table -> assay.AddTable(table))
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name 0"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name 4"
        )
        testCase "insert, default table" (fun () ->
            let assay = create_exampleAssay()
            assay.AddTable(index=0)
            Expect.equal assay.SheetCount 6 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table" "Sheet Name 0"
            Expect.equal assay.SheetNames.[1] "New Table 0" "Sheet Name 1"
            Expect.equal assay.SheetNames.[5] "New Table 4" "Sheet Name 4"
        )
    ]

let private tests_AddTables = 
    testList "AddTables" [
        testCase "append" (fun () ->
            let assay = ArcAssay.create()
            let tables = create_exampleTables()
            assay.AddTables(tables)
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
        testCase "insert" (fun () ->
            let assay = create_exampleAssay()
            let tables = create_exampleTables()
            assay.AddTables(tables, 2)
            Expect.equal assay.SheetCount 10 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name 0"
            Expect.equal assay.SheetNames.[1] "New Table 1" "Sheet Name 1"
            Expect.equal assay.SheetNames.[2] "New Table 0" "Sheet Name 2"
            Expect.equal assay.SheetNames.[6] "New Table 4" "Sheet Name 6"
            Expect.equal assay.SheetNames.[assay.SheetCount-1] "New Table 4" "Sheet Name 9"
        )
    ]

let private tests_Copy = 
    testList "Copy" [
        testCase "Ensure mutability" (fun () ->
            let assay = create_exampleAssay()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            assay.RemoveTable(0)
            Expect.equal assay.SheetCount 4 "SheetCount 2"
            Expect.equal assay.SheetNames.[0] "New Table 1" "Sheet Name 2"
        )
        testCase "Ensure mutability, in table scope" (fun () ->
            let assay = create_exampleAssay()
            let table = assay.GetTable(0)
            Expect.equal assay.SheetCount 5         "SheetCount"
            Expect.equal assay.SheetNames.[0]       "New Table 0" "Sheet Name"
            Expect.equal table.Name "New Table 0"   "Table Sheet Name"
            Expect.equal table.ColumnCount 0        "table.ColumnCount"
            Expect.equal table.RowCount 0           "Table Sheet Name"
            table.AddColumn(CompositeHeader.ProtocolREF, Array.init 5 (fun i -> CompositeCell.FreeText "My Protocol Name"))
            let table2 = assay.GetTable(0)
            Expect.equal table2.Name "New Table 0"   "Table Sheet Name"
            Expect.equal table2.ColumnCount 1        "table.ColumnCount"
            Expect.equal table2.RowCount 5           "Table Sheet Name"
        )
        testCase "Ensure source untouched" (fun () ->
            let assay = create_exampleAssay()
            let assay_expected = create_exampleAssay()
            let assay_copy = assay.Copy()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay_copy.SheetCount 5 "SheetCount"
            Expect.equal assay_copy.SheetNames.[0] "New Table 0" "Sheet Name"
            assay_copy.RemoveTable(0)
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay_copy.SheetCount 4 "SheetCount 2"
            Expect.equal assay_copy.SheetNames.[0] "New Table 1" "Sheet Name 2"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) assay.Sheets assay_expected.Sheets
        )
        testCase "Ensure source untouched, in table scope" (fun () ->
            let assay = create_exampleAssay()
            let assay_expected = create_exampleAssay()
            let assay_copy = assay.Copy()
            let table = assay_copy.GetTable(0)
            Expect.equal assay.SheetCount 5         "SheetCount"
            Expect.equal assay.SheetNames.[0]       "New Table 0" "Sheet Name"
            Expect.equal assay_copy.SheetCount 5    "SheetCount"
            Expect.equal assay_copy.SheetNames.[0]  "New Table 0" "Sheet Name"
            Expect.equal table.Name "New Table 0"   "Table Sheet Name"
            Expect.equal table.ColumnCount 0        "table.ColumnCount"
            Expect.equal table.RowCount 0           "Table Sheet Name"
            table.AddColumn(CompositeHeader.ProtocolREF, Array.init 5 (fun i -> CompositeCell.FreeText "My Protocol Name"))
            let table_shouldBeUnchanged = assay.GetTable(0)
            let table2_copy = assay_copy.GetTable(0)
            Expect.equal table_shouldBeUnchanged.Name "New Table 0"     "Table Sheet Name"
            Expect.equal table_shouldBeUnchanged.ColumnCount 0          "table.ColumnCount"
            Expect.equal table_shouldBeUnchanged.RowCount 0             "Table Sheet Name"
            Expect.equal table2_copy.Name "New Table 0"                 "Table Sheet Name"
            Expect.equal table2_copy.ColumnCount 1                      "table.ColumnCount"
            Expect.equal table2_copy.RowCount 5                         "Table Sheet Name"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) assay.Sheets assay_expected.Sheets
        )
    ]

let private tests_RemoveTable = 
    testList "RemoveTable" [
        testCase "empty table, throw" (fun () ->
            let assay = ArcAssay.create()
            let eval() = assay.RemoveTable(0)
            Expect.throws eval ""
        )
        testCase "ensure table" (fun () ->
            let assay = create_exampleAssay()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
        testCase "remove last" (fun () ->
            let assay = create_exampleAssay()
            assay.RemoveTable(assay.SheetCount-1)
            Expect.equal assay.SheetCount 4 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name first"
            Expect.equal assay.SheetNames.[assay.SheetCount-1] "New Table 3" "Sheet Name last"
        )
        testCase "remove first" (fun () ->
            let assay = create_exampleAssay()
            assay.RemoveTable(0)
            Expect.equal assay.SheetCount 4 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 1" "Sheet Name first"
            Expect.equal assay.SheetNames.[assay.SheetCount-1] "New Table 4" "Sheet Name last"
        )
        testCase "remove middle" (fun () ->
            let assay = create_exampleAssay()
            assay.RemoveTable(2)
            Expect.equal assay.SheetCount 4 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name first"
            Expect.equal assay.SheetNames.[assay.SheetCount-1] "New Table 4" "Sheet Name last"
        )
    ]

let private tests_SetTable = 
    testList "SetTable" [
        testCase "ensure table" (fun () ->
            let assay = create_exampleAssay()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
        testCase "set on append index, throws" (fun () ->
            let assay = create_exampleAssay()
            let table = create_testTable()
            let eval() = assay.SetTable(assay.SheetCount, table)
            Expect.throws eval ""
        )
        testCase "set on first" (fun () ->
            let index = 0
            let assay = create_exampleAssay()
            let table = create_testTable()
            assay.SetTable(index, table)
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[index] "Test" "Sheet Name changed"
            Expect.equal (assay.GetTable(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (assay.GetTable(index).RowCount) 5 "RowCount"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name last"
        )
        testCase "set on last" (fun () ->
            let assay = create_exampleAssay()
            let index = assay.SheetCount-1
            let table = create_testTable()
            assay.SetTable(index, table)
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[index] "Test" "Sheet Name changed"
            Expect.equal (assay.GetTable(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (assay.GetTable(index).RowCount) 5 "RowCount"
        )
        testCase "set on middle" (fun () ->
            let assay = create_exampleAssay()
            let index = 2
            let table = create_testTable()
            assay.SetTable(index, table)
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[index] "Test" "Sheet Name changed"
            Expect.equal (assay.GetTable(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (assay.GetTable(index).RowCount) 5 "RowCount"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name last"
        )
    ]

let private tests_UpdateTable = 
    testList "UpdateTable" [
        testCase "ensure table" (fun () ->
            let assay = create_exampleAssay()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
        testCase "set on append index, throws" (fun () ->
            let assay = create_exampleAssay()
            let eval() = assay.UpdateTable(assay.SheetCount, fun table -> table)
            Expect.throws eval ""
        )
        testCase "update, replace" (fun () ->
            let index = 1
            let assay = create_exampleAssay()
            assay.UpdateTable(index, fun _ ->
                create_testTable()
            )
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[index] "Test" "Sheet Name changed"
            Expect.equal (assay.GetTable(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (assay.GetTable(index).RowCount) 5 "RowCount"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
        testCase "update, AddColumn" (fun () ->
            let index = 1
            let assay = create_exampleAssay()
            assay.UpdateTable(index, fun table ->
                table.AddColumn(column_input.Header, column_input.Cells)
                table
            )
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[index] "New Table 1" "Sheet Name updated"
            Expect.equal (assay.GetTable(index).ColumnCount) 1 "ColumnCount"
            Expect.equal (assay.GetTable(index).RowCount) 5 "RowCount"
            Expect.equal (assay.GetTable(index).Headers.[0]) (CompositeHeader.Input IOType.Source) "Header"
            Expect.equal (assay.GetTable(index).Values.[(0,0)]) (CompositeCell.FreeText "Source_0") "Cell 0,0"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
    ]

let private tests_updateTable = 
    testList "updateTable" [
        testCase "ensure table" (fun () ->
            let assay = create_exampleAssay()
            Expect.equal assay.SheetCount 5 "SheetCount"
            Expect.equal assay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal assay.SheetNames.[4] "New Table 4" "Sheet Name"
        )
        testCase "set on append index, throws" (fun () ->
            let assay = create_exampleAssay()
            let eval() = assay.UpdateTable(assay.SheetCount, fun table -> table)
            Expect.throws eval ""
        )
        testCase "update, replace" (fun () ->
            let index = 1
            let assay = create_exampleAssay()
            let assay_expected = create_exampleAssay()
            let newAssay = 
                assay |> ArcAssay.updateTable (index, fun _ ->
                    create_testTable()
                )
            Expect.equal newAssay.SheetCount 5 "SheetCount"
            Expect.equal newAssay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal newAssay.SheetNames.[index] "Test" "Sheet Name changed"
            Expect.equal (newAssay.GetTable(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (newAssay.GetTable(index).RowCount) 5 "RowCount"
            Expect.equal newAssay.SheetNames.[4] "New Table 4" "Sheet Name"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) assay.Sheets assay_expected.Sheets
        )
        testCase "update, AddColumn" (fun () ->
            let index = 1
            let assay = create_exampleAssay()
            let assay_expected = create_exampleAssay()
            let newAssay = 
                assay |> ArcAssay.updateTable(index, fun table ->
                    table.AddColumn(column_input.Header, column_input.Cells)
                    table
                )
            Expect.equal newAssay.SheetCount 5 "SheetCount"
            Expect.equal newAssay.SheetNames.[0] "New Table 0" "Sheet Name"
            Expect.equal newAssay.SheetNames.[index] "New Table 1" "Sheet Name updated"
            Expect.equal (newAssay.GetTable(index).ColumnCount) 1 "ColumnCount"
            Expect.equal (newAssay.GetTable(index).RowCount) 5 "RowCount"
            Expect.equal (newAssay.GetTable(index).Headers.[0]) (CompositeHeader.Input IOType.Source) "Header"
            Expect.equal (newAssay.GetTable(index).Values.[(0,0)]) (CompositeCell.FreeText "Source_0") "Cell 0,0"
            Expect.equal newAssay.SheetNames.[4] "New Table 4" "Sheet Name"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) assay.Sheets assay_expected.Sheets
        )
    ]

let main = 
    testList "ArcAssay" [
        tests_AddTable
        tests_AddTables
        tests_Copy
        tests_RemoveTable
        tests_SetTable
        tests_UpdateTable
        tests_updateTable
    ]