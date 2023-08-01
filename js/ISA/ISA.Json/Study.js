import { toString, nil, object as object_36 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { ofArray, choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { tryInclude } from "./GEncode.js";
import { Sample_decoder, Source_decoder, Sample_encoder, Source_encoder } from "./Data.js";
import { MaterialAttribute_decoder, MaterialAttribute_encoder, Material_decoder, Material_encoder } from "./Material.js";
import { string, list as list_1, object as object_37 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { StudyMaterials } from "../ISA/JsonTypes/StudyMaterials.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { decoder as decoder_10, encoder } from "./Publication.js";
import { decoder as decoder_11, encoder as encoder_1 } from "./Person.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { Protocol_decoder, Protocol_encoder } from "./Protocol.js";
import { Process_decoder, Process_encoder } from "./Process.js";
import { Assay_decoder, Assay_encoder } from "./Assay.js";
import { Factor_decoder, Factor_encoder } from "./Factor.js";
import { decoder as decoder_12, encoder as encoder_2 } from "./Comment.js";
import { fromString, uri } from "./Decode.js";
import { Study } from "../ISA/JsonTypes/Study.js";
import { ArcStudy } from "../ISA/ArcTypes/ArcStudy.js";

export function StudyMaterials_encoder(options, oa) {
    return object_36(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, ofArray([tryInclude("sources", (oa_1) => Source_encoder(options, oa_1), oa["Sources"]), tryInclude("samples", (oa_2) => Sample_encoder(options, oa_2), oa["Samples"]), tryInclude("otherMaterials", (oa_3) => Material_encoder(options, oa_3), oa["OtherMaterials"])])));
}

export function StudyMaterials_decoder(options) {
    return (path_3) => ((v) => object_37((get$) => {
        let objectArg, objectArg_1, objectArg_2;
        return new StudyMaterials((objectArg = get$.Optional, objectArg.Field("sources", (path, value) => list_1((s, json) => Source_decoder(options, s, json), path, value))), (objectArg_1 = get$.Optional, objectArg_1.Field("samples", (path_1, value_1) => list_1((s_1, json_1) => Sample_decoder(options, s_1, json_1), path_1, value_1))), (objectArg_2 = get$.Optional, objectArg_2.Field("otherMaterials", (path_2, value_2) => list_1((s_2, json_2) => Material_decoder(options, s_2, json_2), path_2, value_2))));
    }, path_3, v));
}

export function Study_genID(s) {
    const matchValue = s.ID;
    if (matchValue == null) {
        const matchValue_1 = s.FileName;
        if (matchValue_1 == null) {
            const matchValue_2 = s.Identifier;
            if (matchValue_2 == null) {
                const matchValue_3 = s.Title;
                if (matchValue_3 == null) {
                    return "#EmptyStudy";
                }
                else {
                    return "#Study_" + replace(matchValue_3, " ", "_");
                }
            }
            else {
                return "#Study_" + replace(matchValue_2, " ", "_");
            }
        }
        else {
            return "#Study" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Study_encoder(options, oa) {
    return object_36(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s_1;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Study_genID(oa), (typeof value === "string") ? ((s_1 = value, s_1)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_2;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_2 = value_3, s_2)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_3;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Study", (typeof value_5 === "string") ? ((s_3 = value_5, s_3)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("filename", (value_7) => {
                let s_4;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_4 = value_8, s_4)) : nil;
            }, oa["FileName"])), delay(() => append(singleton(tryInclude("identifier", (value_10) => {
                let s_5;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_5 = value_11, s_5)) : nil;
            }, oa["Identifier"])), delay(() => append(singleton(tryInclude("title", (value_13) => {
                let s_6;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_6 = value_14, s_6)) : nil;
            }, oa["Title"])), delay(() => append(singleton(tryInclude("description", (value_16) => {
                let s_7;
                const value_17 = value_16;
                return (typeof value_17 === "string") ? ((s_7 = value_17, s_7)) : nil;
            }, oa["Description"])), delay(() => append(singleton(tryInclude("submissionDate", (value_19) => {
                let s_8;
                const value_20 = value_19;
                return (typeof value_20 === "string") ? ((s_8 = value_20, s_8)) : nil;
            }, oa["SubmissionDate"])), delay(() => append(singleton(tryInclude("publicReleaseDate", (value_22) => {
                let s_9;
                const value_23 = value_22;
                return (typeof value_23 === "string") ? ((s_9 = value_23, s_9)) : nil;
            }, oa["PublicReleaseDate"])), delay(() => append(singleton(tryInclude("publications", (oa_1) => encoder(options, oa_1), oa["Publications"])), delay(() => append(singleton(tryInclude("people", (oa_2) => encoder_1(options, oa_2), oa["Contacts"])), delay(() => append(singleton(tryInclude("studyDesignDescriptors", (oa_3) => OntologyAnnotation_encoder(options, oa_3), oa["StudyDesignDescriptors"])), delay(() => append(singleton(tryInclude("protocols", (oa_4) => Protocol_encoder(options, oa_4), oa["Protocols"])), delay(() => append(singleton(tryInclude("materials", (oa_5) => StudyMaterials_encoder(options, oa_5), oa["Materials"])), delay(() => append(singleton(tryInclude("processSequence", (oa_6) => Process_encoder(options, oa_6), oa["ProcessSequence"])), delay(() => append(singleton(tryInclude("assays", (oa_7) => Assay_encoder(options, oa_7), oa["Assays"])), delay(() => append(singleton(tryInclude("factors", (oa_8) => Factor_encoder(options, oa_8), oa["Factors"])), delay(() => append(singleton(tryInclude("characteristicCategories", (oa_9) => MaterialAttribute_encoder(options, oa_9), oa["CharacteristicCategories"])), delay(() => append(singleton(tryInclude("unitCategories", (oa_10) => OntologyAnnotation_encoder(options, oa_10), oa["UnitCategories"])), delay(() => singleton(tryInclude("comments", (comment) => encoder_2(options, comment), oa["Comments"]))))))))))))))))))))))))))))))))))));
        }));
    }))));
}

