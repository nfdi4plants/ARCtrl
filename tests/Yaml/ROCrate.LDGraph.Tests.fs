module Tests.ROCrate.LDGraph

open TestingUtils
open ARCtrl.ROCrate
open ARCtrl.Yaml.ROCrate
open DynamicObj

module YamlFixtures = TestObjects.Yaml.ROCrate

let private readTests = testList "Read" [
    testCase "Minimal_ROCrate" <| fun _ ->
        let graph = LDGraph.fromROCrateYamlString YamlFixtures.roCrate_minimal
        Expect.isSome (graph.TryGetContext()) "context should exist"
        Expect.hasLength graph.Nodes 2 "should have 2 nodes"

        let firstExpectedNode = LDNode("ro-crate-metadata.json", ResizeArray ["CreativeWork"])
        firstExpectedNode.SetProperty("about", LDRef("./"))
        firstExpectedNode.SetProperty("conformsTo", LDRef("https://w3id.org/ro/crate/1.2"))
        let secondExpectedNode = LDNode("./", ResizeArray ["Dataset"])

        Expect.equal graph.Nodes.[0] firstExpectedNode "first node should be metadata"
        Expect.equal graph.Nodes.[1] secondExpectedNode "second node should be dataset"

    testCase "TopLevelProperties_AreParsed" <| fun _ ->
        let graph = LDGraph.fromROCrateYamlString YamlFixtures.roCrate_withTopLevelProperties
        Expect.equal graph.Id (Some "my-graph") "graph id should be parsed"
        let name = Expect.wantSome (DynObj.tryGetTypedPropertyValue<string> "name" graph) "custom string property should exist"
        let version = Expect.wantSome (DynObj.tryGetTypedPropertyValue<int> "version" graph) "custom int property should exist"
        Expect.equal name "Example graph" "custom string property should be parsed"
        Expect.equal version 2 "custom int property should be parsed"
        Expect.hasLength graph.Nodes 1 "graph should have one node"

    testCase "Reject_Missing_Graph" <| fun _ ->
        Expect.throws (fun () -> LDGraph.fromROCrateYamlString YamlFixtures.roCrate_missingGraph |> ignore) "@graph is required"

    testCase "Reject_Graph_Not_Sequence" <| fun _ ->
        Expect.throws (fun () -> LDGraph.fromROCrateYamlString YamlFixtures.roCrate_invalidGraphNotSequence |> ignore) "@graph must be a sequence"

    testCase "Allow_SingleDocument_With_Leading_Separator" <| fun _ ->
        let yaml = "---\n" + YamlFixtures.roCrate_minimal
        let graph = LDGraph.fromROCrateYamlString yaml
        Expect.hasLength graph.Nodes 2 "single document with leading separator should be accepted"

    testCase "Reject_MultiDocument_Stream" <| fun _ ->
        let yaml =
            YamlFixtures.roCrate_minimal + "\n---\n'@context': https://w3id.org/ro/crate/1.2/context\n'@graph': []\n"
        Expect.throws (fun () -> LDGraph.fromROCrateYamlString yaml |> ignore) "YAML-LD parser should reject multi-document streams"

    testCase "LargeNodeCount" <| fun _ ->
        let graph = LDGraph()
        for i in 1 .. 1000 do
            graph.AddNode(LDNode($"node{i}", ResizeArray ["Thing"])) |> ignore
        let yaml = LDGraph.toROCrateYamlString (Some 2) graph
        let graph2 = LDGraph.fromROCrateYamlString yaml
        Expect.hasLength graph2.Nodes 1000 "should have 1000 nodes after roundtrip"
]

let private writeTests = testList "Write" [
    testCase "Roundtrip" <| fun _ ->
        let graph = LDGraph.fromROCrateYamlString YamlFixtures.roCrate_minimal
        let yaml = LDGraph.toROCrateYamlString (Some 2) graph
        let parsed = LDGraph.fromROCrateYamlString yaml
        Expect.equal parsed graph "graph roundtrip should preserve content"
]

let main = testList "ROCrate.LDGraph" [
    readTests
    writeTests
]
