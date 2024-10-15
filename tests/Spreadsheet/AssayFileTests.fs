module ArcAssayTests


open ARCtrl
open TestingUtils
open ARCtrl.Spreadsheet

open TestObjects.Spreadsheet

let testMetaDataFunctions = 

    testList "AssayMetadataTests" [

        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadata |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "ReaderReadsORCID" (fun () -> 
            
            let assay = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadata 
            Expect.equal assay.Performers.Count 3 "Assay should have 3 performers"
            Expect.isSome assay.Performers.[0].ORCID "ORCID should be set"
            Expect.equal assay.Performers.[0].ORCID.Value "0000-0002-1825-0097" "ORCID not read correctly"
            Expect.isNone assay.Performers.[1].ORCID "ORCID should not be set"
            Expect.isSome assay.Performers.[2].ORCID "ORCID should be set"
            Expect.equal assay.Performers.[2].ORCID.Value "0000-0002-1825-0098" "ORCID not read correctly"

        )

        testCase "ReaderSuccessObsoleteSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadata

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterCreatesNoEmptyCells" (fun () ->

            let o = 
                ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadata
                |> ArcAssay.toMetadataSheet
                
            o.CellCollection.GetCells()
            |> Seq.iter (fun c -> Expect.notEqual (c.ValueAsString().Trim()) "" $"Cell {c.Address.ToString()} should not contain empty string")  
        )

        testCase "TestMetadataFromCollection" (fun () ->

            let assay =
                Assay.Proteome.assayMetadataCollection
                |> ArcAssay.fromMetadataCollection

            Expect.isSome assay.MeasurementType "protein expression profiling"
            Expect.isSome assay.TechnologyPlatform "iTRAQ"
            Expect.isSome assay.TechnologyType "mass spectrometry"
            Expect.isSome (assay.Performers.Item 0).LastName "Oliver"
            Expect.isSome (assay.Performers.Item 1).LastName "Juan"
            Expect.isSome (assay.Performers.Item 2).LastName "Leo"
        )

        testCase "WriterSuccessObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataObsoleteSheetName

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInput" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadata
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.workSheetEqual o Assay.Proteome.assayMetadata "Written assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataObsoleteSheetName
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.isTrue (o.Name <> Assay.Proteome.assayMetadataObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )


        testCase "ReaderCorrectnessEmpty" (fun () -> 
            
            let assay = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataEmpty
            Expect.isNone assay.MeasurementType "MeasurementType should not be set"
            Expect.isNone assay.TechnologyPlatform "TechnologyPlatform should not be set"
            Expect.isNone assay.TechnologyType "TechnologyType should not be set"
        )

        testCase "ReaderCorrectnessEmptyStrings" (fun () -> 
            
            let assay = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataEmptyStrings
            Expect.isNone assay.MeasurementType "MeasurementType should not be set"
            Expect.isNone assay.TechnologyPlatform "TechnologyPlatform should not be set"
            Expect.isNone assay.TechnologyType "TechnologyType should not be set"
            Expect.isEmpty assay.Performers "Performers should be empty"
        )

        testCase "ReaderSuccessEmptyObsoleteSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataEmptyObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessEmpty" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataEmpty

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessEmptyObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataEmptyObsoleteSheetName

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInputEmpty" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataEmpty
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet

            Expect.workSheetEqual o Assay.Proteome.assayMetadataEmpty "Written Empty assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataEmptyObsoleteSheetName
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet

            Expect.isTrue (o.Name <> Assay.Proteome.assayMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "ReaderSuccessFromWorkbook" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbook |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessFromWorkbookObsoleteSheetName" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbookObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessFromWorkbookEmpty" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbookEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessFromWorkbookEmptyObsoleteSheetName" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbookEmptyObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessFromWorkbook" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbook

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )        
        
        testCase "WriterSuccessFromWorkbookObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbookObsoleteSheetName

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessFromWorkbookEmpty" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbookEmpty

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessFromWorkbookEmptyObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.Proteome.assayMetadataWorkbookEmptyObsoleteSheetName

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInputFromWorkbook" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataWorkbook
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.workSheetEqual o Assay.Proteome.assayMetadata "Written Empty assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentFromWorkbookObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataWorkbookEmptyObsoleteSheetName
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.isTrue (o.Name <> Assay.Proteome.assayMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "OutputMatchesInputFromWorkbookEmpty" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataWorkbookEmpty
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.workSheetEqual o Assay.Proteome.assayMetadataEmpty "Written Empty assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentFromWorkbookEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataWorkbookEmptyObsoleteSheetName
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.isTrue (o.Name <> Assay.Proteome.assayMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "ReaderSuccessDeprecatedKeys" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataDeprecatedKeys |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccessDeprecatedKeys" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.Proteome.assayMetadataDeprecatedKeys

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInputDeprecatedKeys" (fun () ->
           
            let o = 
                Assay.Proteome.assayMetadataDeprecatedKeys
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.workSheetEqual o Assay.Proteome.assayMetadata "Written assay metadata does not match read assay metadata"
        )


        ]

