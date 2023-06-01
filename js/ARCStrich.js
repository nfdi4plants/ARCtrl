import { Record, Union } from "./fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, union_type, string_type, unit_type } from "./fable_modules/fable-library.4.1.4/Reflection.js";

export class Contract_Data extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Spreadsheet", "Text"];
    }
}

export function Contract_Data_$reflection() {
    return union_type("ARC.API.Contract.Data", [], Contract_Data, () => [[["Item", unit_type]], [["Item", string_type]]]);
}

export class Contract_CRUD extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["CREATE", "READ", "UPDATE", "DELETE"];
    }
}

export function Contract_CRUD_$reflection() {
    return union_type("ARC.API.Contract.CRUD", [], Contract_CRUD, () => [[], [], [], []]);
}

export class Contract_ARCContract extends Record {
    constructor(Path, Data, CRUD) {
        super();
        this.Path = Path;
        this.Data = Data;
        this.CRUD = CRUD;
    }
}

export function Contract_ARCContract_$reflection() {
    return record_type("ARC.API.Contract.ARCContract", [], Contract_ARCContract, () => [["Path", string_type], ["Data", option_type(Contract_Data_$reflection())], ["CRUD", Contract_CRUD_$reflection()]]);
}

export function Assay_register(assay, arc) {
    throw new Error();
}

export function Assay_add(assay, arc) {
    Assay_register(assay, arc);
    throw new Error();
}

export const ARC_add = 1;

export const ARC_remove = 1;

export function ARC_addAssay(assay, arc) {
    throw new Error();
}

export function ARC_Assay_add(assay, arc) {
    throw new Error();
}

export function Study_addAssay(assay, arc) {
    throw new Error();
}

export function Study_Assay_add(assay, arc) {
    throw new Error();
}

export const Usage_a = 1;

export const Usage_s = 6354646;

export const Usage_ass = 2;

ARC_addAssay(Usage_ass, Usage_a);

Study_addAssay(Usage_s, Usage_a);

ARC_Assay_add(Usage_ass, Usage_a);

Study_Assay_add(Usage_s, Usage_a);

export function ARC1_diff(arc1, arc2) {
    throw new Error();
}

export function Assay2_add(assay, arc) {
    throw new Error();
}

