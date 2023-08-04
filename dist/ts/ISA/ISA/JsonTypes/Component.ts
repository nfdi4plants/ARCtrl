import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { map, defaultArg, unwrap, value as value_2, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Value_Name, Value_Ontology, Value_fromString_Z721C83C5, Value__get_Text, Value as Value_1, Value_$reflection, Value_$union } from "./Value.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { create, match } from "../../../fable_modules/fable-library-ts/RegExp.js";
import { AnnotationValue_Text } from "./AnnotationValue.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Comment$ } from "./Comment.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";

export class Component extends Record implements IEquatable<Component> {
    readonly ComponentName: Option<string>;
    readonly ComponentValue: Option<Value_$union>;
    readonly ComponentUnit: Option<OntologyAnnotation>;
    readonly ComponentType: Option<OntologyAnnotation>;
    constructor(ComponentName: Option<string>, ComponentValue: Option<Value_$union>, ComponentUnit: Option<OntologyAnnotation>, ComponentType: Option<OntologyAnnotation>) {
        super();
        this.ComponentName = ComponentName;
        this.ComponentValue = ComponentValue;
        this.ComponentUnit = ComponentUnit;
        this.ComponentType = ComponentType;
    }
}

export function Component_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Component", [], Component, () => [["ComponentName", option_type(string_type)], ["ComponentValue", option_type(Value_$reflection())], ["ComponentUnit", option_type(OntologyAnnotation_$reflection())], ["ComponentType", option_type(OntologyAnnotation_$reflection())]]);
}

export function Component_make(name: Option<string>, value: Option<Value_$union>, unit: Option<OntologyAnnotation>, componentType: Option<OntologyAnnotation>): Component {
    return new Component(name, value, unit, componentType);
}

export function Component_create_61502994(Name?: string, Value?: Value_$union, Unit?: OntologyAnnotation, ComponentType?: OntologyAnnotation): Component {
    return Component_make(Name, Value, Unit, ComponentType);
}

export function Component_get_empty(): Component {
    return Component_create_61502994();
}

/**
 * This function creates a string containing full isa term triplet information about the component
 * 
 * Components do not have enough fields in ISA-JSON to include all existing ontology term information.
 * This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`.
 * Without this string composition we loose the ontology information for the header value.
 */
