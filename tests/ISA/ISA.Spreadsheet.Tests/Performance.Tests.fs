module Tests.Performance

open ARCtrl.ISA
open FsSpreadsheet
open TestingUtils
open ARCtrl.ISA.Spreadsheet

let private tests_Study = testList "Study" [
    testCase "LargeWorkbook" <| fun _ ->
        let fswb = TestObjects.Spreadsheet.Study.LargeFile.Workbook
        let convertToArcFile(fswb:FsWorkbook) =
            let timer_start = System.DateTime.Now
            let s,_ = ArcStudy.fromFsWorkbook fswb
            let timer_end = System.DateTime.Now
            let runtime = (timer_end - timer_start).Milliseconds
            let expected = 300 // this is too high and should be reduced
            Expect.equal s.TableCount 1 "Table count"
            Expect.isTrue (runtime <= expected) $"Expected conversion to be finished in under {expected}, but it took {runtime}"
        convertToArcFile fswb
]

let Main = testList "Performance" [
    tests_Study
]