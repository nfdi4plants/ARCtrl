module ArcInvestigationTests


open ARCtrl.ISA
open FsSpreadsheet
open TestingUtils
open ARCtrl.ISA.Spreadsheet

open TestObjects.Spreadsheet

let private testInvestigationWriterComponents = 
    // Test the single components of invesigation file writing
    testList "InvestigationWriterPartTests" [       
        testCase "CreateEmptyWorkbook" (fun () ->
            let wb = new FsWorkbook()
            Expect.isTrue true "Workbook could not be initialized"
        )
        testCase "CreateSheet" (fun () ->
            let sheet = FsWorksheet("Investigation")
            Expect.equal sheet.Name "Investigation" "Worksheet could not be initialized"
        )
        testCase "InvestigationToRows" (fun () ->
            let i = ArcInvestigation.init("My Investigation")
            let rows = i |> ArcInvestigation.toRows
            Expect.isTrue (rows |> Seq.length |> (<) 0) "Investigation should have at least one row"
        )
        testCase "AddEmptyWorksheet" (fun () ->
            let wb = new FsWorkbook()
            let sheet = FsWorksheet("Investigation")
            wb.AddWorksheet(sheet)                                    
        )
        testCase "FillWorksheet" (fun () ->
            let i = ArcInvestigation.init("My Identifier")
            let sheet = FsWorksheet("Investigation")
            let rows = i |> ArcInvestigation.toRows
            rows
            |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet) 
            Expect.isTrue (sheet.Rows |> Seq.length |> (<) 0) "Worksheet should have at least one row"
        )
        testCase "AddFilledWorksheet" (fun () ->
            let i = ArcInvestigation.init("My Identifier")
            let wb = new FsWorkbook()
            let sheet = FsWorksheet("Investigation")
            let rows = i |> ArcInvestigation.toRows
            rows
            |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet (rowI + 1) r sheet)                     
            wb.AddWorksheet(sheet)
            Expect.isSome (wb.TryGetWorksheetByName "Investigation") "Worksheet should be added to workbook"
        )
        testCase "OnlyConsiderRegisteredStudies" (fun () ->
            let isa = ArcInvestigation("MyInvestigation")
            let registeredStudyIdentifier = "RegisteredStudy"
            let registeredStudy = ArcStudy(registeredStudyIdentifier)
            let unregisteredStudyIdentifier = "UnregisteredStudy"
            let unregisteredStudy = ArcStudy(unregisteredStudyIdentifier)

            isa.AddStudy(unregisteredStudy)
            isa.AddRegisteredStudy(registeredStudy)

            let result = ArcInvestigation.toFsWorkbook isa |> ArcInvestigation.fromFsWorkbook

            Expect.sequenceEqual result.RegisteredStudyIdentifiers [registeredStudyIdentifier] "Only the registered study should be written and read"
        )

                    
                   
    ]

