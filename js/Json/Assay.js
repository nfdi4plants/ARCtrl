import { empty, singleton, ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { bind, defaultArg, unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { tryIncludeList, tryIncludeSeq, tryInclude } from "./Encode.js";
import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder, OntologyAnnotation_ROCrate_decoderDefinedTerm, OntologyAnnotation_ROCrate_decoderPropertyValue, OntologyAnnotation_ROCrate_encoderDefinedTerm, OntologyAnnotation_ROCrate_encoderPropertyValue, OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./OntologyAnnotation.js";
import { decoderCompressed as decoderCompressed_2, encoderCompressed as encoderCompressed_2, decoder as decoder_2, encoder as encoder_4 } from "./DataMap/DataMap.js";
import { decoderCompressed as decoderCompressed_1, encoderCompressed as encoderCompressed_1, decoder as decoder_1, encoder as encoder_5 } from "./Table/ArcTable.js";
import { ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_1, decoder as decoder_3, encoder as encoder_6 } from "./Person.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_3, ROCrate_decoder as ROCrate_decoder_3, ROCrate_encoder as ROCrate_encoder_4, decoder as decoder_4, encoder as encoder_7 } from "./Comment.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { map as map_2, list as list_2, resizeArray, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { ArcAssay } from "../Core/ArcTypes.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { Assay_tryIdentifierFromFileName, createMissingIdentifier, Assay_fileNameFromIdentifier } from "../Core/Helper/Identifier.js";
import { JsonTypes_decomposeTechnologyPlatform, JsonTypes_composeTechnologyPlatform, ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D, ARCtrl_ArcTables__ArcTables_GetProcesses } from "../Core/Conversion.js";
import { getCharacteristics, getUnits, getData } from "../Core/Process/ProcessSequence.js";
import { list as list_1 } from "../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { ISAJson_encoder as ISAJson_encoder_1, ROCrate_encoder as ROCrate_encoder_2 } from "./Data.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_3 } from "./Process/Process.js";
import { context_jsonvalue } from "./context/rocrate/isa_assay_context.js";
import { encoder as encoder_8 } from "./Process/MaterialAttribute.js";
import { encoder as encoder_9 } from "./Process/AssayMaterials.js";
import { Option_fromValueWithDefault } from "../Core/Helper/Collections.js";
import { encode } from "./IDTable.js";
import { Decode_objectNoAdditionalProperties } from "./Decode.js";

export function encoder(assay) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["Identifier", (value = assay.Identifier, {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], tryInclude("MeasurementType", OntologyAnnotation_encoder, assay.MeasurementType), tryInclude("TechnologyType", OntologyAnnotation_encoder, assay.TechnologyType), tryInclude("TechnologyPlatform", OntologyAnnotation_encoder, assay.TechnologyPlatform), tryInclude("DataMap", encoder_4, assay.DataMap), tryIncludeSeq("Tables", encoder_5, assay.Tables), tryIncludeSeq("Performers", encoder_6, assay.Performers), tryIncludeSeq("Comments", encoder_7, assay.Comments)]));
    return {
        Encode(helpers_1) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_1)], values);
            return helpers_1.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, arg_9, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7;
    return ArcAssay.create((objectArg = get$.Required, objectArg.Field("Identifier", string)), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("MeasurementType", OntologyAnnotation_decoder))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("TechnologyType", OntologyAnnotation_decoder))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("TechnologyPlatform", OntologyAnnotation_decoder))), unwrap((arg_9 = resizeArray(decoder_1), (objectArg_4 = get$.Optional, objectArg_4.Field("Tables", arg_9)))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("DataMap", decoder_2))), unwrap((arg_13 = resizeArray(decoder_3), (objectArg_6 = get$.Optional, objectArg_6.Field("Performers", arg_13)))), unwrap((arg_15 = resizeArray(decoder_4), (objectArg_7 = get$.Optional, objectArg_7.Field("Comments", arg_15)))));
});

