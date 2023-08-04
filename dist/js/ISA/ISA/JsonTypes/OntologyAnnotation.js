import { value as value_4, unwrap, map, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { equals, stringHash, int32ToString } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { ActivePatterns_$007CRegex$007C_$007C, tryParseTermAnnotation, ActivePatterns_$007CTermAnnotation$007C_$007C } from "../Regex.js";
import { AnnotationValue_$reflection, AnnotationValue_toString_Z6FAD7738, AnnotationValue_fromString_Z721C83C5 } from "./AnnotationValue.js";
import { filter, map as map_1, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { record_type, array_type, option_type, string_type, getRecordFields, makeRecord } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Comment$_$reflection } from "./Comment.js";

export class OntologyAnnotation extends Record {
    constructor(ID, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.TermSourceREF = TermSourceREF;
        this.LocalID = LocalID;
        this.TermAccessionNumber = TermAccessionNumber;
        this.Comments = Comments;
    }
    static make(id, name, termSourceREF, localID, termAccessionNumber, comments) {
        return new OntologyAnnotation(id, name, termSourceREF, localID, termAccessionNumber, comments);
    }
    static create(Id, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments) {
        return OntologyAnnotation.make(Id, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments);
    }
    static get empty() {
        return OntologyAnnotation.create();
    }
    get NameText() {
        const this$ = this;
        return defaultArg(map((av) => {
            switch (av.tag) {
                case 1:
                    return av.fields[0].toString();
                case 2:
                    return int32ToString(av.fields[0]);
                default:
                    return av.fields[0];
            }
        }, this$.Name), "");
    }
    get TryNameText() {
        const this$ = this;
        return unwrap(map((av) => {
            switch (av.tag) {
                case 1:
                    return av.fields[0].toString();
                case 2:
                    return int32ToString(av.fields[0]);
                default:
                    return av.fields[0];
            }
        }, this$.Name));
    }
    get TermSourceREFString() {
        const this$ = this;
        return defaultArg(this$.TermSourceREF, "");
    }
    get TermAccessionString() {
        const this$ = this;
        return defaultArg(this$.TermAccessionNumber, "");
    }
    static createUriAnnotation(termSourceRef, localTAN) {
        return `${"http://purl.obolibrary.org/obo/"}${termSourceRef}_${localTAN}`;
    }
    static fromString(term, tsr, tan, comments) {
        let activePatternResult, tan_1;
        const patternInput = (tan != null) ? ((activePatternResult = ActivePatterns_$007CTermAnnotation$007C_$007C(tan), (activePatternResult != null) ? ((tan_1 = activePatternResult, [(tsr != null) ? tsr : tan_1.TermSourceREF, tan_1.LocalTAN])) : [tsr, void 0])) : [tsr, void 0];
        const arg_1 = map(AnnotationValue_fromString_Z721C83C5, term);
        return OntologyAnnotation.make(void 0, arg_1, patternInput[0], patternInput[1], tan, comments);
    }
    static fromTermAnnotation(termAnnotation) {
        const r = value_4(tryParseTermAnnotation(termAnnotation));
        const accession = (r.TermSourceREF + ":") + r.LocalTAN;
        return OntologyAnnotation.fromString("", r.TermSourceREF, accession);
    }
    get TermAccessionShort() {
        const this$ = this;
        const matchValue = this$.TermSourceREF;
        const matchValue_1 = this$.LocalID;
        let matchResult, id, tsr;
        if (matchValue != null) {
            if (matchValue_1 != null) {
                matchResult = 0;
                id = matchValue_1;
                tsr = matchValue;
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
                return `${tsr}:${id}`;
            default:
                return "";
        }
    }
    get TermAccessionOntobeeUrl() {
        const this$ = this;
        const matchValue = this$.TermSourceREF;
        const matchValue_1 = this$.LocalID;
        let matchResult, id, tsr;
        if (matchValue != null) {
            if (matchValue_1 != null) {
                matchResult = 0;
                id = matchValue_1;
                tsr = matchValue;
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
                return OntologyAnnotation.createUriAnnotation(tsr, id);
            default:
                return "";
        }
    }
    get TermAccessionAndOntobeeUrlIfShort() {
        const this$ = this;
        const matchValue = this$.TermAccessionNumber;
        if (matchValue != null) {
            const tan = matchValue;
            return (ActivePatterns_$007CRegex$007C_$007C("(?<termsourceref>\\w+?):(?<localtan>\\w+)", tan) != null) ? this$.TermAccessionOntobeeUrl : tan;
        }
        else {
            return "";
        }
    }
    static toString(oa, asOntobeePurlUrlIfShort) {
        let url;
        const asOntobeePurlUrlIfShort_1 = defaultArg(asOntobeePurlUrlIfShort, false);
        const TermName = defaultArg(map(AnnotationValue_toString_Z6FAD7738, oa.Name), "");
        const TermSourceREF = defaultArg(oa.TermSourceREF, "");
        return {
            TermAccessionNumber: asOntobeePurlUrlIfShort_1 ? ((url = oa.TermAccessionAndOntobeeUrlIfShort, (url === "") ? defaultArg(oa.TermAccessionNumber, "") : url)) : defaultArg(oa.TermAccessionNumber, ""),
            TermName: TermName,
            TermSourceREF: TermSourceREF,
        };
    }
    Equals(other) {
        const this$ = this;
        if (other instanceof OntologyAnnotation) {
            return this$["System.IEquatable`1.Equals2B595"](other);
        }
        else if (typeof other === "string") {
            const s = other;
            return ((this$.NameText === s) ? true : (this$.TermAccessionShort === s)) ? true : (this$.TermAccessionOntobeeUrl === s);
        }
        else {
            return false;
        }
    }
    GetHashCode() {
        const this$ = this;
        return stringHash(this$.NameText + this$.TermAccessionShort) | 0;
    }
    static tryGetNameText(oa) {
        return oa.TryNameText;
    }
    static getNameText(oa) {
        return oa.NameText;
    }
    static nameEqualsString(name, oa) {
        return oa.NameText === name;
    }
    static tryGetByName(name, annotations) {
        return tryFind((d) => equals(d.Name, name), annotations);
    }
    static existsByName(name, annotations) {
        return exists((d) => equals(d.Name, name), annotations);
    }
    static add(onotolgyAnnotations, onotolgyAnnotation) {
        return append(onotolgyAnnotations, singleton(onotolgyAnnotation));
    }
    static updateBy(predicate, updateOption, design, annotations) {
        return exists(predicate, annotations) ? map_1((d) => {
            if (predicate(d)) {
                const this$ = updateOption;
                const recordType_1 = d;
                const recordType_2 = design;
                switch (this$.tag) {
                    case 2:
                        return makeRecord(OntologyAnnotation_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 1:
                        return makeRecord(OntologyAnnotation_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 3:
                        return makeRecord(OntologyAnnotation_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    default:
                        return recordType_2;
                }
            }
            else {
                return d;
            }
        }, annotations) : annotations;
    }
    static updateByName(updateOption, design, annotations) {
        return OntologyAnnotation.updateBy((f) => equals(f.Name, design.Name), updateOption, design, annotations);
    }
    static removeByName(name, annotations) {
        return filter((d) => !equals(d.Name, name), annotations);
    }
    static getComments(annotation) {
        return annotation.Comments;
    }
    static mapComments(f, annotation) {
        return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, mapDefault([], f, annotation.Comments));
    }
    static setComments(annotation, comments) {
        return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, comments);
    }
    Copy() {
        const this$ = this;
        const arg_5 = map((array) => map_2((c) => c.Copy(), array), this$.Comments);
        return OntologyAnnotation.make(this$.ID, this$.Name, this$.TermSourceREF, this$.LocalID, this$.TermAccessionNumber, arg_5);
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + this$.NameText;
    }
    "System.IEquatable`1.Equals2B595"(other) {
        const this$ = this;
        return ((this$.TermAccessionNumber != null) && (other.TermAccessionNumber != null)) ? ((other.TermAccessionShort === this$.TermAccessionShort) ? true : (other.TermAccessionOntobeeUrl === this$.TermAccessionOntobeeUrl)) : (((this$.Name != null) && (other.Name != null)) ? (other.NameText === this$.NameText) : ((((this$.TermAccessionNumber == null) && (other.TermAccessionNumber == null)) && (this$.Name == null)) && (other.Name == null)));
    }
}

export function OntologyAnnotation_$reflection() {
    return record_type("ARCtrl.ISA.OntologyAnnotation", [], OntologyAnnotation, () => [["ID", option_type(string_type)], ["Name", option_type(AnnotationValue_$reflection())], ["TermSourceREF", option_type(string_type)], ["LocalID", option_type(string_type)], ["TermAccessionNumber", option_type(string_type)], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

