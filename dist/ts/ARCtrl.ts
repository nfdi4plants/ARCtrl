import { tryFind, append, exactlyOne, choose, map } from "./fable_modules/fable-library-ts/Array.js";
import { toFsWorkbook as toFsWorkbook_2, fromFsWorkbook } from "./ISA/ISA.Spreadsheet/ArcAssay.js";
import { FsWorkbook } from "./fable_modules/FsSpreadsheet.3.3.0/FsWorkbook.fs.js";
import { ArcAssay } from "./ISA/ISA/ArcTypes/ArcAssay.js";
import { tryFromContract } from "./Contracts/Contracts.ArcAssay.js";
import { DTOType, Contract } from "./Contract/Contract.js";
import { defaultArg, unwrap, value as value_1, Option } from "./fable_modules/fable-library-ts/Option.js";
import { toFsWorkbook as toFsWorkbook_1, fromFsWorkbook as fromFsWorkbook_1 } from "./ISA/ISA.Spreadsheet/ArcStudy.js";
import { ArcStudy } from "./ISA/ISA/ArcTypes/ArcStudy.js";
import { tryFromContract as tryFromContract_1 } from "./Contracts/Contracts.ArcStudy.js";
import { toFsWorkbook, fromFsWorkbook as fromFsWorkbook_2 } from "./ISA/ISA.Spreadsheet/InvestigationFile/Investigation.js";
import { tryFromContract as tryFromContract_2 } from "./Contracts/Contracts.ArcInvestigation.js";
import { ArcInvestigation } from "./ISA/ISA/ArcTypes/ArcInvestigation.js";
import { iterate, map as map_1, fold } from "./fable_modules/fable-library-ts/Seq.js";
import { createRunsFolder, createWorkflowsFolder, createInvestigationFile, createStudyFolder, createStudiesFolder, createAssayFolder, createAssaysFolder } from "./FileSystemTree.js";
import { FileSystemTree_Folder, FileSystemTree, FileSystemTree_$union } from "./FileSystem/FileSystemTree.js";
import { FileSystem } from "./FileSystem/FileSystem.js";
import { tryISAReadContractFromPath } from "./Contracts/Contracts.ARCtrl.js";
import { printf, toConsole } from "./fable_modules/fable-library-ts/String.js";
import { IMap } from "./fable_modules/fable-library-ts/Util.js";
import { addToDict } from "./fable_modules/fable-library-ts/MapUtil.js";
import { Assay_fileNameFromIdentifier, Study_fileNameFromIdentifier } from "./ISA/ISA/ArcTypes/Identifier.js";
import { Dictionary_tryGet } from "./fable_modules/FsSpreadsheet.3.3.0/Cells/FsCellsCollection.fs.js";
import { class_type, TypeInfo } from "./fable_modules/fable-library-ts/Reflection.js";

export function ARCAux_getArcAssaysFromContracts(contracts: Contract[]): ArcAssay[] {
    return map<any, ArcAssay>((x: any): ArcAssay => fromFsWorkbook(x as FsWorkbook), choose<Contract, any>(tryFromContract, contracts));
}

export function ARCAux_getArcStudiesFromContracts(contracts: Contract[]): ArcStudy[] {
    return map<any, ArcStudy>((x: any): ArcStudy => fromFsWorkbook_1(x as FsWorkbook), choose<Contract, any>(tryFromContract_1, contracts));
}

export function ARCAux_getArcInvestigationFromContracts(contracts: Contract[]): ArcInvestigation {
    return fromFsWorkbook_2(exactlyOne<any>(choose<Contract, any>(tryFromContract_2, contracts)) as FsWorkbook);
}

export function ARCAux_updateFSByISA(isa: Option<ArcInvestigation>, fs: FileSystem): FileSystem {
    let patternInput: [string[], string[]];
    if (isa == null) {
        patternInput = ([[], []] as [string[], string[]]);
    }
    else {
        const inv: ArcInvestigation = value_1(isa);
        const arg = [[], []] as [string[], string[]];
        patternInput = fold<ArcStudy, [string[], string[]]>((tupledArg: [string[], string[]], s: ArcStudy): [string[], string[]] => ([append<string>(tupledArg[0], [s.Identifier]), append<string>(tupledArg[1], Array.from(map_1<ArcAssay, string>((a: ArcAssay): string => a.Identifier, s.Assays)))] as [string[], string[]]), [arg[0], arg[1]] as [string[], string[]], inv.Studies);
    }
    const assays: FileSystemTree_$union = createAssaysFolder(map<string, FileSystemTree_$union>(createAssayFolder, patternInput[1]));
    const studies: FileSystemTree_$union = createStudiesFolder(map<string, FileSystemTree_$union>(createStudyFolder, patternInput[0]));
    const investigation: FileSystemTree_$union = createInvestigationFile();
    let tree: FileSystem;
    const arg_1: FileSystemTree_$union = FileSystemTree.createRootFolder([investigation, assays, studies]);
    tree = FileSystem.create({
        tree: arg_1,
    });
    return fs.Union(tree);
}

