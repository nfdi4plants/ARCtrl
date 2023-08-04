import { defaultArg, bind } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { match } from "../../fable_modules/fable-library.4.1.4/RegExp.js";
import { printf, toText } from "../../fable_modules/fable-library.4.1.4/String.js";
import { Option_fromValueWithDefault } from "./Conversions.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";

export const Comment_commentRegex = /(?<=Comment\[<).*(?=>\])/gu;

export const Comment_commentRegexNoAngleBrackets = /(?<=Comment\[).*(?=\])/gu;

export function Comment_$007CComment$007C_$007C(key) {
    return bind((k) => {
        const r = match(Comment_commentRegex, k);
        if (r != null) {
            return r[0];
        }
        else {
            const r_1 = match(Comment_commentRegexNoAngleBrackets, k);
            if (r_1 != null) {
                return r_1[0];
            }
            else {
                return void 0;
            }
        }
    }, key);
}

export function Comment_wrapCommentKey(k) {
    return toText(printf("Comment[%s]"))(k);
}

export function Comment_fromString(k, v) {
    const arg_1 = Option_fromValueWithDefault("", k);
    const arg_2 = Option_fromValueWithDefault("", v);
    return Comment$.make(void 0, arg_1, arg_2);
}

export function Comment_toString(c) {
    return [defaultArg(c.Name, ""), defaultArg(c.Value, "")];
}

export const Remark_remarkRegex = /(?<=#).*/gu;

export function Remark_$007CRemark$007C_$007C(key) {
    return bind((k) => {
        const r = match(Remark_remarkRegex, k);
        if (r != null) {
            return r[0];
        }
        else {
            return void 0;
        }
    }, key);
}

export function Remark_wrapRemark(r) {
    return toText(printf("#%s"))(r);
}

