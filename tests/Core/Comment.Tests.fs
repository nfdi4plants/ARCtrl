module Comment.Tests

open TestingUtils
open ARCtrl

let private tests_toAndFromString = testList "ToAndFromString" [
    testCase "Empty" (fun () ->
        let c = Comment()
        let s = c.ToString()
        printfn "%s" s
        let c2 = Comment.fromString s
        Expect.equal c c2 "Should be equal"
    )
    testCase "Only Name" (fun () ->
        let c = Comment("MyName")
        let s = c.ToString()
        printfn "%s" s
        let c2 = Comment.fromString s
        Expect.equal c c2 "Should be equal"
    )
    testCase "Only Value" (fun () ->
        let c = Comment(value = "MyValue")
        let s = c.ToString()
        printfn "%s" s
        let c2 = Comment.fromString s
        Expect.equal c c2 "Should be equal"
    )
    testCase "Name and Value" (fun () ->
        let c = Comment("MyName","MyValue")
        let s = c.ToString()
        printfn "%s" s
        let c2 = Comment.fromString s
        Expect.equal c c2 "Should be equal"
    )
]

let main = testList "Comment" [
    tests_toAndFromString
]