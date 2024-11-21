import { seq } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { decoder as decoder_1, encoder as encoder_1 } from "./DataContext.js";
import { DataMap_$ctor_4E3220A7, DataMap__get_DataContexts } from "../../Core/DataMap.js";
import { resizeArray, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";

export function encoder(dm) {
    const values_1 = [["dataContexts", seq(map(encoder_1, DataMap__get_DataContexts(dm)))]];
    return {
        Encode(helpers) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values_1);
            return helpers.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let arg_1, objectArg;
    return DataMap_$ctor_4E3220A7((arg_1 = resizeArray(decoder_1), (objectArg = get$.Required, objectArg.Field("dataContexts", arg_1))));
});

export function encoderCompressed(stringTable, oaTable, cellTable, dm) {
    return encoder(dm);
}

export function decoderCompressed(stringTable, oaTable, cellTable) {
    return decoder;
}

