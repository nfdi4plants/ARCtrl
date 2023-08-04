import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { map, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { filter, map as map_1, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { record_type, array_type, option_type, string_type, getRecordFields, makeRecord } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Comment$_$reflection } from "./Comment.js";

export class Factor extends Record {
    constructor(ID, Name, FactorType, Comments) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.FactorType = FactorType;
        this.Comments = Comments;
    }
    static make(id, name, factorType, comments) {
        return new Factor(id, name, factorType, comments);
    }
    static create(Id, Name, FactorType, Comments) {
        return Factor.make(Id, Name, FactorType, Comments);
    }
    static get empty() {
        return Factor.create();
    }
    static fromString(name, term, source, accession, comments) {
        const oa = OntologyAnnotation.fromString(term, source, accession, unwrap(comments));
        const arg_1 = fromValueWithDefault("", name);
        const arg_2 = fromValueWithDefault(OntologyAnnotation.empty, oa);
        return Factor.make(void 0, arg_1, arg_2, void 0);
    }
    static toString(factor) {
        const value = {
            TermAccessionNumber: "",
            TermName: "",
            TermSourceREF: "",
        };
        return defaultArg(map((arg) => OntologyAnnotation.toString(arg), factor.FactorType), value);
    }
    get NameText() {
        const this$ = this;
        return defaultArg(this$.Name, "");
    }
    MapCategory(f) {
        const this$ = this;
        return new Factor(this$.ID, this$.Name, map(f, this$.FactorType), this$.Comments);
    }
    SetCategory(c) {
        const this$ = this;
        return new Factor(this$.ID, this$.Name, c, this$.Comments);
    }
    static tryGetByName(name, factors) {
        return tryFind((f) => equals(f.Name, name), factors);
    }
    static existsByName(name, factors) {
        return exists((f) => equals(f.Name, name), factors);
    }
    static add(factors, factor) {
        return append(factors, singleton(factor));
    }
    static updateBy(predicate, updateOption, factor, factors) {
        return exists(predicate, factors) ? map_1((f) => {
            if (predicate(f)) {
                const this$ = updateOption;
                const recordType_1 = f;
                const recordType_2 = factor;
                return (this$.tag === 2) ? makeRecord(Factor_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 1) ? makeRecord(Factor_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) : ((this$.tag === 3) ? makeRecord(Factor_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) : recordType_2));
            }
            else {
                return f;
            }
        }, factors) : factors;
    }
    static updateByName(updateOption, factor, factors) {
        return Factor.updateBy((f) => equals(f.Name, factor.Name), updateOption, factor, factors);
    }
    static removeByName(name, factors) {
        return filter((f) => !equals(f.Name, name), factors);
    }
    static getComments(factor) {
        return factor.Comments;
    }
    static mapComments(f, factor) {
        return new Factor(factor.ID, factor.Name, factor.FactorType, map(f, factor.Comments));
    }
    static setComments(factor, comments) {
        return new Factor(factor.ID, factor.Name, factor.FactorType, comments);
    }
    static getFactorType(factor) {
        return factor.FactorType;
    }
    static mapFactorType(f, factor) {
        return new Factor(factor.ID, factor.Name, map(f, factor.FactorType), factor.Comments);
    }
    static setFactorType(factor, factorType) {
        return new Factor(factor.ID, factor.Name, factorType, factor.Comments);
    }
    static tryGetName(f) {
        return f.Name;
    }
    static getNameAsString(f) {
        return f.NameText;
    }
    static nameEqualsString(name, f) {
        return f.NameText === name;
    }
    Copy() {
        const this$ = this;
        const arg_3 = map((array) => map_2((c) => c.Copy(), array), this$.Comments);
        return Factor.make(this$.ID, this$.Name, this$.FactorType, arg_3);
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + this$.NameText;
    }
}

export function Factor_$reflection() {
    return record_type("ARCtrl.ISA.Factor", [], Factor, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["FactorType", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

