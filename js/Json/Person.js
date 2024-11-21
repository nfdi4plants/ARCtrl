import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { defaultArg, unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "./Encode.js";
import { OntologyAnnotation_ROCrate_decoderDefinedTerm, OntologyAnnotation_ROCrate_encoderDefinedTerm, OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./OntologyAnnotation.js";
import { ROCrate_decoderDisambiguatingDescription, ROCrate_encoderDisambiguatingDescription, decoder as decoder_1, encoder as encoder_1 } from "./Comment.js";
import { tryPick, map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { array as array_3, map as map_3, resizeArray, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Person } from "../Core/Person.js";
import { join, split, replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { context_jsonvalue } from "./context/rocrate/isa_organization_context.js";
import { contextMinimal_jsonValue, context_jsonvalue as context_jsonvalue_1 } from "./context/rocrate/isa_person_context.js";
import { map as map_2 } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { Person_setOrcidFromComments, Person_setCommentFromORCID } from "../Core/Conversion.js";
import { encode } from "./IDTable.js";
import { Decode_objectNoAdditionalProperties } from "./Decode.js";

export function encoder(person) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("firstName", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), person.FirstName), tryInclude("lastName", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), person.LastName), tryInclude("midInitials", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), person.MidInitials), tryInclude("orcid", (value_6) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_6);
        },
    }), person.ORCID), tryInclude("email", (value_8) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_8);
        },
    }), person.EMail), tryInclude("phone", (value_10) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_10);
        },
    }), person.Phone), tryInclude("fax", (value_12) => ({
        Encode(helpers_6) {
            return helpers_6.encodeString(value_12);
        },
    }), person.Fax), tryInclude("address", (value_14) => ({
        Encode(helpers_7) {
            return helpers_7.encodeString(value_14);
        },
    }), person.Address), tryInclude("affiliation", (value_16) => ({
        Encode(helpers_8) {
            return helpers_8.encodeString(value_16);
        },
    }), person.Affiliation), tryIncludeSeq("roles", OntologyAnnotation_encoder, person.Roles), tryIncludeSeq("comments", encoder_1, person.Comments)]));
    return {
        Encode(helpers_9) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_9)], values);
            return helpers_9.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7, objectArg_8, arg_19, objectArg_9, arg_21, objectArg_10;
    return new Person(unwrap((objectArg = get$.Optional, objectArg.Field("orcid", string))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("lastName", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("firstName", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("midInitials", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("email", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("phone", string))), unwrap((objectArg_6 = get$.Optional, objectArg_6.Field("fax", string))), unwrap((objectArg_7 = get$.Optional, objectArg_7.Field("address", string))), unwrap((objectArg_8 = get$.Optional, objectArg_8.Field("affiliation", string))), unwrap((arg_19 = resizeArray(OntologyAnnotation_decoder), (objectArg_9 = get$.Optional, objectArg_9.Field("roles", arg_19)))), unwrap((arg_21 = resizeArray(decoder_1), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", arg_21)))));
});

export function ROCrate_genID(p) {
    const orcid = tryPick((c) => {
        const matchValue = c.Name;
        const matchValue_1 = c.Value;
        let matchResult, n, v;
        if (matchValue != null) {
            if (matchValue_1 != null) {
                matchResult = 0;
                n = matchValue;
                v = matchValue_1;
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
                if (((n === "orcid") ? true : (n === "Orcid")) ? true : (n === "ORCID")) {
                    return v;
                }
                else {
                    return undefined;
                }
            default:
                return undefined;
        }
    }, p.Comments);
    if (orcid == null) {
        const matchValue_3 = p.EMail;
        if (matchValue_3 == null) {
            const matchValue_4 = p.FirstName;
            const matchValue_5 = p.MidInitials;
            const matchValue_6 = p.LastName;
            let matchResult_1, fn, ln, mn, fn_1, ln_1, ln_2, fn_2;
            if (matchValue_4 == null) {
                if (matchValue_5 == null) {
                    if (matchValue_6 != null) {
                        matchResult_1 = 2;
                        ln_2 = matchValue_6;
                    }
                    else {
                        matchResult_1 = 4;
                    }
                }
                else {
                    matchResult_1 = 4;
                }
            }
            else if (matchValue_5 == null) {
                if (matchValue_6 == null) {
                    matchResult_1 = 3;
                    fn_2 = matchValue_4;
                }
                else {
                    matchResult_1 = 1;
                    fn_1 = matchValue_4;
                    ln_1 = matchValue_6;
                }
            }
            else if (matchValue_6 != null) {
                matchResult_1 = 0;
                fn = matchValue_4;
                ln = matchValue_6;
                mn = matchValue_5;
            }
            else {
                matchResult_1 = 4;
            }
            switch (matchResult_1) {
                case 0:
                    return (((("#" + replace(fn, " ", "_")) + "_") + replace(mn, " ", "_")) + "_") + replace(ln, " ", "_");
                case 1:
                    return (("#" + replace(fn_1, " ", "_")) + "_") + replace(ln_1, " ", "_");
                case 2:
                    return "#" + replace(ln_2, " ", "_");
                case 3:
                    return "#" + replace(fn_2, " ", "_");
                default:
                    return "#EmptyPerson";
            }
        }
        else {
            return matchValue_3;
        }
    }
    else {
        return orcid;
    }
}

export function ROCrate_Affiliation_encoder(affiliation) {
    let value_1;
    const values = ofArray([["@type", {
        Encode(helpers) {
            return helpers.encodeString("Organization");
        },
    }], ["@id", (value_1 = replace(`#Organization_${affiliation}`, " ", "_"), {
        Encode(helpers_1) {
            return helpers_1.encodeString(value_1);
        },
    })], ["name", {
        Encode(helpers_2) {
            return helpers_2.encodeString(affiliation);
        },
    }], ["@context", context_jsonvalue]]);
    return {
        Encode(helpers_3) {
            const arg = map_1((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_3)], values);
            return helpers_3.encodeObject(arg);
        },
    };
}

export const ROCrate_Affiliation_decoder = object((get$) => {
    const objectArg = get$.Required;
    return objectArg.Field("name", string);
});

export function ROCrate_encoder(oa) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("Person");
        },
    }], tryInclude("orcid", (value_2) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    }), oa.ORCID), tryInclude("firstName", (value_4) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_4);
        },
    }), oa.FirstName), tryInclude("lastName", (value_6) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_6);
        },
    }), oa.LastName), tryInclude("midInitials", (value_8) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_8);
        },
    }), oa.MidInitials), tryInclude("email", (value_10) => ({
        Encode(helpers_6) {
            return helpers_6.encodeString(value_10);
        },
    }), oa.EMail), tryInclude("phone", (value_12) => ({
        Encode(helpers_7) {
            return helpers_7.encodeString(value_12);
        },
    }), oa.Phone), tryInclude("fax", (value_14) => ({
        Encode(helpers_8) {
            return helpers_8.encodeString(value_14);
        },
    }), oa.Fax), tryInclude("address", (value_16) => ({
        Encode(helpers_9) {
            return helpers_9.encodeString(value_16);
        },
    }), oa.Address), tryInclude("affiliation", ROCrate_Affiliation_encoder, oa.Affiliation), tryIncludeSeq("roles", OntologyAnnotation_ROCrate_encoderDefinedTerm, oa.Roles), tryIncludeSeq("comments", ROCrate_encoderDisambiguatingDescription, oa.Comments), ["@context", context_jsonvalue_1]]));
    return {
        Encode(helpers_10) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_10)], values);
            return helpers_10.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7, objectArg_8, arg_19, objectArg_9, arg_21, objectArg_10;
    return new Person(unwrap((objectArg = get$.Optional, objectArg.Field("orcid", string))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("lastName", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("firstName", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("midInitials", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("email", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("phone", string))), unwrap((objectArg_6 = get$.Optional, objectArg_6.Field("fax", string))), unwrap((objectArg_7 = get$.Optional, objectArg_7.Field("address", string))), unwrap((objectArg_8 = get$.Optional, objectArg_8.Field("affiliation", ROCrate_Affiliation_decoder))), unwrap((arg_19 = resizeArray(OntologyAnnotation_ROCrate_decoderDefinedTerm), (objectArg_9 = get$.Optional, objectArg_9.Field("roles", arg_19)))), unwrap((arg_21 = resizeArray(ROCrate_decoderDisambiguatingDescription), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", arg_21)))));
});

/**
 * This is only used for ro-crate creation. In ISA publication authors are only a string. ro-crate requires person object.
 * Therefore, we try to split the string by common separators and create a minimal person object for ro-crate.
 */
export function ROCrate_encodeAuthorListString(authorList) {
    const values_2 = map_2((name) => {
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@type", {
            Encode(helpers) {
                return helpers.encodeString("Person");
            },
        }], tryInclude("name", (value_1) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_1);
            },
        }), name), ["@context", contextMinimal_jsonValue]]));
        return {
            Encode(helpers_2) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
                return helpers_2.encodeObject(arg);
            },
        };
    }, map_2((s) => s.trim(), split(authorList, [(authorList.indexOf("\t") >= 0) ? "\t" : ((authorList.indexOf(";") >= 0) ? ";" : ",")], undefined, 0)));
    return {
        Encode(helpers_3) {
            const arg_1 = map_2((v_3) => v_3.Encode(helpers_3), values_2);
            return helpers_3.encodeArray(arg_1);
        },
    };
}

