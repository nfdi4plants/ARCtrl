namespace ARCtrl.CWL

type CWLDescription =
    | CommandLineTool of CWLToolDescription
    | Workflow of CWLWorkflowDescription