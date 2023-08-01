import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation__get_TryNameText, OntologyAnnotation__get_NameText, OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
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
    return record_type("ISA.ProtocolParameter", [], ProtocolParameter, () => [["ID", option_type(string_type)], ["ParameterName", option_type(OntologyAnnotation_$reflection())]]);
}

export function ProtocolParameter_make(id, parameterName) {
    return new ProtocolParameter(id, parameterName);
}

export function ProtocolParameter_create_2769312B(Id, ParameterName) {
    return ProtocolParameter_make(Id, ParameterName);
}

export function ProtocolParameter_get_empty() {
    return ProtocolParameter_create_2769312B();
}

/**
 * Create a ISAJson Protocol Parameter from ISATab string entries
 */
export function ProtocolParameter_fromString_703AFBF9(term, source, accession, comments) {
    const oa = OntologyAnnotation_fromString_Z7D8EB286(term, source, accession, unwrap(comments));
    return ProtocolParameter_make(void 0, fromValueWithDefault(OntologyAnnotation_get_empty(), oa));
}

/**
 * Get ISATab string entries from an ISAJson ProtocolParameter object (name,source,accession)
 */
export function ProtocolParameter_toString_2762A46F(pp) {
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map(OntologyAnnotation_toString_473B9D79, pp.ParameterName), value);
}

/**
 * Returns the name of the parameter as string
 */
export function ProtocolParameter__get_NameText(this$) {
    return defaultArg(map(OntologyAnnotation__get_NameText, this$.ParameterName), "");
}

/**
 * Returns the name of the parameter as string
 */
export function ProtocolParameter__get_TryNameText(this$) {
    return bind(OntologyAnnotation__get_TryNameText, this$.ParameterName);
}

export function ProtocolParameter__MapCategory_65D42856(this$, f) {
    return new ProtocolParameter(this$.ID, map(f, this$.ParameterName));
}

export function ProtocolParameter__SetCategory_2FC95D30(this$, c) {
    return new ProtocolParameter(this$.ID, c);
}

/**
 * Returns the name of the paramater as string if it exists
 */
export function ProtocolParameter_tryGetNameText_2762A46F(pp) {
    return ProtocolParameter__get_TryNameText(pp);
}

/**
 * Returns the name of the paramater as string
 */
export function ProtocolParameter_getNameText_2762A46F(pp) {
    return defaultArg(ProtocolParameter_tryGetNameText_2762A46F(pp), "");
}

/**
 * Returns true if the given name matches the name of the parameter
 */
export function ProtocolParameter_nameEqualsString(name, pp) {
    return ProtocolParameter__get_NameText(pp) === name;
}

