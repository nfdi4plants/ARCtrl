import { resizeArray, object, string, float, int, map, oneOf } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { int32ToString } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { choose, ofArray } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { unwrap, map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "./Encode.js";
import { ROCrate_decoderDisambiguatingDescription, ROCrate_encoderDisambiguatingDescription, decoder, encoder } from "./Comment.js";
import { filter, map as map_2 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { OntologyAnnotation } from "../Core/OntologyAnnotation.js";
import { decodeString, encodeString } from "./StringTable.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { URIModule_toString } from "../Core/URI.js";
import { context_jsonvalue } from "./context/rocrate/isa_ontology_annotation_context.js";
import { context_jsonvalue as context_jsonvalue_1 } from "./context/rocrate/property_value_context.js";
import { orderName } from "../Core/Process/ColumnIndex.js";
import { encode } from "./IDTable.js";

export const AnnotationValue_decoder = oneOf(ofArray([map(int32ToString, int), map((value_1) => value_1.toString(), float), string]));

export function OntologyAnnotation_encoder(oa) {
    const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("annotationValue", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), oa.Name), tryInclude("termSource", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), oa.TermSourceREF), tryInclude("termAccession", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), oa.TermAccessionNumber), tryIncludeSeq("comments", encoder, oa.Comments)]));
    return {
        Encode(helpers_3) {
            const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_3)], values);
            return helpers_3.encodeObject(arg);
        },
    };
}

export const OntologyAnnotation_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, arg_7, objectArg_3;
    return OntologyAnnotation.create(unwrap((objectArg = get$.Optional, objectArg.Field("annotationValue", AnnotationValue_decoder))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("termSource", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("termAccession", string))), unwrap((arg_7 = resizeArray(decoder), (objectArg_3 = get$.Optional, objectArg_3.Field("comments", arg_7)))));
});

export function OntologyAnnotation_compressedEncoder(stringTable, oa) {
    const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("a", (s) => encodeString(stringTable, s), oa.Name), tryInclude("ts", (s_1) => encodeString(stringTable, s_1), oa.TermSourceREF), tryInclude("ta", (s_2) => encodeString(stringTable, s_2), oa.TermAccessionNumber), tryIncludeSeq("comments", encoder, oa.Comments)]));
    return {
        Encode(helpers) {
            const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers)], values);
            return helpers.encodeObject(arg);
        },
    };
}

export function OntologyAnnotation_compressedDecoder(stringTable) {
    return object((get$) => {
        let arg_1, objectArg, arg_3, objectArg_1, arg_5, objectArg_2, arg_7, objectArg_3;
        return new OntologyAnnotation(unwrap((arg_1 = decodeString(stringTable), (objectArg = get$.Optional, objectArg.Field("a", arg_1)))), unwrap((arg_3 = decodeString(stringTable), (objectArg_1 = get$.Optional, objectArg_1.Field("ts", arg_3)))), unwrap((arg_5 = decodeString(stringTable), (objectArg_2 = get$.Optional, objectArg_2.Field("ta", arg_5)))), unwrap((arg_7 = resizeArray(decoder), (objectArg_3 = get$.Optional, objectArg_3.Field("comments", arg_7)))));
    });
}

export function OntologyAnnotation_ROCrate_genID(o) {
    const matchValue = o.TermAccessionNumber;
    if (matchValue == null) {
        const matchValue_1 = o.TermSourceREF;
        if (matchValue_1 == null) {
            const matchValue_2 = o.Name;
            if (matchValue_2 == null) {
                return "#DummyOntologyAnnotation";
            }
            else {
                return "#UserTerm_" + replace(matchValue_2, " ", "_");
            }
        }
        else {
            return "#" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function OntologyAnnotation_ROCrate_encoderDefinedTerm(oa) {
    let value;
    const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = OntologyAnnotation_ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("OntologyAnnotation");
        },
    }], tryInclude("annotationValue", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.Name), tryInclude("termSource", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), oa.TermSourceREF), tryInclude("termAccession", (value_6) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_6);
        },
    }), oa.TermAccessionNumber), tryIncludeSeq("comments", ROCrate_encoderDisambiguatingDescription, oa.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_5) {
            const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
}

export const OntologyAnnotation_ROCrate_decoderDefinedTerm = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, arg_7, objectArg_3;
    return OntologyAnnotation.create(unwrap((objectArg = get$.Optional, objectArg.Field("annotationValue", AnnotationValue_decoder))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("termSource", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("termAccession", string))), unwrap((arg_7 = resizeArray(ROCrate_decoderDisambiguatingDescription), (objectArg_3 = get$.Optional, objectArg_3.Field("comments", arg_7)))));
});

export function OntologyAnnotation_ROCrate_encoderPropertyValue(oa) {
    let value;
    const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = OntologyAnnotation_ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("PropertyValue");
        },
    }], tryInclude("category", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.Name), tryInclude("categoryCode", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), oa.TermAccessionNumber), tryIncludeSeq("comments", ROCrate_encoderDisambiguatingDescription, oa.Comments), ["@context", context_jsonvalue_1]]));
    return {
        Encode(helpers_4) {
            const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const OntologyAnnotation_ROCrate_decoderPropertyValue = object((get$) => {
    let objectArg, objectArg_1, arg_5, objectArg_2;
    return OntologyAnnotation.create(unwrap((objectArg = get$.Optional, objectArg.Field("category", string))), undefined, unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("categoryCode", string))), unwrap((arg_5 = resizeArray(ROCrate_decoderDisambiguatingDescription), (objectArg_2 = get$.Optional, objectArg_2.Field("comments", arg_5)))));
});

export function OntologyAnnotation_ISAJson_encoder(idMap, oa) {
    const f = (oa_1) => {
        const comments = filter((c) => {
            const matchValue = c.Name;
            let matchResult, n_1;
            if (matchValue != null) {
                if (matchValue === orderName) {
                    matchResult = 0;
                    n_1 = matchValue;
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
                    return false;
                default:
                    return true;
            }
        }, oa_1.Comments);
        const values = choose((tupledArg) => map_1((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), OntologyAnnotation_ROCrate_genID(oa_1)), tryInclude("annotationValue", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), oa_1.Name), tryInclude("termSource", (value_4) => ({
            Encode(helpers_2) {
                return helpers_2.encodeString(value_4);
            },
        }), oa_1.TermSourceREF), tryInclude("termAccession", (value_6) => ({
            Encode(helpers_3) {
                return helpers_3.encodeString(value_6);
            },
        }), oa_1.TermAccessionNumber), tryIncludeSeq("comments", encoder, comments)]));
        return {
            Encode(helpers_4) {
                const arg = map_2((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_4)], values);
                return helpers_4.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(OntologyAnnotation_ROCrate_genID, f, oa, idMap);
    }
    else {
        return f(oa);
    }
}

export const OntologyAnnotation_ISAJson_decoder = OntologyAnnotation_decoder;