let testAssayFileReader = 


    testList "AssayFileReaderTests" [
        //testCase "ReaderSuccess" (fun () -> 
                       
        //    let readingSuccess = 
        //        try 
        //            Assay.fromFile assayFilePath |> ignore
        //            Result.Ok "DidRun"
        //        with
        //        | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

        //    Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        //)
        //testCase "ReadsCorrectly" (fun () ->        
            
        //    let persons,assay = Assay.fromFile assayFilePath

        //    let expectedProtocols = 
        //        [
        //        Protocol.make None (Some "GreatAssay") None None None None (Some [temperatureUnit;peptidase;temperature;time1]) None None |> Protocol.setRowRange (0,2)
        //        Protocol.make None (Some "SecondAssay") None None None None (Some [temperatureUnit2]) None None |> Protocol.setRowRange (0,2)
        //        ]
                

        //    let expectedFactors = [time2]

        //    let factors = API.Assay.getFactors assay
        //    let protocols = API.Assay.getProtocols assay

        //    Expect.sequenceEqual factors expectedFactors        "Factors were read incorrectly"
        //    Expect.sequenceEqual protocols expectedProtocols    "Protocols were read incorrectly"
        //    Expect.sequenceEqual persons expectedPersons        "Persons were read incorrectly from metadata sheet"

        //    Expect.isSome assay.FileName "FileName was not read"
        //    Expect.equal assay.FileName.Value fileName "FileName was not read correctly"

        //    Expect.isSome assay.TechnologyType "Technology Type was not read"
        //    Expect.equal assay.TechnologyType.Value technologyType "Technology Type was not read correctly"

        //    Expect.isSome assay.CharacteristicCategories "Characteristics were not read"
        //    Expect.equal assay.CharacteristicCategories.Value [leafSize] "Characteristics were not read correctly"

        //    Expect.isSome assay.ProcessSequence "Processes were not read"
        //    assay.ProcessSequence.Value
        //    |> Seq.map (fun p -> Option.defaultValue "" p.Name)
        //    |> fun names -> Expect.sequenceEqual names ["GreatAssay_0";"GreatAssay_1";"SecondAssay_0"] "Process names do not match"

        //)
        //testCase "AroundTheWorldComponents" (fun () ->        

        //    let xlsxFilePath = System.IO.Path.Combine(sourceDirectory,"20220802_TermCols_Assay.xlsx")
        //    let jsonFilePath = System.IO.Path.Combine(sourceDirectory,"20220802_TermCols_Assay.json")
        //    let xlsxOutFilePath = System.IO.Path.Combine(sinkDirectory,"20220802_TermCols_Assay.xlsx")

        //    let ref = Json.Assay.fromFile jsonFilePath
        //    let p,a = Assay.fromFile xlsxFilePath

        //    Expect.equal a.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length "Assay read from xlsx and json do not match. Process sequence does not have the same length"

        //    (a.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay read from xlsx and json do not match. Protocol does not match"
        //        Expect.equal p refP "Assay read from xlsx and json do not match. Process does not match"
        //    ) 

        //    Assay.toFile xlsxOutFilePath p a

        //    let _,a' = Assay.fromFile xlsxOutFilePath


        //    Expect.equal a'.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length "Assay written to xlsx and read in again does no longer match json. Process sequence does not have the same length"

        //    (a'.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay written to xlsx and read in again does no longer match json. Protocol does not match"
        //        Expect.equal p refP "Assay written to xlsx and read in again does no longer match json. Process does not match"
        //    ) 
        //    Expect.sequenceEqual a'.ProcessSequence.Value ref.ProcessSequence.Value ""

        //)
        //testCase "AroundTheWorldProtocolType" (fun () ->        

        //    let xlsxFilePath = System.IO.Path.Combine(sourceDirectory,"20220803_ProtocolType_Assay.xlsx")
        //    let jsonFilePath = System.IO.Path.Combine(sourceDirectory,"20220803_ProtocolType_Assay.json")
        //    let xlsxOutFilePath = System.IO.Path.Combine(sinkDirectory,"20220803_ProtocolType_Assay.xlsx")

        //    let ref = Json.Assay.fromFile jsonFilePath
        //    let p,a = Assay.fromFile xlsxFilePath

        //    Expect.equal a.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length $"Assay read from xlsx and json do not match. Process sequence does not have the same length"


        //    (a.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay read from xlsx and json do not match. Protocol does not match"
        //        Expect.equal p refP "Assay read from xlsx and json do not match. Process does not match"
        //    ) 

        //    Assay.toFile xlsxOutFilePath p a

        //    let _,a' = Assay.fromFile xlsxOutFilePath


        //    Expect.equal a'.ProcessSequence.Value.Length ref.ProcessSequence.Value.Length "Assay written to xlsx and read in again does no longer match json. Process sequence does not have the same length"

        //    (a'.ProcessSequence.Value,ref.ProcessSequence.Value)
        //    ||> List.iter2 (fun p refP -> 
        //        Expect.equal p.ExecutesProtocol.Value refP.ExecutesProtocol.Value "Assay written to xlsx and read in again does no longer match json. Protocol does not match"
        //        Expect.equal p refP "Assay written to xlsx and read in again does no longer match json. Process does not match"
        //    ) 
        //    Expect.sequenceEqual a'.ProcessSequence.Value ref.ProcessSequence.Value ""

        //)
    ]

