module Tests.LabProcess

open ARCtrl.ROCrate

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
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "labprocess_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "bioschemas.org/LabProcess" mandatory_properties
        testCase "name" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "name" "name" mandatory_properties
        testCase "agent" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "agent" "agent" mandatory_properties
        testCase "object" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "object" "object" mandatory_properties
        testCase "result" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "result" "result" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "labprocess_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "bioschemas.org/LabProcess" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "additionalType" all_properties
        testCase "name" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "name" "name" all_properties
        testCase "agent" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "agent" "agent" all_properties
        testCase "object" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "object" "object" all_properties
        testCase "result" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "result" "result" all_properties
        testCase "executesLabProtocol" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "executesLabProtocol" "executesLabProtocol" all_properties
        testCase "parameterValue" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "parameterValue" "parameterValue" all_properties
        testCase "endTime" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "endTime" "endTime" all_properties
        testCase "disambiguatingDescription" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "disambiguatingDescription" "disambiguatingDescription" all_properties

    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "bioschemas.org/LabProcess" "labprocess_mandatory_properties_id" None mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "bioschemas.org/LabProcess" "labprocess_all_properties_id" (Some "additionalType") all_properties
]

let tests_dynamic_members = testList "dynamic members" []

let main = testList "LabProcess" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
]