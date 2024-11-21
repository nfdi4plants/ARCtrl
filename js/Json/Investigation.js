import { empty, unzip, ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { defaultArg, unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeSeq, tryInclude } from "./Encode.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_1, decoder as decoder_3, encoder as encoder_1 } from "./OntologySourceReference.js";
import { ISAJson_decoder as ISAJson_decoder_3, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_3, ROCrate_encoder as ROCrate_encoder_2, decoder as decoder_4, encoder as encoder_2 } from "./Publication.js";
import { ISAJson_decoder as ISAJson_decoder_4, ISAJson_encoder as ISAJson_encoder_3, ROCrate_decoder as ROCrate_decoder_4, ROCrate_encoder as ROCrate_encoder_3, decoder as decoder_5, encoder as encoder_3 } from "./Person.js";
import { decoderCompressed as decoderCompressed_1, encoderCompressed as encoderCompressed_1, decoder as decoder_6, encoder as encoder_4 } from "./Assay.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_4, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_4, decoderCompressed as decoderCompressed_2, encoderCompressed as encoderCompressed_2, decoder as decoder_7, encoder as encoder_5 } from "./Study.js";
import { ISAJson_decoder as ISAJson_decoder_5, ISAJson_encoder as ISAJson_encoder_5, ROCrate_decoder as ROCrate_decoder_5, ROCrate_encoder as ROCrate_encoder_5, decoder as decoder_8, encoder as encoder_6 } from "./Comment.js";
import { concat, map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { list as list_1, resizeArray, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { ArcInvestigation } from "../Core/ArcTypes.js";
import { context_jsonvalue } from "./context/rocrate/isa_investigation_context.js";
import { createMissingIdentifier } from "../Core/Helper/Identifier.js";
import { distinctBy } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { stringHash } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { context_jsonvalue as context_jsonvalue_1, conformsTo_jsonvalue } from "./context/rocrate/rocrate_context.js";
import { Decode_objectNoAdditionalProperties } from "./Decode.js";

export function encoder(inv) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["Identifier", (value = inv.Identifier, {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], tryInclude("Title", (value_1) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_1);
        },
    }), inv.Title), tryInclude("Description", (value_3) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_3);
        },
    }), inv.Description), tryInclude("SubmissionDate", (value_5) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    }), inv.SubmissionDate), tryInclude("PublicReleaseDate", (value_7) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_7);
        },
    }), inv.PublicReleaseDate), tryIncludeSeq("OntologySourceReferences", encoder_1, inv.OntologySourceReferences), tryIncludeSeq("Publications", encoder_2, inv.Publications), tryIncludeSeq("Contacts", encoder_3, inv.Contacts), tryIncludeSeq("Assays", encoder_4, inv.Assays), tryIncludeSeq("Studies", encoder_5, inv.Studies), tryIncludeSeq("RegisteredStudyIdentifiers", (value_9) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_9);
        },
    }), inv.RegisteredStudyIdentifiers), tryIncludeSeq("Comments", encoder_6, inv.Comments)]));
    return {
        Encode(helpers_6) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, arg_19, objectArg_9, arg_21, objectArg_10, arg_23, objectArg_11;
    return new ArcInvestigation((objectArg = get$.Required, objectArg.Field("Identifier", string)), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("Title", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("Description", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("SubmissionDate", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("PublicReleaseDate", string))), unwrap((arg_11 = resizeArray(decoder_3), (objectArg_5 = get$.Optional, objectArg_5.Field("OntologySourceReferences", arg_11)))), unwrap((arg_13 = resizeArray(decoder_4), (objectArg_6 = get$.Optional, objectArg_6.Field("Publications", arg_13)))), unwrap((arg_15 = resizeArray(decoder_5), (objectArg_7 = get$.Optional, objectArg_7.Field("Contacts", arg_15)))), unwrap((arg_17 = resizeArray(decoder_6), (objectArg_8 = get$.Optional, objectArg_8.Field("Assays", arg_17)))), unwrap((arg_19 = resizeArray(decoder_7), (objectArg_9 = get$.Optional, objectArg_9.Field("Studies", arg_19)))), unwrap((arg_21 = resizeArray(string), (objectArg_10 = get$.Optional, objectArg_10.Field("RegisteredStudyIdentifiers", arg_21)))), unwrap((arg_23 = resizeArray(decoder_8), (objectArg_11 = get$.Optional, objectArg_11.Field("Comments", arg_23)))));
});

