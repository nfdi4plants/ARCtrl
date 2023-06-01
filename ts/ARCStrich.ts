import { Record, Union } from "./fable_modules/fable-library-ts/Types.js";
import { record_type, option_type, union_type, string_type, unit_type, TypeInfo } from "./fable_modules/fable-library-ts/Reflection.js";
import { Option } from "./fable_modules/fable-library-ts/Option.js";
import { IComparable, IEquatable } from "./fable_modules/fable-library-ts/Util.js";
import { Assay } from "./ISA/Assay.js";
import { ARC } from "./ARC.js";
import { FSharpList } from "./fable_modules/fable-library-ts/List.js";
import { int32 } from "./fable_modules/fable-library-ts/Int32.js";

export type Contract_Data_$union = 
    | Contract_Data<0>
    | Contract_Data<1>

export type Contract_Data_$cases = {
    0: ["Spreadsheet", [void]],
    1: ["Text", [string]]
}

export function Contract_Data_Spreadsheet(Item: void) {
    return new Contract_Data<0>(0, [Item]);
}

export function Contract_Data_Text(Item: string) {
    return new Contract_Data<1>(1, [Item]);
}

export class Contract_Data<Tag extends keyof Contract_Data_$cases> extends Union<Tag, Contract_Data_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: Contract_Data_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Spreadsheet", "Text"];
    }
}

export function Contract_Data_$reflection(): TypeInfo {
    return union_type("ARC.API.Contract.Data", [], Contract_Data, () => [[["Item", unit_type]], [["Item", string_type]]]);
}

export type Contract_CRUD_$union = 
    | Contract_CRUD<0>
    | Contract_CRUD<1>
    | Contract_CRUD<2>
    | Contract_CRUD<3>

export type Contract_CRUD_$cases = {
    0: ["CREATE", []],
    1: ["READ", []],
    2: ["UPDATE", []],
    3: ["DELETE", []]
}

export function Contract_CRUD_CREATE() {
    return new Contract_CRUD<0>(0, []);
}

export function Contract_CRUD_READ() {
    return new Contract_CRUD<1>(1, []);
}

export function Contract_CRUD_UPDATE() {
    return new Contract_CRUD<2>(2, []);
}

export function Contract_CRUD_DELETE() {
    return new Contract_CRUD<3>(3, []);
}

export class Contract_CRUD<Tag extends keyof Contract_CRUD_$cases> extends Union<Tag, Contract_CRUD_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: Contract_CRUD_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["CREATE", "READ", "UPDATE", "DELETE"];
    }
}

export function Contract_CRUD_$reflection(): TypeInfo {
    return union_type("ARC.API.Contract.CRUD", [], Contract_CRUD, () => [[], [], [], []]);
}

export class Contract_ARCContract extends Record implements IEquatable<Contract_ARCContract>, IComparable<Contract_ARCContract> {
    readonly Path: string;
    readonly Data: Option<Contract_Data_$union>;
    readonly CRUD: Contract_CRUD_$union;
    constructor(Path: string, Data: Option<Contract_Data_$union>, CRUD: Contract_CRUD_$union) {
        super();
        this.Path = Path;
        this.Data = Data;
        this.CRUD = CRUD;
    }
}

export function Contract_ARCContract_$reflection(): TypeInfo {
    return record_type("ARC.API.Contract.ARCContract", [], Contract_ARCContract, () => [["Path", string_type], ["Data", option_type(Contract_Data_$reflection())], ["CRUD", Contract_CRUD_$reflection()]]);
}

export function Assay_register(assay: Assay, arc: ARC): [ARC, FSharpList<Contract_ARCContract>] {
    throw new Error();
}

export function Assay_add(assay: Assay, arc: ARC): [ARC, FSharpList<Contract_ARCContract>] {
    Assay_register(assay, arc);
    throw new Error();
}

export const ARC_add = 1;

export const ARC_remove = 1;

export function ARC_addAssay<$a, $b, $c>(assay: $a, arc: $b): $c {
    throw new Error();
}

export function ARC_Assay_add<$a, $b, $c>(assay: $a, arc: $b): $c {
    throw new Error();
}

export function Study_addAssay<$a, $b, $c>(assay: $a, arc: $b): $c {
    throw new Error();
}

export function Study_Assay_add<$a, $b, $c>(assay: $a, arc: $b): $c {
    throw new Error();
}

export const Usage_a = 1;

export const Usage_s = 6354646;

export const Usage_ass = 2;

ARC_addAssay<int32, int32, void>(Usage_ass, Usage_a);

Study_addAssay<int32, int32, void>(Usage_s, Usage_a);

ARC_Assay_add<int32, int32, void>(Usage_ass, Usage_a);

Study_Assay_add<int32, int32, void>(Usage_s, Usage_a);

export function ARC1_diff(arc1: ARC, arc2: ARC): FSharpList<Contract_ARCContract> {
    throw new Error();
}

export function Assay2_add(assay: Assay, arc: ARC): FSharpList<Contract_ARCContract> {
    throw new Error();
}

