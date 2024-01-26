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

let private tests_Investigation = testList "Investigation" [
    testCase "WriteManyStudies" <| fun _ ->
        let inv = ArcInvestigation.init("MyInvestigation")
        for i = 0 to 1500 do 
            let s = ArcStudy.init($"Study{i}")
            inv.AddRegisteredStudy(s)
        let timer = Stopwatch()
        timer.Start()
        let wb = ArcInvestigation.toFsWorkbook inv
        timer.Stop()
        let runtime = timer.Elapsed.Milliseconds
        let expected = 1000 // this is too high and should be reduced

        Expect.equal (wb.GetWorksheets().Count) 1 "Worksheet count"
        Expect.isTrue (runtime <= expected) $"Expected conversion to be finished in under {expected}, but it took {runtime}"
]

let Main = testList "Performance" [
    tests_Study
    tests_Investigation
]