import { tryFind, exactlyOne, choose, map } from "./fable_modules/fable-library-ts/Array.js";
import { fromFsWorkbook } from "./ISA/ISA.Spreadsheet/ArcAssay.js";
import { FsWorkbook } from "./fable_modules/FsSpreadsheet.3.1.1/FsWorkbook.fs.js";
import { ArcAssay } from "./ISA/ISA/ArcTypes/ArcAssay.js";
import { tryFromContract } from "./Contracts/Contracts.ArcAssay.js";
import { Contract } from "./Contract/Contract.js";
import { defaultArg, value, Option } from "./fable_modules/fable-library-ts/Option.js";
import { fromFsWorkbook as fromFsWorkbook_1 } from "./ISA/ISA.Spreadsheet/ArcStudy.js";
import { ArcStudy } from "./ISA/ISA/ArcTypes/ArcStudy.js";
import { tryFromContract as tryFromContract_1 } from "./Contracts/Contracts.ArcStudy.js";
import { fromFsWorkbook as fromFsWorkbook_2 } from "./ISA/ISA.Spreadsheet/InvestigationFile/Investigation.js";
import { tryFromContract as tryFromContract_2 } from "./Contracts/Contracts.ArcInvestigation.js";
import { ArcInvestigation_$reflection, ArcInvestigation } from "./ISA/ISA/ArcTypes/ArcInvestigation.js";
import { FileSystem_$reflection, FileSystem as FileSystem_1 } from "./FileSystem/FileSystem.js";
import { tryISAReadContractFromPath } from "./Contracts/Contracts.ARCtrl.js";
import { iterate } from "./fable_modules/fable-library-ts/Seq.js";
import { printf, toConsole } from "./fable_modules/fable-library-ts/String.js";
import { Record } from "./fable_modules/fable-library-ts/Types.js";
import { IEquatable } from "./fable_modules/fable-library-ts/Util.js";
import { record_type, unit_type, option_type, TypeInfo } from "./fable_modules/fable-library-ts/Reflection.js";

export function ARCAux_getArcAssaysFromContracts(contracts: Contract[]): ArcAssay[] {
    return map<FsWorkbook, ArcAssay>(fromFsWorkbook, choose<Contract, FsWorkbook>(tryFromContract, contracts));
}

export function ARCAux_getArcStudiesFromContracts(contracts: Contract[]): ArcStudy[] {
    return map<FsWorkbook, ArcStudy>(fromFsWorkbook_1, choose<Contract, FsWorkbook>(tryFromContract_1, contracts));
}

export function ARCAux_getArcInvestigationFromContracts(contracts: Contract[]): ArcInvestigation {
    return fromFsWorkbook_2(exactlyOne<FsWorkbook>(choose<Contract, FsWorkbook>(tryFromContract_2, contracts)));
}

export class ARC extends Record implements IEquatable<ARC> {
    readonly ISA: Option<ArcInvestigation>;
    readonly CWL: Option<void>;
    readonly FileSystem: Option<FileSystem_1>;
    constructor(ISA: Option<ArcInvestigation>, CWL: Option<void>, FileSystem: Option<FileSystem_1>) {
        super();
        this.ISA = ISA;
        this.CWL = CWL;
        this.FileSystem = FileSystem;
    }
    static create(isa?: ArcInvestigation, cwl?: Option<void>, fs?: FileSystem_1): ARC {
        return new ARC(isa, cwl, fs);
    }
    static fromFilePaths(filePaths: string[]): ARC {
        const fs: FileSystem_1 = FileSystem_1.fromFilePaths(filePaths);
        return ARC.create(void 0, void 0, fs);
    }
    addFSFromFilePaths(filePaths: string[]): ARC {
        const this$: ARC = this;
        return new ARC(this$.ISA, this$.CWL, FileSystem_1.fromFilePaths(filePaths));
    }
    getReadContracts(): Contract[] {
        const this$: ARC = this;
        const matchValue: Option<FileSystem_1> = this$.FileSystem;
        if (matchValue == null) {
            throw new Error("Cannot create READ contracts from ARC without FileSystem.\r\n\r\nYou could initialized your ARC with `ARC.fromFilePaths` or run `yourArc.addFSFromFilePaths` to avoid this issue.");
        }
        else {
            const fs: FileSystem_1 = value(matchValue);
            return choose<string, Contract>(tryISAReadContractFromPath, fs.Tree.ToFilePaths());
        }
    }
    addISAFromContracts(contracts: Contract[], enableLogging?: boolean): ARC {
        const this$: ARC = this;
        const enableLogging_1: boolean = defaultArg<boolean>(enableLogging, false);
        const investigation: ArcInvestigation = ARCAux_getArcInvestigationFromContracts(contracts);
        const studies: ArcStudy[] = ARCAux_getArcStudiesFromContracts(contracts);
        const assays: ArcAssay[] = ARCAux_getArcAssaysFromContracts(contracts);
        const copy: ArcInvestigation = investigation.Copy();
        iterate<string>((studyRegisteredIdent: string): void => {
            const studyOpt: Option<ArcStudy> = tryFind<ArcStudy>((s: ArcStudy): boolean => (s.Identifier === studyRegisteredIdent), studies);
            if (studyOpt == null) {
                if (enableLogging_1) {
                    toConsole(printf("Unable to find registered study \'%s\' in fullfilled READ contracts!"))(studyRegisteredIdent);
                }
            }
            else {
                const study: ArcStudy = value(studyOpt);
                if (enableLogging_1) {
                    toConsole(printf("Found study: %s"))(studyRegisteredIdent);
                }
                iterate<string>((assayRegisteredIdent: string): void => {
                    const assayOpt: Option<ArcAssay> = tryFind<ArcAssay>((a: ArcAssay): boolean => (a.Identifier === assayRegisteredIdent), assays);
                    if (assayOpt == null) {
                        if (enableLogging_1) {
                            toConsole(printf("Unable to find registered assay \'%s\' in fullfilled READ contracts!"))(assayRegisteredIdent);
                        }
                    }
                    else {
                        const assay: ArcAssay = value(assayOpt);
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

export function ARC_$reflection(): TypeInfo {
    return record_type("ARCtrl.ARC", [], ARC, () => [["ISA", option_type(ArcInvestigation_$reflection())], ["CWL", option_type(unit_type)], ["FileSystem", option_type(FileSystem_$reflection())]]);
}

