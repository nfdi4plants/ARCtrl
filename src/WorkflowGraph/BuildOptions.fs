namespace ARCtrl.WorkflowGraph

open ARCtrl.CWL

/// Configuration record controlling how workflow graphs are built from CWL processing units.
type WorkflowGraphBuildOptions = {
    RootScope: string
    RootWorkflowFilePath: string option
    TryResolveRunPath: (string -> CWLProcessingUnit option) option
    StrictUnresolvedRunReferences: bool
    ExpandNestedWorkflows: bool
}

[<RequireQualifiedAccess>]
module WorkflowGraphBuildOptions =

    /// Default build options: root scope "root", no resolver, non-strict, expand nested workflows.
    let defaultOptions =
        {
            RootScope = "root"
            RootWorkflowFilePath = None
            TryResolveRunPath = None
            StrictUnresolvedRunReferences = false
            ExpandNestedWorkflows = true
        }

    /// <summary>
    /// Sets the root scope name used as the top-level graph identifier.
    /// </summary>
    /// <param name="rootScope">The root scope name.</param>
    /// <param name="options">The options to update.</param>
    let withRootScope (rootScope: string) (options: WorkflowGraphBuildOptions) =
        { options with RootScope = rootScope }

    /// <summary>
    /// Sets the file path of the root workflow CWL file, used for resolving relative run references.
    /// </summary>
    /// <param name="rootWorkflowFilePath">The optional file path of the root workflow.</param>
    /// <param name="options">The options to update.</param>
    let withRootWorkflowFilePath (rootWorkflowFilePath: string option) (options: WorkflowGraphBuildOptions) =
        { options with RootWorkflowFilePath = rootWorkflowFilePath }

    /// <summary>
    /// Sets the optional function used to resolve CWL run path references to CWLProcessingUnit instances.
    /// </summary>
    /// <param name="tryResolveRunPath">The optional resolver function that maps a path to a CWLProcessingUnit.</param>
    /// <param name="options">The options to update.</param>
    let withTryResolveRunPath (tryResolveRunPath: (string -> CWLProcessingUnit option) option) (options: WorkflowGraphBuildOptions) =
        { options with TryResolveRunPath = tryResolveRunPath }

    /// <summary>
    /// Sets whether unresolved run references should produce diagnostic warnings.
    /// When true, a missing resolver or unresolvable path generates a diagnostic issue.
    /// </summary>
    /// <param name="strict">If true, unresolved references are flagged.</param>
    /// <param name="options">The options to update.</param>
    let withStrictUnresolvedRunReferences (strict: bool) (options: WorkflowGraphBuildOptions) =
        { options with StrictUnresolvedRunReferences = strict }

    /// <summary>
    /// Sets whether nested workflows should be fully expanded in the graph.
    /// When true, steps in nested workflows are recursively built.
    /// </summary>
    /// <param name="expandNestedWorkflows">If true, nested workflows are expanded.</param>
    /// <param name="options">The options to update.</param>
    let withExpandNestedWorkflows (expandNestedWorkflows: bool) (options: WorkflowGraphBuildOptions) =
        { options with ExpandNestedWorkflows = expandNestedWorkflows }
