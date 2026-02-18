namespace ARCtrl.CWL

type CWLProcessingUnit =
    | CommandLineTool of CWLToolDescription
    | Workflow of CWLWorkflowDescription
    | ExpressionTool of CWLExpressionToolDescription
    | Operation of CWLOperationDescription

    /// Returns normalized inputs for all CWL processing unit variants.
    static member getInputs (processingUnit: CWLProcessingUnit) : ResizeArray<CWLInput> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getInputsOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getInputs workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getInputsOrEmpty tool
        | Operation operation -> CWLOperationDescription.getInputs operation

    /// Returns outputs for all CWL processing unit variants.
    static member getOutputs (processingUnit: CWLProcessingUnit) : ResizeArray<CWLOutput> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getOutputs tool
        | Workflow workflow -> CWLWorkflowDescription.getOutputs workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getOutputs tool
        | Operation operation -> CWLOperationDescription.getOutputs operation

    /// Returns requirements for all CWL processing unit variants.
    static member getRequirements (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getRequirementsOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getRequirementsOrEmpty workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getRequirementsOrEmpty tool
        | Operation operation -> CWLOperationDescription.getRequirementsOrEmpty operation

    /// Returns hints for all CWL processing unit variants.
    static member getHints (processingUnit: CWLProcessingUnit) : ResizeArray<HintEntry> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getHintsOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getHintsOrEmpty workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getHintsOrEmpty tool
        | Operation operation -> CWLOperationDescription.getHintsOrEmpty operation

    /// Returns known requirement hints only, dropping unknown extension hints.
    static member getKnownHints (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        CWLProcessingUnit.getHints processingUnit
        |> Seq.choose HintEntry.tryAsRequirement
        |> ResizeArray

[<RequireQualifiedAccess>]
module WorkflowStepRunOps =

    let fromTool (tool: CWLToolDescription) : WorkflowStepRun =
        RunCommandLineTool tool

    let fromWorkflow (workflow: CWLWorkflowDescription) : WorkflowStepRun =
        RunWorkflow workflow

    let fromExpressionTool (expressionTool: CWLExpressionToolDescription) : WorkflowStepRun =
        RunExpressionTool expressionTool

    let fromOperation (operation: CWLOperationDescription) : WorkflowStepRun =
        RunOperation operation

    let tryGetTool (run: WorkflowStepRun) : CWLToolDescription option =
        match run with
        | RunCommandLineTool (:? CWLToolDescription as tool) -> Some tool
        | _ -> None

    let tryGetWorkflow (run: WorkflowStepRun) : CWLWorkflowDescription option =
        match run with
        | RunWorkflow (:? CWLWorkflowDescription as workflow) -> Some workflow
        | _ -> None

    let tryGetExpressionTool (run: WorkflowStepRun) : CWLExpressionToolDescription option =
        match run with
        | RunExpressionTool (:? CWLExpressionToolDescription as expressionTool) -> Some expressionTool
        | _ -> None

    let tryGetOperation (run: WorkflowStepRun) : CWLOperationDescription option =
        match run with
        | RunOperation (:? CWLOperationDescription as operation) -> Some operation
        | _ -> None
