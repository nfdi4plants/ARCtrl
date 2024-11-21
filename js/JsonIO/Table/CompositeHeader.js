import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { encoder, decoder as decoder_1 } from "../../Json/Table/CompositeHeader.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";

export function ARCtrl_CompositeHeader__CompositeHeader_fromJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(decoder_1, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_CompositeHeader__CompositeHeader_toJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = encoder(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_CompositeHeader__CompositeHeader_ToJsonString_71136F3F(this$, spaces) {
    return ARCtrl_CompositeHeader__CompositeHeader_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

