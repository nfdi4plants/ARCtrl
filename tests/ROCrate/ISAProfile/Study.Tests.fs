module Tests.Study

open ARCtrl.ROCrate

open TestingUtils
open Common

let mandatory_properties = Study(
    id = "study_mandatory_properties_id",
    identifier = "identifier"
)

let all_properties = Study(
    id = "study_all_properties_id",
    identifier = "identifier",
    about = "about",
    citation = "citation",
    comment = "comment",
    creator = "creator",
    dateCreated = "dateCreated",
    dateModified = "dateModified",
    datePublished = "datePublished",
    description = "description",
    hasPart = "hasPart",
    headline = "headline",
    url = "url"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "study_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "Study" mandatory_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasProperty "identifier" "identifier" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "study_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "Study" all_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasProperty "identifier" "identifier" all_properties
        testCase "about" <| fun _ -> Expect.ROCrateObjectHasProperty "about" "about" all_properties
        testCase "citation" <| fun _ -> Expect.ROCrateObjectHasProperty "citation" "citation" all_properties
        testCase "comment" <| fun _ -> Expect.ROCrateObjectHasProperty "comment" "comment" all_properties
        testCase "creator" <| fun _ -> Expect.ROCrateObjectHasProperty "creator" "creator" all_properties
        testCase "dateCreated" <| fun _ -> Expect.ROCrateObjectHasProperty "dateCreated" "dateCreated" all_properties
        testCase "dateModified" <| fun _ -> Expect.ROCrateObjectHasProperty "dateModified" "dateModified" all_properties
        testCase "datePublished" <| fun _ -> Expect.ROCrateObjectHasProperty "datePublished" "datePublished" all_properties
        testCase "description" <| fun _ -> Expect.ROCrateObjectHasProperty "description" "description" all_properties
        testCase "hasPart" <| fun _ -> Expect.ROCrateObjectHasProperty "hasPart" "hasPart" all_properties
        testCase "headline" <| fun _ -> Expect.ROCrateObjectHasProperty "headline" "headline" all_properties
        testCase "url" <| fun _ -> Expect.ROCrateObjectHasProperty "url" "url" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "study_mandatory_properties_id" (Some "Study") mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "study_all_properties_id" (Some "Study") all_properties
]

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Study" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
]