export function encoderCompressed(stringTable, oaTable, cellTable, inv) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["Identifier", (value = inv.Identifier, {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], tryInclude("Title", (value_1) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_1);
        },
    }), inv.Title), tryInclude("Description", (value_3) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_3);
        },
    }), inv.Description), tryInclude("SubmissionDate", (value_5) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    }), inv.SubmissionDate), tryInclude("PublicReleaseDate", (value_7) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_7);
        },
    }), inv.PublicReleaseDate), tryIncludeSeq("OntologySourceReferences", encoder_1, inv.OntologySourceReferences), tryIncludeSeq("Publications", encoder_2, inv.Publications), tryIncludeSeq("Contacts", encoder_3, inv.Contacts), tryIncludeSeq("Assays", (assay) => encoderCompressed_1(stringTable, oaTable, cellTable, assay), inv.Assays), tryIncludeSeq("Studies", (study) => encoderCompressed_2(stringTable, oaTable, cellTable, study), inv.Studies), tryIncludeSeq("RegisteredStudyIdentifiers", (value_9) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_9);
        },
    }), inv.RegisteredStudyIdentifiers), tryIncludeSeq("Comments", encoder_6, inv.Comments)]));
    return {
        Encode(helpers_6) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
}

export function decoderCompressed(stringTable, oaTable, cellTable) {
    return object((get$) => {
        let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, arg_19, objectArg_9, arg_21, objectArg_10, arg_23, objectArg_11;
        return new ArcInvestigation((objectArg = get$.Required, objectArg.Field("Identifier", string)), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("Title", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("Description", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("SubmissionDate", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("PublicReleaseDate", string))), unwrap((arg_11 = resizeArray(decoder_3), (objectArg_5 = get$.Optional, objectArg_5.Field("OntologySourceReferences", arg_11)))), unwrap((arg_13 = resizeArray(decoder_4), (objectArg_6 = get$.Optional, objectArg_6.Field("Publications", arg_13)))), unwrap((arg_15 = resizeArray(decoder_5), (objectArg_7 = get$.Optional, objectArg_7.Field("Contacts", arg_15)))), unwrap((arg_17 = resizeArray(decoderCompressed_1(stringTable, oaTable, cellTable)), (objectArg_8 = get$.Optional, objectArg_8.Field("Assays", arg_17)))), unwrap((arg_19 = resizeArray(decoderCompressed_2(stringTable, oaTable, cellTable)), (objectArg_9 = get$.Optional, objectArg_9.Field("Studies", arg_19)))), unwrap((arg_21 = resizeArray(string), (objectArg_10 = get$.Optional, objectArg_10.Field("RegisteredStudyIdentifiers", arg_21)))), unwrap((arg_23 = resizeArray(decoder_8), (objectArg_11 = get$.Optional, objectArg_11.Field("Comments", arg_23)))));
    });
}

export function ROCrate_genID(i) {
    return "./";
}

export function ROCrate_encoder(oa) {
    let value, value_3, value_4;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(oa), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", {
        Encode(helpers_1) {
            return helpers_1.encodeString("Investigation");
        },
    }], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("Investigation");
        },
    }], ["identifier", (value_3 = oa.Identifier, {
        Encode(helpers_3) {
            return helpers_3.encodeString(value_3);
        },
    })], ["filename", (value_4 = ArcInvestigation.FileName, {
        Encode(helpers_4) {
            return helpers_4.encodeString(value_4);
        },
    })], tryInclude("title", (value_5) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_5);
        },
    }), oa.Title), tryInclude("description", (value_7) => ({
        Encode(helpers_6) {
            return helpers_6.encodeString(value_7);
        },
    }), oa.Description), tryInclude("submissionDate", (value_9) => ({
        Encode(helpers_7) {
            return helpers_7.encodeString(value_9);
        },
    }), oa.SubmissionDate), tryInclude("publicReleaseDate", (value_11) => ({
        Encode(helpers_8) {
            return helpers_8.encodeString(value_11);
        },
    }), oa.PublicReleaseDate), tryIncludeSeq("ontologySourceReferences", ROCrate_encoder_1, oa.OntologySourceReferences), tryIncludeSeq("publications", ROCrate_encoder_2, oa.Publications), tryIncludeSeq("people", ROCrate_encoder_3, oa.Contacts), tryIncludeSeq("studies", (s) => ROCrate_encoder_4(undefined, s), oa.Studies), tryIncludeSeq("comments", ROCrate_encoder_5, oa.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_9) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_9)], values);
            return helpers_9.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let arg_3, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, arg_19, objectArg_9;
    let identifier;
    let matchValue;
    const objectArg = get$.Optional;
    matchValue = objectArg.Field("identifier", string);
    identifier = ((matchValue == null) ? createMissingIdentifier() : matchValue);
    const patternInput = unzip(defaultArg((arg_3 = list_1(ROCrate_decoder_1), (objectArg_1 = get$.Optional, objectArg_1.Field("studies", arg_3))), empty()));
    const studiesRaw = patternInput[0];
    let assays;
    const collection = distinctBy((a) => a.Identifier, concat(patternInput[1]), {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    });
    assays = Array.from(collection);
    const studies = Array.from(studiesRaw);
    let studyIdentifiers;
    const collection_1 = map_1((a_1) => a_1.Identifier, studiesRaw);
    studyIdentifiers = Array.from(collection_1);
    return new ArcInvestigation(identifier, unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("title", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("description", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("submissionDate", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("publicReleaseDate", string))), unwrap((arg_13 = resizeArray(ROCrate_decoder_2), (objectArg_6 = get$.Optional, objectArg_6.Field("ontologySourceReferences", arg_13)))), unwrap((arg_15 = resizeArray(ROCrate_decoder_3), (objectArg_7 = get$.Optional, objectArg_7.Field("publications", arg_15)))), unwrap((arg_17 = resizeArray(ROCrate_decoder_4), (objectArg_8 = get$.Optional, objectArg_8.Field("people", arg_17)))), assays, studies, studyIdentifiers, unwrap((arg_19 = resizeArray(ROCrate_decoder_5), (objectArg_9 = get$.Optional, objectArg_9.Field("comments", arg_19)))));
});

