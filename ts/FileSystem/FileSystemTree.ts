import { split } from "./Path.js";
import { FSharpList } from "../fable_modules/fable-library-ts/List.js";
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
    static createFile(name: string): FileSystemTree_$union {
        return FileSystemTree_File(name);
    }
    static createFolder(name: string, children: FileSystemTree_$union[]): FileSystemTree_$union {
        return FileSystemTree_Folder(name, children);
    }
    static addFolder(path: string, fileSystem: FileSystemTree_$union): FileSystemTree_$union {
        const x: FSharpList<string> = split("/", path);
        throw new Error();
    }
    static addFile(path: string, fileSystem: FileSystemTree_$union): FileSystemTree_$union {
        const x: FSharpList<string> = split("/", path);
        throw new Error();
    }
}

export function FileSystemTree_$reflection(): TypeInfo {
    return union_type("FileSystem.FileSystemTree", [], FileSystemTree, () => [[["name", string_type]], [["name", string_type], ["children", array_type(FileSystemTree_$reflection())]]]);
}

