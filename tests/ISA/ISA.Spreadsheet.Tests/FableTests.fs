module FableTests

open ISA.Spreadsheet
open FsSpreadsheet

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif


let tests_typeTranspilation = testList "type transpilation" [
    testCase "FsWorkbook" <| fun _ ->
        let wb = new FsWorkbook()
        Expect.isTrue true ""
    testCase "FsWorksheet" <| fun _ ->
        let wb = new FsWorkbook()
        let ws = new FsWorksheet()
        ws.Name <- "My Worksheet"
        wb.AddWorksheet(ws)
        Expect.equal (wb.GetWorksheets().Length) 1 "length"
        Expect.equal (wb.GetWorksheets().[1].Name) "My Worksheet" "length"
    testCase "FsTable" <| fun _ ->
        let wb = new FsWorkbook()
        let ws = new FsWorksheet()
        ws.Name <- "My Worksheet"
        wb.AddWorksheet(ws)
        Expect.equal (wb.GetWorksheets().Length) 1 "length"
        Expect.equal (wb.GetWorksheets().[1].Name) "My Worksheet" "length"
        let table = FsTable("My Table",FsRangeAddress(FsAddress(1,1),FsAddress(5,5)))
        ws.AddTable(table) |> ignore
        Expect.equal (wb.GetTables().Length) 1 "table length"
        Expect.equal (wb.GetTables().[1].Name) "My Table" "table name"
]


let main = 
    testList "SparseTable" [
]