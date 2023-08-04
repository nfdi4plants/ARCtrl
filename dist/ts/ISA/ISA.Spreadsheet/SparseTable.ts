import { append as append_1, delay, tryItem, length as length_1, isEmpty, iterate, fold, tryPick, maxBy, initialize, choose, map, indexed } from "../../fable_modules/fable-library-ts/Seq.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { defaultArg, value, Option, map as map_1 } from "../../fable_modules/fable-library-ts/Option.js";
import { tryFind, FSharpMap, ofSeq } from "../../fable_modules/fable-library-ts/Map.js";
import { IEnumerator, arrayHash, equalArrays, IEquatable, IMap, equals, comparePrimitives } from "../../fable_modules/fable-library-ts/Util.js";
import { DataType_String, DataType_$union, FsCell } from "../../fable_modules/FsSpreadsheet.3.3.0/Cells/FsCell.fs.js";
import { FsRow } from "../../fable_modules/FsSpreadsheet.3.3.0/FsRow.fs.js";
import { RowBuilder_get_Empty, RowBuilder__Combine_19F30600, RowBuilder_$ctor } from "../../fable_modules/FsSpreadsheet.3.3.0/DSL/RowBuilder.fs.js";
import { RowBuilder } from "../../fable_modules/FsSpreadsheet.3.3.0/DSL/RowBuilder.fs.js";
import { RowElement_IndexedCell, ColumnIndex, RowElement_UnindexedCell, Message_$union, RowElement_$union, SheetEntity$1_$union } from "../../fable_modules/FsSpreadsheet.3.3.0/DSL/Types.fs.js";
import { map as map_2, toArray, initialize as initialize_1, exists, find, cons, append, empty, singleton, FSharpList } from "../../fable_modules/fable-library-ts/List.js";
import { CellBuilder_$ctor, CellBuilder__AsCellElement_825BA8D } from "../../fable_modules/FsSpreadsheet.3.3.0/DSL/CellBuilder.fs.js";
import { Messages_format, SheetEntity$1_some_2B595 } from "../../fable_modules/FsSpreadsheet.3.3.0/DSL/Types.fs.js";
import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.3.0/FsWorksheet.fs.js";
import { Record } from "../../fable_modules/fable-library-ts/Types.js";
import { record_type, list_type, class_type, tuple_type, int32_type, string_type, TypeInfo } from "../../fable_modules/fable-library-ts/Reflection.js";
import { Seq_trySkip, Dictionary_tryGetValue } from "./CollectionAux.js";
import { addToDict, getItemFromDict } from "../../fable_modules/fable-library-ts/MapUtil.js";
import { Dictionary } from "../../fable_modules/fable-library-ts/MutableMap.js";
import { printf, toFail } from "../../fable_modules/fable-library-ts/String.js";
import { Comment$, Remark } from "../ISA/JsonTypes/Comment.js";
import { Comment_wrapCommentKey, Comment_$007CComment$007C_$007C, Remark_$007CRemark$007C_$007C } from "./Comment.js";

export function SparseRowModule_fromValues(v: Iterable<string>): Iterable<[int32, string]> {
    return indexed<string>(v);
}

export function SparseRowModule_getValues(i: Iterable<[int32, string]>): Iterable<string> {
    return map<[int32, string], string>((tuple: [int32, string]): string => tuple[1], i);
}

export function SparseRowModule_fromAllValues(v: Iterable<Option<string>>): Iterable<[int32, string]> {
    return choose<[int32, Option<string>], [int32, string]>((tupledArg: [int32, Option<string>]): Option<[int32, string]> => map_1<string, [int32, string]>((v_1: string): [int32, string] => ([tupledArg[0], v_1] as [int32, string]), tupledArg[1]), indexed<Option<string>>(v));
}

