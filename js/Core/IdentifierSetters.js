import { checkValidCharacters } from "./Helper/Identifier.js";

export function setArcTableName(newName, table) {
    checkValidCharacters(newName);
    table.Name = newName;
    return table;
}

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

