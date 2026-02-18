namespace ARCtrl.WorkflowGraph

open ARCtrl
open ARCtrl.Helper
open ARCtrl.CWL

[<RequireQualifiedAccess>]
module Adapters =

    /// Builds a lookup map of normalized CWL file paths to processing units from all workflows and runs in an investigation.
    let createCwlLookupFromInvestigation (investigation: ArcInvestigation option) =
        match investigation with
        | None ->
            Map.empty
        | Some inv ->
            seq {
                for workflow in inv.Workflows do
                    match workflow.CWLDescription with
                    | Some cwl ->
                        yield Identifier.Workflow.cwlFileNameFromIdentifier workflow.Identifier, cwl
                    | None ->
                        ()
                for run in inv.Runs do
                    match run.CWLDescription with
                    | Some cwl ->
                        yield Identifier.Run.cwlFileNameFromIdentifier run.Identifier, cwl
                    | None ->
                        ()
            }
            |> Seq.map (fun (path, cwl) -> ArcPathHelper.normalizePathKey path, cwl)
            |> Map.ofSeq

    /// Wraps a CWL lookup map into a resolver function suitable for WorkflowGraphBuildOptions.TryResolveRunPath.
    let createResolver (lookup: Map<string, CWLProcessingUnit>) =
        if Map.isEmpty lookup then
            None
        else
            Some (fun (path: string) -> lookup |> Map.tryFind (ArcPathHelper.normalizePathKey path))

    /// Creates a GraphBuildIssue for a workflow or run that has no CWL description.
    let createMissingDescriptionError identifier scopeType =
        GraphBuildIssue.create(
            GraphIssueKind.MissingCwlDescription,
            $"No CWLDescription available for {scopeType} '{identifier}'.",
            scope = identifier
        )

    /// <summary>
    /// Converts an ArcWorkflow into a WorkflowGraph.
    /// Returns Error with a GraphBuildIssue if the workflow has no CWL description.
    /// Automatically creates a resolver from the parent investigation's CWL descriptions.
    /// </summary>
    /// <param name="workflow">The ArcWorkflow to build a graph from.</param>
    let ofWorkflow (workflow: ArcWorkflow) : Result<WorkflowGraph, GraphBuildIssue> =
        match workflow.CWLDescription with
        | None ->
            Error (createMissingDescriptionError workflow.Identifier "workflow")
        | Some processingUnit ->
            let cwlLookup = createCwlLookupFromInvestigation workflow.Investigation
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootScope workflow.Identifier
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some (Identifier.Workflow.cwlFileNameFromIdentifier workflow.Identifier))
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (createResolver cwlLookup)
            Builder.buildWith options processingUnit
            |> Ok

    /// <summary>
    /// Converts an ArcRun into a WorkflowGraph.
    /// Returns Error with a GraphBuildIssue if the run has no CWL description.
    /// Automatically creates a resolver from the parent investigation's CWL descriptions.
    /// </summary>
    /// <param name="run">The ArcRun to build a graph from.</param>
    let ofRun (run: ArcRun) : Result<WorkflowGraph, GraphBuildIssue> =
        match run.CWLDescription with
        | None ->
            Error (createMissingDescriptionError run.Identifier "run")
        | Some processingUnit ->
            let cwlLookup = createCwlLookupFromInvestigation run.Investigation
            let options =
                WorkflowGraphBuildOptions.defaultOptions
                |> WorkflowGraphBuildOptions.withRootScope run.Identifier
                |> WorkflowGraphBuildOptions.withRootWorkflowFilePath (Some (Identifier.Run.cwlFileNameFromIdentifier run.Identifier))
                |> WorkflowGraphBuildOptions.withTryResolveRunPath (createResolver cwlLookup)
            Builder.buildWith options processingUnit
            |> Ok

    /// <summary>
    /// Builds workflow graphs for all workflows and runs in an ArcInvestigation.
    /// Returns a WorkflowGraphIndex containing named results for each workflow and run.
    /// </summary>
    /// <param name="investigation">The ArcInvestigation containing the workflows and runs to graph.</param>
    let ofInvestigation (investigation: ArcInvestigation) : WorkflowGraphIndex =
        let workflowGraphs = ResizeArray()
        let runGraphs = ResizeArray()

        for workflow in investigation.Workflows do
            workflowGraphs.Add(workflow.Identifier, ofWorkflow workflow)
        for run in investigation.Runs do
            runGraphs.Add(run.Identifier, ofRun run)

        WorkflowGraphIndex.create(workflowGraphs = workflowGraphs, runGraphs = runGraphs)
