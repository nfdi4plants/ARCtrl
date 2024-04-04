module OntologyAnnotation.Tests

open TestingUtils
open ARCtrl

module private Helper = 
    let short = "EFO:0000721"
    let uri = "http://purl.obolibrary.org/obo/EFO_0000721" 
    let otherParseable = "http://www.ebi.ac.uk/efo/EFO_0000721"
    let other = "Unparseable"

open Helper

let private tests_hashcode = testList "GetHashCode" [
    testCase "Empty" (fun () ->
        let oa1 = OntologyAnnotation()
        let oa2 = OntologyAnnotation()
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.equal h1 h2 "Hashes should be equal"    
    )
    testCase "Equal" (fun () ->
        let oa1 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
        let oa2 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.equal h1 h2 "Hashes should be equal"
    )
    testCase "Different" (fun () ->
        let oa1 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
        let oa2 = OntologyAnnotation("YourOntology",tsr = "NCBI",tan = "http://purl.obolibrary.org/obo/NCBI_0000123")
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.notEqual h1 h2 "Hashes should not be equal"
    )
]

let private tests_equals = testList "Equals" [
    testCase "Empty" <| fun _ ->   
        let oa1 = OntologyAnnotation()
        let oa2 = OntologyAnnotation()
        Expect.equal oa1 oa2 ""
    testCase "Name only" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model")
        let oa2 = OntologyAnnotation("instrument model")
        Expect.equal oa1 oa2 ""
    testCase "Without tan" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS")
        let oa2 = OntologyAnnotation("instrument model", "MS")
        Expect.equal oa1 oa2 ""
    testCase "Without tan, not equal" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS")
        let oa2 = OntologyAnnotation("instrument model", "MSS")
        Expect.notEqual oa1 oa2 ""
    testCase "Full" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "MS:00000001", ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MS", "MS:00000001", ResizeArray [Comment("KeyName", "Value1")])
        Expect.equal oa1 oa2 ""
    testCase "Full, comments ignored" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "MS:00000001", ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MS", "MS:00000001")
        Expect.equal oa1 oa2 ""
    testCase "TANInfo" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MS", "MS:00000001", ResizeArray [Comment("KeyName", "Value1")])
        Expect.equal oa1 oa2 ""
    testCase "TANInfo, missing tsr" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", tan= "MS:00000001", comments=ResizeArray [Comment("KeyName", "Value1")])
        Expect.equal oa1 oa2 "This should not fail"
    testCase "TANInfo, different tsr" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MSS", tan= "MS:00000001", comments=ResizeArray [Comment("KeyName", "Value1")])
        Expect.notEqual oa1 oa2 ""
        Expect.isFalse (oa1 = oa2) ""
    testCase "not equal" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation(tan= "MS:00000001", comments=ResizeArray [Comment("KeyName", "Value1")])
        Expect.notEqual oa1 oa2 ""
]

let private tests_constructor = testList "constructor" [       
    testCase "FromShort" (fun () ->
        let oa = OntologyAnnotation(tan = short)
        Expect.equal oa.TermAccessionNumber.Value short "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
    )
    testCase "FromUri" (fun () ->          
        let oa = OntologyAnnotation(tan = uri)
        Expect.equal oa.TermAccessionNumber.Value uri "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
    )
    testCase "FromOtherParseable" (fun () ->          
        let oa = OntologyAnnotation(tan = otherParseable)
        Expect.equal oa.TermAccessionNumber.Value otherParseable "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "short TAN incorrect"
    )
    testCase "FromOther" (fun () ->          
        let oa = OntologyAnnotation(tan = other)
        Expect.equal oa.TermAccessionNumber.Value other "TAN incorrect"
    )
    testCase "FromOtherWithTSR" (fun () ->          
        let tsr = "ABC"
        let oa = OntologyAnnotation(tsr = tsr,tan = other)
        Expect.equal oa.TermAccessionNumber.Value other "TAN incorrect"
        Expect.equal oa.TermSourceREF.Value tsr "TSR incorrect"
    )
]

let main = testList "OntologyAnnotation" [
    tests_hashcode
    tests_constructor
    tests_equals
]