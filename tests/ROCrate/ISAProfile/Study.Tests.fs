module Tests.Study

open ARCtrl.ROCrate
open DynamicObj

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
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "study_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "schema.org/Dataset" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDNodeHasAdditionalType "Study" mandatory_properties
        testCase "identifier" <| fun _ -> Expect.LDNodeHasDynamicProperty "identifier" "identifier" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "study_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "schema.org/Dataset" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDNodeHasAdditionalType "Study" all_properties
        testCase "identifier" <| fun _ -> Expect.LDNodeHasDynamicProperty "identifier" "identifier" all_properties
        testCase "about" <| fun _ -> Expect.LDNodeHasDynamicProperty "about" "about" all_properties
        testCase "citation" <| fun _ -> Expect.LDNodeHasDynamicProperty "citation" "citation" all_properties
        testCase "comment" <| fun _ -> Expect.LDNodeHasDynamicProperty "comment" "comment" all_properties
        testCase "creator" <| fun _ -> Expect.LDNodeHasDynamicProperty "creator" "creator" all_properties
        testCase "dateCreated" <| fun _ -> Expect.LDNodeHasDynamicProperty "dateCreated" "dateCreated" all_properties
        testCase "dateModified" <| fun _ -> Expect.LDNodeHasDynamicProperty "dateModified" "dateModified" all_properties
        testCase "datePublished" <| fun _ -> Expect.LDNodeHasDynamicProperty "datePublished" "datePublished" all_properties
        testCase "description" <| fun _ -> Expect.LDNodeHasDynamicProperty "description" "description" all_properties
        testCase "hasPart" <| fun _ -> Expect.LDNodeHasDynamicProperty "hasPart" "hasPart" all_properties
        testCase "headline" <| fun _ -> Expect.LDNodeHasDynamicProperty "headline" "headline" all_properties
        testCase "url" <| fun _ -> Expect.LDNodeHasDynamicProperty "url" "url" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"schema.org/Dataset"|] "study_mandatory_properties_id" [|"Study"|] mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"schema.org/Dataset"|] "study_all_properties_id" [|"Study"|] all_properties
]

let tests_dynamic_members = testSequenced (
    testList "dynamic members" [
        testCase "property not present before setting" <| fun _ -> Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be set"
        testCase "Set dynamic property" <| fun _ ->
            mandatory_properties.SetProperty("yes",42)
            Expect.LDNodeHasDynamicProperty "yes" 42 mandatory_properties
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
            Expect.LDNodeHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = mandatory_properties.TryGetContext()
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            mandatory_properties.RemoveContext() |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
        testCase "can get identifier" <| fun _ ->
            let identifier = mandatory_properties.GetIdentifier()
            Expect.equal identifier "identifier" "identifier was not retrieved correctly"
        testCase "unset identifier throws" <| fun _ ->
            Expect.throws
                (fun () ->
                    let tmp = new Study("id", "identifier")
                    tmp.RemoveProperty("identifier") |> ignore
                    tmp.GetIdentifier() |> ignore
                )
                "unset identifier did not throw"
        testCase "incorrectly typed identifier throws" <| fun _ ->
            Expect.throws
                (fun () ->
                    let tmp = new Study("id", "identifier")
                    tmp.SetProperty("identifier", 42) |> ignore
                    tmp.GetIdentifier() |> ignore
                )
                "incorrectly typed identifier did not throw"
    ]
)

let tests_static_methods = testSequenced (
    testList "static methods" [

        let context = new LDContext()
        context.SetProperty("more", "context")

        testCase "can set context" <| fun _ ->
            LDNode.setContext context mandatory_properties
            Expect.LDNodeHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = LDNode.tryGetContext() mandatory_properties
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            LDNode.removeContext() mandatory_properties |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
    ]
)

let main = testList "Study" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]