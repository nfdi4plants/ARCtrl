import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { ofSeq, append, toArray, reverse, cons, empty, map, ofArray, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Remark, Comment$_$reflection, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { IEnumerator, stringHash, IComparable, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, list_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { SparseRowModule_fromValues, SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133 } from "../SparseTable.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { Study_fileNameFromIdentifier } from "../../ISA/ArcTypes/Identifier.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { value as value_1, Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { ArcStudy } from "../../ISA/ArcTypes/ArcStudy.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Publication } from "../../ISA/JsonTypes/Publication.js";
import { Person } from "../../ISA/JsonTypes/Person.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { ArcTable } from "../../ISA/ArcTypes/ArcTable.js";
import { Protocol } from "../../ISA/JsonTypes/Protocol.js";
import { ArcAssay } from "../../ISA/ArcTypes/ArcAssay.js";
import { Factor } from "../../ISA/JsonTypes/Factor.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./DesignDescriptors.js";
import { toRows as toRows_2, fromRows as fromRows_2 } from "./Publication.js";
import { toRows as toRows_3, fromRows as fromRows_3 } from "./Factors.js";
import { toRows as toRows_4, fromRows as fromRows_4 } from "./Assays.js";
import { toRows as toRows_5, fromRows as fromRows_5 } from "./Protocols.js";
import { toRows as toRows_6, fromRows as fromRows_6 } from "./Contacts.js";
import { singleton, append as append_1, delay, collect } from "../../../fable_modules/fable-library-ts/Seq.js";

export class StudyInfo extends Record implements IEquatable<StudyInfo>, IComparable<StudyInfo> {
    readonly Identifier: string;
    readonly Title: string;
    readonly Description: string;
    readonly SubmissionDate: string;
    readonly PublicReleaseDate: string;
    readonly FileName: string;
    readonly Comments: FSharpList<Comment$>;
    constructor(Identifier: string, Title: string, Description: string, SubmissionDate: string, PublicReleaseDate: string, FileName: string, Comments: FSharpList<Comment$>) {
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

export function StudyInfo_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Spreadsheet.Studies.StudyInfo", [], StudyInfo, () => [["Identifier", string_type], ["Title", string_type], ["Description", string_type], ["SubmissionDate", string_type], ["PublicReleaseDate", string_type], ["FileName", string_type], ["Comments", list_type(Comment$_$reflection())]]);
}

export function StudyInfo_create(identifier: string, title: string, description: string, submissionDate: string, publicReleaseDate: string, fileName: string, comments: FSharpList<Comment$>): StudyInfo {
    return new StudyInfo(identifier, title, description, submissionDate, publicReleaseDate, fileName, comments);
}

export function StudyInfo_get_Labels(): FSharpList<string> {
    return ofArray(["Study Identifier", "Study Title", "Study Description", "Study Submission Date", "Study Public Release Date", "Study File Name"]);
}

export function StudyInfo_FromSparseTable_651559CC(matrix: SparseTable): StudyInfo {
    const comments: FSharpList<Comment$> = map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, 0] as [string, int32])), matrix.CommentKeys);
    return StudyInfo_create(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Identifier", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Title", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Description", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Submission Date", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study Public Release Date", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Study File Name", 0] as [string, int32]), comments);
}

export function StudyInfo_ToSparseTable_1B3D5E9B(study: ArcStudy): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, StudyInfo_get_Labels(), void 0, 2);
    let commentKeys: FSharpList<string> = empty<string>();
    const patternInput: [string, string] = (study.Identifier.indexOf("MISSING_IDENTIFIER_") === 0) ? (["", ""] as [string, string]) : ([study.Identifier, Study_fileNameFromIdentifier(study.Identifier)] as [string, string]);
    addToDict(matrix.Matrix, ["Study Identifier", 1] as [string, int32], patternInput[0]);
    addToDict(matrix.Matrix, ["Study Title", 1] as [string, int32], defaultArg(study.Title, ""));
    addToDict(matrix.Matrix, ["Study Description", 1] as [string, int32], defaultArg(study.Description, ""));
    addToDict(matrix.Matrix, ["Study Submission Date", 1] as [string, int32], defaultArg(study.SubmissionDate, ""));
    addToDict(matrix.Matrix, ["Study Public Release Date", 1] as [string, int32], defaultArg(study.PublicReleaseDate, ""));
    addToDict(matrix.Matrix, ["Study File Name", 1] as [string, int32], patternInput[1]);
    if (!(study.Comments.length === 0)) {
        const array: Comment$[] = study.Comments;
        array.forEach((comment: Comment$): void => {
            const patternInput_1: [string, string] = Comment_toString(comment);
            const n: string = patternInput_1[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, 1] as [string, int32], patternInput_1[1]);
        });
    }
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function StudyInfo_fromRows(lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, StudyInfo] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = SparseTable_FromRows_Z5579EC29(rows, StudyInfo_get_Labels(), lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], StudyInfo_FromSparseTable_651559CC(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, StudyInfo];
}

