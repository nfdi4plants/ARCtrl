import { equals, int32ToString, stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { AnnotationValue_toString_Z3C00A204, AnnotationValue_fromString_Z721C83C5, AnnotationValue_$reflection } from "./AnnotationValue.js";
import { Comment$_$reflection } from "./Comment.js";
import { value as value_4, map, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { ActivePatterns_$007CRegex$007C_$007C, tryParseTermAnnotation, ActivePatterns_$007CTermAnnotation$007C_$007C } from "../Regex.js";
import { empty, filter, map as map_1, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

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
    Equals(other) {
        const this$ = this;
        if (other instanceof OntologyAnnotation) {
            return this$["System.IEquatable`1.Equals2B595"](other);
        }
        else if (typeof other === "string") {
            const s = other;
            return ((OntologyAnnotation__get_NameText(this$) === s) ? true : (OntologyAnnotation__get_TermAccessionShort(this$) === s)) ? true : (OntologyAnnotation__get_TermAccessionOntobeeUrl(this$) === s);
        }
        else {
            return false;
        }
    }
    GetHashCode() {
        const this$ = this;
        return stringHash(OntologyAnnotation__get_NameText(this$) + OntologyAnnotation__get_TermAccessionShort(this$)) | 0;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + OntologyAnnotation__get_NameText(this$);
    }
    "System.IEquatable`1.Equals2B595"(other) {
        const this$ = this;
        return ((this$.TermAccessionNumber != null) && (other.TermAccessionNumber != null)) ? ((OntologyAnnotation__get_TermAccessionShort(other) === OntologyAnnotation__get_TermAccessionShort(this$)) ? true : (OntologyAnnotation__get_TermAccessionOntobeeUrl(other) === OntologyAnnotation__get_TermAccessionOntobeeUrl(this$))) : (((this$.Name != null) && (other.Name != null)) ? (OntologyAnnotation__get_NameText(other) === OntologyAnnotation__get_NameText(this$)) : ((((this$.TermAccessionNumber == null) && (other.TermAccessionNumber == null)) && (this$.Name == null)) && (other.Name == null)));
    }
}

export function OntologyAnnotation_$reflection() {
    return record_type("ISA.OntologyAnnotation", [], OntologyAnnotation, () => [["ID", option_type(string_type)], ["Name", option_type(AnnotationValue_$reflection())], ["TermSourceREF", option_type(string_type)], ["LocalID", option_type(string_type)], ["TermAccessionNumber", option_type(string_type)], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function OntologyAnnotation_make(id, name, termSourceREF, localID, termAccessionNumber, comments) {
    return new OntologyAnnotation(id, name, termSourceREF, localID, termAccessionNumber, comments);
}

/**
 * This function creates the type exactly as given. If you want a more streamlined approach use `OntologyAnnotation.fromString`.
 */
export function OntologyAnnotation_create_131C8C9D(Id, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments) {
    return OntologyAnnotation_make(Id, Name, TermSourceREF, LocalID, TermAccessionNumber, Comments);
}

export function OntologyAnnotation_get_empty() {
    return OntologyAnnotation_create_131C8C9D();
}

/**
 * Returns the name of the ontology as string
 */
export function OntologyAnnotation__get_NameText(this$) {
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

/**
 * Returns the name of the ontology as string
 */
export function OntologyAnnotation__get_TryNameText(this$) {
    return map((av) => {
        switch (av.tag) {
            case 1:
                return av.fields[0].toString();
            case 2:
                return int32ToString(av.fields[0]);
            default:
                return av.fields[0];
        }
    }, this$.Name);
}

/**
 * Returns the term source of the ontology as string
 */
export function OntologyAnnotation__get_TermSourceREFString(this$) {
    return defaultArg(this$.TermSourceREF, "");
}

/**
 * Returns the term accession number of the ontology as string
 */
export function OntologyAnnotation__get_TermAccessionString(this$) {
    return defaultArg(this$.TermAccessionNumber, "");
}

/**
 * Create a path in form of `http://purl.obolibrary.org/obo/MS_1000121` from it's Term Accession Source `MS` and Local Term Accession Number `1000121`.
 */
export function OntologyAnnotation_createUriAnnotation(termSourceRef, localTAN) {
    return `${"http://purl.obolibrary.org/obo/"}${termSourceRef}_${localTAN}`;
}

/**
 * Create a ISAJson Ontology Annotation value from ISATab string entries, will try to reduce `termAccessionNumber` with regex matching.
 * 
 * Exmp. 1: http://purl.obolibrary.org/obo/GO_000001 --> GO:000001
 */
export function OntologyAnnotation_fromString_Z7D8EB286(term, tsr, tan, comments) {
    let activePatternResult, tan_1;
    const patternInput = (tan != null) ? ((activePatternResult = ActivePatterns_$007CTermAnnotation$007C_$007C(tan), (activePatternResult != null) ? ((tan_1 = activePatternResult, [(tsr != null) ? tsr : tan_1.TermSourceREF, tan_1.LocalTAN])) : [tsr, void 0])) : [tsr, void 0];
    return OntologyAnnotation_make(void 0, map(AnnotationValue_fromString_Z721C83C5, term), patternInput[0], patternInput[1], tan, comments);
}

/**
 * Will always be created without `OntologyAnnotion.Name`
 */
export function OntologyAnnotation_fromTermAnnotation_Z721C83C5(termAnnotation) {
    const r = value_4(tryParseTermAnnotation(termAnnotation));
    return OntologyAnnotation_fromString_Z7D8EB286("", r.TermSourceREF, (r.TermSourceREF + ":") + r.LocalTAN);
}

/**
 * Parses any value in `TermAccessionString` to term accession format "termsourceref:localtan". Exmp.: "MS:000001".
 * 
 * If `TermAccessionString` cannot be parsed to this format, returns empty string!
 */
export function OntologyAnnotation__get_TermAccessionShort(this$) {
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

export function OntologyAnnotation__get_TermAccessionOntobeeUrl(this$) {
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
            return OntologyAnnotation_createUriAnnotation(tsr, id);
        default:
            return "";
    }
}

export function OntologyAnnotation__get_TermAccessionAndOntobeeUrlIfShort(this$) {
    const matchValue = this$.TermAccessionNumber;
    if (matchValue != null) {
        const tan = matchValue;
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
export function OntologyAnnotation_toString_473B9D79(oa, asOntobeePurlUrlIfShort) {
    let url;
    const asOntobeePurlUrlIfShort_1 = defaultArg(asOntobeePurlUrlIfShort, false);
    const TermName = defaultArg(map(AnnotationValue_toString_Z3C00A204, oa.Name), "");
    const TermSourceREF = defaultArg(oa.TermSourceREF, "");
    return {
        TermAccessionNumber: asOntobeePurlUrlIfShort_1 ? ((url = OntologyAnnotation__get_TermAccessionAndOntobeeUrlIfShort(oa), (url === "") ? defaultArg(oa.TermAccessionNumber, "") : url)) : defaultArg(oa.TermAccessionNumber, ""),
        TermName: TermName,
        TermSourceREF: TermSourceREF,
    };
}

/**
 * Returns the name of the ontology as string if it has a name
 */
export function OntologyAnnotation_tryGetNameText_2FC95D30(oa) {
    return OntologyAnnotation__get_TryNameText(oa);
}

/**
 * Returns the name of the ontology as string if it has a name
 */
export function OntologyAnnotation_getNameText_2FC95D30(oa) {
    return OntologyAnnotation__get_NameText(oa);
}

/**
 * Returns true if the given name matches the name of the ontology annotation
 */
export function OntologyAnnotation_nameEqualsString(name, oa) {
    return OntologyAnnotation__get_NameText(oa) === name;
}

/**
 * If an ontology annotation with the given annotation value exists in the list, returns it
 */
export function OntologyAnnotation_tryGetByName(name, annotations) {
    return tryFind((d) => equals(d.Name, name), annotations);
}

/**
 * If a ontology annotation with the given annotation value exists in the list, returns true
 */
export function OntologyAnnotation_existsByName(name, annotations) {
    return exists((d) => equals(d.Name, name), annotations);
}

/**
 * Adds the given ontology annotation to the Study.StudyDesignDescriptors
 */
export function OntologyAnnotation_add(onotolgyAnnotations, onotolgyAnnotation) {
    return append(onotolgyAnnotations, singleton(onotolgyAnnotation));
}

/**
 * Updates all ontology annotations for which the predicate returns true with the given ontology annotations values
 */
export function OntologyAnnotation_updateBy(predicate, updateOption, design, annotations) {
    if (exists(predicate, annotations)) {
        return map_1((d) => {
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
        }, annotations);
    }
    else {
        return annotations;
    }
}

/**
 * If an ontology annotation with the same annotation value as the given annotation value exists in the list, updates it with the given ontology annotation
 */
export function OntologyAnnotation_updateByName(updateOption, design, annotations) {
    return OntologyAnnotation_updateBy((f) => equals(f.Name, design.Name), updateOption, design, annotations);
}

/**
 * If a ontology annotation with the annotation value exists in the list, removes it
 */
export function OntologyAnnotation_removeByName(name, annotations) {
    return filter((d) => !equals(d.Name, name), annotations);
}

/**
 * Returns comments of a ontology annotation
 */
export function OntologyAnnotation_getComments_2FC95D30(annotation) {
    return annotation.Comments;
}

/**
 * Applies function f on comments of a ontology annotation
 */
export function OntologyAnnotation_mapComments(f, annotation) {
    return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, mapDefault(empty(), f, annotation.Comments));
}

/**
 * Replaces comments of a ontology annotation by given comment list
 */
export function OntologyAnnotation_setComments(annotation, comments) {
    return new OntologyAnnotation(annotation.ID, annotation.Name, annotation.TermSourceREF, annotation.LocalID, annotation.TermAccessionNumber, comments);
}

