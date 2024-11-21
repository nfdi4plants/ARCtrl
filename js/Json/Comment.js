import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { value as value_6, unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryInclude } from "./Encode.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { map as map_2, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Comment$ } from "../Core/Comment.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { context_jsonvalue } from "./context/rocrate/isa_comment_context.js";
import { toString } from "../fable_modules/fable-library-js.4.22.0/Types.js";
import { encode } from "./IDTable.js";

export function encoder(comment) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("name", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), comment.Name), tryInclude("value", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), comment.Value)]));
    return {
        Encode(helpers_2) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
            return helpers_2.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1;
    return new Comment$(unwrap((objectArg = get$.Optional, objectArg.Field("name", string))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("value", string))));
});

export function ROCrate_genID(c) {
    const matchValue = c.Name;
    if (matchValue == null) {
        return "#EmptyComment";
    }
    else {
        const n = matchValue;
        const v = (c.Value != null) ? ("_" + replace(value_6(c.Value), " ", "_")) : "";
        return ("#Comment_" + replace(n, " ", "_")) + v;
    }
}

export function ROCrate_encoder(comment) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(comment), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("Comment");
        },
    }], tryInclude("name", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), comment.Name), tryInclude("value", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), comment.Value), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_4) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, objectArg_1;
    return new Comment$(unwrap((objectArg = get$.Optional, objectArg.Field("name", string))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("value", string))));
});

export function ROCrate_encoderDisambiguatingDescription(comment) {
    const value = toString(comment);
    return {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    };
}

export const ROCrate_decoderDisambiguatingDescription = map_2((s) => Comment$.fromString(s), string);

export function ISAJson_encoder(idMap, comment) {
    const f = (comment_1) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(comment_1)), tryInclude("name", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), comment_1.Name), tryInclude("value", (value_4) => ({
            Encode(helpers_2) {
                return helpers_2.encodeString(value_4);
            },
        }), comment_1.Value)]));
        return {
            Encode(helpers_3) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_3)], values);
                return helpers_3.encodeObject(arg);
            },
        };
    };
    if (idMap == null) {
        return f(comment);
    }
    else {
        return encode(ROCrate_genID, f, comment, idMap);
    }
}

export const ISAJson_decoder = decoder;

