import { append, toArray, reduce, choose, map as map_1, delay, toList, length, findIndex, tryFindIndex } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { Dictionary_tryFind } from "../Helper/Collections.js";
import { addToSet, addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { toIterator, equals, comparePrimitives, stringHash, compare, disposeSafe, getEnumerator } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { bind, map as map_2, unwrap, defaultArg, value as value_2 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { collect, map as map_3, item, insertRangeInPlace, fold } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { CompositeHeader } from "./CompositeHeader.js";
import { Array_groupBy, distinct } from "../../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { FSharpSet__get_IsEmpty, ofSeq, intersect } from "../../fable_modules/fable-library-js.4.22.0/Set.js";
import { ArcTable } from "./ArcTable.js";
import { tryFind, ofSeq as ofSeq_1 } from "../../fable_modules/fable-library-js.4.22.0/Map.js";
import { Unchecked_fillMissingCells } from "./ArcTableAux.js";
import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";

/**
 * If a table with the given name exists in the TableList, returns it, else returns None.
 */
export function ArcTablesAux_tryFindIndexByTableName(name, tables) {
    return tryFindIndex((t) => (t.Name === name), tables);
}

/**
 * If a table with the given name exists in the TableList, returns it, else fails.
 */
export function ArcTablesAux_findIndexByTableName(name, tables) {
    const matchValue = tryFindIndex((t) => (t.Name === name), tables);
    if (matchValue == null) {
        throw new Error(`Unable to find table with name '${name}'!`);
    }
    else {
        return matchValue | 0;
    }
}

/**
 * Collects the IOType of each distinct entity in the tables. Then merges the IOType of each entity according to the IOType.Merge function.
 */
export function ArcTablesAux_getIOMap(tables) {
    const mappings = new Map([]);
    const includeInMap = (name, ioType) => {
        if (name !== "") {
            const matchValue = Dictionary_tryFind(name, mappings);
            if (matchValue == null) {
                addToDict(mappings, name, ioType);
            }
            else {
                const oldIOType = matchValue;
                const newIOType = oldIOType.Merge(ioType);
                mappings.set(name, newIOType);
            }
        }
    };
    let enumerator = getEnumerator(tables);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const table = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const matchValue_1 = table.TryGetInputColumn();
            if (matchValue_1 == null) {
            }
            else {
                const ic = matchValue_1;
                const ioType_1 = value_2(ic.Header.TryInput());
                ic.Cells.forEach((c) => {
                    includeInMap(c.ToFreeTextCell().AsFreeText, ioType_1);
                });
            }
            const matchValue_2 = table.TryGetOutputColumn();
            if (matchValue_2 == null) {
            }
            else {
                const oc = matchValue_2;
                const ioType_2 = value_2(oc.Header.TryOutput());
                oc.Cells.forEach((c_1) => {
                    includeInMap(c_1.ToFreeTextCell().AsFreeText, ioType_2);
                });
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return mappings;
}

export function ArcTablesAux_applyIOMap(map, tables) {
    let enumerator = getEnumerator(tables);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const table = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const matchValue = table.TryGetInputColumn();
            if (matchValue == null) {
            }
            else {
                const ic = matchValue;
                const index = findIndex((x) => x.isInput, table.Headers) | 0;
                const newIOType = fold((io, c) => {
                    const matchValue_1 = Dictionary_tryFind(c.ToFreeTextCell().AsFreeText, map);
                    if (matchValue_1 == null) {
                        return io;
                    }
                    else {
                        const newIO = matchValue_1;
                        return io.Merge(newIO);
                    }
                }, value_2(ic.Header.TryInput()), ic.Cells);
                table.UpdateHeader(index, new CompositeHeader(11, [newIOType]));
            }
            const matchValue_2 = table.TryGetOutputColumn();
            if (matchValue_2 == null) {
            }
            else {
                const oc = matchValue_2;
                const index_1 = findIndex((x_1) => x_1.isOutput, table.Headers) | 0;
                const newIOType_1 = fold((io_1, c_1) => {
                    const matchValue_3 = Dictionary_tryFind(c_1.ToFreeTextCell().AsFreeText, map);
                    if (matchValue_3 == null) {
                        return io_1;
                    }
                    else {
                        const newIO_1 = matchValue_3;
                        return io_1.Merge(newIO_1);
                    }
                }, value_2(oc.Header.TryOutput()), oc.Cells);
                table.UpdateHeader(index_1, new CompositeHeader(12, [newIOType_1]));
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
}

/**
 * Fails, if the index is out of range of the Tables collection. When allowAppend is set to true, it may be out of range by at most 1.
 */
export function ArcTablesAux_SanityChecks_validateSheetIndex(index, allowAppend, sheets) {
    let x, y;
    if (index < 0) {
        throw new Error("Cannot insert ArcTable at index < 0.");
    }
    if ((x = (index | 0), (y = (sheets.length | 0), allowAppend ? (compare(x, y) > 0) : (compare(x, y) >= 0)))) {
        throw new Error(`Specified index is out of range! Assay contains only ${sheets.length} tables.`);
    }
}

/**
 * Fails, if two tables have the same name.
 */
export function ArcTablesAux_SanityChecks_validateNamesUnique(names) {
    if (!(length(names) === length(distinct(names, {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    })))) {
        throw new Error("Cannot add multiple tables with the same name! Table names inside one assay must be unqiue");
    }
}

/**
 * Fails, if the name is already used by another table.
 */
export function ArcTablesAux_SanityChecks_validateNewNameUnique(newName, existingNames) {
    const matchValue = tryFindIndex((x) => (x === newName), existingNames);
    if (matchValue == null) {
    }
    else {
        throw new Error(`Cannot create table with name ${newName}, as table names must be unique and table at index ${matchValue} has the same name.`);
    }
}

/**
 * Fails, if the name is already used by another table at a different position.
 * 
 * Does not fail, if the newName is the same as the one in the given position.
 */
export function ArcTablesAux_SanityChecks_validateNewNameAtUnique(index, newName, existingNames) {
    const matchValue = tryFindIndex((x) => (x === newName), existingNames);
    if (matchValue == null) {
    }
    else if (index === matchValue) {
        const i_1 = matchValue | 0;
    }
    else {
        const i_2 = matchValue | 0;
        throw new Error(`Cannot create table with name ${newName}, as table names must be unique and table at index ${i_2} has the same name.`);
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
    constructor(initTables) {
        this.tables = ((ArcTablesAux_SanityChecks_validateNamesUnique(map_1((t) => t.Name, initTables)), initTables));
    }
    get Tables() {
        const this$ = this;
        return this$.tables;
    }
    set Tables(newTables) {
        const this$ = this;
        this$.tables = newTables;
    }
    get_Item(index) {
        const this$ = this;
        return this$.Tables[index];
    }
    get TableNames() {
        const this$ = this;
        return toList(delay(() => map_1((s) => s.Name, this$.Tables)));
    }
    get TableCount() {
        const this$ = this;
        return this$.Tables.length | 0;
    }
    AddTable(table, index) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.TableCount) | 0;
        ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, this$.TableNames);
        this$.Tables.splice(index_1, 0, table);
    }
    AddTables(tables, index) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.TableCount) | 0;
        ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNamesUnique(map_1((x) => x.Name, tables), this$.TableNames);
        insertRangeInPlace(index_1, tables, this$.Tables);
    }
    InitTable(tableName, index) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.TableCount) | 0;
        const table = ArcTable.init(tableName);
        ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNameUnique(table.Name, this$.TableNames);
        this$.Tables.splice(index_1, 0, table);
        return table;
    }
    InitTables(tableNames, index) {
        const this$ = this;
        const index_1 = defaultArg(index, this$.TableCount) | 0;
        const tables = map_1((x) => ArcTable.init(x), tableNames);
        ArcTablesAux_SanityChecks_validateSheetIndex(index_1, true, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNamesUnique(map_1((x_1) => x_1.Name, tables), this$.TableNames);
        insertRangeInPlace(index_1, tables, this$.Tables);
    }
    GetTableAt(index) {
        const this$ = this;
        ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.Tables);
        return this$.Tables[index];
    }
    GetTable(name) {
        const this$ = this;
        const index = ArcTablesAux_findIndexByTableName(name, this$.Tables) | 0;
        return this$.GetTableAt(index);
    }
    UpdateTableAt(index, table) {
        const this$ = this;
        ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNameAtUnique(index, table.Name, this$.TableNames);
        this$.Tables[index] = table;
    }
    UpdateTable(name, table) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(name, this$.Tables), table];
        this$.UpdateTableAt(tupledArg[0], tupledArg[1]);
    }
    SetTableAt(index, table) {
        const this$ = this;
        ArcTablesAux_SanityChecks_validateSheetIndex(index, true, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNameAtUnique(index, table.Name, this$.TableNames);
        this$.Tables[index] = table;
    }
    SetTable(name, table) {
        const this$ = this;
        const matchValue = ArcTablesAux_tryFindIndexByTableName(name, this$.Tables);
        if (matchValue == null) {
            this$.AddTable(table);
        }
        else {
            const index = matchValue | 0;
            this$.SetTableAt(index, table);
        }
    }
    RemoveTableAt(index) {
        const this$ = this;
        ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.Tables);
        this$.Tables.splice(index, 1);
    }
    RemoveTable(name) {
        const this$ = this;
        const index = ArcTablesAux_findIndexByTableName(name, this$.Tables) | 0;
        this$.RemoveTableAt(index);
    }
    MapTableAt(index, updateFun) {
        const this$ = this;
        ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.Tables);
        updateFun(this$.Tables[index]);
    }
    MapTable(name, updateFun) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(name, this$.Tables), updateFun];
        this$.MapTableAt(tupledArg[0], tupledArg[1]);
    }
    RenameTableAt(index, newName) {
        const this$ = this;
        ArcTablesAux_SanityChecks_validateSheetIndex(index, false, this$.Tables);
        ArcTablesAux_SanityChecks_validateNewNameUnique(newName, this$.TableNames);
        const table = this$.GetTableAt(index);
        table.Name = newName;
    }
    RenameTable(name, newName) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(name, this$.Tables), newName];
        this$.RenameTableAt(tupledArg[0], tupledArg[1]);
    }
    AddColumnAt(tableIndex, header, cells, columnIndex, forceReplace) {
        const this$ = this;
        this$.MapTableAt(tableIndex, (table) => {
            table.AddColumn(header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
        });
    }
    AddColumn(tableName, header, cells, columnIndex, forceReplace) {
        const this$ = this;
        const i = ArcTablesAux_findIndexByTableName(tableName, this$.Tables) | 0;
        this$.AddColumnAt(i, header, unwrap(cells), unwrap(columnIndex), unwrap(forceReplace));
    }
    RemoveColumnAt(tableIndex, columnIndex) {
        const this$ = this;
        this$.MapTableAt(tableIndex, (table) => {
            table.RemoveColumn(columnIndex);
        });
    }
    RemoveColumn(tableName, columnIndex) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(tableName, this$.Tables), columnIndex];
        this$.RemoveColumnAt(tupledArg[0], tupledArg[1]);
    }
    UpdateColumnAt(tableIndex, columnIndex, header, cells) {
        const this$ = this;
        this$.MapTableAt(tableIndex, (table) => {
            table.UpdateColumn(columnIndex, header, unwrap(cells));
        });
    }
    UpdateColumn(tableName, columnIndex, header, cells) {
        const this$ = this;
        const tableIndex = ArcTablesAux_findIndexByTableName(tableName, this$.Tables) | 0;
        this$.UpdateColumnAt(tableIndex, columnIndex, header, unwrap(cells));
    }
    GetColumnAt(tableIndex, columnIndex) {
        const this$ = this;
        const table = this$.GetTableAt(tableIndex);
        return table.GetColumn(columnIndex);
    }
    GetColumn(tableName, columnIndex) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(tableName, this$.Tables), columnIndex];
        return this$.GetColumnAt(tupledArg[0], tupledArg[1]);
    }
    AddRowAt(tableIndex, cells, rowIndex) {
        const this$ = this;
        this$.MapTableAt(tableIndex, (table) => {
            table.AddRow(unwrap(cells), unwrap(rowIndex));
        });
    }
    AddRow(tableName, cells, rowIndex) {
        const this$ = this;
        const i = ArcTablesAux_findIndexByTableName(tableName, this$.Tables) | 0;
        this$.AddRowAt(i, unwrap(cells), unwrap(rowIndex));
    }
    RemoveRowAt(tableIndex, rowIndex) {
        const this$ = this;
        this$.MapTableAt(tableIndex, (table) => {
            table.RemoveRow(rowIndex);
        });
    }
    RemoveRow(tableName, rowIndex) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(tableName, this$.Tables), rowIndex];
        this$.RemoveRowAt(tupledArg[0], tupledArg[1]);
    }
    UpdateRowAt(tableIndex, rowIndex, cells) {
        const this$ = this;
        this$.MapTableAt(tableIndex, (table) => {
            table.UpdateRow(rowIndex, cells);
        });
    }
    UpdateRow(tableName, rowIndex, cells) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(tableName, this$.Tables), rowIndex, cells];
        this$.UpdateRowAt(tupledArg[0], tupledArg[1], tupledArg[2]);
    }
    GetRowAt(tableIndex, rowIndex) {
        const this$ = this;
        const table = this$.GetTableAt(tableIndex);
        return table.GetRow(rowIndex);
    }
    GetRow(tableName, rowIndex) {
        const this$ = this;
        const tupledArg = [ArcTablesAux_findIndexByTableName(tableName, this$.Tables), rowIndex];
        return this$.GetRowAt(tupledArg[0], tupledArg[1]);
    }
    static ofSeq(tables) {
        return new ArcTables(Array.from(tables));
    }
    MoveTable(oldIndex, newIndex) {
        const this$ = this;
        const table = this$.GetTableAt(oldIndex);
        this$.Tables.splice(oldIndex, 1);
        this$.Tables.splice(newIndex, 0, table);
    }
    static updateReferenceTablesBySheets(referenceTables, sheetTables, keepUnusedRefTables) {
        let collection, s, array;
        const keepUnusedRefTables_1 = defaultArg(keepUnusedRefTables, false);
        const usedTables = new Set([]);
        const referenceTableMap = ofSeq_1(choose((t) => map_2((c) => [item(0, c.Cells).AsFreeText, t], t.TryGetProtocolNameColumn()), referenceTables.Tables), {
            Compare: comparePrimitives,
        });
        return new ArcTables((collection = ((s = map_3((t_3) => {
            Unchecked_fillMissingCells(t_3.Headers, t_3.Values);
            return t_3;
        }, map_3((tupledArg) => reduce((table, table_1) => ArcTable.append(table, table_1), tupledArg[1]), Array_groupBy((t_2) => t_2.Name, map_3((t_1) => {
            const k = defaultArg(bind((c_1) => {
                if (c_1.AsFreeText === "") {
                    return undefined;
                }
                else {
                    return c_1.AsFreeText;
                }
            }, bind((i) => t_1.TryGetCellAt(i, 0), tryFindIndex((x_1) => equals(x_1, new CompositeHeader(8, [])), t_1.Headers))), t_1.Name);
            const matchValue = tryFind(k, referenceTableMap);
            if (matchValue == null) {
                return t_1;
            }
            else {
                const rt = matchValue;
                addToSet(k, usedTables);
                const updatedTable = ArcTable.updateReferenceByAnnotationTable(rt, t_1);
                return ArcTable.create(t_1.Name, updatedTable.Headers, updatedTable.Values);
            }
        }, (array = toArray(sheetTables.Tables), collect(ArcTable.SplitByProtocolREF, array))), {
            Equals: (x_2, y_1) => (x_2 === y_1),
            GetHashCode: stringHash,
        }))), keepUnusedRefTables_1 ? append(choose((kv) => {
            if (usedTables.has(kv[0])) {
                return undefined;
            }
            else {
                return kv[1];
            }
        }, referenceTableMap), s) : s)), Array.from(collection)));
    }
    GetEnumerator() {
        const this$ = this;
        return getEnumerator(this$.Tables);
    }
    [Symbol.iterator]() {
        return toIterator(getEnumerator(this));
    }
    "System.Collections.IEnumerable.GetEnumerator"() {
        const this$ = this;
        return getEnumerator(this$.Tables);
    }
}

export function ArcTables_$reflection() {
    return class_type("ARCtrl.ArcTables", undefined, ArcTables);
}

export function ArcTables_$ctor_Z420F2E1A(initTables) {
    return new ArcTables(initTables);
}