export function SparseRowModule_getAllValues(i: Iterable<[int32, string]>): Iterable<Option<string>> {
    const m: FSharpMap<int32, string> = ofSeq<int32, string>(i, {
        Compare: comparePrimitives,
    });
    return initialize<Option<string>>(maxBy<[int32, string], int32>((tuple: [int32, string]): int32 => tuple[0], i, {
        Compare: comparePrimitives,
    })[0] + 1, (i_1: int32): Option<string> => tryFind<int32, string>(i_1, m));
}

export function SparseRowModule_fromFsRow(r: FsRow): Iterable<[int32, string]> {
    return map<FsCell, [int32, string]>((c: FsCell): [int32, string] => ([c.ColumnNumber - 1, c.Value] as [int32, string]), r.Cells);
}

export function SparseRowModule_tryGetValueAt(i: int32, vs: Iterable<[int32, string]>): Option<string> {
    return tryPick<[int32, string], string>((tupledArg: [int32, string]): Option<string> => {
        if (tupledArg[0] === i) {
            return tupledArg[1];
        }
        else {
            return void 0;
        }
    }, vs);
}

export function SparseRowModule_toDSLRow(vs: Iterable<[int32, string]>): FSharpList<RowElement_$union> {
    const builder$0040: RowBuilder = RowBuilder_$ctor();
    let this$_9: SheetEntity$1_$union<FSharpList<RowElement_$union>>;
    const arg: Iterable<SheetEntity$1_$union<FSharpList<RowElement_$union>>> = map<Option<string>, SheetEntity$1_$union<FSharpList<RowElement_$union>>>((_arg: Option<string>): SheetEntity$1_$union<FSharpList<RowElement_$union>> => {
        const v: Option<string> = _arg;
        if (v == null) {
            let c_1: [[DataType_$union, string], Option<int32>];
            const this$_5: SheetEntity$1_$union<[[DataType_$union, string], Option<int32>]> = CellBuilder__AsCellElement_825BA8D(CellBuilder_$ctor(), SheetEntity$1_some_2B595<FSharpList<[DataType_$union, string]>>(singleton([DataType_String(), ""] as [DataType_$union, string])));
            let matchResult: int32, errs_1: FSharpList<Message_$union>, f_1: [[DataType_$union, string], Option<int32>], ms_2_1: FSharpList<Message_$union>;
            switch (this$_5.tag) {
                case /* NoneOptional */ 1: {
                    if (equals(this$_5.fields[0], empty<Message_$union>())) {
                        matchResult = 1;
                        ms_2_1 = this$_5.fields[0];
                    }
                    else {
                        matchResult = 2;
                    }
                    break;
                }
                case /* NoneRequired */ 2: {
                    if (equals(this$_5.fields[0], empty<Message_$union>())) {
                        matchResult = 1;
                        ms_2_1 = this$_5.fields[0];
                    }
                    else {
                        matchResult = 2;
                    }
                    break;
                }
                default: {
                    matchResult = 0;
                    errs_1 = this$_5.fields[1];
                    f_1 = this$_5.fields[0];
                }
            }
            switch (matchResult) {
                case 0: {
                    c_1 = f_1!;
                    break;
                }
                case 1: {
                    throw new Error("SheetEntity does not contain Value.");
                    break;
                }
                default: {
                    let matchResult_1: int32, ms_3_1: FSharpList<Message_$union>;
                    switch (this$_5.tag) {
                        case /* NoneOptional */ 1: {
                            matchResult_1 = 0;
                            ms_3_1 = this$_5.fields[0];
                            break;
                        }
                        case /* NoneRequired */ 2: {
                            matchResult_1 = 0;
                            ms_3_1 = this$_5.fields[0];
                            break;
                        }
                        default:
                            matchResult_1 = 1;
                    }
                    switch (matchResult_1) {
                        case 0: {
                            throw new Error(`SheetEntity does not contain Value: 
	${Messages_format(ms_3_1!)}`);
                            break;
                        }
                        default:
                            throw new Error("Match failure: FsSpreadsheet.DSL.SheetEntity`1");
                    }
                }
            }
            return SheetEntity$1_some_2B595<FSharpList<RowElement_$union>>(singleton((c_1[1] == null) ? RowElement_UnindexedCell(c_1[0]) : RowElement_IndexedCell(new ColumnIndex(value(c_1[1])), c_1[0])));
        }
        else {
            const v_1: string = value(v);
            let c: [[DataType_$union, string], Option<int32>];
            const this$_2: SheetEntity$1_$union<[[DataType_$union, string], Option<int32>]> = CellBuilder__AsCellElement_825BA8D(CellBuilder_$ctor(), SheetEntity$1_some_2B595<FSharpList<[DataType_$union, string]>>(singleton([DataType_String(), v_1] as [DataType_$union, string])));
            let matchResult_2: int32, errs: FSharpList<Message_$union>, f: [[DataType_$union, string], Option<int32>], ms_2: FSharpList<Message_$union>;
            switch (this$_2.tag) {
                case /* NoneOptional */ 1: {
                    if (equals(this$_2.fields[0], empty<Message_$union>())) {
                        matchResult_2 = 1;
                        ms_2 = this$_2.fields[0];
                    }
                    else {
                        matchResult_2 = 2;
                    }
                    break;
                }
                case /* NoneRequired */ 2: {
                    if (equals(this$_2.fields[0], empty<Message_$union>())) {
                        matchResult_2 = 1;
                        ms_2 = this$_2.fields[0];
                    }
                    else {
                        matchResult_2 = 2;
                    }
                    break;
                }
                default: {
                    matchResult_2 = 0;
                    errs = this$_2.fields[1];
                    f = this$_2.fields[0];
                }
            }
            switch (matchResult_2) {
                case 0: {
                    c = f!;
                    break;
                }
                case 1: {
                    throw new Error("SheetEntity does not contain Value.");
                    break;
                }
                default: {
                    let matchResult_3: int32, ms_3: FSharpList<Message_$union>;
                    switch (this$_2.tag) {
                        case /* NoneOptional */ 1: {
                            matchResult_3 = 0;
                            ms_3 = this$_2.fields[0];
                            break;
                        }
                        case /* NoneRequired */ 2: {
                            matchResult_3 = 0;
                            ms_3 = this$_2.fields[0];
                            break;
                        }
                        default:
                            matchResult_3 = 1;
                    }
                    switch (matchResult_3) {
                        case 0: {
                            throw new Error(`SheetEntity does not contain Value: 
	${Messages_format(ms_3!)}`);
                            break;
                        }
                        default:
                            throw new Error("Match failure: FsSpreadsheet.DSL.SheetEntity`1");
                    }
                }
            }
            return SheetEntity$1_some_2B595<FSharpList<RowElement_$union>>(singleton((c[1] == null) ? RowElement_UnindexedCell(c[0]) : RowElement_IndexedCell(new ColumnIndex(value(c[1])), c[0])));
        }
    }, SparseRowModule_getAllValues(vs));
    this$_9 = fold<SheetEntity$1_$union<FSharpList<RowElement_$union>>, SheetEntity$1_$union<FSharpList<RowElement_$union>>>((state: SheetEntity$1_$union<FSharpList<RowElement_$union>>, we: SheetEntity$1_$union<FSharpList<RowElement_$union>>): SheetEntity$1_$union<FSharpList<RowElement_$union>> => RowBuilder__Combine_19F30600(builder$0040, state, we), RowBuilder_get_Empty(), arg);
    let matchResult_4: int32, errs_2: FSharpList<Message_$union>, f_3: FSharpList<RowElement_$union>, ms_2_2: FSharpList<Message_$union>;
    switch (this$_9.tag) {
        case /* NoneOptional */ 1: {
            if (equals(this$_9.fields[0], empty<Message_$union>())) {
                matchResult_4 = 1;
                ms_2_2 = this$_9.fields[0];
            }
            else {
                matchResult_4 = 2;
            }
            break;
        }
        case /* NoneRequired */ 2: {
            if (equals(this$_9.fields[0], empty<Message_$union>())) {
                matchResult_4 = 1;
                ms_2_2 = this$_9.fields[0];
            }
            else {
                matchResult_4 = 2;
            }
            break;
        }
        default: {
            matchResult_4 = 0;
            errs_2 = this$_9.fields[1];
            f_3 = this$_9.fields[0];
        }
    }
    switch (matchResult_4) {
        case 0:
            return f_3!;
        case 1:
            throw new Error("SheetEntity does not contain Value.");
        default: {
            let matchResult_5: int32, ms_3_2: FSharpList<Message_$union>;
            switch (this$_9.tag) {
                case /* NoneOptional */ 1: {
                    matchResult_5 = 0;
                    ms_3_2 = this$_9.fields[0];
                    break;
                }
                case /* NoneRequired */ 2: {
                    matchResult_5 = 0;
                    ms_3_2 = this$_9.fields[0];
                    break;
                }
                default:
                    matchResult_5 = 1;
            }
            switch (matchResult_5) {
                case 0:
                    throw new Error(`SheetEntity does not contain Value: 
	${Messages_format(ms_3_2!)}`);
                default:
                    throw new Error("Match failure: FsSpreadsheet.DSL.SheetEntity`1");
            }
        }
    }
}

