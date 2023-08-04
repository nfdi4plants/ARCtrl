import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation } from "../../ISA/JsonTypes/OntologyAnnotation.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { unwrap, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { ArcAssay } from "../../ISA/ArcTypes/ArcAssay.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValue_11FD62A8, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Assay_fileNameFromIdentifier, removeMissingIdentifier, Assay_identifierFromFileName, createMissingIdentifier } from "../../ISA/ArcTypes/Identifier.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

export const labels: FSharpList<string> = ofArray(["Measurement Type", "Measurement Type Term Accession Number", "Measurement Type Term Source REF", "Technology Type", "Technology Type Term Accession Number", "Technology Type Term Source REF", "Technology Platform", "File Name"]);

export function fromString(measurementType: string, measurementTypeTermSourceREF: Option<string>, measurementTypeTermAccessionNumber: Option<string>, technologyType: string, technologyTypeTermSourceREF: Option<string>, technologyTypeTermAccessionNumber: Option<string>, technologyPlatform: string, fileName: string, comments: Comment$[]): ArcAssay {
    const measurementType_1: OntologyAnnotation = OntologyAnnotation.fromString(measurementType, measurementTypeTermSourceREF, measurementTypeTermAccessionNumber);
    const technologyType_1: OntologyAnnotation = OntologyAnnotation.fromString(technologyType, technologyTypeTermSourceREF, technologyTypeTermAccessionNumber);
    const arg_1: Option<OntologyAnnotation> = Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, measurementType_1);
    const arg_2: Option<OntologyAnnotation> = Option_fromValueWithDefault<OntologyAnnotation>(OntologyAnnotation.empty, technologyType_1);
    const arg_3: Option<string> = Option_fromValueWithDefault<string>("", technologyPlatform);
    return ArcAssay.make(fileName, arg_1, arg_2, arg_3, [], [], comments);
}

export function fromSparseTable(matrix: SparseTable): FSharpList<ArcAssay> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments: Comment$[] = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(ArcAssay.create(createMissingIdentifier(), void 0, void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize<ArcAssay>(matrix.ColumnCount, (i: int32): ArcAssay => {
            const comments_1: Comment$[] = toArray<Comment$>(map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys));
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Measurement Type", i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type Term Source REF", i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, ["Measurement Type Term Accession Number", i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Technology Type", i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type Term Source REF", i] as [string, int32]), SparseTable__TryGetValue_11FD62A8(matrix, ["Technology Type Term Accession Number", i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", ["Technology Platform", i] as [string, int32]), Assay_identifierFromFileName(SparseTable__TryGetValueDefault_5BAE6133(matrix, createMissingIdentifier(), ["File Name", i] as [string, int32])), comments_1);
        });
    }
}

export function toSparseTable(assays: FSharpList<ArcAssay>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(assays) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<ArcAssay>((i: int32, a: ArcAssay): void => {
        const processedFileName: string = (a.Identifier.indexOf("MISSING_IDENTIFIER_") === 0) ? removeMissingIdentifier(a.Identifier) : Assay_fileNameFromIdentifier(a.Identifier);
        const i_1: int32 = (i + 1) | 0;
        let mt_1: { TermAccessionNumber: string, TermName: string, TermSourceREF: string };
        const mt: OntologyAnnotation = defaultArg(a.MeasurementType, OntologyAnnotation.empty);
        mt_1 = OntologyAnnotation.toString(mt, true);
        let tt_1: { TermAccessionNumber: string, TermName: string, TermSourceREF: string };
        const tt: OntologyAnnotation = defaultArg(a.TechnologyType, OntologyAnnotation.empty);
        tt_1 = OntologyAnnotation.toString(tt, true);
        addToDict(matrix.Matrix, ["Measurement Type", i_1] as [string, int32], mt_1.TermName);
        addToDict(matrix.Matrix, ["Measurement Type Term Accession Number", i_1] as [string, int32], mt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, ["Measurement Type Term Source REF", i_1] as [string, int32], mt_1.TermSourceREF);
        addToDict(matrix.Matrix, ["Technology Type", i_1] as [string, int32], tt_1.TermName);
        addToDict(matrix.Matrix, ["Technology Type Term Accession Number", i_1] as [string, int32], tt_1.TermAccessionNumber);
        addToDict(matrix.Matrix, ["Technology Type Term Source REF", i_1] as [string, int32], tt_1.TermSourceREF);
        addToDict(matrix.Matrix, ["Technology Platform", i_1] as [string, int32], defaultArg(a.TechnologyPlatform, ""));
        addToDict(matrix.Matrix, ["File Name", i_1] as [string, int32], processedFileName);
        if (!(a.Comments.length === 0)) {
            const array: Comment$[] = a.Comments;
            array.forEach((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            });
        }
    }, assays);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(prefix: Option<string>, lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<ArcAssay>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber, unwrap(prefix));
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<ArcAssay>];
}

export function toRows(prefix: Option<string>, assays: FSharpList<ArcAssay>): Iterable<Iterable<[int32, string]>> {
    return SparseTable_ToRows_6A3D4534(toSparseTable(assays), unwrap(prefix));
}

