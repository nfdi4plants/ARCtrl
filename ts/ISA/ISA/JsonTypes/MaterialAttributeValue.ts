import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { bind, defaultArg, value as value_3, Option, map } from "../../../fable_modules/fable-library-ts/Option.js";
import { MaterialAttribute__SetCategory_2FC95D30, MaterialAttribute_create_2769312B, MaterialAttribute__MapCategory_65D42856, MaterialAttribute__get_TryNameText, MaterialAttribute_$reflection, MaterialAttribute, MaterialAttribute__get_NameText } from "./MaterialAttribute.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation, OntologyAnnotation__get_NameText } from "./OntologyAnnotation.js";
import { IISAPrintable } from "../Printer.js";
import { Value_$reflection, Value_$union } from "./Value.js";
import { int32ToString, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { float64 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";

export class MaterialAttributeValue extends Record implements IEquatable<MaterialAttributeValue>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Category: Option<MaterialAttribute>;
    readonly Value: Option<Value_$union>;
    readonly Unit: Option<OntologyAnnotation>;
    constructor(ID: Option<string>, Category: Option<MaterialAttribute>, Value: Option<Value_$union>, Unit: Option<OntologyAnnotation>) {
        super();
        this.ID = ID;
        this.Category = Category;
        this.Value = Value;
        this.Unit = Unit;
    }
    Print(): string {
        const this$: MaterialAttributeValue = this;
        return toString(this$);
    }
    PrintCompact(): string {
        let value_2: string, category_2: string, category_1: string, value_1: string;
        const this$: MaterialAttributeValue = this;
        const category: Option<string> = map<MaterialAttribute, string>(MaterialAttribute__get_NameText, this$.Category);
        const unit: Option<string> = map<OntologyAnnotation, string>(OntologyAnnotation__get_NameText, this$.Unit);
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

export function MaterialAttributeValue_$reflection(): TypeInfo {
    return record_type("ISA.MaterialAttributeValue", [], MaterialAttributeValue, () => [["ID", option_type(string_type)], ["Category", option_type(MaterialAttribute_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttributeValue_make(id: Option<string>, category: Option<MaterialAttribute>, value: Option<Value_$union>, unit: Option<OntologyAnnotation>): MaterialAttributeValue {
    return new MaterialAttributeValue(id, category, value, unit);
}

export function MaterialAttributeValue_create_163BDE77(Id?: string, Category?: MaterialAttribute, Value?: Value_$union, Unit?: OntologyAnnotation): MaterialAttributeValue {
    return MaterialAttributeValue_make(Id, Category, Value, Unit);
}

export function MaterialAttributeValue_get_empty(): MaterialAttributeValue {
    return MaterialAttributeValue_create_163BDE77();
}

/**
 * Returns the name of the category as string
 */
export function MaterialAttributeValue__get_NameText(this$: MaterialAttributeValue): string {
    return defaultArg(map<MaterialAttribute, string>(MaterialAttribute__get_NameText, this$.Category), "");
}

/**
 * Returns the name of the category as string
 */
export function MaterialAttributeValue__get_TryNameText(this$: MaterialAttributeValue): Option<string> {
    return bind<MaterialAttribute, string>(MaterialAttribute__get_TryNameText, this$.Category);
}

export function MaterialAttributeValue__get_ValueText(this$: MaterialAttributeValue): string {
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
            default:
                return OntologyAnnotation__get_NameText(oa.fields[0]);
        }
    }, this$.Value), "");
}

export function MaterialAttributeValue__get_ValueWithUnitText(this$: MaterialAttributeValue): string {
    const unit: Option<string> = map<OntologyAnnotation, string>(OntologyAnnotation__get_NameText, this$.Unit);
    const v: string = MaterialAttributeValue__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u: string = value_3(unit);
        return toText(printf("%s %s"))(v)(u);
    }
}

export function MaterialAttributeValue__MapCategory_65D42856(this$: MaterialAttributeValue, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): MaterialAttributeValue {
    return new MaterialAttributeValue(this$.ID, map<MaterialAttribute, MaterialAttribute>((p: MaterialAttribute): MaterialAttribute => MaterialAttribute__MapCategory_65D42856(p, f), this$.Category), this$.Value, this$.Unit);
}

export function MaterialAttributeValue__SetCategory_2FC95D30(this$: MaterialAttributeValue, c: OntologyAnnotation): MaterialAttributeValue {
    let matchValue: Option<MaterialAttribute>;
    return new MaterialAttributeValue(this$.ID, (matchValue = this$.Category, (matchValue == null) ? MaterialAttribute_create_2769312B(void 0, c) : MaterialAttribute__SetCategory_2FC95D30(value_3(matchValue), c)), this$.Value, this$.Unit);
}

/**
 * Returns the name of the characteristic value as string if it exists
 */
export function MaterialAttributeValue_tryGetNameText_6A64994C(mv: MaterialAttributeValue): Option<string> {
    return MaterialAttributeValue__get_TryNameText(mv);
}

/**
 * Returns the name of the characteristic value as string
 */
export function MaterialAttributeValue_getNameAsString_6A64994C(mv: MaterialAttributeValue): Option<string> {
    return MaterialAttributeValue__get_TryNameText(mv);
}

/**
 * Returns true if the given name matches the name of the characteristic value
 */
export function MaterialAttributeValue_nameEqualsString(name: string, mv: MaterialAttributeValue): boolean {
    return MaterialAttributeValue__get_NameText(mv) === name;
}

