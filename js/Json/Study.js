import { Option_fromValueWithDefault, ResizeArray_map } from "../Core/Helper/Collections.js";
import { defaultArgWith, bind, unwrap, map, value as value_17, defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { empty, map as map_2, singleton, ofArray, choose, tryFind } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { ArcStudy, ArcAssay } from "../Core/ArcTypes.js";
import { tryIncludeList, tryIncludeSeq, tryInclude } from "./Encode.js";
import { ISAJson_decoder as ISAJson_decoder_4, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_3, ROCrate_encoder as ROCrate_encoder_1, decoder as decoder_2, encoder as encoder_5 } from "./Publication.js";
import { ISAJson_decoder as ISAJson_decoder_3, ISAJson_encoder as ISAJson_encoder_3, ROCrate_decoder as ROCrate_decoder_4, ROCrate_encoder as ROCrate_encoder_2, decoder as decoder_3, encoder as encoder_6 } from "./Person.js";
import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder, OntologyAnnotation_ROCrate_decoderDefinedTerm, OntologyAnnotation_ROCrate_encoderDefinedTerm, OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./OntologyAnnotation.js";
import { decoderCompressed as decoderCompressed_1, encoderCompressed as encoderCompressed_1, decoder as decoder_4, encoder as encoder_7 } from "./Table/ArcTable.js";
import { decoderCompressed as decoderCompressed_2, encoderCompressed as encoderCompressed_2, decoder as decoder_5, encoder as encoder_8 } from "./DataMap/DataMap.js";
import { ISAJson_decoder as ISAJson_decoder_5, ISAJson_encoder as ISAJson_encoder_6, ROCrate_decoder as ROCrate_decoder_5, ROCrate_encoder as ROCrate_encoder_5, decoder as decoder_6, encoder as encoder_9 } from "./Comment.js";
import { isEmpty, map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { list as list_2, resizeArray, string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { replace } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { Study_fileNameFromIdentifier, createMissingIdentifier, Study_tryIdentifierFromFileName, Study_tryFileNameFromIdentifier } from "../Core/Helper/Identifier.js";
import { Person_removeSourceAssayComments, Person_getSourceAssayIdentifiersFromComments, Person_setSourceAssayComment, ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D, ARCtrl_ArcTables__ArcTables_GetProcesses } from "../Core/Conversion.js";
import { list as list_1 } from "../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_4, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_3 } from "./Process/Process.js";
import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_5, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_4 } from "./Assay.js";
import { context_jsonvalue } from "./context/rocrate/isa_study_context.js";
import { disposeSafe, getEnumerator } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { getProtocols, getCharacteristics, getFactors, getUnits } from "../Core/Process/ProcessSequence.js";
import { encoder as encoder_10 } from "./Process/Factor.js";
import { encoder as encoder_11 } from "./Process/MaterialAttribute.js";
import { encoder as encoder_12 } from "./Process/StudyMaterials.js";
import { ISAJson_encoder as ISAJson_encoder_1 } from "./Process/Protocol.js";
import { encode } from "./IDTable.js";
import { Decode_objectNoAdditionalProperties } from "./Decode.js";

/**
 * Get registered assays or get assays from `assays` if IsSome
 */
export function Helper_getAssayInformation(assays, study) {
    if (assays != null) {
        return ResizeArray_map((assayId) => defaultArg(tryFind((a) => (a.Identifier === assayId), value_17(assays)), ArcAssay.init(assayId)), study.RegisteredAssayIdentifiers);
    }
    else {
        return study.GetRegisteredAssaysOrIdentifier();
    }
}

export function encoder(study) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["Identifier", (value = study.Identifier, {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], tryInclude("Title", (value_1) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_1);
        },
    }), study.Title), tryInclude("Description", (value_3) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_3);
        },
    }), study.Description), tryInclude("SubmissionDate", (value_5) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    }), study.SubmissionDate), tryInclude("PublicReleaseDate", (value_7) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_7);
        },
    }), study.PublicReleaseDate), tryIncludeSeq("Publications", encoder_5, study.Publications), tryIncludeSeq("Contacts", encoder_6, study.Contacts), tryIncludeSeq("StudyDesignDescriptors", OntologyAnnotation_encoder, study.StudyDesignDescriptors), tryIncludeSeq("Tables", encoder_7, study.Tables), tryInclude("DataMap", encoder_8, study.DataMap), tryIncludeSeq("RegisteredAssayIdentifiers", (value_9) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_9);
        },
    }), study.RegisteredAssayIdentifiers), tryIncludeSeq("Comments", encoder_9, study.Comments)]));
    return {
        Encode(helpers_6) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, objectArg_9, arg_21, objectArg_10, arg_23, objectArg_11;
    return new ArcStudy((objectArg = get$.Required, objectArg.Field("Identifier", string)), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("Title", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("Description", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("SubmissionDate", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("PublicReleaseDate", string))), unwrap((arg_11 = resizeArray(decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("Publications", arg_11)))), unwrap((arg_13 = resizeArray(decoder_3), (objectArg_6 = get$.Optional, objectArg_6.Field("Contacts", arg_13)))), unwrap((arg_15 = resizeArray(OntologyAnnotation_decoder), (objectArg_7 = get$.Optional, objectArg_7.Field("StudyDesignDescriptors", arg_15)))), unwrap((arg_17 = resizeArray(decoder_4), (objectArg_8 = get$.Optional, objectArg_8.Field("Tables", arg_17)))), unwrap((objectArg_9 = get$.Optional, objectArg_9.Field("DataMap", decoder_5))), unwrap((arg_21 = resizeArray(string), (objectArg_10 = get$.Optional, objectArg_10.Field("RegisteredAssayIdentifiers", arg_21)))), unwrap((arg_23 = resizeArray(decoder_6), (objectArg_11 = get$.Optional, objectArg_11.Field("Comments", arg_23)))));
});

