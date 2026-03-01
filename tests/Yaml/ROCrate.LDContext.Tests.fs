module Tests.ROCrate.LDContext

open TestingUtils
open ARCtrl.Yaml.ROCrate

let private contextYaml =
    """
annotationValue: http://schema.org/name
category: http://schema.org/category
unit: http://schema.org/unitCode
id: '@id'
type: '@type'
value: '@value'
"""

let private readTests = testList "Read" [
    testCase "DefinedTerm" <| fun _ ->
        let context = ARCtrl.Yaml.Decode.fromYamlString LDContext.decoder contextYaml
        let resolvedName = Expect.wantSome (context.TryResolveTerm("annotationValue")) "Could not resolve term"
        Expect.equal resolvedName "http://schema.org/name" "term was not resolved correctly"
        Expect.hasLength context.Mappings 6 "context was not read correctly"
]

let private writeTests = testList "Write" [
    testCase "Roundtrip" <| fun _ ->
        let context = ARCtrl.Yaml.Decode.fromYamlString LDContext.decoder contextYaml
        let yaml = LDContext.encoder context |> ARCtrl.Yaml.Encode.toYamlString 2
        let parsed = ARCtrl.Yaml.Decode.fromYamlString LDContext.decoder yaml
        Expect.equal parsed context "context was not written/read correctly"
]

let main = testList "ROCrate.LDContext" [
    readTests
    writeTests
]
