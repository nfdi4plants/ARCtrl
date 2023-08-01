import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { head, tail, ofSeq, append, reverse, cons, iterate, isEmpty, empty, map, ofArray, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Remark_toTuple_Z2023CF4E, Remark, Comment$_$reflection, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { comparePrimitives, getEnumerator, IEnumerator, stringHash, IComparable, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { record_type, list_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Remark_wrapRemark, Comment_toString, Comment_fromString } from "../Comment.js";
import { SparseRowModule_writeToSheet, SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_tryGetValueAt, SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133 } from "../SparseTable.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { value as value_1, Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { ArcInvestigation } from "../../ISA/ArcTypes/ArcInvestigation.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { ArcStudy } from "../../ISA/ArcTypes/ArcStudy.js";
import { OntologySourceReference } from "../../ISA/JsonTypes/OntologySourceReference.js";
import { Publication } from "../../ISA/JsonTypes/Publication.js";
import { Person } from "../../ISA/JsonTypes/Person.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./OntologySourceReference.js";
import { toRows as toRows_2, fromRows as fromRows_2 } from "./Publication.js";
import { toRows as toRows_3, fromRows as fromRows_3 } from "./Contacts.js";
import { toRows as toRows_4, fromRows as fromRows_4 } from "./Study.js";
import { iterateIndexed, map as map_1, toList, collect, singleton, append as append_1, delay } from "../../../fable_modules/fable-library-ts/Seq.js";
import { tryFind, FSharpMap, ofList } from "../../../fable_modules/fable-library-ts/Map.js";
import { printf, toFail } from "../../../fable_modules/fable-library-ts/String.js";
import { FsRow } from "../../../fable_modules/FsSpreadsheet.3.1.1/FsRow.fs.js";
import { FsWorksheet } from "../../../fable_modules/FsSpreadsheet.3.1.1/FsWorksheet.fs.js";
import { FsWorkbook } from "../../../fable_modules/FsSpreadsheet.3.1.1/FsWorkbook.fs.js";

export class InvestigationInfo extends Record implements IEquatable<InvestigationInfo>, IComparable<InvestigationInfo> {
    readonly Identifier: string;
    readonly Title: string;
    readonly Description: string;
    readonly SubmissionDate: string;
    readonly PublicReleaseDate: string;
    readonly Comments: FSharpList<Comment$>;
    constructor(Identifier: string, Title: string, Description: string, SubmissionDate: string, PublicReleaseDate: string, Comments: FSharpList<Comment$>) {
        super();
        this.Identifier = Identifier;
        this.Title = Title;
        this.Description = Description;
        this.SubmissionDate = SubmissionDate;
        this.PublicReleaseDate = PublicReleaseDate;
        this.Comments = Comments;
    }
}

export function InvestigationInfo_$reflection(): TypeInfo {
    return record_type("ISA.Spreadsheet.ArcInvestigation.InvestigationInfo", [], InvestigationInfo, () => [["Identifier", string_type], ["Title", string_type], ["Description", string_type], ["SubmissionDate", string_type], ["PublicReleaseDate", string_type], ["Comments", list_type(Comment$_$reflection())]]);
}

export function InvestigationInfo_create(identifier: string, title: string, description: string, submissionDate: string, publicReleaseDate: string, comments: FSharpList<Comment$>): InvestigationInfo {
    return new InvestigationInfo(identifier, title, description, submissionDate, publicReleaseDate, comments);
}

export function InvestigationInfo_get_Labels(): FSharpList<string> {
    return ofArray(["Investigation Identifier", "Investigation Title", "Investigation Description", "Investigation Submission Date", "Investigation Public Release Date"]);
}

export function InvestigationInfo_FromSparseTable_Z15A4F148(matrix: SparseTable): InvestigationInfo {
    const comments: FSharpList<Comment$> = map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, 0] as [string, int32])), matrix.CommentKeys);
    return InvestigationInfo_create(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Identifier", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Title", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Description", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Submission Date", 0] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Investigation Public Release Date", 0] as [string, int32]), comments);
}

