import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation__get_TryNameText, OntologyAnnotation__get_NameText, OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { bind, map, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { fromValueWithDefault } from "../OptionExtensions.js";

export class MaterialAttribute extends Record {
    constructor(ID, CharacteristicType) {
        super();
        this.ID = ID;
        this.CharacteristicType = CharacteristicType;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + MaterialAttribute__get_NameText(this$);
    }
}

export function MaterialAttribute_$reflection() {
    return record_type("ISA.MaterialAttribute", [], MaterialAttribute, () => [["ID", option_type(string_type)], ["CharacteristicType", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttribute_make(id, characteristicType) {
    return new MaterialAttribute(id, characteristicType);
}

export function MaterialAttribute_create_2769312B(Id, CharacteristicType) {
    return MaterialAttribute_make(Id, CharacteristicType);
}

export function MaterialAttribute_get_empty() {
    return MaterialAttribute_create_2769312B();
}

/**
 * Create a ISAJson MaterialAttribute from ISATab string entries
 */
export function MaterialAttribute_fromString_703AFBF9(term, source, accession, comments) {
    const oa = OntologyAnnotation_fromString_Z7D8EB286(term, source, accession, unwrap(comments));
    return MaterialAttribute_make(void 0, fromValueWithDefault(OntologyAnnotation_get_empty(), oa));
}

/**
 * Get ISATab string entries from an ISAJson MaterialAttribute object
 */
export function MaterialAttribute_toString_Z6476E859(ma) {
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map(OntologyAnnotation_toString_473B9D79, ma.CharacteristicType), value);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_NameText(this$) {
    return defaultArg(map(OntologyAnnotation__get_NameText, this$.CharacteristicType), "");
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_TryNameText(this$) {
    return bind(OntologyAnnotation__get_TryNameText, this$.CharacteristicType);
}

export function MaterialAttribute__MapCategory_65D42856(this$, f) {
    return new MaterialAttribute(this$.ID, map(f, this$.CharacteristicType));
}

export function MaterialAttribute__SetCategory_2FC95D30(this$, c) {
    return new MaterialAttribute(this$.ID, c);
}

/**
 * Returns the name of the characteristic as string if it exists
 */
export function MaterialAttribute_tryGetNameText_Z6476E859(ma) {
    return MaterialAttribute__get_NameText(ma);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute_getNameText_Z6476E859(ma) {
    return MaterialAttribute__get_TryNameText(ma);
}

/**
 * Returns true if the given name matches the name of the characteristic
 */
export function MaterialAttribute_nameEqualsString(name, ma) {
    return MaterialAttribute__get_NameText(ma) === name;
}

