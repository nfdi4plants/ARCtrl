import { Union } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { union_type, int32_type, float64_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { parse } from "../../../fable_modules/fable-library.4.1.4/Double.js";
import { parse as parse_1 } from "../../../fable_modules/fable-library.4.1.4/Int32.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export class AnnotationValue extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Text", "Float", "Int"];
    }
}

export function AnnotationValue_$reflection() {
    return union_type("ISA.AnnotationValue", [], AnnotationValue, () => [[["Item", string_type]], [["Item", float64_type]], [["Item", int32_type]]]);
}

export function AnnotationValue_get_empty() {
    return new AnnotationValue(0, [""]);
}

/**
 * Create a ISAJson Annotation value from a ISATab string entry
 */
export function AnnotationValue_fromString_Z721C83C5(s) {
    try {
        return new AnnotationValue(2, [parse_1(s, 511, false, 32)]);
    }
    catch (matchValue) {
        try {
            return new AnnotationValue(1, [parse(s)]);
        }
        catch (matchValue_1) {
            return new AnnotationValue(0, [s]);
        }
    }
}

/**
 * Get a ISATab string Annotation Name from a ISAJson object
 */
export function AnnotationValue_toString_Z3C00A204(v) {
    switch (v.tag) {
        case 2:
            return int32ToString(v.fields[0]);
        case 1:
            return v.fields[0].toString();
        default:
            return v.fields[0];
    }
}

