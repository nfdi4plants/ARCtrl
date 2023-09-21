module ARCtrl.WebRequest.Tests

open TestingUtils
open ARCtrl.WebRequest

let tests_download = testList "Download" [
    testCaseAsync "simple json" <| async {
        let url = @"https://raw.githubusercontent.com/nfdi4plants/ARCtrl/developer_TemplateRequest/tests/TestingUtils/TestObjects.Json/MinimalJson.json"
        let expected = """{"TestKey": "TestValue"}"""
        do! downloadFile url (fun res ->
            Expect.equal res expected "equal"
        )
    }
]

let main = testList "WebRequest" [
    tests_download
]