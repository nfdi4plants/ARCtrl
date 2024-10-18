module Tests.Assay

open ARCtrl.ROCrate
open DynamicObj

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
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "assay_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "schema.org/Dataset" mandatory_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDObjectHasAdditionalType "Assay" mandatory_properties
        testCase "identifier" <| fun _ -> Expect.LDObjectHasDynamicProperty "identifier" "identifier" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "assay_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "schema.org/Dataset" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDObjectHasAdditionalType "Assay" all_properties
        testCase "identifier" <| fun _ -> Expect.LDObjectHasDynamicProperty "identifier" "identifier" all_properties
        testCase "about" <| fun _ -> Expect.LDObjectHasDynamicProperty "about" "about" all_properties
        testCase "comment" <| fun _ -> Expect.LDObjectHasDynamicProperty "comment" "comment" all_properties
        testCase "creator" <| fun _ -> Expect.LDObjectHasDynamicProperty "creator" "creator" all_properties
        testCase "hasPart" <| fun _ -> Expect.LDObjectHasDynamicProperty "hasPart" "hasPart" all_properties
        testCase "measurementMethod" <| fun _ -> Expect.LDObjectHasDynamicProperty "measurementMethod" "measurementMethod" all_properties
        testCase "measurementTechnique" <| fun _ -> Expect.LDObjectHasDynamicProperty "measurementTechnique" "measurementTechnique" all_properties
        testCase "url" <| fun _ -> Expect.LDObjectHasDynamicProperty "url" "url" all_properties
        testCase "variableMeasured" <| fun _ -> Expect.LDObjectHasDynamicProperty "variableMeasured" "variableMeasured" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers "schema.org/Dataset" "assay_mandatory_properties_id" (Some "Assay") mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers "schema.org/Dataset" "assay_all_properties_id" (Some "Assay") all_properties
]

let tests_dynamic_members = testSequenced (
    testList "dynamic members" [
        testCase "property not present before setting" <| fun _ -> Expect.isNone (DynObj.tryGetTypedPropertyValue<int> "yes" mandatory_properties) "dynamic property 'yes' was set although it was expected not to be set"
        testCase "Set dynamic property" <| fun _ ->
            mandatory_properties.SetProperty("yes",42)
            Expect.LDObjectHasDynamicProperty "yes" 42 mandatory_properties
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
            Expect.LDObjectHasDynamicProperty "@context" context mandatory_properties
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
                    let tmp = new Assay("id", "identifier")
                    tmp.RemoveProperty("identifier") |> ignore
                    tmp.GetIdentifier() |> ignore
                )
                "unset identifier did not throw"
        testCase "incorrectly typed identifier throws" <| fun _ ->
            Expect.throws
                (fun () ->
                    let tmp = new Assay("id", "identifier")
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
            LDObject.setContext context mandatory_properties
            Expect.LDObjectHasDynamicProperty "@context" context mandatory_properties
        testCase "can get context" <| fun _ ->
            let ctx = LDObject.tryGetContext() mandatory_properties
            Expect.equal ctx (Some context) "context was not set correctly"
        testCase "can remove context" <| fun _ ->
            LDObject.removeContext() mandatory_properties |> ignore
            Expect.isNone (DynObj.tryGetTypedPropertyValue<DynamicObj> "@context" mandatory_properties) "context was not removed correctly"
    ]
)

let main = testList "Assay" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]