import { append as append_1, contains as contains_1, concat, fold, map, exactlyOne, tryPick, choose } from "./fable_modules/fable-library-js.4.22.0/Array.js";
import { ARCtrl_ArcAssay__ArcAssay_ToUpdateContract, ARCtrl_ArcAssay__ArcAssay_ToCreateContract_6FCE9E49, ARCtrl_ArcAssay__ArcAssay_ToDeleteContract, ARCtrl_ArcAssay__ArcAssay_tryFromReadContract_Static_7570923F } from "./Contract/ArcAssay.js";
import { ARCtrl_DataMap__DataMap_ToUpdateContractForAssay_Z721C83C5, ARCtrl_DataMap__DataMap_ToCreateContractForAssay_Z721C83C5, ARCtrl_DataMap__DataMap_ToUpdateContractForStudy_Z721C83C5, ARCtrl_DataMap__DataMap_ToCreateContractForStudy_Z721C83C5, ARCtrl_DataMap__DataMap_tryFromReadContractForStudy_Static, ARCtrl_DataMap__DataMap_tryFromReadContractForAssay_Static } from "./Contract/Datamap.js";
import { ARCtrl_ArcStudy__ArcStudy_ToCreateContract_6FCE9E49, ARCtrl_ArcStudy__ArcStudy_ToUpdateContract, ARCtrl_ArcStudy__ArcStudy_tryFromReadContract_Static_7570923F } from "./Contract/ArcStudy.js";
import { ARCtrl_ArcInvestigation__ArcInvestigation_ToUpdateContract, ARCtrl_ArcInvestigation__ArcInvestigation_tryFromReadContract_Static_7570923F } from "./Contract/ArcInvestigation.js";
import { toList, collect, empty, iterate, find, tryFind, map as map_1, singleton, append, delay, contains, toArray } from "./fable_modules/fable-library-js.4.22.0/Seq.js";
import { FileSystemTree } from "./FileSystem/FileSystemTree.js";
import { FileSystem } from "./FileSystem/FileSystem.js";
import { bind, map as map_2, defaultArg, value as value_2, unwrap } from "./fable_modules/fable-library-js.4.22.0/Option.js";
import { fullFillContractBatchAsync } from "./ContractIO/ContractIO.js";
import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "./fable_modules/Fable.Promise.3.2.0/Promise.fs.js";
import { promise } from "./fable_modules/Fable.Promise.3.2.0/PromiseImpl.fs.js";
import { getAllFilePathsAsync } from "./ContractIO/FileSystemHelper.js";
import { FSharpResult$2 } from "./fable_modules/fable-library-js.4.22.0/Result.js";
import { comparePrimitives, safeHash, stringHash } from "./fable_modules/fable-library-js.4.22.0/Util.js";
import { getStudyFolderPath, getAssayFolderPath } from "./FileSystem/Path.js";
import { printf, toText, replace } from "./fable_modules/fable-library-js.4.22.0/String.js";
import { Contract } from "./Contract/Contract.js";
import { tryISAReadContractFromPath } from "./Contract/ARC.js";
import { ArcTables } from "./Core/Table/ArcTables.js";
import { addToDict } from "./fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF } from "./Spreadsheet/ArcInvestigation.js";
import { ArcInvestigation } from "./Core/ArcTypes.js";
import { Assay_datamapFileNameFromIdentifier, Assay_fileNameFromIdentifier, Study_datamapFileNameFromIdentifier, Study_fileNameFromIdentifier } from "./Core/Helper/Identifier.js";
import { ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522 } from "./Spreadsheet/ArcStudy.js";
import { DataMap__get_StaticHash, DataMap__set_StaticHash_Z524259A4 } from "./Core/DataMap.js";
import { toFsWorkbook } from "./Spreadsheet/DataMap.js";
import { ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F } from "./Spreadsheet/ArcAssay.js";
import { Dictionary_tryGet } from "./fable_modules/FsSpreadsheet.6.3.0-alpha.4/Cells/FsCellsCollection.fs.js";
import { Clone_createCloneContract_5000466F, Init_createAddRemoteContract_Z721C83C5, gitignoreContract, gitattributesContract, Init_createInitContract_6DFDD678 } from "./Contract/Git.js";
import { FSharpSet__Contains, ofSeq, unionMany } from "./fable_modules/fable-library-js.4.22.0/Set.js";
import { ofSeq as ofSeq_1 } from "./fable_modules/fable-library-js.4.22.0/Map.js";
import { fromString } from "./fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { encoder, decoder as decoder_1 } from "./JsonIO/ARC.js";
import { toString } from "./fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "./Json/Encode.js";
import { ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_tryFromReadContract_Static_7570923F, ValidationPackagesConfigHelper_ReadContract, ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toDeleteContract_Static_724DAE55, ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toCreateContract_Static_724DAE55, ValidationPackagesConfigHelper_ConfigFilePath } from "./Contract/ValidationPackagesConfig.js";
import { class_type } from "./fable_modules/fable-library-js.4.22.0/Reflection.js";

