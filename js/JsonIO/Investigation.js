import { fromString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { ISAJson_decoder, ISAJson_encoder, ROCrate_encoder, ROCrate_decoder, encoderCompressed, decoderCompressed, encoder, decoder as decoder_1 } from "../Json/Investigation.js";
import { toFail, printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../Json/Encode.js";
import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { encode, decode } from "./Table/Compression.js";

export function ARCtrl_ArcInvestigation__ArcInvestigation_fromJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(decoder_1, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_toJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = encoder(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToJsonString_71136F3F(this$, spaces) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_fromCompressedJsonString_Static_Z721C83C5(s) {
    try {
        const matchValue = fromString(decode(decoderCompressed), s);
        if (matchValue.tag === 1) {
            throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
        }
        else {
            return matchValue.fields[0];
        }
    }
    catch (e_1) {
        const arg_1 = e_1.message;
        return toFail(printf("Error. Unable to parse json string to ArcStudy: %s"))(arg_1);
    }
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_toCompressedJsonString_Static_71136F3F(spaces) {
    return (obj) => toString(defaultArg(spaces, 0), encode(encoderCompressed, obj));
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToCompressedJsonString_71136F3F(this$, spaces) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_toCompressedJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_fromROCrateJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ROCrate_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

/**
 * exports in json-ld format
 */
export function ARCtrl_ArcInvestigation__ArcInvestigation_toROCrateJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = ROCrate_encoder(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToROCrateJsonString_71136F3F(this$, spaces) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_toISAJsonString_Static_Z3B036AA(spaces, useIDReferencing) {
    const idMap = defaultArg(useIDReferencing, false) ? (new Map([])) : undefined;
    return (obj) => {
        const value = ISAJson_encoder(idMap, obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_ArcInvestigation__ArcInvestigation_ToISAJsonString_Z3B036AA(this$, spaces, useIDReferencing) {
    return ARCtrl_ArcInvestigation__ArcInvestigation_toISAJsonString_Static_Z3B036AA(unwrap(spaces), unwrap(useIDReferencing))(this$);
}

