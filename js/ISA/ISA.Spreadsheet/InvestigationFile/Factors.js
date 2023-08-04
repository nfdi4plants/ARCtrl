import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation_toString_5E3DAF0D, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_2EB0E147 } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Factor_create_3A99E5B8, Factor_make } from "../../ISA/JsonTypes/Factor.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
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
    const factorType = OntologyAnnotation_fromString_2EB0E147(designType, typeTermSourceREF, typeTermAccessionNumber);
    return Factor_make(void 0, Option_fromValueWithDefault("", name), Option_fromValueWithDefault(OntologyAnnotation_get_empty(), factorType), Option_fromValueWithDefault([], comments));
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Factor_create_3A99E5B8(void 0, void 0, void 0, SparseTable_GetEmptyComments_651559CC(matrix)));
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
        const ft = OntologyAnnotation_toString_5E3DAF0D(defaultArg(f.FactorType, OntologyAnnotation_get_empty()), true);
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

