import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { value as value_13 } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { toString as toString_1, nil, object as object_6 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { string, object as object_7 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString as fromString_1, uri } from "./Decode.js";
import { Comment$ } from "../ISA/JsonTypes/Comment.js";

export function genID(c) {
    const matchValue = c.ID;
    if (matchValue == null) {
        const matchValue_1 = c.Name;
        if (matchValue_1 == null) {
            return "#EmptyComment";
        }
        else {
            const n = matchValue_1;
            const v = (c.Value != null) ? ("_" + replace(value_13(c.Value), " ", "_")) : "";
            return ("#Comment_" + replace(n, " ", "_")) + v;
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function encoder(options, comment) {
    return object_6(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = genID(comment), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, comment["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Comment", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, comment["Name"])), delay(() => singleton(tryInclude("value", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, comment["Value"]))))));
        }));
    }))));
}

export function decoder(options) {
    return (path_2) => ((v) => object_7((get$) => {
        let objectArg, objectArg_1, objectArg_2;
        return new Comment$((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("value", string)));
    }, path_2, v));
}

export function fromString(s) {
    return fromString_1(uncurry2(decoder(ConverterOptions_$ctor())), s);
}

export function toString(c) {
    return toString_1(2, encoder(ConverterOptions_$ctor(), c));
}

/**
 * exports in json-ld format
 */
export function toStringLD(c) {
    let returnVal;
    return toString_1(2, encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), c));
}

