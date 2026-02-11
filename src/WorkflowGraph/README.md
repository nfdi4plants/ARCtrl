# ARCtrl.WorkflowGraph

`ARCtrl.WorkflowGraph` builds deterministic workflow/run graphs from CWL processing units.

## Basic usage

```fsharp
open ARCtrl.CWL
open ARCtrl.WorkflowGraph

let processingUnit = Decode.decodeCWLProcessingUnit cwlText
let graph = WorkflowGraphApi.build processingUnit
let mermaid = WorkflowGraphApi.toMermaid graph
```

## Build with run resolution

```fsharp
let lookup : string -> CWLProcessingUnit option = fun path -> cwlMap |> Map.tryFind path

let options =
    WorkflowGraphBuildOptions.defaultOptions
    |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some "runs/tests/run.cwl")
    |> WorkflowGraphBuildOptions.withTryResolveRunPath (Some lookup)

let graph = WorkflowGraphApi.buildWith options processingUnit
```

## ARC adapters

```fsharp
let workflowResult = Adapters.ofWorkflow workflow
let runResult = Adapters.ofRun run
let index = Adapters.ofInvestigation investigation
```

## Output formats

```fsharp
// Native outputs
let mermaidText = WorkflowGraphSiren.toMermaid graph
let markdown = WorkflowGraphSiren.toMarkdown graph

// Optional image conversion via external renderer callbacks
// (for example Mermaid CLI or a rendering web service)
let svgOpt = WorkflowGraphSiren.toSvgWith renderMermaidToSvg graph
let pngBytesOpt = WorkflowGraphSiren.toPngBytesWith renderMermaidToPng graph
```

Only Mermaid text and Markdown are produced directly by this library.
SVG/PNG are callback-based extension points and require you to provide the renderer.

For .NET targets, save helpers are available:

- `WorkflowGraphSiren.saveMermaid`
- `WorkflowGraphSiren.saveMarkdown`
- `WorkflowGraphSiren.saveSvgWith`
- `WorkflowGraphSiren.savePngWith`
