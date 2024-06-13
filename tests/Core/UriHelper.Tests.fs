module UriHelper.Tests

open TestingUtils
open ARCtrl

let private tests_createUri =
    testList "CreateUri" [
        testCase "DPBO" (fun () ->
            let tsr = "DPBO"
            let localTan = "0002006"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"http://purl.org/nfdi4plants/ontology/dpbo/DPBO_0002006"
            Expect.equal uri expectedUri ""
        )
        testCase "MS" (fun () ->
            let tsr = "MS"
            let localTan = "1000121"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1000121"
            Expect.equal uri expectedUri ""
        )
        testCase "PO" (fun () ->
            let tsr = "PO"
            let localTan = "0007033"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"https://www.ebi.ac.uk/ols4/ontologies/po/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FPO_0007033"
            Expect.equal uri expectedUri ""
        )
        testCase "RO" (fun () ->
            let tsr = "RO"
            let localTan = "0002533"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"https://www.ebi.ac.uk/ols4/ontologies/ro/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FRO_0002533"
            Expect.equal uri expectedUri ""
        )
        testCase "EFO" (fun () ->
            let tsr = "EFO"
            let localTan = "0000001"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"https://bioregistry.io/EFO:0000001"
            Expect.equal uri expectedUri ""
        )
        testCase "NCIT" (fun () ->
            let tsr = "NCIT"
            let localTan = "C12345"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"https://bioregistry.io/NCIT:C12345"
            Expect.equal uri expectedUri ""
        )
        testCase "BackupPurl" (fun () ->
            let tsr = "OGG"
            let localTan = "3000009507"

            let uri = Helper.Url.createOAUri tsr localTan
            let expectedUri = @"http://purl.obolibrary.org/obo/OGG_3000009507"
            Expect.equal uri expectedUri ""
        )
    ]

let main = 
    testList "UriHelper" [
        tests_createUri
    ]

