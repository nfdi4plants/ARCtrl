import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, list_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Remark_toTuple_Z26CAFFFA, Comment$_$reflection } from "../../ISA/JsonTypes/Comment.js";
import { head, tail, isEmpty, ofSeq, append, toArray, reverse, cons, empty, map, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { Remark_wrapRemark, Comment_toString, Comment_fromString } from "../Comment.js";
import { SparseRowModule_writeToSheet, SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_tryGetValueAt, SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133 } from "../SparseTable.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { value as value_1, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { comparePrimitives, getEnumerator, stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { ArcInvestigation } from "../../ISA/ArcTypes/ArcInvestigation.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./OntologySourceReference.js";
import { toRows as toRows_2, fromRows as fromRows_2 } from "./Publication.js";
import { toRows as toRows_3, fromRows as fromRows_3 } from "./Contacts.js";
import { toRows as toRows_4, fromRows as fromRows_4 } from "./Study.js";
import { iterateIndexed, head as head_1, map as map_1, toList, collect, singleton, append as append_1, delay } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { tryFind, ofList } from "../../../fable_modules/fable-library.4.1.4/Map.js";
import { printf, toFail } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { FsWorksheet } from "../../../fable_modules/FsSpreadsheet.3.3.0/FsWorksheet.fs.js";
import { FsWorkbook } from "../../../fable_modules/FsSpreadsheet.3.3.0/FsWorkbook.fs.js";

export class InvestigationInfo extends Record {
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

export function InvestigationInfo_$reflection() {
    return record_type("ARCtrl.ISA.Spreadsheet.ArcInvestigation.InvestigationInfo", [], InvestigationInfo, () => [["Identifier", string_type], ["Title", string_type], ["Description", string_type], ["SubmissionDate", string_type], ["PublicReleaseDate", string_type], ["Comments", list_type(Comment$_$reflection())]]);
}

export function InvestigationInfo_create(identifier, title, description, submissionDate, publicReleaseDate, comments) {
    return new InvestigationInfo(identifier, title, description, submissionDate, publicReleaseDate, comments);
}

export function InvestigationInfo_get_Labels() {
    return ofArray(["Investigation Identifier", "Investigation Title", "Investigation Description", "Investigation Submission Date", "Investigation Public Release Date"]);
}

export function InvestigationInfo_FromSparseTable_651559CC(matrix) {
    const comments = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, 0])), matrix.CommentKeys);
    return InvestigationInfo_create(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Identifier", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Title", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Description", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Submission Date", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Public Release Date", 0]), comments);
}

export function InvestigationInfo_ToSparseTable_Z4D87C88C(investigation) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, InvestigationInfo_get_Labels(), void 0, 2);
    let commentKeys = empty();
    addToDict(matrix.Matrix, ["Investigation Identifier", 1], investigation.Identifier);
    addToDict(matrix.Matrix, ["Investigation Title", 1], defaultArg(investigation.Title, ""));
    addToDict(matrix.Matrix, ["Investigation Description", 1], defaultArg(investigation.Description, ""));
    addToDict(matrix.Matrix, ["Investigation Submission Date", 1], defaultArg(investigation.SubmissionDate, ""));
    addToDict(matrix.Matrix, ["Investigation Public Release Date", 1], defaultArg(investigation.PublicReleaseDate, ""));
    if (!(investigation.Comments.length === 0)) {
        const array = investigation.Comments;
        array.forEach((comment) => {
            const patternInput = Comment_toString(comment);
            const n = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, 1], patternInput[1]);
        });
    }
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function InvestigationInfo_fromRows(lineNumber, rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, InvestigationInfo_get_Labels(), lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], InvestigationInfo_FromSparseTable_651559CC(tupledArg[3])];
}

export function InvestigationInfo_toRows_Z4D87C88C(investigation) {
    return SparseTable_ToRows_6A3D4534(InvestigationInfo_ToSparseTable_Z4D87C88C(investigation));
}

export function fromParts(investigationInfo, ontologySourceReference, publications, contacts, studies, remarks) {
    const arg_1 = Option_fromValueWithDefault("", investigationInfo.Title);
    const arg_2 = Option_fromValueWithDefault("", investigationInfo.Description);
    const arg_3 = Option_fromValueWithDefault("", investigationInfo.SubmissionDate);
    const arg_4 = Option_fromValueWithDefault("", investigationInfo.PublicReleaseDate);
    const arg_5 = toArray(ontologySourceReference);
    const arg_6 = toArray(publications);
    const arg_7 = toArray(contacts);
    const arg_8 = Array.from(studies);
    const arg_9 = toArray(investigationInfo.Comments);
    const arg_10 = toArray(remarks);
    return ArcInvestigation.make(investigationInfo.Identifier, arg_1, arg_2, arg_3, arg_4, arg_5, arg_6, arg_7, arg_8, arg_9, arg_10);
}

