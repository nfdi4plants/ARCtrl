import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.1.1/FsWorksheet.fs.js";
import { iterate, tryHead, head, exists, map, singleton, append, delay, iterateIndexed } from "../../fable_modules/fable-library-ts/Seq.js";
import { SparseRowModule_tryGetValueAt, SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_writeToSheet } from "./SparseTable.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { fromRows as fromRows_1, toRows as toRows_1 } from "./InvestigationFile/Assays.js";
import { choose, empty, isEmpty, FSharpList, singleton as singleton_1 } from "../../fable_modules/fable-library-ts/List.js";
import { fromRows as fromRows_2, toRows as toRows_2 } from "./InvestigationFile/Contacts.js";
import { ArcAssay } from "../ISA/ArcTypes/ArcAssay.js";
import { FsRow } from "../../fable_modules/FsSpreadsheet.3.1.1/FsRow.fs.js";
import { value as value_1, defaultArg, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { IEnumerator, getEnumerator } from "../../fable_modules/fable-library-ts/Util.js";
import { Remark } from "../ISA/JsonTypes/Comment.js";
import { Person } from "../ISA/JsonTypes/Person.js";
import { createMissingIdentifier } from "../ISA/ArcTypes/Identifier.js";
import { printf, toConsole } from "../../fable_modules/fable-library-ts/String.js";
import { toFsWorksheet, tryFromFsWorksheet } from "./ArcTable.js";
import { ArcTable } from "../ISA/ArcTypes/ArcTable.js";
import { FsWorkbook } from "../../fable_modules/FsSpreadsheet.3.1.1/FsWorkbook.fs.js";

export function toMetadataSheet(assay: ArcAssay): FsWorksheet {
    let assay_1: ArcAssay;
    const sheet: FsWorksheet = new FsWorksheet("isa_assay");
    iterateIndexed<Iterable<[int32, string]>>((rowI: int32, r: Iterable<[int32, string]>): void => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, (assay_1 = assay, delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["ASSAY"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append<Iterable<[int32, string]>>(toRows_1("Assay", singleton_1(assay_1)), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["ASSAY PERFORMERS"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => toRows_2("Assay Person", assay_1.Performers))))))))));
    return sheet;
}

export function fromMetadataSheet(sheet: FsWorksheet): ArcAssay {
    const rows_1: Iterable<Iterable<[int32, string]>> = map<FsRow, Iterable<[int32, string]>>(SparseRowModule_fromFsRow, sheet.Rows);
    const patternInput: [Option<string>, Option<string>] = exists<Iterable<[int32, string]>>((row: Iterable<[int32, string]>): boolean => {
        const s: string = head<[int32, string]>(row)[1];
        return s.indexOf("Assay") === 0;
    }, rows_1) ? (["Assay", "Assay Person"] as [Option<string>, Option<string>]) : ([void 0, void 0] as [Option<string>, Option<string>]);
    const en: IEnumerator<Iterable<[int32, string]>> = getEnumerator(rows_1);
    const loop = (lastLine_mut: Option<string>, assays_mut: FSharpList<ArcAssay>, contacts_mut: FSharpList<Person>, lineNumber_mut: int32): ArcAssay => {
        let k: string;
        loop:
        while (true) {
            const lastLine: Option<string> = lastLine_mut, assays: FSharpList<ArcAssay> = assays_mut, contacts: FSharpList<Person> = contacts_mut, lineNumber: int32 = lineNumber_mut;
            let matchResult: int32, k_2: string, k_3: string, k_4: Option<string>;
            if (lastLine != null) {
                if ((k = value_1(lastLine), (k === "ASSAY") ? true : (k === "ASSAY METADATA"))) {
                    matchResult = 0;
                    k_2 = value_1(lastLine);
                }
                else if (value_1(lastLine) === "ASSAY PERFORMERS") {
                    matchResult = 1;
                    k_3 = value_1(lastLine);
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
                    const patternInput_1: [Option<string>, int32, FSharpList<Remark>, FSharpList<ArcAssay>] = fromRows_1(patternInput[0], lineNumber + 1, en);
                    lastLine_mut = patternInput_1[0];
                    assays_mut = patternInput_1[3];
                    contacts_mut = contacts;
                    lineNumber_mut = patternInput_1[1];
                    continue loop;
                }
                case 1: {
                    const patternInput_2: [Option<string>, int32, FSharpList<Remark>, FSharpList<Person>] = fromRows_2(patternInput[1], lineNumber + 1, en);
                    lastLine_mut = patternInput_2[0];
                    assays_mut = assays;
                    contacts_mut = patternInput_2[3];
                    lineNumber_mut = patternInput_2[1];
                    continue loop;
                }
                default: {
                    let matchResult_1: int32, assays_2: FSharpList<ArcAssay>, contacts_2: FSharpList<Person>;
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
                            const arg_1: ArcAssay = defaultArg(tryHead<ArcAssay>(assays_2!), ArcAssay.create(createMissingIdentifier()));
                            return ArcAssay.setPerformers(contacts_2!, arg_1);
                        }
                    }
                }
            }
            break;
        }
    };
    if (en["System.Collections.IEnumerator.MoveNext"]()) {
        return loop(SparseRowModule_tryGetValueAt(0, en["System.Collections.Generic.IEnumerator`1.get_Current"]()), empty<ArcAssay>(), empty<Person>(), 1);
    }
    else {
        throw new Error("empty assay metadata sheet");
    }
}

/**
 * Reads an assay from a spreadsheet
 */
export function fromFsWorkbook(doc: FsWorkbook): ArcAssay {
    let assayMetaData: ArcAssay;
    const matchValue: Option<FsWorksheet> = doc.TryGetWorksheetByName("isa_assay");
    if (matchValue == null) {
        const matchValue_1: Option<FsWorksheet> = doc.TryGetWorksheetByName("Assay");
        if (matchValue_1 == null) {
            toConsole(printf("Cannot retrieve metadata: Assay file does not contain \"%s\" or \"%s\" sheet."))("isa_assay")("Assay");
            assayMetaData = ArcAssay.create(createMissingIdentifier());
        }
        else {
            assayMetaData = fromMetadataSheet(value_1(matchValue_1));
        }
    }
    else {
        assayMetaData = fromMetadataSheet(value_1(matchValue));
    }
    const sheets: FSharpList<ArcTable> = choose<FsWorksheet, ArcTable>(tryFromFsWorksheet, doc.GetWorksheets());
    if (isEmpty(sheets)) {
        return assayMetaData;
    }
    else {
        assayMetaData.Tables = Array.from(sheets);
        return assayMetaData;
    }
}

export function toFsWorkbook(assay: ArcAssay): FsWorkbook {
    const doc: FsWorkbook = new FsWorkbook();
    const metaDataSheet: FsWorksheet = toMetadataSheet(assay);
    doc.AddWorksheet(metaDataSheet);
    iterate<ArcTable>((arg_1: ArcTable): void => {
        const arg: FsWorksheet = toFsWorksheet(arg_1);
        doc.AddWorksheet(arg);
    }, assay.Tables);
    return doc;
}

