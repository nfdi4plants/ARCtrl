module Tests.LDObject

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common

let context =
    new LDContext()
    |> DynObj.withProperty "more" "context"

let mandatory_properties = LDObject("LDObject_mandatory_properties_id", ResizeArray[|"someType"|])
let mandatory_properties_with_context =
    LDObject("LDObject_mandatory_properties_id", ResizeArray[|"someType"|])
    |> DynObj.withProperty "@context" context

let all_properties = LDObject("LDObject_all_properties_id", ResizeArray[|"someType"|], additionalType = ResizeArray[|"additionalType"|])
let all_properties_with_context =
    LDObject("LDObject_all_properties_id", ResizeArray[|"someType"|], additionalType = ResizeArray[|"additionalType"|])
    |> DynObj.withProperty "@context" (context.CopyDynamicProperties())

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "LDObject_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "someType" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDObjectHasId "LDObject_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDObjectHasType "someType" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDObjectHasAdditionalType "additionalType" all_properties
    ]
]

let tests_interface_members = testList "interface members" [
    testCase "mandatoryProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers [|"someType"|] "LDObject_mandatory_properties_id" [||] mandatory_properties
    testCase "allProperties" <| fun _ -> Expect.LDObjectHasExpectedInterfaceMembers [|"someType"|] "LDObject_all_properties_id" [|"additionalType"|] all_properties
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
    ]
)

let tests_static_methods = testSequenced (
    testList "static methods" [
        testList "context" [
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
        testList "tryFromDynamicObj" [
            let compatibleDynObj =
                let tmp = DynamicObj()
                tmp
                |> DynObj.withProperty "@type" "someType"
                |> DynObj.withProperty "@id" "LDObject_all_properties_id"
                |> DynObj.withProperty "additionalType" "additionalType"

            let compatibleDynObjWithContext =
                let tmp = DynamicObj()
                tmp
                |> DynObj.withProperty "@type" "someType"
                |> DynObj.withProperty "@id" "LDObject_all_properties_id"
                |> DynObj.withProperty "additionalType" "additionalType"
                |> DynObj.withProperty "@context" context

            let incompatibleDynObj =
                let tmp = DynamicObj()
                tmp
                |> DynObj.withProperty "@type" "someType"

            testCase "can convert compatible DynObj to LDObject" <| fun _ ->
                let roc = Expect.wantSome (LDObject.tryFromDynamicObj compatibleDynObj) "LDObject.tryFromDynamicObj did not return Some"
                Expect.equal roc all_properties "LDObject was not created correctly from compatible DynamicObj"
            testCase "can convert compatible DynObj with context to LDObject" <| fun _ ->
                let roc = Expect.wantSome (LDObject.tryFromDynamicObj compatibleDynObjWithContext) "LDObject.tryFromDynamicObj did not return Some"
                Expect.equal roc all_properties_with_context "LDObject was not created correctly from compatible DynamicObj"
            testCase "cannot convert incompatible DynObj to LDObject" <| fun _ ->
                Expect.isNone (LDObject.tryFromDynamicObj incompatibleDynObj) "LDObject.tryFromDynamicObj did not return None"
        ]
    ]
)

let main = testList "LDObject" [
    tests_profile_object_is_valid
    tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
]