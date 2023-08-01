import { defaultArg } from "../fable_modules/fable-library.4.1.4/Option.js";
import { FileSystemTree_$reflection, FileSystemTree } from "./FileSystemTree.js";
import { Record } from "../fable_modules/fable-library.4.1.4/Types.js";
import { Commit_$reflection } from "./Commit.js";
import { record_type, array_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";

export class FileSystem extends Record {
    constructor(Tree, History) {
        super();
        this.Tree = Tree;
        this.History = History;
    }
    static create({ tree, history }) {
        return new FileSystem(tree, defaultArg(history, []));
    }
    AddFile(path) {
        const this$ = this;
        return new FileSystem(this$.Tree.AddFile(path), this$.History);
    }
    static addFile(path) {
        return (fs) => fs.AddFile(path);
    }
    static fromFilePaths(paths) {
        const tree = FileSystemTree.fromFilePaths(paths);
        return FileSystem.create({
            tree: tree,
        });
    }
}

export function FileSystem_$reflection() {
    return record_type("FileSystem.FileSystem", [], FileSystem, () => [["Tree", FileSystemTree_$reflection()], ["History", array_type(Commit_$reflection())]]);
}

