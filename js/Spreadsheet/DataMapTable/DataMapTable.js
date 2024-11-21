import { iterateIndexed, head, length, iterate, exists, ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { Aux_List_groupWhen } from "../AnnotationTable/ArcTable.js";
import { toList, map, delay, toArray, item, tryFind } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { DataContext_$ctor_Z780A8A2A } from "../../Core/DataContext.js";
import { rangeDouble } from "../../fable_modules/fable-library-js.4.22.0/Range.js";
import { toFsColumns, setFromFsColumns } from "./DataMapColumn.js";
import { printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { DataMap__get_DataContexts, DataMap_$ctor_4E3220A7 } from "../../Core/DataMap.js";
import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorksheet.fs.js";
import { FsRangeAddress_$ctor_7E77A4A0 } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Ranges/FsRangeAddress.fs.js";
import { FsAddress_$ctor_Z37302880 } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsAddress.fs.js";
import { Dictionary_tryGet } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Cells/FsCellsCollection.fs.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { FsRangeBase__Cell_Z3407A44B } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Ranges/FsRangeBase.fs.js";

export const helperColumnStrings = ofArray(["Term Source REF", "Term Accession Number", "Data Format", "Data Selector Format"]);

export function groupColumnsByHeader(columns) {
    return Aux_List_groupWhen((c) => {
        const v = c.Item(1).ValueAsString();
        return !exists((s) => v.startsWith(s), helperColumnStrings);
    }, columns);
}

/**
 * Returns the annotation table of the worksheet if it exists, else returns None
 */
export function tryDataMapTable(sheet) {
    return tryFind((t) => t.Name.startsWith("datamapTable"), sheet.Tables);
}

/**
 * Groups and parses a collection of single columns into the according ISA composite columns
 */
export function composeColumns(columns) {
    const l = (item(0, columns).MaxRowIndex - 1) | 0;
    const dc = Array.from(toArray(delay(() => map((i) => DataContext_$ctor_Z780A8A2A(), rangeDouble(0, 1, l - 1)))));
    iterate((arg) => {
        setFromFsColumns(dc, arg);
    }, groupColumnsByHeader(toList(columns)));
    return dc;
}

/**
 * Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
 */
export function tryFromFsWorksheet(sheet) {
    try {
        const matchValue = tryDataMapTable(sheet);
        if (matchValue == null) {
            return undefined;
        }
        else {
            const t = matchValue;
            return DataMap_$ctor_4E3220A7(composeColumns(t.GetColumns(sheet.CellCollection)));
        }
    }
    catch (err) {
        const arg = sheet.Name;
        const arg_1 = err.message;
        return toFail(printf("Could not parse datamap table with name \"%s\":\n%s"))(arg)(arg_1);
    }
}

export function toFsWorksheet(table) {
    const stringCount = new Map([]);
    const ws = new FsWorksheet("isa_datamap");
    if (DataMap__get_DataContexts(table).length === 0) {
        return ws;
    }
    else {
        const columns = toFsColumns(DataMap__get_DataContexts(table));
        const maxRow = length(head(columns)) | 0;
        const maxCol = length(columns) | 0;
        const fsTable = ws.Table("datamapTable", FsRangeAddress_$ctor_7E77A4A0(FsAddress_$ctor_Z37302880(1, 1), FsAddress_$ctor_Z37302880(maxRow, maxCol)));
        iterateIndexed((colI, col) => {
            iterateIndexed((rowI, cell) => {
                let value;
                const v = cell.ValueAsString();
                if (rowI === 0) {
                    const matchValue = Dictionary_tryGet(v, stringCount);
                    if (matchValue == null) {
                        addToDict(stringCount, cell.ValueAsString(), "");
                        value = v;
                    }
                    else {
                        const spaces = matchValue;
                        stringCount.set(v, spaces + " ");
                        value = ((v + " ") + spaces);
                    }
                }
                else {
                    value = v;
                }
                const address = FsAddress_$ctor_Z37302880(rowI + 1, colI + 1);
                FsRangeBase__Cell_Z3407A44B(fsTable, address, ws.CellCollection).SetValueAs(value);
            }, col);
        }, columns);
        ws.RescanRows();
        return ws;
    }
}

