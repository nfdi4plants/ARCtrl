import { map } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { Template_encoderCompressed, Template_decoderCompressed, Template_decoder, Template_encoder } from "../../Json/Table/Templates.js";
import { array as array_2 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { toText, printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";
import { defaultArg, unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { encode, decode } from "./Compression.js";

export function Templates_encoder(templates) {
    const values = map(Template_encoder, templates);
    return {
        Encode(helpers) {
            const arg = map((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

export const Templates_decoder = array_2(Template_decoder);

export function Templates_fromJsonString(jsonString) {
    try {
        const matchValue = fromString(Templates_decoder, jsonString);
        if (matchValue.tag === 1) {
            throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
        }
        else {
            return matchValue.fields[0];
        }
    }
    catch (exn) {
        return toFail(printf("Error. Given json string cannot be parsed to Templates map: %A"))(exn);
    }
}

export function Templates_toJsonString(spaces, templates) {
    return toString(spaces, Templates_encoder(templates));
}

export function ARCtrl_Template__Template_fromJsonString_Static_Z721C83C5(jsonString) {
    try {
        const matchValue = fromString(Template_decoder, jsonString);
        if (matchValue.tag === 1) {
            throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
        }
        else {
            return matchValue.fields[0];
        }
    }
    catch (exn) {
        return toFail(printf("Error. Given json string cannot be parsed to Template: %A"))(exn);
    }
}

export function ARCtrl_Template__Template_toJsonString_Static_71136F3F(spaces) {
    return (template) => toString(defaultSpaces(spaces), Template_encoder(template));
}

export function ARCtrl_Template__Template_toJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Template__Template_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_Template__Template_fromCompressedJsonString_Static_Z721C83C5(s) {
    try {
        const matchValue = fromString(decode(Template_decoderCompressed), s);
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

export function ARCtrl_Template__Template_toCompressedJsonString_Static_71136F3F(spaces) {
    return (obj) => toString(defaultArg(spaces, 0), encode(Template_encoderCompressed, obj));
}

export function ARCtrl_Template__Template_toCompressedJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Template__Template_toCompressedJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

