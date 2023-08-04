import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { defaultArg, value as value_3, Option, map } from "../../../fable_modules/fable-library-ts/Option.js";
import { Factor_$reflection, Factor } from "./Factor.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IISAPrintable } from "../Printer.js";
import { Value_$reflection, Value_$union } from "./Value.js";
import { int32ToString, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { float64 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";

export class FactorValue extends Record implements IEquatable<FactorValue>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Category: Option<Factor>;
    readonly Value: Option<Value_$union>;
    readonly Unit: Option<OntologyAnnotation>;
    constructor(ID: Option<string>, Category: Option<Factor>, Value: Option<Value_$union>, Unit: Option<OntologyAnnotation>) {
        super();
        this.ID = ID;
        this.Category = Category;
        this.Value = Value;
        this.Unit = Unit;
    }
    Print(): string {
        const this$: FactorValue = this;
        return toString(this$);
    }
    PrintCompact(): string {
        let value_2: string, category_2: string, category_1: string, value_1: string;
        const this$: FactorValue = this;
        const category: Option<string> = map<Factor, string>((f: Factor): string => f.NameText, this$.Category);
        const unit: Option<string> = map<OntologyAnnotation, string>((oa: OntologyAnnotation): string => oa.NameText, this$.Unit);
        const value: Option<string> = map<Value_$union, string>((v: Value_$union): string => {
            const s: string = (v as IISAPrintable).PrintCompact();
            if (unit == null) {
                return s;
            }
            else {
                return (s + " ") + value_3(unit);
            }
        }, this$.Value);
        return (category == null) ? ((value == null) ? "" : ((value_2 = value_3(value), value_2))) : ((value == null) ? ((category_2 = value_3(category), (category_2 + ":") + "No Value")) : ((category_1 = value_3(category), (value_1 = value_3(value), (category_1 + ":") + value_1))));
    }
}

export function FactorValue_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.FactorValue", [], FactorValue, () => [["ID", option_type(string_type)], ["Category", option_type(Factor_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

export function FactorValue_make(id: Option<string>, category: Option<Factor>, value: Option<Value_$union>, unit: Option<OntologyAnnotation>): FactorValue {
    return new FactorValue(id, category, value, unit);
}

export function FactorValue_create_18335379(Id?: string, Category?: Factor, Value?: Value_$union, Unit?: OntologyAnnotation): FactorValue {
    return FactorValue_make(Id, Category, Value, Unit);
}

export function FactorValue_get_empty(): FactorValue {
    return FactorValue_create_18335379();
}

export function FactorValue__get_ValueText(this$: FactorValue): string {
    return defaultArg(map<Value_$union, string>((oa: Value_$union): string => {
        switch (oa.tag) {
            case /* Float */ 2: {
                const f: float64 = oa.fields[0];
                return f.toString();
            }
            case /* Int */ 1:
                return int32ToString(oa.fields[0]);
            case /* Name */ 3:
                return oa.fields[0];
            default: {
                const oa_1: OntologyAnnotation = oa.fields[0];
                return oa_1.NameText;
            }
        }
    }, this$.Value), "");
}

export function FactorValue__get_ValueWithUnitText(this$: FactorValue): string {
    const unit: Option<string> = map<OntologyAnnotation, string>((oa: OntologyAnnotation): string => oa.NameText, this$.Unit);
    const v: string = FactorValue__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u: string = value_3(unit);
        return toText(printf("%s %s"))(v)(u);
    }
}

export function FactorValue__get_NameText(this$: FactorValue): string {
    return defaultArg(map<Factor, string>((factor: Factor): string => factor.NameText, this$.Category), "");
}

export function FactorValue__MapCategory_Z69DD836A(this$: FactorValue, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): FactorValue {
    return new FactorValue(this$.ID, map<Factor, Factor>((p: Factor): Factor => p.MapCategory(f), this$.Category), this$.Value, this$.Unit);
}

export function FactorValue__SetCategory_Z4C0FE73C(this$: FactorValue, c: OntologyAnnotation): FactorValue {
    let matchValue: Option<Factor>, p: Factor;
    return new FactorValue(this$.ID, (matchValue = this$.Category, (matchValue == null) ? Factor.create(void 0, void 0, c) : ((p = value_3(matchValue), p.SetCategory(c)))), this$.Value, this$.Unit);
}

/**
 * Returns the name of the factor value as string
 */
export function FactorValue_getNameAsString_Z2623397E(fv: FactorValue): string {
    return FactorValue__get_NameText(fv);
}

/**
 * Returns true if the given name matches the name of the factor value
 */
export function FactorValue_nameEqualsString(name: string, fv: FactorValue): boolean {
    return FactorValue__get_NameText(fv) === name;
}

