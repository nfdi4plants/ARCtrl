module ArcTable.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let private tests_member = 
    testList "Member" [
        let TableName = "Test"
        let table = ArcTable.init(TableName)
        testCase "ColumnCount empty" (fun () ->
            Expect.equal table.ColumnCount 0 "ColumnCount = 0"
        )
        testCase "RowCount empty" (fun () ->
            Expect.equal table.RowCount 0 "RowCount = 0"
        )
    ]

let private tests_addColumn =
    testList "addColumn" [
        let TableName = "Test"
        let oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
        let oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
        let oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
        let oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
        let header_io = CompositeHeader.Input IOType.Source
        let header_chara = CompositeHeader.Characteristic oa_species
        let header_component = CompositeHeader.Component oa_instrumentModel
        testList "New Table" [
            let table = ArcTable.init(TableName)
            testCase "IO column, no cells" (fun () ->
                let header = header_io
                let newTable = table.addColumn(header)
                Expect.equal newTable.RowCount 0 "RowCount = 0"
                Expect.equal newTable.ColumnCount 1 "ColumnCount = 1"
                Expect.equal newTable.ValueHeaders.[0] header "header"
            )
            testCase "term column, no cells" (fun () ->
                let header = header_chara
                let newTable = table.addColumn(header)
                Expect.equal newTable.RowCount 0 "RowCount = 0"
                Expect.equal newTable.ColumnCount 1 "ColumnCount = 1"
                Expect.equal newTable.ValueHeaders.[0] header "header"
            )
            testCase "IO column, with cells" (fun () ->
                let header = header_io
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable = table.addColumn(header, cells)
                Expect.equal newTable.RowCount 5 "RowCount"
                Expect.equal newTable.ColumnCount 1 "ColumnCount"
                Expect.equal newTable.ValueHeaders.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in Array.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.mySequenceEqual newTable.Values expected "values"
            )
            testCase "term column, with cells" (fun () ->
                let header = header_chara
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable = table.addColumn(header, cells)
                Expect.equal newTable.RowCount 5 "RowCount"
                Expect.equal newTable.ColumnCount 1 "ColumnCount"
                Expect.equal newTable.ValueHeaders.[0] header "header"
                let expected = 
                    let m = [ for rowIndex, cell in Array.indexed cells do yield (0, rowIndex), cell] |> Map.ofList
                    System.Collections.Generic.Dictionary<int*int,CompositeCell>(m)
                TestingUtils.mySequenceEqual newTable.Values expected "values"
            )
            testCase "IO column, with wrong cells" (fun () ->
                let header = header_io
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable() = table.addColumn(header, cells) |> ignore
                Expect.throws newTable ""
            )
            testCase "term column, with wrong cells" (fun () ->
                let header = header_chara
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable() = table.addColumn(header, cells) |> ignore
                Expect.throws newTable ""
            )
            testCase "IO column, with cells at index" (fun () ->
                let header = header_io
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable = table.addColumn(header, cells, 0)
                Expect.equal newTable.RowCount 5 "RowCount"
                Expect.equal newTable.ColumnCount 1 "ColumnCount"
            )
            testCase "term column, with cells at index" (fun () ->
                let header = header_chara
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable = table.addColumn(header, cells, 0)
                Expect.equal newTable.RowCount 5 "RowCount"
                Expect.equal newTable.ColumnCount 1 "ColumnCount"
            )
            testCase "IO column, with cells at outside index" (fun () ->
                let header = header_io
                let cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                let newTable() = table.addColumn(header, cells, 1) |> ignore
                Expect.throws newTable ""
            )
            testCase "term column, with cells at outside index" (fun () ->
                let header = header_chara
                let cells = Array.init 5 (fun _ -> CompositeCell.createTerm oa_chlamy)
                let newTable() = table.addColumn(header, cells, 1) |> ignore
                Expect.throws newTable ""
            )
        ]
        testList "Existing Table" [
            /// Table contains 5 rows and 2 columns
            let createCells_chara (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
            let table = 
                let io_cells = Array.init 5 (fun i -> CompositeCell.createFreeText  $"Source_{i}")
                ArcTable
                    .init(TableName)
                    .addColumn(header_io, io_cells)
            testCase "Ensure base table" (fun () ->
                Expect.equal table.ColumnCount 1 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
            )
            testCase "add equal rows" (fun () ->
                let table = table.addColumn(header_chara, createCells_chara 5)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ValueHeaders.[0] header_io "Header 0"
                Expect.equal table.ValueHeaders.[1] header_chara "Header 1"
            )
            testCase "add less rows" (fun () ->
                let cells = createCells_chara 2
                let table = table.addColumn(header_chara, cells)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ValueHeaders.[0] header_io "Header 0"
                Expect.equal table.ValueHeaders.[1] header_chara "Header 1"
                let expected = 
                    Array.init 5 (fun i -> 
                        let c = if i <= 1 then CompositeCell.createTerm oa_chlamy else CompositeCell.emptyTerm
                        System.Collections.Generic.KeyValuePair((1,i), c) 
                    )
                let actual = Array.ofSeq table.Values
                Expect.containsAll actual expected "extendedValues"
            )
            testCase "add more rows" (fun () ->
                let cells = createCells_chara 8
                let table = table.addColumn(header_chara, cells)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 8 "RowCount"
                Expect.equal table.ValueHeaders.[0] header_io "Header 0"
                Expect.equal table.ValueHeaders.[1] header_chara "Header 1"
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
                let cells = createCells_chara 5
                let table = table.addColumn(header_chara, cells, 0)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ValueHeaders.[0] header_chara "Header chara"
                Expect.equal table.ValueHeaders.[1] header_io "Header io"
            )
            testCase "add less rows, insert at" (fun () ->
                let cells = createCells_chara 2
                let table = table.addColumn(header_chara, cells, 0)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 5 "RowCount"
                Expect.equal table.ValueHeaders.[0] header_chara "Header chara"
                Expect.equal table.ValueHeaders.[1] header_io "Header io"
                let expected = 
                    Array.init 5 (fun i -> 
                        let c = if i <= 1 then CompositeCell.createTerm oa_chlamy else CompositeCell.emptyTerm
                        System.Collections.Generic.KeyValuePair((0,i), c) 
                    )
                let actual = Array.ofSeq table.Values
                Expect.containsAll actual expected "extendedValues"
            )
            testCase "add more rows, insert at" (fun () ->
                let cells = createCells_chara 8
                let table = table.addColumn(header_chara, cells, 0)
                Expect.equal table.ColumnCount 2 "ColumnCount"
                Expect.equal table.RowCount 8 "RowCount"
                Expect.equal table.ValueHeaders.[0] header_chara "Header chara"
                Expect.equal table.ValueHeaders.[1] header_io "Header io"
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
        ]
    ]

let main = 
    testList "ArcTable" [
        tests_member
        tests_addColumn
    ]