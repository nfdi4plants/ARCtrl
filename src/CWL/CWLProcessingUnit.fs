namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

/// A reference to a name in the same document starts with a #, as in #main. The name is the same without it.
module internal LocalReference =

    let withoutHash (reference: string) : string =
        if reference.StartsWith "#" then reference.Substring 1 else reference

/// How a packed document finds a process, as cwltool does. A runner starts with the first process whose id is main. A
/// run of the form #id reaches the last process with that id, because a later object of one id replaces the earlier ones.
/// The model and the decoder find the processes of a document through these two functions only. The ids are the ids of
/// the processes in the order of the document, with none for a process without an id.
module GraphMembers =

    let private positionsOf (wanted: string) (ids: seq<string option>) : int list =
        ids
        |> Seq.indexed
        |> Seq.filter (fun (_, id) -> id |> Option.map LocalReference.withoutHash = Some wanted)
        |> Seq.map fst
        |> List.ofSeq

    /// The position of the process that a runner starts with.
    let tryStart (ids: seq<string option>) : int option =
        positionsOf "main" ids |> List.tryHead

    /// The position of the process that a run of the form #id reaches. The id may start with a #.
    let tryReach (id: string) (ids: seq<string option>) : int option =
        positionsOf (LocalReference.withoutHash id) ids |> List.tryLast

type CWLProcessingUnit =
    | CommandLineTool of CWLToolDescription
    | Workflow of CWLWorkflowDescription
    | ExpressionTool of CWLExpressionToolDescription
    | Operation of CWLOperationDescription

    /// Returns a new list of the inputs of the process that are objects, for every kind of process. A change of the list
    /// changes nothing in the process.
    static member getInputValues (processingUnit: CWLProcessingUnit) : ResizeArray<CWLInput> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getInputValuesOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getInputValues workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getInputValuesOrEmpty tool
        | Operation operation -> CWLOperationDescription.getInputValues operation

    /// Returns a new list of the outputs of the process that are objects, for every kind of process. A change of the list
    /// changes nothing in the process.
    static member getOutputValues (processingUnit: CWLProcessingUnit) : ResizeArray<CWLOutput> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getOutputValues tool
        | Workflow workflow -> CWLWorkflowDescription.getOutputValues workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getOutputValues tool
        | Operation operation -> CWLOperationDescription.getOutputValues operation

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

    /// Returns intent for all CWL processing unit variants.
    static member getIntent (processingUnit: CWLProcessingUnit) : ResizeArray<SchemaSaladString> =
        match processingUnit with
        | CommandLineTool tool -> CWLToolDescription.getIntentOrEmpty tool
        | Workflow workflow -> CWLWorkflowDescription.getIntentOrEmpty workflow
        | ExpressionTool tool -> CWLExpressionToolDescription.getIntentOrEmpty tool
        | Operation operation -> CWLOperationDescription.getIntentOrEmpty operation

    /// Returns known requirement hints only, dropping unknown extension hints.
    static member getKnownHints (processingUnit: CWLProcessingUnit) : ResizeArray<Requirement> =
        CWLProcessingUnit.getHints processingUnit
        |> Seq.choose HintEntry.tryAsRequirement
        |> ResizeArray

    /// Returns the id of the process of any variant, as it is written.
    static member tryGetId (processingUnit: CWLProcessingUnit) : string option =
        match processingUnit with
        | CommandLineTool tool -> tool.Id
        | Workflow workflow -> workflow.Id
        | ExpressionTool tool -> tool.Id
        | Operation operation -> operation.Id

    /// True when both values wrap the same description object. Two union values around one object are the same process.
    static member isSameObject (left: CWLProcessingUnit) (right: CWLProcessingUnit) : bool =
        match left, right with
        | CommandLineTool leftTool, CommandLineTool rightTool -> System.Object.ReferenceEquals(leftTool, rightTool)
        | Workflow leftWorkflow, Workflow rightWorkflow -> System.Object.ReferenceEquals(leftWorkflow, rightWorkflow)
        | ExpressionTool leftTool, ExpressionTool rightTool -> System.Object.ReferenceEquals(leftTool, rightTool)
        | Operation leftOperation, Operation rightOperation -> System.Object.ReferenceEquals(leftOperation, rightOperation)
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


/// A packed CWL document. It has no process at its root. The processes stand under $graph and all use the cwlVersion
/// of the root. A step of a workflow refers to another process of the graph by its id, as in run: "#tool". The other
/// keys of the root, such as $namespaces and $schemas, are dynamic properties of the graph.
[<AttachMembers>]
type CWLGraph(processes: ResizeArray<CWLProcessingUnit>, ?cwlVersion: string) =
    inherit DynamicObj()

    let mutable _cwlVersion: string = cwlVersion |> Option.defaultValue "v1.2"
    let mutable _processes: ResizeArray<CWLProcessingUnit> = processes

    member this.CWLVersion
        with get() = _cwlVersion
        and set(value) = _cwlVersion <- value

    member this.Processes
        with get() = _processes
        and set(value) = _processes <- value

    static member KnownFieldNames =
        CWLKnownFieldNames.graph

    static member private idsOf (graph: CWLGraph) : seq<string option> =
        graph.Processes |> Seq.map CWLProcessingUnit.tryGetId

    /// Returns the process that a run of the form #id reaches: the last process with the id. The id of a process and the
    /// id that is looked up may start with a #.
    static member tryFindProcess (id: string) (graph: CWLGraph) : CWLProcessingUnit option =
        GraphMembers.tryReach id (CWLGraph.idsOf graph) |> Option.map (fun position -> graph.Processes.[position])

    /// Returns the process main of the graph, which a runner starts with: the first process with the id main.
    static member tryFindMain (graph: CWLGraph) : CWLProcessingUnit option =
        GraphMembers.tryStart (CWLGraph.idsOf graph) |> Option.map (fun position -> graph.Processes.[position])

    /// True when the process is the process main of the graph, the object that tryFindMain finds. The id is read on each
    /// call, so renaming a process in place changes the answer.
    static member isMain (processingUnit: CWLProcessingUnit) (graph: CWLGraph) : bool =
        CWLGraph.tryFindMain graph |> Option.exists (CWLProcessingUnit.isSameObject processingUnit)

    override this.Equals(o: obj) =
        match o with
        | :? CWLGraph as other ->
            this.CWLVersion = other.CWLVersion
            && HashHelpers.seqEqual this.Processes other.Processes
            && DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.CWLVersion, HashHelpers.hashSeq this.Processes, DynamicObjHelpers.hashDynamicProperties this)

/// A CWL document: a single process, or a packed document with a graph of processes.
[<RequireQualifiedAccess>]
type CWLDocument =
    | Process of CWLProcessingUnit
    | Graph of CWLGraph

    /// Returns the process that a runner starts with: the process of a document with one process, or the process with
    /// the id main of a packed document. A packed document without that id has none.
    static member tryGetMain (document: CWLDocument) : CWLProcessingUnit option =
        match document with
        | Process processingUnit -> Some processingUnit
        | Graph graph -> CWLGraph.tryFindMain graph
