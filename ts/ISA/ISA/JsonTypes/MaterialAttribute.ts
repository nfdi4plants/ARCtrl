import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { bind, map, defaultArg, unwrap, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation__get_TryNameText, OntologyAnnotation__get_NameText, OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Comment$ } from "./Comment.js";

export class MaterialAttribute extends Record implements IEquatable<MaterialAttribute>, IISAPrintable {
    readonly ID: Option<string>;
    readonly CharacteristicType: Option<OntologyAnnotation>;
    constructor(ID: Option<string>, CharacteristicType: Option<OntologyAnnotation>) {
        super();
        this.ID = ID;
        this.CharacteristicType = CharacteristicType;
    }
    Print(): string {
        const this$: MaterialAttribute = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: MaterialAttribute = this;
        return "OA " + MaterialAttribute__get_NameText(this$);
    }
}

export function MaterialAttribute_$reflection(): TypeInfo {
    return record_type("ISA.MaterialAttribute", [], MaterialAttribute, () => [["ID", option_type(string_type)], ["CharacteristicType", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttribute_make(id: Option<string>, characteristicType: Option<OntologyAnnotation>): MaterialAttribute {
    return new MaterialAttribute(id, characteristicType);
}

export function MaterialAttribute_create_2769312B(Id?: string, CharacteristicType?: OntologyAnnotation): MaterialAttribute {
    return MaterialAttribute_make(Id, CharacteristicType);
}

export function MaterialAttribute_get_empty(): MaterialAttribute {
    return MaterialAttribute_create_2769312B();
}

/**
 * Create a ISAJson MaterialAttribute from ISATab string entries
 */
export function MaterialAttribute_fromString_703AFBF9(term: string, source: string, accession: string, comments?: FSharpList<Comment$>): MaterialAttribute {
    const oa: OntologyAnnotation = OntologyAnnotation_fromString_Z7D8EB286(term, source, accession, unwrap(comments));
    return MaterialAttribute_make(void 0, fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation_get_empty(), oa));
}

/**
 * Get ISATab string entries from an ISAJson MaterialAttribute object
 */
export function MaterialAttribute_toString_Z6476E859(ma: MaterialAttribute): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
    const value: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>(OntologyAnnotation_toString_473B9D79, ma.CharacteristicType), value);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_NameText(this$: MaterialAttribute): string {
    return defaultArg(map<OntologyAnnotation, string>(OntologyAnnotation__get_NameText, this$.CharacteristicType), "");
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_TryNameText(this$: MaterialAttribute): Option<string> {
    return bind<OntologyAnnotation, string>(OntologyAnnotation__get_TryNameText, this$.CharacteristicType);
}

export function MaterialAttribute__MapCategory_65D42856(this$: MaterialAttribute, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): MaterialAttribute {
    return new MaterialAttribute(this$.ID, map<OntologyAnnotation, OntologyAnnotation>(f, this$.CharacteristicType));
}

export function MaterialAttribute__SetCategory_2FC95D30(this$: MaterialAttribute, c: OntologyAnnotation): MaterialAttribute {
    return new MaterialAttribute(this$.ID, c);
}

/**
 * Returns the name of the characteristic as string if it exists
 */
export function MaterialAttribute_tryGetNameText_Z6476E859(ma: MaterialAttribute): string {
    return MaterialAttribute__get_NameText(ma);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute_getNameText_Z6476E859(ma: MaterialAttribute): Option<string> {
    return MaterialAttribute__get_TryNameText(ma);
}

/**
 * Returns true if the given name matches the name of the characteristic
 */
export function MaterialAttribute_nameEqualsString(name: string, ma: MaterialAttribute): boolean {
    return MaterialAttribute__get_NameText(ma) === name;
}

