module Tests.Decoder

open TestingUtils
open ARCtrl
open ARCtrl.Json

module private TestHelper =

    open Thoth.Json.Core

    type Person = {
      Name: string
      Age: int
      Gender: string
    }

    let encoder (person: Person) =
      Encode.object [
        "name", Encode.string person.Name
        "age", Encode.int person.Age
        "gender", Encode.string person.Gender
      ]

    let decoder : Decoder<Person> =
      Decode.object (fun get ->
        {
          Name = get.Optional.Field "name" Decode.string |> Option.defaultValue ""
          Age = get.Optional.Field "age" Decode.int |> Option.defaultValue 0
          Gender = get.Optional.Field "gender" Decode.string |> Option.defaultValue ""
        }
      )

open TestHelper

let private tests_NoAdditionalProperties = testList "NoAdditionalProperties" [
    testCase "ensure" <| fun _ ->
        let person = {Name="Kevin"; Age = 30; Gender = "Male"}
        let jsonStr = encoder person |> Encode.toJsonString 0
        let person2 = Decode.fromJsonString decoder jsonStr       
        Expect.equal person2 person ""
    testCase "fail" <| fun _ ->
        let person = {Name="Kevin"; Age = 30; Gender = "Male"}
        let decoder = decoder |> Decode.noAdditionalProperties ["name"; "age"]
        let jsonStr = encoder person |> Encode.toJsonString 0
        let person2 = fun () -> Decode.fromJsonString decoder jsonStr |> ignore
        Expect.throws person2 ""
    testCase "success" <| fun _ ->
        let person = {Name="Kevin"; Age = 30; Gender = "Male"}
        let decoder = decoder |> Decode.noAdditionalProperties ["name"; "age"; "gender"]
        let jsonStr = encoder person |> Encode.toJsonString 0
        let person2 = Decode.fromJsonString decoder jsonStr
        Expect.equal person2 person ""
]

let Main = testList "Decoder" [
    tests_NoAdditionalProperties
]

