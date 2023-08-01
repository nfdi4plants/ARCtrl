import { iterateIndexed, length, sortBy, collect, toArray, empty, tail, head, isEmpty, cons, singleton, fold, map, reverse } from "../../fable_modules/fable-library.4.1.4/List.js";
import { tryParseTermAnnotation } from "../ISA/Regex.js";
import { map as map_1, toList, tryFind } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { toFsColumns, fixDeprecatedIOHeader, fromFsColumns } from "./CompositeColumn.js";
import { ArcTable } from "../ISA/ArcTypes/ArcTable.js";
import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.1.1/FsWorksheet.fs.js";
import { comparePrimitives } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { FsRangeAddress_$ctor_7E77A4A0 } from "../../fable_modules/FsSpreadsheet.3.1.1/Ranges/FsRangeAddress.fs.js";
import { FsAddress_$ctor_Z37302880 } from "../../fable_modules/FsSpreadsheet.3.1.1/FsAddress.fs.js";
import { FsRangeBase__Cell_Z3407A44B } from "../../fable_modules/FsSpreadsheet.3.1.1/Ranges/FsRangeBase.fs.js";

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

export function groupColumnsByHeader(columns) {
    return Aux_List_groupWhen((c) => {
        if (tryParseTermAnnotation(c.Item(1).Value) == null) {
            return c.Item(1).Value !== "Unit";
        }
        else {
            return false;
        }
    }, columns);
}

/**
 * Returns the annotation table of the worksheet if it exists, else returns None
 */
export function tryAnnotationTable(sheet) {
    return tryFind((t) => (t.Name.indexOf("annotationTable") === 0), sheet.Tables);
}

/**
 * Groups and parses a collection of single columns into the according ISA composite columns
 */
export function composeColumns(columns) {
    return toArray(map(fromFsColumns, groupColumnsByHeader(toList(columns))));
}

/**
 * Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
 */
export function tryFromFsWorksheet(sheet) {
    const matchValue = tryAnnotationTable(sheet);
    if (matchValue == null) {
        return void 0;
    }
    else {
        const t = matchValue;
        const compositeColumns = composeColumns(map_1(fixDeprecatedIOHeader, t.GetColumns(sheet.CellCollection)));
        return ArcTable.addColumns(compositeColumns)(ArcTable.init(sheet.Name));
    }
}

export function toFsWorksheet(table) {
    const ws = new FsWorksheet(table.Name);
    const columns = collect(toFsColumns, sortBy(classifyColumnOrder, table.Columns, {
        Compare: comparePrimitives,
    }));
    const maxRow = length(head(columns)) | 0;
    const maxCol = length(columns) | 0;
    const fsTable = ws.Table("annotationTable", FsRangeAddress_$ctor_7E77A4A0(FsAddress_$ctor_Z37302880(1, 1), FsAddress_$ctor_Z37302880(maxRow, maxCol)));
    iterateIndexed((colI, col) => {
        iterateIndexed((rowI, cell) => {
            const address = FsAddress_$ctor_Z37302880(rowI + 1, colI + 1);
            FsRangeBase__Cell_Z3407A44B(fsTable, address, ws.CellCollection).SetValueAs(cell.Value);
        }, col);
    }, columns);
    return ws;
}

