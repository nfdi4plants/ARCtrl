import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation_toString_5E3DAF0D, OntologyAnnotation_fromString_2EB0E147, OntologyAnnotation_create_Z61CCDEEC } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const designTypeLabel = "Type";

export const designTypeTermAccessionNumberLabel = "Type Term Accession Number";

export const designTypeTermSourceREFLabel = "Type Term Source REF";

export const labels = ofArray([designTypeLabel, designTypeTermAccessionNumberLabel, designTypeTermSourceREFLabel]);

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(OntologyAnnotation_create_Z61CCDEEC(void 0, void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_651559CC(matrix)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = toArray(map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys));
            return OntologyAnnotation_fromString_2EB0E147(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [designTypeLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [designTypeTermSourceREFLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [designTypeTermAccessionNumberLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(designs) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(designs) + 1);
    let commentKeys = empty();
    iterateIndexed((i, d) => {
        const i_1 = (i + 1) | 0;
        const oa = OntologyAnnotation_toString_5E3DAF0D(d, true);
        addToDict(matrix.Matrix, [designTypeLabel, i_1], oa.TermName);
        addToDict(matrix.Matrix, [designTypeTermAccessionNumberLabel, i_1], oa.TermAccessionNumber);
        addToDict(matrix.Matrix, [designTypeTermSourceREFLabel, i_1], oa.TermSourceREF);
        const matchValue = d.Comments;
        if (matchValue != null) {
            const array = matchValue;
            array.forEach((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            });
        }
    }, designs);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, lineNumber, rows) {
    const tupledArg = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, prefix);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(prefix, designs) {
    const m = toSparseTable(designs);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, prefix);
    }
}

