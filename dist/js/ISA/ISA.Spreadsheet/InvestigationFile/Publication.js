import { reverse, cons, iterateIndexed, empty, map as map_1, toArray, initialize, singleton, length, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { defaultArg, map } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { URIModule_fromString } from "../../ISA/JsonTypes/URI.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Publication } from "../../ISA/JsonTypes/Publication.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const pubMedIDLabel = "PubMed ID";

export const doiLabel = "DOI";

export const authorListLabel = "Author List";

export const titleLabel = "Title";

export const statusLabel = "Status";

export const statusTermAccessionNumberLabel = "Status Term Accession Number";

export const statusTermSourceREFLabel = "Status Term Source REF";

export const labels = ofArray([pubMedIDLabel, doiLabel, authorListLabel, titleLabel, statusLabel, statusTermAccessionNumberLabel, statusTermSourceREFLabel]);

export function fromString(pubMedID, doi, author, title, status, statusTermSourceREF, statusTermAccessionNumber, comments) {
    const status_1 = OntologyAnnotation.fromString(status, statusTermSourceREF, statusTermAccessionNumber);
    const arg = map(URIModule_fromString, Option_fromValueWithDefault("", pubMedID));
    const arg_1 = Option_fromValueWithDefault("", doi);
    const arg_2 = Option_fromValueWithDefault("", author);
    const arg_3 = Option_fromValueWithDefault("", title);
    const arg_4 = Option_fromValueWithDefault(OntologyAnnotation.empty, status_1);
    const arg_5 = Option_fromValueWithDefault([], comments);
    return Publication.make(arg, arg_1, arg_2, arg_3, arg_4, arg_5);
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(Publication.create(void 0, void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = toArray(map_1((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [pubMedIDLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [doiLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [authorListLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [titleLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [statusLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermSourceREFLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermAccessionNumberLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(publications) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(publications) + 1);
    let commentKeys = empty();
    iterateIndexed((i, p) => {
        const i_1 = (i + 1) | 0;
        let s_1;
        const s = defaultArg(p.Status, OntologyAnnotation.empty);
        s_1 = OntologyAnnotation.toString(s, true);
        addToDict(matrix.Matrix, [pubMedIDLabel, i_1], defaultArg(p.PubMedID, ""));
        addToDict(matrix.Matrix, [doiLabel, i_1], defaultArg(p.DOI, ""));
        addToDict(matrix.Matrix, [authorListLabel, i_1], defaultArg(p.Authors, ""));
        addToDict(matrix.Matrix, [titleLabel, i_1], defaultArg(p.Title, ""));
        addToDict(matrix.Matrix, [statusLabel, i_1], s_1.TermName);
        addToDict(matrix.Matrix, [statusTermAccessionNumberLabel, i_1], s_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [statusTermSourceREFLabel, i_1], s_1.TermSourceREF);
        const matchValue = p.Comments;
        if (matchValue != null) {
            const array = matchValue;
            array.forEach((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            });
        }
    }, publications);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, lineNumber, rows) {
    const tupledArg = (prefix == null) ? SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber) : SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, prefix);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(prefix, publications) {
    const m = toSparseTable(publications);
    if (prefix == null) {
        return SparseTable_ToRows_6A3D4534(m);
    }
    else {
        return SparseTable_ToRows_6A3D4534(m, prefix);
    }
}

