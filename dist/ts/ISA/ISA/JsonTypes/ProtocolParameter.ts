import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { bind, map, defaultArg, unwrap, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Comment$ } from "./Comment.js";

export class ProtocolParameter extends Record implements IEquatable<ProtocolParameter>, IISAPrintable {
    readonly ID: Option<string>;
    readonly ParameterName: Option<OntologyAnnotation>;
    constructor(ID: Option<string>, ParameterName: Option<OntologyAnnotation>) {
        super();
        this.ID = ID;
        this.ParameterName = ParameterName;
    }
    Print(): string {
        const this$: ProtocolParameter = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: ProtocolParameter = this;
        return "OA " + ProtocolParameter__get_NameText(this$);
    }
}

export function ProtocolParameter_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.ProtocolParameter", [], ProtocolParameter, () => [["ID", option_type(string_type)], ["ParameterName", option_type(OntologyAnnotation_$reflection())]]);
}

export function ProtocolParameter_make(id: Option<string>, parameterName: Option<OntologyAnnotation>): ProtocolParameter {
    return new ProtocolParameter(id, parameterName);
}

export function ProtocolParameter_create_Z6C54B221(Id?: string, ParameterName?: OntologyAnnotation): ProtocolParameter {
    return ProtocolParameter_make(Id, ParameterName);
}

export function ProtocolParameter_get_empty(): ProtocolParameter {
    return ProtocolParameter_create_Z6C54B221();
}

/**
 * Create a ISAJson Protocol Parameter from ISATab string entries
 */
export function ProtocolParameter_fromString_Z2304A83C(term: string, source: string, accession: string, comments?: Comment$[]): ProtocolParameter {
    const oa: OntologyAnnotation = OntologyAnnotation.fromString(term, source, accession, unwrap(comments));
    return ProtocolParameter_make(void 0, fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, oa));
}

/**
 * Get ISATab string entries from an ISAJson ProtocolParameter object (name,source,accession)
 */
export function ProtocolParameter_toString_Z3A4310A5(pp: ProtocolParameter): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
    const value: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>((arg: OntologyAnnotation): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } => OntologyAnnotation.toString(arg), pp.ParameterName), value);
}

/**
 * Returns the name of the parameter as string
 */
export function ProtocolParameter__get_NameText(this$: ProtocolParameter): string {
    return defaultArg(map<OntologyAnnotation, string>((oa: OntologyAnnotation): string => oa.NameText, this$.ParameterName), "");
}

/**
 * Returns the name of the parameter as string
 */
export function ProtocolParameter__get_TryNameText(this$: ProtocolParameter): Option<string> {
    return bind<OntologyAnnotation, string>((oa: OntologyAnnotation): Option<string> => oa.TryNameText, this$.ParameterName);
}

export function ProtocolParameter__MapCategory_Z69DD836A(this$: ProtocolParameter, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): ProtocolParameter {
    return new ProtocolParameter(this$.ID, map<OntologyAnnotation, OntologyAnnotation>(f, this$.ParameterName));
}

export function ProtocolParameter__SetCategory_Z4C0FE73C(this$: ProtocolParameter, c: OntologyAnnotation): ProtocolParameter {
    return new ProtocolParameter(this$.ID, c);
}

/**
 * Returns the name of the paramater as string if it exists
 */
export function ProtocolParameter_tryGetNameText_Z3A4310A5(pp: ProtocolParameter): Option<string> {
    return ProtocolParameter__get_TryNameText(pp);
}

/**
 * Returns the name of the paramater as string
 */
export function ProtocolParameter_getNameText_Z3A4310A5(pp: ProtocolParameter): string {
    return defaultArg(ProtocolParameter_tryGetNameText_Z3A4310A5(pp), "");
}

/**
 * Returns true if the given name matches the name of the parameter
 */
export function ProtocolParameter_nameEqualsString(name: string, pp: ProtocolParameter): boolean {
    return ProtocolParameter__get_NameText(pp) === name;
}

