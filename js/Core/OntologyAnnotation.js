import { value as value_4, defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { Pattern_TermAnnotationShortPattern, ActivePatterns_$007CRegex$007C_$007C, tryParseTermAnnotation } from "./Helper/Regex.js";
import { createOAUri } from "./Helper/Url.js";
import { StringBuilder__Append_Z721C83C5, StringBuilder_$ctor } from "../fable_modules/fable-library-js.4.22.0/System.Text.js";
import { printf, toText, join } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toArray, empty, singleton, append, delay, toList } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { hash as hash_1, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { ResizeArray_map } from "./Helper/Collections.js";
import { safeHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class OntologyAnnotation {
    constructor(name, tsr, tan, comments) {
        this._name = ((name == null) ? undefined : ((name === "") ? undefined : name));
        this._termSourceREF = ((tsr == null) ? undefined : ((tsr === "") ? undefined : tsr));
        this._termAccessionNumber = ((tan == null) ? undefined : ((tan === "") ? undefined : ((tan === ":") ? undefined : tan)));
        this._comments = defaultArg(comments, []);
    }
    get Name() {
        const this$ = this;
        return unwrap(this$._name);
    }
    set Name(name) {
        const this$ = this;
        this$._name = name;
    }
    get TermSourceREF() {
        const this$ = this;
        return unwrap(this$._termSourceREF);
    }
    set TermSourceREF(tsr) {
        const this$ = this;
        this$._termSourceREF = tsr;
    }
    get TermAccessionNumber() {
        const this$ = this;
        return unwrap(this$._termAccessionNumber);
    }
    set TermAccessionNumber(tan) {
        const this$ = this;
        this$._termAccessionNumber = tan;
    }
    get Comments() {
        const this$ = this;
        return this$._comments;
    }
    set Comments(comments) {
        const this$ = this;
        this$._comments = comments;
    }
    static make(name, tsr, tan, comments) {
        return new OntologyAnnotation(unwrap(name), unwrap(tsr), unwrap(tan), comments);
    }
    static create(name, tsr, tan, comments) {
        const comments_1 = defaultArg(comments, []);
        return OntologyAnnotation.make(name, tsr, tan, comments_1);
    }
    get TANInfo() {
        let matchValue, tan, matchValue_1, matchValue_2, tsr;
        const this$ = this;
        return unwrap((matchValue = this$.TermAccessionNumber, (matchValue == null) ? undefined : ((tan = matchValue, (matchValue_1 = tryParseTermAnnotation(tan), (matchValue_1 == null) ? ((matchValue_2 = this$.TermSourceREF, (matchValue_2 == null) ? undefined : ((matchValue_2 === "") ? undefined : ((tsr = matchValue_2, {
            IDSpace: tsr,
            LocalID: tan,
        }))))) : matchValue_1)))));
    }
    get NameText() {
        const this$ = this;
        return defaultArg(this$.Name, "");
    }
    static createUriAnnotation(termSourceRef, localTAN) {
        return createOAUri(termSourceRef, localTAN);
    }
    static fromTermAnnotation(tan, name) {
        const matchValue = tryParseTermAnnotation(tan);
        if (matchValue == null) {
            return OntologyAnnotation.create(unwrap(name), undefined, tan);
        }
        else {
            const r = matchValue;
            const accession = (r.IDSpace + ":") + r.LocalID;
            return OntologyAnnotation.create(unwrap(name), r.IDSpace, accession);
        }
    }
    get TermAccessionShort() {
        const this$ = this;
        const matchValue = this$.TANInfo;
        if (matchValue != null) {
            const id = matchValue;
            return `${id.IDSpace}:${id.LocalID}`;
        }
        else {
            return "";
        }
    }
    get TermAccessionOntobeeUrl() {
        const this$ = this;
        const matchValue = this$.TANInfo;
        if (matchValue != null) {
            const id = matchValue;
            return OntologyAnnotation.createUriAnnotation(id.IDSpace, id.LocalID);
        }
        else {
            return "";
        }
    }
    get TermAccessionAndOntobeeUrlIfShort() {
        const this$ = this;
        const matchValue = this$.TermAccessionNumber;
        if (matchValue != null) {
            const tan = matchValue;
            return (ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationShortPattern, tan) != null) ? this$.TermAccessionOntobeeUrl : tan;
        }
        else {
            return "";
        }
    }
    static toStringObject(oa, asOntobeePurlUrlIfShort) {
        let url;
        const asOntobeePurlUrlIfShort_1 = defaultArg(asOntobeePurlUrlIfShort, false);
        const TermName = defaultArg(oa.Name, "");
        const TermSourceREF = defaultArg(oa.TermSourceREF, "");
        return {
            TermAccessionNumber: asOntobeePurlUrlIfShort_1 ? ((url = oa.TermAccessionAndOntobeeUrlIfShort, (url === "") ? defaultArg(oa.TermAccessionNumber, "") : url)) : defaultArg(oa.TermAccessionNumber, ""),
            TermName: TermName,
            TermSourceREF: TermSourceREF,
        };
    }
    toString() {
        const this$ = this;
        const sb = StringBuilder_$ctor();
        StringBuilder__Append_Z721C83C5(sb, "{");
        StringBuilder__Append_Z721C83C5(sb, join("; ", toList(delay(() => {
            let arg;
            return append((this$.Name != null) ? singleton((arg = value_4(this$.Name), toText(printf("Name = %A"))(arg))) : empty(), delay(() => {
                let arg_1;
                return append((this$.TermSourceREF != null) ? singleton((arg_1 = value_4(this$.TermSourceREF), toText(printf("TSR = %A"))(arg_1))) : empty(), delay(() => {
                    let arg_2;
                    return append((this$.TermAccessionNumber != null) ? singleton((arg_2 = value_4(this$.TermAccessionNumber), toText(printf("TAN = %A"))(arg_2))) : empty(), delay(() => {
                        let arg_3;
                        return (this$.Comments.length !== 0) ? singleton((arg_3 = this$.Comments, toText(printf("Comments = %A"))(arg_3))) : empty();
                    }));
                }));
            }));
        }))));
        StringBuilder__Append_Z721C83C5(sb, "}");
        return toString(sb);
    }
    isEmpty() {
        const this$ = this;
        return (((this$.Name == null) && (this$.TermSourceREF == null)) && (this$.TermAccessionNumber == null)) && (this$.Comments.length === 0);
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray(toArray(delay(() => append(singleton(boxHashOption(this$.Name)), delay(() => {
            let tsr_1, taninfo_1, tsr;
            const matchValue = this$.TermSourceREF;
            const matchValue_1 = this$.TANInfo;
            if (matchValue != null) {
                return (matchValue_1 == null) ? ((tsr_1 = matchValue, singleton(boxHashArray([tsr_1, defaultArg(this$.TermAccessionNumber, "")])))) : ((taninfo_1 = matchValue_1, (tsr = matchValue, singleton(boxHashArray([tsr, (taninfo_1.IDSpace + ":") + taninfo_1.LocalID])))));
            }
            else if (matchValue_1 == null) {
                const tan_1 = defaultArg(this$.TermAccessionNumber, "");
                return singleton(boxHashArray([defaultArg(this$.TermAccessionNumber, ""), tan_1]));
            }
            else {
                const taninfo = matchValue_1;
                return singleton(boxHashArray([taninfo.IDSpace, (taninfo.IDSpace + ":") + taninfo.LocalID]));
            }
        }))))) | 0;
    }
    Equals(obj) {
        const this$ = this;
        return hash_1(this$) === hash_1(obj);
    }
    Copy() {
        const this$ = this;
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        const name = this$.Name;
        const tsr = this$.TermSourceREF;
        const tan = this$.TermAccessionNumber;
        return OntologyAnnotation.make(name, tsr, tan, nextComments);
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        return "OA " + this$.NameText;
    }
    CompareTo(obj) {
        const this$ = this;
        if (obj instanceof OntologyAnnotation) {
            const oa = obj;
            const hash = safeHash(this$) | 0;
            const otherHash = safeHash(oa) | 0;
            return ((hash === otherHash) ? 0 : ((hash < otherHash) ? -1 : 1)) | 0;
        }
        else {
            return 1;
        }
    }
}

export function OntologyAnnotation_$reflection() {
    return class_type("ARCtrl.OntologyAnnotation", undefined, OntologyAnnotation);
}

export function OntologyAnnotation_$ctor_Z54349580(name, tsr, tan, comments) {
    return new OntologyAnnotation(name, tsr, tan, comments);
}

