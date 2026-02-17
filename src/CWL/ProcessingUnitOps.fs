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