export function encoderCompressed(stringTable, oaTable, cellTable, assay) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["Identifier", (value = assay.Identifier, {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], tryInclude("MeasurementType", OntologyAnnotation_encoder, assay.MeasurementType), tryInclude("TechnologyType", OntologyAnnotation_encoder, assay.TechnologyType), tryInclude("TechnologyPlatform", OntologyAnnotation_encoder, assay.TechnologyPlatform), tryIncludeSeq("Tables", (table) => encoderCompressed_1(stringTable, oaTable, cellTable, table), assay.Tables), tryInclude("DataMap", (dm) => encoderCompressed_2(stringTable, oaTable, cellTable, dm), assay.DataMap), tryIncludeSeq("Performers", encoder_6, assay.Performers), tryIncludeSeq("Comments", encoder_7, assay.Comments)]));
    return {
        Encode(helpers_1) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_1)], values);
            return helpers_1.encodeObject(arg);
        },
    };
}

export function decoderCompressed(stringTable, oaTable, cellTable) {
    return object((get$) => {
        let objectArg, objectArg_1, objectArg_2, objectArg_3, arg_9, objectArg_4, arg_11, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7;
        return ArcAssay.create((objectArg = get$.Required, objectArg.Field("Identifier", string)), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("MeasurementType", OntologyAnnotation_decoder))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("TechnologyType", OntologyAnnotation_decoder))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("TechnologyPlatform", OntologyAnnotation_decoder))), unwrap((arg_9 = resizeArray(decoderCompressed_1(stringTable, oaTable, cellTable)), (objectArg_4 = get$.Optional, objectArg_4.Field("Tables", arg_9)))), unwrap((arg_11 = decoderCompressed_2(stringTable, oaTable, cellTable), (objectArg_5 = get$.Optional, objectArg_5.Field("DataMap", arg_11)))), unwrap((arg_13 = resizeArray(decoder_3), (objectArg_6 = get$.Optional, objectArg_6.Field("Performers", arg_13)))), unwrap((arg_15 = resizeArray(decoder_4), (objectArg_7 = get$.Optional, objectArg_7.Field("Comments", arg_15)))));
    });
}

export function ROCrate_genID(a) {
    const matchValue = a.Identifier;
    if (matchValue === "") {
        return "#EmptyAssay";
    }
    else {
        return `#assay/${replace(matchValue, " ", "_")}`;
    }
}

export function ROCrate_encoder(studyName, a) {
    let value, value_3, assayName;
    const fileName = Assay_fileNameFromIdentifier(a.Identifier);
    const processes = ARCtrl_ArcTables__ArcTables_GetProcesses(a);
    const dataFiles = getData(processes);
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(a), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", list_1(singleton({
        Encode(helpers_1) {
            return helpers_1.encodeString("Assay");
        },
    }))], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("Assay");
        },
    }], ["identifier", (value_3 = a.Identifier, {
        Encode(helpers_3) {
            return helpers_3.encodeString(value_3);
        },
    })], ["filename", {
        Encode(helpers_4) {
            return helpers_4.encodeString(fileName);
        },
    }], tryInclude("measurementType", OntologyAnnotation_ROCrate_encoderPropertyValue, a.MeasurementType), tryInclude("technologyType", OntologyAnnotation_ROCrate_encoderDefinedTerm, a.TechnologyType), tryInclude("technologyPlatform", OntologyAnnotation_ROCrate_encoderDefinedTerm, a.TechnologyPlatform), tryIncludeSeq("performers", ROCrate_encoder_1, a.Performers), tryIncludeList("dataFiles", ROCrate_encoder_2, dataFiles), tryIncludeList("processSequence", (assayName = a.Identifier, (oa_5) => ROCrate_encoder_3(studyName, assayName, oa_5)), processes), tryIncludeSeq("comments", ROCrate_encoder_4, a.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_5) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, arg_3, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_12, objectArg_5, arg_14, objectArg_6;
    const identifier = defaultArg((objectArg = get$.Optional, objectArg.Field("identifier", string)), createMissingIdentifier());
    const tables = map((arg_4) => {
        const a = ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D(arg_4);
        return a.Tables;
    }, (arg_3 = list_2(ROCrate_decoder_1), (objectArg_1 = get$.Optional, objectArg_1.Field("processSequence", arg_3))));
    return new ArcAssay(identifier, unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("measurementType", OntologyAnnotation_ROCrate_decoderPropertyValue))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("technologyType", OntologyAnnotation_ROCrate_decoderDefinedTerm))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("technologyPlatform", OntologyAnnotation_ROCrate_decoderDefinedTerm))), unwrap(tables), undefined, unwrap((arg_12 = resizeArray(ROCrate_decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("performers", arg_12)))), unwrap((arg_14 = resizeArray(ROCrate_decoder_3), (objectArg_6 = get$.Optional, objectArg_6.Field("comments", arg_14)))));
});