let private testInvestigationFile = 

    testList "InvestigationXLSXTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcInvestigation.fromFsWorkbook Investigation.fullInvestigation |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )
        testCase "ReaderSuccessDeprecatedSheetName" (fun () -> 
            
            let readingSuccess = 
                try  
                    ArcInvestigation.fromFsWorkbook Investigation.fullInvestigationObsoleteSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )
        testCase "ReaderFailureWrongSheetName" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcInvestigation.fromFsWorkbook Investigation.fullInvestigationWrongSheetName |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isError readingSuccess "Reading the investigation file should fail if the sheet name is wrong"
        )
        testCase "WriterSuccess" (fun () ->

            let i = ArcInvestigation.fromFsWorkbook Investigation.fullInvestigation

            let writingSuccess = 
                try 
                    ArcInvestigation.toFsWorkbook i |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInput" (fun () ->
           
            let i = 
                Investigation.fullInvestigation.GetWorksheetByName "isa_investigation"
                
            let o = 
                Investigation.fullInvestigation
                |> ArcInvestigation.fromFsWorkbook
                |> ArcInvestigation.toFsWorkbook
                |> fun wb -> wb.GetWorksheetByName "isa_investigation"               
                
            Expect.workSheetEqual o i "Written investigation file does not match read investigation file"
        )

        testCase "ReaderIgnoresEmptyStudy" (fun () -> 
            let emptyInvestigation = ArcInvestigation.init("My Identifier")
            let wb = ArcInvestigation.toFsWorkbook emptyInvestigation
            let i = ArcInvestigation.fromFsWorkbook wb
            Expect.isEmpty i.Studies "Empty study in investigation should be read to empty ResizeArray"
        )

        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcInvestigation.fromFsWorkbook Investigation.emptyInvestigation |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessEmpty" (fun () ->

            let i = ArcInvestigation.fromFsWorkbook Investigation.emptyInvestigation

            let writingSuccess = 
                try 
                    ArcInvestigation.toFsWorkbook i |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        //testCase "OutputMatchesInputEmpty" (fun () ->

        //    let i = 
        //        Investigation.emptyInvestigation.GetWorksheetByName "isa_investigation"
        //        |> fun ws -> ws.Rows
        //        |> Seq.map (fun r -> r.Cells |> Seq.map (fun c -> c.Value) |> Seq.reduce (fun a b -> a + b)) 
        //    let o = 
        //        Investigation.emptyInvestigation
        //        |> ArcInvestigation.fromFsWorkbook
        //        |> ArcInvestigation.toFsWorkbook
        //        |> fun wb -> wb.GetWorksheetByName "isa_investigation"               
        //        |> fun ws -> ws.Rows
        //        |> Seq.map (fun r -> r.Cells |> Seq.map (fun c -> c.Value) |> Seq.reduce (fun a b -> a + b)) 


        //    Expect.sequenceEqual o i "Written empty investigation file does not match read empty investigation file"
        //)
        ]
        |> testSequenced





    //    testCase "InvestigationInfo" (fun () -> 

    //        let investigation = IO.fromFile referenceInvestigationFilePath

    //        let testInfo = 
    //            InvestigationInfo.create 
    //                "BII-I-1"
    //                "Growth control of the eukaryote cell: a systems biology study in yeast"
    //                "Background Cell growth underlies many key cellular and developmental processes, yet a limited number of studies have been carried out on cell-growth regulation. Comprehensive studies at the transcriptional, proteomic and metabolic levels under defined controlled conditions are currently lacking. Results Metabolic control analysis is being exploited in a systems biology study of the eukaryotic cell. Using chemostat culture, we have measured the impact of changes in flux (growth rate) on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae. Each functional genomic level shows clear growth-rate-associated trends and discriminates between carbon-sufficient and carbon-limited conditions. Genes consistently and significantly upregulated with increasing growth rate are frequently essential and encode evolutionarily conserved proteins of known function that participate in many protein-protein interactions. In contrast, more unknown, and fewer essential, genes are downregulated with increasing growth rate; their protein products rarely interact with one another. A large proportion of yeast genes under positive growth-rate control share orthologs with other eukaryotes, including humans. Significantly, transcription of genes encoding components of the TOR complex (a major controller of eukaryotic cell growth) is not subject to growth-rate regulation. Moreover, integrative studies reveal the extent and importance of post-transcriptional control, patterns of control of metabolic fluxes at the level of enzyme synthesis, and the relevance of specific enzymatic reactions in the control of metabolic fluxes during cell growth. Conclusion This work constitutes a first comprehensive systems biology study on growth-rate control in the eukaryotic cell. The results have direct implications for advanced studies on cell growth, in vivo regulation of metabolic fluxes for comprehensive metabolic engineering, and for the design of genome-scale systems biology models of the eukaryotic cell."
    //                "4/30/2007"
    //                "3/10/2009"
    //                ["Created With Configuration","";"Last Opened With Configuration",""]
            
    //        let info = Person(firstName="Max",midInitials="P",lastName="Mustermann",phone="0123456789",roles="Scientist,Engineer,GeneralExpert")

    //        Expect.equal             
    //            (getIdentificationKeyValues personOfInterest)
    //            (getIdentificationKeyValues testPerson)
    //            "GetIdnetificationKeyValues returned an unexpected array"
    //    )
    //]
    //|> testSequenced

//[<Tests>]
//let testInvestigationFileReading = 
    
//    let testDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
//    let referenceInvestigationFilePath = System.IO.Path.Combine(testDirectory,"isa.investigation.xlsx")

