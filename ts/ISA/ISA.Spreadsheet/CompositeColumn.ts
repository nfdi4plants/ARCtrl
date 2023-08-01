import { CompositeHeader_$union, CompositeHeader_Output, CompositeHeader_Input, IOType_Source, IOType_$union, IOType } from "../ISA/ArcTypes/CompositeHeader.js";
import { toString } from "../../fable_modules/fable-library-ts/Types.js";
import { FsColumn } from "../../fable_modules/FsSpreadsheet.3.1.1/FsColumn.fs.js";
import { toFsCells, fromFsCells } from "./CompositeHeader.js";
import { singleton as singleton_1, ofArray, FSharpList, item, map } from "../../fable_modules/fable-library-ts/List.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.3.1.1/Cells/FsCell.fs.js";
import { FsAddress__get_RowNumber } from "../../fable_modules/FsSpreadsheet.3.1.1/FsAddress.fs.js";
import { FsRangeAddress__get_LastAddress } from "../../fable_modules/FsSpreadsheet.3.1.1/Ranges/FsRangeAddress.fs.js";
import { FsRangeBase__get_RangeAddress } from "../../fable_modules/FsSpreadsheet.3.1.1/Ranges/FsRangeBase.fs.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { singleton, append, toList, exists, map as map_1, delay, toArray } from "../../fable_modules/fable-library-ts/Seq.js";
import { toFsCells as toFsCells_1, fromFsCells as fromFsCells_1 } from "./CompositeCell.js";
import { CompositeCell_$union } from "../ISA/ArcTypes/CompositeCell.js";
import { rangeDouble } from "../../fable_modules/fable-library-ts/Range.js";
import { CompositeColumn } from "../ISA/ArcTypes/CompositeColumn.js";
import { map as map_2 } from "../../fable_modules/fable-library-ts/Array.js";

/**
 * Checks if the column header is a deprecated IO Header. If so, fixes it.
 * 
 * The old format of IO Headers was only the type of IO so, e.g. "Source Name" or "Raw Data File".
 * 
 * A "Source Name" column will now be mapped to the propper "Input [Source Name]", and all other IO types will be mapped to "Output [<IO Type>]".
 */
export function fixDeprecatedIOHeader(col: FsColumn): FsColumn {
    const matchValue: IOType_$union = IOType.ofString(col.Item(1).Value);
    switch (matchValue.tag) {
        case /* FreeText */ 6:
            return col;
        case /* Source */ 0: {
            col.Item(1).SetValueAs<any>(toString(CompositeHeader_Input(IOType_Source())));
            return col;
        }
        default: {
            col.Item(1).SetValueAs<any>(toString(CompositeHeader_Output(matchValue)));
            return col;
        }
    }
}

export function fromFsColumns(columns: FSharpList<FsColumn>): CompositeColumn {
    const header: CompositeHeader_$union = fromFsCells(map<FsColumn, FsCell>((c: FsColumn): FsCell => c.Item(1), columns));
    const l: int32 = FsAddress__get_RowNumber(FsRangeAddress__get_LastAddress(FsRangeBase__get_RangeAddress(item(0, columns)))) | 0;
    const cells_2: CompositeCell_$union[] = toArray<CompositeCell_$union>(delay<CompositeCell_$union>((): Iterable<CompositeCell_$union> => map_1<int32, CompositeCell_$union>((i: int32): CompositeCell_$union => fromFsCells_1(map<FsColumn, FsCell>((c_1: FsColumn): FsCell => c_1.Item(i), columns)), rangeDouble(2, 1, l))));
    return CompositeColumn.create(header, cells_2);
}

export function toFsColumns(column: CompositeColumn): FSharpList<FSharpList<FsCell>> {
    const hasUnit: boolean = exists<CompositeCell_$union>((c: CompositeCell_$union): boolean => c.isUnitized, column.Cells);
    const isTerm: boolean = column.Header.IsTermColumn;
    const header: FSharpList<FsCell> = toFsCells(hasUnit, column.Header);
    const cells: FSharpList<FsCell>[] = map_2<CompositeCell_$union, FSharpList<FsCell>>((cell: CompositeCell_$union): FSharpList<FsCell> => toFsCells_1(isTerm, hasUnit, cell), column.Cells);
    if (hasUnit) {
        return ofArray([toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(0, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i: int32): FsCell => item(0, cells[i]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(1, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_1: int32): FsCell => item(1, cells[i_1]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(2, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_2: int32): FsCell => item(2, cells[i_2]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(3, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_3: int32): FsCell => item(3, cells[i_3]), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else if (isTerm) {
        return ofArray([toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(0, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_4: int32): FsCell => item(0, cells[i_4]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(1, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_5: int32): FsCell => item(1, cells[i_5]), rangeDouble(0, 1, column.Cells.length - 1)))))), toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(2, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_6: int32): FsCell => item(2, cells[i_6]), rangeDouble(0, 1, column.Cells.length - 1))))))]);
    }
    else {
        return singleton_1(toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton<FsCell>(item(0, header)), delay<FsCell>((): Iterable<FsCell> => map_1<int32, FsCell>((i_7: int32): FsCell => item(0, cells[i_7]), rangeDouble(0, 1, column.Cells.length - 1)))))));
    }
}

