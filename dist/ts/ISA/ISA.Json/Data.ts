import { DataFile_ImageFile, DataFile_DerivedDataFile, DataFile_RawDataFile, DataFile_$union, DataFile } from "../ISA/JsonTypes/DataFile.js";
import { toString, object as object_10, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions } from "./ConverterOptions.js";
import { IOptionalGetter, IGetters, list as list_1, object as object_11, string } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_Ok, FSharpResult$2_Error, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_BadPrimitive, ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { value as value_11, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Data } from "../ISA/JsonTypes/Data.js";
import { FSharpList, ofArray, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { tryInclude } from "./GEncode.js";
import { decoder as decoder_3, encoder } from "./Comment.js";
import { fromString, uri, hasUnknownFields } from "./Decode.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { Source } from "../ISA/JsonTypes/Source.js";
import { MaterialAttributeValue_decoder, MaterialAttributeValue_encoder } from "./Material.js";
import { MaterialAttributeValue } from "../ISA/JsonTypes/MaterialAttributeValue.js";
import { Sample } from "../ISA/JsonTypes/Sample.js";
import { FactorValue_decoder, FactorValue_encoder } from "./Factor.js";
import { FactorValue } from "../ISA/JsonTypes/FactorValue.js";

export function DataFile_encoder(options: ConverterOptions, value: any): any {
    if (value instanceof DataFile) {
        if (value.tag === /* DerivedDataFile */ 1) {
            return "Derived Data File";
        }
        else if (value.tag === /* ImageFile */ 2) {
            return "Image File";
        }
        else {
            return "Raw Data File";
        }
    }
    else {
        return nil;
    }
}

export function DataFile_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<DataFile_$union, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<string, [string, ErrorReason_$union]> = string(s, json);
    if (matchValue.tag === /* Error */ 1) {
        return FSharpResult$2_Error<DataFile_$union, [string, ErrorReason_$union]>(matchValue.fields[0]);
    }
    else {
        switch (matchValue.fields[0]) {
            case "Raw Data File":
                return FSharpResult$2_Ok<DataFile_$union, [string, ErrorReason_$union]>(DataFile_RawDataFile());
            case "Derived Data File":
                return FSharpResult$2_Ok<DataFile_$union, [string, ErrorReason_$union]>(DataFile_DerivedDataFile());
            case "Image File":
                return FSharpResult$2_Ok<DataFile_$union, [string, ErrorReason_$union]>(DataFile_ImageFile());
            default: {
                const s_1: string = matchValue.fields[0];
                return FSharpResult$2_Error<DataFile_$union, [string, ErrorReason_$union]>([`Could not parse ${s_1}.`, ErrorReason_BadPrimitive(s_1, nil)] as [string, ErrorReason_$union]);
            }
        }
    }
}

export function Data_genID(d: Data): string {
    const matchValue: Option<string> = d.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = d.Name;
        if (matchValue_1 == null) {
            return "#EmptyData";
        }
        else {
            return value_11(matchValue_1);
        }
    }
    else {
        return URIModule_toString(value_11(matchValue));
    }
}

export function Data_encoder(options: ConverterOptions, oa: any): any {
    return object_10(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Data_genID(oa as Data), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Data", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("type", (value_10: any): any => DataFile_encoder(options, value_10), oa["DataType"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), oa["Comments"]))))))));
        }));
    }))));
}

export function Data_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<Data, [string, ErrorReason_$union]> {
    if (hasUnknownFields(ofArray(["@id", "name", "type", "comments", "@type"]), json)) {
        return FSharpResult$2_Error<Data, [string, ErrorReason_$union]>(["Unknown fields in Data", ErrorReason_BadPrimitive(s, nil)] as [string, ErrorReason_$union]);
    }
    else {
        return object_11<Data>((get$: IGetters): Data => {
            let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter;
            return new Data((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field<DataFile_$union>("type", (s_2: string, json_2: any): FSharpResult$2_$union<DataFile_$union, [string, ErrorReason_$union]> => DataFile_decoder(options, s_2, json_2))), (arg_7 = ((decoder = decoder_3(options), (path_1: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_1: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field<FSharpList<Comment$>>("comments", uncurry2(arg_7)))));
        }, s, json);
    }
}

export function Data_fromString(s: string): Data {
    let options: ConverterOptions;
    return fromString<Data>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<Data, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<Data, [string, ErrorReason_$union]> => Data_decoder(options, s_1, json)))), s);
}

export function Data_toString(m: Data): string {
    return toString(2, Data_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Data_toStringLD(d: Data): string {
    let returnVal: ConverterOptions;
    return toString(2, Data_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), d));
}

export function Source_genID(s: Source): string {
    const matchValue: Option<string> = s.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = s.Name;
        if (matchValue_1 == null) {
            return "#EmptySource";
        }
        else {
            return "#Source_" + replace(value_11(matchValue_1), " ", "_");
        }
    }
    else {
        return URIModule_toString(value_11(matchValue));
    }
}

export function Source_encoder(options: ConverterOptions, oa: any): any {
    return object_10(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s_1: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Source_genID(oa as Source), (typeof value === "string") ? ((s_1 = (value as string), s_1)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_2: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_2 = (value_3 as string), s_2)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_3: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Source", (typeof value_5 === "string") ? ((s_3 = (value_5 as string), s_3)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_4: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_4 = (value_8 as string), s_4)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("characteristics", (oa_1: any): any => MaterialAttributeValue_encoder(options, oa_1), oa["Characteristics"]))))));
        }));
    }))));
}