export function ARCAux_getArcAssaysFromContracts(contracts) {
    return choose(ARCtrl_ArcAssay__ArcAssay_tryFromReadContract_Static_7570923F, contracts);
}

export function ARCAux_getAssayDataMapFromContracts(assayIdentifier, contracts) {
    return tryPick((c) => ARCtrl_DataMap__DataMap_tryFromReadContractForAssay_Static(assayIdentifier, c), contracts);
}

export function ARCAux_getArcStudiesFromContracts(contracts) {
    return choose(ARCtrl_ArcStudy__ArcStudy_tryFromReadContract_Static_7570923F, contracts);
}

export function ARCAux_getStudyDataMapFromContracts(studyIdentifier, contracts) {
    return tryPick((c) => ARCtrl_DataMap__DataMap_tryFromReadContractForStudy_Static(studyIdentifier, c), contracts);
}

export function ARCAux_getArcInvestigationFromContracts(contracts) {
    return exactlyOne(choose(ARCtrl_ArcInvestigation__ArcInvestigation_tryFromReadContract_Static_7570923F, contracts));
}

export function ARCAux_updateFSByISA(isa, fs) {
    let patternInput;
    if (isa == null) {
        patternInput = [[], []];
    }
    else {
        const inv = isa;
        patternInput = [toArray(inv.Studies), toArray(inv.Assays)];
    }
    let assaysFolder;
    const assays_1 = map((a) => FileSystemTree.createAssayFolder(a.Identifier, a.DataMap != null), patternInput[1]);
    assaysFolder = FileSystemTree.createAssaysFolder(assays_1);
    let studiesFolder;
    const studies_1 = map((s) => FileSystemTree.createStudyFolder(s.Identifier, s.DataMap != null), patternInput[0]);
    studiesFolder = FileSystemTree.createStudiesFolder(studies_1);
    const investigation = FileSystemTree.createInvestigationFile();
    let tree_1;
    const tree = FileSystemTree.createRootFolder([investigation, assaysFolder, studiesFolder]);
    tree_1 = FileSystem.create({
        tree: tree,
    });
    return fs.Union(tree_1);
}

export function ARCAux_updateFSByCWL(cwl, fs) {
    const workflows = FileSystemTree.createWorkflowsFolder([]);
    const runs = FileSystemTree.createRunsFolder([]);
    let tree_1;
    const tree = FileSystemTree.createRootFolder([workflows, runs]);
    tree_1 = FileSystem.create({
        tree: tree,
    });
    return fs.Union(tree_1);
}

