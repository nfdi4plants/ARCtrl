import { Dictionary } from "../../../fable_modules/fable-library.4.1.4/MutableMap.js";
import { stringHash, compareArrays, safeHash, disposeSafe, getEnumerator, equals, comparePrimitives, arrayHash, equalArrays } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map as map_2, value as value_9, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { ProcessParsing_processToRows, ProcessParsing_alignByHeaders, ProcessParsing_getProcessGetter, Unchecked_removeRowCells_withIndexChange, Unchecked_addRow, Unchecked_getEmptyCellForHeader, Unchecked_removeColumnCells_withIndexChange, tryFindDuplicateUniqueInArray, Unchecked_removeColumnCells, Unchecked_removeHeader, Unchecked_fillMissingCells, Unchecked_addColumn, tryFindDuplicateUnique, Unchecked_setCellAt, SanityChecks_validateColumn, SanityChecks_validateRowIndex, SanityChecks_validateColumnIndex, Unchecked_tryGetCellAt, getRowCount, getColumnCount } from "./ArcTableAux.js";
import { sortBy, forAll2, append, zip, fold, filter, length, singleton, collect, findIndex, toArray, removeAt, map, delay, toList } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { rangeDouble } from "../../../fable_modules/fable-library.4.1.4/Range.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { singleton as singleton_1, initialize, sortDescending, iterateIndexed, map as map_1 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { collect as collect_1, initialize as initialize_1, singleton as singleton_2, empty, iterate, isEmpty } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { StringBuilder__AppendLine_Z721C83C5, StringBuilder_$ctor } from "../../../fable_modules/fable-library.4.1.4/System.Text.js";
import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { CompositeCell_$reflection, CompositeCell } from "./CompositeCell.js";
import { CompositeHeader_$reflection, CompositeHeader } from "./CompositeHeader.js";
import { ISA_Component__Component_TryGetColumnIndex, ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex } from "../JsonTypes/ColumnIndex.js";
import { Protocol_get_empty, Protocol_addComponent, Protocol_addParameter, Protocol_setName, Protocol_setDescription, Protocol_setUri, Protocol_setVersion, Protocol_setProtocolType } from "../JsonTypes/Protocol.js";
import { OntologyAnnotation_get_empty } from "../JsonTypes/OntologyAnnotation.js";
import { ProtocolParameter_create_2769312B } from "../JsonTypes/ProtocolParameter.js";
import { Component_create_Z33AADEE0 } from "../JsonTypes/Component.js";
import { List_distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { toProtocol } from "./CompositeRow.js";
import { join } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { record_type, class_type, tuple_type, int32_type, array_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class ArcTable extends Record {
    constructor(Name, Headers, Values) {
        super();
        this.Name = Name;
        this.Headers = Headers;
        this.Values = Values;
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
    Validate(raiseException) {
        const this$ = this;
        let isValid = true;
        for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
            const column = this$.GetColumn(columnIndex);
            isValid = column.validate(unwrap(raiseException));
        }
        return isValid;
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
        return toList(delay(() => map((i) => this$.GetColumn(i), rangeDouble(0, 1, this$.ColumnCount - 1))));
    }
    Copy() {
        const this$ = this;
        return ArcTable.create(this$.Name, Array.from(this$.Headers), new Dictionary(this$.Values, {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
    }
    TryGetCellAt(column, row) {
        const this$ = this;
        return Unchecked_tryGetCellAt(column, row, this$.Values);
    }
    static tryGetCellAt(column, row) {
        return (table) => table.TryGetCellAt(column, row);
    }
    UpdateCellAt(columnIndex, rowIndex, c) {
        const this$ = this;
        SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
        SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        SanityChecks_validateColumn(CompositeColumn.create(this$.Headers[columnIndex], [c]));
        Unchecked_setCellAt(columnIndex, rowIndex, c, this$.Values);
    }
    static updateCellAt(columnIndex, rowIndex, cell) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateCellAt(columnIndex, rowIndex, cell);
            return newTable;
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
        if (c.validate()) {
            let setHeader;
            setHeader = (this$.Headers[index] = newHeader);
        }
        else if (forceConvertCells_1) {
            const convertedCells = newHeader.IsTermColumn ? map_1((c_1) => c_1.ToTermCell(), c.Cells) : map_1((c_2) => c_2.ToFreeTextCell(), c.Cells);
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
    AddColumn(header, cells, index, forceReplace) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.ColumnCount) | 0;
        const cells_1 = defaultArg(cells, []);
        const forceReplace_1 = defaultArg(forceReplace, false);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        SanityChecks_validateColumn(CompositeColumn.create(header, cells_1));
        Unchecked_addColumn(header, cells_1, index_1, forceReplace_1, this$.Headers, this$.Values);
        Unchecked_fillMissingCells(this$.Headers, this$.Values);
    }
    static addColumn(header, cells, index, forceReplace) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddColumn(header, unwrap(cells), unwrap(index), unwrap(forceReplace));
            return newTable;
        };
    }
    UpdateColumn(columnIndex, header, cells) {
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
        Unchecked_fillMissingCells(this$.Headers, this$.Values);
    }
    static updatetColumn(columnIndex, header, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.UpdateColumn(columnIndex, header, unwrap(cells));
            return newTable;
        };
    }
    InsertColumn(header, index, cells) {
        const this$ = this;
        this$.AddColumn(header, unwrap(cells), index, false);
    }
    static insertColumn(header, index, cells) {
        return (table) => {
            const newTable = table.Copy();
            newTable.InsertColumn(header, index, unwrap(cells));
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
    AddColumns(columns, index, forceReplace) {
        const this$ = this;
        let index_1 = defaultArg(index, this$.ColumnCount);
        const forceReplace_1 = defaultArg(forceReplace, false);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        const duplicates = tryFindDuplicateUniqueInArray(map((x) => x.Header, columns));
        if (!isEmpty(duplicates)) {
            const sb = StringBuilder_$ctor();
            StringBuilder__AppendLine_Z721C83C5(sb, "Found duplicate unique columns in `columns`.");
            iterate((x_1) => {
                StringBuilder__AppendLine_Z721C83C5(sb, `Duplicate \`${x_1.HeaderType}\` at index ${x_1.Index1} and ${x_1.Index2}.`);
            }, duplicates);
            throw new Error(toString(sb));
        }
        columns.forEach((x_2) => {
            SanityChecks_validateColumn(x_2);
        });
        columns.forEach((col) => {
            const prevHeadersCount = this$.Headers.length | 0;
            Unchecked_addColumn(col.Header, col.Cells, index_1, forceReplace_1, this$.Headers, this$.Values);
            if (this$.Headers.length > prevHeadersCount) {
                index_1 = ((index_1 + 1) | 0);
            }
        });
        Unchecked_fillMissingCells(this$.Headers, this$.Values);
    }
    static addColumns(columns, index) {
        return (table) => {
            const newTable = table.Copy();
            newTable.AddColumns(columns, index);
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
        const cells = toArray(delay(() => map((i) => value_9(this$.TryGetCellAt(columnIndex, i)), rangeDouble(0, 1, this$.RowCount - 1))));
        return CompositeColumn.create(h, cells);
    }
    static getColumn(index) {
        return (table) => table.GetColumn(index);
    }
    GetColumnByHeader(header) {
        const this$ = this;
        const index = findIndex((x) => equals(x, header), this$.Headers) | 0;
        return this$.GetColumn(index);
    }
    AddRow(cells, index) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.RowCount) | 0;
        const cells_1 = (cells == null) ? toArray(delay(() => collect((columnIndex) => singleton(Unchecked_getEmptyCellForHeader(this$.Headers[columnIndex], Unchecked_tryGetCellAt(columnIndex, 0, this$.Values))), rangeDouble(0, 1, this$.ColumnCount - 1)))) : value_9(cells);
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
            SanityChecks_validateColumn(CompositeColumn.create(h_1, [cells_1[columnIndex_1]]));
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
            const row_1 = rows[idx];
            for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
                const h = this$.Headers[columnIndex];
                SanityChecks_validateColumn(CompositeColumn.create(h, [row_1[columnIndex]]));
            }
        }
        rows.forEach((row_2) => {
            Unchecked_addRow(index_1, row_2, this$.Headers, this$.Values);
            index_1 = ((index_1 + 1) | 0);
        });
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
        const removeCells = Unchecked_removeRowCells_withIndexChange(index, this$.ColumnCount, this$.RowCount, this$.Values);
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
    GetRow(rowIndex) {
        const this$ = this;
        SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        return toArray(delay(() => map((columnIndex) => value_9(this$.TryGetCellAt(columnIndex, rowIndex)), rangeDouble(0, 1, this$.ColumnCount - 1))));
    }
    static getRow(index) {
        return (table) => table.GetRow(index);
    }
    static insertParameterValue(t, p) {
        throw new Error();
    }
    static getParameterValues(t) {
        throw new Error();
    }
    AddProtocolTypeColumn(types, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((arg) => (new CompositeCell(0, [arg])), array), types);
        this$.AddColumn(new CompositeHeader(4, []), cells, index);
    }
    AddProtocolVersionColumn(versions, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((arg) => (new CompositeCell(1, [arg])), array), versions);
        this$.AddColumn(new CompositeHeader(7, []), cells, index);
    }
    AddProtocolUriColumn(uris, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((arg) => (new CompositeCell(1, [arg])), array), uris);
        this$.AddColumn(new CompositeHeader(6, []), cells, index);
    }
    AddProtocolDescriptionColumn(descriptions, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((arg) => (new CompositeCell(1, [arg])), array), descriptions);
        this$.AddColumn(new CompositeHeader(5, []), cells, index);
    }
    AddProtocolNameColumn(names, index) {
        const this$ = this;
        const cells = map_2((array) => map_1((arg) => (new CompositeCell(1, [arg])), array), names);
        this$.AddColumn(new CompositeHeader(8, []), cells, index);
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
    GetComponentColumns() {
        const this$ = this;
        return map_1((h_1) => this$.GetColumnByHeader(h_1), toArray(filter((h) => h.isComponent, this$.Headers)));
    }
    static fromProtocol(p) {
        const t = ArcTable.init(defaultArg(p.Name, ""));
        const enumerator = getEnumerator(defaultArg(p.Parameters, empty()));
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const pp = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                t.AddColumn(new CompositeHeader(3, [value_9(pp.ParameterName)]), void 0, ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(pp));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        const enumerator_1 = getEnumerator(defaultArg(p.Components, empty()));
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const c = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const v_1 = map_2((arg) => singleton_1(CompositeCell.fromValue(arg, unwrap(c.ComponentUnit))), c.ComponentValue);
                t.AddColumn(new CompositeHeader(3, [value_9(c.ComponentType)]), v_1, ISA_Component__Component_TryGetColumnIndex(c));
            }
        }
        finally {
            disposeSafe(enumerator_1);
        }
        map_2((d) => {
            t.AddProtocolDescriptionColumn([d]);
        }, p.Description);
        map_2((d_1) => {
            t.AddProtocolVersionColumn([d_1]);
        }, p.Version);
        map_2((d_2) => {
            t.AddProtocolTypeColumn([d_2]);
        }, p.ProtocolType);
        map_2((d_3) => {
            t.AddProtocolUriColumn([d_3]);
        }, p.Uri);
        map_2((d_4) => {
            t.AddProtocolNameColumn([d_4]);
        }, p.Name);
        return t;
    }
    GetProtocols() {
        const this$ = this;
        return (this$.RowCount === 0) ? singleton_2(fold((p, h) => {
            switch (h.tag) {
                case 4:
                    return Protocol_setProtocolType(p, OntologyAnnotation_get_empty());
                case 7:
                    return Protocol_setVersion(p, "");
                case 6:
                    return Protocol_setUri(p, "");
                case 5:
                    return Protocol_setDescription(p, "");
                case 8:
                    return Protocol_setName(p, "");
                case 3:
                    return Protocol_addParameter(ProtocolParameter_create_2769312B(void 0, h.fields[0]), p);
                case 0:
                    return Protocol_addComponent(Component_create_Z33AADEE0(void 0, void 0, void 0, h.fields[0]), p);
                default:
                    return p;
            }
        }, Protocol_get_empty(), this$.Headers)) : List_distinct(initialize_1(this$.RowCount, (i) => toProtocol(zip(this$.Headers, this$.GetRow(i)))), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    GetProcesses() {
        const this$ = this;
        let getter;
        const clo = ProcessParsing_getProcessGetter(this$.Name, this$.Headers);
        getter = ((arg) => {
            const clo_1 = clo(arg);
            return clo_1;
        });
        return toList(delay(() => map((i) => getter(this$.Values)(i), rangeDouble(0, 1, this$.RowCount - 1))));
    }
    static fromProcesses(name, ps) {
        const tupledArg = ProcessParsing_alignByHeaders(collect_1(ProcessParsing_processToRows, ps));
        return ArcTable.create(name, tupledArg[0], tupledArg[1]);
    }
    toString() {
        const this$ = this;
        return join("\n", toList(delay(() => append(singleton(`Table: ${this$.Name}`), delay(() => append(singleton("-------------"), delay(() => append(singleton(join("\t|\t", map(toString, this$.Headers))), delay(() => map((rowI) => join("\t|\t", map(toString, this$.GetRow(rowI))), rangeDouble(0, 1, this$.RowCount - 1)))))))))));
    }
    Equals(other) {
        const this$ = this;
        if (other instanceof ArcTable) {
            const table = other;
            const sameName = this$.Name === table.Name;
            const h1 = this$.Headers;
            const h2 = table.Headers;
            const sameHeaderLength = h1.length === h2.length;
            const sameHeaders = forAll2(equals, h1, h2);
            const b1 = sortBy((kv) => kv[0], this$.Values, {
                Compare: compareArrays,
            });
            const b2 = sortBy((kv_1) => kv_1[0], table.Values, {
                Compare: compareArrays,
            });
            return (((sameName && sameHeaderLength) && sameHeaders) && (length(b1) === length(b2))) && forAll2(equals, b1, b2);
        }
        else {
            return false;
        }
    }
    GetHashCode() {
        const this$ = this;
        return ((stringHash(this$.Name) + fold((state, ele) => (state + safeHash(ele)), 0, this$.Headers)) + fold((state_2, ele_1) => (state_2 + safeHash(ele_1)), 0, this$.Values)) | 0;
    }
}

export function ArcTable_$reflection() {
    return record_type("ISA.ArcTable", [], ArcTable, () => [["Name", string_type], ["Headers", array_type(CompositeHeader_$reflection())], ["Values", class_type("System.Collections.Generic.Dictionary`2", [tuple_type(int32_type, int32_type), CompositeCell_$reflection()])]]);
}

