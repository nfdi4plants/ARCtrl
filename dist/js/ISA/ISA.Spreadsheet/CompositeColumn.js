import { CompositeHeader, IOType } from "../ISA/ArcTypes/CompositeHeader.js";
import { toString } from "../../fable_modules/fable-library.4.1.4/Types.js";
import { toFsCells, fromFsCells } from "./CompositeHeader.js";
import { singleton as singleton_1, ofArray, item, map } from "../../fable_modules/fable-library.4.1.4/List.js";
import { FsAddress__get_RowNumber } from "../../fable_modules/FsSpreadsheet.3.3.0/FsAddress.fs.js";
import { FsRangeAddress__get_LastAddress } from "../../fable_modules/FsSpreadsheet.3.3.0/Ranges/FsRangeAddress.fs.js";
import { FsRangeBase__get_RangeAddress } from "../../fable_modules/FsSpreadsheet.3.3.0/Ranges/FsRangeBase.fs.js";
import { singleton, append, toList, exists, map as map_1, delay, toArray } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { toFsCells as toFsCells_1, fromFsCells as fromFsCells_1 } from "./CompositeCell.js";
import { rangeDouble } from "../../fable_modules/fable-library.4.1.4/Range.js";
import { CompositeColumn } from "../ISA/ArcTypes/CompositeColumn.js";
import { map as map_2 } from "../../fable_modules/fable-library.4.1.4/Array.js";

/**
 * Checks if the column header is a deprecated IO Header. If so, fixes it.
 * 
 * The old format of IO Headers was only the type of IO so, e.g. "Source Name" or "Raw Data File".
 * 
 * A "Source Name" column will now be mapped to the propper "Input [Source Name]", and all other IO types will be mapped to "Output [<IO Type>]".
 */
export function fixDeprecatedIOHeader(col) {
    const matchValue = IOType.ofString(col.Item(1).Value);
    switch (matchValue.tag) {
        case 6:
            return col;
        case 0: {
            col.Item(1).SetValueAs(toString(new CompositeHeader(11, [new IOType(0, [])])));
            return col;
        }
        default: {
            col.Item(1).SetValueAs(toString(new CompositeHeader(12, [matchValue])));
            return col;
        }
    }
}

export function fromFsColumns(columns) {
    const header = fromFsCells(map((c) => c.Item(1), columns));
    const l = FsAddress__get_RowNumber(FsRangeAddress__get_LastAddress(FsRangeBase__get_RangeAddress(item(0, columns)))) | 0;
    const cells_2 = toArray(delay(() => map_1((i) => fromFsCells_1(map((c_1) => c_1.Item(i), columns)), rangeDouble(2, 1, l))));
    return CompositeColumn.create(header, cells_2);
}

export function toFsColumns(column) {
    const hasUnit = exists((c) => c.isUnitized, column.Cells);
    const isTerm = column.Header.IsTermColumn;
    const header = toFsCells(hasUnit, column.Header);
    const cells = map_2((cell) => toFsCells_1(isTerm, hasUnit, cell), column.Cells);
    if (hasUnit) {
        return ofArray([toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i) => item(0, cells[i]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(1, header)), delay(() => map_1((i_1) => item(1, cells[i_1]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(2, header)), delay(() => map_1((i_2) => item(2, cells[i_2]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(3, header)), delay(() => map_1((i_3) => item(3, cells[i_3]), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else if (isTerm) {
        return ofArray([toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i_4) => item(0, cells[i_4]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(1, header)), delay(() => map_1((i_5) => item(1, cells[i_5]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList(delay(() => append(singleton(item(2, header)), delay(() => map_1((i_6) => item(2, cells[i_6]), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else {
        return singleton_1(toList(delay(() => append(singleton(item(0, header)), delay(() => map_1((i_7) => item(0, cells[i_7]), rangeDouble(0, 1, column.Cells.length - 1)))))));
    }
}

