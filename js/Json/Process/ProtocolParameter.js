import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder, OntologyAnnotation_ROCrate_genID } from "../OntologyAnnotation.js";
import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryInclude } from "../Encode.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { encode } from "../IDTable.js";
import { object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { ProtocolParameter } from "../../Core/Process/ProtocolParameter.js";

export function genID(p) {
    const matchValue = p.ParameterName;
    if (matchValue == null) {
        return "#EmptyProtocolParameter";
    }
    else {
        return `#ProtocolParameter/${OntologyAnnotation_ROCrate_genID(matchValue)}`;
    }
}

export function encoder(idMap, value) {
    const f = (value_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value_2) => ({
            Encode(helpers) {
                return helpers.encodeString(value_2);
            },
        }), genID(value_1)), tryInclude("parameterName", (oa) => OntologyAnnotation_ISAJson_encoder(idMap, oa), value_1.ParameterName)]));
        return {
            Encode(helpers_1) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_1)], values);
                return helpers_1.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(genID, f, value, idMap);
    }
    else {
        return f(value);
    }
}

export const decoder = object((get$) => {
    let objectArg;
    return new ProtocolParameter(undefined, (objectArg = get$.Optional, objectArg.Field("parameterName", OntologyAnnotation_ISAJson_decoder)));
});

