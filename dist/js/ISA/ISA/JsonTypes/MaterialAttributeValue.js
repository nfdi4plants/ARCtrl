import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { bind, defaultArg, map } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { MaterialAttribute__SetCategory_Z4C0FE73C, MaterialAttribute_create_Z6C54B221, MaterialAttribute__MapCategory_Z69DD836A, MaterialAttribute__get_TryNameText, MaterialAttribute_$reflection, MaterialAttribute__get_NameText } from "./MaterialAttribute.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Value_$reflection } from "./Value.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class MaterialAttributeValue extends Record {
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
        const category = map(MaterialAttribute__get_NameText, this$.Category);
        const unit = map((oa) => oa.NameText, this$.Unit);
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

export function MaterialAttributeValue_$reflection() {
    return record_type("ARCtrl.ISA.MaterialAttributeValue", [], MaterialAttributeValue, () => [["ID", option_type(string_type)], ["Category", option_type(MaterialAttribute_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttributeValue_make(id, category, value, unit) {
    return new MaterialAttributeValue(id, category, value, unit);
}

export function MaterialAttributeValue_create_7F714043(Id, Category, Value, Unit) {
    return MaterialAttributeValue_make(Id, Category, Value, Unit);
}

export function MaterialAttributeValue_get_empty() {
    return MaterialAttributeValue_create_7F714043();
}

/**
 * Returns the name of the category as string
 */
export function MaterialAttributeValue__get_NameText(this$) {
    return defaultArg(map(MaterialAttribute__get_NameText, this$.Category), "");
}

/**
 * Returns the name of the category as string
 */
export function MaterialAttributeValue__get_TryNameText(this$) {
    return bind(MaterialAttribute__get_TryNameText, this$.Category);
}

export function MaterialAttributeValue__get_ValueText(this$) {
    return defaultArg(map((oa) => {
        switch (oa.tag) {
            case 2:
                return oa.fields[0].toString();
            case 1:
                return int32ToString(oa.fields[0]);
            case 3:
                return oa.fields[0];
            default:
                return oa.fields[0].NameText;
        }
    }, this$.Value), "");
}

export function MaterialAttributeValue__get_ValueWithUnitText(this$) {
    const unit = map((oa) => oa.NameText, this$.Unit);
    const v = MaterialAttributeValue__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u = unit;
        return toText(printf("%s %s"))(v)(u);
    }
}

export function MaterialAttributeValue__MapCategory_Z69DD836A(this$, f) {
    return new MaterialAttributeValue(this$.ID, map((p) => MaterialAttribute__MapCategory_Z69DD836A(p, f), this$.Category), this$.Value, this$.Unit);
}

export function MaterialAttributeValue__SetCategory_Z4C0FE73C(this$, c) {
    let matchValue;
    return new MaterialAttributeValue(this$.ID, (matchValue = this$.Category, (matchValue == null) ? MaterialAttribute_create_Z6C54B221(void 0, c) : MaterialAttribute__SetCategory_Z4C0FE73C(matchValue, c)), this$.Value, this$.Unit);
}

/**
 * Returns the name of the characteristic value as string if it exists
 */
export function MaterialAttributeValue_tryGetNameText_43A5B238(mv) {
    return MaterialAttributeValue__get_TryNameText(mv);
}

/**
 * Returns the name of the characteristic value as string
 */
export function MaterialAttributeValue_getNameAsString_43A5B238(mv) {
    return MaterialAttributeValue__get_TryNameText(mv);
}

/**
 * Returns true if the given name matches the name of the characteristic value
 */
export function MaterialAttributeValue_nameEqualsString(name, mv) {
    return MaterialAttributeValue__get_NameText(mv) === name;
}

