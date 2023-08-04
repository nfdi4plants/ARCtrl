import { Record, toString } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { choose, empty, length } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { printf, toText } from "../../../fable_modules/fable-library.4.1.4/String.js";
import { MaterialType_$reflection, MaterialType__get_AsString } from "./MaterialType.js";
import { record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { MaterialAttributeValue_$reflection } from "./MaterialAttributeValue.js";

export class Material extends Record {
    constructor(ID, Name, MaterialType, Characteristics, DerivesFrom) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.MaterialType = MaterialType;
        this.Characteristics = Characteristics;
        this.DerivesFrom = DerivesFrom;
    }
    Print() {
        const this$ = this;
        return toString(this$);
    }
    PrintCompact() {
        const this$ = this;
        const chars = length(defaultArg(this$.Characteristics, empty())) | 0;
        const matchValue = this$.MaterialType;
        if (matchValue == null) {
            const arg_3 = Material__get_NameText(this$);
            return toText(printf("%s [%i characteristics]"))(arg_3)(chars);
        }
        else {
            const t = matchValue;
            const arg = Material__get_NameText(this$);
            const arg_1 = MaterialType__get_AsString(t);
            return toText(printf("%s [%s; %i characteristics]"))(arg)(arg_1)(chars);
        }
    }
}

export function Material_$reflection() {
    return record_type("ARCtrl.ISA.Material", [], Material, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["MaterialType", option_type(MaterialType_$reflection())], ["Characteristics", option_type(list_type(MaterialAttributeValue_$reflection()))], ["DerivesFrom", option_type(list_type(Material_$reflection()))]]);
}

export function Material_make(id, name, materialType, characteristics, derivesFrom) {
    return new Material(id, name, materialType, characteristics, derivesFrom);
}

export function Material_create_Z31BE6CDD(Id, Name, MaterialType, Characteristics, DerivesFrom) {
    return Material_make(Id, Name, MaterialType, Characteristics, DerivesFrom);
}

export function Material_get_empty() {
    return Material_create_Z31BE6CDD();
}

export function Material__get_NameText(this$) {
    return defaultArg(this$.Name, "");
}

export function Material_getUnits_Z42815C11(m) {
    return choose((c) => c.Unit, defaultArg(m.Characteristics, empty()));
}

export function Material_setCharacteristicValues(values, m) {
    return new Material(m.ID, m.Name, m.MaterialType, values, m.DerivesFrom);
}

