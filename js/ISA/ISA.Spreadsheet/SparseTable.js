import { append as append_1, delay, tryItem, length as length_1, isEmpty, iterate, fold, tryPick, maxBy, initialize, choose, map, indexed } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { defaultArg, map as map_1 } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { tryFind, ofSeq } from "../../fable_modules/fable-library.4.1.4/Map.js";
import { arrayHash, equalArrays, equals, comparePrimitives } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { RowBuilder_get_Empty, RowBuilder__Combine_19F30600, RowBuilder_$ctor } from "../../fable_modules/FsSpreadsheet.3.1.1/DSL/RowBuilder.fs.js";
import { CellBuilder_$ctor, CellBuilder__AsCellElement_825BA8D } from "../../fable_modules/FsSpreadsheet.3.1.1/DSL/CellBuilder.fs.js";
import { Messages_format, SheetEntity$1_some_2B595 } from "../../fable_modules/FsSpreadsheet.3.1.1/DSL/Types.fs.js";
import { DataType } from "../../fable_modules/FsSpreadsheet.3.1.1/Cells/FsCell.fs.js";
import { map as map_2, initialize as initialize_1, exists, find, cons, append, empty, singleton } from "../../fable_modules/fable-library.4.1.4/List.js";
import { ColumnIndex, RowElement } from "../../fable_modules/FsSpreadsheet.3.1.1/DSL/Types.fs.js";
import { Record } from "../../fable_modules/fable-library.4.1.4/Types.js";
import { record_type, list_type, class_type, tuple_type, int32_type, string_type } from "../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Seq_trySkip, Dictionary_tryGetValue } from "./CollectionAux.js";
import { addToDict, getItemFromDict } from "../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { Dictionary } from "../../fable_modules/fable-library.4.1.4/MutableMap.js";
import { printf, toFail } from "../../fable_modules/fable-library.4.1.4/String.js";
import { Comment_create_250E0578, Remark_make } from "../ISA/JsonTypes/Comment.js";
import { Comment_wrapCommentKey, Comment_$007CComment$007C_$007C, Remark_$007CRemark$007C_$007C } from "./Comment.js";

export function SparseRowModule_fromValues(v) {
    return indexed(v);
}

export function SparseRowModule_getValues(i) {
    return map((tuple) => tuple[1], i);
}

export function SparseRowModule_fromAllValues(v) {
    return choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), indexed(v));
}

export function SparseRowModule_getAllValues(i) {
    const m = ofSeq(i, {
        Compare: comparePrimitives,
    });
    return initialize(maxBy((tuple) => tuple[0], i, {
        Compare: comparePrimitives,
    })[0] + 1, (i_1) => tryFind(i_1, m));
}

export function SparseRowModule_fromFsRow(r) {
    return map((c) => [c.ColumnNumber - 1, c.Value], r.Cells);
}

export function SparseRowModule_tryGetValueAt(i, vs) {
    return tryPick((tupledArg) => {
        if (tupledArg[0] === i) {
            return tupledArg[1];
        }
        else {
            return void 0;
        }
    }, vs);
}

