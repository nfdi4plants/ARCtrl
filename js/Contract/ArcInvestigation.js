import { combineMany } from "../FileSystem/Path.js";
import { item, equalsWith } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { defaultOf } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { Contract } from "./Contract.js";
import { ARCtrl_ArcInvestigation__ArcInvestigation_fromFsWorkbook_Static_32154C9D, ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF } from "../Spreadsheet/ArcInvestigation.js";

export function $007CInvestigationPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 1)) {
        if (item(0, input) === "isa.investigation.xlsx") {
            matchResult = 0;
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
            return combineMany(input);
        default:
            return undefined;
    }
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToCreateContract(this$) {
    return Contract.createCreate("isa.investigation.xlsx", "ISA_Investigation", ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(this$));
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToUpdateContract(this$) {
    return Contract.createUpdate("isa.investigation.xlsx", "ISA_Investigation", ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(this$));
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_toCreateContract_Static_Z720BD3FF(inv) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_ToCreateContract(inv);
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_toUpdateContract_Static_Z720BD3FF(inv) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_ToUpdateContract(inv);
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_tryFromReadContract_Static_7570923F(c) {
    let matchResult, fsworkbook;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "ISA_Investigation") {
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
            return ARCtrl_ArcInvestigation__ArcInvestigation_fromFsWorkbook_Static_32154C9D(fsworkbook);
        default:
            return undefined;
    }
}

