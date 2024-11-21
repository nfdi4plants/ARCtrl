import { fromString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { ROCrate_encoder, ROCrate_decoder, ISAJson_encoder, ISAJson_decoder } from "../Json/DataFile.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../Json/Encode.js";
import { unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";

export function ARCtrl_DataFile__DataFile_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_DataFile__DataFile_toISAJsonString_Static_71136F3F(spaces) {
    return (f) => {
        const value = ISAJson_encoder(f);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_DataFile__DataFile_ToISAJsonString_71136F3F(this$, spaces) {
    return ARCtrl_DataFile__DataFile_toISAJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_DataFile__DataFile_fromROCrateJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ROCrate_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_DataFile__DataFile_toROCrateJsonString_Static_71136F3F(spaces) {
    return (f) => {
        const value = ROCrate_encoder(f);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_DataFile__DataFile_ToROCrateJsonString_71136F3F(this$, spaces) {
    return ARCtrl_DataFile__DataFile_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

