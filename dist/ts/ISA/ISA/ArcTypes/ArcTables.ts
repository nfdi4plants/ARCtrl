import { map, delay, toList, length, tryFindIndex } from "../../../fable_modules/fable-library-ts/Seq.js";
import { ArcTable } from "./ArcTable.js";
import { unwrap, defaultArg, value, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { IMap, comparePrimitives, stringHash, compare } from "../../../fable_modules/fable-library-ts/Util.js";
import { distinct } from "../../../fable_modules/fable-library-ts/Seq2.js";
import { FSharpSet__get_IsEmpty, FSharpSet, ofSeq, intersect } from "../../../fable_modules/fable-library-ts/Set.js";
import { class_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { map as map_1, collect, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { insertRangeInPlace } from "../../../fable_modules/fable-library-ts/Array.js";
import { CompositeHeader_$union } from "./CompositeHeader.js";
import { CompositeCell_$union } from "./CompositeCell.js";
import { CompositeColumn } from "./CompositeColumn.js";
import { Process } from "../JsonTypes/Process.js";
import { ProcessParsing_groupProcesses, ProcessParsing_processToRows, ProcessParsing_alignByHeaders } from "./ArcTableAux.js";

export function ArcTablesAux_indexByTableName(name: string, tables: ArcTable[]): int32 {
    const matchValue: Option<int32> = tryFindIndex<ArcTable>((t: ArcTable): boolean => (t.Name === name), tables);
    if (matchValue == null) {
        throw new Error(`Unable to find table with name '${name}'!`);
    }
    else {
        return value(matchValue) | 0;
    }
}

export function ArcTablesAux_SanityChecks_validateSheetIndex(index: int32, allowAppend: boolean, sheets: ArcTable[]): void {
    let x: int32, y: int32;
    if (index < 0) {
        throw new Error("Cannot insert ArcTable at index < 0.");
    }
    if ((x = (index | 0), (y = (sheets.length | 0), allowAppend ? (compare(x, y) > 0) : (compare(x, y) >= 0)))) {
        throw new Error(`Specified index is out of range! Assay contains only ${sheets.length} tables.`);
    }
}

export function ArcTablesAux_SanityChecks_validateNamesUnique(names: Iterable<string>): void {
    if (!(length<string>(names) === length<string>(distinct<string>(names, {
        Equals: (x: string, y: string): boolean => (x === y),
        GetHashCode: stringHash,
    })))) {
        throw new Error("Cannot add multiple tables with the same name! Table names inside one assay must be unqiue");
    }
}

export function ArcTablesAux_SanityChecks_validateNewNameUnique(newName: string, existingNames: Iterable<string>): void {
    const matchValue: Option<int32> = tryFindIndex<string>((x: string): boolean => (x === newName), existingNames);
    if (matchValue == null) {
    }
    else {
        throw new Error(`Cannot create table with name ${newName}, as table names must be unique and table at index ${value(matchValue)} has the same name.`);
    }
}

export function ArcTablesAux_SanityChecks_validateNewNamesUnique(newNames: Iterable<string>, existingNames: Iterable<string>): void {
    ArcTablesAux_SanityChecks_validateNamesUnique(newNames);
    const same: FSharpSet<string> = intersect<string>(ofSeq<string>(newNames, {
        Compare: comparePrimitives,
    }), ofSeq<string>(existingNames, {
        Compare: comparePrimitives,
    }));
    if (!FSharpSet__get_IsEmpty(same)) {
        throw new Error(`Cannot create tables with the names ${same}, as table names must be unique.`);
    }
}

export class ArcTables {
    readonly thisTables: ArcTable[];
    constructor(thisTables: ArcTable[]) {
        this.thisTables = thisTables;
    }
}

export function ArcTables_$reflection(): TypeInfo {
    return class_type("ARCtrl.ISA.ArcTables", void 0, ArcTables);
}

export function ArcTables_$ctor_Z18C2F36D(thisTables: ArcTable[]): ArcTables {
    return new ArcTables(thisTables);
}

export function ArcTables__get_Count(this$: ArcTables): int32 {
    return this$.thisTables.length;
}

export function ArcTables__get_TableNames(this$: ArcTables): FSharpList<string> {
    return toList<string>(delay<string>((): Iterable<string> => map<ArcTable, string>((s: ArcTable): string => s.Name, this$.thisTables)));
}

export function ArcTables__get_Tables(this$: ArcTables): ArcTable[] {
    return this$.thisTables;
}

export function ArcTables__get_Item_Z524259A4(this$: ArcTables, index: int32): ArcTable {
    return this$.thisTables[index];
}

export function ArcTables__AddTable_EC12B15(this$: ArcTables, table: ArcTable, index: Option<int32>): void {
    const index_1: int32 = defaultArg<int32>(index, ArcTables__get_Count(this$)) | 0;
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, ArcTables__get_TableNames(this$));
    this$.thisTables.splice(index_1, 0, table);
}

export function ArcTables__AddTables_Z2D453886(this$: ArcTables, tables: Iterable<ArcTable>, index: Option<int32>): void {
    const index_1: int32 = defaultArg<int32>(index, ArcTables__get_Count(this$)) | 0;
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNamesUnique(map<ArcTable, string>((x: ArcTable): string => x.Name, tables), ArcTables__get_TableNames(this$));
    insertRangeInPlace(index_1, tables, this$.thisTables);
}

export function ArcTables__InitTable_3B406CA4(this$: ArcTables, tableName: string, index: Option<int32>): ArcTable {
    const index_1: int32 = defaultArg<int32>(index, ArcTables__get_Count(this$)) | 0;
    const table: ArcTable = ArcTable.init(tableName);
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, ArcTables__get_TableNames(this$));
    this$.thisTables.splice(index_1, 0, table);
    return table;
}

export function ArcTables__InitTables_7B28792B(this$: ArcTables, tableNames: Iterable<string>, index: Option<int32>): void {
    const index_1: int32 = defaultArg<int32>(index, ArcTables__get_Count(this$)) | 0;
    const tables: Iterable<ArcTable> = map<string, ArcTable>((x: string): ArcTable => ArcTable.init(x), tableNames);
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNamesUnique(map<ArcTable, string>((x_1: ArcTable): string => x_1.Name, tables), ArcTables__get_TableNames(this$));
    insertRangeInPlace(index_1, tables, this$.thisTables);
}

export function ArcTables__GetTableAt_Z524259A4(this$: ArcTables, index: int32): ArcTable {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    return this$.thisTables[index];
}

export function ArcTables__GetTable_Z721C83C5(this$: ArcTables, name: string): ArcTable {
    return ArcTables__GetTableAt_Z524259A4(this$, ArcTablesAux_indexByTableName(name, this$.thisTables));
}

export function ArcTables__UpdateTableAt_7E571736(this$: ArcTables, index: int32, table: ArcTable): void {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, ArcTables__get_TableNames(this$));
    this$.thisTables[index] = table;
}

