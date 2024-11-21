import { bind, unwrap, map, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { int32ToString } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { ProtocolParameter_$reflection, ProtocolParameter } from "./ProtocolParameter.js";
import { Record, toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, option_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { Value_$reflection } from "../Value.js";
import { OntologyAnnotation_$reflection } from "../OntologyAnnotation.js";

export class ProcessParameterValue extends Record {
    constructor(Category, Value, Unit) {
        super();
        this.Category = Category;
        this.Value = Value;
        this.Unit = Unit;
    }
    static make(category, value, unit) {
        return new ProcessParameterValue(category, value, unit);
    }
    static create(Category, Value, Unit) {
        return ProcessParameterValue.make(Category, Value, Unit);
    }
    static get empty() {
        return ProcessParameterValue.create();
    }
    get NameText() {
        const this$ = this;
        return defaultArg(map((oa) => oa.NameText, this$.Category), "");
    }
    get TryNameText() {
        const this$ = this;
        return unwrap(bind((oa) => oa.TryNameText, this$.Category));
    }
    get ValueText() {
        const this$ = this;
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
    get ValueWithUnitText() {
        const this$ = this;
        const unit = map((oa) => oa.NameText, this$.Unit);
        const v = this$.ValueText;
        if (unit == null) {
            return v;
        }
        else {
            const u = unit;
            return toText(printf("%s %s"))(v)(u);
        }
    }
    MapCategory(f) {
        const this$ = this;
        return new ProcessParameterValue(map((p) => p.MapCategory(f), this$.Category), this$.Value, this$.Unit);
    }
    SetCategory(c) {
        let matchValue, p;
        const this$ = this;
        return new ProcessParameterValue((matchValue = this$.Category, (matchValue == null) ? ProtocolParameter.create(undefined, c) : ((p = matchValue, p.SetCategory(c)))), this$.Value, this$.Unit);
    }
    static tryGetNameText(pv) {
        return pv.TryNameText;
    }
    static getNameText(pv) {
        return pv.NameText;
    }
    static nameEqualsString(name, pv) {
        return pv.NameText === name;
    }
    static getCategory(pv) {
        return pv.Category;
    }
    static createAsPV(alternateName, measurementMethod, description, category, value, unit) {
        const category_1 = map((c) => ProtocolParameter.create(undefined, c), category);
        return ProcessParameterValue.create(unwrap(category_1), unwrap(value), unwrap(unit));
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        let value_2, category_2, category_1, value_1;
        const this$ = this;
        const category = map((f) => f.NameText, this$.Category);
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
    AlternateName() {
        return undefined;
    }
    MeasurementMethod() {
        return undefined;
    }
    Description() {
        return undefined;
    }
    GetCategory() {
        const this$ = this;
        return bind((p) => p.ParameterName, this$.Category);
    }
    GetAdditionalType() {
        return "ProcessParameterValue";
    }
    GetValue() {
        const this$ = this;
        return this$.Value;
    }
    GetUnit() {
        const this$ = this;
        return this$.Unit;
    }
}

export function ProcessParameterValue_$reflection() {
    return record_type("ARCtrl.Process.ProcessParameterValue", [], ProcessParameterValue, () => [["Category", option_type(ProtocolParameter_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