export function ROCrate_encodeRoCrate(oa) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([tryInclude("@type", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), "CreativeWork"), tryInclude("@id", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), "ro-crate-metadata.json"), tryInclude("about", ROCrate_encoder, oa), ["conformsTo", conformsTo_jsonvalue], ["@context", context_jsonvalue_1]]));
    return {
        Encode(helpers_2) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_2)], values);
            return helpers_2.encodeObject(arg);
        },
    };
}

export const ISAJson_allowedFields = ofArray(["@id", "filename", "identifier", "title", "description", "submissionDate", "publicReleaseDate", "ontologySourceReferences", "publications", "people", "studies", "comments", "@type", "@context"]);

export function ISAJson_encoder(idMap, inv) {
    let value, value_1, value_2;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(inv), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["filename", (value_1 = ArcInvestigation.FileName, {
        Encode(helpers_1) {
            return helpers_1.encodeString(value_1);
        },
    })], ["identifier", (value_2 = inv.Identifier, {
        Encode(helpers_2) {
            return helpers_2.encodeString(value_2);
        },
    })], tryInclude("title", (value_3) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_3);
        },
    }), inv.Title), tryInclude("description", (value_5) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_5);
        },
    }), inv.Description), tryInclude("submissionDate", (value_7) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_7);
        },
    }), inv.SubmissionDate), tryInclude("publicReleaseDate", (value_9) => ({
        Encode(helpers_6) {
            return helpers_6.encodeString(value_9);
        },
    }), inv.PublicReleaseDate), tryIncludeSeq("ontologySourceReferences", (osr) => ISAJson_encoder_1(idMap, osr), inv.OntologySourceReferences), tryIncludeSeq("publications", (oa) => ISAJson_encoder_2(idMap, oa), inv.Publications), tryIncludeSeq("people", (person) => ISAJson_encoder_3(idMap, person), inv.Contacts), tryIncludeSeq("studies", (s) => ISAJson_encoder_4(idMap, undefined, s), inv.Studies), tryIncludeSeq("comments", (comment) => ISAJson_encoder_5(idMap, comment), inv.Comments)]));
    return {
        Encode(helpers_7) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_7)], values);
            return helpers_7.encodeObject(arg);
        },
    };
}

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let arg_3, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, arg_19, objectArg_9;
    let identifer;
    let matchValue;
    const objectArg = get$.Optional;
    matchValue = objectArg.Field("identifier", string);
    identifer = ((matchValue == null) ? createMissingIdentifier() : matchValue);
    const patternInput = unzip(defaultArg((arg_3 = list_1(ISAJson_decoder_1), (objectArg_1 = get$.Optional, objectArg_1.Field("studies", arg_3))), empty()));
    const studiesRaw = patternInput[0];
    let assays;
    const collection = distinctBy((a) => a.Identifier, concat(patternInput[1]), {
        Equals: (x, y) => (x === y),
        GetHashCode: stringHash,
    });
    assays = Array.from(collection);
    const studies = Array.from(studiesRaw);
    let studyIdentifiers;
    const collection_1 = map_1((a_1) => a_1.Identifier, studiesRaw);
    studyIdentifiers = Array.from(collection_1);
    return new ArcInvestigation(identifer, unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("title", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("description", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("submissionDate", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("publicReleaseDate", string))), unwrap((arg_13 = resizeArray(ISAJson_decoder_2), (objectArg_6 = get$.Optional, objectArg_6.Field("ontologySourceReferences", arg_13)))), unwrap((arg_15 = resizeArray(ISAJson_decoder_3), (objectArg_7 = get$.Optional, objectArg_7.Field("publications", arg_15)))), unwrap((arg_17 = resizeArray(ISAJson_decoder_4), (objectArg_8 = get$.Optional, objectArg_8.Field("people", arg_17)))), assays, studies, studyIdentifiers, unwrap((arg_19 = resizeArray(ISAJson_decoder_5), (objectArg_9 = get$.Optional, objectArg_9.Field("comments", arg_19)))));
});

