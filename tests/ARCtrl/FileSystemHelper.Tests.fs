module ARCtrl.FileSystemHelper.Tests

open TestingUtils
open ARCtrl
open CrossAsync
open ARCtrl.ArcPathHelper

let fileExists =
    testList "FileExists" [
        testCaseCrossAsync "simple" (crossAsync {
            let p = TestObjects.IO.simpleTextFilePath
            let! result = FileSystemHelper.fileExistsAsync p
            Expect.isTrue result "File does not exist."
        })
        testCaseCrossAsync "nonExisting" (crossAsync {
            let p = "bdlieihawdbawndfefnsefsfnse.dawd"
            let! result = FileSystemHelper.fileExistsAsync p
            Expect.isFalse result "File exists."
        })
    ]

let directoryExists =
    testList "DirectoryExists" [
        testCaseCrossAsync "simple" (crossAsync {
            let p = TestObjects.IO.testObjectsBaseFolder
            let! result = FileSystemHelper.directoryExistsAsync p
            Expect.isTrue result "Directory does not exist."
        })
        testCaseCrossAsync "nonExisting" (crossAsync {
            let p = "bdlieihawdbawndfefnsefsfnse"
            let! result = FileSystemHelper.directoryExistsAsync p
            Expect.isFalse result "Directory exists."
        })
    ]

let ensureDirectoryOfFile =
    testList "EnsureDirectoryOfFile" [
        testCaseCrossAsync "simple" (crossAsync {
            let directoryPath = combine TestObjects.IO.testResultsFolder "EnsuredDirectory"
            let filePath = combine directoryPath "EnsuredFile.txt"

            do! FileSystemHelper.deleteFileOrDirectoryAsync directoryPath // how about deleting it inside the test instead of build action if you require it to not exist one here @hlweil?? ~Kevin Frey
            let! directoryExistsBefore = FileSystemHelper.directoryExistsAsync directoryPath
            Expect.isFalse directoryExistsBefore "Directory should not exist before."

            do! FileSystemHelper.ensureDirectoryOfFileAsync filePath

            let! directoryExistsAfter = FileSystemHelper.directoryExistsAsync directoryPath
            Expect.isTrue directoryExistsAfter "Directory should exist after."
        })
    ]

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
            do! FileSystemHelper.writeFileTextAsync p t
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
        testCaseCrossAsync "SubDirectoryWithEnsureDir" (crossAsync {
            let subDir = ArcPathHelper.combine TestObjects.IO.testResultsFolder "SubFolder"
            let p = ArcPathHelper.combine subDir "SimpleText.txt"
            let t = "Hello"
            do! FileSystemHelper.ensureDirectoryAsync subDir
            do! FileSystemHelper.writeFileTextAsync p t
            let! result = FileSystemHelper.readFileTextAsync p
            let expected = "Hello"
            Expect.equal result expected "Text was not read correctly."
        })
    ]

let readFileXlsx = 
    testList "ReadWorkbook" [
        testCaseCrossAsync "simple" (crossAsync {
            let p = TestObjects.IO.simpleWorkbookPath
            let! result = FileSystemHelper.readFileXlsxAsync p
            let ws = Expect.wantSome (result.TryGetWorksheetByName "TestSheet") "Workbook does not contain worksheet"
            let row1 = Expect.wantSome (ws.TryGetRowValuesAt 1) "Worksheet does not contain row 1"
            let row1AsInts = row1 |> Seq.map (string >> int)
            let expected = [1;2;3]
            Expect.sequenceEqual row1AsInts expected "Worksheet does not contain correct values"
            let row2 = Expect.wantSome (ws.TryGetRowValuesAt 2) "Worksheet does not contain row 2"
            let expected = ["A";"B";"C"] |> Seq.map box
            Expect.sequenceEqual row2 expected "Worksheet does not contain correct values"

        })
    ]

let writeFileXlsx = 
    testList "WriteWorkbook" [
        testCaseCrossAsync "simple" (crossAsync {
            do! ARCtrl.FileSystemHelper.ensureDirectoryAsync TestObjects.IO.testResultsFolder
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "Workbook.xlsx"
            let wb = TestObjects.Spreadsheet.Investigation.BII_I_1.fullInvestigation           
            do! FileSystemHelper.writeFileXlsxAsync p wb
            let! result = FileSystemHelper.readFileXlsxAsync p
            Expect.workBookEqual wb result "Workbook was not read correctly."
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
            Expect.pathSequenceEqual result expected "Files were not found correctly."
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
            Expect.pathSequenceEqual result expected "Directories were not found correctly."
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
                    $"/File1.txt"
                    $"/File2.csv"
                    $"/SubFolder/File3.xlsx" |> FileSystemHelper.standardizeSlashes
                    $"/SubFolder/SubSubFolder/File4" |> FileSystemHelper.standardizeSlashes
                ]
            Expect.pathSequenceEqual result expected "File Paths were not found correctly."
            
        })
    ]

let rename = 
    testList "Rename" [
        testCaseCrossAsync "simpleFile" (crossAsync {
            let oldFileName = "OldFile.txt"
            let newFileName = "NewFile.txt"
            let oldFilePath = ArcPathHelper.combine TestObjects.IO.testResultsFolder oldFileName
            let newFilePath = ArcPathHelper.combine TestObjects.IO.testResultsFolder newFileName
            do! FileSystemHelper.deleteFileOrDirectoryAsync oldFileName
            do! FileSystemHelper.deleteFileOrDirectoryAsync newFilePath
            let text = "Hello"

            do! FileSystemHelper.writeFileTextAsync oldFilePath text
            do! FileSystemHelper.renameFileOrDirectoryAsync oldFilePath newFilePath

            let! oldFileExists = FileSystemHelper.fileExistsAsync oldFilePath
            Expect.isFalse oldFileExists "Old file still exists."
            let! result = FileSystemHelper.fileExistsAsync newFilePath
            Expect.isTrue result "File does not exist."
        })
        testCaseCrossAsync "simpleFolder" (crossAsync {
            let oldFolderName = "OldFolder"
            let newFolderName = "NewFolder"
            let oldFolderPath = ArcPathHelper.combine TestObjects.IO.testResultsFolder oldFolderName
            let newFolderPath = ArcPathHelper.combine TestObjects.IO.testResultsFolder newFolderName
            do! FileSystemHelper.deleteFileOrDirectoryAsync oldFolderPath
            do! FileSystemHelper.deleteFileOrDirectoryAsync newFolderPath
            let oldFilePath = ArcPathHelper.combine oldFolderPath "File.txt"
            let newFilePath = ArcPathHelper.combine newFolderPath "File.txt"
            let text = "Hello"
            do! FileSystemHelper.ensureDirectoryAsync oldFolderPath
            do! FileSystemHelper.writeFileTextAsync oldFilePath text
            do! FileSystemHelper.renameFileOrDirectoryAsync oldFolderPath newFolderPath

            let! oldFolderExists = FileSystemHelper.directoryExistsAsync oldFolderPath
            Expect.isFalse oldFolderExists "Old folder still exists."
            let! newFolderExists = FileSystemHelper.directoryExistsAsync newFolderPath
            Expect.isTrue newFolderExists "Folder does not exist."
            let! result = FileSystemHelper.readFileTextAsync newFilePath
            Expect.equal result text "Text was not read correctly."

        })
    ]

let main = 
    testList "PathTests" [
        fileExists
        directoryExists
        ensureDirectoryOfFile
        readFileText
        writeFileText
        readFileXlsx
        writeFileXlsx
        getSubFiles
        getSubDirectories
        getAllFilePaths
        rename
    ]
