module ARCtrl.FileSystemHelper.Tests

open TestingUtils
open ARCtrl
open System.Text.Json


let readFileText = 
    testList "ReadText" [
        ftestCase "simple" (fun () -> 
            let p = TestObjects.IO.simpleTextFilePath
            let result = FileSystemHelper.readFileText p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        )
    ]

let writeFileText = 
    testList "WriteText" [
        ftestCase "simple" (fun () ->
            ARCtrl.FileSystemHelper.ensureDirectory TestObjects.IO.testResultsFolder
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "SimpleText.txt"
            let t = "Hello"
            printfn "write to %s" p
            FileSystemHelper.writeFileText p t
            let result = FileSystemHelper.readFileText p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        )
        ftestCase "SubDirectoryWithEnsureDir" (fun () ->
            let subDir = ArcPathHelper.combine TestObjects.IO.testResultsFolder "SubFolder"
            let p = ArcPathHelper.combine subDir "SimpleText.txt"
            let t = "Hello"
            printfn "write to %s" p
            FileSystemHelper.ensureDirectory subDir
            FileSystemHelper.writeFileText p t
            let result = FileSystemHelper.readFileText p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        )
    ]

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
        readFileText
        writeFileText
        getAllFilePaths
    ]
