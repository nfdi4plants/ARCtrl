module ArcStudyTests

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

    testList "StudyMetadataTests" [
        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcStudy.fromMetadataSheet Study.studyMetadataEmpty |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "ReaderSuccessEmptyObsoleteSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcStudy.fromMetadataSheet Study.studyMetadataEmptyObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessEmpty" (fun () ->

            let study,assays = ArcStudy.fromMetadataSheet Study.studyMetadataEmpty

            let writingSuccess = 
                try 
                    ArcStudy.toMetadataSheet study (Some assays) |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSuccessEmptyObsoleteSheetName" (fun () ->

            let study,assays = 
                ArcStudy.fromMetadataSheet Study.studyMetadataEmptyObsoleteSheetName
            let writingSuccess = 
                try 
                    ArcStudy.toMetadataSheet study (Some assays) |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInputEmpty" (fun () ->
           
            let o = 
                Study.studyMetadataEmpty
                |> ArcStudy.fromMetadataSheet
                |> fun (s,a) -> ArcStudy.toMetadataSheet s (Some a)

            Expect.workSheetEqual o Study.studyMetadataEmpty "Written Empty study metadata does not match read study metadata"
        )
        testCase "OutputMatchesInput" (fun () ->
           
            let o = 
                Study.studyMetadata
                |> ArcStudy.fromMetadataSheet
                |> fun (s,a) -> ArcStudy.toMetadataSheet s (Some a)

            Expect.workSheetEqual o Study.studyMetadata "Written Empty study metadata does not match read study metadata"
        )

        testCase "OutputSheetNamesDifferentEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                Study.studyMetadataEmptyObsoleteSheetName
                |> ArcStudy.fromMetadataSheet
                |> fun (s,a) -> ArcStudy.toMetadataSheet s (Some a)

            Expect.isTrue (o.Name <> Study.studyMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )
    ]

let main = 
    testList "StudyFile" [
        testMetaDataFunctions
    ]