export function InvestigationInfo_ToSparseTable_Z1FC82C0(investigation: ArcInvestigation): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, InvestigationInfo_get_Labels(), void 0, 2);
    let commentKeys: FSharpList<string> = empty<string>();
    addToDict(matrix.Matrix, ["Investigation Identifier", 1] as [string, int32], investigation.Identifier);
    addToDict(matrix.Matrix, ["Investigation Title", 1] as [string, int32], defaultArg(investigation.Title, ""));
    addToDict(matrix.Matrix, ["Investigation Description", 1] as [string, int32], defaultArg(investigation.Description, ""));
    addToDict(matrix.Matrix, ["Investigation Submission Date", 1] as [string, int32], defaultArg(investigation.SubmissionDate, ""));
    addToDict(matrix.Matrix, ["Investigation Public Release Date", 1] as [string, int32], defaultArg(investigation.PublicReleaseDate, ""));
    if (!isEmpty(investigation.Comments)) {
        iterate<Comment$>((comment: Comment$): void => {
            const patternInput: [string, string] = Comment_toString(comment);
            const n: string = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, 1] as [string, int32], patternInput[1]);
        }, investigation.Comments);
    }
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function InvestigationInfo_fromRows(lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, InvestigationInfo] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = SparseTable_FromRows_Z5579EC29(rows, InvestigationInfo_get_Labels(), lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], InvestigationInfo_FromSparseTable_Z15A4F148(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, InvestigationInfo];
}

export function InvestigationInfo_toRows_Z1FC82C0(investigation: ArcInvestigation): Iterable<Iterable<[int32, string]>> {
    return SparseTable_ToRows_584133C0(InvestigationInfo_ToSparseTable_Z1FC82C0(investigation));
}

export function fromParts(investigationInfo: InvestigationInfo, ontologySourceReference: FSharpList<OntologySourceReference>, publications: FSharpList<Publication>, contacts: FSharpList<Person>, studies: FSharpList<ArcStudy>, remarks: FSharpList<Remark>): ArcInvestigation {
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", investigationInfo.Title);
    const arg_2: Option<string> = Option_fromValueWithDefault<string>("", investigationInfo.Description);
    const arg_3: Option<string> = Option_fromValueWithDefault<string>("", investigationInfo.SubmissionDate);
    const arg_4: Option<string> = Option_fromValueWithDefault<string>("", investigationInfo.PublicReleaseDate);
    const arg_8: ArcStudy[] = Array.from(studies);
    return ArcInvestigation.make(investigationInfo.Identifier, arg_1, arg_2, arg_3, arg_4, ontologySourceReference, publications, contacts, arg_8, investigationInfo.Comments, remarks);
}

