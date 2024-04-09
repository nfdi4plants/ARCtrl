module Arc.Tests

open Expecto
open ARCtrl.NET
open System.IO
open ARCtrl

let testInputFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects")
let testOutputFolder = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestResults")

let testLoad =

    testList "Load" [
        testCase "simpleARC" (fun () -> 
            let p = System.IO.Path.Combine(testInputFolder,"ARC_SimpleARC")
            let result = ARC.load(p)
            
            Expect.isSome result.ISA "Should contain an ISA part"
            Expect.isNone result.CWL "Should not contain a CWL part"

            let isa = result.ISA.Value
            Expect.equal isa.StudyCount 1 "Should contain 1 study"
            Expect.equal isa.AssayCount 1 "Should contain 1 assay"
            Expect.equal isa.RegisteredStudies.Count 1 "Should contain 1 registered study"
            
            let s = isa.Studies.[0]
            Expect.equal s.RegisteredAssayCount 1 "Should contain 1 registered assay"
            Expect.equal s.TableCount 3 "Study should contain 3 tables"

            let a = s.RegisteredAssays.[0]
            Expect.equal a.TableCount 4 "Assay should contain 4 tables"
            
        )
    ]


let testWrite =

    testList "Write" [
        testCase "empty" (fun () -> 
            let p = System.IO.Path.Combine(testOutputFolder,"ARC_Write_Empty")
            let a = ARC()
            a.Write(p)

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/assays/.gitkeep";
                    "/studies/.gitkeep";
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort


            let paths = 
                Path.getAllFilePaths p
                |> Array.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."            
        )
        testCase "SimpleARC" (fun () -> 
            let p = Path.Combine(testOutputFolder,"ARC_Write_SimpleARC")
            let arc = ARC()

            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()
            arc.Write(p)

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{studyName}/isa.study.xlsx"
                    $"/studies/{studyName}/README.md"
                    $"/studies/{studyName}/protocols/.gitkeep";
                    $"/studies/{studyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort


            let paths = 
                Path.getAllFilePaths p
                |> Array.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."            
        )
        testCase "LoadSimpleARCAndAddAssay" (fun () -> 
            let p = System.IO.Path.Combine(testOutputFolder,"ARC_Write_SimpleARC")
            let arc = ARC.load(p)

            let i = arc.ISA.Value

            let existingStudyName = "MyStudy"
            let existingAssayName = "MyAssay"

            let assayName = "YourAssay"
            i.InitAssay(assayName) |> ignore
            arc.ISA <- Some i

            arc.UpdateFileSystem()
            arc.Write(p)

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{existingStudyName}/isa.study.xlsx"
                    $"/studies/{existingStudyName}/README.md"
                    $"/studies/{existingStudyName}/protocols/.gitkeep";
                    $"/studies/{existingStudyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{existingAssayName}/isa.assay.xlsx"
                    $"/assays/{existingAssayName}/README.md"
                    $"/assays/{existingAssayName}/protocols/.gitkeep"
                    $"/assays/{existingAssayName}/dataset/.gitkeep"
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort


            let paths = 
                Path.getAllFilePaths p
                |> Array.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."            
        )
        |> testSequenced
    ]

[<Tests>]
let main = 
    testList "ARC_Tests" [
        testLoad
        testWrite
    ]