export function ArcTables__UpdateTable_51766571(this$: ArcTables, name: string, table: ArcTable): void {
    const tupledArg = [ArcTablesAux_indexByTableName(name, this$.thisTables), table] as [int32, ArcTable];
    ArcTables__UpdateTableAt_7E571736(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__RemoveTableAt_Z524259A4(this$: ArcTables, index: int32): void {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    this$.thisTables.splice(index, 1);
}

export function ArcTables__RemoveTable_Z721C83C5(this$: ArcTables, name: string): void {
    ArcTables__RemoveTableAt_Z524259A4(this$, ArcTablesAux_indexByTableName(name, this$.thisTables));
}

export function ArcTables__MapTableAt_8FC095C(this$: ArcTables, index: int32, updateFun: ((arg0: ArcTable) => void)): void {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    updateFun(this$.thisTables[index]);
}

export function ArcTables__MapTable_27DD7B1B(this$: ArcTables, name: string, updateFun: ((arg0: ArcTable) => void)): void {
    const tupledArg = [ArcTablesAux_indexByTableName(name, this$.thisTables), updateFun] as [int32, ((arg0: ArcTable) => void)];
    ArcTables__MapTableAt_8FC095C(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__RenameTableAt_Z176EF219(this$: ArcTables, index: int32, newName: string): void {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(newName, ArcTables__get_TableNames(this$));
    const table: ArcTable = ArcTables__GetTableAt_Z524259A4(this$, index);
    ArcTables__UpdateTableAt_7E571736(this$, index, new ArcTable(newName, table.Headers, table.Values));
}

export function ArcTables__RenameTable_Z384F8060(this$: ArcTables, name: string, newName: string): void {
    const tupledArg = [ArcTablesAux_indexByTableName(name, this$.thisTables), newName] as [int32, string];
    ArcTables__RenameTableAt_Z176EF219(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__AddColumnAt_6A9784DB(this$: ArcTables, tableIndex: int32, header: CompositeHeader_$union, cells: Option<CompositeCell_$union[]>, columnIndex: Option<int32>, forceReplace: Option<boolean>): void {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table: ArcTable): void => {
        table.AddColumn(header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    });
}

export function ArcTables__AddColumn_1FF50D3C(this$: ArcTables, tableName: string, header: CompositeHeader_$union, cells: Option<CompositeCell_$union[]>, columnIndex: Option<int32>, forceReplace: Option<boolean>): void {
    ArcTables__AddColumnAt_6A9784DB(this$, ArcTablesAux_indexByTableName(tableName, this$.thisTables), header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
}

export function ArcTables__RemoveColumnAt_Z37302880(this$: ArcTables, tableIndex: int32, columnIndex: int32): void {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table: ArcTable): void => {
        table.RemoveColumn(columnIndex);
    });
}

export function ArcTables__RemoveColumn_Z18115A39(this$: ArcTables, tableName: string, columnIndex: int32): void {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), columnIndex] as [int32, int32];
    ArcTables__RemoveColumnAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__UpdateColumnAt_21300791(this$: ArcTables, tableIndex: int32, columnIndex: int32, header: CompositeHeader_$union, cells: Option<CompositeCell_$union[]>): void {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table: ArcTable): void => {
        table.UpdateColumn(columnIndex, header, unwrap(cells));
    });
}

export function ArcTables__UpdateColumn_Z6083042A(this$: ArcTables, tableName: string, columnIndex: int32, header: CompositeHeader_$union, cells: Option<CompositeCell_$union[]>): void {
    ArcTables__UpdateColumnAt_21300791(this$, ArcTablesAux_indexByTableName(tableName, this$.thisTables), columnIndex, header, unwrap(cells));
}

export function ArcTables__GetColumnAt_Z37302880(this$: ArcTables, tableIndex: int32, columnIndex: int32): CompositeColumn {
    const table: ArcTable = ArcTables__GetTableAt_Z524259A4(this$, tableIndex);
    return table.GetColumn(columnIndex);
}

export function ArcTables__GetColumn_Z18115A39(this$: ArcTables, tableName: string, columnIndex: int32): CompositeColumn {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), columnIndex] as [int32, int32];
    return ArcTables__GetColumnAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__AddRowAt_Z12CDB784(this$: ArcTables, tableIndex: int32, cells: Option<CompositeCell_$union[]>, rowIndex: Option<int32>): void {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table: ArcTable): void => {
        table.AddRow(unwrap(cells), unwrap(rowIndex));
    });
}

