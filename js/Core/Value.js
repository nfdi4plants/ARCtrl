import { tryParse } from "../fable_modules/fable-library-js.4.22.0/Int32.js";
import { Union, toString, FSharpRef } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { tryParse as tryParse_1 } from "../fable_modules/fable-library-js.4.22.0/Double.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { unwrap, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { int32ToString } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { union_type, string_type, float64_type, int32_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Value extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Ontology", "Int", "Float", "Name"];
    }
    static fromString(value) {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse(value, 511, false, 32, new FSharpRef(() => outArg, (v) => {
            outArg = (v | 0);
        })), outArg];
        if (matchValue[0]) {
            return new Value(1, [matchValue[1]]);
        }
        else {
            let matchValue_1;
            let outArg_1 = 0;
            matchValue_1 = [tryParse_1(value, new FSharpRef(() => outArg_1, (v_2) => {
                outArg_1 = v_2;
            })), outArg_1];
            return matchValue_1[0] ? (new Value(2, [matchValue_1[1]])) : (new Value(3, [value]));
        }
    }
    static fromOptions(value, termSource, termAccesssion) {
        let value_1;
        return (value == null) ? ((termSource == null) ? ((termAccesssion == null) ? undefined : (new Value(0, [OntologyAnnotation.create(defaultArg(value, ""), unwrap(termSource), unwrap(termAccesssion))]))) : (new Value(0, [OntologyAnnotation.create(defaultArg(value, ""), unwrap(termSource), unwrap(termAccesssion))]))) : ((termSource == null) ? ((termAccesssion == null) ? ((value_1 = value, Value.fromString(value_1))) : (new Value(0, [OntologyAnnotation.create(defaultArg(value, ""), unwrap(termSource), unwrap(termAccesssion))]))) : (new Value(0, [OntologyAnnotation.create(defaultArg(value, ""), unwrap(termSource), unwrap(termAccesssion))])));
    }
    static toOptions(value) {
        switch (value.tag) {
            case 1:
                return [int32ToString(value.fields[0]), undefined, undefined];
            case 2:
                return [value.fields[0].toString(), undefined, undefined];
            case 3:
                return [value.fields[0], undefined, undefined];
            default: {
                const oa = value.fields[0];
                return [oa.Name, oa.TermAccessionNumber, oa.TermSourceREF];
            }
        }
    }
    get Text() {
        const this$ = this;
        return (this$.tag === 2) ? this$.fields[0].toString() : ((this$.tag === 1) ? int32ToString(this$.fields[0]) : ((this$.tag === 3) ? this$.fields[0] : this$.fields[0].NameText));
    }
    AsName() {
        const this$ = this;
        if (this$.tag === 3) {
            return this$.fields[0];
        }
        else {
            throw new Error(`Value ${this$} is not of case name`);
        }
    }
    AsInt() {
        const this$ = this;
        if (this$.tag === 1) {
            return this$.fields[0] | 0;
        }
        else {
            throw new Error(`Value ${this$} is not of case int`);
        }
    }
    AsFloat() {
        const this$ = this;
        if (this$.tag === 2) {
            return this$.fields[0];
        }
        else {
            throw new Error(`Value ${this$} is not of case float`);
        }
    }
    AsOntology() {
        const this$ = this;
        if (this$.tag === 0) {
            return this$.fields[0];
        }
        else {
            throw new Error(`Value ${this$} is not of case ontology`);
        }
    }
    get IsAnOntology() {
        const this$ = this;
        return this$.tag === 0;
    }
    get IsNumerical() {
        const this$ = this;
        switch (this$.tag) {
            case 1:
            case 2:
                return true;
            default:
                return false;
        }
    }
    get IsAnInt() {
        const this$ = this;
        return this$.tag === 1;
    }
    get IsAFloat() {
        const this$ = this;
        return this$.tag === 2;
    }
    get IsAText() {
        const this$ = this;
        return this$.tag === 3;
    }
    static getText(v) {
        return v.Text;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return (this$.tag === 1) ? toText(printf("%i"))(this$.fields[0]) : ((this$.tag === 2) ? toText(printf("%f"))(this$.fields[0]) : ((this$.tag === 3) ? this$.fields[0] : this$.fields[0].NameText));
    }
}

export function Value_$reflection() {
    return union_type("ARCtrl.Value", [], Value, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", int32_type]], [["Item", float64_type]], [["Item", string_type]]]);
}

