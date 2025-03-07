module Tests.LDContext

open TestingUtils
open TestObjects.Json.ROCrate
open ARCtrl
open ARCtrl.ROCrate
open ARCtrl.Json
open DynamicObj

let private test_read = testList "Read" [
    testCase "DefinedTerm" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_DefinedTerm
        let resolvedName = Expect.wantSome (context.TryResolveTerm("annotationValue")) "Could not resolve term"
        Expect.equal resolvedName "http://schema.org/name" "term was not resolved correctly"
        Expect.hasLength context.Mappings 6 "context was not read correctly"
    ]

let private test_write = testList "Write" [
    testCase "onlyIDAndType" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_DefinedTerm
        let json = LDContext.encoder context |> Encode.toJsonString 0 
        Expect.stringEqual json context_DefinedTerm "context was not written correctly"
    ]

let main = testList "LDContext" [
    test_read
    test_write
]