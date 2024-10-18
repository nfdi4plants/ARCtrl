module Tests.LabProcess

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let mandatory_properties = LabProcess(
    id = "labprocess_mandatory_properties_id",
    name = "name",
    agent = "agent",
    object = "object",
    result = "result"
)

let all_properties = LabProcess(
    id = "labprocess_all_properties_id",
    name = "name",
    agent = "agent",
    object = "object",
    result = "result",
    additionalType = "additionalType",
    executesLabProtocol = "executesLabProtocol",
    parameterValue = "parameterValue",
    endTime = "endTime",
    disambiguatingDescription = "disambiguatingDescription"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "labprocess_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "bioschemas.org/LabProcess" mandatory_properties
        testCase "name" <| fun _ -> Expect.LDObjectHasDynamicProperty "name" "name" mandatory_properties
        testCase "agent" <| fun _ -> Expect.LDObjectHasDynamicProperty "agent" "agent" mandatory_properties
        testCase "object" <| fun _ -> Expect.LDObjectHasDynamicProperty "object" "object" mandatory_properties
        testCase "result" <| fun _ -> Expect.LDObjectHasDynamicProperty "result" "result" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "labprocess_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "bioschemas.org/LabProcess" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDObjectHasAdditionalType "additionalType" all_properties
        testCase "name" <| fun _ -> Expect.LDObjectHasDynamicProperty "name" "name" all_properties
        testCase "agent" <| fun _ -> Expect.LDObjectHasDynamicProperty "agent" "agent" all_properties
        testCase "object" <| fun _ -> Expect.LDObjectHasDynamicProperty "object" "object" all_properties
        testCase "result" <| fun _ -> Expect.LDObjectHasDynamicProperty "result" "result" all_properties
        testCase "executesLabProtocol" <| fun _ -> Expect.LDObjectHasDynamicProperty "executesLabProtocol" "executesLabProtocol" all_properties
        testCase "parameterValue" <| fun _ -> Expect.LDObjectHasDynamicProperty "parameterValue" "parameterValue" all_properties
        testCase "endTime" <| fun _ -> Expect.LDObjectHasDynamicProperty "endTime" "endTime" all_properties
        testCase "disambiguatingDescription" <| fun _ -> Expect.LDObjectHasDynamicProperty "disambiguatingDescription" "disambiguatingDescription" all_properties

    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers "bioschemas.org/LabProcess" "labprocess_mandatory_properties_id" None mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers "bioschemas.org/LabProcess" "labprocess_all_properties_id" (Some "additionalType") all_properties
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

let main = testList "LabProcess" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]