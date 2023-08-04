import { AnnotationValue_Int, AnnotationValue_Float, AnnotationValue_Text, AnnotationValue_$union, AnnotationValue } from "../ISA/JsonTypes/AnnotationValue.js";
import { toString, object as object_10, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID, ConverterOptions } from "./ConverterOptions.js";
import { IOptionalGetter, IGetters, array, object as object_11, string, float, int } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2_Ok, FSharpResult$2_Error, FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { float64, int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { bind, value as value_16, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { OntologySourceReference } from "../ISA/JsonTypes/OntologySourceReference.js";
import { choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { tryInclude } from "./GEncode.js";
import { decoder as decoder_1, encoder } from "./Comment.js";
import { fromString, uri } from "./Decode.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { ActivePatterns_$007CTermAnnotation$007C_$007C } from "../ISA/Regex.js";

export function AnnotationValue_encoder(options: ConverterOptions, value: any): any {
    if (value instanceof AnnotationValue) {
        if (value.tag === /* Int */ 2) {
            return (value as AnnotationValue<2>).fields[0];
        }
        else if (value.tag === /* Text */ 0) {
            return (value as AnnotationValue<0>).fields[0];
        }
        else {
            return (value as AnnotationValue<1>).fields[0];
        }
    }
    else {
        return nil;
    }
}

export function AnnotationValue_decoder(options: ConverterOptions, s: string, json: any): FSharpResult$2_$union<AnnotationValue_$union, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<int32, [string, ErrorReason_$union]> = int(s)(json);
    if (matchValue.tag === /* Error */ 1) {
        const matchValue_1: FSharpResult$2_$union<float64, [string, ErrorReason_$union]> = float(s, json);
        if (matchValue_1.tag === /* Error */ 1) {
            const matchValue_2: FSharpResult$2_$union<string, [string, ErrorReason_$union]> = string(s, json);
            if (matchValue_2.tag === /* Error */ 1) {
                return FSharpResult$2_Error<AnnotationValue_$union, [string, ErrorReason_$union]>(matchValue_2.fields[0]);
            }
            else {
                return FSharpResult$2_Ok<AnnotationValue_$union, [string, ErrorReason_$union]>(AnnotationValue_Text(matchValue_2.fields[0]));
            }
        }
        else {
            return FSharpResult$2_Ok<AnnotationValue_$union, [string, ErrorReason_$union]>(AnnotationValue_Float(matchValue_1.fields[0]));
        }
    }
    else {
        return FSharpResult$2_Ok<AnnotationValue_$union, [string, ErrorReason_$union]>(AnnotationValue_Int(matchValue.fields[0]));
    }
}

export function OntologySourceReference_genID(o: OntologySourceReference): string {
    const matchValue: Option<string> = o.File;
    if (matchValue == null) {
        const matchValue_1: Option<string> = o.Name;
        if (matchValue_1 == null) {
            return "#DummyOntologySourceRef";
        }
        else {
            return "#OntologySourceRef_" + replace(value_16(matchValue_1), " ", "_");
        }
    }
    else {
        return value_16(matchValue);
    }
}

export function OntologySourceReference_encoder(options: ConverterOptions, osr: any): any {
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
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = OntologySourceReference_genID(osr as OntologySourceReference), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_2: any, s_1: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_2 = "OntologySourceReference", (typeof value_2 === "string") ? ((s_1 = (value_2 as string), s_1)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("description", (value_4: any): any => {
                let s_2: string;
                const value_5: any = value_4;
                return (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil;
            }, osr["Description"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("file", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, osr["File"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, osr["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("version", (value_13: any): any => {
                let s_5: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_5 = (value_14 as string), s_5)) : nil;
            }, osr["Version"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), osr["Comments"]))))))))))));
        }));
    }))));
}

export function OntologySourceReference_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologySourceReference, [string, ErrorReason_$union]>)) {
    return (path_4: string): ((arg0: any) => FSharpResult$2_$union<OntologySourceReference, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<OntologySourceReference, [string, ErrorReason_$union]> => object_11<OntologySourceReference>((get$: IGetters): OntologySourceReference => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, objectArg_3: IOptionalGetter, arg_9: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_4: IOptionalGetter;
        return new OntologySourceReference((objectArg = get$.Optional, objectArg.Field<string>("description", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("file", string)), (objectArg_2 = get$.Optional, objectArg_2.Field<string>("name", string)), (objectArg_3 = get$.Optional, objectArg_3.Field<string>("version", string)), (arg_9 = ((decoder = decoder_1(options), (path_3: string): ((arg0: any) => FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]>) => ((value_3: any): FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]> => array<Comment$>(uncurry2(decoder), path_3, value_3)))), (objectArg_4 = get$.Optional, objectArg_4.Field<Comment$[]>("comments", uncurry2(arg_9)))));
    }, path_4, v));
}

