module RegexTests

open ARCtrl

open TestingUtils

open ARCtrl.Spreadsheet.Comment

let comment_tests = 
    testList "Comment" [
        testCase "Simple[<" (fun () ->
            let value = "Name"
            let key = $"Comment[<{value}>]"
            let result = 
                match key with
                | Comment r -> Some r
                | _ -> None
            let r = Expect.wantSome result "Matching correct comment against regex did not return result"
            Expect.equal value r "Matching correct comment against regex did not return correct value"
        )
        testCase "Simple [<" (fun () ->
            let value = "Name"
            let key = $"Comment [<{value}>]"
            let result = 
                match key with
                | Comment r -> Some r
                | _ -> None
            let r = Expect.wantSome result "Matching correct comment against regex did not return result"
            Expect.equal value r "Matching correct comment against regex did not return correct value"
        )
        testCase "Simple[" (fun () ->
            let value = "Name"
            let key = $"Comment[<{value}>]"
            let result = 
                match key with
                | Comment r -> Some r
                | _ -> None
            let r = Expect.wantSome result "Matching correct comment against regex did not return result"
            Expect.equal value r "Matching correct comment against regex did not return correct value"
        )
        testCase "Simple [" (fun () ->
            let value = "Name"
            let key = $"Comment [{value}]"
            let result = 
                match key with
                | Comment r -> Some r
                | _ -> None
            let r = Expect.wantSome result "Matching correct comment against regex did not return result"
            Expect.equal value r "Matching correct comment against regex did not return correct value"
        )
        testCase "Wrong (" (fun () ->
            let value = "Name"
            let key = $"Comment ({value})"
            let result = 
                match key with
                | Comment r -> Some r
                | _ -> None
            Expect.isNone result "Matching incorrect comment against regex did not return None"
        )
        testCase "Wrong NoValue [<" (fun () ->
            let key = "Comment [<>]"
            let result = 
                match key with
                | Comment r -> Some r
                | _ -> None
            Expect.isNone result "Matching incorrect comment against regex did not return None"
        )
        ]


let main = 
    testList "Regex" [
        comment_tests
    ]