import { checkValidCharacters } from "./Identifier.js";
import { ArcAssay } from "./ArcAssay.js";
import { ArcStudy } from "./ArcStudy.js";
import { ArcInvestigation } from "./ArcInvestigation.js";

export function setAssayIdentifier(newIdentifier: string, assay: ArcAssay): ArcAssay {
    checkValidCharacters(newIdentifier);
    assay.Identifier = newIdentifier;
    return assay;
}

export function setStudyIdentifier(newIdentifier: string, study: ArcStudy): ArcStudy {
    checkValidCharacters(newIdentifier);
    study.Identifier = newIdentifier;
    return study;
}

export function setInvestigationIdentifier(newIdentifier: string, investigation: ArcInvestigation): ArcInvestigation {
    checkValidCharacters(newIdentifier);
    investigation.Identifier = newIdentifier;
    return investigation;
}

