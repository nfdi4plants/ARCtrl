module Tests.Study

open ARCtrl
open ARCtrl.Json
open TestingUtils


module Helper =
    
    let create_empty() = ArcStudy.init("My Fancy Stundy")
    let create_filled() = ArcStudy.create(
        "My Study",
        "My Title",
        "My Description",
        #if !FABLE_COMPILER
        "2024-03-15T20:20:39",
        #else
        "2024-03-15",
        #endif        
        "2024-04-20",
        ResizeArray [|Publication.create(doi="any-nice-doi-42")|],
        ResizeArray [|Person.create(firstName="Kevin", lastName="Frey", phone="023382093810")|], 
        ResizeArray [|OntologyAnnotation(); OntologyAnnotation()|],
        ResizeArray [ArcTable.Helper.create_filled(); ArcTable.init("Table 2")],
        registeredAssayIdentifiers = ResizeArray ["Assay 1"; "Assay 2"],
        comments = ResizeArray [|Comment.create("Hello", "World")|]
    )

    let compareFields =
        fun (actual: ArcStudy) (expected: ArcStudy) ->
            Expect.equal actual.Identifier expected.Identifier "Identifier"
            Expect.equal actual.Title expected.Title "Title"
            Expect.equal actual.Description expected.Description "Description"
            Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
            Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
            Expect.sequenceEqual actual.Publications expected.Publications "Publications"
            Expect.sequenceEqual actual.Contacts expected.Contacts "Contacts"
            Expect.sequenceEqual actual.StudyDesignDescriptors expected.StudyDesignDescriptors "StudyDesignDescriptors"
            Expect.sequenceEqual actual.Tables expected.Tables "Tables"
            Expect.sequenceEqual actual.RegisteredAssayIdentifiers expected.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
            Expect.sequenceEqual actual.Comments expected.Comments "RegisteredAssayIdentifiers"
        |> Some

open Helper


let private test_coreEmpty =
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcStudy.toJsonString
        ArcStudy.fromJsonString
        None
        compareFields

let private test_compressedEmpty =
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcStudy.toCompressedJsonString
        ArcStudy.fromCompressedJsonString
        None
        compareFields

let private test_isaEmpty =
    createBaseJsonTests
        "isa-empty"
        create_empty
        (fun () (s) -> ArcStudy.toISAJsonString [] s)
        (ArcStudy.fromISAJsonString >> fun (s,_) -> s)
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateStudy)
        #else
        None
        #endif
        compareFields

let private test_roCrateEmpty =

    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        (fun () (s) -> ArcStudy.toROCrateJsonString [] s)
        (ArcStudy.fromROCrateJsonString >> fun (s,_) -> s)
        None
        compareFields

let private test_core =
    createBaseJsonTests
        "core"
        create_filled
        ArcStudy.toJsonString
        ArcStudy.fromJsonString
        None
        compareFields

let private test_compressed =
    createBaseJsonTests
        "compressed"
        create_filled
        ArcStudy.toCompressedJsonString
        ArcStudy.fromCompressedJsonString
        None
        compareFields

let private test_isa =
    createBaseJsonTests
        "isa"
        create_filled
        ArcStudy.toISAJsonString
        (ArcStudy.fromISAJsonString >> fun (s,_) -> s)
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateStudy)
        #else
        None
        #endif
        compareFields

let private test_roCrate =
    createBaseJsonTests
        "ROCrate"
        create_filled
        ArcStudy.toROCrateJsonString
        (ArcStudy.fromROCrateJsonString >> fun (s,_) -> s)
        None
        compareFields

let main = testList "Study" [
    test_coreEmpty
    test_core
    test_compressedEmpty
    test_compressed
    test_isaEmpty
    test_isa
    test_roCrateEmpty
    test_roCrate
]