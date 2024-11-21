import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { ROCrate_encoder, ROCrate_decoder, ISAJson_encoder, ISAJson_decoder } from "../../Json/Process/Component.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { unwrap, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";

export function ARCtrl_Process_Component__Component_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_Process_Component__Component_toISAJsonString_Static_Z3B036AA(spaces, useIDReferencing) {
    const idMap = defaultArg(useIDReferencing, false) ? (new Map([])) : undefined;
    return (f) => {
        const value = ISAJson_encoder(idMap, f);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Process_Component__Component_toISAJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Process_Component__Component_toISAJsonString_Static_Z3B036AA(unwrap(spaces))(this$);
}

export function ARCtrl_Process_Component__Component_fromROCrateJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(ROCrate_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_Process_Component__Component_toROCrateJsonString_Static_71136F3F(spaces) {
    return (f) => {
        const value = ROCrate_encoder(f);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_Process_Component__Component_toROCrateJsonString_71136F3F(this$, spaces) {
    return ARCtrl_Process_Component__Component_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

