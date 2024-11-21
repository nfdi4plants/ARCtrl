module Tests.LDObject

open TestingUtils
open TestObjects.Json.ROCrate
open ARCtrl
open ARCtrl.ROCrate
open ARCtrl.Json
open DynamicObj

let private test_read = testList "Read" [
    testCase "onlyIDAndType" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.onlyIDAndType)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
    testCase "onlyID" <| fun _ ->
        let f = fun _ -> LDObject.fromROCrateJsonString(GenericObjects.onlyID) |> ignore
        Expect.throws f "Should fail if Type is missing"
    testCase "onlyType" <| fun _ ->
        let f = fun _ -> LDObject.fromROCrateJsonString(GenericObjects.onlyType) |> ignore
        Expect.throws f "Should fail if ID is missing"
    testCase "withStringFields" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withStringFields)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let name = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "name" json) "field name was not parsed"
        Expect.equal name "MyName" "field name was not parsed correctly"
        let description = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "description" json) "field description was not parsed"
        Expect.equal description "MyDescription" "field description was not parsed correctly"
    testCase "withIntFields" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withIntFields)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let number = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "number" json) "field number was not parsed"
        Expect.equal number 42 "field number was not parsed correctly"
        let anotherNumber = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "anotherNumber" json) "field anotherNumber was not parsed"
        Expect.equal anotherNumber 1337 "field anotherNumber was not parsed correctly"
    testCase "withStringArray" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withStringArray)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let names = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "names" json) "field names was not parsed"
        Expect.equal names.Count 2 "ResizeArray length is wrong"
        Expect.equal names.[0] "MyName" "First name was not parsed correctly"
        Expect.equal names.[1] "MySecondName" "Second name was not parsed correctly"
    testCase "withNestedObject" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withNestedObject)
        Expect.equal json.Id "OuterIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<LDObject> "nested" json) "field nested was not parsed"
        Expect.equal nested.Id "MyIdentifier" "nested id was not parsed correctly"
        Expect.sequenceEqual nested.SchemaType ResizeArray["MyType"] "nested type was not parsed correctly"
    testCase "withObjectArray" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withObjectArray)
        Expect.equal json.Id "OuterIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" json) "field nested was not parsed"
        Expect.equal nested.Count 2 "ResizeArray length is wrong"
        let o1 = nested.[0] :?> LDObject
        Expect.equal o1.Id "MyIdentifier" "First nested id was not parsed correctly"
        Expect.sequenceEqual o1.SchemaType ResizeArray["MyType"] "First nested type was not parsed correctly"
        let o2 = nested.[1] :?> LDObject
        Expect.equal o2.Id "MyIdentifier" "Second nested id was not parsed correctly"
        Expect.sequenceEqual o2.SchemaType ResizeArray["MyType"] "Second nested type was not parsed correctly"
    testCase "withMixedArray" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withMixedArray)
        Expect.equal json.Id "OuterIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "nested" json) "field nested was not parsed"
        Expect.equal nested.Count 3 "ResizeArray length is wrong"
        let o1 = nested.[0] :?> LDObject
        Expect.equal o1.Id "MyIdentifier" "First nested id of object was not parsed correctly"
        Expect.sequenceEqual o1.SchemaType ResizeArray["MyType"] "First nested type of object was not parsed correctly"
        let o2 = nested.[1] :?> string
        Expect.equal o2 "Value2" "Second nested string was not parsed correctly"
        let o3 = nested.[2] :?> int
        Expect.equal o3 42 "Third nested int was not parsed correctly"
    testCase "withAdditionalTypeString" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withAdditionalTypeString)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        Expect.sequenceEqual json.AdditionalType ResizeArray["additionalType"] "additionalType was not parsed correctly"
    testCase "withAdditionalTypeArray" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withAdditionalTypeArray)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        Expect.sequenceEqual json.AdditionalType ResizeArray["additionalType"] "additionalType was not parsed correctly"
    testCase "withAddtionalTypeArrayMultipleEntries" <| fun _ ->
        let json = LDObject.fromROCrateJsonString(GenericObjects.withAddtionalTypeArrayMultipleEntries)
        Expect.equal json.Id "MyIdentifier" "id was not parsed correctly"
        Expect.sequenceEqual json.SchemaType ResizeArray["MyType"] "type was not parsed correctly"
        Expect.sequenceEqual json.AdditionalType ResizeArray["additionalType1"; "additionalType2"] "additionalType was not parsed correctly"
]

let test_write = testList "write" [
    // The tests suffixed with 'NoTypeArray' are not real roundtrips, as we parse string OR array fields but always write arrays for the @type field.
    testCase "onlyIDAndType" <| fun _ ->
        let json = GenericObjects.onlyIDAndType
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "onlyIDAndTypeNoTypeArray" <| fun _ ->
        let json = GenericObjects.onlyIDAndTypeNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.onlyIDAndType "Output string is not correct"

    testCase "withStringFields" <| fun _ ->
        let json = GenericObjects.withStringFields
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withStringFieldsNoTypeArray" <| fun _ ->
        let json = GenericObjects.withStringFieldsNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withStringFields "Output string is not correct"

    testCase "withIntFields" <| fun _ ->
        let json = GenericObjects.withIntFields
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withIntFieldsNoTypeArray" <| fun _ ->
        let json = GenericObjects.withIntFieldsNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withIntFields "Output string is not correct"

    testCase "withStringArray" <| fun _ ->
        let json = GenericObjects.withStringArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withStringArrayNoTypeArray" <| fun _ ->
        let json = GenericObjects.withStringArrayNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withStringArray "Output string is not correct"

    testCase "withNestedObject" <| fun _ ->
        let json = GenericObjects.withNestedObject
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withNestedObjectNoTypeArray" <| fun _ ->
        let json = GenericObjects.withNestedObjectNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withNestedObject "Output string is not correct"

    testCase "withObjectArray" <| fun _ ->
        let json = GenericObjects.withObjectArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withObjectArrayNoTypeArray" <| fun _ ->
        let json = GenericObjects.withObjectArrayNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withObjectArray "Output string is not correct"

    testCase "withMixedArray" <| fun _ ->
        let json = GenericObjects.withMixedArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withMixedArrayNoTypeArray" <| fun _ ->
        let json = GenericObjects.withMixedArrayNoTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withMixedArray "Output string is not correct"

    testCase "withAddtionalTypeArray" <| fun _ ->
        let json = GenericObjects.withAdditionalTypeArray
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"
    testCase "withAddtionalTypeArrayMultipleEntries" <| fun _ ->
        let json = GenericObjects.withAddtionalTypeArrayMultipleEntries
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output json "Output string is not correct"

    testCase "withAddtionalTypeString" <| fun _ ->
        let json = GenericObjects.withAdditionalTypeString
        let object = LDObject.fromROCrateJsonString(json)
        let output = LDObject.toROCrateJsonString() object
        Expect.stringEqual output GenericObjects.withAdditionalTypeArray "Output string is not correct"
]

let main = testList "LDObject" [
    test_read
    test_write
]