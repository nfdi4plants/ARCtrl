import { Option, value, some } from "../fable_modules/fable-library-ts/Option.js";
import { int32 } from "../fable_modules/fable-library-ts/Int32.js";
import { Contract } from "../Contract/Contract.js";
import { combineMany } from "../FileSystem/Path.js";
import { equalsWith } from "../fable_modules/fable-library-ts/Array.js";
import { defaultOf } from "../fable_modules/fable-library-ts/Util.js";

export function tryFromContract(c: Contract): Option<any> {
    let matchResult: int32, fsworkbook: any;
    if (c.Operation === "READ") {
        if (c.DTOType != null) {
            if (value(c.DTOType) === "ISA_Assay") {
                if (c.DTO != null) {
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
    switch (matchResult) {
        case 0:
            return some(fsworkbook!);
        default:
            return void 0;
    }
}

export function $007CAssayPath$007C_$007C(input: string[]): Option<string> {
    let matchResult: int32;
    if (!equalsWith((x: string, y: string): boolean => (x === y), input, defaultOf()) && (input.length === 3)) {
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
            const anyAssayName: string = input[1];
            return combineMany(input);
        }
        default:
            return void 0;
    }
}

