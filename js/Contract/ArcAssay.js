import { equalsWith, item } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { getAssayFolderPath, combineMany } from "../FileSystem/Path.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { unwrap, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { Assay_fileNameFromIdentifier } from "../Core/Helper/Identifier.js";
import { Contract } from "./Contract.js";
import { ARCtrl_ArcAssay__ArcAssay_fromFsWorkbook_Static_32154C9D, ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F } from "../Spreadsheet/ArcAssay.js";
import { empty, singleton, collect, append, delay, toArray } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { FileSystemTree } from "../FileSystem/FileSystemTree.js";

export function $007CAssayPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 3)) {
        if (item(0, input) === "assays") {
            if (item(2, input) === "isa.assay.xlsx") {
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
            const anyAssayName = item(1, input);
            return combineMany(input);
        }
        default:
            return undefined;
    }
}

export function ARCtrl_ArcAssay__ArcAssay_ToCreateContract_6FCE9E49(this$, WithFolder) {
    const withFolder = defaultArg(WithFolder, false);
    const path = Assay_fileNameFromIdentifier(this$.Identifier);
    const c = Contract.createCreate(path, "ISA_Assay", ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(this$));
    return toArray(delay(() => {
        let folderFS;
        return append(withFolder ? ((folderFS = FileSystemTree.createAssaysFolder([FileSystemTree.createAssayFolder(this$.Identifier)]), collect((p) => (((p !== path) && (p !== "assays/.gitkeep")) ? singleton(Contract.createCreate(p, "PlainText")) : empty()), folderFS.ToFilePaths(false)))) : empty(), delay(() => singleton(c)));
    }));
}

export function ARCtrl_ArcAssay__ArcAssay_ToUpdateContract(this$) {
    const path = Assay_fileNameFromIdentifier(this$.Identifier);
    return Contract.createUpdate(path, "ISA_Assay", ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(this$));
}

export function ARCtrl_ArcAssay__ArcAssay_ToDeleteContract(this$) {
    const path = getAssayFolderPath(this$.Identifier);
    return Contract.createDelete(path);
}

export function ARCtrl_ArcAssay__ArcAssay_toDeleteContract_Static_1501C0F8(assay) {
    return ARCtrl_ArcAssay__ArcAssay_ToDeleteContract(assay);
}

export function ARCtrl_ArcAssay__ArcAssay_toCreateContract_Static_Z2508BE4F(assay, WithFolder) {
    return ARCtrl_ArcAssay__ArcAssay_ToCreateContract_6FCE9E49(assay, unwrap(WithFolder));
}

export function ARCtrl_ArcAssay__ArcAssay_toUpdateContract_Static_1501C0F8(assay) {
    return ARCtrl_ArcAssay__ArcAssay_ToUpdateContract(assay);
}

export function ARCtrl_ArcAssay__ArcAssay_tryFromReadContract_Static_7570923F(c) {
    let matchResult, fsworkbook;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "ISA_Assay") {
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
            return ARCtrl_ArcAssay__ArcAssay_fromFsWorkbook_Static_32154C9D(fsworkbook);
        default:
            return undefined;
    }
}

