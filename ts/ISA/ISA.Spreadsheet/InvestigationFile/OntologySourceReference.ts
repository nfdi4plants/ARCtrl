import { reverse, cons, iterate, iterateIndexed, map, initialize, singleton, length, empty, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologySourceReference_create_55205B02, OntologySourceReference, OntologySourceReference_make } from "../../ISA/JsonTypes/OntologySourceReference.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { SparseTable_ToRows_584133C0, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_Z15A4F148 } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { value as value_1, Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

export const nameLabel = "Term Source Name";

export const fileLabel = "Term Source File";

export const versionLabel = "Term Source Version";

export const descriptionLabel = "Term Source Description";

export const labels: FSharpList<string> = ofArray([nameLabel, fileLabel, versionLabel, descriptionLabel]);

export function fromString(description: string, file: string, name: string, version: string, comments: FSharpList<Comment$>): OntologySourceReference {
    return OntologySourceReference_make(Option_fromValueWithDefault<string>("", description), Option_fromValueWithDefault<string>("", file), Option_fromValueWithDefault<string>("", name), Option_fromValueWithDefault<string>("", version), Option_fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), comments));
}

export function fromSparseTable(matrix: SparseTable): FSharpList<OntologySourceReference> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        return singleton(OntologySourceReference_create_55205B02(void 0, void 0, void 0, void 0, SparseTable_GetEmptyComments_Z15A4F148(matrix)));
    }
    else {
        return initialize<OntologySourceReference>(matrix.ColumnCount, (i: int32): OntologySourceReference => {
            const comments_1: FSharpList<Comment$> = map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys);
            return fromString(SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [descriptionLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [fileLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [nameLabel, i] as [string, int32]), SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [versionLabel, i] as [string, int32]), comments_1);
        });
    }
}

export function toSparseTable(ontologySources: FSharpList<OntologySourceReference>): SparseTable {
    const matrix: SparseTable = SparseTable_Create_Z2192E64B(void 0, labels, void 0, length(ontologySources) + 1);
    let commentKeys: FSharpList<string> = empty<string>();
    iterateIndexed<OntologySourceReference>((i: int32, o: OntologySourceReference): void => {
        const i_1: int32 = (i + 1) | 0;
        addToDict(matrix.Matrix, [nameLabel, i_1] as [string, int32], defaultArg(o.Name, ""));
        addToDict(matrix.Matrix, [fileLabel, i_1] as [string, int32], defaultArg(o.File, ""));
        addToDict(matrix.Matrix, [versionLabel, i_1] as [string, int32], defaultArg(o.Version, ""));
        addToDict(matrix.Matrix, [descriptionLabel, i_1] as [string, int32], defaultArg(o.Description, ""));
        const matchValue: Option<FSharpList<Comment$>> = o.Comments;
        if (matchValue != null) {
            iterate<Comment$>((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            }, value_1(matchValue));
        }
    }, ontologySources);
    return new SparseTable(matrix.Matrix, matrix.Keys, reverse<string>(List_distinct<string>(commentKeys, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })), matrix.ColumnCount);
}

export function fromRows(lineNumber: int32, rows: IEnumerator<Iterable<[int32, string]>>): [Option<string>, int32, FSharpList<Remark>, FSharpList<OntologySourceReference>] {
    const tupledArg: [Option<string>, int32, FSharpList<Remark>, SparseTable] = SparseTable_FromRows_Z5579EC29(rows, labels, lineNumber);
    return [tupledArg[0], tupledArg[1], tupledArg[2], fromSparseTable(tupledArg[3])] as [Option<string>, int32, FSharpList<Remark>, FSharpList<OntologySourceReference>];
}

export function toRows(termSources: FSharpList<OntologySourceReference>): Iterable<Iterable<[int32, string]>> {
    return SparseTable_ToRows_584133C0(toSparseTable(termSources));
}

