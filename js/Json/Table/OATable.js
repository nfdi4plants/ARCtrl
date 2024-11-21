import { sortBy, map, toArray } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { comparePrimitives } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { item, map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { OntologyAnnotation_compressedDecoder, OntologyAnnotation_compressedEncoder } from "../OntologyAnnotation.js";
import { int, object, array as array_2 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Dictionary_tryFind } from "../../Core/Helper/Collections.js";
import { addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";

export function arrayFromMap(otm) {
    return toArray(map((kv_1) => kv_1[0], sortBy((kv) => kv[1], otm, {
        Compare: comparePrimitives,
    })));
}

export function encoder(stringTable, ot) {
    const values = map_1((oa) => OntologyAnnotation_compressedEncoder(stringTable, oa), ot);
    return {
        Encode(helpers) {
            const arg = map_1((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

export function decoder(stringTable) {
    return array_2(OntologyAnnotation_compressedDecoder(stringTable));
}

export function encodeOA(otm, oa) {
    const matchValue = Dictionary_tryFind(oa, otm);
    if (matchValue == null) {
        const i_1 = otm.size | 0;
        addToDict(otm, oa, i_1);
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

export function decodeOA(ot) {
    return object((get$) => item(get$.Required.Raw(int), ot));
}

