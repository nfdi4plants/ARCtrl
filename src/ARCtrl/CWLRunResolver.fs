module ARCtrl.CWLRunResolver

open ARCtrl
open ARCtrl.FileSystem

/// Normalizes a file path for use as a lookup key.
let pathKey (path: string) : string =
    ArcPathHelper.normalizePathKey path

type RunResolutionState = {
    mutable Cache: Map<string, CWL.CWLProcessingUnit>
}

/// The path of a CWL file without the fragment of a reference to one of its processes, as in runs/r/run.cwl#shout.
let private documentPathOf (filePath: string) : string =
    let hash = filePath.IndexOf '#'
    if hash >= 0 then filePath.Substring(0, hash) else filePath

/// The lookup path of a process of a packed document: the path of the file and the id of the process as its fragment,
/// as in runs/r/run.cwl#shout. The id may start with a #.
let packedProcessPath (documentPath: string) (id: string) : string =
    let name = if id.StartsWith "#" then id.Substring 1 else id
    documentPathOf documentPath + "#" + name

/// The entries that a CWL file adds to the lookup of run references. A document with one process adds it under the path
/// of the file. A packed document adds its process main under the path of the file, and each of its processes under the
/// path with its id as the fragment. A step with run: "#shout" refers to the document that holds it, so the path in
/// the lookup keeps two documents with a process of the same id apart.
let lookupEntriesOfDocument (path: string) (document: CWL.CWLDocument) : (string * CWL.CWLProcessingUnit) list =
    match document with
    | CWL.CWLDocument.Process processingUnit ->
        [ pathKey path, processingUnit ]
    | CWL.CWLDocument.Graph graph ->
        [
            match CWL.CWLGraph.tryFindProcess "main" graph with
            | Some main -> yield pathKey path, main
            | None -> ()
            for processingUnit in graph.Processes do
                match CWL.CWLProcessingUnit.tryGetId processingUnit with
                | Some id -> yield pathKey (packedProcessPath path id), processingUnit
                | None -> ()
        ]

/// Generates candidate lookup paths for a run reference relative to a workflow file. A reference is a path that ends in
/// .cwl, optionally followed by a fragment as in tools.cwl#shout, or a bare fragment #id. A bare fragment refers to a
/// process of the packed document that holds the workflow, and the lookup holds that process under the path of the
/// document and the fragment. A path with a fragment refers to a process of another packed document, so the fragment
/// stays behind each candidate path.
let getRunNodeCandidates (workflowFilePath: string) (runPath: string) : string [] =
    if System.String.IsNullOrWhiteSpace runPath then
        [||]
    else
        let trimmedRunPath = runPath.Trim()
        if trimmedRunPath.StartsWith "#" && trimmedRunPath.Length > 1 then
            [|
                documentPathOf workflowFilePath + trimmedRunPath
                trimmedRunPath
            |]
            |> Array.filter (fun p -> not (System.String.IsNullOrWhiteSpace p))
            |> Array.distinct
        else
            let documentPath = documentPathOf trimmedRunPath
            let fragment = trimmedRunPath.Substring documentPath.Length
            if not (documentPath.EndsWith(".cwl", System.StringComparison.OrdinalIgnoreCase)) then
                [||]
            else
                let normalizedRunPath = ArcPathHelper.normalize documentPath
                let runPathInWorkflowDirectory = ArcPathHelper.resolvePathFromFile workflowFilePath documentPath
                let normalizedRunPathInWorkflowDirectory = ArcPathHelper.normalize runPathInWorkflowDirectory

                [|
                    documentPath
                    normalizedRunPath
                    runPathInWorkflowDirectory
                    normalizedRunPathInWorkflowDirectory
                |]
                |> Array.filter (fun p -> not (System.String.IsNullOrWhiteSpace p))
                |> Array.distinct
                |> Array.map (fun path -> path + fragment)

let cacheResolvedProcessingUnit (state: RunResolutionState) (candidateKeys: string []) (processingUnit: CWL.CWLProcessingUnit) =
    for key in candidateKeys do
        state.Cache <- state.Cache.Add(key, processingUnit)

let runFromProcessingUnit (processingUnit: CWL.CWLProcessingUnit) =
    match processingUnit with
    | CWL.CommandLineTool tool -> CWL.WorkflowStepRunOps.fromTool tool
    | CWL.Workflow workflow -> CWL.WorkflowStepRunOps.fromWorkflow workflow
    | CWL.ExpressionTool expressionTool -> CWL.WorkflowStepRunOps.fromExpressionTool expressionTool
    | CWL.Operation operation -> CWL.WorkflowStepRunOps.fromOperation operation

/// Iterates all steps in a workflow and resolves each step's run field recursively.
let rec resolveWorkflowRunsRecursiveWithResolver
    (workflowFilePath: string)
    (activePathKeys: Set<string>)
    (state: RunResolutionState)
    (workflow: CWL.CWLWorkflowDescription)
    (tryResolveRunPath: string -> CWL.CWLProcessingUnit option)
    : unit =
    for step in workflow.Steps do
        let resolvedRun =
            resolveWorkflowStepRunRecursiveWithResolver workflowFilePath activePathKeys state step.Run tryResolveRunPath
        step.Run <- resolvedRun

