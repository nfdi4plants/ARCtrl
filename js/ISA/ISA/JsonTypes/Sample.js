import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { append, choose, empty, length } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { MaterialAttributeValue_$reflection } from "./MaterialAttributeValue.js";
import { FactorValue_$reflection } from "./FactorValue.js";
import { Source_$reflection } from "./Source.js";

export class Sample extends Record {
    constructor(ID, Name, Characteristics, FactorValues, DerivesFrom) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Characteristics = Characteristics;
        this.FactorValues = FactorValues;
        this.DerivesFrom = DerivesFrom;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        const chars = length(defaultArg(this$.Characteristics, empty())) | 0;
        const facts = length(defaultArg(this$.FactorValues, empty())) | 0;
        const arg = Sample__get_NameAsString(this$);
        return toText(printf("%s [%i characteristics; %i factors]"))(arg)(chars)(facts);
    }
}

export function Sample_$reflection() {
    return record_type("ISA.Sample", [], Sample, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Characteristics", option_type(list_type(MaterialAttributeValue_$reflection()))], ["FactorValues", option_type(list_type(FactorValue_$reflection()))], ["DerivesFrom", option_type(list_type(Source_$reflection()))]]);
}

export function Sample_make(id, name, characteristics, factorValues, derivesFrom) {
    return new Sample(id, name, characteristics, factorValues, derivesFrom);
}

export function Sample_create_3A6378D6(Id, Name, Characteristics, FactorValues, DerivesFrom) {
    return Sample_make(Id, Name, Characteristics, FactorValues, DerivesFrom);
}

export function Sample_get_empty() {
    return Sample_create_3A6378D6();
}

export function Sample__get_NameAsString(this$) {
    return defaultArg(this$.Name, "");
}

export function Sample_getCharacteristicUnits_Z23050B6A(s) {
    return choose((c) => c.Unit, defaultArg(s.Characteristics, empty()));
}

export function Sample_getFactorUnits_Z23050B6A(s) {
    return choose((c) => c.Unit, defaultArg(s.FactorValues, empty()));
}

export function Sample_getUnits_Z23050B6A(s) {
    return append(Sample_getCharacteristicUnits_Z23050B6A(s), Sample_getFactorUnits_Z23050B6A(s));
}

export function Sample_setCharacteristicValues(values, s) {
    return new Sample(s.ID, s.Name, values, s.FactorValues, s.DerivesFrom);
}

export function Sample_setFactorValues(values, s) {
    return new Sample(s.ID, s.Name, s.Characteristics, values, s.DerivesFrom);
}

