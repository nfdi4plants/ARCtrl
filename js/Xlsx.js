import { toFsWorkbook, fromFsWorkbook } from "./Spreadsheet/DataMap.js";
import { FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static } from "./fable_modules/FsSpreadsheet.Js.6.3.0-alpha.4/FsExtensions.fs.js";
import { class_type } from "./fable_modules/fable-library-js.4.22.0/Reflection.js";
import { ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F, ARCtrl_ArcAssay__ArcAssay_fromFsWorkbook_Static_32154C9D } from "./Spreadsheet/ArcAssay.js";
import { ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522, ARCtrl_ArcStudy__ArcStudy_fromFsWorkbook_Static_32154C9D } from "./Spreadsheet/ArcStudy.js";
import { map, unwrap } from "./fable_modules/fable-library-js.4.22.0/Option.js";
import { ofSeq } from "./fable_modules/fable-library-js.4.22.0/List.js";
import { ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF, ARCtrl_ArcInvestigation__ArcInvestigation_fromFsWorkbook_Static_32154C9D } from "./Spreadsheet/ArcInvestigation.js";

export class XlsxHelper_DatamapXlsx {
    constructor() {
    }
    fromFsWorkbook(fswb) {
        return fromFsWorkbook(fswb);
    }
    toFsWorkbook(datamap) {
        return toFsWorkbook(datamap);
    }
    toXlsxFile(path, datamap) {
        return FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static(path, toFsWorkbook(datamap));
    }
}

export function XlsxHelper_DatamapXlsx_$reflection() {
    return class_type("ARCtrl.XlsxHelper.DatamapXlsx", undefined, XlsxHelper_DatamapXlsx);
}

export function XlsxHelper_DatamapXlsx_$ctor() {
    return new XlsxHelper_DatamapXlsx();
}

export class XlsxHelper_AssayXlsx {
    constructor() {
    }
    fromFsWorkbook(fswb) {
        return ARCtrl_ArcAssay__ArcAssay_fromFsWorkbook_Static_32154C9D(fswb);
    }
    toFsWorkbook(assay) {
        return ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(assay);
    }
    toXlsxFile(path, assay) {
        return FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static(path, ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(assay));
    }
}

export function XlsxHelper_AssayXlsx_$reflection() {
    return class_type("ARCtrl.XlsxHelper.AssayXlsx", undefined, XlsxHelper_AssayXlsx);
}

export function XlsxHelper_AssayXlsx_$ctor() {
    return new XlsxHelper_AssayXlsx();
}

export class XlsxHelper_StudyXlsx {
    constructor() {
    }
    fromFsWorkbook(fswb) {
        return ARCtrl_ArcStudy__ArcStudy_fromFsWorkbook_Static_32154C9D(fswb);
    }
    toFsWorkbook(study, assays) {
        return ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(study, unwrap(map(ofSeq, assays)));
    }
    toXlsxFile(path, study, assays) {
        return FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static(path, ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(study, unwrap(map(ofSeq, assays))));
    }
}

export function XlsxHelper_StudyXlsx_$reflection() {
    return class_type("ARCtrl.XlsxHelper.StudyXlsx", undefined, XlsxHelper_StudyXlsx);
}

export function XlsxHelper_StudyXlsx_$ctor() {
    return new XlsxHelper_StudyXlsx();
}

export class XlsxHelper_InvestigationXlsx {
    constructor() {
    }
    fromFsWorkbook(fswb) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_fromFsWorkbook_Static_32154C9D(fswb);
    }
    toFsWorkbook(investigation) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(investigation);
    }
    toXlsxFile(path, investigation) {
        return FsSpreadsheet_FsWorkbook__FsWorkbook_toXlsxFile_Static(path, ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(investigation));
    }
}

export function XlsxHelper_InvestigationXlsx_$reflection() {
    return class_type("ARCtrl.XlsxHelper.InvestigationXlsx", undefined, XlsxHelper_InvestigationXlsx);
}

export function XlsxHelper_InvestigationXlsx_$ctor() {
    return new XlsxHelper_InvestigationXlsx();
}

export class XlsxController {
    constructor() {
    }
    static get Datamap() {
        return new XlsxHelper_DatamapXlsx();
    }
    static get Assay() {
        return new XlsxHelper_AssayXlsx();
    }
    static get Study() {
        return new XlsxHelper_StudyXlsx();
    }
    static get Investigation() {
        return new XlsxHelper_InvestigationXlsx();
    }
}

export function XlsxController_$reflection() {
    return class_type("ARCtrl.XlsxController", undefined, XlsxController);
}

