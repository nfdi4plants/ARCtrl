import { fromString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Decode.fs.js";
import { encoderCompressed, decoderCompressed, encoder, decoder as decoder_2 } from "../../Json/Table/ArcTable.js";
import { printf, toText } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { toString } from "../../fable_modules/Thoth.Json.JavaScript.0.3.0/Encode.fs.js";
import { defaultSpaces } from "../../Json/Encode.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { arrayFromMap as arrayFromMap_2, encoder as encoder_3, decoder as decoder_3 } from "../../Json/StringTable.js";
import { arrayFromMap as arrayFromMap_1, encoder as encoder_2, decoder as decoder_4 } from "../../Json/Table/OATable.js";
import { arrayFromMap, encoder as encoder_1, decoder as decoder_5 } from "../../Json/Table/CellTable.js";
import { Dictionary } from "../../fable_modules/fable-library-js.4.22.0/MutableMap.js";
import { safeHash, equals } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";

export function ARCtrl_ArcTable__ArcTable_fromJsonString_Static_Z721C83C5(s) {
    const matchValue = fromString(decoder_2, s);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_ArcTable__ArcTable_toJsonString_Static_71136F3F(spaces) {
    return (obj) => {
        const value = encoder(obj);
        return toString(defaultSpaces(spaces), value);
    };
}

export function ARCtrl_ArcTable__ArcTable_ToJsonString_71136F3F(this$, spaces) {
    return ARCtrl_ArcTable__ArcTable_toJsonString_Static_71136F3F(unwrap(spaces))(this$);
}

export function ARCtrl_ArcTable__ArcTable_fromCompressedJsonString_Static_Z721C83C5(jsonString) {
    const matchValue = fromString(object((get$) => {
        let arg_5, objectArg_2;
        let stringTable;
        const objectArg = get$.Required;
        stringTable = objectArg.Field("stringTable", decoder_3);
        let oaTable;
        const arg_3 = decoder_4(stringTable);
        const objectArg_1 = get$.Required;
        oaTable = objectArg_1.Field("oaTable", arg_3);
        const arg_7 = decoderCompressed(stringTable, oaTable, (arg_5 = decoder_5(stringTable, oaTable), (objectArg_2 = get$.Required, objectArg_2.Field("cellTable", arg_5))));
        const objectArg_3 = get$.Required;
        return objectArg_3.Field("table", arg_7);
    }), jsonString);
    if (matchValue.tag === 1) {
        throw new Error(toText(printf("Error decoding string: %O"))(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function ARCtrl_ArcTable__ArcTable_ToCompressedJsonString_71136F3F(this$, spaces) {
    let values;
    const spaces_1 = defaultSpaces(spaces) | 0;
    const stringTable = new Map([]);
    const oaTable = new Dictionary([], {
        Equals: equals,
        GetHashCode: safeHash,
    });
    const cellTable = new Dictionary([], {
        Equals: equals,
        GetHashCode: safeHash,
    });
    const arcTable = encoderCompressed(stringTable, oaTable, cellTable, this$);
    return toString(spaces_1, (values = [["cellTable", encoder_1(stringTable, oaTable, arrayFromMap(cellTable))], ["oaTable", encoder_2(stringTable, arrayFromMap_1(oaTable))], ["stringTable", encoder_3(arrayFromMap_2(stringTable))], ["table", arcTable]], {
        Encode(helpers) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values);
            return helpers.encodeObject(arg);
        },
    }));
}

export function ARCtrl_ArcTable__ArcTable_toCompressedJsonString_Static_71136F3F(spaces) {
    return (obj) => ARCtrl_ArcTable__ArcTable_ToCompressedJsonString_71136F3F(obj, unwrap(spaces));
}

