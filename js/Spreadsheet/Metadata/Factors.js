import { reverse, cons, iterateIndexed, empty, map, initialize, singleton, length, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { defaultArg, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_iter, Option_fromValueWithDefault } from "../../Core/Helper/Collections.js";
import { Factor } from "../../Core/Process/Factor.js";
import { SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_3ECCA699 } from "./SparseTable.js";
import { Comment_toString, Comment_fromString } from "./Comment.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";

export const nameLabel = "Name";

export const factorTypeLabel = "Type";

export const typeTermAccessionNumberLabel = "Type Term Accession Number";

export const typeTermSourceREFLabel = "Type Term Source REF";

export const labels = ofArray([nameLabel, factorTypeLabel, typeTermAccessionNumberLabel, typeTermSourceREFLabel]);

export function fromString(name, designType, typeTermSourceREF, typeTermAccessionNumber, comments) {
    const factorType = OntologyAnnotation.create(unwrap(designType), unwrap(typeTermSourceREF), unwrap(typeTermAccessionNumber));
    const factorType_1 = Option_fromValueWithDefault(new OntologyAnnotation(), factorType);
    const comments_1 = Option_fromValueWithDefault([], comments);
    return Factor.make(name, factorType_1, comments_1);
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_3ECCA699(matrix);
        return singleton(Factor.create(undefined, undefined, comments));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            let comments_1;
            const collection = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            comments_1 = Array.from(collection);
            return fromString(SparseTable__TryGetValue_11FD62A8(matrix, [nameLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [factorTypeLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermSourceREFLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [typeTermAccessionNumberLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(factors) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, labels, undefined, length(factors) + 1);
    let commentKeys = empty();
    iterateIndexed((i, f) => {
        const i_1 = (i + 1) | 0;
        let ft;
        const f_1 = defaultArg(f.FactorType, new OntologyAnnotation());
        ft = OntologyAnnotation.toStringObject(f_1, true);
        addToDict(matrix.Matrix, [nameLabel, i_1], defaultArg(f.Name, ""));
        addToDict(matrix.Matrix, [factorTypeLabel, i_1], ft.TermName);
        addToDict(matrix.Matrix, [typeTermAccessionNumberLabel, i_1], ft.TermAccessionNumber);
        addToDict(matrix.Matrix, [typeTermSourceREFLabel, i_1], ft.TermSourceREF);
        const matchValue = f.Comments;
        if (matchValue != null) {
            ResizeArray_iter((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            }, matchValue);
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
        return SparseTable_ToRows_759CAFC1(m);
    }
    else {
        return SparseTable_ToRows_759CAFC1(m, prefix);
    }
}

