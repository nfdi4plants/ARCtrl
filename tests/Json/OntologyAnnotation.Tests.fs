module Tests.OntologyAnnotation

open ARCtrl
open ARCtrl.Json
open TestingUtils

module private Helper =

    let create_comment() = Comment.create("comment", "This is a comment")

    let create_oa() = OntologyAnnotation.create("Peptidase", "MS", "http://purl.obolibrary.org/obo/NCIT_C16965",ResizeArray [|create_comment()|])

open Helper

let private tests_core =
    //let json = """{"annotationValue":"Peptidase","termSource":"MS","termAccession":"http://purl.obolibrary.org/obo/NCIT_C16965","comments":[{"name":"comment","value":"This is a comment"}]}"""
    createBaseJsonTests
        "core"
        create_oa
        OntologyAnnotation.toJsonString
        OntologyAnnotation.fromJsonString

let private tests_isa =
    //let json = """{"annotationValue":"Peptidase","termSource":"MS","termAccession":"http://purl.obolibrary.org/obo/NCIT_C16965","comments":[{"name":"comment","value":"This is a comment"}]}"""
    createBaseJsonTests
        "isa"
        create_oa
       
        OntologyAnnotation.toISAJsonString
        OntologyAnnotation.fromISAJsonString
    
let private tests_roCrate =
    //let json = """{"@id":"http://purl.obolibrary.org/obo/NCIT_C16965","@type":"OntologyAnnotation","annotationValue":"Peptidase","termSource":"MS","termAccession":"http://purl.obolibrary.org/obo/NCIT_C16965","comments":["{\"@id\":\"#Comment_comment_This_is_a_comment\",\"@type\":\"Comment\",\"name\":\"comment\",\"value\":\"This is a comment\",\"@context\":{\"sdo\":\"http://schema.org/\",\"Comment\":\"sdo:Comment\",\"name\":\"sdo:name\",\"value\":\"sdo:text\"}}"],"@context":{"sdo":"http://schema.org/","OntologyAnnotation":"sdo:DefinedTerm","annotationValue":"sdo:name","termSource":"sdo:inDefinedTermSet","termAccession":"sdo:termCode","comments":"sdo:disambiguatingDescription"}}"""
    createBaseJsonTests
        "isa"
        create_oa
        OntologyAnnotation.toROCrateJsonString
        OntologyAnnotation.fromROCrateJsonString
    

let Main = testList "OntologyAnnotation" [
    tests_core
    tests_isa
    tests_roCrate        
]