import { FileSystemTree_$reflection, FileSystemTree } from "./FileSystemTree.js";
import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { Commit_$reflection } from "./Commit.js";
import { record_type, list_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";

export class FileSystem extends Record {
    constructor(Tree, History) {
        super();
        this.Tree = Tree;
        this.History = History;
    }
    static addFolder(path, fileSystem) {
        (arg) => ((arg_1) => FileSystemTree.addFolder(arg, arg_1));
        throw new Error();
    }
    static addFile(path, fileSystem) {
        (arg) => ((arg_1) => FileSystemTree.addFile(arg, arg_1));
        throw new Error();
    }
}

export function FileSystem_$reflection() {
    return record_type("FileSystem.FileSystem", [], FileSystem, () => [["Tree", FileSystemTree_$reflection()], ["History", list_type(Commit_$reflection())]]);
}

