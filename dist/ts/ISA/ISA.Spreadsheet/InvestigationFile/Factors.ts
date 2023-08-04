import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { value as value_1, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { Factor } from "../../ISA/JsonTypes/Factor.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

export const nameLabel = "Name";

export const factorTypeLabel = "Type";

export const typeTermAccessionNumberLabel = "Type Term Accession Number";

export const typeTermSourceREFLabel = "Type Term Source REF";

export const labels: FSharpList<string> = ofArray([nameLabel, factorTypeLabel, typeTermAccessionNumberLabel, typeTermSourceREFLabel]);

export function fromString(name: string, designType: string, typeTermSourceREF: Option<string>, typeTermAccessionNumber: Option<string>, comments: Comment$[]): Factor {
    const factorType: OntologyAnnotation = OntologyAnnotation.fromString(designType, typeTermSourceREF, typeTermAccessionNumber);
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", name);
    const arg_2: Option<OntologyAnnotation> = Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, factorType);
    const arg_3: Option<Comment$[]> = Option_fromValueWithDefault<Comment$[]>([], comments);
    return Factor.make(void 0, arg_1, arg_2, arg_3);
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Factor> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments: Comment$[] = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(Factor.create(void 0, void 0, void 0, comments));
    }
    else {
        return initialize<Factor>(matrix.ColumnCount, (i: int32): Factor => {
            const comments_1: Comment$[] = toArray<Comment$>(map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [factorTypeLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermAccessionNumberLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(factors: FSharpList<Factor>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(factors) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Factor>((i: int32, f: Factor): void => {
        const i_1: int32 = (i + 1) | 0;
        let ft: { TermAccessionNumber: string, TermName: string, TermSourceREF: string };
        const f_1: OntologyAnnotation = defaultArg(f.FactorType, OntologyAnnotation.empty);
        ft = OntologyAnnotation.toString(f_1, true);
        addToDict(matrix.Matrix, [nameLabel, i_1] as [string, int32], defaultArg(f.Name, ""));
        addToDict(matrix.Matrix, [factorTypeLabel, i_1] as [string, int32], ft.TermName);
        addToDict(matrix.Matrix, [typeTermAccessionNumberLabel, i_1] as [string, int32], ft.TermAccessionNumber);
        addToDict(matrix.Matrix, [typeTermSourceREFLabel, i_1] as [string, int32], ft.TermSourceREF);
        const matchValue: Option<Comment$[]> = f.Comments;
        if (matchValue != null) {
            const array: Comment$[] = value_1(matchValue);
            array.forEach((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            });
        }
    }, factors);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix: Option<string>, lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<Factor>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, value_1(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<Factor>];
}

export function toRows(prefix: Option<string>, factors: FSharpList<Factor>): Iterable<Iterable<[int32, string]>> {
    const m: SparseTable = toSparseTable(factors);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, value_1(prefix));
    }
}

