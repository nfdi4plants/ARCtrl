import { replace } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { append, empty, ofArray, singleton, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { defaultArg, map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { list as list_4 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { tryIncludeListOpt, tryInclude } from "../Encode.js";
import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder, OntologyAnnotation_ROCrate_decoderDefinedTerm, OntologyAnnotation_ROCrate_encoderDefinedTerm } from "../OntologyAnnotation.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_1 } from "./Component.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_2 } from "../Comment.js";
import { context_jsonvalue } from "../context/rocrate/isa_protocol_context.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { string, list as list_5, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Option_fromValueWithDefault } from "../../Core/Helper/Collections.js";
import { Decode_uri } from "../Decode.js";
import { Protocol } from "../../Core/Process/Protocol.js";
import { decoder, encoder } from "./ProtocolParameter.js";
import { encode } from "../IDTable.js";

export function ROCrate_genID(studyName, assayName, processName, p) {
    const matchValue = p.ID;
    let matchResult, id_1;
    if (matchValue != null) {
        if (matchValue !== "") {
            matchResult = 0;
            id_1 = matchValue;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return id_1;
        default: {
            const matchValue_1 = p.Uri;
            if (matchValue_1 == null) {
                const matchValue_2 = p.Name;
                if (matchValue_2 == null) {
                    let matchResult_1, an, pn, sn, pn_1, sn_1, pn_2;
                    if (studyName == null) {
                        if (assayName == null) {
                            if (processName != null) {
                                matchResult_1 = 2;
                                pn_2 = processName;
                            }
                            else {
                                matchResult_1 = 3;
                            }
                        }
                        else {
                            matchResult_1 = 3;
                        }
                    }
                    else if (assayName == null) {
                        if (processName != null) {
                            matchResult_1 = 1;
                            pn_1 = processName;
                            sn_1 = studyName;
                        }
                        else {
                            matchResult_1 = 3;
                        }
                    }
                    else if (processName != null) {
                        matchResult_1 = 0;
                        an = assayName;
                        pn = processName;
                        sn = studyName;
                    }
                    else {
                        matchResult_1 = 3;
                    }
                    switch (matchResult_1) {
                        case 0:
                            return (((("#Protocol_" + replace(sn, " ", "_")) + "_") + replace(an, " ", "_")) + "_") + replace(pn, " ", "_");
                        case 1:
                            return (("#Protocol_" + replace(sn_1, " ", "_")) + "_") + replace(pn_1, " ", "_");
                        case 2:
                            return "#Protocol_" + replace(pn_2, " ", "_");
                        default:
                            return "#EmptyProtocol";
                    }
                }
                else {
                    return "#Protocol_" + replace(matchValue_2, " ", "_");
                }
            }
            else {
                return matchValue_1;
            }
        }
    }
}

export function ROCrate_encoder(studyName, assayName, processName, oa) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(studyName, assayName, processName, oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", list_4(singleton({
        Encode(helpers_1) {
            return helpers_1.encodeString("Protocol");
        },
    }))], tryInclude("name", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.Name), tryInclude("protocolType", OntologyAnnotation_ROCrate_encoderDefinedTerm, oa.ProtocolType), tryInclude("description", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), oa.Description), tryInclude("uri", (value_6) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_6);
        },
    }), oa.Uri), tryInclude("version", (value_8) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_8);
        },
    }), oa.Version), tryIncludeListOpt("components", ROCrate_encoder_1, oa.Components), tryIncludeListOpt("comments", ROCrate_encoder_2, oa.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_6) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let list_3, list_1, arg_1, objectArg, arg_3, objectArg_1, arg_5, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7, objectArg_8, arg_19, objectArg_9;
    const components = Option_fromValueWithDefault(empty(), (list_3 = ((list_1 = defaultArg((arg_1 = list_5(ROCrate_decoder_1), (objectArg = get$.Optional, objectArg.Field("components", arg_1))), empty()), append(defaultArg((arg_3 = list_5(ROCrate_decoder_1), (objectArg_1 = get$.Optional, objectArg_1.Field("reagents", arg_3))), empty()), list_1))), append(defaultArg((arg_5 = list_5(ROCrate_decoder_1), (objectArg_2 = get$.Optional, objectArg_2.Field("computationalTools", arg_5))), empty()), list_3)));
    return new Protocol((objectArg_3 = get$.Optional, objectArg_3.Field("@id", Decode_uri)), (objectArg_4 = get$.Optional, objectArg_4.Field("name", string)), (objectArg_5 = get$.Optional, objectArg_5.Field("protocolType", OntologyAnnotation_ROCrate_decoderDefinedTerm)), (objectArg_6 = get$.Optional, objectArg_6.Field("description", string)), (objectArg_7 = get$.Optional, objectArg_7.Field("uri", Decode_uri)), (objectArg_8 = get$.Optional, objectArg_8.Field("version", string)), undefined, components, (arg_19 = list_5(ROCrate_decoder_2), (objectArg_9 = get$.Optional, objectArg_9.Field("comments", arg_19))));
});

export function ISAJson_encoder(studyName, assayName, processName, idMap, oa) {
    const f = (oa_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(studyName, assayName, processName, oa_1)), tryInclude("name", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), oa_1.Name), tryInclude("protocolType", (oa_2) => OntologyAnnotation_ISAJson_encoder(idMap, oa_2), oa_1.ProtocolType), tryInclude("description", (value_4) => ({
            Encode(helpers_2) {
                return helpers_2.encodeString(value_4);
            },
        }), oa_1.Description), tryInclude("uri", (value_6) => ({
            Encode(helpers_3) {
                return helpers_3.encodeString(value_6);
            },
        }), oa_1.Uri), tryInclude("version", (value_8) => ({
            Encode(helpers_4) {
                return helpers_4.encodeString(value_8);
            },
        }), oa_1.Version), tryIncludeListOpt("parameters", (value_10) => encoder(idMap, value_10), oa_1.Parameters), tryIncludeListOpt("components", (c) => ISAJson_encoder_1(idMap, c), oa_1.Components), tryIncludeListOpt("comments", (comment) => ISAJson_encoder_2(idMap, comment), oa_1.Comments)]));
        return {
            Encode(helpers_5) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_5)], values);
                return helpers_5.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode((p_1) => ROCrate_genID(studyName, assayName, processName, p_1), f, oa, idMap);
    }
    else {
        return f(oa);
    }
}

export const ISAJson_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8;
    return new Protocol((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("protocolType", OntologyAnnotation_ISAJson_decoder)), (objectArg_3 = get$.Optional, objectArg_3.Field("description", string)), (objectArg_4 = get$.Optional, objectArg_4.Field("uri", Decode_uri)), (objectArg_5 = get$.Optional, objectArg_5.Field("version", string)), (arg_13 = list_5(decoder), (objectArg_6 = get$.Optional, objectArg_6.Field("parameters", arg_13))), (arg_15 = list_5(ISAJson_decoder_1), (objectArg_7 = get$.Optional, objectArg_7.Field("components", arg_15))), (arg_17 = list_5(ISAJson_decoder_2), (objectArg_8 = get$.Optional, objectArg_8.Field("comments", arg_17))));
});

