import { value as value_19 } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { toString, nil, object as object_18 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { list as list_1, string, object as object_19 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString, uri } from "./Decode.js";
import { ProtocolParameter } from "../ISA/JsonTypes/ProtocolParameter.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { Result_Map } from "../../fable_modules/fable-library.4.1.4/Choice.js";
import { Component, Component_decomposeName_Z721C83C5 } from "../ISA/JsonTypes/Component.js";
import { decoder as decoder_3, encoder } from "./Comment.js";
import { Protocol } from "../ISA/JsonTypes/Protocol.js";

export function ProtocolParameter_genID(pp) {
    const matchValue = pp.ID;
    if (matchValue == null) {
        const matchValue_1 = pp.ParameterName;
        let matchResult, n_1;
        if (matchValue_1 != null) {
            if (!(matchValue_1.ID == null)) {
                matchResult = 0;
                n_1 = matchValue_1;
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
                return "#Param_" + value_19(n_1.ID);
            default:
                return "#EmptyProtocolParameter";
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function ProtocolParameter_encoder(options, oa) {
    return object_18(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = ProtocolParameter_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "ProtocolParameter", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => singleton(tryInclude("parameterName", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["ParameterName"]))));
        }));
    }))));
}

export function ProtocolParameter_decoder(options) {
    return (path) => ((v) => object_19((get$) => {
        let objectArg, arg_3, objectArg_1;
        return new ProtocolParameter((objectArg = get$.Optional, objectArg.Field("@id", uri)), (arg_3 = OntologyAnnotation_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field("parameterName", uncurry2(arg_3)))));
    }, path, v));
}

export function ProtocolParameter_fromString(s) {
    return fromString(uncurry2(ProtocolParameter_decoder(ConverterOptions_$ctor())), s);
}

export function ProtocolParameter_toString(p) {
    return toString(2, ProtocolParameter_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function ProtocolParameter_toStringLD(p) {
    let returnVal;
    return toString(2, ProtocolParameter_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function Component_genID(c) {
    const matchValue = c.ComponentName;
    if (matchValue == null) {
        return "#EmptyComponent";
    }
    else {
        return "#Component_" + replace(matchValue, " ", "_");
    }
}

export function Component_encoder(options, oa) {
    return object_18(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Component_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : empty(), delay(() => {
            let value_2, s_1;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_2 = "Component", (typeof value_2 === "string") ? ((s_1 = value_2, s_1)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("componentName", (value_4) => {
                let s_2;
                const value_5 = value_4;
                return (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil;
            }, oa["ComponentName"])), delay(() => singleton(tryInclude("componentType", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["ComponentType"]))))));
        }));
    }))));
}

export function Component_decoder(options, s, json) {
    return Result_Map((c) => {
        let patternInput;
        const matchValue = c.ComponentName;
        if (matchValue == null) {
            patternInput = [void 0, void 0];
        }
        else {
            const tupledArg = Component_decomposeName_Z721C83C5(matchValue);
            patternInput = [tupledArg[0], tupledArg[1]];
        }
        return new Component(c.ComponentName, patternInput[0], patternInput[1], c.ComponentType);
    }, object_19((get$) => {
        let objectArg, arg_3, objectArg_1;
        return new Component((objectArg = get$.Optional, objectArg.Field("componentName", uri)), void 0, void 0, (arg_3 = OntologyAnnotation_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field("componentType", uncurry2(arg_3)))));
    }, s, json));
}

export function Component_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => Component_decoder(options, s_1, json)))), s);
}

export function Component_toString(p) {
    return toString(2, Component_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Component_toStringLD(p) {
    let returnVal;
    return toString(2, Component_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

export function Protocol_genID(p) {
    const matchValue = p.ID;
    if (matchValue == null) {
        const matchValue_1 = p.Uri;
        if (matchValue_1 == null) {
            const matchValue_2 = p.Name;
            if (matchValue_2 == null) {
                return "#EmptyProtocol";
            }
            else {
                return "#Protocol_" + replace(matchValue_2, " ", "_");
            }
        }
        else {
            return matchValue_1;
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Protocol_encoder(options, oa) {
    return object_18(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Protocol_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Protocol", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["Name"])), delay(() => append(singleton(tryInclude("protocolType", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["ProtocolType"])), delay(() => append(singleton(tryInclude("description", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, oa["Description"])), delay(() => append(singleton(tryInclude("uri", (value_13) => {
                let s_5;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_5 = value_14, s_5)) : nil;
            }, oa["Uri"])), delay(() => append(singleton(tryInclude("version", (value_16) => {
                let s_6;
                const value_17 = value_16;
                return (typeof value_17 === "string") ? ((s_6 = value_17, s_6)) : nil;
            }, oa["Version"])), delay(() => append(singleton(tryInclude("parameters", (oa_2) => ProtocolParameter_encoder(options, oa_2), oa["Parameters"])), delay(() => append(singleton(tryInclude("components", (oa_3) => Component_encoder(options, oa_3), oa["Components"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), oa["Comments"]))))))))))))))))));
        }));
    }))));
}

export function Protocol_decoder(options) {
    return (path_6) => ((v) => object_19((get$) => {
        let objectArg, objectArg_1, arg_5, objectArg_2, objectArg_3, objectArg_4, objectArg_5, arg_13, decoder, objectArg_6, objectArg_7, arg_17, decoder_2, objectArg_8;
        return new Protocol((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field("protocolType", uncurry2(arg_5)))), (objectArg_3 = get$.Optional, objectArg_3.Field("description", string)), (objectArg_4 = get$.Optional, objectArg_4.Field("uri", uri)), (objectArg_5 = get$.Optional, objectArg_5.Field("version", string)), (arg_13 = ((decoder = ProtocolParameter_decoder(options), (path_3) => ((value_3) => list_1(uncurry2(decoder), path_3, value_3)))), (objectArg_6 = get$.Optional, objectArg_6.Field("parameters", uncurry2(arg_13)))), (objectArg_7 = get$.Optional, objectArg_7.Field("components", (path_4, value_4) => list_1((s_2, json_2) => Component_decoder(options, s_2, json_2), path_4, value_4))), (arg_17 = ((decoder_2 = decoder_3(options), (path_5) => ((value_5) => list_1(uncurry2(decoder_2), path_5, value_5)))), (objectArg_8 = get$.Optional, objectArg_8.Field("comments", uncurry2(arg_17)))));
    }, path_6, v));
}

export function Protocol_fromString(s) {
    return fromString(uncurry2(Protocol_decoder(ConverterOptions_$ctor())), s);
}

export function Protocol_toString(p) {
    return toString(2, Protocol_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Protocol_toStringLD(p) {
    let returnVal;
    return toString(2, Protocol_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

