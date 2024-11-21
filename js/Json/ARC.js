import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryInclude } from "./Encode.js";
import { ROCrate_decoder, ROCrate_encoder } from "./Investigation.js";
import { context_jsonvalue, conformsTo_jsonvalue } from "./context/rocrate/rocrate_context.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";

export function encoder(isa) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@type", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), "CreativeWork"), tryInclude("@id", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), "ro-crate-metadata.json"), tryInclude("about", ROCrate_encoder, isa), ["conformsTo", conformsTo_jsonvalue], ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_2) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
            return helpers_2.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    const objectArg = get$.Optional;
    return objectArg.Field("about", ROCrate_decoder);
});

