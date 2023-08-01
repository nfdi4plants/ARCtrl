import { value as value_13, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { replace } from "../../fable_modules/fable-library-ts/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";
import { toString as toString_1, nil, object as object_6 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { choose } from "../../fable_modules/fable-library-ts/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { IOptionalGetter, IGetters, string, object as object_7 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString as fromString_1, uri } from "./Decode.js";
import { FSharpResult$2_$union } from "../../fable_modules/fable-library-ts/Choice.js";
import { ErrorReason_$union } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";

export function genID(c: Comment$): string {
    const matchValue: Option<string> = c.ID;
    if (matchValue == null) {
        const matchValue_1: Option<string> = c.Name;
        if (matchValue_1 == null) {
            return "#EmptyComment";
        }
        else {
            const n: string = value_13(matchValue_1);
            const v: string = (c.Value != null) ? ("_" + replace(value_13(c.Value), " ", "_")) : "";
            return ("#Comment_" + replace(n, " ", "_")) + v;
        }
    }
    else {
        return URIModule_toString(value_13(matchValue));
    }
}

export function encoder(options: ConverterOptions, comment: any): any {
    return object_6(choose<[string, any], [string, any]>((tupledArg: [string, any]): Option<[string, any]> => {
        const v: any = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v] as [string, any];
        }
    }, toList<[string, any]>(delay<[string, any]>((): Iterable<[string, any]> => {
        let value: any, s: string;
        return append<[string, any]>(ConverterOptions__get_SetID(options) ? singleton<[string, any]>(["@id", (value = genID(comment as Comment$), (typeof value === "string") ? ((s = (value as string), s)) : nil)] as [string, any]) : singleton<[string, any]>(tryInclude<string>("@id", (value_2: any): any => {
            let s_1: string;
            const value_3: any = value_2;
            return (typeof value_3 === "string") ? ((s_1 = (value_3 as string), s_1)) : nil;
        }, comment["ID"])), delay<[string, any]>((): Iterable<[string, any]> => {
            let value_5: any, s_2: string;
            return append<[string, any]>(ConverterOptions__get_IncludeType(options) ? singleton<[string, any]>(["@type", (value_5 = "Comment", (typeof value_5 === "string") ? ((s_2 = (value_5 as string), s_2)) : nil)] as [string, any]) : empty<[string, any]>(), delay<[string, any]>((): Iterable<[string, any]> => append<[string, any]>(singleton<[string, any]>(tryInclude<string>("name", (value_7: any): any => {
                let s_3: string;
                const value_8: any = value_7;
                return (typeof value_8 === "string") ? ((s_3 = (value_8 as string), s_3)) : nil;
            }, comment["Name"])), delay<[string, any]>((): Iterable<[string, any]> => singleton<[string, any]>(tryInclude<string>("value", (value_10: any): any => {
                let s_4: string;
                const value_11: any = value_10;
                return (typeof value_11 === "string") ? ((s_4 = (value_11 as string), s_4)) : nil;
            }, comment["Value"]))))));
        }));
    }))));
}

export function decoder(options: ConverterOptions): ((arg0: string) => ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>)) {
    return (path_2: string): ((arg0: any) => FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]>) => ((v: any): FSharpResult$2_$union<Comment$, [string, ErrorReason_$union]> => object_7<Comment$>((get$: IGetters): Comment$ => {
        let objectArg: IOptionalGetter, objectArg_1: IOptionalGetter, objectArg_2: IOptionalGetter;
        return new Comment$((objectArg = get$.Optional, objectArg.Field<string>("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field<string>("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field<string>("value", string)));
    }, path_2, v));
}

export function fromString(s: string): Comment$ {
    return fromString_1<Comment$>(uncurry2(decoder(ConverterOptions_$ctor())), s);
}

export function toString(c: Comment$): string {
    return toString_1(2, encoder(ConverterOptions_$ctor(), c));
}

/**
 * exports in json-ld format
 */
export function toStringLD(c: Comment$): string {
    let returnVal: ConverterOptions;
    return toString_1(2, encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), c));
}

