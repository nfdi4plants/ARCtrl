import { checkValidCharacters } from "./Identifier.js";

export function setAssayIdentifier(newIdentifier, assay) {
    checkValidCharacters(newIdentifier);
    assay.Identifier = newIdentifier;
    return assay;
}

export function setStudyIdentifier(newIdentifier, study) {
    checkValidCharacters(newIdentifier);
    study.Identifier = newIdentifier;
    return study;
}

export function setInvestigationIdentifier(newIdentifier, investigation) {
    checkValidCharacters(newIdentifier);
    investigation.Identifier = newIdentifier;
    return investigation;
}

