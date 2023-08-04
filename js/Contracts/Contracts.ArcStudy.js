import { some } from "../fable_modules/fable-library.4.1.4/Option.js";
import { combineMany } from "../FileSystem/Path.js";
import { equalsWith } from "../fable_modules/fable-library.4.1.4/Array.js";
import { defaultOf } from "../fable_modules/fable-library.4.1.4/Util.js";

export function tryFromContract(c) {
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
            return some(fsworkbook);
        default:
            return void 0;
    }
}

export function $007CStudyPath$007C_$007C(input) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), input, defaultOf()) && (input.length === 3)) {
        if (input[0] === "studies") {
            if (input[2] === "isa.study.xlsx") {
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
            const anyStudyName = input[1];
            return combineMany(input);
        }
        default:
            return void 0;
    }
}

