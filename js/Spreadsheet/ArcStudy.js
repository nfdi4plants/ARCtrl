import { FsWorksheet } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorksheet.fs.js";
import { iterate, tryPick, tryFind, map, append, iterateIndexed } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { SparseRowModule_fromAllValues, SparseRowModule_getAllValues, SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_writeToSheet } from "./Metadata/SparseTable.js";
import { fromRows, toRows } from "./Metadata/Study.js";
import { getEnumerator } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { toConsole, printf, toFail } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { unwrap, toArray, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ArcStudy } from "../Core/ArcTypes.js";
import { createMissingIdentifier } from "../Core/Helper/Identifier.js";
import { empty } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { ResizeArray_isEmpty, ResizeArray_choose } from "../Core/Helper/Collections.js";
import { toFsWorksheet as toFsWorksheet_1, tryFromFsWorksheet } from "./AnnotationTable/ArcTable.js";
import { toFsWorksheet, tryFromFsWorksheet as tryFromFsWorksheet_1 } from "./DataMapTable/DataMapTable.js";
import { FsWorkbook } from "../fable_modules/FsSpreadsheet.6.3.0-alpha.4/FsWorkbook.fs.js";

export function ArcStudy_toMetadataSheet(study, assays) {
    let source_1;
    const sheet = new FsWorksheet("isa_study");
    iterateIndexed((rowI, r) => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, (source_1 = toRows(study, assays), append([SparseRowModule_fromValues(["STUDY"])], source_1)));
    return sheet;
}

export function ArcStudy_fromRows(rows) {
    const en = getEnumerator(rows);
    en["System.Collections.IEnumerator.MoveNext"]();
    return fromRows(2, en)[3];
}

export function ArcStudy_fromMetadataSheet(sheet) {
    try {
        return defaultArg(ArcStudy_fromRows(map(SparseRowModule_fromFsRow, sheet.Rows)), [ArcStudy.create(createMissingIdentifier()), empty()]);
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Failed while parsing metadatasheet: %s"))(arg);
    }
}

export function ArcStudy_toMetadataCollection(study, assays) {
    let source_1;
    return map(SparseRowModule_getAllValues, (source_1 = toRows(study, assays), append([SparseRowModule_fromValues(["STUDY"])], source_1)));
}

export function ArcStudy_fromMetadataCollection(collection) {
    try {
        return defaultArg(ArcStudy_fromRows(map(SparseRowModule_fromAllValues, collection)), [ArcStudy.create(createMissingIdentifier()), empty()]);
    }
    catch (err) {
        const arg = err.message;
        return toFail(printf("Failed while parsing metadatasheet: %s"))(arg);
    }
}

export function ArcStudy_isMetadataSheetName(name) {
    if (name === "isa_study") {
        return true;
    }
    else {
        return name === "Study";
    }
}

export function ArcStudy_isMetadataSheet(sheet) {
    return ArcStudy_isMetadataSheetName(sheet.Name);
}

export function ArcStudy_tryGetMetadataSheet(doc) {
    return tryFind(ArcStudy_isMetadataSheet, doc.GetWorksheets());
}

/**
 * Reads an assay from a spreadsheet
 */
export function ARCtrl_ArcStudy__ArcStudy_fromFsWorkbook_Static_32154C9D(doc) {
    try {
        let patternInput;
        const matchValue = ArcStudy_tryGetMetadataSheet(doc);
        if (matchValue == null) {
            toConsole(printf("Cannot retrieve metadata: Study file does not contain \"%s\" or \"%s\" sheet."))("isa_study")("Study");
            patternInput = [ArcStudy.create(createMissingIdentifier()), empty()];
        }
        else {
            patternInput = ArcStudy_fromMetadataSheet(matchValue);
        }
        const studyMetadata = patternInput[0];
        const sheets = doc.GetWorksheets();
        const annotationTables = ResizeArray_choose(tryFromFsWorksheet, sheets);
        const datamapSheet = tryPick(tryFromFsWorksheet_1, sheets);
        if (!ResizeArray_isEmpty(annotationTables)) {
            studyMetadata.Tables = annotationTables;
        }
        studyMetadata.DataMap = datamapSheet;
        return [studyMetadata, patternInput[1]];
    }
    catch (err) {
        const arg_2 = err.message;
        return toFail(printf("Could not parse study: \n%s"))(arg_2);
    }
}

/**
 * Write a study to a spreadsheet
 * 
 * If datamapSheet is true, the datamap will be written to a worksheet inside study workbook. Default: true
 */
export function ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(study, assays, datamapSheet) {
    const datamapSheet_1 = defaultArg(datamapSheet, true);
    const doc = new FsWorkbook();
    const metadataSheet = ArcStudy_toMetadataSheet(study, assays);
    doc.AddWorksheet(metadataSheet);
    if (datamapSheet_1) {
        iterate((arg) => {
            const sheet = toFsWorksheet(arg);
            doc.AddWorksheet(sheet);
        }, toArray(study.DataMap));
    }
    iterateIndexed((i, arg_1) => {
        const sheet_1 = toFsWorksheet_1(i, arg_1);
        doc.AddWorksheet(sheet_1);
    }, study.Tables);
    return doc;
}

export function ARCtrl_ArcStudy__ArcStudy_ToFsWorkbook_257FC1F0(this$, assays, datamapSheet) {
    return ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(this$, unwrap(assays), unwrap(datamapSheet));
}

