import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.3.0/FsWorksheet.fs.js";
import { iterate, isEmpty as isEmpty_1, choose, tryHead, head, exists, map, singleton, append, delay, iterateIndexed } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { SparseRowModule_tryGetValueAt, SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_writeToSheet } from "./SparseTable.js";
import { fromRows as fromRows_1, toRows as toRows_1 } from "./InvestigationFile/Assays.js";
import { empty, isEmpty, toArray, ofArray, singleton as singleton_1 } from "../../fable_modules/fable-library.4.1.4/List.js";
import { fromRows as fromRows_2, toRows as toRows_2 } from "./InvestigationFile/Contacts.js";
import { getEnumerator } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { ArcAssay } from "../ISA/ArcTypes/ArcAssay.js";
import { createMissingIdentifier } from "../ISA/ArcTypes/Identifier.js";
import { defaultArg } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { printf, toConsole } from "../../fable_modules/fable-library.4.1.4/String.js";
import { toFsWorksheet, tryFromFsWorksheet } from "./ArcTable.js";
import { FsWorkbook } from "../../fable_modules/FsSpreadsheet.3.3.0/FsWorkbook.fs.js";

export function toMetadataSheet(assay) {
    let assay_1;
    const sheet = new FsWorksheet("isa_assay");
    iterateIndexed((rowI, r) => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, (assay_1 = assay, delay(() => append(singleton(SparseRowModule_fromValues(["ASSAY"])), delay(() => append(toRows_1("Assay", singleton_1(assay_1)), delay(() => append(singleton(SparseRowModule_fromValues(["ASSAY PERFORMERS"])), delay(() => toRows_2("Assay Person", ofArray(assay_1.Performers)))))))))));
    return sheet;
}

export function fromMetadataSheet(sheet) {
    const rows_1 = map(SparseRowModule_fromFsRow, sheet.Rows);
    const patternInput = exists((row) => {
        const s = head(row)[1];
        return s.indexOf("Assay") === 0;
    }, rows_1) ? ["Assay", "Assay Person"] : [void 0, void 0];
    const en = getEnumerator(rows_1);
    const loop = (lastLine_mut, assays_mut, contacts_mut, lineNumber_mut) => {
        let k;
        loop:
        while (true) {
            const lastLine = lastLine_mut, assays = assays_mut, contacts = contacts_mut, lineNumber = lineNumber_mut;
            let matchResult, k_2, k_3, k_4;
            if (lastLine != null) {
                if ((k = lastLine, (k === "ASSAY") ? true : (k === "ASSAY METADATA"))) {
                    matchResult = 0;
                    k_2 = lastLine;
                }
                else if (lastLine === "ASSAY PERFORMERS") {
                    matchResult = 1;
                    k_3 = lastLine;
                }
                else {
                    matchResult = 2;
                    k_4 = lastLine;
                }
            }
            else {
                matchResult = 2;
                k_4 = lastLine;
            }
            switch (matchResult) {
                case 0: {
                    const patternInput_1 = fromRows_1(patternInput[0], lineNumber + 1, en);
                    lastLine_mut = patternInput_1[0];
                    assays_mut = patternInput_1[3];
                    contacts_mut = contacts;
                    lineNumber_mut = patternInput_1[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_2 = fromRows_2(patternInput[1], lineNumber + 1, en);
                    lastLine_mut = patternInput_2[0];
                    assays_mut = assays;
                    contacts_mut = patternInput_2[3];
                    lineNumber_mut = patternInput_2[1];
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
                            const arg = toArray(contacts_2);
                            const arg_1 = defaultArg(tryHead(assays_2), ArcAssay.create(createMissingIdentifier()));
                            return ArcAssay.setPerformers(arg, arg_1);
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

/**
 * Reads an assay from a spreadsheet
 */
export function fromFsWorkbook(doc) {
    let assayMetaData;
    const matchValue = doc.TryGetWorksheetByName("isa_assay");
    if (matchValue == null) {
        const matchValue_1 = doc.TryGetWorksheetByName("Assay");
        if (matchValue_1 == null) {
            toConsole(printf("Cannot retrieve metadata: Assay file does not contain \"%s\" or \"%s\" sheet."))("isa_assay")("Assay");
            assayMetaData = ArcAssay.create(createMissingIdentifier());
        }
        else {
            assayMetaData = fromMetadataSheet(matchValue_1);
        }
    }
    else {
        assayMetaData = fromMetadataSheet(matchValue);
    }
    const sheets = choose(tryFromFsWorksheet, doc.GetWorksheets());
    if (isEmpty_1(sheets)) {
        return assayMetaData;
    }
    else {
        assayMetaData.Tables = Array.from(sheets);
        return assayMetaData;
    }
}

export function toFsWorkbook(assay) {
    const doc = new FsWorkbook();
    const metaDataSheet = toMetadataSheet(assay);
    doc.AddWorksheet(metaDataSheet);
    iterate((arg_1) => {
        const arg = toFsWorksheet(arg_1);
        doc.AddWorksheet(arg);
    }, assay.Tables);
    return doc;
}

