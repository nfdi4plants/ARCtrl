import { defaultArg, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { append, map as map_1, isEmpty } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { list as list_1, seq } from "../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { item, map as map_2 } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { map as map_3, isEmpty as isEmpty_1 } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Date.js";
import { Json } from "../fable_modules/Thoth.Json.Core.0.4.0/Types.fs.js";

/**
 * Try to encode the given object using the given encoder, or return Encode.nil if the object is null
 */
export function tryInclude(name, encoder, value) {
    return [name, map(encoder, value)];
}

/**
 * Try to encode the given object using the given encoder, or return Encode.nil if the object is null
 */
export function tryIncludeSeq(name, encoder, value) {
    return [name, isEmpty(value) ? undefined : seq(map_1(encoder, value))];
}

export function tryIncludeArray(name, encoder, value) {
    let values;
    return [name, (value.length === 0) ? undefined : ((values = map_2(encoder, value), {
        Encode(helpers) {
            const arg = map_2((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    }))];
}

export function tryIncludeList(name, encoder, value) {
    return [name, isEmpty_1(value) ? undefined : list_1(map_3(encoder, value))];
}

export function tryIncludeListOpt(name, encoder, value) {
    let o;
    return [name, (value != null) ? ((o = value, isEmpty_1(o) ? undefined : list_1(map_3(encoder, o)))) : undefined];
}

export const DefaultSpaces = 0;

export function defaultSpaces(spaces) {
    return defaultArg(spaces, DefaultSpaces);
}

export function dateTime(d) {
    const value = item(0, toString(d, "O", {}).split("+"));
    return {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    };
}

export function addPropertyToObject(name, value, obj) {
    if (obj.tag === 5) {
        return new Json(5, [append(obj.fields[0], [[name, value]])]);
    }
    else {
        throw new Error("Expected object");
    }
}

