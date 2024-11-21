import { tryItem, map, setItem, item, skip } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { CompositeHeader, IOType } from "../../Core/Table/CompositeHeader.js";
import { toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { toStringCells, fromStringCells } from "./CompositeHeader.js";
import { singleton, append, toList, exists, map as map_1, delay, toArray } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { rangeDouble } from "../../fable_modules/fable-library-js.4.22.0/Range.js";
import { CompositeColumn } from "../../Core/Table/CompositeColumn.js";
import { toStringCells as toStringCells_1 } from "./CompositeCell.js";
import { defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { map as map_2, singleton as singleton_1, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Cells/FsCell.fs.js";

/**
 * Checks if the column header is a deprecated IO Header. If so, fixes it.
 * 
 * The old format of IO Headers was only the type of IO so, e.g. "Source Name" or "Raw Data File".
 * 
 * A "Source Name" column will now be mapped to the propper "Input [Source Name]", and all other IO types will be mapped to "Output [<IO Type>]".
 */
export function fixDeprecatedIOHeader(stringCellCol) {
    if (stringCellCol.length === 0) {
        throw new Error("Can\'t fix IOHeader Invalid column, neither header nor values given");
    }
    const values = skip(1, stringCellCol);
    const matchValue = IOType.ofString(item(0, stringCellCol));
    switch (matchValue.tag) {
        case 4:
            return stringCellCol;
        case 0: {
            setItem(stringCellCol, 0, toString(new CompositeHeader(11, [new IOType(0, [])])));
            return stringCellCol;
        }
        default: {
            setItem(stringCellCol, 0, toString(new CompositeHeader(12, [matchValue])));
            return stringCellCol;
        }
    }
}

export function fromStringCellColumns(columns) {
    const patternInput = fromStringCells(map((c) => item(0, c), columns));
    const l = item(0, columns).length | 0;
    const cells = toArray(delay(() => map_1((i) => patternInput[1](map((c_1) => item(i, c_1), columns)), rangeDouble(1, 1, l - 1))));
    return CompositeColumn.create(patternInput[0], cells);
}

export function fromFsColumns(columns) {
    return fromStringCellColumns(map((c) => {
        c.ToDenseColumn();
        return map((c_1) => c_1.ValueAsString(), toArray(c.Cells));
    }, columns));
}

export function toStringCellColumns(column) {
    const hasUnit = exists((c) => c.isUnitized, column.Cells);
    const isTerm = column.Header.IsTermColumn;
    const isData = column.Header.IsDataColumn && exists((c_1) => c_1.isData, column.Cells);
    const header = toStringCells(hasUnit, column.Header);
    const cells = map((cell) => toStringCells_1(isTerm, hasUnit, cell), column.Cells);
    const getCellOrDefault = (ri, ci, cells_1) => defaultArg(tryItem(ci, item(ri, cells_1)), "");
    if (hasUnit) {
        return ofArray([toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i) => getCellOrDefault(i, 0, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(1, header)), delay(() => map_1((i_1) => getCellOrDefault(i_1, 1, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(2, header)), delay(() => map_1((i_2) => getCellOrDefault(i_2, 2, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(3, header)), delay(() => map_1((i_3) => getCellOrDefault(i_3, 3, cells), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else if (isTerm) {
        return ofArray([toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i_4) => getCellOrDefault(i_4, 0, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(1, header)), delay(() => map_1((i_5) => getCellOrDefault(i_5, 1, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(2, header)), delay(() => map_1((i_6) => getCellOrDefault(i_6, 2, cells), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else if (isData) {
        return ofArray([toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i_7) => getCellOrDefault(i_7, 0, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(1, header)), delay(() => map_1((i_8) => getCellOrDefault(i_8, 1, cells), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(2, header)), delay(() => map_1((i_9) => getCellOrDefault(i_9, 2, cells), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else {
        return singleton_1(toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i_10) => item(0, item(i_10, cells)), rangeDouble(0, 1, column.Cells.length - 1)))))));
    }
}

export function toFsColumns(column) {
    return map_2((c) => map_2((s) => (new FsCell(s)), c), toStringCellColumns(column));
}

