import { toString, nil, object as object_22 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { ofArray, choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { tryInclude } from "./GEncode.js";
import { Data_decoder, Data_encoder, Sample_decoder, Sample_encoder } from "./Data.js";
import { MaterialAttribute_decoder, MaterialAttribute_encoder, Material_decoder, Material_encoder } from "./Material.js";
import { string, list as list_1, object as object_23 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { AssayMaterials } from "../ISA/JsonTypes/AssayMaterials.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { Process_decoder, Process_encoder } from "./Process.js";
import { decoder as decoder_5, encoder } from "./Comment.js";
import { fromString, uri } from "./Decode.js";
import { Assay } from "../ISA/JsonTypes/Assay.js";
import { ArcAssay } from "../ISA/ArcTypes/ArcAssay.js";

export function AssayMaterials_encoder(options, oa) {
    return object_22(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, ofArray([tryInclude("samples", (oa_1) => Sample_encoder(options, oa_1), oa["Samples"]), tryInclude("otherMaterials", (oa_2) => Material_encoder(options, oa_2), oa["OtherMaterials"])])));
}

export function AssayMaterials_decoder(options) {
    return (path_2) => ((v) => object_23((get$) => {
        let objectArg, objectArg_1;
        return new AssayMaterials((objectArg = get$.Optional, objectArg.Field("samples", (path, value) => list_1((s, json) => Sample_decoder(options, s, json), path, value))), (objectArg_1 = get$.Optional, objectArg_1.Field("otherMaterials", (path_1, value_1) => list_1((s_1, json_1) => Material_decoder(options, s_1, json_1), path_1, value_1))));
    }, path_2, v));
}

export function Assay_genID(a) {
    const matchValue = a.ID;
    if (matchValue == null) {
        const matchValue_1 = a.FileName;
        if (matchValue_1 == null) {
            return "#EmptyAssay";
        }
        else {
            return matchValue_1;
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Assay_encoder(options, oa) {
    return object_22(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Assay_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Assay", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("filename", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["FileName"])), delay(() => append(singleton(tryInclude("measurementType", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["MeasurementType"])), delay(() => append(singleton(tryInclude("technologyType", (oa_2) => OntologyAnnotation_encoder(options, oa_2), oa["TechnologyType"])), delay(() => append(singleton(tryInclude("technologyPlatform", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, oa["TechnologyPlatform"])), delay(() => append(singleton(tryInclude("dataFiles", (oa_3) => Data_encoder(options, oa_3), oa["DataFiles"])), delay(() => append(singleton(tryInclude("materials", (oa_4) => AssayMaterials_encoder(options, oa_4), oa["Materials"])), delay(() => append(singleton(tryInclude("characteristicCategories", (oa_5) => MaterialAttribute_encoder(options, oa_5), oa["CharacteristicCategories"])), delay(() => append(singleton(tryInclude("unitCategories", (oa_6) => OntologyAnnotation_encoder(options, oa_6), oa["UnitCategories"])), delay(() => append(singleton(tryInclude("processSequence", (oa_7) => Process_encoder(options, oa_7), oa["ProcessSequence"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), oa["Comments"]))))))))))))))))))))));
        }));
    }))));
}

export function Assay_decoder(options) {
    return (path_7) => ((v) => object_23((get$) => {
        let objectArg, objectArg_1, arg_5, objectArg_2, arg_7, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, decoder_1, objectArg_7, arg_17, decoder_2, objectArg_8, arg_19, decoder_3, objectArg_9, arg_21, decoder_4, objectArg_10;
        return new Assay((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("filename", string)), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field("measurementType", uncurry2(arg_5)))), (arg_7 = OntologyAnnotation_decoder(options), (objectArg_3 = get$.Optional, objectArg_3.Field("technologyType", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field("technologyPlatform", string)), (objectArg_5 = get$.Optional, objectArg_5.Field("dataFiles", (path_2, value_2) => list_1((s_1, json_1) => Data_decoder(options, s_1, json_1), path_2, value_2))), (arg_13 = AssayMaterials_decoder(options), (objectArg_6 = get$.Optional, objectArg_6.Field("materials", uncurry2(arg_13)))), (arg_15 = ((decoder_1 = MaterialAttribute_decoder(options), (path_3) => ((value_3) => list_1(uncurry2(decoder_1), path_3, value_3)))), (objectArg_7 = get$.Optional, objectArg_7.Field("characteristicCategories", uncurry2(arg_15)))), (arg_17 = ((decoder_2 = OntologyAnnotation_decoder(options), (path_4) => ((value_4) => list_1(uncurry2(decoder_2), path_4, value_4)))), (objectArg_8 = get$.Optional, objectArg_8.Field("unitCategories", uncurry2(arg_17)))), (arg_19 = ((decoder_3 = Process_decoder(options), (path_5) => ((value_5) => list_1(uncurry2(decoder_3), path_5, value_5)))), (objectArg_9 = get$.Optional, objectArg_9.Field("processSequence", uncurry2(arg_19)))), (arg_21 = ((decoder_4 = decoder_5(options), (path_6) => ((value_6) => list_1(uncurry2(decoder_4), path_6, value_6)))), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", uncurry2(arg_21)))));
    }, path_7, v));
}

export function Assay_fromString(s) {
    return fromString(uncurry2(Assay_decoder(ConverterOptions_$ctor())), s);
}

export function Assay_toString(p) {
    return toString(2, Assay_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Assay_toStringLD(a) {
    let returnVal;
    return toString(2, Assay_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a));
}

export function ArcAssay_fromString(s) {
    const arg = fromString(uncurry2(Assay_decoder(ConverterOptions_$ctor())), s);
    return ArcAssay.fromAssay(arg);
}

export function ArcAssay_toString(a) {
    return toString(2, Assay_encoder(ConverterOptions_$ctor(), a.ToAssay()));
}

/**
 * exports in json-ld format
 */
export function ArcAssay_toStringLD(a) {
    let returnVal;
    return toString(2, Assay_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a.ToAssay()));
}

