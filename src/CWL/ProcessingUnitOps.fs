namespace ARCtrl.CWL

[<RequireQualifiedAccess>]
module ProcessingUnitOps =

    /// Returns the tool's inputs or an empty ResizeArray if None.
    let getToolInputsOrEmpty (tool: CWLToolDescription) =
        tool.Inputs |> Option.defaultValue (ResizeArray())

    /// Returns the expression tool's inputs or an empty ResizeArray if None.
    let getExpressionToolInputsOrEmpty (tool: CWLExpressionToolDescription) =
        tool.Inputs |> Option.defaultValue (ResizeArray())

    /// Returns the tool's inputs, creating and assigning a new empty ResizeArray if None.
    let getOrCreateToolInputs (tool: CWLToolDescription) =
        match tool.Inputs with
        | Some inputs -> inputs
        | None ->
            let inputs = ResizeArray()
            tool.Inputs <- Some inputs
            inputs

    /// Returns the expression tool's inputs, creating and assigning a new empty ResizeArray if None.
    let getOrCreateExpressionToolInputs (tool: CWLExpressionToolDescription) =
        match tool.Inputs with
        | Some inputs -> inputs
        | None ->
            let inputs = ResizeArray()
            tool.Inputs <- Some inputs
            inputs

    /// Returns normalized inputs for all CWL processing unit variants.
    let getInputs (processingUnit: CWLProcessingUnit) : ResizeArray<CWLInput> =
        match processingUnit with
        | CommandLineTool tool -> getToolInputsOrEmpty tool
        | Workflow workflow -> workflow.Inputs
        | ExpressionTool tool -> getExpressionToolInputsOrEmpty tool

    /// Returns outputs for all CWL processing unit variants.
    let getOutputs (processingUnit: CWLProcessingUnit) : ResizeArray<CWLOutput> =
        match processingUnit with
        | CommandLineTool tool -> tool.Outputs
        | Workflow workflow -> workflow.Outputs
        | ExpressionTool tool -> tool.Outputs

    /// Returns requirements for all CWL processing unit variants.
    let getRequirements (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        match processingUnit with
        | CommandLineTool tool -> tool.Requirements |> Option.defaultValue (ResizeArray())
        | Workflow workflow -> workflow.Requirements |> Option.defaultValue (ResizeArray())
        | ExpressionTool tool -> tool.Requirements |> Option.defaultValue (ResizeArray())

    /// Returns hints for all CWL processing unit variants.
    let getHints (processingUnit: CWLProcessingUnit) : ResizeArray<HintEntry> =
        match processingUnit with
        | CommandLineTool tool -> tool.Hints |> Option.defaultValue (ResizeArray())
        | Workflow workflow -> workflow.Hints |> Option.defaultValue (ResizeArray())
        | ExpressionTool tool -> tool.Hints |> Option.defaultValue (ResizeArray())

    /// Returns the tool's hints, creating and assigning a new empty ResizeArray if None.
    let getOrCreateToolHints (tool: CWLToolDescription) =
        match tool.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            tool.Hints <- Some hints
            hints

    /// Returns the expression tool's hints, creating and assigning a new empty ResizeArray if None.
    let getOrCreateExpressionToolHints (tool: CWLExpressionToolDescription) =
        match tool.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            tool.Hints <- Some hints
            hints

    /// Returns the workflow's hints, creating and assigning a new empty ResizeArray if None.
    let getOrCreateWorkflowHints (workflow: CWLWorkflowDescription) =
        match workflow.Hints with
        | Some hints -> hints
        | None ->
            let hints = ResizeArray()
            workflow.Hints <- Some hints
            hints

    /// Returns known requirement hints only, dropping unknown extension hints.
    let getKnownHints (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        getHints processingUnit
        |> Seq.choose HintEntry.tryAsRequirement
        |> ResizeArray
