import { map as map_1, defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { equals } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { StringBuilder__Append_Z721C83C5, StringBuilder_$ctor } from "../fable_modules/fable-library-js.4.22.0/System.Text.js";
import { replace, printf, toText, join } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { ofArray, choose, map } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { Record, toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { create, match } from "../fable_modules/fable-library-js.4.22.0/RegExp.js";
import { Pattern_handleGroupPatterns } from "./Helper/Regex.js";
import { record_type, string_type, int32_type, class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export class Comment$ {
    constructor(name, value) {
        this._name = name;
        this._value = value;
    }
    get Name() {
        const this$ = this;
        return unwrap(this$._name);
    }
    set Name(name) {
        const this$ = this;
        this$._name = name;
    }
    get Value() {
        const this$ = this;
        return unwrap(this$._value);
    }
    set Value(value) {
        const this$ = this;
        this$._value = value;
    }
    static make(name, value) {
        return new Comment$(unwrap(name), unwrap(value));
    }
    static create(name, value) {
        return Comment$.make(name, value);
    }
    static toString(comment) {
        return [defaultArg(comment.Name, ""), defaultArg(comment.Value, "")];
    }
    Copy() {
        const this$ = this;
        const name = this$.Name;
        const value = this$.Value;
        return Comment$.make(name, value);
    }
    Equals(obj) {
        let c;
        const this$ = this;
        return (obj instanceof Comment$) && ((c = obj, equals(c.Name, this$.Name) && equals(c.Value, this$.Value)));
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.Name), boxHashOption(this$.Value)]) | 0;
    }
    toString() {
        const this$ = this;
        const sb = StringBuilder_$ctor();
        StringBuilder__Append_Z721C83C5(sb, "Comment {");
        StringBuilder__Append_Z721C83C5(sb, join(", ", map((tupledArg_1) => toText(printf("%s = \"%s\""))(tupledArg_1[0])(tupledArg_1[1]), choose((tupledArg) => map_1((o) => [tupledArg[0], o], tupledArg[1]), ofArray([["Name", this$.Name], ["Value", this$.Value]])))));
        StringBuilder__Append_Z721C83C5(sb, "}");
        return toString(sb);
    }
    static fromString(s) {
        const nameResult = match(create(Pattern_handleGroupPatterns("Name = \"[^\"]*\"")), s);
        const valueResult = match(create(Pattern_handleGroupPatterns("Value = \"[^\"]*\"")), s);
        return new Comment$(unwrap((nameResult != null) ? replace(replace(nameResult[0], "Name = ", ""), "\"", "") : undefined), unwrap((valueResult != null) ? replace(replace(valueResult[0], "Value = ", ""), "\"", "") : undefined));
    }
}

export function Comment$_$reflection() {
    return class_type("ARCtrl.Comment", undefined, Comment$);
}

export function Comment_$ctor_40457300(name, value) {
    return new Comment$(name, value);
}

export class Remark extends Record {
    constructor(Line, Value) {
        super();
        this.Line = (Line | 0);
        this.Value = Value;
    }
    static make(line, value) {
        return new Remark(line, value);
    }
    static create(line, value) {
        return Remark.make(line, value);
    }
    static toTuple(remark) {
        return [remark.Line, remark.Value];
    }
    Copy() {
        const this$ = this;
        return Remark.make(this$.Line, this$.Value);
    }
}

export function Remark_$reflection() {
    return record_type("ARCtrl.Remark", [], Remark, () => [["Line", int32_type], ["Value", string_type]]);
}

