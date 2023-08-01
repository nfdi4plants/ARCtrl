import { int32 } from "../fable_modules/fable-library-ts/Int32.js";
import { FsWorkbook } from "../fable_modules/FsSpreadsheet.3.1.1/FsWorkbook.fs.js";
import { Option, value } from "../fable_modules/fable-library-ts/Option.js";
import { Contract } from "../Contract/Contract.js";
import { combineMany } from "../FileSystem/Path.js";
import { equalsWith } from "../fable_modules/fable-library-ts/Array.js";
import { defaultOf } from "../fable_modules/fable-library-ts/Util.js";

export function tryFromContract(c: Contract): Option<FsWorkbook> {
    let matchResult: int32, fsworkbook: FsWorkbook;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (value(c.DTOType) === "ISA_Investigation") {
                if (c.DTO != null) {
                    if (value(c.DTO) instanceof FsWorkbook) {
                        matchResult = 0;
                        fsworkbook = value(c.DTO);
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
        case 0:
            return fsworkbook!;
        default:
            return void 0;
    }
}

export function $007CInvestigationPath$007C_$007C(input: string[]): Option<string> {
    let matchResult: int32;
    if (!equalsWith((x: string, y: string): boolean => (x === y), input, defaultOf()) && (input.length === 1)) {
        if (input[0] === "isa.investigation.xlsx") {
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
            return void 0;
    }
}

