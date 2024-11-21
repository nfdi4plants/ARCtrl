import { Record } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, list_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { Remark, Comment$_$reflection } from "../Core/Comment.js";
import { head, tail, isEmpty, ofSeq, concat, unzip, append, reverse, cons, empty, map, ofArray } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { Remark_wrapRemark, Comment_toString, Comment_fromString } from "./Metadata/Comment.js";
import { SparseRowModule_writeToSheet, SparseRowModule_fromFsRow, SparseRowModule_fromAllValues, SparseRowModule_getAllValues, SparseRowModule_fromValues, SparseRowModule_tryGetValueAt, SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133 } from "./Metadata/SparseTable.js";
import { addToDict } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { value as value_1, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { Option_fromValueWithDefault, ResizeArray_iter } from "../Core/Helper/Collections.js";
import { List_distinctBy, List_distinct } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { comparePrimitives, getEnumerator, stringHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { ArcStudy, ArcInvestigation } from "../Core/ArcTypes.js";
import { toRows, fromRows } from "./Metadata/OntologySourceReference.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./Metadata/Publication.js";
import { toRows as toRows_2, fromRows as fromRows_2 } from "./Metadata/Contacts.js";
import { toRows as toRows_3, fromRows as fromRows_3 } from "./Metadata/Study.js";
import { iterateIndexed, tryFind as tryFind_1, map as map_1, toList, collect, singleton, append as append_1, delay } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { tryFind, ofList } from "../fable_modules/fable-library-js.4.22.0/Map.js";
import { printf, toFail } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { FsWorksheet } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorksheet.fs.js";
import { FsWorkbook } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorkbook.fs.js";

export class ArcInvestigation_InvestigationInfo extends Record {
    constructor(Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Comments) {
        super();
        this.Identifier = Identifier;
        this.Title = Title;
        this.Description = Description;
        this.SubmissionDate = SubmissionDate;
        this.PublicReleaseDate = PublicReleaseDate;
        this.Comments = Comments;
    }
}

export function ArcInvestigation_InvestigationInfo_$reflection() {
    return record_type("ARCtrl.Spreadsheet.ArcInvestigation.InvestigationInfo", [], ArcInvestigation_InvestigationInfo, () => [["Identifier", string_type], ["Title", string_type], ["Description", string_type], ["SubmissionDate", string_type], ["PublicReleaseDate", string_type], ["Comments", list_type(Comment$_$reflection())]]);
}

export function ArcInvestigation_InvestigationInfo_create(identifier, title, description, submissionDate, publicReleaseDate, comments) {
    return new ArcInvestigation_InvestigationInfo(identifier, title, description, submissionDate, publicReleaseDate, comments);
}

export function ArcInvestigation_InvestigationInfo_get_Labels() {
    return ofArray(["Investigation Identifier", "Investigation Title", "Investigation Description", "Investigation Submission Date", "Investigation Public Release Date"]);
}

export function ArcInvestigation_InvestigationInfo_FromSparseTable_3ECCA699(matrix) {
    const comments = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, 0])), matrix.CommentKeys);
    return ArcInvestigation_InvestigationInfo_create(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Identifier", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Title", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Description", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Submission Date", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Public Release Date", 0]), comments);
}

export function ArcInvestigation_InvestigationInfo_ToSparseTable_Z720BD3FF(investigation) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, ArcInvestigation_InvestigationInfo_get_Labels(), undefined, 2);
    let commentKeys = empty();
    addToDict(matrix.Matrix, ["Investigation Identifier", 1], investigation.Identifier);
    addToDict(matrix.Matrix, ["Investigation Title", 1], defaultArg(investigation.Title, ""));
    addToDict(matrix.Matrix, ["Investigation Description", 1], defaultArg(investigation.Description, ""));
    addToDict(matrix.Matrix, ["Investigation Submission Date", 1], defaultArg(investigation.SubmissionDate, ""));
    addToDict(matrix.Matrix, ["Investigation Public Release Date", 1], defaultArg(investigation.PublicReleaseDate, ""));
    ResizeArray_iter((comment) => {
        const patternInput = Comment_toString(comment);
        const n = patternInput[0];
        commentKeys = cons(n, commentKeys);
        addToDict(matrix.Matrix, [n, 1], patternInput[1]);
    }, investigation.Comments);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function ArcInvestigation_InvestigationInfo_fromRows(lineNumber, rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, ArcInvestigation_InvestigationInfo_get_Labels(), lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], ArcInvestigation_InvestigationInfo_FromSparseTable_3ECCA699(tupledArg[3])];
}

