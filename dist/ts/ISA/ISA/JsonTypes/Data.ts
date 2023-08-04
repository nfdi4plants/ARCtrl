import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { defaultArg, value as value_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { DataFile_$reflection, DataFile__get_AsString, DataFile_$union } from "./DataFile.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { IComparable, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class Data extends Record implements IEquatable<Data>, IComparable<Data>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly DataType: Option<DataFile_$union>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, Name: Option<string>, DataType: Option<DataFile_$union>, Comments: Option<FSharpList<Comment$>>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.DataType = DataType;
        this.Comments = Comments;
    }
    Print(): string {
        const this$: Data = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Data = this;
        const matchValue: Option<DataFile_$union> = this$.DataType;
        if (matchValue == null) {
            const arg_2: string = Data__get_NameAsString(this$);
            return toText(printf("%s"))(arg_2);
        }
        else {
            const t: DataFile_$union = value_1(matchValue);
            const arg: string = Data__get_NameAsString(this$);
            const arg_1: string = DataFile__get_AsString(t);
            return toText(printf("%s [%s]"))(arg)(arg_1);
        }
    }
}

export function Data_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Data", [], Data, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["DataType", option_type(DataFile_$reflection())], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Data_make(id: Option<string>, name: Option<string>, dataType: Option<DataFile_$union>, comments: Option<FSharpList<Comment$>>): Data {
    return new Data(id, name, dataType, comments);
}

export function Data_create_Z326CF519(Id?: string, Name?: string, DataType?: DataFile_$union, Comments?: FSharpList<Comment$>): Data {
    return Data_make(Id, Name, DataType, Comments);
}

export function Data_get_empty(): Data {
    return Data_create_Z326CF519();
}

export function Data__get_NameAsString(this$: Data): string {
    return defaultArg(this$.Name, "");
}

