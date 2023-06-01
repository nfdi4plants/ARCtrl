module ISAXLSXInvestigationTests

open FsSpreadsheet.ExcelIO
open ISA

open Expecto
open TestingUtils

open ISA.XLSX

[<Tests>]
let testInvestigationFile = 

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceInvestigationFilePath = System.IO.Path.Combine(sourceDirectory,"isa.investigation.xlsx")
    let outputInvestigationFilePath = System.IO.Path.Combine(sinkDirectory,"new_isa.investigation.xlsx")

    let referenceEmptyInvestigationFilePath = System.IO.Path.Combine(sourceDirectory,"empty_isa.investigation.xlsx")
    let outputEmptyInvestigationFilePath = System.IO.Path.Combine(sinkDirectory,"new_empty_isa.investigation.xlsx")

    testList "InvestigationXLSXTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Investigation.fromFile referenceInvestigationFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let i = Investigation.fromFile referenceInvestigationFilePath

            let writingSuccess = 
                try 
                    Investigation.toFile outputInvestigationFilePath i
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInput" (fun () ->

            let i =                
                Spreadsheet.fromFile referenceInvestigationFilePath false
                |> Spreadsheet.getRowsBySheetIndex 0u 
                |> Seq.map (Row.getRowValues None >> Seq.reduce (fun a b -> a + b))
            let o = 
                Spreadsheet.fromFile outputInvestigationFilePath false
                |> Spreadsheet.getRowsBySheetIndex 0u 
                |> Seq.map (Row.getRowValues None >> Seq.reduce (fun a b -> a + b))

            Expect.sequenceEqual o i "Written investigation file does not match read investigation file"
        )
        |> testSequenced

        testCase "ReaderIgnoresEmptyStudy" (fun () -> 
            let bytes = Investigation.toBytes Investigation.empty
            let i = Investigation.fromBytes bytes
            Expect.isFalse i.Studies.IsSome "Empty study in investigation should be read to None, but here is Some"
        )

        testCase "ReaderSuccessEmpty" (fun () -> 
            
            let readingSuccess = 
                try 
                    Investigation.fromFile referenceEmptyInvestigationFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the empty test file failed: %s" err.Message)
            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccessEmpty" (fun () ->

            let i = Investigation.fromFile referenceEmptyInvestigationFilePath

            let writingSuccess = 
                try 
                    Investigation.toFile outputEmptyInvestigationFilePath i
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the empty test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInputEmpty" (fun () ->

            let i =                
                Spreadsheet.fromFile referenceEmptyInvestigationFilePath false
                |> Spreadsheet.getRowsBySheetIndex 0u 
                |> Seq.map (Row.getRowValues None >> Seq.reduce (fun a b -> a + b))
            let o = 
                Spreadsheet.fromFile outputEmptyInvestigationFilePath false
                |> Spreadsheet.getRowsBySheetIndex 0u 
                |> Seq.map (Row.getRowValues None >> Seq.reduce (fun a b -> a + b))

            Expect.sequenceEqual o i "Written empty investigation file does not match read empty investigation file"
        )
        |> testSequenced
    ]


