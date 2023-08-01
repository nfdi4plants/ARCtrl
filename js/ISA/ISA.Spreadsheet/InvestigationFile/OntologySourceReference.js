import { reverse, cons, iterate, iterateIndexed, map, initialize, singleton, length, empty, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologySourceReference_create_55205B02, OntologySourceReference_make } from "../../ISA/JsonTypes/OntologySourceReference.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
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
    return OntologySourceReference_make(Option_fromValueWithDefault("", description), Option_fromValueWithDefault("", file), Option_fromValueWithDefault("", name), Option_fromValueWithDefault("", version), Option_fromValueWithDefault(empty(), comments));
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(OntologySourceReference_create_55205B02(void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
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
            iterate((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            }, matchValue);
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
    return SparseTable_ToRows_584133C0(toSparseTable(termSources));
}

