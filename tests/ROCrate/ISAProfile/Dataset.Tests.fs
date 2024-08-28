module Tests.Dataset

open ARCtrl.ROCrate

open TestingUtils
open Common

let mandatory_properties = Dataset("dataset_mandatory_properties_id")
let all_properties = Dataset("dataset_all_properties_id", additionalType = "additionalType")

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "dataset_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "dataset_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "additionalType" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "dataset_mandatory_properties_id" None mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "dataset_all_properties_id" (Some "additionalType") all_properties
]

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Dataset" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
]