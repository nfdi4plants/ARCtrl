module ARCtrl.Contract.ARC

open ARCtrl.ArcPathHelper

open ARCtrl.Contract

// Assumptions: 
// 1. All ISA .xlsx files MUST have the same names, either "isa.investigation.xlsx", "isa.assay.xlsx" or "isa.study.xlsx"
// 2. All ISA .xlsx files MUST be placed at the correct place. 
//      - isa.investigation.xlsx: MUST be in root
//      - isa.assay.xlsx: MUST be in root/assays/ANY_ASSAY_NAME/
//      - isa.study.xlsx: MUST be in root/studies/ANY_STUDY_NAME/
// 3. We ignore all other .xlsx files.
/// Tries to create READ contract with DTOType = ISA_Assay, ISA_Study or ISA_Investigation from given path relative to ARC root.
let tryISAReadContractFromPath (path: string) = 
    let split = split path
    match split with
    | InvestigationPath p -> 
        Some <| Contract.createRead(p, DTOType.ISA_Investigation) 
    | AssayPath p ->
        Some <| Contract.createRead(p, DTOType.ISA_Assay) 
    | StudyPath p ->
        Some <| Contract.createRead(p, DTOType.ISA_Study)
    | WorkflowPath p ->
        Some <| Contract.createRead(p, DTOType.ISA_Workflow)
    | WorkflowCWLPath p ->
        Some <| Contract.createRead(p, DTOType.CWL)
    | RunPath p ->
        Some <| Contract.createRead(p, DTOType.ISA_Run)
    | RunCWLPath p ->
        Some <| Contract.createRead(p, DTOType.CWL)
    | RunYMLPath p ->
        Some <| Contract.createRead(p, DTOType.YAML)
    | DatamapPath p ->
        Some <| Contract.createRead(p, DTOType.ISA_Datamap)
    | LicensePath p ->
        Some <| Contract.createRead(p, DTOType.PlainText)
    | anyElse -> 
        None

