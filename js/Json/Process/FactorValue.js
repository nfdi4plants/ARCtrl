import { genID, decoder, encoder } from "../PropertyValue.js";
import { FactorValue, FactorValue_createAsPV } from "../../Core/Process/FactorValue.js";
import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryInclude } from "../Encode.js";
import { decoder as decoder_1, encoder as encoder_1 } from "./Factor.js";
import { decoder as decoder_2, encoder as encoder_2 } from "./Value.js";
import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder } from "../OntologyAnnotation.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { encode } from "../IDTable.js";
import { object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Decode_uri } from "../Decode.js";

export const ROCrate_encoder = encoder;

export const ROCrate_decoder = decoder(FactorValue_createAsPV);

export function ISAJson_genID(fv) {
    return genID(fv);
}

export function ISAJson_encoder(idMap, fv) {
    const f = (fv_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ISAJson_genID(fv_1)), tryInclude("category", (value_2) => encoder_1(idMap, value_2), fv_1.Category), tryInclude("value", (value_3) => encoder_2(idMap, value_3), fv_1.Value), tryInclude("unit", (oa) => OntologyAnnotation_ISAJson_encoder(idMap, oa), fv_1.Unit)]));
        return {
            Encode(helpers_1) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_1)], values);
                return helpers_1.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(ISAJson_genID, f, fv, idMap);
    }
    else {
        return f(fv);
    }
}

export const ISAJson_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3;
    return new FactorValue((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("category", decoder_1)), (objectArg_2 = get$.Optional, objectArg_2.Field("value", decoder_2)), (objectArg_3 = get$.Optional, objectArg_3.Field("unit", OntologyAnnotation_ISAJson_decoder)));
});

