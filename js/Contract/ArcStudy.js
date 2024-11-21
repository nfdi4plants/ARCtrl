import { equalsWith, item } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { getStudyFolderPath, combineMany } from "../FileSystem/Path.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { unwrap, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { Study_fileNameFromIdentifier } from "../Core/Helper/Identifier.js";
import { Contract } from "./Contract.js";
import { ARCtrl_ArcStudy__ArcStudy_fromFsWorkbook_Static_32154C9D, ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522 } from "../Spreadsheet/ArcStudy.js";
import { empty, singleton, collect, append, delay, toArray } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { FileSystemTree } from "../FileSystem/FileSystemTree.js";

export function $007CStudyPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 3)) {
        if (item(0, input) === "studies") {
            if (item(2, input) === "isa.study.xlsx") {
                matchResult = 0;
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
            const anyStudyName = item(1, input);
            return combineMany(input);
        }
        default:
            return undefined;
    }
}

export function ARCtrl_ArcStudy__ArcStudy_ToCreateContract_6FCE9E49(this$, WithFolder) {
    const withFolder = defaultArg(WithFolder, false);
    const path = Study_fileNameFromIdentifier(this$.Identifier);
    const c = Contract.createCreate(path, "ISA_Study", ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(this$));
    return toArray(delay(() => {
        let folderFS;
        return append(withFolder ? ((folderFS = FileSystemTree.createStudiesFolder([FileSystemTree.createStudyFolder(this$.Identifier)]), collect((p) => (((p !== path) && (p !== "studies/.gitkeep")) ? singleton(Contract.createCreate(p, "PlainText")) : empty()), folderFS.ToFilePaths(false)))) : empty(), delay(() => singleton(c)));
    }));
}

export function ARCtrl_ArcStudy__ArcStudy_ToUpdateContract(this$) {
    const path = Study_fileNameFromIdentifier(this$.Identifier);
    return Contract.createUpdate(path, "ISA_Study", ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(this$));
}

export function ARCtrl_ArcStudy__ArcStudy_ToDeleteContract(this$) {
    const path = getStudyFolderPath(this$.Identifier);
    return Contract.createDelete(path);
}

export function ARCtrl_ArcStudy__ArcStudy_toDeleteContract_Static_1680536E(study) {
    return ARCtrl_ArcStudy__ArcStudy_ToDeleteContract(study);
}

export function ARCtrl_ArcStudy__ArcStudy_toCreateContract_Static_Z76BBA099(study, WithFolder) {
    return ARCtrl_ArcStudy__ArcStudy_ToCreateContract_6FCE9E49(study, unwrap(WithFolder));
}

export function ARCtrl_ArcStudy__ArcStudy_toUpdateContract_Static_1680536E(study) {
    return ARCtrl_ArcStudy__ArcStudy_ToUpdateContract(study);
}

export function ARCtrl_ArcStudy__ArcStudy_tryFromReadContract_Static_7570923F(c) {
    let matchResult, fsworkbook;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "ISA_Study") {
                if (c.DTO != null) {
                    matchResult = 0;
                    fsworkbook = c.DTO;
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
        case 0:
            return ARCtrl_ArcStudy__ArcStudy_fromFsWorkbook_Static_32154C9D(fsworkbook);
        default:
            return undefined;
    }
}