//    testList "testInvestigationFileReading" [
//        testCase "GetInvestigation" (fun () ->
//            let testInvestigation =
//                InvestigationItem(
//                    identifier = "BII-I-1",
//                    title = "Growth control of the eukaryote cell: a systems biology study in yeast",
//                    description = "Background Cell growth underlies many key cellular and developmental processes, yet a limited number of studies have been carried out on cell-growth regulation. Comprehensive studies at the transcriptional, proteomic and metabolic levels under defined controlled conditions are currently lacking. Results Metabolic control analysis is being exploited in a systems biology study of the eukaryotic cell. Using chemostat culture, we have measured the impact of changes in flux (growth rate) on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae. Each functional genomic level shows clear growth-rate-associated trends and discriminates between carbon-sufficient and carbon-limited conditions. Genes consistently and significantly upregulated with increasing growth rate are frequently essential and encode evolutionarily conserved proteins of known function that participate in many protein-protein interactions. In contrast, more unknown, and fewer essential, genes are downregulated with increasing growth rate; their protein products rarely interact with one another. A large proportion of yeast genes under positive growth-rate control share orthologs with other eukaryotes, including humans. Significantly, transcription of genes encoding components of the TOR complex (a major controller of eukaryotic cell growth) is not subject to growth-rate regulation. Moreover, integrative studies reveal the extent and importance of post-transcriptional control, patterns of control of metabolic fluxes at the level of enzyme synthesis, and the relevance of specific enzymatic reactions in the control of metabolic fluxes during cell growth. Conclusion This work constitutes a first comprehensive systems biology study on growth-rate control in the eukaryotic cell. The results have direct implications for advanced studies on cell growth, in vivo regulation of metabolic fluxes for comprehensive metabolic engineering, and for the design of genome-scale systems biology models of the eukaryotic cell.",
//                    submissionDate = "2007-04-30",
//                    publicReleaseDate = "2009-03-10"
//                )

//            let doc = Spreadsheet.fromFile referenceInvestigationFilePath false

//            let retrievedInvestigation = ISA_Investigation.tryGetInvestigation doc

//            Expect.isSome retrievedInvestigation "Could not retrieve investigation"

//            Expect.sequenceEqual
//                (retrievedInvestigation.Value |> getKeyValues)
//                (testInvestigation |> getKeyValues)
//                "Could not retrieve the correct investigation from investigation file"

//            doc
//            |> Spreadsheet.close
//        )
//        testCase "GetItemInStudy" (fun () -> 
//            let testItem = 
//                Factor(
//                    name="rate",
//                    factorType="rate",
//                    typeTermAccessionNumber="http://purl.obolibrary.org/obo/PATO_0000161",
//                    typeTermSourceREF="PATO"
//                )

//            let studyID = "BII-S-1"
//            let itemToGet = Factor(name="rate")
            
//            let doc = Spreadsheet.fromFile referenceInvestigationFilePath false

//            let retrievedItem = ISA_Investigation.tryGetItemInStudy itemToGet studyID doc

//            Expect.isSome retrievedItem "Could not retrieve item from investigation file"

//            Expect.sequenceEqual
//                (retrievedItem.Value |> getKeyValues)
//                (testItem |> getKeyValues)
//                "Could not retrieve the correct item from investigation file"

//            doc
//            |> Spreadsheet.close
//        )
//        testCase "GetItemWithMultipleIdsFromStudy" (fun () -> 
//            let testItem = 
//                Person(
//                    lastName="Juan",
//                    firstName="Castrillo",
//                    midInitials="I",
//                    address="Oxford Road, Manchester M13 9PT, UK",
//                    affiliation="Faculty of Life Sciences, Michael Smith Building, University of Manchester",
//                    roles="author"
//                )

//            let studyID = "BII-S-2"
//            let itemToGet = Person(lastName="Juan",firstName="Castrillo")
            
//            let doc = Spreadsheet.fromFile referenceInvestigationFilePath false

//            let retrievedItem = ISA_Investigation.tryGetItemInStudy itemToGet studyID doc

//            Expect.isSome retrievedItem "Could not retrieve item from investigation file"

//            Expect.sequenceEqual
//                (retrievedItem.Value |> getKeyValues)
//                (testItem |> getKeyValues)
//                "Could not retrieve the correct item from investigation file"

//            doc
//            |> Spreadsheet.close
//        )
//        testCase "ListStudies" (fun () ->
//            let testStudies = [StudyItem(identifier="BII-S-1");StudyItem(identifier="BII-S-2")] |> Seq.map getIdentificationKeyValues
        
//            let doc = Spreadsheet.fromFile referenceInvestigationFilePath false
        
