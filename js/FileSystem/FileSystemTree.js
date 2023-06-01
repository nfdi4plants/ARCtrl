import { split } from "./Path.js";
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
    static createFile(name) {
        return new FileSystemTree(0, [name]);
    }
    static createFolder(name, children) {
        return new FileSystemTree(1, [name, children]);
    }
    static addFolder(path, fileSystem) {
        const x = split("/", path);
        throw new Error();
    }
    static addFile(path, fileSystem) {
        const x = split("/", path);
        throw new Error();
    }
}

export function FileSystemTree_$reflection() {
    return union_type("FileSystem.FileSystemTree", [], FileSystemTree, () => [[["name", string_type]], [["name", string_type], ["children", array_type(FileSystemTree_$reflection())]]]);
}

