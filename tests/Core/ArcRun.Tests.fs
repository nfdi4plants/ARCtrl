module ArcRun.Tests

open ARCtrl
open ARCtrl.Helper
open TestingUtils

module Helper =
    let TableName = "Test"
    let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
    let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")

    /// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
    let tableValues_printable (table:ArcTable) = 
        [
            for KeyValue((c,r),v) in table.Values do
                yield $"({c},{r}) {v}"
        ]

    let createCells_FreeText pretext (count) = ResizeArray.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    let createCells_Term (count) = ResizeArray.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
    let createCells_Unitized (count) = ResizeArray.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation()))
    /// Input [Source] --> Source_0 .. Source_4
    let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 5)
    let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 5)
    let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 5)
    let column_param = CompositeColumn.create(CompositeHeader.Parameter (OntologyAnnotation()), createCells_Unitized 5)
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
    let create_exampleTables(appendStr:string) = Array.init 5 (fun i -> ArcTable.init($"{appendStr} Table {i}"))

    /// Valid TestRun with empty tables: 
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleRun() =
        let run = ArcRun("MyRun")
        let sheets = create_exampleTables("My")
        sheets |> Array.iter (fun table -> run.AddTable(table))
        run

open Helper

let private test_create =
    testList "create" [
        testCase "constructor" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Best Run On Earth"
            let description = "Run to be used for being the best"
            let oa_mt = OntologyAnnotation("measurement type")
            let oa_tt = OntologyAnnotation("technology type")
            let technologyPlatform = OntologyAnnotation("tp")
            let tables = ResizeArray([ArcTable.init("MyTable1")])
            let performers = ResizeArray [|Person(firstName = "Kevin", lastName = "Frey")|]
            let workflowIdentifiers = ResizeArray [|"Workflow1"|]
            let comments = ResizeArray [|Comment.create("Comment Name")|]
            let actual = ArcRun(identifier, title = title, description = description, measurementType = oa_mt, technologyType = oa_tt, technologyPlatform = technologyPlatform, workflowIdentifiers = workflowIdentifiers, tables = tables, performers = performers, comments = comments)
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.MeasurementType (Some oa_mt) "MeasurementType"
            Expect.equal actual.TechnologyType (Some oa_tt) "TechnologyType"
            Expect.equal actual.TechnologyPlatform (Some technologyPlatform) "technologyPlatform"
            Expect.equal actual.WorkflowIdentifiers workflowIdentifiers "workflowIdentifiers"
            Expect.equal actual.Tables tables "tables"
            Expect.equal actual.Performers performers "performers"
            Expect.equal actual.Comments comments "Comments"
        testCase "constructor_tpOntology" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Best Run On Earth"
            let description = "Run to be used for being the best"
            let oa_mt = OntologyAnnotation("measurement type")
            let oa_tt = OntologyAnnotation("technology type")
            let technologyPlatform = OntologyAnnotation("tp","ABC","ABC:123")
            let workflowIdentifiers = ResizeArray [|"Workflow1"|]
            let tables = ResizeArray([ArcTable.init("MyTable1")])
            let performers = ResizeArray [|Person.create(firstName = "Kevin", lastName = "Frey")|]
            let comments = ResizeArray [|Comment.create("Comment Name")|]
            let actual = ArcRun(identifier, title = title, description = description, measurementType = oa_mt, technologyType = oa_tt, technologyPlatform = technologyPlatform, workflowIdentifiers = workflowIdentifiers, tables = tables, performers = performers, comments = comments)
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.MeasurementType (Some oa_mt) "MeasurementType"
            Expect.equal actual.TechnologyType (Some oa_tt) "TechnologyType"
            Expect.equal actual.TechnologyPlatform (Some technologyPlatform) "technologyPlatform"
            Expect.equal actual.WorkflowIdentifiers workflowIdentifiers "workflowIdentifiers"
            Expect.equal actual.Tables tables "tables"
            Expect.equal actual.Performers performers "performers"
            Expect.equal actual.Comments comments "Comments"
        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Best Run On Earth"
            let description = "Run to be used for being the best"
            let oa_mt = OntologyAnnotation("measurement type")
            let oa_tt = OntologyAnnotation("technology type")
            let technologyPlatform = OntologyAnnotation("tp")
            let workflowIdentifiers = ResizeArray [|"Workflow1"|]
            let tables = ResizeArray([ArcTable.init("MyTable1")])
            let performers = ResizeArray[|Person.create(firstName = "Kevin", lastName = "Frey")|]
            let comments = ResizeArray[|Comment.create("Comment Name")|]
            let actual = ArcRun.create(identifier, title = title, description = description, measurementType = oa_mt, technologyType = oa_tt, technologyPlatform = technologyPlatform, workflowIdentifiers = workflowIdentifiers, tables = tables, performers = performers, comments = comments)
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.MeasurementType (Some oa_mt) "MeasurementType"
            Expect.equal actual.TechnologyType (Some oa_tt) "TechnologyType"
            Expect.equal actual.TechnologyPlatform (Some technologyPlatform) "technologyPlatform"
            Expect.equal actual.WorkflowIdentifiers workflowIdentifiers "workflowIdentifiers"
            Expect.equal actual.Tables tables "tables"
            Expect.equal actual.Performers performers "performers"
            Expect.equal actual.Comments comments "Comments"
        testCase "init" <| fun _ ->
            let identifier = "MyIdentifier"
            let actual = ArcRun.init identifier
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.MeasurementType None "MeasurementType"
            Expect.equal actual.TechnologyType None "TechnologyType"
            Expect.equal actual.TechnologyPlatform None "technologyPlatform"
            Expect.equal actual.WorkflowIdentifiers.Count 0 "workflowIdentifiers"
            Expect.equal actual.Tables.Count 0 "tables"
            Expect.equal actual.Performers.Count 0 "performers"
            Expect.equal actual.Comments.Count 0 "Comments"
        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Best Run On Earth"
            let description = Some "Run to be used for being the best"
            let measurementType = Some (OntologyAnnotation("Measurement Type"))
            let technologyType = Some (OntologyAnnotation("Technology Type"))
            let technologyPlatform = Some (OntologyAnnotation("Technology Platform"))
            let workflowIdentifiers = ResizeArray [|"Workflow1"|]
            let tables = ResizeArray([ArcTable.init("Table 1")])
            let performers = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let comments = ResizeArray [|Comment.create("Comment 1")|]

            let actual = 
                ArcRun.make
                    identifier
                    title
                    description
                    measurementType
                    technologyType
                    technologyPlatform
                    workflowIdentifiers
                    tables
                    None
                    performers
                    comments

            Expect.equal actual.Identifier identifier "Identifier"
            Expect.equal actual.MeasurementType measurementType "MeasurementType"
            Expect.equal actual.TechnologyType technologyType "TechnologyType"
            Expect.equal actual.TechnologyPlatform technologyPlatform "TechnologyPlatform"
            Expect.equal actual.WorkflowIdentifiers workflowIdentifiers "WorkflowIdentifiers"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.Performers performers "Performers"
            Expect.equal actual.Comments comments "Comments"
        testCase "createDuplicateTablesNames" <| fun _ ->
            let table1 = ArcTable.init("Table 1")
            let table2 = ArcTable.init("Table 1")
            let tables = ResizeArray([table1; table2])
            let createRun =
                fun () -> ArcRun.create("MyRun", tables = tables) |> ignore
            Expect.throws createRun "throws, duplicate table names"
        testCase "failsForInvalidCharacters" <| fun _ ->
            let createRun =
                fun () -> ArcRun("My{Run") |> ignore
            Expect.throws createRun "throws, invalid characters"
        testCase "whiteSpaceTrimmed" <| fun _ ->
            let run = ArcRun(" MyRun ")
            Expect.equal run.Identifier "MyRun" "Identifier"
    ]

