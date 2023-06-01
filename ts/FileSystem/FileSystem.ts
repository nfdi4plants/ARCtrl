import { FileSystemTree_$reflection, FileSystemTree_$union, FileSystemTree } from "./FileSystemTree.js";
import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { FSharpList } from "../fable_modules/fable-library-ts/List.js";
import { Commit_$reflection, Commit } from "./Commit.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { record_type, list_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export class FileSystem extends Record implements IEquatable<FileSystem>, IComparable<FileSystem> {
    readonly Tree: FileSystemTree_$union;
    readonly History: FSharpList<Commit>;
    constructor(Tree: FileSystemTree_$union, History: FSharpList<Commit>) {
        super();
        this.Tree = Tree;
        this.History = History;
    }
    static addFolder(path: string, fileSystem: FileSystem): FileSystem {
        (arg: string): ((arg0: FileSystemTree_$union) => FileSystemTree_$union) => ((arg_1: FileSystemTree_$union): FileSystemTree_$union => FileSystemTree.addFolder(arg, arg_1));
        throw new Error();
    }
    static addFile(path: string, fileSystem: FileSystem): FileSystem {
        (arg: string): ((arg0: FileSystemTree_$union) => FileSystemTree_$union) => ((arg_1: FileSystemTree_$union): FileSystemTree_$union => FileSystemTree.addFile(arg, arg_1));
        throw new Error();
    }
}

export function FileSystem_$reflection(): TypeInfo {
    return record_type("FileSystem.FileSystem", [], FileSystem, () => [["Tree", FileSystemTree_$reflection()], ["History", list_type(Commit_$reflection())]]);
}

