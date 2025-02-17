module Tests.LDNode

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common
open ARCtrl.Helper

let context =
    new LDContext()
    //|> DynObj.withProperty "more" "context"

context.AddMapping("more","context")

let mandatory_properties = LDNode("LDNode_mandatory_properties_id", ResizeArray[|"someType"|])
let mandatory_properties_with_context =
    LDNode("LDNode_mandatory_properties_id", ResizeArray[|"someType"|])
    |> DynObj.withProperty "@context" context

let all_properties = LDNode("LDNode_all_properties_id", ResizeArray[|"someType"|], additionalType = ResizeArray[|"additionalType"|])
let all_properties_with_context =
    LDNode("LDNode_all_properties_id", ResizeArray[|"someType"|], additionalType = ResizeArray[|"additionalType"|])
    |> DynObj.withProperty "@context" (context.DeepCopy())

let tests_profile_object_is_valid = testList "constructed properties" [
    testList "mandatory properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "LDNode_mandatory_properties_id" mandatory_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "someType" mandatory_properties
    ]
    testList "all properties" [
        testCase "Id" <| fun _ -> Expect.LDNodeHasId "LDNode_all_properties_id" all_properties
        testCase "SchemaType" <| fun _ -> Expect.LDNodeHasType "someType" all_properties
        testCase "AdditionalType" <| fun _ -> Expect.LDNodeHasAdditionalType "additionalType" all_properties
    ]
]

//let tests_interface_members = testList "interface members" [
//    testCase "mandatoryProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"someType"|] "LDNode_mandatory_properties_id" [||] mandatory_properties
//    testCase "allProperties" <| fun _ -> Expect.LDNodeHasExpectedInterfaceMembers [|"someType"|] "LDNode_all_properties_id" [|"additionalType"|] all_properties
//]

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
        context.AddMapping("more", "context")

        testCase "can set context" <| fun _ ->
            mandatory_properties.SetContext context
            Expect.LDNodeHasDynamicProperty "@context" context mandatory_properties
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
            context.AddMapping("more", "context")

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
        testList "tryFromDynamicObj" [
            let compatibleDynObj =
                let tmp = DynamicObj()
                tmp
                |> DynObj.withProperty "@type" "someType"
                |> DynObj.withProperty "@id" "LDNode_all_properties_id"
                |> DynObj.withProperty "additionalType" "additionalType"

            let compatibleDynObjWithContext =
                let tmp = DynamicObj()
                tmp
                |> DynObj.withProperty "@type" "someType"
                |> DynObj.withProperty "@id" "LDNode_all_properties_id"
                |> DynObj.withProperty "additionalType" "additionalType"
                |> DynObj.withProperty "@context" context

            let incompatibleDynObj =
                let tmp = DynamicObj()
                tmp
                |> DynObj.withProperty "@type" "someType"

            testCase "can convert compatible DynObj to LDNode" <| fun _ ->
                let roc = Expect.wantSome (LDNode.tryFromDynamicObj compatibleDynObj) "LDNode.tryFromDynamicObj did not return Some"
                Expect.equal roc all_properties "LDNode was not created correctly from compatible DynamicObj"
            testCase "can convert compatible DynObj with context to LDNode" <| fun _ ->
                let roc = Expect.wantSome (LDNode.tryFromDynamicObj compatibleDynObjWithContext) "LDNode.tryFromDynamicObj did not return Some"
                Expect.equal roc all_properties_with_context "LDNode was not created correctly from compatible DynamicObj"
            testCase "cannot convert incompatible DynObj to LDNode" <| fun _ ->
                Expect.isNone (LDNode.tryFromDynamicObj incompatibleDynObj) "LDNode.tryFromDynamicObj did not return None"
        ]
    ]
)

