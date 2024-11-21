import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "./Encode.js";
import { OntologyAnnotation_ROCrate_decoderDefinedTerm, OntologyAnnotation_ROCrate_encoderDefinedTerm, OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./OntologyAnnotation.js";
import { ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoderDisambiguatingDescription, ROCrate_encoderDisambiguatingDescription, decoder as decoder_2, encoder as encoder_1 } from "./Comment.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { resizeArray, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Publication } from "../Core/Publication.js";
import { Decode_noAdditionalProperties, Decode_uri } from "./Decode.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { ROCrate_decodeAuthorListString, ROCrate_encodeAuthorListString } from "./Person.js";
import { context_jsonvalue } from "./context/rocrate/isa_publication_context.js";

export function encoder(oa) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("pubMedID", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), oa.PubMedID), tryInclude("doi", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), oa.DOI), tryInclude("authorList", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), oa.Authors), tryInclude("title", (value_6) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_6);
        },
    }), oa.Title), tryInclude("status", OntologyAnnotation_encoder, oa.Status), tryIncludeSeq("comments", encoder_1, oa.Comments)]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5;
    return new Publication(unwrap((objectArg = get$.Optional, objectArg.Field("pubMedID", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("doi", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("authorList", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("title", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("status", OntologyAnnotation_decoder))), unwrap((arg_11 = resizeArray(decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", arg_11)))));
});

export function ROCrate_genID(p) {
    const matchValue = p.DOI;
    if (matchValue == null) {
        const matchValue_1 = p.PubMedID;
        if (matchValue_1 == null) {
            const matchValue_2 = p.Title;
            if (matchValue_2 == null) {
                return "#EmptyPublication";
            }
            else {
                return "#Pub_" + replace(matchValue_2, " ", "_");
            }
        }
        else {
            return matchValue_1;
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
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("Publication");
        },
    }], tryInclude("pubMedID", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.PubMedID), tryInclude("doi", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), oa.DOI), tryInclude("authorList", ROCrate_encodeAuthorListString, oa.Authors), tryInclude("title", (value_6) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_6);
        },
    }), oa.Title), tryInclude("status", OntologyAnnotation_ROCrate_encoderDefinedTerm, oa.Status), tryIncludeSeq("comments", ROCrate_encoderDisambiguatingDescription, oa.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_5) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5;
    return new Publication(unwrap((objectArg = get$.Optional, objectArg.Field("pubMedID", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("doi", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("authorList", ROCrate_decodeAuthorListString))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("title", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("status", OntologyAnnotation_ROCrate_decoderDefinedTerm))), unwrap((arg_11 = resizeArray(ROCrate_decoderDisambiguatingDescription), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", arg_11)))));
});

export function ISAJson_encoder(idMap, oa) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("pubMedID", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), oa.PubMedID), tryInclude("doi", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), oa.DOI), tryInclude("authorList", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), oa.Authors), tryInclude("title", (value_6) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_6);
        },
    }), oa.Title), tryInclude("status", OntologyAnnotation_encoder, oa.Status), tryIncludeSeq("comments", (comment) => ISAJson_encoder_1(idMap, comment), oa.Comments)]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ISAJson_allowedFields = ofArray(["pubMedID", "doi", "authorList", "title", "status", "comments"]);

export const ISAJson_decoder = Decode_noAdditionalProperties(ISAJson_allowedFields, decoder);

