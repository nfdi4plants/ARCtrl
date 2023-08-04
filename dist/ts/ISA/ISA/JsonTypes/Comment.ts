import { defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { IComparable, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { int32_type, record_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";

export class Comment$ extends Record implements IEquatable<Comment$>, IComparable<Comment$> {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly Value: Option<string>;
    constructor(ID: Option<string>, Name: Option<string>, Value: Option<string>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Value = Value;
    }
    static make(id: Option<string>, name: Option<string>, value: Option<string>): Comment$ {
        return new Comment$(id, name, value);
    }
    static create(Id?: string, Name?: string, Value?: string): Comment$ {
        return Comment$.make(Id, Name, Value);
    }
    static fromString(name: string, value: string): Comment$ {
        return Comment$.create(void 0, name, value);
    }
    static toString(comment: Comment$): [string, string] {
        return [defaultArg(comment.Name, ""), defaultArg(comment.Value, "")] as [string, string];
    }
    Copy(): Comment$ {
        const this$: Comment$ = this;
        return Comment$.make(this$.ID, this$.Name, this$.Value);
    }
}

export function Comment$_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Comment", [], Comment$, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Value", option_type(string_type)]]);
}

export class Remark extends Record implements IEquatable<Remark>, IComparable<Remark> {
    readonly Line: int32;
    readonly Value: string;
    constructor(Line: int32, Value: string) {
        super();
        this.Line = (Line | 0);
        this.Value = Value;
    }
    static make(line: int32, value: string): Remark {
        return new Remark(line, value);
    }
    static create(line: int32, value: string): Remark {
        return Remark.make(line, value);
    }
    static toTuple(remark: Remark): [int32, string] {
        return [remark.Line, remark.Value] as [int32, string];
    }
    Copy(): Remark {
        const this$: Remark = this;
        return Remark.make(this$.Line, this$.Value);
    }
}

export function Remark_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Remark", [], Remark, () => [["Line", int32_type], ["Value", string_type]]);
}