let tests_TryGetProperty = testList "TryGetProperty" [
    testList "NoContext" [
        testCase "null" <| fun _ -> 
            let node = new LDNode("MyNode",ResizeArray ["https://schema.org/Thing"])
            let v = node.TryGetProperty("MyProperty")
            Expect.isNone v "missing property was resolved"
        testCase "fullIRI" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let nameKey = "https://schema.org/name"
            let nameValue = "MyName"
            node.SetProperty(nameKey, nameValue)
            let v = node.TryGetProperty(nameKey)
            let v = Expect.wantSome v "property was not resolved"
            Expect.equal v nameValue "property was not resolved correctly"
        testCase "Term" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let nameKey = "name"
            let nameValue = "MyName"
            node.SetProperty(nameKey, nameValue)
            let v = node.TryGetProperty(nameKey)
            let v = Expect.wantSome v "property was not resolved"
            Expect.equal v nameValue "property was not resolved correctly"
    ]
    testList "SimpleContext" [
        testCase "null" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("name", "https://schema.org/name")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let v = node.TryGetProperty("name")
            Expect.isNone v "missing property was resolved"
        testCase "IRISet_TermGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("name", "https://schema.org/name")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let nameKey = "https://schema.org/name"
            let nameValue = "MyName"
            node.SetProperty(nameKey, nameValue)
            let v = node.TryGetProperty("name")
            let v = Expect.wantSome v "property was not resolved"
            Expect.equal v nameValue "property was not resolved correctly"
        testCase "TermSet_TermGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("name", "https://schema.org/name")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let nameKey = "name"
            let nameValue = "MyName"
            node.SetProperty(nameKey, nameValue)
            let v = node.TryGetProperty("name")
            let v = Expect.wantSome v "property was not resolved"
            Expect.equal v nameValue "property was not resolved correctly"
        testCase "IRISet_IRIGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("name", "https://schema.org/name")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let nameKey = "https://schema.org/name"
            let nameValue = "MyName"
            node.SetProperty(nameKey, nameValue)
            let v = node.TryGetProperty(nameKey)
            let v = Expect.wantSome v "property was not resolved"
            Expect.equal v nameValue "property was not resolved correctly"
        testCase "TermSet_IRIGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("name", "https://schema.org/name")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let nameKey = "name"
            let nameValue = "MyName"
            node.SetProperty(nameKey, nameValue)
            let v = node.TryGetProperty("https://schema.org/name")
            let v = Expect.wantSome v "property was not resolved"
            Expect.equal v nameValue "property was not resolved correctly"
        ]
    ]

let tests_HasType = testList "HasType" [
    testList "NoContext" [
        testCase "null" <| fun _ -> 
            let node = new LDNode("MyNode",ResizeArray ["https://schema.org/Thing"])
            let v = node.HasType("https://schema.org/Person")
            Expect.isFalse v "missing type was resolved"
        testCase "fullIRI" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let v = node.HasType("https://schema.org/Thing")
            Expect.isTrue v "type was not resolved"
        testCase "Term" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["Thing"])
            let v = node.HasType("Thing")
            Expect.isTrue v "type was not resolved"
    ]
    testList "SimpleContext" [
        testCase "null" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("Thing", "https://schema.org/Thing")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let v = node.HasType("https://schema.org/Person")
            Expect.isFalse v "missing type was resolved"
        testCase "IRISet_IRIGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("Thing", "https://schema.org/Thing")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let v = node.HasType("https://schema.org/Thing")
            Expect.isTrue v "type was not resolved"
        testCase "TermSet_TermGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("Thing", "https://schema.org/Thing")
            let node = new LDNode("MyNode", ResizeArray ["Thing"],context = context)
            let v = node.HasType("Thing")
            Expect.isTrue v "type was not resolved"
        testCase "IRISet_TermGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("Thing", "https://schema.org/Thing")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"],context = context)
            let v = node.HasType("Thing")
            Expect.isTrue v "type was not resolved"
        testCase "TermSet_IRIGet" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("Thing", "https://schema.org/Thing")
            let node = new LDNode("MyNode", ResizeArray ["Thing"],context = context)
            let v = node.HasType("https://schema.org/Thing")
            Expect.isTrue v "type was not resolved"
        testCase "MultiType" <| fun _ ->
            let context = new LDContext()
            context.AddMapping("Thing", "https://schema.org/Thing")
            context.AddMapping("Person", "https://schema.org/Person")
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing";"https://schema.org/Person"],context = context)
            let v = node.HasType("https://schema.org/Thing")
            Expect.isTrue v "type was not resolved"
            let v = node.HasType("https://schema.org/Person")
            Expect.isTrue v "type was not resolved"
            let v = node.HasType("Thing")
            Expect.isTrue v "type was not resolved"
            let v = node.HasType("Person")
            Expect.isTrue v "type was not resolved"      
    ]
]



