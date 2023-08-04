import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, ofArray } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { ArcAssay } from "../../ISA/ArcTypes/ArcAssay.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Assay_fileNameFromIdentifier, removeMissingIdentifier, Assay_identifierFromFileName, createMissingIdentifier } from "../../ISA/ArcTypes/Identifier.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { unwrap, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { addToDict } from "../../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { stringHash } from "../../../fable_modules/fable-library.4.1.4/Util.js";

export const labels = ofArray(["Measurement Type", "Measurement Type Term Accession Number", "Measurement Type Term Source REF", "Technology Type", "Technology Type Term Accession Number", "Technology Type Term Source REF", "Technology Platform", "File Name"]);

export function fromString(measurementType, measurementTypeTermSourceREF, measurementTypeTermAccessionNumber, technologyType, technologyTypeTermSourceREF, technologyTypeTermAccessionNumber, technologyPlatform, fileName, comments) {
    const measurementType_1 = OntologyAnnotation.fromString(measurementType, measurementTypeTermSourceREF, measurementTypeTermAccessionNumber);
    const technologyType_1 = OntologyAnnotation.fromString(technologyType, technologyTypeTermSourceREF, technologyTypeTermAccessionNumber);
    const arg_1 = Option_fromValueWithDefault(OntologyAnnotation.empty, measurementType_1);
    const arg_2 = Option_fromValueWithDefault(OntologyAnnotation.empty, technologyType_1);
    const arg_3 = Option_fromValueWithDefault("", technologyPlatform);
    return ArcAssay.make(fileName, arg_1, arg_2, arg_3, [], [], comments);
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(ArcAssay.create(createMissingIdentifier(), void 0, void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            const comments_1 = toArray(map((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Measurement Type", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type Term Source REF", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type Term Accession Number", i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Technology Type", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type Term Source REF", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type Term Accession Number", i]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Technology Platform", i]), Assay_identifierFromFileName(SparseTable__TryGetValueDefault_5BAE6133(matrix, createMissingIdentifier(), ["File Name", i])), comments_1);
        });
    }
}

export function toSparseTable(assays) {
    const matrix = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(assays) + 1);
    let commentKeys = empty();
    iterateIndexed((i, a) => {
        const processedFileName = (a.Identifier.indexOf("MISSING_IDENTIFIER_") === 0) ? removeMissingIdentifier(a.Identifier) : Assay_fileNameFromIdentifier(a.Identifier);
        const i_1 = (i + 1) | 0;
        let mt_1;
        const mt = defaultArg(a.MeasurementType, OntologyAnnotation.empty);
        mt_1 = OntologyAnnotation.toString(mt, true);
        let tt_1;
        const tt = defaultArg(a.TechnologyType, OntologyAnnotation.empty);
        tt_1 = OntologyAnnotation.toString(tt, true);
        addToDict(matrix.Matrix, ["Measurement Type", i_1], mt_1.TermName);
        addToDict(matrix.Matrix, ["Measurement Type Term Accession Number", i_1], mt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, ["Measurement Type Term Source REF", i_1], mt_1.TermSourceREF);
        addToDict(matrix.Matrix, ["Technology Type", i_1], tt_1.TermName);
        addToDict(matrix.Matrix, ["Technology Type Term Accession Number", i_1], tt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, ["Technology Type Term Source REF", i_1], tt_1.TermSourceREF);
        addToDict(matrix.Matrix, ["Technology Platform", i_1], defaultArg(a.TechnologyPlatform, ""));
        addToDict(matrix.Matrix, ["File Name", i_1], processedFileName);
        if (!(a.Comments.length === 0)) {
            const array = a.Comments;
            array.forEach((comment) => {
                const patternInput = Comment_toString(comment);
                const n = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
            });
        }
    }, assays);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse(List_distinct(commentKeys, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix, lineNumber, rows) {
    const tupledArg = SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, unwrap(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])];
}

export function toRows(prefix, assays) {
    return SparseTable_ToRows_6A3D4534(toSparseTable(assays), unwrap(prefix));
}

