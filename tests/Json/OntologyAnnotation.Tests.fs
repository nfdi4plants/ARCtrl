module Tests.OntologyAnnotation

open ARCtrl
open ARCtrl.Json
open TestingUtils

module private Helper =

    let create_comment() = Comment.create("comment", "This is a comment")

    let create_oa() = OntologyAnnotation.create("Peptidase", "MS", "http://purl.obolibrary.org/obo/NCIT_C16965",ResizeArray [|create_comment()|])

open Helper

let private tests_core =
    createBaseJsonTests
        "core"
        create_oa
        OntologyAnnotation.toJsonString
        OntologyAnnotation.fromJsonString
        None

let private tests_isa =
    createBaseJsonTests
        "isa"
        create_oa
       
        OntologyAnnotation.toISAJsonString
        OntologyAnnotation.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateOntologyAnnotation)
        #else
        None
        #endif

let private tests_roCrate = testList "ROCrate" [
    testCase "Write-PropertyValue" <| fun _ -> 
        let oa = create_oa()
        let actual = OntologyAnnotation.ROCrate.encoderPropertyValue oa |> Encode.toJsonString 0
        let expected = TestObjects.Json.ROCrate.propertyValue
        Expect.stringEqual actual expected ""
    testCase "Write-DefinedTerm" <| fun _ -> 
        let oa = create_oa()
        let actual = OntologyAnnotation.ROCrate.encoderDefinedTerm oa |> Encode.toJsonString 0
        let expected = TestObjects.Json.ROCrate.definedTerm
        Expect.stringEqual actual expected ""
    createBaseJsonTests
        ""
        create_oa
        OntologyAnnotation.toROCrateJsonString
        OntologyAnnotation.fromROCrateJsonString
        None
]
    
let main = testList "OntologyAnnotation" [
    tests_core
    tests_isa
    tests_roCrate
]