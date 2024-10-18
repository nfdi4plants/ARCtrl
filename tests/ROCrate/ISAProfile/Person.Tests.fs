module Tests.Person

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let mandatory_properties = Person(
    id = "person_mandatory_properties_id",
    givenName = "givenName"
)

let all_properties = Person(
    id = "person_all_properties_id",
    givenName = "givenName",
    additionalType = "additionalType",
    familyName = "familyName",
    email = "email",
    identifier = "identifier",
    affiliation = "affiliation",
    jobTitle = "jobTitle",
    additionalName = "additionalName",
    address = "address",
    telephone = "telephone",
    faxNumber = "faxNumber",
    disambiguatingDescription = "disambiguatingDescription"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "person_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Person" mandatory_properties
        testCase "givenName" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "givenName" "givenName" all_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "person_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Person" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "additionalType" all_properties
        testCase "givenName" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "givenName" "givenName" all_properties
        testCase "familyName" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "familyName" "familyName" all_properties
        testCase "email" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "email" "email" all_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "identifier" "identifier" all_properties
        testCase "affiliation" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "affiliation" "affiliation" all_properties
        testCase "jobTitle" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "jobTitle" "jobTitle" all_properties
        testCase "additionalName" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "additionalName" "additionalName" all_properties
        testCase "address" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "address" "address" all_properties
        testCase "telephone" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "telephone" "telephone" all_properties
        testCase "faxNumber" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "faxNumber" "faxNumber" all_properties
        testCase "disambiguatingDescription" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "disambiguatingDescription" "disambiguatingDescription" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Person" "person_mandatory_properties_id" None mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Person" "person_all_properties_id" (Some "additionalType") all_properties
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

let tests_instance_methods = testSequenced (
    testList "instance methods" [

        let context = new LDContext()
        context.SetProperty("more", "context")

        testCase "can set context" <| fun _ ->
            mandatory_properties.SetContext context
            Expect.ROCrateObjectHasDynamicProperty "@context" context mandatory_properties
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
        context.SetProperty("more", "context")

        testCase "can set context" <| fun _ ->
            ROCrateObject.setContext context mandatory_properties
            Expect.ROCrateObjectHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = ROCrateObject.tryGetContext() mandatory_properties
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            ROCrateObject.removeContext() mandatory_properties |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
    ]
)

let main = testList "Person" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]