//            let retrievedStudies = ISA_Investigation.getStudies doc |> Seq.map getIdentificationKeyValues

//            Expect.sequenceEqual
//                retrievedStudies
//                testStudies
//                "Could not retrieve the correct studies from the investigation file"
//            doc
//            |> Spreadsheet.close
//        )
//    ]

//[<Tests>]
//let testInvestigationFileManipulations = 

//    let testDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
//    let investigationFilePath = System.IO.Path.Combine(testDirectory,"new.isa.investigation.xlsx")
    
//    testList "InvestigationFileManipulations" [       
//        /// Test Passes, if the investigation file is created and the investigation info is correctly inserted
//        testCase "CreateInvestigationFile" (fun () -> 
//            let investigation = 
//                InvestigationItem(
//                    identifier="MyInvestigation",
//                    title="Testing the investigation manipulation functions reveals proper implementation"
//                )

//            ISA_Investigation.createEmpty investigationFilePath investigation

//            Expect.isTrue
//                (System.IO.File.Exists investigationFilePath)
//                "Investigation File was not created"            

//            let doc = Spreadsheet.fromFile investigationFilePath false 

//            let retrievedInvestigation = ISA_Investigation.tryGetInvestigation doc

//            Expect.isSome retrievedInvestigation "Investigation file was not filled out"

//            Expect.sequenceEqual
//                (retrievedInvestigation.Value |> getKeyValues)
//                (investigation |> getKeyValues)
//                "Investigation file was not filled out correctly"

//            Spreadsheet.close doc
//        )
//        /// Test Passes, if the study is correctly inserted
//        testCase "AddStudy" (fun () -> 
//            let study = 
//                StudyItem(
//                    identifier="MyStudy1",
//                    description="Check if the study is added properly"
//                )

//            let doc = Spreadsheet.fromFile investigationFilePath true 

//            Expect.isSome
//                (ISA_Investigation.tryAddStudy study doc)
//                "Could not add study"

//            let retrievedStudy = ISA_Investigation.tryGetStudy "MyStudy1" doc

//            Expect.isSome retrievedStudy "Study could not be found"

//            Expect.sequenceEqual
//                (retrievedStudy.Value |> getKeyValues)
//                (study |> getKeyValues)
//                "Study was not inserted correctly correctly"

//            Spreadsheet.close doc
//        )
//        /// Test Passes, if the study is not inserted, as a study with the same name already exists
//        testCase "AddStudyWithSameName" (fun () -> 
//            let study = 
//                StudyItem(
//                    identifier="MyStudy1",
//                    description="Check if the study is added properly"
//                )

//            let doc = Spreadsheet.fromFile investigationFilePath true 

//            Expect.isNone
//                (ISA_Investigation.tryAddStudy study doc)
//                "Did not return None even though a study with the same name was already present"

//            Spreadsheet.close doc
//        )
//        /// Test Passes, if the study is correctly inserted
//        testCase "AddSecondStudy" (fun () -> 
//            let study1 = 
//                StudyItem(
//                    identifier="MyStudy1",
//                    description="Check if the study is added properly"
//                )

//            let study2 = 
//                StudyItem(
//                    identifier="MyStudy2",
//                    description="Check if the second study is added properly"
//                )

//            let doc = Spreadsheet.fromFile investigationFilePath true 

//            Expect.isSome
//                (ISA_Investigation.tryAddStudy study2 doc)
//                "Could not add study"

//            Expect.sequenceEqual
//                (ISA_Investigation.getStudies doc |> Seq.map getKeyValues)
//                ([study1;study2] |> Seq.map getKeyValues)
//                "The Sequences do not match the expected seqs"
           
//            Spreadsheet.close doc
//        )
//        /// Test Passes, if the assay is correctly inserted
//        testCase "AddAssay" (fun () ->
//            let assay =
//                Assay(
//                    fileName="MyAssay1",
//                    measurementType="JustLooking",
//                    technologyType="Eyes"
//                )
            
//            let study = "MyStudy1"

//            let doc = Spreadsheet.fromFile investigationFilePath true 
            
//            Expect.isSome
//                (ISA_Investigation.tryAddItemToStudy assay study doc)
//                "Could not add assay"
            
//            let retrievedAssay = ISA_Investigation.tryGetItemInStudy (Assay(fileName="MyAssay1")) "MyStudy1" doc

//            Expect.isSome retrievedAssay "Assay could not be found"

