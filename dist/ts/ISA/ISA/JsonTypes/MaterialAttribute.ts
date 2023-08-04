import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { bind, map, defaultArg, unwrap, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
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
    return record_type("ARCtrl.ISA.MaterialAttribute", [], MaterialAttribute, () => [["ID", option_type(string_type)], ["CharacteristicType", option_type(OntologyAnnotation_$reflection())]]);
}

export function MaterialAttribute_make(id: Option<string>, characteristicType: Option<OntologyAnnotation>): MaterialAttribute {
    return new MaterialAttribute(id, characteristicType);
}

export function MaterialAttribute_create_Z6C54B221(Id?: string, CharacteristicType?: OntologyAnnotation): MaterialAttribute {
    return MaterialAttribute_make(Id, CharacteristicType);
}

export function MaterialAttribute_get_empty(): MaterialAttribute {
    return MaterialAttribute_create_Z6C54B221();
}

/**
 * Create a ISAJson MaterialAttribute from ISATab string entries
 */
export function MaterialAttribute_fromString_Z2304A83C(term: string, source: string, accession: string, comments?: Comment$[]): MaterialAttribute {
    const oa: OntologyAnnotation = OntologyAnnotation.fromString(term, source, accession, unwrap(comments));
    return MaterialAttribute_make(void 0, fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, oa));
}

/**
 * Get ISATab string entries from an ISAJson MaterialAttribute object
 */
export function MaterialAttribute_toString_Z5F39696D(ma: MaterialAttribute): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
    const value: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>((arg: OntologyAnnotation): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } => OntologyAnnotation.toString(arg), ma.CharacteristicType), value);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_NameText(this$: MaterialAttribute): string {
    return defaultArg(map<OntologyAnnotation, string>((oa: OntologyAnnotation): string => oa.NameText, this$.CharacteristicType), "");
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute__get_TryNameText(this$: MaterialAttribute): Option<string> {
    return bind<OntologyAnnotation, string>((oa: OntologyAnnotation): Option<string> => oa.TryNameText, this$.CharacteristicType);
}

export function MaterialAttribute__MapCategory_Z69DD836A(this$: MaterialAttribute, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): MaterialAttribute {
    return new MaterialAttribute(this$.ID, map<OntologyAnnotation, OntologyAnnotation>(f, this$.CharacteristicType));
}

export function MaterialAttribute__SetCategory_Z4C0FE73C(this$: MaterialAttribute, c: OntologyAnnotation): MaterialAttribute {
    return new MaterialAttribute(this$.ID, c);
}

/**
 * Returns the name of the characteristic as string if it exists
 */
export function MaterialAttribute_tryGetNameText_Z5F39696D(ma: MaterialAttribute): string {
    return MaterialAttribute__get_NameText(ma);
}

/**
 * Returns the name of the characteristic as string
 */
export function MaterialAttribute_getNameText_Z5F39696D(ma: MaterialAttribute): Option<string> {
    return MaterialAttribute__get_TryNameText(ma);
}

/**
 * Returns true if the given name matches the name of the characteristic
 */
export function MaterialAttribute_nameEqualsString(name: string, ma: MaterialAttribute): boolean {
    return MaterialAttribute__get_NameText(ma) === name;
}

