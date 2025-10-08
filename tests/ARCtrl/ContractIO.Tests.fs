module ARCtrl.ContractIO.Tests

open TestingUtils
open ARCtrl
open ARCtrl.Contract
open ARCtrl.FileSystemHelper
open FsSpreadsheet
open CrossAsync

let testRead =

    testList "Read" [
        testCaseCrossAsync "TextFile" (crossAsync {
            let fileName = "TestReadMe.txt"
            let contract = Contract.createRead(fileName,DTOType.PlainText)
            let dto = DTO.Text "This is a test"
            let expected = 
                {contract with DTO = Some dto}
            let! result = fulfillReadContractAsync TestObjects.IO.testContractsFolder contract
            let resultContract = Expect.wantOk result "Contract was not fulfilled correctly"
            Expect.equal resultContract expected $"Text was not read correctly"
        })
        testCaseCrossAsync "XLSXFile" (crossAsync {
            let fileName = "TestWorkbook.xlsx"
            let contract = Contract.createRead(fileName,DTOType.ISA_Study)
            let! result = fulfillReadContractAsync TestObjects.IO.testContractsFolder contract
            let resultContract = Expect.wantOk result "Contract was not fulfilled correctly"
            let dto = Expect.wantSome resultContract.DTO "DTO was not read correctly"
            Expect.isTrue dto.isSpreadsheet "DTO was not read correctly"
            let wb = dto.AsSpreadsheet() :?> FsSpreadsheet.FsWorkbook
            let ws = Expect.wantSome (wb.TryGetWorksheetByName "TestSheet") "Workbook does not contain worksheet"
            let row1 = Expect.wantSome (ws.TryGetRowValuesAt 1) "Worksheet does not contain row 1"
            let row1AsInts = row1 |> Seq.map (string >> int)
            let expected = [1;2;3]
            Expect.sequenceEqual row1AsInts expected "Worksheet does not contain correct values"
            let row2 = Expect.wantSome (ws.TryGetRowValuesAt 2) "Worksheet does not contain row 2"
            let expected = ["A";"B";"C"] |> Seq.map box
            Expect.sequenceEqual row2 expected "Worksheet does not contain correct values"      
        })
    ]


let testWrite =

    testList "Write" [
        testCaseCrossAsync "TextFileEmpty" (crossAsync {
            let fileName = "TestEmpty.txt"
            let contract = Contract.createCreate(fileName,DTOType.PlainText)

            do! FileSystemHelper.ensureDirectoryAsync TestObjects.IO.testResultsFolder

            let! resultContract = fulfillWriteContractAsync TestObjects.IO.testResultsFolder contract

            Expect.isOk resultContract "Contract was not fulfilled correctly"

            let filePath = ArcPathHelper.combine TestObjects.IO.testResultsFolder fileName
            let! fileExists = FileSystemHelper.fileExistsAsync filePath
            Expect.isTrue fileExists $"File {filePath} was not created"
            let! resultText = FileSystemHelper.readFileTextAsync filePath
            Expect.equal resultText "" $"File {filePath} was not empty"
        })
        testCaseCrossAsync "TextFile" (crossAsync {
            let testText = "This is a test"
            let fileName = "TestReadMe.txt"
            let dto = DTO.Text testText
            let contract = Contract.createCreate(fileName,DTOType.PlainText, dto)

            do! FileSystemHelper.ensureDirectoryAsync TestObjects.IO.testResultsFolder

            let! resultContract = fulfillWriteContractAsync TestObjects.IO.testResultsFolder contract

            Expect.isOk resultContract "Contract was not fulfilled correctly"

            let filePath = ArcPathHelper.combine TestObjects.IO.testResultsFolder fileName
            let! fileExists = FileSystemHelper.fileExistsAsync filePath
            Expect.isTrue fileExists $"File {filePath} was not created"

            let! resultText = FileSystemHelper.readFileTextAsync filePath
            Expect.equal resultText testText $"File {filePath} was not empty"
        })
        testCaseCrossAsync "XLSXFile" (crossAsync { 

            let worksheetName = "TestSheet"
            let testWB = new FsWorkbook()
            let testSheet = testWB.InitWorksheet (worksheetName)
            testSheet.Row(1).Item(1).Value <- "A1"
            testSheet.Row(1).Item(2).Value <- "B1"
            testSheet.Row(1).Item(3).Value <- "C1"
            let fileName = "TestWorkbook.xlsx"
            let dto = DTO.Spreadsheet testWB
            let contract = Contract.createCreate(fileName,DTOType.ISA_Assay,dto)

            do! FileSystemHelper.ensureDirectoryAsync TestObjects.IO.testResultsFolder

            let! resultContract = fulfillWriteContractAsync TestObjects.IO.testResultsFolder contract

            Expect.isOk resultContract "Contract was not fulfilled correctly"

            let filePath = ArcPathHelper.combine TestObjects.IO.testResultsFolder fileName
            
            let! wb = FileSystemHelper.readFileXlsxAsync filePath
            let ws = Expect.wantSome (wb.TryGetWorksheetByName worksheetName) "Workbook does not contain worksheet"
            let row1 = Expect.wantSome (ws.TryGetRowValuesAt 1) "Worksheet does not contain row 1"
            let expected = ["A1";"B1";"C1"] |> Seq.map box
            Expect.sequenceEqual row1 expected "Worksheet does not contain correct values"      
        })
    ]

let testRename =

    testList "Rename" [
        testCaseCrossAsync "Text" (crossAsync {
            let oldFileName = "ContractTestOld.txt"
            let newFileName = "ContractTestNew.txt"
            let content = "This is a test"
            let oldPath = ArcPathHelper.combine TestObjects.IO.testResultsFolder oldFileName
            let newPath = ArcPathHelper.combine TestObjects.IO.testResultsFolder newFileName
            do! FileSystemHelper.deleteFileOrDirectoryAsync oldPath
            do! FileSystemHelper.deleteFileOrDirectoryAsync newPath
            do! FileSystemHelper.writeFileTextAsync oldPath content

            let contract = Contract.createRename(oldFileName,newFileName)

            let! resultContract = fullfillRenameContractAsync TestObjects.IO.testResultsFolder contract

            Expect.isOk resultContract "Contract was not fulfilled correctly"

            let! oldFileExists = FileSystemHelper.fileExistsAsync oldPath
            Expect.isFalse oldFileExists $"File {oldPath} was not deleted"

            let! newFileExists = FileSystemHelper.fileExistsAsync newPath
            Expect.isTrue newFileExists $"File {newPath} was not created"
        })
    ]

let testRemove =

    testList "Remove" [
        testCaseCrossAsync "Text" (crossAsync {
            let fileName = "TestRemove.txt"
            let content = "This is a test"
            let filePath = ArcPathHelper.combine TestObjects.IO.testResultsFolder fileName
            do! FileSystemHelper.writeFileTextAsync filePath content

            let! fileExistsBeforeDelete = FileSystemHelper.fileExistsAsync filePath
            Expect.isTrue fileExistsBeforeDelete $"File {filePath} was not created"

            let contract = Contract.createDelete(fileName)

            let! resultContract = fullfillDeleteContractAsync TestObjects.IO.testResultsFolder contract

            Expect.isOk resultContract "Contract was not fulfilled correctly"

            let! fileExistsAferDelete = FileSystemHelper.fileExistsAsync filePath
            Expect.isFalse fileExistsAferDelete $"File {filePath} was not deleted"
        })
    ]

let main = 
    testList "ContractTests" [
        testRead
        testWrite
        testRename
        testRemove

    ]