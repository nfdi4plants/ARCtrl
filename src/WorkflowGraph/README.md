# ARCtrl.WorkflowGraph

`ARCtrl.WorkflowGraph` converts CWL processing units and ARC domain objects into a deterministic, queryable graph model.
The graph is intended for analysis and visualization (Mermaid/Siren), while preserving enough metadata and diagnostics to explain how edges were inferred.

## General idea

The builder normalizes CWL structure into a small graph vocabulary:

- **Processing unit nodes** (`Workflow`, `CommandLineTool`, `ExpressionTool`, unresolved/external references)
- **Step nodes** (workflow internal execution steps)
- **Port nodes** (typed as input/output)
- **Edges** for containment, calls, dataflow, and workflow input/output bindings

This gives one stable representation for:

- simple tools,
- single workflows,
- nested workflows,
- workflows/runs with resolvable or unresolved `run` references.

## Build pipeline (high-level)

1. Parse CWL into `CWLProcessingUnit`.
2. Build graph nodes/edges using `Builder.build` or `Builder.buildWith`.
3. Optionally resolve `run` paths (relative/normalized) via `TryResolveRunPath`.
4. Collect diagnostics (`MissingReference`, `ResolutionFailed`, `CycleDetected`, ...).
5. Query via `WorkflowGraphQueries` or render via `WorkflowGraphSiren`.

## Core types

The central model is defined in `GraphTypes.fs`:

- `WorkflowGraph`
    - Root node id
    - `Nodes`, `Edges`
    - `Diagnostics` (`GraphBuildIssue`)
- `WorkflowGraphNode`
    - `Kind` (`ProcessingUnitNode`, `StepNode`, `PortNode`)
    - `OwnerNodeId`, `Reference`, `Metadata`
- `WorkflowGraphEdge`
    - `Kind` (`Contains`, `Calls`, `DataFlow`, `BindsWorkflowInput`, `BindsWorkflowOutput`)
    - optional edge label
- `GraphBuildIssue`
    - structured diagnostics with kind/message/scope/reference

Important helper modules:

- `GraphId`: deterministic id generation and scope/path normalization
- `ReferenceParsing`: parses `step/port` references and step output ids
- `WorkflowGraphQueries`: read-only graph inspection utilities

## ARC Adapters

`Adapters` bridge ARC domain objects and the graph builder so callers do not need to wire resolver logic manually.

- `Adapters.ofWorkflow : ArcWorkflow -> Result<WorkflowGraph, GraphBuildIssue>`
- `Adapters.ofRun : ArcRun -> Result<WorkflowGraph, GraphBuildIssue>`
- `Adapters.ofInvestigation : ArcInvestigation -> WorkflowGraphIndex`

Internally, adapters:

1. Extract CWL descriptions from workflows/runs in the surrounding investigation.
2. Build a normalized path lookup (`createCwlLookupFromInvestigation`).
3. Convert the lookup to a resolver function (`createResolver`).
4. Configure `WorkflowGraphBuildOptions` (`RootScope`, `RootWorkflowFilePath`, resolver).
5. Return structured `MissingCwlDescription` errors when required CWL is absent.

Use adapters when your entry point is ARC objects; use `Builder` directly when your entry point is already a CWL processing unit.

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

## ARC adapters usage

```fsharp
let workflowResult = Adapters.ofWorkflow workflow
let runResult = Adapters.ofRun run
let index = Adapters.ofInvestigation investigation
```

## Output formats

```fsharp
let mermaidText = WorkflowGraphSiren.toMermaid graph
let markdown = WorkflowGraphSiren.toMarkdown graph
```

The visualization layer supports direct Mermaid output and Markdown-wrapped Mermaid blocks.