export function ArcInvestigation_InvestigationInfo_toRows_Z720BD3FF(investigation) {
    return SparseTable_ToRows_759CAFC1(ArcInvestigation_InvestigationInfo_ToSparseTable_Z720BD3FF(investigation));
}

export function ArcInvestigation_fromParts(investigationInfo, ontologySourceReference, publications, contacts, studies, assays, remarks) {
    const studyIdentifiers = map((s) => s.Identifier, studies);
    const title = Option_fromValueWithDefault("", investigationInfo.Title);
    const description = Option_fromValueWithDefault("", investigationInfo.Description);
    const submissionDate = Option_fromValueWithDefault("", investigationInfo.SubmissionDate);
    const publicReleaseDate = Option_fromValueWithDefault("", investigationInfo.PublicReleaseDate);
    const ontologySourceReferences = Array.from(ontologySourceReference);
    const publications_1 = Array.from(publications);
    const contacts_1 = Array.from(contacts);
    const assays_1 = Array.from(assays);
    const studies_1 = Array.from(studies);
    const registeredStudyIdentifiers = Array.from(studyIdentifiers);
    const comments = Array.from(investigationInfo.Comments);
    const remarks_1 = Array.from(remarks);
    return ArcInvestigation.make(investigationInfo.Identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications_1, contacts_1, assays_1, studies_1, registeredStudyIdentifiers, comments, remarks_1);
}

