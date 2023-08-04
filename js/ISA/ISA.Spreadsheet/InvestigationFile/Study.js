import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, list_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Comment$_$reflection } from "../../ISA/JsonTypes/Comment.js";
import { ofSeq, append, toArray, reverse, cons, empty, map, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { SparseRowModule_fromValues, SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133 } from "../SparseTable.js";
import { Study_fileNameFromIdentifier } from "../../ISA/ArcTypes/Identifier.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { ArcTable } from "../../ISA/ArcTypes/ArcTable.js";
import { ArcStudy } from "../../ISA/ArcTypes/ArcStudy.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./DesignDescriptors.js";
import { toRows as toRows_2, fromRows as fromRows_2 } from "./Publication.js";
import { toRows as toRows_3, fromRows as fromRows_3 } from "./Factors.js";
import { toRows as toRows_4, fromRows as fromRows_4 } from "./Assays.js";
import { toRows as toRows_5, fromRows as fromRows_5 } from "./Protocols.js";
import { toRows as toRows_6, fromRows as fromRows_6 } from "./Contacts.js";
import { singleton, append as append_1, delay, collect } from "../../../fable_modules/fable-library.4.1.4/Seq.js";

export class StudyInfo extends Record {
    constructor(Identifier, Title, Description, SubmissionDate, PublicReleaseDate, FileName, Comments) {
        super();
        this.Identifier = Identifier;
        this.Title = Title;
        this.Description = Description;
        this.SubmissionDate = SubmissionDate;
        this.PublicReleaseDate = PublicReleaseDate;
        this.FileName = FileName;
        this.Comments = Comments;
    }
}

export function StudyInfo_$reflection() {
    return record_type("ARCtrl.ISA.Spreadsheet.Studies.StudyInfo", [], StudyInfo, () => [["Identifier", string_type], ["Title", string_type], ["Description", string_type], ["SubmissionDate", string_type], ["PublicReleaseDate", string_type], ["FileName", string_type], ["Comments", list_type(Comment$_$reflection())]]);
}

export function StudyInfo_create(identifier, title, description, submissionDate, publicReleaseDate, fileName, comments) {
    return new StudyInfo(identifier, title, description, submissionDate, publicReleaseDate, fileName, comments);
}

export function StudyInfo_get_Labels() {
    return ofArray(["Study Identifier", "Study Title", "Study Description", "Study Submission Date", "Study Public Release Date", "Study File Name"]);
}

export function StudyInfo_FromSparseTable_651559CC(matrix) {
    const comments = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, 0])), matrix.CommentKeys);
    return StudyInfo_create(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Identifier", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Title", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Description", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Submission Date", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Public Release Date", 0]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study File Name", 0]), comments);
}

export function StudyInfo_ToSparseTable_1B3D5E9B(study) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, StudyInfo_get_Labels(), void 0, 2);
    let commentKeys = empty();
    const patternInput = (study.Identifier.indexOf("MISSING_IDENTIFIER_") === 0) ? ["", ""] : [study.Identifier, Study_fileNameFromIdentifier(study.Identifier)];
    addToDict(matrix.Matrix, ["Study Identifier", 1], patternInput[0]);
    addToDict(matrix.Matrix, ["Study Title", 1], defaultArg(study.Title, ""));
    addToDict(matrix.Matrix, ["Study Description", 1], defaultArg(study.Description, ""));
    addToDict(matrix.Matrix, ["Study Submission Date", 1], defaultArg(study.SubmissionDate, ""));
    addToDict(matrix.Matrix, ["Study Public Release Date", 1], defaultArg(study.PublicReleaseDate, ""));
    addToDict(matrix.Matrix, ["Study File Name", 1], patternInput[1]);
    if (!(study.Comments.length === 0)) {
        const array = study.Comments;
        array.forEach((comment) => {
            const patternInput_1 = Comment_toString(comment);
            const n = patternInput_1[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, 1], patternInput_1[1]);
        });
    }
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function StudyInfo_fromRows(lineNumber, rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, StudyInfo_get_Labels(), lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], StudyInfo_FromSparseTable_651559CC(tupledArg[3])];
}

export function StudyInfo_toRows_1B3D5E9B(study) {
    return SparseTable_ToRows_6A3D4534(StudyInfo_ToSparseTable_1B3D5E9B(study));
}

export function fromParts(studyInfo, designDescriptors, publications, factors, assays, protocols, contacts) {
    let arcstudy;
    const arg_1 = Option_fromValueWithDefault("", studyInfo.Title);
    const arg_2 = Option_fromValueWithDefault("", studyInfo.Description);
    const arg_3 = Option_fromValueWithDefault("", studyInfo.SubmissionDate);
    const arg_4 = Option_fromValueWithDefault("", studyInfo.PublicReleaseDate);
    const arg_5 = toArray(publications);
    const arg_6 = toArray(contacts);
    const arg_7 = toArray(designDescriptors);
    let arg_8;
    const arg_13 = map((arg_12) => ArcTable.fromProtocol(arg_12), protocols);
    arg_8 = Array.from(arg_13);
    const arg_9 = Array.from(assays);
    const arg_10 = toArray(factors);
    const arg_11 = toArray(studyInfo.Comments);
    arcstudy = ArcStudy.make(studyInfo.Identifier, arg_1, arg_2, arg_3, arg_4, arg_5, arg_6, arg_7, arg_8, arg_9, arg_10, arg_11);
    if (arcstudy.isEmpty && (arcstudy.Identifier === "")) {
        return void 0;
    }
    else {
        return arcstudy;
    }
}

