module Tests.Person

open ARCtrl
open ARCtrl.Process
open ARCtrl.Json
open TestingUtils
open TestObjects.Json

module Helper =

    let create() = Person.create(
        "0000-0002-8510-6810", 
        "Frey", 
        "Kevin", 
        email="myfantasymail@wow.de", 
        phone="09812-39128",
        address="Musterstraße 42, 12345 Beispielstadt, Deutschland",
        affiliation="RPTU",
        roles=ResizeArray [OntologyAnnotation("researcher");OntologyAnnotation("developer","dev","dev:00000001")],
        comments = ResizeArray [Comment("Wow","Very Wow")]
    )

open Helper

let private tests_isa = testList "ISA" [
    createBaseJsonTests 
        "base"
        create
        Person.toISAJsonString
        Person.fromISAJsonString
        None
        None
    testCase "ReaderSuccess" (fun () -> 
        let readingSuccess = 
            try 
                Person.fromISAJsonString Person.person |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)
        Expect.isOk readingSuccess (Result.getMessage readingSuccess)
    )

    testCase "WriterSuccess" (fun () ->
        let a = Person.fromISAJsonString Person.person
        let writingSuccess = 
            try 
                Person.toISAJsonString () a |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)
        Expect.isOk writingSuccess (Result.getMessage writingSuccess)
    )
    #if !FABLE_COMPILER_PYTHON
    testAsync "WriterSchemaCorrectness" {
        let a = Person.fromISAJsonString Person.person
        let s = Person.toISAJsonString () a
        let! validation = Validation.validatePerson s
        Expect.isTrue validation.Success $"Person did not match schema: {validation.GetErrors()}"
    }
    #endif
    testCase "OutputMatchesInput" (fun () ->
        let o = Person.fromJsonString Person.person |> Person.toJsonString ()
        let expected = Person.person
        let actual = o
        Expect.stringEqual actual expected "Written person file does not match read person file"
    )
        
    testCase "WithORCID ReaderCorrectness" (fun () -> 
        let p = Person.fromISAJsonString Person.personWithORCID
        Expect.hasLength p.Comments 0 "Comments should be None"
        Expect.isSome p.ORCID "ORCID should be Some"
        Expect.equal p.ORCID.Value "0000-0002-1825-0097" "ORCID not as expected"

    )
    #if !FABLE_COMPILER_PYTHON
    testAsync "WithORCID WriterSchemaCorrectness" {
        let a = Person.fromISAJsonString Person.personWithORCID
        let s = Person.toISAJsonString () a
        let! validation = Validation.validatePerson s
        Expect.isTrue validation.Success $"Person did not match schema: {validation.GetErrors()}"
    }
    #endif
    testCase "WithORCID OutputMatchesInput" (fun () ->
        let o = 
            Person.fromISAJsonString Person.personWithORCID
            |> Person.toISAJsonString ()
        let expected = Person.personWithORCID
        let actual = o
        Expect.stringEqual actual expected "Written person file does not match read person file"
    )
]

let private tests_rocrate = testList "ROCrate" [
    // pending until: https://github.com/nfdi4plants/ARCtrl/issues/334
    #if FABLE_COMPILER_PYTHON
    ptestCase "Write" <| fun _ ->
    #else
    testCase "Write" <| fun _ ->
    #endif
        let person = create()
        let actual = Person.toROCrateJsonString () person
        let expected = TestObjects.Json.ROCrate.person
        Expect.stringEqual actual expected ""
    createBaseJsonTests 
        ""
        create
        Person.toROCrateJsonString
        Person.fromROCrateJsonString
        None
        None
]

let tests = testList "core" [
     createBaseJsonTests 
        "base"
        create
        Person.toJsonString
        Person.fromJsonString
        None
        None
]

let main = testList "Person" [
    tests_isa
    tests_rocrate
    tests
]