import { reverse, cons, iterateIndexed, empty, map, initialize, singleton, length, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { OntologySourceReference } from "../../Core/OntologySourceReference.js";
import { SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_3ECCA699 } from "./SparseTable.js";
import { Comment_toString, Comment_fromString } from "./Comment.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_iter } from "../../Core/Helper/Collections.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";

export const nameLabel = "Term Source Name";

export const fileLabel = "Term Source File";

export const versionLabel = "Term Source Version";

export const descriptionLabel = "Term Source Description";

export const labels = ofArray([nameLabel, fileLabel, versionLabel, descriptionLabel]);

export function fromString(description, file, name, version, comments) {
    return OntologySourceReference.make(description, file, name, version, comments);
}

export function fromSparseTable(matrix) {
    let returnVal;
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_3ECCA699(matrix);
        return singleton((returnVal = OntologySourceReference.create(), (returnVal.Comments = comments, returnVal)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            let comments_1;
            const collection = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            comments_1 = Array.from(collection);
            return fromString(SparseTable__TryGetValue_11FD62A8(matrix, [descriptionLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [fileLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [nameLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [versionLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(ontologySources) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, labels, undefined, length(ontologySources) + 1);
    let commentKeys = empty();
    iterateIndexed((i, o) => {
        const i_1 = (i + 1) | 0;
        addToDict(matrix.Matrix, [nameLabel, i_1], defaultArg(o.Name, ""));
        addToDict(matrix.Matrix, [fileLabel, i_1], defaultArg(o.File, ""));
        addToDict(matrix.Matrix, [versionLabel, i_1], defaultArg(o.Version, ""));
        addToDict(matrix.Matrix, [descriptionLabel, i_1], defaultArg(o.Description, ""));
        ResizeArray_iter((comment) => {
            const patternInput = Comment_toString(comment);
            const n = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
        }, o.Comments);
    }, ontologySources);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(lineNumber, rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(termSources) {
    return SparseTable_ToRows_759CAFC1(toSparseTable(termSources));
}

