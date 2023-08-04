import { reverse, cons, iterateIndexed, empty, map as map_1, toArray, initialize, singleton, length, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { value as value_1, defaultArg, Option, map } from "../../../fable_modules/fable-library-ts/Option.js";
import { URIModule_fromString } from "../../ISA/JsonTypes/URI.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { Publication } from "../../ISA/JsonTypes/Publication.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

export const pubMedIDLabel = "PubMed ID";

export const doiLabel = "DOI";

export const authorListLabel = "Author List";

export const titleLabel = "Title";

export const statusLabel = "Status";

export const statusTermAccessionNumberLabel = "Status Term Accession Number";

export const statusTermSourceREFLabel = "Status Term Source REF";

export const labels: FSharpList<string> = ofArray([pubMedIDLabel, doiLabel, authorListLabel, titleLabel, statusLabel, statusTermAccessionNumberLabel, statusTermSourceREFLabel]);

export function fromString(pubMedID: string, doi: string, author: string, title: string, status: string, statusTermSourceREF: Option<string>, statusTermAccessionNumber: Option<string>, comments: Comment$[]): Publication {
    const status_1: OntologyAnnotation = OntologyAnnotation.fromString(status, statusTermSourceREF, statusTermAccessionNumber);
    const arg: Option<string> = map<string, string>(URIModule_fromString, Option_fromValueWithDefault<string>("", pubMedID));
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", doi);
    const arg_2: Option<string> = Option_fromValueWithDefault<string>("", author);
    const arg_3: Option<string> = Option_fromValueWithDefault<string>("", title);
    const arg_4: Option<OntologyAnnotation> = Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, status_1);
    const arg_5: Option<Comment$[]> = Option_fromValueWithDefault<Comment$[]>([], comments);
    return Publication.make(arg, arg_1, arg_2, arg_3, arg_4, arg_5);
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Publication> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments: Comment$[] = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(Publication.create(void 0, void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize<Publication>(matrix.ColumnCount, (i: int32): Publication => {
            const comments_1: Comment$[] = toArray<Comment$>(map_1<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [pubMedIDLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [doiLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [authorListLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [titleLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [statusLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermAccessionNumberLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(publications: FSharpList<Publication>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(publications) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Publication>((i: int32, p: Publication): void => {
        const i_1: int32 = (i + 1) | 0;
        let s_1: { TermAccessionNumber: string, TermName: string, TermSourceREF: string };
        const s: OntologyAnnotation = defaultArg(p.Status, OntologyAnnotation.empty);
        s_1 = OntologyAnnotation.toString(s, true);
        addToDict(matrix.Matrix, [pubMedIDLabel, i_1] as [string, int32], defaultArg(p.PubMedID, ""));
        addToDict(matrix.Matrix, [doiLabel, i_1] as [string, int32], defaultArg(p.DOI, ""));
        addToDict(matrix.Matrix, [authorListLabel, i_1] as [string, int32], defaultArg(p.Authors, ""));
        addToDict(matrix.Matrix, [titleLabel, i_1] as [string, int32], defaultArg(p.Title, ""));
        addToDict(matrix.Matrix, [statusLabel, i_1] as [string, int32], s_1.TermName);
        addToDict(matrix.Matrix, [statusTermAccessionNumberLabel, i_1] as [string, int32], s_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [statusTermSourceREFLabel, i_1] as [string, int32], s_1.TermSourceREF);
        const matchValue: Option<Comment$[]> = p.Comments;
        if (matchValue != null) {
            const array: Comment$[] = value_1(matchValue);
            array.forEach((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            });
        }
    }, publications);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix: Option<string>, lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<Publication>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, value_1(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<Publication>];
}

export function toRows(prefix: Option<string>, publications: FSharpList<Publication>): Iterable<Iterable<[int32, string]>> {
    const m: SparseTable = toSparseTable(publications);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, value_1(prefix));
    }
}

