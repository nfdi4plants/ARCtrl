import { reverse, cons, iterateIndexed, empty, map as map_1, initialize, singleton, length, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { defaultArg, map, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_iter, Option_fromValueWithDefault } from "../../Core/Helper/Collections.js";
import { JsonTypes_composeTechnologyPlatform, JsonTypes_decomposeTechnologyPlatform } from "../../Core/Conversion.js";
import { ArcAssay } from "../../Core/ArcTypes.js";
import { SparseTable_ToRows_759CAFC1, SparseTable_FromRows_Z5579EC29, SparseTable, SparseTable_Create_Z2192E64B, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_3ECCA699 } from "./SparseTable.js";
import { Assay_fileNameFromIdentifier, removeMissingIdentifier, Assay_identifierFromFileName, createMissingIdentifier } from "../../Core/Helper/Identifier.js";
import { Comment_toString, Comment_fromString } from "./Comment.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { List_distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";

export const labels = ofArray(["Measurement Type", "Measurement Type Term Accession Number", "Measurement Type Term Source REF", "Technology Type", "Technology Type Term Accession Number", "Technology Type Term Source REF", "Technology Platform", "File Name"]);

export function fromString(measurementType, measurementTypeTermSourceREF, measurementTypeTermAccessionNumber, technologyType, technologyTypeTermSourceREF, technologyTypeTermAccessionNumber, technologyPlatform, fileName, comments) {
    const measurementType_1 = OntologyAnnotation.create(unwrap(measurementType), unwrap(measurementTypeTermSourceREF), unwrap(measurementTypeTermAccessionNumber));
    const technologyType_1 = OntologyAnnotation.create(unwrap(technologyType), unwrap(technologyTypeTermSourceREF), unwrap(technologyTypeTermAccessionNumber));
    const measurementType_2 = Option_fromValueWithDefault(new OntologyAnnotation(), measurementType_1);
    const technologyType_2 = Option_fromValueWithDefault(new OntologyAnnotation(), technologyType_1);
    const technologyPlatform_1 = map(JsonTypes_decomposeTechnologyPlatform, technologyPlatform);
    return ArcAssay.make(fileName, measurementType_2, technologyType_2, technologyPlatform_1, [], undefined, [], comments);
}

export function fromSparseTable(matrix) {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments = SparseTable_GetEmptyComments_3ECCA699(matrix);
        return singleton(ArcAssay.create(createMissingIdentifier(), undefined, undefined, undefined, undefined, undefined, undefined, comments));
    }
    else {
        return initialize(matrix.ColumnCount, (i) => {
            let comments_1;
            const collection = map_1((k) => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i])), matrix.CommentKeys);
            comments_1 = Array.from(collection);
            return fromString(SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type Term Source REF", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type Term Accession Number", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type Term Source REF", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type Term Accession Number", i]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Platform", i]), Assay_identifierFromFileName(SparseTable__TryGetValueDefault_5BAE6133(matrix, createMissingIdentifier(), ["File Name", i])), comments_1);
        });
    }
}

export function toSparseTable(assays) {
    const matrix = SparseTable_Create_Z2192E64B(undefined, labels, undefined, length(assays) + 1);
    let commentKeys = empty();
    iterateIndexed((i, a) => {
        const processedFileName = a.Identifier.startsWith("MISSING_IDENTIFIER_") ? removeMissingIdentifier(a.Identifier) : Assay_fileNameFromIdentifier(a.Identifier);
        const i_1 = (i + 1) | 0;
        let mt_1;
        const mt = defaultArg(a.MeasurementType, new OntologyAnnotation());
        mt_1 = OntologyAnnotation.toStringObject(mt, true);
        let tt_1;
        const tt = defaultArg(a.TechnologyType, new OntologyAnnotation());
        tt_1 = OntologyAnnotation.toStringObject(tt, true);
        addToDict(matrix.Matrix, ["Measurement Type", i_1], mt_1.TermName);
        addToDict(matrix.Matrix, ["Measurement Type Term Accession Number", i_1], mt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, ["Measurement Type Term Source REF", i_1], mt_1.TermSourceREF);
        addToDict(matrix.Matrix, ["Technology Type", i_1], tt_1.TermName);
        addToDict(matrix.Matrix, ["Technology Type Term Accession Number", i_1], tt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, ["Technology Type Term Source REF", i_1], tt_1.TermSourceREF);
        addToDict(matrix.Matrix, ["Technology Platform", i_1], defaultArg(map(JsonTypes_composeTechnologyPlatform, a.TechnologyPlatform), ""));
        addToDict(matrix.Matrix, ["File Name", i_1], processedFileName);
        ResizeArray_iter((comment) => {
            const patternInput = Comment_toString(comment);
            const n = patternInput[0];
            commentKeys = cons(n, commentKeys);
            addToDict(matrix.Matrix, [n, i_1], patternInput[1]);
        }, a.Comments);
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
    return SparseTable_ToRows_759CAFC1(toSparseTable(assays), unwrap(prefix));
}

