module Tests.LDNode

open TestingUtils
open TestObjects.Json.ROCrate
open ARCtrl
open ARCtrl.ROCrate
open ARCtrl.Json
open DynamicObj

let private test_read = testList "Read" [
    testCase "onlyIDAndType" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.onlyIDAndType)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
    testCase "onlyID" <| fun _ ->
        let f = fun _ -> LDNode.fromROCrateJsonString(GenericObjects.onlyID) |> ignore
        Expect.throws f "Should fail if Type is missing"
    testCase "onlyType" <| fun _ ->
        let f = fun _ -> LDNode.fromROCrateJsonString(GenericObjects.onlyType) |> ignore
        Expect.throws f "Should fail if ID is missing"
    testCase "twoTypesAndID" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.twoTypesAndID)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"; "MySecondType"] "type was not parsed correctly"
    testCase "withStringFields" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withStringFields)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let name = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "name" json) "field name was not parsed"
        Expect.equal name "MyName" "field name was not parsed correctly"
        let description = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "description" json) "field description was not parsed"
        Expect.equal description "MyDescription" "field description was not parsed correctly"
    testCase "withIntFields" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withIntFields)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let number = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "number" json) "field number was not parsed"
        Expect.equal number 42 "field number was not parsed correctly"
        let anotherNumber = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "anotherNumber" json) "field anotherNumber was not parsed"
        Expect.equal anotherNumber 1337 "field anotherNumber was not parsed correctly"
    testCase "withStringArray" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withStringArray)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let names = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "names" json) "field names was not parsed"
        Expect.equal names.Count 2 "ResizeArray length is wrong"
        Expect.equal names.[0] "MyName" "First name was not parsed correctly"
        Expect.equal names.[1] "MySecondName" "Second name was not parsed correctly"
    testCase "withExpandedStringFieldNoType" <| fun _ -> 
        let json = LDNode.fromROCrateJsonString(GenericObjects.withExpandedStringFieldNoType)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let name = Expect.wantSome (json.TryGetProperty "name") "field name was not parsed"
        let expected = LDValue("MyName")
        Expect.equal name expected "field name was not parsed correctly"
    testCase "withExpandedStringFieldWithType" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withExpandedStringFieldWithType)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let name = Expect.wantSome (json.TryGetProperty "name") "field name was not parsed"
        let expected = LDValue("MyName", valueType = "http://www.w3.org/2001/XMLSchema#string")
        Expect.equal name expected "field name was not parsed correctly"
    testCase "withExpandedIntFieldWithType" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withExpandedIntFieldWithType)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let number = Expect.wantSome (json.TryGetProperty "number") "field number was not parsed"
        let expected = LDValue(42, valueType = "http://www.w3.org/2001/XMLSchema#int")
        Expect.equal number expected "field number was not parsed correctly"
    testCase "withLDRefObject" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withLDRefObject)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let ref = Expect.wantSome (json.TryGetProperty "nested") "field ref was not parsed"
        let expected = LDRef("RefIdentifier")
        Expect.equal ref expected "ref id was not parsed correctly"
    testCase "withNestedObject" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withNestedObject)
        Expect.equal json.Id "OuterIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<LDNode> "nested" json) "field nested was not parsed"
        Expect.equal nested.Id "MyIdentifier" "nested id was not parsed correctly"
        Expect.sequenceEqual nested.SchemaType ResizeArray["MyType"] "nested type was not parsed correctly"
    testCase "withObjectArray" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withObjectArray)
        Expect.equal json.Id "OuterIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" json) "field nested was not parsed"
        Expect.equal nested.Count 2 "ResizeArray length is wrong"
        let o1 = nested.[0] :?> LDNode
        Expect.equal o1.Id "MyIdentifier" "First nested id was not parsed correctly"
        Expect.sequenceEqual o1.SchemaType ResizeArray["MyType"] "First nested type was not parsed correctly"
        let o2 = nested.[1] :?> LDNode
        Expect.equal o2.Id "MyIdentifier" "Second nested id was not parsed correctly"
        Expect.sequenceEqual o2.SchemaType ResizeArray["MyType"] "Second nested type was not parsed correctly"
    testCase "withMixedArray" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withMixedArray)
        Expect.equal json.Id "OuterIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" json) "field nested was not parsed"
        Expect.equal nested.Count 3 "ResizeArray length is wrong"
        let o1 = nested.[0] :?> LDNode
        Expect.equal o1.Id "MyIdentifier" "First nested id of object was not parsed correctly"
        Expect.sequenceEqual o1.SchemaType ResizeArray["MyType"] "First nested type of object was not parsed correctly"
        let o2 = nested.[1] :?> string
        Expect.equal o2 "Value2" "Second nested string was not parsed correctly"
        let o3 = nested.[2] :?> int
        Expect.equal o3 42 "Third nested int was not parsed correctly"
    testCase "withAdditionalTypeString" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withAdditionalTypeString)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        Expect.sequenceEqual json.AdditionalType ResizeArray["additionalType"] "additionalType was not parsed correctly"
    testCase "withAdditionalTypeArray" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withAdditionalTypeArray)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        Expect.sequenceEqual json.AdditionalType ResizeArray["additionalType"] "additionalType was not parsed correctly"
    testCase "withAddtionalTypeArrayMultipleEntries" <| fun _ ->
        let json = LDNode.fromROCrateJsonString(GenericObjects.withAddtionalTypeArrayMultipleEntries)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        Expect.sequenceEqual json.AdditionalType ResizeArray["additionalType1"; "additionalType2"] "additionalType was not parsed correctly"
]

