import { Investigation_$reflection, Investigation } from "./ISA/Investigation.js";
import { FileSystem_$reflection, FileSystem } from "./FileSystem/FileSystem.js";
import { fold } from "./fable_modules/fable-library-ts/Array.js";
import { combine } from "./FileSystem/Path.js";
import { combineAssaySubfolderPaths, combineAssayFolderPath } from "./ARCPath.js";
import { Assay } from "./ISA/Assay.js";
import { Record } from "./fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "./fable_modules/fable-library-ts/Util.js";
import { record_type, unit_type, TypeInfo } from "./fable_modules/fable-library-ts/Reflection.js";

export class ARC extends Record implements IEquatable<ARC>, IComparable<ARC> {
    readonly ISA: Investigation;
    readonly CWL: void;
    readonly FileSystem: FileSystem;
    constructor(ISA: Investigation, CWL: void, FileSystem: FileSystem) {
        super();
        this.ISA = ISA;
        this.CWL = CWL;
        this.FileSystem = FileSystem;
    }
    static create(isa: Investigation, cwl: void, fs: FileSystem): ARC {
        return new ARC(isa, void 0, fs);
    }
    static updateISA(isa: Investigation, arc: ARC): ARC {
        throw new Error();
    }
    static updateCWL(cwl: void, arc: ARC): ARC {
        throw new Error();
    }
    static updateFileSystem(fileSystem: FileSystem, arc: ARC): ARC {
        throw new Error();
    }
    static addFile(path: string, arc: ARC): ARC {
        (arg: string): ((arg0: FileSystem) => FileSystem) => ((arg_1: FileSystem): FileSystem => FileSystem.addFile(arg, arg_1));
        (arg_2: FileSystem): ((arg0: ARC) => ARC) => ((arg_3: ARC): ARC => ARC.updateFileSystem(arg_2, arg_3));
        throw new Error();
    }
    static addFolder(path: string, arc: ARC): ARC {
        (arg: string): ((arg0: FileSystem) => FileSystem) => ((arg_1: FileSystem): FileSystem => FileSystem.addFolder(arg, arg_1));
        (arg_2: FileSystem): ((arg0: ARC) => ARC) => ((arg_3: ARC): ARC => ARC.updateFileSystem(arg_2, arg_3));
        throw new Error();
    }
    static addFolders(paths: string[], arc: ARC): ARC {
        fold<string, ARC>((arc_1: ARC, path: string): ARC => ARC.addFolder(path, arc_1), arc, paths);
        throw new Error();
    }
    static addEmptyFolder(path: string, arc: ARC): ARC {
        let arg_2: string;
        (arg: string): ((arg0: FileSystem) => FileSystem) => ((arg_1: FileSystem): FileSystem => FileSystem.addFolder(arg, arg_1));
        (arg_2 = combine(path, ".gitkeep"), (arg_3: FileSystem): FileSystem => FileSystem.addFile(arg_2, arg_3));
        (arg_4: FileSystem): ((arg0: ARC) => ARC) => ((arg_5: ARC): ARC => ARC.updateFileSystem(arg_4, arg_5));
        throw new Error();
    }
    static addEmptyFolders(paths: string[], arc: ARC): ARC {
        fold<string, ARC>((arc_1: ARC, path: string): ARC => ARC.addEmptyFolder(path, arc_1), arc, paths);
        throw new Error();
    }
    static addAssay(assay: Assay, studyIdentifier: string, arc: ARC): ARC {
        const assayFolderPath: string = combineAssayFolderPath(assay);
        const assaySubFolderPaths: string[] = combineAssaySubfolderPaths(assay);
        const assayReadmeFilePath: string = combine(assayFolderPath, "Readme.md");
        const arg_9: Investigation = Investigation.addAssay(assay, studyIdentifier, arc.ISA);
        let arg_10: ARC;
        let arg_8: ARC;
        const arg_6: ARC = ARC.addFolder(assayFolderPath, arc);
        arg_8 = ARC.addEmptyFolders(assaySubFolderPaths, arg_6);
        arg_10 = ARC.addFile(assayReadmeFilePath, arg_8);
        return ARC.updateISA(arg_9, arg_10);
    }
}

export function ARC_$reflection(): TypeInfo {
    return record_type("ARC.ARC", [], ARC, () => [["ISA", Investigation_$reflection()], ["CWL", unit_type], ["FileSystem", FileSystem_$reflection()]]);
}

