﻿module Tests.Investigation

open ARCtrl
open ARCtrl.Json
open TestingUtils


module Helper =
    let create_empty() = ArcInvestigation.create("My Astonishing Investigation")
    let create_filled() = 
        let assay1 = Assay.Helper.create_filled();
        let assay2 = Assay.Helper.create_empty();
        let study1 = Study.Helper.create_filled();
        let study2 = Study.Helper.create_empty()
        study1.RegisteredAssayIdentifiers <- ResizeArray()
        study2.RegisteredAssayIdentifiers <- ResizeArray()
        let inv =
            ArcInvestigation.make
                "My Inv"
                (Some "My Title")
                (Some "My Description")
                (Some "2024-03-15")
                (Some "2024-04-20T20:20:39")
                (ResizeArray [OntologySourceReference.create("Description", "path/to/file", "OSR Name")])
                (ResizeArray [Publication.create(doi="any-nice-doi-42")])
                (ResizeArray [Person.create(firstName="Kevin", lastName="Frey")])
                (ResizeArray [])
                (ResizeArray [study1; study2])
                (ResizeArray [study1.Identifier; study2.Identifier])
                (ResizeArray [Comment.create("Hello", "World")])
                (ResizeArray())
        inv.AddAssay(assay1, inv.Studies)
        inv.AddAssay(assay2, inv.Studies)
        inv

    let compareFields (expected: ArcInvestigation) (actual: ArcInvestigation) =
        Expect.equal actual.Identifier expected.Identifier "Identifier"
        Expect.equal actual.Title expected.Title "Title"
        Expect.equal actual.Description expected.Description "Description"
        Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
        Expect.sequenceEqual actual.OntologySourceReferences expected.OntologySourceReferences "OntologySourceReferences"
        Expect.sequenceEqual actual.Publications expected.Publications "Publications"
        Expect.sequenceEqual actual.Contacts expected.Contacts "Contacts"
        Expect.sequenceEqual actual.Assays expected.Assays "Assays"
        Expect.sequenceEqual actual.Studies expected.Studies "Studies"
        Expect.sequenceEqual actual.RegisteredStudyIdentifiers expected.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
        Expect.sequenceEqual actual.Comments expected.Comments "Comments"
        Expect.sequenceEqual actual.Remarks expected.Remarks "Remarks"
    
open Helper

let private test_coreEmpty =
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcInvestigation.toJsonString
        ArcInvestigation.fromJsonString
        None

let private test_compressedEmpty =
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcInvestigation.toCompressedJsonString
        ArcInvestigation.fromCompressedJsonString
        None

let private test_isaEmpty =
    createBaseJsonTests
        "isa-empty"
        create_empty
        ArcInvestigation.toISAJsonString
        ArcInvestigation.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateInvestigation)
        #else
        None
        #endif

let private test_roCrateEmpty =
    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        ArcInvestigation.toROCrateJsonString
        ArcInvestigation.fromROCrateJsonString
        None


let private test_core =
    createBaseJsonTests
        "core"
        create_filled
        ArcInvestigation.toJsonString
        ArcInvestigation.fromJsonString
        None

let private test_compressed =

    createBaseJsonTests
        "compressed"
        create_filled
        ArcInvestigation.toCompressedJsonString
        ArcInvestigation.fromCompressedJsonString
        None

let private test_isa =

    createBaseJsonTests
        "isa"
        create_filled
        ArcInvestigation.toISAJsonString
        ArcInvestigation.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateInvestigation)
        #else
        None
        #endif

let private test_roCrate =
    createBaseJsonTests
        "ROCrate"
        create_filled
        ArcInvestigation.toROCrateJsonString
        ArcInvestigation.fromROCrateJsonString
        None

let main = testList "Investigation" [
    test_coreEmpty
    test_core
    test_compressedEmpty
    test_compressed
    test_isaEmpty
    test_isa
    test_roCrateEmpty
    test_roCrate
]