let private tests_AddTable = 
    testList "AddTable" [
        testCase "append, default table" (fun () ->
            let run = ArcRun("MyRun")
            let table = ArcTable.init("New Table 1")
            run.AddTable(table)
            Expect.equal run.TableCount 1 "TableCount"
            Expect.equal run.TableNames.[0] "New Table 1" "Sheet Name"
        )
        testCase "append, default table, iter 5x" (fun () ->
            let run = ArcRun("MyRun")
            for i in 1 .. 5 do 
                run.AddTable(ArcTable.init($"New Table {i}"))
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "New Table 1" "Sheet Name 0"
            Expect.equal run.TableNames.[4] "New Table 5" "Sheet Name 4"
        )
        testCase "append, table" (fun () ->
            let run = ArcRun("MyRun")
            let sheet = ArcTable.init($"MY NICE TABLE")
            run.AddTable(sheet)
            Expect.equal run.TableCount 1 "TableCount"
            Expect.equal run.TableNames.[0] "MY NICE TABLE" "Sheet Name 0"
        )
        testCase "append, tables, iter 5x" (fun () ->
            let run = ArcRun("MyRun")
            let sheets = Array.init 5 (fun i -> ArcTable.init($"New Table {i}"))
            sheets |> Array.iter (fun table -> run.AddTable(table))
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "New Table 0" "Sheet Name 0"
            Expect.equal run.TableNames.[4] "New Table 4" "Sheet Name 4"
        )
        testCase "insert, default table" (fun () ->
            let run = create_exampleRun()
            run.AddTable(ArcTable.init("New Table 1"), index=0)
            Expect.equal run.TableCount 6 "TableCount"
            Expect.equal run.TableNames.[0] "New Table 1" "Sheet Name 0"
            Expect.equal run.TableNames.[1] "My Table 0" "Sheet Name 1"
            Expect.equal run.TableNames.[5] "My Table 4" "Sheet Name 4"
        )
        testCase "add, duplicate name, throws" (fun () ->
            let run = ArcRun("MyRun")
            run.AddTable(ArcTable.init("New Table 1"))
            Expect.equal run.TableCount 1 "TableCount"
            Expect.equal run.TableNames.[0] "New Table 1" "Sheet Name"
            let eval() = run.AddTable(ArcTable.init "New Table 1")
            Expect.throws eval "throws"
        )
    ]

