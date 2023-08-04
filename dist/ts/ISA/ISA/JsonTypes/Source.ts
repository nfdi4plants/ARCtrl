import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { choose, FSharpList, empty, length } from "../../../fable_modules/fable-library-ts/List.js";
import { Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { MaterialAttributeValue_$reflection, MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";

export class Source extends Record implements IEquatable<Source>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly Characteristics: Option<FSharpList<MaterialAttributeValue>>;
    constructor(ID: Option<string>, Name: Option<string>, Characteristics: Option<FSharpList<MaterialAttributeValue>>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Characteristics = Characteristics;
    }
    Print(): string {
        const this$: Source = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Source = this;
        const l: int32 = length<MaterialAttributeValue>(defaultArg(this$.Characteristics, empty<MaterialAttributeValue>())) | 0;
        const arg: string = Source__get_NameAsString(this$);
        return toText(printf("%s [%i characteristics]"))(arg)(l);
    }
}

export function Source_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Source", [], Source, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Characteristics", option_type(list_type(MaterialAttributeValue_$reflection()))]]);
}

export function Source_make(id: Option<string>, name: Option<string>, characteristics: Option<FSharpList<MaterialAttributeValue>>): Source {
    return new Source(id, name, characteristics);
}

export function Source_create_7A281ED9(Id?: string, Name?: string, Characteristics?: FSharpList<MaterialAttributeValue>): Source {
    return Source_make(Id, Name, Characteristics);
}

export function Source_get_empty(): Source {
    return Source_create_7A281ED9();
}

export function Source__get_NameAsString(this$: Source): string {
    return defaultArg(this$.Name, "");
}

export function Source_getUnits_Z28BE5327(m: Source): FSharpList<OntologyAnnotation> {
    return choose<MaterialAttributeValue, OntologyAnnotation>((c: MaterialAttributeValue): Option<OntologyAnnotation> => c.Unit, defaultArg(m.Characteristics, empty<MaterialAttributeValue>()));
}

export function Source_setCharacteristicValues(values: FSharpList<MaterialAttributeValue>, m: Source): Source {
    return new Source(m.ID, m.Name, values);
}

