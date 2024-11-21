import { replace } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { URIModule_toString } from "../../Core/URI.js";
import { tail, head, isEmpty, ofArray, singleton, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { list as list_1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { tryIncludeListOpt, tryInclude } from "../Encode.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_1 } from "./MaterialAttributeValue.js";
import { context_jsonvalue } from "../context/rocrate/isa_source_context.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { string, list as list_2, unit, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Decode_objectNoAdditionalProperties, Decode_uri } from "../Decode.js";
import { Source } from "../../Core/Process/Source.js";
import { encode } from "../IDTable.js";

export function ROCrate_genID(s) {
    const matchValue = s.ID;
    if (matchValue == null) {
        const matchValue_1 = s.Name;
        if (matchValue_1 == null) {
            return "#EmptySource";
        }
        else {
            return "#Source_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
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
            return helpers_1.encodeString("Source");
        },
    }))], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("Source");
        },
    }], tryInclude("name", (value_3) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_3);
        },
    }), oa.Name), tryIncludeListOpt("characteristics", ROCrate_encoder_1, oa.Characteristics), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg_4, objectArg_5, arg_13, objectArg_6;
    let matchValue;
    const objectArg = get$.Optional;
    matchValue = objectArg.Field("additionalType", Decode_uri);
    let matchResult;
    if (matchValue == null) {
        matchResult = 0;
    }
    else if (matchValue === "Source") {
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
    let matchValue_1;
    const arg_5 = list_2(string);
    const objectArg_2 = get$.Optional;
    matchValue_1 = objectArg_2.Field("@type", arg_5);
    let matchResult_1;
    if (matchValue_1 == null) {
        matchResult_1 = 0;
    }
    else if (!isEmpty(matchValue_1)) {
        if (head(matchValue_1) === "Source") {
            if (isEmpty(tail(matchValue_1))) {
                matchResult_1 = 0;
            }
            else {
                matchResult_1 = 1;
            }
        }
        else {
            matchResult_1 = 1;
        }
    }
    else {
        matchResult_1 = 1;
    }
    switch (matchResult_1) {
        case 1: {
            const objectArg_3 = get$.Required;
            objectArg_3.Field("FailBecauseNotSample", unit);
            break;
        }
    }
    return new Source((objectArg_4 = get$.Optional, objectArg_4.Field("@id", Decode_uri)), (objectArg_5 = get$.Optional, objectArg_5.Field("name", string)), (arg_13 = list_2(ROCrate_decoder_1), (objectArg_6 = get$.Optional, objectArg_6.Field("characteristics", arg_13))));
});

export function ISAJson_encoder(idMap, oa) {
    const f = (oa_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(oa_1)), tryInclude("name", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), oa_1.Name), tryIncludeListOpt("characteristics", (oa_2) => ISAJson_encoder_1(idMap, oa_2), oa_1.Characteristics)]));
        return {
            Encode(helpers_2) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
                return helpers_2.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(ROCrate_genID, f, oa, idMap);
    }
    else {
        return f(oa);
    }
}

export const ISAJson_allowedFields = ofArray(["@id", "name", "characteristics", "@type", "@context"]);

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let objectArg, objectArg_1, arg_5, objectArg_2;
    return new Source((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (arg_5 = list_2(ISAJson_decoder_1), (objectArg_2 = get$.Optional, objectArg_2.Field("characteristics", arg_5))));
});

