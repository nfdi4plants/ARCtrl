import { iterate, isEmpty as isEmpty_1, tryPick, choose, tryFind, map, iterateIndexed, singleton, append, delay, tryHead, head, exists } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { getEnumerator } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { toRows, fromRows } from "./Metadata/Assays.js";
import { toRows as toRows_1, fromRows as fromRows_1 } from "./Metadata/Contacts.js";
import { ArcAssay } from "../Core/ArcTypes.js";
import { createMissingIdentifier } from "../Core/Helper/Identifier.js";
import { unwrap, toArray, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ofSeq, singleton as singleton_1, empty, isEmpty } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { SparseRowModule_fromAllValues, SparseRowModule_getAllValues, SparseRowModule_fromFsRow, SparseRowModule_writeToSheet, SparseRowModule_fromValues, SparseRowModule_tryGetValueAt } from "./Metadata/SparseTable.js";
import { FsWorksheet } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorksheet.fs.js";
import { toConsole, printf, toFail } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toFsWorksheet as toFsWorksheet_1, tryFromFsWorksheet } from "./AnnotationTable/ArcTable.js";
import { toFsWorksheet, tryFromFsWorksheet as tryFromFsWorksheet_1 } from "./DataMapTable/DataMapTable.js";
import { FsWorkbook } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorkbook.fs.js";

