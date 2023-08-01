import { Union } from "../../../fable_modules/fable-library-ts/Types.js";
import { parse as parse_1, int32, float64 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { union_type, int32_type, float64_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { parse } from "../../../fable_modules/fable-library-ts/Double.js";
import { int32ToString } from "../../../fable_modules/fable-library-ts/Util.js";

export type AnnotationValue_$union = 
    | AnnotationValue<0>
    | AnnotationValue<1>
    | AnnotationValue<2>

export type AnnotationValue_$cases = {
    0: ["Text", [string]],
    1: ["Float", [float64]],
    2: ["Int", [int32]]
}

export function AnnotationValue_Text(Item: string) {
    return new AnnotationValue<0>(0, [Item]);
}

export function AnnotationValue_Float(Item: float64) {
    return new AnnotationValue<1>(1, [Item]);
}

export function AnnotationValue_Int(Item: int32) {
    return new AnnotationValue<2>(2, [Item]);
}

export class AnnotationValue<Tag extends keyof AnnotationValue_$cases> extends Union<Tag, AnnotationValue_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: AnnotationValue_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Text", "Float", "Int"];
    }
}

export function AnnotationValue_$reflection(): TypeInfo {
    return union_type("ISA.AnnotationValue", [], AnnotationValue, () => [[["Item", string_type]], [["Item", float64_type]], [["Item", int32_type]]]);
}

export function AnnotationValue_get_empty(): AnnotationValue_$union {
    return AnnotationValue_Text("");
}

/**
 * Create a ISAJson Annotation value from a ISATab string entry
 */
export function AnnotationValue_fromString_Z721C83C5(s: string): AnnotationValue_$union {
    try {
        return AnnotationValue_Int(parse_1(s, 511, false, 32));
    }
    catch (matchValue: any) {
        try {
            return AnnotationValue_Float(parse(s));
        }
        catch (matchValue_1: any) {
            return AnnotationValue_Text(s);
        }
    }
}

/**
 * Get a ISATab string Annotation Name from a ISAJson object
 */
export function AnnotationValue_toString_Z3C00A204(v: AnnotationValue_$union): string {
    switch (v.tag) {
        case /* Int */ 2:
            return int32ToString(v.fields[0]);
        case /* Float */ 1: {
            const f: float64 = v.fields[0];
            return f.toString();
        }
        default:
            return v.fields[0];
    }
}

