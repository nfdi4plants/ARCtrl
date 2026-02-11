namespace ARCtrl.CWL

type CWLProcessingUnit =
    | CommandLineTool of CWLToolDescription
    | Workflow of CWLWorkflowDescription
    | ExpressionTool of CWLExpressionToolDescription

[<RequireQualifiedAccess>]
module WorkflowStepRunOps =

    let fromTool (tool: CWLToolDescription) : WorkflowStepRun =
        RunCommandLineTool tool

    let fromWorkflow (workflow: CWLWorkflowDescription) : WorkflowStepRun =
        RunWorkflow workflow

    let fromExpressionTool (expressionTool: CWLExpressionToolDescription) : WorkflowStepRun =
        RunExpressionTool expressionTool

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
