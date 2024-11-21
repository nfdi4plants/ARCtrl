module ARCtrl.Template.Tests

open Thoth.Json.Core


open ARCtrl.Json
open ARCtrl
open CrossAsync
open TestingUtils

let private tests_Web = testList "Web" [
    testCaseAsync "getTemplates" <| async {
        let! templatesMap = ARCtrl.Template.Web.getTemplates(None)
        Expect.isTrue (templatesMap.Length > 0) "Count > 0"
    }
]

let main = testList "Templates" [
    tests_Web
]