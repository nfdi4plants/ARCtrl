import { Record } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { Value, Value_$reflection } from "../Value.js";
import { record_type, option_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { OntologyAnnotation, OntologyAnnotation_$reflection } from "../OntologyAnnotation.js";
import { defaultArg, unwrap, map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ActivePatterns_$007CRegex$007C_$007C } from "../Helper/Regex.js";
import { Option_fromValueWithDefault } from "../Helper/Collections.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";

export class Component extends Record {
    constructor(ComponentValue, ComponentUnit, ComponentType) {
        super();
        this.ComponentValue = ComponentValue;
        this.ComponentUnit = ComponentUnit;
        this.ComponentType = ComponentType;
    }
    AlternateName() {
        const this$ = this;
        return Component__get_ComponentName(this$);
    }
    MeasurementMethod() {
        return undefined;
    }
    Description() {
        return undefined;
    }
    GetCategory() {
        const this$ = this;
        return this$.ComponentType;
    }
    GetAdditionalType() {
        return "Component";
    }
    GetValue() {
        const this$ = this;
        return this$.ComponentValue;
    }
    GetUnit() {
        const this$ = this;
        return this$.ComponentUnit;
    }
}

export function Component_$reflection() {
    return record_type("ARCtrl.Process.Component", [], Component, () => [["ComponentValue", option_type(Value_$reflection())], ["ComponentUnit", option_type(OntologyAnnotation_$reflection())], ["ComponentType", option_type(OntologyAnnotation_$reflection())]]);
}

export function Component__get_ComponentName(this$) {
    return map((v) => Component_composeName(v, this$.ComponentUnit), this$.ComponentValue);
}

export function Component_make(value, unit, componentType) {
    return new Component(value, unit, componentType);
}

export function Component_create_Z2F0B38C7(value, unit, componentType) {
    return Component_make(value, unit, componentType);
}

export function Component_get_empty() {
    return Component_create_Z2F0B38C7();
}

/**
 * This function creates a string containing full isa term triplet information about the component
 * 
 * Components do not have enough fields in ISA-JSON to include all existing ontology term information.
 * This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`.
 * Without this string composition we loose the ontology information for the header value.
 */
export function Component_composeName(value, unit) {
    if (value.tag === 0) {
        const oa = value.fields[0];
        return `${oa.NameText} (${oa.TermAccessionShort})`;
    }
    else if (unit != null) {
        const u = unit;
        return `${value.Text} ${u.NameText} (${u.TermAccessionShort})`;
    }
    else {
        return `${value.Text}`;
    }
}

/**
 * This function parses the given Component header string format into the ISA-JSON Component type
 * 
 * Components do not have enough fields in ISA-JSON to include all existing ontology term information.
 * This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`.
 * Without this string composition we loose the ontology information for the header value.
 */
export function Component_decomposeName_Z721C83C5(name) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("^(?<value>[\\d\\.]+) (?<unit>.+) \\((?<ontology>[^(]*:[^)]*)\\)", name);
    if (activePatternResult != null) {
        const unitr = activePatternResult;
        let oa;
        const tan = (unitr.groups && unitr.groups.ontology) || "";
        oa = OntologyAnnotation.fromTermAnnotation(tan);
        let v;
        const value = (unitr.groups && unitr.groups.value) || "";
        v = Value.fromString(value);
        const u = (unitr.groups && unitr.groups.unit) || "";
        oa.Name = u;
        return [v, oa];
    }
    else {
        const activePatternResult_1 = ActivePatterns_$007CRegex$007C_$007C("^(?<value>[^\\(]+) \\((?<ontology>[^(]*:[^)]*)\\)", name);
        if (activePatternResult_1 != null) {
            const r = activePatternResult_1;
            let oa_1;
            const tan_1 = (r.groups && r.groups.ontology) || "";
            oa_1 = OntologyAnnotation.fromTermAnnotation(tan_1);
            const v_1 = (r.groups && r.groups.value) || "";
            oa_1.Name = v_1;
            return [new Value(0, [oa_1]), undefined];
        }
        else {
            const activePatternResult_2 = ActivePatterns_$007CRegex$007C_$007C("^(?<value>[^\\(\\)]+) \\(\\)", name);
            if (activePatternResult_2 != null) {
                const r_1 = activePatternResult_2;
                return [new Value(0, [new OntologyAnnotation((r_1.groups && r_1.groups.value) || "")]), undefined];
            }
            else {
                return [new Value(3, [name]), undefined];
            }
        }
    }
}

/**
 * Create a ISAJson Component from ISATab string entries
 */
export function Component_fromISAString_7C9A7CF8(name, term, source, accession, comments) {
    let cType;
    const v = OntologyAnnotation.create(unwrap(term), unwrap(source), unwrap(accession), unwrap(comments));
    cType = Option_fromValueWithDefault(new OntologyAnnotation(), v);
    if (name == null) {
        return Component_make(undefined, undefined, cType);
    }
    else {
        const patternInput = Component_decomposeName_Z721C83C5(name);
        return Component_make(Option_fromValueWithDefault(new Value(3, [""]), patternInput[0]), patternInput[1], cType);
    }
}

/**
 * Get ISATab string entries from an ISAJson Component object
 */
export function Component_toStringObject_Z685B8F25(c) {
    let oa_1;
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    oa_1 = defaultArg(map((oa) => OntologyAnnotation.toStringObject(oa), c.ComponentType), value);
    return [defaultArg(Component__get_ComponentName(c), ""), oa_1];
}

export function Component__get_NameText(this$) {
    return defaultArg(map((c) => c.NameText, this$.ComponentType), "");
}

/**
 * Returns the ontology of the category of the Value as string
 */
export function Component__get_UnitText(this$) {
    return defaultArg(map((c) => c.NameText, this$.ComponentUnit), "");
}

export function Component__get_ValueText(this$) {
    return defaultArg(map((c) => c.Text, this$.ComponentValue), "");
}

export function Component__get_ValueWithUnitText(this$) {
    const unit = map((oa) => oa.NameText, this$.ComponentUnit);
    const v = Component__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u = unit;
        return toText(printf("%s %s"))(v)(u);
    }
}

export function Component__MapCategory_658CFBF6(this$, f) {
    return new Component(this$.ComponentValue, this$.ComponentUnit, map(f, this$.ComponentType));
}

export function Component__SetCategory_ZDED3A0F(this$, c) {
    return new Component(this$.ComponentValue, this$.ComponentUnit, c);
}

export function Component_createAsPV(alternateName, measurementMethod, description, category, value, unit) {
    return Component_create_Z2F0B38C7(unwrap(value), unwrap(unit), unwrap(category));
}

