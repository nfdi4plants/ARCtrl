module ARCtrl.FileSystemHelper.Tests

open TestingUtils
open ARCtrl
open CrossAsync

let readFileText = 
    testList "ReadText" [
        testCaseCrossAsync "simple" (crossAsync {
            let p = TestObjects.IO.simpleTextFilePath
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
    ]

let writeFileText = 
    testList "WriteText" [
        testCaseCrossAsync "simple" (crossAsync {
            do! ARCtrl.FileSystemHelper.ensureDirectoryAsync TestObjects.IO.testResultsFolder
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "SimpleText.txt"
            let t = "Hello"
            printfn "write to %s" p
            do! FileSystemHelper.writeFileTextAsync p t
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
        testCaseCrossAsync "SubDirectoryWithEnsureDir" (crossAsync {
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

let getSubFiles = 
    testList "GetSubFiles" [
        testCaseCrossAsync "simple" (crossAsync {
            let p = TestObjects.IO.testSubPathsFolder
            let! result = FileSystemHelper.getSubFilesAsync p
            let expected = 
                [
                    $"{TestObjects.IO.testSubPathsFolder}/File1.txt" |> FileSystemHelper.standardizeSlashes
                    $"{TestObjects.IO.testSubPathsFolder}/File2.csv" |> FileSystemHelper.standardizeSlashes
                ]
            Expect.sequenceEqual result expected "Files were not found correctly."
        })
    ]

let getSubDirectories =
    testList "GetSubDirectories" [
        testCaseCrossAsync "simple" (crossAsync {
            let p = TestObjects.IO.testSubPathsFolder
            let! result = FileSystemHelper.getSubDirectoriesAsync p
            let expected = 
                [
                    $"{TestObjects.IO.testSubPathsFolder}/SubFolder" |> FileSystemHelper.standardizeSlashes
                ]
            Expect.sequenceEqual result expected "Directories were not found correctly."
        })
    ]


let getAllFilePaths =

    testList "GetAllFilePaths" [
        testCaseCrossAsync "simple" (crossAsync {
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
        getSubFiles
        getSubDirectories
        getAllFilePaths
    ]
