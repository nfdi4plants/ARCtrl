module ARCtrl.FileSystemHelper.Tests

open TestingUtils
open ARCtrl
open System.Text.Json
open CrossAsync

let readFileText = 
    testList "ReadText" [
        testCaseAsync "simple" (crossAsync {
            let p = TestObjects.IO.simpleTextFilePath
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
    ]

let writeFileText = 
    testList "WriteText" [
        testCaseAsync "simple" (crossAsync {
            do! ARCtrl.FileSystemHelper.ensureDirectoryAsync TestObjects.IO.testResultsFolder
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "SimpleText.txt"
            let t = "Hello"
            printfn "write to %s" p
            do! FileSystemHelper.writeFileTextAsync p t
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
        testCaseAsync "SubDirectoryWithEnsureDir" (crossAsync {
            let subDir = ArcPathHelper.combine TestObjects.IO.testResultsFolder "SubFolder"
            let p = ArcPathHelper.combine subDir "SimpleText.txt"
            let t = "Hello"
            printfn "write to %s" p
            do! FileSystemHelper.ensureDirectoryAsync subDir
            do! FileSystemHelper.writeFileTextAsync p t
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
    ]

let getAllFilePaths =

    testList "GetAllFilePaths" [
        testCaseAsync "simple" (crossAsync {
            let p = TestObjects.IO.testSubPathsFolder
            let! result =
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort
            let expected = 
                [
                    "/File1.txt"
                    "/File2.csv"
                    "/SubFolder/File3.xlsx"
                    "/SubFolder/SubSubFolder/File4"
                ]
            Expect.sequenceEqual result expected "File Paths were not found correctly."
            
        })
    ]

let main = 
    testList "PathTests" [
        readFileText
        writeFileText
        getAllFilePaths
    ]
