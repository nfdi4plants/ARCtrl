import { Dictionary } from "../../fable_modules/fable-library-js.4.22.0/MutableMap.js";
import { safeHash, equals } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { decoder as decoder_3, arrayFromMap, encoder as encoder_1 } from "../../Json/Table/CellTable.js";
import { decoder as decoder_2, arrayFromMap as arrayFromMap_1, encoder as encoder_2 } from "../../Json/Table/OATable.js";
import { decoder as decoder_1, arrayFromMap as arrayFromMap_2, encoder as encoder_3 } from "../../Json/StringTable.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { object as object_1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";

export function encode(encoder, obj) {
    const stringTable = new Map([]);
    const oaTable = new Dictionary([], {
        Equals: equals,
        GetHashCode: safeHash,
    });
    const cellTable = new Dictionary([], {
        Equals: equals,
        GetHashCode: safeHash,
    });
    const object = encoder(stringTable, oaTable, cellTable, obj);
    toString(0, object);
    const encodedCellTable = encoder_1(stringTable, oaTable, arrayFromMap(cellTable));
    const encodedOATable = encoder_2(stringTable, arrayFromMap_1(oaTable));
    const encodedStringTable = encoder_3(arrayFromMap_2(stringTable));
    return {
        Encode(helpers) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], [["cellTable", encodedCellTable], ["oaTable", encodedOATable], ["stringTable", encodedStringTable], ["object", object]]);
            return helpers.encodeObject(arg);
        },
    };
}

export function decode(decoder) {
    return object_1((get$) => {
        let arg_5, objectArg_2;
        let stringTable;
        const objectArg = get$.Required;
        stringTable = objectArg.Field("stringTable", decoder_1);
        let oaTable;
        const arg_3 = decoder_2(stringTable);
        const objectArg_1 = get$.Required;
        oaTable = objectArg_1.Field("oaTable", arg_3);
        const arg_7 = decoder(stringTable, oaTable, (arg_5 = decoder_3(stringTable, oaTable), (objectArg_2 = get$.Required, objectArg_2.Field("cellTable", arg_5))));
        const objectArg_3 = get$.Required;
        return objectArg_3.Field("object", arg_7);
    });
}

