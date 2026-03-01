module Tests.ROCrate.LDNode

open TestingUtils
open ARCtrl.ROCrate
open ARCtrl.Yaml.ROCrate
open DynamicObj

let private yaml_onlyIDAndType =
    """
'@id': MyIdentifier
'@type': MyType
"""

let private yaml_twoTypesAndID =
    """
'@id': MyIdentifier
'@type':
  - MyType
  - MySecondType
"""

let private yaml_withMixedFields =
    """
'@id': OuterIdentifier
'@type': MyType
name: MyName
number: 42
nested:
  - '@id': MyIdentifier
    '@type': MyType
  - Value2
  - 1337
"""

let private readTests = testList "Read" [
    testCase "onlyIDAndType" <| fun _ ->
        let node = LDNode.fromROCrateYamlString yaml_onlyIDAndType
        Expect.equal node.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual node.SchemaType (ResizeArray ["MyType"]) "type was not parsed correctly"

    testCase "twoTypesAndID" <| fun _ ->
        let node = LDNode.fromROCrateYamlString yaml_twoTypesAndID
        Expect.equal node.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual node.SchemaType (ResizeArray ["MyType"; "MySecondType"]) "type was not parsed correctly"

    testCase "withMixedFields" <| fun _ ->
        let node = LDNode.fromROCrateYamlString yaml_withMixedFields
        let name = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "name" node) "name was not parsed"
        let number = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "number" node) "number was not parsed"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" node) "nested was not parsed"
        Expect.equal name "MyName" "name value was wrong"
        Expect.equal number 42 "number value was wrong"
        Expect.equal nested.Count 3 "nested count was wrong"
]

let private writeTests = testList "Write" [
    testCase "roundtrip (toYaml >> fromYaml)" <| fun _ ->
        let original = LDNode.fromROCrateYamlString yaml_withMixedFields
        let yaml = LDNode.toROCrateYamlString (Some 2) original
        let parsed = LDNode.fromROCrateYamlString yaml
        Expect.equal parsed original "Roundtrip conversion failed"
]

let main = testList "ROCrate.LDNode" [
    readTests
    writeTests
]
