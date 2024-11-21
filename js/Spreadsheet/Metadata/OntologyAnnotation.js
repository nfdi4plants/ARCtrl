import { reverse, cons, iterateIndexed, empty, ofArray, map, initialize, singleton, length } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_3ECCA699 } from "./SparseTable.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { Comment_toString, Comment_fromString } from "./Comment.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { ResizeArray_iter } from "../../Core/Helper/Collections.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";

export function fromSparseTable(label, labelTSR, labelTAN, matrix) {
    let returnVal;
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_3ECCA699(matrix);
        return singleton((returnVal = OntologyAnnotation.create(), (returnVal.Comments = comments, returnVal)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            let comments_1;
            const collection = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            comments_1 = Array.from(collection);
            return new OntologyAnnotation(unwrap(SparseTable__TryGetValue_11FD62A8(matrix, [label, i])), unwrap(SparseTable__TryGetValue_11FD62A8(matrix, [labelTSR, i])), unwrap(SparseTable__TryGetValue_11FD62A8(matrix, [labelTAN, i])), comments_1);
        });
    }
}

export function toSparseTable(label, labelTSR, labelTAN, designs) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, ofArray([label, labelTAN, labelTSR]), undefined, length(designs) + 1);
    let commentKeys = empty();
    iterateIndexed((i, d) => {
        const i_1 = (i + 1) | 0;
        const oa = OntologyAnnotation.toStringObject(d, true);
        addToDict(matrix.Matrix, [label, i_1], oa.TermName);
        addToDict(matrix.Matrix, [labelTAN, i_1], oa.TermAccessionNumber);
        addToDict(matrix.Matrix, [labelTSR, i_1], oa.TermSourceREF);
        ResizeArray_iter((comment) => {
            const patternInput = Comment_toString(comment);
            const n = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
        }, d.Comments);
    }, designs);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, label, labelTSR, labelTAN, lineNumber, rows) {
    const labels = ofArray([label, labelTAN, labelTSR]);
    const tupledArg = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, prefix);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(label, labelTSR, labelTAN, tupledArg[3])];
}

export function toRows(prefix, label, labelTSR, labelTAN, designs) {
    const m = toSparseTable(label, labelTSR, labelTAN, designs);
    if (prefix == null) {
        return SparseTable_ToRows_759CAFC1(m);
    }
    else {
        return SparseTable_ToRows_759CAFC1(m, prefix);
    }
}

