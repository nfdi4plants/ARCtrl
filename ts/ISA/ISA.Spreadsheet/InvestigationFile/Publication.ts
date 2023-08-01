import { reverse, cons, iterate, iterateIndexed, map as map_1, initialize, singleton, length, empty, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation, OntologyAnnotation_fromString_Z7D8EB286 } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Publication_create_Z3E55064F, Publication, Publication_make } from "../../ISA/JsonTypes/Publication.js";
import { value as value_1, defaultArg, Option, map } from "../../../fable_modules/fable-library-ts/Option.js";
import { URIModule_fromString } from "../../ISA/JsonTypes/URI.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
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

export function fromString(pubMedID: string, doi: string, author: string, title: string, status: string, statusTermSourceREF: Option<string>, statusTermAccessionNumber: Option<string>, comments: FSharpList<Comment$>): Publication {
    const status_1: OntologyAnnotation = OntologyAnnotation_fromString_Z7D8EB286(status, statusTermSourceREF, statusTermAccessionNumber);
    return Publication_make(map<string, string>(URIModule_fromString, Option_fromValueWithDefault<string>("", pubMedID)), Option_fromValueWithDefault<string>("", doi), Option_fromValueWithDefault<string>("", author), Option_fromValueWithDefault<string>("", title), Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation_get_empty(), status_1), Option_fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), comments));
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Publication> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Publication_create_Z3E55064F(void 0, void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize<Publication>(matrix.ColumnCount, (i: int32): Publication => {
            const comments_1: FSharpList<Comment$> = map_1<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [pubMedIDLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [doiLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [authorListLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [titleLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [statusLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermAccessionNumberLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(publications: FSharpList<Publication>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(publications) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Publication>((i: int32, p: Publication): void => {
        const i_1: int32 = (i + 1) | 0;
        const s_1: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = OntologyAnnotation_toString_473B9D79(defaultArg(p.Status, OntologyAnnotation_get_empty()), true);
        addToDict(matrix.Matrix, [pubMedIDLabel, i_1] as [string, int32], defaultArg(p.PubMedID, ""));
        addToDict(matrix.Matrix, [doiLabel, i_1] as [string, int32], defaultArg(p.DOI, ""));
        addToDict(matrix.Matrix, [authorListLabel, i_1] as [string, int32], defaultArg(p.Authors, ""));
        addToDict(matrix.Matrix, [titleLabel, i_1] as [string, int32], defaultArg(p.Title, ""));
        addToDict(matrix.Matrix, [statusLabel, i_1] as [string, int32], s_1.TermName);
        addToDict(matrix.Matrix, [statusTermAccessionNumberLabel, i_1] as [string, int32], s_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [statusTermSourceREFLabel, i_1] as [string, int32], s_1.TermSourceREF);
        const matchValue: Option<FSharpList<Comment$>> = p.Comments;
        if (matchValue != null) {
            iterate<Comment$>((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            }, value_1(matchValue));
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
        return SparseTable_ToRows_584133C0(m);
    }
    else {
        return SparseTable_ToRows_584133C0(m, value_1(prefix));
    }
}

