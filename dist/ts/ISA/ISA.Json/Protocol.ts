import { value as value_19, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { ProtocolParameter } from "../ISA/JsonTypes/ProtocolParameter.js";
import { toString, nil, object as object_18 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { FSharpList, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { list as list_1, string, IOptionalGetter, IGetters, object as object_19 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString, uri } from "./Decode.js";
import { Result_Map, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { Component_decomposeName_Z721C83C5, Component } from "../ISA/JsonTypes/Component.js";
import { Value_$union } from "../ISA/JsonTypes/Value.js";
import { Protocol } from "../ISA/JsonTypes/Protocol.js";
import { decoder as decoder_3, encoder } from "./Comment.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";

export function ProtocolParameter_genID(pp: ProtocolParameter): string {
    const matchValue: Option<string> = pp.ID;
    if (matchValue == null) {
        const matchValue_1: Option<OntologyAnnotation> = pp.ParameterName;
        let matchResult: int32, n_1: OntologyAnnotation;
        if (matchValue_1 != null) {
            if (!(value_19(matchValue_1).ID == null)) {
                matchResult = 0;
                n_1 = value_19(matchValue_1);
            }
            else {
                matchResult = 1;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0:
                return "#Param_" + value_19(n_1!.ID);
            default:
                return "#EmptyProtocolParameter";
        }
    }
    else {
        return URIModule_toString(value_19(matchValue));
    }
}

export function ProtocolParameter_encoder(options: ConverterOptions, oa: any): any {
    return object_18(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = ProtocolParameter_genID(oa as ProtocolParameter), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "ProtocolParameter", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("parameterName", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["ParameterName"]))));
        }));
    }))));
}

export function ProtocolParameter_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<ProtocolParameter, [string, ErrorReason_$union]>)) {
    return (path: string): ((arg0: any) => FSharpResult$2_$union<ProtocolParameter, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<ProtocolParameter, [string, ErrorReason_$union]> => object_19<ProtocolParameter>((get$: IGetters): ProtocolParameter => {
        let objectArg: IOptionalGetter, arg_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_1: IOptionalGetter;
        return new ProtocolParameter((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (arg_3 = OntologyAnnotation_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field<OntologyAnnotation>("parameterName", uncurry2(arg_3)))));
    }, path, v));
}

export function ProtocolParameter_fromString(s: string): ProtocolParameter {
    return fromString<ProtocolParameter>(uncurry2(ProtocolParameter_decoder(ConverterOptions_$ctor())), s);
}

export function ProtocolParameter_toString(p: ProtocolParameter): string {
    return toString(2, ProtocolParameter_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function ProtocolParameter_toStringLD(p: ProtocolParameter): string {
    let returnVal: ConverterOptions;
    return toString(2, ProtocolParameter_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function Component_genID(c: Component): string {
    const matchValue: Option<string> = c.ComponentName;
    if (matchValue == null) {
        return "#EmptyComponent";
    }
    else {
        return "#Component_" + replace(value_19(matchValue), " ", "_");
    }
}

export function Component_encoder(options: ConverterOptions, oa: any): any {
    return object_18(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Component_genID(oa as Component), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_2: any, s_1: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_2 = "Component", (typeof value_2 === "string") ? ((s_1 = (value_2 as string), s_1)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("componentName", (value_4: any): any => {
                let s_2: string;
                const value_5: any = value_4;
                return (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil;
            }, oa["ComponentName"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("componentType", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["ComponentType"]))))));
        }));
    }))));
}

export function Component_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<Component, [string, ErrorReason_$union]> {
    return Result_Map<Component, Component, [string, ErrorReason_$union]>((c: Component): Component => {
        let patternInput: [Option<Value_$union>, Option<OntologyAnnotation>];
        const matchValue: Option<string> = c.ComponentName;
        if (matchValue == null) {
            patternInput = ([void 0, void 0] as [Option<Value_$union>, Option<OntologyAnnotation>]);
        }
        else {
            const tupledArg: [Value_$union, Option<OntologyAnnotation>] = Component_decomposeName_Z721C83C5(value_19(matchValue));
            patternInput = ([tupledArg[0], tupledArg[1]] as [Option<Value_$union>, Option<OntologyAnnotation>]);
        }
        return new Component(c.ComponentName, patternInput[0], patternInput[1], c.ComponentType);
    }, object_19<Component>((get$: IGetters): Component => {
        let objectArg: IOptionalGetter, arg_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_1: IOptionalGetter;
        return new Component((objectArg = get$.Optional, objectArg.Field<string>("componentName", uri)), void 0, void 0, (arg_3 = OntologyAnnotation_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field<OntologyAnnotation>("componentType", uncurry2(arg_3)))));
    }, s, json));
}

