import { map as map_1, defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_map } from "./Helper/Collections.js";
import { boxHashSeq, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { safeHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { StringBuilder__Append_Z721C83C5, StringBuilder_$ctor } from "../fable_modules/fable-library-js.4.22.0/System.Text.js";
import { printf, toText, join } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { ofArray, choose, map } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { length } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Publication {
    constructor(pubMedID, doi, authors, title, status, comments) {
        this._pubMedID = pubMedID;
        this._doi = doi;
        this._authors = authors;
        this._title = title;
        this._status = status;
        this._comments = defaultArg(comments, []);
    }
    get PubMedID() {
        const this$ = this;
        return unwrap(this$._pubMedID);
    }
    set PubMedID(pubMedID) {
        const this$ = this;
        this$._pubMedID = pubMedID;
    }
    get DOI() {
        const this$ = this;
        return unwrap(this$._doi);
    }
    set DOI(doi) {
        const this$ = this;
        this$._doi = doi;
    }
    get Authors() {
        const this$ = this;
        return unwrap(this$._authors);
    }
    set Authors(authors) {
        const this$ = this;
        this$._authors = authors;
    }
    get Title() {
        const this$ = this;
        return unwrap(this$._title);
    }
    set Title(title) {
        const this$ = this;
        this$._title = title;
    }
    get Status() {
        const this$ = this;
        return unwrap(this$._status);
    }
    set Status(status) {
        const this$ = this;
        this$._status = status;
    }
    get Comments() {
        const this$ = this;
        return this$._comments;
    }
    set Comments(comments) {
        const this$ = this;
        this$._comments = comments;
    }
    static make(pubMedID, doi, authors, title, status, comments) {
        return new Publication(unwrap(pubMedID), unwrap(doi), unwrap(authors), unwrap(title), unwrap(status), comments);
    }
    static create(pubMedID, doi, authors, title, status, comments) {
        const comments_1 = defaultArg(comments, []);
        return Publication.make(pubMedID, doi, authors, title, status, comments_1);
    }
    static empty() {
        return Publication.create();
    }
    Copy() {
        const this$ = this;
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        const pubMedID = this$.PubMedID;
        const doi = this$.DOI;
        const authors = this$.Authors;
        const title = this$.Title;
        const status = this$.Status;
        return Publication.make(pubMedID, doi, authors, title, status, nextComments);
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.DOI), boxHashOption(this$.Title), boxHashOption(this$.Authors), boxHashOption(this$.PubMedID), boxHashOption(this$.Status), boxHashSeq(this$.Comments)]) | 0;
    }
    Equals(obj) {
        let p;
        const this$ = this;
        return (obj instanceof Publication) && ((p = obj, safeHash(this$) === safeHash(p)));
    }
    toString() {
        let option, clo, arg_1;
        const this$ = this;
        const sb = StringBuilder_$ctor();
        StringBuilder__Append_Z721C83C5(sb, "Publication {\n\t");
        StringBuilder__Append_Z721C83C5(sb, join(",\n\t", map((tupledArg_1) => toText(printf("%s = %A"))(tupledArg_1[0])(tupledArg_1[1]), choose((tupledArg) => map_1((o) => [tupledArg[0], o], tupledArg[1]), ofArray([["PubMedID", this$.PubMedID], ["DOI", this$.DOI], ["Authors", this$.Authors], ["Title", this$.Title], ["Status", (option = this$.Status, map_1((clo = toText(printf("%A")), clo), option))], ["Comments", (length(this$.Comments) > 0) ? ((arg_1 = this$.Comments, toText(printf("%A"))(arg_1))) : undefined]])))));
        StringBuilder__Append_Z721C83C5(sb, "\n}");
        return toString(sb);
    }
}

export function Publication_$reflection() {
    return class_type("ARCtrl.Publication", undefined, Publication);
}

export function Publication_$ctor_Z645CED36(pubMedID, doi, authors, title, status, comments) {
    return new Publication(pubMedID, doi, authors, title, status, comments);
}

