module ARCtrl.CWLRunResolver

open ARCtrl
open ARCtrl.FileSystem

/// Normalizes a file path for use as a lookup key.
let pathKey (path: string) : string =
    ArcPathHelper.normalizePathKey path

type RunResolutionState = {
    mutable Cache: Map<string, CWL.CWLProcessingUnit>
}

/// Generates candidate lookup paths for a .cwl run reference relative to a workflow file.
let getRunNodeCandidates (workflowFilePath: string) (runPath: string) : string [] =
    if System.String.IsNullOrWhiteSpace runPath then
        [||]
    else
        let trimmedRunPath = runPath.Trim()
        if not (trimmedRunPath.EndsWith(".cwl", System.StringComparison.OrdinalIgnoreCase)) then
            [||]
        else
            let normalizedRunPath = ArcPathHelper.normalize trimmedRunPath
            let runPathInWorkflowDirectory = ArcPathHelper.resolvePathFromFile workflowFilePath trimmedRunPath
            let normalizedRunPathInWorkflowDirectory = ArcPathHelper.normalize runPathInWorkflowDirectory

            [|
                trimmedRunPath
                normalizedRunPath
                runPathInWorkflowDirectory
                normalizedRunPathInWorkflowDirectory
            |]
            |> Array.filter (fun p -> not (System.String.IsNullOrWhiteSpace p))
            |> Array.distinct

let cacheResolvedProcessingUnit (state: RunResolutionState) (candidateKeys: string []) (processingUnit: CWL.CWLProcessingUnit) =
    for key in candidateKeys do
        state.Cache <- state.Cache.Add(key, processingUnit)

let runFromProcessingUnit (processingUnit: CWL.CWLProcessingUnit) =
    match processingUnit with
    | CWL.CommandLineTool tool -> CWL.WorkflowStepRunOps.fromTool tool
    | CWL.Workflow workflow -> CWL.WorkflowStepRunOps.fromWorkflow workflow
    | CWL.ExpressionTool expressionTool -> CWL.WorkflowStepRunOps.fromExpressionTool expressionTool

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
        resolveWorkflowRunsRecursiveWithResolver workflowFilePath Set.empty state workflow tryResolveRunPath
        CWL.Workflow workflow
    | CWL.CommandLineTool _ ->
        processingUnit
    | CWL.ExpressionTool _ ->
        processingUnit

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

