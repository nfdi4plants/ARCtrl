import { CompositeHeader_$reflection, CompositeHeader_Parameter, CompositeHeader_ProtocolREF, CompositeHeader_ProtocolDescription, CompositeHeader_ProtocolUri, CompositeHeader_ProtocolVersion, CompositeHeader_ProtocolType, CompositeHeader_$union } from "./CompositeHeader.js";
import { stringHash, compareArrays, safeHash, IDisposable, disposeSafe, IEnumerator, getEnumerator, equals, comparePrimitives, arrayHash, equalArrays, IMap } from "../../../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { CompositeCell_$reflection, CompositeCell, CompositeCell_FreeText, CompositeCell_Term, CompositeCell_$union } from "./CompositeCell.js";
import { Dictionary } from "../../../fable_modules/fable-library-ts/MutableMap.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { map as map_2, value as value_9, defaultArg, Option, unwrap } from "../../../fable_modules/fable-library-ts/Option.js";
import { ProcessParsing_processToRows, ProcessParsing_alignByHeaders, ProcessParsing_getProcessGetter, Unchecked_removeRowCells_withIndexChange, Unchecked_addRow, Unchecked_getEmptyCellForHeader, Unchecked_removeColumnCells_withIndexChange, tryFindDuplicateUniqueInArray, Unchecked_removeColumnCells, Unchecked_removeHeader, Unchecked_fillMissingCells, Unchecked_addColumn, tryFindDuplicateUnique, Unchecked_setCellAt, SanityChecks_validateColumn, SanityChecks_validateRowIndex, SanityChecks_validateColumnIndex, Unchecked_tryGetCellAt, getRowCount, getColumnCount } from "./ArcTableAux.js";
import { sortBy, forAll2, append, zip, fold, filter, length, singleton, collect, findIndex, toArray, removeAt, map, delay, toList } from "../../../fable_modules/fable-library-ts/Seq.js";
import { rangeDouble } from "../../../fable_modules/fable-library-ts/Range.js";
import { collect as collect_1, initialize as initialize_1, singleton as singleton_2, empty, iterate, isEmpty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { singleton as singleton_1, initialize, sortDescending, iterateIndexed, map as map_1 } from "../../../fable_modules/fable-library-ts/Array.js";
import { StringBuilder__AppendLine_Z721C83C5, StringBuilder_$ctor } from "../../../fable_modules/fable-library-ts/System.Text.js";
import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { ProcessParameterValue } from "../JsonTypes/ProcessParameterValue.js";
import { OntologyAnnotation_get_empty, OntologyAnnotation } from "../JsonTypes/OntologyAnnotation.js";
import { ProtocolParameter_create_2769312B, ProtocolParameter } from "../JsonTypes/ProtocolParameter.js";
import { ISA_Component__Component_TryGetColumnIndex, ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex } from "../JsonTypes/ColumnIndex.js";
import { Component_create_Z33AADEE0, Component } from "../JsonTypes/Component.js";
import { Value_$union } from "../JsonTypes/Value.js";
import { Protocol_get_empty, Protocol_addComponent, Protocol_addParameter, Protocol_setName, Protocol_setDescription, Protocol_setUri, Protocol_setVersion, Protocol_setProtocolType, Protocol } from "../JsonTypes/Protocol.js";
import { List_distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { toProtocol } from "./CompositeRow.js";
import { Process } from "../JsonTypes/Process.js";
import { join } from "../../../fable_modules/fable-library-ts/String.js";
import { record_type, class_type, tuple_type, int32_type, array_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class ArcTable extends Record {
    readonly Name: string;
    readonly Headers: CompositeHeader_$union[];
    readonly Values: IMap<[int32, int32], CompositeCell_$union>;
    constructor(Name: string, Headers: CompositeHeader_$union[], Values: IMap<[int32, int32], CompositeCell_$union>) {
        super();
        this.Name = Name;
        this.Headers = Headers;
        this.Values = Values;
    }
    static create(name: string, headers: CompositeHeader_$union[], values: IMap<[int32, int32], CompositeCell_$union>): ArcTable {
        return new ArcTable(name, headers, values);
    }
    static init(name: string): ArcTable {
        return new ArcTable(name, [], new Dictionary<[int32, int32], CompositeCell_$union>([], {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
    }
    Validate(raiseException?: boolean): boolean {
        const this$: ArcTable = this;
        let isValid = true;
        for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
            const column: CompositeColumn = this$.GetColumn(columnIndex);
            isValid = column.validate(unwrap(raiseException));
        }
        return isValid;
    }
    static validate(raiseException?: boolean): ((arg0: ArcTable) => boolean) {
        return (table: ArcTable): boolean => table.Validate(unwrap(raiseException));
    }
    get ColumnCount(): int32 {
        const this$: ArcTable = this;
        return getColumnCount(this$.Headers) | 0;
    }
    get RowCount(): int32 {
        const this$: ArcTable = this;
        return getRowCount(this$.Values) | 0;
    }
    get Columns(): FSharpList<CompositeColumn> {
        const this$: ArcTable = this;
        return toList<CompositeColumn>(delay<CompositeColumn>((): Iterable<CompositeColumn> => map<int32, CompositeColumn>((i: int32): CompositeColumn => this$.GetColumn(i), rangeDouble(0, 1, this$.ColumnCount - 1))));
    }
    Copy(): ArcTable {
        const this$: ArcTable = this;
        return ArcTable.create(this$.Name, Array.from(this$.Headers), new Dictionary<[int32, int32], CompositeCell_$union>(this$.Values, {
            Equals: equalArrays,
            GetHashCode: arrayHash,
        }));
    }
    TryGetCellAt(column: int32, row: int32): Option<CompositeCell_$union> {
        const this$: ArcTable = this;
        return Unchecked_tryGetCellAt(column, row, this$.Values);
    }
    static tryGetCellAt(column: int32, row: int32): ((arg0: ArcTable) => Option<CompositeCell_$union>) {
        return (table: ArcTable): Option<CompositeCell_$union> => table.TryGetCellAt(column, row);
    }
    UpdateCellAt(columnIndex: int32, rowIndex: int32, c: CompositeCell_$union): void {
        const this$: ArcTable = this;
        SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
        SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        SanityChecks_validateColumn(CompositeColumn.create(this$.Headers[columnIndex], [c]));
        Unchecked_setCellAt(columnIndex, rowIndex, c, this$.Values);
    }
    static updateCellAt(columnIndex: int32, rowIndex: int32, cell: CompositeCell_$union): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.UpdateCellAt(columnIndex, rowIndex, cell);
            return newTable;
        };
    }
    UpdateHeader(index: int32, newHeader: CompositeHeader_$union, forceConvertCells?: boolean): void {
        const this$: ArcTable = this;
        const forceConvertCells_1: boolean = defaultArg(forceConvertCells, false);
        SanityChecks_validateColumnIndex(index, this$.ColumnCount, false);
        const header: CompositeHeader_$union = newHeader;
        const matchValue: Option<int32> = tryFindDuplicateUnique(header, removeAt<CompositeHeader_$union>(index, this$.Headers));
        if (matchValue != null) {
            throw new Error(`Invalid input. Tried setting unique header \`${header}\`, but header of same type already exists at index ${value_9(matchValue)}.`);
        }
        const c: CompositeColumn = new CompositeColumn(newHeader, this$.GetColumn(index).Cells);
        if (c.validate()) {
            let setHeader: any;
            setHeader = (this$.Headers[index] = newHeader);
        }
        else if (forceConvertCells_1) {
            const convertedCells: CompositeCell_$union[] = newHeader.IsTermColumn ? map_1<CompositeCell_$union, CompositeCell_$union>((c_1: CompositeCell_$union): CompositeCell_$union => c_1.ToTermCell(), c.Cells) : map_1<CompositeCell_$union, CompositeCell_$union>((c_2: CompositeCell_$union): CompositeCell_$union => c_2.ToFreeTextCell(), c.Cells);
            this$.UpdateColumn(index, newHeader, convertedCells);
        }
        else {
            throw new Error("Tried setting header for column with invalid type of cells. Set `forceConvertCells` flag to automatically convert cells into valid CompositeCell type.");
        }
    }
    static updateHeader(index: int32, header: CompositeHeader_$union): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.UpdateHeader(index, header);
            return newTable;
        };
    }
    AddColumn(header: CompositeHeader_$union, cells?: CompositeCell_$union[], index?: int32, forceReplace?: boolean): void {
        const this$: ArcTable = this;
        const index_1: int32 = defaultArg<int32>(index, this$.ColumnCount) | 0;
        const cells_1: CompositeCell_$union[] = defaultArg<CompositeCell_$union[]>(cells, []);
        const forceReplace_1: boolean = defaultArg<boolean>(forceReplace, false);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        SanityChecks_validateColumn(CompositeColumn.create(header, cells_1));
        Unchecked_addColumn(header, cells_1, index_1, forceReplace_1, this$.Headers, this$.Values);
        Unchecked_fillMissingCells(this$.Headers, this$.Values);
    }
    static addColumn(header: CompositeHeader_$union, cells?: CompositeCell_$union[], index?: int32, forceReplace?: boolean): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AddColumn(header, unwrap(cells), unwrap(index), unwrap(forceReplace));
            return newTable;
        };
    }
    UpdateColumn(columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): void {
        const this$: ArcTable = this;
        SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
        const column: CompositeColumn = CompositeColumn.create(header, unwrap(cells));
        SanityChecks_validateColumn(column);
        const header_1: CompositeHeader_$union = column.Header;
        const matchValue: Option<int32> = tryFindDuplicateUnique(header_1, removeAt<CompositeHeader_$union>(columnIndex, this$.Headers));
        if (matchValue != null) {
            throw new Error(`Invalid input. Tried setting unique header \`${header_1}\`, but header of same type already exists at index ${value_9(matchValue)}.`);
        }
        Unchecked_removeHeader(columnIndex, this$.Headers);
        Unchecked_removeColumnCells(columnIndex, this$.Values);
        this$.Headers.splice(columnIndex, 0, column.Header);
        iterateIndexed<CompositeCell_$union>((rowIndex: int32, v: CompositeCell_$union): void => {
            Unchecked_setCellAt(columnIndex, rowIndex, v, this$.Values);
        }, column.Cells);
        Unchecked_fillMissingCells(this$.Headers, this$.Values);
    }
    static updatetColumn(columnIndex: int32, header: CompositeHeader_$union, cells?: CompositeCell_$union[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.UpdateColumn(columnIndex, header, unwrap(cells));
            return newTable;
        };
    }
    InsertColumn(header: CompositeHeader_$union, index: int32, cells?: CompositeCell_$union[]): void {
        const this$: ArcTable = this;
        this$.AddColumn(header, unwrap(cells), index, false);
    }
    static insertColumn(header: CompositeHeader_$union, index: int32, cells?: CompositeCell_$union[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.InsertColumn(header, index, unwrap(cells));
            return newTable;
        };
    }
    AppendColumn(header: CompositeHeader_$union, cells?: CompositeCell_$union[]): void {
        const this$: ArcTable = this;
        this$.AddColumn(header, unwrap(cells), this$.ColumnCount, false);
    }
    static appendColumn(header: CompositeHeader_$union, cells?: CompositeCell_$union[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AppendColumn(header, unwrap(cells));
            return newTable;
        };
    }
    AddColumns(columns: CompositeColumn[], index?: int32, forceReplace?: boolean): void {
        const this$: ArcTable = this;
        let index_1: int32 = defaultArg<int32>(index, this$.ColumnCount);
        const forceReplace_1: boolean = defaultArg<boolean>(forceReplace, false);
        SanityChecks_validateColumnIndex(index_1, this$.ColumnCount, true);
        const duplicates: FSharpList<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }> = tryFindDuplicateUniqueInArray(map<CompositeColumn, CompositeHeader_$union>((x: CompositeColumn): CompositeHeader_$union => x.Header, columns));
        if (!isEmpty(duplicates)) {
            const sb: any = StringBuilder_$ctor();
            StringBuilder__AppendLine_Z721C83C5(sb, "Found duplicate unique columns in `columns`.");
            iterate<{ HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }>((x_1: { HeaderType: CompositeHeader_$union, Index1: int32, Index2: int32 }): void => {
                StringBuilder__AppendLine_Z721C83C5(sb, `Duplicate \`${x_1.HeaderType}\` at index ${x_1.Index1} and ${x_1.Index2}.`);
            }, duplicates);
            throw new Error(toString(sb));
        }
        columns.forEach((x_2: CompositeColumn): void => {
            SanityChecks_validateColumn(x_2);
        });
        columns.forEach((col: CompositeColumn): void => {
            const prevHeadersCount: int32 = this$.Headers.length | 0;
            Unchecked_addColumn(col.Header, col.Cells, index_1, forceReplace_1, this$.Headers, this$.Values);
            if (this$.Headers.length > prevHeadersCount) {
                index_1 = ((index_1 + 1) | 0);
            }
        });
        Unchecked_fillMissingCells(this$.Headers, this$.Values);
    }
    static addColumns(columns: CompositeColumn[], index?: int32): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AddColumns(columns, index);
            return newTable;
        };
    }
    RemoveColumn(index: int32): void {
        const this$: ArcTable = this;
        SanityChecks_validateColumnIndex(index, this$.ColumnCount, false);
        const columnCount: int32 = this$.ColumnCount | 0;
        Unchecked_removeHeader(index, this$.Headers);
        Unchecked_removeColumnCells_withIndexChange(index, columnCount, this$.RowCount, this$.Values);
    }
    static removeColumn(index: int32): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.RemoveColumn(index);
            return newTable;
        };
    }
    RemoveColumns(indexArr: int32[]): void {
        const this$: ArcTable = this;
        indexArr.forEach((index: int32): void => {
            SanityChecks_validateColumnIndex(index, this$.ColumnCount, false);
        });
        const indexArr_1: int32[] = sortDescending<int32>(indexArr, {
            Compare: comparePrimitives,
        });
        indexArr_1.forEach((index_1: int32): void => {
            this$.RemoveColumn(index_1);
        });
    }
    static removeColumns(indexArr: int32[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.RemoveColumns(indexArr);
            return newTable;
        };
    }
    GetColumn(columnIndex: int32): CompositeColumn {
        const this$: ArcTable = this;
        SanityChecks_validateColumnIndex(columnIndex, this$.ColumnCount, false);
        const h: CompositeHeader_$union = this$.Headers[columnIndex];
        const cells: CompositeCell_$union[] = toArray<CompositeCell_$union>(delay<CompositeCell_$union>((): Iterable<CompositeCell_$union> => map<int32, CompositeCell_$union>((i: int32): CompositeCell_$union => value_9(this$.TryGetCellAt(columnIndex, i)), rangeDouble(0, 1, this$.RowCount - 1))));
        return CompositeColumn.create(h, cells);
    }
    static getColumn(index: int32): ((arg0: ArcTable) => CompositeColumn) {
        return (table: ArcTable): CompositeColumn => table.GetColumn(index);
    }
    GetColumnByHeader(header: CompositeHeader_$union): CompositeColumn {
        const this$: ArcTable = this;
        const index: int32 = findIndex<CompositeHeader_$union>((x: CompositeHeader_$union): boolean => equals(x, header), this$.Headers) | 0;
        return this$.GetColumn(index);
    }
    AddRow(cells?: CompositeCell_$union[], index?: int32): void {
        const this$: ArcTable = this;
        const index_1: int32 = defaultArg<int32>(index, this$.RowCount) | 0;
        const cells_1: CompositeCell_$union[] = (cells == null) ? toArray<CompositeCell_$union>(delay<CompositeCell_$union>((): Iterable<CompositeCell_$union> => collect<int32, Iterable<CompositeCell_$union>, CompositeCell_$union>((columnIndex: int32): Iterable<CompositeCell_$union> => singleton<CompositeCell_$union>(Unchecked_getEmptyCellForHeader(this$.Headers[columnIndex], Unchecked_tryGetCellAt(columnIndex, 0, this$.Values))), rangeDouble(0, 1, this$.ColumnCount - 1)))) : value_9(cells);
        SanityChecks_validateRowIndex(index_1, this$.RowCount, true);
        const newCells: Iterable<CompositeCell_$union> = cells_1;
        const columnCount: int32 = this$.ColumnCount | 0;
        const newCellsCount: int32 = length<CompositeCell_$union>(newCells) | 0;
        if (columnCount === 0) {
            throw new Error("Table contains no columns! Cannot add row to empty table!");
        }
        else if (newCellsCount !== columnCount) {
            throw new Error(`Cannot add a new row with ${newCellsCount} cells, as the table has ${columnCount} columns.`);
        }
        for (let columnIndex_1 = 0; columnIndex_1 <= (this$.ColumnCount - 1); columnIndex_1++) {
            const h_1: CompositeHeader_$union = this$.Headers[columnIndex_1];
            SanityChecks_validateColumn(CompositeColumn.create(h_1, [cells_1[columnIndex_1]]));
        }
        Unchecked_addRow(index_1, cells_1, this$.Headers, this$.Values);
    }
    static addRow(cells?: CompositeCell_$union[], index?: int32): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AddRow(unwrap(cells), unwrap(index));
            return newTable;
        };
    }
    UpdateRow(rowIndex: int32, cells: CompositeCell_$union[]): void {
        const this$: ArcTable = this;
        SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        const newCells: Iterable<CompositeCell_$union> = cells;
        const columnCount: int32 = this$.RowCount | 0;
        const newCellsCount: int32 = length<CompositeCell_$union>(newCells) | 0;
        if (columnCount === 0) {
            throw new Error("Table contains no columns! Cannot add row to empty table!");
        }
        else if (newCellsCount !== columnCount) {
            throw new Error(`Cannot add a new row with ${newCellsCount} cells, as the table has ${columnCount} columns.`);
        }
        iterateIndexed<CompositeCell_$union>((i: int32, cell: CompositeCell_$union): void => {
            const h: CompositeHeader_$union = this$.Headers[i];
            SanityChecks_validateColumn(CompositeColumn.create(h, [cell]));
        }, cells);
        iterateIndexed<CompositeCell_$union>((columnIndex: int32, cell_1: CompositeCell_$union): void => {
            Unchecked_setCellAt(columnIndex, rowIndex, cell_1, this$.Values);
        }, cells);
    }
    static updateRow(rowIndex: int32, cells: CompositeCell_$union[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.UpdateRow(rowIndex, cells);
            return newTable;
        };
    }
    AppendRow(cells?: CompositeCell_$union[]): void {
        const this$: ArcTable = this;
        this$.AddRow(unwrap(cells), this$.RowCount);
    }
    static appendRow(cells?: CompositeCell_$union[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AppendRow(unwrap(cells));
            return newTable;
        };
    }
    InsertRow(index: int32, cells?: CompositeCell_$union[]): void {
        const this$: ArcTable = this;
        this$.AddRow(unwrap(cells), index);
    }
    static insertRow(index: int32, cells?: CompositeCell_$union[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AddRow(unwrap(cells), index);
            return newTable;
        };
    }
    AddRows(rows: CompositeCell_$union[][], index?: int32): void {
        const this$: ArcTable = this;
        let index_1: int32 = defaultArg<int32>(index, this$.RowCount);
        SanityChecks_validateRowIndex(index_1, this$.RowCount, true);
        rows.forEach((row: CompositeCell_$union[]): void => {
            const newCells: Iterable<CompositeCell_$union> = row;
            const columnCount: int32 = this$.ColumnCount | 0;
            const newCellsCount: int32 = length<CompositeCell_$union>(newCells) | 0;
            if (columnCount === 0) {
                throw new Error("Table contains no columns! Cannot add row to empty table!");
            }
            else if (newCellsCount !== columnCount) {
                throw new Error(`Cannot add a new row with ${newCellsCount} cells, as the table has ${columnCount} columns.`);
            }
        });
        for (let idx = 0; idx <= (rows.length - 1); idx++) {
            const row_1: CompositeCell_$union[] = rows[idx];
            for (let columnIndex = 0; columnIndex <= (this$.ColumnCount - 1); columnIndex++) {
                const h: CompositeHeader_$union = this$.Headers[columnIndex];
                SanityChecks_validateColumn(CompositeColumn.create(h, [row_1[columnIndex]]));
            }
        }
        rows.forEach((row_2: CompositeCell_$union[]): void => {
            Unchecked_addRow(index_1, row_2, this$.Headers, this$.Values);
            index_1 = ((index_1 + 1) | 0);
        });
    }
    static addRows(rows: CompositeCell_$union[][], index?: int32): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AddRows(rows, unwrap(index));
            return newTable;
        };
    }
    AddRowsEmpty(rowCount: int32, index?: int32): void {
        const this$: ArcTable = this;
        const row: CompositeCell_$union[] = toArray<CompositeCell_$union>(delay<CompositeCell_$union>((): Iterable<CompositeCell_$union> => collect<int32, Iterable<CompositeCell_$union>, CompositeCell_$union>((columnIndex: int32): Iterable<CompositeCell_$union> => singleton<CompositeCell_$union>(Unchecked_getEmptyCellForHeader(this$.Headers[columnIndex], Unchecked_tryGetCellAt(columnIndex, 0, this$.Values))), rangeDouble(0, 1, this$.ColumnCount - 1))));
        const rows: CompositeCell_$union[][] = initialize<CompositeCell_$union[]>(rowCount, (_arg: int32): CompositeCell_$union[] => row);
        this$.AddRows(rows, unwrap(index));
    }
    static addRowsEmpty(rowCount: int32, index?: int32): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.AddRowsEmpty(rowCount, unwrap(index));
            return newTable;
        };
    }
    RemoveRow(index: int32): void {
        const this$: ArcTable = this;
        SanityChecks_validateRowIndex(index, this$.RowCount, false);
        const removeCells: any = Unchecked_removeRowCells_withIndexChange(index, this$.ColumnCount, this$.RowCount, this$.Values);
    }
    static removeRow(index: int32): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.RemoveRow(index);
            return newTable;
        };
    }
    RemoveRows(indexArr: int32[]): void {
        const this$: ArcTable = this;
        indexArr.forEach((index: int32): void => {
            SanityChecks_validateRowIndex(index, this$.RowCount, false);
        });
        const indexArr_1: int32[] = sortDescending<int32>(indexArr, {
            Compare: comparePrimitives,
        });
        indexArr_1.forEach((index_1: int32): void => {
            this$.RemoveRow(index_1);
        });
    }
    static removeRows(indexArr: int32[]): ((arg0: ArcTable) => ArcTable) {
        return (table: ArcTable): ArcTable => {
            const newTable: ArcTable = table.Copy();
            newTable.RemoveColumns(indexArr);
            return newTable;
        };
    }
    GetRow(rowIndex: int32): CompositeCell_$union[] {
        const this$: ArcTable = this;
        SanityChecks_validateRowIndex(rowIndex, this$.RowCount, false);
        return toArray<CompositeCell_$union>(delay<CompositeCell_$union>((): Iterable<CompositeCell_$union> => map<int32, CompositeCell_$union>((columnIndex: int32): CompositeCell_$union => value_9(this$.TryGetCellAt(columnIndex, rowIndex)), rangeDouble(0, 1, this$.ColumnCount - 1))));
    }
    static getRow(index: int32): ((arg0: ArcTable) => CompositeCell_$union[]) {
        return (table: ArcTable): CompositeCell_$union[] => table.GetRow(index);
    }
    static insertParameterValue(t: ArcTable, p: ProcessParameterValue): ArcTable {
        throw new Error();
    }
    static getParameterValues(t: ArcTable): ProcessParameterValue[] {
        throw new Error();
    }
    AddProtocolTypeColumn(types?: OntologyAnnotation[], index?: int32): void {
        const this$: ArcTable = this;
        const cells: Option<CompositeCell_$union[]> = map_2<OntologyAnnotation[], CompositeCell_$union[]>((array: OntologyAnnotation[]): CompositeCell_$union[] => map_1<OntologyAnnotation, CompositeCell_$union>(CompositeCell_Term, array), types);
        this$.AddColumn(CompositeHeader_ProtocolType(), cells, index);
    }
    AddProtocolVersionColumn(versions?: string[], index?: int32): void {
        const this$: ArcTable = this;
        const cells: Option<CompositeCell_$union[]> = map_2<string[], CompositeCell_$union[]>((array: string[]): CompositeCell_$union[] => map_1<string, CompositeCell_$union>(CompositeCell_FreeText, array), versions);
        this$.AddColumn(CompositeHeader_ProtocolVersion(), cells, index);
    }
    AddProtocolUriColumn(uris?: string[], index?: int32): void {
        const this$: ArcTable = this;
        const cells: Option<CompositeCell_$union[]> = map_2<string[], CompositeCell_$union[]>((array: string[]): CompositeCell_$union[] => map_1<string, CompositeCell_$union>(CompositeCell_FreeText, array), uris);
        this$.AddColumn(CompositeHeader_ProtocolUri(), cells, index);
    }
    AddProtocolDescriptionColumn(descriptions?: string[], index?: int32): void {
        const this$: ArcTable = this;
        const cells: Option<CompositeCell_$union[]> = map_2<string[], CompositeCell_$union[]>((array: string[]): CompositeCell_$union[] => map_1<string, CompositeCell_$union>(CompositeCell_FreeText, array), descriptions);
        this$.AddColumn(CompositeHeader_ProtocolDescription(), cells, index);
    }
    AddProtocolNameColumn(names?: string[], index?: int32): void {
        const this$: ArcTable = this;
        const cells: Option<CompositeCell_$union[]> = map_2<string[], CompositeCell_$union[]>((array: string[]): CompositeCell_$union[] => map_1<string, CompositeCell_$union>(CompositeCell_FreeText, array), names);
        this$.AddColumn(CompositeHeader_ProtocolREF(), cells, index);
    }
    GetProtocolTypeColumn(): CompositeColumn {
        const this$: ArcTable = this;
        return this$.GetColumnByHeader(CompositeHeader_ProtocolType());
    }
    GetProtocolVersionColumn(): CompositeColumn {
        const this$: ArcTable = this;
        return this$.GetColumnByHeader(CompositeHeader_ProtocolVersion());
    }
    GetProtocolUriColumn(): CompositeColumn {
        const this$: ArcTable = this;
        return this$.GetColumnByHeader(CompositeHeader_ProtocolUri());
    }
    GetProtocolDescriptionColumn(): CompositeColumn {
        const this$: ArcTable = this;
        return this$.GetColumnByHeader(CompositeHeader_ProtocolDescription());
    }
    GetProtocolNameColumn(): CompositeColumn {
        const this$: ArcTable = this;
        return this$.GetColumnByHeader(CompositeHeader_ProtocolREF());
    }
    GetComponentColumns(): CompositeColumn[] {
        const this$: ArcTable = this;
        return map_1<CompositeHeader_$union, CompositeColumn>((h_1: CompositeHeader_$union): CompositeColumn => this$.GetColumnByHeader(h_1), toArray<CompositeHeader_$union>(filter<CompositeHeader_$union>((h: CompositeHeader_$union): boolean => h.isComponent, this$.Headers)));
    }
    static fromProtocol(p: Protocol): ArcTable {
        const t: ArcTable = ArcTable.init(defaultArg(p.Name, ""));
        const enumerator: IEnumerator<ProtocolParameter> = getEnumerator(defaultArg(p.Parameters, empty<ProtocolParameter>()));
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const pp: ProtocolParameter = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                t.AddColumn(CompositeHeader_Parameter(value_9(pp.ParameterName)), void 0, ISA_ProtocolParameter__ProtocolParameter_TryGetColumnIndex(pp));
            }
        }
        finally {
            disposeSafe(enumerator as IDisposable);
        }
        const enumerator_1: IEnumerator<Component> = getEnumerator(defaultArg(p.Components, empty<Component>()));
        try {
            while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                const c: Component = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const v_1: Option<CompositeCell_$union[]> = map_2<Value_$union, CompositeCell_$union[]>((arg: Value_$union): CompositeCell_$union[] => singleton_1<CompositeCell_$union>(CompositeCell.fromValue(arg, unwrap(c.ComponentUnit))), c.ComponentValue);
                t.AddColumn(CompositeHeader_Parameter(value_9(c.ComponentType)), v_1, ISA_Component__Component_TryGetColumnIndex(c));
            }
        }
        finally {
            disposeSafe(enumerator_1 as IDisposable);
        }
        map_2<string, void>((d: string): void => {
            t.AddProtocolDescriptionColumn([d]);
        }, p.Description);
        map_2<string, void>((d_1: string): void => {
            t.AddProtocolVersionColumn([d_1]);
        }, p.Version);
        map_2<OntologyAnnotation, void>((d_2: OntologyAnnotation): void => {
            t.AddProtocolTypeColumn([d_2]);
        }, p.ProtocolType);
        map_2<string, void>((d_3: string): void => {
            t.AddProtocolUriColumn([d_3]);
        }, p.Uri);
        map_2<string, void>((d_4: string): void => {
            t.AddProtocolNameColumn([d_4]);
        }, p.Name);
        return t;
    }
    GetProtocols(): FSharpList<Protocol> {
        const this$: ArcTable = this;
        return (this$.RowCount === 0) ? singleton_2(fold<CompositeHeader_$union, Protocol>((p: Protocol, h: CompositeHeader_$union): Protocol => {
            switch (h.tag) {
                case /* ProtocolType */ 4:
                    return Protocol_setProtocolType(p, OntologyAnnotation_get_empty());
                case /* ProtocolVersion */ 7:
                    return Protocol_setVersion(p, "");
                case /* ProtocolUri */ 6:
                    return Protocol_setUri(p, "");
                case /* ProtocolDescription */ 5:
                    return Protocol_setDescription(p, "");
                case /* ProtocolREF */ 8:
                    return Protocol_setName(p, "");
                case /* Parameter */ 3:
                    return Protocol_addParameter(ProtocolParameter_create_2769312B(void 0, h.fields[0]), p);
                case /* Component */ 0:
                    return Protocol_addComponent(Component_create_Z33AADEE0(void 0, void 0, void 0, h.fields[0]), p);
                default:
                    return p;
            }
        }, Protocol_get_empty(), this$.Headers)) : List_distinct<Protocol>(initialize_1<Protocol>(this$.RowCount, (i: int32): Protocol => toProtocol(zip<CompositeHeader_$union, CompositeCell_$union>(this$.Headers, this$.GetRow(i)))), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    GetProcesses(): FSharpList<Process> {
        const this$: ArcTable = this;
        let getter: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Process));
        const clo: ((arg0: IMap<[int32, int32], CompositeCell_$union>) => ((arg0: int32) => Process)) = ProcessParsing_getProcessGetter(this$.Name, this$.Headers);
        getter = ((arg: IMap<[int32, int32], CompositeCell_$union>): ((arg0: int32) => Process) => {
            const clo_1: ((arg0: int32) => Process) = clo(arg);
            return clo_1;
        });
        return toList<Process>(delay<Process>((): Iterable<Process> => map<int32, Process>((i: int32): Process => getter(this$.Values)(i), rangeDouble(0, 1, this$.RowCount - 1))));
    }
    static fromProcesses(name: string, ps: FSharpList<Process>): ArcTable {
        const tupledArg: [CompositeHeader_$union[], IMap<[int32, int32], CompositeCell_$union>] = ProcessParsing_alignByHeaders(collect_1<Process, FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>(ProcessParsing_processToRows, ps));
        return ArcTable.create(name, tupledArg[0], tupledArg[1]);
    }
    toString(): string {
        const this$: ArcTable = this;
        return join("\n", toList<string>(delay<string>((): Iterable<string> => append<string>(singleton<string>(`Table: ${this$.Name}`), delay<string>((): Iterable<string> => append<string>(singleton<string>("-------------"), delay<string>((): Iterable<string> => append<string>(singleton<string>(join("\t|\t", map<CompositeHeader_$union, string>(toString, this$.Headers))), delay<string>((): Iterable<string> => map<int32, string>((rowI: int32): string => join("\t|\t", map<CompositeCell_$union, string>(toString, this$.GetRow(rowI))), rangeDouble(0, 1, this$.RowCount - 1)))))))))));
    }
    Equals(other: any): boolean {
        const this$: ArcTable = this;
        if (other instanceof ArcTable) {
            const table = other as ArcTable;
            const sameName: boolean = this$.Name === table.Name;
            const h1: CompositeHeader_$union[] = this$.Headers;
            const h2: CompositeHeader_$union[] = table.Headers;
            const sameHeaderLength: boolean = h1.length === h2.length;
            const sameHeaders: boolean = forAll2<CompositeHeader_$union, CompositeHeader_$union>(equals, h1, h2);
            const b1: Iterable<[[int32, int32], CompositeCell_$union]> = sortBy<[[int32, int32], CompositeCell_$union], [int32, int32]>((kv: [[int32, int32], CompositeCell_$union]): [int32, int32] => kv[0], this$.Values, {
                Compare: compareArrays,
            });
            const b2: Iterable<[[int32, int32], CompositeCell_$union]> = sortBy<[[int32, int32], CompositeCell_$union], [int32, int32]>((kv_1: [[int32, int32], CompositeCell_$union]): [int32, int32] => kv_1[0], table.Values, {
                Compare: compareArrays,
            });
            return (((sameName && sameHeaderLength) && sameHeaders) && (length<[[int32, int32], CompositeCell_$union]>(b1) === length<[[int32, int32], CompositeCell_$union]>(b2))) && forAll2<[[int32, int32], CompositeCell_$union], [[int32, int32], CompositeCell_$union]>(equals, b1, b2);
        }
        else {
            return false;
        }
    }
    GetHashCode(): int32 {
        const this$: ArcTable = this;
        return ((stringHash(this$.Name) + fold<CompositeHeader_$union, int32>((state: int32, ele: CompositeHeader_$union): int32 => (state + safeHash(ele)), 0, this$.Headers)) + fold<[[int32, int32], CompositeCell_$union], int32>((state_2: int32, ele_1: [[int32, int32], CompositeCell_$union]): int32 => (state_2 + safeHash(ele_1)), 0, this$.Values)) | 0;
    }
}

export function ArcTable_$reflection(): TypeInfo {
    return record_type("ISA.ArcTable", [], ArcTable, () => [["Name", string_type], ["Headers", array_type(CompositeHeader_$reflection())], ["Values", class_type("System.Collections.Generic.Dictionary`2", [tuple_type(int32_type, int32_type), CompositeCell_$reflection()])]]);
}

