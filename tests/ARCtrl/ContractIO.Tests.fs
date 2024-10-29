module ARCtrl.ContractIO.Tests

open TestingUtils
open ARCtrl
open ARCtrl.Contract
open ARCtrl.FileSystemHelper
open FsSpreadsheet
open FsSpreadsheet.Net
let testRead =

    testList "Read" [
        testCase "TextFile" (fun () -> 
            let fileName = "TestReadMe.txt"
            let contract = Contract.createRead(fileName,DTOType.PlainText)
            let dto = DTO.Text "This is a test"
            let expected = 
                {contract with DTO = Some dto}
            let result = fulfillReadContract TestObjects.IO.testInputFolder contract
            let resultContract = Expect.wantOk result "Contract was not fulfilled correctly"
            Expect.equal resultContract expected $"Text was not read correctly"
        )
        testCase "XLSXFile" (fun () ->
            let fileName = "TestWorkbook.xlsx"
            let contract = Contract.createRead(fileName,DTOType.ISA_Study)
            let result = fulfillReadContract TestObjects.IO.testInputFolder contract
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
        )
    ]


let testWrite =

    testList "Write" [
        testCase "TextFileEmpty" (fun () -> 
            let fileName = "TestEmpty.txt"
            let contract = Contract.createCreate(fileName,DTOType.PlainText)

            Expect.wantOk (fulfillWriteContract TestObjects.IO.testOutputFolder contract) "Contract was not fulfilled correctly"

            let filePath = ArcPathHelper.combine TestObjects.IO.testOutputFolder fileName
            Expect.isTrue (System.IO.File.Exists filePath) $"File {filePath} was not created"
            Expect.equal (FileSystemHelper.readFileText filePath) "" $"File {filePath} was not empty"
        )
        testCase "TextFile" (fun () -> 

            let testText = "This is a test"
            let fileName = "TestReadMe.txt"
            let dto = DTO.Text testText
            let contract = Contract.createCreate(fileName,DTOType.PlainText,dto)

            Expect.wantOk (fulfillWriteContract TestObjects.IO.testOutputFolder contract) "Contract was not fulfilled correctly"

            let filePath = ArcPathHelper.combine TestObjects.IO.testOutputFolder fileName
            Expect.isTrue (System.IO.File.Exists filePath) $"File {filePath} was not created"
            Expect.equal (FileSystemHelper.readFileText filePath) testText $"File {filePath} was not empty"
        )
        testCase "XLSXFile" (fun () -> 

            let worksheetName = "TestSheet"
            let testWB = new FsWorkbook()
            let testSheet = testWB.InitWorksheet (worksheetName)
            testSheet.Row(1).Item(1).Value <- "A1"
            testSheet.Row(1).Item(2).Value <- "B1"
            testSheet.Row(1).Item(3).Value <- "C1"
            let fileName = "TestWorkbook.xlsx"
            let dto = DTO.Spreadsheet testWB
            let contract = Contract.createCreate(fileName,DTOType.ISA_Assay,dto)

            Expect.wantOk (fulfillWriteContract TestObjects.IO.testOutputFolder contract) "Contract was not fulfilled correctly"

            let filePath = ArcPathHelper.combine TestObjects.IO.testOutputFolder fileName
            
            let wb = FsWorkbook.fromXlsxFile filePath
            let ws = Expect.wantSome (wb.TryGetWorksheetByName worksheetName) "Workbook does not contain worksheet"
            let row1 = Expect.wantSome (ws.TryGetRowValuesAt 1) "Worksheet does not contain row 1"
            let expected = ["A1";"B1";"C1"] |> Seq.map box
            Expect.sequenceEqual row1 expected "Worksheet does not contain correct values"      
        )
    ]

let testExecute =

    testList "Write" [
        testCase "Implement" (fun () -> 
            Expect.isTrue false "ImplementTest"           
        )
    ]

let main = 
    testList "ContractTests" [
        testRead
        testWrite
    ]