let private tests_AddTables = 
    testList "AddTables" [
        testCase "append" (fun () ->
            let run = ArcRun("MyRun")
            let tables = create_exampleTables("My")
            run.AddTables(tables)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
        )
        testCase "insert" (fun () ->
            let run = create_exampleRun()
            let tables = create_exampleTables("Next")
            run.AddTables(tables, 2)
            Expect.equal run.TableCount 10 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name 0"
            Expect.equal run.TableNames.[1] "My Table 1" "Sheet Name 1"
            Expect.equal run.TableNames.[2] "Next Table 0" "Sheet Name 2"
            Expect.equal run.TableNames.[6] "Next Table 4" "Sheet Name 6"
            Expect.equal run.TableNames.[run.TableCount-1] "My Table 4" "Sheet Name 9"
        )
        testCase "add, duplicate name, throws" (fun () ->
            let run = ArcRun("MyRun")
            let tables = create_exampleTables("My")
            run.AddTables(tables)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
            let eval() = run.AddTables(tables)
            Expect.throws eval "throws, duplicate table names"
        )
        testCase "add, duplicate new names, throws" (fun () ->
            let run = ArcRun("MyRun")
            let tables = create_exampleTables("My") |> Array.map (fun x -> x |> IdentifierSetters.setArcTableName "Duplicate Name")
            let eval() = run.AddTables(tables)
            Expect.throws eval "throws, duplicate table names"
        )
    ]

