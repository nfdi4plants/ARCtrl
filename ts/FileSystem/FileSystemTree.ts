import { Option, unwrap, defaultArg } from "../fable_modules/fable-library-ts/Option.js";
import { singleton, append, delay, toArray } from "../fable_modules/fable-library-ts/Seq.js";
import { choose, equalsWith, tail, head, map } from "../fable_modules/fable-library-ts/Array.js";
import { Array_distinct, Array_groupBy } from "../fable_modules/fable-library-ts/Seq2.js";
import { curry2, arrayHash, stringHash } from "../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../fable_modules/fable-library-ts/Int32.js";
import { combineMany, split } from "./Path.js";
import { empty, FSharpList, reverse, toArray as toArray_1, cons } from "../fable_modules/fable-library-ts/List.js";
import { Union } from "../fable_modules/fable-library-ts/Types.js";
import { union_type, array_type, string_type, TypeInfo } from "../fable_modules/fable-library-ts/Reflection.js";

export type FileSystemTree_$union = 
    | FileSystemTree<0>
    | FileSystemTree<1>

export type FileSystemTree_$cases = {
    0: ["File", [string]],
    1: ["Folder", [string, FileSystemTree_$union[]]]
}

export function FileSystemTree_File(name: string) {
    return new FileSystemTree<0>(0, [name]);
}

export function FileSystemTree_Folder(name: string, children: FileSystemTree_$union[]) {
    return new FileSystemTree<1>(1, [name, children]);
}

