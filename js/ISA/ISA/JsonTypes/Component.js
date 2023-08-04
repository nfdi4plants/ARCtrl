import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Value as Value_1, Value_fromString_Z721C83C5, Value__get_Text, Value_$reflection } from "./Value.js";
import { OntologyAnnotation_toString_5E3DAF0D, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_2EB0E147, OntologyAnnotation, OntologyAnnotation_fromTermAnnotation_Z721C83C5, OntologyAnnotation__get_TermAccessionShort, OntologyAnnotation__get_NameText, OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { create, match } from "../../../fable_modules/fable-library.4.1.4/RegExp.js";
import { AnnotationValue } from "./AnnotationValue.js";
import { map, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class Component extends Record {
    constructor(ComponentName, ComponentValue, ComponentUnit, ComponentType) {
        super();
        this.ComponentName = ComponentName;
        this.ComponentValue = ComponentValue;
        this.ComponentUnit = ComponentUnit;
        this.ComponentType = ComponentType;
    }
}

export function Component_$reflection() {
    return record_type("ARCtrl.ISA.Component", [], Component, () => [["ComponentName", option_type(string_type)], ["ComponentValue", option_type(Value_$reflection())], ["ComponentUnit", option_type(OntologyAnnotation_$reflection())], ["ComponentType", option_type(OntologyAnnotation_$reflection())]]);
}

export function Component_make(name, value, unit, componentType) {
    return new Component(name, value, unit, componentType);
}

export function Component_create_61502994(Name, Value, Unit, ComponentType) {
    return Component_make(Name, Value, Unit, ComponentType);
}

export function Component_get_empty() {
    return Component_create_61502994();
}

/**
 * This function creates a string containing full isa term triplet information about the component
 * 
 * Components do not have enough fields in ISA-JSON to include all existing ontology term information.
 * This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`.
 * Without this string composition we loose the ontology information for the header value.
 */
export function Component_composeName(value, unit) {
    if (value == null) {
        return "";
    }
    else if (value.tag === 0) {
        const oa = value.fields[0];
        return `${OntologyAnnotation__get_NameText(oa)} (${OntologyAnnotation__get_TermAccessionShort(oa)})`;
    }
    else if (unit != null) {
        const u = unit;
        const v_1 = value;
        return `${Value__get_Text(v_1)} ${OntologyAnnotation__get_NameText(u)} (${OntologyAnnotation__get_TermAccessionShort(u)})`;
    }
    else {
        const v = value;
        return `${Value__get_Text(v)}`;
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
    const r = match(create("(?<value>[^\\(]+) \\((?<ontology>[^(]*:[^)]*)\\)"), name);
    const unitr = match(create("(?<value>[\\d\\.]+) (?<unit>.+) \\((?<ontology>[^(]*:[^)]*)\\)"), name);
    if (unitr != null) {
        const oa = OntologyAnnotation_fromTermAnnotation_Z721C83C5((unitr.groups && unitr.groups.ontology) || "");
        return [Value_fromString_Z721C83C5((unitr.groups && unitr.groups.value) || ""), new OntologyAnnotation(oa.ID, new AnnotationValue(0, [(unitr.groups && unitr.groups.unit) || ""]), oa.TermSourceREF, oa.LocalID, oa.TermAccessionNumber, oa.Comments)];
    }
    else if (r != null) {
        const oa_1 = OntologyAnnotation_fromTermAnnotation_Z721C83C5((r.groups && r.groups.ontology) || "");
        return [new Value_1(0, [new OntologyAnnotation(oa_1.ID, new AnnotationValue(0, [Value__get_Text(Value_fromString_Z721C83C5((r.groups && r.groups.value) || ""))]), oa_1.TermSourceREF, oa_1.LocalID, oa_1.TermAccessionNumber, oa_1.Comments)]), void 0];
    }
    else {
        return [new Value_1(3, [name]), void 0];
    }
}

/**
 * Create a ISAJson Component from ISATab string entries
 */
export function Component_fromString_Z61E08C1(name, term, source, accession, comments) {
    let cType;
    const v = OntologyAnnotation_fromString_2EB0E147(unwrap(term), unwrap(source), unwrap(accession), unwrap(comments));
    cType = fromValueWithDefault(OntologyAnnotation_get_empty(), v);
    if (name == null) {
        return Component_make(void 0, void 0, void 0, cType);
    }
    else {
        const patternInput = Component_decomposeName_Z721C83C5(name);
        return Component_make(name, fromValueWithDefault(new Value_1(3, [""]), patternInput[0]), patternInput[1], cType);
    }
}

export function Component_fromOptions(value, unit, header) {
    return Component_make(fromValueWithDefault("", Component_composeName(value, unit)), value, unit, header);
}

/**
 * Get ISATab string entries from an ISAJson Component object
 */
export function Component_toString_Z609B8895(c) {
    let oa;
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    oa = defaultArg(map(OntologyAnnotation_toString_5E3DAF0D, c.ComponentType), value);
    return [defaultArg(c.ComponentName, ""), oa];
}

export function Component__get_NameText(this$) {
    return defaultArg(map(OntologyAnnotation__get_NameText, this$.ComponentType), "");
}

/**
 * Returns the ontology of the category of the Value as string
 */
export function Component__get_UnitText(this$) {
    return defaultArg(map(OntologyAnnotation__get_NameText, this$.ComponentUnit), "");
}

export function Component__get_ValueText(this$) {
    return defaultArg(map(Value__get_Text, this$.ComponentValue), "");
}

export function Component__get_ValueWithUnitText(this$) {
    const unit = map(OntologyAnnotation__get_NameText, this$.ComponentUnit);
    const v = Component__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u = unit;
        return toText(printf("%s %s"))(v)(u);
    }
}

export function Component__MapCategory_Z69DD836A(this$, f) {
    return new Component(this$.ComponentName, this$.ComponentValue, this$.ComponentUnit, map(f, this$.ComponentType));
}

export function Component__SetCategory_Z4C0FE73C(this$, c) {
    return new Component(this$.ComponentName, this$.ComponentValue, this$.ComponentUnit, c);
}

