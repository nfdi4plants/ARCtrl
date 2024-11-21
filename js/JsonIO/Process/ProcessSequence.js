import { class_type } from "../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { list as list_1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { ROCrate_encoder, ROCrate_decoder, ISAJson_encoder, ISAJson_decoder } from "../../Json/Process/Process.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { list as list_2 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";

export class ProcessSequence {
    constructor() {
    }
}

export function ProcessSequence_$reflection() {
    return class_type("ARCtrl.Json.ProcessSequence", undefined, ProcessSequence);
}

export function ProcessSequence_fromISAJsonString_Z721C83C5(s) {
    const matchValue = fromString(list_1(ISAJson_decoder), s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ProcessSequence_toISAJsonString_Z3B036AA(spaces, useIDReferencing) {
    const idMap = defaultArg(useIDReferencing, false) ? (new Map([])) : undefined;
    return (f) => {
        const value = list_2(map((oa) => ISAJson_encoder(undefined, undefined, idMap, oa), f));
        return toString(defaultSpaces(spaces), value);
    };
}

export function ProcessSequence_fromROCrateJsonString_Z721C83C5(s) {
    const matchValue = fromString(list_1(ROCrate_decoder), s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ProcessSequence_toROCrateJsonString_39E0BC3F(studyName, assayName, spaces) {
    return (f) => {
        const value = list_2(map((oa) => ROCrate_encoder(studyName, assayName, oa), f));
        return toString(defaultSpaces(spaces), value);
    };
}