let tests_ArcTable = testList "ArcTable" [
    testList "Read/Write" [
        let assayIdentifier = "MyAssay"
        let tableName = "AssayProtocol"
        let protocolName = "MyProtocol"
        let inputHeader = CompositeHeader.Input IOType.Sample
        let inputCell = (CompositeCell.createFreeText "inputValueName")
        let createTestAssay() = 
            let assay = ArcAssay(assayIdentifier)
            let t = ArcTable.init(tableName)
            t.AddProtocolNameColumn(Array.create 2 protocolName)
            t.AddColumn(inputHeader, Array.create 2 inputCell)
            assay.AddTable t
            assay
        let ensureTestAssay (assay: ArcAssay) =
            Expect.equal assay.Identifier assayIdentifier "identifier"
            Expect.equal assay.TableCount 1 "tablecount"
            Expect.equal assay.Tables.[0].Name tableName "table.name"
            Expect.equal assay.Tables.[0].ColumnCount 2 "table.columncount"
        testCase "ensure test assay" <| fun _ ->
            let assay = createTestAssay()
            ensureTestAssay assay
        testCase "ArcAssay.toFsWorkbook" <| fun _ ->
            let assay = createTestAssay()
            let wb = ArcAssay.toFsWorkbook assay
            let wss = wb.GetWorksheets()
            Expect.hasLength wss 2 "worksheets count"
            let metadata = wss.[0]
            let tableWorksheet = wss.[1]
            Expect.hasLength metadata.Tables 0 "no tables in metadata sheet"
            Expect.hasLength tableWorksheet.Tables 1 "1 table in protocol sheet"
            Expect.equal tableWorksheet.Name tableName "worksheet.name"
            let table = tableWorksheet.Tables.[0]
            Expect.equal (table.ColumnCount()) 2 "table.columncount"
        testCase "roundabout" <| fun _ ->
            let assay = createTestAssay()
            let wb = ArcAssay.toFsWorkbook assay
            let actual = ArcAssay.fromFsWorkbook wb
            ensureTestAssay actual
    ]
]

let main = 
    testList "AssayFile" [
        testMetaDataFunctions
        tests_ArcTable
    ]