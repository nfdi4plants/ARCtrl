module Tests.LabProtocol

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let mandatory_properties = LabProtocol(
    id = "labprotocol_mandatory_properties_id"
)

let all_properties = LabProtocol(
    id = "labprotocol_all_properties_id",
    additionalType = ResizeArray([|"additionalType"|]),
    name = "name",
    intendedUse = "intendedUse",
    description = "description",
    url = "url",
    comment = "comment",
    version = "version",
    labEquipment = "labEquipment",
    reagent = "reagent",
    computationalTool = "computationalTool"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "labprotocol_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "bioschemas.org/LabProtocol" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "labprotocol_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "bioschemas.org/LabProtocol" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDNodeHasAdditionalType "additionalType" all_properties
        testCase "name" <| fun _ -> Expect.LDNodeHasDynamicProperty "name" "name" all_properties
        testCase "intendedUse" <| fun _ -> Expect.LDNodeHasDynamicProperty "intendedUse" "intendedUse" all_properties
        testCase "description" <| fun _ -> Expect.LDNodeHasDynamicProperty "description" "description" all_properties
        testCase "url" <| fun _ -> Expect.LDNodeHasDynamicProperty "url" "url" all_properties
        testCase "comment" <| fun _ -> Expect.LDNodeHasDynamicProperty "comment" "comment" all_properties
        testCase "version" <| fun _ -> Expect.LDNodeHasDynamicProperty "version" "version" all_properties
        testCase "labEquipment" <| fun _ -> Expect.LDNodeHasDynamicProperty "labEquipment" "labEquipment" all_properties
        testCase "reagent" <| fun _ -> Expect.LDNodeHasDynamicProperty "reagent" "reagent" all_properties
        testCase "computationalTool" <| fun _ -> Expect.LDNodeHasDynamicProperty "computationalTool" "computationalTool" all_properties
    ]
]

//let tests_interface_members = testList "interface members" [
//    testCase "mandatoryProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"bioschemas.org/LabProtocol"|] "labprotocol_mandatory_properties_id" [||] mandatory_properties
//    testCase "allProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"bioschemas.org/LabProtocol"|] "labprotocol_all_properties_id" [|"additionalType"|] all_properties
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

let main = testList "LabProtocol" [
    tests_profile_object_is_valid
    //tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]