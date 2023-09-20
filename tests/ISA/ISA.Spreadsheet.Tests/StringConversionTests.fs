module StringConversionTests

open ARCtrl.ISA

open TestingUtils

let singleItems =

    testList "SingleItems" [
 
        //testCase "FullStrings" (fun () -> 
        //    let name = "Name"
        //    let term = "Term"
        //    let accession = "Accession"
        //    let source = "Source"

        //    let testOntology = OntologyAnnotation.make None (Some (Text term)) (Some source) (Some accession) None
        //    let ontology = OntologyAnnotation.fromString(term,source,accession)

        //    Expect.equal ontology testOntology "Ontology Annotation was not created correctly from strings"
        //    Expect.equal (OntologyAnnotation.toString ontology) (term,source,accession) "Ontology Annotation was not parsed correctly to strings"

        //    let testComponent = Component.make (Some name) (Some (Value.Name name)) None (Some testOntology)
        //    let componentx = Component.fromString name term source  accession

        //    Expect.equal componentx testComponent "Component was not created correctly from strings"
        //    Expect.equal (Component.toString componentx) (name,term,source,accession) "Component was not parsed correctly to strings"

        //    let testPParam = ProtocolParameter.make None (Some testOntology)
        //    let pParam = ProtocolParameter.fromString term source accession

        //    Expect.equal pParam testPParam "Protocol Parameter was not created correctly from strings"
        //    Expect.equal (ProtocolParameter.toString pParam) (term,source,accession) "Protocol Parameter was not parsed correctly to strings"
        //)
        //testCase "EmptyString" (fun () -> 
        //    let name = ""
        //    let term = ""
        //    let accession = ""
        //    let source = ""

        //    let testOntology = OntologyAnnotation.make None None None None None
        //    let ontology = OntologyAnnotation.fromString term accession source

        //    Expect.equal ontology testOntology "Empty Ontology Annotation was not created correctly from strings"
        //    Expect.equal (OntologyAnnotation.toString ontology) (term,accession,source) "Empty Ontology Annotation was not parsed correctly to strings"

        //    let testComponent = Component.make None None None None
        //    let componentx = Component.fromString name term accession source 

        //    Expect.equal componentx testComponent "Empty Component was not created correctly from strings"
        //    Expect.equal (Component.toString componentx) (name,term,accession,source) "Empty Component was not parsed correctly to strings"

        //    let testPParam = ProtocolParameter.make None None
        //    let pParam = ProtocolParameter.fromString term accession source

        //    Expect.equal pParam testPParam "Empty Protocol Parameter was not created correctly from strings"
        //    Expect.equal (ProtocolParameter.toString pParam) (term,accession,source) "Empty Protocol Parameter was not parsed correctly to strings"
        //)

    ]