export function Component_composeName(value: Option<Value_$union>, unit: Option<OntologyAnnotation>): string {
    if (value == null) {
        return "";
    }
    else if (value_2(value).tag === /* Ontology */ 0) {
        const oa: OntologyAnnotation = (value_2(value) as Value_1<0>).fields[0];
        return `${oa.NameText} (${oa.TermAccessionShort})`;
    }
    else if (unit != null) {
        const u: OntologyAnnotation = value_2(unit);
        const v_1: Value_$union = value_2(value);
        return `${Value__get_Text(v_1)} ${u.NameText} (${u.TermAccessionShort})`;
    }
    else {
        const v: Value_$union = value_2(value);
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
export function Component_decomposeName_Z721C83C5(name: string): [Value_$union, Option<OntologyAnnotation>] {
    const r: any = match(create("(?<value>[^\\(]+) \\((?<ontology>[^(]*:[^)]*)\\)"), name);
    const unitr: any = match(create("(?<value>[\\d\\.]+) (?<unit>.+) \\((?<ontology>[^(]*:[^)]*)\\)"), name);
    if (unitr != null) {
        let oa: OntologyAnnotation;
        const arg: string = (unitr.groups && unitr.groups.ontology) || "";
        oa = OntologyAnnotation.fromTermAnnotation(arg);
        return [Value_fromString_Z721C83C5((unitr.groups && unitr.groups.value) || ""), new OntologyAnnotation(oa.ID, AnnotationValue_Text((unitr.groups && unitr.groups.unit) || ""), oa.TermSourceREF, oa.LocalID, oa.TermAccessionNumber, oa.Comments)] as [Value_$union, Option<OntologyAnnotation>];
    }
    else if (r != null) {
        let oa_1: OntologyAnnotation;
        const arg_2: string = (r.groups && r.groups.ontology) || "";
        oa_1 = OntologyAnnotation.fromTermAnnotation(arg_2);
        return [Value_Ontology(new OntologyAnnotation(oa_1.ID, AnnotationValue_Text(Value__get_Text(Value_fromString_Z721C83C5((r.groups && r.groups.value) || ""))), oa_1.TermSourceREF, oa_1.LocalID, oa_1.TermAccessionNumber, oa_1.Comments)), void 0] as [Value_$union, Option<OntologyAnnotation>];
    }
    else {
        return [Value_Name(name), void 0] as [Value_$union, Option<OntologyAnnotation>];
    }
}

/**
 * Create a ISAJson Component from ISATab string entries
 */
export function Component_fromString_Z61E08C1(name?: string, term?: string, source?: string, accession?: string, comments?: Comment$[]): Component {
    let cType: Option<OntologyAnnotation>;
    const v: OntologyAnnotation = OntologyAnnotation.fromString(unwrap(term), unwrap(source), unwrap(accession), unwrap(comments));
    cType = fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, v);
    if (name == null) {
        return Component_make(void 0, void 0, void 0, cType);
    }
    else {
        const patternInput: [Value_$union, Option<OntologyAnnotation>] = Component_decomposeName_Z721C83C5(value_2(name));
        return Component_make(name, fromValueWithDefault<Value_$union>(Value_Name(""), patternInput[0]), patternInput[1], cType);
    }
}

export function Component_fromOptions(value: Option<Value_$union>, unit: Option<OntologyAnnotation>, header: Option<OntologyAnnotation>): Component {
    return Component_make(fromValueWithDefault<string>("", Component_composeName(value, unit)), value, unit, header);
}

/**
 * Get ISATab string entries from an ISAJson Component object
 */
export function Component_toString_Z609B8895(c: Component): [string, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }] {
    let oa: { TermAccessionNumber: string, TermName: string, TermSourceREF: string };
    const value: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    oa = defaultArg(map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>((arg: OntologyAnnotation): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } => OntologyAnnotation.toString(arg), c.ComponentType), value);
    return [defaultArg(c.ComponentName, ""), oa] as [string, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }];
}

export function Component__get_NameText(this$: Component): string {
    return defaultArg(map<OntologyAnnotation, string>((c: OntologyAnnotation): string => c.NameText, this$.ComponentType), "");
}

/**
 * Returns the ontology of the category of the Value as string
 */
export function Component__get_UnitText(this$: Component): string {
    return defaultArg(map<OntologyAnnotation, string>((c: OntologyAnnotation): string => c.NameText, this$.ComponentUnit), "");
}

export function Component__get_ValueText(this$: Component): string {
    return defaultArg(map<Value_$union, string>(Value__get_Text, this$.ComponentValue), "");
}

export function Component__get_ValueWithUnitText(this$: Component): string {
    const unit: Option<string> = map<OntologyAnnotation, string>((oa: OntologyAnnotation): string => oa.NameText, this$.ComponentUnit);
    const v: string = Component__get_ValueText(this$);
    if (unit == null) {
        return v;
    }
    else {
        const u: string = value_2(unit);
        return toText(printf("%s %s"))(v)(u);
    }
}

export function Component__MapCategory_Z69DD836A(this$: Component, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): Component {
    return new Component(this$.ComponentName, this$.ComponentValue, this$.ComponentUnit, map<OntologyAnnotation, OntologyAnnotation>(f, this$.ComponentType));
}

export function Component__SetCategory_Z4C0FE73C(this$: Component, c: OntologyAnnotation): Component {
    return new Component(this$.ComponentName, this$.ComponentValue, this$.ComponentUnit, c);
}

