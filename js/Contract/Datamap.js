import { equalsWith, item } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { combineMany } from "../FileSystem/Path.js";
import { safeHash, defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { Study_datamapFileNameFromIdentifier, Assay_datamapFileNameFromIdentifier } from "../Core/Helper/Identifier.js";
import { Contract } from "./Contract.js";
import { fromFsWorkbook, toFsWorkbook } from "../Spreadsheet/DataMap.js";
import { DataMap__set_StaticHash_Z524259A4 } from "../Core/DataMap.js";

export function $007CDatamapPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 3)) {
        if (item(0, input) === "assays") {
            if (item(2, input) === "isa.datamap.xlsx") {
                matchResult = 0;
            }
            else {
                matchResult = 2;
            }
        }
        else if (item(0, input) === "studies") {
            if (item(2, input) === "isa.datamap.xlsx") {
                matchResult = 1;
            }
            else {
                matchResult = 2;
            }
        }
        else {
            matchResult = 2;
        }
    }
    else {
        matchResult = 2;
    }
    switch (matchResult) {
        case 0: {
            const anyAssayName = item(1, input);
            return combineMany(input);
        }
        case 1: {
            const anyStudyName = item(1, input);
            return combineMany(input);
        }
        default:
            return undefined;
    }
}

export function ARCtrl_DataMap__DataMap_ToCreateContractForAssay_Z721C83C5(this$, assayIdentifier) {
    const path = Assay_datamapFileNameFromIdentifier(assayIdentifier);
    return Contract.createCreate(path, "ISA_Datamap", toFsWorkbook(this$));
}

export function ARCtrl_DataMap__DataMap_ToUpdateContractForAssay_Z721C83C5(this$, assayIdentifier) {
    const path = Assay_datamapFileNameFromIdentifier(assayIdentifier);
    return Contract.createUpdate(path, "ISA_Datamap", toFsWorkbook(this$));
}

export function ARCtrl_DataMap__DataMap_ToDeleteContractForAssay_Z721C83C5(this$, assayIdentifier) {
    const path = Assay_datamapFileNameFromIdentifier(assayIdentifier);
    return Contract.createDelete(path);
}

export function ARCtrl_DataMap__DataMap_toDeleteContractForAssay_Static_Z721C83C5(assayIdentifier) {
    return (dataMap) => ARCtrl_DataMap__DataMap_ToDeleteContractForAssay_Z721C83C5(dataMap, assayIdentifier);
}

export function ARCtrl_DataMap__DataMap_toUpdateContractForAssay_Static_Z721C83C5(assayIdentifier) {
    return (dataMap) => ARCtrl_DataMap__DataMap_ToUpdateContractForAssay_Z721C83C5(dataMap, assayIdentifier);
}

export function ARCtrl_DataMap__DataMap_tryFromReadContractForAssay_Static(assayIdentifier, c) {
    let fsworkbook;
    const path = Assay_datamapFileNameFromIdentifier(assayIdentifier);
    let matchResult, fsworkbook_1, p_1;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "ISA_Datamap") {
                if (c.DTO != null) {
                    if ((fsworkbook = c.DTO, c.Path === path)) {
                        matchResult = 0;
                        fsworkbook_1 = c.DTO;
                        p_1 = c.Path;
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0: {
            const dm = fromFsWorkbook(fsworkbook_1);
            DataMap__set_StaticHash_Z524259A4(dm, safeHash(dm));
            return dm;
        }
        default:
            return undefined;
    }
}

export function ARCtrl_DataMap__DataMap_ToCreateContractForStudy_Z721C83C5(this$, studyIdentifier) {
    const path = Study_datamapFileNameFromIdentifier(studyIdentifier);
    return Contract.createCreate(path, "ISA_Datamap", toFsWorkbook(this$));
}

export function ARCtrl_DataMap__DataMap_ToUpdateContractForStudy_Z721C83C5(this$, studyIdentifier) {
    const path = Study_datamapFileNameFromIdentifier(studyIdentifier);
    return Contract.createUpdate(path, "ISA_Datamap", toFsWorkbook(this$));
}

export function ARCtrl_DataMap__DataMap_ToDeleteContractForStudy_Z721C83C5(this$, studyIdentifier) {
    const path = Study_datamapFileNameFromIdentifier(studyIdentifier);
    return Contract.createDelete(path);
}

export function ARCtrl_DataMap__DataMap_toDeleteContractForStudy_Static_Z721C83C5(studyIdentifier) {
    return (dataMap) => ARCtrl_DataMap__DataMap_ToDeleteContractForStudy_Z721C83C5(dataMap, studyIdentifier);
}

export function ARCtrl_DataMap__DataMap_toUpdateContractForStudy_Static_Z721C83C5(studyIdentifier) {
    return (dataMap) => ARCtrl_DataMap__DataMap_ToUpdateContractForStudy_Z721C83C5(dataMap, studyIdentifier);
}

export function ARCtrl_DataMap__DataMap_tryFromReadContractForStudy_Static(studyIdentifier, c) {
    let fsworkbook;
    const path = Study_datamapFileNameFromIdentifier(studyIdentifier);
    let matchResult, fsworkbook_1, p_1;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "ISA_Datamap") {
                if (c.DTO != null) {
                    if ((fsworkbook = c.DTO, c.Path === path)) {
                        matchResult = 0;
                        fsworkbook_1 = c.DTO;
                        p_1 = c.Path;
                    }
                    else {
                        matchResult = 1;
                    }
                }
                else {
                    matchResult = 1;
                }
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0: {
            const dm = fromFsWorkbook(fsworkbook_1);
            DataMap__set_StaticHash_Z524259A4(dm, safeHash(dm));
            return dm;
        }
        default:
            return undefined;
    }
}

