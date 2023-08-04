import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { OntologySourceReference } from "../../ISA/JsonTypes/OntologySourceReference.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const nameLabel = "Term Source Name";

export const fileLabel = "Term Source File";

export const versionLabel = "Term Source Version";

export const descriptionLabel = "Term Source Description";

export const labels = ofArray([nameLabel, fileLabel, versionLabel, descriptionLabel]);

export function fromString(description, file, name, version, comments) {
    const arg = Option_fromValueWithDefault("", description);
    const arg_1 = Option_fromValueWithDefault("", file);
    const arg_2 = Option_fromValueWithDefault("", name);
    const arg_3 = Option_fromValueWithDefault("", version);
    const arg_4 = Option_fromValueWithDefault([], comments);
    return OntologySourceReference.make(arg, arg_1, arg_2, arg_3, arg_4);
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(OntologySourceReference.create(void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = toArray(map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [descriptionLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [fileLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [versionLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(ontologySources) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(ontologySources) + 1);
    let commentKeys = empty();
    iterateIndexed((i, o) => {
        const i_1 = (i + 1) | 0;
        addToDict(matrix.Matrix, [nameLabel, i_1], defaultArg(o.Name, ""));
        addToDict(matrix.Matrix, [fileLabel, i_1], defaultArg(o.File, ""));
        addToDict(matrix.Matrix, [versionLabel, i_1], defaultArg(o.Version, ""));
        addToDict(matrix.Matrix, [descriptionLabel, i_1], defaultArg(o.Description, ""));
        const matchValue = o.Comments;
        if (matchValue != null) {
            const array = matchValue;
            array.forEach((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            });
        }
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
    return SparseTable_ToRows_6A3D4534(toSparseTable(termSources));
}

