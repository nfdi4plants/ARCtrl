import { FsWorkbook } from "../fable_modules/FsSpreadsheet.3.1.1/FsWorkbook.fs.js";
import { combineMany } from "../FileSystem/Path.js";
import { equalsWith } from "../fable_modules/fable-library.4.1.4/Array.js";
import { defaultOf } from "../fable_modules/fable-library.4.1.4/Util.js";

export function tryFromContract(c) {
    let matchResult, fsworkbook;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (c.DTOType === "ISA_Assay") {
                if (c.DTO != null) {
                    if (c.DTO instanceof FsWorkbook) {
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
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return fsworkbook;
        default:
            return void 0;
    }
}

export function $007CAssayPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 3)) {
        if (input[0] === "assays") {
            if (input[2] === "isa.assay.xlsx") {
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
            const anyAssayName = input[1];
            return combineMany(input);
        }
        default:
            return void 0;
    }
}

