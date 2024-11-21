import { fromString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { ISAJson_encoder, ISAJson_decoder, ROCrate_encoder, ROCrate_decoder, encoder, decoder as decoder_1 } from "../Json/Comment.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../Json/Encode.js";
import { unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";

export function ARCtrl_Comment__Comment_fromJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(decoder_1, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_Comment__Comment_toJsonString_Static_71136F3F(spaces) {
    return (c) => {
        const value = encoder(c);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Comment__Comment_toJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Comment__Comment_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_Comment__Comment_fromROCrateJsonString_Static_Z721C83C5(s) {
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
export function ARCtrl_Comment__Comment_toROCrateJsonString_Static_71136F3F(spaces) {
    return (c) => {
        const value = ROCrate_encoder(c);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Comment__Comment_toROCrateJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Comment__Comment_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_Comment__Comment_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_Comment__Comment_toISAJsonString_Static_71136F3F(spaces) {
    return (c) => {
        const value = ISAJson_encoder(undefined, c);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Comment__Comment_toISAJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Comment__Comment_toISAJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

