[<AutoOpenAttribute>]
module BaseJsonTests

open TestingUtils
open ARCtrl.Json



let createBaseJsonTests (name:string) (createObj: unit -> 'A) (toJson: unit -> 'A -> string) (fromJson: string -> 'A) (schemaValidation : Validation.ValidateFunction option) =
    testList $"baseTests - {name}" [

        if schemaValidation.IsSome then
            testAsync "WriterSchemaCorrectness" {
                let obj = createObj()
                let json = toJson () obj
                let! validation = schemaValidation.Value json
                let errorList = validation.GetErrors() |> Array.fold (fun acc x -> acc + x + "\n") ""
                Expect.isTrue validation.Success $"Object did not match schema: {errorList}"
            }

        testCase "roundabout" <| fun _ ->
            let obj = createObj()
            let json = toJson () obj
            let actual = fromJson json
            let expected = createObj()
            Expect.equal actual expected ""
    ]