import { value as value_25, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Investigation } from "../ISA/JsonTypes/Investigation.js";
import { toString, nil, object as object_24 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { empty as empty_1, FSharpList, choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologySourceReference_decoder, OntologySourceReference_encoder } from "./Ontology.js";
import { decoder as decoder_5, encoder } from "./Publication.js";
import { decoder as decoder_6, encoder as encoder_1 } from "./Person.js";
import { Study_decoder, Study_encoder } from "./Study.js";
import { decoder as decoder_7, encoder as encoder_2 } from "./Comment.js";
import { IOptionalGetter, IGetters, list as list_1, string, object as object_25 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { OntologySourceReference } from "../ISA/JsonTypes/OntologySourceReference.js";
import { Publication } from "../ISA/JsonTypes/Publication.js";
import { Person } from "../ISA/JsonTypes/Person.js";
import { Study } from "../ISA/JsonTypes/Study.js";
import { Remark, Comment$ } from "../ISA/JsonTypes/Comment.js";
import { fromString } from "./Decode.js";
import { ArcInvestigation } from "../ISA/ArcTypes/ArcInvestigation.js";

export function Investigation_genID(i: Investigation): string {
    const matchValue: Option<string> = i.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = i.FileName;
        if (matchValue_1 == null) {
            const matchValue_2: Option<string> = i.Identifier;
            if (matchValue_2 == null) {
                const matchValue_3: Option<string> = i.Title;
                if (matchValue_3 == null) {
                    return "#EmptyStudy";
                }
                else {
                    return "#Study_" + replace(value_25(matchValue_3), " ", "_");
                }
            }
            else {
                return "#Study_" + replace(value_25(matchValue_2), " ", "_");
            }
        }
        else {
            return "#Study_" + replace(value_25(matchValue_1), " ", "_");
        }
    }
    else {
        return URIModule_toString(value_25(matchValue));
    }
}

export function Investigation_encoder(options: ConverterOptions, oa: any): any {
    return object_24(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = Investigation_genID(oa as Investigation), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Investigation", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("filename", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["FileName"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("identifier", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, oa["Identifier"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("title", (value_13: any): any => {
                let s_5: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_5 = (value_14 as string), s_5)) : nil;
            }, oa["Title"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("description", (value_16: any): any => {
                let s_6: string;
                const value_17: any = value_16;
                return (typeof value_17 === "string") ? ((s_6 = (value_17 as string), s_6)) : nil;
            }, oa["Description"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("submissionDate", (value_19: any): any => {
                let s_7: string;
                const value_20: any = value_19;
                return (typeof value_20 === "string") ? ((s_7 = (value_20 as string), s_7)) : nil;
            }, oa["SubmissionDate"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("publicReleaseDate", (value_22: any): any => {
                let s_8: string;
                const value_23: any = value_22;
                return (typeof value_23 === "string") ? ((s_8 = (value_23 as string), s_8)) : nil;
            }, oa["PublicReleaseDate"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("ontologySourceReferences", (osr: any): any => OntologySourceReference_encoder(options, osr), oa["OntologySourceReferences"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("publications", (oa_1: any): any => encoder(options, oa_1), oa["Publications"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("people", (oa_2: any): any => encoder_1(options, oa_2), oa["Contacts"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("studies", (oa_3: any): any => Study_encoder(options, oa_3), oa["Studies"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder_2(options, comment), oa["Comments"]))))))))))))))))))))))));
        }));
    }))));
}

