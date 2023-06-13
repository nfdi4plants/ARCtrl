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
        testCase "ColumnCount empty" (fun () ->
            let table = ArcTable.init(TableName)
            Expect.equal table.RowCount 0 "RowCount = 0"
            Expect.equal table.ColumnCount 0 "ColumnCount = 0"
        )
    ]

let main = 
    testList "ArcTable" [
        tests_member
    ]