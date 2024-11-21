import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_map } from "./Helper/Collections.js";
import { boxHashSeq, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { safeHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class OntologySourceReference {
    constructor(description, file, name, version, comments) {
        this._description = description;
        this._file = file;
        this._name = name;
        this._version = version;
        this._comments = defaultArg(comments, []);
    }
    get Description() {
        const this$ = this;
        return unwrap(this$._description);
    }
    set Description(description) {
        const this$ = this;
        this$._description = description;
    }
    get File() {
        const this$ = this;
        return unwrap(this$._file);
    }
    set File(file) {
        const this$ = this;
        this$._file = file;
    }
    get Name() {
        const this$ = this;
        return unwrap(this$._name);
    }
    set Name(name) {
        const this$ = this;
        this$._name = name;
    }
    get Version() {
        const this$ = this;
        return unwrap(this$._version);
    }
    set Version(version) {
        const this$ = this;
        this$._version = version;
    }
    get Comments() {
        const this$ = this;
        return this$._comments;
    }
    set Comments(comments) {
        const this$ = this;
        this$._comments = comments;
    }
    static make(description, file, name, version, comments) {
        return new OntologySourceReference(unwrap(description), unwrap(file), unwrap(name), unwrap(version), comments);
    }
    static create(description, file, name, version, comments) {
        const comments_1 = defaultArg(comments, []);
        return OntologySourceReference.make(description, file, name, version, comments_1);
    }
    static get empty() {
        return OntologySourceReference.create();
    }
    Copy() {
        const this$ = this;
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        const description = this$.Description;
        const file = this$.File;
        const name = this$.Name;
        const version = this$.Version;
        return OntologySourceReference.make(description, file, name, version, nextComments);
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.Description), boxHashOption(this$.File), boxHashOption(this$.Name), boxHashOption(this$.Version), boxHashSeq(this$.Comments)]) | 0;
    }
    Equals(obj) {
        let t;
        const this$ = this;
        return (obj instanceof OntologySourceReference) && ((t = obj, safeHash(this$) === safeHash(t)));
    }
}

export function OntologySourceReference_$reflection() {
    return class_type("ARCtrl.OntologySourceReference", undefined, OntologySourceReference);
}

export function OntologySourceReference_$ctor_7C9A7CF8(description, file, name, version, comments) {
    return new OntologySourceReference(description, file, name, version, comments);
}

