import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.3.0/FsWorksheet.fs.js";
import { iterate, isEmpty, choose, map, singleton, append, delay, iterateIndexed } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_writeToSheet } from "./SparseTable.js";
import { fromRows as fromRows_1, StudyInfo_toRows_1B3D5E9B } from "./InvestigationFile/Study.js";
import { defaultArg } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { getEnumerator } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { ArcStudy } from "../ISA/ArcTypes/ArcStudy.js";
import { createMissingIdentifier } from "../ISA/ArcTypes/Identifier.js";
import { printf, toConsole } from "../../fable_modules/fable-library.4.1.4/String.js";
import { toFsWorksheet, tryFromFsWorksheet } from "./ArcTable.js";
import { FsWorkbook } from "../../fable_modules/FsSpreadsheet.3.3.0/FsWorkbook.fs.js";

export function toMetadataSheet(study) {
    const sheet = new FsWorksheet("isa_study");
    iterateIndexed((rowI, r) => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, delay(() => append(singleton(SparseRowModule_fromValues(["STUDY"])), delay(() => StudyInfo_toRows_1B3D5E9B(study)))));
    return sheet;
}

export function fromMetadataSheet(sheet) {
    let en;
    return defaultArg((en = getEnumerator(map(SparseRowModule_fromFsRow, sheet.Rows)), (void en["System.Collections.IEnumerator.MoveNext"](), fromRows_1(2, en)[3])), ArcStudy.create(createMissingIdentifier()));
}

/**
 * Reads an assay from a spreadsheet
 */
export function fromFsWorkbook(doc) {
    let studyMetadata;
    const matchValue = doc.TryGetWorksheetByName("isa_study");
    if (matchValue == null) {
        const matchValue_1 = doc.TryGetWorksheetByName("Study");
        if (matchValue_1 == null) {
            toConsole(printf("Cannot retrieve metadata: Study file does not contain \"%s\" or \"%s\" sheet."))("isa_study")("Study");
            studyMetadata = ArcStudy.create(createMissingIdentifier());
        }
        else {
            studyMetadata = fromMetadataSheet(matchValue_1);
        }
    }
    else {
        studyMetadata = fromMetadataSheet(matchValue);
    }
    const sheets = choose(tryFromFsWorksheet, doc.GetWorksheets());
    if (isEmpty(sheets)) {
        return studyMetadata;
    }
    else {
        studyMetadata.Tables = Array.from(sheets);
        return studyMetadata;
    }
}

export function toFsWorkbook(study) {
    const doc = new FsWorkbook();
    const metaDataSheet = toMetadataSheet(study);
    doc.AddWorksheet(metaDataSheet);
    iterate((arg_1) => {
        const arg = toFsWorksheet(arg_1);
        doc.AddWorksheet(arg);
    }, study.Tables);
    return doc;
}