export class ARC {
    constructor(isa, cwl, fs) {
        this.cwl = cwl;
        this._isa = isa;
        this._cwl = this.cwl;
        this._fs = ARCAux_updateFSByCWL(this.cwl, ARCAux_updateFSByISA(isa, defaultArg(fs, FileSystem.create({
            tree: new FileSystemTree(1, ["", []]),
        }))));
    }
    get ISA() {
        const this$ = this;
        return unwrap(this$._isa);
    }
    set ISA(newISA) {
        const this$ = this;
        this$._isa = newISA;
        this$.UpdateFileSystem();
    }
    get CWL() {
        const this$ = this;
        return this$.cwl;
    }
    get FileSystem() {
        const this$ = this;
        return this$._fs;
    }
    set FileSystem(fs) {
        const this$ = this;
        this$._fs = fs;
    }
    WriteAsync(arcPath) {
        const this$ = this;
        return fullFillContractBatchAsync(arcPath, this$.GetWriteContracts());
    }
    UpdateAsync(arcPath) {
        const this$ = this;
        return fullFillContractBatchAsync(arcPath, this$.GetUpdateContracts());
    }
    static loadAsync(arcPath) {
        return PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (getAllFilePathsAsync(arcPath).then((_arg) => {
            const arc = ARC.fromFilePaths(toArray(_arg));
            const contracts = arc.GetReadContracts();
            return fullFillContractBatchAsync(arcPath, contracts).then((_arg_1) => {
                const fulFilledContracts = _arg_1;
                if (fulFilledContracts.tag === 1) {
                    return Promise.resolve(new FSharpResult$2(1, [fulFilledContracts.fields[0]]));
                }
                else {
                    arc.SetISAFromContracts(fulFilledContracts.fields[0]);
                    return Promise.resolve(new FSharpResult$2(0, [arc]));
                }
            });
        }))));
    }
    GetAssayRemoveContracts(assayIdentifier) {
        let i;
        const this$ = this;
        let isa;
        const matchValue = this$.ISA;
        if (matchValue == null) {
            throw new Error("Cannot remove assay from null ISA value.");
        }
        else if ((i = matchValue, contains(assayIdentifier, i.AssayIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        }))) {
            const i_1 = matchValue;
            isa = i_1;
        }
        else {
            throw new Error("ARC does not contain assay with given name");
        }
        const assay = isa.GetAssay(assayIdentifier);
        const studies = assay.StudiesRegisteredIn;
        isa.RemoveAssay(assayIdentifier);
        const paths = this$.FileSystem.Tree.ToFilePaths();
        const assayFolderPath = getAssayFolderPath(assayIdentifier);
        const filteredPaths = paths.filter((p) => !p.startsWith(assayFolderPath));
        this$.SetFilePaths(filteredPaths);
        return toArray(delay(() => append(singleton(ARCtrl_ArcAssay__ArcAssay_ToDeleteContract(assay)), delay(() => append(singleton(ARCtrl_ArcInvestigation__ArcInvestigation_ToUpdateContract(isa)), delay(() => map_1(ARCtrl_ArcStudy__ArcStudy_ToUpdateContract, studies)))))));
    }
    RemoveAssayAsync(arcPath, assayIdentifier) {
        const this$ = this;
        return fullFillContractBatchAsync(arcPath, this$.GetAssayRemoveContracts(assayIdentifier));
    }
    GetAssayRenameContracts(oldAssayIdentifier, newAssayIdentifier) {
        let i;
        const this$ = this;
        let isa;
        const matchValue = this$.ISA;
        if (matchValue == null) {
            throw new Error("Cannot rename assay in null ISA value.");
        }
        else if ((i = matchValue, contains(oldAssayIdentifier, i.AssayIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        }))) {
            const i_1 = matchValue;
            isa = i_1;
        }
        else {
            throw new Error("ARC does not contain assay with given name");
        }
        isa.RenameAssay(oldAssayIdentifier, newAssayIdentifier);
        const paths = this$.FileSystem.Tree.ToFilePaths();
        const oldAssayFolderPath = getAssayFolderPath(oldAssayIdentifier);
        const newAssayFolderPath = getAssayFolderPath(newAssayIdentifier);
        const renamedPaths = map((p) => replace(p, oldAssayFolderPath, newAssayFolderPath), paths);
        this$.SetFilePaths(renamedPaths);
        return toArray(delay(() => append(singleton(Contract.createRename(oldAssayFolderPath, newAssayFolderPath)), delay(() => this$.GetUpdateContracts()))));
    }
    RenameAssayAsync(arcPath, oldAssayIdentifier, newAssayIdentifier) {
        const this$ = this;
        return fullFillContractBatchAsync(arcPath, this$.GetAssayRenameContracts(oldAssayIdentifier, newAssayIdentifier));
    }
    GetStudyRemoveContracts(studyIdentifier) {
        const this$ = this;
        let isa;
        const matchValue = this$.ISA;
        if (matchValue == null) {
            throw new Error("Cannot remove study from null ISA value.");
        }
        else {
            isa = matchValue;
        }
        isa.RemoveStudy(studyIdentifier);
        const paths = this$.FileSystem.Tree.ToFilePaths();
        const studyFolderPath = getStudyFolderPath(studyIdentifier);
        const filteredPaths = paths.filter((p) => !p.startsWith(studyFolderPath));
        this$.SetFilePaths(filteredPaths);
        return [Contract.createDelete(studyFolderPath), ARCtrl_ArcInvestigation__ArcInvestigation_ToUpdateContract(isa)];
    }
    RemoveStudyAsync(arcPath, studyIdentifier) {
        const this$ = this;
        return fullFillContractBatchAsync(arcPath, this$.GetStudyRemoveContracts(studyIdentifier));
    }
    GetStudyRenameContracts(oldStudyIdentifier, newStudyIdentifier) {
        let i;
        const this$ = this;
        let isa;
        const matchValue = this$.ISA;
        if (matchValue == null) {
            throw new Error("Cannot rename study in null ISA value.");
        }
        else if ((i = matchValue, contains(oldStudyIdentifier, i.StudyIdentifiers, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        }))) {
            const i_1 = matchValue;
            isa = i_1;
        }
        else {
            throw new Error("ARC does not contain study with given name");
        }
        isa.RenameStudy(oldStudyIdentifier, newStudyIdentifier);
        const paths = this$.FileSystem.Tree.ToFilePaths();
        const oldStudyFolderPath = getStudyFolderPath(oldStudyIdentifier);
        const newStudyFolderPath = getStudyFolderPath(newStudyIdentifier);
        const renamedPaths = map((p) => replace(p, oldStudyFolderPath, newStudyFolderPath), paths);
        this$.SetFilePaths(renamedPaths);
        return toArray(delay(() => append(singleton(Contract.createRename(oldStudyFolderPath, newStudyFolderPath)), delay(() => this$.GetUpdateContracts()))));
    }
    RenameStudyAsync(arcPath, oldStudyIdentifier, newStudyIdentifier) {
        const this$ = this;
        return fullFillContractBatchAsync(arcPath, this$.GetStudyRenameContracts(oldStudyIdentifier, newStudyIdentifier));
    }
    static fromFilePaths(filePaths) {
        return new ARC(undefined, undefined, FileSystem.fromFilePaths(filePaths));
    }
    SetFilePaths(filePaths) {
        const this$ = this;
        const tree = FileSystemTree.fromFilePaths(filePaths);
        this$._fs = (new FileSystem(tree, this$._fs.History));
    }
    GetReadContracts() {
        const this$ = this;
        return choose(tryISAReadContractFromPath, this$._fs.Tree.ToFilePaths());
    }
    SetISAFromContracts(contracts) {
        const this$ = this;
        const investigation = ARCAux_getArcInvestigationFromContracts(contracts);
        const studies = map((tuple) => tuple[0], ARCAux_getArcStudiesFromContracts(contracts));
        const assays = ARCAux_getArcAssaysFromContracts(contracts);
        const array_2 = investigation.AssayIdentifiers;
        array_2.forEach((ai) => {
            if (!assays.some((a) => (a.Identifier === ai))) {
                investigation.DeleteAssay(ai);
            }
        });
        const array_4 = investigation.StudyIdentifiers;
        array_4.forEach((si) => {
            if (!studies.some((s) => (s.Identifier === si))) {
                investigation.DeleteStudy(si);
            }
        });
        studies.forEach((study) => {
            const registeredStudyOpt = tryFind((s_1) => (s_1.Identifier === study.Identifier), investigation.Studies);
            if (registeredStudyOpt == null) {
                investigation.AddStudy(study);
            }
            else {
                const registeredStudy = registeredStudyOpt;
                registeredStudy.UpdateReferenceByStudyFile(study, true);
            }
            const datamap = ARCAux_getAssayDataMapFromContracts(study.Identifier, contracts);
            if (study.DataMap == null) {
                study.DataMap = datamap;
            }
            study.StaticHash = (study.GetLightHashCode() | 0);
        });
        assays.forEach((assay) => {
            const registeredAssayOpt = tryFind((a_1) => (a_1.Identifier === assay.Identifier), investigation.Assays);
            if (registeredAssayOpt == null) {
                investigation.AddAssay(assay);
            }
            else {
                const registeredAssay = registeredAssayOpt;
                registeredAssay.UpdateReferenceByAssayFile(assay, true);
            }
            const assay_1 = find((a_2) => (a_2.Identifier === assay.Identifier), investigation.Assays);
            let updatedTables;
            const array_6 = assay_1.StudiesRegisteredIn;
            updatedTables = fold((tables, study_1) => ArcTables.updateReferenceTablesBySheets(new ArcTables(study_1.Tables), tables, false), new ArcTables(assay_1.Tables), array_6);
            const datamap_1 = ARCAux_getAssayDataMapFromContracts(assay_1.Identifier, contracts);
            if (assay_1.DataMap == null) {
                assay_1.DataMap = datamap_1;
            }
            assay_1.Tables = updatedTables.Tables;
        });
        iterate((a_3) => {
            a_3.StaticHash = (a_3.GetLightHashCode() | 0);
        }, investigation.Assays);
        iterate((s_2) => {
            s_2.StaticHash = (s_2.GetLightHashCode() | 0);
        }, investigation.Studies);
        investigation.StaticHash = (investigation.GetLightHashCode() | 0);
        this$.ISA = investigation;
    }
    UpdateFileSystem() {
        const this$ = this;
        let newFS;
        const fs = ARCAux_updateFSByISA(this$._isa, this$._fs);
        newFS = ARCAux_updateFSByCWL(this$._cwl, fs);
        this$._fs = newFS;
    }
    GetWriteContracts() {
        const this$ = this;
        const workbooks = new Map([]);
        const matchValue = this$.ISA;
        if (matchValue == null) {
            addToDict(workbooks, "isa.investigation.xlsx", ["ISA_Investigation", ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(ArcInvestigation.create("MISSING_IDENTIFIER_"))]);
        }
        else {
            const inv = matchValue;
            addToDict(workbooks, "isa.investigation.xlsx", ["ISA_Investigation", ARCtrl_ArcInvestigation__ArcInvestigation_toFsWorkbook_Static_Z720BD3FF(inv)]);
            inv.StaticHash = (inv.GetLightHashCode() | 0);
            iterate((s) => {
                s.StaticHash = (s.GetLightHashCode() | 0);
                addToDict(workbooks, Study_fileNameFromIdentifier(s.Identifier), ["ISA_Study", ARCtrl_ArcStudy__ArcStudy_toFsWorkbook_Static_Z4CEFA522(s)]);
                if (s.DataMap != null) {
                    const dm = value_2(s.DataMap);
                    DataMap__set_StaticHash_Z524259A4(dm, safeHash(dm));
                    addToDict(workbooks, Study_datamapFileNameFromIdentifier(s.Identifier), ["ISA_Datamap", toFsWorkbook(dm)]);
                }
            }, inv.Studies);
            iterate((a) => {
                a.StaticHash = (a.GetLightHashCode() | 0);
                addToDict(workbooks, Assay_fileNameFromIdentifier(a.Identifier), ["ISA_Assay", ARCtrl_ArcAssay__ArcAssay_toFsWorkbook_Static_Z2508BE4F(a)]);
                if (a.DataMap != null) {
                    const dm_1 = value_2(a.DataMap);
                    DataMap__set_StaticHash_Z524259A4(dm_1, safeHash(dm_1));
                    addToDict(workbooks, Assay_datamapFileNameFromIdentifier(a.Identifier), ["ISA_Datamap", toFsWorkbook(dm_1)]);
                }
            }, inv.Assays);
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
        }, this$._fs.Tree.ToFilePaths(true));
    }
    GetUpdateContracts() {
        let inv, inv_1, inv_2;
        const this$ = this;
        const matchValue = this$.ISA;
        return (matchValue != null) ? (((inv = matchValue, inv.StaticHash === 0)) ? ((inv_1 = matchValue, this$.GetWriteContracts())) : ((inv_2 = matchValue, toArray(delay(() => {
            const hash = inv_2.GetLightHashCode() | 0;
            return append((inv_2.StaticHash !== hash) ? singleton(ARCtrl_ArcInvestigation__ArcInvestigation_ToUpdateContract(inv_2)) : empty(), delay(() => {
                inv_2.StaticHash = (hash | 0);
                return append(collect((s) => {
                    const hash_1 = s.GetLightHashCode() | 0;
                    return append((s.StaticHash === 0) ? ARCtrl_ArcStudy__ArcStudy_ToCreateContract_6FCE9E49(s, true) : ((s.StaticHash !== hash_1) ? singleton(ARCtrl_ArcStudy__ArcStudy_ToUpdateContract(s)) : empty()), delay(() => {
                        let dm_1;
                        s.StaticHash = (hash_1 | 0);
                        const matchValue_1 = s.DataMap;
                        let matchResult, dm_2, dm_3;
                        if (matchValue_1 != null) {
                            if (DataMap__get_StaticHash(matchValue_1) === 0) {
                                matchResult = 0;
                                dm_2 = matchValue_1;
                            }
                            else if ((dm_1 = matchValue_1, DataMap__get_StaticHash(dm_1) !== safeHash(dm_1))) {
                                matchResult = 1;
                                dm_3 = matchValue_1;
                            }
                            else {
                                matchResult = 2;
                            }
                        }
                        else {
                            matchResult = 2;
                        }
                        switch (matchResult) {
                            case 0:
                                return append(singleton(ARCtrl_DataMap__DataMap_ToCreateContractForStudy_Z721C83C5(dm_2, s.Identifier)), delay(() => {
                                    DataMap__set_StaticHash_Z524259A4(dm_2, safeHash(dm_2));
                                    return empty();
                                }));
                            case 1:
                                return append(singleton(ARCtrl_DataMap__DataMap_ToUpdateContractForStudy_Z721C83C5(dm_3, s.Identifier)), delay(() => {
                                    DataMap__set_StaticHash_Z524259A4(dm_3, safeHash(dm_3));
                                    return empty();
                                }));
                            default: {
                                return empty();
                            }
                        }
                    }));
                }, inv_2.Studies), delay(() => collect((a) => {
                    const hash_2 = a.GetLightHashCode() | 0;
                    return append((a.StaticHash === 0) ? ARCtrl_ArcAssay__ArcAssay_ToCreateContract_6FCE9E49(a, true) : ((a.StaticHash !== hash_2) ? singleton(ARCtrl_ArcAssay__ArcAssay_ToUpdateContract(a)) : empty()), delay(() => {
                        let dm_5;
                        a.StaticHash = (hash_2 | 0);
                        const matchValue_2 = a.DataMap;
                        let matchResult_1, dm_6, dm_7;
                        if (matchValue_2 != null) {
                            if (DataMap__get_StaticHash(matchValue_2) === 0) {
                                matchResult_1 = 0;
                                dm_6 = matchValue_2;
                            }
                            else if ((dm_5 = matchValue_2, DataMap__get_StaticHash(dm_5) !== safeHash(dm_5))) {
                                matchResult_1 = 1;
                                dm_7 = matchValue_2;
                            }
                            else {
                                matchResult_1 = 2;
                            }
                        }
                        else {
                            matchResult_1 = 2;
                        }
                        switch (matchResult_1) {
                            case 0:
                                return append(singleton(ARCtrl_DataMap__DataMap_ToCreateContractForAssay_Z721C83C5(dm_6, a.Identifier)), delay(() => {
                                    DataMap__set_StaticHash_Z524259A4(dm_6, safeHash(dm_6));
                                    return empty();
                                }));
                            case 1:
                                return append(singleton(ARCtrl_DataMap__DataMap_ToUpdateContractForAssay_Z721C83C5(dm_7, a.Identifier)), delay(() => {
                                    DataMap__set_StaticHash_Z524259A4(dm_7, safeHash(dm_7));
                                    return empty();
                                }));
                            default: {
                                return empty();
                            }
                        }
                    }));
                }, inv_2.Assays)));
            }));
        }))))) : this$.GetWriteContracts();
    }
    GetGitInitContracts(branch, repositoryAddress, defaultGitignore) {
        const defaultGitignore_1 = defaultArg(defaultGitignore, false);
        return toArray(delay(() => append(singleton(Init_createInitContract_6DFDD678(unwrap(branch))), delay(() => append(singleton(gitattributesContract), delay(() => append(defaultGitignore_1 ? singleton(gitignoreContract) : empty(), delay(() => ((repositoryAddress != null) ? singleton(Init_createAddRemoteContract_Z721C83C5(value_2(repositoryAddress))) : empty())))))))));
    }
    static getCloneContract(remoteUrl, merge, branch, token, nolfs) {
        return Clone_createCloneContract_5000466F(remoteUrl, unwrap(merge), unwrap(branch), unwrap(token), unwrap(nolfs));
    }
    Copy() {
        const this$ = this;
        const isaCopy = map_2((i) => i.Copy(), this$._isa);
        const fsCopy = this$._fs.Copy();
        return new ARC(unwrap(isaCopy), this$._cwl, fsCopy);
    }
    GetRegisteredPayload(IgnoreHidden) {
        let tree, paths, array_3;
        const this$ = this;
        const registeredStudies = defaultArg(map_2((isa) => isa.Studies.slice(), map_2((i) => i.Copy(), this$._isa)), []);
        const registeredAssays = concat(map((s) => s.RegisteredAssays.slice(), registeredStudies));
        const includeFiles = unionMany([ofSeq(["isa.investigation.xlsx", "README.md"], {
            Compare: comparePrimitives,
        }), unionMany(map((s_1) => {
            const studyFoldername = `${"studies"}/${s_1.Identifier}`;
            return ofSeq(toList(delay(() => append(singleton(`${studyFoldername}/${"isa.study.xlsx"}`), delay(() => append(singleton(`${studyFoldername}/${"README.md"}`), delay(() => collect((table) => collect((kv) => {
                const textValue = kv[1].ToFreeTextCell().AsFreeText;
                return append(singleton(textValue), delay(() => append(singleton(`${studyFoldername}/${"resources"}/${textValue}`), delay(() => singleton(`${studyFoldername}/${"protocols"}/${textValue}`)))));
            }, table.Values), s_1.Tables))))))), {
                Compare: comparePrimitives,
            });
        }, registeredStudies), {
            Compare: comparePrimitives,
        }), unionMany(map((a) => {
            const assayFoldername = `${"assays"}/${a.Identifier}`;
            return ofSeq(toList(delay(() => append(singleton(`${assayFoldername}/${"isa.assay.xlsx"}`), delay(() => append(singleton(`${assayFoldername}/${"README.md"}`), delay(() => collect((table_1) => collect((kv_1) => {
                const textValue_1 = kv_1[1].ToFreeTextCell().AsFreeText;
                return append(singleton(textValue_1), delay(() => append(singleton(`${assayFoldername}/${"dataset"}/${textValue_1}`), delay(() => singleton(`${assayFoldername}/${"protocols"}/${textValue_1}`)))));
            }, table_1.Values), a.Tables))))))), {
                Compare: comparePrimitives,
            });
        }, registeredAssays), {
            Compare: comparePrimitives,
        })], {
            Compare: comparePrimitives,
        });
        const ignoreHidden = defaultArg(IgnoreHidden, true);
        const fsCopy = this$._fs.Copy();
        return defaultArg(bind((tree_1) => {
            if (ignoreHidden) {
                return FileSystemTree.filterFolders((n_1) => !n_1.startsWith("."))(tree_1);
            }
            else {
                return tree_1;
            }
        }, (tree = ((paths = ((array_3 = FileSystemTree.toFilePaths()(fsCopy.Tree), array_3.filter((p) => {
            if (p.startsWith("workflows") ? true : p.startsWith("runs")) {
                return true;
            }
            else {
                return FSharpSet__Contains(includeFiles, p);
            }
        }))), FileSystemTree.fromFilePaths(paths))), ignoreHidden ? FileSystemTree.filterFiles((n) => !n.startsWith("."))(tree) : tree)), FileSystemTree.fromFilePaths([]));
    }
    GetAdditionalPayload(IgnoreHidden) {
        let tree, paths, array;
        const this$ = this;
        const ignoreHidden = defaultArg(IgnoreHidden, true);
        const registeredPayload = ofSeq(FileSystemTree.toFilePaths()(this$.GetRegisteredPayload()), {
            Compare: comparePrimitives,
        });
        return defaultArg(bind((tree_1) => {
            if (ignoreHidden) {
                return FileSystemTree.filterFolders((n_1) => !n_1.startsWith("."))(tree_1);
            }
            else {
                return tree_1;
            }
        }, (tree = ((paths = ((array = FileSystemTree.toFilePaths()(this$._fs.Copy().Tree), array.filter((p) => !FSharpSet__Contains(registeredPayload, p)))), FileSystemTree.fromFilePaths(paths))), ignoreHidden ? FileSystemTree.filterFiles((n) => !n.startsWith("."))(tree) : tree)), FileSystemTree.fromFilePaths([]));
    }
    static get DefaultContracts() {
        return ofSeq_1([[".gitignore", gitignoreContract], [".gitattributes", gitattributesContract]], {
            Compare: comparePrimitives,
        });
    }
    static fromROCrateJsonString(s) {
        let matchValue;
        return new ARC(unwrap((matchValue = fromString(decoder_1, s), (matchValue.tag === 1) ? (() => {
            throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
        })() : matchValue.fields[0])));
    }
    ToROCrateJsonString(spaces) {
        const this$ = this;
        const value = encoder(value_2(this$._isa));
        return toString(defaultSpaces(spaces), value);
    }
    static toROCrateJsonString(spaces) {
        return (obj) => obj.ToROCrateJsonString(unwrap(spaces));
    }
    GetValidationPackagesConfigWriteContract(vpc) {
        const this$ = this;
        const paths = this$.FileSystem.Tree.ToFilePaths();
        if (!contains_1(ValidationPackagesConfigHelper_ConfigFilePath, paths, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        })) {
            const filePaths = append_1([ValidationPackagesConfigHelper_ConfigFilePath], paths);
            this$.SetFilePaths(filePaths);
        }
        return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toCreateContract_Static_724DAE55(vpc);
    }
    GetValidationPackagesConfigDeleteContract(vpc) {
        const this$ = this;
        const paths = this$.FileSystem.Tree.ToFilePaths();
        if (contains_1(ValidationPackagesConfigHelper_ConfigFilePath, paths, {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        })) {
            const filePaths = paths.filter((p) => !(p === ValidationPackagesConfigHelper_ConfigFilePath));
            this$.SetFilePaths(filePaths);
        }
        return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_toDeleteContract_Static_724DAE55(vpc);
    }
    GetValidationPackagesConfigReadContract() {
        return ValidationPackagesConfigHelper_ReadContract;
    }
    GetValidationPackagesConfigFromReadContract(contract) {
        return ARCtrl_ValidationPackages_ValidationPackagesConfig__ValidationPackagesConfig_tryFromReadContract_Static_7570923F(contract);
    }
}

export function ARC_$reflection() {
    return class_type("ARCtrl.ARC", undefined, ARC);
}

export function ARC_$ctor_79978BA1(isa, cwl, fs) {
    return new ARC(isa, cwl, fs);
}

