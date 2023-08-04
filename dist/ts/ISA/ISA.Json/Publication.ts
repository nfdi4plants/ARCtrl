import { value as value_16, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { Publication } from "../ISA/JsonTypes/Publication.js";
import { toString as toString_1, nil, object as object_12 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { decoder as decoder_2, encoder as encoder_1 } from "./Comment.js";
import { IOptionalGetter, IGetters, array, string, object as object_13 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString as fromString_1, uri } from "./Decode.js";
import { FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";

export function genID(p: Publication): string {
    const matchValue: Option<string> = p.DOI;
    if (matchValue == null) {
        const matchValue_1: Option<string> = p.PubMedID;
        if (matchValue_1 == null) {
            const matchValue_2: Option<string> = p.Title;
            if (matchValue_2 == null) {
                return "#EmptyPublication";
            }
            else {
                return "#Pub_" + replace(value_16(matchValue_2), " ", "_");
            }
        }
        else {
            return value_16(matchValue_1);
        }
    }
    else {
        return value_16(matchValue);
    }
}

export function encoder(options: ConverterOptions, oa: any): any {
    return object_12(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = genID(oa as Publication), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_2: any, s_1: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_2 = "Publication", (typeof value_2 === "string") ? ((s_1 = (value_2 as string), s_1)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("pubMedID", (value_4: any): any => {
                let s_2: string;
                const value_5: any = value_4;
                return (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil;
            }, oa["PubMedID"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("doi", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["DOI"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("authorList", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, oa["Authors"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("title", (value_13: any): any => {
                let s_5: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_5 = (value_14 as string), s_5)) : nil;
            }, oa["Title"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("status", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["Status"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder_1(options, comment), oa["Comments"]))))))))))))));
        }));
    }))));
}

export function decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Publication, [string, ErrorReason_$union]>)) {
    return (path_4: string): ((arg0: any) => FSharpResult$2_$union<Publication, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Publication, [string, ErrorReason_$union]> => object_13<Publication>((get$: IGetters): Publication => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, objectArg_3: IOptionalGetter, arg_9: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_4: IOptionalGetter, arg_11: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]>)), decoder_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_5: IOptionalGetter;
        return new Publication((objectArg = get$.Optional, objectArg.Field<string>("pubMedID", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("doi", string)), (objectArg_2 = get$.Optional, objectArg_2.Field<string>("authorList", string)), (objectArg_3 = get$.Optional, objectArg_3.Field<string>("title", string)), (arg_9 = OntologyAnnotation_decoder(options), (objectArg_4 = get$.Optional, objectArg_4.Field<OntologyAnnotation>("status", uncurry2(arg_9)))), (arg_11 = ((decoder_1 = decoder_2(options), (path_3: string): ((arg0: any) => FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]>) => ((value_3: any): FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]> => array<Comment$>(uncurry2(decoder_1), path_3, value_3)))), (objectArg_5 = get$.Optional, objectArg_5.Field<Comment$[]>("comments", uncurry2(arg_11)))));
    }, path_4, v));
}

export function fromString(s: string): Publication {
    return fromString_1<Publication>(uncurry2(decoder(ConverterOptions_$ctor())), s);
}

export function toString(p: Publication): string {
    return toString_1(2, encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function toStringLD(p: Publication): string {
    let returnVal: ConverterOptions;
    return toString_1(2, encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

