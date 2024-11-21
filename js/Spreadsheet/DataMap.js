import { printf, toFail } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { tryPick } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { toFsWorksheet, tryFromFsWorksheet } from "./DataMapTable/DataMapTable.js";
import { FsWorkbook } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorkbook.fs.js";

/**
 * Reads an datamap from a spreadsheet
 */
export function fromFsWorkbook(doc) {
    try {
        const dataMapTable = tryPick(tryFromFsWorksheet, doc.GetWorksheets());
        if (dataMapTable == null) {
            throw new Error("No DataMapTable found in workbook");
        }
        else {
            return dataMapTable;
        }
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Could not parse datamap: \n%s"))(arg);
    }
}

export function toFsWorkbook(dataMap) {
    const doc = new FsWorkbook();
    const sheet = toFsWorksheet(dataMap);
    doc.AddWorksheet(sheet);
    return doc;
}

