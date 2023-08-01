import { fromString as fromString_1, string } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_Ok, FSharpResult$2_Error, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_FailMessage, ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { printf, toText } from "../../fable_modules/fable-library-ts/String.js";
import { contains, exists } from "../../fable_modules/fable-library-ts/Seq.js";
import { stringHash } from "../../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { FSharpList } from "../../fable_modules/fable-library-ts/List.js";

export function isURI(s: string): boolean {
    return true;
}

export function uri<$a>(s: string, json: $a): FSharpResult$2_$union<string, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<string, [string, ErrorReason_$union]> = string(s, json);
    if (matchValue.tag === /* Error */ 1) {
        return FSharpResult$2_Error<string, [string, ErrorReason_$union]>(matchValue.fields[0]);
    }
    else if (isURI(matchValue.fields[0])) {
        return FSharpResult$2_Ok<string, [string, ErrorReason_$union]>(matchValue.fields[0]);
    }
    else {
        const s_3: string = matchValue.fields[0];
        return FSharpResult$2_Error<string, [string, ErrorReason_$union]>([s_3, ErrorReason_FailMessage(toText(printf("Expected URI, got %s"))(s_3))] as [string, ErrorReason_$union]);
    }
}

export function fromString<a>(decoder: ((arg0: string, arg1: any) => FSharpResult$2_$union<a, [string, ErrorReason_$union]>), s: string): a {
    let arg: string;
    const matchValue: FSharpResult$2_$union<a, string> = fromString_1<a>(decoder, s);
    if (matchValue.tag === /* Error */ 1) {
        throw new Error((arg = matchValue.fields[0], toText(printf("Error decoding string: %s"))(arg)));
    }
    else {
        return matchValue.fields[0];
    }
}

export function getFieldNames<$a>(json: any): $a {
    return Object.getOwnPropertyNames(json);
}

export function hasUnknownFields(knownFields: FSharpList<string>, json: any): boolean {
    return exists<string>((x: string): boolean => !contains<string>(x, knownFields, {
        Equals: (x_1: string, y: string): boolean => (x_1 === y),
        GetHashCode: stringHash,
    }), getFieldNames<Iterable<string>>(json));
}

