module ARCtrl.FileSystemHelper.Tests

open TestingUtils
open ARCtrl
open System.Text.Json


let getAllFilePaths =

    testList "GetAllFilePaths" [
        testCase "simple" (fun () -> 
            let p = TestObjects.IO.testSubPathsFolder
            let result = FileSystemHelper.getAllFilePaths p
            let expected = 
                [
                    "/File1.txt"
                    "/File2.csv"
                    "/SubFolder/File3.xlsx"
                    "/SubFolder/SubSubFolder/File4"
                ]
            Expect.sequenceEqual result expected "File Paths were not found correctly."
            
        )
    ]

let main = 
    testList "PathTests" [
        getAllFilePaths
    ]
