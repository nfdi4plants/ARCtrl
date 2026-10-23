module Tests.LDGraph

open TestingUtils
open TestObjects.Json.ROCrate
open ARCtrl
open ARCtrl.ROCrate
open ARCtrl.Json
open DynamicObj

let private test_read = testList "Read" [
    testCase "Minimal_ROCrate" <| fun _ ->
        let graph = LDGraph.fromROCrateJsonString roCrate_minimal
        // Expect.sequenceEqual graph.Properties.Keys ["@context"] "only context property should exist"
        Expect.isSome (graph.TryGetContext()) "context should exist"
        Expect.isEmpty (graph.GetDynamicPropertyNames()) "only context property should exist"
        Expect.hasLength graph.Nodes 2 "should have 2 nodes"
        let firstExpectedObject = LDNode("ro-crate-metadata.json", ResizeArray ["CreativeWork"])
        firstExpectedObject.SetProperty("about", LDRef("./"))
        firstExpectedObject.SetProperty("conformsTo", LDRef("https://w3id.org/ro/crate/1.2"))
        let secondExpectedObject = LDNode("./", ResizeArray ["Dataset"])
        Expect.equal graph.Nodes.[0] firstExpectedObject "first node should be the metadata"
        Expect.equal graph.Nodes.[1] secondExpectedObject "second node should be the dataset"
    testCase "LargeNodeCount (issue #545)" <| fun _ ->
        let graph = LDGraph()
        for i in 1 .. 1000 do
            let node = LDNode($"node{i}", ResizeArray ["Thing"])
            graph.AddNode node |> ignore
        Expect.hasLength graph.Nodes 1000 "should have 1000 nodes"
        let ro = graph.ToROCrateJsonString()
        let graph2 = LDGraph.fromROCrateJsonString ro
        Expect.hasLength graph2.Nodes 1000 "should have 1000 nodes"
    // https://github.com/nfdi4plants/ARCtrl/issues/607
    testCase "NormalizeDataEntityIds" <| fun _ ->
        let graph = LDGraph.fromROCrateJsonString roCrate_unnormalizedDataEntityIds
        let context = graph.TryGetContext()
        Expect.hasLength graph.Nodes 5 "Both versions of the file should be merged into one node"
        Expect.isNone (graph.TryGetNode "studies/MyStudy/resources/data.txt") "File without leading ./ should not exist"
        let file = Expect.wantSome (graph.TryGetNode "./studies/MyStudy/resources/data.txt") "File with leading ./ should exist"
        Expect.equal (LDFile.getNameAsString(file, ?context = context)) "./studies/MyStudy/resources/data.txt" "File name should be normalized"
        let study = Expect.wantSome (graph.TryGetNode "./studies/MyStudy/") "Study dataset with leading ./ should exist"
        Expect.equal (LDDataset.getNameAsString(study, ?context = context)) "My Study" "Study name should not be changed"
        let studyParts = study.GetPropertyValues(LDDataset.hasPart, ?context = context) |> Seq.map (fun r -> (r :?> LDRef).Id) |> List.ofSeq
        Expect.equal studyParts ["./studies/MyStudy/resources/data.txt"] "Study should reference the normalized file once"
        let root = Expect.wantSome (graph.TryGetNode "./") "Root dataset should exist"
        let rootParts = root.GetPropertyValues(LDDataset.hasPart, ?context = context) |> Seq.map (fun r -> (r :?> LDRef).Id) |> List.ofSeq
        Expect.equal rootParts ["./studies/MyStudy/"; "https://example.org/data.txt"] "Root should reference the normalized study and the unchanged url"
        Expect.isSome (graph.TryGetNode "https://example.org/data.txt") "Url should not be changed"
]

let private test_write = testList "Write" [
    testCase "Minimal_ROCrate" <| fun _ ->
        let graph = LDGraph.fromROCrateJsonString roCrate_minimal
        let json = graph.ToROCrateJsonString()
        Expect.stringEqual json roCrate_minimal "should be equal"
]

let main = testList "LDGraph" [
    test_read
    test_write
]