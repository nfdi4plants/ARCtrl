import { Union } from "../../../fable_modules/fable-library-ts/Types.js";
import { union_type, array_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export type ValidationResult_$union = 
    | ValidationResult<0>
    | ValidationResult<1>

export type ValidationResult_$cases = {
    0: ["Ok", []],
    1: ["Failed", [string[]]]
}

export function ValidationResult_Ok() {
    return new ValidationResult<0>(0, []);
}

export function ValidationResult_Failed(Item: string[]) {
    return new ValidationResult<1>(1, [Item]);
}

export class ValidationResult<Tag extends keyof ValidationResult_$cases> extends Union<Tag, ValidationResult_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: ValidationResult_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Ok", "Failed"];
    }
}

export function ValidationResult_$reflection(): TypeInfo {
    return union_type("ISA.Json.ValidationTypes.ValidationResult", [], ValidationResult, () => [[], [["Item", array_type(string_type)]]]);
}

export function ValidationResult__get_Success(this$: ValidationResult_$union): boolean {
    if (this$.tag === /* Ok */ 0) {
        return true;
    }
    else {
        return false;
    }
}

export function ValidationResult__GetErrors(this$: ValidationResult_$union): string[] {
    if (this$.tag === /* Failed */ 1) {
        return this$.fields[0];
    }
    else {
        return [];
    }
}

export function ValidationResult_OfJSchemaOutput_Z6EC48F6B(output: [boolean, string[]]): ValidationResult_$union {
    if (output[0]) {
        return ValidationResult_Ok();
    }
    else {
        return ValidationResult_Failed(output[1]);
    }
}

