import { Value_Int, Value_Float, Value_Ontology, Value_Name, Value_$union, Value } from "../ISA/JsonTypes/Value.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { object as object_8, toString, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions_$ctor, ConverterOptions } from "./ConverterOptions.js";
import { IOptionalGetter, IGetters, list as list_1, object as object_9, string, float, int } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_Ok, FSharpResult$2_Error, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { float64, int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { uri, fromString } from "./Decode.js";
import { equals, uncurry2 } from "../../fable_modules/fable-library-ts/Util.js";
import { value as value_10, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Factor } from "../ISA/JsonTypes/Factor.js";
import { FSharpList, choose } from "../../fable_modules/fable-library-ts/List.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { tryInclude } from "./GEncode.js";
import { decoder as decoder_1, encoder } from "./Comment.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { FactorValue } from "../ISA/JsonTypes/FactorValue.js";

export function Value_encoder(options: ConverterOptions, value: any): any {
    if (value instanceof Value) {
        if (value.tag === /* Int */ 1) {
            return (value as Value<1>).fields[0];
        }
        else if (value.tag === /* Name */ 3) {
            return (value as Value<3>).fields[0];
        }
        else if (value.tag === /* Ontology */ 0) {
            return OntologyAnnotation_encoder(options, (value as Value<0>).fields[0]);
        }
        else {
            return (value as Value<2>).fields[0];
        }
    }
    else {
        return nil;
    }
}

export function Value_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<Value_$union, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<int32, [string, ErrorReason_$union]> = int(s)(json);
    if (matchValue.tag === /* Error */ 1) {
        const matchValue_1: FSharpResult$2_$union<float64, [string, ErrorReason_$union]> = float(s, json);
        if (matchValue_1.tag === /* Error */ 1) {
            const matchValue_2: FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]> = OntologyAnnotation_decoder(options)(s)(json);
            if (matchValue_2.tag === /* Error */ 1) {
                const matchValue_3: FSharpResult$2_$union<string, [string, ErrorReason_$union]> = string(s, json);
                if (matchValue_3.tag === /* Error */ 1) {
                    return FSharpResult$2_Error<Value_$union, [string, ErrorReason_$union]>(matchValue_3.fields[0]);
                }
                else {
                    return FSharpResult$2_Ok<Value_$union, [string, ErrorReason_$union]>(Value_Name(matchValue_3.fields[0]));
                }
            }
            else {
                return FSharpResult$2_Ok<Value_$union, [string, ErrorReason_$union]>(Value_Ontology(matchValue_2.fields[0]));
            }
        }
        else {
            return FSharpResult$2_Ok<Value_$union, [string, ErrorReason_$union]>(Value_Float(matchValue_1.fields[0]));
        }
    }
    else {
        return FSharpResult$2_Ok<Value_$union, [string, ErrorReason_$union]>(Value_Int(matchValue.fields[0]));
    }
}

export function Value_fromString(s: string): Value_$union {
    let options: ConverterOptions;
    return fromString<Value_$union>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<Value_$union, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<Value_$union, [string, ErrorReason_$union]> => Value_decoder(options, s_1, json)))), s);
}

export function Value_toString(v: Value_$union): string {
    return toString(2, Value_encoder(ConverterOptions_$ctor(), v));
}

export function Factor_genID(f: Factor): string {
    const matchValue: Option<string> = f.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = f.Name;
        if (matchValue_1 == null) {
            return "#EmptyFactor";
        }
        else {
            return "#Factor_" + replace(value_10(matchValue_1), " ", "_");
        }
    }
    else {
        return URIModule_toString(value_10(matchValue));
    }
}

export function Factor_encoder(options: ConverterOptions, oa: any): any {
    return object_8(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Factor_genID(oa as Factor), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Factor", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("factorName", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("factorType", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["FactorType"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), oa["Comments"]))))))));
        }));
    }))));
}

export function Factor_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Factor, [string, ErrorReason_$union]>)) {
    return (path_2: string): ((arg0: any) => FSharpResult$2_$union<Factor, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Factor, [string, ErrorReason_$union]> => object_9<Factor>((get$: IGetters): Factor => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter;
        return new Factor((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("factorName", string)), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field<OntologyAnnotation>("factorType", uncurry2(arg_5)))), (arg_7 = ((decoder = decoder_1(options), (path_1: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_1: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field<FSharpList<Comment$>>("comments", uncurry2(arg_7)))));
    }, path_2, v));
}

export function Factor_fromString(s: string): Factor {
    return fromString<Factor>(uncurry2(Factor_decoder(ConverterOptions_$ctor())), s);
}

export function Factor_toString(f: Factor): string {
    return toString(2, Factor_encoder(ConverterOptions_$ctor(), f));
}

/**
 * exports in json-ld format
 */
export function Factor_toStringLD(f: Factor): string {
    let returnVal: ConverterOptions;
    return toString(2, Factor_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), f));
}

export function FactorValue_genID(fv: FactorValue): string {
    const matchValue: Option<string> = fv.ID;
    if (matchValue == null) {
        return "#EmptyFactorValue";
    }
    else {
        return URIModule_toString(value_10(matchValue));
    }
}

export function FactorValue_encoder(options: ConverterOptions, oa: any): any {
    return object_8(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = FactorValue_genID(oa as FactorValue), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "FactorValue", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("category", (oa_1: any): any => Factor_encoder(options, oa_1), oa["Category"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("value", (value_7: any): any => Value_encoder(options, value_7), oa["Value"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("unit", (oa_2: any): any => OntologyAnnotation_encoder(options, oa_2), oa["Unit"]))))))));
        }));
    }))));
}

export function FactorValue_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FactorValue, [string, ErrorReason_$union]>)) {
    return (path: string): ((arg0: any) => FSharpResult$2_$union<FactorValue, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<FactorValue, [string, ErrorReason_$union]> => object_9<FactorValue>((get$: IGetters): FactorValue => {
        let objectArg: IOptionalGetter, arg_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Factor, [string, ErrorReason_$union]>)), objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter;
        return new FactorValue((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (arg_3 = Factor_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field<Factor>("category", uncurry2(arg_3)))), (objectArg_2 = get$.Optional, objectArg_2.Field<Value_$union>("value", (s_1: string, json_1: any): FSharpResult$2_$union<Value_$union, [string, ErrorReason_$union]> => Value_decoder(options, s_1, json_1))), (arg_7 = OntologyAnnotation_decoder(options), (objectArg_3 = get$.Optional, objectArg_3.Field<OntologyAnnotation>("unit", uncurry2(arg_7)))));
    }, path, v));
}

export function FactorValue_fromString(s: string): FactorValue {
    return fromString<FactorValue>(uncurry2(FactorValue_decoder(ConverterOptions_$ctor())), s);
}

export function FactorValue_toString(f: FactorValue): string {
    return toString(2, FactorValue_encoder(ConverterOptions_$ctor(), f));
}

/**
 * exports in json-ld format
 */
export function FactorValue_toStringLD(f: FactorValue): string {
    let returnVal: ConverterOptions;
    return toString(2, FactorValue_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), f));
}

