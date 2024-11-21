import { replace } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { ofArray, singleton, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { list as list_1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { tryIncludeListOpt, tryInclude } from "../Encode.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_1 } from "./MaterialType.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_2 } from "./MaterialAttributeValue.js";
import { context_jsonvalue } from "../context/rocrate/isa_material_context.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { list as list_2, string, unit, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Decode_objectNoAdditionalProperties, Decode_uri } from "../Decode.js";
import { Material } from "../../Core/Process/Material.js";
import { encode } from "../IDTable.js";

export function ROCrate_genID(m) {
    const matchValue = m.ID;
    if (matchValue == null) {
        const matchValue_1 = m.Name;
        if (matchValue_1 == null) {
            return "#EmptyMaterial";
        }
        else {
            return "#Material_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return matchValue;
    }
}

export function ROCrate_encoder(oa) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", list_1(singleton({
        Encode(helpers_1) {
            return helpers_1.encodeString("Material");
        },
    }))], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("Material");
        },
    }], tryInclude("name", (value_3) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_3);
        },
    }), oa.Name), tryInclude("type", ROCrate_encoder_1, oa.MaterialType), tryIncludeListOpt("characteristics", ROCrate_encoder_2, oa.Characteristics), tryIncludeListOpt("derivesFrom", ROCrate_encoder, oa.DerivesFrom), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = (() => {
    const decode = () => object((get$) => {
        let objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5, arg_13, objectArg_6;
        let matchValue;
        const objectArg = get$.Optional;
        matchValue = objectArg.Field("additionalType", Decode_uri);
        let matchResult;
        if (matchValue == null) {
            matchResult = 0;
        }
        else if (matchValue === "Material") {
            matchResult = 0;
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 1: {
                const objectArg_1 = get$.Required;
                objectArg_1.Field("FailBecauseNotSample", unit);
                break;
            }
        }
        return new Material((objectArg_2 = get$.Optional, objectArg_2.Field("@id", Decode_uri)), (objectArg_3 = get$.Optional, objectArg_3.Field("name", string)), (objectArg_4 = get$.Optional, objectArg_4.Field("type", ROCrate_decoder_1)), (arg_11 = list_2(ROCrate_decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("characteristics", arg_11))), (arg_13 = list_2(decode()), (objectArg_6 = get$.Optional, objectArg_6.Field("derivesFrom", arg_13))));
    });
    return decode();
})();

export function ISAJson_encoder(idMap, c) {
    const f = (oa) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(oa)), tryInclude("name", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), oa.Name), tryInclude("type", ISAJson_encoder_1, oa.MaterialType), tryIncludeListOpt("characteristics", (oa_1) => ISAJson_encoder_2(idMap, oa_1), oa.Characteristics), tryIncludeListOpt("derivesFrom", (c_1) => ISAJson_encoder(idMap, c_1), oa.DerivesFrom)]));
        return {
            Encode(helpers_2) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
                return helpers_2.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(ROCrate_genID, f, c, idMap);
    }
    else {
        return f(c);
    }
}

export const ISAJson_allowedFields = ofArray(["@id", "@type", "name", "type", "characteristics", "derivesFrom", "@context"]);

export const ISAJson_decoder = (() => {
    const decode = () => Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
        let objectArg, objectArg_1, objectArg_2, arg_7, objectArg_3, arg_9, objectArg_4;
        return new Material((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("type", ISAJson_decoder_1)), (arg_7 = list_2(ISAJson_decoder_2), (objectArg_3 = get$.Optional, objectArg_3.Field("characteristics", arg_7))), (arg_9 = list_2(decode()), (objectArg_4 = get$.Optional, objectArg_4.Field("derivesFrom", arg_9))));
    });
    return decode();
})();

