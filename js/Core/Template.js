import { safeHash, equals } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { Union } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type, union_type, string_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { newGuid } from "../fable_modules/fable-library-js.4.22.0/Guid.js";
import { ArcTable } from "./Table/ArcTable.js";
import { SemVer_tryOfString_Z721C83C5 } from "./Helper/SemVer.js";
import { ResizeArray_map } from "./Helper/Collections.js";
import { hashDateTime, boxHashSeq, boxHashArray } from "./Helper/HashCodes.js";
import { now } from "../fable_modules/fable-library-js.4.22.0/Date.js";

export class Organisation extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["DataPLANT", "Other"];
    }
    static ofString(str) {
        return (str.toLocaleLowerCase() === "dataplant") ? (new Organisation(0, [])) : (new Organisation(1, [str]));
    }
    toString() {
        const this$ = this;
        return (this$.tag === 1) ? this$.fields[0] : "DataPLANT";
    }
    IsOfficial() {
        const this$ = this;
        return equals(this$, new Organisation(0, []));
    }
}

export function Organisation_$reflection() {
    return union_type("ARCtrl.Organisation", [], Organisation, () => [[], [["Item", string_type]]]);
}

export class Template {
    constructor(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated) {
        const name_1 = defaultArg(name, "");
        const description_1 = defaultArg(description, "");
        const organisation_1 = defaultArg(organisation, new Organisation(1, ["Custom Organisation"]));
        const version_1 = defaultArg(version, "0.0.0");
        const authors_1 = defaultArg(authors, []);
        const repos_1 = defaultArg(repos, []);
        const tags_1 = defaultArg(tags, []);
        const lastUpdated_1 = defaultArg(lastUpdated, now());
        this["Id@"] = id;
        this["Table@"] = table;
        this["Name@"] = name_1;
        this["Description@"] = description_1;
        this["Organisation@"] = organisation_1;
        this["Version@"] = version_1;
        this["Authors@"] = authors_1;
        this["EndpointRepositories@"] = repos_1;
        this["Tags@"] = tags_1;
        this["LastUpdated@"] = lastUpdated_1;
    }
    get Id() {
        const __ = this;
        return __["Id@"];
    }
    set Id(v) {
        const __ = this;
        __["Id@"] = v;
    }
    get Table() {
        const __ = this;
        return __["Table@"];
    }
    set Table(v) {
        const __ = this;
        __["Table@"] = v;
    }
    get Name() {
        const __ = this;
        return __["Name@"];
    }
    set Name(v) {
        const __ = this;
        __["Name@"] = v;
    }
    get Description() {
        const __ = this;
        return __["Description@"];
    }
    set Description(v) {
        const __ = this;
        __["Description@"] = v;
    }
    get Organisation() {
        const __ = this;
        return __["Organisation@"];
    }
    set Organisation(v) {
        const __ = this;
        __["Organisation@"] = v;
    }
    get Version() {
        const __ = this;
        return __["Version@"];
    }
    set Version(v) {
        const __ = this;
        __["Version@"] = v;
    }
    get Authors() {
        const __ = this;
        return __["Authors@"];
    }
    set Authors(v) {
        const __ = this;
        __["Authors@"] = v;
    }
    get EndpointRepositories() {
        const __ = this;
        return __["EndpointRepositories@"];
    }
    set EndpointRepositories(v) {
        const __ = this;
        __["EndpointRepositories@"] = v;
    }
    get Tags() {
        const __ = this;
        return __["Tags@"];
    }
    set Tags(v) {
        const __ = this;
        __["Tags@"] = v;
    }
    get LastUpdated() {
        const __ = this;
        return __["LastUpdated@"];
    }
    set LastUpdated(v) {
        const __ = this;
        __["LastUpdated@"] = v;
    }
    static make(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated) {
        return new Template(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated);
    }
    static create(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated) {
        return new Template(id, table, unwrap(name), unwrap(description), unwrap(organisation), unwrap(version), unwrap(authors), unwrap(repos), unwrap(tags), unwrap(lastUpdated));
    }
    static init(templateName) {
        return new Template(newGuid(), ArcTable.init(templateName), templateName);
    }
    get SemVer() {
        const this$ = this;
        return unwrap(SemVer_tryOfString_Z721C83C5(this$.Version));
    }
    ReferenceEquals(other) {
        const this$ = this;
        return this$ === other;
    }
    StructurallyEquals(other) {
        const this$ = this;
        return safeHash(this$) === safeHash(other);
    }
    Copy() {
        const this$ = this;
        const nextAuthors = ResizeArray_map((c) => c.Copy(), this$.Authors);
        const nextRepos = ResizeArray_map((c_1) => c_1.Copy(), this$.EndpointRepositories);
        const nextTags = ResizeArray_map((c_2) => c_2.Copy(), this$.Tags);
        return Template.create(this$.Id, this$.Table.Copy(), this$.Name, this$.Description, this$.Organisation, this$.Version, nextAuthors, nextRepos, nextTags, this$.LastUpdated);
    }
    Equals(other) {
        let template;
        const this$ = this;
        return (other instanceof Template) && ((template = other, this$.StructurallyEquals(template)));
    }
    GetHashCode() {
        let copyOfStruct;
        const this$ = this;
        return boxHashArray([(copyOfStruct = this$.Id, copyOfStruct), safeHash(this$.Table), this$.Name, safeHash(this$.Organisation), this$.Version, boxHashSeq(this$.Authors), boxHashSeq(this$.EndpointRepositories), boxHashSeq(this$.Tags), hashDateTime(this$.LastUpdated)]) | 0;
    }
}

export function Template_$reflection() {
    return class_type("ARCtrl.Template", undefined, Template);
}

export function Template_$ctor_Z17EA765C(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated) {
    return new Template(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated);
}

