namespace ARCtrl.CWL

[<RequireQualifiedAccess>]
module ProcessingUnitOps =

    let getToolInputsOrEmpty (tool: CWLToolDescription) =
        tool.Inputs |> Option.defaultValue (ResizeArray())

    let getExpressionToolInputsOrEmpty (tool: CWLExpressionToolDescription) =
        tool.Inputs |> Option.defaultValue (ResizeArray())

    let getOrCreateToolInputs (tool: CWLToolDescription) =
        match tool.Inputs with
        | Some inputs -> inputs
        | None ->
            let inputs = ResizeArray()
            tool.Inputs <- Some inputs
            inputs

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
