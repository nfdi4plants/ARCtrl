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
        "My Submission Date",
        "My Release Date",
        ResizeArray [|Publication.create(doi="any-nice-doi-42")|],
        ResizeArray [|Person.create(firstName="Kevin", lastName="Frey", phone="023382093810")|], 
        ResizeArray [|OntologyAnnotation(); OntologyAnnotation()|],
        ResizeArray [ArcTable.Helper.create_filled(); ArcTable.init("Table 2")],
        ResizeArray ["Assay 1"; "Assay 2"],
        ResizeArray [|Comment.create("Hello", "World")|]
    )

open Helper


let private test_coreEmpty =
    let json = """{"Identifier":"My Assay"}"""
    createBaseJsonTests
        "core-empty"
        create_empty
        json
        ArcStudy.toJsonString
        ArcStudy.fromJsonString

let private test_compressedEmpty =
    let json = """{"cellTable":[],"oaTable":[],"stringTable":[],"object":{"Identifier":"My Assay"}}"""
    createBaseJsonTests
        "compressed-empty"
        create_empty
        json
        ArcStudy.toCompressedJsonString
        ArcStudy.fromCompressedJsonString

//let private test_isaEmpty =
//    let json = """{"filename":"assays/My Assay/isa.assay.xlsx"}"""
//    createBaseJsonTests
//        "isa-empty"
//        create_empty
//        json
//        ArcStudy.toISAJsonString
//        ArcStudy.fromISAJsonString

//let private test_roCrateEmpty =
//    let json = """{"@id":"#assay/My_Assay","@type":["Assay"],"additionalType":"Assay","identifier":"My Assay","filename":"assays/My Assay/isa.assay.xlsx","@context":{"sdo":"http://schema.org/","Assay":"sdo:Dataset","identifier":"sdo:identifier","additionalType":"sdo:additionalType","measurementType":"sdo:variableMeasured","technologyType":"sdo:measurementTechnique","technologyPlatform":"sdo:measurementMethod","dataFiles":"sdo:hasPart","performers":"sdo:creator","processSequences":"sdo:about","comments":"sdo:comment","filename":"sdo:url"}}"""
//    createBaseJsonTests
//        "ROCrate-empty"
//        create_empty
//        json
//        ArcStudy.toROCrateJsonString
//        ArcStudy.fromROCrateJsonString


let Main = testList "Study" [
    ()
]