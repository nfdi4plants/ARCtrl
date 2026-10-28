namespace ARCtrl.WorkflowGraph

open ARCtrl
open ARCtrl.CWL
open DynamicObj

[<RequireQualifiedAccess>]
module Builder =

    /// Internal mutable state tracking nodes and edges during graph building.
    type BuildState = {
        Graph: WorkflowGraph
        mutable NodeIds: Set<WorkflowGraphNodeId>
        mutable EdgeIds: Set<WorkflowGraphEdgeId>
        mutable RunResolutionStack: Set<string>
        /// The workflows whose steps are being built, innermost first. A workflow that runs one of them, inline or
        /// through a resolved run, would build without end.
        mutable ActiveWorkflows: CWLProcessingUnit list
        mutable RunResolutionCache: Map<string, WorkflowStepRun>
        mutable RunNodeCache: Map<string, WorkflowGraphNodeId>
        Options: WorkflowGraphBuildOptions
    }

    /// Result of resolving a CWL run reference string.
    type RunResolutionResult = {
        ResolvedRun: WorkflowStepRun
        ResolvedFromPath: string option
        AttemptedLookup: bool
        ResolutionPathKeys: string []
    }

    /// Creates an empty WorkflowGraph with the given root node ID.
    let createGraph (rootNodeId: WorkflowGraphNodeId) =
        WorkflowGraph.create(rootNodeId)

    /// Creates initial build state from options and root node ID.
    let createState (options: WorkflowGraphBuildOptions) (rootNodeId: WorkflowGraphNodeId) =
        {
            Graph = createGraph rootNodeId
            NodeIds = Set.empty
            EdgeIds = Set.empty
            RunResolutionStack = Set.empty
            ActiveWorkflows = []
            RunResolutionCache = Map.empty
            RunNodeCache = Map.empty
            Options = options
        }

    /// Appends a diagnostic issue to the graph's diagnostics list.
    let addDiagnostic (state: BuildState) (kind: GraphIssueKind) (message: string) (scope: string option) (reference: string option) =
        state.Graph.Diagnostics.Add(GraphBuildIssue.create(kind, message, ?scope = scope, ?reference = reference))

    /// Checks if a node ID is already registered in the build state.
    let hasNode (state: BuildState) (nodeId: WorkflowGraphNodeId) =
        state.NodeIds.Contains nodeId

    /// Adds a node to the graph if not already present.
    let addNode (state: BuildState) (node: WorkflowGraphNode) =
        if state.NodeIds.Contains node.Id |> not then
            state.NodeIds <- state.NodeIds.Add node.Id
            state.Graph.Nodes.Add node

    /// Adds an edge to the graph if not already present.
    let addEdge (state: BuildState) (edge: WorkflowGraphEdge) =
        if state.EdgeIds.Contains edge.Id |> not then
            state.EdgeIds <- state.EdgeIds.Add edge.Id
            state.Graph.Edges.Add edge

    /// Creates and adds an edge with a given kind, source, target, and optional label.
    let addEdgeByType (state: BuildState) (kind: EdgeKind) (sourceNodeId: WorkflowGraphNodeId) (targetNodeId: WorkflowGraphNodeId) (label: string option) =
        let edge =
            WorkflowGraphEdge.create
                (GraphId.edgeId kind sourceNodeId targetNodeId, sourceNodeId, targetNodeId, kind, ?label = label)
        addEdge state edge

    /// Builds a DynamicObj metadata instance from key-value pairs where values are present.
    let createMetadata (pairs: (string * obj option) list) : DynamicObj option =
        let metadata = DynamicObj()
        let mutable hasMetadata = false
        for key, valueOpt in pairs do
            match valueOpt with
            | Some value ->
                hasMetadata <- true
                DynObj.setProperty key value metadata
            | None ->
                ()
        if hasMetadata then Some metadata else None

    /// Creates and adds a processing unit node to the graph. Returns the node ID.
    let addProcessingUnitNode
        (state: BuildState)
        (scope: string)
        (kind: ProcessingUnitKind)
        (label: string)
        (ownerNodeId: WorkflowGraphNodeId option)
        (reference: string option)
        (metadata: DynamicObj option)
        =
        let nodeId = GraphId.unitNodeId scope
        let node =
            WorkflowGraphNode.create
                (nodeId, NodeKind.ProcessingUnitNode kind, label, ?ownerNodeId = ownerNodeId, ?reference = reference, ?metadata = metadata)
        addNode state node
        nodeId

    /// Creates and adds a port node (input or output) owned by a given node. Returns the node ID.
    let addPortNode
        (state: BuildState)
        (ownerNodeId: WorkflowGraphNodeId)
        (direction: PortDirection)
        (portId: string)
        (label: string)
        =
        let nodeId = GraphId.portNodeId ownerNodeId direction portId
        let node =
            WorkflowGraphNode.create
                (nodeId, NodeKind.PortNode direction, label, ownerNodeId = ownerNodeId)
        addNode state node
        nodeId

    /// How the graph names a step. A step with an id is named by its id, read against the id of the workflow, as the step
    /// #main/first of a document that writes its ids in full is first. A step without an id is named by its position among
    /// the entries of the steps of the workflow, in a namespace apart from the ids of the other steps. The name is a place
    /// in the graph and no id of the step.
    type StepIdentity = {
        /// The ID of the node of the step.
        NodeId: WorkflowGraphNodeId
        /// The name that the node shows when the step has no label, and that the diagnostics use.
        Name: string
        /// The segment that the scope of the process of the step ends in.
        RunSegment: string
    }

    /// The identity of a step in the graph. The position is that of the entry among the steps of the workflow.
    let stepIdentity (scope: string) (workflowId: string option) (position: int) (step: WorkflowStep) : StepIdentity =
        match step.Id with
        | Some id ->
            let name = ReferenceParsing.relativeId workflowId id
            { NodeId = GraphId.stepNodeId scope name; Name = name; RunSegment = $"{name}/run" }
        | None ->
            { NodeId = GraphId.anonymousStepNodeId scope position; Name = $"steps[{position}]"; RunSegment = $"anonymous:{position}/run" }

    /// Creates a step node for a CWL workflow step, adds a Contains edge from the workflow. Returns the step node ID.
    let addStepNode
        (state: BuildState)
        (workflowNodeId: WorkflowGraphNodeId)
        (identity: StepIdentity)
        (step: WorkflowStep)
        =
        let stepName = identity.Name
        let stepNodeId = identity.NodeId
        let metadata =
            createMetadata [
                "when", step.When_ |> Option.map (fun expression -> box expression.AsDirectiveString)
                "scatterMethod", step.ScatterMethod |> Option.map (fun sm -> sm.AsCwlString |> box)
                "scatter", step.Scatter |> Option.map (fun values -> values |> Seq.map (fun value -> value.AsDirectiveString) |> Seq.toArray |> box)
            ]
        let node =
            WorkflowGraphNode.create
                (stepNodeId, NodeKind.StepNode, defaultArg (step.Label |> Option.bind SchemaSaladString.tryLiteral) stepName, ownerNodeId = workflowNodeId, ?metadata = metadata)
        addNode state node
        addEdgeByType state EdgeKind.Contains workflowNodeId stepNodeId None
        stepNodeId

    /// Adds all input and output port nodes for a CWL workflow.
    let addWorkflowPorts (state: BuildState) (workflowNodeId: WorkflowGraphNodeId) (workflow: CWLWorkflowDescription) =
        let relative = ReferenceParsing.relativeId workflow.Id
        // An import stands for the ports of a file that the decoder does not read, so it gets no node.
        CWLWorkflowDescription.getInputValues workflow
        |> Seq.iter (fun input -> addPortNode state workflowNodeId PortDirection.Input (relative input.Name) (relative input.Name) |> ignore)
        CWLWorkflowDescription.getOutputValues workflow
        |> Seq.iter (fun output -> addPortNode state workflowNodeId PortDirection.Output (relative output.Name) (relative output.Name) |> ignore)

    /// Adds all input and output port nodes for a CWL CommandLineTool.
    let addToolPorts (state: BuildState) (toolNodeId: WorkflowGraphNodeId) (tool: CWLToolDescription) =
        CWLToolDescription.getInputValuesOrEmpty tool
        |> Seq.iter (fun input -> addPortNode state toolNodeId PortDirection.Input input.Name input.Name |> ignore)
        CWLToolDescription.getOutputValues tool
        |> Seq.iter (fun output -> addPortNode state toolNodeId PortDirection.Output output.Name output.Name |> ignore)

    /// Adds all input and output port nodes for a CWL ExpressionTool.
    let addExpressionToolPorts (state: BuildState) (toolNodeId: WorkflowGraphNodeId) (tool: CWLExpressionToolDescription) =
        CWLExpressionToolDescription.getInputValuesOrEmpty tool
        |> Seq.iter (fun input -> addPortNode state toolNodeId PortDirection.Input input.Name input.Name |> ignore)
        CWLExpressionToolDescription.getOutputValues tool
        |> Seq.iter (fun output -> addPortNode state toolNodeId PortDirection.Output output.Name output.Name |> ignore)

    /// Adds all input and output port nodes for a CWL Operation.
    let addOperationPorts (state: BuildState) (operationNodeId: WorkflowGraphNodeId) (operation: CWLOperationDescription) =
        CWLOperationDescription.getInputValues operation
        |> Seq.iter (fun input -> addPortNode state operationNodeId PortDirection.Input input.Name input.Name |> ignore)
        CWLOperationDescription.getOutputValues operation
        |> Seq.iter (fun output -> addPortNode state operationNodeId PortDirection.Output output.Name output.Name |> ignore)

    /// Adds all input and output port nodes for a workflow step.
    let addStepPorts (state: BuildState) (stepNodeId: WorkflowGraphNodeId) (step: WorkflowStep) =
        let relative = ReferenceParsing.relativeId step.Id
        // An import or a mixin stands for ports of a file that the decoder does not read, and an include for the id of an output
        // of a file, so none of them gets a node.
        WorkflowStep.getInputValues step
        |> Seq.iter (fun input -> addPortNode state stepNodeId PortDirection.Input (relative input.Id) (relative input.Id) |> ignore)
        step.Out
        |> Seq.choose StepOutput.tryId
        |> Seq.iter (fun id ->
            let outputId = relative id
            addPortNode state stepNodeId PortDirection.Output outputId outputId |> ignore
        )

    /// Strips '#' prefixes, query strings, and fragment identifiers from a reference string.
    let trimToNameCandidate (value: string) =
        if System.String.IsNullOrWhiteSpace value then
            ""
        else
            let trimmed = value.Trim()
            if trimmed.StartsWith("#") then
                trimmed.Substring(1)
            else
                let hashIndex = trimmed.IndexOf('#')
                let withoutHash =
                    if hashIndex >= 0 then
                        trimmed.Substring(0, hashIndex)
                    else
                        trimmed
                let queryIndex = withoutHash.IndexOf('?')
                if queryIndex >= 0 then
                    withoutHash.Substring(0, queryIndex)
                else
                    withoutHash

    /// Returns true if a run path ends in '.cwl'.
    let isCwlReference (runPath: string) =
        let candidate = trimToNameCandidate runPath
        System.String.IsNullOrWhiteSpace candidate |> not
        && candidate.EndsWith(".cwl", System.StringComparison.OrdinalIgnoreCase)

    /// Extracts the basename of a path and strips the '.cwl' extension.
    let tryBasenameNoCwlExtension (value: string) =
        let candidate = trimToNameCandidate value
        if System.String.IsNullOrWhiteSpace candidate then
            None
        else
            let fileName =
                if candidate.Contains("/") || candidate.Contains("\\") then
                    ArcPathHelper.getFileName candidate
                else
                    candidate
            let withoutCwlExtension =
                if fileName.EndsWith(".cwl", System.StringComparison.OrdinalIgnoreCase) then
                    fileName.Substring(0, fileName.Length - ".cwl".Length)
                else
                    fileName
            if System.String.IsNullOrWhiteSpace withoutCwlExtension then
                None
            else
                Some withoutCwlExtension

    /// Selects the first available processing unit display name from dynamic metadata:
    /// `name` first, then `id`; each value is normalized to a CWL basename.
    let tryGetProcessingUnitName (processingUnit: #DynamicObj) =
        [ "name"; "id" ]
        |> List.tryPick (fun key ->
            DynObj.tryGetTypedPropertyValue<string> key processingUnit
            |> Option.bind tryBasenameNoCwlExtension
        )

    /// Picks the first non-None candidate label, or falls back to a default.
    let chooseLabel (fallback: string) (candidates: string option list) =
        candidates
        |> List.tryPick id
        |> Option.defaultValue fallback

    /// Generates multiple path candidates to try when resolving a run reference.
    let getRunPathCandidates (workflowFilePath: string option) (runPath: string) : string [] =
        if System.String.IsNullOrWhiteSpace runPath then
            [||]
        else
            let trimmedRunPath = runPath.Trim()
            let normalizedRunPath = ArcPathHelper.normalize trimmedRunPath
            match workflowFilePath with
            | Some workflowPath when System.String.IsNullOrWhiteSpace workflowPath |> not ->
                let pathFromWorkflowFile = ArcPathHelper.resolvePathFromFile workflowPath trimmedRunPath
                let normalizedPathFromWorkflowFile = ArcPathHelper.normalize pathFromWorkflowFile
                [|
                    trimmedRunPath
                    normalizedRunPath
                    pathFromWorkflowFile
                    normalizedPathFromWorkflowFile
                |]
                |> Array.filter (System.String.IsNullOrWhiteSpace >> not)
                |> Array.distinct
            | _ ->
                [|
                    trimmedRunPath
                    normalizedRunPath
                |]
                |> Array.filter (System.String.IsNullOrWhiteSpace >> not)
                |> Array.distinct

    /// Attempts to resolve a CWL run string reference to an actual CWLProcessingUnit via the configured resolver.
    let resolveRunString
        (state: BuildState)
        (workflowFilePath: string option)
        (runPath: string)
        : RunResolutionResult
        =
        if System.String.IsNullOrWhiteSpace runPath || isCwlReference runPath |> not then
            {
                ResolvedRun = RunString runPath
                ResolvedFromPath = None
                AttemptedLookup = false
                ResolutionPathKeys = [||]
            }
        else
            match state.Options.TryResolveRunPath with
            | None ->
                if state.Options.StrictUnresolvedRunReferences then
                    addDiagnostic
                        state
                        GraphIssueKind.ResolutionFailed
                        $"Unable to resolve CWL run reference '{runPath}' because no run resolver is configured."
                        workflowFilePath
                        (Some runPath)
                {
                    ResolvedRun = RunString runPath
                    ResolvedFromPath = None
                    AttemptedLookup = state.Options.StrictUnresolvedRunReferences
                    ResolutionPathKeys = [||]
                }
            | Some tryResolveRunPath ->
                let candidates = getRunPathCandidates workflowFilePath runPath
                let candidateKeys =
                    candidates
                    |> Array.map ArcPathHelper.normalizePathKey
                    |> Array.distinct
                let mutable resolvedRun: (WorkflowStepRun * string) option = None

                for candidate in candidates do
                    if resolvedRun.IsNone then
                        let pathKey = ArcPathHelper.normalizePathKey candidate
                        if state.RunResolutionStack.Contains pathKey then
                            addDiagnostic
                                state
                                GraphIssueKind.CycleDetected
                                $"Cycle detected while resolving run reference '{candidate}'."
                                workflowFilePath
                                (Some candidate)
                        elif state.RunResolutionCache.ContainsKey pathKey then
                            resolvedRun <- Some (state.RunResolutionCache.[pathKey], candidate)
                        else
                            match tryResolveRunPath candidate with
                            | Some (CommandLineTool tool) ->
                                let run = WorkflowStepRunOps.fromTool tool
                                for key in candidateKeys do
                                    state.RunResolutionCache <- state.RunResolutionCache.Add(key, run)
                                resolvedRun <- Some (run, candidate)
                            | Some (Workflow workflow) ->
                                let run = WorkflowStepRunOps.fromWorkflow workflow
                                for key in candidateKeys do
                                    state.RunResolutionCache <- state.RunResolutionCache.Add(key, run)
                                resolvedRun <- Some (run, candidate)
                            | Some (ExpressionTool expressionTool) ->
                                let run = WorkflowStepRunOps.fromExpressionTool expressionTool
                                for key in candidateKeys do
                                    state.RunResolutionCache <- state.RunResolutionCache.Add(key, run)
                                resolvedRun <- Some (run, candidate)
                            | Some (Operation operation) ->
                                let run = WorkflowStepRunOps.fromOperation operation
                                for key in candidateKeys do
                                    state.RunResolutionCache <- state.RunResolutionCache.Add(key, run)
                                resolvedRun <- Some (run, candidate)
                            // A step has no run for a process whose class is a directive, so the reference stays unresolved.
                            | Some (RawProcess _)
                            | None ->
                                ()

                match resolvedRun with
                | Some (run, resolvedPath) ->
                    {
                        ResolvedRun = run
                        ResolvedFromPath = Some resolvedPath
                        AttemptedLookup = true
                        ResolutionPathKeys = candidateKeys
                    }
                | None ->
                    addDiagnostic
                        state
                        GraphIssueKind.ResolutionFailed
                        $"Unable to resolve CWL run reference '{runPath}'."
                        workflowFilePath
                        (Some runPath)
                    {
                        ResolvedRun = RunString runPath
                        ResolvedFromPath = None
                        AttemptedLookup = true
                        ResolutionPathKeys = candidateKeys
                    }

    /// Recursively builds graph nodes and edges for a CWLProcessingUnit (CommandLineTool, ExpressionTool, Operation, or Workflow).
    let rec buildProcessingUnit
        (state: BuildState)
        (scope: string)
        (workflowFilePath: string option)
        (ownerNodeId: WorkflowGraphNodeId option)
        (isRoot: bool)
        (processingUnit: CWLProcessingUnit)
        : WorkflowGraphNodeId
        =
        match processingUnit with
        | CommandLineTool tool ->
            let label =
                chooseLabel
                    "CommandLineTool"
                    [
                        tryGetProcessingUnitName tool
                        tool.Label |> Option.bind SchemaSaladString.tryLiteral |> Option.bind tryBasenameNoCwlExtension
                        tool.BaseCommand |> Option.bind Seq.tryHead |> Option.bind SchemaSaladString.tryLiteral |> Option.bind tryBasenameNoCwlExtension
                        workflowFilePath |> Option.bind tryBasenameNoCwlExtension
                    ]
            let metadata = createMetadata [ "cwlVersion", Some (box tool.CWLVersion) ]
            let nodeId =
                addProcessingUnitNode
                    state
                    scope
                    ProcessingUnitKind.CommandLineTool
                    label
                    ownerNodeId
                    None
                    metadata
            addToolPorts state nodeId tool
            nodeId
        | ExpressionTool expressionTool ->
            let label =
                chooseLabel
                    "ExpressionTool"
                    [
                        tryGetProcessingUnitName expressionTool
                        expressionTool.Label |> Option.bind SchemaSaladString.tryLiteral |> Option.bind tryBasenameNoCwlExtension
                        workflowFilePath |> Option.bind tryBasenameNoCwlExtension
                    ]
            let metadata =
                createMetadata [
                    "cwlVersion", Some (box expressionTool.CWLVersion)
                    "expression", Some (box expressionTool.Expression.AsDirectiveString)
                ]
            let nodeId =
                addProcessingUnitNode
                    state
                    scope
                    ProcessingUnitKind.ExpressionTool
                    label
                    ownerNodeId
                    None
                    metadata
            addExpressionToolPorts state nodeId expressionTool
            nodeId
        | Operation operation ->
            let label =
                chooseLabel
                    "Operation"
                    [
                        tryGetProcessingUnitName operation
                        operation.Label |> Option.bind SchemaSaladString.tryLiteral |> Option.bind tryBasenameNoCwlExtension
                        workflowFilePath |> Option.bind tryBasenameNoCwlExtension
                    ]
            let metadata =
                createMetadata [
                    "cwlVersion", Some (box operation.CWLVersion)
                ]
            let nodeId =
                addProcessingUnitNode
                    state
                    scope
                    ProcessingUnitKind.Operation
                    label
                    ownerNodeId
                    None
                    metadata
            addOperationPorts state nodeId operation
            nodeId
        | Workflow workflow ->
            let label =
                chooseLabel
                    "Workflow"
                    [
                        tryGetProcessingUnitName workflow
                        workflow.Label |> Option.bind SchemaSaladString.tryLiteral |> Option.bind tryBasenameNoCwlExtension
                        workflowFilePath |> Option.bind tryBasenameNoCwlExtension
                    ]
            let metadata = createMetadata [ "cwlVersion", Some (box workflow.CWLVersion) ]
            let nodeId =
                addProcessingUnitNode
                    state
                    scope
                    ProcessingUnitKind.Workflow
                    label
                    ownerNodeId
                    None
                    metadata
            addWorkflowPorts state nodeId workflow
            if isRoot || state.Options.ExpandNestedWorkflows then
                if state.ActiveWorkflows |> List.exists (CWLProcessingUnit.isSameObject processingUnit) then
                    // The workflow runs itself, so the second node shows its ports and leaves its steps to the first one.
                    addDiagnostic
                        state
                        GraphIssueKind.CycleDetected
                        $"Cycle detected: the workflow '{label}' runs itself through its steps."
                        workflowFilePath
                        None
                else
                    let previousWorkflows = state.ActiveWorkflows
                    state.ActiveWorkflows <- processingUnit :: previousWorkflows
                    try
                        buildWorkflowSteps state scope workflowFilePath nodeId workflow
                    finally
                        state.ActiveWorkflows <- previousWorkflows
            nodeId
        | RawProcess raw ->
            // A file decides the class of the process and the decoder reads no file, so the graph shows one node without ports.
            let label =
                chooseLabel
                    "Process with a directive as its class"
                    [
                        raw.TryScalarField "id" |> Option.bind tryBasenameNoCwlExtension
                        workflowFilePath |> Option.bind tryBasenameNoCwlExtension
                    ]
            addProcessingUnitNode state scope ProcessingUnitKind.ExternalReference label ownerNodeId None None

    /// Resolves and builds the graph representation of a workflow step's run field.
    and buildRunNode
        (state: BuildState)
        (runScope: string)
        (stepNodeId: WorkflowGraphNodeId)
        (workflowFilePath: string option)
        (run: WorkflowStepRun)
        : WorkflowStepRun * WorkflowGraphNodeId
        =
        match run with
        | RunString runPath
        | RunImport runPath ->
            let resolution = resolveRunString state workflowFilePath runPath
            match resolution.ResolvedRun with
            | RunString unresolvedPath ->
                let unresolvedLabel =
                    chooseLabel
                        unresolvedPath
                        [
                            if isCwlReference unresolvedPath then
                                unresolvedPath |> tryBasenameNoCwlExtension
                            else
                                None
                        ]
                let kind =
                    if resolution.AttemptedLookup && isCwlReference unresolvedPath then
                        ProcessingUnitKind.UnresolvedReference
                    else
                        ProcessingUnitKind.ExternalReference
                let nodeId =
                    addProcessingUnitNode
                        state
                        runScope
                        kind
                        unresolvedLabel
                        (Some stepNodeId)
                        (Some unresolvedPath)
                        None
                resolution.ResolvedRun, nodeId
            | resolvedRun ->
                let cachedRunNodeId =
                    resolution.ResolutionPathKeys
                    |> Array.tryPick (fun key -> state.RunNodeCache |> Map.tryFind key)
                match cachedRunNodeId with
                | Some nodeId ->
                    resolvedRun, nodeId
                | None ->
                    let builtRun, builtNodeId =
                        match resolution.ResolvedFromPath with
                        | Some resolvedPath ->
                            let resolvedPathKey = ArcPathHelper.normalizePathKey resolvedPath
                            let previousStack = state.RunResolutionStack
                            state.RunResolutionStack <- previousStack.Add resolvedPathKey
                            try
                                buildRunNode state runScope stepNodeId resolution.ResolvedFromPath resolvedRun
                            finally
                                state.RunResolutionStack <- previousStack
                        | None ->
                            buildRunNode state runScope stepNodeId resolution.ResolvedFromPath resolvedRun
                    for key in resolution.ResolutionPathKeys do
                        state.RunNodeCache <- state.RunNodeCache.Add(key, builtNodeId)
                    builtRun, builtNodeId
        | RunMixed _ ->
            // A file completes the process through $mixin and the decoder reads no file, so the graph cannot follow it.
            let nodeId =
                addProcessingUnitNode state runScope ProcessingUnitKind.ExternalReference "Process with a mixin" (Some stepNodeId) None None
            run, nodeId
        | RunRaw raw ->
            // A file decides the class of the process and the decoder reads no file, so the graph cannot follow it.
            let label = chooseLabel "Process with a directive as its class" [ raw.TryScalarField "id" |> Option.bind tryBasenameNoCwlExtension ]
            let nodeId = addProcessingUnitNode state runScope ProcessingUnitKind.ExternalReference label (Some stepNodeId) None None
            run, nodeId
        | RunCommandLineTool _ ->
            match WorkflowStepRunOps.tryGetTool run with
            | Some tool ->
                let nodeId =
                    buildProcessingUnit
                        state
                        runScope
                        workflowFilePath
                        (Some stepNodeId)
                        false
                        (CommandLineTool tool)
                run, nodeId
            | None ->
                let badNodeId =
                    addProcessingUnitNode
                        state
                        runScope
                        ProcessingUnitKind.UnresolvedReference
                        "Invalid CommandLineTool payload"
                        (Some stepNodeId)
                        None
                        None
                addDiagnostic
                    state
                    GraphIssueKind.UnexpectedRuntimeType
                    "WorkflowStepRun.RunCommandLineTool had an unexpected payload type."
                    workflowFilePath
                    None
                run, badNodeId
        | RunWorkflow _ ->
            match WorkflowStepRunOps.tryGetWorkflow run with
            | Some workflow ->
                let nestedWorkflowPath =
                    workflowFilePath
                    |> Option.defaultValue ""
                    |> fun currentPath ->
                        if System.String.IsNullOrWhiteSpace currentPath then None else Some currentPath
                let nodeId =
                    buildProcessingUnit
                        state
                        runScope
                        nestedWorkflowPath
                        (Some stepNodeId)
                        false
                        (Workflow workflow)
                run, nodeId
            | None ->
                let badNodeId =
                    addProcessingUnitNode
                        state
                        runScope
                        ProcessingUnitKind.UnresolvedReference
                        "Invalid Workflow payload"
                        (Some stepNodeId)
                        None
                        None
                addDiagnostic
                    state
                    GraphIssueKind.UnexpectedRuntimeType
                    "WorkflowStepRun.RunWorkflow had an unexpected payload type."
                    workflowFilePath
                    None
                run, badNodeId
        | RunExpressionTool _ ->
            match WorkflowStepRunOps.tryGetExpressionTool run with
            | Some expressionTool ->
                let nodeId =
                    buildProcessingUnit
                        state
                        runScope
                        workflowFilePath
                        (Some stepNodeId)
                        false
                        (ExpressionTool expressionTool)
                run, nodeId
            | None ->
                let badNodeId =
                    addProcessingUnitNode
                        state
                        runScope
                        ProcessingUnitKind.UnresolvedReference
                        "Invalid ExpressionTool payload"
                        (Some stepNodeId)
                        None
                        None
                addDiagnostic
                    state
                    GraphIssueKind.UnexpectedRuntimeType
                    "WorkflowStepRun.RunExpressionTool had an unexpected payload type."
                    workflowFilePath
                    None
                run, badNodeId
        | RunOperation _ ->
            match WorkflowStepRunOps.tryGetOperation run with
            | Some operation ->
                let nodeId =
                    buildProcessingUnit
                        state
                        runScope
                        workflowFilePath
                        (Some stepNodeId)
                        false
                        (Operation operation)
                run, nodeId
            | None ->
                let badNodeId =
                    addProcessingUnitNode
                        state
                        runScope
                        ProcessingUnitKind.UnresolvedReference
                        "Invalid Operation payload"
                        (Some stepNodeId)
                        None
                        None
                addDiagnostic
                    state
                    GraphIssueKind.UnexpectedRuntimeType
                    "WorkflowStepRun.RunOperation had an unexpected payload type."
                    workflowFilePath
                    None
                run, badNodeId

    /// Connects step input ports to their sources (workflow inputs or other step outputs).
    and wireStepInputs
        (state: BuildState)
        (scope: string)
        (workflowNodeId: WorkflowGraphNodeId)
        (workflowId: string option)
        (identity: StepIdentity)
        (step: WorkflowStep)
        =
        let stepNodeId = identity.NodeId

        for stepInput in WorkflowStep.getInputValues step do
            let targetInputPortId = GraphId.portNodeId stepNodeId PortDirection.Input (ReferenceParsing.relativeId step.Id stepInput.Id)
            match stepInput.Source with
            | None ->
                ()
            | Some sources ->
                // A directive names a file that the decoder does not read, so it gets no edge.
                for source in sources.AsValues() |> Seq.choose SchemaSaladString.tryLiteral do
                    let sourceRef = ReferenceParsing.parseSourceReference (ReferenceParsing.relativeId workflowId source)
                    if System.String.IsNullOrWhiteSpace sourceRef.PortId then
                        addDiagnostic
                            state
                            GraphIssueKind.InvalidReference
                            $"Step '{identity.Name}' input '{stepInput.Id}' has invalid source '{source}'."
                            (Some scope)
                            (Some source)
                    else
                        let sourcePortId =
                            match sourceRef.StepId with
                            | Some sourceStepId ->
                                GraphId.portNodeId
                                    (GraphId.stepNodeId scope sourceStepId)
                                    PortDirection.Output
                                    sourceRef.PortId
                            | None ->
                                GraphId.portNodeId workflowNodeId PortDirection.Input sourceRef.PortId

                        if hasNode state sourcePortId && hasNode state targetInputPortId then
                            let edgeKind =
                                match sourceRef.StepId with
                                | Some _ -> EdgeKind.DataFlow
                                | None -> EdgeKind.BindsWorkflowInput
                            addEdgeByType state edgeKind sourcePortId targetInputPortId None
                        else
                            addDiagnostic
                                state
                                GraphIssueKind.MissingReference
                                $"Unable to wire source '{source}' to step input '{identity.Name}/{stepInput.Id}'."
                                (Some scope)
                                (Some source)

    /// Connects workflow output ports to their sources (step outputs or workflow inputs).
    and wireWorkflowOutputs
        (state: BuildState)
        (scope: string)
        (workflowNodeId: WorkflowGraphNodeId)
        (workflow: CWLWorkflowDescription)
        =
        for output in CWLWorkflowDescription.getOutputValues workflow do
            let outputName = ReferenceParsing.relativeId workflow.Id output.Name
            let outputSources = output.GetOutputSources()
            for outputSource in outputSources do
                let parsedSource = ReferenceParsing.parseSourceReference (ReferenceParsing.relativeId workflow.Id outputSource)
                if System.String.IsNullOrWhiteSpace parsedSource.PortId then
                    addDiagnostic
                        state
                        GraphIssueKind.InvalidReference
                        $"Workflow output '{output.Name}' has invalid outputSource '{outputSource}'."
                        (Some scope)
                        (Some outputSource)
                else
                    let sourcePortId =
                        match parsedSource.StepId with
                        | Some sourceStepId ->
                            GraphId.portNodeId
                                (GraphId.stepNodeId scope sourceStepId)
                                PortDirection.Output
                                parsedSource.PortId
                        | None ->
                            GraphId.portNodeId workflowNodeId PortDirection.Input parsedSource.PortId

                    let targetPortId = GraphId.portNodeId workflowNodeId PortDirection.Output outputName
                    if hasNode state sourcePortId && hasNode state targetPortId then
                        addEdgeByType state EdgeKind.BindsWorkflowOutput sourcePortId targetPortId None
                    else
                        addDiagnostic
                            state
                            GraphIssueKind.MissingReference
                            $"Unable to wire workflow output '{output.Name}' from source '{outputSource}'."
                            (Some scope)
                            (Some outputSource)

    /// Iterates over all workflow steps, creating nodes, resolving run targets, and wiring data flows.
    and buildWorkflowSteps
        (state: BuildState)
        (scope: string)
        (workflowFilePath: string option)
        (workflowNodeId: WorkflowGraphNodeId)
        (workflow: CWLWorkflowDescription)
        =
        // The identity of a step comes from its place among the entries, so an import or a mixin between the steps keeps the
        // places of the others.
        let steps =
            workflow.Steps
            |> Seq.indexed
            |> Seq.choose (fun (position, entry) -> ListEntry.tryValue entry |> Option.map (fun step -> step, stepIdentity scope workflow.Id position step))
            |> Seq.toList
        for step, identity in steps do
            let stepNodeId = addStepNode state workflowNodeId identity step
            addStepPorts state stepNodeId step

            let runScope = GraphId.childScope scope identity.RunSegment
            let _, runNodeId = buildRunNode state runScope stepNodeId workflowFilePath (WorkflowStep.getResolvedRun step)
            addEdgeByType state EdgeKind.Calls stepNodeId runNodeId (Some "calls")

        steps
        |> List.iter (fun (step, identity) -> wireStepInputs state scope workflowNodeId workflow.Id identity step)

        wireWorkflowOutputs state scope workflowNodeId workflow

    /// <summary>
    /// Builds a WorkflowGraph from a CWLProcessingUnit using the specified build options.
    /// This is the primary entry point for graph construction with full control over scope,
    /// run resolution, nested workflow expansion, and strictness.
    /// </summary>
    /// <param name="options">Configuration controlling root scope, run resolution, nested workflow expansion, and strictness.</param>
    /// <param name="processingUnit">The CWL processing unit (Workflow, CommandLineTool, or ExpressionTool) to build the graph from.</param>
    let buildWith (options: WorkflowGraphBuildOptions) (processingUnit: CWLProcessingUnit) : WorkflowGraph =
        let rootScope = GraphId.normalizeSegment options.RootScope
        let rootScope = if System.String.IsNullOrWhiteSpace rootScope then "root" else rootScope
        let rootNodeId = GraphId.unitNodeId rootScope
        let state = createState options rootNodeId
        buildProcessingUnit state rootScope options.RootWorkflowFilePath None true processingUnit |> ignore
        state.Graph

    /// <summary>
    /// Builds a WorkflowGraph from a CWLProcessingUnit using default build options.
    /// Convenience wrapper around buildWith with WorkflowGraphBuildOptions.defaultOptions.
    /// </summary>
    /// <param name="processingUnit">The CWL processing unit to build the graph from.</param>
    let build (processingUnit: CWLProcessingUnit) : WorkflowGraph =
        buildWith WorkflowGraphBuildOptions.defaultOptions processingUnit
