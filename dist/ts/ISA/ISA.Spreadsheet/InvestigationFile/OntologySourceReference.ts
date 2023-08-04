import { reverse, cons, iterateIndexed, empty, map, toArray, initialize, singleton, length, FSharpList, ofArray } from "../../../fable_modules/fable-library-ts/List.js";
import { Option_fromValueWithDefault } from "../Conversions.js";
import { value as value_1, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Remark, Comment$ } from "../../ISA/JsonTypes/Comment.js";
import { OntologySourceReference } from "../../ISA/JsonTypes/OntologySourceReference.js";
import { SparseTable_ToRows_6A3D4534, SparseTable_FromRows_Z5579EC29, SparseTable_Create_Z2192E64B, SparseTable, SparseTable__TryGetValueDefault_5BAE6133, SparseTable_GetEmptyComments_651559CC } from "../SparseTable.js";
import { Comment_toString, Comment_fromString } from "../Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { addToDict } from "../../../fable_modules/fable-library-ts/MapUtil.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { IEnumerator, stringHash } from "../../../fable_modules/fable-library-ts/Util.js";

export const nameLabel = "Term Source Name";

export const fileLabel = "Term Source File";

export const versionLabel = "Term Source Version";

export const descriptionLabel = "Term Source Description";

export const labels: FSharpList<string> = ofArray([nameLabel, fileLabel, versionLabel, descriptionLabel]);

export function fromString(description: string, file: string, name: string, version: string, comments: Comment$[]): OntologySourceReference {
    const arg: Option<string> = Option_fromValueWithDefault<string>("", description);
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", file);
    const arg_2: Option<string> = Option_fromValueWithDefault<string>("", name);
    const arg_3: Option<string> = Option_fromValueWithDefault<string>("", version);
    const arg_4: Option<Comment$[]> = Option_fromValueWithDefault<Comment$[]>([], comments);
    return OntologySourceReference.make(arg, arg_1, arg_2, arg_3, arg_4);
}

export function fromSparseTable(matrix: SparseTable): FSharpList<OntologySourceReference> {
    if ((matrix.ColumnCount === 0) && (length(matrix.CommentKeys) !== 0)) {
        const comments: Comment$[] = SparseTable_GetEmptyComments_651559CC(matrix);
        return singleton(OntologySourceReference.create(void 0, void 0, void 0, void 0, comments));
    }
    else {
        return initialize<OntologySourceReference>(matrix.ColumnCount, (i: int32): OntologySourceReference => {
            const comments_1: Comment$[] = toArray<Comment$>(map<string, Comment$>((k: string): Comment$ => Comment_fromString(k, SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [k, i] as [string, int32])), matrix.CommentKeys));
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
        const matchValue: Option<Comment$[]> = o.Comments;
        if (matchValue != null) {
            const array: Comment$[] = value_1(matchValue);
            array.forEach((comment: Comment$): void => {
                const patternInput: [string, string] = Comment_toString(comment);
                const n: string = patternInput[0];
                commentKeys = cons(n, commentKeys);
                addToDict(matrix.Matrix, [n, i_1] as [string, int32], patternInput[1]);
            });
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
    return SparseTable_ToRows_6A3D4534(toSparseTable(termSources));
}

