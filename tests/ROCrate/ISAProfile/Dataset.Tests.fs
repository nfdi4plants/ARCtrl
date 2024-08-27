module Tests.Dataset

open ARCtrl.ROCrate

open TestingUtils

let default_constructor = Dataset("id")

let tests_profile_object_is_valid = testList "profile object is valid" [
    testCase "Id" <| fun _ -> Expect.equal default_constructor.Id "id" "mandatory field Id was not correct"
    testCase "SchemaType" <| fun _ -> Expect.equal default_constructor.SchemaType "schema.org/Dataset" "mandatory field SchemaType was not correct"
    testCase "AdditionalType" <| fun _ -> Expect.isNone default_constructor.AdditionalType "optional field AdditionalType was not correct"
    testCase "Properties" <| fun _ -> Expect.isEmpty default_constructor.Properties "Dynamic properties were not empty"
]

let tests_interface_members = testList "interface members" []

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Dataset" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
]