module ARCtrl.WebRequest.Tests

open TestingUtils
open ARCtrl.WebRequest

let tests_download = testList "Download" [
    testCaseAsync "simple json" <| async {
        let url = @"https://raw.githubusercontent.com/nfdi4plants/ARCtrl/developer/tests/TestingUtils/TestObjects.Json/MinimalJson.json"
        let expected = """{"TestKey": "TestValue"}"""
        let! jsonString = downloadFile url 
        Expect.equal jsonString expected "equal"
    }
]

let main = testList "WebRequest" [
    tests_download
]