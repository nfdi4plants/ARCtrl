module Tests.Sample

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let mandatory_properties = Sample(
    id = "sample_mandatory_properties_id",
    name = "name"
)

let all_properties = Sample(
    id = "sample_all_properties_id",
    name = "name",
    additionalType = "additionalType",
    additionalProperty = "additionalProperty",
    derivesFrom = "derivesFrom"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "sample_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "bioschemas.org/Sample" mandatory_properties
        testCase "name" <| fun _ -> Expect.LDObjectHasDynamicProperty "name" "name" all_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "sample_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "bioschemas.org/Sample" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDObjectHasAdditionalType "additionalType" all_properties
        testCase "name" <| fun _ -> Expect.LDObjectHasDynamicProperty "name" "name" all_properties
        testCase "additionalProperty" <| fun _ -> Expect.LDObjectHasDynamicProperty "additionalProperty" "additionalProperty" all_properties
        testCase "derivesFrom" <| fun _ -> Expect.LDObjectHasDynamicProperty "derivesFrom" "derivesFrom" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers "bioschemas.org/Sample" "sample_mandatory_properties_id" None mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers "bioschemas.org/Sample" "sample_all_properties_id" (Some "additionalType") all_properties
]

let tests_dynamic_members = testSequenced (
    testList "dynamic members" [
        testCase "property not present before setting" <| fun _ -> Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be set"
        testCase "Set dynamic property" <| fun _ ->
            mandatory_properties.SetProperty("yes",42)
            Expect.LDObjectHasDynamicProperty "yes" 42 mandatory_properties
        testCase "Remove dynamic property" <| fun _ ->
            mandatory_properties.RemoveProperty("yes") |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be removed"
    ]
)

let tests_instance_methods = testSequenced (
    testList "instance methods" [

        let context = new LDContext()
        context.SetProperty("more", "context")

        testCase "can set context" <| fun _ ->
            mandatory_properties.SetContext context
            Expect.LDObjectHasDynamicProperty "@context" context mandatory_properties
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
        context.SetProperty("more", "context")

        testCase "can set context" <| fun _ ->
            LDObject.setContext context mandatory_properties
            Expect.LDObjectHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = LDObject.tryGetContext() mandatory_properties
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            LDObject.removeContext() mandatory_properties |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
    ]
)

let main = testList "Sample" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]