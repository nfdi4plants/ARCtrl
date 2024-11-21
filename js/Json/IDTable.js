import { singleton } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { addToDict } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";

export function encodeID(id) {
    const values = singleton(["@id", {
        Encode(helpers) {
            return helpers.encodeString(id);
        },
    }]);
    return {
        Encode(helpers_1) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_1)], values);
            return helpers_1.encodeObject(arg);
        },
    };
}

export function encode(genID, encoder, value, table) {
    const id = genID(value);
    if (table.has(id)) {
        return encodeID(id);
    }
    else {
        const v = encoder(value);
        addToDict(table, genID(value), v);
        return v;
    }
}

