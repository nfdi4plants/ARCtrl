module ArcStudyTests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ISA
open TestingUtils
open ISA.Spreadsheet

let testMetaDataFunctions = 

    testList "StudyMetadataTests" [
        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcStudy.fromMetadataSheet TestObjects.Study.studyMetadataEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessEmptyObsoleteSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcStudy.fromMetadataSheet TestObjects.Study.studyMetadataEmptyObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        ptestCase "WriterSuccessEmpty" (fun () ->

            let a = ArcStudy.fromMetadataSheet TestObjects.Study.studyMetadataEmpty

            let writingSuccess = 
                try 
                    ArcStudy.toMetadataSheet a.Value |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        ptestCase "WriterSuccessEmptyObsoleteSheetName" (fun () ->

            let a = ArcStudy.fromMetadataSheet TestObjects.Study.studyMetadataEmptyObsoleteSheetName

            let writingSuccess = 
                try 
                    ArcStudy.toMetadataSheet a.Value |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        ptestCase "OutputMatchesInputEmpty" (fun () ->
           
            let o = 
                TestObjects.Study.studyMetadataEmpty
                |> ArcStudy.fromMetadataSheet
                |> Option.map ArcStudy.toMetadataSheet

            Expect.workSheetEqual o.Value TestObjects.Study.studyMetadataEmpty "Written Empty study metadata does not match read study metadata"
        )

        ptestCase "OutputSheetNamesDifferentEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                TestObjects.Study.studyMetadataEmptyObsoleteSheetName
                |> ArcStudy.fromMetadataSheet
                |> Option.map ArcStudy.toMetadataSheet

            Expect.isTrue (o.Value.Name <> TestObjects.Study.studyMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )
    ]

let main = 
    testList "StudyFile" [
        testMetaDataFunctions
    ]