export function ArcAssay_fromRows(rows) {
    const patternInput = exists((row) => {
        const s = head(row)[1];
        return s.startsWith("Assay");
    }, rows) ? ["Assay", "Assay Person"] : [undefined, undefined];
    const en = getEnumerator(rows);
    const loop = (lastRow_mut, assays_mut, contacts_mut, rowNumber_mut) => {
        let prefix;
        loop:
        while (true) {
            const lastRow = lastRow_mut, assays = assays_mut, contacts = contacts_mut, rowNumber = rowNumber_mut;
            let matchResult, prefix_2, prefix_3;
            if (lastRow != null) {
                if ((prefix = lastRow, (prefix === "ASSAY") ? true : (prefix === "ASSAY METADATA"))) {
                    matchResult = 0;
                    prefix_2 = lastRow;
                }
                else if (lastRow === "ASSAY PERFORMERS") {
                    matchResult = 1;
                    prefix_3 = lastRow;
                }
                else {
                    matchResult = 2;
                }
            }
            else {
                matchResult = 2;
            }
            switch (matchResult) {
                case 0: {
                    const patternInput_1 = fromRows(patternInput[0], rowNumber + 1, en);
                    lastRow_mut = patternInput_1[0];
                    assays_mut = patternInput_1[3];
                    contacts_mut = contacts;
                    rowNumber_mut = patternInput_1[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_2 = fromRows_1(patternInput[1], rowNumber + 1, en);
                    lastRow_mut = patternInput_2[0];
                    assays_mut = assays;
                    contacts_mut = patternInput_2[3];
                    rowNumber_mut = patternInput_2[1];
                    continue loop;
                }
                default: {
                    let matchResult_1, assays_2, contacts_2;
                    if (isEmpty(assays)) {
                        if (isEmpty(contacts)) {
                            matchResult_1 = 0;
                        }
                        else {
                            matchResult_1 = 1;
                            assays_2 = assays;
                            contacts_2 = contacts;
                        }
                    }
                    else {
                        matchResult_1 = 1;
                        assays_2 = assays;
                        contacts_2 = contacts;
                    }
                    switch (matchResult_1) {
                        case 0:
                            return ArcAssay.create(createMissingIdentifier());
                        default: {
                            const performers = Array.from(contacts_2);
                            const assay = defaultArg(tryHead(assays_2), ArcAssay.create(createMissingIdentifier()));
                            return ArcAssay.setPerformers(performers, assay);
                        }
                    }
                }
            }
            break;
        }
    };
    if (en["System.Collections.IEnumerator.MoveNext"]()) {
        return loop(SparseRowModule_tryGetValueAt(0, en["System.Collections.Generic.IEnumerator`1.get_Current"]()), empty(), empty(), 1);
    }
    else {
        throw new Error("empty assay metadata sheet");
    }
}

export function ArcAssay_toRows(assay) {
    return delay(() => append(singleton(SparseRowModule_fromValues(["ASSAY"])), delay(() => append(toRows("Assay", singleton_1(assay)), delay(() => append(singleton(SparseRowModule_fromValues(["ASSAY PERFORMERS"])), delay(() => toRows_1("Assay Person", ofSeq(assay.Performers)))))))));
}

export function ArcAssay_toMetadataSheet(assay) {
    const sheet = new FsWorksheet("isa_assay");
    iterateIndexed((rowI, r) => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, ArcAssay_toRows(assay));
    return sheet;
}

export function ArcAssay_fromMetadataSheet(sheet) {
    try {
        const rows = map(SparseRowModule_fromFsRow, sheet.Rows);
        const hasPrefix = exists((row) => {
            const s = head(row)[1];
            return s.startsWith("Assay");
        }, rows);
        return ArcAssay_fromRows(rows);
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Failed while parsing metadatasheet: %s"))(arg);
    }
}

export function ArcAssay_toMetadataCollection(assay) {
    return map(SparseRowModule_getAllValues, ArcAssay_toRows(assay));
}

export function ArcAssay_fromMetadataCollection(collection) {
    try {
        return ArcAssay_fromRows(map(SparseRowModule_fromAllValues, collection));
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Failed while parsing metadatasheet: %s"))(arg);
    }
}

export function ArcAssay_isMetadataSheetName(name) {
    if (name === "isa_assay") {
        return true;
    }
    else {
        return name === "Assay";
    }
}

export function ArcAssay_isMetadataSheet(sheet) {
    return ArcAssay_isMetadataSheetName(sheet.Name);
}

export function ArcAssay_tryGetMetadataSheet(doc) {
    return tryFind(ArcAssay_isMetadataSheet, doc.GetWorksheets());
}

/**
 * Reads an assay from a spreadsheet
 */
export function ARCtrl_ArcAssay__ArcAssay_fromFsWorkbook_Static_32154C9D(doc) {
    try {
        let assayMetadata;
        const matchValue = ArcAssay_tryGetMetadataSheet(doc);
        if (matchValue == null) {
            toConsole(printf("Cannot retrieve metadata: Assay file does not contain \"%s\" or \"%s\" sheet."))("isa_assay")("Assay");
            assayMetadata = ArcAssay.create(createMissingIdentifier());
        }
        else {
            assayMetadata = ArcAssay_fromMetadataSheet(matchValue);
        }
        const sheets = doc.GetWorksheets();
        const annotationTables = choose(tryFromFsWorksheet, sheets);
        const datamapSheet = tryPick(tryFromFsWorksheet_1, sheets);
        if (!isEmpty_1(annotationTables)) {
            assayMetadata.Tables = Array.from(annotationTables);
        }
        assayMetadata.DataMap = datamapSheet;
        return assayMetadata;
    }
    catch (err) {
        const arg_2 = err.message;
        return toFail(printf("Could not parse assay: \n%s"))(arg_2);
    }
}

/**
 * Write an assay to a spreadsheet
 * 
 * If datamapSheet is true, the datamap will be written to a worksheet inside assay workbook.
 */
export function ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(assay, datamapSheet) {
    const datamapSheet_1 = defaultArg(datamapSheet, true);
    const doc = new FsWorkbook();
    const metadataSheet = ArcAssay_toMetadataSheet(assay);
    doc.AddWorksheet(metadataSheet);
    if (datamapSheet_1) {
        iterate((arg) => {
            const sheet = toFsWorksheet(arg);
            doc.AddWorksheet(sheet);
        }, toArray(assay.DataMap));
    }
    iterateIndexed((i, arg_1) => {
        const sheet_1 = toFsWorksheet_1(i, arg_1);
        doc.AddWorksheet(sheet_1);
    }, assay.Tables);
    return doc;
}

/**
 * Write an assay to a spreadsheet
 * 
 * If datamapSheet is true, the datamap will be written to a worksheet inside assay workbook.
 */
export function ARCtrl_ArcAssay__ArcAssay_ToFsWorkbook_6FCE9E49(this$, datamapSheet) {
    return ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(this$, unwrap(datamapSheet));
}

