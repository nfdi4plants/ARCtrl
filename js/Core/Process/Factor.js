import { OntologyAnnotation_$reflection, OntologyAnnotation } from "../OntologyAnnotation.js";
import { map, defaultArg, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_map, Option_fromValueWithDefault } from "../Helper/Collections.js";
import { filter, singleton, append, exists, tryFind } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { equals } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { Record, toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, array_type, option_type, string_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { Comment$_$reflection } from "../Comment.js";

export class Factor extends Record {
    constructor(Name, FactorType, Comments) {
        super();
        this.Name = Name;
        this.FactorType = FactorType;
        this.Comments = Comments;
    }
    static make(name, factorType, comments) {
        return new Factor(name, factorType, comments);
    }
    static create(Name, FactorType, Comments) {
        return Factor.make(Name, FactorType, Comments);
    }
    static get empty() {
        return Factor.create();
    }
    static fromString(name, term, source, accession, comments) {
        const oa = OntologyAnnotation.create(term, source, accession, unwrap(comments));
        const name_1 = Option_fromValueWithDefault("", name);
        const factorType = Option_fromValueWithDefault(new OntologyAnnotation(), oa);
        return Factor.make(name_1, factorType, undefined);
    }
    static toStringObject(factor) {
        const value = {
            TermAccessionNumber: "",
            TermName: "",
            TermSourceREF: "",
        };
        return defaultArg(map((oa) => OntologyAnnotation.toStringObject(oa), factor.FactorType), value);
    }
    get NameText() {
        const this$ = this;
        return defaultArg(this$.Name, "");
    }
    MapCategory(f) {
        const this$ = this;
        return new Factor(this$.Name, map(f, this$.FactorType), this$.Comments);
    }
    SetCategory(c) {
        const this$ = this;
        return new Factor(this$.Name, c, this$.Comments);
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
    static removeByName(name, factors) {
        return filter((f) => !equals(f.Name, name), factors);
    }
    static getComments(factor) {
        return factor.Comments;
    }
    static mapComments(f, factor) {
        return new Factor(factor.Name, factor.FactorType, map(f, factor.Comments));
    }
    static setComments(factor, comments) {
        return new Factor(factor.Name, factor.FactorType, comments);
    }
    static getFactorType(factor) {
        return factor.FactorType;
    }
    static mapFactorType(f, factor) {
        return new Factor(factor.Name, map(f, factor.FactorType), factor.Comments);
    }
    static setFactorType(factor, factorType) {
        return new Factor(factor.Name, factorType, factor.Comments);
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
        const comments = map((a) => ResizeArray_map((c) => c.Copy(), a), this$.Comments);
        return Factor.make(this$.Name, this$.FactorType, comments);
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
    return record_type("ARCtrl.Process.Factor", [], Factor, () => [["Name", option_type(string_type)], ["FactorType", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

