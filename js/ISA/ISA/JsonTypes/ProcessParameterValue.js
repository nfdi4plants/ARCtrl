import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { bind, defaultArg, map } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { ProtocolParameter__SetCategory_Z4C0FE73C, ProtocolParameter_create_Z6C54B221, ProtocolParameter__MapCategory_Z69DD836A, ProtocolParameter__get_TryNameText, ProtocolParameter_$reflection, ProtocolParameter__get_NameText } from "./ProtocolParameter.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation__get_NameText } from "./OntologyAnnotation.js";
import { record_type, option_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Value_$reflection } from "./Value.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class ProcessParameterValue extends Record {
    constructor(Category, Value, Unit) {
        super();
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
        const category = map(ProtocolParameter__get_NameText, this$.Category);
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

export function ProcessParameterValue_$reflection() {
    return record_type("ARCtrl.ISA.ProcessParameterValue", [], ProcessParameterValue, () => [["Category", option_type(ProtocolParameter_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

export function ProcessParameterValue_make(category, value, unit) {
    return new ProcessParameterValue(category, value, unit);
}

export function ProcessParameterValue_create_569825F3(Category, Value, Unit) {
    return ProcessParameterValue_make(Category, Value, Unit);
}

export function ProcessParameterValue_get_empty() {
    return ProcessParameterValue_create_569825F3();
}

/**
 * Returns the name of the category as string
 */
export function ProcessParameterValue__get_NameText(this$) {
    return defaultArg(map(ProtocolParameter__get_NameText, this$.Category), "");
}

/**
 * Returns the name of the category as string
 */
export function ProcessParameterValue__get_TryNameText(this$) {
    return bind(ProtocolParameter__get_TryNameText, this$.Category);
}

export function ProcessParameterValue__get_ValueText(this$) {
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

export function ProcessParameterValue__get_ValueWithUnitText(this$) {
    const unit = map(OntologyAnnotation__get_NameText, this$.Unit);
    const v = ProcessParameterValue__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u = unit;
        return toText(printf("%s %s"))(v)(u);
    }
}

export function ProcessParameterValue__MapCategory_Z69DD836A(this$, f) {
    return new ProcessParameterValue(map((p) => ProtocolParameter__MapCategory_Z69DD836A(p, f), this$.Category), this$.Value, this$.Unit);
}

export function ProcessParameterValue__SetCategory_Z4C0FE73C(this$, c) {
    let matchValue;
    return new ProcessParameterValue((matchValue = this$.Category, (matchValue == null) ? ProtocolParameter_create_Z6C54B221(void 0, c) : ProtocolParameter__SetCategory_Z4C0FE73C(matchValue, c)), this$.Value, this$.Unit);
}

/**
 * Returns the name of the paramater value as string if it exists
 */
export function ProcessParameterValue_tryGetNameText_5FD7232D(pv) {
    return ProcessParameterValue__get_TryNameText(pv);
}

/**
 * Returns the name of the paramater value as string
 */
export function ProcessParameterValue_getNameText_5FD7232D(pv) {
    return ProcessParameterValue__get_NameText(pv);
}

/**
 * Returns true if the given name matches the name of the parameter value
 */
export function ProcessParameterValue_nameEqualsString(name, pv) {
    return ProcessParameterValue__get_NameText(pv) === name;
}

export function ProcessParameterValue_getCategory_5FD7232D(pv) {
    return pv.Category;
}