export function ArcInvestigation_fromRows(rows) {
    const en = getEnumerator(rows);
    const emptyInvestigationInfo = ArcInvestigation_InvestigationInfo_create("", "", "", "", "", empty());
    const loop = (lastLine_mut, ontologySourceReferences_mut, investigationInfo_mut, publications_mut, contacts_mut, studies_mut, remarks_mut, lineNumber_mut) => {
        loop:
        while (true) {
            const lastLine = lastLine_mut, ontologySourceReferences = ontologySourceReferences_mut, investigationInfo = investigationInfo_mut, publications = publications_mut, contacts = contacts_mut, studies = studies_mut, remarks = remarks_mut, lineNumber = lineNumber_mut;
            let matchResult, k_5, k_6, k_7, k_8, k_9;
            if (lastLine != null) {
                switch (lastLine) {
                    case "ONTOLOGY SOURCE REFERENCE": {
                        matchResult = 0;
                        k_5 = lastLine;
                        break;
                    }
                    case "INVESTIGATION": {
                        matchResult = 1;
                        k_6 = lastLine;
                        break;
                    }
                    case "INVESTIGATION PUBLICATIONS": {
                        matchResult = 2;
                        k_7 = lastLine;
                        break;
                    }
                    case "INVESTIGATION CONTACTS": {
                        matchResult = 3;
                        k_8 = lastLine;
                        break;
                    }
                    case "STUDY": {
                        matchResult = 4;
                        k_9 = lastLine;
                        break;
                    }
                    default:
                        matchResult = 5;
                }
            }
            else {
                matchResult = 5;
            }
            switch (matchResult) {
                case 0: {
                    const patternInput = fromRows(lineNumber + 1, en);
                    lastLine_mut = patternInput[0];
                    ontologySourceReferences_mut = patternInput[3];
                    investigationInfo_mut = investigationInfo;
                    publications_mut = publications;
                    contacts_mut = contacts;
                    studies_mut = studies;
                    remarks_mut = append(remarks, patternInput[2]);
                    lineNumber_mut = patternInput[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_1 = ArcInvestigation_InvestigationInfo_fromRows(lineNumber + 1, en);
                    lastLine_mut = patternInput_1[0];
                    ontologySourceReferences_mut = ontologySourceReferences;
                    investigationInfo_mut = patternInput_1[3];
                    publications_mut = publications;
                    contacts_mut = contacts;
                    studies_mut = studies;
                    remarks_mut = append(remarks, patternInput_1[2]);
                    lineNumber_mut = patternInput_1[1];
                    continue loop;
                }
                case 2: {
                    const patternInput_2 = fromRows_1("Investigation Publication", lineNumber + 1, en);
                    lastLine_mut = patternInput_2[0];
                    ontologySourceReferences_mut = ontologySourceReferences;
                    investigationInfo_mut = investigationInfo;
                    publications_mut = patternInput_2[3];
                    contacts_mut = contacts;
                    studies_mut = studies;
                    remarks_mut = append(remarks, patternInput_2[2]);
                    lineNumber_mut = patternInput_2[1];
                    continue loop;
                }
                case 3: {
                    const patternInput_3 = fromRows_2("Investigation Person", lineNumber + 1, en);
                    lastLine_mut = patternInput_3[0];
                    ontologySourceReferences_mut = ontologySourceReferences;
                    investigationInfo_mut = investigationInfo;
                    publications_mut = publications;
                    contacts_mut = patternInput_3[3];
                    studies_mut = studies;
                    remarks_mut = append(remarks, patternInput_3[2]);
                    lineNumber_mut = patternInput_3[1];
                    continue loop;
                }
                case 4: {
                    const patternInput_4 = fromRows_3(lineNumber + 1, en);
                    const study = patternInput_4[3];
                    const newRemarks_4 = patternInput_4[2];
                    const lineNumber_6 = patternInput_4[1] | 0;
                    const currentLine_4 = patternInput_4[0];
                    if (study != null) {
                        lastLine_mut = currentLine_4;
                        ontologySourceReferences_mut = ontologySourceReferences;
                        investigationInfo_mut = investigationInfo;
                        publications_mut = publications;
                        contacts_mut = contacts;
                        studies_mut = cons(value_1(study), studies);
                        remarks_mut = append(remarks, newRemarks_4);
                        lineNumber_mut = lineNumber_6;
                        continue loop;
                    }
                    else {
                        lastLine_mut = currentLine_4;
                        ontologySourceReferences_mut = ontologySourceReferences;
                        investigationInfo_mut = investigationInfo;
                        publications_mut = publications;
                        contacts_mut = contacts;
                        studies_mut = studies;
                        remarks_mut = append(remarks, newRemarks_4);
                        lineNumber_mut = lineNumber_6;
                        continue loop;
                    }
                }
                default: {
                    let patternInput_5;
                    const tupledArg = unzip(studies);
                    patternInput_5 = [reverse(tupledArg[0]), List_distinctBy((a_1) => a_1.Identifier, concat(tupledArg[1]), {
                        Equals: (x, y) => (x === y),
                        GetHashCode: stringHash,
                    })];
                    return ArcInvestigation_fromParts(investigationInfo, ontologySourceReferences, publications, contacts, patternInput_5[0], patternInput_5[1], remarks);
                }
            }
            break;
        }
    };
    if (en["System.Collections.IEnumerator.MoveNext"]()) {
        return loop(SparseRowModule_tryGetValueAt(0, en["System.Collections.Generic.IEnumerator`1.get_Current"]()), empty(), emptyInvestigationInfo, empty(), empty(), empty(), empty(), 1);
    }
    else {
        throw new Error("emptyInvestigationFile");
    }
}

export function ArcInvestigation_toRows(investigation) {
    let remarks, rows;
    return (remarks = ofSeq(investigation.Remarks), (rows = delay(() => append_1(singleton(SparseRowModule_fromValues(["ONTOLOGY SOURCE REFERENCE"])), delay(() => append_1(toRows(ofSeq(investigation.OntologySourceReferences)), delay(() => append_1(singleton(SparseRowModule_fromValues(["INVESTIGATION"])), delay(() => append_1(ArcInvestigation_InvestigationInfo_toRows_Z720BD3FF(investigation), delay(() => append_1(singleton(SparseRowModule_fromValues(["INVESTIGATION PUBLICATIONS"])), delay(() => append_1(toRows_1("Investigation Publication", ofSeq(investigation.Publications)), delay(() => append_1(singleton(SparseRowModule_fromValues(["INVESTIGATION CONTACTS"])), delay(() => append_1(toRows_2("Investigation Person", ofSeq(investigation.Contacts)), delay(() => collect((studyIdentifier) => {
        const study = defaultArg(investigation.TryGetStudy(studyIdentifier), new ArcStudy(studyIdentifier));
        return append_1(singleton(SparseRowModule_fromValues(["STUDY"])), delay(() => toRows_3(study, undefined)));
    }, investigation.RegisteredStudyIdentifiers)))))))))))))))))), (() => {
        try {
            const rm = ofList(map((remark) => Remark.toTuple(remark), remarks), {
                Compare: comparePrimitives,
            });
            const loop = (i_mut, l_mut, nl_mut) => {
                loop:
                while (true) {
                    const i = i_mut, l = l_mut, nl = nl_mut;
                    const matchValue = tryFind(i, rm);
                    if (matchValue == null) {
                        if (!isEmpty(l)) {
                            i_mut = (i + 1);
                            l_mut = tail(l);
                            nl_mut = cons(head(l), nl);
                            continue loop;
                        }
                        else {
                            return nl;
                        }
                    }
                    else {
                        const remark_1 = matchValue;
                        i_mut = (i + 1);
                        l_mut = l;
                        nl_mut = cons(SparseRowModule_fromValues([Remark_wrapRemark(remark_1)]), nl);
                        continue loop;
                    }
                    break;
                }
            };
            return reverse(loop(1, ofSeq(rows), empty()));
        }
        catch (matchValue_1) {
            return toList(rows);
        }
    })()));
}

export function ArcInvestigation_toMetadataCollection(investigation) {
    return map_1(SparseRowModule_getAllValues, ArcInvestigation_toRows(investigation));
}

export function ArcInvestigation_fromMetadataCollection(collection) {
    return ArcInvestigation_fromRows(map_1(SparseRowModule_fromAllValues, collection));
}

export function ArcInvestigation_isMetadataSheetName(name) {
    if (name === "isa_investigation") {
        return true;
    }
    else {
        return name === "Investigation";
    }
}

export function ArcInvestigation_isMetadataSheet(sheet) {
    return ArcInvestigation_isMetadataSheetName(sheet.Name);
}

export function ArcInvestigation_tryGetMetadataSheet(doc) {
    return tryFind_1(ArcInvestigation_isMetadataSheet, doc.GetWorksheets());
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_fromFsWorkbook_Static_32154C9D(doc) {
    let sheet_1, matchValue;
    try {
        return ArcInvestigation_fromRows(map_1(SparseRowModule_fromFsRow, (sheet_1 = ((matchValue = ArcInvestigation_tryGetMetadataSheet(doc), (matchValue == null) ? (() => {
            throw new Error("Could not find metadata sheet with sheetname \"isa_investigation\" or deprecated sheetname \"Investigation\"");
        })() : matchValue)), FsWorksheet.getRows(sheet_1))));
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Could not read investigation from spreadsheet: %s"))(arg);
    }
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(investigation) {
    try {
        const wb = new FsWorkbook();
        const sheet = new FsWorksheet("isa_investigation");
        iterateIndexed((rowI, r) => {
            SparseRowModule_writeToSheet(rowI + 1, r, sheet);
        }, ArcInvestigation_toRows(investigation));
        wb.AddWorksheet(sheet);
        return wb;
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Could not write investigation to spreadsheet: %s"))(arg);
    }
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToFsWorkbook(this$) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(this$);
}