export function ISAJson_encoder(studyName, idMap, a) {
    const f = (a_1) => {
        let assayName;
        const fileName = Assay_fileNameFromIdentifier(a_1.Identifier);
        const processes = ARCtrl_ArcTables__ArcTables_GetProcesses(a_1);
        const encodedUnits = tryIncludeList("unitCategories", (oa) => OntologyAnnotation_ISAJson_encoder(idMap, oa), getUnits(processes));
        const encodedCharacteristics = tryIncludeList("characteristicCategories", (value_1) => encoder_8(idMap, value_1), getCharacteristics(processes));
        const encodedMaterials = tryInclude("materials", (ps) => encoder_9(idMap, ps), Option_fromValueWithDefault(empty(), processes));
        const encocedDataFiles = tryIncludeList("dataFiles", (oa_1) => ISAJson_encoder_1(idMap, oa_1), getData(processes));
        const units = getUnits(processes);
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["filename", {
            Encode(helpers) {
                return helpers.encodeString(fileName);
            },
        }], tryInclude("@id", (value_5) => ({
            Encode(helpers_1) {
                return helpers_1.encodeString(value_5);
            },
        }), ROCrate_genID(a_1)), tryInclude("measurementType", (oa_2) => OntologyAnnotation_ISAJson_encoder(idMap, oa_2), a_1.MeasurementType), tryInclude("technologyType", (oa_3) => OntologyAnnotation_ISAJson_encoder(idMap, oa_3), a_1.TechnologyType), tryInclude("technologyPlatform", (value_7) => ({
            Encode(helpers_2) {
                return helpers_2.encodeString(value_7);
            },
        }), map(JsonTypes_composeTechnologyPlatform, a_1.TechnologyPlatform)), encocedDataFiles, encodedMaterials, encodedCharacteristics, encodedUnits, tryIncludeList("processSequence", (assayName = a_1.Identifier, (oa_4) => ISAJson_encoder_2(studyName, assayName, idMap, oa_4)), processes), tryIncludeSeq("comments", (comment) => ISAJson_encoder_3(idMap, comment), a_1.Comments)]));
        return {
            Encode(helpers_3) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_3)], values);
                return helpers_3.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(ROCrate_genID, f, a, idMap);
    }
    else {
        return f(a);
    }
}

export const ISAJson_allowedFields = ofArray(["@id", "filename", "measurementType", "technologyType", "technologyPlatform", "dataFiles", "materials", "characteristicCategories", "unitCategories", "processSequence", "comments", "@type", "@context"]);

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let objectArg, arg_3, objectArg_1, objectArg_2, objectArg_3, arg_10, objectArg_4, arg_12, objectArg_5;
    const identifier = defaultArg(bind(Assay_tryIdentifierFromFileName, (objectArg = get$.Optional, objectArg.Field("filename", string))), createMissingIdentifier());
    const tables = map((arg_4) => {
        const a = ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D(arg_4);
        return a.Tables;
    }, (arg_3 = list_2(ISAJson_decoder_1), (objectArg_1 = get$.Optional, objectArg_1.Field("processSequence", arg_3))));
    return new ArcAssay(identifier, unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("measurementType", OntologyAnnotation_ISAJson_decoder))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("technologyType", OntologyAnnotation_ISAJson_decoder))), unwrap((arg_10 = map_2(JsonTypes_decomposeTechnologyPlatform, string), (objectArg_4 = get$.Optional, objectArg_4.Field("technologyPlatform", arg_10)))), unwrap(tables), undefined, undefined, unwrap((arg_12 = resizeArray(ISAJson_decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", arg_12)))));
});

