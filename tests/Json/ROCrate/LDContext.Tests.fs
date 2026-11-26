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
    testCase "ROCrate_V1_3_Proxy" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder "\"https://w3id.org/ro/crate/1.3/context\""
        Expect.equal context.Name (Some Context.proxy_V1_3) "context name was not set correctly"
        let resolvedDataset = Expect.wantSome (context.TryResolveTerm("Dataset")) "Could not resolve term Dataset"
        Expect.equal resolvedDataset "http://schema.org/Dataset" "term Dataset was not resolved correctly"
        // IRI changed from 1.2 to 1.3
        let resolvedWorkflow = Expect.wantSome (context.TryResolveTerm("ComputationalWorkflow")) "Could not resolve term ComputationalWorkflow"
        Expect.equal resolvedWorkflow "https://bioschemas.org/terms/ComputationalWorkflow" "term ComputationalWorkflow was not resolved correctly"
        // Term added in 1.3
        let resolvedCertification = Expect.wantSome (context.TryResolveTerm("Certification")) "Could not resolve term Certification"
        Expect.equal resolvedCertification "http://schema.org/Certification" "term Certification was not resolved correctly"
        Expect.hasLength context.Mappings 3069 "context does not contain all terms"
    ]

let private test_write = testList "Write" [
    testCase "onlyIDAndType" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_DefinedTerm
        let json = LDContext.encoder context |> Encode.toJsonString 0 
        Expect.stringEqual json context_DefinedTerm "context was not written correctly"
    testCase "ROCrate_V1_3_Proxy" <| fun _ ->
        let json = LDContext.encoder (Context.initV1_3()) |> Encode.toJsonString 0
        Expect.equal json "\"https://w3id.org/ro/crate/1.3/context\"" "context was not written as proxy"
    ]

let main = testList "LDContext" [
    test_read
    test_write
]