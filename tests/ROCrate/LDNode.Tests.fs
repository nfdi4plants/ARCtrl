module Tests.LDNode

open ARCtrl.ROCrate
open DynamicObj

open TestingUtils
open Common
open ARCtrl.Helper

let context =
    new LDContext()
    //|> DynObj.withProperty "more" "context"

let thingType() = ResizeArray ["https://schema.org/Thing"]

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
            Expect.isTrue v "Thing IRI was not resolved"
            let v = node.HasType("https://schema.org/Person")
            Expect.isTrue v "Person IRI was not resolved"
            let v = node.HasType("Thing")
            Expect.isTrue v "Thing Term was not resolved"
            let v = node.HasType("Person")
            Expect.isTrue v "Person Term was not resolved"     
    ]
]



let tests_GetPropertyValues = testList "GetPropertyValues" [
    testCase "null" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let v = node.GetPropertyValues("https://schema.org/name")
        Expect.isEmpty v "missing type was resolved"
    testCase "StringSequence" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let values = seq {"Name1";"Name2"}
        node.SetProperty("https://schema.org/name", values)
        let v = node.GetPropertyValues("https://schema.org/name") |> ResizeArray.map (fun x -> x :?> string)
        Expect.sequenceEqual v values "values were not resolved"
    testCase "StringArray" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let values = ResizeArray ["Name1";"Name2"]
        node.SetProperty("https://schema.org/name", values)
        let v = node.GetPropertyValues("https://schema.org/name") |> ResizeArray.map (fun x -> x :?> string)
        Expect.sequenceEqual v values "values were not resolved"
    testCase "SingleString" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let value = "Name1"
        node.SetProperty("https://schema.org/name", value)
        let v = node.GetPropertyValues("https://schema.org/name") |> ResizeArray.map (fun x -> x :?> string)
        Expect.sequenceEqual v (ResizeArray [value]) "values were not resolved"
    testCase "SingleNode" <| fun _ ->
        let internalNode1 = new LDNode("MyNode1", ResizeArray ["https://schema.org/Thing"])
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", internalNode1)
        let v = node.GetPropertyValues("https://schema.org/about") |> ResizeArray.map (fun x -> x :?> LDNode)
        Expect.sequenceEqual v (ResizeArray [internalNode1]) "values were not resolved"
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
    ptestCase "Flattened_NoGraph" <| fun _ ->
        let internalNode1 = new LDNode("MyNode1",ResizeArray ["https://schema.org/Thing"])
        let internalNode2 = new LDNode("MyNode2",ResizeArray ["https://schema.org/Thing"])
        let values = seq {internalNode1;internalNode2}
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", values)
        let graph = node.Flatten()
        let f () = node.GetPropertyNodes("https://schema.org/about") |> ignore
        Expect.throws f "Should fail, as LDRefs can't be resolved" // Or maybe not? Setting this to pending for now
    testCase "Flattened_WithGraph" <| fun _ ->
        let internalNode1 = new LDNode("MyNode1",ResizeArray ["https://schema.org/Thing"])
        let internalNode2 = new LDNode("MyNode2",ResizeArray ["https://schema.org/Thing"])
        let values = seq {internalNode1;internalNode2}
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", values)
        let graph = node.Flatten()
        let v = node.GetPropertyNodes("https://schema.org/about", graph = graph)
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

let tests_TryGetPropertyAsSingleNode = testList "TryGetPropertyAsSingleNode" [
    testCase "PropertyDoesNotExist" <| fun _ ->
        let node = new LDNode(id = "MyNode", schemaType = ResizeArray ["https://schema.org/Thing"])
        let v = node.TryGetPropertyAsSingleNode("name")
        Expect.isNone v "property was resolved"
    testCase "PropertyIsNotNode" <| fun _ ->
        let node = new LDNode(id = "MyNode", schemaType = ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("name", "MyName")
        let v = node.TryGetPropertyAsSingleNode("name")
        Expect.isNone v "property was resolved"
    testCase "PropertyIsNode" <| fun _ ->
        let internalNode = new LDNode(id = "MyInternal", schemaType = ResizeArray ["https://schema.org/Thing"])
        let node = new LDNode(id = "MyNode", schemaType = ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("about", internalNode)
        let v = Expect.wantSome (node.TryGetPropertyAsSingleNode("about")) "property was not resolved"
        Expect.equal v internalNode "property was not resolved correctly"
    testCase "PropertyIsLDRef_NoGraph" <| fun _ ->
        let node = new LDNode(id = "MyNode", schemaType = ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("about", LDRef("MyInternal"))
        let v = node.TryGetPropertyAsSingleNode("about")
        Expect.isNone v "property was resolved"
    testCase "PropertyIsLDRef_WithGraph" <| fun _ ->
        let internalNode = new LDNode(id = "MyInternal", schemaType = ResizeArray ["https://schema.org/Thing"])
        let graph = new LDGraph()
        graph.AddNode(internalNode)
        let node = new LDNode(id = "MyNode", schemaType = ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("about", LDRef("MyInternal"))
        let v = Expect.wantSome (node.TryGetPropertyAsSingleNode("about", graph = graph)) "property was not resolved"
        Expect.equal v internalNode "property was not resolved correctly"
    ]

let tests_Compact_InPlace = testList "Compact_InPlace" [
        testCase "Type_SetContextTrue" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let context = new LDContext()
            context.AddMapping("thing", "https://schema.org/Thing")
            node.Compact_InPlace(context,setContext=true)
            let ctx = Expect.wantSome (node.TryGetContext()) "context was not set"
            Expect.equal (ctx.TryResolveTerm("thing")) (Some "https://schema.org/Thing") "context was not set correctly"
        testCase "Type_SetContextFalse" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let context = new LDContext()
            context.AddMapping("thing", "https://schema.org/Thing")
            node.Compact_InPlace(context,setContext=false)
            Expect.isNone (node.TryGetContext()) "context was set"
        testCase "Type" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let context = new LDContext()
            context.AddMapping("thing", "https://schema.org/Thing")
            node.Compact_InPlace(context)
            Expect.sequenceEqual node.SchemaType ["thing"] "type was not compacted"
        testCase "StringValue" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let value = LDValue("MyValue")
            node.SetProperty("https://schema.org/name", value)
            let context = new LDContext()
            context.AddMapping("name", "https://schema.org/name")
            node.Compact_InPlace(context)
            // Check compaction of value object to value
            let v = Expect.wantSome (node.TryGetProperty("name")) "property does not exist anymore"
            Expect.equal v "MyValue" "property value was not compacted"
            // Check compaction of property name
            Expect.isTrue ((node :> DynamicObj).HasProperty("name")) "compacted property was not found"
            Expect.isFalse((node :> DynamicObj).HasProperty("https://schema.org/name")) "property name was not compacted"
        testCase "IntValue" <| fun _ ->
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            let value = LDValue(42)
            node.SetProperty("https://schema.org/age", value)
            let context = new LDContext()
            context.AddMapping("age", "https://schema.org/age")
            node.Compact_InPlace(context)
            // Check compaction of value object to value
            let v = Expect.wantSome (node.TryGetProperty("age")) "property does not exist anymore"
            Expect.equal v 42 "property value was not compacted"
            // Check compaction of property name
            Expect.isTrue ((node :> DynamicObj).HasProperty("age")) "compacted property was not found"
            Expect.isFalse((node :> DynamicObj).HasProperty("https://schema.org/age")) "property name was not compacted"
        testCase "NodeValue_Recursive" <| fun _ ->
            let internalNode = new LDNode("MyInternalNode", ResizeArray ["https://schema.org/Thing"])
            let internalValue = LDValue("MyName")
            internalNode.SetProperty("https://schema.org/name", internalValue)
            let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
            node.SetProperty("https://schema.org/about", internalNode)
            let context = new LDContext()
            context.AddMapping("about", "https://schema.org/about")
            context.AddMapping("name", "https://schema.org/name")
            node.Compact_InPlace(context)
            // Check compaction of outer node
            let internalNode = Expect.wantSome (node.TryGetProperty("about")) "outer property does not exist anymore"
            Expect.isTrue ((node :> DynamicObj).HasProperty("about")) "outer compacted property was not found"
            Expect.isFalse((node :> DynamicObj).HasProperty("https://schema.org/about")) "outer property name was not compacted"
            // Check compaction of outer node
            let internalNode = internalNode :?> LDNode
            let v = Expect.wantSome (internalNode.TryGetProperty("name")) "inner property does not exist anymore"
            Expect.equal v "MyName" "inner property value was not compacted"
            Expect.isTrue ((internalNode :> DynamicObj).HasProperty("name")) "inner compacted property was not found"
            Expect.isFalse((internalNode :> DynamicObj).HasProperty("https://schema.org/name")) "inner property name was not compacted"
    ]

let tests_Flatten = testList "Flatten" [
    testCase "EmptyNode" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let graph = node.Flatten()
        Expect.sequenceEqual graph.Nodes [node] "graph was not flattened"
    testCase "SingleNodeValue_Recursive" <| fun _ ->
        let internalNode = new LDNode("MyInternalNode", ResizeArray ["https://schema.org/Thing"])
        let internalValue = "MyName"
        internalNode.SetProperty("https://schema.org/name", internalValue)
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", internalNode)
        let graph = node.Flatten()
        Expect.equal graph.Nodes.Count 2 "Graph should have two nodes"
        let oNode = Expect.wantSome (graph.TryGetNode("MyNode")) "outer node was not found"
        let nodeRef = Expect.wantSome (oNode.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
        Expect.equal nodeRef (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        let iNode = Expect.wantSome (graph.TryGetNode("MyInternalNode")) "inner node was not found"
        let v = Expect.wantSome (iNode.TryGetProperty("https://schema.org/name")) "inner property does not exist anymore"
        Expect.equal v "MyName" "inner property value was not found"
    testCase "PointsToTheSameEmptyNode" <| fun _ ->
        let internalNode() = new LDNode("MyInternalNode", ResizeArray ["https://schema.org/Thing"])
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", internalNode())
        node.SetProperty("https://schema.org/hasPart", internalNode())
        let graph = node.Flatten()
        Expect.equal graph.Nodes.Count 2 "Graph should have two nodes"
        let oNode = Expect.wantSome (graph.TryGetNode("MyNode")) "outer node was not found"
        let nodeRef = Expect.wantSome (oNode.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
        Expect.equal nodeRef (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        let nodeRef = Expect.wantSome (oNode.TryGetProperty("https://schema.org/hasPart")) "outer property should still reference inner node"
        Expect.equal nodeRef (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        Expect.isSome (graph.TryGetNode("MyInternalNode")) "inner node was not found"
    testCase "PointsToSameNodeWithDifferentProperties" <| fun _ ->
        let internalNode1 = new LDNode("MyInternalNode", ResizeArray ["https://schema.org/Thing"])
        internalNode1.SetProperty("https://schema.org/name", "MyName")
        let internalNode2 = new LDNode("MyInternalNode", ResizeArray ["https://schema.org/Thing"])
        internalNode2.SetProperty("https://schema.org/age", 42)
        let node = new LDNode("MyNode" , ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/about", internalNode1)
        node.SetProperty("https://schema.org/hasPart", internalNode2)
        let graph = node.Flatten()
        Expect.equal graph.Nodes.Count 2 "Graph should have 2 nodes"
        let oNode = Expect.wantSome (graph.TryGetNode("MyNode")) "outer node was not found"
        let nodeRef = Expect.wantSome (oNode.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
        Expect.equal nodeRef (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        let nodeRef = Expect.wantSome (oNode.TryGetProperty("https://schema.org/hasPart")) "outer property should still reference inner node"
        Expect.equal nodeRef (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        let iNode = Expect.wantSome (graph.TryGetNode("MyInternalNode")) "inner node was not found"
        let v = Expect.wantSome (iNode.TryGetProperty("https://schema.org/name")) "inner property does not exist anymore"
        Expect.equal v "MyName" "inner property value was not found"
        let v = Expect.wantSome (iNode.TryGetProperty("https://schema.org/age")) "inner property does not exist anymore"
        Expect.equal v 42 "inner property value was not found"
    ]

let tests_getPropertyNames = testList "GetPropertyNames" [
    testCase "EmptyNode" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        let names = node.GetPropertyNames()
        Expect.isEmpty names "Empty node should have no properties"
    testCase "NoContext" <| fun _ ->
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"])
        node.SetProperty("https://schema.org/name", "MyName")
        let names = node.GetPropertyNames()
        Expect.sequenceEqual names ["https://schema.org/name"] "Property name was not found"
    testCase "WithContext" <| fun _ ->
        let context = new LDContext()
        context.AddMapping("name", "https://schema.org/name")
        let node = new LDNode("MyNode", ResizeArray ["https://schema.org/Thing"], context = context)
        node.SetProperty("https://schema.org/name", "MyName")
        let names = node.GetPropertyNames()
        Expect.sequenceEqual names ["https://schema.org/name"] "Property name was not found"
]


let tests_mergeAppendInto_InPlace = testList "mergeAppendInto_InPlace" [
    testCase "EmptyNodes" <| fun _ ->
        let getNode1() = new LDNode("MyNode1", thingType())
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.isEmpty (node2.GetPropertyNames()) "Should have no properties"

    testCase "DifferentKeys" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "MyName")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/age", 42)
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/age"; "https://schema.org/name"] "Should have both Properties"
        Expect.equal (node2.TryGetProperty("https://schema.org/age").Value) 42 "Property value was not copied"
        Expect.equal (node2.TryGetProperty("https://schema.org/name").Value) "MyName" "Property value was not copied"

    testCase "SameKeys_SamePropertyValue" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "MyName")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", "MyName")
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        Expect.equal (node2.TryGetProperty("https://schema.org/name").Value) "MyName" "Property value should not have been modified"

    testCase "SameKeys_DifferentStringValue" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "MyName")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", "MyOtherName")
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["MyName";"MyOtherName"] "Property value should have been modified"

    testCase "SameKeys_StringVsInt" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "MyName")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", 50)
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["MyName" |> box ;50] "Property value should have been modified"

    testCase "SameKeys_DifferentPropertyValue" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "MyName")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", "MyOtherName")
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["MyName";"MyOtherName"] "Property value should have been modified"

    testCase "SameKeys_ArrayVsString_NotExisting" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "MyOtherName")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", ["Name1";"Name2"])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["Name1";"Name2";"MyOtherName"] "Property value should have been modified"

    testCase "SameKeys_ArrayVsString_Existing" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", "Name1")
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", ["Name1";"Name2"])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["Name1";"Name2"] "Property value should have been modified"

    testCase "SameKeys_ArrayVsInt_NotExisting" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", 3)
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", [1;2])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value [1;2;3] "Property value should have been modified"

    testCase "SameKeys_ArrayVsInt_Existing" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", 1)
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", [1;2])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value [1;2] "Property value should have been modified"

    testCase "SameKeys_ArraysWithDifferentValues" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", ["Name1";"Name2"])
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", ["Name3";"Name4"])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["Name3";"Name4";"Name1";"Name2"] "Property value should have been modified"

    testCase "SameKeys_ArraysWithSameValues" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", ["Name1";"Name2"])
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", ["Name1";"Name2"])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["Name1";"Name2"] "Property value should not have been modified"

    testCase "SameKeys_ArraysWithOverlappingValues" <| fun _ ->
        let getNode1() =
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/name", ["Name1";"Name2"])
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/name", ["Name2";"Name3"])
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/name"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/name").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value ["Name2";"Name3";"Name1"] "Property value should not have been modified"

    testCase "SameKeys_LDNode_NotExisting" <| fun _ ->
        let getNode1() =
            let internalNode = new LDNode("MyInternalNode", thingType())
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/about", internalNode)
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/about", "String54")
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/about"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/about").Value
        Expect.isTrue (value :? System.Collections.IEnumerable) "Property value should now be sequence"
        let value = (value :?> System.Collections.IEnumerable)
        Expect.genericSequenceEqual value [new LDNode("MyInternalNode", thingType()) |> box;"String54"] "Property value should have been modified"

    testCase "SameKeys_LDNode_AlreadyExisting" <| fun _ ->
        let getNode1() =
            let internalNode = new LDNode("MyInternalNode", thingType())
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/about", internalNode)
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/about", new LDNode("MyInternalNode", thingType()))
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/about"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/about").Value
        Expect.equal value (new LDNode("MyInternalNode", thingType())) "Property value should have been modified"

    testCase "SameKeys_LDNodeLDRefExisting" <| fun _ ->
        let getNode1() =
            let internalNode = new LDNode("MyInternalNode", thingType())
            let n = new LDNode("MyNode1", thingType())
            n.SetProperty("https://schema.org/about", internalNode)
            n
        let node1 = getNode1()
        let node2 = new LDNode("MyNode2", thingType())
        node2.SetProperty("https://schema.org/about", new LDRef("MyInternalNode"))
        node1.MergeAppendInto_InPlace(node2)
        Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
        Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/about"] "Should have the Property"
        let value = node2.TryGetProperty("https://schema.org/about").Value
        Expect.equal value (new LDRef("MyInternalNode")) "Property value should have been modified"
    testList "WithGraph" [
        testCase "DifferentKeys_String" <| fun _ ->
            let graph = new LDGraph()
            let getNode1() =
                let n = new LDNode("MyNode1", thingType())
                n.SetProperty("https://schema.org/name", "MyName")
                n
            let node1 = getNode1()
            let node2 = new LDNode("MyNode2", thingType())
            node2.SetProperty("https://schema.org/age", 42)
            node1.MergeAppendInto_InPlace(node2, flattenTo = graph)
            Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
            Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/age"; "https://schema.org/name"] "Should have both Properties"
            Expect.equal graph.Nodes.Count 0 "Graph should have 0 nodes"

        testCase "DifferentKeys_LDNode" <| fun _ ->
            let graph = new LDGraph()
            let getNode1() =
                let internalNode = new LDNode("MyInternalNode", thingType())
                let n = new LDNode("MyNode1", thingType())
                n.SetProperty("https://schema.org/about", internalNode)
                n
            let node1 = getNode1()
            let node2 = new LDNode("MyNode2", thingType())
            node2.SetProperty("https://schema.org/age", 42)
            node1.MergeAppendInto_InPlace(node2, flattenTo = graph)
            Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
            Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/age"; "https://schema.org/about"] "Should have both Properties"
            Expect.equal graph.Nodes.Count 1 "Graph should have 1 node"
            Expect.isSome (graph.TryGetNode("MyInternalNode")) "inner node was not found"
            let ref = Expect.wantSome (node2.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
            Expect.equal ref (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        testCase "SameKeys_SameLDNode" <| fun _ ->
            let getNode1() =
                let internalNode = new LDNode("MyInternalNode", thingType())
                let n = new LDNode("MyNode1", thingType())
                n.SetProperty("https://schema.org/about", internalNode)
                n
            let node1 = getNode1()
            let node2 = new LDNode("MyNode2", thingType())
            node2.SetProperty("https://schema.org/about", new LDNode("MyInternalNode", thingType()))
            let graph = node2.Flatten()
            node1.MergeAppendInto_InPlace(node2, flattenTo = graph)
            Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
            Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/about"] "Should have the Property"
            Expect.equal graph.Nodes.Count 2 "Graph should have 2 nodes"
            Expect.isSome (graph.TryGetNode("MyNode2")) "outer node was not found"
            Expect.isSome (graph.TryGetNode("MyInternalNode")) "inner node was not found"
            let ref = Expect.wantSome (node2.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
            Expect.equal ref (LDRef("MyInternalNode")) "property value was not replaced by id reference"
        testCase "SameKeys_DifferentLDNode" <| fun _ ->
            let getNode1() =
                let internalNode = new LDNode("MyInternalNode", thingType())
                let n = new LDNode("MyNode1", thingType())
                n.SetProperty("https://schema.org/about", internalNode)
                n
            let node1 = getNode1()
            let node2 = new LDNode("MyNode2", thingType())
            node2.SetProperty("https://schema.org/about", new LDNode("MyOtherInternalNode", thingType()))
            let graph = node2.Flatten()
            node1.MergeAppendInto_InPlace(node2, flattenTo = graph)
            Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
            Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/about"] "Should have the Property"
            Expect.equal graph.Nodes.Count 3 "Graph should have 3 nodes"
            Expect.isSome (graph.TryGetNode("MyNode2")) "outer node was not found"
            Expect.isSome (graph.TryGetNode("MyInternalNode")) "inner node 1 was not found"
            Expect.isSome (graph.TryGetNode("MyOtherInternalNode")) "inner node 2 was not found"
            let refs = Expect.wantSome (node2.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
            Expect.isTrue (refs :? System.Collections.IEnumerable) "Property value should now be sequence"
            let refs = (refs :?> System.Collections.IEnumerable)
            Expect.genericSequenceEqual refs [LDRef("MyInternalNode");LDRef("MyOtherInternalNode")] "Property value should have been modified"
        testCase "SameKey_NewLDNodeSeq" <| fun _ ->
            let getNode1() =
                let internalNode1 = new LDNode("MyInternalNode1", thingType())
                let internalNode2 = new LDNode("MyInternalNode2", thingType())
                let n = new LDNode("MyNode1", thingType())
                n.SetProperty("https://schema.org/about", [internalNode1;internalNode2])
                n
            let node2 = new LDNode("MyNode2", thingType())
            let internalNode3 = new LDNode("MyInternalNode3", thingType())
            node2.SetProperty("https://schema.org/about", [internalNode3])
            let graph = node2.Flatten()
            let node1 = getNode1()
            node1.MergeAppendInto_InPlace(node2, flattenTo = graph)
            Expect.equal node1 (getNode1()) "Node1 should not have changed modified"
            Expect.sequenceEqual (node2.GetPropertyNames()) ["https://schema.org/about"] "Should have the Property"
            Expect.equal graph.Nodes.Count 4 "Graph should have 4 nodes"
            Expect.isSome (graph.TryGetNode("MyNode2")) "outer node was not found"
            Expect.isSome (graph.TryGetNode("MyInternalNode1")) "inner node 1 was not found"
            Expect.isSome (graph.TryGetNode("MyInternalNode2")) "inner node 2 was not found"
            Expect.isSome (graph.TryGetNode("MyInternalNode3")) "inner node 3 was not found"
            let refs = Expect.wantSome (node2.TryGetProperty("https://schema.org/about")) "outer property should still reference inner node"
            Expect.isTrue (refs :? System.Collections.IEnumerable) "Property value should now be sequence"
            let refs = (refs :?> System.Collections.IEnumerable)
            Expect.genericSequenceEqual refs [LDRef("MyInternalNode3");LDRef("MyInternalNode1");LDRef("MyInternalNode2")] "Property value should have been modified"
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
    tests_TryGetPropertyAsSingleNode
    tests_Compact_InPlace
    tests_Flatten
    tests_getPropertyNames
    tests_mergeAppendInto_InPlace
]