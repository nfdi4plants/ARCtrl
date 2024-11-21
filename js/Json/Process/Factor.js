import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder, OntologyAnnotation_ROCrate_genID } from "../OntologyAnnotation.js";
import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { defaultArg, map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "../Encode.js";
import { ISAJson_decoder, ISAJson_encoder } from "../Comment.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { encode } from "../IDTable.js";
import { resizeArray, string, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Factor } from "../../Core/Process/Factor.js";

export function genID(f) {
    const matchValue = f.Name;
    if (matchValue == null) {
        const matchValue_1 = f.FactorType;
        if (matchValue_1 == null) {
            return "#EmptyFactor";
        }
        else {
            return `#Factor/${OntologyAnnotation_ROCrate_genID(matchValue_1)}`;
        }
    }
    else {
        return `#Factor/${matchValue}`;
    }
}

export function encoder(idMap, value) {
    const f_1 = (value_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value_2) => ({
            Encode(helpers) {
                return helpers.encodeString(value_2);
            },
        }), genID(value_1)), tryInclude("factorName", (value_4) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_4);
            },
        }), value_1.Name), tryInclude("factorType", (oa) => OntologyAnnotation_ISAJson_encoder(idMap, oa), value_1.FactorType), tryIncludeSeq("comments", (comment) => ISAJson_encoder(idMap, comment), defaultArg(value_1.Comments, []))]));
        return {
            Encode(helpers_2) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
                return helpers_2.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(genID, f_1, value, idMap);
    }
    else {
        return f_1(value);
    }
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, arg_5, objectArg_2;
    return new Factor((objectArg = get$.Optional, objectArg.Field("factorName", string)), (objectArg_1 = get$.Optional, objectArg_1.Field("factorType", OntologyAnnotation_ISAJson_decoder)), (arg_5 = resizeArray(ISAJson_decoder), (objectArg_2 = get$.Optional, objectArg_2.Field("comments", arg_5))));
});