let aggregated =
     testList "StringAggregationTests" [
 
            //testCase "FullStrings" (fun () -> 
            //    let names = "Name1;Name2"
            //    let terms = "Term1;Term2"
            //    let accessions = "Accession1;Accession2"
            //    let sources = "Source1;Source2"

            //    let testOntologies = 
            //        [
            //            OntologyAnnotation.fromString "Term1" "Accession1" "Source1"
            //            OntologyAnnotation.fromString "Term2" "Accession2" "Source2"
            //        ]
            //    let ontologies = OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources

            //    Expect.sequenceEqual ontologies testOntologies "Ontology Annotations were not created correctly from aggregated strings"
            //    Expect.equal (OntologyAnnotation.toAggregatedStrings ';' ontologies) (terms,accessions,sources) "Ontology Annotations were not parsed correctly to aggregated strings"

            //    let testComponents = 
            //        [
            //            Component.fromString "Name1" "Term1" "Accession1" "Source1"
            //            Component.fromString "Name2" "Term2" "Accession2" "Source2"
            //        ]
            //    let components = Component.fromAggregatedStrings ';' names terms accessions sources

            //    Expect.sequenceEqual components testComponents "Components were not created correctly from aggregated strings"
            //    Expect.equal (Component.toAggregatedStrings ';' components) (names,terms,accessions,sources) "Components were not parsed correctly to aggregated strings"

            //    let testPParams = 
            //        [
            //            ProtocolParameter.fromString "Term1" "Accession1" "Source1"
            //            ProtocolParameter.fromString "Term2" "Accession2" "Source2"
            //        ]
            //    let pParams = ProtocolParameter.fromAggregatedStrings ';' terms accessions sources

            //    Expect.sequenceEqual pParams testPParams "Protocol Parameters were not created correctly from aggregated strings"
            //    Expect.equal (ProtocolParameter.toAggregatedStrings ';' pParams) (terms,accessions,sources) "Protocol Parameters were not parsed correctly to aggregated strings"
            //)

            //testCase "EmptyStrings" (fun () -> 
            //    let names = ""
            //    let terms = ""
            //    let accessions = ""
            //    let sources = ""

            //    let testOntologies = [ ]
            //    let ontologies = OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources

            //    Expect.sequenceEqual ontologies testOntologies "Ontology Annotations were not created correctly from empty aggregated strings. Empty strings should results in an empty list"
            //    Expect.equal (OntologyAnnotation.toAggregatedStrings ';' ontologies) (terms,accessions,sources) "Ontology Annotations were not parsed correctly to empty aggregated strings"

            //    let testComponents = []
            //    let components = Component.fromAggregatedStrings ';' names terms accessions sources

            //    Expect.sequenceEqual components testComponents "Components were not created correctly from empty aggregated strings. Empty strings should results in an empty list"
            //    Expect.equal (Component.toAggregatedStrings ';' components) (names,terms,accessions,sources) "Components were not parsed correctly to empty aggregated strings"

            //    let testPParams = []
            //    let pParams = ProtocolParameter.fromAggregatedStrings ';' terms accessions sources

            //    Expect.sequenceEqual pParams testPParams "Protocol Parameters were not created correctly from aggregated strings. Empty strings should results in an empty list"
            //    Expect.equal (ProtocolParameter.toAggregatedStrings ';' pParams) (terms,accessions,sources) "Protocol Parameters were not parsed correctly to aggregated strings"
            //)
            //testCase "PartlyEmptyStrings" (fun () -> 
            //    let names = ""
            //    let terms = "Term1;Term2"
            //    let accessions = "Accession1;Accession2"
            //    let sources = ""

            //    let testOntologies = 
            //        [
            //            OntologyAnnotation.fromString "Term1" "Accession1" ""
            //            OntologyAnnotation.fromString "Term2" "Accession2" ""
            //        ]
            //    let ontologies = OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources

            //    Expect.sequenceEqual ontologies testOntologies "Ontology Annotations were not created correctly from partly empty aggregated strings"
            //    Expect.equal (OntologyAnnotation.toAggregatedStrings ';' ontologies) (terms,accessions,";") "Ontology Annotations were not parsed correctly to partly empty aggregated strings"

            //    let testComponents = 
            //        [
            //            Component.fromString "" "Term1" "Accession1" ""
            //            Component.fromString "" "Term2" "Accession2" ""
            //        ]
            //    let components = Component.fromAggregatedStrings ';' names terms accessions sources

            //    Expect.sequenceEqual components testComponents "Components were not created correctly from partly empty aggregated strings"
            //    Expect.equal (Component.toAggregatedStrings ';' components) (";",terms,accessions,";") "Components were not parsed correctly to partly empty aggregated strings"

            //    let testPParams = 
            //        [
            //            ProtocolParameter.fromString "Term1" "Accession1" ""
            //            ProtocolParameter.fromString "Term2" "Accession2" ""
            //        ]
            //    let pParams = ProtocolParameter.fromAggregatedStrings ';' terms accessions sources

            //    Expect.sequenceEqual pParams testPParams "Protocol Parameters were not created correctly from partly empty aggregated strings"
            //    Expect.equal (ProtocolParameter.toAggregatedStrings ';' pParams) (terms,accessions,";") "Protocol Parameters were not parsed correctly to partly empty aggregated strings"
            //)

            //testCase "DifferingLengths" (fun () -> 
            //    let names = "Name1"
            //    let terms = "Term1;Term2"
            //    let accessions = "Accession1;Accession2"
            //    let sources = "Accession2"

            //    let ontologies = 
            //        try OntologyAnnotation.fromAggregatedStrings ';' terms accessions sources |> Some 
            //        with
            //        | _ -> None

            //    Expect.isNone ontologies "Parsing aggregated string to ontologies should have failed because of differing lengths"

            //    let components = 
            //        try Component.fromAggregatedStrings ';' names terms accessions sources |> Some 
            //        with
            //        | _ -> None

            //    Expect.isNone components "Parsing aggregated string to compnents should have failed because of differing lengths"
                
            //    let pParams = 
            //        try ProtocolParameter.fromAggregatedStrings ';' terms accessions sources |> Some 
            //        with
            //        | _ -> None

            //    Expect.isNone pParams "Parsing aggregated string to protocol parameters should have failed because of differing lengths"
                
            //)
        ]

let value = 
    testList "Value" [
            //testCase "ParseOntology"(fun () ->

            //    let value = Value.fromOptions (Some "Name") (Some "Source") (Some "Accession")

            //    Expect.isSome value "Should have returned Value but returned None"

            //    let expectedAnnotationValue = AnnotationValue.Text "Name"
            //    let expectedAnnotation = OntologyAnnotation.make None (Some expectedAnnotationValue) (Some "Source") (Some "Accession") None
            //    let expectedValue = Value.Ontology expectedAnnotation

            //    Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            //)
            //testCase "ParseText"(fun () ->

            //    let value = Value.fromOptions (Some "Name") None None

            //    Expect.isSome value "Should have returned Value but returned None"

            //    let expectedValue = Value.Name "Name"

            //    Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            //)
            //testCase "ParseInt"(fun () ->

            //    let value = Value.fromOptions (Some "5") None None

            //    Expect.isSome value "Should have returned Value but returned None"

            //    let expectedValue = Value.Int 5

            //    Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            //)
            //testCase "ParseFloat"(fun () ->

            //    let value = Value.fromOptions (Some "2.3") None None

            //    Expect.isSome value "Should have returned Value but returned None"

            //    let expectedValue = Value.Float 2.3

            //    Expect.equal value.Value expectedValue "Value was parsed incorrectly"
            //)

        ]


let main = 
    testList "StringConversion" [
    ]