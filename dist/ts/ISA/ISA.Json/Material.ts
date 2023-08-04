import { MaterialType_LabeledExtractName, MaterialType_ExtractName, MaterialType_$union, MaterialType } from "../ISA/JsonTypes/MaterialType.js";
import { toString, object as object_10, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions } from "./ConverterOptions.js";
import { list as list_1, IOptionalGetter, IGetters, object as object_11, string } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_Ok, FSharpResult$2_Error, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_BadPrimitive, ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { value as value_11, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { MaterialAttribute } from "../ISA/JsonTypes/MaterialAttribute.js";
import { FSharpList, ofArray, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { hasUnknownFields, fromString, uri } from "./Decode.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { MaterialAttributeValue } from "../ISA/JsonTypes/MaterialAttributeValue.js";
import { Value_decoder, Value_encoder } from "./Factor.js";
import { Value_$union } from "../ISA/JsonTypes/Value.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { Material } from "../ISA/JsonTypes/Material.js";

export function MaterialType_encoder(options: ConverterOptions, value: any): any {
    if (value instanceof MaterialType) {
        if (value.tag === /* LabeledExtractName */ 1) {
            return "Labeled Extract Name";
        }
        else {
            return "Extract Name";
        }
    }
    else {
        return nil;
    }
}

export function MaterialType_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<MaterialType_$union, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<string, [string, ErrorReason_$union]> = string(s, json);
    if (matchValue.tag === /* Error */ 1) {
        return FSharpResult$2_Error<MaterialType_$union, [string, ErrorReason_$union]>(matchValue.fields[0]);
    }
    else {
        switch (matchValue.fields[0]) {
            case "Extract Name":
                return FSharpResult$2_Ok<MaterialType_$union, [string, ErrorReason_$union]>(MaterialType_ExtractName());
            case "Labeled Extract Name":
                return FSharpResult$2_Ok<MaterialType_$union, [string, ErrorReason_$union]>(MaterialType_LabeledExtractName());
            default: {
                const s_1: string = matchValue.fields[0];
                return FSharpResult$2_Error<MaterialType_$union, [string, ErrorReason_$union]>([`Could not parse ${s_1}No other value than "Extract Name" or "Labeled Extract Name" allowed for materialtype`, ErrorReason_BadPrimitive(s_1, nil)] as [string, ErrorReason_$union]);
            }
        }
    }
}

export function MaterialAttribute_genID(m: MaterialAttribute): string {
    const matchValue: Option<string> = m.ID;
    if (matchValue == null) {
        return "#EmptyMaterialAttribute";
    }
    else {
        return URIModule_toString(value_11(matchValue));
    }
}

export function MaterialAttribute_encoder(options: ConverterOptions, oa: any): any {
    return object_10(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = MaterialAttribute_genID(oa as MaterialAttribute), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "MaterialAttribute", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("characteristicType", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["CharacteristicType"]))));
        }));
    }))));
}

export function MaterialAttribute_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttribute, [string, ErrorReason_$union]>)) {
    return (path: string): ((arg0: any) => FSharpResult$2_$union<MaterialAttribute, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<MaterialAttribute, [string, ErrorReason_$union]> => object_11<MaterialAttribute>((get$: IGetters): MaterialAttribute => {
        let objectArg: IOptionalGetter, arg_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_1: IOptionalGetter;
        return new MaterialAttribute((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (arg_3 = OntologyAnnotation_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field<OntologyAnnotation>("characteristicType", uncurry2(arg_3)))));
    }, path, v));
}

export function MaterialAttribute_fromString(s: string): MaterialAttribute {
    return fromString<MaterialAttribute>(uncurry2(MaterialAttribute_decoder(ConverterOptions_$ctor())), s);
}

export function MaterialAttribute_toString(m: MaterialAttribute): string {
    return toString(2, MaterialAttribute_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function MaterialAttribute_toStringLD(m: MaterialAttribute): string {
    let returnVal: ConverterOptions;
    return toString(2, MaterialAttribute_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

export function MaterialAttributeValue_genID(m: MaterialAttributeValue): string {
    const matchValue: Option<string> = m.ID;
    if (matchValue == null) {
        return "#EmptyMaterialAttributeValue";
    }
    else {
        return URIModule_toString(value_11(matchValue));
    }
}

export function MaterialAttributeValue_encoder(options: ConverterOptions, oa: any): any {
    return object_10(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = MaterialAttributeValue_genID(oa as MaterialAttributeValue), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "MaterialAttributeValue", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("category", (oa_1: any): any => MaterialAttribute_encoder(options, oa_1), oa["Category"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("value", (value_7: any): any => Value_encoder(options, value_7), oa["Value"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("unit", (oa_2: any): any => OntologyAnnotation_encoder(options, oa_2), oa["Unit"]))))))));
        }));
    }))));
}

