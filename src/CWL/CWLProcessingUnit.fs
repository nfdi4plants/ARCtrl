namespace ARCtrl.CWL

/// An id may start with a #, as in #main. The id is the same without it.
module internal LocalReference =

    let withoutHash (reference: string) : string =
        if reference.StartsWith "#" then reference.Substring 1 else reference

type CWLProcessingUnit =
    | CommandLineTool of CWLToolDescription
    | Workflow of CWLWorkflowDescription
    | ExpressionTool of CWLExpressionToolDescription
    | Operation of CWLOperationDescription
    /// A process whose class is a directive, as in class: {$import: path}. The file decides which of the four processes
    /// this is, and the decoder reads no file, so the mapping is kept as it was written. The inputs, outputs, requirements
    /// and hints of such a process are empty here, because the model cannot tell where they stand.
    | RawProcess of RawYaml

    /// Returns a new list of the inputs of the process that are objects, for every kind of process. A change of the list
    /// changes nothing in the process.
    static member getInputValues (processingUnit: CWLProcessingUnit) : ResizeArray<CWLInput> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getInputValuesOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getInputValues workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getInputValuesOrEmpty tool
        | Operation operation -> CWLOperationDescription.getInputValues operation
        | RawProcess _ -> ResizeArray()

    /// Returns a new list of the outputs of the process that are objects, for every kind of process. A change of the list
    /// changes nothing in the process.
    static member getOutputValues (processingUnit: CWLProcessingUnit) : ResizeArray<CWLOutput> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getOutputValues tool
        | Workflow workflow -> CWLWorkflowDescription.getOutputValues workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getOutputValues tool
        | Operation operation -> CWLOperationDescription.getOutputValues operation
        | RawProcess _ -> ResizeArray()

    /// Returns requirements for all CWL processing unit variants.
    static member getRequirements (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getRequirementsOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getRequirementsOrEmpty workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getRequirementsOrEmpty tool
        | Operation operation -> CWLOperationDescription.getRequirementsOrEmpty operation
        | RawProcess _ -> ResizeArray()

    /// Returns hints for all CWL processing unit variants.
    static member getHints (processingUnit: CWLProcessingUnit) : ResizeArray<HintEntry> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getHintsOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getHintsOrEmpty workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getHintsOrEmpty tool
        | Operation operation -> CWLOperationDescription.getHintsOrEmpty operation
        | RawProcess _ -> ResizeArray()

    /// Returns intent for all CWL processing unit variants.
    static member getIntent (processingUnit: CWLProcessingUnit) : ResizeArray<SchemaSaladString> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getIntentOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getIntentOrEmpty workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getIntentOrEmpty tool
        | Operation operation -> CWLOperationDescription.getIntentOrEmpty operation
        | RawProcess _ -> ResizeArray()

    /// Returns known requirement hints only, dropping unknown extension hints.
    static member getKnownHints (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        CWLProcessingUnit.getHints processingUnit
        |> Seq.choose HintEntry.tryAsRequirement
        |> ResizeArray

    /// True when both values wrap the same description object. Two union values around one object are the same process.
    static member isSameObject (left: CWLProcessingUnit) (right: CWLProcessingUnit) : bool =
        match left, right with
        | CommandLineTool leftTool, CommandLineTool rightTool -> System.Object.ReferenceEquals(leftTool, rightTool)
        | Workflow leftWorkflow, Workflow rightWorkflow -> System.Object.ReferenceEquals(leftWorkflow, rightWorkflow)
        | ExpressionTool leftTool, ExpressionTool rightTool -> System.Object.ReferenceEquals(leftTool, rightTool)
        | Operation leftOperation, Operation rightOperation -> System.Object.ReferenceEquals(leftOperation, rightOperation)
        | RawProcess leftRaw, RawProcess rightRaw -> System.Object.ReferenceEquals(leftRaw, rightRaw)
        | _ -> false

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

