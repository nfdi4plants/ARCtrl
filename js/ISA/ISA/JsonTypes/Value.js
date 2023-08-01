import { Union, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation_$reflection, OntologyAnnotation__get_NameText } from "./OntologyAnnotation.js";
import { union_type, string_type, float64_type, int32_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { parse } from "../../../fable_modules/fable-library.4.1.4/Double.js";
import { parse as parse_1 } from "../../../fable_modules/fable-library.4.1.4/Int32.js";
import { map, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { AnnotationValue_toString_Z3C00A204 } from "./AnnotationValue.js";

export class Value extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Ontology", "Int", "Float", "Name"];
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return (this$.tag === 1) ? toText(printf("%i"))(this$.fields[0]) : ((this$.tag === 2) ? toText(printf("%f"))(this$.fields[0]) : ((this$.tag === 3) ? this$.fields[0] : OntologyAnnotation__get_NameText(this$.fields[0])));
    }
}

export function Value_$reflection() {
    return union_type("ISA.Value", [], Value, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", int32_type]], [["Item", float64_type]], [["Item", string_type]]]);
}

export function Value_fromString_Z721C83C5(value) {
    try {
        return new Value(1, [parse_1(value, 511, false, 32)]);
    }
    catch (matchValue) {
        try {
            return new Value(2, [parse(value)]);
        }
        catch (matchValue_1) {
            return new Value(3, [value]);
        }
    }
}

export function Value_fromOptions(value, termSource, termAccesssion) {
    let matchResult, value_1;
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
            value_1 = value;
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
                    return new Value(1, [parse_1(value_1, 511, false, 32)]);
                }
                catch (matchValue_1) {
                    try {
                        return new Value(2, [parse(value_1)]);
                    }
                    catch (matchValue_2) {
                        return new Value(3, [value_1]);
                    }
                }
            })();
        case 1:
            return void 0;
        default:
            return new Value(0, [OntologyAnnotation_fromString_Z7D8EB286(defaultArg(value, ""), termSource, termAccesssion)]);
    }
}

export function Value_toOptions_Z277CD705(value) {
    switch (value.tag) {
        case 1:
            return [int32ToString(value.fields[0]), void 0, void 0];
        case 2:
            return [value.fields[0].toString(), void 0, void 0];
        case 3:
            return [value.fields[0], void 0, void 0];
        default: {
            const oa = value.fields[0];
            return [map(AnnotationValue_toString_Z3C00A204, oa.Name), oa.TermAccessionNumber, oa.TermSourceREF];
        }
    }
}

export function Value__get_Text(this$) {
    switch (this$.tag) {
        case 2:
            return this$.fields[0].toString();
        case 1:
            return int32ToString(this$.fields[0]);
        case 3:
            return this$.fields[0];
        default:
            return OntologyAnnotation__get_NameText(this$.fields[0]);
    }
}

export function Value__AsName(this$) {
    if (this$.tag === 3) {
        return this$.fields[0];
    }
    else {
        throw new Error(`Value ${this$} is not of case name`);
    }
}

export function Value__AsInt(this$) {
    if (this$.tag === 1) {
        return this$.fields[0] | 0;
    }
    else {
        throw new Error(`Value ${this$} is not of case int`);
    }
}

export function Value__AsFloat(this$) {
    if (this$.tag === 2) {
        return this$.fields[0];
    }
    else {
        throw new Error(`Value ${this$} is not of case float`);
    }
}

export function Value__AsOntology(this$) {
    if (this$.tag === 0) {
        return this$.fields[0];
    }
    else {
        throw new Error(`Value ${this$} is not of case ontology`);
    }
}

export function Value__get_IsAnOntology(this$) {
    if (this$.tag === 0) {
        return true;
    }
    else {
        return false;
    }
}

export function Value__get_IsNumerical(this$) {
    switch (this$.tag) {
        case 1:
        case 2:
            return true;
        default:
            return false;
    }
}

export function Value__get_IsAnInt(this$) {
    if (this$.tag === 1) {
        return true;
    }
    else {
        return false;
    }
}

export function Value__get_IsAFloat(this$) {
    if (this$.tag === 2) {
        return true;
    }
    else {
        return false;
    }
}

export function Value__get_IsAText(this$) {
    if (this$.tag === 3) {
        return true;
    }
    else {
        return false;
    }
}

export function Value_getText_Z277CD705(v) {
    return Value__get_Text(v);
}