export function fromRows(rows: Iterable<Iterable<[int32, string]>>): ArcInvestigation {
    const en: IEnumerator<Iterable<[int32, string]>> = getEnumerator(rows);
    const emptyInvestigationInfo: InvestigationInfo = InvestigationInfo_create("", "", "", "", "", empty<Comment$>());
    const loop = (lastLine_mut: Option<string>, ontologySourceReferences_mut: FSharpList<OntologySourceReference>, investigationInfo_mut: InvestigationInfo, publications_mut: FSharpList<Publication>, contacts_mut: FSharpList<Person>, studies_mut: FSharpList<ArcStudy>, remarks_mut: FSharpList<Remark>, lineNumber_mut: int32): ArcInvestigation => {
        loop:
        while (true) {
            const lastLine: Option<string> = lastLine_mut, ontologySourceReferences: FSharpList<OntologySourceReference> = ontologySourceReferences_mut, investigationInfo: InvestigationInfo = investigationInfo_mut, publications: FSharpList<Publication> = publications_mut, contacts: FSharpList<Person> = contacts_mut, studies: FSharpList<ArcStudy> = studies_mut, remarks: FSharpList<Remark> = remarks_mut, lineNumber: int32 = lineNumber_mut;
            let matchResult: int32, k_5: string, k_6: string, k_7: string, k_8: string, k_9: string, k_10: Option<string>;
            if (lastLine != null) {
                switch (value_1(lastLine)) {
                    case "ONTOLOGY SOURCE REFERENCE": {
                        matchResult = 0;
                        k_5 = value_1(lastLine);
                        break;
                    }
                    case "INVESTIGATION": {
                        matchResult = 1;
                        k_6 = value_1(lastLine);
                        break;
                    }
                    case "INVESTIGATION PUBLICATIONS": {
                        matchResult = 2;
                        k_7 = value_1(lastLine);
                        break;
                    }
                    case "INVESTIGATION CONTACTS": {
                        matchResult = 3;
                        k_8 = value_1(lastLine);
                        break;
                    }
                    case "STUDY": {
                        matchResult = 4;
                        k_9 = value_1(lastLine);
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
                    const patternInput: [Option<string>, int32, FSharpList<Remark>, FSharpList<OntologySourceReference>] = fromRows_1(lineNumber + 1, en);
                    lastLine_mut = patternInput[0];
                    ontologySourceReferences_mut = patternInput[3];
                    investigationInfo_mut = investigationInfo;
                    publications_mut = publications;
                    contacts_mut = contacts;
                    studies_mut = studies;
                    remarks_mut = append<Remark>(remarks, patternInput[2]);
                    lineNumber_mut = patternInput[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_1: [Option<string>, int32, FSharpList<Remark>, InvestigationInfo] = InvestigationInfo_fromRows(lineNumber + 1, en);
                    lastLine_mut = patternInput_1[0];
                    ontologySourceReferences_mut = ontologySourceReferences;
                    investigationInfo_mut = patternInput_1[3];
                    publications_mut = publications;
                    contacts_mut = contacts;
                    studies_mut = studies;
                    remarks_mut = append<Remark>(remarks, patternInput_1[2]);
                    lineNumber_mut = patternInput_1[1];
                    continue loop;
                }
                case 2: {
                    const patternInput_2: [Option<string>, int32, FSharpList<Remark>, FSharpList<Publication>] = fromRows_2("Investigation Publication", lineNumber + 1, en);
                    lastLine_mut = patternInput_2[0];
                    ontologySourceReferences_mut = ontologySourceReferences;
                    investigationInfo_mut = investigationInfo;
                    publications_mut = patternInput_2[3];
                    contacts_mut = contacts;
                    studies_mut = studies;
                    remarks_mut = append<Remark>(remarks, patternInput_2[2]);
                    lineNumber_mut = patternInput_2[1];
                    continue loop;
                }
                case 3: {
                    const patternInput_3: [Option<string>, int32, FSharpList<Remark>, FSharpList<Person>] = fromRows_3("Investigation Person", lineNumber + 1, en);
                    lastLine_mut = patternInput_3[0];
                    ontologySourceReferences_mut = ontologySourceReferences;
                    investigationInfo_mut = investigationInfo;
                    publications_mut = publications;
                    contacts_mut = patternInput_3[3];
                    studies_mut = studies;
                    remarks_mut = append<Remark>(remarks, patternInput_3[2]);
                    lineNumber_mut = patternInput_3[1];
                    continue loop;
                }
                case 4: {
                    const patternInput_4: [Option<string>, int32, FSharpList<Remark>, Option<ArcStudy>] = fromRows_4(lineNumber + 1, en);
                    const study: Option<ArcStudy> = patternInput_4[3];
                    const newRemarks_4: FSharpList<Remark> = patternInput_4[2];
                    const lineNumber_5: int32 = patternInput_4[1] | 0;
                    const currentLine_4: Option<string> = patternInput_4[0];
                    if (study != null) {
                        lastLine_mut = currentLine_4;
                        ontologySourceReferences_mut = ontologySourceReferences;
                        investigationInfo_mut = investigationInfo;
                        publications_mut = publications;
                        contacts_mut = contacts;
                        studies_mut = cons(value_1(study), studies);
                        remarks_mut = append<Remark>(remarks, newRemarks_4);
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
                        remarks_mut = append<Remark>(remarks, newRemarks_4);
                        lineNumber_mut = lineNumber_5;
                        continue loop;
                    }
                }
                default:
                    return fromParts(investigationInfo, ontologySourceReferences, publications, contacts, reverse<ArcStudy>(studies), remarks);
            }
            break;
        }
    };
    if (en["System.Collections.IEnumerator.MoveNext"]()) {
        return loop(SparseRowModule_tryGetValueAt(0, en["System.Collections.Generic.IEnumerator`1.get_Current"]()), empty<OntologySourceReference>(), emptyInvestigationInfo, empty<Publication>(), empty<Person>(), empty<ArcStudy>(), empty<Remark>(), 1);
    }
    else {
        throw new Error("emptyInvestigationFile");
    }
}

export function toRows(investigation: ArcInvestigation): Iterable<Iterable<[int32, string]>> {
    let remarks: FSharpList<Remark>, rows: Iterable<Iterable<[int32, string]>>;
    return (remarks = investigation.Remarks, (rows = delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["ONTOLOGY SOURCE REFERENCE"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_1(investigation.OntologySourceReferences), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["INVESTIGATION"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(InvestigationInfo_toRows_Z1FC82C0(investigation), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["INVESTIGATION PUBLICATIONS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_2("Investigation Publication", investigation.Publications), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["INVESTIGATION CONTACTS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(toRows_3("Investigation Person", investigation.Contacts), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => collect<ArcStudy, Iterable<Iterable<[int32, string]>>, Iterable<[int32, string]>>((study: ArcStudy): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => toRows_4(study))), ofSeq<ArcStudy>(investigation.Studies))))))))))))))))))), (() => {
        try {
            const rm: FSharpMap<int32, string> = ofList<int32, string>(map<Remark, [int32, string]>(Remark_toTuple_Z2023CF4E, remarks), {
                Compare: comparePrimitives,
            });
            const loop = (i_mut: int32, l_mut: FSharpList<Iterable<[int32, string]>>, nl_mut: FSharpList<Iterable<[int32, string]>>): FSharpList<Iterable<[int32, string]>> => {
                loop:
                while (true) {
                    const i: int32 = i_mut, l: FSharpList<Iterable<[int32, string]>> = l_mut, nl: FSharpList<Iterable<[int32, string]>> = nl_mut;
                    const matchValue: Option<string> = tryFind<int32, string>(i, rm);
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
                        const remark: string = value_1(matchValue);
                        i_mut = (i + 1);
                        l_mut = l;
                        nl_mut = cons(SparseRowModule_fromValues([Remark_wrapRemark(remark)]), nl);
                        continue loop;
                    }
                    break;
                }
            };
            return reverse<Iterable<[int32, string]>>(loop(1, ofSeq<Iterable<[int32, string]>>(rows), empty<Iterable<[int32, string]>>()));
        }
        catch (matchValue_1: any) {
            return toList<Iterable<[int32, string]>>(rows);
        }
    })()));
}

export function fromFsWorkbook(doc: FsWorkbook): ArcInvestigation {
    let arg: FsWorksheet;
    try {
        return fromRows(map_1<FsRow, Iterable<[int32, string]>>(SparseRowModule_fromFsRow, (arg = head<FsWorksheet>(doc.GetWorksheets()), FsWorksheet.getRows(arg))));
    }
    catch (err: any) {
        const arg_1: string = err.message;
        return toFail(printf("Could not read investigation from spreadsheet: %s"))(arg_1);
    }
}

export function toFsWorkbook(investigation: ArcInvestigation): FsWorkbook {
    try {
        const wb: FsWorkbook = new FsWorkbook();
        const sheet: FsWorksheet = new FsWorksheet("Investigation");
        iterateIndexed<Iterable<[int32, string]>>((rowI: int32, r: Iterable<[int32, string]>): void => {
            SparseRowModule_writeToSheet(rowI + 1, r, sheet);
        }, toRows(investigation));
        wb.AddWorksheet(sheet);
        return wb;
    }
    catch (err: any) {
        const arg: string = err.message;
        return toFail(printf("Could not write investigation to spreadsheet: %s"))(arg);
    }
}

