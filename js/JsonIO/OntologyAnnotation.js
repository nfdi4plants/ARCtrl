import { fromString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { OntologyAnnotation_ISAJson_encoder, OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ROCrate_encoderDefinedTerm, OntologyAnnotation_ROCrate_decoderDefinedTerm, OntologyAnnotation_encoder, OntologyAnnotation_decoder } from "../Json/OntologyAnnotation.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../Json/Encode.js";
import { unwrap } from "../fable_modules/fable-library-js.4.22.0/Option.js";

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_fromJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(OntologyAnnotation_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_toJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = OntologyAnnotation_encoder(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_ToJsonString_71136F3F(this$, spaces) {
    return ARCtrl_OntologyAnnotation__OntologyAnnotation_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_fromROCrateJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(OntologyAnnotation_ROCrate_decoderDefinedTerm, s);
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
export function ARCtrl_OntologyAnnotation__OntologyAnnotation_toROCrateJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = OntologyAnnotation_ROCrate_encoderDefinedTerm(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_ToROCrateJsonString_71136F3F(this$, spaces) {
    return ARCtrl_OntologyAnnotation__OntologyAnnotation_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_fromISAJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(OntologyAnnotation_ISAJson_decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_toISAJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = OntologyAnnotation_ISAJson_encoder(undefined, obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_OntologyAnnotation__OntologyAnnotation_ToISAJsonString_71136F3F(this$, spaces) {
    return ARCtrl_OntologyAnnotation__OntologyAnnotation_toISAJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