export function ArcTables__AddRow_Z16315DE5(this$: ArcTables, tableName: string, cells: Option<CompositeCell_$union[]>, rowIndex: Option<int32>): void {
    ArcTables__AddRowAt_Z12CDB784(this$, ArcTablesAux_indexByTableName(tableName, this$.thisTables), unwrap(cells), unwrap(rowIndex));
}

export function ArcTables__RemoveRowAt_Z37302880(this$: ArcTables, tableIndex: int32, rowIndex: int32): void {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table: ArcTable): void => {
        table.RemoveRow(rowIndex);
    });
}

export function ArcTables__RemoveRow_Z18115A39(this$: ArcTables, tableName: string, rowIndex: int32): void {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), rowIndex] as [int32, int32];
    ArcTables__RemoveRowAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__UpdateRowAt_1CF7B5DC(this$: ArcTables, tableIndex: int32, rowIndex: int32, cells: CompositeCell_$union[]): void {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table: ArcTable): void => {
        table.UpdateRow(rowIndex, cells);
    });
}

export function ArcTables__UpdateRow_1BFE2CFB(this$: ArcTables, tableName: string, rowIndex: int32, cells: CompositeCell_$union[]): void {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), rowIndex, cells] as [int32, int32, CompositeCell_$union[]];
    ArcTables__UpdateRowAt_1CF7B5DC(this$, tupledArg[0], tupledArg[1], tupledArg[2]);
}

export function ArcTables__GetRowAt_Z37302880(this$: ArcTables, tableIndex: int32, rowIndex: int32): CompositeCell_$union[] {
    const table: ArcTable = ArcTables__GetTableAt_Z524259A4(this$, tableIndex);
    return table.GetRow(rowIndex);
}

export function ArcTables__GetRow_Z18115A39(this$: ArcTables, tableName: string, rowIndex: int32): CompositeCell_$union[] {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), rowIndex] as [int32, int32];
    return ArcTables__GetRowAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

/**
 * Return a list of all the processes in all the tables.
 */
export function ArcTables__GetProcesses(this$: ArcTables): FSharpList<Process> {
    return collect<ArcTable, Process>((t: ArcTable): FSharpList<Process> => t.GetProcesses(), toList<ArcTable>(ArcTables__get_Tables(this$)));
}

/**
 * Create a collection of tables from a list of processes.
 * 
 * For this, the processes are grouped by nameroot ("nameroot_1", "nameroot_2" ...) or exectued protocol if no name exists
 * 
 * Then each group is converted to a table with this nameroot as sheetname
 */
export function ArcTables_fromProcesses_134EBDED(ps: FSharpList<Process>): ArcTables {
    let arg: FSharpList<ArcTable>;
    return ArcTables_$ctor_Z18C2F36D((arg = map_1<[string, FSharpList<Process>], ArcTable>((tupledArg: [string, FSharpList<Process>]): ArcTable => {
        const tupledArg_1: [CompositeHeader_$union[], IMap<[int32, int32], CompositeCell_$union>] = ProcessParsing_alignByHeaders(collect<Process, FSharpList<[CompositeHeader_$union, CompositeCell_$union]>>(ProcessParsing_processToRows, tupledArg[1]));
        return ArcTable.create(tupledArg[0], tupledArg_1[0], tupledArg_1[1]);
    }, ProcessParsing_groupProcesses(ps)), Array.from(arg)));
}

