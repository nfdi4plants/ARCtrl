/// This module contains helper functions to handle study/assay/investigation identifiers in an unsafe, forced way.
module ISA.IdentifierHandler 

let setAssayIdentifier (newIdentifier: string) (assay: ArcAssay) =
    assay.Identifier <- newIdentifier
    assay

let setStudyIdentifier (newIdentifier: string) (study: ArcStudy) =
    study.Identifier <- newIdentifier
    study

let setInvestigationIdentifier (newIdentifier: string) (investigation: ArcInvestigation) =
    investigation.Identifier <- newIdentifier
    investigation



