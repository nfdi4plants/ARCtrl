import { Value } from "../ISA/JsonTypes/Value.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { object as object_8, toString, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { array, object as object_9, string, float, int } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library.4.1.4/Choice.js";
import { uri, fromString } from "./Decode.js";
import { equals, uncurry2 } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions_$ctor } from "./ConverterOptions.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { tryInclude } from "./GEncode.js";
import { decoder as decoder_1, encoder } from "./Comment.js";
import { Factor } from "../ISA/JsonTypes/Factor.js";
import { FactorValue } from "../ISA/JsonTypes/FactorValue.js";

export function Value_encoder(options, value) {
    if (value instanceof Value) {
        if (value.tag === 1) {
            return value.fields[0];
        }
        else if (value.tag === 3) {
            return value.fields[0];
        }
        else if (value.tag === 0) {
            return OntologyAnnotation_encoder(options, value.fields[0]);
        }
        else {
            return value.fields[0];
        }
    }
    else {
        return nil;
    }
}

export function Value_decoder(options, s, json) {
    const matchValue = int(s)(json);
    if (matchValue.tag === 1) {
        const matchValue_1 = float(s, json);
        if (matchValue_1.tag === 1) {
            const matchValue_2 = OntologyAnnotation_decoder(options)(s)(json);
            if (matchValue_2.tag === 1) {
                const matchValue_3 = string(s, json);
                if (matchValue_3.tag === 1) {
                    return new FSharpResult$2(1, [matchValue_3.fields[0]]);
                }
                else {
                    return new FSharpResult$2(0, [new Value(3, [matchValue_3.fields[0]])]);
                }
            }
            else {
                return new FSharpResult$2(0, [new Value(0, [matchValue_2.fields[0]])]);
            }
        }
        else {
            return new FSharpResult$2(0, [new Value(2, [matchValue_1.fields[0]])]);
        }
    }
    else {
        return new FSharpResult$2(0, [new Value(1, [matchValue.fields[0]])]);
    }
}

export function Value_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => Value_decoder(options, s_1, json)))), s);
}

export function Value_toString(v) {
    return toString(2, Value_encoder(ConverterOptions_$ctor(), v));
}

export function Factor_genID(f) {
    const matchValue = f.ID;
    if (matchValue == null) {
        const matchValue_1 = f.Name;
        if (matchValue_1 == null) {
            return "#EmptyFactor";
        }
        else {
            return "#Factor_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Factor_encoder(options, oa) {
    return object_8(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Factor_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Factor", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("factorName", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["Name"])), delay(() => append(singleton(tryInclude("factorType", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["FactorType"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), oa["Comments"]))))))));
        }));
    }))));
}

export function Factor_decoder(options) {
    return (path_2) => ((v) => object_9((get$) => {
        let objectArg, objectArg_1, arg_5, objectArg_2, arg_7, decoder, objectArg_3;
        return new Factor((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("factorName", string)), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field("factorType", uncurry2(arg_5)))), (arg_7 = ((decoder = decoder_1(options), (path_1) => ((value_1) => array(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field("comments", uncurry2(arg_7)))));
    }, path_2, v));
}

export function Factor_fromString(s) {
    return fromString(uncurry2(Factor_decoder(ConverterOptions_$ctor())), s);
}

export function Factor_toString(f) {
    return toString(2, Factor_encoder(ConverterOptions_$ctor(), f));
}

/**
 * exports in json-ld format
 */
export function Factor_toStringLD(f) {
    let returnVal;
    return toString(2, Factor_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), f));
}

export function FactorValue_genID(fv) {
    const matchValue = fv.ID;
    if (matchValue == null) {
        return "#EmptyFactorValue";
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function FactorValue_encoder(options, oa) {
    return object_8(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = FactorValue_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "FactorValue", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("category", (oa_1) => Factor_encoder(options, oa_1), oa["Category"])), delay(() => append(singleton(tryInclude("value", (value_7) => Value_encoder(options, value_7), oa["Value"])), delay(() => singleton(tryInclude("unit", (oa_2) => OntologyAnnotation_encoder(options, oa_2), oa["Unit"]))))))));
        }));
    }))));
}

export function FactorValue_decoder(options) {
    return (path) => ((v) => object_9((get$) => {
        let objectArg, arg_3, objectArg_1, objectArg_2, arg_7, objectArg_3;
        return new FactorValue((objectArg = get$.Optional, objectArg.Field("@id", uri)), (arg_3 = Factor_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field("category", uncurry2(arg_3)))), (objectArg_2 = get$.Optional, objectArg_2.Field("value", (s_1, json_1) => Value_decoder(options, s_1, json_1))), (arg_7 = OntologyAnnotation_decoder(options), (objectArg_3 = get$.Optional, objectArg_3.Field("unit", uncurry2(arg_7)))));
    }, path, v));
}

export function FactorValue_fromString(s) {
    return fromString(uncurry2(FactorValue_decoder(ConverterOptions_$ctor())), s);
}

export function FactorValue_toString(f) {
    return toString(2, FactorValue_encoder(ConverterOptions_$ctor(), f));
}

/**
 * exports in json-ld format
 */
export function FactorValue_toStringLD(f) {
    let returnVal;
    return toString(2, FactorValue_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), f));
}