[<Tests>]
let testStringConversions =

    testList "StringConversionTests" [

        testList "SingleItems" [
 
            testCase "FullStrings" (fun () -> 
                let name = "Name"
                let term = "Term"
                let accession = "Accession"
                let source = "Source"

                let testOntology = OntologyAnnotation.make None (Some (Text term)) (Some source) (Some accession) None
                let ontology = OntologyAnnotation.fromString term source accession

                Expect.equal ontology testOntology "Ontology Annotation was not created correctly from strings"
                Expect.equal (OntologyAnnotation.toString ontology) (term,source,accession) "Ontology Annotation was not parsed correctly to strings"

                let testComponent = Component.make (Some name) (Some (Value.Name name)) None (Some testOntology)
                let componentx = Component.fromString name term source  accession

                Expect.equal componentx testComponent "Component was not created correctly from strings"
                Expect.equal (Component.toString componentx) (name,term,source,accession) "Component was not parsed correctly to strings"

                let testPParam = ProtocolParameter.make None (Some testOntology)
                let pParam = ProtocolParameter.fromString term source accession

                Expect.equal pParam testPParam "Protocol Parameter was not created correctly from strings"
                Expect.equal (ProtocolParameter.toString pParam) (term,source,accession) "Protocol Parameter was not parsed correctly to strings"
            )
            testCase "EmptyString" (fun () -> 
                let name = ""
                let term = ""
                let accession = ""
                let source = ""

                let testOntology = OntologyAnnotation.make None None None None None
                let ontology = OntologyAnnotation.fromString term accession source

                Expect.equal ontology testOntology "Empty Ontology Annotation was not created correctly from strings"
                Expect.equal (OntologyAnnotation.toString ontology) (term,accession,source) "Empty Ontology Annotation was not parsed correctly to strings"

                let testComponent = Component.make None None None None
                let componentx = Component.fromString name term accession source 

                Expect.equal componentx testComponent "Empty Component was not created correctly from strings"
                Expect.equal (Component.toString componentx) (name,term,accession,source) "Empty Component was not parsed correctly to strings"

                let testPParam = ProtocolParameter.make None None
                let pParam = ProtocolParameter.fromString term accession source

                Expect.equal pParam testPParam "Empty Protocol Parameter was not created correctly from strings"
                Expect.equal (ProtocolParameter.toString pParam) (term,accession,source) "Empty Protocol Parameter was not parsed correctly to strings"
            )

        ]
        // Some
        testList "StringAggregationTests" [
 
            testCase "FullStrings" (fun () -> 
                let names = "Name1;Name2"
                let terms = "Term1;Term2"
                let accessions = "Accession1;Accession2"
                let sources = "Source1;Source2"

                let testOntologies = 
                    [
                        OntologyAnnotation.fromString "Term1" "Accession1" "Source1"
                        OntologyAnnotation.fromString "Term2" "Accession2" "Source2"
                    ]
                let ontologies = OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources

                Expect.sequenceEqual ontologies testOntologies "Ontology Annotations were not created correctly from aggregated strings"
                Expect.equal (OntologyAnnotation.toAggregatedStrings ';' ontologies) (terms,accessions,sources) "Ontology Annotations were not parsed correctly to aggregated strings"

                let testComponents = 
                    [
                        Component.fromString "Name1" "Term1" "Accession1" "Source1"
                        Component.fromString "Name2" "Term2" "Accession2" "Source2"
                    ]
                let components = Component.fromAggregatedStrings ';' names terms accessions sources

                Expect.sequenceEqual components testComponents "Components were not created correctly from aggregated strings"
                Expect.equal (Component.toAggregatedStrings ';' components) (names,terms,accessions,sources) "Components were not parsed correctly to aggregated strings"

                let testPParams = 
                    [
                        ProtocolParameter.fromString "Term1" "Accession1" "Source1"
                        ProtocolParameter.fromString "Term2" "Accession2" "Source2"
                    ]
                let pParams = ProtocolParameter.fromAggregatedStrings ';' terms accessions sources

                Expect.sequenceEqual pParams testPParams "Protocol Parameters were not created correctly from aggregated strings"
                Expect.equal (ProtocolParameter.toAggregatedStrings ';' pParams) (terms,accessions,sources) "Protocol Parameters were not parsed correctly to aggregated strings"
            )

            testCase "EmptyStrings" (fun () -> 
                let names = ""
                let terms = ""
                let accessions = ""
                let sources = ""

                let testOntologies = [ ]
                let ontologies = OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources

                Expect.sequenceEqual ontologies testOntologies "Ontology Annotations were not created correctly from empty aggregated strings. Empty strings should results in an empty list"
                Expect.equal (OntologyAnnotation.toAggregatedStrings ';' ontologies) (terms,accessions,sources) "Ontology Annotations were not parsed correctly to empty aggregated strings"

                let testComponents = []
                let components = Component.fromAggregatedStrings ';' names terms accessions sources

                Expect.sequenceEqual components testComponents "Components were not created correctly from empty aggregated strings. Empty strings should results in an empty list"
                Expect.equal (Component.toAggregatedStrings ';' components) (names,terms,accessions,sources) "Components were not parsed correctly to empty aggregated strings"

                let testPParams = []
                let pParams = ProtocolParameter.fromAggregatedStrings ';' terms accessions sources

                Expect.sequenceEqual pParams testPParams "Protocol Parameters were not created correctly from aggregated strings. Empty strings should results in an empty list"
                Expect.equal (ProtocolParameter.toAggregatedStrings ';' pParams) (terms,accessions,sources) "Protocol Parameters were not parsed correctly to aggregated strings"
            )
            testCase "PartlyEmptyStrings" (fun () -> 
                let names = ""
                let terms = "Term1;Term2"
                let accessions = "Accession1;Accession2"
                let sources = ""

                let testOntologies = 
                    [
                        OntologyAnnotation.fromString "Term1" "Accession1" ""
                        OntologyAnnotation.fromString "Term2" "Accession2" ""
                    ]
                let ontologies = OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources

                Expect.sequenceEqual ontologies testOntologies "Ontology Annotations were not created correctly from partly empty aggregated strings"
                Expect.equal (OntologyAnnotation.toAggregatedStrings ';' ontologies) (terms,accessions,";") "Ontology Annotations were not parsed correctly to partly empty aggregated strings"

                let testComponents = 
                    [
                        Component.fromString "" "Term1" "Accession1" ""
                        Component.fromString "" "Term2" "Accession2" ""
                    ]
                let components = Component.fromAggregatedStrings ';' names terms accessions sources

                Expect.sequenceEqual components testComponents "Components were not created correctly from partly empty aggregated strings"
                Expect.equal (Component.toAggregatedStrings ';' components) (";",terms,accessions,";") "Components were not parsed correctly to partly empty aggregated strings"

                let testPParams = 
                    [
                        ProtocolParameter.fromString "Term1" "Accession1" ""
                        ProtocolParameter.fromString "Term2" "Accession2" ""
                    ]
                let pParams = ProtocolParameter.fromAggregatedStrings ';' terms accessions sources

                Expect.sequenceEqual pParams testPParams "Protocol Parameters were not created correctly from partly empty aggregated strings"
                Expect.equal (ProtocolParameter.toAggregatedStrings ';' pParams) (terms,accessions,";") "Protocol Parameters were not parsed correctly to partly empty aggregated strings"
            )

            testCase "DifferingLengths" (fun () -> 
                let names = "Name1"
                let terms = "Term1;Term2"
                let accessions = "Accession1;Accession2"
                let sources = "Accession2"

                let ontologies = 
                    try OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources |> Some 
                    with
                    | _ -> None

                Expect.isNone ontologies "Parsing aggregated string to ontologies should have failed because of differing lengths"

                let components = 
                    try Component.fromAggregatedStrings ';' names terms accessions sources |> Some 
                    with
                    | _ -> None

                Expect.isNone components "Parsing aggregated string to compnents should have failed because of differing lengths"
                
                let pParams = 
                    try ProtocolParameter.fromAggregatedStrings ';' terms accessions sources |> Some 
                    with
                    | _ -> None

                Expect.isNone pParams "Parsing aggregated string to protocol parameters should have failed because of differing lengths"
                
            )
        ]
        testList "Value" [
            testCase "ParseOntology"(fun () ->

                let value = Value.fromOptions (Some "Name") (Some "Source") (Some "Accession")

                Expect.isSome value "Should have returned Value but returned None"

                let expectedAnnotationValue = AnnotationValue.Text "Name"
                let expectedAnnotation = OntologyAnnotation.make None (Some expectedAnnotationValue) (Some "Source") (Some "Accession") None
                let expectedValue = Value.Ontology expectedAnnotation

                Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            )
            testCase "ParseText"(fun () ->

                let value = Value.fromOptions (Some "Name") None None

                Expect.isSome value "Should have returned Value but returned None"

                let expectedValue = Value.Name "Name"

                Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            )
            testCase "ParseInt"(fun () ->

                let value = Value.fromOptions (Some "5") None None

                Expect.isSome value "Should have returned Value but returned None"

                let expectedValue = Value.Int 5

                Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            )
            testCase "ParseFloat"(fun () ->

                let value = Value.fromOptions (Some "2.3") None None

                Expect.isSome value "Should have returned Value but returned None"

                let expectedValue = Value.Float 2.3

                Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            )

        ]
    ]
    |> testSequenced

