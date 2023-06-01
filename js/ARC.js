import { FileSystem_$reflection, FileSystem } from "./FileSystem/FileSystem.js";
import { fold } from "./fable_modules/fable-library.4.1.4/Array.js";
import { combine } from "./FileSystem/Path.js";
import { combineAssaySubfolderPaths, combineAssayFolderPath } from "./ARCPath.js";
import { Investigation_$reflection, Investigation } from "./ISA/Investigation.js";
import { Record } from "./fable_modules/fable-library.4.1.4/Types.js";
import { record_type, unit_type } from "./fable_modules/fable-library.4.1.4/Reflection.js";

export class ARC extends Record {
    constructor(ISA, CWL, FileSystem) {
        super();
        this.ISA = ISA;
        this.CWL = CWL;
        this.FileSystem = FileSystem;
    }
    static create(isa, cwl, fs) {
        return new ARC(isa, void 0, fs);
    }
    static updateISA(isa, arc) {
        throw new Error();
    }
    static updateCWL(cwl, arc) {
        throw new Error();
    }
    static updateFileSystem(fileSystem, arc) {
        throw new Error();
    }
    static addFile(path, arc) {
        (arg) => ((arg_1) => FileSystem.addFile(arg, arg_1));
        (arg_2) => ((arg_3) => ARC.updateFileSystem(arg_2, arg_3));
        throw new Error();
    }
    static addFolder(path, arc) {
        (arg) => ((arg_1) => FileSystem.addFolder(arg, arg_1));
        (arg_2) => ((arg_3) => ARC.updateFileSystem(arg_2, arg_3));
        throw new Error();
    }
    static addFolders(paths, arc) {
        fold((arc_1, path) => ARC.addFolder(path, arc_1), arc, paths);
        throw new Error();
    }
    static addEmptyFolder(path, arc) {
        let arg_2;
        (arg) => ((arg_1) => FileSystem.addFolder(arg, arg_1));
        (arg_2 = combine(path, ".gitkeep"), (arg_3) => FileSystem.addFile(arg_2, arg_3));
        (arg_4) => ((arg_5) => ARC.updateFileSystem(arg_4, arg_5));
        throw new Error();
    }
    static addEmptyFolders(paths, arc) {
        fold((arc_1, path) => ARC.addEmptyFolder(path, arc_1), arc, paths);
        throw new Error();
    }
    static addAssay(assay, studyIdentifier, arc) {
        const assayFolderPath = combineAssayFolderPath(assay);
        const assaySubFolderPaths = combineAssaySubfolderPaths(assay);
        const assayReadmeFilePath = combine(assayFolderPath, "Readme.md");
        const arg_9 = Investigation.addAssay(assay, studyIdentifier, arc.ISA);
        let arg_10;
        let arg_8;
        const arg_6 = ARC.addFolder(assayFolderPath, arc);
        arg_8 = ARC.addEmptyFolders(assaySubFolderPaths, arg_6);
        arg_10 = ARC.addFile(assayReadmeFilePath, arg_8);
        return ARC.updateISA(arg_9, arg_10);
    }
}

export function ARC_$reflection() {
    return record_type("ARC.ARC", [], ARC, () => [["ISA", Investigation_$reflection()], ["CWL", unit_type], ["FileSystem", FileSystem_$reflection()]]);
}