export function encoderCompressed(stringTable, oaTable, cellTable, study) {
    let value;
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["Identifier", (value = study.Identifier, {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], tryInclude("Title", (value_1) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_1);
        },
    }), study.Title), tryInclude("Description", (value_3) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_3);
        },
    }), study.Description), tryInclude("SubmissionDate", (value_5) => ({
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    }), study.SubmissionDate), tryInclude("PublicReleaseDate", (value_7) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_7);
        },
    }), study.PublicReleaseDate), tryIncludeSeq("Publications", encoder_5, study.Publications), tryIncludeSeq("Contacts", encoder_6, study.Contacts), tryIncludeSeq("StudyDesignDescriptors", OntologyAnnotation_encoder, study.StudyDesignDescriptors), tryIncludeSeq("Tables", (table) => encoderCompressed_1(stringTable, oaTable, cellTable, table), study.Tables), tryInclude("DataMap", (dm) => encoderCompressed_2(stringTable, oaTable, cellTable, dm), study.DataMap), tryIncludeSeq("RegisteredAssayIdentifiers", (value_9) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_9);
        },
    }), study.RegisteredAssayIdentifiers), tryIncludeSeq("Comments", encoder_9, study.Comments)]));
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
        return new ArcStudy((objectArg = get$.Required, objectArg.Field("Identifier", string)), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("Title", string))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("Description", string))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("SubmissionDate", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("PublicReleaseDate", string))), unwrap((arg_11 = resizeArray(decoder_2), (objectArg_5 = get$.Optional, objectArg_5.Field("Publications", arg_11)))), unwrap((arg_13 = resizeArray(decoder_3), (objectArg_6 = get$.Optional, objectArg_6.Field("Contacts", arg_13)))), unwrap((arg_15 = resizeArray(OntologyAnnotation_decoder), (objectArg_7 = get$.Optional, objectArg_7.Field("StudyDesignDescriptors", arg_15)))), unwrap((arg_17 = resizeArray(decoderCompressed_1(stringTable, oaTable, cellTable)), (objectArg_8 = get$.Optional, objectArg_8.Field("Tables", arg_17)))), unwrap((arg_19 = decoderCompressed_2(stringTable, oaTable, cellTable), (objectArg_9 = get$.Optional, objectArg_9.Field("DataMap", arg_19)))), unwrap((arg_21 = resizeArray(string), (objectArg_10 = get$.Optional, objectArg_10.Field("RegisteredAssayIdentifiers", arg_21)))), unwrap((arg_23 = resizeArray(decoder_6), (objectArg_11 = get$.Optional, objectArg_11.Field("Comments", arg_23)))));
    });
}

