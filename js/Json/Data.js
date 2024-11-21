import { singleton, ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "./Encode.js";
import { ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_1, ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_1 } from "./DataFile.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_2, decoder as decoder_1, encoder as encoder_1 } from "./Comment.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { resizeArray, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Data } from "../Core/Data.js";
import { Decode_objectNoAdditionalProperties, Decode_uri } from "./Decode.js";
import { decodeString, encodeString } from "./StringTable.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { URIModule_toString } from "../Core/URI.js";
import { list as list_1 } from "../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { context_jsonvalue } from "./context/rocrate/isa_data_context.js";
import { encode } from "./IDTable.js";

export function encoder(d) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), d.ID), tryInclude("name", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), d.Name), tryInclude("dataType", ISAJson_encoder_1, d.DataType), tryInclude("format", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), d.Format), tryInclude("selectorFormat", (value_6) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_6);
        },
    }), d.SelectorFormat), tryIncludeSeq("comments", encoder_1, d.Comments)]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5;
    return new Data(unwrap((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("name", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("dataType", ISAJson_decoder_1))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("format", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("selectorFormat", Decode_uri))), unwrap((arg_11 = resizeArray(decoder_1), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", arg_11)))));
});

export function compressedEncoder(stringTable, d) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("i", (s) => encodeString(stringTable, s), d.ID), tryInclude("n", (s_1) => encodeString(stringTable, s_1), d.Name), tryInclude("d", ISAJson_encoder_1, d.DataType), tryInclude("f", (s_2) => encodeString(stringTable, s_2), d.Format), tryInclude("s", (s_3) => encodeString(stringTable, s_3), d.SelectorFormat), tryIncludeSeq("c", encoder_1, d.Comments)]));
    return {
        Encode(helpers) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers)], values);
            return helpers.encodeObject(arg);
        },
    };
}

export function compressedDecoder(stringTable) {
    return object((get$) => {
        let arg_1, objectArg, arg_3, objectArg_1, objectArg_2, arg_7, objectArg_3, arg_9, objectArg_4, arg_11, objectArg_5;
        return new Data(unwrap((arg_1 = decodeString(stringTable), (objectArg = get$.Optional, objectArg.Field("i", arg_1)))), unwrap((arg_3 = decodeString(stringTable), (objectArg_1 = get$.Optional, objectArg_1.Field("n", arg_3)))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("d", ISAJson_decoder_1))), unwrap((arg_7 = decodeString(stringTable), (objectArg_3 = get$.Optional, objectArg_3.Field("f", arg_7)))), unwrap((arg_9 = decodeString(stringTable), (objectArg_4 = get$.Optional, objectArg_4.Field("s", arg_9)))), unwrap((arg_11 = resizeArray(decoder_1), (objectArg_5 = get$.Optional, objectArg_5.Field("c", arg_11)))));
    });
}

export function ROCrate_genID(d) {
    const matchValue = d.ID;
    if (matchValue == null) {
        const matchValue_1 = d.Name;
        if (matchValue_1 == null) {
            return "#EmptyData";
        }
        else {
            return replace(matchValue_1, " ", "_");
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
            return helpers_1.encodeString("Data");
        },
    }))], tryInclude("name", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.Name), tryInclude("type", ROCrate_encoder_1, oa.DataType), tryInclude("encodingFormat", (value_5) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    }), oa.Format), tryInclude("usageInfo", (value_7) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_7);
        },
    }), oa.SelectorFormat), tryIncludeSeq("comments", ROCrate_encoder_2, oa.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_5) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5;
    return new Data(unwrap((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("name", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("type", ROCrate_decoder_1))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("encodingFormat", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("usageInfo", Decode_uri))), unwrap((arg_11 = resizeArray(ROCrate_decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", arg_11)))));
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
        }), oa_1.Name), tryInclude("type", ISAJson_encoder_1, oa_1.DataType), tryIncludeSeq("comments", (comment) => ISAJson_encoder_2(idMap, comment), oa_1.Comments)]));
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

export const ISAJson_allowedFields = ofArray(["@id", "name", "type", "comments", "@type", "@context"]);

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let objectArg, objectArg_1, objectArg_2, arg_7, objectArg_3;
    return new Data(unwrap((objectArg = get$.Optional, objectArg.Field("@id", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("name", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("type", ISAJson_decoder_1))), undefined, undefined, unwrap((arg_7 = resizeArray(ISAJson_decoder_2), (objectArg_3 = get$.Optional, objectArg_3.Field("comments", arg_7)))));
});