export function fromRows(lineNumber, en) {
    const loop = (lastLine_mut, studyInfo_mut, designDescriptors_mut, publications_mut, factors_mut, assays_mut, protocols_mut, contacts_mut, remarks_mut, lineNumber_1_mut) => {
        loop:
        while (true) {
            const lastLine = lastLine_mut, studyInfo = studyInfo_mut, designDescriptors = designDescriptors_mut, publications = publications_mut, factors = factors_mut, assays = assays_mut, protocols = protocols_mut, contacts = contacts_mut, remarks = remarks_mut, lineNumber_1 = lineNumber_1_mut;
            let matchResult, k_6, k_7, k_8, k_9, k_10, k_11, k_12;
            if (lastLine != null) {
                switch (lastLine) {
                    case "STUDY DESIGN DESCRIPTORS": {
                        matchResult = 0;
                        k_6 = lastLine;
                        break;
                    }
                    case "STUDY PUBLICATIONS": {
                        matchResult = 1;
                        k_7 = lastLine;
                        break;
                    }
                    case "STUDY FACTORS": {
                        matchResult = 2;
                        k_8 = lastLine;
                        break;
                    }
                    case "STUDY ASSAYS": {
                        matchResult = 3;
                        k_9 = lastLine;
                        break;
                    }
                    case "STUDY PROTOCOLS": {
                        matchResult = 4;
                        k_10 = lastLine;
                        break;
                    }
                    case "STUDY CONTACTS": {
                        matchResult = 5;
                        k_11 = lastLine;
                        break;
                    }
                    default: {
                        matchResult = 6;
                        k_12 = lastLine;
                    }
                }
            }
            else {
                matchResult = 6;
                k_12 = lastLine;
            }
            switch (matchResult) {
                case 0: {
                    const patternInput = fromRows_1("Study Design", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = patternInput[3];
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append(remarks, patternInput[2]);
                    lineNumber_1_mut = patternInput[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_1 = fromRows_2("Study Publication", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_1[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = patternInput_1[3];
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append(remarks, patternInput_1[2]);
                    lineNumber_1_mut = patternInput_1[1];
                    continue loop;
                }
                case 2: {
                    const patternInput_2 = fromRows_3("Study Factor", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_2[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = patternInput_2[3];
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append(remarks, patternInput_2[2]);
                    lineNumber_1_mut = patternInput_2[1];
                    continue loop;
                }
                case 3: {
                    const patternInput_3 = fromRows_4("Study Assay", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_3[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = patternInput_3[3];
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append(remarks, patternInput_3[2]);
                    lineNumber_1_mut = patternInput_3[1];
                    continue loop;
                }
                case 4: {
                    const patternInput_4 = fromRows_5("Study Protocol", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_4[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = patternInput_4[3];
                    contacts_mut = contacts;
                    remarks_mut = append(remarks, patternInput_4[2]);
                    lineNumber_1_mut = patternInput_4[1];
                    continue loop;
                }
                case 5: {
                    const patternInput_5 = fromRows_6("Study Person", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_5[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = patternInput_5[3];
                    remarks_mut = append(remarks, patternInput_5[2]);
                    lineNumber_1_mut = patternInput_5[1];
                    continue loop;
                }
                default:
                    return [k_12, lineNumber_1, remarks, fromParts(studyInfo, designDescriptors, publications, factors, assays, protocols, contacts)];
            }
            break;
        }
    };
    const patternInput_6 = StudyInfo_fromRows(lineNumber, en);
    return loop(patternInput_6[0], patternInput_6[3], empty(), empty(), empty(), empty(), empty(), empty(), patternInput_6[2], patternInput_6[1]);
}

export function toRows(study) {
    const protocols = ofSeq(collect((p) => p.GetProtocols(), study.Tables));
    return delay(() => append_1(StudyInfo_toRows_1B3D5E9B(study), delay(() => append_1(singleton(SparseRowModule_fromValues(["STUDY DESIGN DESCRIPTORS"])), delay(() => append_1(toRows_1("Study Design", ofArray(study.StudyDesignDescriptors)), delay(() => append_1(singleton(SparseRowModule_fromValues(["STUDY PUBLICATIONS"])), delay(() => append_1(toRows_2("Study Publication", ofArray(study.Publications)), delay(() => append_1(singleton(SparseRowModule_fromValues(["STUDY FACTORS"])), delay(() => append_1(toRows_3("Study Factor", ofArray(study.Factors)), delay(() => append_1(singleton(SparseRowModule_fromValues(["STUDY ASSAYS"])), delay(() => append_1(toRows_4("Study Assay", ofSeq(study.Assays)), delay(() => append_1(singleton(SparseRowModule_fromValues(["STUDY PROTOCOLS"])), delay(() => append_1(toRows_5("Study Protocol", protocols), delay(() => append_1(singleton(SparseRowModule_fromValues(["STUDY CONTACTS"])), delay(() => toRows_6("Study Person", ofArray(study.Contacts)))))))))))))))))))))))))));
}

