import { replace } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { tail, head, isEmpty, ofArray, singleton, choose, append, empty, map } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_1 } from "./MaterialAttributeValue.js";
import { map as map_1, defaultArg } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_2 } from "./FactorValue.js";
import { list as list_5 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { tryIncludeListOpt, tryIncludeList, tryInclude } from "../Encode.js";
import { context_jsonvalue } from "../context/rocrate/isa_sample_context.js";
import { map as map_2 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { list as list_6, unit, object, string } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library-js.4.22.0/Result.js";
import { Decode_objectNoAdditionalProperties, Decode_uri } from "../Decode.js";
import { Option_fromValueWithDefault } from "../../Core/Helper/Collections.js";
import { ISAJson_decoder as ISAJson_decoder_3, ISAJson_encoder as ISAJson_encoder_3, ROCrate_decoder as ROCrate_decoder_3 } from "./Source.js";
import { Sample } from "../../Core/Process/Sample.js";
import { encode } from "../IDTable.js";

export function ROCrate_genID(s) {
    const matchValue = s.ID;
    if (matchValue == null) {
        const matchValue_1 = s.Name;
        if (matchValue_1 == null) {
            return "#EmptySample";
        }
        else {
            return "#Sample_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return matchValue;
    }
}

export function ROCrate_encoder(oa) {
    let value_2;
    let additionalProperties;
    const list_3 = map(ROCrate_encoder_1, defaultArg(oa.Characteristics, empty()));
    additionalProperties = append(map(ROCrate_encoder_2, defaultArg(oa.FactorValues, empty())), list_3);
    const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value_2 = ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value_2);
        },
    })], ["@type", list_5(singleton({
        Encode(helpers_1) {
            return helpers_1.encodeString("Sample");
        },
    }))], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("Sample");
        },
    }], tryInclude("name", (value_5) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    }), oa.Name), tryIncludeList("additionalProperties", (x) => x, additionalProperties), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_4) {
            const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ROCrate_additionalPropertyDecoder = {
    Decode(s, json) {
        let matchValue;
        if ((s.hasProperty("additionalType", json) ? ((matchValue = string.Decode(s, s.getProperty("additionalType", json)), (matchValue.tag === 0) ? matchValue.fields[0] : "")) : "") === "FactorValue") {
            const matchValue_1 = ROCrate_decoder_1.Decode(s, json);
            return (matchValue_1.tag === 1) ? (new FSharpResult$2(1, [matchValue_1.fields[0]])) : (new FSharpResult$2(0, [[undefined, matchValue_1.fields[0]]]));
        }
        else {
            const matchValue_2 = ROCrate_decoder_2.Decode(s, json);
            return (matchValue_2.tag === 1) ? (new FSharpResult$2(1, [matchValue_2.fields[0]])) : (new FSharpResult$2(0, [[matchValue_2.fields[0], undefined]]));
        }
    },
};

export const ROCrate_decoder = object((get$) => {
    let objectArg_5, objectArg_6, arg_15, objectArg_7;
    let matchValue;
    const objectArg = get$.Optional;
    matchValue = objectArg.Field("additionalType", Decode_uri);
    let matchResult;
    if (matchValue == null) {
        matchResult = 0;
    }
    else if (matchValue === "Sample") {
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
    const arg_5 = list_6(string);
    const objectArg_2 = get$.Optional;
    matchValue_1 = objectArg_2.Field("@type", arg_5);
    let matchResult_1;
    if (matchValue_1 == null) {
        matchResult_1 = 0;
    }
    else if (!isEmpty(matchValue_1)) {
        if (head(matchValue_1) === "Sample") {
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
    let additionalProperties;
    const arg_9 = list_6(ROCrate_additionalPropertyDecoder);
    const objectArg_4 = get$.Optional;
    additionalProperties = objectArg_4.Field("additionalProperties", arg_9);
    let patternInput;
    if (additionalProperties != null) {
        const additionalProperties_1 = additionalProperties;
        patternInput = [Option_fromValueWithDefault(empty(), choose((tuple) => tuple[0], additionalProperties_1)), Option_fromValueWithDefault(empty(), choose((tuple_1) => tuple_1[1], additionalProperties_1))];
    }
    else {
        patternInput = [undefined, undefined];
    }
    return new Sample((objectArg_5 = get$.Optional, objectArg_5.Field("@id", Decode_uri)), (objectArg_6 = get$.Optional, objectArg_6.Field("name", string)), patternInput[0], patternInput[1], (arg_15 = list_6(ROCrate_decoder_3), (objectArg_7 = get$.Optional, objectArg_7.Field("derivesFrom", arg_15))));
});

export function ISAJson_encoder(idMap, oa) {
    const f = (oa_1) => {
        const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(oa_1)), tryInclude("name", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), oa_1.Name), tryIncludeListOpt("characteristics", (oa_2) => ISAJson_encoder_1(idMap, oa_2), oa_1.Characteristics), tryIncludeListOpt("factorValues", (fv) => ISAJson_encoder_2(idMap, fv), oa_1.FactorValues), tryIncludeListOpt("derivesFrom", (oa_3) => ISAJson_encoder_3(idMap, oa_3), oa_1.DerivesFrom)]));
        return {
            Encode(helpers_2) {
                const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
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

export const ISAJson_allowedFields = ofArray(["@id", "name", "characteristics", "factorValues", "derivesFrom", "@type", "@context"]);

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let objectArg, objectArg_1, arg_5, objectArg_2, arg_7, objectArg_3, arg_9, objectArg_4;
    return new Sample((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (arg_5 = list_6(ISAJson_decoder_1), (objectArg_2 = get$.Optional, objectArg_2.Field("characteristics", arg_5))), (arg_7 = list_6(ISAJson_decoder_2), (objectArg_3 = get$.Optional, objectArg_3.Field("factorValues", arg_7))), (arg_9 = list_6(ISAJson_decoder_3), (objectArg_4 = get$.Optional, objectArg_4.Field("derivesFrom", arg_9))));
});

