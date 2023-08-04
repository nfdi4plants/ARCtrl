import { value as value_4, unwrap, map, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { AnnotationValue_$reflection, AnnotationValue_toString_Z6FAD7738, AnnotationValue_fromString_Z721C83C5, AnnotationValue_$union } from "./AnnotationValue.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { int32, float64 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { IEquatable, equals, stringHash, int32ToString } from "../../../fable_modules/fable-library-ts/Util.js";
import { ActivePatterns_$007CRegex$007C_$007C, tryParseTermAnnotation, ActivePatterns_$007CTermAnnotation$007C_$007C } from "../Regex.js";
import { filter, map as map_1, singleton, append, exists, FSharpList, tryFind } from "../../../fable_modules/fable-library-ts/List.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { record_type, array_type, option_type, string_type, TypeInfo, getRecordFields, makeRecord } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { IISAPrintable } from "../Printer.js";

export class OntologyAnnotation extends Record implements IEquatable<OntologyAnnotation>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<AnnotationValue_$union>;
    readonly TermSourceREF: Option<string>;
    readonly LocalID: Option<string>;
    readonly TermAccessionNumber: Option<string>;
    readonly Comments: Option<Comment$[]>;
    constructor(ID: Option<string>, Name: Option<AnnotationValue_$union>, TermSourceREF: Option<string>, LocalID: Option<string>, TermAccessionNumber: Option<string>, Comments: Option<Comment$[]>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.TermSourceREF = TermSourceREF;
        this.LocalID = LocalID;
        this.TermAccessionNumber = TermAccessionNumber;
        this.Comments = Comments;
    }
    static make(id: Option<string>, name: Option<AnnotationValue_$union>, termSourceREF: Option<string>, localID: Option<string>, termAccessionNumber: Option<string>, comments: Option<Comment$[]>): OntologyAnnotation {
        return new OntologyAnnotation(id, name, termSourceREF, localID, termAccessionNumber, comments);
    }
    static create(Id?: string, Name?: AnnotationValue_$union, TermSourceREF?: string, LocalID?: string, TermAccessionNumber?: string, Comments?: Comment$[]): OntologyAnnotation {
        return OntologyAnnotation.make(Id, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments);
    }
    static get empty(): OntologyAnnotation {
        return OntologyAnnotation.create();
    }
    get NameText(): string {
        const this$: OntologyAnnotation = this;
        return defaultArg(map<AnnotationValue_$union, string>((av: AnnotationValue_$union): string => {
            switch (av.tag) {
                case /* Float */ 1: {
                    const f: float64 = av.fields[0];
                    return f.toString();
                }
                case /* Int */ 2:
                    return int32ToString(av.fields[0]);
                default:
                    return av.fields[0];
            }
        }, this$.Name), "");
    }
    get TryNameText(): string | undefined {
        const this$: OntologyAnnotation = this;
        return unwrap(map<AnnotationValue_$union, string>((av: AnnotationValue_$union): string => {
            switch (av.tag) {
                case /* Float */ 1: {
                    const f: float64 = av.fields[0];
                    return f.toString();
                }
                case /* Int */ 2:
                    return int32ToString(av.fields[0]);
                default:
                    return av.fields[0];
            }
        }, this$.Name));
    }
    get TermSourceREFString(): string {
        const this$: OntologyAnnotation = this;
        return defaultArg(this$.TermSourceREF, "");
    }
    get TermAccessionString(): string {
        const this$: OntologyAnnotation = this;
        return defaultArg(this$.TermAccessionNumber, "");
    }
    static createUriAnnotation(termSourceRef: string, localTAN: string): string {
        return `${"http://purl.obolibrary.org/obo/"}${termSourceRef}_${localTAN}`;
    }
    static fromString(term?: string, tsr?: string, tan?: string, comments?: Comment$[]): OntologyAnnotation {
        let activePatternResult: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }>, tan_1: { LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string };
        const patternInput: [Option<string>, Option<string>] = (tan != null) ? ((activePatternResult = ActivePatterns_$007CTermAnnotation$007C_$007C(value_4(tan)), (activePatternResult != null) ? ((tan_1 = value_4(activePatternResult), [(tsr != null) ? tsr : tan_1.TermSourceREF, tan_1.LocalTAN] as [Option<string>, Option<string>])) : ([tsr, void 0] as [Option<string>, Option<string>]))) : ([tsr, void 0] as [Option<string>, Option<string>]);
        const arg_1: Option<AnnotationValue_$union> = map<string, AnnotationValue_$union>(AnnotationValue_fromString_Z721C83C5, term);
        return OntologyAnnotation.make(void 0, arg_1, patternInput[0], patternInput[1], tan, comments);
    }
    static fromTermAnnotation(termAnnotation: string): OntologyAnnotation {
        const r: { LocalTAN: string, TermSourceREF: string } = value_4(tryParseTermAnnotation(termAnnotation));
        const accession: string = (r.TermSourceREF + ":") + r.LocalTAN;
        return OntologyAnnotation.fromString("", r.TermSourceREF, accession);
    }
    get TermAccessionShort(): string {
        const this$: OntologyAnnotation = this;
        const matchValue: Option<string> = this$.TermSourceREF;
        const matchValue_1: Option<string> = this$.LocalID;
        let matchResult: int32, id: string, tsr: string;
        if (matchValue != null) {
            if (matchValue_1 != null) {
                matchResult = 0;
                id = value_4(matchValue_1);
                tsr = value_4(matchValue);
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return `${tsr!}:${id!}`;
            default:
                return "";
        }
    }
    get TermAccessionOntobeeUrl(): string {
        const this$: OntologyAnnotation = this;
        const matchValue: Option<string> = this$.TermSourceREF;
        const matchValue_1: Option<string> = this$.LocalID;
        let matchResult: int32, id: string, tsr: string;
        if (matchValue != null) {
            if (matchValue_1 != null) {
                matchResult = 0;
                id = value_4(matchValue_1);
                tsr = value_4(matchValue);
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return OntologyAnnotation.createUriAnnotation(tsr!, id!);
            default:
                return "";
        }
    }
    get TermAccessionAndOntobeeUrlIfShort(): string {
        const this$: OntologyAnnotation = this;
        const matchValue: Option<string> = this$.TermAccessionNumber;
        if (matchValue != null) {
            const tan: string = value_4(matchValue);
            return (ActivePatterns_$007CRegex$007C_$007C("(?<termsourceref>\\w+?):(?<localtan>\\w+)", tan) != null) ? this$.TermAccessionOntobeeUrl : tan;
        }
        else {
            return "";
        }
    }
    static toString(oa: OntologyAnnotation, asOntobeePurlUrlIfShort?: boolean): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
        let url: string;
        const asOntobeePurlUrlIfShort_1: boolean = defaultArg(asOntobeePurlUrlIfShort, false);
        const TermName: string = defaultArg(map<AnnotationValue_$union, string>(AnnotationValue_toString_Z6FAD7738, oa.Name), "");
        const TermSourceREF: string = defaultArg(oa.TermSourceREF, "");
        return {
            TermAccessionNumber: asOntobeePurlUrlIfShort_1 ? ((url = oa.TermAccessionAndOntobeeUrlIfShort, (url === "") ? defaultArg(oa.TermAccessionNumber, "") : url)) : defaultArg(oa.TermAccessionNumber, ""),
            TermName: TermName,
            TermSourceREF: TermSourceREF,
        };
    }
    Equals(other: any): boolean {
        const this$: OntologyAnnotation = this;
        if (other instanceof OntologyAnnotation) {
            const oa: OntologyAnnotation = other;
            return this$["System.IEquatable`1.Equals2B595"](oa);
        }
        else if (typeof other === "string") {
            const s: string = other;
            return ((this$.NameText === s) ? true : (this$.TermAccessionShort === s)) ? true : (this$.TermAccessionOntobeeUrl === s);
        }
        else {
            return false;
        }
    }
    GetHashCode(): int32 {
        const this$: OntologyAnnotation = this;
        return stringHash(this$.NameText + this$.TermAccessionShort) | 0;
    }
    static tryGetNameText(oa: OntologyAnnotation): Option<string> {
        return oa.TryNameText;
    }
    static getNameText(oa: OntologyAnnotation): string {
        return oa.NameText;
    }
    static nameEqualsString(name: string, oa: OntologyAnnotation): boolean {
        return oa.NameText === name;
    }
    static tryGetByName(name: AnnotationValue_$union, annotations: FSharpList<OntologyAnnotation>): Option<OntologyAnnotation> {
        return tryFind<OntologyAnnotation>((d: OntologyAnnotation): boolean => equals(d.Name, name), annotations);
    }
    static existsByName(name: AnnotationValue_$union, annotations: FSharpList<OntologyAnnotation>): boolean {
        return exists<OntologyAnnotation>((d: OntologyAnnotation): boolean => equals(d.Name, name), annotations);
    }
    static add(onotolgyAnnotations: FSharpList<OntologyAnnotation>, onotolgyAnnotation: OntologyAnnotation): FSharpList<OntologyAnnotation> {
        return append<OntologyAnnotation>(onotolgyAnnotations, singleton(onotolgyAnnotation));
    }
    static updateBy(predicate: ((arg0: OntologyAnnotation) => boolean), updateOption: Update_UpdateOptions_$union, design: OntologyAnnotation, annotations: FSharpList<OntologyAnnotation>): FSharpList<OntologyAnnotation> {
        return exists<OntologyAnnotation>(predicate, annotations) ? map_1<OntologyAnnotation, OntologyAnnotation>((d: OntologyAnnotation): OntologyAnnotation => {
            if (predicate(d)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: OntologyAnnotation = d;
                const recordType_2: OntologyAnnotation = design;
                switch (this$.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(OntologyAnnotation_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as OntologyAnnotation;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(OntologyAnnotation_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as OntologyAnnotation;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(OntologyAnnotation_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as OntologyAnnotation;
                    default:
                        return recordType_2;
                }
            }
            else {
                return d;
            }
        }, annotations) : annotations;
    }
    static updateByName(updateOption: Update_UpdateOptions_$union, design: OntologyAnnotation, annotations: FSharpList<OntologyAnnotation>): FSharpList<OntologyAnnotation> {
        return OntologyAnnotation.updateBy((f: OntologyAnnotation): boolean => equals(f.Name, design.Name), updateOption, design, annotations);
    }
    static removeByName(name: AnnotationValue_$union, annotations: FSharpList<OntologyAnnotation>): FSharpList<OntologyAnnotation> {
        return filter<OntologyAnnotation>((d: OntologyAnnotation): boolean => !equals(d.Name, name), annotations);
    }
    static getComments(annotation: OntologyAnnotation): Option<Comment$[]> {
        return annotation.Comments;
    }
    static mapComments(f: ((arg0: Comment$[]) => Comment$[]), annotation: OntologyAnnotation): OntologyAnnotation {
        return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, mapDefault<Comment$[]>([], f, annotation.Comments));
    }
    static setComments(annotation: OntologyAnnotation, comments: Comment$[]): OntologyAnnotation {
        return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, comments);
    }
    Copy(): OntologyAnnotation {
        const this$: OntologyAnnotation = this;
        const arg_5: Option<Comment$[]> = map<Comment$[], Comment$[]>((array: Comment$[]): Comment$[] => map_2<Comment$, Comment$>((c: Comment$): Comment$ => c.Copy(), array), this$.Comments);
        return OntologyAnnotation.make(this$.ID, this$.Name, this$.TermSourceREF, this$.LocalID, this$.TermAccessionNumber, arg_5);
    }
    Print(): string {
        const this$: OntologyAnnotation = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: OntologyAnnotation = this;
        return "OA " + this$.NameText;
    }
    "System.IEquatable`1.Equals2B595"(other: OntologyAnnotation): boolean {
        const this$: OntologyAnnotation = this;
        return ((this$.TermAccessionNumber != null) && (other.TermAccessionNumber != null)) ? ((other.TermAccessionShort === this$.TermAccessionShort) ? true : (other.TermAccessionOntobeeUrl === this$.TermAccessionOntobeeUrl)) : (((this$.Name != null) && (other.Name != null)) ? (other.NameText === this$.NameText) : ((((this$.TermAccessionNumber == null) && (other.TermAccessionNumber == null)) && (this$.Name == null)) && (other.Name == null)));
    }
}

export function OntologyAnnotation_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.OntologyAnnotation", [], OntologyAnnotation, () => [["ID", option_type(string_type)], ["Name", option_type(AnnotationValue_$reflection())], ["TermSourceREF", option_type(string_type)], ["LocalID", option_type(string_type)], ["TermAccessionNumber", option_type(string_type)], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

