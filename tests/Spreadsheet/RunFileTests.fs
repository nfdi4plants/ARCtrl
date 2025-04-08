module ArcRunTests


open ARCtrl
open TestingUtils
open ARCtrl.Spreadsheet

open TestObjects.Spreadsheet

let testMetaDataFunctions = 

    testList "RunMetadataTests" [

        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcRun.fromMetadataSheet Run.Proteomics.runMetadata |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "ReaderReadsORCID" (fun () -> 
            
            let run = ArcRun.fromMetadataSheet Run.Proteomics.runMetadata
            Expect.equal run.Performers.Count 2 "Run should have 2 contacts"
            Expect.isSome run.Performers.[0].ORCID "ORCID should be set"
            Expect.equal run.Performers.[0].ORCID.Value "1234-5678-9012-3456" "ORCID not read correctly"
            Expect.isSome run.Performers.[1].ORCID "ORCID should be set"
            Expect.equal run.Performers.[1].ORCID.Value "9876-5432-1098-7654" "ORCID not read correctly"
        )

        testCase "WriterSuccess" (fun () ->

            let r = ArcRun.fromMetadataSheet Run.Proteomics.runMetadata

            let writingSuccess = 
                try 
                    ArcRun.toMetadataSheet r |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterCreatesNoEmptyCells" (fun () ->

            let o = 
                ArcRun.fromMetadataSheet Run.Proteomics.runMetadata
                |> ArcRun.toMetadataSheet
                
            o.CellCollection.GetCells()
            |> Seq.iter (fun c -> Expect.notEqual (c.ValueAsString().Trim()) "" $"Cell {c.Address.ToString()} should not contain empty string")  
        )

        testCase "OutputMatchesInput" (fun () ->
           
            let o =             
                Run.Proteomics.runMetadata
                |> ArcRun.fromMetadataSheet 
                |> ArcRun.toMetadataSheet
                
            Expect.workSheetEqual o Run.Proteomics.runMetadata "Written run metadata does not match read assay metadata"
        )                
    ]

let testWorkbookParsing =
    testList "Workbook" [
        testCase "Tables WriteRead" (fun () ->
            let run = ArcRun.init("MyRun")
            let table = run.InitTable("Table1")
            table.AddColumn(CompositeHeader.Input(IOType.Data))
            let wb = ArcRun.toFsWorkbook run
            Expect.hasLength (wb.GetWorksheets()) 2 "Workbook should have 2 worksheets"
            let run2 = ArcRun.fromFsWorkbook wb
            Expect.equal run2.Identifier run.Identifier "Run identifiers should be equal"
            Expect.hasLength run2.Tables 1 "Run should have 1 table"
        )

    ]


let main = 
    testList "RunFile" [
        testMetaDataFunctions
        testWorkbookParsing
    ]