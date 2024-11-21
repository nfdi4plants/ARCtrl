import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "./Encode.js";
import { ISAJson_encoder as ISAJson_encoder_1, decoder as decoder_1, encoder as encoder_1 } from "./Comment.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { OntologySourceReference } from "../Core/OntologySourceReference.js";
import { Decode_resizeArray, Decode_uri } from "./Decode.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { context_jsonvalue } from "./context/rocrate/isa_ontology_source_reference_context.js";

export function encoder(osr) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("description", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), osr.Description), tryInclude("file", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), osr.File), tryInclude("name", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), osr.Name), tryInclude("version", (value_6) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_6);
        },
    }), osr.Version), tryIncludeSeq("comments", encoder_1, osr.Comments)]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, arg_9, objectArg_4;
    return new OntologySourceReference(unwrap((objectArg = get$.Optional, objectArg.Field("description", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("file", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("name", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("version", string))), unwrap((arg_9 = Decode_resizeArray(decoder_1), (objectArg_4 = get$.Optional, objectArg_4.Field("comments", arg_9)))));
});

export function ROCrate_genID(o) {
    const matchValue = o.File;
    if (matchValue == null) {
        const matchValue_1 = o.Name;
        if (matchValue_1 == null) {
            return "#DummyOntologySourceRef";
        }
        else {
            return "#OntologySourceRef_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return matchValue;
    }
}

export function ROCrate_encoder(osr) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(osr), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("OntologySourceReference");
        },
    }], tryInclude("description", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), osr.Description), tryInclude("file", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), osr.File), tryInclude("name", (value_6) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_6);
        },
    }), osr.Name), tryInclude("version", (value_8) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_8);
        },
    }), osr.Version), tryIncludeSeq("comments", encoder_1, osr.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_6) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, arg_9, objectArg_4;
    return new OntologySourceReference(unwrap((objectArg = get$.Optional, objectArg.Field("description", Decode_uri))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("file", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("name", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("version", string))), unwrap((arg_9 = Decode_resizeArray(decoder_1), (objectArg_4 = get$.Optional, objectArg_4.Field("comments", arg_9)))));
});

export function ISAJson_encoder(idMap, osr) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("description", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), osr.Description), tryInclude("file", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), osr.File), tryInclude("name", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), osr.Name), tryInclude("version", (value_6) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_6);
        },
    }), osr.Version), tryIncludeSeq("comments", (comment) => ISAJson_encoder_1(idMap, comment), osr.Comments)]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ISAJson_decoder = decoder;

