module Tests.VisualizationSiren

open ARCtrl
open ARCtrl.CWL
open ARCtrl.FileSystem
open ARCtrl.WorkflowGraph
open Siren
open TestingUtils
open CrossAsync

let private workflowFixturePath =
    ArcPathHelper.combineMany [|
        TestObjects.IO.testSimpleARCWithCWL
        "workflows"
        "ProteomIQon"
        "workflow.cwl"
    |]

let private runFixturePath =
    ArcPathHelper.combineMany [|
        TestObjects.IO.testSimpleARCWithCWL
        "runs"
        "tests"
        "run.cwl"
    |]

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

let private buildRunResolverFromFixtures () =
    crossAsync {
        let! relativePaths = FileSystemHelper.getAllFilePathsAsync TestObjects.IO.testSimpleARCWithCWL
        let cwlRelativePaths =
            relativePaths
            |> Array.filter (fun p -> p.EndsWith(".cwl", System.StringComparison.OrdinalIgnoreCase))
        let! resolvedEntries =
            cwlRelativePaths
            |> Array.map (fun relativePath ->
                crossAsync {
                    let absolutePath = ArcPathHelper.combine TestObjects.IO.testSimpleARCWithCWL relativePath
                    let! content = FileSystemHelper.readFileTextAsync absolutePath
                    return ArcPathHelper.normalizePathKey relativePath, Decode.decodeCWLProcessingUnit content
                }
            )
            |> CrossAsync.all
        let map = resolvedEntries |> Map.ofArray
        return fun (path: string) -> map |> Map.tryFind (ArcPathHelper.normalizePathKey path)
    }

let tests_visualization =
    testList "Visualization.Siren" [
        testCase "sanitizeMermaidId removes unsupported characters deterministically" <| fun () ->
            let input = "port:step:root/MzMLToMzlite/out/dir"
            let expected = "port_step_root_MzMLToMzlite_out_dir"
            let once = WorkflowGraphSiren.sanitizeMermaidId input
            let twice = WorkflowGraphSiren.sanitizeMermaidId input
            Expect.equal once expected ""
            Expect.equal twice expected ""
            Expect.isFalse (once.Contains ":") ""
            Expect.isFalse (once.Contains "/") ""

        testCaseCrossAsync "fromGraph workflow fixture includes known step labels and link styles" (crossAsync {
            let! content = FileSystemHelper.readFileTextAsync workflowFixturePath
            let graph = content |> Decode.decodeCWLProcessingUnit |> Builder.build
            let mermaid = graph |> WorkflowGraphSiren.toMermaid
            Expect.stringContains mermaid "flowchart TD" ""
            Expect.stringContains mermaid "PeptideSpectrumMatching" ""
            Expect.stringContains mermaid "MzMLToMzlite" ""
            Expect.stringContains mermaid "==>" ""
            Expect.stringContains mermaid "-->|calls|" ""
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
            Expect.stringContains mermaid "PeptideSpectrumMatching" ""
            Expect.stringContains mermaid "Workflow" ""
        })

        testCase "node shape mapping renders workflow/tool/step/ports distinctly" <| fun () ->
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
            Expect.stringContains mermaid "[[" ""
            Expect.stringContains mermaid "{{" ""
            Expect.stringContains mermaid "(" ""
            Expect.stringContains mermaid "([" ""

        testCase "edge mapping renders dataflow thick and hides contains by default" <| fun () ->
            let graph = buildSimpleWorkflowGraph ()
            let mermaid = WorkflowGraphSiren.toMermaid graph
            Expect.stringContains mermaid "==>" ""
            Expect.stringContains mermaid "-->|calls|" ""
            Expect.isFalse (mermaid.Contains "contains|") ""

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