export function ROCrate_genID(a) {
    const matchValue = a.Identifier;
    if (matchValue === "") {
        return "#EmptyStudy";
    }
    else {
        return `#study/${replace(matchValue, " ", "_")}`;
    }
}

export function ROCrate_encoder(assays, s) {
    let value, value_3, studyName, studyName_1;
    const fileName = Study_tryFileNameFromIdentifier(s.Identifier);
    const processes = ARCtrl_ArcTables__ArcTables_GetProcesses(s);
    const assays_1 = Helper_getAssayInformation(assays, s);
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value = ROCrate_genID(s), {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    })], ["@type", list_1(singleton({
        Encode(helpers_1) {
            return helpers_1.encodeString("Study");
        },
    }))], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("Study");
        },
    }], ["identifier", (value_3 = s.Identifier, {
        Encode(helpers_3) {
            return helpers_3.encodeString(value_3);
        },
    })], tryInclude("filename", (value_4) => ({
        Encode(helpers_4) {
            return helpers_4.encodeString(value_4);
        },
    }), fileName), tryInclude("title", (value_6) => ({
        Encode(helpers_5) {
            return helpers_5.encodeString(value_6);
        },
    }), s.Title), tryInclude("description", (value_8) => ({
        Encode(helpers_6) {
            return helpers_6.encodeString(value_8);
        },
    }), s.Description), tryIncludeSeq("studyDesignDescriptors", OntologyAnnotation_ROCrate_encoderDefinedTerm, s.StudyDesignDescriptors), tryInclude("submissionDate", (value_10) => ({
        Encode(helpers_7) {
            return helpers_7.encodeString(value_10);
        },
    }), s.SubmissionDate), tryInclude("publicReleaseDate", (value_12) => ({
        Encode(helpers_8) {
            return helpers_8.encodeString(value_12);
        },
    }), s.PublicReleaseDate), tryIncludeSeq("publications", ROCrate_encoder_1, s.Publications), tryIncludeSeq("people", ROCrate_encoder_2, s.Contacts), tryIncludeList("processSequence", (studyName = s.Identifier, (oa_3) => ROCrate_encoder_3(studyName, undefined, oa_3)), processes), tryIncludeSeq("assays", (studyName_1 = s.Identifier, (a_1) => ROCrate_encoder_4(studyName_1, a_1)), assays_1), tryIncludeSeq("comments", ROCrate_encoder_5, s.Comments), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_9) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_9)], values);
            return helpers_9.encodeObject(arg);
        },
    };
}

