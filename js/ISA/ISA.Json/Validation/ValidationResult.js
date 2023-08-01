import { Union } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { union_type, array_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class ValidationResult extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Ok", "Failed"];
    }
}

export function ValidationResult_$reflection() {
    return union_type("ISA.Json.ValidationTypes.ValidationResult", [], ValidationResult, () => [[], [["Item", array_type(string_type)]]]);
}

export function ValidationResult__get_Success(this$) {
    if (this$.tag === 0) {
        return true;
    }
    else {
        return false;
    }
}

export function ValidationResult__GetErrors(this$) {
    if (this$.tag === 1) {
        return this$.fields[0];
    }
    else {
        return [];
    }
}

export function ValidationResult_OfJSchemaOutput_Z6EC48F6B(output) {
    if (output[0]) {
        return new ValidationResult(0, []);
    }
    else {
        return new ValidationResult(1, [output[1]]);
    }
}