export function SparseRowModule_toDSLRow(vs) {
    const builder$0040 = RowBuilder_$ctor();
    let this$_9;
    const arg = map((_arg) => {
        const v = _arg;
        if (v == null) {
            let c_1;
            const this$_5 = CellBuilder__AsCellElement_825BA8D(CellBuilder_$ctor(), SheetEntity$1_some_2B595(singleton([new DataType(0, []), ""])));
            let matchResult, errs_1, f_1, ms_2_1;
            switch (this$_5.tag) {
                case 1: {
                    if (equals(this$_5.fields[0], empty())) {
                        matchResult = 1;
                        ms_2_1 = this$_5.fields[0];
                    }
                    else {
                        matchResult = 2;
                    }
                    break;
                }
                case 2: {
                    if (equals(this$_5.fields[0], empty())) {
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
                    c_1 = f_1;
                    break;
                }
                case 1: {
                    throw new Error("SheetEntity does not contain Value.");
                    break;
                }
                default: {
                    let matchResult_1, ms_3_1;
                    switch (this$_5.tag) {
                        case 1: {
                            matchResult_1 = 0;
                            ms_3_1 = this$_5.fields[0];
                            break;
                        }
                        case 2: {
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
	${Messages_format(ms_3_1)}`);
                            break;
                        }
                        default:
                            throw new Error("Match failure: FsSpreadsheet.DSL.SheetEntity`1");
                    }
                }
            }
            return SheetEntity$1_some_2B595(singleton((c_1[1] == null) ? (new RowElement(1, [c_1[0]])) : (new RowElement(0, [new ColumnIndex(c_1[1]), c_1[0]]))));
        }
        else {
            const v_1 = v;
            let c;
            const this$_2 = CellBuilder__AsCellElement_825BA8D(CellBuilder_$ctor(), SheetEntity$1_some_2B595(singleton([new DataType(0, []), v_1])));
            let matchResult_2, errs, f, ms_2;
            switch (this$_2.tag) {
                case 1: {
                    if (equals(this$_2.fields[0], empty())) {
                        matchResult_2 = 1;
                        ms_2 = this$_2.fields[0];
                    }
                    else {
                        matchResult_2 = 2;
                    }
                    break;
                }
                case 2: {
                    if (equals(this$_2.fields[0], empty())) {
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
                    c = f;
                    break;
                }
                case 1: {
                    throw new Error("SheetEntity does not contain Value.");
                    break;
                }
                default: {
                    let matchResult_3, ms_3;
                    switch (this$_2.tag) {
                        case 1: {
                            matchResult_3 = 0;
                            ms_3 = this$_2.fields[0];
                            break;
                        }
                        case 2: {
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
	${Messages_format(ms_3)}`);
                            break;
                        }
                        default:
                            throw new Error("Match failure: FsSpreadsheet.DSL.SheetEntity`1");
                    }
                }
            }
            return SheetEntity$1_some_2B595(singleton((c[1] == null) ? (new RowElement(1, [c[0]])) : (new RowElement(0, [new ColumnIndex(c[1]), c[0]]))));
        }
    }, SparseRowModule_getAllValues(vs));
    this$_9 = fold((state, we) => RowBuilder__Combine_19F30600(builder$0040, state, we), RowBuilder_get_Empty(), arg);
    let matchResult_4, errs_2, f_3, ms_2_2;
    switch (this$_9.tag) {
        case 1: {
            if (equals(this$_9.fields[0], empty())) {
                matchResult_4 = 1;
                ms_2_2 = this$_9.fields[0];
            }
            else {
                matchResult_4 = 2;
            }
            break;
        }
        case 2: {
            if (equals(this$_9.fields[0], empty())) {
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
            return f_3;
        case 1:
            throw new Error("SheetEntity does not contain Value.");
        default: {
            let matchResult_5, ms_3_2;
            switch (this$_9.tag) {
                case 1: {
                    matchResult_5 = 0;
                    ms_3_2 = this$_9.fields[0];
                    break;
                }
                case 2: {
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
	${Messages_format(ms_3_2)}`);
                default:
                    throw new Error("Match failure: FsSpreadsheet.DSL.SheetEntity`1");
            }
        }
    }
}

export function SparseRowModule_readFromSheet(sheet) {
    return map(SparseRowModule_fromFsRow, sheet.Rows);
}

export function SparseRowModule_writeToSheet(rowI, row, sheet) {
    const fsRow = sheet.Row(rowI);
    iterate((tupledArg) => {
        fsRow.Item(tupledArg[0] + 1).SetValueAs(tupledArg[1]);
    }, row);
}

export class SparseTable extends Record {
    constructor(Matrix, Keys, CommentKeys, ColumnCount) {
        super();
        this.Matrix = Matrix;
        this.Keys = Keys;
        this.CommentKeys = CommentKeys;
        this.ColumnCount = (ColumnCount | 0);
    }
}

export function SparseTable_$reflection() {
    return record_type("ISA.Spreadsheet.SparseTable", [], SparseTable, () => [["Matrix", class_type("System.Collections.Generic.Dictionary`2", [tuple_type(string_type, int32_type), string_type])], ["Keys", list_type(string_type)], ["CommentKeys", list_type(string_type)], ["ColumnCount", int32_type]]);
}

export function SparseTable__TryGetValue_11FD62A8(this$, key) {
    return Dictionary_tryGetValue(key, this$.Matrix);
}

export function SparseTable__TryGetValueDefault_5BAE6133(this$, defaultValue, key) {
    if (this$.Matrix.has(key)) {
        return getItemFromDict(this$.Matrix, key);
    }
    else {
        return defaultValue;
    }
}

export function SparseTable_Create_Z2192E64B(matrix, keys, commentKeys, length) {
    return new SparseTable(defaultArg(matrix, new Dictionary([], {
        Equals: equalArrays,
        GetHashCode: arrayHash,
    })), defaultArg(keys, empty()), defaultArg(commentKeys, empty()), defaultArg(length, 0));
}

export function SparseTable_AddRow(key, values, matrix) {
    iterate((tupledArg) => {
        addToDict(matrix.Matrix, [key, tupledArg[0]], tupledArg[1]);
    }, values);
    const length = (isEmpty(values) ? 0 : (1 + maxBy((tuple) => tuple[0], values, {
        Compare: comparePrimitives,
    })[0])) | 0;
    return new SparseTable(matrix.Matrix, append(matrix.Keys, singleton(key)), matrix.CommentKeys, (length > matrix.ColumnCount) ? length : matrix.ColumnCount);
}

export function SparseTable_AddEmptyComment(key, matrix) {
    return new SparseTable(matrix.Matrix, matrix.Keys, append(matrix.CommentKeys, singleton(key)), matrix.ColumnCount);
}

export function SparseTable_AddComment(key, values, matrix) {
    if (length_1(values) === 0) {
        return SparseTable_AddEmptyComment(key, matrix);
    }
    else {
        iterate((tupledArg) => {
            addToDict(matrix.Matrix, [key, tupledArg[0]], tupledArg[1]);
        }, values);
        const length = (isEmpty(values) ? 0 : (1 + maxBy((tuple) => tuple[0], values, {
            Compare: comparePrimitives,
        })[0])) | 0;
        return new SparseTable(matrix.Matrix, matrix.Keys, append(matrix.CommentKeys, singleton(key)), (length > matrix.ColumnCount) ? length : matrix.ColumnCount);
    }
}

export function SparseTable_FromRows_Z5579EC29(en, labels, lineNumber, prefix) {
    try {
        const prefix_1 = (prefix == null) ? "" : (prefix + " ");
        const loop = (matrix_mut, remarks_mut, lineNumber_1_mut) => {
            let v_2, k_1;
            loop:
            while (true) {
                const matrix = matrix_mut, remarks = remarks_mut, lineNumber_1 = lineNumber_1_mut;
                if (en["System.Collections.IEnumerator.MoveNext"]()) {
                    const row = map((tupledArg) => [tupledArg[0] - 1, tupledArg[1]], en["System.Collections.Generic.IEnumerator`1.get_Current"]());
                    const matchValue = map_1((tuple) => tuple[1], tryItem(0, row));
                    const vals = Seq_trySkip(1, row);
                    const key = matchValue;
                    let matchResult, k, v_1;
                    const activePatternResult = Comment_$007CComment$007C_$007C(key);
                    if (activePatternResult != null) {
                        if (vals != null) {
                            matchResult = 0;
                            k = activePatternResult;
                            v_1 = vals;
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
                            matrix_mut = SparseTable_AddComment(k, v_1, matrix);
                            remarks_mut = remarks;
                            lineNumber_1_mut = (lineNumber_1 + 1);
                            continue loop;
                        }
                        default: {
                            let matchResult_1, k_2, k_3, v_3, k_4;
                            const activePatternResult_1 = Remark_$007CRemark$007C_$007C(key);
                            if (activePatternResult_1 != null) {
                                matchResult_1 = 0;
                                k_2 = activePatternResult_1;
                            }
                            else if (key != null) {
                                if (vals != null) {
                                    if ((v_2 = vals, (k_1 = key, exists((label) => (k_1 === (prefix_1 + label)), labels)))) {
                                        matchResult_1 = 1;
                                        k_3 = key;
                                        v_3 = vals;
                                    }
                                    else {
                                        matchResult_1 = 2;
                                        k_4 = key;
                                    }
                                }
                                else {
                                    matchResult_1 = 2;
                                    k_4 = key;
                                }
                            }
                            else {
                                matchResult_1 = 3;
                            }
                            switch (matchResult_1) {
                                case 0: {
                                    matrix_mut = matrix;
                                    remarks_mut = cons(Remark_make(lineNumber_1, k_2), remarks);
                                    lineNumber_1_mut = (lineNumber_1 + 1);
                                    continue loop;
                                }
                                case 1: {
                                    matrix_mut = SparseTable_AddRow(find((label_1) => (k_3 === (prefix_1 + label_1)), labels), v_3, matrix);
                                    remarks_mut = remarks;
                                    lineNumber_1_mut = (lineNumber_1 + 1);
                                    continue loop;
                                }
                                case 2:
                                    return [k_4, lineNumber_1, remarks, matrix];
                                default:
                                    return [void 0, lineNumber_1, remarks, matrix];
                            }
                        }
                    }
                }
                else {
                    return [void 0, lineNumber_1, remarks, matrix];
                }
                break;
            }
        };
        return loop(SparseTable_Create_Z2192E64B(), empty(), lineNumber);
    }
    catch (err) {
        const arg_9 = err.message;
        return toFail(printf("Error parsing block in investigation file starting from line number %i: %s"))(lineNumber)(arg_9);
    }
}

export function SparseTable_ToRows_584133C0(matrix, prefix) {
    const prefix_1 = (prefix == null) ? "" : (prefix + " ");
    return delay(() => append_1(map((key) => SparseRowModule_fromValues(cons(prefix_1 + key, initialize_1(matrix.ColumnCount - 1, (i) => SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [key, i + 1])))), matrix.Keys), delay(() => map((key_1) => SparseRowModule_fromValues(cons(Comment_wrapCommentKey(key_1), initialize_1(matrix.ColumnCount - 1, (i_1) => SparseTable__TryGetValueDefault_5BAE6133(matrix, "", [key_1, i_1 + 1])))), matrix.CommentKeys))));
}

export function SparseTable_GetEmptyComments_Z15A4F148(matrix) {
    return map_2((key) => Comment_create_250E0578(void 0, key), matrix.CommentKeys);
}