export function SparseRowModule_readFromSheet(sheet: FsWorksheet): Iterable<Iterable<[int32, string]>> {
    return map<FsRow, Iterable<[int32, string]>>(SparseRowModule_fromFsRow, sheet.Rows);
}

export function SparseRowModule_writeToSheet(rowI: int32, row: Iterable<[int32, string]>, sheet: FsWorksheet): void {
    const fsRow: FsRow = sheet.Row(rowI);
    iterate<[int32, string]>((tupledArg: [int32, string]): void => {
        fsRow.Item(tupledArg[0] + 1).SetValueAs<any>(tupledArg[1]);
    }, row);
}

export class SparseTable extends Record implements IEquatable<SparseTable> {
    readonly Matrix: IMap<[string, int32], string>;
    readonly Keys: FSharpList<string>;
    readonly CommentKeys: FSharpList<string>;
    readonly ColumnCount: int32;
    constructor(Matrix: IMap<[string, int32], string>, Keys: FSharpList<string>, CommentKeys: FSharpList<string>, ColumnCount: int32) {
        super();
        this.Matrix = Matrix;
        this.Keys = Keys;
        this.CommentKeys = CommentKeys;
        this.ColumnCount = (ColumnCount | 0);
    }
}

export function SparseTable_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Spreadsheet.SparseTable", [], SparseTable, () => [["Matrix", class_type("System.Collections.Generic.Dictionary`2", [tuple_type(string_type, int32_type), string_type])], ["Keys", list_type(string_type)], ["CommentKeys", list_type(string_type)], ["ColumnCount", int32_type]]);
}

export function SparseTable__TryGetValue_11FD62A8(this$: SparseTable, key: [string, int32]): Option<string> {
    return Dictionary_tryGetValue<[string, int32], string>(key, this$.Matrix);
}

export function SparseTable__TryGetValueDefault_5BAE6133(this$: SparseTable, defaultValue: string, key: [string, int32]): string {
    if (this$.Matrix.has(key)) {
        return getItemFromDict(this$.Matrix, key);
    }
    else {
        return defaultValue;
    }
}

export function SparseTable_Create_Z2192E64B(matrix?: IMap<[string, int32], string>, keys?: FSharpList<string>, commentKeys?: FSharpList<string>, length?: int32): SparseTable {
    return new SparseTable(defaultArg(matrix, new Dictionary<[string, int32], string>([], {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    })), defaultArg(keys, empty<string>()), defaultArg(commentKeys, empty<string>()), defaultArg(length, 0));
}

export function SparseTable_AddRow(key: string, values: Iterable<[int32, string]>, matrix: SparseTable): SparseTable {
    iterate<[int32, string]>((tupledArg: [int32, string]): void => {
        addToDict(matrix.Matrix, [key, tupledArg[0]] as [string, int32], tupledArg[1]);
    }, values);
    const length: int32 = (isEmpty<[int32, string]>(values) ? 0 : (1 + maxBy<[int32, string], int32>((tuple: [int32, string]): int32 => tuple[0], values, {
        Compare: comparePrimitives,
    })[0])) | 0;
    return new SparseTable(matrix.Matrix, append<string>(matrix.Keys, singleton(key)), matrix.CommentKeys, (length > matrix.ColumnCount) ? length : matrix.ColumnCount);
}

export function SparseTable_AddEmptyComment(key: string, matrix: SparseTable): SparseTable {
    return new SparseTable(matrix.Matrix, matrix.Keys, append<string>(matrix.CommentKeys, singleton(key)), matrix.ColumnCount);
}

export function SparseTable_AddComment(key: string, values: Iterable<[int32, string]>, matrix: SparseTable): SparseTable {
    if (length_1<[int32, string]>(values) === 0) {
        return SparseTable_AddEmptyComment(key, matrix);
    }
    else {
        iterate<[int32, string]>((tupledArg: [int32, string]): void => {
            addToDict(matrix.Matrix, [key, tupledArg[0]] as [string, int32], tupledArg[1]);
        }, values);
        const length: int32 = (isEmpty<[int32, string]>(values) ? 0 : (1 + maxBy<[int32, string], int32>((tuple: [int32, string]): int32 => tuple[0], values, {
            Compare: comparePrimitives,
        })[0])) | 0;
        return new SparseTable(matrix.Matrix, matrix.Keys, append<string>(matrix.CommentKeys, singleton(key)), (length > matrix.ColumnCount) ? length : matrix.ColumnCount);
    }
}

export function SparseTable_FromRows_Z5579EC29(en: IEnumerator<Iterable<[int32, string]>>, labels: FSharpList<string>, lineNumber: int32, prefix?: string): [Option<string>, int32, FSharpList<Remark>, SparseTable] {
    try {
        const prefix_1: string = (prefix == null) ? "" : (value(prefix) + " ");
        const loop = (matrix_mut: SparseTable, remarks_mut: FSharpList<Remark>, lineNumber_1_mut: int32): [Option<string>, int32, FSharpList<Remark>, SparseTable] => {
            let v_2: Iterable<[int32, string]>, k_1: string;
            loop:
            while (true) {
                const matrix: SparseTable = matrix_mut, remarks: FSharpList<Remark> = remarks_mut, lineNumber_1: int32 = lineNumber_1_mut;
                if (en["System.Collections.IEnumerator.MoveNext"]()) {
                    const row: Iterable<[int32, string]> = map<[int32, string], [int32, string]>((tupledArg: [int32, string]): [int32, string] => ([tupledArg[0] - 1, tupledArg[1]] as [int32, string]), en["System.Collections.Generic.IEnumerator`1.get_Current"]());
                    const matchValue: Option<string> = map_1<[int32, string], string>((tuple: [int32, string]): string => tuple[1], tryItem<[int32, string]>(0, row));
                    const vals: Option<Iterable<[int32, string]>> = Seq_trySkip<[int32, string]>(1, row);
                    const key: Option<string> = matchValue;
                    let matchResult: int32, k: string, v_1: Iterable<[int32, string]>;
                    const activePatternResult: Option<string> = Comment_$007CComment$007C_$007C(key);
                    if (activePatternResult != null) {
                        if (vals != null) {
                            matchResult = 0;
                            k = value(activePatternResult);
                            v_1 = value(vals);
                        }
                        else {
                            matchResult = 1;
                        }
                    }
                    else {
                        matchResult = 1;
                    }
                    switch (matchResult) {
                        case 0: {
                            matrix_mut = SparseTable_AddComment(k!, v_1!, matrix);
                            remarks_mut = remarks;
                            lineNumber_1_mut = (lineNumber_1 + 1);
                            continue loop;
                        }
                        default: {
                            let matchResult_1: int32, k_2: string, k_3: string, v_3: Iterable<[int32, string]>, k_4: string;
                            const activePatternResult_1: Option<string> = Remark_$007CRemark$007C_$007C(key);
                            if (activePatternResult_1 != null) {
                                matchResult_1 = 0;
                                k_2 = value(activePatternResult_1);
                            }
                            else if (key != null) {
                                if (vals != null) {
                                    if ((v_2 = value(vals), (k_1 = value(key), exists<string>((label: string): boolean => (k_1 === (prefix_1 + label)), labels)))) {
                                        matchResult_1 = 1;
                                        k_3 = value(key);
                                        v_3 = value(vals);
                                    }
                                    else {
                                        matchResult_1 = 2;
                                        k_4 = value(key);
                                    }
                                }
                                else {
                                    matchResult_1 = 2;
                                    k_4 = value(key);
                                }
                            }
                            else {
                                matchResult_1 = 3;
                            }
                            switch (matchResult_1) {
                                case 0: {
                                    matrix_mut = matrix;
                                    remarks_mut = cons(Remark.make(lineNumber_1, k_2!), remarks);
                                    lineNumber_1_mut = (lineNumber_1 + 1);
                                    continue loop;
                                }
                                case 1: {
                                    matrix_mut = SparseTable_AddRow(find<string>((label_1: string): boolean => (k_3! === (prefix_1 + label_1)), labels), v_3!, matrix);
                                    remarks_mut = remarks;
                                    lineNumber_1_mut = (lineNumber_1 + 1);
                                    continue loop;
                                }
                                case 2:
                                    return [k_4!, lineNumber_1, remarks, matrix] as [Option<string>, int32, FSharpList<Remark>, SparseTable];
                                default:
                                    return [void 0, lineNumber_1, remarks, matrix] as [Option<string>, int32, FSharpList<Remark>, SparseTable];
                            }
                        }
                    }
                }
                else {
                    return [void 0, lineNumber_1, remarks, matrix] as [Option<string>, int32, FSharpList<Remark>, SparseTable];
                }
                break;
            }
        };
        return loop(SparseTable_Create_Z2192E64B(), empty<Remark>(), lineNumber);
    }
    catch (err: any) {
        const arg_9: string = err.message;
        return toFail(printf("Error parsing block in investigation file starting from line number %i: %s"))(lineNumber)(arg_9);
    }
}

export function SparseTable_ToRows_6A3D4534(matrix: SparseTable, prefix?: string): Iterable<Iterable<[int32, string]>> {
    const prefix_1: string = (prefix == null) ? "" : (value(prefix) + " ");
    return delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append_1<Iterable<[int32, string]>>(map<string, Iterable<[int32, string]>>((key: string): Iterable<[int32, string]> => SparseRowModule_fromValues(cons(prefix_1 + key, initialize_1<string>(matrix.ColumnCount - 1, (i: int32): string => SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [key, i + 1] as [string, int32])))), matrix.Keys), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => map<string, Iterable<[int32, string]>>((key_1: string): Iterable<[int32, string]> => SparseRowModule_fromValues(cons(Comment_wrapCommentKey(key_1), initialize_1<string>(matrix.ColumnCount - 1, (i_1: int32): string => SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [key_1, i_1 + 1] as [string, int32])))), matrix.CommentKeys))));
}

export function SparseTable_GetEmptyComments_651559CC(matrix: SparseTable): Comment$[] {
    return toArray<Comment$>(map_2<string, Comment$>((key: string): Comment$ => Comment$.create(void 0, key), matrix.CommentKeys));
}

