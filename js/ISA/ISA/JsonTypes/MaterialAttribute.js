import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation__get_TryNameText, OntologyAnnotation__get_NameText, OntologyAnnotation_toString_5E3DAF0D, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_2EB0E147, OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
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
    return record_type("ARCtrl.ISA.MaterialAttribute", [], MaterialAttribute, () => [["ID", option_type(string_type)], ["CharacteristicType", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttribute_make(id, characteristicType) {
    return new MaterialAttribute(id, characteristicType);
}

export function MaterialAttribute_create_Z6C54B221(Id, CharacteristicType) {
    return MaterialAttribute_make(Id, CharacteristicType);
}

export function MaterialAttribute_get_empty() {
    return MaterialAttribute_create_Z6C54B221();
}

/**
 * Create a ISAJson MaterialAttribute from ISATab string entries
 */
export function MaterialAttribute_fromString_Z2304A83C(term, source, accession, comments) {
    const oa = OntologyAnnotation_fromString_2EB0E147(term, source, accession, unwrap(comments));
    return MaterialAttribute_make(void 0, fromValueWithDefault(OntologyAnnotation_get_empty(), oa));
}

/**
 * Get ISATab string entries from an ISAJson MaterialAttribute object
 */
export function MaterialAttribute_toString_Z5F39696D(ma) {
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map(OntologyAnnotation_toString_5E3DAF0D, ma.CharacteristicType), value);
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

export function MaterialAttribute__MapCategory_Z69DD836A(this$, f) {
    return new MaterialAttribute(this$.ID, map(f, this$.CharacteristicType));
}

export function MaterialAttribute__SetCategory_Z4C0FE73C(this$, c) {
    return new MaterialAttribute(this$.ID, c);
}

/**
 * Returns the name of the characteristic as string if it exists
 */
export function MaterialAttribute_tryGetNameText_Z5F39696D(ma) {
    return MaterialAttribute__get_NameText(ma);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute_getNameText_Z5F39696D(ma) {
    return MaterialAttribute__get_TryNameText(ma);
}

/**
 * Returns true if the given name matches the name of the characteristic
 */
export function MaterialAttribute_nameEqualsString(name, ma) {
    return MaterialAttribute__get_NameText(ma) === name;
}