export function Investigation_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Investigation, [string, ErrorReason_$union]>)) {
    return (path_12: string): ((arg0: any) => FSharpResult$2_$union<Investigation, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Investigation, [string, ErrorReason_$union]> => object_25<Investigation>((get$: IGetters): Investigation => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter, objectArg_5: IOptionalGetter, objectArg_6: IOptionalGetter, arg_15: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologySourceReference>, [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologySourceReference, [string, ErrorReason_$union]>)), objectArg_7: IOptionalGetter, arg_17: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Publication>, [string, ErrorReason_$union]>)), decoder_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Publication, [string, ErrorReason_$union]>)), objectArg_8: IOptionalGetter, arg_19: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Person>, [string, ErrorReason_$union]>)), decoder_2: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Person, [string, ErrorReason_$union]>)), objectArg_9: IOptionalGetter, arg_21: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Study>, [string, ErrorReason_$union]>)), decoder_3: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Study, [string, ErrorReason_$union]>)), objectArg_10: IOptionalGetter, arg_23: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder_4: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_11: IOptionalGetter;
        return new Investigation((objectArg = get$.Optional, objectArg.Field<string>("@id", string)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("filename", string)), (objectArg_2 = get$.Optional, objectArg_2.Field<string>("identifier", string)), (objectArg_3 = get$.Optional, objectArg_3.Field<string>("title", string)), (objectArg_4 = get$.Optional, objectArg_4.Field<string>("description", string)), (objectArg_5 = get$.Optional, objectArg_5.Field<string>("submissionDate", string)), (objectArg_6 = get$.Optional, objectArg_6.Field<string>("publicReleaseDate", string)), (arg_15 = ((decoder = OntologySourceReference_decoder(options), (path_7: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologySourceReference>, [string, ErrorReason_$union]>) => ((value_7: any): FSharpResult$2_$union<FSharpList<OntologySourceReference>, [string, ErrorReason_$union]> => list_1<OntologySourceReference>(uncurry2(decoder), path_7, value_7)))), (objectArg_7 = get$.Optional, objectArg_7.Field<FSharpList<OntologySourceReference>>("ontologySourceReferences", uncurry2(arg_15)))), (arg_17 = ((decoder_1 = decoder_5(options), (path_8: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Publication>, [string, ErrorReason_$union]>) => ((value_8: any): FSharpResult$2_$union<FSharpList<Publication>, [string, ErrorReason_$union]> => list_1<Publication>(uncurry2(decoder_1), path_8, value_8)))), (objectArg_8 = get$.Optional, objectArg_8.Field<FSharpList<Publication>>("publications", uncurry2(arg_17)))), (arg_19 = ((decoder_2 = decoder_6(options), (path_9: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Person>, [string, ErrorReason_$union]>) => ((value_9: any): FSharpResult$2_$union<FSharpList<Person>, [string, ErrorReason_$union]> => list_1<Person>(uncurry2(decoder_2), path_9, value_9)))), (objectArg_9 = get$.Optional, objectArg_9.Field<FSharpList<Person>>("people", uncurry2(arg_19)))), (arg_21 = ((decoder_3 = Study_decoder(options), (path_10: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Study>, [string, ErrorReason_$union]>) => ((value_10: any): FSharpResult$2_$union<FSharpList<Study>, [string, ErrorReason_$union]> => list_1<Study>(uncurry2(decoder_3), path_10, value_10)))), (objectArg_10 = get$.Optional, objectArg_10.Field<FSharpList<Study>>("studies", uncurry2(arg_21)))), (arg_23 = ((decoder_4 = decoder_7(options), (path_11: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_11: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder_4), path_11, value_11)))), (objectArg_11 = get$.Optional, objectArg_11.Field<FSharpList<Comment$>>("comments", uncurry2(arg_23)))), empty_1<Remark>());
    }, path_12, v));
}

export function Investigation_fromString(s: string): Investigation {
    return fromString<Investigation>(uncurry2(Investigation_decoder(ConverterOptions_$ctor())), s);
}

export function Investigation_toString(p: Investigation): string {
    return toString(2, Investigation_encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function Investigation_toStringLD(i: Investigation): string {
    let returnVal: ConverterOptions;
    return toString(2, Investigation_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), i));
}

export function ArcInvestigation_fromString(s: string): ArcInvestigation {
    const arg: Investigation = fromString<Investigation>(uncurry2(Investigation_decoder(ConverterOptions_$ctor())), s);
    return ArcInvestigation.fromInvestigation(arg);
}

export function ArcInvestigation_toString(a: ArcInvestigation): string {
    return toString(2, Investigation_encoder(ConverterOptions_$ctor(), a.ToInvestigation()));
}

/**
 * exports in json-ld format
 */
export function ArcInvestigation_toStringLD(a: ArcInvestigation): string {
    let returnVal: ConverterOptions;
    return toString(2, Investigation_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), a.ToInvestigation()));
}

