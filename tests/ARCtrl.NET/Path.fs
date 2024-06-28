module Path.Tests

open Expecto
open ARCtrl.NET
open System.Text.Json


let getAllFilePaths =

    testList "GetAllFilePaths" [
        testCase "simple" (fun () -> 
            let p = System.IO.Path.Combine(__SOURCE_DIRECTORY__,@"TestObjects\Path_findSubPaths")
            let result = Path.getAllFilePaths p
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

[<Tests>]
let main = 
    testList "PathTests" [
        getAllFilePaths
    ]
