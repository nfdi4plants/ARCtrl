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

/// True when the process is a target or reaches one through its steps: an inline workflow of a step, or the workflow
/// that a run reference of a step resolved to, and so on further down. The walk compares objects and visits each workflow
/// once, so it ends on a cycle that the steps hold already.
let private reaches (isTarget: CWL.CWLProcessingUnit -> bool) (processingUnit: CWL.CWLProcessingUnit) : bool =
    let visited = ResizeArray<CWL.CWLProcessingUnit>()
    let rec visit (candidate: CWL.CWLProcessingUnit) : bool =
        if isTarget candidate then
            true
        elif visited |> Seq.exists (CWL.CWLProcessingUnit.isSameObject candidate) then
            false
        else
            visited.Add candidate
            match candidate with
            | CWL.Workflow workflow ->
                CWL.CWLWorkflowDescription.getStepValues workflow
                |> Seq.exists (fun step ->
                    [ CWL.WorkflowStepRunOps.tryGetWorkflow step.Run
                      step.ResolvedRun.Run |> Option.bind CWL.WorkflowStepRunOps.tryGetWorkflow ]
                    |> List.choose id
                    |> List.exists (fun workflow -> visit (CWL.Workflow workflow)))
            | _ -> false
    visit processingUnit

/// True when the process is one of the given processes or reaches one through its steps.
let private reachesAny (targets: CWL.CWLProcessingUnit list) (processingUnit: CWL.CWLProcessingUnit) : bool =
    reaches (fun candidate -> targets |> List.exists (CWL.CWLProcessingUnit.isSameObject candidate)) processingUnit

/// Resolves the run reference of each step of a workflow and of each workflow nested inline in it. A step keeps its
/// run as it is written and receives the process in its ResolvedRun. A step that holds a process already keeps it,
/// because the lookup hands out one object for each file and the first resolution of that object decided its steps.
/// The active workflows are the workflows whose steps are being resolved, outermost last. An inline workflow that is
/// one of them holds itself, so the walk does not enter it again.
let rec resolveWorkflowRunsRecursiveWithResolver
    (workflowFilePath: string)
    (activePathKeys: Set<string>)
    (activeWorkflows: CWL.CWLProcessingUnit list)
    (state: RunResolutionState)
    (workflow: CWL.CWLWorkflowDescription)
    (tryResolveRunPath: string -> CWL.CWLProcessingUnit option)
    : unit =
    let activeWorkflows = CWL.Workflow workflow :: activeWorkflows
    // An imported step stands for steps of a file that the decoder does not read, so it has no run to resolve.
    for step in CWL.CWLWorkflowDescription.getStepValues workflow do
        match step.Run with
        | CWL.RunString runPath
        | CWL.RunImport runPath when step.ResolvedRun.Run.IsNone ->
            step.ResolvedRun <-
                { Run = resolveRunReferenceWithResolver workflowFilePath activePathKeys activeWorkflows state runPath tryResolveRunPath }
        | CWL.RunWorkflow _ ->
            match CWL.WorkflowStepRunOps.tryGetWorkflow step.Run with
            | Some inlineWorkflow when not (activeWorkflows |> List.exists (CWL.CWLProcessingUnit.isSameObject (CWL.Workflow inlineWorkflow))) ->
                resolveWorkflowRunsRecursiveWithResolver workflowFilePath activePathKeys activeWorkflows state inlineWorkflow tryResolveRunPath
            | _ -> ()
        | _ -> ()

/// Tries each candidate path of a run reference against the resolver and returns the run of the first process that it
/// finds. A candidate that leads back to a file that is being resolved is skipped, and so is a process that is one of the
/// active workflows or reaches one, because the step of the innermost one would then hold a process that holds the step.
/// The cache holds a process under the path it was found at, because the same relative path in the directory of another
/// workflow is another file.
and resolveRunReferenceWithResolver
    (workflowFilePath: string)
    (activePathKeys: Set<string>)
    (activeWorkflows: CWL.CWLProcessingUnit list)
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
                | Some cached when not (reachesAny activeWorkflows cached) ->
                    tryRunFromProcessingUnit cached
                | Some _ ->
                    tryResolveCandidate (index + 1)
                | None ->
                    match tryResolveRunPath candidate |> Option.bind (fun found -> tryRunFromProcessingUnit found |> Option.map (fun run -> found, run)) with
                    | Some (processingUnit, run) when not (reachesAny activeWorkflows processingUnit) ->
                        match processingUnit with
                        | CWL.Workflow workflow ->
                            resolveWorkflowRunsRecursiveWithResolver candidate (activePathKeys.Add key) activeWorkflows state workflow tryResolveRunPath
                        | _ -> ()
                        cacheResolvedProcessingUnit state key processingUnit
                        Some run
                    | _ ->
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
        // The workflow is active from the start. A run that leads back to its file stays unresolved, so no resolution
        // puts a process inside itself.
        let activePathKeys = Set.ofList [ pathKey workflowFilePath ]
        resolveWorkflowRunsRecursiveWithResolver workflowFilePath activePathKeys [] state workflow tryResolveRunPath
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
/// Resolves the run reference of a single workflow step by looking up the run path string in the provided resolver
/// function. The step keeps its run and receives the process in its ResolvedRun. The workflow that holds the step is not
/// known here, so a process whose steps lead back to the step stays out of the ResolvedRun.
/// </summary>
/// <param name="workflowFilePath">The file path of the parent workflow CWL file, used for resolving relative paths.</param>
/// <param name="step">The step whose run to resolve.</param>
/// <param name="tryResolveRunPath">A function that maps a path string to an optional CWLProcessingUnit.</param>
let resolveWorkflowStepFromLookup (workflowFilePath: string) (step: CWL.WorkflowStep) (tryResolveRunPath: string -> CWL.CWLProcessingUnit option) : unit =
    let state = { Cache = Map.empty }
    match step.Run with
    | CWL.RunString runPath
    | CWL.RunImport runPath ->
        let holdsStep (processingUnit: CWL.CWLProcessingUnit) =
            match processingUnit with
            | CWL.Workflow workflow -> CWL.CWLWorkflowDescription.getStepValues workflow |> Seq.exists (fun other -> System.Object.ReferenceEquals(other, step))
            | _ -> false
        let resolved = resolveRunReferenceWithResolver workflowFilePath Set.empty [] state runPath tryResolveRunPath
        let leadsBack (run: CWL.WorkflowStepRun) =
            CWL.WorkflowStepRunOps.tryGetWorkflow run |> Option.exists (fun workflow -> reaches holdsStep (CWL.Workflow workflow))
        step.ResolvedRun <- { Run = resolved |> Option.filter (leadsBack >> not) }
    | CWL.RunWorkflow _ ->
        match CWL.WorkflowStepRunOps.tryGetWorkflow step.Run with
        | Some inlineWorkflow ->
            resolveWorkflowRunsRecursiveWithResolver workflowFilePath Set.empty [] state inlineWorkflow tryResolveRunPath
        | None -> ()
    | _ -> ()
