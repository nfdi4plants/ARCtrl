import { reverse, cons, iterate, iterateIndexed, map, initialize, singleton, length, empty, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation, OntologyAnnotation_fromString_Z7D8EB286 } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Factor_create_Z3D2B374F, Factor, Factor_make } from "../../ISA/JsonTypes/Factor.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { value as value_1, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
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

export function fromString(name: string, designType: string, typeTermSourceREF: Option<string>, typeTermAccessionNumber: Option<string>, comments: FSharpList<Comment$>): Factor {
    const factorType: OntologyAnnotation = OntologyAnnotation_fromString_Z7D8EB286(designType, typeTermSourceREF, typeTermAccessionNumber);
    return Factor_make(void 0, Option_fromValueWithDefault<string>("", name), Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation_get_empty(), factorType), Option_fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), comments));
}

export function fromSparseTable(matrix: SparseTable): FSharpList<Factor> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Factor_create_Z3D2B374F(void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize<Factor>(matrix.ColumnCount, (i: int32): Factor => {
            const comments_1: FSharpList<Comment$> = map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [factorTypeLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermSourceREFLabel, i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermAccessionNumberLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(factors: FSharpList<Factor>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(factors) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<Factor>((i: int32, f: Factor): void => {
        const i_1: int32 = (i + 1) | 0;
        const ft: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = OntologyAnnotation_toString_473B9D79(defaultArg(f.FactorType, OntologyAnnotation_get_empty()), true);
        addToDict(matrix.Matrix, [nameLabel, i_1] as [string, int32], defaultArg(f.Name, ""));
        addToDict(matrix.Matrix, [factorTypeLabel, i_1] as [string, int32], ft.TermName);
        addToDict(matrix.Matrix, [typeTermAccessionNumberLabel, i_1] as [string, int32], ft.TermAccessionNumber);
        addToDict(matrix.Matrix, [typeTermSourceREFLabel, i_1] as [string, int32], ft.TermSourceREF);
        const matchValue: Option<FSharpList<Comment$>> = f.Comments;
        if (matchValue != null) {
            iterate<Comment$>((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            }, value_1(matchValue));
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
        return SparseTable_ToRows_584133C0(m);
    }
    else {
        return SparseTable_ToRows_584133C0(m, value_1(prefix));
    }
}

