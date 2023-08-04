import { unwrap, defaultArg } from "../fable_modules/fable-library.4.1.4/Option.js";
import { singleton, append, delay, toArray } from "../fable_modules/fable-library.4.1.4/Seq.js";
import { append as append_1, choose, equalsWith, tail, head, map } from "../fable_modules/fable-library.4.1.4/Array.js";
import { Array_distinct, Array_groupBy } from "../fable_modules/fable-library.4.1.4/Seq2.js";
import { curry2, arrayHash, stringHash } from "../fable_modules/fable-library.4.1.4/Util.js";
import { combineMany, split } from "./Path.js";
import { empty, reverse, toArray as toArray_1, cons } from "../fable_modules/fable-library.4.1.4/List.js";
import { Union } from "../fable_modules/fable-library.4.1.4/Types.js";
import { union_type, array_type, string_type } from "../fable_modules/fable-library.4.1.4/Reflection.js";

export class FileSystemTree extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["File", "Folder"];
    }
    get Name() {
        const this$ = this;
        let n;
        if (this$.tag === 1) {
            n = this$.fields[0];
        }
        else {
            n = this$.fields[0];
        }
        return n;
    }
    get isFolder() {
        const this$ = this;
        return this$.tag === 1;
    }
    get isFile() {
        const this$ = this;
        return this$.tag === 0;
    }
    static get ROOT_NAME() {
        return "root";
    }
    static createFile(name) {
        return new FileSystemTree(0, [name]);
    }
    static createFolder(name, children) {
        return new FileSystemTree(1, [name, defaultArg(children, [])]);
    }
    static createRootFolder(children) {
        return new FileSystemTree(1, [FileSystemTree.ROOT_NAME, children]);
    }
    AddFile(path) {
        const this$ = this;
        const existingPaths = this$.ToFilePaths();
        const filePaths = toArray(delay(() => append(singleton(path), delay(() => existingPaths))));
        return FileSystemTree.fromFilePaths(filePaths);
    }
    static addFile(path) {
        return (tree) => tree.AddFile(path);
    }
    static fromFilePaths(paths) {
        const loop = (paths_1, parent) => {
            const files = map((arg_1) => {
                const arg = head(arg_1);
                return FileSystemTree.createFile(arg);
            }, paths_1.filter((p) => (p.length === 1)));
            const folders = map((tupledArg) => {
                const parent_1 = FileSystemTree.createFolder(tupledArg[0], []);
                return loop(map(tail, tupledArg[1]), parent_1);
            }, Array_groupBy(head, paths_1.filter((p_1) => (p_1.length > 1)), {
                Equals: (x_3, y_2) => (x_3 === y_2),
                GetHashCode: stringHash,
            }));
            if (parent.tag === 1) {
                return FileSystemTree.createFolder(parent.fields[0], toArray(delay(() => append(files, delay(() => folders)))));
            }
            else {
                return parent;
            }
        };
        return loop(Array_distinct(map(split, paths), {
            Equals: (x, y) => equalsWith((x_1, y_1) => (x_1 === y_1), x, y),
            GetHashCode: arrayHash,
        }), FileSystemTree.createFolder(FileSystemTree.ROOT_NAME, []));
    }
    ToFilePaths(removeRoot) {
        const this$ = this;
        const res = [];
        const loop = (output, parent) => {
            if (parent.tag === 1) {
                parent.fields[1].forEach((filest) => {
                    loop(cons(parent.fields[0], output), filest);
                });
            }
            else {
                const arg = combineMany(toArray_1(reverse(cons(parent.fields[0], output))));
                void (res.push(arg));
            }
        };
        if (defaultArg(removeRoot, true)) {
            if (this$.tag === 0) {
                void (res.push(this$.fields[0]));
            }
            else {
                const action_1 = curry2(loop)(empty());
                this$.fields[1].forEach(action_1);
            }
        }
        else {
            loop(empty(), this$);
        }
        return Array.from(res);
    }
    static toFilePaths(removeRoot) {
        return (root) => root.ToFilePaths(unwrap(removeRoot));
    }
    Filter(predicate) {
        const this$ = this;
        const loop = (parent) => {
            if (parent.tag === 1) {
                const filteredChildren = choose(loop, parent.fields[1]);
                if (filteredChildren.length === 0) {
                    return void 0;
                }
                else {
                    return new FileSystemTree(1, [parent.fields[0], filteredChildren]);
                }
            }
            else {
                const n = parent.fields[0];
                if (predicate(n)) {
                    return new FileSystemTree(0, [n]);
                }
                else {
                    return void 0;
                }
            }
        };
        return loop(this$);
    }
    static filter(predicate) {
        return (tree) => tree.Filter(predicate);
    }
    Union(otherFST) {
        let array_1;
        const this$ = this;
        const arg = Array_distinct((array_1 = this$.ToFilePaths(), append_1(otherFST.ToFilePaths(), array_1)), {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        });
        return FileSystemTree.fromFilePaths(arg);
    }
}

export function FileSystemTree_$reflection() {
    return union_type("ARCtrl.FileSystem.FileSystemTree", [], FileSystemTree, () => [[["name", string_type]], [["name", string_type], ["children", array_type(FileSystemTree_$reflection())]]]);
}

