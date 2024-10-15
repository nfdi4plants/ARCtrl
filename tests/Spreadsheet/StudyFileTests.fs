module ArcStudyTests


open TestingUtils
open ARCtrl
open ARCtrl.Spreadsheet

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
                    ArcStudy.fromMetadataSheet Study.BII_S_1.studyMetadataEmptyObsoleteSheetName |> ignore
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
                ArcStudy.fromMetadataSheet Study.BII_S_1.studyMetadataEmptyObsoleteSheetName
            let writingSuccess = 
                try 
                    ArcStudy.toMetadataSheet study (Some assays) |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the Empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "TestMetadataFromCollection" (fun () ->

            let study, assays = 
                Study.BII_S_1.studyMetadataCollection
                |> ArcStudy.fromMetadataCollection

            Expect.isSome study.Title "Study of the impact of changes in flux on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae under different nutrient limitations"
            Expect.isSome study.Description "We wished to study the impact of growth rate on the total complement of mRNA molecules, proteins, and metabolites in S. cerevisiae, independent of any nutritional or other physiological effects. To achieve this, we carried out our analyses on yeast grown in steady-state chemostat culture under four different nutrient limitations (glucose, ammonium, phosphate, and sulfate) at three different dilution (that is, growth) rates (D = u = 0.07, 0.1, and 0.2/hour, equivalent to population doubling times (Td) of 10 hours, 7 hours, and 3.5 hours, respectively; u = specific growth rate defined as grams of biomass generated per gram of biomass present per unit time)."
            Expect.isSome study.SubmissionDate "2007-04-30"

            Expect.isSome assays.[0].MeasurementType "protein expression profiling"
            Expect.isSome assays.[1].MeasurementType "etabolite profiling"
            Expect.isSome assays.[2].MeasurementType "transcription profiling"
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
                Study.BII_S_1.studyMetadata
                |> ArcStudy.fromMetadataSheet
                |> fun (s,a) -> ArcStudy.toMetadataSheet s (Some a)

            Expect.workSheetEqual o Study.BII_S_1.studyMetadata "Written study metadata does not match read study metadata"
        )

        testCase "OutputSheetNamesDifferentEmptyObsoleteSheetName" (fun () ->
           
            let o = 
                Study.BII_S_1.studyMetadataEmptyObsoleteSheetName
                |> ArcStudy.fromMetadataSheet
                |> fun (s,a) -> ArcStudy.toMetadataSheet s (Some a)

            Expect.isTrue (o.Name <> Study.BII_S_1.studyMetadataEmptyObsoleteSheetName.Name) "sheet names were expected to be different (obsolete replaced by new)"
        )
    ]

open TestObjects.Contract.ISA.SimpleISA

let testFullStudyFile = 
    testList "FullStudyFileTests" [
 
        testCase "WriterSuccess" (fun () ->
            let writingSuccess = 
                try 
                    let study,assays = ArcStudy.fromFsWorkbook Study.bII_S_1WB
                    ArcStudy.toFsWorkbook(study,assays) |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )
        ptestCase "InputMatchesOutput" (fun () ->
            let study, assays = ArcStudy.fromFsWorkbook Study.bII_S_1WB
            let o = ArcStudy.toFsWorkbook(study,assays)

            Expect.workBookEqual o Study.bII_S_1WB "Written study file does not match read study file"
        )
    ]

let main = 
    testList "StudyFile" [
        testMetaDataFunctions
        testFullStudyFile
    ]