export const ROCrate_decodeAuthorListString = map_3((v) => join(", ", v), array_3(object((get$) => {
    let objectArg;
    return defaultArg((objectArg = get$.Optional, objectArg.Field("name", string)), "");
})));

export const ISAJson_allowedFields = ofArray(["@id", "firstName", "lastName", "midInitials", "email", "phone", "fax", "address", "affiliation", "roles", "comments", "@type", "@context"]);

export function ISAJson_encoder(idMap, person) {
    const f = (person_1) => {
        const person_2 = Person_setCommentFromORCID(person_1);
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@id", (value) => ({
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        }), ROCrate_genID(person_2)), tryInclude("firstName", (value_2) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_2);
            },
        }), person_2.FirstName), tryInclude("lastName", (value_4) => ({
            Encode(helpers_2) {
                return helpers_2.encodeString(value_4);
            },
        }), person_2.LastName), tryInclude("midInitials", (value_6) => ({
            Encode(helpers_3) {
                return helpers_3.encodeString(value_6);
            },
        }), person_2.MidInitials), tryInclude("email", (value_8) => ({
            Encode(helpers_4) {
                return helpers_4.encodeString(value_8);
            },
        }), person_2.EMail), tryInclude("phone", (value_10) => ({
            Encode(helpers_5) {
                return helpers_5.encodeString(value_10);
            },
        }), person_2.Phone), tryInclude("fax", (value_12) => ({
            Encode(helpers_6) {
                return helpers_6.encodeString(value_12);
            },
        }), person_2.Fax), tryInclude("address", (value_14) => ({
            Encode(helpers_7) {
                return helpers_7.encodeString(value_14);
            },
        }), person_2.Address), tryInclude("affiliation", (value_16) => ({
            Encode(helpers_8) {
                return helpers_8.encodeString(value_16);
            },
        }), person_2.Affiliation), tryIncludeSeq("roles", OntologyAnnotation_encoder, person_2.Roles), tryIncludeSeq("comments", encoder_1, person_2.Comments)]));
        return {
            Encode(helpers_9) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_9)], values);
                return helpers_9.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(ROCrate_genID, f, person, idMap);
    }
    else {
        return f(person);
    }
}

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7, arg_17, objectArg_8, arg_19, objectArg_9;
    return Person_setOrcidFromComments(new Person(undefined, unwrap((objectArg = get$.Optional, objectArg.Field("lastName", string))), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("firstName", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("midInitials", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("email", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("phone", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("fax", string))), unwrap((objectArg_6 = get$.Optional, objectArg_6.Field("address", string))), unwrap((objectArg_7 = get$.Optional, objectArg_7.Field("affiliation", string))), unwrap((arg_17 = resizeArray(OntologyAnnotation_decoder), (objectArg_8 = get$.Optional, objectArg_8.Field("roles", arg_17)))), unwrap((arg_19 = resizeArray(decoder_1), (objectArg_9 = get$.Optional, objectArg_9.Field("comments", arg_19))))));
});

