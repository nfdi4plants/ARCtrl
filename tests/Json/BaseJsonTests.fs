[<AutoOpenAttribute>]
module BaseJsonTests

open TestingUtils

let createBaseJsonTests (name:string) (createObj: unit -> 'A) (jsonString: string) (toJson: unit -> 'A -> string) (fromJson: string -> 'A) =
    testList $"baseTests - {name}" [
        testCase "write" (fun () -> 
            let obj = createObj()
            let actual = toJson () obj
            let expected = jsonString
            Expect.equal actual expected ""
        )
        testCase "read" (fun () -> 
            let json = jsonString
            let expected = createObj()
            let actual = fromJson json
            Expect.equal actual expected ""
        )
        testCase "roundabout" <| fun _ ->
            let obj = createObj()
            let json = toJson () obj
            let actual = fromJson json
            let expected = createObj()
            Expect.equal actual expected ""
    ]