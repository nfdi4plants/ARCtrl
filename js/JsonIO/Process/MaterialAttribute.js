import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { encoder, decoder as decoder_1 } from "../../Json/Process/MaterialAttribute.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { unwrap, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(decoder_1, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_toISAJsonString_Static_Z3B036AA(spaces, useIDReferencing) {
    const idMap = defaultArg(useIDReferencing, false) ? (new Map([])) : undefined;
    return (v) => {
        const value = encoder(idMap, v);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Process_MaterialAttribute__MaterialAttribute_ToJsonString_Z3B036AA(this$, spaces, useIDReferencing) {
    return ARCtrl_Process_MaterialAttribute__MaterialAttribute_toISAJsonString_Static_Z3B036AA(unwrap(spaces), unwrap(useIDReferencing))(this$);
}

