import { defaultArg, Option, bind } from "../../fable_modules/fable-library-ts/Option.js";
import { match } from "../../fable_modules/fable-library-ts/RegExp.js";
import { printf, toText } from "../../fable_modules/fable-library-ts/String.js";
import { Option_fromValueWithDefault } from "./Conversions.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";

export const Comment_commentRegex = /(?<=Comment\[<).*(?=>\])/gu;

export const Comment_commentRegexNoAngleBrackets = /(?<=Comment\[).*(?=\])/gu;

export function Comment_$007CComment$007C_$007C(key: Option<string>): Option<string> {
    return bind<string, string>((k: string): Option<string> => {
        const r: any = match(Comment_commentRegex, k);
        if (r != null) {
            return r[0];
        }
        else {
            const r_1: any = match(Comment_commentRegexNoAngleBrackets, k);
            if (r_1 != null) {
                return r_1[0];
            }
            else {
                return void 0;
            }
        }
    }, key);
}

export function Comment_wrapCommentKey(k: string): string {
    return toText(printf("Comment[%s]"))(k);
}

export function Comment_fromString(k: string, v: string): Comment$ {
    const arg_1: Option<string> = Option_fromValueWithDefault<string>("", k);
    const arg_2: Option<string> = Option_fromValueWithDefault<string>("", v);
    return Comment$.make(void 0, arg_1, arg_2);
}

export function Comment_toString(c: Comment$): [string, string] {
    return [defaultArg(c.Name, ""), defaultArg(c.Value, "")] as [string, string];
}

export const Remark_remarkRegex = /(?<=#).*/gu;

export function Remark_$007CRemark$007C_$007C(key: Option<string>): Option<string> {
    return bind<string, string>((k: string): Option<string> => {
        const r: any = match(Remark_remarkRegex, k);
        if (r != null) {
            return r[0];
        }
        else {
            return void 0;
        }
    }, key);
}

export function Remark_wrapRemark(r: string): string {
    return toText(printf("#%s"))(r);
}