export const ROCrate_decoder = object((get$) => {
    let objectArg, arg_6, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, arg_16, objectArg_7, arg_18, objectArg_8, arg_20, objectArg_9, arg_22, objectArg_10;
    const identifier = defaultArg(bind(Study_tryIdentifierFromFileName, (objectArg = get$.Optional, objectArg.Field("filename", string))), createMissingIdentifier());
    let assays;
    const arg_3 = list_2(ROCrate_decoder_1);
    const objectArg_1 = get$.Optional;
    assays = objectArg_1.Field("assays", arg_3);
    const assayIdentifiers = map((arg_4) => {
        const collection = map_2((a) => a.Identifier, arg_4);
        return Array.from(collection);
    }, assays);
    const tables = map((ps) => ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D(ps).Tables, (arg_6 = list_2(ROCrate_decoder_2), (objectArg_2 = get$.Optional, objectArg_2.Field("processSequence", arg_6))));
    return [new ArcStudy(identifier, unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("title", string))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("description", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("submissionDate", string))), unwrap((objectArg_6 = get$.Optional, objectArg_6.Field("publicReleaseDate", string))), unwrap((arg_16 = resizeArray(ROCrate_decoder_3), (objectArg_7 = get$.Optional, objectArg_7.Field("publications", arg_16)))), unwrap((arg_18 = resizeArray(ROCrate_decoder_4), (objectArg_8 = get$.Optional, objectArg_8.Field("people", arg_18)))), unwrap((arg_20 = resizeArray(OntologyAnnotation_ROCrate_decoderDefinedTerm), (objectArg_9 = get$.Optional, objectArg_9.Field("studyDesignDescriptors", arg_20)))), unwrap(tables), undefined, unwrap(assayIdentifiers), unwrap((arg_22 = resizeArray(ROCrate_decoder_5), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", arg_22))))), defaultArg(assays, empty())];
});

/**
 * If assays.IsSome then try to get registered assays from external list, otherwise try to access investigation or create empty defaults.
 */
export function ISAJson_encoder(idMap, assays, s) {
    const f = (s_1) => {
        let studyName, value_6, value_8, studyName_1, studyName_2;
        const study = s_1.Copy(true);
        const fileName = Study_fileNameFromIdentifier(study.Identifier);
        let assays_1;
        const n = [];
        let enumerator = getEnumerator(Helper_getAssayInformation(assays, study));
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const a = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const assay = a.Copy();
                let enumerator_1 = getEnumerator(assay.Performers);
                try {
                    while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                        const person_1 = Person_setSourceAssayComment(enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"](), assay.Identifier);
                        void (study.Contacts.push(person_1));
                    }
                }
                finally {
                    disposeSafe(enumerator_1);
                }
                assay.Performers = [];
                void (n.push(assay));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        assays_1 = n;
        const processes = ARCtrl_ArcTables__ArcTables_GetProcesses(study);
        const encodedUnits = tryIncludeList("unitCategories", (oa) => OntologyAnnotation_ISAJson_encoder(idMap, oa), getUnits(processes));
        const encodedFactors = tryIncludeList("factors", (value_1) => encoder_10(idMap, value_1), getFactors(processes));
        const encodedCharacteristics = tryIncludeList("characteristicCategories", (value_3) => encoder_11(idMap, value_3), getCharacteristics(processes));
        const encodedMaterials = tryInclude("materials", (ps) => encoder_12(idMap, ps), Option_fromValueWithDefault(empty(), processes));
        let encodedProtocols;
        const value_5 = getProtocols(processes);
        encodedProtocols = tryIncludeList("protocols", (studyName = s_1.Identifier, (oa_1) => ISAJson_encoder_1(studyName, undefined, undefined, idMap, oa_1)), value_5);
        const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["@id", (value_6 = ROCrate_genID(study), {
            Encode(helpers) {
                return helpers.encodeString(value_6);
            },
        })], ["filename", {
            Encode(helpers_1) {
                return helpers_1.encodeString(fileName);
            },
        }], ["identifier", (value_8 = study.Identifier, {
            Encode(helpers_2) {
                return helpers_2.encodeString(value_8);
            },
        })], tryInclude("title", (value_9) => ({
            Encode(helpers_3) {
                return helpers_3.encodeString(value_9);
            },
        }), study.Title), tryInclude("description", (value_11) => ({
            Encode(helpers_4) {
                return helpers_4.encodeString(value_11);
            },
        }), study.Description), tryInclude("submissionDate", (value_13) => ({
            Encode(helpers_5) {
                return helpers_5.encodeString(value_13);
            },
        }), study.SubmissionDate), tryInclude("publicReleaseDate", (value_15) => ({
            Encode(helpers_6) {
                return helpers_6.encodeString(value_15);
            },
        }), study.PublicReleaseDate), tryIncludeSeq("publications", (oa_2) => ISAJson_encoder_2(idMap, oa_2), study.Publications), tryIncludeSeq("people", (person_2) => ISAJson_encoder_3(idMap, person_2), study.Contacts), tryIncludeSeq("studyDesignDescriptors", (oa_3) => OntologyAnnotation_ISAJson_encoder(idMap, oa_3), study.StudyDesignDescriptors), encodedProtocols, encodedMaterials, tryIncludeList("processSequence", (studyName_1 = s_1.Identifier, (oa_4) => ISAJson_encoder_4(studyName_1, undefined, idMap, oa_4)), processes), tryIncludeSeq("assays", (studyName_2 = s_1.Identifier, (a_2) => ISAJson_encoder_5(studyName_2, idMap, a_2)), assays_1), encodedFactors, encodedCharacteristics, encodedUnits, tryIncludeSeq("comments", (comment) => ISAJson_encoder_6(idMap, comment), study.Comments)]));
        return {
            Encode(helpers_7) {
                const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_7)], values);
                return helpers_7.encodeObject(arg);
            },
        };
    };
    if (idMap != null) {
        return encode(ROCrate_genID, f, s, idMap);
    }
    else {
        return f(s);
    }
}

