import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { bind, defaultArg, value as value_3, Option, map } from "../../../fable_modules/fable-library-ts/Option.js";
import { ProtocolParameter__SetCategory_Z4C0FE73C, ProtocolParameter_create_Z6C54B221, ProtocolParameter__MapCategory_Z69DD836A, ProtocolParameter__get_TryNameText, ProtocolParameter_$reflection, ProtocolParameter, ProtocolParameter__get_NameText } from "./ProtocolParameter.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IISAPrintable } from "../Printer.js";
import { Value_$reflection, Value_$union } from "./Value.js";
import { int32ToString, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { float64 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";

export class ProcessParameterValue extends Record implements IEquatable<ProcessParameterValue>, IISAPrintable {
    readonly Category: Option<ProtocolParameter>;
    readonly Value: Option<Value_$union>;
    readonly Unit: Option<OntologyAnnotation>;
    constructor(Category: Option<ProtocolParameter>, Value: Option<Value_$union>, Unit: Option<OntologyAnnotation>) {
        super();
        this.Category = Category;
        this.Value = Value;
        this.Unit = Unit;
    }
    Print(): string {
        const this$: ProcessParameterValue = this;
        return toString(this$);
    }
    PrintCompact(): string {
        let value_2: string, category_2: string, category_1: string, value_1: string;
        const this$: ProcessParameterValue = this;
        const category: Option<string> = map<ProtocolParameter, string>(ProtocolParameter__get_NameText, this$.Category);
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

export function ProcessParameterValue_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.ProcessParameterValue", [], ProcessParameterValue, () => [["Category", option_type(ProtocolParameter_$reflection())], ["Value", option_type(Value_$reflection())], ["Unit", option_type(OntologyAnnotation_$reflection())]]);
}

export function ProcessParameterValue_make(category: Option<ProtocolParameter>, value: Option<Value_$union>, unit: Option<OntologyAnnotation>): ProcessParameterValue {
    return new ProcessParameterValue(category, value, unit);
}

export function ProcessParameterValue_create_569825F3(Category?: ProtocolParameter, Value?: Value_$union, Unit?: OntologyAnnotation): ProcessParameterValue {
    return ProcessParameterValue_make(Category, Value, Unit);
}

export function ProcessParameterValue_get_empty(): ProcessParameterValue {
    return ProcessParameterValue_create_569825F3();
}

/**
 * Returns the name of the category as string
 */
export function ProcessParameterValue__get_NameText(this$: ProcessParameterValue): string {
    return defaultArg(map<ProtocolParameter, string>(ProtocolParameter__get_NameText, this$.Category), "");
}

/**
 * Returns the name of the category as string
 */
export function ProcessParameterValue__get_TryNameText(this$: ProcessParameterValue): Option<string> {
    return bind<ProtocolParameter, string>(ProtocolParameter__get_TryNameText, this$.Category);
}

export function ProcessParameterValue__get_ValueText(this$: ProcessParameterValue): string {
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

export function ProcessParameterValue__get_ValueWithUnitText(this$: ProcessParameterValue): string {
    const unit: Option<string> = map<OntologyAnnotation, string>((oa: OntologyAnnotation): string => oa.NameText, this$.Unit);
    const v: string = ProcessParameterValue__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u: string = value_3(unit);
        return toText(printf("%s %s"))(v)(u);
    }
}

export function ProcessParameterValue__MapCategory_Z69DD836A(this$: ProcessParameterValue, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): ProcessParameterValue {
    return new ProcessParameterValue(map<ProtocolParameter, ProtocolParameter>((p: ProtocolParameter): ProtocolParameter => ProtocolParameter__MapCategory_Z69DD836A(p, f), this$.Category), this$.Value, this$.Unit);
}

export function ProcessParameterValue__SetCategory_Z4C0FE73C(this$: ProcessParameterValue, c: OntologyAnnotation): ProcessParameterValue {
    let matchValue: Option<ProtocolParameter>;
    return new ProcessParameterValue((matchValue = this$.Category, (matchValue == null) ? ProtocolParameter_create_Z6C54B221(void 0, c) : ProtocolParameter__SetCategory_Z4C0FE73C(value_3(matchValue), c)), this$.Value, this$.Unit);
}

/**
 * Returns the name of the paramater value as string if it exists
 */
export function ProcessParameterValue_tryGetNameText_5FD7232D(pv: ProcessParameterValue): Option<string> {
    return ProcessParameterValue__get_TryNameText(pv);
}

/**
 * Returns the name of the paramater value as string
 */
export function ProcessParameterValue_getNameText_5FD7232D(pv: ProcessParameterValue): string {
    return ProcessParameterValue__get_NameText(pv);
}

/**
 * Returns true if the given name matches the name of the parameter value
 */
export function ProcessParameterValue_nameEqualsString(name: string, pv: ProcessParameterValue): boolean {
    return ProcessParameterValue__get_NameText(pv) === name;
}

export function ProcessParameterValue_getCategory_5FD7232D(pv: ProcessParameterValue): Option<ProtocolParameter> {
    return pv.Category;
}

