import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { value as value_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

export const designTypeLabel = "Type";

export const designTypeTermAccessionNumberLabel = "Type Term Accession Number";

export const designTypeTermSourceREFLabel = "Type Term Source REF";

export const labels: FSharpList<string> = ofArray([designTypeLabel, designTypeTermAccessionNumberLabel, designTypeTermSourceREFLabel]);

export function fromSparseTable(matrix: SparseTable): FSharpList<OntologyAnnotation> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments: Comment$[] = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(OntologyAnnotation.create(void 0, void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize<OntologyAnnotation>(matrix.ColumnCount, (i: int32): OntologyAnnotation => {
            const comments_1: Comment$[] = toArray<Comment$>(map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys));
            return OntologyAnnotation.fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [designTypeLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [designTypeTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [designTypeTermAccessionNumberLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(designs: FSharpList<OntologyAnnotation>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(designs) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<OntologyAnnotation>((i: int32, d: OntologyAnnotation): void => {
        const i_1: int32 = (i + 1) | 0;
        const oa: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = OntologyAnnotation.toString(d, true);
        addToDict(matrix.Matrix, [designTypeLabel, i_1] as [string, int32], oa.TermName);
        addToDict(matrix.Matrix, [designTypeTermAccessionNumberLabel, i_1] as [string, int32], oa.TermAccessionNumber);
        addToDict(matrix.Matrix, [designTypeTermSourceREFLabel, i_1] as [string, int32], oa.TermSourceREF);
        const matchValue: Option<Comment$[]> = d.Comments;
        if (matchValue != null) {
            const array: Comment$[] = value_1(matchValue);
            array.forEach((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            });
        }
    }, designs);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix: Option<string>, lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<OntologyAnnotation>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, value_1(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<OntologyAnnotation>];
}

export function toRows(prefix: Option<string>, designs: FSharpList<OntologyAnnotation>): Iterable<Iterable<[int32, string]>> {
    const m: SparseTable = toSparseTable(designs);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, value_1(prefix));
    }
}

