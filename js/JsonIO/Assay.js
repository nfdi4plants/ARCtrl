import { fromString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { ISAJson_decoder, ISAJson_encoder, ROCrate_encoder, ROCrate_decoder, encoderCompressed, decoderCompressed, encoder, decoder as decoder_1 } from "../Json/Assay.js";
import { toFail, printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../Json/Encode.js";
import { defaultArg, unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { encode, decode } from "./Table/Compression.js";

export function ARCtrl_ArcAssay__ArcAssay_fromJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(decoder_1, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_ArcAssay__ArcAssay_toJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = encoder(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcAssay__ArcAssay_ToJsonString_71136F3F(this$, spaces) {
    return ARCtrl_ArcAssay__ArcAssay_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_ArcAssay__ArcAssay_fromCompressedJsonString_Static_Z721C83C5(s) {
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
        return toFail(printf("Error. Unable to parse json string to ArcAssay: %s"))(arg_1);
    }
}

export function ARCtrl_ArcAssay__ArcAssay_toCompressedJsonString_Static_71136F3F(spaces) {
    return (obj) => toString(defaultArg(spaces, 0), encode(encoderCompressed, obj));
}

export function ARCtrl_ArcAssay__ArcAssay_ToCompressedJsonString_71136F3F(this$, spaces) {
    return ARCtrl_ArcAssay__ArcAssay_toCompressedJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_ArcAssay__ArcAssay_fromROCrateJsonString_Static_Z721C83C5(s) {
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
export function ARCtrl_ArcAssay__ArcAssay_toROCrateJsonString_Static_5CABCA47(studyName, spaces) {
    return (obj) => {
        const value = ROCrate_encoder(studyName, obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcAssay__ArcAssay_ToROCrateJsonString_5CABCA47(this$, studyName, spaces) {
    return ARCtrl_ArcAssay__ArcAssay_toROCrateJsonString_Static_5CABCA47(unwrap(studyName), unwrap(spaces))(this$);
}

export function ARCtrl_ArcAssay__ArcAssay_toISAJsonString_Static_Z3B036AA(spaces, useIDReferencing) {
    const idMap = defaultArg(useIDReferencing, false) ? (new Map([])) : undefined;
    return (obj) => {
        const value = ISAJson_encoder(undefined, idMap, obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcAssay__ArcAssay_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_ArcAssay__ArcAssay_ToISAJsonString_Z3B036AA(this$, spaces, useIDReferencing) {
    return ARCtrl_ArcAssay__ArcAssay_toISAJsonString_Static_Z3B036AA(unwrap(spaces), unwrap(useIDReferencing))(this$);
}

