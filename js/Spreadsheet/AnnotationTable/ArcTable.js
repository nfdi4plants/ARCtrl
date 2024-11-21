import { iterateIndexed, length, sortBy, collect as collect_1, exists, toArray, ofArray, empty, tail, head, isEmpty, cons, singleton, fold, map, reverse } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { item, map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { fold as fold_1, singleton as singleton_1, collect, map as map_2, delay, toArray as toArray_1, tryFind } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { toStringCellColumns, fixDeprecatedIOHeader, fromStringCellColumns } from "./CompositeColumn.js";
import { printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { Dictionary_tryGet, FsCellsCollection__TryGetCell_Z37302880 } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Cells/FsCellsCollection.fs.js";
import { rangeDouble } from "../../fable_modules/fable-library-js.4.22.0/Range.js";
import { FsAddress_$ctor_Z37302880, FsAddress__get_ColumnNumber, FsAddress__get_RowNumber } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsAddress.fs.js";
import { FsRangeAddress_$ctor_7E77A4A0, FsRangeAddress__get_LastAddress } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Ranges/FsRangeAddress.fs.js";
import { FsRangeBase__Cell_Z3407A44B, FsRangeBase__get_RangeAddress } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Ranges/FsRangeBase.fs.js";
import { ArcTable } from "../../Core/Table/ArcTable.js";
import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorksheet.fs.js";
import { comparePrimitives } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { max } from "../../fable_modules/fable-library-js.4.22.0/Double.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";

/**
 * Iterates over elements of the input list and groups adjacent elements.
 * A new group is started when the specified predicate holds about the element
 * of the list (and at the beginning of the iteration).
 * 
 * For example:
 * List.groupWhen isOdd [3;3;2;4;1;2] = [[3]; [3; 2; 4]; [1; 2]]
 */
export function Aux_List_groupWhen(f, list) {
    return reverse(map(reverse, fold((acc, e) => {
        const matchValue = f(e);
        if (matchValue) {
            return cons(singleton(e), acc);
        }
        else if (!isEmpty(acc)) {
            return cons(cons(e, head(acc)), tail(acc));
        }
        else {
            return singleton(singleton(e));
        }
    }, empty(), list)));
}

export function classifyHeaderOrder(header) {
    switch (header.tag) {
        case 4:
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
            return 2;
        case 0:
        case 1:
        case 2:
        case 3:
        case 14:
        case 13:
            return 3;
        case 12:
            return 4;
        default:
            return 1;
    }
}

export function classifyColumnOrder(column) {
    return classifyHeaderOrder(column.Header);
}

export const helperColumnStrings = ofArray(["Term Source REF", "Term Accession Number", "Unit", "Data Format", "Data Selector Format"]);

export function groupColumnsByHeader(stringCellColumns) {
    return map_1(toArray, toArray(Aux_List_groupWhen((c) => {
        const v = item(0, c);
        return !exists((s) => v.startsWith(s), helperColumnStrings);
    }, ofArray(stringCellColumns))));
}

/**
 * Returns the annotation table of the worksheet if it exists, else returns None
 */
export function tryAnnotationTable(sheet) {
    return tryFind((t) => t.Name.startsWith("annotationTable"), sheet.Tables);
}

/**
 * Groups and parses a collection of single columns into the according ISA composite columns
 */
export function composeColumns(stringCellColumns) {
    return map_1(fromStringCellColumns, groupColumnsByHeader(stringCellColumns));
}

/**
 * Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
 */
export function tryFromFsWorksheet(sheet) {
    try {
        const matchValue = tryAnnotationTable(sheet);
        if (matchValue == null) {
            return undefined;
        }
        else {
            const t = matchValue;
            const compositeColumns = composeColumns(map_1(fixDeprecatedIOHeader, toArray_1(delay(() => map_2((c) => toArray_1(delay(() => collect((r) => {
                const matchValue_1 = FsCellsCollection__TryGetCell_Z37302880(sheet.CellCollection, r, c);
                if (matchValue_1 == null) {
                    return singleton_1("");
                }
                else {
                    const cell = matchValue_1;
                    return singleton_1(cell.ValueAsString());
                }
            }, rangeDouble(1, 1, FsAddress__get_RowNumber(FsRangeAddress__get_LastAddress(FsRangeBase__get_RangeAddress(t))))))), rangeDouble(1, 1, FsAddress__get_ColumnNumber(FsRangeAddress__get_LastAddress(FsRangeBase__get_RangeAddress(t)))))))));
            return ArcTable.addColumns(compositeColumns, undefined, true)(ArcTable.init(sheet.Name));
        }
    }
    catch (err) {
        const arg = sheet.Name;
        const arg_1 = err.message;
        return toFail(printf("Could not parse table with name \"%s\":\n%s"))(arg)(arg_1);
    }
}

export function toFsWorksheet(index, table) {
    const stringCount = new Map([]);
    const ws = new FsWorksheet(table.Name);
    if (table.ColumnCount === 0) {
        return ws;
    }
    else {
        const columns = collect_1(toStringCellColumns, sortBy(classifyColumnOrder, ofArray(table.Columns), {
            Compare: comparePrimitives,
        }));
        let tableRowCount;
        const maxRow = fold_1((acc, c) => max(acc, length(c)), 0, columns) | 0;
        tableRowCount = ((maxRow === 1) ? 2 : maxRow);
        const tableColumnCount = length(columns) | 0;
        const name = (index == null) ? "annotationTable" : (`${"annotationTable"}${index}`);
        const fsTable = ws.Table(name, FsRangeAddress_$ctor_7E77A4A0(FsAddress_$ctor_Z37302880(1, 1), FsAddress_$ctor_Z37302880(tableRowCount, tableColumnCount)));
        iterateIndexed((colI, col) => {
            iterateIndexed((rowI, stringCell) => {
                let value;
                if (rowI === 0) {
                    const matchValue = Dictionary_tryGet(stringCell, stringCount);
                    if (matchValue == null) {
                        addToDict(stringCount, stringCell, "");
                        value = stringCell;
                    }
                    else {
                        const spaces = matchValue;
                        stringCount.set(stringCell, spaces + " ");
                        value = ((stringCell + " ") + spaces);
                    }
                }
                else {
                    value = stringCell;
                }
                const address = FsAddress_$ctor_Z37302880(rowI + 1, colI + 1);
                FsRangeBase__Cell_Z3407A44B(fsTable, address, ws.CellCollection).SetValueAs(value);
            }, col);
        }, columns);
        return ws;
    }
}