export function fromRows(rows) {
    const en = getEnumerator(rows);
    const emptyInvestigationInfo = InvestigationInfo_create("", "", "", "", "", empty());
    const loop = (lastLine_mut, ontologySourceReferences_mut, investigationInfo_mut, publications_mut, contacts_mut, studies_mut, remarks_mut, lineNumber_mut) => {
        loop:
        while (true) {
            const lastLine = lastLine_mut, ontologySourceReferences = ontologySourceReferences_mut, investigationInfo = investigationInfo_mut, publications = publications_mut, contacts = contacts_mut, studies = studies_mut, remarks = remarks_mut, lineNumber = lineNumber_mut;
            let matchResult, k_5, k_6, k_7, k_8, k_9, k_10;
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
                    default: {
                        matchResult = 5;
                        k_10 = lastLine;
                    }
                }
            }
            else {
                matchResult = 5;
                k_10 = lastLine;
            }
            switch (matchResult) {
                case 0: {
                    const patternInput = fromRows_1(lineNumber + 1, en);
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
                    const patternInput_1 = InvestigationInfo_fromRows(lineNumber + 1, en);
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
                    const patternInput_2 = fromRows_2("Investigation Publication", lineNumber + 1, en);
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
                    const patternInput_3 = fromRows_3("Investigation Person", lineNumber + 1, en);
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
                    const patternInput_4 = fromRows_4(lineNumber + 1, en);
                    const study = patternInput_4[3];
                    const newRemarks_4 = patternInput_4[2];
                    const lineNumber_5 = patternInput_4[1] | 0;
                    const currentLine_4 = patternInput_4[0];
                    if (study != null) {
                        lastLine_mut = currentLine_4;
                        ontologySourceReferences_mut = ontologySourceReferences;
                        investigationInfo_mut = investigationInfo;
                        publications_mut = publications;
                        contacts_mut = contacts;
                        studies_mut = cons(value_1(study), studies);
                        remarks_mut = append(remarks, newRemarks_4);
                        lineNumber_mut = lineNumber_5;
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
                        lineNumber_mut = lineNumber_5;
                        continue loop;
                    }
                }
                default:
                    return fromParts(investigationInfo, ontologySourceReferences, publications, contacts, reverse(studies), remarks);
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

export function toRows(investigation) {
    let remarks, rows;
    return (remarks = ofArray(investigation.Remarks), (rows = delay(() => append_1(singleton(SparseRowModule_fromValues(["ONTOLOGY SOURCE REFERENCE"])), delay(() => append_1(toRows_1(ofArray(investigation.OntologySourceReferences)), delay(() => append_1(singleton(SparseRowModule_fromValues(["INVESTIGATION"])), delay(() => append_1(InvestigationInfo_toRows_Z4D87C88C(investigation), delay(() => append_1(singleton(SparseRowModule_fromValues(["INVESTIGATION PUBLICATIONS"])), delay(() => append_1(toRows_2("Investigation Publication", ofArray(investigation.Publications)), delay(() => append_1(singleton(SparseRowModule_fromValues(["INVESTIGATION CONTACTS"])), delay(() => append_1(toRows_3("Investigation Person", ofArray(investigation.Contacts)), delay(() => collect((study) => append_1(singleton(SparseRowModule_fromValues(["STUDY"])), delay(() => toRows_4(study))), ofSeq(investigation.Studies))))))))))))))))))), (() => {
        try {
            const rm = ofList(map(Remark_toTuple_Z26CAFFFA, remarks), {
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
                        const remark = matchValue;
                        i_mut = (i + 1);
                        l_mut = l;
                        nl_mut = cons(SparseRowModule_fromValues([Remark_wrapRemark(remark)]), nl);
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

export function fromFsWorkbook(doc) {
    let arg;
    try {
        return fromRows(map_1(SparseRowModule_fromFsRow, (arg = head_1(doc.GetWorksheets()), FsWorksheet.getRows(arg))));
    }
    catch (err) {
        const arg_1 = err.message;
        return toFail(printf("Could not read investigation from spreadsheet: %s"))(arg_1);
    }
}

export function toFsWorkbook(investigation) {
    try {
        const wb = new FsWorkbook();
        const sheet = new FsWorksheet("Investigation");
        iterateIndexed((rowI, r) => {
            SparseRowModule_writeToSheet(rowI + 1, r, sheet);
        }, toRows(investigation));
        wb.AddWorksheet(sheet);
        return wb;
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Could not write investigation to spreadsheet: %s"))(arg);
    }
}

