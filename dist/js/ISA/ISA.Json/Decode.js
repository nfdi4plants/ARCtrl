import { fromString as fromString_1, string } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library.4.1.4/Choice.js";
import { printf, toText } from "../../fable_modules/fable-library.4.1.4/String.js";
import { ErrorReason } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { contains, exists } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { stringHash } from "../../fable_modules/fable-library.4.1.4/Util.js";

export function isURI(s) {
    return true;
}

export function uri(s, json) {
    const matchValue = string(s, json);
    if (matchValue.tag === 1) {
        return new FSharpResult$2(1, [matchValue.fields[0]]);
    }
    else if (isURI(matchValue.fields[0])) {
        return new FSharpResult$2(0, [matchValue.fields[0]]);
    }
    else {
        const s_3 = matchValue.fields[0];
        return new FSharpResult$2(1, [[s_3, new ErrorReason(6, [toText(printf("Expected URI, got %s"))(s_3)])]]);
    }
}

export function fromString(decoder, s) {
    const matchValue = fromString_1(decoder, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %s"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function getFieldNames(json) {
    return Object.getOwnPropertyNames(json);
}

export function hasUnknownFields(knownFields, json) {
    return exists((x) => !contains(x, knownFields, {
        Equals: (x_1, y) => (x_1 === y),
        GetHashCode: stringHash,
    }), getFieldNames(json));
}