let private tests_Copy = 
    testList "Copy" [
        testCase "Ensure mutability" (fun () ->
            let run = create_exampleRun()
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            run.RemoveTableAt(0)
            Expect.equal run.TableCount 4 "TableCount 2"
            Expect.equal run.TableNames.[0] "My Table 1" "Sheet Name 2"
        )
        testCase "Ensure mutability, in table scope" (fun () ->
            let run = create_exampleRun()
            let table = run.GetTableAt(0)
            Expect.equal run.TableCount 5                 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0"  "Sheet Name"
            Expect.equal table.Name "My Table 0"            "Table Sheet Name"
            Expect.equal table.ColumnCount 0                "table.ColumnCount"
            Expect.equal table.RowCount 0                   "Table Sheet Name"
            table.AddColumn(CompositeHeader.ProtocolREF, ResizeArray.init 5 (fun i -> CompositeCell.FreeText "My Protocol Name"))
            let table2 = run.GetTableAt(0)
            Expect.equal table2.Name "My Table 0"           "Table Sheet Name"
            Expect.equal table2.ColumnCount 1               "table.ColumnCount"
            Expect.equal table2.RowCount 5                  "Table Sheet Name"
        )
        testCase "Ensure source untouched" (fun () ->
            let run = create_exampleRun()
            let run_expected = create_exampleRun()
            let run_copy = run.Copy()
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run_copy.TableCount 5 "TableCount"
            Expect.equal run_copy.TableNames.[0] "My Table 0" "Sheet Name"
            run_copy.RemoveTableAt(0)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run_copy.TableCount 4 "TableCount 2"
            Expect.equal run_copy.TableNames.[0] "My Table 1" "Sheet Name 2"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) run.Tables run_expected.Tables
        )
        testCase "Ensure source untouched, in table scope" (fun () ->
            let run = create_exampleRun()
            let run_expected = create_exampleRun()
            let run_copy = run.Copy()
            let table = run_copy.GetTableAt(0)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run_copy.TableCount 5 "TableCount"
            Expect.equal run_copy.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal table.Name "My Table 0" "Table Sheet Name"
            Expect.equal table.ColumnCount 0 "table.ColumnCount"
            Expect.equal table.RowCount 0 "Table Sheet Name"
            table.AddColumn(CompositeHeader.ProtocolREF, ResizeArray.init 5 (fun i -> CompositeCell.FreeText "My Protocol Name"))
            let table_shouldBeUnchanged = run.GetTableAt(0)
            let table2_copy = run_copy.GetTableAt(0)
            Expect.equal table_shouldBeUnchanged.Name "My Table 0"     "Table Sheet Name"
            Expect.equal table_shouldBeUnchanged.ColumnCount 0          "table.ColumnCount"
            Expect.equal table_shouldBeUnchanged.RowCount 0             "Table Sheet Name"
            Expect.equal table2_copy.Name "My Table 0"                 "Table Sheet Name"
            Expect.equal table2_copy.ColumnCount 1                      "table.ColumnCount"
            Expect.equal table2_copy.RowCount 5                         "Table Sheet Name"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) run.Tables run_expected.Tables
        )
    ]

let private tests_RemoveTable = 
    testList "RemoveTableAt" [
        testCase "empty table, throw" (fun () ->
            let run = ArcRun("MyRun")
            let eval() = run.RemoveTableAt(0)
            Expect.throws eval ""
        )
        testCase "ensure table" (fun () ->
            let run = create_exampleRun()
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
        )
        testCase "remove last" (fun () ->
            let run = create_exampleRun()
            run.RemoveTableAt(run.TableCount-1)
            Expect.equal run.TableCount 4 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name first"
            Expect.equal run.TableNames.[run.TableCount-1] "My Table 3" "Sheet Name last"
        )
        testCase "remove first" (fun () ->
            let run = create_exampleRun()
            run.RemoveTableAt(0)
            Expect.equal run.TableCount 4 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 1" "Sheet Name first"
            Expect.equal run.TableNames.[run.TableCount-1] "My Table 4" "Sheet Name last"
        )
        testCase "remove middle" (fun () ->
            let run = create_exampleRun()
            run.RemoveTableAt(2)
            Expect.equal run.TableCount 4 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name first"
            Expect.equal run.TableNames.[run.TableCount-1] "My Table 4" "Sheet Name last"
        )
    ]

