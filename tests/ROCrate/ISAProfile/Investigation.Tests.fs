module Tests.Investigation

open ARCtrl.ROCrate

open TestingUtils

let default_constructor = Investigation("id")

let tests_profile_object_is_valid = testList "profile object is valid" [
    testCase "Id" <| fun _ -> Expect.equal default_constructor.Id "id" "mandatory field Id was not correct"
    testCase "SchemaType" <| fun _ -> Expect.equal default_constructor.SchemaType "schema.org/Dataset" "mandatory field SchemaType was not correct"
    testCase "AdditionalType" <| fun _ -> Expect.equal default_constructor.AdditionalType (Some "Investigation") "mandatory field AdditionalType was not correct"
    testCase "Properties" <| fun _ -> Expect.isEmpty default_constructor.Properties "Dynamic properties were not empty"

    // default constructors are missing mandatory fields from the profile. This is by design.
    testCase "missing identifier" <| fun _ -> Expect.throws (fun _ -> default_constructor.GetValue("identifier") |> ignore) "mandatory profile field identifier was present althoughit was expected to be missing"
]

let tests_static_methods = testList "static methods" []

let tests_interface_members = testList "interface members" []

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Investigation" [
    tests_profile_object_is_valid
    tests_static_methods
    tests_interface_members
    tests_dynamic_members
]