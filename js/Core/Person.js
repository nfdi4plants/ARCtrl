import { map as map_1, defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryFind } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { equals } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { ofArray, choose, map, singleton, append } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { ResizeArray_map } from "./Helper/Collections.js";
import { hash, boxHashSeq, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { StringBuilder__Append_Z721C83C5, StringBuilder_$ctor } from "../fable_modules/fable-library-js.4.22.0/System.Text.js";
import { printf, toText, join } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { length } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Person {
    constructor(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments) {
        this._orcid = orcid;
        this._lastName = lastName;
        this._firstName = firstName;
        this._midInitials = midInitials;
        this._email = email;
        this._phone = phone;
        this._fax = fax;
        this._address = address;
        this._affiliation = affiliation;
        this._roles = defaultArg(roles, []);
        this._comments = defaultArg(comments, []);
    }
    get ORCID() {
        const this$ = this;
        return unwrap(this$._orcid);
    }
    set ORCID(orcid) {
        const this$ = this;
        this$._orcid = orcid;
    }
    get FirstName() {
        const this$ = this;
        return unwrap(this$._firstName);
    }
    set FirstName(firstName) {
        const this$ = this;
        this$._firstName = firstName;
    }
    get LastName() {
        const this$ = this;
        return unwrap(this$._lastName);
    }
    set LastName(lastName) {
        const this$ = this;
        this$._lastName = lastName;
    }
    get MidInitials() {
        const this$ = this;
        return unwrap(this$._midInitials);
    }
    set MidInitials(midInitials) {
        const this$ = this;
        this$._midInitials = midInitials;
    }
    get Address() {
        const this$ = this;
        return unwrap(this$._address);
    }
    set Address(address) {
        const this$ = this;
        this$._address = address;
    }
    get Affiliation() {
        const this$ = this;
        return unwrap(this$._affiliation);
    }
    set Affiliation(affiliation) {
        const this$ = this;
        this$._affiliation = affiliation;
    }
    get EMail() {
        const this$ = this;
        return unwrap(this$._email);
    }
    set EMail(email) {
        const this$ = this;
        this$._email = email;
    }
    get Phone() {
        const this$ = this;
        return unwrap(this$._phone);
    }
    set Phone(phone) {
        const this$ = this;
        this$._phone = phone;
    }
    get Fax() {
        const this$ = this;
        return unwrap(this$._fax);
    }
    set Fax(fax) {
        const this$ = this;
        this$._fax = fax;
    }
    get Roles() {
        const this$ = this;
        return this$._roles;
    }
    set Roles(roles) {
        const this$ = this;
        this$._roles = roles;
    }
    get Comments() {
        const this$ = this;
        return this$._comments;
    }
    set Comments(comments) {
        const this$ = this;
        this$._comments = comments;
    }
    static make(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments) {
        return new Person(unwrap(orcid), unwrap(lastName), unwrap(firstName), unwrap(midInitials), unwrap(email), unwrap(phone), unwrap(fax), unwrap(address), unwrap(affiliation), roles, comments);
    }
    static create(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments) {
        const roles_1 = defaultArg(roles, []);
        const comments_1 = defaultArg(comments, []);
        return Person.make(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles_1, comments_1);
    }
    static get empty() {
        return Person.create();
    }
    static tryGetByFullName(firstName, midInitials, lastName, persons) {
        return tryFind((p) => {
            if (midInitials === "") {
                if (equals(p.FirstName, firstName)) {
                    return equals(p.LastName, lastName);
                }
                else {
                    return false;
                }
            }
            else if (equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) {
                return equals(p.LastName, lastName);
            }
            else {
                return false;
            }
        }, persons);
    }
    static existsByFullName(firstName, midInitials, lastName, persons) {
        return persons.some((p) => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
    }
    static add(persons, person) {
        return append(persons, singleton(person));
    }
    static removeByFullName(firstName, midInitials, lastName, persons) {
        return persons.filter((p) => ((midInitials === "") ? !(equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : !((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
    }
    Copy() {
        const this$ = this;
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        const nextRoles = ResizeArray_map((c_1) => c_1.Copy(), this$.Roles);
        const orcid = this$.ORCID;
        const lastName = this$.LastName;
        const firstName = this$.FirstName;
        const midInitials = this$.MidInitials;
        const email = this$.EMail;
        const phone = this$.Phone;
        const fax = this$.Fax;
        const address = this$.Address;
        const affiliation = this$.Affiliation;
        return Person.make(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, nextRoles, nextComments);
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.ORCID), boxHashOption(this$.LastName), boxHashOption(this$.FirstName), boxHashOption(this$.MidInitials), boxHashOption(this$.EMail), boxHashOption(this$.Phone), boxHashOption(this$.Fax), boxHashOption(this$.Address), boxHashOption(this$.Affiliation), boxHashSeq(this$.Roles), boxHashSeq(this$.Comments)]) | 0;
    }
    Equals(obj) {
        const this$ = this;
        return hash(this$) === hash(obj);
    }
    toString() {
        let arg, arg_1;
        const this$ = this;
        const sb = StringBuilder_$ctor();
        StringBuilder__Append_Z721C83C5(sb, "Person {\n\t");
        StringBuilder__Append_Z721C83C5(sb, join(",\n\t", map((tupledArg_1) => toText(printf("%s = %A"))(tupledArg_1[0])(tupledArg_1[1]), choose((tupledArg) => map_1((o) => [tupledArg[0], o], tupledArg[1]), ofArray([["FirstName", this$.FirstName], ["LastName", this$.LastName], ["MidInitials", this$.MidInitials], ["EMail", this$.EMail], ["Phone", this$.Phone], ["Address", this$.Address], ["Affiliation", this$.Affiliation], ["Fax", this$.Fax], ["ORCID", this$.ORCID], ["Roles", (length(this$.Roles) > 0) ? ((arg = this$.Roles, toText(printf("%A"))(arg))) : undefined], ["Comments", (length(this$.Comments) > 0) ? ((arg_1 = this$.Comments, toText(printf("%A"))(arg_1))) : undefined]])))));
        StringBuilder__Append_Z721C83C5(sb, "\n}");
        return toString(sb);
    }
}

export function Person_$reflection() {
    return class_type("ARCtrl.Person", undefined, Person);
}

export function Person_$ctor_Z2F6491B5(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments) {
    return new Person(orcid, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments);
}

