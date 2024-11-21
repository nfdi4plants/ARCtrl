import { getMaterials, getSamples, getSources } from "../../Core/Process/ProcessSequence.js";
import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeList } from "../../Json/Encode.js";
import { ISAJson_encoder } from "../../Json/Process/Source.js";
import { ISAJson_encoder as ISAJson_encoder_1 } from "../../Json/Process/Sample.js";
import { ISAJson_encoder as ISAJson_encoder_2 } from "../../Json/Process/Material.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";

export function encoder(idMap, ps) {
    const source = getSources(ps);
    const samples = getSamples(ps);
    const materials = getMaterials(ps);
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryIncludeList("sources", (oa) => ISAJson_encoder(idMap, oa), source), tryIncludeList("samples", (oa_1) => ISAJson_encoder_1(idMap, oa_1), samples), tryIncludeList("otherMaterials", (c) => ISAJson_encoder_2(idMap, c), materials)]));
    return {
        Encode(helpers) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers)], values);
            return helpers.encodeObject(arg);
        },
    };
}

