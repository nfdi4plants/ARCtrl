import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation, OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { bind, map, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { fromValueWithDefault } from "../OptionExtensions.js";

export class ProtocolParameter extends Record {
    constructor(ID, ParameterName) {
        super();
        this.ID = ID;
        this.ParameterName = ParameterName;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + ProtocolParameter__get_NameText(this$);
    }
}

export function ProtocolParameter_$reflection() {
    return record_type("ARCtrl.ISA.ProtocolParameter", [], ProtocolParameter, () => [["ID", option_type(string_type)], ["ParameterName", option_type(OntologyAnnotation_$reflection())]]);
}

export function ProtocolParameter_make(id, parameterName) {
    return new ProtocolParameter(id, parameterName);
}

export function ProtocolParameter_create_Z6C54B221(Id, ParameterName) {
    return ProtocolParameter_make(Id, ParameterName);
}

export function ProtocolParameter_get_empty() {
    return ProtocolParameter_create_Z6C54B221();
}

/**
 * Create a ISAJson Protocol Parameter from ISATab string entries
 */
export function ProtocolParameter_fromString_Z2304A83C(term, source, accession, comments) {
    const oa = OntologyAnnotation.fromString(term, source, accession, unwrap(comments));
    return ProtocolParameter_make(void 0, fromValueWithDefault(OntologyAnnotation.empty, oa));
}

/**
 * Get ISATab string entries from an ISAJson ProtocolParameter object (name,source,accession)
 */
export function ProtocolParameter_toString_Z3A4310A5(pp) {
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map((arg) => OntologyAnnotation.toString(arg), pp.ParameterName), value);
}

/**
 * Returns the name of the parameter as string
 */
export function ProtocolParameter__get_NameText(this$) {
    return defaultArg(map((oa) => oa.NameText, this$.ParameterName), "");
}

/**
 * Returns the name of the parameter as string
 */
export function ProtocolParameter__get_TryNameText(this$) {
    return bind((oa) => oa.TryNameText, this$.ParameterName);
}

export function ProtocolParameter__MapCategory_Z69DD836A(this$, f) {
    return new ProtocolParameter(this$.ID, map(f, this$.ParameterName));
}

export function ProtocolParameter__SetCategory_Z4C0FE73C(this$, c) {
    return new ProtocolParameter(this$.ID, c);
}

/**
 * Returns the name of the paramater as string if it exists
 */
export function ProtocolParameter_tryGetNameText_Z3A4310A5(pp) {
    return ProtocolParameter__get_TryNameText(pp);
}

/**
 * Returns the name of the paramater as string
 */
export function ProtocolParameter_getNameText_Z3A4310A5(pp) {
    return defaultArg(ProtocolParameter_tryGetNameText_Z3A4310A5(pp), "");
}

/**
 * Returns true if the given name matches the name of the parameter
 */
export function ProtocolParameter_nameEqualsString(name, pp) {
    return ProtocolParameter__get_NameText(pp) === name;
}

