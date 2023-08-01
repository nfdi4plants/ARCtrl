import { Record, toString } from "../../../fable_modules/fable-library-ts/Types.js";
import { append, choose, FSharpList, empty, length } from "../../../fable_modules/fable-library-ts/List.js";
import { Option, defaultArg } from "../../../fable_modules/fable-library-ts/Option.js";
import { MaterialAttributeValue_$reflection, MaterialAttributeValue } from "./MaterialAttributeValue.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { FactorValue_$reflection, FactorValue } from "./FactorValue.js";
import { printf, toText } from "../../../fable_modules/fable-library-ts/String.js";
import { Source_$reflection, Source } from "./Source.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { IISAPrintable } from "../Printer.js";
import { record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { OntologyAnnotation } from "./OntologyAnnotation.js";

export class Sample extends Record implements IEquatable<Sample>, IISAPrintable {
    readonly ID: Option<string>;
    readonly Name: Option<string>;
    readonly Characteristics: Option<FSharpList<MaterialAttributeValue>>;
    readonly FactorValues: Option<FSharpList<FactorValue>>;
    readonly DerivesFrom: Option<FSharpList<Source>>;
    constructor(ID: Option<string>, Name: Option<string>, Characteristics: Option<FSharpList<MaterialAttributeValue>>, FactorValues: Option<FSharpList<FactorValue>>, DerivesFrom: Option<FSharpList<Source>>) {
        super();
        this.ID = ID;
        this.Name = Name;
        this.Characteristics = Characteristics;
        this.FactorValues = FactorValues;
        this.DerivesFrom = DerivesFrom;
    }
    Print(): string {
        const this$: Sample = this;
        return toString(this$);
    }
    PrintCompact(): string {
        const this$: Sample = this;
        const chars: int32 = length<MaterialAttributeValue>(defaultArg(this$.Characteristics, empty<MaterialAttributeValue>())) | 0;
        const facts: int32 = length<FactorValue>(defaultArg(this$.FactorValues, empty<FactorValue>())) | 0;
        const arg: string = Sample__get_NameAsString(this$);
        return toText(printf("%s [%i characteristics; %i factors]"))(arg)(chars)(facts);
    }
}

export function Sample_$reflection(): TypeInfo {
    return record_type("ISA.Sample", [], Sample, () => [["ID", option_type(string_type)], ["Name", option_type(string_type)], ["Characteristics", option_type(list_type(MaterialAttributeValue_$reflection()))], ["FactorValues", option_type(list_type(FactorValue_$reflection()))], ["DerivesFrom", option_type(list_type(Source_$reflection()))]]);
}

export function Sample_make(id: Option<string>, name: Option<string>, characteristics: Option<FSharpList<MaterialAttributeValue>>, factorValues: Option<FSharpList<FactorValue>>, derivesFrom: Option<FSharpList<Source>>): Sample {
    return new Sample(id, name, characteristics, factorValues, derivesFrom);
}

export function Sample_create_3A6378D6(Id?: string, Name?: string, Characteristics?: FSharpList<MaterialAttributeValue>, FactorValues?: FSharpList<FactorValue>, DerivesFrom?: FSharpList<Source>): Sample {
    return Sample_make(Id, Name, Characteristics, FactorValues, DerivesFrom);
}

export function Sample_get_empty(): Sample {
    return Sample_create_3A6378D6();
}

export function Sample__get_NameAsString(this$: Sample): string {
    return defaultArg(this$.Name, "");
}

export function Sample_getCharacteristicUnits_Z23050B6A(s: Sample): FSharpList<OntologyAnnotation> {
    return choose<MaterialAttributeValue, OntologyAnnotation>((c: MaterialAttributeValue): Option<OntologyAnnotation> => c.Unit, defaultArg(s.Characteristics, empty<MaterialAttributeValue>()));
}

export function Sample_getFactorUnits_Z23050B6A(s: Sample): FSharpList<OntologyAnnotation> {
    return choose<FactorValue, OntologyAnnotation>((c: FactorValue): Option<OntologyAnnotation> => c.Unit, defaultArg(s.FactorValues, empty<FactorValue>()));
}

export function Sample_getUnits_Z23050B6A(s: Sample): FSharpList<OntologyAnnotation> {
    return append<OntologyAnnotation>(Sample_getCharacteristicUnits_Z23050B6A(s), Sample_getFactorUnits_Z23050B6A(s));
}

export function Sample_setCharacteristicValues(values: FSharpList<MaterialAttributeValue>, s: Sample): Sample {
    return new Sample(s.ID, s.Name, values, s.FactorValues, s.DerivesFrom);
}

export function Sample_setFactorValues(values: FSharpList<FactorValue>, s: Sample): Sample {
    return new Sample(s.ID, s.Name, s.Characteristics, values, s.DerivesFrom);
}

