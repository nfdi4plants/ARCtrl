import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { defaultArg, map } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Factor__SetCategory_2FC95D30, Factor_create_Z3D2B374F, Factor__MapCategory_65D42856, Factor_$reflection, Factor__get_NameText } from "./Factor.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation__get_NameText } from "./OntologyAnnotation.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Value_$reflection } from "./Value.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class FactorValue extends Record {
    constructor(ID, Category, Value, Unit) {
        super();
        this.ID = ID;
        this.Category = Category;
        this.Value = Value;
        this.Unit = Unit;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        let value_2, category_2, category_1, value_1;
        const this$ = this;
        const category = map(Factor__get_NameText, this$.Category);
        const unit = map(OntologyAnnotation__get_NameText, this$.Unit);
        const value = map((v) => {
            const s = v.PrintCompact();
            if (unit == null) {
                return s;
            }
            else {
                return (s + " ") + unit;
            }
        }, this$.Value);
        return (category == null) ? ((value == null) ? "" : ((value_2 = value, value_2))) : ((value == null) ? ((category_2 = category, (category_2 + ":") + "No Value")) : ((category_1 = category, (value_1 = value, (category_1 + ":") + value_1))));
    }
}

export function FactorValue_$reflection() {
    return record_type("ISA.FactorValue", [], FactorValue, () => [["ID", option_type(string_type)], ["Category", option_type(Factor_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

export function FactorValue_make(id, category, value, unit) {
    return new FactorValue(id, category, value, unit);
}

export function FactorValue_create_Z54E26173(Id, Category, Value, Unit) {
    return FactorValue_make(Id, Category, Value, Unit);
}

export function FactorValue_get_empty() {
    return FactorValue_create_Z54E26173();
}

export function FactorValue__get_ValueText(this$) {
    return defaultArg(map((oa) => {
        switch (oa.tag) {
            case 2:
                return oa.fields[0].toString();
            case 1:
                return int32ToString(oa.fields[0]);
            case 3:
                return oa.fields[0];
            default:
                return OntologyAnnotation__get_NameText(oa.fields[0]);
        }
    }, this$.Value), "");
}

export function FactorValue__get_ValueWithUnitText(this$) {
    const unit = map(OntologyAnnotation__get_NameText, this$.Unit);
    const v = FactorValue__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u = unit;
        return toText(printf("%s %s"))(v)(u);
    }
}

export function FactorValue__get_NameText(this$) {
    return defaultArg(map(Factor__get_NameText, this$.Category), "");
}

export function FactorValue__MapCategory_65D42856(this$, f) {
    return new FactorValue(this$.ID, map((p) => Factor__MapCategory_65D42856(p, f), this$.Category), this$.Value, this$.Unit);
}

export function FactorValue__SetCategory_2FC95D30(this$, c) {
    let matchValue;
    return new FactorValue(this$.ID, (matchValue = this$.Category, (matchValue == null) ? Factor_create_Z3D2B374F(void 0, void 0, c) : Factor__SetCategory_2FC95D30(matchValue, c)), this$.Value, this$.Unit);
}

/**
 * Returns the name of the factor value as string
 */
export function FactorValue_getNameAsString_2A4175B6(fv) {
    return FactorValue__get_NameText(fv);
}

/**
 * Returns true if the given name matches the name of the factor value
 */
export function FactorValue_nameEqualsString(name, fv) {
    return FactorValue__get_NameText(fv) === name;
}

