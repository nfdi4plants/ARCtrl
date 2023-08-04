import { toString, nil, object as object_22 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { FSharpList, ofArray, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { value as value_13, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { tryInclude } from "./GEncode.js";
import { Data_decoder, Data_encoder, Sample_decoder, Sample_encoder } from "./Data.js";
import { MaterialAttribute_decoder, MaterialAttribute_encoder, Material_decoder, Material_encoder } from "./Material.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions } from "./ConverterOptions.js";
import { string, IOptionalGetter, IGetters, list as list_1, object as object_23 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { Sample } from "../ISA/JsonTypes/Sample.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { Material } from "../ISA/JsonTypes/Material.js";
import { AssayMaterials } from "../ISA/JsonTypes/AssayMaterials.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Assay } from "../ISA/JsonTypes/Assay.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { Process_decoder, Process_encoder } from "./Process.js";
import { decoder as decoder_5, encoder } from "./Comment.js";
import { fromString, uri } from "./Decode.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { Data } from "../ISA/JsonTypes/Data.js";
import { MaterialAttribute } from "../ISA/JsonTypes/MaterialAttribute.js";
import { Process } from "../ISA/JsonTypes/Process.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { ArcAssay } from "../ISA/ArcTypes/ArcAssay.js";

export function AssayMaterials_encoder(options: ConverterOptions, oa: any): any {
    return object_22(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, ofArray([tryInclude<string>("samples", (oa_1: any): any => Sample_encoder(options, oa_1), oa["Samples"]), tryInclude<string>("otherMaterials", (oa_2: any): any => Material_encoder(options, oa_2), oa["OtherMaterials"])])));
}

export function AssayMaterials_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<AssayMaterials, [string, ErrorReason_$union]>)) {
    return (path_2: string): ((arg0: any) => FSharpResult$2_$union<AssayMaterials, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<AssayMaterials, [string, ErrorReason_$union]> => object_23<AssayMaterials>((get$: IGetters): AssayMaterials => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter;
        return new AssayMaterials((objectArg = get$.Optional, objectArg.Field<FSharpList<Sample>>("samples", (path: string, value: any): FSharpResult$2_$union<FSharpList<Sample>, [string, ErrorReason_$union]> => list_1<Sample>((s: string, json: any): FSharpResult$2_$union<Sample, [string, ErrorReason_$union]> => Sample_decoder(options, s, json), path, value))), (objectArg_1 = get$.Optional, objectArg_1.Field<FSharpList<Material>>("otherMaterials", (path_1: string, value_1: any): FSharpResult$2_$union<FSharpList<Material>, [string, ErrorReason_$union]> => list_1<Material>((s_1: string, json_1: any): FSharpResult$2_$union<Material, [string, ErrorReason_$union]> => Material_decoder(options, s_1, json_1), path_1, value_1))));
    }, path_2, v));
}

export function Assay_genID(a: Assay): string {
    const matchValue: Option<string> = a.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = a.FileName;
        if (matchValue_1 == null) {
            return "#EmptyAssay";
        }
        else {
            return value_13(matchValue_1);
        }
    }
    else {
        return URIModule_toString(value_13(matchValue));
    }
}

export function Assay_encoder(options: ConverterOptions, oa: any): any {
    return object_22(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Assay_genID(oa as Assay), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Assay", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("filename", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["FileName"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("measurementType", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["MeasurementType"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("technologyType", (oa_2: any): any => OntologyAnnotation_encoder(options, oa_2), oa["TechnologyType"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("technologyPlatform", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, oa["TechnologyPlatform"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("dataFiles", (oa_3: any): any => Data_encoder(options, oa_3), oa["DataFiles"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("materials", (oa_4: any): any => AssayMaterials_encoder(options, oa_4), oa["Materials"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("characteristicCategories", (oa_5: any): any => MaterialAttribute_encoder(options, oa_5), oa["CharacteristicCategories"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("unitCategories", (oa_6: any): any => OntologyAnnotation_encoder(options, oa_6), oa["UnitCategories"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("processSequence", (oa_7: any): any => Process_encoder(options, oa_7), oa["ProcessSequence"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), oa["Comments"]))))))))))))))))))))));
        }));
    }))));
}

