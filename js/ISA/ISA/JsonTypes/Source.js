import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { choose, empty, length } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { MaterialAttributeValue_$reflection } from "./MaterialAttributeValue.js";

export class Source extends Record {
    constructor(ID, Name, Characteristics) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Characteristics = Characteristics;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        const l = length(defaultArg(this$.Characteristics, empty())) | 0;
        const arg = Source__get_NameAsString(this$);
        return toText(printf("%s [%i characteristics]"))(arg)(l);
    }
}

export function Source_$reflection() {
    return record_type("ISA.Source", [], Source, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Characteristics", option_type(list_type(MaterialAttributeValue_$reflection()))]]);
}

export function Source_make(id, name, characteristics) {
    return new Source(id, name, characteristics);
}

export function Source_create_Z32235993(Id, Name, Characteristics) {
    return Source_make(Id, Name, Characteristics);
}

export function Source_get_empty() {
    return Source_create_Z32235993();
}

export function Source__get_NameAsString(this$) {
    return defaultArg(this$.Name, "");
}

export function Source_getUnits_Z220A6393(m) {
    return choose((c) => c.Unit, defaultArg(m.Characteristics, empty()));
}

export function Source_setCharacteristicValues(values, m) {
    return new Source(m.ID, m.Name, values);
}