export function MaterialAttributeValue_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttributeValue, [string, ErrorReason_$union]>)) {
    return (path: string): ((arg0: any) => FSharpResult$2_$union<MaterialAttributeValue, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<MaterialAttributeValue, [string, ErrorReason_$union]> => object_11<MaterialAttributeValue>((get$: IGetters): MaterialAttributeValue => {
        let objectArg: IOptionalGetter, arg_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttribute, [string, ErrorReason_$union]>)), objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter;
        return new MaterialAttributeValue((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (arg_3 = MaterialAttribute_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field<MaterialAttribute>("category", uncurry2(arg_3)))), (objectArg_2 = get$.Optional, objectArg_2.Field<Value_$union>("value", (s_1: string, json_1: any): FSharpResult$2_$union<Value_$union, [string, ErrorReason_$union]> => Value_decoder(options, s_1, json_1))), (arg_7 = OntologyAnnotation_decoder(options), (objectArg_3 = get$.Optional, objectArg_3.Field<OntologyAnnotation>("unit", uncurry2(arg_7)))));
    }, path, v));
}

export function MaterialAttributeValue_fromString(s: string): MaterialAttributeValue {
    return fromString<MaterialAttributeValue>(uncurry2(MaterialAttributeValue_decoder(ConverterOptions_$ctor())), s);
}

export function MaterialAttributeValue_toString(m: MaterialAttributeValue): string {
    return toString(2, MaterialAttributeValue_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function MaterialAttributeValue_toStringLD(m: MaterialAttributeValue): string {
    let returnVal: ConverterOptions;
    return toString(2, MaterialAttributeValue_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

export function Material_genID(m: Material): string {
    const matchValue: Option<string> = m.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = m.Name;
        if (matchValue_1 == null) {
            return "#EmptyMaterial";
        }
        else {
            return "#Material_" + replace(value_11(matchValue_1), " ", "_");
        }
    }
    else {
        return value_11(matchValue);
    }
}

export function Material_encoder(options: ConverterOptions, oa: any): any {
    return object_10(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Material_genID(oa as Material), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Material", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("type", (value_10: any): any => MaterialType_encoder(options, value_10), oa["MaterialType"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("characteristics", (oa_1: any): any => MaterialAttributeValue_encoder(options, oa_1), oa["Characteristics"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("derivesFrom", (oa_2: any): any => Material_encoder(options, oa_2), oa["DerivesFrom"]))))))))));
        }));
    }))));
}

export function Material_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<Material, [string, ErrorReason_$union]> {
    if (hasUnknownFields(ofArray(["@id", "@type", "name", "type", "characteristics", "derivesFrom"]), json)) {
        return FSharpResult$2_Error<Material, [string, ErrorReason_$union]>(["Unknown fields in material", ErrorReason_BadPrimitive(s, nil)] as [string, ErrorReason_$union]);
    }
    else {
        return object_11<Material>((get$: IGetters): Material => {
            let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttributeValue, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter;
            return new Material((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field<MaterialType_$union>("type", (s_2: string, json_2: any): FSharpResult$2_$union<MaterialType_$union, [string, ErrorReason_$union]> => MaterialType_decoder(options, s_2, json_2))), (arg_7 = ((decoder = MaterialAttributeValue_decoder(options), (path_1: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]>) => ((value_1: any): FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]> => list_1<MaterialAttributeValue>(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field<FSharpList<MaterialAttributeValue>>("characteristics", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field<FSharpList<Material>>("derivesFrom", (path_2: string, value_2: any): FSharpResult$2_$union<FSharpList<Material>, [string, ErrorReason_$union]> => list_1<Material>((s_3: string, json_3: any): FSharpResult$2_$union<Material, [string, ErrorReason_$union]> => Material_decoder(options, s_3, json_3), path_2, value_2))));
        }, s, json);
    }
}

export function Material_fromString(s: string): Material {
    let options: ConverterOptions;
    return fromString<Material>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<Material, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<Material, [string, ErrorReason_$union]> => Material_decoder(options, s_1, json)))), s);
}

export function Material_toString(m: Material): string {
    return toString(2, Material_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Material_toStringLD(m: Material): string {
    let returnVal: ConverterOptions;
    return toString(2, Material_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

