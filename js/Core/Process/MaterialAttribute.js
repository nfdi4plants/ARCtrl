import { Record, toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, option_type, string_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { OntologyAnnotation, OntologyAnnotation_$reflection } from "../OntologyAnnotation.js";
import { bind, map, defaultArg, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { Option_fromValueWithDefault } from "../Helper/Collections.js";

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
    return record_type("ARCtrl.Process.MaterialAttribute", [], MaterialAttribute, () => [["ID", option_type(string_type)], ["CharacteristicType", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttribute_make(id, characteristicType) {
    return new MaterialAttribute(id, characteristicType);
}

export function MaterialAttribute_create_A220A8A(Id, CharacteristicType) {
    return MaterialAttribute_make(Id, CharacteristicType);
}

export function MaterialAttribute_get_empty() {
    return MaterialAttribute_create_A220A8A();
}

/**
 * Create a ISAJson MaterialAttribute from ISATab string entries
 */
export function MaterialAttribute_fromString_5980DC03(term, source, accession, comments) {
    const oa = OntologyAnnotation.create(term, source, accession, unwrap(comments));
    return MaterialAttribute_make(undefined, Option_fromValueWithDefault(new OntologyAnnotation(), oa));
}

/**
 * Get ISATab string entries from an ISAJson MaterialAttribute object
 */
export function MaterialAttribute_toStringObject_Z1E3B85DD(ma) {
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map((oa) => OntologyAnnotation.toStringObject(oa), ma.CharacteristicType), value);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_NameText(this$) {
    return defaultArg(map((oa) => oa.NameText, this$.CharacteristicType), "");
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_TryNameText(this$) {
    return bind((oa) => oa.Name, this$.CharacteristicType);
}

export function MaterialAttribute__MapCategory_658CFBF6(this$, f) {
    return new MaterialAttribute(this$.ID, map(f, this$.CharacteristicType));
}

export function MaterialAttribute__SetCategory_ZDED3A0F(this$, c) {
    return new MaterialAttribute(this$.ID, c);
}

/**
 * Returns the name of the characteristic as string if it exists
 */
export function MaterialAttribute_tryGetNameText_Z1E3B85DD(ma) {
    return MaterialAttribute__get_NameText(ma);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute_getNameText_Z1E3B85DD(ma) {
    return MaterialAttribute__get_TryNameText(ma);
}

/**
 * Returns true if the given name matches the name of the characteristic
 */
export function MaterialAttribute_nameEqualsString(name, ma) {
    return MaterialAttribute__get_NameText(ma) === name;
}

