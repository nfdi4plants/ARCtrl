module ARCtrl.CWLRunResolver

open ARCtrl
open ARCtrl.FileSystem

let private pathKey (path: string) : string =
    ArcPathHelper.normalizePathKey path

let private getRunNodeCandidates (workflowFilePath: string) (runPath: string) : string [] =
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

let rec private resolveWorkflowRunsRecursiveWithResolver (workflowFilePath: string) (visited: Set<string>) (workflow: CWL.CWLWorkflowDescription) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : Set<string> =
    let mutable visitedState = visited
    for step in workflow.Steps do
        let resolvedRun, nextVisited =
            resolveWorkflowStepRunRecursiveWithResolver workflowFilePath visitedState step.Run tryResolveRunPath
        step.Run <- resolvedRun
        visitedState <- nextVisited
    visitedState

and private resolveWorkflowStepRunRecursiveWithResolver (workflowFilePath: string) (visited: Set<string>) (run: CWL.WorkflowStepRun) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.WorkflowStepRun * Set<string> =
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

/// Resolve CWL WorkflowStep `run` references for a processing unit via path lookup.
let resolveRunReferencesFromLookup (workflowFilePath: string) (processingUnit: CWL.CWLProcessingUnit) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.CWLProcessingUnit =
    match processingUnit with
    | CWL.Workflow workflow ->
        resolveWorkflowRunsRecursiveWithResolver workflowFilePath Set.empty workflow tryResolveRunPath
        |> ignore
        CWL.Workflow workflow
    | CWL.CommandLineTool _ ->
        processingUnit

/// Resolve a single workflow step run value via path lookup.
let resolveWorkflowStepRunFromLookup (workflowFilePath: string) (run: CWL.WorkflowStepRun) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : CWL.WorkflowStepRun =
    resolveWorkflowStepRunRecursiveWithResolver workflowFilePath Set.empty run tryResolveRunPath
    |> fst

