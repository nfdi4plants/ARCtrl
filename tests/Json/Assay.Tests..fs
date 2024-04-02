module Tests.Assay

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_empty() = ArcAssay.create("My Assay")
    let create_filled() = 
        ArcAssay.create(
            "My Cool Assay",
            OntologyAnnotation("MT", "MS", "MS:424242", ResizeArray [Comment.create("ByeBye","Space")]), 
            OntologyAnnotation("TT", "MS", "MS:696969"), 
            OntologyAnnotation("TP", "MS", "MS:123456", ResizeArray [Comment.create("Hello","Space")]), 
            ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            ResizeArray [|Comment.create("Hello", "World")|]
        )

open Helper

let private test_coreEmpty =
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcAssay.toJsonString
        ArcAssay.fromJsonString
        None

let private test_compressedEmpty =
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcAssay.toCompressedJsonString
        ArcAssay.fromCompressedJsonString
        None

let private test_isaEmpty =
    createBaseJsonTests
        "isa-empty"
        create_empty
        ArcAssay.toISAJsonString
        ArcAssay.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateAssay)
        #else
        None
        #endif

let private test_roCrateEmpty =
    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        (fun () -> ArcAssay.toROCrateJsonString None)
        ArcAssay.fromROCrateJsonString
        None

let private test_core =
    createBaseJsonTests
        "core"
        create_filled
        ArcAssay.toJsonString
        ArcAssay.fromJsonString
        None

let private test_compressed =
    createBaseJsonTests
        "compressed"
        create_filled
        ArcAssay.toCompressedJsonString
        ArcAssay.fromCompressedJsonString
        None

open TestObjects.Json

let private test_isa =
    createBaseJsonTests
        "isa"
        (fun () -> 
            let a = create_filled() 
            a.Performers <- ResizeArray() // ISA-JSON Assay does not contain persons
            a)
        ArcAssay.toISAJsonString
        ArcAssay.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateAssay)
        #else
        None
        #endif

let private test_roCrate =
    createBaseJsonTests
        "ROCrate"
        create_filled
        (fun () -> ArcAssay.toROCrateJsonString None)
        ArcAssay.fromROCrateJsonString
        None

let main = testList "Assay" [
    test_coreEmpty
    test_compressedEmpty
    test_isaEmpty
    test_roCrateEmpty
    test_core
    test_compressed
    test_isa
    test_roCrate
]