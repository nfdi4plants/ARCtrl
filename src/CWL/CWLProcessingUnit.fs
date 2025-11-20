namespace ARCtrl.CWL

type CWLProcessingUnit =
    | CommandLineTool of CWLToolDescription
    | Workflow of CWLWorkflowDescription