export function Study_decoder(options) {
    return (path_16) => ((v) => object_37((get$) => {
        let arg_33, decoder_8, objectArg_16, arg_35, decoder_9, objectArg_17;
        let ID;
        const objectArg = get$.Optional;
        ID = objectArg.Field("@id", uri);
        let FileName;
        const objectArg_1 = get$.Optional;
        FileName = objectArg_1.Field("filename", string);
        let Identifier;
        const objectArg_2 = get$.Optional;
        Identifier = objectArg_2.Field("identifier", string);
        let Title;
        const objectArg_3 = get$.Optional;
        Title = objectArg_3.Field("title", string);
        let Description;
        const objectArg_4 = get$.Optional;
        Description = objectArg_4.Field("description", string);
        let SubmissionDate;
        const objectArg_5 = get$.Optional;
        SubmissionDate = objectArg_5.Field("submissionDate", string);
        let PublicReleaseDate;
        const objectArg_6 = get$.Optional;
        PublicReleaseDate = objectArg_6.Field("publicReleaseDate", string);
        let Publications;
        let arg_15;
        const decoder = decoder_10(options);
        arg_15 = ((path_6) => ((value_6) => list_1(uncurry2(decoder), path_6, value_6)));
        const objectArg_7 = get$.Optional;
        Publications = objectArg_7.Field("publications", uncurry2(arg_15));
        let Contacts;
        let arg_17;
        const decoder_1 = decoder_11(options);
        arg_17 = ((path_7) => ((value_7) => list_1(uncurry2(decoder_1), path_7, value_7)));
        const objectArg_8 = get$.Optional;
        Contacts = objectArg_8.Field("people", uncurry2(arg_17));
        let StudyDesignDescriptors;
        let arg_19;
        const decoder_2 = OntologyAnnotation_decoder(options);
        arg_19 = ((path_8) => ((value_8) => list_1(uncurry2(decoder_2), path_8, value_8)));
        const objectArg_9 = get$.Optional;
        StudyDesignDescriptors = objectArg_9.Field("studyDesignDescriptors", uncurry2(arg_19));
        let Protocols;
        let arg_21;
        const decoder_3 = Protocol_decoder(options);
        arg_21 = ((path_9) => ((value_9) => list_1(uncurry2(decoder_3), path_9, value_9)));
        const objectArg_10 = get$.Optional;
        Protocols = objectArg_10.Field("protocols", uncurry2(arg_21));
        let Materials;
        const arg_23 = StudyMaterials_decoder(options);
        const objectArg_11 = get$.Optional;
        Materials = objectArg_11.Field("materials", uncurry2(arg_23));
        let Assays;
        let arg_25;
        const decoder_4 = Assay_decoder(options);
        arg_25 = ((path_10) => ((value_10) => list_1(uncurry2(decoder_4), path_10, value_10)));
        const objectArg_12 = get$.Optional;
        Assays = objectArg_12.Field("assays", uncurry2(arg_25));
        let Factors;
        let arg_27;
        const decoder_5 = Factor_decoder(options);
        arg_27 = ((path_11) => ((value_11) => list_1(uncurry2(decoder_5), path_11, value_11)));
        const objectArg_13 = get$.Optional;
        Factors = objectArg_13.Field("factors", uncurry2(arg_27));
        let CharacteristicCategories;
        let arg_29;
        const decoder_6 = MaterialAttribute_decoder(options);
        arg_29 = ((path_12) => ((value_12) => list_1(uncurry2(decoder_6), path_12, value_12)));
        const objectArg_14 = get$.Optional;
        CharacteristicCategories = objectArg_14.Field("characteristicCategories", uncurry2(arg_29));
        let UnitCategories;
        let arg_31;
        const decoder_7 = OntologyAnnotation_decoder(options);
        arg_31 = ((path_13) => ((value_13) => list_1(uncurry2(decoder_7), path_13, value_13)));
        const objectArg_15 = get$.Optional;
        UnitCategories = objectArg_15.Field("unitCategories", uncurry2(arg_31));
        return new Study(ID, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, Publications, Contacts, StudyDesignDescriptors, Protocols, Materials, (arg_33 = ((decoder_8 = Process_decoder(options), (path_14) => ((value_14) => list_1(uncurry2(decoder_8), path_14, value_14)))), (objectArg_16 = get$.Optional, objectArg_16.Field("processSequence", uncurry2(arg_33)))), Assays, Factors, CharacteristicCategories, UnitCategories, (arg_35 = ((decoder_9 = decoder_12(options), (path_15) => ((value_15) => list_1(uncurry2(decoder_9), path_15, value_15)))), (objectArg_17 = get$.Optional, objectArg_17.Field("comments", uncurry2(arg_35)))));
    }, path_16, v));
}

export function Study_fromString(s) {
    return fromString(uncurry2(Study_decoder(ConverterOptions_$ctor())), s);
}

export function Study_toString(p) {
    return toString(2, Study_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Study_toStringLD(s) {
    let returnVal;
    return toString(2, Study_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), s));
}

export function ArcStudy_fromString(s) {
    const arg = fromString(uncurry2(Study_decoder(ConverterOptions_$ctor())), s);
    return ArcStudy.fromStudy(arg);
}

export function ArcStudy_toString(a) {
    return toString(2, Study_encoder(ConverterOptions_$ctor(), a.ToStudy()));
}

/**
 * exports in json-ld format
 */
export function ArcStudy_toStringLD(a) {
    let returnVal;
    return toString(2, Study_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a.ToStudy()));
}

