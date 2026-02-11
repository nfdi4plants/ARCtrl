module Tests.VisualizationSiren

open ARCtrl
open ARCtrl.CWL
open ARCtrl.WorkflowGraph
open Siren
open CrossAsync
open TestingUtils
open Tests.WorkflowGraphTestHelpers

let private buildSimpleWorkflowGraph () =
    let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  y:
    type: string
    outputSource: step2/out
steps:
  step1:
    run: ./tool1.cwl
    in:
      in1: x
    out: [out]
  step2:
    run: ./tool2.cwl
    in:
      in1: step1/out
    out: [out]"""
    yaml
    |> Decode.decodeCWLProcessingUnit
    |> Builder.build

let private createGraphWithNodes (rootId: string) (nodes: WorkflowGraphNode list) =
    {
        RootProcessingUnitNodeId = rootId
        Nodes = ResizeArray(nodes)
        Edges = ResizeArray()
        Diagnostics = ResizeArray()
    }

let tests_visualization =
    testList "Visualization.Siren" [
        testCase "sanitizeMermaidId removes unsupported characters deterministically" <| fun () ->
            let input = "port:step:root/MzMLToMzlite/out/dir"
            let expected = "port_step_root__MzMLToMzlite__out__dir"
            let once = WorkflowGraphSiren.sanitizeMermaidId input
            let twice = WorkflowGraphSiren.sanitizeMermaidId input
            Expect.equal once expected ""
            Expect.equal twice expected ""
            Expect.isFalse (once.Contains ":") ""
            Expect.isFalse (once.Contains "/") ""

        testCase "labels with leading slash are quoted in Mermaid output" <| fun () ->
            let toolNode =
                {
                    Id = "unit:tool1"
                    Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool
                    Label = "/tools/my-tool"
                    OwnerNodeId = None
                    Reference = None
                    Metadata = None
                }
            let graph = createGraphWithNodes toolNode.Id [ toolNode ]
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.isFalse (mermaid.Contains("\"#quot;")) "No double escaping expected"
            Expect.stringContains mermaid "unit_tool1[\"/tools/my-tool\"]" ""

        testCase "labels without special chars are not quoted" <| fun () ->
            let processingNode =
                {
                    Id = "unit:step1"
                    Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool
                    Label = "My Step"
                    OwnerNodeId = None
                    Reference = None
                    Metadata = None
                }
            let graph = createGraphWithNodes processingNode.Id [ processingNode ]
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.isFalse (mermaid.Contains("\"My Step\"")) "Plain labels should not be quoted"
            Expect.stringContains mermaid "[My Step]" ""

        testCase "labels containing double quotes are escaped" <| fun () ->
            let processingNode =
                {
                    Id = "unit:node1"
                    Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool
                    Label = "say \"hello\""
                    OwnerNodeId = None
                    Reference = None
                    Metadata = None
                }
            let graph = createGraphWithNodes processingNode.Id [ processingNode ]
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "#quot;" ""
            Expect.stringContains mermaid "[\"say #quot;hello#quot;\"]" ""

        testCase "root input nodes connect to processing units with labeled edges" <| fun () ->
            let parentNode =
                {
                    Id = "unit:parent"
                    Kind = NodeKind.ProcessingUnitNode ProcessingUnitKind.CommandLineTool
                    Label = "/tools/parent-tool"
                    OwnerNodeId = None
                    Reference = None
                    Metadata = None
                }
            let childNode =
                {
                    Id = "port:unit:parent/in/child"
                    Kind = NodeKind.PortNode PortDirection.Input
                    Label = "child-port"
                    OwnerNodeId = Some parentNode.Id
                    Reference = None
                    Metadata = None
                }
            let graph = createGraphWithNodes parentNode.Id [ parentNode; childNode ]
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "[\"/tools/parent-tool\"]" ""
            Expect.stringContains mermaid "port_unit_parent__in__child[child-port]" ""
            Expect.stringContains mermaid "port_unit_parent__in__child-->|child-port|unit_parent" ""

        testCase "initial inputs and final outputs are rendered as groups" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "subgraph wg_initial_inputs[Initial Inputs]" ""
            Expect.stringContains mermaid "subgraph wg_final_outputs[Workflow Outputs]" ""

        testCase "group ordering keeps inputs above processing and outputs below" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let mermaid = WorkflowGraphSiren.toMermaid graph
            let inputGroupIndex = mermaid.IndexOf("subgraph wg_initial_inputs", System.StringComparison.Ordinal)
            let processingIndex = mermaid.IndexOf("unit_root__step1__run", System.StringComparison.Ordinal)
            let outputGroupIndex = mermaid.IndexOf("subgraph wg_final_outputs", System.StringComparison.Ordinal)
            Expect.isTrue (inputGroupIndex >= 0) "Input group missing"
            Expect.isTrue (processingIndex >= 0) "Processing node missing"
            Expect.isTrue (outputGroupIndex >= 0) "Output group missing"
            Expect.isTrue (inputGroupIndex < processingIndex) "Input group should be emitted before processing nodes"
            Expect.isTrue (processingIndex < outputGroupIndex) "Output group should be emitted after processing nodes"

        testCaseCrossAsync "fromGraph workflow fixture contains processing units and labeled dependency edges" (crossAsync {
            let! content = FileSystemHelper.readFileTextAsync workflowFixturePath
            let graph = content |> Decode.decodeCWLProcessingUnit |> Builder.build
            let mermaid = graph |> WorkflowGraphSiren.toMermaid
            Expect.stringContains mermaid "flowchart TD" ""
            Expect.stringContains mermaid "PeptideSpectrumMatching" ""
            Expect.stringContains mermaid "MzMLToMzlite" ""
            Expect.stringContains mermaid "-->|inputDirectory|" ""
            Expect.stringContains mermaid "-->|database|" ""
            Expect.isFalse (mermaid.Contains "contains|") ""
        })

        testCase "fromProcessingUnit mermaid output is deterministic" <| fun () ->
            let processingUnit = Decode.decodeCWLProcessingUnit TestObjects.CWL.Workflow.workflowFile
            let mermaid1 =
                processingUnit
                |> WorkflowGraphSiren.fromProcessingUnit
                |> siren.write
            let mermaid2 =
                processingUnit
                |> WorkflowGraphSiren.fromProcessingUnit
                |> siren.write
            Expect.equal mermaid1 mermaid2 ""
            Expect.isTrue (mermaid1.Length > 0) ""

        testCaseCrossAsync "fromProcessingUnitResolved expands nested run workflow" (crossAsync {
            let! content = FileSystemHelper.readFileTextAsync runFixturePath
            let processingUnit = Decode.decodeCWLProcessingUnit content
            let! resolver = buildRunResolverFromFixtures ()
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "runs/tests/run.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some resolver)
            let mermaid =
                processingUnit
                |> WorkflowGraphSiren.fromProcessingUnitResolved options
                |> siren.write
            Expect.stringContains mermaid "unit_root__Workflow__run[" ""
            Expect.stringContains mermaid "unit_root__Workflow__run__PeptideSpectrumMatching__run[proteomiqon-peptidespectrummatching]" ""
            Expect.stringContains mermaid "unit_root__Workflow__run-->unit_root__Workflow__run__PeptideSpectrumMatching__run" ""
            Expect.isFalse (mermaid.Contains "/tools/proteomiqon-peptidespectrummatching") ""
        })

        testCase "node and style mapping renders processing units plus initial/final boxes" <| fun () ->
            let tool = Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "workflow.cwl")
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some (fun _ -> Some (CommandLineTool tool)))
            let yaml = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  y:
    type: string
    outputSource: s/out
steps:
  s:
    run: ./tool.cwl
    in:
      in1: x
    out: [out]"""
            let graph =
                yaml
                |> Decode.decodeCWLProcessingUnit
                |> Builder.buildWith options
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "unit_root__s__run[dotnet]" ""
            Expect.stringContains mermaid "port_unit_root__in__x[x]" ""
            Expect.stringContains mermaid "port_unit_root__out__y[y]" ""
            Expect.stringContains mermaid "classDef wg_processing" ""
            Expect.stringContains mermaid "classDef wg_initial_input" ""
            Expect.stringContains mermaid "classDef wg_final_output" ""
            Expect.stringContains mermaid "class unit_root__s__run wg_processing;" ""
            Expect.stringContains mermaid "class port_unit_root__in__x wg_initial_input;" ""
            Expect.stringContains mermaid "class port_unit_root__out__y wg_final_output;" ""

        testCase "edge mapping uses labeled input edges and unlabeled final-output edges" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "unit_root__step1__run-->|in1|unit_root__step2__run" ""
            Expect.stringContains mermaid "unit_root__step2__run-->port_unit_root__out__y" ""
            Expect.isFalse (mermaid.Contains "contains|") ""
            Expect.isFalse (mermaid.Contains "-->|calls|") ""

        testCase "toMermaid output has flowchart directive and links" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "flowchart TD" ""
            Expect.stringContains mermaid "-->" ""

        testCase "toMarkdown wraps output in mermaid fenced block" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let markdown = WorkflowGraphSiren.toMarkdown graph
            Expect.isTrue (markdown.StartsWith("```mermaid")) ""
            Expect.stringContains markdown "flowchart TD" ""
            Expect.isTrue (markdown.EndsWith("```")) ""

        testCase "styling enabled emits classDef and class assignments" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let options =
                {
                    WorkflowGraphVisualizationOptions.defaultOptions with
                        EnableStyling = true
                }
            let mermaid = WorkflowGraphSiren.toMermaidWithOptions options graph
            Expect.stringContains mermaid "classDef" ""
            Expect.stringContains mermaid "class " ""
    ]

let main = testList "Visualization.Siren" [ tests_visualization ]
