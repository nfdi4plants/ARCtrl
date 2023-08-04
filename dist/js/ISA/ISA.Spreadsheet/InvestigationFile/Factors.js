import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Factor } from "../../ISA/JsonTypes/Factor.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const nameLabel = "Name";

export const factorTypeLabel = "Type";

export const typeTermAccessionNumberLabel = "Type Term Accession Number";

export const typeTermSourceREFLabel = "Type Term Source REF";

export const labels = ofArray([nameLabel, factorTypeLabel, typeTermAccessionNumberLabel, typeTermSourceREFLabel]);

export function fromString(name, designType, typeTermSourceREF, typeTermAccessionNumber, comments) {
    const factorType = OntologyAnnotation.fromString(designType, typeTermSourceREF, typeTermAccessionNumber);
    const arg_1 = Option_fromValueWithDefault("", name);
    const arg_2 = Option_fromValueWithDefault(OntologyAnnotation.empty, factorType);
    const arg_3 = Option_fromValueWithDefault([], comments);
    return Factor.make(void 0, arg_1, arg_2, arg_3);
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(Factor.create(void 0, void 0, void 0, comments));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = toArray(map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [factorTypeLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermSourceREFLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermAccessionNumberLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(factors) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(factors) + 1);
    let commentKeys = empty();
    iterateIndexed((i, f) => {
        const i_1 = (i + 1) | 0;
        let ft;
        const f_1 = defaultArg(f.FactorType, OntologyAnnotation.empty);
        ft = OntologyAnnotation.toString(f_1, true);
        addToDict(matrix.Matrix, [nameLabel, i_1], defaultArg(f.Name, ""));
        addToDict(matrix.Matrix, [factorTypeLabel, i_1], ft.TermName);
        addToDict(matrix.Matrix, [typeTermAccessionNumberLabel, i_1], ft.TermAccessionNumber);
        addToDict(matrix.Matrix, [typeTermSourceREFLabel, i_1], ft.TermSourceREF);
        const matchValue = f.Comments;
        if (matchValue != null) {
            const array = matchValue;
            array.forEach((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            });
        }
    }, factors);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, lineNumber, rows) {
    const tupledArg = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, prefix);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(prefix, factors) {
    const m = toSparseTable(factors);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, prefix);
    }
}

