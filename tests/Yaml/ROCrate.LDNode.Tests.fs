module Tests.ROCrate.LDNode

open TestingUtils
open ARCtrl.ROCrate
open ARCtrl.Yaml.ROCrate
open DynamicObj

module YamlFixtures = TestObjects.Yaml.ROCrate

let private readTests = testList "Read" [
    testCase "Fixture_Contains_AtType_Key" <| fun _ ->
        let element = ARCtrl.Yaml.Decode.fromYamlString id YamlFixtures.GenericObjects.onlyIDAndType
        let mappings = ARCtrl.Yaml.ROCrate.Helpers.getMappings element
        let keys = mappings |> List.map fst
        Expect.containsAll keys ["@id"; "@type"] "Fixture YAML should parse with both @id and @type keys"

    testCase "Unquoted_At_Keys_Are_Parsed" <| fun _ ->
        let element = ARCtrl.Yaml.Decode.fromYamlString id "@id: MyIdentifier\n@type: MyType"
        let mappings = ARCtrl.Yaml.ROCrate.Helpers.getMappings element
        let keys = mappings |> List.map fst
        Expect.containsAll keys ["@id"; "@type"] "Unquoted @-keys should parse"

    testCase "onlyIDAndType" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.onlyIDAndType
        Expect.equal node.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual node.SchemaType (ResizeArray ["MyType"]) "type was not parsed correctly"

    testCase "onlyID" <| fun _ ->
        Expect.throws (fun () -> LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.onlyID |> ignore) "Should fail if @type is missing"

    testCase "onlyType" <| fun _ ->
        Expect.throws (fun () -> LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.onlyType |> ignore) "Should fail if @id is missing"

    testCase "Strict_At_Keywords_Required" <| fun _ ->
        Expect.throws (fun () -> LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withoutAtKeywords |> ignore) "Should require JSON-LD '@' keywords"

    testCase "twoTypesAndID" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.twoTypesAndID
        Expect.equal node.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual node.SchemaType (ResizeArray ["MyType"; "MySecondType"]) "type was not parsed correctly"

    testCase "withMixedFields" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withMixedFields
        let name = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "name" node) "name was not parsed"
        let number = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "number" node) "number was not parsed"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" node) "nested was not parsed"
        Expect.equal name "MyName" "name value was wrong"
        Expect.equal number 42 "number value was wrong"
        Expect.equal nested.Count 3 "nested count was wrong"

    testCase "withStringFields" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withStringFields
        let name = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "name" node) "name was not parsed"
        let description = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "description" node) "description was not parsed"
        Expect.equal name "MyName" "name value was wrong"
        Expect.equal description "MyDescription" "description value was wrong"

    testCase "withIntFields" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withIntFields
        let number = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "number" node) "number was not parsed"
        let anotherNumber = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "anotherNumber" node) "anotherNumber was not parsed"
        Expect.equal number 42 "number value was wrong"
        Expect.equal anotherNumber 1337 "anotherNumber value was wrong"

    testCase "withStringArray" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withStringArray
        let names = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "names" node) "names was not parsed"
        Expect.equal names.Count 2 "names count was wrong"
        Expect.equal names.[0] "MyName" "first name was wrong"
        Expect.equal names.[1] "MySecondName" "second name was wrong"

    testCase "withExpandedStringFieldNoType" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withExpandedStringFieldNoType
        let name = Expect.wantSome (node.TryGetProperty "name") "name was not parsed"
        Expect.equal name (LDValue("MyName")) "expanded string value was wrong"

    testCase "withExpandedIntFieldWithType" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withExpandedIntFieldWithType
        let number = Expect.wantSome (node.TryGetProperty "number") "number was not parsed"
        Expect.equal number (LDValue(42, valueType = "http://www.w3.org/2001/XMLSchema#int")) "expanded int value was wrong"

    testCase "withLDRefObject" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withLDRefObject
        let refObj = Expect.wantSome (node.TryGetProperty "nested") "nested ref was not parsed"
        Expect.equal refObj (LDRef("RefIdentifier")) "LDRef value was wrong"

    testCase "withNestedObject" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withNestedObject
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<LDNode> "nested" node) "nested node was not parsed"
        Expect.equal nested.Id "MyIdentifier" "nested id was wrong"
        Expect.sequenceEqual nested.SchemaType (ResizeArray ["MyType"]) "nested type was wrong"

    testCase "withObjectArray" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withObjectArray
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" node) "nested array was not parsed"
        Expect.equal nested.Count 2 "nested count was wrong"
        let n1 = nested.[0] :?> LDNode
        let n2 = nested.[1] :?> LDNode
        Expect.equal n1.Id "MyIdentifier" "first nested id was wrong"
        Expect.equal n2.Id "MySecondIdentifier" "second nested id was wrong"

    testCase "withAdditionalTypeString" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withAdditionalTypeString
        Expect.sequenceEqual node.AdditionalType (ResizeArray ["additionalType"]) "additionalType string was not parsed"

    testCase "withAdditionalTypeArray" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withAdditionalTypeArray
        Expect.sequenceEqual node.AdditionalType (ResizeArray ["additionalType1"; "additionalType2"]) "additionalType array was not parsed"

    testCase "nullValue" <| fun _ ->
        let node = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withNullValue
        Expect.isNone (node.TryGetProperty "name") "null properties should not be set"

    testCase "Reject_MultiDocument_Stream" <| fun _ ->
        let yaml = YamlFixtures.GenericObjects.onlyIDAndType + "\n---\n'@id': Another\n'@type': MyType\n"
        Expect.throws (fun () -> LDNode.fromROCrateYamlString yaml |> ignore) "YAML-LD parser should reject multi-document streams"
]

let private writeTests = testList "Write" [
    testCase "roundtrip (toYaml >> fromYaml)" <| fun _ ->
        let original = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withMixedFields
        let yaml = LDNode.toROCrateYamlString (Some 2) original
        let parsed = LDNode.fromROCrateYamlString yaml
        Expect.equal parsed original "Roundtrip conversion failed"

    testCase "roundtrip_bool_property" <| fun _ ->
        let original = LDNode("MyID", ResizeArray ["MyType"])
        original.SetProperty("isTrue", true)
        let yaml = LDNode.toROCrateYamlString (Some 2) original
        let parsed = LDNode.fromROCrateYamlString yaml
        Expect.equal parsed original "Bool property roundtrip failed"

    testCase "roundtrip_additionalType" <| fun _ ->
        let original = LDNode.fromROCrateYamlString YamlFixtures.GenericObjects.withAdditionalTypeArray
        let yaml = LDNode.toROCrateYamlString (Some 2) original
        let parsed = LDNode.fromROCrateYamlString yaml
        Expect.equal parsed original "additionalType roundtrip failed"
]

let main = testList "ROCrate.LDNode" [
    readTests
    writeTests
]
