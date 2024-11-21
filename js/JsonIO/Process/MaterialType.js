import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { ISAJson_encoder, ISAJson_decoder } from "../../Json/Process/MaterialType.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";

export function ARCtrl_Process_MaterialType__MaterialType_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_Process_MaterialType__MaterialType_toISAJsonString_Static_71136F3F(spaces) {
    return (f) => {
        const value = ISAJson_encoder(f);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Process_MaterialType__MaterialType_ToISAJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Process_MaterialType__MaterialType_toISAJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