export function Assay_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Assay, [string, ErrorReason_$union]>)) {
    return (path_7: string): ((arg0: any) => FSharpResult$2_$union<Assay, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Assay, [string, ErrorReason_$union]> => object_23<Assay>((get$: IGetters): Assay => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter, objectArg_5: IOptionalGetter, arg_13: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<AssayMaterials, [string, ErrorReason_$union]>)), objectArg_6: IOptionalGetter, arg_15: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttribute>, [string, ErrorReason_$union]>)), decoder_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttribute, [string, ErrorReason_$union]>)), objectArg_7: IOptionalGetter, arg_17: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>)), decoder_2: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_8: IOptionalGetter, arg_19: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]>)), decoder_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Process, [string, ErrorReason_$union]>)), objectArg_9: IOptionalGetter, arg_21: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder_4: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_10: IOptionalGetter;
        return new Assay((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("filename", string)), (arg_5 = OntologyAnnotation_decoder(options), (objectArg_2 = get$.Optional, objectArg_2.Field<OntologyAnnotation>("measurementType", uncurry2(arg_5)))), (arg_7 = OntologyAnnotation_decoder(options), (objectArg_3 = get$.Optional, objectArg_3.Field<OntologyAnnotation>("technologyType", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field<string>("technologyPlatform", string)), (objectArg_5 = get$.Optional, objectArg_5.Field<FSharpList<Data>>("dataFiles", (path_2: string, value_2: any): FSharpResult$2_$union<FSharpList<Data>, [string, ErrorReason_$union]> => list_1<Data>((s_1: string, json_1: any): FSharpResult$2_$union<Data, [string, ErrorReason_$union]> => Data_decoder(options, s_1, json_1), path_2, value_2))), (arg_13 = AssayMaterials_decoder(options), (objectArg_6 = get$.Optional, objectArg_6.Field<AssayMaterials>("materials", uncurry2(arg_13)))), (arg_15 = ((decoder_1 = MaterialAttribute_decoder(options), (path_3: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttribute>, [string, ErrorReason_$union]>) => ((value_3: any): FSharpResult$2_$union<FSharpList<MaterialAttribute>, [string, ErrorReason_$union]> => list_1<MaterialAttribute>(uncurry2(decoder_1), path_3, value_3)))), (objectArg_7 = get$.Optional, objectArg_7.Field<FSharpList<MaterialAttribute>>("characteristicCategories", uncurry2(arg_15)))), (arg_17 = ((decoder_2 = OntologyAnnotation_decoder(options), (path_4: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>) => ((value_4: any): FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]> => list_1<OntologyAnnotation>(uncurry2(decoder_2), path_4, value_4)))), (objectArg_8 = get$.Optional, objectArg_8.Field<FSharpList<OntologyAnnotation>>("unitCategories", uncurry2(arg_17)))), (arg_19 = ((decoder_3 = Process_decoder(options), (path_5: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]>) => ((value_5: any): FSharpResult$2_$union<FSharpList<Process>, [string, ErrorReason_$union]> => list_1<Process>(uncurry2(decoder_3), path_5, value_5)))), (objectArg_9 = get$.Optional, objectArg_9.Field<FSharpList<Process>>("processSequence", uncurry2(arg_19)))), (arg_21 = ((decoder_4 = decoder_5(options), (path_6: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_6: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder_4), path_6, value_6)))), (objectArg_10 = get$.Optional, objectArg_10.Field<FSharpList<Comment$>>("comments", uncurry2(arg_21)))));
    }, path_7, v));
}

export function Assay_fromString(s: string): Assay {
    return fromString<Assay>(uncurry2(Assay_decoder(ConverterOptions_$ctor())), s);
}

export function Assay_toString(p: Assay): string {
    return toString(2, Assay_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Assay_toStringLD(a: Assay): string {
    let returnVal: ConverterOptions;
    return toString(2, Assay_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a));
}

export function ArcAssay_fromString(s: string): ArcAssay {
    const arg: Assay = fromString<Assay>(uncurry2(Assay_decoder(ConverterOptions_$ctor())), s);
    return ArcAssay.fromAssay(arg);
}

export function ArcAssay_toString(a: ArcAssay): string {
    return toString(2, Assay_encoder(ConverterOptions_$ctor(), a.ToAssay()));
}

/**
 * exports in json-ld format
 */
export function ArcAssay_toStringLD(a: ArcAssay): string {
    let returnVal: ConverterOptions;
    return toString(2, Assay_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a.ToAssay()));
}

