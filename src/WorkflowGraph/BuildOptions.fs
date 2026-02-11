namespace ARCtrl.WorkflowGraph

open ARCtrl.CWL

type WorkflowGraphBuildOptions = {
    RootScope: string
    RootWorkflowFilePath: string option
    TryResolveRunPath: (string -> CWLProcessingUnit option) option
    StrictUnresolvedRunReferences: bool
    ExpandNestedWorkflows: bool
}

[<RequireQualifiedAccess>]
module WorkflowGraphBuildOptions =

    let defaultOptions =
        {
            RootScope = "root"
            RootWorkflowFilePath = None
            TryResolveRunPath = None
            StrictUnresolvedRunReferences = false
            ExpandNestedWorkflows = true
        }

    let withRootScope (rootScope: string) (options: WorkflowGraphBuildOptions) =
        { options with RootScope = rootScope }

    let withRootWorkflowFilePath (rootWorkflowFilePath: string option) (options: WorkflowGraphBuildOptions) =
        { options with RootWorkflowFilePath = rootWorkflowFilePath }

    let withTryResolveRunPath (tryResolveRunPath: (string -> CWLProcessingUnit option) option) (options: WorkflowGraphBuildOptions) =
        { options with TryResolveRunPath = tryResolveRunPath }

    let withStrictUnresolvedRunReferences (strict: bool) (options: WorkflowGraphBuildOptions) =
        { options with StrictUnresolvedRunReferences = strict }

    let withExpandNestedWorkflows (expandNestedWorkflows: bool) (options: WorkflowGraphBuildOptions) =
        { options with ExpandNestedWorkflows = expandNestedWorkflows }
