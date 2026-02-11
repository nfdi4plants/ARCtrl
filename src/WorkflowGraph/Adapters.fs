namespace ARCtrl.WorkflowGraph

open ARCtrl
open ARCtrl.Helper
open ARCtrl.CWL

[<RequireQualifiedAccess>]
module Adapters =

    let private createCwlLookupFromInvestigation (investigation: ArcInvestigation option) =
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

    let private createResolver (lookup: Map<string, CWLProcessingUnit>) =
        if Map.isEmpty lookup then
            None
        else
            Some (fun (path: string) -> lookup |> Map.tryFind (ArcPathHelper.normalizePathKey path))

    let private createMissingDescriptionError identifier scopeType =
        {
            Kind = GraphIssueKind.MissingCwlDescription
            Message = $"No CWLDescription available for {scopeType} '{identifier}'."
            Scope = Some identifier
            Reference = None
        }

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

    let ofInvestigation (investigation: ArcInvestigation) : WorkflowGraphIndex =
        let workflowGraphs = ResizeArray()
        let runGraphs = ResizeArray()

        for workflow in investigation.Workflows do
            workflowGraphs.Add(workflow.Identifier, ofWorkflow workflow)
        for run in investigation.Runs do
            runGraphs.Add(run.Identifier, ofRun run)

        {
            WorkflowGraphs = workflowGraphs
            RunGraphs = runGraphs
        }