[<Tests>]
let testSparseTable =

    testList "SparseTableTests" [
 
        testCase "Create" (fun () -> 
            
            let keys = ["A";"B"]
            let length = 2

            let sparseTable = SparseTable.Create(keys = keys,length = length)
            
            Expect.equal sparseTable.Matrix.Count 0 "Dictionary was not empty"
            Expect.equal sparseTable.Keys keys "Keys were not taken properly"
            Expect.equal sparseTable.CommentKeys [] "Comment keys should be empty"
            Expect.equal sparseTable.Length length "Length did not match"

        )

        testCase "AddRow" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]

            let sparseTableFirstRow = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow

            Expect.equal sparseTableFirstRow.Matrix.Count  2           "FirstRowAdded: Dictionary was not empty"
            Expect.equal sparseTableFirstRow.Keys          [firstKey]  "FirstRowAdded: Keys were not updated properly"
            Expect.equal sparseTableFirstRow.CommentKeys   []          "FirstRowAdded: Comment keys should be empty"
            Expect.equal sparseTableFirstRow.Length        3           "FirstRowAdded: Length did not update according to item count"
            
            let sparseTableSecondRow = 
                sparseTableFirstRow
                |> SparseTable.AddRow secondKey secondRow

            Expect.equal sparseTableSecondRow.Matrix.Count  3                       "SecondRowAdded: Dictionary was not empty"
            Expect.equal sparseTableSecondRow.Keys          [firstKey;secondKey]    "SecondRowAdded: Keys were not updated properly"
            Expect.equal sparseTableSecondRow.CommentKeys   []                      "SecondRowAdded: Comment keys should be empty"
            Expect.equal sparseTableSecondRow.Length        5                       "SecondRowAdded: Length did not update according to item count"            

        )

        testCase "AddComment" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]
            let firstComment,firstCommentRow = "CommentSameLength",[1,"Lel";2,"Lal";3,"Lul"]
            let secondComment,secondCommentRow = "CommentLonger",[2,"Lal";5,"Sho"]

            let sparseTableFirstComment = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow
                |> SparseTable.AddRow secondKey secondRow
                |> SparseTable.AddComment firstComment firstCommentRow

            Expect.equal sparseTableFirstComment.Matrix.Count  6                       "FirstCommentAdded: Dictionary was not empty"
            Expect.equal sparseTableFirstComment.Keys          [firstKey;secondKey]    "FirstCommentAdded: Keys were not updated properly"
            Expect.equal sparseTableFirstComment.CommentKeys   [firstComment]          "FirstCommentAdded: Comment keys should be empty"
            Expect.equal sparseTableFirstComment.Length        5                       "FirstCommentAdded: Length did not update according to item count"

            let sparseTableSecondComment = 
                sparseTableFirstComment
                |> SparseTable.AddComment secondComment secondCommentRow

            Expect.equal sparseTableSecondComment.Matrix.Count  8                              "SecondCommentAdded: Dictionary was not empty"
            Expect.equal sparseTableSecondComment.Keys          [firstKey;secondKey]           "SecondCommentAdded: Keys were not update properly"
            Expect.equal sparseTableSecondComment.CommentKeys   [firstComment;secondComment]   "SecondCommentAdded: Comment keys should be empty"
            Expect.equal sparseTableSecondComment.Length        6                              "SecondCommentAdded: Length did not update according to item count"                    
        )
        |> testSequenced

        testCase "ToRow" (fun () ->

            let firstKey,firstRow = "Greetings",[1,"Hello";2,"Bye"]
            let secondKey,secondRow = "AndAgain",[4,"Hello Again"]
            let firstComment,firstCommentRow = "CommentSameLength",[1,"Lel";2,"Lal";3,"Lul"]
            let secondComment,secondCommentRow = "CommentLonger",[2,"Lal";5,"Sho"]

            let sparseTable = 
                SparseTable.Create()
                |> SparseTable.AddRow firstKey firstRow
                |> SparseTable.AddRow secondKey secondRow
                |> SparseTable.AddComment firstComment firstCommentRow
                |> SparseTable.AddComment secondComment secondCommentRow


            let testRows = 
                [
                    firstKey ::     List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) firstRow with | Some (_,v) -> v | None -> "")
                    secondKey ::    List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) secondRow with | Some (_,v) -> v | None -> "")
                    Comment.wrapCommentKey firstComment ::  List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) firstCommentRow with | Some (_,v) -> v | None -> "")
                    Comment.wrapCommentKey secondComment :: List.init 5 (fun i -> match Seq.tryFind (fst >> (=) (i+1)) secondCommentRow with | Some (_,v) -> v | None -> "")               
                ]

            sparseTable
            |> SparseTable.ToRows
            |> Seq.iteri (fun i r ->               
                let testSeq = Seq.item i testRows
                Expect.sequenceEqual (SparseRow.getValues r) testSeq ""
            
            )

        )
        |> testSequenced
    ]