export function Component_fromString(s: string): Component {
    let options: ConverterOptions;
    return fromString<Component>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<Component, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<Component, [string, ErrorReason_$union]> => Component_decoder(options, s_1, json)))), s);
}

export function Component_toString(p: Component): string {
    return toString(2, Component_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Component_toStringLD(p: Component): string {
    let returnVal: ConverterOptions;
    return toString(2, Component_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function Protocol_genID(p: Protocol): string {
    const matchValue: Option<string> = p.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = p.Uri;
        if (matchValue_1 == null) {
            const matchValue_2: Option<string> = p.Name;
            if (matchValue_2 == null) {
                return "#EmptyProtocol";
            }
            else {
                return "#Protocol_" + replace(value_19(matchValue_2), " ", "_");
            }
        }
        else {
            return value_19(matchValue_1);
        }
    }
    else {
        return URIModule_toString(value_19(matchValue));
    }
}

export function Protocol_encoder(options: ConverterOptions, oa: any): any {
    return object_18(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Protocol_genID(oa as Protocol), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Protocol", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("protocolType", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["ProtocolType"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("description", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, oa["Description"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("uri", (value_13: any): any => {
                let s_5: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_5 = (value_14 as string), s_5)) : nil;
            }, oa["Uri"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("version", (value_16: any): any => {
                let s_6: string;
                const value_17: any = value_16;
                return (typeof value_17 === "string") ? ((s_6 = (value_17 as string), s_6)) : nil;
            }, oa["Version"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("parameters", (oa_2: any): any => ProtocolParameter_encoder(options, oa_2), oa["Parameters"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("components", (oa_3: any): any => Component_encoder(options, oa_3), oa["Components"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), oa["Comments"]))))))))))))))))));
        }));
    }))));
}

export function Protocol_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Protocol, [string, ErrorReason_$union]>)) {
    return (path_6: string): ((arg0: any) => FSharpResult$2_$union<Protocol, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Protocol, [string, ErrorReason_$union]> => object_19<Protocol>((get$: IGetters): Protocol => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter, objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter, objectArg_5: IOptionalGetter, arg_13: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<ProtocolParameter>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<ProtocolParameter, [string, ErrorReason_$union]>)), objectArg_6: IOptionalGetter, objectArg_7: IOptionalGetter, arg_17: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder_2: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_8: IOptionalGetter;
        return new Protocol((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field<OntologyAnnotation>("protocolType", uncurry2(arg_5)))), (objectArg_3 = get$.Optional, objectArg_3.Field<string>("description", string)), (objectArg_4 = get$.Optional, objectArg_4.Field<string>("uri", uri)), (objectArg_5 = get$.Optional, objectArg_5.Field<string>("version", string)), (arg_13 = ((decoder = ProtocolParameter_decoder(options), (path_3: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<ProtocolParameter>, [string, ErrorReason_$union]>) => ((value_3: any): FSharpResult$2_$union<FSharpList<ProtocolParameter>, [string, ErrorReason_$union]> => list_1<ProtocolParameter>(uncurry2(decoder), path_3, value_3)))), (objectArg_6 = get$.Optional, objectArg_6.Field<FSharpList<ProtocolParameter>>("parameters", uncurry2(arg_13)))), (objectArg_7 = get$.Optional, objectArg_7.Field<FSharpList<Component>>("components", (path_4: string, value_4: any): FSharpResult$2_$union<FSharpList<Component>, [string, ErrorReason_$union]> => list_1<Component>((s_2: string, json_2: any): FSharpResult$2_$union<Component, [string, ErrorReason_$union]> => Component_decoder(options, s_2, json_2), path_4, value_4))), (arg_17 = ((decoder_2 = decoder_3(options), (path_5: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_5: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder_2), path_5, value_5)))), (objectArg_8 = get$.Optional, objectArg_8.Field<FSharpList<Comment$>>("comments", uncurry2(arg_17)))));
    }, path_6, v));
}

export function Protocol_fromString(s: string): Protocol {
    return fromString<Protocol>(uncurry2(Protocol_decoder(ConverterOptions_$ctor())), s);
}

export function Protocol_toString(p: Protocol): string {
    return toString(2, Protocol_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Protocol_toStringLD(p: Protocol): string {
    let returnVal: ConverterOptions;
    return toString(2, Protocol_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

