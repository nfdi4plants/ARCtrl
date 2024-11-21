import { boxHashValues, Unchecked_alignByHeaders, Unchecked_extendToRowCount, Unchecked_removeRowCells_withIndexChange, Unchecked_addRows, Unchecked_addRow, Unchecked_getEmptyCellForHeader, Unchecked_moveColumnTo, Unchecked_removeColumnCells_withIndexChange, tryFindDuplicateUniqueInArray, Unchecked_removeColumnCells, Unchecked_removeHeader, Unchecked_fillMissingCells, Unchecked_addColumn, SanityChecks_validateColumn, tryFindDuplicateUnique, Unchecked_setCellAt, SanityChecks_validateRowIndex, SanityChecks_validateColumnIndex, Unchecked_tryGetCellAt, getRowCount, getColumnCount, SanityChecks_validate } from "./ArcTableAux.js";
import { Dictionary } from "../../fable_modules/fable-library-js.4.22.0/MutableMap.js";
import { compareArrays, safeHash, comparePrimitives, equals, disposeSafe, getEnumerator, arrayHash, equalArrays } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { value as value_3, map as map_2, unwrap, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { item as item_1, fold, append as append_1, toList, indexed as indexed_1, choose, filter, length, tryFindIndex, empty, singleton, collect, removeAt, iterate, map, delay, toArray } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { rangeDouble } from "../../fable_modules/fable-library-js.4.22.0/Range.js";
import { ResizeArray_map } from "../Helper/Collections.js";
import { getItemFromDict, addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { join, printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { sortBy, indexed, mapIndexed, item, sortDescending, iterateIndexed, initialize, map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { append, iterate as iterate_1, isEmpty } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { StringBuilder__AppendLine_Z721C83C5, StringBuilder_$ctor } from "../../fable_modules/fable-library-js.4.22.0/System.Text.js";
import { toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { CompositeCell } from "./CompositeCell.js";
import { CompositeHeader } from "./CompositeHeader.js";
import { Array_groupBy } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { boxHashSeq, boxHashArray } from "../Helper/HashCodes.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class ArcTable {
    constructor(name, headers, values) {
        const valid = SanityChecks_validate(headers, values, true);
        this["name@21"] = name;
        this["headers@22"] = headers;
        this["values@23"] = values;
    }
    get Headers() {
        const this$ = this;
        return this$["headers@22"];
    }
    set Headers(newHeaders) {
        const this$ = this;
        SanityChecks_validate(newHeaders, this$["values@23"], true);
        this$["headers@22"] = newHeaders;
    }
    get Values() {
        const this$ = this;
        return this$["values@23"];
    }
    set Values(newValues) {
        const this$ = this;
        SanityChecks_validate(this$["headers@22"], newValues, true);
        this$["values@23"] = newValues;
    }
    get Name() {
        const this$ = this;
        return this$["name@21"];
    }
    set Name(newName) {
        const this$ = this;
        this$["name@21"] = newName;
    }
    static create(name, headers, values) {
        return new ArcTable(name, headers, values);
    }
    static init(name) {
        return new ArcTable(name, [], new Dictionary([], {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
    }
    static createFromHeaders(name, headers) {
        return ArcTable.create(name, headers, new Dictionary([], {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
    }
    static createFromRows(name, headers, rows) {
        const t = ArcTable.createFromHeaders(name, headers);
        t.AddRows(rows);
        return t;
    }
    Validate(raiseException) {
        const this$ = this;
        const raiseException_1 = defaultArg(raiseException, true);
        return SanityChecks_validate(this$.Headers, this$.Values, raiseException_1);
    }
    static validate(raiseException) {
        return (table) => table.Validate(unwrap(raiseException));
    }
    get ColumnCount() {
        const this$ = this;
        return getColumnCount(this$.Headers) | 0;
    }
    get RowCount() {
        const this$ = this;
        return getRowCount(this$.Values) | 0;
    }
    get Columns() {
        const this$ = this;
        return toArray(delay(() => map((i) => this$.GetColumn(i), rangeDouble(0, 1, this$.ColumnCount - 1))));
    }
    Copy() {
        const this$ = this;
        const nextHeaders = ResizeArray_map((h) => h.Copy(), this$.Headers);
        const nextValues = new Dictionary([], {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        });
        iterate((tupledArg) => {
            const ci = tupledArg[0] | 0;
            const ri = tupledArg[1] | 0;
            addToDict(nextValues, [ci, ri], getItemFromDict(this$.Values, [ci, ri]).Copy());
        }, this$.Values.keys());
        return ArcTable.create(this$.Name, nextHeaders, nextValues);
    }
    TryGetCellAt(column, row) {
        const this$ = this;
        return Unchecked_tryGetCellAt(column, row, this$.Values);
    }
    static tryGetCellAt(column, row) {
        return (table) => table.TryGetCellAt(column, row);
    }
    GetCellAt(column, row) {
        const this$ = this;
        try {
            return getItemFromDict(this$.Values, [column, row]);
        }
        catch (matchValue) {
            const arg_2 = this$.Name;
            return toFail(printf("Unable to find cell for index: (%i, %i) in table %s"))(column)(row)(arg_2);
        }
    }
    static getCellAt(column, row) {
        return (table) => table.GetCellAt(column, row);
    }
    IterColumns(action) {
        const this$ = this;
        for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
            action(this$.GetColumn(columnIndex));
        }
    }
    static iterColumns(action) {
        return (table) => {
            const copy = table.Copy();
            copy.IterColumns(action);
            return copy;
        };
    }
    IteriColumns(action) {
        const this$ = this;
        for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
            action(columnIndex, this$.GetColumn(columnIndex));
        }
    }
    static iteriColumns(action) {
        return (table) => {
            const copy = table.Copy();
            copy.IteriColumns(action);
            return copy;
        };
    }
    UpdateCellAt(columnIndex, rowIndex, c, skipValidation) {
        const this$ = this;
        if (!defaultArg(skipValidation, false)) {
            SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
            SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
            c.ValidateAgainstHeader(this$.Headers[columnIndex], true);
        }
        Unchecked_setCellAt(columnIndex, rowIndex, c, this$.Values);
    }
    static updateCellAt(columnIndex, rowIndex, cell, skipValidation) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateCellAt(columnIndex, rowIndex, cell, unwrap(skipValidation));
            return newTable;
        };
    }
    SetCellAt(columnIndex, rowIndex, c, skipValidation) {
        const this$ = this;
        if (!defaultArg(skipValidation, false)) {
            SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
            c.ValidateAgainstHeader(this$.Headers[columnIndex], true);
        }
        Unchecked_setCellAt(columnIndex, rowIndex, c, this$.Values);
    }
    static setCellAt(columnIndex, rowIndex, cell, skipValidation) {
        return (table) => {
            const newTable = table.Copy();
            newTable.SetCellAt(columnIndex, rowIndex, cell, unwrap(skipValidation));
            return newTable;
        };
    }
    UpdateCellsBy(f, skipValidation) {
        const this$ = this;
        const skipValidation_1 = defaultArg(skipValidation, false);
        let enumerator = getEnumerator(this$.Values);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const kv = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const patternInput = kv[0];
                const ri = patternInput[1] | 0;
                const ci = patternInput[0] | 0;
                const newCell = f(ci, ri, kv[1]);
                if (!skipValidation_1) {
                    newCell.ValidateAgainstHeader(this$.Headers[ci], true);
                }
                Unchecked_setCellAt(ci, ri, newCell, this$.Values);
            }
        }
        finally {
            disposeSafe(enumerator);
        }
    }
    static updateCellsBy(f, skipValidation) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateCellsBy(f, unwrap(skipValidation));
        };
    }
    UpdateCellBy(columnIndex, rowIndex, f, skipValidation) {
        const this$ = this;
        const skipValidation_1 = defaultArg(skipValidation, false);
        if (!skipValidation_1) {
            SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
            SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        }
        const newCell = f(this$.GetCellAt(columnIndex, rowIndex));
        if (!skipValidation_1) {
            newCell.ValidateAgainstHeader(this$.Headers[columnIndex], true);
        }
        Unchecked_setCellAt(columnIndex, rowIndex, newCell, this$.Values);
    }
    static updateCellBy(columnIndex, rowIndex, f, skipValidation) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateCellBy(columnIndex, rowIndex, f, unwrap(skipValidation));
        };
    }
    UpdateHeader(index, newHeader, forceConvertCells) {
        const this$ = this;
        const forceConvertCells_1 = defaultArg(forceConvertCells, false);
        SanityChecks_validateColumnIndex(index, this$.ColumnCount, false);
        const header = newHeader;
        const matchValue = tryFindDuplicateUnique(header, removeAt(index, this$.Headers));
        if (matchValue != null) {
            throw new Error(`Invalid input. Tried setting unique header \`${header}\`, but header of same type already exists at index ${matchValue}.`);
        }
        const c = new CompositeColumn(newHeader, this$.GetColumn(index).Cells);
        if (c.Validate()) {
            let setHeader;
            setHeader = (this$.Headers[index] = newHeader);
        }
        else if (forceConvertCells_1) {
            const convertedCells = newHeader.IsTermColumn ? map_1((c_1) => {
                if (c_1.isFreeText) {
                    return c_1.ToTermCell();
                }
                else {
                    return c_1;
                }
            }, c.Cells) : map_1((c_2) => c_2.ToFreeTextCell(), c.Cells);
            this$.UpdateColumn(index, newHeader, convertedCells);
        }
        else {
            throw new Error("Tried setting header for column with invalid type of cells. Set `forceConvertCells` flag to automatically convert cells into valid CompositeCell type.");
        }
    }
    static updateHeader(index, header) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateHeader(index, header);
            return newTable;
        };
    }
    AddColumn(header, cells, index, forceReplace, skipFillMissing) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.ColumnCount) | 0;
        const cells_1 = defaultArg(cells, []);
        const forceReplace_1 = defaultArg(forceReplace, false);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        SanityChecks_validateColumn(CompositeColumn.create(header, cells_1));
        Unchecked_addColumn(header, cells_1, index_1, forceReplace_1, false, this$.Headers, this$.Values);
        if (!equals(skipFillMissing, true)) {
            Unchecked_fillMissingCells(this$.Headers, this$.Values);
        }
    }
    static addColumn(header, cells, index, forceReplace) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddColumn(header, unwrap(cells), unwrap(index), unwrap(forceReplace));
            return newTable;
        };
    }
    AddColumnFill(header, cell, index, forceReplace) {
        const this$ = this;
        const cells = initialize(this$.RowCount, (_arg) => cell.Copy());
        this$.AddColumn(header, cells, unwrap(index), unwrap(forceReplace));
    }
    static addColumnFill(header, cell, index, forceReplace) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddColumnFill(header, cell, unwrap(index), unwrap(forceReplace));
            return newTable;
        };
    }
    UpdateColumn(columnIndex, header, cells, skipFillMissing) {
        const this$ = this;
        SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
        const column = CompositeColumn.create(header, unwrap(cells));
        SanityChecks_validateColumn(column);
        const header_1 = column.Header;
        const matchValue = tryFindDuplicateUnique(header_1, removeAt(columnIndex, this$.Headers));
        if (matchValue != null) {
            throw new Error(`Invalid input. Tried setting unique header \`${header_1}\`, but header of same type already exists at index ${matchValue}.`);
        }
        Unchecked_removeHeader(columnIndex, this$.Headers);
        Unchecked_removeColumnCells(columnIndex, this$.Values);
        this$.Headers.splice(columnIndex, 0, column.Header);
        iterateIndexed((rowIndex, v) => {
            Unchecked_setCellAt(columnIndex, rowIndex, v, this$.Values);
        }, column.Cells);
        if (!equals(skipFillMissing, true)) {
            Unchecked_fillMissingCells(this$.Headers, this$.Values);
        }
    }
    static updateColumn(columnIndex, header, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateColumn(columnIndex, header, unwrap(cells));
            return newTable;
        };
    }
    InsertColumn(index, header, cells) {
        const this$ = this;
        this$.AddColumn(header, unwrap(cells), index, false);
    }
    static insertColumn(index, header, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.InsertColumn(index, header, unwrap(cells));
            return newTable;
        };
    }
    AppendColumn(header, cells) {
        const this$ = this;
        this$.AddColumn(header, unwrap(cells), this$.ColumnCount, false);
    }
    static appendColumn(header, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AppendColumn(header, unwrap(cells));
            return newTable;
        };
    }
    AddColumns(columns, index, forceReplace, skipFillMissing) {
        const this$ = this;
        let index_1 = defaultArg(index, this$.ColumnCount);
        const forceReplace_1 = defaultArg(forceReplace, false);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        const duplicates = tryFindDuplicateUniqueInArray(map((x) => x.Header, columns));
        if (!isEmpty(duplicates)) {
            const sb = StringBuilder_$ctor();
            StringBuilder__AppendLine_Z721C83C5(sb, "Found duplicate unique columns in `columns`.");
            iterate_1((x_1) => {
                StringBuilder__AppendLine_Z721C83C5(sb, `Duplicate \`${x_1.HeaderType}\` at index ${x_1.Index1} and ${x_1.Index2}.`);
            }, duplicates);
            throw new Error(toString(sb));
        }
        columns.forEach((x_2) => {
            SanityChecks_validateColumn(x_2);
        });
        columns.forEach((col) => {
            const prevHeadersCount = this$.Headers.length | 0;
            Unchecked_addColumn(col.Header, col.Cells, index_1, forceReplace_1, false, this$.Headers, this$.Values);
            if (this$.Headers.length > prevHeadersCount) {
                index_1 = ((index_1 + 1) | 0);
            }
        });
        if (!equals(skipFillMissing, true)) {
            Unchecked_fillMissingCells(this$.Headers, this$.Values);
        }
    }
    static addColumns(columns, index, skipFillMissing) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddColumns(columns, unwrap(index), undefined, unwrap(skipFillMissing));
            return newTable;
        };
    }
    RemoveColumn(index) {
        const this$ = this;
        SanityChecks_validateColumnIndex(index, this$.ColumnCount, false);
        const columnCount = this$.ColumnCount | 0;
        Unchecked_removeHeader(index, this$.Headers);
        Unchecked_removeColumnCells_withIndexChange(index, columnCount, this$.RowCount, this$.Values);
    }
    static removeColumn(index) {
        return (table) => {
            const newTable = table.Copy();
            newTable.RemoveColumn(index);
            return newTable;
        };
    }
    RemoveColumns(indexArr) {
        const this$ = this;
        indexArr.forEach((index) => {
            SanityChecks_validateColumnIndex(index, this$.ColumnCount, false);
        });
        const indexArr_1 = sortDescending(indexArr, {
            Compare: comparePrimitives,
        });
        indexArr_1.forEach((index_1) => {
            this$.RemoveColumn(index_1);
        });
    }
    static removeColumns(indexArr) {
        return (table) => {
            const newTable = table.Copy();
            newTable.RemoveColumns(indexArr);
            return newTable;
        };
    }
    GetColumn(columnIndex) {
        const this$ = this;
        SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
        const h = this$.Headers[columnIndex];
        const cells = toArray(delay(() => collect((i) => {
            const matchValue = this$.TryGetCellAt(columnIndex, i);
            if (matchValue != null) {
                return singleton(matchValue);
            }
            else {
                toFail(printf("Unable to find cell for index: (%i, %i)"))(columnIndex)(i);
                return empty();
            }
        }, rangeDouble(0, 1, this$.RowCount - 1))));
        return CompositeColumn.create(h, cells);
    }
    static getColumn(index) {
        return (table) => table.GetColumn(index);
    }
    TryGetColumnByHeader(header) {
        const this$ = this;
        return map_2((i) => this$.GetColumn(i), tryFindIndex((x) => equals(x, header), this$.Headers));
    }
    static tryGetColumnByHeader(header) {
        return (table) => table.TryGetColumnByHeader(header);
    }
    TryGetColumnByHeaderBy(headerPredicate) {
        const this$ = this;
        return map_2((i) => this$.GetColumn(i), tryFindIndex(headerPredicate, this$.Headers));
    }
    static tryGetColumnByHeaderBy(headerPredicate) {
        return (table) => table.TryGetColumnByHeaderBy(headerPredicate);
    }
    GetColumnByHeader(header) {
        const this$ = this;
        const matchValue = this$.TryGetColumnByHeader(header);
        if (matchValue == null) {
            const arg = this$.Name;
            return toFail(printf("Unable to find column with header in table %s: %O"))(arg)(header);
        }
        else {
            return matchValue;
        }
    }
    static getColumnByHeader(header) {
        return (table) => table.GetColumnByHeader(header);
    }
    TryGetInputColumn() {
        const this$ = this;
        return map_2((i) => this$.GetColumn(i), tryFindIndex((x) => x.isInput, this$.Headers));
    }
    static tryGetInputColumn() {
        return (table) => table.TryGetInputColumn();
    }
    GetInputColumn() {
        const this$ = this;
        const matchValue = this$.TryGetInputColumn();
        if (matchValue == null) {
            const arg = this$.Name;
            return toFail(printf("Unable to find input column in table %s"))(arg);
        }
        else {
            return matchValue;
        }
    }
    static getInputColumn() {
        return (table) => table.GetInputColumn();
    }
    TryGetOutputColumn() {
        const this$ = this;
        return map_2((i) => this$.GetColumn(i), tryFindIndex((x) => x.isOutput, this$.Headers));
    }
    static tryGetOutputColumn() {
        return (table) => table.TryGetOutputColumn();
    }
    GetOutputColumn() {
        const this$ = this;
        const matchValue = this$.TryGetOutputColumn();
        if (matchValue == null) {
            const arg = this$.Name;
            return toFail(printf("Unable to find output column in table %s"))(arg);
        }
        else {
            return matchValue;
        }
    }
    static getOutputColumn() {
        return (table) => table.GetOutputColumn();
    }
    MoveColumn(startCol, endCol) {
        const this$ = this;
        if (startCol === endCol) {
        }
        else if ((startCol < 0) ? true : (startCol >= this$.ColumnCount)) {
            toFail(printf("Cannt move column. Invalid start column index: %i"))(startCol);
        }
        else if ((endCol < 0) ? true : (endCol >= this$.ColumnCount)) {
            toFail(printf("Cannt move column. Invalid end column index: %i"))(endCol);
        }
        else {
            Unchecked_moveColumnTo(this$.RowCount, startCol, endCol, this$.Headers, this$.Values);
        }
    }
    static moveColumn(startCol, endCol) {
        return (table) => {
            const newTable = table.Copy();
            newTable.MoveColumn(startCol, endCol);
            return newTable;
        };
    }
    AddRow(cells, index) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.RowCount) | 0;
        const cells_1 = (cells == null) ? toArray(delay(() => collect((columnIndex) => singleton(Unchecked_getEmptyCellForHeader(this$.Headers[columnIndex], Unchecked_tryGetCellAt(columnIndex, 0, this$.Values))), rangeDouble(0, 1, this$.ColumnCount - 1)))) : value_3(cells);
        SanityChecks_validateRowIndex(index_1, this$.RowCount, true);
        const columnCount = this$.ColumnCount | 0;
        const newCellsCount = length(cells_1) | 0;
        if (columnCount === 0) {
            throw new Error("Table contains no columns! Cannot add row to empty table!");
        }
        else if (newCellsCount !== columnCount) {
            throw new Error(`Cannot add a new row with ${newCellsCount} cells, as the table has ${columnCount} columns.`);
        }
        for (let columnIndex_1 = 0; columnIndex_1 <= (this$.ColumnCount - 1); columnIndex_1++) {
            const h_1 = this$.Headers[columnIndex_1];
            SanityChecks_validateColumn(CompositeColumn.create(h_1, [item(columnIndex_1, cells_1)]));
        }
        Unchecked_addRow(index_1, cells_1, this$.Headers, this$.Values);
    }
    static addRow(cells, index) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddRow(unwrap(cells), unwrap(index));
            return newTable;
        };
    }
    UpdateRow(rowIndex, cells) {
        const this$ = this;
        SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        const columnCount = this$.RowCount | 0;
        const newCellsCount = length(cells) | 0;
        if (columnCount === 0) {
            throw new Error("Table contains no columns! Cannot add row to empty table!");
        }
        else if (newCellsCount !== columnCount) {
            throw new Error(`Cannot add a new row with ${newCellsCount} cells, as the table has ${columnCount} columns.`);
        }
        iterateIndexed((i, cell) => {
            const h = this$.Headers[i];
            SanityChecks_validateColumn(CompositeColumn.create(h, [cell]));
        }, cells);
        iterateIndexed((columnIndex, cell_1) => {
            Unchecked_setCellAt(columnIndex, rowIndex, cell_1, this$.Values);
        }, cells);
    }
    static updateRow(rowIndex, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateRow(rowIndex, cells);
            return newTable;
        };
    }
    AppendRow(cells) {
        const this$ = this;
        this$.AddRow(unwrap(cells), this$.RowCount);
    }
    static appendRow(cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AppendRow(unwrap(cells));
            return newTable;
        };
    }
    InsertRow(index, cells) {
        const this$ = this;
        this$.AddRow(unwrap(cells), index);
    }
    static insertRow(index, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddRow(unwrap(cells), index);
            return newTable;
        };
    }
    AddRows(rows, index) {
        const this$ = this;
        let index_1 = defaultArg(index, this$.RowCount);
        SanityChecks_validateRowIndex(index_1, this$.RowCount, true);
        rows.forEach((row) => {
            const columnCount = this$.ColumnCount | 0;
            const newCellsCount = length(row) | 0;
            if (columnCount === 0) {
                throw new Error("Table contains no columns! Cannot add row to empty table!");
            }
            else if (newCellsCount !== columnCount) {
                throw new Error(`Cannot add a new row with ${newCellsCount} cells, as the table has ${columnCount} columns.`);
            }
        });
        for (let idx = 0; idx <= (rows.length - 1); idx++) {
            const row_1 = item(idx, rows);
            for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
                const h = this$.Headers[columnIndex];
                SanityChecks_validateColumn(CompositeColumn.create(h, [item(columnIndex, row_1)]));
            }
        }
        Unchecked_addRows(index_1, rows, this$.Headers, this$.Values);
    }
    static addRows(rows, index) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddRows(rows, unwrap(index));
            return newTable;
        };
    }
    AddRowsEmpty(rowCount, index) {
        const this$ = this;
        const row = toArray(delay(() => collect((columnIndex) => singleton(Unchecked_getEmptyCellForHeader(this$.Headers[columnIndex], Unchecked_tryGetCellAt(columnIndex, 0, this$.Values))), rangeDouble(0, 1, this$.ColumnCount - 1))));
        const rows = initialize(rowCount, (_arg) => row);
        this$.AddRows(rows, unwrap(index));
    }
    static addRowsEmpty(rowCount, index) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddRowsEmpty(rowCount, unwrap(index));
            return newTable;
        };
    }
    RemoveRow(index) {
        const this$ = this;
        SanityChecks_validateRowIndex(index, this$.RowCount, false);
        Unchecked_removeRowCells_withIndexChange(index, this$.ColumnCount, this$.RowCount, this$.Values);
    }
    static removeRow(index) {
        return (table) => {
            const newTable = table.Copy();
            newTable.RemoveRow(index);
            return newTable;
        };
    }
    RemoveRows(indexArr) {
        const this$ = this;
        indexArr.forEach((index) => {
            SanityChecks_validateRowIndex(index, this$.RowCount, false);
        });
        const indexArr_1 = sortDescending(indexArr, {
            Compare: comparePrimitives,
        });
        indexArr_1.forEach((index_1) => {
            this$.RemoveRow(index_1);
        });
    }
    static removeRows(indexArr) {
        return (table) => {
            const newTable = table.Copy();
            newTable.RemoveColumns(indexArr);
            return newTable;
        };
    }
    GetRow(rowIndex, SkipValidation) {
        const this$ = this;
        if (!equals(SkipValidation, true)) {
            SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        }
        return toArray(delay(() => map((columnIndex) => value_3(this$.TryGetCellAt(columnIndex, rowIndex)), rangeDouble(0, 1, this$.ColumnCount - 1))));
    }
    static getRow(index) {
        return (table) => table.GetRow(index);
    }
    Join(table, index, joinOptions, forceReplace, skipFillMissing) {
        const this$ = this;
        const joinOptions_1 = defaultArg(joinOptions, "headers");
        const forceReplace_1 = defaultArg(forceReplace, false);
        const skipFillMissing_1 = defaultArg(skipFillMissing, false);
        let index_1 = defaultArg(index, this$.ColumnCount);
        index_1 = (((index_1 === -1) ? this$.ColumnCount : index_1) | 0);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        const onlyHeaders = joinOptions_1 === "headers";
        let columns;
        const pre = table.Columns;
        columns = ((joinOptions_1 === "withUnit") ? map_1((c_1) => {
            const unitsOpt = c_1.TryGetColumnUnits();
            if (unitsOpt == null) {
                return new CompositeColumn(c_1.Header, []);
            }
            else {
                return new CompositeColumn(c_1.Header, map_1((u) => CompositeCell.createUnitized("", u), unitsOpt));
            }
        }, pre) : ((joinOptions_1 === "withValues") ? pre : map_1((c) => (new CompositeColumn(c.Header, [])), pre)));
        const duplicates = tryFindDuplicateUniqueInArray(map((x) => x.Header, columns));
        if (!isEmpty(duplicates)) {
            const sb = StringBuilder_$ctor();
            StringBuilder__AppendLine_Z721C83C5(sb, "Found duplicate unique columns in `columns`.");
            iterate_1((x_1) => {
                StringBuilder__AppendLine_Z721C83C5(sb, `Duplicate \`${x_1.HeaderType}\` at index ${x_1.Index1} and ${x_1.Index2}.`);
            }, duplicates);
            throw new Error(toString(sb));
        }
        columns.forEach((x_2) => {
            SanityChecks_validateColumn(x_2);
        });
        columns.forEach((col) => {
            const prevHeadersCount = this$.Headers.length | 0;
            Unchecked_addColumn(col.Header, col.Cells, index_1, forceReplace_1, onlyHeaders, this$.Headers, this$.Values);
            if (this$.Headers.length > prevHeadersCount) {
                index_1 = ((index_1 + 1) | 0);
            }
        });
        if (!skipFillMissing_1) {
            Unchecked_fillMissingCells(this$.Headers, this$.Values);
        }
    }
    static join(table, index, joinOptions, forceReplace) {
        return (this$) => {
            const copy = this$.Copy();
            copy.Join(table, unwrap(index), unwrap(joinOptions), unwrap(forceReplace));
            return copy;
        };
    }
    AddProtocolTypeColumn(types, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((Item) => (new CompositeCell(0, [Item])), array), types);
        this$.AddColumn(new CompositeHeader(4, []), unwrap(cells), unwrap(index));
    }
    AddProtocolVersionColumn(versions, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((Item) => (new CompositeCell(1, [Item])), array), versions);
        this$.AddColumn(new CompositeHeader(7, []), unwrap(cells), unwrap(index));
    }
    AddProtocolUriColumn(uris, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((Item) => (new CompositeCell(1, [Item])), array), uris);
        this$.AddColumn(new CompositeHeader(6, []), unwrap(cells), unwrap(index));
    }
    AddProtocolDescriptionColumn(descriptions, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((Item) => (new CompositeCell(1, [Item])), array), descriptions);
        this$.AddColumn(new CompositeHeader(5, []), unwrap(cells), unwrap(index));
    }
    AddProtocolNameColumn(names, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((Item) => (new CompositeCell(1, [Item])), array), names);
        this$.AddColumn(new CompositeHeader(8, []), unwrap(cells), unwrap(index));
    }
    GetProtocolTypeColumn() {
        const this$ = this;
        return this$.GetColumnByHeader(new CompositeHeader(4, []));
    }
    GetProtocolVersionColumn() {
        const this$ = this;
        return this$.GetColumnByHeader(new CompositeHeader(7, []));
    }
    GetProtocolUriColumn() {
        const this$ = this;
        return this$.GetColumnByHeader(new CompositeHeader(6, []));
    }
    GetProtocolDescriptionColumn() {
        const this$ = this;
        return this$.GetColumnByHeader(new CompositeHeader(5, []));
    }
    GetProtocolNameColumn() {
        const this$ = this;
        return this$.GetColumnByHeader(new CompositeHeader(8, []));
    }
    TryGetProtocolNameColumn() {
        const this$ = this;
        return this$.TryGetColumnByHeader(new CompositeHeader(8, []));
    }
    GetComponentColumns() {
        const this$ = this;
        return map_1((h_1) => this$.GetColumnByHeader(h_1), toArray(filter((h) => h.isComponent, this$.Headers)));
    }
    static SplitByColumnValues(columnIndex) {
        return (table) => mapIndexed((i, indexGroup) => {
            let headers;
            const collection = table.Headers;
            headers = Array.from(collection);
            const rows = map_1((i_1) => table.GetRow(i_1, true), indexGroup);
            return ArcTable.createFromRows(table.Name, headers, rows);
        }, map_1((tupledArg) => map_1((tuple_1) => tuple_1[0], tupledArg[1], Int32Array), Array_groupBy((tuple) => tuple[1], indexed(table.GetColumn(columnIndex).Cells), {
            Equals: equals,
            GetHashCode: safeHash,
        })));
    }
    static SplitByColumnValuesByHeader(header) {
        return (table) => {
            const index = tryFindIndex((x) => equals(x, header), table.Headers);
            if (index == null) {
                return [table.Copy()];
            }
            else {
                const i = index | 0;
                return ArcTable.SplitByColumnValues(i)(table);
            }
        };
    }
    static get SplitByProtocolREF() {
        return (table) => ArcTable.SplitByColumnValuesByHeader(new CompositeHeader(8, []))(table);
    }
    static updateReferenceByAnnotationTable(refTable, annotationTable) {
        const refTable_1 = refTable.Copy();
        const annotationTable_1 = annotationTable.Copy();
        const nonProtocolColumns = toArray(choose((tupledArg) => {
            if (tupledArg[1].isProtocolColumn) {
                return undefined;
            }
            else {
                return tupledArg[0];
            }
        }, indexed_1(refTable_1.Headers)));
        refTable_1.RemoveColumns(nonProtocolColumns);
        Unchecked_extendToRowCount(annotationTable_1.RowCount, refTable_1.Headers, refTable_1.Values);
        const arr = annotationTable_1.Columns;
        for (let idx = 0; idx <= (arr.length - 1); idx++) {
            const c = item(idx, arr);
            refTable_1.AddColumn(c.Header, c.Cells, undefined, true);
        }
        return refTable_1;
    }
    static append(table1, table2) {
        const getList = (t) => toList(delay(() => map((row) => toList(delay(() => map((col) => [t.Headers[col], getItemFromDict(t.Values, [col, row])], rangeDouble(0, 1, t.ColumnCount - 1)))), rangeDouble(0, 1, t.RowCount - 1))));
        const patternInput = Unchecked_alignByHeaders(false, append(getList(table1), getList(table2)));
        return ArcTable.create(table1.Name, patternInput[0], patternInput[1]);
    }
    toString() {
        const this$ = this;
        const rowCount = this$.RowCount | 0;
        return join("\n", toList(delay(() => append_1(singleton(`Table: ${this$.Name}`), delay(() => append_1(singleton("-------------"), delay(() => append_1(singleton(join("\t|\t", map(toString, this$.Headers))), delay(() => ((rowCount > 50) ? append_1(map((rowI) => join("\t|\t", map(toString, this$.GetRow(rowI))), rangeDouble(0, 1, 19)), delay(() => append_1(singleton("..."), delay(() => map((rowI_1) => join("\t|\t", map(toString, this$.GetRow(rowI_1))), rangeDouble(rowCount - 20, 1, rowCount - 1)))))) : ((rowCount === 0) ? singleton("No rows") : map((rowI_2) => join("\t|\t", map(toString, this$.GetRow(rowI_2))), rangeDouble(0, 1, rowCount - 1)))))))))))));
    }
    StructurallyEquals(other) {
        let a, b;
        const this$ = this;
        const sort = (arg) => sortBy((_arg) => _arg[0], Array.from(arg), {
            Compare: compareArrays,
        });
        if ((this$.Name === other.Name) && ((a = this$.Headers, (b = other.Headers, (length(a) === length(b)) && fold((acc, e) => {
            if (acc) {
                return e;
            }
            else {
                return false;
            }
        }, true, toList(delay(() => map((i) => equals(item_1(i, a), item_1(i, b)), rangeDouble(0, 1, length(a) - 1))))))))) {
            const a_1 = sort(this$.Values);
            const b_1 = sort(other.Values);
            return (length(a_1) === length(b_1)) && fold((acc_1, e_1) => {
                if (acc_1) {
                    return e_1;
                }
                else {
                    return false;
                }
            }, true, toList(delay(() => map((i_1) => equals(item_1(i_1, a_1), item_1(i_1, b_1)), rangeDouble(0, 1, length(a_1) - 1)))));
        }
        else {
            return false;
        }
    }
    ReferenceEquals(other) {
        const this$ = this;
        return this$ === other;
    }
    Equals(other) {
        let table;
        const this$ = this;
        return (other instanceof ArcTable) && ((table = other, this$.StructurallyEquals(table)));
    }
    GetHashCode() {
        const this$ = this;
        const vHash = boxHashValues(this$.ColumnCount, this$.Values);
        return boxHashArray([this$.Name, boxHashSeq(this$.Headers), vHash]) | 0;
    }
}

export function ArcTable_$reflection() {
    return class_type("ARCtrl.ArcTable", undefined, ArcTable);
}

export function ArcTable_$ctor_76CAD84E(name, headers, values) {
    return new ArcTable(name, headers, values);
}