//            Expect.sequenceEqual
//                (retrievedAssay.Value |> getKeyValues)
//                (assay |> getKeyValues)
//                "Study was not inserted correctly correctly"

//            Spreadsheet.close doc                    
//        )
//        /// Test Passes, if the second assay is correctly inserted
//        testCase "AddSecondAssay" (fun () ->
//            let assay =
//                Assay(
//                    fileName="MyAssay2",
//                    measurementType="Running",
//                    technologyType="Legs"
//                )
            
//            let study = "MyStudy1"

//            let doc = Spreadsheet.fromFile investigationFilePath true 
            
//            Expect.isSome
//                (ISA_Investigation.tryAddItemToStudy assay study doc)
//                "Could not add assay"
            
//            let retrievedAssay = ISA_Investigation.tryGetItemInStudy (Assay(fileName="MyAssay2")) "MyStudy1" doc

//            Expect.isSome retrievedAssay "Assay could not be found"

//            Expect.sequenceEqual
//                (retrievedAssay.Value |> getKeyValues)
//                (assay |> getKeyValues)
//                "Assay was not inserted correctly correctly"

//            Spreadsheet.close doc                    
//        )
//        /// Test Passes, if the values of the assay are correctly inserted
//        testCase "UpdateAssay" (fun () ->
//            let updatedAssay =
//                Assay(
//                    fileName="MyAssay2",
//                    measurementType="RunningStraight",
//                    measurementTypeTermAccessionNumber="123",
//                    technologyType="Legs"
//                )
            
//            let study = "MyStudy1"

//            let doc = Spreadsheet.fromFile investigationFilePath true 
            
//            Expect.isSome
//                (ISA_Investigation.tryUpdateItemInStudy updatedAssay study doc)
//                "Could not add assay"
            
//            let retrievedAssay = ISA_Investigation.tryGetItemInStudy (Assay(fileName="MyAssay2")) "MyStudy1" doc

//            Expect.isSome retrievedAssay "Assay could not be found"

//            Expect.sequenceEqual
//                (retrievedAssay.Value |> getKeyValues)
//                (updatedAssay |> getKeyValues)
//                "Assay was not updated correctly"

//            Spreadsheet.close doc                    
//        )
//        /// Test Passes, if the correct assay is being removed
//        testCase "RemoveAssay" (fun () ->

//            let assayToRemove = Assay (fileName = "MyAssay1")

//            let remainingAssay =
//                Assay(
//                    fileName="MyAssay2",
//                    measurementType="RunningStraight",
//                    measurementTypeTermAccessionNumber="123",
//                    technologyType="Legs"
//                )

//            let study = "MyStudy1"
            
//            let doc = Spreadsheet.fromFile investigationFilePath true 
                        
//            Expect.isSome
//                (ISA_Investigation.tryRemoveItemFromStudy assayToRemove study doc)
//                "Could not remove assay"
            
//            Expect.sequenceEqual
//                (ISA_Investigation.getItemsInStudy (Assay()) study doc |> Seq.map getKeyValues)
//                ([remainingAssay] |> Seq.map getKeyValues)
//                "The Sequences do not match the expected seqs"

//            Spreadsheet.close doc            
//        )
//        /// Test Passes, if the second assay is being removed and the scope gets deleted, as it is empty
//        testCase "RemoveRemainingAssay" (fun () ->

//            let assayToRemove = Assay (fileName = "MyAssay2")

//            let study = "MyStudy1"
            
//            let doc = Spreadsheet.fromFile investigationFilePath true 
                        
//            Expect.isSome
//                (ISA_Investigation.tryRemoveItemFromStudy assayToRemove study doc)
//                "Could not remove assay"
                
//            let workbookPart = doc |> Spreadsheet.getWorkbookPart

//            let sheet = WorkbookPart.getDataOfFirstSheet workbookPart

//            let studyScope = ISA_Investigation.tryGetStudyScope workbookPart study sheet |> Option.get

//            let itemScope = ISA_Investigation.tryGetItemScope workbookPart studyScope (Assay()) sheet

//            Expect.isNone
//                itemScope
//                "Empty Scope was not removed after both assays were removed"

//            Spreadsheet.close doc            
//        )
//    ]
//    |> testSequenced

let main = 
    testList "InvestigationFile" [
        testInvestigationWriterComponents
        testInvestigationFile
    ]