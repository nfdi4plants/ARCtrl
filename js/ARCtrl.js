import { tryFind, exactlyOne, choose, map } from "./fable_modules/fable-library.4.1.4/Array.js";
import { fromFsWorkbook } from "./ISA/ISA.Spreadsheet/ArcAssay.js";
import { tryFromContract } from "./Contracts/Contracts.ArcAssay.js";
import { fromFsWorkbook as fromFsWorkbook_1 } from "./ISA/ISA.Spreadsheet/ArcStudy.js";
import { tryFromContract as tryFromContract_1 } from "./Contracts/Contracts.ArcStudy.js";
import { fromFsWorkbook as fromFsWorkbook_2 } from "./ISA/ISA.Spreadsheet/InvestigationFile/Investigation.js";
import { tryFromContract as tryFromContract_2 } from "./Contracts/Contracts.ArcInvestigation.js";
import { FileSystem_$reflection, FileSystem as FileSystem_1 } from "./FileSystem/FileSystem.js";
import { tryISAReadContractFromPath } from "./Contracts/Contracts.ARCtrl.js";
import { defaultArg } from "./fable_modules/fable-library.4.1.4/Option.js";
import { iterate } from "./fable_modules/fable-library.4.1.4/Seq.js";
import { printf, toConsole } from "./fable_modules/fable-library.4.1.4/String.js";
import { Record } from "./fable_modules/fable-library.4.1.4/Types.js";
import { ArcInvestigation_$reflection } from "./ISA/ISA/ArcTypes/ArcInvestigation.js";
import { record_type, unit_type, option_type } from "./fable_modules/fable-library.4.1.4/Reflection.js";

export function ARCAux_getArcAssaysFromContracts(contracts) {
    return map(fromFsWorkbook, choose(tryFromContract, contracts));
}

export function ARCAux_getArcStudiesFromContracts(contracts) {
    return map(fromFsWorkbook_1, choose(tryFromContract_1, contracts));
}

export function ARCAux_getArcInvestigationFromContracts(contracts) {
    return fromFsWorkbook_2(exactlyOne(choose(tryFromContract_2, contracts)));
}

export class ARC extends Record {
    constructor(ISA, CWL, FileSystem) {
        super();
        this.ISA = ISA;
        this.CWL = CWL;
        this.FileSystem = FileSystem;
    }
    static create(isa, cwl, fs) {
        return new ARC(isa, cwl, fs);
    }
    static fromFilePaths(filePaths) {
        const fs = FileSystem_1.fromFilePaths(filePaths);
        return ARC.create(void 0, void 0, fs);
    }
    addFSFromFilePaths(filePaths) {
        const this$ = this;
        return new ARC(this$.ISA, this$.CWL, FileSystem_1.fromFilePaths(filePaths));
    }
    getReadContracts() {
        const this$ = this;
        const matchValue = this$.FileSystem;
        if (matchValue == null) {
            throw new Error("Cannot create READ contracts from ARC without FileSystem.\r\n\r\nYou could initialized your ARC with `ARC.fromFilePaths` or run `yourArc.addFSFromFilePaths` to avoid this issue.");
        }
        else {
            const fs = matchValue;
            return choose(tryISAReadContractFromPath, fs.Tree.ToFilePaths());
        }
    }
    addISAFromContracts(contracts, enableLogging) {
        const this$ = this;
        const enableLogging_1 = defaultArg(enableLogging, false);
        const investigation = ARCAux_getArcInvestigationFromContracts(contracts);
        const studies = ARCAux_getArcStudiesFromContracts(contracts);
        const assays = ARCAux_getArcAssaysFromContracts(contracts);
        const copy = investigation.Copy();
        iterate((studyRegisteredIdent) => {
            const studyOpt = tryFind((s) => (s.Identifier === studyRegisteredIdent), studies);
            if (studyOpt == null) {
                if (enableLogging_1) {
                    toConsole(printf("Unable to find registered study \'%s\' in fullfilled READ contracts!"))(studyRegisteredIdent);
                }
            }
            else {
                const study = studyOpt;
                if (enableLogging_1) {
                    toConsole(printf("Found study: %s"))(studyRegisteredIdent);
                }
                iterate((assayRegisteredIdent) => {
                    const assayOpt = tryFind((a) => (a.Identifier === assayRegisteredIdent), assays);
                    if (assayOpt == null) {
                        if (enableLogging_1) {
                            toConsole(printf("Unable to find registered assay \'%s\' in fullfilled READ contracts!"))(assayRegisteredIdent);
                        }
                    }
                    else {
                        const assay = assayOpt;
                        if (enableLogging_1) {
                            toConsole(printf("Found assay: %s - %s"))(studyRegisteredIdent)(assayRegisteredIdent);
                        }
                        study.AddAssay(assay);
                    }
                }, copy.GetStudy(studyRegisteredIdent).AssayIdentifiers);
                investigation.SetStudy(studyRegisteredIdent, study);
            }
        }, copy.StudyIdentifiers);
        return new ARC(investigation, this$.CWL, this$.FileSystem);
    }
}

export function ARC_$reflection() {
    return record_type("ARCtrl.ARC", [], ARC, () => [["ISA", option_type(ArcInvestigation_$reflection())], ["CWL", option_type(unit_type)], ["FileSystem", option_type(FileSystem_$reflection())]]);
}

