import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286, OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Comment$_$reflection } from "./Comment.js";
import { map, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { filter, map as map_1, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";

export class Factor extends Record {
    constructor(ID, Name, FactorType, Comments) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.FactorType = FactorType;
        this.Comments = Comments;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + Factor__get_NameText(this$);
    }
}

export function Factor_$reflection() {
    return record_type("ISA.Factor", [], Factor, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["FactorType", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Factor_make(id, name, factorType, comments) {
    return new Factor(id, name, factorType, comments);
}

export function Factor_create_Z3D2B374F(Id, Name, FactorType, Comments) {
    return Factor_make(Id, Name, FactorType, Comments);
}

export function Factor_get_empty() {
    return Factor_create_Z3D2B374F();
}

/**
 * Create a ISAJson Factor from ISATab string entries
 */
export function Factor_fromString_Z5D76503E(name, term, source, accession, comments) {
    const oa = OntologyAnnotation_fromString_Z7D8EB286(term, source, accession, unwrap(comments));
    return Factor_make(void 0, fromValueWithDefault("", name), fromValueWithDefault(OntologyAnnotation_get_empty(), oa), void 0);
}

/**
 * Get ISATab string entries from an ISAJson Factor object
 */
export function Factor_toString_E353FDD(factor) {
    const value = {
        TermAccessionNumber: "",
        TermName: "",
        TermSourceREF: "",
    };
    return defaultArg(map(OntologyAnnotation_toString_473B9D79, factor.FactorType), value);
}

export function Factor__get_NameText(this$) {
    return defaultArg(this$.Name, "");
}

export function Factor__MapCategory_65D42856(this$, f) {
    return new Factor(this$.ID, this$.Name, map(f, this$.FactorType), this$.Comments);
}

export function Factor__SetCategory_2FC95D30(this$, c) {
    return new Factor(this$.ID, this$.Name, c, this$.Comments);
}

/**
 * If a factor with the given name exists in the list, returns it
 */
export function Factor_tryGetByName(name, factors) {
    return tryFind((f) => equals(f.Name, name), factors);
}

/**
 * If a factor with the given name exists in the list exists, returns true
 */
export function Factor_existsByName(name, factors) {
    return exists((f) => equals(f.Name, name), factors);
}

/**
 * adds the given factor to the factors
 */
export function Factor_add(factors, factor) {
    return append(factors, singleton(factor));
}

/**
 * Updates all factors for which the predicate returns true with the given factor values
 */
export function Factor_updateBy(predicate, updateOption, factor, factors) {
    if (exists(predicate, factors)) {
        return map_1((f) => {
            if (predicate(f)) {
                const this$ = updateOption;
                const recordType_1 = f;
                const recordType_2 = factor;
                return (this$.tag === 2) ? makeRecord(Factor_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 1) ? makeRecord(Factor_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 3) ? makeRecord(Factor_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : recordType_2));
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
export function Factor_updateByName(updateOption, factor, factors) {
    return Factor_updateBy((f) => equals(f.Name, factor.Name), updateOption, factor, factors);
}

/**
 * If a factor with the given name exists in the list, removes it
 */
export function Factor_removeByName(name, factors) {
    return filter((f) => !equals(f.Name, name), factors);
}

/**
 * Returns comments of a factor
 */
export function Factor_getComments_E353FDD(factor) {
    return factor.Comments;
}

/**
 * Applies function f on comments of a factor
 */
export function Factor_mapComments(f, factor) {
    return new Factor(factor.ID, factor.Name, factor.FactorType, map(f, factor.Comments));
}

/**
 * Replaces comments of a factor by given comment list
 */
export function Factor_setComments(factor, comments) {
    return new Factor(factor.ID, factor.Name, factor.FactorType, comments);
}

/**
 * Returns factor type of a factor
 */
export function Factor_getFactorType_E353FDD(factor) {
    return factor.FactorType;
}

/**
 * Applies function f on factor type of a factor
 */
export function Factor_mapFactorType(f, factor) {
    return new Factor(factor.ID, factor.Name, map(f, factor.FactorType), factor.Comments);
}

/**
 * Replaces factor type of a factor by given factor type
 */
export function Factor_setFactorType(factor, factorType) {
    return new Factor(factor.ID, factor.Name, factorType, factor.Comments);
}

/**
 * Returns the name of the factor as string if it exists
 */
export function Factor_tryGetName_E353FDD(f) {
    return f.Name;
}

/**
 * Returns the name of the factor as string
 */
export function Factor_getNameAsString_E353FDD(f) {
    return Factor__get_NameText(f);
}

/**
 * Returns true if the given name matches the name of the factor
 */
export function Factor_nameEqualsString(name, f) {
    return Factor__get_NameText(f) === name;
}

