import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { AnnotationValue } from "../JsonTypes/AnnotationValue.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Union } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { union_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class CompositeCell extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Term", "FreeText", "Unitized"];
    }
    static fromValue(value, unit) {
        let matchResult, t, i, i_1, u, f, f_1, u_1, s;
        switch (value.tag) {
            case 1: {
                if (unit != null) {
                    matchResult = 2;
                    i_1 = value.fields[0];
                    u = unit;
                }
                else {
                    matchResult = 1;
                    i = value.fields[0];
                }
                break;
            }
            case 2: {
                if (unit != null) {
                    matchResult = 4;
                    f_1 = value.fields[0];
                    u_1 = unit;
                }
                else {
                    matchResult = 3;
                    f = value.fields[0];
                }
                break;
            }
            case 3: {
                if (unit == null) {
                    matchResult = 5;
                    s = value.fields[0];
                }
                else {
                    matchResult = 6;
                }
                break;
            }
            default:
                if (unit == null) {
                    matchResult = 0;
                    t = value.fields[0];
                }
                else {
                    matchResult = 6;
                }
        }
        switch (matchResult) {
            case 0:
                return new CompositeCell(0, [t]);
            case 1:
                return new CompositeCell(1, [int32ToString(i)]);
            case 2:
                return new CompositeCell(2, [int32ToString(i_1), u]);
            case 3:
                return new CompositeCell(1, [f.toString()]);
            case 4:
                return new CompositeCell(2, [f_1.toString(), u_1]);
            case 5:
                return new CompositeCell(1, [s]);
            default:
                throw new Error("could not convert value to cell, invalid combination of value and unit");
        }
    }
    get isUnitized() {
        const this$ = this;
        return this$.tag === 2;
    }
    get isTerm() {
        const this$ = this;
        return this$.tag === 0;
    }
    get isFreeText() {
        const this$ = this;
        return this$.tag === 1;
    }
    ToUnitizedCell() {
        const this$ = this;
        return (this$.tag === 1) ? (new CompositeCell(2, ["", OntologyAnnotation.create(void 0, new AnnotationValue(0, [this$.fields[0]]))])) : ((this$.tag === 0) ? (new CompositeCell(2, ["", this$.fields[0]])) : this$);
    }
    ToTermCell() {
        const this$ = this;
        return (this$.tag === 2) ? (new CompositeCell(0, [this$.fields[1]])) : ((this$.tag === 1) ? (new CompositeCell(0, [OntologyAnnotation.create(void 0, new AnnotationValue(0, [this$.fields[0]]))])) : this$);
    }
    ToFreeTextCell() {
        const this$ = this;
        return (this$.tag === 0) ? (new CompositeCell(1, [this$.fields[0].NameText])) : ((this$.tag === 2) ? (new CompositeCell(1, [this$.fields[1].NameText])) : this$);
    }
    get AsUnitized() {
        const this$ = this;
        if (this$.tag === 2) {
            return [this$.fields[0], this$.fields[1]];
        }
        else {
            throw new Error("Not a Unitized cell.");
        }
    }
    get AsTerm() {
        const this$ = this;
        if (this$.tag === 0) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a Swate TermCell.");
        }
    }
    get AsFreeText() {
        const this$ = this;
        if (this$.tag === 1) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a Swate TermCell.");
        }
    }
    static createTerm(oa) {
        return new CompositeCell(0, [oa]);
    }
    static createTermFromString(name, tsr, tan) {
        return new CompositeCell(0, [OntologyAnnotation.fromString(name, tsr, tan)]);
    }
    static createUnitized(value, oa) {
        return new CompositeCell(2, [value, defaultArg(oa, OntologyAnnotation.empty)]);
    }
    static createUnitizedFromString(value, name, tsr, tan) {
        const tupledArg = [value, OntologyAnnotation.fromString(name, tsr, tan)];
        return new CompositeCell(2, [tupledArg[0], tupledArg[1]]);
    }
    static createFreeText(value) {
        return new CompositeCell(1, [value]);
    }
    static get emptyTerm() {
        return new CompositeCell(0, [OntologyAnnotation.empty]);
    }
    static get emptyFreeText() {
        return new CompositeCell(1, [""]);
    }
    static get emptyUnitized() {
        return new CompositeCell(2, ["", OntologyAnnotation.empty]);
    }
    toString() {
        const this$ = this;
        return (this$.tag === 1) ? this$.fields[0] : ((this$.tag === 2) ? (`${this$.fields[0]} ${this$.fields[1].NameText}`) : (`Term${this$.fields[0].NameText}`));
    }
}

export function CompositeCell_$reflection() {
    return union_type("ARCtrl.ISA.CompositeCell", [], CompositeCell, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", string_type]], [["Item1", string_type], ["Item2", OntologyAnnotation_$reflection()]]]);
}

