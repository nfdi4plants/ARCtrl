import { int32ToString } from "../../../fable_modules/fable-library-ts/Util.js";
import { float64, int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation__get_NameText, OntologyAnnotation_create_131C8C9D, OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { defaultArg, value as value_1 } from "../../../fable_modules/fable-library-ts/Option.js";
import { Value_$union } from "../JsonTypes/Value.js";
import { AnnotationValue_Text } from "../JsonTypes/AnnotationValue.js";
import { Union } from "../../../fable_modules/fable-library-ts/Types.js";
import { union_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export type CompositeCell_$union = 
    | CompositeCell<0>
    | CompositeCell<1>
    | CompositeCell<2>

export type CompositeCell_$cases = {
    0: ["Term", [OntologyAnnotation]],
    1: ["FreeText", [string]],
    2: ["Unitized", [string, OntologyAnnotation]]
}

export function CompositeCell_Term(Item: OntologyAnnotation) {
    return new CompositeCell<0>(0, [Item]);
}

export function CompositeCell_FreeText(Item: string) {
    return new CompositeCell<1>(1, [Item]);
}

export function CompositeCell_Unitized(Item1: string, Item2: OntologyAnnotation) {
    return new CompositeCell<2>(2, [Item1, Item2]);
}

export class CompositeCell<Tag extends keyof CompositeCell_$cases> extends Union<Tag, CompositeCell_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: CompositeCell_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["Term", "FreeText", "Unitized"];
    }
    static fromValue(value: Value_$union, unit?: OntologyAnnotation): CompositeCell_$union {
        let matchResult: int32, t: OntologyAnnotation, i: int32, i_1: int32, u: OntologyAnnotation, f: float64, f_1: float64, u_1: OntologyAnnotation, s: string;
        switch (value.tag) {
            case /* Int */ 1: {
                if (unit != null) {
                    matchResult = 2;
                    i_1 = value.fields[0];
                    u = value_1(unit);
                }
                else {
                    matchResult = 1;
                    i = value.fields[0];
                }
                break;
            }
            case /* Float */ 2: {
                if (unit != null) {
                    matchResult = 4;
                    f_1 = value.fields[0];
                    u_1 = value_1(unit);
                }
                else {
                    matchResult = 3;
                    f = value.fields[0];
                }
                break;
            }
            case /* Name */ 3: {
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
                return CompositeCell_Term(t!);
            case 1:
                return CompositeCell_FreeText(int32ToString(i!));
            case 2:
                return CompositeCell_Unitized(int32ToString(i_1!), u!);
            case 3:
                return CompositeCell_FreeText(f!.toString());
            case 4:
                return CompositeCell_Unitized(f_1!.toString(), u_1!);
            case 5:
                return CompositeCell_FreeText(s!);
            default:
                throw new Error("could not convert value to cell, invalid combination of value and unit");
        }
    }
    get isUnitized(): boolean {
        const this$ = this as CompositeCell_$union;
        return this$.tag === /* Unitized */ 2;
    }
    get isTerm(): boolean {
        const this$ = this as CompositeCell_$union;
        return this$.tag === /* Term */ 0;
    }
    get isFreeText(): boolean {
        const this$ = this as CompositeCell_$union;
        return this$.tag === /* FreeText */ 1;
    }
    ToUnitizedCell(): CompositeCell_$union {
        const this$ = this as CompositeCell_$union;
        return (this$.tag === /* FreeText */ 1) ? CompositeCell_Unitized("", OntologyAnnotation_create_131C8C9D(void 0, AnnotationValue_Text(this$.fields[0]))) : ((this$.tag === /* Term */ 0) ? CompositeCell_Unitized("", this$.fields[0]) : this$);
    }
    ToTermCell(): CompositeCell_$union {
        const this$ = this as CompositeCell_$union;
        return (this$.tag === /* Unitized */ 2) ? CompositeCell_Term(this$.fields[1]) : ((this$.tag === /* FreeText */ 1) ? CompositeCell_Term(OntologyAnnotation_create_131C8C9D(void 0, AnnotationValue_Text(this$.fields[0]))) : this$);
    }
    ToFreeTextCell(): CompositeCell_$union {
        const this$ = this as CompositeCell_$union;
        switch (this$.tag) {
            case /* Term */ 0:
                return CompositeCell_FreeText(OntologyAnnotation__get_NameText(this$.fields[0]));
            case /* Unitized */ 2: {
                const v: string = this$.fields[0];
                return CompositeCell_FreeText(OntologyAnnotation__get_NameText(this$.fields[1]));
            }
            default:
                return this$;
        }
    }
    get AsUnitized(): [string, OntologyAnnotation] {
        const this$ = this as CompositeCell_$union;
        if (this$.tag === /* Unitized */ 2) {
            return [this$.fields[0], this$.fields[1]] as [string, OntologyAnnotation];
        }
        else {
            throw new Error("Not a Unitized cell.");
        }
    }
    get AsTerm(): OntologyAnnotation {
        const this$ = this as CompositeCell_$union;
        if (this$.tag === /* Term */ 0) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a Swate TermCell.");
        }
    }
    get AsFreeText(): string {
        const this$ = this as CompositeCell_$union;
        if (this$.tag === /* FreeText */ 1) {
            return this$.fields[0];
        }
        else {
            throw new Error("Not a Swate TermCell.");
        }
    }
    static createTerm(oa: OntologyAnnotation): CompositeCell_$union {
        return CompositeCell_Term(oa);
    }
    static createTermFromString(name?: string, tsr?: string, tan?: string): CompositeCell_$union {
        return CompositeCell_Term(OntologyAnnotation_fromString_Z7D8EB286(name, tsr, tan));
    }
    static createUnitized(value: string, oa?: OntologyAnnotation): CompositeCell_$union {
        return CompositeCell_Unitized(value, defaultArg(oa, OntologyAnnotation_get_empty()));
    }
    static createUnitizedFromString(value: string, name?: string, tsr?: string, tan?: string): CompositeCell_$union {
        const tupledArg = [value, OntologyAnnotation_fromString_Z7D8EB286(name, tsr, tan)] as [string, OntologyAnnotation];
        return CompositeCell_Unitized(tupledArg[0], tupledArg[1]);
    }
    static createFreeText(value: string): CompositeCell_$union {
        return CompositeCell_FreeText(value);
    }
    static get emptyTerm(): CompositeCell_$union {
        return CompositeCell_Term(OntologyAnnotation_get_empty());
    }
    static get emptyFreeText(): CompositeCell_$union {
        return CompositeCell_FreeText("");
    }
    static get emptyUnitized(): CompositeCell_$union {
        return CompositeCell_Unitized("", OntologyAnnotation_get_empty());
    }
    toString(): string {
        const this$ = this as CompositeCell_$union;
        return (this$.tag === /* FreeText */ 1) ? this$.fields[0] : ((this$.tag === /* Unitized */ 2) ? (`${this$.fields[0]} ${OntologyAnnotation__get_NameText(this$.fields[1])}`) : (`Term${OntologyAnnotation__get_NameText(this$.fields[0])}`));
    }
}

export function CompositeCell_$reflection(): TypeInfo {
    return union_type("ISA.CompositeCell", [], CompositeCell, () => [[["Item", OntologyAnnotation_$reflection()]], [["Item", string_type]], [["Item1", string_type], ["Item2", OntologyAnnotation_$reflection()]]]);
}