export function Source_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<Source, [string, ErrorReason_$union]> {
    if (hasUnknownFields(ofArray(["@id", "name", "characteristics", "@type"]), json)) {
        return FSharpResult$2_Error<Source, [string, ErrorReason_$union]>(["Unknown fields in Source", ErrorReason_BadPrimitive(s, nil)] as [string, ErrorReason_$union]);
    }
    else {
        return object_11<Source>((get$: IGetters): Source => {
            let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttributeValue, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter;
            return new Source((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (arg_5 = ((decoder = MaterialAttributeValue_decoder(options), (path_1: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]>) => ((value_1: any): FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]> => list_1<MaterialAttributeValue>(uncurry2(decoder), path_1, value_1)))), (objectArg_2 = get$.Optional, objectArg_2.Field<FSharpList<MaterialAttributeValue>>("characteristics", uncurry2(arg_5)))));
        }, s, json);
    }
}

export function Source_fromString(s: string): Source {
    let options: ConverterOptions;
    return fromString<Source>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<Source, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<Source, [string, ErrorReason_$union]> => Source_decoder(options, s_1, json)))), s);
}

export function Source_toString(m: Source): string {
    return toString(2, Source_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Source_toStringLD(s: Source): string {
    let returnVal: ConverterOptions;
    return toString(2, Source_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), s));
}

export function Sample_genID(s: Sample): string {
    const matchValue: Option<string> = s.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = s.Name;
        if (matchValue_1 == null) {
            return "#EmptySample";
        }
        else {
            return "#Sample_" + replace(value_11(matchValue_1), " ", "_");
        }
    }
    else {
        return value_11(matchValue);
    }
}

export function Sample_encoder(options: ConverterOptions, oa: any): any {
    return object_10(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s_1: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Sample_genID(oa as Sample), (typeof value === "string") ? ((s_1 = (value as string), s_1)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_2: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_2 = (value_3 as string), s_2)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_3: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Sample", (typeof value_5 === "string") ? ((s_3 = (value_5 as string), s_3)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_4: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_4 = (value_8 as string), s_4)) : nil;
            }, oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("characteristics", (oa_1: any): any => MaterialAttributeValue_encoder(options, oa_1), oa["Characteristics"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("factorValues", (oa_2: any): any => FactorValue_encoder(options, oa_2), oa["FactorValues"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("derivesFrom", (oa_3: any): any => Source_encoder(options, oa_3), oa["DerivesFrom"]))))))))));
        }));
    }))));
}

export function Sample_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<Sample, [string, ErrorReason_$union]> {
    if (hasUnknownFields(ofArray(["@id", "name", "characteristics", "factorValues", "derivesFrom", "@type"]), json)) {
        return FSharpResult$2_Error<Sample, [string, ErrorReason_$union]>(["Unknown fields in Sample", ErrorReason_BadPrimitive(s, nil)] as [string, ErrorReason_$union]);
    }
    else {
        return object_11<Sample>((get$: IGetters): Sample => {
            let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, arg_5: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<MaterialAttributeValue, [string, ErrorReason_$union]>)), objectArg_2: IOptionalGetter, arg_7: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<FactorValue>, [string, ErrorReason_$union]>)), decoder_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FactorValue, [string, ErrorReason_$union]>)), objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter;
            return new Sample((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (arg_5 = ((decoder = MaterialAttributeValue_decoder(options), (path_1: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]>) => ((value_1: any): FSharpResult$2_$union<FSharpList<MaterialAttributeValue>, [string, ErrorReason_$union]> => list_1<MaterialAttributeValue>(uncurry2(decoder), path_1, value_1)))), (objectArg_2 = get$.Optional, objectArg_2.Field<FSharpList<MaterialAttributeValue>>("characteristics", uncurry2(arg_5)))), (arg_7 = ((decoder_1 = FactorValue_decoder(options), (path_2: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<FactorValue>, [string, ErrorReason_$union]>) => ((value_2: any): FSharpResult$2_$union<FSharpList<FactorValue>, [string, ErrorReason_$union]> => list_1<FactorValue>(uncurry2(decoder_1), path_2, value_2)))), (objectArg_3 = get$.Optional, objectArg_3.Field<FSharpList<FactorValue>>("factorValues", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field<FSharpList<Source>>("derivesFrom", (path_3: string, value_3: any): FSharpResult$2_$union<FSharpList<Source>, [string, ErrorReason_$union]> => list_1<Source>((s_2: string, json_2: any): FSharpResult$2_$union<Source, [string, ErrorReason_$union]> => Source_decoder(options, s_2, json_2), path_3, value_3))));
        }, s, json);
    }
}

export function Sample_fromString(s: string): Sample {
    let options: ConverterOptions;
    return fromString<Sample>(uncurry2((options = ConverterOptions_$ctor(), (s_1: string): ((arg0: any) => FSharpResult$2_$union<Sample, [string, ErrorReason_$union]>) => ((json: any): FSharpResult$2_$union<Sample, [string, ErrorReason_$union]> => Sample_decoder(options, s_1, json)))), s);
}

export function Sample_toString(m: Sample): string {
    return toString(2, Sample_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Sample_toStringLD(s: Sample): string {
    let returnVal: ConverterOptions;
    return toString(2, Sample_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), s));
}

