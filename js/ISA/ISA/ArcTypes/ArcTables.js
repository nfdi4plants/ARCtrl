import { map, delay, toList, length, tryFindIndex } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { comparePrimitives, stringHash, compare } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { distinct } from "../../../fable_modules/fable-library.4.1.4/Seq2.js";
import { FSharpSet__get_IsEmpty, ofSeq, intersect } from "../../../fable_modules/fable-library.4.1.4/Set.js";
import { class_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { unwrap, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { insertRangeInPlace } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { ArcTable } from "./ArcTable.js";
import { map as map_1, collect } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { ProcessParsing_groupProcesses, ProcessParsing_processToRows, ProcessParsing_alignByHeaders } from "./ArcTableAux.js";

export function ArcTablesAux_indexByTableName(name, tables) {
    const matchValue = tryFindIndex((t) => (t.Name === name), tables);
    if (matchValue == null) {
        throw new Error(`Unable to find table with name '${name}'!`);
    }
    else {
        return matchValue | 0;
    }
}

export function ArcTablesAux_SanityChecks_validateSheetIndex(index, allowAppend, sheets) {
    let x, y;
    if (index < 0) {
        throw new Error("Cannot insert ArcTable at index < 0.");
    }
    if ((x = (index | 0), (y = (sheets.length | 0), allowAppend ? (compare(x, y) > 0) : (compare(x, y) >= 0)))) {
        throw new Error(`Specified index is out of range! Assay contains only ${sheets.length} tables.`);
    }
}

export function ArcTablesAux_SanityChecks_validateNamesUnique(names) {
    if (!(length(names) === length(distinct(names, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })))) {
        throw new Error("Cannot add multiple tables with the same name! Table names inside one assay must be unqiue");
    }
}

export function ArcTablesAux_SanityChecks_validateNewNameUnique(newName, existingNames) {
    const matchValue = tryFindIndex((x) => (x === newName), existingNames);
    if (matchValue == null) {
    }
    else {
        throw new Error(`Cannot create table with name ${newName}, as table names must be unique and table at index ${matchValue} has the same name.`);
    }
}

export function ArcTablesAux_SanityChecks_validateNewNamesUnique(newNames, existingNames) {
    ArcTablesAux_SanityChecks_validateNamesUnique(newNames);
    const same = intersect(ofSeq(newNames, {
        Compare: comparePrimitives,
    }), ofSeq(existingNames, {
        Compare: comparePrimitives,
    }));
    if (!FSharpSet__get_IsEmpty(same)) {
        throw new Error(`Cannot create tables with the names ${same}, as table names must be unique.`);
    }
}

export class ArcTables {
    constructor(thisTables) {
        this.thisTables = thisTables;
    }
}

export function ArcTables_$reflection() {
    return class_type("ARCtrl.ISA.ArcTables", void 0, ArcTables);
}

export function ArcTables_$ctor_Z18C2F36D(thisTables) {
    return new ArcTables(thisTables);
}

export function ArcTables__get_Count(this$) {
    return this$.thisTables.length;
}

export function ArcTables__get_TableNames(this$) {
    return toList(delay(() => map((s) => s.Name, this$.thisTables)));
}

export function ArcTables__get_Tables(this$) {
    return this$.thisTables;
}

export function ArcTables__get_Item_Z524259A4(this$, index) {
    return this$.thisTables[index];
}

export function ArcTables__AddTable_EC12B15(this$, table, index) {
    const index_1 = defaultArg(index, ArcTables__get_Count(this$)) | 0;
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, ArcTables__get_TableNames(this$));
    this$.thisTables.splice(index_1, 0, table);
}

export function ArcTables__AddTables_Z2D453886(this$, tables, index) {
    const index_1 = defaultArg(index, ArcTables__get_Count(this$)) | 0;
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNamesUnique(map((x) => x.Name, tables), ArcTables__get_TableNames(this$));
    insertRangeInPlace(index_1, tables, this$.thisTables);
}

export function ArcTables__InitTable_3B406CA4(this$, tableName, index) {
    const index_1 = defaultArg(index, ArcTables__get_Count(this$)) | 0;
    const table = ArcTable.init(tableName);
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, ArcTables__get_TableNames(this$));
    this$.thisTables.splice(index_1, 0, table);
    return table;
}

export function ArcTables__InitTables_7B28792B(this$, tableNames, index) {
    const index_1 = defaultArg(index, ArcTables__get_Count(this$)) | 0;
    const tables = map((x) => ArcTable.init(x), tableNames);
    ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNamesUnique(map((x_1) => x_1.Name, tables), ArcTables__get_TableNames(this$));
    insertRangeInPlace(index_1, tables, this$.thisTables);
}

export function ArcTables__GetTableAt_Z524259A4(this$, index) {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    return this$.thisTables[index];
}

export function ArcTables__GetTable_Z721C83C5(this$, name) {
    return ArcTables__GetTableAt_Z524259A4(this$, ArcTablesAux_indexByTableName(name, this$.thisTables));
}

export function ArcTables__UpdateTableAt_7E571736(this$, index, table) {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, ArcTables__get_TableNames(this$));
    this$.thisTables[index] = table;
}

