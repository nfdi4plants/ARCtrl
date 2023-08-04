import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { toString, nil, object as object_24 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { empty as empty_1, choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologySourceReference_decoder, OntologySourceReference_encoder } from "./Ontology.js";
import { decoder as decoder_5, encoder } from "./Publication.js";
import { decoder as decoder_6, encoder as encoder_1 } from "./Person.js";
import { Study_decoder, Study_encoder } from "./Study.js";
import { decoder as decoder_7, encoder as encoder_2 } from "./Comment.js";
import { list as list_1, string, object as object_25 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { Investigation } from "../ISA/JsonTypes/Investigation.js";
import { fromString } from "./Decode.js";
import { ArcInvestigation } from "../ISA/ArcTypes/ArcInvestigation.js";

export function Investigation_genID(i) {
    const matchValue = i.ID;
    if (matchValue == null) {
        const matchValue_1 = i.FileName;
        if (matchValue_1 == null) {
            const matchValue_2 = i.Identifier;
            if (matchValue_2 == null) {
                const matchValue_3 = i.Title;
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
            return "#Study_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Investigation_encoder(options, oa) {
    return object_24(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Investigation_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Investigation", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("filename", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["FileName"])), delay(() => append(singleton(tryInclude("identifier", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, oa["Identifier"])), delay(() => append(singleton(tryInclude("title", (value_13) => {
                let s_5;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_5 = value_14, s_5)) : nil;
            }, oa["Title"])), delay(() => append(singleton(tryInclude("description", (value_16) => {
                let s_6;
                const value_17 = value_16;
                return (typeof value_17 === "string") ? ((s_6 = value_17, s_6)) : nil;
            }, oa["Description"])), delay(() => append(singleton(tryInclude("submissionDate", (value_19) => {
                let s_7;
                const value_20 = value_19;
                return (typeof value_20 === "string") ? ((s_7 = value_20, s_7)) : nil;
            }, oa["SubmissionDate"])), delay(() => append(singleton(tryInclude("publicReleaseDate", (value_22) => {
                let s_8;
                const value_23 = value_22;
                return (typeof value_23 === "string") ? ((s_8 = value_23, s_8)) : nil;
            }, oa["PublicReleaseDate"])), delay(() => append(singleton(tryInclude("ontologySourceReferences", (osr) => OntologySourceReference_encoder(options, osr), oa["OntologySourceReferences"])), delay(() => append(singleton(tryInclude("publications", (oa_1) => encoder(options, oa_1), oa["Publications"])), delay(() => append(singleton(tryInclude("people", (oa_2) => encoder_1(options, oa_2), oa["Contacts"])), delay(() => append(singleton(tryInclude("studies", (oa_3) => Study_encoder(options, oa_3), oa["Studies"])), delay(() => singleton(tryInclude("comments", (comment) => encoder_2(options, comment), oa["Comments"]))))))))))))))))))))))));
        }));
    }))));
}

export function Investigation_decoder(options) {
    return (path_12) => ((v) => object_25((get$) => {
        let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, arg_15, decoder, objectArg_7, arg_17, decoder_1, objectArg_8, arg_19, decoder_2, objectArg_9, arg_21, decoder_3, objectArg_10, arg_23, decoder_4, objectArg_11;
        return new Investigation((objectArg = get$.Optional, objectArg.Field("@id", string)), (objectArg_1 = get$.Optional, objectArg_1.Field("filename", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("identifier", string)), (objectArg_3 = get$.Optional, objectArg_3.Field("title", string)), (objectArg_4 = get$.Optional, objectArg_4.Field("description", string)), (objectArg_5 = get$.Optional, objectArg_5.Field("submissionDate", string)), (objectArg_6 = get$.Optional, objectArg_6.Field("publicReleaseDate", string)), (arg_15 = ((decoder = OntologySourceReference_decoder(options), (path_7) => ((value_7) => list_1(uncurry2(decoder), path_7, value_7)))), (objectArg_7 = get$.Optional, objectArg_7.Field("ontologySourceReferences", uncurry2(arg_15)))), (arg_17 = ((decoder_1 = decoder_5(options), (path_8) => ((value_8) => list_1(uncurry2(decoder_1), path_8, value_8)))), (objectArg_8 = get$.Optional, objectArg_8.Field("publications", uncurry2(arg_17)))), (arg_19 = ((decoder_2 = decoder_6(options), (path_9) => ((value_9) => list_1(uncurry2(decoder_2), path_9, value_9)))), (objectArg_9 = get$.Optional, objectArg_9.Field("people", uncurry2(arg_19)))), (arg_21 = ((decoder_3 = Study_decoder(options), (path_10) => ((value_10) => list_1(uncurry2(decoder_3), path_10, value_10)))), (objectArg_10 = get$.Optional, objectArg_10.Field("studies", uncurry2(arg_21)))), (arg_23 = ((decoder_4 = decoder_7(options), (path_11) => ((value_11) => list_1(uncurry2(decoder_4), path_11, value_11)))), (objectArg_11 = get$.Optional, objectArg_11.Field("comments", uncurry2(arg_23)))), empty_1());
    }, path_12, v));
}

export function Investigation_fromString(s) {
    return fromString(uncurry2(Investigation_decoder(ConverterOptions_$ctor())), s);
}

export function Investigation_toString(p) {
    return toString(2, Investigation_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Investigation_toStringLD(i) {
    let returnVal;
    return toString(2, Investigation_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), i));
}

export function ArcInvestigation_fromString(s) {
    const arg = fromString(uncurry2(Investigation_decoder(ConverterOptions_$ctor())), s);
    return ArcInvestigation.fromInvestigation(arg);
}

export function ArcInvestigation_toString(a) {
    return toString(2, Investigation_encoder(ConverterOptions_$ctor(), a.ToInvestigation()));
}

/**
 * exports in json-ld format
 */
export function ArcInvestigation_toStringLD(a) {
    let returnVal;
    return toString(2, Investigation_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a.ToInvestigation()));
}

