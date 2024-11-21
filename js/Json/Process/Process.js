import { replace } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { URIModule_toString } from "../../Core/URI.js";
import { ofArray, singleton, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { list as list_1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { tryIncludeListOpt, tryInclude } from "../Encode.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_1 } from "./Protocol.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_2 } from "./ProcessParameterValue.js";
import { ROCrate_decodeAuthorListString, ROCrate_encodeAuthorListString } from "../Person.js";
import { ISAJson_decoder as ISAJson_decoder_3, ISAJson_encoder as ISAJson_encoder_3, ROCrate_decoder as ROCrate_decoder_3, ROCrate_encoder as ROCrate_encoder_3 } from "./ProcessInput.js";
import { ISAJson_decoder as ISAJson_decoder_4, ISAJson_encoder as ISAJson_encoder_4, ROCrate_decoder as ROCrate_decoder_4, ROCrate_encoder as ROCrate_encoder_4 } from "./ProcessOutput.js";
import { ISAJson_decoder as ISAJson_decoder_5, ISAJson_encoder as ISAJson_encoder_5, ROCrate_decoder as ROCrate_decoder_5, ROCrate_encoder as ROCrate_encoder_5 } from "../Comment.js";
import { context_jsonvalue } from "../context/rocrate/isa_process_context.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { list as list_2, string, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Decode_uri } from "../Decode.js";
import { Process } from "../../Core/Process/Process.js";
import { encode } from "../IDTable.js";

export function ROCrate_genID(p) {
    const matchValue = p.ID;
    if (matchValue == null) {
        const matchValue_1 = p.Name;
        if (matchValue_1 == null) {
            return "#EmptyProcess";
        }
        else {
            return "#Process_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function ROCrate_encoder(studyName, assayName, oa) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", list_1(singleton({
        Encode(helpers_1) {
            return helpers_1.encodeString("Process");
        },
    }))], tryInclude("name", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.Name), tryInclude("executesProtocol", (oa_1) => ROCrate_encoder_1(studyName, assayName, oa.Name, oa_1), oa.ExecutesProtocol), tryIncludeListOpt("parameterValues", ROCrate_encoder_2, oa.ParameterValues), tryInclude("performer", ROCrate_encodeAuthorListString, oa.Performer), tryInclude("date", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), oa.Date), tryIncludeListOpt("inputs", ROCrate_encoder_3, oa.Inputs), tryIncludeListOpt("outputs", ROCrate_encoder_4, oa.Outputs), tryIncludeListOpt("comments", ROCrate_encoder_5, oa.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, arg_7, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8;
    return new Process((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("executesProtocol", ROCrate_decoder_1)), (arg_7 = list_2(ROCrate_decoder_2), (objectArg_3 = get$.Optional, objectArg_3.Field("parameterValues", arg_7))), (objectArg_4 = get$.Optional, objectArg_4.Field("performer", ROCrate_decodeAuthorListString)), (objectArg_5 = get$.Optional, objectArg_5.Field("date", string)), undefined, undefined, (arg_13 = list_2(ROCrate_decoder_3), (objectArg_6 = get$.Optional, objectArg_6.Field("inputs", arg_13))), (arg_15 = list_2(ROCrate_decoder_4), (objectArg_7 = get$.Optional, objectArg_7.Field("outputs", arg_15))), (arg_17 = list_2(ROCrate_decoder_5), (objectArg_8 = get$.Optional, objectArg_8.Field("comments", arg_17))));
});

export function ISAJson_encoder(studyName, assayName, idMap, oa) {
    const f = (oa_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(oa_1)), tryInclude("name", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), oa_1.Name), tryInclude("executesProtocol", (oa_2) => ISAJson_encoder_1(studyName, assayName, oa_1.Name, idMap, oa_2), oa_1.ExecutesProtocol), tryIncludeListOpt("parameterValues", (oa_3) => ISAJson_encoder_2(idMap, oa_3), oa_1.ParameterValues), tryInclude("performer", (value_4) => ({
            Encode(helpers_2) {
                return helpers_2.encodeString(value_4);
            },
        }), oa_1.Performer), tryInclude("date", (value_6) => ({
            Encode(helpers_3) {
                return helpers_3.encodeString(value_6);
            },
        }), oa_1.Date), tryInclude("previousProcess", (oa_4) => ISAJson_encoder(studyName, assayName, idMap, oa_4), oa_1.PreviousProcess), tryInclude("nextProcess", (oa_5) => ISAJson_encoder(studyName, assayName, idMap, oa_5), oa_1.NextProcess), tryIncludeListOpt("inputs", (value_8) => ISAJson_encoder_3(idMap, value_8), oa_1.Inputs), tryIncludeListOpt("outputs", (value_9) => ISAJson_encoder_4(idMap, value_9), oa_1.Outputs), tryIncludeListOpt("comments", (comment) => ISAJson_encoder_5(idMap, comment), oa_1.Comments)]));
        return {
            Encode(helpers_4) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
                return helpers_4.encodeObject(arg);
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

export const ISAJson_decoder = (() => {
    const decode = () => object((get$) => {
        let objectArg, objectArg_1, objectArg_2, arg_7, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, arg_19, objectArg_9, arg_21, objectArg_10;
        return new Process((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("executesProtocol", ISAJson_decoder_1)), (arg_7 = list_2(ISAJson_decoder_2), (objectArg_3 = get$.Optional, objectArg_3.Field("parameterValues", arg_7))), (objectArg_4 = get$.Optional, objectArg_4.Field("performer", string)), (objectArg_5 = get$.Optional, objectArg_5.Field("date", string)), (arg_13 = decode(), (objectArg_6 = get$.Optional, objectArg_6.Field("previousProcess", arg_13))), (arg_15 = decode(), (objectArg_7 = get$.Optional, objectArg_7.Field("nextProcess", arg_15))), (arg_17 = list_2(ISAJson_decoder_3), (objectArg_8 = get$.Optional, objectArg_8.Field("inputs", arg_17))), (arg_19 = list_2(ISAJson_decoder_4), (objectArg_9 = get$.Optional, objectArg_9.Field("outputs", arg_19))), (arg_21 = list_2(ISAJson_decoder_5), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", arg_21))));
    });
    return decode();
})();

