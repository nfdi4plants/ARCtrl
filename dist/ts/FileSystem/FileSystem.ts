import { defaultArg } from "../fable_modules/fable-library-ts/Option.js";
import { Commit_$reflection, Commit } from "./Commit.js";
import { FileSystemTree_$reflection, FileSystemTree, FileSystemTree_$union } from "./FileSystemTree.js";
import { append } from "../fable_modules/fable-library-ts/Array.js";
import { Record } from "../fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "../fable_modules/fable-library-ts/Util.js";
import { record_type, array_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export class FileSystem extends Record implements IEquatable<FileSystem>, IComparable<FileSystem> {
    readonly Tree: FileSystemTree_$union;
    readonly History: Commit[];
    constructor(Tree: FileSystemTree_$union, History: Commit[]) {
        super();
        this.Tree = Tree;
        this.History = History;
    }
    static create({ tree, history }: {tree: FileSystemTree_$union, history?: Commit[] }): FileSystem {
        return new FileSystem(tree, defaultArg<Commit[]>(history, []));
    }
    AddFile(path: string): FileSystem {
        const this$: FileSystem = this;
        return new FileSystem(this$.Tree.AddFile(path), this$.History);
    }
    static addFile(path: string): ((arg0: FileSystem) => FileSystem) {
        return (fs: FileSystem): FileSystem => fs.AddFile(path);
    }
    static fromFilePaths(paths: string[]): FileSystem {
        const tree: FileSystemTree_$union = FileSystemTree.fromFilePaths(paths);
        return FileSystem.create({
            tree: tree,
        });
    }
    Union(other: FileSystem): FileSystem {
        const this$: FileSystem = this;
        const tree: FileSystemTree_$union = this$.Tree.Union(other.Tree);
        const history: Commit[] = append<Commit>(this$.History, other.History);
        return FileSystem.create({
            tree: tree,
            history: history,
        });
    }
}

export function FileSystem_$reflection(): TypeInfo {
    return record_type("ARCtrl.FileSystem.FileSystem", [], FileSystem, () => [["Tree", FileSystemTree_$reflection()], ["History", array_type(Commit_$reflection())]]);
}