let test_write = testList "write" [
    // The tests suffixed with 'NoTypeArray' are not real roundtrips, as we parse string OR array fields but always write arrays for the @type field.
    testCase "onlyIDAndType" <| fun _ ->
        let json = GenericObjects.onlyIDAndType
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "onlyIDAndTypeNoTypeArray" <| fun _ ->
        let json = GenericObjects.onlyIDAndTypeNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.onlyIDAndType "Output string is not correct"

    testCase "twoTypesAndID" <| fun _ ->
        let json = GenericObjects.twoTypesAndID
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"

    testCase "withStringFields" <| fun _ ->
        let json = GenericObjects.withStringFields
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withStringFieldsNoTypeArray" <| fun _ ->
        let json = GenericObjects.withStringFieldsNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withStringFields "Output string is not correct"

    testCase "withIntFields" <| fun _ ->
        let json = GenericObjects.withIntFields
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withIntFieldsNoTypeArray" <| fun _ ->
        let json = GenericObjects.withIntFieldsNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withIntFields "Output string is not correct"

    testCase "withStringArray" <| fun _ ->
        let json = GenericObjects.withStringArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withStringArrayNoTypeArray" <| fun _ ->
        let json = GenericObjects.withStringArrayNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withStringArray "Output string is not correct"

    testCase "withNestedObject" <| fun _ ->
        let json = GenericObjects.withNestedObject
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withNestedObjectNoTypeArray" <| fun _ ->
        let json = GenericObjects.withNestedObjectNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withNestedObject "Output string is not correct"

    testCase "withObjectArray" <| fun _ ->
        let json = GenericObjects.withObjectArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withObjectArrayNoTypeArray" <| fun _ ->
        let json = GenericObjects.withObjectArrayNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withObjectArray "Output string is not correct"

    testCase "withMixedArray" <| fun _ ->
        let json = GenericObjects.withMixedArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withMixedArrayNoTypeArray" <| fun _ ->
        let json = GenericObjects.withMixedArrayNoTypeArray
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withMixedArray "Output string is not correct"

    testCase "withAddtionalTypeArray" <| fun _ ->
        let json = GenericObjects.withAdditionalTypeArray
        let jsonOut = GenericObjects.withAdditionalTypeString
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output jsonOut "Output string is not correct"
    testCase "withAddtionalTypeArrayMultipleEntries" <| fun _ ->
        let json = GenericObjects.withAddtionalTypeArrayMultipleEntries
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"

    testCase "withAddtionalTypeString" <| fun _ ->
        let json = GenericObjects.withAdditionalTypeString
        let object = LDNode.fromROCrateJsonString(json)
        let output = LDNode.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
]

let main = testList "LDNode" [
    test_read
    test_write
]