/// Resolves a single WorkflowStepRun, trying each candidate path against the resolver for RunString values.
and resolveWorkflowStepRunRecursiveWithResolver
    (workflowFilePath: string)
    (activePathKeys: Set<string>)
    (state: RunResolutionState)
    (run: CWL.WorkflowStepRun)
    (tryResolveRunPath: string -> CWL.CWLProcessingUnit option)
    : CWL.WorkflowStepRun =
    match run with
    | CWL.RunString runPath ->
        let candidates = getRunNodeCandidates workflowFilePath runPath
        let candidateKeys = candidates |> Array.map pathKey |> Array.distinct
        let rec tryResolveCandidate index =
            if index >= candidates.Length then
                CWL.RunString runPath
            else
                let candidate = candidates.[index]
                let key = pathKey candidate
                if activePathKeys.Contains key then
                    tryResolveCandidate (index + 1)
                else
                    match state.Cache.TryFind key with
                    | Some cached ->
                        runFromProcessingUnit cached
                    | None ->
                        match tryResolveRunPath candidate with
                        | Some (CWL.Workflow workflow) ->
                            resolveWorkflowRunsRecursiveWithResolver candidate (activePathKeys.Add key) state workflow tryResolveRunPath
                            let resolved = CWL.Workflow workflow
                            cacheResolvedProcessingUnit state candidateKeys resolved
                            runFromProcessingUnit resolved
                        | Some processingUnit ->
                            cacheResolvedProcessingUnit state candidateKeys processingUnit
                            runFromProcessingUnit processingUnit
                        | None ->
                            tryResolveCandidate (index + 1)
        tryResolveCandidate 0
    | CWL.RunWorkflow _ ->
        match CWL.WorkflowStepRunOps.tryGetWorkflow run with
        | Some workflow ->
            resolveWorkflowRunsRecursiveWithResolver workflowFilePath activePathKeys state workflow tryResolveRunPath
            CWL.WorkflowStepRunOps.fromWorkflow workflow
        | None ->
            run
    | CWL.RunCommandLineTool _ ->
        run
    | CWL.RunExpressionTool _ ->
        run
    | CWL.RunOperation _ ->
        run

/// <summary>
/// Resolves all CWL WorkflowStep run references for a processing unit by looking up
/// run path strings in the provided resolver function. For Workflow processing units,
/// this recursively resolves all nested step run references in-place.
/// CommandLineTool and ExpressionTool processing units are returned unchanged.
/// </summary>
/// <param name="workflowFilePath">The file path of the workflow CWL file, used for resolving relative paths.</param>
/// <param name="processingUnit">The CWL processing unit whose run references to resolve.</param>
/// <param name="tryResolveRunPath">A function that maps a path string to an optional CWLProcessingUnit.</param>
let resolveRunReferencesFromLookup (workflowFilePath: string) (processingUnit: CWL.CWLProcessingUnit) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.CWLProcessingUnit =
    match processingUnit with
    | CWL.Workflow workflow ->
        let state = { Cache = Map.empty }
        // The workflow is active from the start. A run that leads back to its file, or to its process main, stays a
        // reference, so no resolution puts a process inside itself.
        let activePathKeys = Set.ofList [ pathKey workflowFilePath; pathKey (packedProcessPath workflowFilePath "main") ]
        resolveWorkflowRunsRecursiveWithResolver workflowFilePath activePathKeys state workflow tryResolveRunPath
        CWL.Workflow workflow
    | CWL.CommandLineTool _ ->
        processingUnit
    | CWL.ExpressionTool _ ->
        processingUnit
    | CWL.Operation _ ->
        processingUnit

/// <summary>
/// Resolves the run references of the CWL file of a run or a workflow. A file with one process resolves like
/// resolveRunReferencesFromLookup and has no graph. A packed document starts with its process main. The run references
/// of the form #id resolve to the processes of this document, which the resolver takes from the graph itself, so the
/// steps of main and of the other processes hold the objects of the graph. The graph writes a step that holds one of
/// its processes as a reference again, and the inlined runs do not end up in the written file. Every other reference
/// resolves through the lookup.
/// </summary>
/// <param name="filePath">The file path of the CWL file, used for resolving relative paths.</param>
/// <param name="document">The decoded CWL file.</param>
/// <param name="tryResolveRunPath">A function that maps a path string to an optional CWLProcessingUnit.</param>
/// <returns>The process that the file starts with, and the graph of a packed document.</returns>
let resolveRunReferencesOfDocument (filePath: string) (document: CWL.CWLDocument) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.CWLProcessingUnit * CWL.CWLGraph option =
    match document with
    | CWL.CWLDocument.Process processingUnit ->
        resolveRunReferencesFromLookup filePath processingUnit tryResolveRunPath, None
    | CWL.CWLDocument.Graph graph ->
        match CWL.CWLGraph.tryFindProcess "main" graph with
        | None ->
            failwith $"The packed CWL document {filePath} has no process with the id main."
        | Some main ->
            let processesOfDocument = lookupEntriesOfDocument filePath document |> Map.ofList
            let tryResolveInDocument (path: string) =
                match processesOfDocument.TryFind (pathKey path) with
                | Some processingUnit -> Some processingUnit
                | None -> tryResolveRunPath path
            resolveRunReferencesFromLookup filePath main tryResolveInDocument, Some graph

/// <summary>
/// Resolves a single workflow step run value by looking up the run path string
/// in the provided resolver function. Returns the resolved run value.
/// </summary>
/// <param name="workflowFilePath">The file path of the parent workflow CWL file, used for resolving relative paths.</param>
/// <param name="run">The WorkflowStepRun value to resolve.</param>
/// <param name="tryResolveRunPath">A function that maps a path string to an optional CWLProcessingUnit.</param>
let resolveWorkflowStepRunFromLookup (workflowFilePath: string) (run: CWL.WorkflowStepRun) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.WorkflowStepRun =
    let state = { Cache = Map.empty }
    resolveWorkflowStepRunRecursiveWithResolver workflowFilePath Set.empty state run tryResolveRunPath
