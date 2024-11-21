import { item, map, setItem, fill } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { iterate } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { int, string, array as array_2 } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Dictionary_tryFind } from "../Core/Helper/Collections.js";
import { addToDict } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { FSharpResult$2 } from "../fable_modules/fable-library-js.4.22.0/Result.js";

export function arrayFromMap(otm) {
    const a = fill(new Array(otm.size), 0, otm.size, "");
    iterate((kv) => {
        setItem(a, kv[1], kv[0]);
    }, otm);
    return a;
}

export function encoder(ot) {
    const values = map((value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), ot);
    return {
        Encode(helpers_1) {
            const arg = map((v) => v.Encode(helpers_1), values);
            return helpers_1.encodeArray(arg);
        },
    };
}

export const decoder = array_2(string);

export function encodeString(otm, s) {
    const matchValue = Dictionary_tryFind(s, otm);
    if (matchValue == null) {
        const i_1 = otm.size | 0;
        addToDict(otm, s, i_1);
        return {
            Encode(helpers_1) {
                return helpers_1.encodeSignedIntegralNumber(i_1);
            },
        };
    }
    else {
        const i = matchValue | 0;
        return {
            Encode(helpers) {
                return helpers.encodeSignedIntegralNumber(i);
            },
        };
    }
}

export function decodeString(ot) {
    return {
        Decode(s, json) {
            const matchValue = int.Decode(s, json);
            return (matchValue.tag === 1) ? (new FSharpResult$2(1, [matchValue.fields[0]])) : (new FSharpResult$2(0, [item(matchValue.fields[0], ot)]));
        },
    };
}

