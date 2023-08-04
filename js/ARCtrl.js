import { tryFind, append, exactlyOne, choose, map } from "./fable_modules/fable-library.4.1.4/Array.js";
import { toFsWorkbook as toFsWorkbook_2, fromFsWorkbook } from "./ISA/ISA.Spreadsheet/ArcAssay.js";
import { tryFromContract } from "./Contracts/Contracts.ArcAssay.js";
import { toFsWorkbook as toFsWorkbook_1, fromFsWorkbook as fromFsWorkbook_1 } from "./ISA/ISA.Spreadsheet/ArcStudy.js";
import { tryFromContract as tryFromContract_1 } from "./Contracts/Contracts.ArcStudy.js";
import { toFsWorkbook, fromFsWorkbook as fromFsWorkbook_2 } from "./ISA/ISA.Spreadsheet/InvestigationFile/Investigation.js";
import { tryFromContract as tryFromContract_2 } from "./Contracts/Contracts.ArcInvestigation.js";
import { iterate, map as map_1, fold } from "./fable_modules/fable-library.4.1.4/Seq.js";
import { createRunsFolder, createWorkflowsFolder, createInvestigationFile, createStudyFolder, createStudiesFolder, createAssayFolder, createAssaysFolder } from "./FileSystemTree.js";
import { FileSystemTree } from "./FileSystem/FileSystemTree.js";
import { FileSystem } from "./FileSystem/FileSystem.js";
import { defaultArg, unwrap } from "./fable_modules/fable-library.4.1.4/Option.js";
import { tryISAReadContractFromPath } from "./Contracts/Contracts.ARCtrl.js";
import { printf, toConsole } from "./fable_modules/fable-library.4.1.4/String.js";
import { addToDict } from "./fable_modules/fable-library.4.1.4/MapUtil.js";
import { ArcInvestigation } from "./ISA/ISA/ArcTypes/ArcInvestigation.js";
import { Assay_fileNameFromIdentifier, Study_fileNameFromIdentifier } from "./ISA/ISA/ArcTypes/Identifier.js";
import { Dictionary_tryGet } from "./fable_modules/FsSpreadsheet.3.3.0/Cells/FsCellsCollection.fs.js";
import { Contract } from "./Contract/Contract.js";
import { class_type } from "./fable_modules/fable-library.4.1.4/Reflection.js";

export function ARCAux_getArcAssaysFromContracts(contracts) {
    return map(fromFsWorkbook, choose(tryFromContract, contracts));
}

export function ARCAux_getArcStudiesFromContracts(contracts) {
    return map(fromFsWorkbook_1, choose(tryFromContract_1, contracts));
}

export function ARCAux_getArcInvestigationFromContracts(contracts) {
    return fromFsWorkbook_2(exactlyOne(choose(tryFromContract_2, contracts)));
}

export function ARCAux_updateFSByISA(isa, fs) {
    let patternInput;
    if (isa == null) {
        patternInput = [[], []];
    }
    else {
        const inv = isa;
        const arg = [[], []];
        patternInput = fold((tupledArg, s) => [append(tupledArg[0], [s.Identifier]), append(tupledArg[1], Array.from(map_1((a) => a.Identifier, s.Assays)))], [arg[0], arg[1]], inv.Studies);
    }
    const assays = createAssaysFolder(map(createAssayFolder, patternInput[1]));
    const studies = createStudiesFolder(map(createStudyFolder, patternInput[0]));
    const investigation = createInvestigationFile();
    let tree;
    const arg_1 = FileSystemTree.createRootFolder([investigation, assays, studies]);
    tree = FileSystem.create({
        tree: arg_1,
    });
    return fs.Union(tree);
}

export function ARCAux_updateFSByCWL(cwl, fs) {
    const workflows = createWorkflowsFolder([]);
    const runs = createRunsFolder([]);
    let tree;
    const arg = FileSystemTree.createRootFolder([workflows, runs]);
    tree = FileSystem.create({
        tree: arg,
    });
    return fs.Union(tree);
}

export class ARC {
    constructor(isa, cwl, fs) {
        let fs_2, fs_1;
        this["isa@59"] = isa;
        this["cwl@60"] = cwl;
        this["fs@61"] = ((fs_2 = ((fs_1 = defaultArg(fs, FileSystem.create({
            tree: new FileSystemTree(1, ["", []]),
        })), ARCAux_updateFSByISA(this["isa@59"], fs_1))), ARCAux_updateFSByCWL(this["cwl@60"], fs_2)));
    }
    get ISA() {
        const this$ = this;
        return unwrap(this$["isa@59"]);
    }
    get CWL() {
        const this$ = this;
        return this$["cwl@60"];
    }
    get FileSystem() {
        const this$ = this;
        return this$["fs@61"];
    }
    static fromFilePaths(filePaths) {
        return new ARC(void 0, void 0, FileSystem.fromFilePaths(filePaths));
    }
    GetReadContracts() {
        const this$ = this;
        return choose(tryISAReadContractFromPath, this$["fs@61"].Tree.ToFilePaths());
    }
    SetISAFromContracts(contracts, enableLogging) {
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
        this$["isa@59"] = investigation;
    }
    UpdateFileSystem() {
        const this$ = this;
        let newFS;
        const fs = ARCAux_updateFSByISA(this$["isa@59"], this$["fs@61"]);
        newFS = ARCAux_updateFSByCWL(this$["cwl@60"], fs);
        this$["fs@61"] = newFS;
    }
    GetWriteContracts() {
        const this$ = this;
        const workbooks = new Map([]);
        const matchValue = this$.ISA;
        if (matchValue == null) {
            addToDict(workbooks, "isa.investigation.xlsx", ["ISA_Investigation", toFsWorkbook(ArcInvestigation.create("MISSING_IDENTIFIER_"))]);
            toConsole(printf("ARC contains no ISA part."));
        }
        else {
            const inv = matchValue;
            addToDict(workbooks, "isa.investigation.xlsx", ["ISA_Investigation", toFsWorkbook(inv)]);
            iterate((s) => {
                addToDict(workbooks, Study_fileNameFromIdentifier(s.Identifier), ["ISA_Study", toFsWorkbook_1(s)]);
                iterate((a) => {
                    addToDict(workbooks, Assay_fileNameFromIdentifier(a.Identifier), ["ISA_Assay", toFsWorkbook_2(a)]);
                }, s.Assays);
            }, inv.Studies);
        }
        return map((fp) => {
            const matchValue_1 = Dictionary_tryGet(fp, workbooks);
            if (matchValue_1 == null) {
                return Contract.createCreate(fp, "PlainText");
            }
            else {
                const wb = matchValue_1[1];
                const dto = matchValue_1[0];
                return Contract.createCreate(fp, dto, wb);
            }
        }, this$["fs@61"].Tree.ToFilePaths(true));
    }
}

export function ARC_$reflection() {
    return class_type("ARCtrl.ARC", void 0, ARC);
}

export function ARC_$ctor_81B80F4(isa, cwl, fs) {
    return new ARC(isa, cwl, fs);
}

