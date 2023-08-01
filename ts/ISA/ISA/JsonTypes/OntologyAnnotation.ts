import { equals, int32ToString, IEquatable, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";
import { float64, int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { value as value_4, map, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { AnnotationValue_toString_Z3C00A204, AnnotationValue_fromString_Z721C83C5, AnnotationValue_$reflection, AnnotationValue_$union } from "./AnnotationValue.js";
import { empty, filter, map as map_1, singleton, append, exists, tryFind, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { IISAPrintable } from "../Printer.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { ActivePatterns_$007CRegex$007C_$007C, tryParseTermAnnotation, ActivePatterns_$007CTermAnnotation$007C_$007C } from "../Regex.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

export class OntologyAnnotation extends Record implements IEquatable<OntologyAnnotation>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<AnnotationValue_$union>;
    readonly TermSourceREF: Option<string>;
    readonly LocalID: Option<string>;
    readonly TermAccessionNumber: Option<string>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, Name: Option<AnnotationValue_$union>, TermSourceREF: Option<string>, LocalID: Option<string>, TermAccessionNumber: Option<string>, Comments: Option<FSharpList<Comment$>>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.TermSourceREF = TermSourceREF;
        this.LocalID = LocalID;
        this.TermAccessionNumber = TermAccessionNumber;
        this.Comments = Comments;
    }
    Equals(other: any): boolean {
        const this$: OntologyAnnotation = this;
        if (other instanceof OntologyAnnotation) {
            const oa: OntologyAnnotation = other;
            return this$["System.IEquatable`1.Equals2B595"](oa);
        }
        else if (typeof other === "string") {
            const s: string = other;
            return ((OntologyAnnotation__get_NameText(this$) === s) ? true : (OntologyAnnotation__get_TermAccessionShort(this$) === s)) ? true : (OntologyAnnotation__get_TermAccessionOntobeeUrl(this$) === s);
        }
        else {
            return false;
        }
    }
    GetHashCode(): int32 {
        const this$: OntologyAnnotation = this;
        return stringHash(OntologyAnnotation__get_NameText(this$) + OntologyAnnotation__get_TermAccessionShort(this$)) | 0;
    }
    Print(): string {
        const this$: OntologyAnnotation = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: OntologyAnnotation = this;
        return "OA " + OntologyAnnotation__get_NameText(this$);
    }
    "System.IEquatable`1.Equals2B595"(other: OntologyAnnotation): boolean {
        const this$: OntologyAnnotation = this;
        return ((this$.TermAccessionNumber != null) && (other.TermAccessionNumber != null)) ? ((OntologyAnnotation__get_TermAccessionShort(other) === OntologyAnnotation__get_TermAccessionShort(this$)) ? true : (OntologyAnnotation__get_TermAccessionOntobeeUrl(other) === OntologyAnnotation__get_TermAccessionOntobeeUrl(this$))) : (((this$.Name != null) && (other.Name != null)) ? (OntologyAnnotation__get_NameText(other) === OntologyAnnotation__get_NameText(this$)) : ((((this$.TermAccessionNumber == null) && (other.TermAccessionNumber == null)) && (this$.Name == null)) && (other.Name == null)));
    }
}

export function OntologyAnnotation_$reflection(): TypeInfo {
    return record_type("ISA.OntologyAnnotation", [], OntologyAnnotation, () => [["ID", option_type(string_type)], ["Name", option_type(AnnotationValue_$reflection())], ["TermSourceREF", option_type(string_type)], ["LocalID", option_type(string_type)], ["TermAccessionNumber", option_type(string_type)], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function OntologyAnnotation_make(id: Option<string>, name: Option<AnnotationValue_$union>, termSourceREF: Option<string>, localID: Option<string>, termAccessionNumber: Option<string>, comments: Option<FSharpList<Comment$>>): OntologyAnnotation {
    return new OntologyAnnotation(id, name, termSourceREF, localID, termAccessionNumber, comments);
}

/**
 * This function creates the type exactly as given. If you want a more streamlined approach use `OntologyAnnotation.fromString`.
 */
export function OntologyAnnotation_create_131C8C9D(Id?: string, Name?: AnnotationValue_$union, TermSourceREF?: string, LocalID?: string, TermAccessionNumber?: string, Comments?: FSharpList<Comment$>): OntologyAnnotation {
    return OntologyAnnotation_make(Id, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments);
}

export function OntologyAnnotation_get_empty(): OntologyAnnotation {
    return OntologyAnnotation_create_131C8C9D();
}

/**
 * Returns the name of the ontology as string
 */
export function OntologyAnnotation__get_NameText(this$: OntologyAnnotation): string {
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

/**
 * Returns the name of the ontology as string
 */
export function OntologyAnnotation__get_TryNameText(this$: OntologyAnnotation): Option<string> {
    return map<AnnotationValue_$union, string>((av: AnnotationValue_$union): string => {
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
    }, this$.Name);
}

/**
 * Returns the term source of the ontology as string
 */
export function OntologyAnnotation__get_TermSourceREFString(this$: OntologyAnnotation): string {
    return defaultArg(this$.TermSourceREF, "");
}

/**
 * Returns the term accession number of the ontology as string
 */
export function OntologyAnnotation__get_TermAccessionString(this$: OntologyAnnotation): string {
    return defaultArg(this$.TermAccessionNumber, "");
}

/**
 * Create a path in form of `http://purl.obolibrary.org/obo/MS_1000121` from it's Term Accession Source `MS` and Local Term Accession Number `1000121`.
 */
export function OntologyAnnotation_createUriAnnotation(termSourceRef: string, localTAN: string): string {
    return `${"http://purl.obolibrary.org/obo/"}${termSourceRef}_${localTAN}`;
}

/**
 * Create a ISAJson Ontology Annotation value from ISATab string entries, will try to reduce `termAccessionNumber` with regex matching.
 * 
 * Exmp. 1: http://purl.obolibrary.org/obo/GO_000001 --> GO:000001
 */
export function OntologyAnnotation_fromString_Z7D8EB286(term?: string, tsr?: string, tan?: string, comments?: FSharpList<Comment$>): OntologyAnnotation {
    let activePatternResult: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }>, tan_1: { LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string };
    const patternInput: [Option<string>, Option<string>] = (tan != null) ? ((activePatternResult = ActivePatterns_$007CTermAnnotation$007C_$007C(value_4(tan)), (activePatternResult != null) ? ((tan_1 = value_4(activePatternResult), [(tsr != null) ? tsr : tan_1.TermSourceREF, tan_1.LocalTAN] as [Option<string>, Option<string>])) : ([tsr, void 0] as [Option<string>, Option<string>]))) : ([tsr, void 0] as [Option<string>, Option<string>]);
    return OntologyAnnotation_make(void 0, map<string, AnnotationValue_$union>(AnnotationValue_fromString_Z721C83C5, term), patternInput[0], patternInput[1], tan, comments);
}

/**
 * Will always be created without `OntologyAnnotion.Name`
 */
export function OntologyAnnotation_fromTermAnnotation_Z721C83C5(termAnnotation: string): OntologyAnnotation {
    const r: { LocalTAN: string, TermSourceREF: string } = value_4(tryParseTermAnnotation(termAnnotation));
    return OntologyAnnotation_fromString_Z7D8EB286("", r.TermSourceREF, (r.TermSourceREF + ":") + r.LocalTAN);
}

/**
 * Parses any value in `TermAccessionString` to term accession format "termsourceref:localtan". Exmp.: "MS:000001".
 * 
 * If `TermAccessionString` cannot be parsed to this format, returns empty string!
 */
export function OntologyAnnotation__get_TermAccessionShort(this$: OntologyAnnotation): string {
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

export function OntologyAnnotation__get_TermAccessionOntobeeUrl(this$: OntologyAnnotation): string {
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
            return OntologyAnnotation_createUriAnnotation(tsr!, id!);
        default:
            return "";
    }
}

export function OntologyAnnotation__get_TermAccessionAndOntobeeUrlIfShort(this$: OntologyAnnotation): string {
    const matchValue: Option<string> = this$.TermAccessionNumber;
    if (matchValue != null) {
        const tan: string = value_4(matchValue);
        if (ActivePatterns_$007CRegex$007C_$007C("(?<termsourceref>\\w+?):(?<localtan>\\w+)", tan) != null) {
            return OntologyAnnotation__get_TermAccessionOntobeeUrl(this$);
        }
        else {
            return tan;
        }
    }
    else {
        return "";
    }
}

/**
 * Get a ISATab string entries from an ISAJson Ontology Annotation object (name,source,accession)
 * 
 * `asOntobeePurlUrl`: option to return term accession in Ontobee purl-url format (`http://purl.obolibrary.org/obo/MS_1000121`)
 */
export function OntologyAnnotation_toString_473B9D79(oa: OntologyAnnotation, asOntobeePurlUrlIfShort?: boolean): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } {
    let url: string;
    const asOntobeePurlUrlIfShort_1: boolean = defaultArg(asOntobeePurlUrlIfShort, false);
    const TermName: string = defaultArg(map<AnnotationValue_$union, string>(AnnotationValue_toString_Z3C00A204, oa.Name), "");
    const TermSourceREF: string = defaultArg(oa.TermSourceREF, "");
    return {
        TermAccessionNumber: asOntobeePurlUrlIfShort_1 ? ((url = OntologyAnnotation__get_TermAccessionAndOntobeeUrlIfShort(oa), (url === "") ? defaultArg(oa.TermAccessionNumber, "") : url)) : defaultArg(oa.TermAccessionNumber, ""),
        TermName: TermName,
        TermSourceREF: TermSourceREF,
    };
}

/**
 * Returns the name of the ontology as string if it has a name
 */
export function OntologyAnnotation_tryGetNameText_2FC95D30(oa: OntologyAnnotation): Option<string> {
    return OntologyAnnotation__get_TryNameText(oa);
}

/**
 * Returns the name of the ontology as string if it has a name
 */
export function OntologyAnnotation_getNameText_2FC95D30(oa: OntologyAnnotation): string {
    return OntologyAnnotation__get_NameText(oa);
}

/**
 * Returns true if the given name matches the name of the ontology annotation
 */
export function OntologyAnnotation_nameEqualsString(name: string, oa: OntologyAnnotation): boolean {
    return OntologyAnnotation__get_NameText(oa) === name;
}

/**
 * If an ontology annotation with the given annotation value exists in the list, returns it
 */
export function OntologyAnnotation_tryGetByName(name: AnnotationValue_$union, annotations: FSharpList<OntologyAnnotation>): Option<OntologyAnnotation> {
    return tryFind<OntologyAnnotation>((d: OntologyAnnotation): boolean => equals(d.Name, name), annotations);
}

/**
 * If a ontology annotation with the given annotation value exists in the list, returns true
 */
export function OntologyAnnotation_existsByName(name: AnnotationValue_$union, annotations: FSharpList<OntologyAnnotation>): boolean {
    return exists<OntologyAnnotation>((d: OntologyAnnotation): boolean => equals(d.Name, name), annotations);
}

/**
 * Adds the given ontology annotation to the Study.StudyDesignDescriptors
 */
export function OntologyAnnotation_add(onotolgyAnnotations: FSharpList<OntologyAnnotation>, onotolgyAnnotation: OntologyAnnotation): FSharpList<OntologyAnnotation> {
    return append<OntologyAnnotation>(onotolgyAnnotations, singleton(onotolgyAnnotation));
}

/**
 * Updates all ontology annotations for which the predicate returns true with the given ontology annotations values
 */
export function OntologyAnnotation_updateBy(predicate: ((arg0: OntologyAnnotation) => boolean), updateOption: Update_UpdateOptions_$union, design: OntologyAnnotation, annotations: FSharpList<OntologyAnnotation>): FSharpList<OntologyAnnotation> {
    if (exists<OntologyAnnotation>(predicate, annotations)) {
        return map_1<OntologyAnnotation, OntologyAnnotation>((d: OntologyAnnotation): OntologyAnnotation => {
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
        }, annotations);
    }
    else {
        return annotations;
    }
}

/**
 * If an ontology annotation with the same annotation value as the given annotation value exists in the list, updates it with the given ontology annotation
 */
export function OntologyAnnotation_updateByName(updateOption: Update_UpdateOptions_$union, design: OntologyAnnotation, annotations: FSharpList<OntologyAnnotation>): FSharpList<OntologyAnnotation> {
    return OntologyAnnotation_updateBy((f: OntologyAnnotation): boolean => equals(f.Name, design.Name), updateOption, design, annotations);
}

/**
 * If a ontology annotation with the annotation value exists in the list, removes it
 */
export function OntologyAnnotation_removeByName(name: AnnotationValue_$union, annotations: FSharpList<OntologyAnnotation>): FSharpList<OntologyAnnotation> {
    return filter<OntologyAnnotation>((d: OntologyAnnotation): boolean => !equals(d.Name, name), annotations);
}

/**
 * Returns comments of a ontology annotation
 */
export function OntologyAnnotation_getComments_2FC95D30(annotation: OntologyAnnotation): Option<FSharpList<Comment$>> {
    return annotation.Comments;
}

/**
 * Applies function f on comments of a ontology annotation
 */
export function OntologyAnnotation_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), annotation: OntologyAnnotation): OntologyAnnotation {
    return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, mapDefault<FSharpList<Comment$>>(empty<Comment$>(), f, annotation.Comments));
}

/**
 * Replaces comments of a ontology annotation by given comment list
 */
export function OntologyAnnotation_setComments(annotation: OntologyAnnotation, comments: FSharpList<Comment$>): OntologyAnnotation {
    return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, comments);
}

