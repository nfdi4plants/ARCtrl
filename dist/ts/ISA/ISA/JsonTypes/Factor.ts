import { map, defaultArg, unwrap, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { filter, map as map_1, singleton, append, exists, FSharpList, tryFind } from "../../../fable_modules/fable-library-ts/List.js";
import { IEquatable, equals } from "../../../fable_modules/fable-library-ts/Util.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { record_type, array_type, option_type, string_type, TypeInfo, getRecordFields, makeRecord } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { IISAPrintable } from "../Printer.js";

export class Factor extends Record implements IEquatable<Factor>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly FactorType: Option<OntologyAnnotation>;
    readonly Comments: Option<Comment$[]>;
    constructor(ID: Option<string>, Name: Option<string>, FactorType: Option<OntologyAnnotation>, Comments: Option<Comment$[]>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.FactorType = FactorType;
        this.Comments = Comments;
    }
    static make(id: Option<string>, name: Option<string>, factorType: Option<OntologyAnnotation>, comments: Option<Comment$[]>): Factor {
        return new Factor(id, name, factorType, comments);
    }
    static create(Id?: string, Name?: string, FactorType?: OntologyAnnotation, Comments?: Comment$[]): Factor {
        return Factor.make(Id, Name, FactorType, Comments);
    }
    static get empty(): Factor {
        return Factor.create();
    }
    static fromString(name: string, term: string, source: string, accession: string, comments?: Comment$[]): Factor {
        const oa: OntologyAnnotation = OntologyAnnotation.fromString(term, source, accession, unwrap(comments));
        const arg_1: Option<string> = fromValueWithDefault<string>("", name);
        const arg_2: Option<OntologyAnnotation> = fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, oa);
        return Factor.make(void 0, arg_1, arg_2, void 0);
    }
    static toString(factor: Factor): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
        const value: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = {
            TermAccessionNumber: "",
            TermName: "",
            TermSourceREF: "",
        };
        return defaultArg(map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>((arg: OntologyAnnotation): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } => OntologyAnnotation.toString(arg), factor.FactorType), value);
    }
    get NameText(): string {
        const this$: Factor = this;
        return defaultArg(this$.Name, "");
    }
    MapCategory(f: ((arg0: OntologyAnnotation) => OntologyAnnotation)): Factor {
        const this$: Factor = this;
        return new Factor(this$.ID, this$.Name, map<OntologyAnnotation, OntologyAnnotation>(f, this$.FactorType), this$.Comments);
    }
    SetCategory(c: OntologyAnnotation): Factor {
        const this$: Factor = this;
        return new Factor(this$.ID, this$.Name, c, this$.Comments);
    }
    static tryGetByName(name: string, factors: FSharpList<Factor>): Option<Factor> {
        return tryFind<Factor>((f: Factor): boolean => equals(f.Name, name), factors);
    }
    static existsByName(name: string, factors: FSharpList<Factor>): boolean {
        return exists<Factor>((f: Factor): boolean => equals(f.Name, name), factors);
    }
    static add(factors: FSharpList<Factor>, factor: Factor): FSharpList<Factor> {
        return append<Factor>(factors, singleton(factor));
    }
    static updateBy(predicate: ((arg0: Factor) => boolean), updateOption: Update_UpdateOptions_$union, factor: Factor, factors: FSharpList<Factor>): FSharpList<Factor> {
        return exists<Factor>(predicate, factors) ? map_1<Factor, Factor>((f: Factor): Factor => {
            if (predicate(f)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Factor = f;
                const recordType_2: Factor = factor;
                return (this$.tag === /* UpdateAllAppendLists */ 2) ? (makeRecord(Factor_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Factor) : ((this$.tag === /* UpdateByExisting */ 1) ? (makeRecord(Factor_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Factor) : ((this$.tag === /* UpdateByExistingAppendLists */ 3) ? (makeRecord(Factor_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Factor) : recordType_2));
            }
            else {
                return f;
            }
        }, factors) : factors;
    }
    static updateByName(updateOption: Update_UpdateOptions_$union, factor: Factor, factors: FSharpList<Factor>): FSharpList<Factor> {
        return Factor.updateBy((f: Factor): boolean => equals(f.Name, factor.Name), updateOption, factor, factors);
    }
    static removeByName(name: string, factors: FSharpList<Factor>): FSharpList<Factor> {
        return filter<Factor>((f: Factor): boolean => !equals(f.Name, name), factors);
    }
    static getComments(factor: Factor): Option<Comment$[]> {
        return factor.Comments;
    }
    static mapComments(f: ((arg0: Comment$[]) => Comment$[]), factor: Factor): Factor {
        return new Factor(factor.ID, factor.Name, factor.FactorType, map<Comment$[], Comment$[]>(f, factor.Comments));
    }
    static setComments(factor: Factor, comments: Comment$[]): Factor {
        return new Factor(factor.ID, factor.Name, factor.FactorType, comments);
    }
    static getFactorType(factor: Factor): Option<OntologyAnnotation> {
        return factor.FactorType;
    }
    static mapFactorType(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), factor: Factor): Factor {
        return new Factor(factor.ID, factor.Name, map<OntologyAnnotation, OntologyAnnotation>(f, factor.FactorType), factor.Comments);
    }
    static setFactorType(factor: Factor, factorType: OntologyAnnotation): Factor {
        return new Factor(factor.ID, factor.Name, factorType, factor.Comments);
    }
    static tryGetName(f: Factor): Option<string> {
        return f.Name;
    }
    static getNameAsString(f: Factor): string {
        return f.NameText;
    }
    static nameEqualsString(name: string, f: Factor): boolean {
        return f.NameText === name;
    }
    Copy(): Factor {
        const this$: Factor = this;
        const arg_3: Option<Comment$[]> = map<Comment$[], Comment$[]>((array: Comment$[]): Comment$[] => map_2<Comment$, Comment$>((c: Comment$): Comment$ => c.Copy(), array), this$.Comments);
        return Factor.make(this$.ID, this$.Name, this$.FactorType, arg_3);
    }
    Print(): string {
        const this$: Factor = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Factor = this;
        return "OA " + this$.NameText;
    }
}

export function Factor_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Factor", [], Factor, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["FactorType", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

