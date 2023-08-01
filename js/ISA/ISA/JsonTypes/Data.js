import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { DataFile_$reflection, DataFile__get_AsString } from "./DataFile.js";
import { record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Comment$_$reflection } from "./Comment.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";

export class Data extends Record {
    constructor(ID, Name, DataType, Comments) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.DataType = DataType;
        this.Comments = Comments;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        const matchValue = this$.DataType;
        if (matchValue == null) {
            const arg_2 = Data__get_NameAsString(this$);
            return toText(printf("%s"))(arg_2);
        }
        else {
            const t = matchValue;
            const arg = Data__get_NameAsString(this$);
            const arg_1 = DataFile__get_AsString(t);
            return toText(printf("%s [%s]"))(arg)(arg_1);
        }
    }
}

export function Data_$reflection() {
    return record_type("ISA.Data", [], Data, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["DataType", option_type(DataFile_$reflection())], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Data_make(id, name, dataType, comments) {
    return new Data(id, name, dataType, comments);
}

export function Data_create_Z748D099(Id, Name, DataType, Comments) {
    return Data_make(Id, Name, DataType, Comments);
}

export function Data_get_empty() {
    return Data_create_Z748D099();
}

export function Data__get_NameAsString(this$) {
    return defaultArg(this$.Name, "");
}

