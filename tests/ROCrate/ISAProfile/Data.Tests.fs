module Tests.Data

open ARCtrl.ROCrate

open TestingUtils

let tests_profile_object_is_valid = testList "profile object is valid" []

let tests_static_methods = testList "static methods" []

let tests_interface_members = testList "interface members" []

let tests_dynamic_members = testList "dynamic members" []

let main = testList "Data" [
    tests_profile_object_is_valid
    tests_static_methods
    tests_interface_members
    tests_dynamic_members
]

