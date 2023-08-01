import { value as value_31, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { choose, tryPick, FSharpList } from "../../fable_modules/fable-library-ts/List.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Person } from "../ISA/JsonTypes/Person.js";
import { toString as toString_1, nil, object as object_22 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { decoder as decoder_3, encoder as encoder_1 } from "./Comment.js";
import { IGetters, list as list_1, string, IOptionalGetter, object as object_23 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString as fromString_1, uri } from "./Decode.js";
import { FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";

export function genID(p: Person): string {
    const matchValue: Option<string> = p.ID;
    if (matchValue == null) {
        let orcid: Option<string>;
        const matchValue_1: Option<FSharpList<Comment$>> = p.Comments;
        orcid = ((matchValue_1 == null) ? void 0 : tryPick<Comment$, string>((c: Comment$): Option<string> => {
            const matchValue_2: Option<string> = c.Name;
            const matchValue_3: Option<string> = c.Value;
            let matchResult: int32, n: string, v: string;
            if (matchValue_2 != null) {
                if (matchValue_3 != null) {
                    matchResult = 0;
                    n = value_31(matchValue_2);
                    v = value_31(matchValue_3);
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
                    if (((n! === "orcid") ? true : (n! === "Orcid")) ? true : (n! === "ORCID")) {
                        return v!;
                    }
                    else {
                        return void 0;
                    }
                default:
                    return void 0;
            }
        }, value_31(matchValue_1)));
        if (orcid == null) {
            const matchValue_5: Option<string> = p.EMail;
            if (matchValue_5 == null) {
                const matchValue_6: Option<string> = p.FirstName;
                const matchValue_7: Option<string> = p.MidInitials;
                const matchValue_8: Option<string> = p.LastName;
                let matchResult_1: int32, fn: string, ln: string, mn: string, fn_1: string, ln_1: string, ln_2: string, fn_2: string;
                if (matchValue_6 == null) {
                    if (matchValue_7 == null) {
                        if (matchValue_8 != null) {
                            matchResult_1 = 2;
                            ln_2 = value_31(matchValue_8);
                        }
                        else {
                            matchResult_1 = 4;
                        }
                    }
                    else {
                        matchResult_1 = 4;
                    }
                }
                else if (matchValue_7 == null) {
                    if (matchValue_8 == null) {
                        matchResult_1 = 3;
                        fn_2 = value_31(matchValue_6);
                    }
                    else {
                        matchResult_1 = 1;
                        fn_1 = value_31(matchValue_6);
                        ln_1 = value_31(matchValue_8);
                    }
                }
                else if (matchValue_8 != null) {
                    matchResult_1 = 0;
                    fn = value_31(matchValue_6);
                    ln = value_31(matchValue_8);
                    mn = value_31(matchValue_7);
                }
                else {
                    matchResult_1 = 4;
                }
                switch (matchResult_1) {
                    case 0:
                        return (((("#" + replace(fn!, " ", "_")) + "_") + replace(mn!, " ", "_")) + "_") + replace(ln!, " ", "_");
                    case 1:
                        return (("#" + replace(fn_1!, " ", "_")) + "_") + replace(ln_1!, " ", "_");
                    case 2:
                        return "#" + replace(ln_2!, " ", "_");
                    case 3:
                        return "#" + replace(fn_2!, " ", "_");
                    default:
                        return "#EmptyPerson";
                }
            }
            else {
                return value_31(matchValue_5);
            }
        }
        else {
            return value_31(orcid);
        }
    }
    else {
        return URIModule_toString(value_31(matchValue));
    }
}

export function encoder(options: ConverterOptions, oa: any): any {
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
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = genID(oa as Person), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, oa["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Person", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("firstName", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, oa["FirstName"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("lastName", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, oa["LastName"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("midInitials", (value_13: any): any => {
                let s_5: string;
                const value_14: any = value_13;
                return (typeof value_14 === "string") ? ((s_5 = (value_14 as string), s_5)) : nil;
            }, oa["MidInitials"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("email", (value_16: any): any => {
                let s_6: string;
                const value_17: any = value_16;
                return (typeof value_17 === "string") ? ((s_6 = (value_17 as string), s_6)) : nil;
            }, oa["EMail"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("phone", (value_19: any): any => {
                let s_7: string;
                const value_20: any = value_19;
                return (typeof value_20 === "string") ? ((s_7 = (value_20 as string), s_7)) : nil;
            }, oa["Phone"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("fax", (value_22: any): any => {
                let s_8: string;
                const value_23: any = value_22;
                return (typeof value_23 === "string") ? ((s_8 = (value_23 as string), s_8)) : nil;
            }, oa["Fax"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("address", (value_25: any): any => {
                let s_9: string;
                const value_26: any = value_25;
                return (typeof value_26 === "string") ? ((s_9 = (value_26 as string), s_9)) : nil;
            }, oa["Address"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("affiliation", (value_28: any): any => {
                let s_10: string;
                const value_29: any = value_28;
                return (typeof value_29 === "string") ? ((s_10 = (value_29 as string), s_10)) : nil;
            }, oa["Affiliation"])), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("roles", (oa_1: any): any => OntologyAnnotation_encoder(options, oa_1), oa["Roles"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("comments", (comment: any): any => encoder_1(options, comment), oa["Comments"]))))))))))))))))))))));
        }));
    }))));
}

