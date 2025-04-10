module Tests.PropertyValue

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let mandatory_properties = PropertyValue(
    id = "propertyvalue_mandatory_properties_id",
    name = "name",
    value = "value"
)

let all_properties = PropertyValue(
    id = "propertyvalue_all_properties_id",
    name = "name",
    value = "value",
    propertyID = "propertyID",
    unitCode = "unitCode",
    unitText = "unitText",
    valueReference = "valueReference",
    additionalType = ResizeArray([|"additionalType"|])
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "propertyvalue_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "schema.org/PropertyValue" mandatory_properties
        testCase "name" <| fun _ -> Expect.LDNodeHasDynamicProperty "name" "name" all_properties
        testCase "value" <| fun _ -> Expect.LDNodeHasDynamicProperty "value" "value" all_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "propertyvalue_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "schema.org/PropertyValue" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDNodeHasAdditionalType "additionalType" all_properties
        testCase "name" <| fun _ -> Expect.LDNodeHasDynamicProperty "name" "name" all_properties
        testCase "value" <| fun _ -> Expect.LDNodeHasDynamicProperty "value" "value" all_properties
        testCase "propertyID" <| fun _ -> Expect.LDNodeHasDynamicProperty "propertyID" "propertyID" all_properties
        testCase "unitCode" <| fun _ -> Expect.LDNodeHasDynamicProperty "unitCode" "unitCode" all_properties
        testCase "unitText" <| fun _ -> Expect.LDNodeHasDynamicProperty "unitText" "unitText" all_properties
        testCase "valueReference" <| fun _ -> Expect.LDNodeHasDynamicProperty "valueReference" "valueReference" all_properties
    ]
]

//let tests_interface_members = testList "interface members" [
//    testCase "mandatoryProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"schema.org/PropertyValue"|] "propertyvalue_mandatory_properties_id" [||] mandatory_properties
//    testCase "allProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"schema.org/PropertyValue"|] "propertyvalue_all_properties_id" [|"additionalType"|] all_properties
//]

let tests_dynamic_members = testSequenced (
    testList "dynamic members" [
        testCase "property not present before setting" <| fun _ -> Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be set"
        testCase "Set dynamic property" <| fun _ ->
            mandatory_properties.SetProperty("yes",42)
            Expect.LDNodeHasDynamicProperty "yes" 42 mandatory_properties
        testCase "Remove dynamic property" <| fun _ ->
            mandatory_properties.RemoveProperty("yes") |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be removed"
    ]
)

let tests_instance_methods = testSequenced (
    testList "instance methods" [

        let context = new LDContext()
        context.AddMapping("more", "context")

        testCase "can set context" <| fun _ ->
            mandatory_properties.SetContext context
            Expect.LDNodeHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = mandatory_properties.TryGetContext()
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            mandatory_properties.RemoveContext() |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
    ]
)

let tests_static_methods = testSequenced (
    testList "static methods" [

        let context = new LDContext()
        context.AddMapping("more", "context")

        testCase "can set context" <| fun _ ->
            LDNode.setContext context mandatory_properties
            Expect.LDNodeHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = LDNode.tryGetContext() mandatory_properties
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            LDNode.removeContext() mandatory_properties |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
    ]
)

let main = testList "PropertyValue" [
    tests_profile_object_is_valid
    //tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]