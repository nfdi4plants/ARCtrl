import { ActivePatterns_$007CRegex$007C_$007C } from "../../Core/Helper/Regex.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { Option_fromValueWithDefault } from "./Conversions.js";
import { Comment$ } from "../../Core/Comment.js";
import { bind, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";

export const Comment_commentValueKey = "commentValue";

export const Comment_commentPattern = `Comment\\s*\\[<(?<${Comment_commentValueKey}>.+)>\\]`;

export const Comment_commentPatternNoAngleBrackets = `Comment\\s*\\[(?<${Comment_commentValueKey}>.+)\\]`;

export function Comment_$007CComment$007C_$007C(key) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Comment_commentPattern, key);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (r.groups && r.groups[Comment_commentValueKey]) || "";
    }
    else {
        const activePatternResult_1 = ActivePatterns_$007CRegex$007C_$007C(Comment_commentPatternNoAngleBrackets, key);
        if (activePatternResult_1 != null) {
            const r_1 = activePatternResult_1;
            const v = (r_1.groups && r_1.groups[Comment_commentValueKey]) || "";
            if (v === "<>") {
                return undefined;
            }
            else {
                return v;
            }
        }
        else {
            return undefined;
        }
    }
}

export function Comment_wrapCommentKey(k) {
    return toText(printf("Comment[%s]"))(k);
}

export function Comment_fromString(k, v) {
    const name = Option_fromValueWithDefault("", k);
    const value = Option_fromValueWithDefault("", v);
    return Comment$.make(name, value);
}

export function Comment_toString(c) {
    return [defaultArg(c.Name, ""), defaultArg(c.Value, "")];
}

export const Remark_remarkValueKey = "remarkValue";

export const Remark_remarkPattern = `#(?<${Remark_remarkValueKey}>.+)`;

export function Remark_$007CRemark$007C_$007C(key) {
    return bind((k) => {
        const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Remark_remarkPattern, k);
        if (activePatternResult != null) {
            const r = activePatternResult;
            return (r.groups && r.groups[Remark_remarkValueKey]) || "";
        }
        else {
            return undefined;
        }
    }, key);
}

export function Remark_wrapRemark(r) {
    return toText(printf("#%s"))(r);
}

