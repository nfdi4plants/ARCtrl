module Tests.Study

open ARCtrl.ROCrate

open TestingUtils

let default_constructor = Study("id")

let mandatory_properties =

let all_properties = ()

let tests_profile_object_is_valid = testList "profile object is valid" [
    testList "default constructor" [
        testCase "Id" <| fun _ -> Expect.equal default_constructor.Id "id" "mandatory properties Id was not correct"
        testCase "SchemaType" <| fun _ -> Expect.equal default_constructor.SchemaType "schema.org/Dataset" "mandatory property SchemaType was not correct"
        testCase "AdditionalType" <| fun _ -> Expect.equal default_constructor.AdditionalType (Some "Study") "mandatory property AdditionalType was not correct"
        testCase "Properties" <| fun _ -> Expect.isEmpty default_constructor.Properties "Dynamic properties were not empty"

        // default constructors are missing mandatory propertys from the profile. This is by design.
        testCase "missing identifier" <| fun _ -> Expect.throws (fun _ -> default_constructor.GetValue("identifier") |> ignore) "mandatory profile property identifier was present althoughit was expected to be missing"
    ]
]

let tests_static_methods = testList "static methods" []

let tests_interface_members = testList "interface members" []

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Study" [
    tests_profile_object_is_valid
    tests_static_methods
    tests_interface_members
    tests_dynamic_members
]