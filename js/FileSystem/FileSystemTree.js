import { unwrap, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { append as append_1, choose, equalsWith, tail, head, map, tryFind } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { singleton, append, delay, toArray } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { Array_distinct, Array_groupBy } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { curry2, arrayHash, stringHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { combineMany, split } from "./Path.js";
import { empty, reverse, toArray as toArray_1, cons } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { Union } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { union_type, array_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

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
    TryGetChildByName(name) {
        const this$ = this;
        return (this$.tag === 0) ? undefined : tryFind((c) => (c.Name === name), this$.fields[1]);
    }
    static tryGetChildByName(name) {
        return (fst) => fst.TryGetChildByName(name);
    }
    ContainsChildWithName(name) {
        const this$ = this;
        return (this$.tag === 0) ? false : this$.fields[1].some((c) => (c.Name === name));
    }
    static containsChildWithName(name) {
        return (fst) => fst.ContainsChildWithName(name);
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
            const files = map((arg) => {
                const name = head(arg);
                return FileSystemTree.createFile(name);
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
                const item = combineMany(toArray_1(reverse(cons(parent.fields[0], output))));
                void (res.push(item));
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
    FilterFiles(predicate) {
        const this$ = this;
        const loop = (parent) => {
            if (parent.tag === 1) {
                return new FileSystemTree(1, [parent.fields[0], choose(loop, parent.fields[1])]);
            }
            else {
                const n = parent.fields[0];
                if (predicate(n)) {
                    return new FileSystemTree(0, [n]);
                }
                else {
                    return undefined;
                }
            }
        };
        return loop(this$);
    }
    static filterFiles(predicate) {
        return (tree) => tree.FilterFiles(predicate);
    }
    FilterFolders(predicate) {
        const this$ = this;
        const loop = (parent) => {
            if (parent.tag === 1) {
                const n_1 = parent.fields[0];
                if (predicate(n_1)) {
                    return new FileSystemTree(1, [n_1, choose(loop, parent.fields[1])]);
                }
                else {
                    return undefined;
                }
            }
            else {
                return new FileSystemTree(0, [parent.fields[0]]);
            }
        };
        return loop(this$);
    }
    static filterFolders(predicate) {
        return (tree) => tree.FilterFolders(predicate);
    }
    Filter(predicate) {
        const this$ = this;
        const loop = (parent) => {
            if (parent.tag === 1) {
                const n_1 = parent.fields[0];
                if (predicate(n_1)) {
                    const filteredChildren = choose(loop, parent.fields[1]);
                    if (filteredChildren.length === 0) {
                        return undefined;
                    }
                    else {
                        return new FileSystemTree(1, [n_1, filteredChildren]);
                    }
                }
                else {
                    return undefined;
                }
            }
            else {
                const n = parent.fields[0];
                if (predicate(n)) {
                    return new FileSystemTree(0, [n]);
                }
                else {
                    return undefined;
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
        const paths = Array_distinct((array_1 = this$.ToFilePaths(), append_1(otherFST.ToFilePaths(), array_1)), {
            Equals: (x, y) => (x === y),
            GetHashCode: stringHash,
        });
        return FileSystemTree.fromFilePaths(paths);
    }
    Copy() {
        const this$ = this;
        return (this$.tag === 0) ? (new FileSystemTree(0, [this$.fields[0]])) : (new FileSystemTree(1, [this$.fields[0], map((c) => c.Copy(), this$.fields[1])]));
    }
    static createGitKeepFile() {
        return FileSystemTree.createFile(".gitkeep");
    }
    static createReadmeFile() {
        return FileSystemTree.createFile("README.md");
    }
    static createEmptyFolder(name) {
        return FileSystemTree.createFolder(name, [FileSystemTree.createGitKeepFile()]);
    }
    static createAssayFolder(assayName, hasDataMap) {
        const hasDataMap_1 = defaultArg(hasDataMap, false);
        const dataset = FileSystemTree.createEmptyFolder("dataset");
        const protocols = FileSystemTree.createEmptyFolder("protocols");
        const readme = FileSystemTree.createReadmeFile();
        const assayFile = FileSystemTree.createFile("isa.assay.xlsx");
        if (hasDataMap_1) {
            const dataMapFile = FileSystemTree.createFile("isa.datamap.xlsx");
            return FileSystemTree.createFolder(assayName, [dataset, protocols, assayFile, readme, dataMapFile]);
        }
        else {
            return FileSystemTree.createFolder(assayName, [dataset, protocols, assayFile, readme]);
        }
    }
    static createStudyFolder(studyName, hasDataMap) {
        const hasDataMap_1 = defaultArg(hasDataMap, false);
        const resources = FileSystemTree.createEmptyFolder("resources");
        const protocols = FileSystemTree.createEmptyFolder("protocols");
        const readme = FileSystemTree.createReadmeFile();
        const studyFile = FileSystemTree.createFile("isa.study.xlsx");
        if (hasDataMap_1) {
            const dataMapFile = FileSystemTree.createFile("isa.datamap.xlsx");
            return FileSystemTree.createFolder(studyName, [resources, protocols, studyFile, readme, dataMapFile]);
        }
        else {
            return FileSystemTree.createFolder(studyName, [resources, protocols, studyFile, readme]);
        }
    }
    static createInvestigationFile() {
        return FileSystemTree.createFile("isa.investigation.xlsx");
    }
    static createAssaysFolder(assays) {
        return FileSystemTree.createFolder("assays", append_1([FileSystemTree.createGitKeepFile()], assays));
    }
    static createStudiesFolder(studies) {
        return FileSystemTree.createFolder("studies", append_1([FileSystemTree.createGitKeepFile()], studies));
    }
    static createWorkflowsFolder(workflows) {
        return FileSystemTree.createFolder("workflows", append_1([FileSystemTree.createGitKeepFile()], workflows));
    }
    static createRunsFolder(runs) {
        return FileSystemTree.createFolder("runs", append_1([FileSystemTree.createGitKeepFile()], runs));
    }
}

export function FileSystemTree_$reflection() {
    return union_type("ARCtrl.FileSystem.FileSystemTree", [], FileSystemTree, () => [[["name", string_type]], [["name", string_type], ["children", array_type(FileSystemTree_$reflection())]]]);
}

