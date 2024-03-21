module Tests.Performance

open ARCtrl
open FsSpreadsheet
open TestingUtils
open ARCtrl.Spreadsheet

let private tests_Study = testList "Study" [
    testCase "LargeWorkbook" <| fun _ ->
        let fswb = TestObjects.Spreadsheet.Study.LargeFile.Workbook
        let convertToArcFile(fswb:FsWorkbook) =
            let timer_start = System.DateTime.Now
            let s,_ = ArcStudy.fromFsWorkbook fswb
            let timer_end = System.DateTime.Now
            let runtime = (timer_end - timer_start).Milliseconds
            #if FABLE_COMPILER_PYTHON
            let expectedMs = 1500
            #else
            let expectedMs = 300 // this is too high and should be reduced
            #endif
            Expect.equal s.TableCount 1 "Table count"
            Expect.isTrue (runtime <= expectedMs) $"Expected conversion to be finished in under {expectedMs}, but it took {runtime}"
        convertToArcFile fswb
]

let private tests_Investigation = testList "Investigation" [
    testCase "WriteManyStudies" <| fun _ ->
        let inv = ArcInvestigation.init("MyInvestigation")
        for i = 0 to 1500 do 
            let s = ArcStudy.init($"Study{i}")
            inv.AddRegisteredStudy(s)
        let testF = fun () -> ArcInvestigation.toFsWorkbook inv
        #if FABLE_COMPILER_PYTHON
        let expectedMs = 50000
        #else
        let expectedMs = 1000
        #endif
        let wb = Expect.wantFaster testF expectedMs "Parsing investigation to Workbook is too slow"    
        Expect.equal (wb.GetWorksheets().Count) 1 "Worksheet count"
]

let Main = testList "Performance" [
    tests_Study
    tests_Investigation
]