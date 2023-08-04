import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { choose, FSharpList, empty, length } from "../../../fable_modules/fable-library-ts/List.js";
import { value as value_1, Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { MaterialAttributeValue_$reflection, MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { MaterialType_$reflection, MaterialType__get_AsString, MaterialType_$union } from "./MaterialType.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";

export class Material extends Record implements IEquatable<Material>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly MaterialType: Option<MaterialType_$union>;
    readonly Characteristics: Option<FSharpList<MaterialAttributeValue>>;
    readonly DerivesFrom: Option<FSharpList<Material>>;
    constructor(ID: Option<string>, Name: Option<string>, MaterialType: Option<MaterialType_$union>, Characteristics: Option<FSharpList<MaterialAttributeValue>>, DerivesFrom: Option<FSharpList<Material>>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.MaterialType = MaterialType;
        this.Characteristics = Characteristics;
        this.DerivesFrom = DerivesFrom;
    }
    Print(): string {
        const this$: Material = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Material = this;
        const chars: int32 = length<MaterialAttributeValue>(defaultArg(this$.Characteristics, empty<MaterialAttributeValue>())) | 0;
        const matchValue: Option<MaterialType_$union> = this$.MaterialType;
        if (matchValue == null) {
            const arg_3: string = Material__get_NameText(this$);
            return toText(printf("%s [%i characteristics]"))(arg_3)(chars);
        }
        else {
            const t: MaterialType_$union = value_1(matchValue);
            const arg: string = Material__get_NameText(this$);
            const arg_1: string = MaterialType__get_AsString(t);
            return toText(printf("%s [%s; %i characteristics]"))(arg)(arg_1)(chars);
        }
    }
}

export function Material_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Material", [], Material, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["MaterialType", option_type(MaterialType_$reflection())], ["Characteristics", option_type(list_type(MaterialAttributeValue_$reflection()))], ["DerivesFrom", option_type(list_type(Material_$reflection()))]]);
}

export function Material_make(id: Option<string>, name: Option<string>, materialType: Option<MaterialType_$union>, characteristics: Option<FSharpList<MaterialAttributeValue>>, derivesFrom: Option<FSharpList<Material>>): Material {
    return new Material(id, name, materialType, characteristics, derivesFrom);
}

export function Material_create_Z31BE6CDD(Id?: string, Name?: string, MaterialType?: MaterialType_$union, Characteristics?: FSharpList<MaterialAttributeValue>, DerivesFrom?: FSharpList<Material>): Material {
    return Material_make(Id, Name, MaterialType, Characteristics, DerivesFrom);
}

export function Material_get_empty(): Material {
    return Material_create_Z31BE6CDD();
}

export function Material__get_NameText(this$: Material): string {
    return defaultArg(this$.Name, "");
}

export function Material_getUnits_Z42815C11(m: Material): FSharpList<OntologyAnnotation> {
    return choose<MaterialAttributeValue, OntologyAnnotation>((c: MaterialAttributeValue): Option<OntologyAnnotation> => c.Unit, defaultArg(m.Characteristics, empty<MaterialAttributeValue>()));
}

export function Material_setCharacteristicValues(values: FSharpList<MaterialAttributeValue>, m: Material): Material {
    return new Material(m.ID, m.Name, m.MaterialType, values, m.DerivesFrom);
}

