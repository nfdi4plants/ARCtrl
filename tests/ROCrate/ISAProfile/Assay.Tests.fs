module Tests.Assay

open ARCtrl.ROCrate

open TestingUtils
open Common

let mandatory_properties = Assay(
    id = "assay_mandatory_properties_id",
    identifier = "identifier"
)

let all_properties = Assay(
    id = "assay_all_properties_id",
    identifier = "identifier",
    about = "about",
    comment = "comment",
    creator = "creator",
    hasPart = "hasPart",
    measurementMethod = "measurementMethod",
    measurementTechnique = "measurementTechnique",
    url = "url",
    variableMeasured = "variableMeasured"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "assay_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "Assay" mandatory_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasProperty "identifier" "identifier" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "assay_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "Assay" all_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasProperty "identifier" "identifier" all_properties
        testCase "about" <| fun _ -> Expect.ROCrateObjectHasProperty "about" "about" all_properties
        testCase "comment" <| fun _ -> Expect.ROCrateObjectHasProperty "comment" "comment" all_properties
        testCase "creator" <| fun _ -> Expect.ROCrateObjectHasProperty "creator" "creator" all_properties
        testCase "hasPart" <| fun _ -> Expect.ROCrateObjectHasProperty "hasPart" "hasPart" all_properties
        testCase "measurementMethod" <| fun _ -> Expect.ROCrateObjectHasProperty "measurementMethod" "measurementMethod" all_properties
        testCase "measurementTechnique" <| fun _ -> Expect.ROCrateObjectHasProperty "measurementTechnique" "measurementTechnique" all_properties
        testCase "url" <| fun _ -> Expect.ROCrateObjectHasProperty "url" "url" all_properties
        testCase "variableMeasured" <| fun _ -> Expect.ROCrateObjectHasProperty "variableMeasured" "variableMeasured" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "assay_mandatory_properties_id" (Some "Assay") mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "assay_all_properties_id" (Some "Assay") all_properties
]

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Assay" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
]