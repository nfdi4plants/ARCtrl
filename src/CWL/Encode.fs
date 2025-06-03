namespace ARCtrl.CWL

module Encode =

    /// Decode a CWL file string written in the YAML format into a CWLToolDescription
    let encodeCommandLineTool (clt : CWLToolDescription) : string =
        failwith "Encoding of CommandLineTool is not implemented yet"

    /// Decode a CWL file string written in the YAML format into a CWLWorkflowDescription
    let encodeWorkflow (wf : CWLWorkflowDescription) : string =
        failwith "Encoding of Workflow is not implemented yet"

    let encodeCWLProcessingUnit (cwl : CWLProcessingUnit) : string =
        match cwl with
        | CommandLineTool clt -> encodeCommandLineTool clt
        | Workflow wf -> encodeWorkflow wf