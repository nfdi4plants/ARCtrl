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
            match CWL.CWLGraph.tryFindMain graph with
            | Some main -> yield pathKey path, main
            | None -> ()
            // A run of the form #id reaches the last process with the id, so a process that a later one replaces has no entry.
            for id in graph.Processes |> Seq.choose CWL.CWLProcessingUnit.tryGetId |> Seq.distinct do
                match CWL.CWLGraph.tryFindProcess id graph with
                | Some processingUnit -> yield pathKey (packedProcessPath path id), processingUnit
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

/// Keeps the process that a reference resolved to, so that every later reference to the same file finds the same object.
let cacheResolvedProcessingUnit (state: RunResolutionState) (key: string) (processingUnit: CWL.CWLProcessingUnit) =
    state.Cache <- state.Cache.Add(key, processingUnit)

/// The run of a step that stands for the process. A process whose class is a directive has none, because the file that
/// decides its class is not read, so a reference to it stays unresolved.
let tryRunFromProcessingUnit (processingUnit: CWL.CWLProcessingUnit) : CWL.WorkflowStepRun option =
    match processingUnit with
    | CWL.CommandLineTool tool -> Some (CWL.WorkflowStepRunOps.fromTool tool)
    | CWL.Workflow workflow -> Some (CWL.WorkflowStepRunOps.fromWorkflow workflow)
    | CWL.ExpressionTool expressionTool -> Some (CWL.WorkflowStepRunOps.fromExpressionTool expressionTool)
    | CWL.Operation operation -> Some (CWL.WorkflowStepRunOps.fromOperation operation)
    | CWL.RawProcess _ -> None

/// Resolves the run reference of each step of a workflow and of each workflow nested inline in it. A step keeps its
/// run as it is written and receives the process in its ResolvedRun. A step that holds a process already keeps it,
/// because the lookup hands out one object for each file and the first resolution of that object decided its steps.
let rec resolveWorkflowRunsRecursiveWithResolver
    (workflowFilePath: string)
    (activePathKeys: Set<string>)
    (state: RunResolutionState)
    (workflow: CWL.CWLWorkflowDescription)
    (tryResolveRunPath: string -> CWL.CWLProcessingUnit option)
    : unit =
    // An imported step stands for steps of a file that the decoder does not read, so it has no run to resolve.
    for step in CWL.CWLWorkflowDescription.getStepValues workflow do
        match step.Run with
        | CWL.RunString runPath
        | CWL.RunImport runPath when step.ResolvedRun.Run.IsNone ->
            step.ResolvedRun <-
                { Run = resolveRunReferenceWithResolver workflowFilePath activePathKeys state runPath tryResolveRunPath }
        | CWL.RunWorkflow _ ->
            match CWL.WorkflowStepRunOps.tryGetWorkflow step.Run with
            | Some inlineWorkflow ->
                resolveWorkflowRunsRecursiveWithResolver workflowFilePath activePathKeys state inlineWorkflow tryResolveRunPath
            | None -> ()
        | _ -> ()

/// Tries each candidate path of a run reference against the resolver and returns the run of the first process that it
/// finds. A candidate that leads back to a file that is being resolved is skipped, so no process ends up inside itself.
/// The cache holds a process under the path it was found at, because the same relative path in the directory of another
/// workflow is another file.
and resolveRunReferenceWithResolver
    (workflowFilePath: string)
    (activePathKeys: Set<string>)
    (state: RunResolutionState)
    (runPath: string)
    (tryResolveRunPath: string -> CWL.CWLProcessingUnit option)
    : CWL.WorkflowStepRun option =
    let candidates = getRunNodeCandidates workflowFilePath runPath
    let rec tryResolveCandidate index =
        if index >= candidates.Length then
            None
        else
            let candidate = candidates.[index]
            let key = pathKey candidate
            if activePathKeys.Contains key then
                tryResolveCandidate (index + 1)
            else
                match state.Cache.TryFind key with
                | Some cached ->
                    tryRunFromProcessingUnit cached
                | None ->
                    match tryResolveRunPath candidate |> Option.bind (fun found -> tryRunFromProcessingUnit found |> Option.map (fun run -> found, run)) with
                    | Some (processingUnit, run) ->
                        match processingUnit with
                        | CWL.Workflow workflow ->
                            resolveWorkflowRunsRecursiveWithResolver candidate (activePathKeys.Add key) state workflow tryResolveRunPath
                        | _ -> ()
                        cacheResolvedProcessingUnit state key processingUnit
                        Some run
                    | None ->
                        tryResolveCandidate (index + 1)
    tryResolveCandidate 0

