import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { toString as toString_1, nil, object as object_12 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { decoder as decoder_2, encoder as encoder_1 } from "./Comment.js";
import { list as list_1, string, object as object_13 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString as fromString_1, uri } from "./Decode.js";
import { Publication } from "../ISA/JsonTypes/Publication.js";

export function genID(p) {
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

export function encoder(options, oa) {
    return object_12(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : empty(), delay(() => {
            let value_2, s_1;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_2 = "Publication", (typeof value_2 === "string") ? ((s_1 = value_2, s_1)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("pubMedID", (value_4) => {
                let s_2;
                const value_5 = value_4;
                return (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil;
            }, oa["PubMedID"])), delay(() => append(singleton(tryInclude("doi", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["DOI"])), delay(() => append(singleton(tryInclude("authorList", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, oa["Authors"])), delay(() => append(singleton(tryInclude("title", (value_13) => {
                let s_5;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_5 = value_14, s_5)) : nil;
            }, oa["Title"])), delay(() => append(singleton(tryInclude("status", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["Status"])), delay(() => singleton(tryInclude("comments", (comment) => encoder_1(options, comment), oa["Comments"]))))))))))))));
        }));
    }))));
}

export function decoder(options) {
    return (path_4) => ((v) => object_13((get$) => {
        let objectArg, objectArg_1, objectArg_2, objectArg_3, arg_9, objectArg_4, arg_11, decoder_1, objectArg_5;
        return new Publication((objectArg = get$.Optional, objectArg.Field("pubMedID", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("doi", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("authorList", string)), (objectArg_3 = get$.Optional, objectArg_3.Field("title", string)), (arg_9 = OntologyAnnotation_decoder(options), (objectArg_4 = get$.Optional, objectArg_4.Field("status", uncurry2(arg_9)))), (arg_11 = ((decoder_1 = decoder_2(options), (path_3) => ((value_3) => list_1(uncurry2(decoder_1), path_3, value_3)))), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", uncurry2(arg_11)))));
    }, path_4, v));
}

export function fromString(s) {
    return fromString_1(uncurry2(decoder(ConverterOptions_$ctor())), s);
}

export function toString(p) {
    return toString_1(2, encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function toStringLD(p) {
    let returnVal;
    return toString_1(2, encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

