module Tests.Publication

open ARCtrl
open ARCtrl.Process
open ARCtrl.Json
open TestingUtils
open TestObjects.Json

module Helper =

    let create() =
        Publication.create(
            "37108605",
            "10.3390/ijms24087444",
            "Felix Jung, Kevin Frey, David Zimmer, Timo Mühlhaus",
            "DeepSTABp: A Deep Learning Approach for the Prediction of Thermal Protein Stability",
            OntologyAnnotation("published","EFO","EFO:0001796"),
            ResizeArray [Comment("ByeBye", "World"); Comment("Hello", "Space")]
        )

    let compareObj (actual:Publication) (expected:Publication) =
        Expect.equal actual.PubMedID expected.PubMedID "PubMedID"
        Expect.equal actual.DOI expected.DOI "DOI"
        Expect.equal actual.Authors expected.Authors "Authors"
        Expect.equal actual.Title expected.Title "Title"
        Expect.equal actual.Status expected.Status "Status"
        Expect.sequenceEqual actual.Comments expected.Comments "Comments"

open Helper

let tests_isa = testList "isa" [
    createBaseJsonTests
        ""
        create
        Publication.toISAJsonString
        Publication.fromISAJsonString
        
    testCase "ReaderSuccess" <| fun () -> 
        let readingSuccess = 
            try 
                Publication.fromISAJsonString Publication.publication |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)
        Expect.isOk readingSuccess (Result.getMessage readingSuccess)
    testCase "WriterSuccess" <| fun () ->
        let a = Publication.fromISAJsonString Publication.publication
        let writingSuccess = 
            try 
                Publication.toISAJsonString () a |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)
        Expect.isOk writingSuccess (Result.getMessage writingSuccess)

    #if !FABLE_COMPILER_PYTHON
    testAsync "WriterSchemaCorrectness" {
        let a = Publication.fromISAJsonString Publication.publication
        let s = Publication.toISAJsonString () a
        let! validation = Validation.validatePublication s
        Expect.isTrue validation.Success $"Publication did not match schema: {validation.GetErrors()}"
    }
    #endif

    testCase "OutputMatchesInput" <| fun () ->
        let o = 
            Publication.fromISAJsonString Publication.publication
            |> Publication.toISAJsonString ()
        let expected = Publication.publication
        let actual = o
        Expect.stringEqual actual expected "Written Publication file does not match read publication file"
]

let tests_rocrate = testList "rocrate" [
    testCase "compare single fields" <| fun _ ->
        let o = create()
        let json = Publication.toROCrateJsonString () o
        let oNew = Publication.fromROCrateJsonString json
        compareObj o oNew
    createBaseJsonTests
        ""
        create
        Publication.toROCrateJsonString
        Publication.fromROCrateJsonString
]

let tests = 
    createBaseJsonTests
        ""
        create
        Publication.toJsonString
        Publication.fromJsonString

let main = testList "Publication" [
    tests
    tests_isa
    tests_rocrate
]
