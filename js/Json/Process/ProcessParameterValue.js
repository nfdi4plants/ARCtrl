import { decoder, encoder } from "../PropertyValue.js";
import { ProcessParameterValue } from "../../Core/Process/ProcessParameterValue.js";
import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryInclude } from "../Encode.js";
import { decoder as decoder_1, encoder as encoder_1 } from "./ProtocolParameter.js";
import { decoder as decoder_2, encoder as encoder_2 } from "./Value.js";
import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder } from "../OntologyAnnotation.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";

export const ROCrate_encoder = encoder;

export const ROCrate_decoder = decoder((alternateName, measurementMethod, description, category, value, unit) => ProcessParameterValue.createAsPV(alternateName, measurementMethod, description, category, value, unit));

export function ISAJson_genID(oa) {
    throw new Error("Not implemented");
}

export function ISAJson_encoder(idMap, oa) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("category", (value) => encoder_1(idMap, value), oa.Category), tryInclude("value", (value_1) => encoder_2(idMap, value_1), oa.Value), tryInclude("unit", (oa_1) => OntologyAnnotation_ISAJson_encoder(idMap, oa_1), oa.Unit)]));
    return {
        Encode(helpers) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers)], values);
            return helpers.encodeObject(arg);
        },
    };
}

export const ISAJson_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2;
    return new ProcessParameterValue((objectArg = get$.Optional, objectArg.Field("category", decoder_1)), (objectArg_1 = get$.Optional, objectArg_1.Field("value", decoder_2)), (objectArg_2 = get$.Optional, objectArg_2.Field("unit", OntologyAnnotation_ISAJson_decoder)));
});

