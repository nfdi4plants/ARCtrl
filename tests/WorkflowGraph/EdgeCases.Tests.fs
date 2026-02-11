module Tests.EdgeCases

open ARCtrl.CWL
open ARCtrl.WorkflowGraph
open DynamicObj
open TestingUtils

let private buildGraphFromYaml (yaml: string) =
    yaml
    |> Decode.decodeCWLProcessingUnit
    |> Builder.build

let tests_edgeCases =
    testList "EdgeCases" [
        testCase "malformed source // creates diagnostic" <| fun () ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: //
    out: [out]"""
            let graph = buildGraphFromYaml yaml
            Expect.isTrue (graph.Diagnostics.Count >= 1) ""

        testCase "reference to non-existent step output creates diagnostic and no dataflow edge" <| fun () ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: NonExistentStep/out
    out: [out]"""
            let graph = buildGraphFromYaml yaml
            let missingRefDiagnostics =
                graph.Diagnostics
                |> Seq.filter (fun d -> d.Kind = GraphIssueKind.MissingReference)
                |> Seq.length
            Expect.isTrue (missingRefDiagnostics >= 1) ""
            let dataFlowEdges =
                graph.Edges
                |> Seq.filter (fun e -> e.Kind = EdgeKind.DataFlow)
                |> Seq.length
            Expect.equal dataFlowEdges 0 ""

        testCase "workflow output with invalid source creates diagnostic" <| fun () ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: BadStep/badPort
steps:
  step1:
    run: ./tool.cwl
    in: {}
    out: [out]"""
            let graph = buildGraphFromYaml yaml
            let missingRefDiagnostics =
                graph.Diagnostics
                |> Seq.filter (fun d -> d.Kind = GraphIssueKind.MissingReference)
                |> Seq.length
            Expect.isTrue (missingRefDiagnostics >= 1) ""

        testCase "graph IDs are deterministic between builds" <| fun () ->
            let yaml = TestObjects.CWL.Workflow.workflowWithExtendedStepFile
            let graph1 = buildGraphFromYaml yaml
            let graph2 = buildGraphFromYaml yaml

            let nodeIds1 = graph1.Nodes |> Seq.map (fun n -> n.Id) |> Seq.sort |> Seq.toArray
            let nodeIds2 = graph2.Nodes |> Seq.map (fun n -> n.Id) |> Seq.sort |> Seq.toArray
            let edgeIds1 = graph1.Edges |> Seq.map (fun e -> e.Id) |> Seq.sort |> Seq.toArray
            let edgeIds2 = graph2.Edges |> Seq.map (fun e -> e.Id) |> Seq.sort |> Seq.toArray

            Expect.sequenceEqual nodeIds1 nodeIds2 ""
            Expect.sequenceEqual edgeIds1 edgeIds2 ""

        testCase "building same processing unit instance twice is deterministic" <| fun () ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in: {}
    out: [out]"""
            let processingUnit = Decode.decodeCWLProcessingUnit yaml
            let resolvedTool = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun path ->
                    if path.EndsWith("tool.cwl") then Some (CommandLineTool resolvedTool) else None
                ))

            let graph1 = Builder.buildWith options processingUnit
            let graph2 = Builder.buildWith options processingUnit

            let nodeIds1 = graph1.Nodes |> Seq.map (fun n -> n.Id) |> Seq.sort |> Seq.toArray
            let nodeIds2 = graph2.Nodes |> Seq.map (fun n -> n.Id) |> Seq.sort |> Seq.toArray
            let edgeIds1 = graph1.Edges |> Seq.map (fun e -> e.Id) |> Seq.sort |> Seq.toArray
            let edgeIds2 = graph2.Edges |> Seq.map (fun e -> e.Id) |> Seq.sort |> Seq.toArray

            Expect.sequenceEqual nodeIds1 nodeIds2 ""
            Expect.sequenceEqual edgeIds1 edgeIds2 ""

        testCase "buildWith does not mutate workflow step run payloads" <| fun () ->
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in: {}
    out: [out]"""
            let processingUnit = Decode.decodeCWLProcessingUnit yaml
            let resolvedTool = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun path ->
                    if path.EndsWith("tool.cwl") then Some (CommandLineTool resolvedTool) else None
                ))

            let assertRunString () =
                match processingUnit with
                | Workflow wf ->
                    match wf.Steps.[0].Run with
                    | RunString runPath -> Expect.equal runPath "./tool.cwl" ""
                    | other -> Expect.isTrue false $"Expected RunString but got %A{other}"
                | _ ->
                    Expect.isTrue false "Expected workflow processing unit"

            assertRunString ()
            let _graph = Builder.buildWith options processingUnit
            assertRunString ()

        testCase "scatter fields do not break graph build and are preserved as metadata" <| fun () ->
            let graph = buildGraphFromYaml TestObjects.CWL.Workflow.workflowWithScatterAndScatterMethodFile
            let stepNodeOpt =
                graph.Nodes
                |> Seq.tryFind (fun n -> n.Kind = NodeKind.StepNode && n.Label = "step1")
            let stepNode = Expect.wantSome stepNodeOpt "Expected step1 node"
            let metadata = Expect.wantSome stepNode.Metadata "Expected step metadata"
            let scatterMethod = DynObj.tryGetTypedPropertyValue<string> "scatterMethod" metadata
            let scatter = DynObj.tryGetTypedPropertyValue<string []> "scatter" metadata
            Expect.equal scatterMethod (Some "flat_crossproduct") ""
            Expect.equal scatter (Some [|"in1"; "in2"|]) ""
    ]

let main = testList "EdgeCases" [ tests_edgeCases ]
