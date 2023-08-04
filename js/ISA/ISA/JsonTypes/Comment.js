import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { int32_type, record_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";

export class Comment$ extends Record {
    constructor(ID, Name, Value) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Value = Value;
    }
}

export function Comment$_$reflection() {
    return record_type("ARCtrl.ISA.Comment", [], Comment$, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Value", option_type(string_type)]]);
}

export function Comment_make(id, name, value) {
    return new Comment$(id, name, value);
}

export function Comment_create_250E0578(Id, Name, Value) {
    return Comment_make(Id, Name, Value);
}

export function Comment_fromString(name, value) {
    return Comment_create_250E0578(void 0, name, value);
}

export function Comment_toString_ZFA4E8A9(comment) {
    return [defaultArg(comment.Name, ""), defaultArg(comment.Value, "")];
}

export class Remark extends Record {
    constructor(Line, Value) {
        super();
        this.Line = (Line | 0);
        this.Value = Value;
    }
}

export function Remark_$reflection() {
    return record_type("ARCtrl.ISA.Remark", [], Remark, () => [["Line", int32_type], ["Value", string_type]]);
}

export function Remark_make(line, value) {
    return new Remark(line, value);
}

export function Remark_create_Z176EF219(line, value) {
    return Remark_make(line, value);
}

export function Remark_toTuple_Z26CAFFFA(remark) {
    return [remark.Line, remark.Value];
}

