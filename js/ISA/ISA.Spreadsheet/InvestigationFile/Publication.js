import { reverse, cons, iterate, iterateIndexed, map as map_1, initialize, singleton, length, empty, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation_toString_473B9D79, OntologyAnnotation_get_empty, OntologyAnnotation_fromString_Z7D8EB286 } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Publication_create_Z3E55064F, Publication_make } from "../../ISA/JsonTypes/Publication.js";
import { defaultArg, map } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { URIModule_fromString } from "../../ISA/JsonTypes/URI.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
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
    const status_1 = OntologyAnnotation_fromString_Z7D8EB286(status, statusTermSourceREF, statusTermAccessionNumber);
    return Publication_make(map(URIModule_fromString, Option_fromValueWithDefault("", pubMedID)), Option_fromValueWithDefault("", doi), Option_fromValueWithDefault("", author), Option_fromValueWithDefault("", title), Option_fromValueWithDefault(OntologyAnnotation_get_empty(), status_1), Option_fromValueWithDefault(empty(), comments));
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(Publication_create_Z3E55064F(void 0, void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = map_1((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [pubMedIDLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [doiLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [authorListLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [titleLabel, i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [statusLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermSourceREFLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermAccessionNumberLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(publications) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(publications) + 1);
    let commentKeys = empty();
    iterateIndexed((i, p) => {
        const i_1 = (i + 1) | 0;
        const s_1 = OntologyAnnotation_toString_473B9D79(defaultArg(p.Status, OntologyAnnotation_get_empty()), true);
        addToDict(matrix.Matrix, [pubMedIDLabel, i_1], defaultArg(p.PubMedID, ""));
        addToDict(matrix.Matrix, [doiLabel, i_1], defaultArg(p.DOI, ""));
        addToDict(matrix.Matrix, [authorListLabel, i_1], defaultArg(p.Authors, ""));
        addToDict(matrix.Matrix, [titleLabel, i_1], defaultArg(p.Title, ""));
        addToDict(matrix.Matrix, [statusLabel, i_1], s_1.TermName);
        addToDict(matrix.Matrix, [statusTermAccessionNumberLabel, i_1], s_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [statusTermSourceREFLabel, i_1], s_1.TermSourceREF);
        const matchValue = p.Comments;
        if (matchValue != null) {
            iterate((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            }, matchValue);
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
        return SparseTable_ToRows_584133C0(m);
    }
    else {
        return SparseTable_ToRows_584133C0(m, prefix);
    }
}

