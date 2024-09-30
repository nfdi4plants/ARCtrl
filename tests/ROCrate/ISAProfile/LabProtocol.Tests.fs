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
    additionalType = "additionalType",
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
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "labprotocol_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "bioschemas.org/LabProtocol" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "labprotocol_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "bioschemas.org/LabProtocol" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "additionalType" all_properties
        testCase "name" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "name" "name" all_properties
        testCase "intendedUse" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "intendedUse" "intendedUse" all_properties
        testCase "description" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "description" "description" all_properties
        testCase "url" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "url" "url" all_properties
        testCase "comment" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "comment" "comment" all_properties
        testCase "version" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "version" "version" all_properties
        testCase "labEquipment" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "labEquipment" "labEquipment" all_properties
        testCase "reagent" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "reagent" "reagent" all_properties
        testCase "computationalTool" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "computationalTool" "computationalTool" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "bioschemas.org/LabProtocol" "labprotocol_mandatory_properties_id" None mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "bioschemas.org/LabProtocol" "labprotocol_all_properties_id" (Some "additionalType") all_properties
]

let tests_dynamic_members = testSequenced (
    testList "dynamic members" [
        testCase "property not present before setting" <| fun _ -> Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be set"
        testCase "Set dynamic property" <| fun _ ->
            mandatory_properties.SetProperty("yes",42)
            Expect.ROCrateObjectHasDynamicProperty "yes" 42 mandatory_properties
        testCase "Remove dynamic property" <| fun _ ->
            mandatory_properties.RemoveProperty("yes") |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be removed"
    ]
)

let main = testList "LabProtocol" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
]