let private tests_UpdateTableAt = 
    testList "UpdateTableAt" [
        testCase "ensure table" (fun () ->
            let run = create_exampleRun()
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
        )
        testCase "set on append index, throws" (fun () ->
            let run = create_exampleRun()
            let table = create_testTable()
            let eval() = run.UpdateTableAt(run.TableCount, table)
            Expect.throws eval ""
        )
        testCase "set on first" (fun () ->
            let index = 0
            let run = create_exampleRun()
            let table = create_testTable()
            run.UpdateTableAt(index, table)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[index] "Test" "Sheet Name changed"
            Expect.equal (run.GetTableAt(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (run.GetTableAt(index).RowCount) 5 "RowCount"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name last"
        )
        testCase "set on last" (fun () ->
            let run = create_exampleRun()
            let index = run.TableCount-1
            let table = create_testTable()
            run.UpdateTableAt(index, table)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[index] "Test" "Sheet Name changed"
            Expect.equal (run.GetTableAt(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (run.GetTableAt(index).RowCount) 5 "RowCount"
        )
        testCase "set on middle" (fun () ->
            let run = create_exampleRun()
            let index = 2
            let table = create_testTable()
            run.UpdateTableAt(index, table)
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[index] "Test" "Sheet Name changed"
            Expect.equal (run.GetTableAt(index).ColumnCount) 5 "ColumnCount"
            Expect.equal (run.GetTableAt(index).RowCount) 5 "RowCount"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name last"
        )
        testCase "set duplicate name" (fun () ->
            let run = create_exampleRun()
            let index = 2
            let table = create_testTable() |> IdentifierSetters.setArcTableName "My Table 0"
            let eval() = run.UpdateTableAt(index, table)
            Expect.throws eval "throws, duplicate name"
        )
    ]

let private tests_UpdateTable_Mutable = 
    testList "UpdateTable" [
        testCase "ensure table" (fun () ->
            let run = create_exampleRun()
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
        )
        testCase "set on append index, throws" (fun () ->
            let run = create_exampleRun()
            let eval() = run.MapTableAt(run.TableCount, fun table -> ())
            Expect.throws eval ""
        )
        testCase "update, AddColumn" (fun () ->
            let index = 1
            let run = create_exampleRun()
            run.MapTableAt(index, fun table ->
                table.AddColumn(column_input.Header, column_input.Cells)
            )
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[index] "My Table 1" "Sheet Name updated"
            Expect.equal (run.GetTableAt(index).ColumnCount) 1 "ColumnCount"
            Expect.equal (run.GetTableAt(index).RowCount) 5 "RowCount"
            Expect.equal (run.GetTableAt(index).Headers.[0]) (CompositeHeader.Input IOType.Source) "Header"
            Expect.equal (run.GetTableAt(index).Values.[(0,0)]) (CompositeCell.FreeText "Source_0") "Cell 0,0"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
        )
    ]

let private tests_updateTable_Copy = 
    testList "updateTableAt" [
        testCase "ensure table" (fun () ->
            let run = create_exampleRun()
            Expect.equal run.TableCount 5 "TableCount"
            Expect.equal run.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal run.TableNames.[4] "My Table 4" "Sheet Name"
        )
        testCase "set on append index, throws" (fun () ->
            let run = create_exampleRun()
            let eval() = run.MapTableAt(run.TableCount, fun table -> ())
            Expect.throws eval ""
        )
        testCase "update, AddColumn" (fun () ->
            let index = 1
            let run = create_exampleRun()
            let run_expected = create_exampleRun()
            let newRun = 
                run |> ArcRun.mapTableAt(index, fun table ->
                    table.AddColumn(column_input.Header, column_input.Cells)
                )
            Expect.equal newRun.TableCount 5 "TableCount"
            Expect.equal newRun.TableNames.[0] "My Table 0" "Sheet Name"
            Expect.equal newRun.TableNames.[index] "My Table 1" "Sheet Name updated"
            Expect.equal (newRun.GetTableAt(index).ColumnCount) 1 "ColumnCount"
            Expect.equal (newRun.GetTableAt(index).RowCount) 5 "RowCount"
            Expect.equal (newRun.GetTableAt(index).Headers.[0]) (CompositeHeader.Input IOType.Source) "Header"
            Expect.equal (newRun.GetTableAt(index).Values.[(0,0)]) (CompositeCell.FreeText "Source_0") "Cell 0,0"
            Expect.equal newRun.TableNames.[4] "My Table 4" "Sheet Name"
            Seq.iteri2 (fun i tabl0 table1 -> 
                Expect.equal tabl0 table1 $"equal {i}"
            ) run.Tables run_expected.Tables
        )
    ]

let private tests_technologyPlatform = 
    testList "technologyPlatformTests" [
        let name = "MyOntology"
        let nameWithParentheses = "MyOntology (Mine)"
        let tsr = "ABC"
        let tan = "ABC:123"
        let tp_Term = OntologyAnnotation(name,tsr,tan)
        let tp_TermWithParentheses = OntologyAnnotation(nameWithParentheses,tsr,tan)
        let tp_String = OntologyAnnotation(name)
        testCase "compose Term" (fun () ->
            let s = Process.Conversion.JsonTypes.composeTechnologyPlatform tp_Term
            Expect.equal s $"{name} ({tan})" "Term was not correctly composed as string."
        )
        testCase "decompose Term" (fun () ->
            let s = Process.Conversion.JsonTypes.composeTechnologyPlatform tp_Term
            let pt_new = Process.Conversion.JsonTypes.decomposeTechnologyPlatform s
            Expect.equal pt_new.NameText name "NameText should match"
            Expect.equal pt_new.TermAccessionShort tan "ShortTan should match"
        )
        testCase "compose Term With Parentheses" (fun () ->
            let s = Process.Conversion.JsonTypes.composeTechnologyPlatform tp_TermWithParentheses
            Expect.equal s $"{nameWithParentheses} ({tan})" "Term was not correctly composed as string."
        )
        testCase "decompose Term" (fun () ->
            let s = Process.Conversion.JsonTypes.composeTechnologyPlatform tp_TermWithParentheses
            let pt_new = Process.Conversion.JsonTypes.decomposeTechnologyPlatform s
            Expect.equal pt_new.NameText nameWithParentheses "NameText should match"
            Expect.equal pt_new.TermAccessionShort tan "ShortTan should match"
        )
        testCase "compose String" (fun () ->
            let s = Process.Conversion.JsonTypes.composeTechnologyPlatform tp_String
            Expect.equal s $"{name}" "String was not correctly composed as string."
        )
        testCase "decompose String" (fun () ->
            let s = Process.Conversion.JsonTypes.composeTechnologyPlatform tp_String
            let pt_new = Process.Conversion.JsonTypes.decomposeTechnologyPlatform s
            Expect.equal pt_new.NameText name "NameText should match"
            Expect.isNone pt_new.TermAccessionNumber "TermAccessionNumber should be None"
            Expect.equal pt_new.TermAccessionShort "" "ShortTan should match"
        )
    ]

let private tests_UpdateBy = testList "UpdateBy" [
    let create_testRun() = 
        ArcRun.create(
            "MyRun",
            title = "Best Run On Earth",
            description = "Run to be used for being the best",
            measurementType = OntologyAnnotation("MyMeasurementType"),
            technologyType = OntologyAnnotation("MyTechnologyType"),
            technologyPlatform = OntologyAnnotation("MyTechnologyPlatform"),
            workflowIdentifiers = ResizeArray [|"Workflow1"|],
            tables = ResizeArray([ArcTable.init("MyTable")]),
            performers = ResizeArray [|Person(firstName="Kevin", lastName="Frey")|],
            comments = ResizeArray [|Comment(name="CommentName", value="CommentValue")|]
        )
    testCase "UpdateBy, full replace" <| fun _ ->
        let actual = create_testRun()
        let next = 
            ArcRun.create(
                "NextRun",
                "Next Title",
                "Next Description",
                OntologyAnnotation("NextMeasurementType"),
                OntologyAnnotation("NextTechnologyType"),
                workflowIdentifiers = ResizeArray [|"Workflow2"|],
                tables= ResizeArray ([ArcTable.init("NextTable")]),
                performers= ResizeArray [|Person(firstName="NextKevin", lastName="NextFrey")|]
            )
        actual.UpdateBy(next)
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.MeasurementType next.MeasurementType "MeasurementType"
        Expect.equal actual.TechnologyType next.TechnologyType "TechnologyType"
        Expect.equal actual.TechnologyPlatform next.TechnologyPlatform "TechnologyPlatform"
        Expect.sequenceEqual actual.WorkflowIdentifiers next.WorkflowIdentifiers "WorkflowIdentifiers"
        Expect.sequenceEqual actual.Tables next.Tables "Tables"
        Expect.sequenceEqual actual.Performers next.Performers "Performers"
        Expect.sequenceEqual actual.Comments next.Comments "Comments"
    testCase "UpdateBy, replace existing" <| fun _ ->
        let actual = create_testRun()
        let next = 
            ArcRun.create(
                "NextRun",
                "Next Title",
                "Next Description",
                OntologyAnnotation("NextMeasurementType"),
                OntologyAnnotation("NextTechnologyType"),
                tables= ResizeArray ([ArcTable.init("NextTable")]),
                performers= ResizeArray [|Person(firstName="NextKevin", lastName="NextFrey")|]
            )
        let expected = create_testRun()
        actual.UpdateBy(next, true)
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.MeasurementType next.MeasurementType "MeasurementType"
        Expect.equal actual.TechnologyType next.TechnologyType "TechnologyType"
        Expect.equal actual.TechnologyPlatform expected.TechnologyPlatform "TechnologyPlatform"
        Expect.sequenceEqual actual.Tables next.Tables "Tables"
        Expect.sequenceEqual actual.Performers next.Performers "Performers"
        Expect.sequenceEqual actual.Comments expected.Comments "Comments"
    testCase "UpdateBy, replace existing, empty tables" <| fun _ ->
        let actual = create_testRun()
        let next = 
            ArcRun.create(
                "NextRun",
                "Next Title",
                "Next Description",
                OntologyAnnotation("NextMeasurementType"),
                OntologyAnnotation("NextTechnologyType"),
                performers= ResizeArray [|Person(firstName="NextKevin", lastName="NextFrey")|]
            )
        let expected = create_testRun()
        actual.UpdateBy(next, true)
        TestingUtils.Expect.sequenceEqual actual.Tables expected.Tables "Tables"
    testCase "UpdateBy, replace existing, append" <| fun _ ->
        let actual = create_testRun()
        let next = 
            ArcRun.create(
                "NextRun",
                "Next Title",
                "Next Description",
                OntologyAnnotation("NextMeasurementType"),
                OntologyAnnotation("NextTechnologyType"),
                workflowIdentifiers = ResizeArray [|"Workflow2"|],
                tables=ResizeArray([ArcTable.init("NextTable")]),
                performers= ResizeArray [|Person(firstName="NextKevin", lastName="NextFrey")|]
            )
        let expected = create_testRun()
        actual.UpdateBy(next, true, true)
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.MeasurementType next.MeasurementType "MeasurementType"
        Expect.equal actual.TechnologyType next.TechnologyType "TechnologyType"
        Expect.equal actual.TechnologyPlatform expected.TechnologyPlatform "TechnologyPlatform"
        Expect.sequenceEqual actual.WorkflowIdentifiers (ResizeArray([|"Workflow1"; "Workflow2"|])) "WorkflowIdentifiers"
        Expect.sequenceEqual actual.Tables (ResizeArray([yield! expected.Tables; yield! next.Tables])) "Tables"
        let exptectedPerformer = ResizeArray [|yield! expected.Performers; yield! next.Performers|]
        let expectedComments = ResizeArray [|yield! expected.Comments; yield! next.Comments|]
        Expect.sequenceEqual actual.Performers exptectedPerformer "Performers"
        Expect.sequenceEqual actual.Comments expectedComments "Comments"
    testCase "UpdateBy, replace all, append" <| fun _ ->
        let actual = create_testRun()
        let next = 
            ArcRun.create(
                "NextRun",
                "Next Title",
                "Next Description",
                OntologyAnnotation("NextMeasurementType"),
                OntologyAnnotation("NextTechnologyType"),
                workflowIdentifiers = ResizeArray [|"Workflow2"|],
                tables= ResizeArray ([ArcTable.init("NextTable")]),
                performers= ResizeArray [|Person(firstName="NextKevin", lastName="NextFrey")|]
            )
        let expected = create_testRun()
        actual.UpdateBy(next, false, true)
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.MeasurementType next.MeasurementType "MeasurementType"
        Expect.equal actual.TechnologyType next.TechnologyType "TechnologyType"
        Expect.equal actual.TechnologyPlatform next.TechnologyPlatform "TechnologyPlatform"
        Expect.sequenceEqual actual.WorkflowIdentifiers (ResizeArray([|"Workflow1"; "Workflow2"|])) "WorkflowIdentifiers"
        Expect.sequenceEqual actual.Tables (ResizeArray([yield! expected.Tables; yield! next.Tables])) "Tables"
        let expectedPerformers = ResizeArray [|yield! expected.Performers; yield! next.Performers|]
        let expectedComments = ResizeArray [|yield! expected.Comments; yield! next.Comments|]
        Expect.sequenceEqual actual.Performers expectedPerformers "Performers"
        Expect.sequenceEqual actual.Comments expectedComments "Comments"
]

let private tests_GetHashCode = testList "GetHashCode" [
    let createFullRun(name) =
        ArcRun.make 
            name
            ("Best Run On Earth" |> Some)
            ("Run to be used for being the best" |> Some)
            (OntologyAnnotation "mt" |> Some)
            (OntologyAnnotation "tt" |> Some)
            (OntologyAnnotation "tp" |> Some)
            (ResizeArray [|"Workflow1"; "Workflow2"|])
            (ResizeArray([ArcTable.init("My Table"); ArcTable.Tests.create_testTable()]))
            None
            (ResizeArray [|Person(firstName="John",lastName="Doe"); Person(firstName="Jane",lastName="Doe")|])
            (ResizeArray [|Comment("Hello", "World"); Comment("ByeBye", "World") |])
    testCase "passing" <| fun _ ->
        let actual = ArcRun.init("MyRun")
        Expect.isSome (actual.GetHashCode() |> Some) ""
    testCase "equal minimal" <| fun _ -> 
        let run = ArcRun.init("MyRun")
        let copy = run.Copy()
        let run2 = ArcRun.init("MyRun")
        Expect.equal run copy "equal"
        Expect.equal (run.GetHashCode()) (copy.GetHashCode()) "copy hash equal"
        Expect.equal (run.GetHashCode()) (run2.GetHashCode()) "run2 hash equal"
    testCase "equal" <| fun _ -> 
        let run = createFullRun "MyRun"
        let copy = run.Copy()
        Expect.equal (run.GetHashCode()) (copy.GetHashCode()) ""
    testCase "notEqual" <| fun _ ->
        let x1 = ArcRun.init("My run")
        let x2 = ArcRun.init("My other run")
        Expect.notEqual x1 x2 "not equal"
        Expect.notEqual (x1.GetHashCode()) (x2.GetHashCode()) "not equal hash"
    testCase "unequalWorkflowIdentifiers" <| fun _ ->
        let run1 = createFullRun "MyRun"
        let run2 = createFullRun "MyRun"
        run2.WorkflowIdentifiers.Add("Workflow3")
        Expect.notEqual (run1.GetHashCode()) (run2.GetHashCode()) "unequal hash"
]

let main = 
    testList "ArcRun" [
        test_create
        tests_AddTable
        tests_AddTables
        tests_Copy
        tests_RemoveTable
        tests_UpdateTableAt
        tests_UpdateTable_Mutable
        tests_updateTable_Copy
        tests_technologyPlatform
        tests_UpdateBy
        tests_GetHashCode
    ]