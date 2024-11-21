import { ActivePatterns_$007CRegex$007C_$007C } from "./Regex.js";
import { newGuid } from "../../fable_modules/fable-library-js.4.22.0/Guid.js";
import { combineMany } from "../../FileSystem/Path.js";

export function tryCheckValidCharacters(identifier) {
    if (ActivePatterns_$007CRegex$007C_$007C("^[a-zA-Z0-9_\\- ]+$", identifier) != null) {
        return true;
    }
    else {
        return false;
    }
}

export function checkValidCharacters(identifier) {
    if (tryCheckValidCharacters(identifier)) {
    }
    else {
        throw new Error(`New identifier "${identifier}"contains forbidden characters! Allowed characters are: letters, digits, underscore (_), dash (-) and whitespace ( ).`);
    }
}

export function createMissingIdentifier() {
    let copyOfStruct;
    return "MISSING_IDENTIFIER_" + ((copyOfStruct = newGuid(), copyOfStruct));
}

export function isMissingIdentifier(str) {
    return str.startsWith("MISSING_IDENTIFIER_");
}

export function removeMissingIdentifier(str) {
    if (str.startsWith("MISSING_IDENTIFIER_")) {
        return "";
    }
    else {
        return str;
    }
}

/**
 * On read-in the FileName can be any combination of "assays" (assay folder name), assayIdentifier and "isa.assay.xlsx" (the actual file name).
 * 
 * This functions extracts assayIdentifier from any of these combinations and returns it.
 */
export function Assay_identifierFromFileName(fileName) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("^(assays(\\/|\\\\))?(?<identifier>[a-zA-Z0-9_\\- ]+)((\\/|\\\\)isa.assay.xlsx)?$", fileName);
    if (activePatternResult != null) {
        const m = activePatternResult;
        return (m.groups && m.groups.identifier) || "";
    }
    else {
        throw new Error(`Cannot parse assay identifier from FileName \`${fileName}\``);
    }
}

/**
 * On read-in the FileName can be any combination of "assays" (assay folder name), assayIdentifier and "isa.assay.xlsx" (the actual file name).
 * 
 * This functions extracts assayIdentifier from any of these combinations and returns it.
 */
export function Assay_tryIdentifierFromFileName(fileName) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("^(assays(\\/|\\\\))?(?<identifier>[a-zA-Z0-9_\\- ]+)((\\/|\\\\)isa.assay.xlsx)?$", fileName);
    if (activePatternResult != null) {
        const m = activePatternResult;
        return (m.groups && m.groups.identifier) || "";
    }
    else {
        return undefined;
    }
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.assay.xlsx`.
 */
export function Assay_fileNameFromIdentifier(identifier) {
    checkValidCharacters(identifier);
    return combineMany(["assays", identifier, "isa.assay.xlsx"]);
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.assay.xlsx`.
 */
export function Assay_tryFileNameFromIdentifier(identifier) {
    if (tryCheckValidCharacters(identifier)) {
        return combineMany(["assays", identifier, "isa.assay.xlsx"]);
    }
    else {
        return undefined;
    }
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.datamap.xlsx`.
 */
export function Assay_datamapFileNameFromIdentifier(identifier) {
    checkValidCharacters(identifier);
    return combineMany(["assays", identifier, "isa.datamap.xlsx"]);
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `assays/assayIdentifier/isa.datamap.xlsx`.
 */
export function Assay_tryDatamapFileNameFromIdentifier(identifier) {
    if (tryCheckValidCharacters(identifier)) {
        return combineMany(["assays", identifier, "isa.datamap.xlsx"]);
    }
    else {
        return undefined;
    }
}

/**
 * On read-in the FileName can be any combination of "studies" (study folder name), studyIdentifier and "isa.study.xlsx" (the actual file name).
 * 
 * This functions extracts studyIdentifier from any of these combinations and returns it.
 */
export function Study_identifierFromFileName(fileName) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("^(studies(\\/|\\\\))?(?<identifier>[a-zA-Z0-9_\\- ]+)((\\/|\\\\)isa.study.xlsx)?$", fileName);
    if (activePatternResult != null) {
        const m = activePatternResult;
        return (m.groups && m.groups.identifier) || "";
    }
    else {
        throw new Error(`Cannot parse study identifier from FileName \`${fileName}\``);
    }
}

/**
 * On read-in the FileName can be any combination of "studies" (study folder name), studyIdentifier and "isa.study.xlsx" (the actual file name).
 * 
 * This functions extracts studyIdentifier from any of these combinations and returns it.
 */
export function Study_tryIdentifierFromFileName(fileName) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("^(studies(\\/|\\\\))?(?<identifier>[a-zA-Z0-9_\\- ]+)((\\/|\\\\)isa.study.xlsx)?$", fileName);
    if (activePatternResult != null) {
        const m = activePatternResult;
        return (m.groups && m.groups.identifier) || "";
    }
    else {
        return undefined;
    }
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.study.xlsx`.
 */
export function Study_fileNameFromIdentifier(identifier) {
    checkValidCharacters(identifier);
    return combineMany(["studies", identifier, "isa.study.xlsx"]);
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.study.xlsx`.
 */
export function Study_tryFileNameFromIdentifier(identifier) {
    if (tryCheckValidCharacters(identifier)) {
        return combineMany(["studies", identifier, "isa.study.xlsx"]);
    }
    else {
        return undefined;
    }
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.investigation.xlsx`.
 */
export function Study_datamapFileNameFromIdentifier(identifier) {
    checkValidCharacters(identifier);
    return combineMany(["studies", identifier, "isa.datamap.xlsx"]);
}

/**
 * On writing a xlsx file we unify our output to a relative path to ARC root. So: `studies/studyIdentifier/isa.investigation.xlsx`.
 */
export function Study_tryDatamapFileNameFromIdentifier(identifier) {
    if (tryCheckValidCharacters(identifier)) {
        return combineMany(["studies", identifier, "isa.datamap.xlsx"]);
    }
    else {
        return undefined;
    }
}