export const ISAJson_allowedFields = ofArray(["@id", "filename", "identifier", "title", "description", "submissionDate", "publicReleaseDate", "publications", "people", "studyDesignDescriptors", "protocols", "materials", "assays", "factors", "characteristicCategories", "unitCategories", "processSequence", "comments", "@type", "@context"]);

export const ISAJson_decoder = Decode_objectNoAdditionalProperties(ISAJson_allowedFields, (get$) => {
    let objectArg, arg_5, objectArg_2, objectArg_5, objectArg_6, objectArg_7, objectArg_8, arg_21, objectArg_9, arg_23, objectArg_10, arg_25, objectArg_11;
    const identifier = defaultArgWith((objectArg = get$.Optional, objectArg.Field("identifier", string)), () => {
        let objectArg_1;
        return defaultArg(bind(Study_tryIdentifierFromFileName, (objectArg_1 = get$.Optional, objectArg_1.Field("filename", string))), createMissingIdentifier());
    });
    const tables = map((arg_6) => {
        const a = ARCtrl_ArcTables__ArcTables_fromProcesses_Static_62A3309D(arg_6);
        return a.Tables;
    }, (arg_5 = list_2(ISAJson_decoder_1), (objectArg_2 = get$.Optional, objectArg_2.Field("processSequence", arg_5))));
    let assays;
    const arg_8 = list_2(ISAJson_decoder_2);
    const objectArg_3 = get$.Optional;
    assays = objectArg_3.Field("assays", arg_8);
    let personsRaw;
    const arg_10 = resizeArray(ISAJson_decoder_3);
    const objectArg_4 = get$.Optional;
    personsRaw = objectArg_4.Field("people", arg_10);
    const persons = [];
    if (personsRaw != null) {
        let enumerator = getEnumerator(value_17(personsRaw));
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const person = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const sourceAssays = Person_getSourceAssayIdentifiersFromComments(person);
                const enumerator_1 = getEnumerator(sourceAssays);
                try {
                    while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                        const assayIdentifier = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                        const enumerator_2 = getEnumerator(value_17(assays));
                        try {
                            while (enumerator_2["System.Collections.IEnumerator.MoveNext"]()) {
                                const assay = enumerator_2["System.Collections.Generic.IEnumerator`1.get_Current"]();
                                if (assay.Identifier === assayIdentifier) {
                                    void (assay.Performers.push(person));
                                }
                            }
                        }
                        finally {
                            disposeSafe(enumerator_2);
                        }
                    }
                }
                finally {
                    disposeSafe(enumerator_1);
                }
                person.Comments = Person_removeSourceAssayComments(person);
                if (isEmpty(sourceAssays)) {
                    void (persons.push(person));
                }
            }
        }
        finally {
            disposeSafe(enumerator);
        }
    }
    const assayIdentifiers = map((arg_11) => {
        const collection = map_2((a_1) => a_1.Identifier, arg_11);
        return Array.from(collection);
    }, assays);
    return [new ArcStudy(identifier, unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("title", string))), unwrap((objectArg_6 = get$.Optional, objectArg_6.Field("description", string))), unwrap((objectArg_7 = get$.Optional, objectArg_7.Field("submissionDate", string))), unwrap((objectArg_8 = get$.Optional, objectArg_8.Field("publicReleaseDate", string))), unwrap((arg_21 = resizeArray(ISAJson_decoder_4), (objectArg_9 = get$.Optional, objectArg_9.Field("publications", arg_21)))), unwrap((persons.length === 0) ? undefined : persons), unwrap((arg_23 = resizeArray(OntologyAnnotation_ISAJson_decoder), (objectArg_10 = get$.Optional, objectArg_10.Field("studyDesignDescriptors", arg_23)))), unwrap(tables), undefined, unwrap(assayIdentifiers), unwrap((arg_25 = resizeArray(ISAJson_decoder_5), (objectArg_11 = get$.Optional, objectArg_11.Field("comments", arg_25))))), defaultArg(assays, empty())];
});

