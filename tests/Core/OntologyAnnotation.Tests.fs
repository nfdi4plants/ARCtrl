module OntologyAnnotation.Tests

open TestingUtils
open ARCtrl

module Helper =
    let tsr = "EFO"
    let localID = "0000721"
    let short = $"{tsr}:{localID}"
    let uri = $"https://bioregistry.io/{tsr}:{localID}" 
    let otherParseable = $"http://www.ebi.ac.uk/efo/{tsr}_{localID}"
    let other = "Unparseable"

open Helper

let private tests_hashcode = testList "GetHashCode" [
    testCase "Empty" <| fun _ ->
        let oa1 = OntologyAnnotation()
        let oa2 = OntologyAnnotation()
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.equal h1 h2 "Hashes should be equal"    
    testCase "Equal"  <| fun _ ->
        let oa1 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
        let oa2 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.equal h1 h2 "Hashes should be equal"
    testCase "Different"  <| fun _ ->
        let oa1 = OntologyAnnotation("MyOntology",tsr = "EFO",tan = uri)
        let oa2 = OntologyAnnotation("YourOntology",tsr = "NCBI",tan = "http://purl.obolibrary.org/obo/NCBI_0000123")
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.notEqual h1 h2 "Hashes should not be equal"
    // Fix in future, js hash codes for strings are unfortunately not avalanching
    ptestCase "js_clash" <| fun _ ->
        let oa1 = OntologyAnnotation(name = "VBT04P")
        let oa2 = OntologyAnnotation(name = "VBT07s")
        let h1 = oa1.GetHashCode()
        let h2 = oa2.GetHashCode()
        Expect.notEqual h1 h2 "Hashes should not be equal"

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
        let oa1 = OntologyAnnotation("instrument model", "MS", "MS:00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MS", "MS:00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        Expect.equal oa1 oa2 ""
    testCase "Full, comments ignored" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "MS:00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MS", "MS:00000001")
        Expect.equal oa1 oa2 ""
    testCase "TANInfo" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MS", "MS:00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        Expect.equal oa1 oa2 ""
    testCase "TANInfo, missing tsr" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", tan= "MS:00000001", comments=ResizeArray [Comment("KeyName", "Value1")])
        Expect.equal oa1 oa2 "This should not fail"
    testCase "TANInfo, different tsr" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation("instrument model", "MSS", tan= "MS:00000001", comments=ResizeArray [Comment("KeyName", "Value1")])
        Expect.notEqual oa1 oa2 ""
        Expect.isFalse (oa1 = oa2) ""
    // https://github.com/nfdi4plants/ARCtrl/issues/551
    testCase "TANInfo, deprecated OLS uri equals OBO purl" <| fun _ ->
        let oa1 = OntologyAnnotation("seedling", "PO", "https://www.ebi.ac.uk/ols4/ontologies/po/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FPO_0008037")
        let oa2 = OntologyAnnotation("seedling", "PO", "http://purl.obolibrary.org/obo/PO_0008037")
        Expect.equal oa1 oa2 "Annotations with deprecated OLS uri and OBO purl should be equal"
        Expect.equal (oa1.GetHashCode()) (oa2.GetHashCode()) "Hashes should be equal"
    testCase "TSR lowercase" <| fun _ ->
        let oa1 = OntologyAnnotation("UniProt ID", "edam", "EDAM:2291")
        let oa2 = OntologyAnnotation("UniProt ID", "EDAM", "EDAM:2291")
        Expect.equal oa1 oa2 ""
    testCase "not equal" <| fun _ ->   
        let oa1 = OntologyAnnotation("instrument model", "MS", "http://purl.obolibrary.org/obo/MS_00000001", comments = ResizeArray [Comment("KeyName", "Value1")])
        let oa2 = OntologyAnnotation(tan= "MS:00000001", comments=ResizeArray [Comment("KeyName", "Value1")])
        Expect.notEqual oa1 oa2 ""
    testCase "js_clash" <| fun _ ->
        let oa1 = OntologyAnnotation(name = "VBT04P")
        let oa2 = OntologyAnnotation(name = "VBT07s")
        Expect.notEqual oa1 oa2 ""
]