export function decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Person, [string, ErrorReason_$union]>)) {
    return (path_10: string): ((arg0: any) => FSharpResult$2_$union<Person, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Person, [string, ErrorReason_$union]> => object_23<Person>((get$: IGetters): Person => {
        let objectArg_2: IOptionalGetter, objectArg_3: IOptionalGetter, objectArg_4: IOptionalGetter, objectArg_5: IOptionalGetter, objectArg_6: IOptionalGetter, objectArg_7: IOptionalGetter, objectArg_8: IOptionalGetter, arg_19: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>)), decoder_1: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<OntologyAnnotation, [string, ErrorReason_$union]>)), objectArg_9: IOptionalGetter, arg_21: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>)), decoder_2: ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)), objectArg_10: IOptionalGetter;
        let ID: Option<string>;
        const objectArg: IOptionalGetter = get$.Optional;
        ID = objectArg.Field<string>("@id", uri);
        let FirstName: Option<string>;
        const objectArg_1: IOptionalGetter = get$.Optional;
        FirstName = objectArg_1.Field<string>("firstName", string);
        return new Person(ID, (objectArg_2 = get$.Optional, objectArg_2.Field<string>("lastName", string)), FirstName, (objectArg_3 = get$.Optional, objectArg_3.Field<string>("midInitials", string)), (objectArg_4 = get$.Optional, objectArg_4.Field<string>("email", string)), (objectArg_5 = get$.Optional, objectArg_5.Field<string>("phone", string)), (objectArg_6 = get$.Optional, objectArg_6.Field<string>("fax", string)), (objectArg_7 = get$.Optional, objectArg_7.Field<string>("address", string)), (objectArg_8 = get$.Optional, objectArg_8.Field<string>("affiliation", string)), (arg_19 = ((decoder_1 = OntologyAnnotation_decoder(options), (path_8: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]>) => ((value_8: any): FSharpResult$2_$union<FSharpList<OntologyAnnotation>, [string, ErrorReason_$union]> => list_1<OntologyAnnotation>(uncurry2(decoder_1), path_8, value_8)))), (objectArg_9 = get$.Optional, objectArg_9.Field<FSharpList<OntologyAnnotation>>("roles", uncurry2(arg_19)))), (arg_21 = ((decoder_2 = decoder_3(options), (path_9: string): ((arg0: any) => FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]>) => ((value_9: any): FSharpResult$2_$union<FSharpList<Comment$>, [string, ErrorReason_$union]> => list_1<Comment$>(uncurry2(decoder_2), path_9, value_9)))), (objectArg_10 = get$.Optional, objectArg_10.Field<FSharpList<Comment$>>("comments", uncurry2(arg_21)))));
    }, path_10, v));
}

export function fromString(s: string): Person {
    return fromString_1<Person>(uncurry2(decoder(ConverterOptions_$ctor())), s);
}

export function toString(p: Person): string {
    return toString_1(2, encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function toStringLD(p: Person): string {
    let returnVal: ConverterOptions;
    return toString_1(2, encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

