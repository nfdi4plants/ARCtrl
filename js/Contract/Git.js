import { Contract, CLITool } from "./Contract.js";
import { dgi } from "../FileSystem/DefaultGitignore.js";
import { dga } from "../FileSystem/DefaultGitattributes.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { value, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { empty, singleton, append, delay, toArray } from "../fable_modules/fable-library-js.4.22.0/Seq.js";

export function gitWithArgs(arguments$) {
    return CLITool.create("git", arguments$);
}

export function createGitContractAt(path, arguments$) {
    return Contract.createExecute(gitWithArgs(arguments$), path);
}

export function createGitContract(arguments$) {
    return Contract.createExecute(gitWithArgs(arguments$));
}

export const gitignoreContract = Contract.createCreate(".gitignore", "PlainText", dgi);

export const gitattributesContract = Contract.createCreate(".gitattributes", "PlainText", dga);

export class Init {
    constructor() {
    }
}

export function Init_$reflection() {
    return class_type("ARCtrl.Contract.Git.Init", undefined, Init);
}

export class Clone {
    constructor() {
    }
}

export function Clone_$reflection() {
    return class_type("ARCtrl.Contract.Git.Clone", undefined, Clone);
}

export function Init_get_init() {
    return "init";
}

export function Init_get_branchFlag() {
    return "-b";
}

export function Init_get_remote() {
    return "remote";
}

export function Init_get_add() {
    return "add";
}

export function Init_get_origin() {
    return "origin";
}

export function Init_createInitContract_6DFDD678(branch) {
    const branch_1 = defaultArg(branch, "main");
    return createGitContract([Init_get_init(), Init_get_branchFlag(), branch_1]);
}

export function Init_createAddRemoteContract_Z721C83C5(remoteUrl) {
    return createGitContract([Init_get_remote(), Init_get_add(), Init_get_origin(), remoteUrl]);
}

export function Clone_get_clone() {
    return "clone";
}

export function Clone_get_branchFlag() {
    return "-b";
}

export function Clone_get_noLFSConfig() {
    return "-c \"filter.lfs.smudge = git-lfs smudge --skip -- %f\" -c \"filter.lfs.process = git-lfs filter-process --skip\"";
}

export function Clone_formatRepoString(username, pass, url) {
    return replace(url, "https://", "https://" + (((username + ":") + pass) + "@"));
}

export function Clone_createCloneContract_5000466F(remoteUrl, merge, branch, token, nolfs) {
    const nolfs_1 = defaultArg(nolfs, false);
    const merge_1 = defaultArg(merge, false);
    const remoteUrl_1 = (token == null) ? remoteUrl : Clone_formatRepoString(token[0], token[1], remoteUrl);
    return createGitContract(toArray(delay(() => append(singleton(Clone_get_clone()), delay(() => append(nolfs_1 ? singleton(Clone_get_noLFSConfig()) : empty(), delay(() => append((branch != null) ? singleton(Clone_get_branchFlag()) : empty(), delay(() => append((branch != null) ? singleton(value(branch)) : empty(), delay(() => append(singleton(remoteUrl_1), delay(() => (merge_1 ? singleton(".") : empty()))))))))))))));
}

