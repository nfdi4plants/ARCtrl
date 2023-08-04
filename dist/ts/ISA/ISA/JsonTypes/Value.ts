import { Union, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { parse as parse_1, float64, int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IISAPrintable } from "../Printer.js";
import { union_type, string_type, float64_type, int32_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { parse } from "../../../fable_modules/fable-library-ts/Double.js";
import { map, Option, value as value_2, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { int32ToString } from "../../../fable_modules/fable-library-ts/Util.js";
import { AnnotationValue_$union, AnnotationValue_toString_Z6FAD7738 } from "./AnnotationValue.js";

export type Value_$union = 
    | Value<0>
    | Value<1>
    | Value<2>
    | Value<3>

export type Value_$cases = {
    0: ["Ontology", [OntologyAnnotation]],
    1: ["Int", [int32]],
    2: ["Float", [float64]],
    3: ["Name", [string]]
}

export function Value_Ontology(Item: OntologyAnnotation) {
    return new Value<0>(0, [Item]);
}

export function Value_Int(Item: int32) {
    return new Value<1>(1, [Item]);
}

export function Value_Float(Item: float64) {
    return new Value<2>(2, [Item]);
}

export function Value_Name(Item: string) {
    return new Value<3>(3, [Item]);
}

export class Value<Tag extends keyof Value_$cases> extends Union<Tag, Value_$cases[Tag][0]> implements IISAPrintable {
    constructor(readonly tag: Tag, readonly fields: Value_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Ontology", "Int", "Float", "Name"];
    }
    Print(): string {
        const this$ = this as Value_$union;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$ = this as Value_$union;
        switch (this$.tag) {
            case /* Int */ 1: {
                const i: int32 = this$.fields[0] | 0;
                return toText(printf("%i"))(i);
            }
            case /* Float */ 2: {
                const f: float64 = this$.fields[0];
                return toText(printf("%f"))(f);
            }
            case /* Name */ 3:
                return this$.fields[0];
            default: {
                const oa: OntologyAnnotation = this$.fields[0];
                return oa.NameText;
            }
        }
    }
}

export function Value_$reflection(): TypeInfo {
    return union_type("ARCtrl.ISA.Value", [], Value, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", int32_type]], [["Item", float64_type]], [["Item", string_type]]]);
}

export function Value_fromString_Z721C83C5(value: string): Value_$union {
    try {
        return Value_Int(parse_1(value, 511, false, 32));
    }
    catch (matchValue: any) {
        try {
            return Value_Float(parse(value));
        }
        catch (matchValue_1: any) {
            return Value_Name(value);
        }
    }
}

export function Value_fromOptions(value: Option<string>, termSource: Option<string>, termAccesssion: Option<string>): Option<Value_$union> {
    let matchResult: int32, value_1: string;
    if (value == null) {
        if (termSource == null) {
            if (termAccesssion == null) {
                matchResult = 1;
            }
            else {
                matchResult = 2;
            }
        }
        else {
            matchResult = 2;
        }
    }
    else if (termSource == null) {
        if (termAccesssion == null) {
            matchResult = 0;
            value_1 = value_2(value);
        }
        else {
            matchResult = 2;
        }
    }
    else {
        matchResult = 2;
    }
    switch (matchResult) {
        case 0:
            return (() => {
                try {
                    return Value_Int(parse_1(value_1!, 511, false, 32));
                }
                catch (matchValue_1: any) {
                    try {
                        return Value_Float(parse(value_1!));
                    }
                    catch (matchValue_2: any) {
                        return Value_Name(value_1!);
                    }
                }
            })();
        case 1:
            return void 0;
        default:
            return Value_Ontology(OntologyAnnotation.fromString(defaultArg(value, ""), termSource, termAccesssion));
    }
}

export function Value_toOptions_72E9EF0F(value: Value_$union): [Option<string>, Option<string>, Option<string>] {
    switch (value.tag) {
        case /* Int */ 1:
            return [int32ToString(value.fields[0]), void 0, void 0] as [Option<string>, Option<string>, Option<string>];
        case /* Float */ 2: {
            const f: float64 = value.fields[0];
            return [f.toString(), void 0, void 0] as [Option<string>, Option<string>, Option<string>];
        }
        case /* Name */ 3:
            return [value.fields[0], void 0, void 0] as [Option<string>, Option<string>, Option<string>];
        default: {
            const oa: OntologyAnnotation = value.fields[0];
            return [map<AnnotationValue_$union, string>(AnnotationValue_toString_Z6FAD7738, oa.Name), oa.TermAccessionNumber, oa.TermSourceREF] as [Option<string>, Option<string>, Option<string>];
        }
    }
}

export function Value__get_Text(this$: Value_$union): string {
    switch (this$.tag) {
        case /* Float */ 2: {
            const f: float64 = this$.fields[0];
            return f.toString();
        }
        case /* Int */ 1:
            return int32ToString(this$.fields[0]);
        case /* Name */ 3:
            return this$.fields[0];
        default: {
            const oa: OntologyAnnotation = this$.fields[0];
            return oa.NameText;
        }
    }
}

export function Value__AsName(this$: Value_$union): string {
    if (this$.tag === /* Name */ 3) {
        return this$.fields[0];
    }
    else {
        throw new Error(`Value ${this$} is not of case name`);
    }
}

export function Value__AsInt(this$: Value_$union): int32 {
    if (this$.tag === /* Int */ 1) {
        return this$.fields[0] | 0;
    }
    else {
        throw new Error(`Value ${this$} is not of case int`);
    }
}

export function Value__AsFloat(this$: Value_$union): float64 {
    if (this$.tag === /* Float */ 2) {
        return this$.fields[0];
    }
    else {
        throw new Error(`Value ${this$} is not of case float`);
    }
}

export function Value__AsOntology(this$: Value_$union): OntologyAnnotation {
    if (this$.tag === /* Ontology */ 0) {
        return this$.fields[0];
    }
    else {
        throw new Error(`Value ${this$} is not of case ontology`);
    }
}

export function Value__get_IsAnOntology(this$: Value_$union): boolean {
    if (this$.tag === /* Ontology */ 0) {
        const oa: OntologyAnnotation = this$.fields[0];
        return true;
    }
    else {
        return false;
    }
}

export function Value__get_IsNumerical(this$: Value_$union): boolean {
    switch (this$.tag) {
        case /* Int */ 1:
        case /* Float */ 2:
            return true;
        default:
            return false;
    }
}

export function Value__get_IsAnInt(this$: Value_$union): boolean {
    if (this$.tag === /* Int */ 1) {
        return true;
    }
    else {
        return false;
    }
}

export function Value__get_IsAFloat(this$: Value_$union): boolean {
    if (this$.tag === /* Float */ 2) {
        return true;
    }
    else {
        return false;
    }
}

export function Value__get_IsAText(this$: Value_$union): boolean {
    if (this$.tag === /* Name */ 3) {
        return true;
    }
    else {
        return false;
    }
}

export function Value_getText_72E9EF0F(v: Value_$union): string {
    return Value__get_Text(v);
}