export function StudyInfo_toRows_1B3D5E9B(study: ArcStudy): Iterable<Iterable<[int32, string]>> {
    return SparseTable_ToRows_6A3D4534(StudyInfo_ToSparseTable_1B3D5E9B(study));
}

export function fromParts(studyInfo: StudyInfo, designDescriptors: FSharpList<OntologyAnnotation>, publications: FSharpList<Publication>, factors: FSharpList<Factor>, assays: FSharpList<ArcAssay>, protocols: FSharpList<Protocol>, contacts: FSharpList<Person>): Option<ArcStudy> {
    let arcstudy: ArcStudy;
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", studyInfo.Title);
    const arg_2: Option<string> = Option_fromValueWithDefault<string>("", studyInfo.Description);
    const arg_3: Option<string> = Option_fromValueWithDefault<string>("", studyInfo.SubmissionDate);
    const arg_4: Option<string> = Option_fromValueWithDefault<string>("", studyInfo.PublicReleaseDate);
    const arg_5: Publication[] = toArray<Publication>(publications);
    const arg_6: Person[] = toArray<Person>(contacts);
    const arg_7: OntologyAnnotation[] = toArray<OntologyAnnotation>(designDescriptors);
    let arg_8: ArcTable[];
    const arg_13: FSharpList<ArcTable> = map<Protocol, ArcTable>((arg_12: Protocol): ArcTable => ArcTable.fromProtocol(arg_12), protocols);
    arg_8 = Array.from(arg_13);
    const arg_9: ArcAssay[] = Array.from(assays);
    const arg_10: Factor[] = toArray<Factor>(factors);
    const arg_11: Comment$[] = toArray<Comment$>(studyInfo.Comments);
    arcstudy = ArcStudy.make(studyInfo.Identifier, arg_1, arg_2, arg_3, arg_4, arg_5, arg_6, arg_7, arg_8, arg_9, arg_10, arg_11);
    if (arcstudy.isEmpty && (arcstudy.Identifier === "")) {
        return void 0;
    }
    else {
        return arcstudy;
    }
}

