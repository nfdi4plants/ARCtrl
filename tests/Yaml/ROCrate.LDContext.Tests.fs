module Tests.ROCrate.LDContext

open TestingUtils
open ARCtrl.Yaml.ROCrate

module YamlFixtures = TestObjects.Yaml.ROCrate

let private readTests = testList "Read" [
    testCase "DefinedTerm" <| fun _ ->
        let context = ARCtrl.Yaml.Decode.fromYamlString LDContext.decoder YamlFixtures.context_DefinedTerm
        let resolvedName = Expect.wantSome (context.TryResolveTerm("annotationValue")) "Could not resolve term"
        let resolvedId = Expect.wantSome (context.TryResolveTerm("id")) "Could not resolve id term"
        Expect.equal resolvedName "http://schema.org/name" "term was not resolved correctly"
        Expect.equal resolvedId "@id" "id keyword alias mapping should be preserved as a regular context term"
        Expect.hasLength context.Mappings 6 "context was not read correctly"
]

let private writeTests = testList "Write" [
    testCase "Roundtrip" <| fun _ ->
        let context = ARCtrl.Yaml.Decode.fromYamlString LDContext.decoder YamlFixtures.context_DefinedTerm
        let yaml = LDContext.encoder context |> ARCtrl.Yaml.Encode.toYamlString 2
        let parsed = ARCtrl.Yaml.Decode.fromYamlString LDContext.decoder yaml
        Expect.equal parsed context "context was not written/read correctly"
]

let main = testList "ROCrate.LDContext" [
    readTests
    writeTests
]