export function ARCAux_updateFSByCWL(cwl: Option<void>, fs: FileSystem): FileSystem {
    const workflows: FileSystemTree_$union = createWorkflowsFolder([]);
    const runs: FileSystemTree_$union = createRunsFolder([]);
    let tree: FileSystem;
    const arg: FileSystemTree_$union = FileSystemTree.createRootFolder([workflows, runs]);
    tree = FileSystem.create({
        tree: arg,
    });
    return fs.Union(tree);
}

export class ARC {
    "isa@59": Option<ArcInvestigation>;
    "cwl@60": Option<void>;
    "fs@61": FileSystem;
    constructor(isa?: ArcInvestigation, cwl?: Option<void>, fs?: FileSystem) {
        let fs_2: FileSystem, fs_1: FileSystem;
        this["isa@59"] = isa;
        this["cwl@60"] = cwl;
        this["fs@61"] = ((fs_2 = ((fs_1 = defaultArg(fs, FileSystem.create({
            tree: FileSystemTree_Folder("", []),
        })), ARCAux_updateFSByISA(this["isa@59"], fs_1))), ARCAux_updateFSByCWL(this["cwl@60"], fs_2)));
    }
    get ISA(): ArcInvestigation | undefined {
        const this$: ARC = this;
        return unwrap(this$["isa@59"]);
    }
    get CWL(): Option<void> {
        const this$: ARC = this;
        return this$["cwl@60"];
    }
    get FileSystem(): FileSystem {
        const this$: ARC = this;
        return this$["fs@61"];
    }
    static fromFilePaths(filePaths: string[]): ARC {
        return new ARC(void 0, void 0, FileSystem.fromFilePaths(filePaths));
    }
    GetReadContracts(): Contract[] {
        const this$: ARC = this;
        return choose<string, Contract>(tryISAReadContractFromPath, this$["fs@61"].Tree.ToFilePaths());
    }
    SetISAFromContracts(contracts: Contract[], enableLogging?: boolean): void {
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
                const study: ArcStudy = value_1(studyOpt);
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
                        const assay: ArcAssay = value_1(assayOpt);
                        if (enableLogging_1) {
                            toConsole(printf("Found assay: %s - %s"))(studyRegisteredIdent)(assayRegisteredIdent);
                        }
                        study.AddAssay(assay);
                    }
                }, copy.GetStudy(studyRegisteredIdent).AssayIdentifiers);
                investigation.SetStudy(studyRegisteredIdent, study);
            }
        }, copy.StudyIdentifiers);
        this$["isa@59"] = investigation;
    }
    UpdateFileSystem(): void {
        const this$: ARC = this;
        let newFS: FileSystem;
        const fs: FileSystem = ARCAux_updateFSByISA(this$["isa@59"], this$["fs@61"]);
        newFS = ARCAux_updateFSByCWL(this$["cwl@60"], fs);
        this$["fs@61"] = newFS;
    }
    GetWriteContracts(): Contract[] {
        const this$: ARC = this;
        const workbooks: IMap<string, [DTOType, FsWorkbook]> = new Map<string, [DTOType, FsWorkbook]>([]);
        const matchValue: Option<ArcInvestigation> = this$.ISA;
        if (matchValue == null) {
            addToDict(workbooks, "isa.investigation.xlsx", ["ISA_Investigation", toFsWorkbook(ArcInvestigation.create("MISSING_IDENTIFIER_"))] as [string, FsWorkbook]);
            toConsole(printf("ARC contains no ISA part."));
        }
        else {
            const inv: ArcInvestigation = value_1(matchValue);
            addToDict(workbooks, "isa.investigation.xlsx", ["ISA_Investigation", toFsWorkbook(inv)] as [string, FsWorkbook]);
            iterate<ArcStudy>((s: ArcStudy): void => {
                addToDict(workbooks, Study_fileNameFromIdentifier(s.Identifier), ["ISA_Study", toFsWorkbook_1(s)] as [string, FsWorkbook]);
                iterate<ArcAssay>((a: ArcAssay): void => {
                    addToDict(workbooks, Assay_fileNameFromIdentifier(a.Identifier), ["ISA_Assay", toFsWorkbook_2(a)] as [string, FsWorkbook]);
                }, s.Assays);
            }, inv.Studies);
        }
        return map<string, Contract>((fp: string): Contract => {
            const matchValue_1: Option<[DTOType, FsWorkbook]> = Dictionary_tryGet<string, [DTOType, FsWorkbook]>(fp, workbooks);
            if (matchValue_1 == null) {
                return Contract.createCreate(fp, "PlainText");
            }
            else {
                const wb: FsWorkbook = value_1(matchValue_1)[1];
                const dto: DTOType = value_1(matchValue_1)[0];
                return Contract.createCreate(fp, dto, wb);
            }
        }, this$["fs@61"].Tree.ToFilePaths(true));
    }
}

export function ARC_$reflection(): TypeInfo {
    return class_type("ARCtrl.ARC", void 0, ARC);
}

export function ARC_$ctor_81B80F4(isa?: ArcInvestigation, cwl?: Option<void>, fs?: FileSystem): ARC {
    return new ARC(isa, cwl, fs);
}

