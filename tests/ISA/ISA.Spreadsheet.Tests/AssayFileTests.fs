module ArcAssayTests


#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ARCtrl.ISA
open TestingUtils
open ARCtrl.ISA.Spreadsheet

open TestObjects.Spreadsheet

let testMetaDataFunctions = 

    testList "AssayMetadataTests" [

        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.assayMetadata |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "ReaderReadsORCID" (fun () -> 
            
            let assay = ArcAssay.fromMetadataSheet Assay.assayMetadata 
            Expect.equal assay.Performers.Length 3 "Assay should have 3 performers"
            Expect.isSome assay.Performers.[0].ORCID "ORCID should be set"
            Expect.equal assay.Performers.[0].ORCID.Value "0000-0002-1825-0097" "ORCID not read correctly"
            Expect.isNone assay.Performers.[1].ORCID "ORCID should not be set"
            Expect.isSome assay.Performers.[2].ORCID "ORCID should be set"
            Expect.equal assay.Performers.[2].ORCID.Value "0000-0002-1825-0098" "ORCID not read correctly"

        )

        testCase "ReaderSuccessObsoleteSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.assayMetadataObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.assayMetadata

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
                ArcAssay.fromMetadataSheet Assay.assayMetadata
                |> ArcAssay.toMetadataSheet
                
            o.CellCollection.GetCells()
            |> Seq.iter (fun c -> Expect.notEqual (c.Value.Trim()) "" $"Cell {c.Address.ToString()} should not contain empty string")  
        )

        testCase "WriterSuccessObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.assayMetadataObsoleteSheetName

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
                Assay.assayMetadata
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.workSheetEqual o Assay.assayMetadata "Written assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.assayMetadataObsoleteSheetName
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.isTrue (o.Name <> Assay.assayMetadataObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.assayMetadataEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )


        testCase "ReaderCorrectnessEmpty" (fun () -> 
            
            let assay = ArcAssay.fromMetadataSheet Assay.assayMetadataEmpty
            Expect.isNone assay.MeasurementType "MeasurementType should not be set"
            Expect.isNone assay.TechnologyPlatform "TechnologyPlatform should not be set"
            Expect.isNone assay.TechnologyType "TechnologyType should not be set"
        )

        testCase "ReaderCorrectnessEmptyStrings" (fun () -> 
            
            let assay = ArcAssay.fromMetadataSheet Assay.assayMetadataEmptyStrings
            Expect.isNone assay.MeasurementType "MeasurementType should not be set"
            Expect.isNone assay.TechnologyPlatform "TechnologyPlatform should not be set"
            Expect.isNone assay.TechnologyType "TechnologyType should not be set"
            Expect.isEmpty assay.Performers "Performers should be empty"
        )

        testCase "ReaderSuccessEmptyObsoleteSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.assayMetadataEmptyObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessEmpty" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.assayMetadataEmpty

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessEmptyObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.assayMetadataEmptyObsoleteSheetName

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
                Assay.assayMetadataEmpty
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet

            Expect.workSheetEqual o Assay.assayMetadataEmpty "Written Empty assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.assayMetadataEmptyObsoleteSheetName
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet

            Expect.isTrue (o.Name <> Assay.assayMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "ReaderSuccessFromWorkbook" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbook |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessFromWorkbookObsoleteSheetName" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbookObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessFromWorkbookEmpty" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbookEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessFromWorkbookEmptyObsoleteSheetName" (fun () ->
            let readingSuccess = 
                try 
                    ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbookEmptyObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessFromWorkbook" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbook

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )        
        
        testCase "WriterSuccessFromWorkbookObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbookObsoleteSheetName

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessFromWorkbookEmpty" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbookEmpty

            let writingSuccess = 
                try 
                    ArcAssay.toMetadataSheet a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessFromWorkbookEmptyObsoleteSheetName" (fun () ->

            let a = ArcAssay.fromFsWorkbook Assay.assayMetadataWorkbookEmptyObsoleteSheetName

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
                Assay.assayMetadataWorkbook
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.workSheetEqual o Assay.assayMetadata "Written Empty assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentFromWorkbookObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.assayMetadataWorkbookEmptyObsoleteSheetName
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.isTrue (o.Name <> Assay.assayMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "OutputMatchesInputFromWorkbookEmpty" (fun () ->
           
            let o = 
                Assay.assayMetadataWorkbookEmpty
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.workSheetEqual o Assay.assayMetadataEmpty "Written Empty assay metadata does not match read assay metadata"
        )

        testCase "OutputSheetNamesDifferentFromWorkbookEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                Assay.assayMetadataWorkbookEmptyObsoleteSheetName
                |> ArcAssay.fromFsWorkbook
                |> ArcAssay.toMetadataSheet

            Expect.isTrue (o.Name <> Assay.assayMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )

        testCase "ReaderSuccessDeprecatedKeys" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcAssay.fromMetadataSheet Assay.assayMetadataDeprecatedKeys |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccessDeprecatedKeys" (fun () ->

            let a = ArcAssay.fromMetadataSheet Assay.assayMetadataDeprecatedKeys

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
                Assay.assayMetadataDeprecatedKeys
                |> ArcAssay.fromMetadataSheet
                |> ArcAssay.toMetadataSheet
                
            Expect.workSheetEqual o Assay.assayMetadata "Written assay metadata does not match read assay metadata"
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

        //    Expect.mySequenceEqual factors expectedFactors        "Factors were read incorrectly"
        //    Expect.mySequenceEqual protocols expectedProtocols    "Protocols were read incorrectly"
        //    Expect.mySequenceEqual persons expectedPersons        "Persons were read incorrectly from metadata sheet"

        //    Expect.isSome assay.FileName "FileName was not read"
        //    Expect.equal assay.FileName.Value fileName "FileName was not read correctly"

        //    Expect.isSome assay.TechnologyType "Technology Type was not read"
        //    Expect.equal assay.TechnologyType.Value technologyType "Technology Type was not read correctly"

        //    Expect.isSome assay.CharacteristicCategories "Characteristics were not read"
        //    Expect.equal assay.CharacteristicCategories.Value [leafSize] "Characteristics were not read correctly"

        //    Expect.isSome assay.ProcessSequence "Processes were not read"
        //    assay.ProcessSequence.Value
        //    |> Seq.map (fun p -> Option.defaultValue "" p.Name)
        //    |> fun names -> Expect.mySequenceEqual names ["GreatAssay_0";"GreatAssay_1";"SecondAssay_0"] "Process names do not match"

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
        //    Expect.mySequenceEqual a'.ProcessSequence.Value ref.ProcessSequence.Value ""

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
        //    Expect.mySequenceEqual a'.ProcessSequence.Value ref.ProcessSequence.Value ""

        //)
    ]


let main = 
    testList "AssayFile" [
        testMetaDataFunctions
    ]