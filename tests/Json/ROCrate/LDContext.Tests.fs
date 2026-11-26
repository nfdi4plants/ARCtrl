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
    testCase "ARC_Proxy" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder "\"https://www.researchobject.org/ro-terms/arc/context.jsonld\""
        Expect.equal context.Name (Some Context.proxy_ARC) "context name was not set correctly"
        let resolvedColumnIndex = Expect.wantSome (context.TryResolveTerm("columnIndex")) "Could not resolve term columnIndex"
        Expect.equal resolvedColumnIndex "https://w3id.org/ro/terms/arc#columnIndex" "term columnIndex was not resolved correctly"
        Expect.hasLength context.Mappings 1 "context was not read correctly"
    testCase "ArrayWithARC_Proxy" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_roCrateWithARCContext
        Expect.hasLength context.BaseContexts 3 "context does not contain all three contexts"
        Expect.equal (context.TryResolveTerm("Dataset")) (Some "http://schema.org/Dataset") "term of the RO-Crate context was not resolved correctly"
        Expect.equal (context.TryResolveTerm("Sample")) (Some "https://bioschemas.org/Sample") "term of the inline context was not resolved correctly"
        Expect.equal (context.TryResolveTerm("columnIndex")) (Some "https://w3id.org/ro/terms/arc#columnIndex") "term of the ARC context was not resolved correctly"
    testCase "KeywordsWithoutMapping" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_withKeywords
        let resolvedName = Expect.wantSome (context.TryResolveTerm("name")) "Could not resolve term"
        Expect.equal resolvedName "http://schema.org/name" "term was not resolved correctly"
        Expect.hasLength context.Mappings 2 "keywords should not be read as terms"
    testCase "ExpandedTermDefinition" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_expandedTermDefinition
        let resolvedColumnIndex = Expect.wantSome (context.TryResolveTerm("columnIndex")) "Could not resolve term columnIndex"
        Expect.equal resolvedColumnIndex "https://w3id.org/ro/terms/arc#columnIndex" "term of the expanded term definition was not resolved correctly"
        let resolvedName = Expect.wantSome (context.TryResolveTerm("name")) "Could not resolve term name"
        Expect.equal resolvedName "http://schema.org/name" "term next to the expanded term definition was not resolved correctly"
        Expect.hasLength context.Mappings 2 "context was not read correctly"
    ]

let private test_write = testList "Write" [
    testCase "onlyIDAndType" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_DefinedTerm
        let json = LDContext.encoder context |> Encode.toJsonString 0 
        Expect.stringEqual json context_DefinedTerm "context was not written correctly"
    testCase "ROCrate_V1_3_Proxy" <| fun _ ->
        let json = LDContext.encoder (Context.initV1_3()) |> Encode.toJsonString 0
        Expect.equal json "\"https://w3id.org/ro/crate/1.3/context\"" "context was not written as proxy"
    testCase "ArrayWithARC_Proxy" <| fun _ ->
        let context = Decode.fromJsonString LDContext.decoder context_roCrateWithARCContext
        let json = LDContext.encoder context |> Encode.toJsonString 0
        Expect.stringEqual json context_roCrateWithARCContext "context was not written correctly"
    ]

let main = testList "LDContext" [
    test_read
    test_write
]