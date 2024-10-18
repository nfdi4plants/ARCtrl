module Tests.Investigation

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let mandatory_properties = Investigation(
    id = "investigation_mandatory_properties_id",
    identifier = "identifier"
)

let all_properties = Investigation(
    id = "investigation_all_properties_id",
    identifier = "identifier",
    citation = "citation",
    comment = "comment",
    creator = "creator",
    dateCreated = "dateCreated",
    dateModified = "dateModified",
    datePublished = "datePublished",
    hasPart = "hasPart",
    headline = "headline",
    mentions = "mentions",
    url = "url",
    description = "description"
)

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "investigation_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "Investigation" mandatory_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "identifier" "identifier" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.ROCrateObjectHasId "investigation_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.ROCrateObjectHasType "schema.org/Dataset" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.ROCrateObjectHasAdditionalType "Investigation" all_properties
        testCase "identifier" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "identifier" "identifier" all_properties
        testCase "citation" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "citation" "citation" all_properties
        testCase "comment" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "comment" "comment" all_properties
        testCase "creator" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "creator" "creator" all_properties
        testCase "dateCreated" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "dateCreated" "dateCreated" all_properties
        testCase "dateModified" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "dateModified" "dateModified" all_properties
        testCase "datePublished" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "datePublished" "datePublished" all_properties
        testCase "hasPart" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "hasPart" "hasPart" all_properties
        testCase "headline" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "headline" "headline" all_properties
        testCase "mentions" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "mentions" "mentions" all_properties
        testCase "url" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "url" "url" all_properties
        testCase "description" <| fun _ -> Expect.ROCrateObjectHasDynamicProperty "description" "description" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "investigation_mandatory_properties_id" (Some "Investigation") mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.ROCrateObjectHasExpectedInterfaceMembers "schema.org/Dataset" "investigation_all_properties_id" (Some "Investigation") all_properties
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
        testCase "can get identifier" <| fun _ ->
            let identifier = mandatory_properties.GetIdentifier()
            Expect.equal identifier "identifier" "identifier was not retrieved correctly"
        testCase "unset identifier throws" <| fun _ ->
            Expect.throws
                (fun () ->
                    let tmp = new Investigation("id", "identifier")
                    tmp.RemoveProperty("identifier") |> ignore
                    tmp.GetIdentifier() |> ignore
                )
                "unset identifier did not throw"
        testCase "incorrectly typed identifier throws" <| fun _ ->
            Expect.throws
                (fun () ->
                    let tmp = new Investigation("id", "identifier")
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

let main = testList "Investigation" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]