let tests_GetPropertyValues = testList "GetPropertyValues" [
    testCase "null" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let v = node.GetPropertyValues("https://schema.org/name")
        Expect.isEmpty v "missing type was resolved"
    testCase "SequenceSet" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let values = seq {"Name1";"Name2"}
        node.SetProperty("https://schema.org/name", values)
        let v = node.GetPropertyValues("https://schema.org/name") |> ResizeArray.map (fun x -> x :?> string)
        Expect.sequenceEqual v values "values were not resolved"
    testCase "ArraySet" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let values = ResizeArray ["Name1";"Name2"]
        node.SetProperty("https://schema.org/name", values)
        let v = node.GetPropertyValues("https://schema.org/name") |> ResizeArray.map (fun x -> x :?> string)
        Expect.sequenceEqual v values "values were not resolved"
    testCase "SingleSet" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let value = "Name1"
        node.SetProperty("https://schema.org/name", value)
        let v = node.GetPropertyValues("https://schema.org/name") |> ResizeArray.map (fun x -> x :?> string)
        Expect.sequenceEqual v (ResizeArray [value]) "values were not resolved"
    testList "Filter" [
        testCase "OnlyRetrieveNumbers" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let values : ResizeArray<obj> = ResizeArray [box "Name1";3;4;"Name2"]
            node.SetProperty("https://schema.org/name", values)
            let filter = fun (x : obj) _ -> x :? int
            let v = node.GetPropertyValues("https://schema.org/name", filter = filter) |> ResizeArray.map (fun x -> x :?> int)
            Expect.sequenceEqual v (ResizeArray [3;4]) "values were not resolved"
    ]        
]

let tests_GetPropertyNodes = testList "GetPropertyNodes" [
    testCase "null" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let v = node.GetPropertyNodes("https://schema.org/name")
        Expect.isEmpty v "missing type was resolved"
    testCase "SequenceSet" <| fun _ ->
        let internalNode1 = new LDNode("MyNode1",ResizeArray ["https://schema.org/Thing"])
        let internalNode2 = new LDNode("MyNode2",ResizeArray ["https://schema.org/Thing"])
        let values = seq {internalNode1;internalNode2}
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", values)
        let v = node.GetPropertyNodes("https://schema.org/about")
        Expect.sequenceEqual v (ResizeArray [internalNode1;internalNode2]) "values were not resolved"
    testCase "IgnoreNonNodes" <| fun _ ->
        let internalNode1 = new LDNode("MyNode1",ResizeArray ["https://schema.org/Thing"])
        let internalNode2 = new LDNode("MyNode2",ResizeArray ["https://schema.org/Thing"])
        let values : seq<obj> = seq {internalNode1 |> box;5;internalNode2;"NotANode"}
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", values)
        let v = node.GetPropertyNodes("https://schema.org/about")
        Expect.sequenceEqual v (ResizeArray [internalNode1;internalNode2]) "values were not resolved"
    testList "Filter" [
        testCase "FilterForType" <| fun _ ->
            let internalNode1 = new LDNode("MyNode1", ResizeArray ["https://schema.org/Person"])
            let internalNode2 = new LDNode("MyNode2", ResizeArray ["https://schema.org/CreativeWork"])
            let values = ResizeArray [internalNode1;internalNode2]
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            node.SetProperty("https://schema.org/about", values)
            let filter = fun (x : LDNode) _ -> x.HasType("https://schema.org/Person")
            let v = node.GetPropertyNodes("https://schema.org/about", filter = filter)
            Expect.sequenceEqual v (ResizeArray [internalNode1]) "values were not resolved"
    ]
]

let main = testList "LDNode" [
    tests_profile_object_is_valid
    //tests_interface_members
    tests_dynamic_members
    tests_instance_methods
    tests_static_methods
    tests_TryGetProperty
    tests_HasType
    tests_GetPropertyValues
    tests_GetPropertyNodes
]