export function OntologySourceReference_fromString(s: string): OntologySourceReference {
    return fromString<OntologySourceReference>(uncurry2(OntologySourceReference_decoder(ConverterOptions_$ctor())), s);
}

export function OntologySourceReference_toString(oa: OntologySourceReference): string {
    return toString(2, OntologySourceReference_encoder(ConverterOptions_$ctor(), oa));
}

/**
 * exports in json-ld format
 */
export function OntologySourceReference_toStringLD(oa: OntologySourceReference): string {
    let returnVal: ConverterOptions;
    return toString(2, OntologySourceReference_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), oa));
}

export function OntologyAnnotation_genID(o: OntologyAnnotation): string {
    const matchValue: Option<string> = o.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = o.TermAccessionNumber;
        if (matchValue_1 == null) {
            const matchValue_2: Option<string> = o.TermSourceREF;
            if (matchValue_2 == null) {
                return "#DummyOntologyAnnotation";
            }
            else {
                return "#" + replace(value_16(matchValue_2), " ", "_");
            }
        }
        else {
            return value_16(matchValue_1);
        }
    }
    else {
        return URIModule_toString(value_16(matchValue));
    }
}

export function OntologyAnnotation_encoder(options: ConverterOptions, oa: any): any {
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
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = OntologyAnnotation_genID(oa as OntologyAnnotation), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "OntologyAnnotation", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("annotationValue", (value_7: any): any => AnnotationValue_encoder(options, value_7), oa["Name"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("termSource", (value_8: any): any => {
                let s_3: string;
                const value_9: any = value_8;
                return (typeof value_9 === "string") ? ((s_3 = (value_9 as string), s_3)) : nil;
            }, oa["TermSourceREF"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("termAccession", (value_11: any): any => {
                let s_4: string;
                const value_12: any = value_11;
                return (typeof value_12 === "string") ? ((s_4 = (value_12 as string), s_4)) : nil;
            }, oa["TermAccessionNumber"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder(options, comment), oa["Comments"]))))))))));
        }));
    }))));
}

export function OntologyAnnotation_localIDDecoder(s: string, json: any): FSharpResult$2_$union<string, [string, ErrorReason_$union]> {
    const matchValue: FSharpResult$2_$union<string, [string, ErrorReason_$union]> = string(s, json);
    let matchResult: int32, tan: { LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string };
    if (matchValue.tag === /* Ok */ 0) {
        const activePatternResult: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTermAnnotation$007C_$007C(matchValue.fields[0]);
        if (activePatternResult != null) {
            matchResult = 0;
            tan = value_16(activePatternResult);
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return FSharpResult$2_Ok<string, [string, ErrorReason_$union]>(tan!.TermSourceREF);
        default:
            return FSharpResult$2_Ok<string, [string, ErrorReason_$union]>("");
    }
}

export function OntologyAnnotation_decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)) {
    return (path_3: string): ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]> => object_11<OntologyAnnotation>((get$: IGetters): OntologyAnnotation => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter, objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter, arg_11: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]>)), decoder: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_5: IOptionalGetter;
        return new OntologyAnnotation((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<AnnotationValue_$union>("annotationValue", (s_1: string, json_1: any): FSharpResult$2_$union<AnnotationValue_$union, [string, ErrorReason_$union]> => AnnotationValue_decoder(options, s_1, json_1))), (objectArg_2 = get$.Optional, objectArg_2.Field<string>("termSource", string)), bind<string, string>((s_3: string): Option<string> => {
            if (s_3 === "") {
                return void 0;
            }
            else {
                return s_3;
            }
        }, (objectArg_3 = get$.Optional, objectArg_3.Field<string>("termAccession", OntologyAnnotation_localIDDecoder))), (objectArg_4 = get$.Optional, objectArg_4.Field<string>("termAccession", string)), (arg_11 = ((decoder = decoder_1(options), (path_2: string): ((arg0: any) => FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]>) => ((value_2: any): FSharpResult$2_$union<Comment$[], [string, ErrorReason_$union]> => array<Comment$>(uncurry2(decoder), path_2, value_2)))), (objectArg_5 = get$.Optional, objectArg_5.Field<Comment$[]>("comments", uncurry2(arg_11)))));
    }, path_3, v));
}

export function OntologyAnnotation_fromString(s: string): OntologyAnnotation {
    return fromString<OntologyAnnotation>(uncurry2(OntologyAnnotation_decoder(ConverterOptions_$ctor())), s);
}

export function OntologyAnnotation_toString(oa: OntologyAnnotation): string {
    return toString(2, OntologyAnnotation_encoder(ConverterOptions_$ctor(), oa));
}

/**
 * exports in json-ld format
 */
export function OntologyAnnotation_toStringLD(oa: OntologyAnnotation): string {
    let returnVal: ConverterOptions;
    return toString(2, OntologyAnnotation_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), oa));
}