//        testCase "InvestigationInfo" (fun () -> 

//            let investigation = IO.fromFile referenceInvestigationFilePath

//            let testInfo = 
//                InvestigationInfo.create 
//                    "BII-I-1"
//                    "Growth control of the eukaryote cell: a systems biology study in yeast"
//                    "Background Cell growth underlies many key cellular and developmental processes, yet a limited number of studies have been carried out on cell-growth regulation. Comprehensive studies at the transcriptional, proteomic and metabolic levels under defined controlled conditions are currently lacking. Results Metabolic control analysis is being exploited in a systems biology study of the eukaryotic cell. Using chemostat culture, we have measured the impact of changes in flux (growth rate) on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae. Each functional genomic level shows clear growth-rate-associated trends and discriminates between carbon-sufficient and carbon-limited conditions. Genes consistently and significantly upregulated with increasing growth rate are frequently essential and encode evolutionarily conserved proteins of known function that participate in many protein-protein interactions. In contrast, more unknown, and fewer essential, genes are downregulated with increasing growth rate; their protein products rarely interact with one another. A large proportion of yeast genes under positive growth-rate control share orthologs with other eukaryotes, including humans. Significantly, transcription of genes encoding components of the TOR complex (a major controller of eukaryotic cell growth) is not subject to growth-rate regulation. Moreover, integrative studies reveal the extent and importance of post-transcriptional control, patterns of control of metabolic fluxes at the level of enzyme synthesis, and the relevance of specific enzymatic reactions in the control of metabolic fluxes during cell growth. Conclusion This work constitutes a first comprehensive systems biology study on growth-rate control in the eukaryotic cell. The results have direct implications for advanced studies on cell growth, in vivo regulation of metabolic fluxes for comprehensive metabolic engineering, and for the design of genome-scale systems biology models of the eukaryotic cell."
//                    "4/30/2007"
//                    "3/10/2009"
//                    ["Created With Configuration","";"Last Opened With Configuration",""]
            
//            let info = Person(firstName="Max",midInitials="P",lastName="Mustermann",phone="0123456789",roles="Scientist,Engineer,GeneralExpert")

//            Expect.equal             
//                (getIdentificationKeyValues personOfInterest)
//                (getIdentificationKeyValues testPerson)
//                "GetIdnetificationKeyValues returned an unexpected array"
//        )
//    ]
//    |> testSequenced

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