/// <summary>
/// Resolves all CWL WorkflowStep run references for a processing unit by looking up
/// run path strings in the provided resolver function. For Workflow processing units,
/// this recursively resolves all nested step run references in-place: each step keeps its run
/// reference and holds the process in its ResolvedRun.
/// CommandLineTool and ExpressionTool processing units are returned unchanged.
/// </summary>
/// <param name="workflowFilePath">The file path of the workflow CWL file, used for resolving relative paths.</param>
/// <param name="processingUnit">The CWL processing unit whose run references to resolve.</param>
/// <param name="tryResolveRunPath">A function that maps a path string to an optional CWLProcessingUnit.</param>
let resolveRunReferencesFromLookup (workflowFilePath: string) (processingUnit: CWL.CWLProcessingUnit) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.CWLProcessingUnit =
    match processingUnit with
    | CWL.Workflow workflow ->
        let state = { Cache = Map.empty }
        // The workflow is active from the start. A run that leads back to its file, or to its process main, stays
        // unresolved, so no resolution puts a process inside itself.
        let activePathKeys = Set.ofList [ pathKey workflowFilePath; pathKey (packedProcessPath workflowFilePath "main") ]
        resolveWorkflowRunsRecursiveWithResolver workflowFilePath activePathKeys state workflow tryResolveRunPath
        CWL.Workflow workflow
    | CWL.CommandLineTool _ ->
        processingUnit
    | CWL.ExpressionTool _ ->
        processingUnit
    | CWL.Operation _ ->
        processingUnit
    | CWL.RawProcess _ ->
        processingUnit

/// <summary>
/// Resolves the run references of the CWL file of a run or a workflow. A file with one process resolves like
/// resolveRunReferencesFromLookup and has no graph. A packed document starts with its process main. The run references
/// of the form #id resolve to the processes of this document, which the resolver takes from the graph itself, so the
/// ResolvedRun of the steps of main and of the other processes holds the objects of the graph. Every other reference
/// resolves through the lookup. The steps keep the references as they are written, so the file is written as it was read.
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
        match CWL.CWLGraph.tryFindMain graph with
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
/// Resolves the run reference of a single workflow step by looking up the run path string in the provided resolver
/// function. The step keeps its run and receives the process in its ResolvedRun.
/// </summary>
/// <param name="workflowFilePath">The file path of the parent workflow CWL file, used for resolving relative paths.</param>
/// <param name="step">The step whose run to resolve.</param>
/// <param name="tryResolveRunPath">A function that maps a path string to an optional CWLProcessingUnit.</param>
let resolveWorkflowStepFromLookup (workflowFilePath: string) (step: CWL.WorkflowStep) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : unit =
    let state = { Cache = Map.empty }
    match step.Run with
    | CWL.RunString runPath
    | CWL.RunImport runPath ->
        step.ResolvedRun <-
            { Run = resolveRunReferenceWithResolver workflowFilePath Set.empty state runPath tryResolveRunPath }
    | CWL.RunWorkflow _ ->
        match CWL.WorkflowStepRunOps.tryGetWorkflow step.Run with
        | Some inlineWorkflow ->
            resolveWorkflowRunsRecursiveWithResolver workflowFilePath Set.empty state inlineWorkflow tryResolveRunPath
        | None -> ()
    | _ -> ()
