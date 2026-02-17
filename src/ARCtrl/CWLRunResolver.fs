module ARCtrl.CWLRunResolver

open ARCtrl
open ARCtrl.FileSystem

/// Normalizes a file path for use as a lookup key.
let pathKey (path: string) : string =
    ArcPathHelper.normalizePathKey path

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

/// Iterates all steps in a workflow and resolves each step's run field recursively, tracking visited paths to prevent cycles.
let rec resolveWorkflowRunsRecursiveWithResolver (workflowFilePath: string) (visited: Set<string>) (workflow: CWL.CWLWorkflowDescription) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : Set<string> =
    let mutable visitedState = visited
    for step in workflow.Steps do
        let resolvedRun, nextVisited =
            resolveWorkflowStepRunRecursiveWithResolver workflowFilePath visitedState step.Run tryResolveRunPath
        step.Run <- resolvedRun
        visitedState <- nextVisited
    visitedState

/// Resolves a single WorkflowStepRun, trying each candidate path against the resolver for RunString values.
and resolveWorkflowStepRunRecursiveWithResolver (workflowFilePath: string) (visited: Set<string>) (run: CWL.WorkflowStepRun) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.WorkflowStepRun * Set<string> =
    match run with
    | CWL.RunString runPath ->
        let candidates = getRunNodeCandidates workflowFilePath runPath
        candidates
        |> Array.tryPick (fun resolvedRunPath ->
            let key = pathKey resolvedRunPath
            if visited.Contains key then
                None
            else
                match tryResolveRunPath resolvedRunPath with
                | Some (CWL.CommandLineTool tool) ->
                    Some (CWL.WorkflowStepRunOps.fromTool tool, visited.Add key)
                | Some (CWL.Workflow workflow) ->
                    let visitedWithCurrent = visited.Add key
                    let nextVisited =
                        resolveWorkflowRunsRecursiveWithResolver resolvedRunPath visitedWithCurrent workflow tryResolveRunPath
                    Some (CWL.WorkflowStepRunOps.fromWorkflow workflow, nextVisited)
                | Some (CWL.ExpressionTool expressionTool) ->
                    Some (CWL.WorkflowStepRunOps.fromExpressionTool expressionTool, visited.Add key)
                | None ->
                    None
        )
        |> Option.defaultValue (run, visited)
    | CWL.RunWorkflow _ ->
        match CWL.WorkflowStepRunOps.tryGetWorkflow run with
        | Some workflow ->
            let nextVisited =
                resolveWorkflowRunsRecursiveWithResolver workflowFilePath visited workflow tryResolveRunPath
            CWL.WorkflowStepRunOps.fromWorkflow workflow, nextVisited
        | None ->
            run, visited
    | CWL.RunCommandLineTool _ ->
        run, visited
    | CWL.RunExpressionTool _ ->
        run, visited

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
        resolveWorkflowRunsRecursiveWithResolver workflowFilePath Set.empty workflow tryResolveRunPath
        |> ignore
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
    resolveWorkflowStepRunRecursiveWithResolver workflowFilePath Set.empty run tryResolveRunPath
    |> fst

