module Tests.ROCrate.LDGraph

open TestingUtils
open ARCtrl.ROCrate
open ARCtrl.Yaml.ROCrate

let private roCrateMinimalYaml =
    """
'@context': https://w3id.org/ro/crate/1.2/context
'@graph':
  - '@id': ro-crate-metadata.json
    '@type': CreativeWork
    about:
      '@id': ./
    conformsTo:
      '@id': https://w3id.org/ro/crate/1.2
  - '@id': ./
    '@type': Dataset
"""

let private readTests = testList "Read" [
    testCase "Minimal_ROCrate" <| fun _ ->
        let graph = LDGraph.fromROCrateYamlString roCrateMinimalYaml
        Expect.isSome (graph.TryGetContext()) "context should exist"
        Expect.hasLength graph.Nodes 2 "should have 2 nodes"

        let firstExpectedNode = LDNode("ro-crate-metadata.json", ResizeArray ["CreativeWork"])
        firstExpectedNode.SetProperty("about", LDRef("./"))
        firstExpectedNode.SetProperty("conformsTo", LDRef("https://w3id.org/ro/crate/1.2"))
        let secondExpectedNode = LDNode("./", ResizeArray ["Dataset"])

        Expect.equal graph.Nodes.[0] firstExpectedNode "first node should be metadata"
        Expect.equal graph.Nodes.[1] secondExpectedNode "second node should be dataset"
]

let private writeTests = testList "Write" [
    testCase "Roundtrip" <| fun _ ->
        let graph = LDGraph.fromROCrateYamlString roCrateMinimalYaml
        let yaml = LDGraph.toROCrateYamlString (Some 2) graph
        let parsed = LDGraph.fromROCrateYamlString yaml
        Expect.equal parsed graph "graph roundtrip should preserve content"
]

let main = testList "ROCrate.LDGraph" [
    readTests
    writeTests
]
