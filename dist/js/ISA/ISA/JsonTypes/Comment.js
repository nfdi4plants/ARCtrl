import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { int32_type, record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class Comment$ extends Record {
    constructor(ID, Name, Value) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Value = Value;
    }
    static make(id, name, value) {
        return new Comment$(id, name, value);
    }
    static create(Id, Name, Value) {
        return Comment$.make(Id, Name, Value);
    }
    static fromString(name, value) {
        return Comment$.create(void 0, name, value);
    }
    static toString(comment) {
        return [defaultArg(comment.Name, ""), defaultArg(comment.Value, "")];
    }
    Copy() {
        const this$ = this;
        return Comment$.make(this$.ID, this$.Name, this$.Value);
    }
}

export function Comment$_$reflection() {
    return record_type("ARCtrl.ISA.Comment", [], Comment$, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Value", option_type(string_type)]]);
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
    return record_type("ARCtrl.ISA.Remark", [], Remark, () => [["Line", int32_type], ["Value", string_type]]);
}