export function fromRows(lineNumber: int32, en: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, Option<ArcStudy>] {
    const loop = (lastLine_mut: Option<string>, studyInfo_mut: StudyInfo, designDescriptors_mut: FSharpList<OntologyAnnotation>, publications_mut: FSharpList<Publication>, factors_mut: FSharpList<Factor>, assays_mut: FSharpList<ArcAssay>, protocols_mut: FSharpList<Protocol>, contacts_mut: FSharpList<Person>, remarks_mut: FSharpList<Remark>, lineNumber_1_mut: int32): [Option<string>, int32, FSharpList<Remark>, Option<ArcStudy>] => {
        loop:
        while (true) {
            const lastLine: Option<string> = lastLine_mut, studyInfo: StudyInfo = studyInfo_mut, designDescriptors: FSharpList<OntologyAnnotation> = designDescriptors_mut, publications: FSharpList<Publication> = publications_mut, factors: FSharpList<Factor> = factors_mut, assays: FSharpList<ArcAssay> = assays_mut, protocols: FSharpList<Protocol> = protocols_mut, contacts: FSharpList<Person> = contacts_mut, remarks: FSharpList<Remark> = remarks_mut, lineNumber_1: int32 = lineNumber_1_mut;
            let matchResult: int32, k_6: string, k_7: string, k_8: string, k_9: string, k_10: string, k_11: string, k_12: Option<string>;
            if (lastLine != null) {
                switch (value_1(lastLine)) {
                    case "STUDY DESIGN DESCRIPTORS": {
                        matchResult = 0;
                        k_6 = value_1(lastLine);
                        break;
                    }
                    case "STUDY PUBLICATIONS": {
                        matchResult = 1;
                        k_7 = value_1(lastLine);
                        break;
                    }
                    case "STUDY FACTORS": {
                        matchResult = 2;
                        k_8 = value_1(lastLine);
                        break;
                    }
                    case "STUDY ASSAYS": {
                        matchResult = 3;
                        k_9 = value_1(lastLine);
                        break;
                    }
                    case "STUDY PROTOCOLS": {
                        matchResult = 4;
                        k_10 = value_1(lastLine);
                        break;
                    }
                    case "STUDY CONTACTS": {
                        matchResult = 5;
                        k_11 = value_1(lastLine);
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
                    const patternInput: [Option<string>, int32, FSharpList<Remark>, FSharpList<OntologyAnnotation>] = fromRows_1("Study Design", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = patternInput[3];
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append<Remark>(remarks, patternInput[2]);
                    lineNumber_1_mut = patternInput[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_1: [Option<string>, int32, FSharpList<Remark>, FSharpList<Publication>] = fromRows_2("Study Publication", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_1[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = patternInput_1[3];
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append<Remark>(remarks, patternInput_1[2]);
                    lineNumber_1_mut = patternInput_1[1];
                    continue loop;
                }
                case 2: {
                    const patternInput_2: [Option<string>, int32, FSharpList<Remark>, FSharpList<Factor>] = fromRows_3("Study Factor", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_2[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = patternInput_2[3];
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append<Remark>(remarks, patternInput_2[2]);
                    lineNumber_1_mut = patternInput_2[1];
                    continue loop;
                }
                case 3: {
                    const patternInput_3: [Option<string>, int32, FSharpList<Remark>, FSharpList<ArcAssay>] = fromRows_4("Study Assay", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_3[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = patternInput_3[3];
                    protocols_mut = protocols;
                    contacts_mut = contacts;
                    remarks_mut = append<Remark>(remarks, patternInput_3[2]);
                    lineNumber_1_mut = patternInput_3[1];
                    continue loop;
                }
                case 4: {
                    const patternInput_4: [Option<string>, int32, FSharpList<Remark>, FSharpList<Protocol>] = fromRows_5("Study Protocol", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_4[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = patternInput_4[3];
                    contacts_mut = contacts;
                    remarks_mut = append<Remark>(remarks, patternInput_4[2]);
                    lineNumber_1_mut = patternInput_4[1];
                    continue loop;
                }
                case 5: {
                    const patternInput_5: [Option<string>, int32, FSharpList<Remark>, FSharpList<Person>] = fromRows_6("Study Person", lineNumber_1 + 1, en);
                    lastLine_mut = patternInput_5[0];
                    studyInfo_mut = studyInfo;
                    designDescriptors_mut = designDescriptors;
                    publications_mut = publications;
                    factors_mut = factors;
                    assays_mut = assays;
                    protocols_mut = protocols;
                    contacts_mut = patternInput_5[3];
                    remarks_mut = append<Remark>(remarks, patternInput_5[2]);
                    lineNumber_1_mut = patternInput_5[1];
                    continue loop;
                }
                default:
                    return [k_12!, lineNumber_1, remarks, fromParts(studyInfo, designDescriptors, publications, factors, assays, protocols, contacts)] as [Option<string>, int32, FSharpList<Remark>, Option<ArcStudy>];
            }
            break;
        }
    };
    const patternInput_6: [Option<string>, int32, FSharpList<Remark>, StudyInfo] = StudyInfo_fromRows(lineNumber, en);
    return loop(patternInput_6[0], patternInput_6[3], empty<OntologyAnnotation>(), empty<Publication>(), empty<Factor>(), empty<ArcAssay>(), empty<Protocol>(), empty<Person>(), patternInput_6[2], patternInput_6[1]);
}

export function toRows(study: ArcStudy): Iterable<Iterable<[int32, string]>> {
    const protocols: FSharpList<Protocol> = ofSeq<Protocol>(collect<ArcTable, FSharpList<Protocol>, Protocol>((p: ArcTable): FSharpList<Protocol> => p.GetProtocols(), study.Tables));
    return delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(StudyInfo_toRows_1B3D5E9B(study), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY DESIGN DESCRIPTORS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_1("Study Design", ofArray<OntologyAnnotation>(study.StudyDesignDescriptors)), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY PUBLICATIONS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_2("Study Publication", ofArray<Publication>(study.Publications)), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY FACTORS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_3("Study Factor", ofArray<Factor>(study.Factors)), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY ASSAYS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_4("Study Assay", ofSeq<ArcAssay>(study.Assays)), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY PROTOCOLS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_5("Study Protocol", protocols), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY CONTACTS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => toRows_6("Study Person", ofArray<Person>(study.Contacts)))))))))))))))))))))))))));
}