export function ArcTables__UpdateTable_51766571(this$, name, table) {
    const tupledArg = [ArcTablesAux_indexByTableName(name, this$.thisTables), table];
    ArcTables__UpdateTableAt_7E571736(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__RemoveTableAt_Z524259A4(this$, index) {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    this$.thisTables.splice(index, 1);
}

export function ArcTables__RemoveTable_Z721C83C5(this$, name) {
    ArcTables__RemoveTableAt_Z524259A4(this$, ArcTablesAux_indexByTableName(name, this$.thisTables));
}

export function ArcTables__MapTableAt_8FC095C(this$, index, updateFun) {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    updateFun(this$.thisTables[index]);
}

export function ArcTables__MapTable_27DD7B1B(this$, name, updateFun) {
    const tupledArg = [ArcTablesAux_indexByTableName(name, this$.thisTables), updateFun];
    ArcTables__MapTableAt_8FC095C(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__RenameTableAt_Z176EF219(this$, index, newName) {
    ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.thisTables);
    ArcTablesAux_SanityChecks_validateNewNameUnique(newName, ArcTables__get_TableNames(this$));
    const table = ArcTables__GetTableAt_Z524259A4(this$, index);
    ArcTables__UpdateTableAt_7E571736(this$, index, new ArcTable(newName, table.Headers, table.Values));
}

export function ArcTables__RenameTable_Z384F8060(this$, name, newName) {
    const tupledArg = [ArcTablesAux_indexByTableName(name, this$.thisTables), newName];
    ArcTables__RenameTableAt_Z176EF219(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__AddColumnAt_6A9784DB(this$, tableIndex, header, cells, columnIndex, forceReplace) {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table) => {
        table.AddColumn(header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    });
}

export function ArcTables__AddColumn_1FF50D3C(this$, tableName, header, cells, columnIndex, forceReplace) {
    ArcTables__AddColumnAt_6A9784DB(this$, ArcTablesAux_indexByTableName(tableName, this$.thisTables), header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
}

export function ArcTables__RemoveColumnAt_Z37302880(this$, tableIndex, columnIndex) {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table) => {
        table.RemoveColumn(columnIndex);
    });
}

export function ArcTables__RemoveColumn_Z18115A39(this$, tableName, columnIndex) {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), columnIndex];
    ArcTables__RemoveColumnAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__UpdateColumnAt_21300791(this$, tableIndex, columnIndex, header, cells) {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table) => {
        table.UpdateColumn(columnIndex, header, unwrap(cells));
    });
}

export function ArcTables__UpdateColumn_Z6083042A(this$, tableName, columnIndex, header, cells) {
    ArcTables__UpdateColumnAt_21300791(this$, ArcTablesAux_indexByTableName(tableName, this$.thisTables), columnIndex, header, unwrap(cells));
}

export function ArcTables__GetColumnAt_Z37302880(this$, tableIndex, columnIndex) {
    const table = ArcTables__GetTableAt_Z524259A4(this$, tableIndex);
    return table.GetColumn(columnIndex);
}

export function ArcTables__GetColumn_Z18115A39(this$, tableName, columnIndex) {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), columnIndex];
    return ArcTables__GetColumnAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__AddRowAt_Z12CDB784(this$, tableIndex, cells, rowIndex) {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table) => {
        table.AddRow(unwrap(cells), unwrap(rowIndex));
    });
}

export function ArcTables__AddRow_Z16315DE5(this$, tableName, cells, rowIndex) {
    ArcTables__AddRowAt_Z12CDB784(this$, ArcTablesAux_indexByTableName(tableName, this$.thisTables), unwrap(cells), unwrap(rowIndex));
}

export function ArcTables__RemoveRowAt_Z37302880(this$, tableIndex, rowIndex) {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table) => {
        table.RemoveRow(rowIndex);
    });
}

export function ArcTables__RemoveRow_Z18115A39(this$, tableName, rowIndex) {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), rowIndex];
    ArcTables__RemoveRowAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

export function ArcTables__UpdateRowAt_1CF7B5DC(this$, tableIndex, rowIndex, cells) {
    ArcTables__MapTableAt_8FC095C(this$, tableIndex, (table) => {
        table.UpdateRow(rowIndex, cells);
    });
}

export function ArcTables__UpdateRow_1BFE2CFB(this$, tableName, rowIndex, cells) {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), rowIndex, cells];
    ArcTables__UpdateRowAt_1CF7B5DC(this$, tupledArg[0], tupledArg[1], tupledArg[2]);
}

export function ArcTables__GetRowAt_Z37302880(this$, tableIndex, rowIndex) {
    const table = ArcTables__GetTableAt_Z524259A4(this$, tableIndex);
    return table.GetRow(rowIndex);
}

export function ArcTables__GetRow_Z18115A39(this$, tableName, rowIndex) {
    const tupledArg = [ArcTablesAux_indexByTableName(tableName, this$.thisTables), rowIndex];
    return ArcTables__GetRowAt_Z37302880(this$, tupledArg[0], tupledArg[1]);
}

/**
 * Return a list of all the processes in all the tables.
 */
export function ArcTables__GetProcesses(this$) {
    return collect((t) => t.GetProcesses(), toList(ArcTables__get_Tables(this$)));
}

/**
 * Create a collection of tables from a list of processes.
 * 
 * For this, the processes are grouped by nameroot ("nameroot_1", "nameroot_2" ...) or exectued protocol if no name exists
 * 
 * Then each group is converted to a table with this nameroot as sheetname
 */
export function ArcTables_fromProcesses_134EBDED(ps) {
    let arg;
    return ArcTables_$ctor_Z18C2F36D((arg = map_1((tupledArg) => {
        const tupledArg_1 = ProcessParsing_alignByHeaders(collect(ProcessParsing_processToRows, tupledArg[1]));
        return ArcTable.create(tupledArg[0], tupledArg_1[0], tupledArg_1[1]);
    }, ProcessParsing_groupProcesses(ps)), Array.from(arg)));
}