let private tests_constructor = testList "constructor" [
    testCase "FromSplitTSRAndLocalID" (fun () ->
        let oa = OntologyAnnotation(tsr = tsr, tan = localID)
        Expect.equal oa.TermAccessionNumber.Value localID "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "uri TAN incorrect"
    )
    testCase "FromShort" (fun () ->
        let oa = OntologyAnnotation(tan = short)
        Expect.equal oa.TermAccessionNumber.Value short "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "uri TAN incorrect"
    )
    testCase "FromUri" (fun () ->          
        let oa = OntologyAnnotation(tan = uri)
        Expect.equal oa.TermAccessionNumber.Value uri "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "uri TAN incorrect"
    )
    testCase "FromOtherParseable" (fun () ->          
        let oa = OntologyAnnotation(tan = otherParseable)
        Expect.equal oa.TermAccessionNumber.Value otherParseable "TAN incorrect"
        Expect.equal oa.TermAccessionShort short "short TAN incorrect"
        Expect.equal oa.TermAccessionOntobeeUrl uri "uri TAN incorrect"
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
    // https://github.com/nfdi4plants/ARCtrl/issues/551
    // https://github.com/nfdi4plants/ARCtrl/issues/391
    testCase "FromShort_MS_PO_RO" (fun () ->
        for tsr, localID in ["MS","1000121"; "PO","0008037"; "RO","0002533"] do
            let oa = OntologyAnnotation(tan = $"{tsr}:{localID}")
            Expect.equal oa.TermAccessionOntobeeUrl $"http://purl.obolibrary.org/obo/{tsr}_{localID}" $"{tsr}: uri TAN should be OBO purl"
            Expect.equal oa.TermAccessionAndOntobeeUrlIfShort $"http://purl.obolibrary.org/obo/{tsr}_{localID}" $"{tsr}: short TAN should be written as OBO purl"
    )
    testCase "FromDeprecatedOLSUri" (fun () ->
        // Not built with string interpolation, as Fable treats the % in it as format specifiers
        for tsr, localID, olsUri in [
            "MS", "1000121", "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1000121"
            "PO", "0008037", "https://www.ebi.ac.uk/ols4/ontologies/po/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FPO_0008037"
            "RO", "0002533", "https://www.ebi.ac.uk/ols4/ontologies/ro/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FRO_0002533"
        ] do
            let oa = OntologyAnnotation(tsr = tsr, tan = olsUri)
            Expect.equal oa.TermAccessionNumber.Value olsUri $"{tsr}: TAN should not be changed"
            Expect.equal oa.TermAccessionShort $"{tsr}:{localID}" $"{tsr}: short TAN incorrect"
            Expect.equal oa.TermAccessionAndOntobeeUrlIfShort olsUri $"{tsr}: existing uri should be written unchanged"
            Expect.equal oa.TermAccessionOntobeeUrl $"http://purl.obolibrary.org/obo/{tsr}_{localID}" $"{tsr}: uri TAN should be OBO purl"
    )
    testCase "FromTermAnnotation_DeprecatedOLSUri" (fun () ->
        let olsUri = "https://www.ebi.ac.uk/ols4/ontologies/po/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FPO_0008037"
        let oa = OntologyAnnotation.fromTermAnnotation(olsUri, name = "seedling")
        Expect.equal oa.Name (Some "seedling") "Name incorrect"
        Expect.equal oa.TermSourceREF (Some "PO") "TSR incorrect"
        Expect.equal oa.TermAccessionNumber (Some "PO:0008037") "TAN incorrect"
    )
]

let main = testList "OntologyAnnotation" [
    tests_hashcode
    tests_constructor
    tests_equals
]