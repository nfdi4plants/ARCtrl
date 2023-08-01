import { FsWorksheet } from "../../fable_modules/FsSpreadsheet.3.1.1/FsWorksheet.fs.js";
import { iterate, map, singleton, append, delay, iterateIndexed } from "../../fable_modules/fable-library-ts/Seq.js";
import { SparseRowModule_fromFsRow, SparseRowModule_fromValues, SparseRowModule_writeToSheet } from "./SparseTable.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { fromRows as fromRows_1, StudyInfo_toRows_331096F } from "./InvestigationFile/Study.js";
import { ArcStudy } from "../ISA/ArcTypes/ArcStudy.js";
import { value as value_2, Option, defaultArg } from "../../fable_modules/fable-library-ts/Option.js";
import { IEnumerator, getEnumerator } from "../../fable_modules/fable-library-ts/Util.js";
import { FsRow } from "../../fable_modules/FsSpreadsheet.3.1.1/FsRow.fs.js";
import { createMissingIdentifier } from "../ISA/ArcTypes/Identifier.js";
import { printf, toConsole } from "../../fable_modules/fable-library-ts/String.js";
import { isEmpty, FSharpList, choose } from "../../fable_modules/fable-library-ts/List.js";
import { toFsWorksheet, tryFromFsWorksheet } from "./ArcTable.js";
import { ArcTable } from "../ISA/ArcTypes/ArcTable.js";
import { FsWorkbook } from "../../fable_modules/FsSpreadsheet.3.1.1/FsWorkbook.fs.js";

export function toMetadataSheet(study: ArcStudy): FsWorksheet {
    const sheet: FsWorksheet = new FsWorksheet("isa_study");
    iterateIndexed<Iterable<[int32, string]>>((rowI: int32, r: Iterable<[int32, string]>): void => {
        SparseRowModule_writeToSheet(rowI + 1, r, sheet);
    }, delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => append<Iterable<[int32, string]>>(singleton<Iterable<[int32, string]>>(SparseRowModule_fromValues(["STUDY"])), delay<Iterable<[int32, string]>>((): Iterable<Iterable<[int32, string]>> => StudyInfo_toRows_331096F(study)))));
    return sheet;
}

export function fromMetadataSheet(sheet: FsWorksheet): ArcStudy {
    let en: IEnumerator<Iterable<[int32, string]>>;
    return defaultArg((en = getEnumerator(map<FsRow, Iterable<[int32, string]>>(SparseRowModule_fromFsRow, sheet.Rows)), (void en["System.Collections.IEnumerator.MoveNext"](), fromRows_1(2, en)[3])), ArcStudy.create(createMissingIdentifier()));
}

/**
 * Reads an assay from a spreadsheet
 */
export function fromFsWorkbook(doc: FsWorkbook): ArcStudy {
    let studyMetadata: ArcStudy;
    const matchValue: Option<FsWorksheet> = doc.TryGetWorksheetByName("isa_study");
    if (matchValue == null) {
        const matchValue_1: Option<FsWorksheet> = doc.TryGetWorksheetByName("Study");
        if (matchValue_1 == null) {
            toConsole(printf("Cannot retrieve metadata: Study file does not contain \"%s\" or \"%s\" sheet."))("isa_study")("Study");
            studyMetadata = ArcStudy.create(createMissingIdentifier());
        }
        else {
            studyMetadata = fromMetadataSheet(value_2(matchValue_1));
        }
    }
    else {
        studyMetadata = fromMetadataSheet(value_2(matchValue));
    }
    const sheets: FSharpList<ArcTable> = choose<FsWorksheet, ArcTable>(tryFromFsWorksheet, doc.GetWorksheets());
    if (isEmpty(sheets)) {
        return studyMetadata;
    }
    else {
        studyMetadata.Tables = Array.from(sheets);
        return studyMetadata;
    }
}

export function toFsWorkbook(study: ArcStudy): FsWorkbook {
    const doc: FsWorkbook = new FsWorkbook();
    const metaDataSheet: FsWorksheet = toMetadataSheet(study);
    doc.AddWorksheet(metaDataSheet);
    iterate<ArcTable>((arg_1: ArcTable): void => {
        const arg: FsWorksheet = toFsWorksheet(arg_1);
        doc.AddWorksheet(arg);
    }, study.Tables);
    return doc;
}