export class FileSystemTree<Tag extends keyof FileSystemTree_$cases> extends Union<Tag, FileSystemTree_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: FileSystemTree_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["File", "Folder"];
    }
    get Name(): string {
        const this$ = this as FileSystemTree_$union;
        let n: string;
        if (this$.tag === /* Folder */ 1) {
            n = this$.fields[0];
        }
        else {
            n = this$.fields[0];
        }
        return n!;
    }
    get isFolder(): boolean {
        const this$ = this as FileSystemTree_$union;
        return this$.tag === /* Folder */ 1;
    }
    get isFile(): boolean {
        const this$ = this as FileSystemTree_$union;
        return this$.tag === /* File */ 0;
    }
    static get ROOT_NAME(): string {
        return "root";
    }
    static createFile(name: string): FileSystemTree_$union {
        return FileSystemTree_File(name);
    }
    static createFolder(name: string, children?: FileSystemTree_$union[]): FileSystemTree_$union {
        return FileSystemTree_Folder(name, defaultArg<FileSystemTree_$union[]>(children, []));
    }
    AddFile(path: string): FileSystemTree_$union {
        const this$ = this as FileSystemTree_$union;
        const existingPaths: string[] = this$.ToFilePaths();
        const filePaths: string[] = toArray<string>(delay<string>((): Iterable<string> => append<string>(singleton<string>(path), delay<string>((): Iterable<string> => existingPaths))));
        return FileSystemTree.fromFilePaths(filePaths);
    }
    static addFile(path: string): ((arg0: FileSystemTree_$union) => FileSystemTree_$union) {
        return (tree: FileSystemTree_$union): FileSystemTree_$union => tree.AddFile(path);
    }
    static fromFilePaths(paths: string[]): FileSystemTree_$union {
        const loop = (paths_1: string[][], parent: FileSystemTree_$union): FileSystemTree_$union => {
            const files: FileSystemTree_$union[] = map<string[], FileSystemTree_$union>((arg_1: string[]): FileSystemTree_$union => {
                const arg: string = head<string>(arg_1);
                return FileSystemTree.createFile(arg);
            }, paths_1.filter((p: string[]): boolean => (p.length === 1)));
            const folders: FileSystemTree_$union[] = map<[string, string[][]], FileSystemTree_$union>((tupledArg: [string, string[][]]): FileSystemTree_$union => {
                const parent_1: FileSystemTree_$union = FileSystemTree.createFolder(tupledArg[0], []);
                return loop(map<string[], string[]>(tail, tupledArg[1]), parent_1);
            }, Array_groupBy<string[], string>(head, paths_1.filter((p_1: string[]): boolean => (p_1.length > 1)), {
                Equals: (x_3: string, y_2: string): boolean => (x_3 === y_2),
                GetHashCode: stringHash,
            }));
            if (parent.tag === /* Folder */ 1) {
                const name: string = parent.fields[0];
                return FileSystemTree.createFolder(name, toArray<FileSystemTree_$union>(delay<FileSystemTree_$union>((): Iterable<FileSystemTree_$union> => append<FileSystemTree_$union>(files, delay<FileSystemTree_$union>((): Iterable<FileSystemTree_$union> => folders)))));
            }
            else {
                return parent;
            }
        };
        return loop(Array_distinct<string[]>(map<string, string[]>(split, paths), {
            Equals: (x: string[], y: string[]): boolean => equalsWith((x_1: string, y_1: string): boolean => (x_1 === y_1), x, y),
            GetHashCode: arrayHash,
        }), FileSystemTree.createFolder(FileSystemTree.ROOT_NAME, []));
    }
    ToFilePaths(removeRoot?: boolean): string[] {
        const this$ = this as FileSystemTree_$union;
        const res: string[] = [];
        const loop = (output: FSharpList<string>, parent: FileSystemTree_$union): void => {
            if (parent.tag === /* Folder */ 1) {
                const n_1: string = parent.fields[0];
                const array: FileSystemTree_$union[] = parent.fields[1];
                array.forEach((filest: FileSystemTree_$union): void => {
                    loop(cons(n_1, output), filest);
                });
            }
            else {
                const arg: string = combineMany(toArray_1<string>(reverse<string>(cons(parent.fields[0], output))));
                void (res.push(arg));
            }
        };
        if (defaultArg<boolean>(removeRoot, true)) {
            if (this$.tag === /* File */ 0) {
                const n_2: string = this$.fields[0];
                void (res.push(n_2));
            }
            else {
                const children_1: FileSystemTree_$union[] = this$.fields[1];
                const action_1: ((arg0: FileSystemTree_$union) => void) = curry2(loop)(empty<string>());
                children_1.forEach(action_1);
            }
        }
        else {
            loop(empty<string>(), this$);
        }
        return Array.from(res);
    }
    static toFilePaths(removeRoot?: boolean): ((arg0: FileSystemTree_$union) => string[]) {
        return (root: FileSystemTree_$union): string[] => root.ToFilePaths(unwrap(removeRoot));
    }
    Filter(predicate: ((arg0: string) => boolean)): Option<FileSystemTree_$union> {
        const this$ = this as FileSystemTree_$union;
        const loop = (parent: FileSystemTree_$union): Option<FileSystemTree_$union> => {
            if (parent.tag === /* Folder */ 1) {
                const n_1: string = parent.fields[0];
                const filteredChildren: FileSystemTree_$union[] = choose<FileSystemTree_$union, FileSystemTree_$union>(loop, parent.fields[1]);
                if (filteredChildren.length === 0) {
                    return void 0;
                }
                else {
                    return FileSystemTree_Folder(n_1, filteredChildren);
                }
            }
            else {
                const n: string = parent.fields[0];
                if (predicate(n)) {
                    return FileSystemTree_File(n);
                }
                else {
                    return void 0;
                }
            }
        };
        return loop(this$);
    }
    static filter(predicate: ((arg0: string) => boolean)): ((arg0: FileSystemTree_$union) => Option<FileSystemTree_$union>) {
        return (tree: FileSystemTree_$union): Option<FileSystemTree_$union> => tree.Filter(predicate);
    }
}

export function FileSystemTree_$reflection(): TypeInfo {
    return union_type("FileSystem.FileSystemTree", [], FileSystemTree, () => [[["name", string_type]], [["name", string_type], ["children", array_type(FileSystemTree_$reflection())]]]);
}

