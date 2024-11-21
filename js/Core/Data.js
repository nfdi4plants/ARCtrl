import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { item } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_map } from "./Helper/Collections.js";
import { hash, boxHashSeq, boxHashOption, boxHashArray } from "./Helper/HashCodes.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { DataFile__get_AsString } from "./DataFile.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export function DataAux_nameFromPathAndSelector(path, selector) {
    return toText(printf("%s#%s"))(path)(selector);
}

export function DataAux_pathAndSelectorFromName(name) {
    const parts = name.split("#");
    if (parts.length === 2) {
        return [item(0, parts), item(1, parts)];
    }
    else {
        return [name, undefined];
    }
}

export class Data {
    constructor(id, name, dataType, format, selectorFormat, comments) {
        this._id = id;
        let patternInput_1;
        if (name == null) {
            patternInput_1 = [undefined, undefined];
        }
        else {
            const patternInput = DataAux_pathAndSelectorFromName(name);
            patternInput_1 = [patternInput[0], patternInput[1]];
        }
        this._selector = patternInput_1[1];
        this._filePath = patternInput_1[0];
        this._dataType = dataType;
        this._format = format;
        this._selectorFormat = selectorFormat;
        this._comments = defaultArg(comments, []);
    }
    get ID() {
        const this$ = this;
        return unwrap(this$._id);
    }
    set ID(id) {
        const this$ = this;
        this$._id = id;
    }
    get Name() {
        let matchValue, matchValue_1, p_1, p, s;
        const this$ = this;
        return unwrap((matchValue = this$._filePath, (matchValue_1 = this$._selector, (matchValue == null) ? undefined : ((matchValue_1 == null) ? ((p_1 = matchValue, p_1)) : ((p = matchValue, (s = matchValue_1, DataAux_nameFromPathAndSelector(p, s))))))));
    }
    set Name(name) {
        const this$ = this;
        if (name == null) {
            this$._filePath = undefined;
            this$._selector = undefined;
        }
        else {
            const patternInput = DataAux_pathAndSelectorFromName(name);
            this$._filePath = patternInput[0];
            this$._selector = patternInput[1];
        }
    }
    get FilePath() {
        const this$ = this;
        return unwrap(this$._filePath);
    }
    set FilePath(filePath) {
        const this$ = this;
        this$._filePath = filePath;
    }
    get Selector() {
        const this$ = this;
        return unwrap(this$._selector);
    }
    set Selector(selector) {
        const this$ = this;
        this$._selector = selector;
    }
    get DataType() {
        const this$ = this;
        return unwrap(this$._dataType);
    }
    set DataType(dataType) {
        const this$ = this;
        this$._dataType = dataType;
    }
    get Format() {
        const this$ = this;
        return unwrap(this$._format);
    }
    set Format(format) {
        const this$ = this;
        this$._format = format;
    }
    get SelectorFormat() {
        const this$ = this;
        return unwrap(this$._selectorFormat);
    }
    set SelectorFormat(selectorFormat) {
        const this$ = this;
        this$._selectorFormat = selectorFormat;
    }
    get Comments() {
        const this$ = this;
        return this$._comments;
    }
    set Comments(comments) {
        const this$ = this;
        this$._comments = comments;
    }
    static make(id, name, dataType, format, selectorFormat, comments) {
        return new Data(unwrap(id), unwrap(name), unwrap(dataType), unwrap(format), unwrap(selectorFormat), unwrap(comments));
    }
    static create(Id, Name, DataType, Format, SelectorFormat, Comments) {
        return Data.make(Id, Name, DataType, Format, SelectorFormat, Comments);
    }
    static get empty() {
        return Data.create();
    }
    get NameText() {
        const this$ = this;
        return defaultArg(this$.Name, "");
    }
    Copy() {
        const this$ = this;
        const nextComments = ResizeArray_map((c) => c.Copy(), this$.Comments);
        return new Data(unwrap(this$.ID), unwrap(this$.Name), unwrap(this$.DataType), unwrap(this$.Format), unwrap(this$.SelectorFormat), nextComments);
    }
    GetHashCode() {
        const this$ = this;
        return boxHashArray([boxHashOption(this$.ID), boxHashOption(this$.Name), boxHashOption(this$.DataType), boxHashOption(this$.Format), boxHashOption(this$.SelectorFormat), boxHashSeq(this$.Comments)]) | 0;
    }
    Equals(obj) {
        const this$ = this;
        return hash(this$) === hash(obj);
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        const matchValue = this$.DataType;
        if (matchValue == null) {
            const arg_2 = this$.NameText;
            return toText(printf("%s"))(arg_2);
        }
        else {
            const t = matchValue;
            const arg = this$.NameText;
            const arg_1 = DataFile__get_AsString(t);
            return toText(printf("%s [%s]"))(arg)(arg_1);
        }
    }
}

export function Data_$reflection() {
    return class_type("ARCtrl.Data", undefined, Data);
}

export function Data_$ctor_5909441C(id, name, dataType, format, selectorFormat, comments) {
    return new Data(id, name, dataType, format, selectorFormat, comments);
}

