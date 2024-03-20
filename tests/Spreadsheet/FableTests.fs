module FableTests

open ARCtrl.ISA.Spreadsheet
open FsSpreadsheet

open TestingUtils

let tests_typeTranspilation = testList "type transpilation" [
    testCase "FsWorkbook" <| fun _ ->
        let wb = new FsWorkbook()
        Expect.isTrue true ""
    testCase "FsWorksheet" <| fun _ ->
        let wb = new FsWorkbook()
        let ws = FsWorksheet.init ("My Worksheet")
        wb.AddWorksheet(ws)
        Expect.equal (wb.GetWorksheets().Count) 1 "length"
        Expect.equal (wb.GetWorksheets().[0].Name) "My Worksheet" "length"
    testCase "FsTable" <| fun _ ->
        let wb = new FsWorkbook()
        let ws = FsWorksheet.init ("My Worksheet")
        wb.AddWorksheet(ws)
        Expect.equal (wb.GetWorksheets().Count) 1 "length"
        Expect.equal (wb.GetWorksheets().[0].Name) "My Worksheet" "length"
        let table = FsTable("MyTable",FsRangeAddress(FsAddress(1,1),FsAddress(5,5)))
        ws.AddTable(table) |> ignore
        Expect.equal (wb.GetTables().Length) 1 "table length"
        Expect.equal (wb.GetTables().[0].Name) "MyTable" "table name"
]


let main = testList "FableTests" [
    tests_typeTranspilation
]