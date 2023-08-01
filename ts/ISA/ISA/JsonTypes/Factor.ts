import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { map, defaultArg, unwrap, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { filter, map as map_1, singleton, append, exists, tryFind, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";

export class Factor extends Record implements IEquatable<Factor>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly FactorType: Option<OntologyAnnotation>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, Name: Option<string>, FactorType: Option<OntologyAnnotation>, Comments: Option<FSharpList<Comment$>>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.FactorType = FactorType;
        this.Comments = Comments;
    }
    Print(): string {
        const this$: Factor = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Factor = this;
        return "OA " + Factor__get_NameText(this$);
    }
}

export function Factor_$reflection(): TypeInfo {
    return record_type("ISA.Factor", [], Factor, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["FactorType", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Factor_make(id: Option<string>, name: Option<string>, factorType: Option<OntologyAnnotation>, comments: Option<FSharpList<Comment$>>): Factor {
    return new Factor(id, name, factorType, comments);
}

export function Factor_create_Z3D2B374F(Id?: string, Name?: string, FactorType?: OntologyAnnotation, Comments?: FSharpList<Comment$>): Factor {
    return Factor_make(Id, Name, FactorType, Comments);
}

export function Factor_get_empty(): Factor {
    return Factor_create_Z3D2B374F();
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function Factor_fromString_Z5D76503E(name: string, term: string, source: string, accession: string, comments?: FSharpList<Comment$>): Factor {
    const oa: OntologyAnnotation = OntologyAnnotation_fromString_Z7D8EB286(term, source, accession, unwrap(comments));
    return Factor_make(void 0, fromValueWithDefault<string>("", name), fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation_get_empty(), oa), void 0);
}

/**
 * Get ISATab string entries from an ISAJson Factor object
 */
export function Factor_toString_E353FDD(factor: Factor): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
    const value: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>(OntologyAnnotation_toString_473B9D79, factor.FactorType), value);
}

export function Factor__get_NameText(this$: Factor): string {
    return defaultArg(this$.Name, "");
}

export function Factor__MapCategory_65D42856(this$: Factor, f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): Factor {
    return new Factor(this$.ID, this$.Name, map<OntologyAnnotation, OntologyAnnotation>(f, this$.FactorType), this$.Comments);
}

export function Factor__SetCategory_2FC95D30(this$: Factor, c: OntologyAnnotation): Factor {
    return new Factor(this$.ID, this$.Name, c, this$.Comments);
}

/**
 * If a factor with the given name exists in the list, returns it
 */
export function Factor_tryGetByName(name: string, factors: FSharpList<Factor>): Option<Factor> {
    return tryFind<Factor>((f: Factor): boolean => equals(f.Name, name), factors);
}

/**
 * If a factor with the given name exists in the list exists, returns true
 */
export function Factor_existsByName(name: string, factors: FSharpList<Factor>): boolean {
    return exists<Factor>((f: Factor): boolean => equals(f.Name, name), factors);
}

/**
 * adds the given factor to the factors
 */
export function Factor_add(factors: FSharpList<Factor>, factor: Factor): FSharpList<Factor> {
    return append<Factor>(factors, singleton(factor));
}

/**
 * Updates all factors for which the predicate returns true with the given factor values
 */
export function Factor_updateBy(predicate: ((arg0: Factor) => boolean), updateOption: Update_UpdateOptions_$union, factor: Factor, factors: FSharpList<Factor>): FSharpList<Factor> {
    if (exists<Factor>(predicate, factors)) {
        return map_1<Factor, Factor>((f: Factor): Factor => {
            if (predicate(f)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Factor = f;
                const recordType_2: Factor = factor;
                return (this$.tag === /* UpdateAllAppendLists */ 2) ? (makeRecord(Factor_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Factor) : ((this$.tag === /* UpdateByExisting */ 1) ? (makeRecord(Factor_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Factor) : ((this$.tag === /* UpdateByExistingAppendLists */ 3) ? (makeRecord(Factor_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Factor) : recordType_2));
            }
            else {
                return f;
            }
        }, factors);
    }
    else {
        return factors;
    }
}

/**
 * Updates all factors with the same name as the given factor with its values
 */
export function Factor_updateByName(updateOption: Update_UpdateOptions_$union, factor: Factor, factors: FSharpList<Factor>): FSharpList<Factor> {
    return Factor_updateBy((f: Factor): boolean => equals(f.Name, factor.Name), updateOption, factor, factors);
}

/**
 * If a factor with the given name exists in the list, removes it
 */
export function Factor_removeByName(name: string, factors: FSharpList<Factor>): FSharpList<Factor> {
    return filter<Factor>((f: Factor): boolean => !equals(f.Name, name), factors);
}

/**
 * Returns comments of a factor
 */
export function Factor_getComments_E353FDD(factor: Factor): Option<FSharpList<Comment$>> {
    return factor.Comments;
}

/**
 * Applies function f on comments of a factor
 */
export function Factor_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), factor: Factor): Factor {
    return new Factor(factor.ID, factor.Name, factor.FactorType, map<FSharpList<Comment$>, FSharpList<Comment$>>(f, factor.Comments));
}

/**
 * Replaces comments of a factor by given comment list
 */
export function Factor_setComments(factor: Factor, comments: FSharpList<Comment$>): Factor {
    return new Factor(factor.ID, factor.Name, factor.FactorType, comments);
}

/**
 * Returns factor type of a factor
 */
export function Factor_getFactorType_E353FDD(factor: Factor): Option<OntologyAnnotation> {
    return factor.FactorType;
}

/**
 * Applies function f on factor type of a factor
 */
export function Factor_mapFactorType(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), factor: Factor): Factor {
    return new Factor(factor.ID, factor.Name, map<OntologyAnnotation, OntologyAnnotation>(f, factor.FactorType), factor.Comments);
}

/**
 * Replaces factor type of a factor by given factor type
 */
export function Factor_setFactorType(factor: Factor, factorType: OntologyAnnotation): Factor {
    return new Factor(factor.ID, factor.Name, factorType, factor.Comments);
}

/**
 * Returns the name of the factor as string if it exists
 */
export function Factor_tryGetName_E353FDD(f: Factor): Option<string> {
    return f.Name;
}

/**
 * Returns the name of the factor as string
 */
export function Factor_getNameAsString_E353FDD(f: Factor): string {
    return Factor__get_NameText(f);
}

/**
 * Returns true if the given name matches the name of the factor
 */
export function Factor_nameEqualsString(name: string, f: Factor): boolean {
    return Factor__get_NameText(f) === name;
}

