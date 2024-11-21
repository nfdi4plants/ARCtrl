import { reverse, cons, iterateIndexed, empty, map as map_1, initialize, singleton, length, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { defaultArg, map, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { URIModule_fromString } from "../../Core/URI.js";
import { ResizeArray_iter, Option_fromValueWithDefault } from "../../Core/Helper/Collections.js";
import { Publication } from "../../Core/Publication.js";
import { SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_3ECCA699 } from "./SparseTable.js";
import { Comment_toString, Comment_fromString } from "./Comment.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";

export const pubMedIDLabel = "PubMed ID";

export const doiLabel = "DOI";

export const authorListLabel = "Author List";

export const titleLabel = "Title";

export const statusLabel = "Status";

export const statusTermAccessionNumberLabel = "Status Term Accession Number";

export const statusTermSourceREFLabel = "Status Term Source REF";

export const labels = ofArray([pubMedIDLabel, doiLabel, authorListLabel, titleLabel, statusLabel, statusTermAccessionNumberLabel, statusTermSourceREFLabel]);

export function fromString(pubMedID, doi, author, title, status, statusTermSourceREF, statusTermAccessionNumber, comments) {
    const status_1 = new OntologyAnnotation(unwrap(status), unwrap(statusTermSourceREF), unwrap(statusTermAccessionNumber));
    const pubMedID_1 = map(URIModule_fromString, pubMedID);
    const status_2 = Option_fromValueWithDefault(new OntologyAnnotation(), status_1);
    return Publication.make(pubMedID_1, doi, author, title, status_2, comments);
}

export function fromSparseTable(matrix) {
    let returnVal;
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_3ECCA699(matrix);
        return singleton((returnVal = Publication.create(), (returnVal.Comments = comments, returnVal)));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            let comments_1;
            const collection = map_1((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            comments_1 = Array.from(collection);
            return fromString(SparseTable__TryGetValue_11FD62A8(matrix, [pubMedIDLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [doiLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [authorListLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [titleLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermSourceREFLabel, i]), SparseTable__TryGetValue_11FD62A8(matrix, [statusTermAccessionNumberLabel, i]), comments_1);
        });
    }
}

export function toSparseTable(publications) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, labels, undefined, length(publications) + 1);
    let commentKeys = empty();
    iterateIndexed((i, p) => {
        const i_1 = (i + 1) | 0;
        let s_1;
        const s = defaultArg(p.Status, new OntologyAnnotation());
        s_1 = OntologyAnnotation.toStringObject(s, true);
        addToDict(matrix.Matrix, [pubMedIDLabel, i_1], defaultArg(p.PubMedID, ""));
        addToDict(matrix.Matrix, [doiLabel, i_1], defaultArg(p.DOI, ""));
        addToDict(matrix.Matrix, [authorListLabel, i_1], defaultArg(p.Authors, ""));
        addToDict(matrix.Matrix, [titleLabel, i_1], defaultArg(p.Title, ""));
        addToDict(matrix.Matrix, [statusLabel, i_1], s_1.TermName);
        addToDict(matrix.Matrix, [statusTermAccessionNumberLabel, i_1], s_1.TermAccessionNumber);
        addToDict(matrix.Matrix, [statusTermSourceREFLabel, i_1], s_1.TermSourceREF);
        ResizeArray_iter((comment) => {
            const patternInput = Comment_toString(comment);
            const n = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
        }, p.Comments);
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
        return SparseTable_ToRows_759CAFC1(m);
    }
    else {
        return SparseTable_ToRows_759CAFC1(m, prefix);
    }
}

