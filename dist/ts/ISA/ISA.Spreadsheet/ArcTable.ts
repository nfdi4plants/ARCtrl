import { iterateIndexed, length, sortBy, collect, toArray, empty, tail, head, isEmpty, cons, singleton, fold, FSharpList, map, reverse } from "../../fable_modules/fable-library-ts/List.js";
import { CompositeHeader_$union } from "../ISA/ArcTypes/CompositeHeader.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { CompositeColumn } from "../ISA/ArcTypes/CompositeColumn.js";
import { tryParseReferenceColumnHeader } from "../ISA/Regex.js";
import { FsColumn } from "../../fable_modules/FsSpreadsheet.3.3.0/FsColumn.fs.js";
import { map as map_1, toList, tryFind } from "../../fable_modules/fable-library-ts/Seq.js";
import { FsTable } from "../../fable_modules/FsSpreadsheet.3.3.0/Tables/FsTable.fs.js";
import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.3.0/FsWorksheet.fs.js";
import { value as value_1, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { toFsColumns, fixDeprecatedIOHeader, fromFsColumns } from "./CompositeColumn.js";
import { ArcTable } from "../ISA/ArcTypes/ArcTable.js";
import { comparePrimitives, IMap } from "../../fable_modules/fable-library-ts/Util.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.3.3.0/Cells/FsCell.fs.js";
import { FsRangeAddress_$ctor_7E77A4A0 } from "../../fable_modules/FsSpreadsheet.3.3.0/Ranges/FsRangeAddress.fs.js";
import { FsAddress, FsAddress_$ctor_Z37302880 } from "../../fable_modules/FsSpreadsheet.3.3.0/FsAddress.fs.js";
import { Dictionary_tryGet } from "../../fable_modules/FsSpreadsheet.3.3.0/Cells/FsCellsCollection.fs.js";
import { addToDict } from "../../fable_modules/fable-library-ts/MapUtil.js";
import { FsRangeBase__Cell_Z3407A44B } from "../../fable_modules/FsSpreadsheet.3.3.0/Ranges/FsRangeBase.fs.js";

/**
 * Iterates over elements of the input list and groups adjacent elements.
 * A new group is started when the specified predicate holds about the element
 * of the list (and at the beginning of the iteration).
 * 
 * For example:
 * List.groupWhen isOdd [3;3;2;4;1;2] = [[3]; [3; 2; 4]; [1; 2]]
 */
export function Aux_List_groupWhen<$a>(f: ((arg0: $a) => boolean), list: FSharpList<$a>): FSharpList<FSharpList<$a>> {
    return reverse<FSharpList<$a>>(map<FSharpList<$a>, FSharpList<$a>>(reverse, fold<$a, FSharpList<FSharpList<$a>>>((acc: FSharpList<FSharpList<$a>>, e: $a): FSharpList<FSharpList<$a>> => {
        const matchValue: boolean = f(e);
        if (matchValue) {
            return cons(singleton(e), acc);
        }
        else if (!isEmpty(acc)) {
            return cons(cons(e, head(acc)), tail(acc));
        }
        else {
            return singleton(singleton(e));
        }
    }, empty<FSharpList<$a>>(), list)));
}

export function classifyHeaderOrder(header: CompositeHeader_$union): int32 {
    switch (header.tag) {
        case /* ProtocolType */ 4:
        case /* ProtocolDescription */ 5:
        case /* ProtocolUri */ 6:
        case /* ProtocolVersion */ 7:
        case /* ProtocolREF */ 8:
        case /* Performer */ 9:
        case /* Date */ 10:
            return 2;
        case /* Component */ 0:
        case /* Characteristic */ 1:
        case /* Factor */ 2:
        case /* Parameter */ 3:
        case /* FreeText */ 13:
            return 3;
        case /* Output */ 12:
            return 4;
        default:
            return 1;
    }
}

export function classifyColumnOrder(column: CompositeColumn): int32 {
    return classifyHeaderOrder(column.Header);
}

export function groupColumnsByHeader(columns: FSharpList<FsColumn>): FSharpList<FSharpList<FsColumn>> {
    return Aux_List_groupWhen<FsColumn>((c: FsColumn): boolean => {
        if (tryParseReferenceColumnHeader(c.Item(1).Value) == null) {
            return !(c.Item(1).Value.indexOf("Unit") === 0);
        }
        else {
            return false;
        }
    }, columns);
}

/**
 * Returns the annotation table of the worksheet if it exists, else returns None
 */
export function tryAnnotationTable(sheet: FsWorksheet): Option<FsTable> {
    return tryFind<FsTable>((t: FsTable): boolean => (t.Name.indexOf("annotationTable") === 0), sheet.Tables);
}

/**
 * Groups and parses a collection of single columns into the according ISA composite columns
 */
export function composeColumns(columns: Iterable<FsColumn>): CompositeColumn[] {
    return toArray<CompositeColumn>(map<FSharpList<FsColumn>, CompositeColumn>(fromFsColumns, groupColumnsByHeader(toList<FsColumn>(columns))));
}

/**
 * Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
 */
export function tryFromFsWorksheet(sheet: FsWorksheet): Option<ArcTable> {
    const matchValue: Option<FsTable> = tryAnnotationTable(sheet);
    if (matchValue == null) {
        return void 0;
    }
    else {
        const t: FsTable = value_1(matchValue);
        const compositeColumns: CompositeColumn[] = composeColumns(map_1<FsColumn, FsColumn>(fixDeprecatedIOHeader, t.GetColumns(sheet.CellCollection)));
        return ArcTable.addColumns(compositeColumns)(ArcTable.init(sheet.Name));
    }
}

export function toFsWorksheet(table: ArcTable): FsWorksheet {
    const stringCount: IMap<string, string> = new Map<string, string>([]);
    const ws: FsWorksheet = new FsWorksheet(table.Name);
    const columns: FSharpList<FSharpList<FsCell>> = collect<CompositeColumn, FSharpList<FsCell>>(toFsColumns, sortBy<CompositeColumn, int32>(classifyColumnOrder, table.Columns, {
        Compare: comparePrimitives,
    }));
    const maxRow: int32 = length(head(columns)) | 0;
    const maxCol: int32 = length(columns) | 0;
    const fsTable: FsTable = ws.Table("annotationTable", FsRangeAddress_$ctor_7E77A4A0(FsAddress_$ctor_Z37302880(1, 1), FsAddress_$ctor_Z37302880(maxRow, maxCol)));
    iterateIndexed<FSharpList<FsCell>>((colI: int32, col: FSharpList<FsCell>): void => {
        iterateIndexed<FsCell>((rowI: int32, cell: FsCell): void => {
            let value: string;
            if (rowI === 0) {
                const matchValue: Option<string> = Dictionary_tryGet<string, string>(cell.Value, stringCount);
                if (matchValue == null) {
                    addToDict(stringCount, cell.Value, "");
                    value = cell.Value;
                }
                else {
                    const spaces: string = value_1(matchValue);
                    stringCount.set(cell.Value, spaces + " ");
                    value = ((cell.Value + " ") + spaces);
                }
            }
            else {
                value = cell.Value;
            }
            const address: FsAddress = FsAddress_$ctor_Z37302880(rowI + 1, colI + 1);
            FsRangeBase__Cell_Z3407A44B(fsTable, address, ws.CellCollection).SetValueAs<any>(value);
        }, col);
    }, columns);
    return ws;
}

