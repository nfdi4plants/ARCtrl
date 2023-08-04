import { DataFile } from "../ISA/JsonTypes/DataFile.js";
import { toString, object as object_10, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { list as list_1, object as object_11, string } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library.4.1.4/Choice.js";
import { ErrorReason } from "../../fable_modules/Thoth.Json.10.1.0/Types.fs.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { ofArray, choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { decoder as decoder_3, encoder } from "./Comment.js";
import { fromString, uri, hasUnknownFields } from "./Decode.js";
import { Data } from "../ISA/JsonTypes/Data.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { MaterialAttributeValue_decoder, MaterialAttributeValue_encoder } from "./Material.js";
import { Source } from "../ISA/JsonTypes/Source.js";
import { FactorValue_decoder, FactorValue_encoder } from "./Factor.js";
import { Sample } from "../ISA/JsonTypes/Sample.js";

export function DataFile_encoder(options, value) {
    if (value instanceof DataFile) {
        if (value.tag === 1) {
            return "Derived Data File";
        }
        else if (value.tag === 2) {
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

export function DataFile_decoder(options, s, json) {
    const matchValue = string(s, json);
    if (matchValue.tag === 1) {
        return new FSharpResult$2(1, [matchValue.fields[0]]);
    }
    else {
        switch (matchValue.fields[0]) {
            case "Raw Data File":
                return new FSharpResult$2(0, [new DataFile(0, [])]);
            case "Derived Data File":
                return new FSharpResult$2(0, [new DataFile(1, [])]);
            case "Image File":
                return new FSharpResult$2(0, [new DataFile(2, [])]);
            default: {
                const s_1 = matchValue.fields[0];
                return new FSharpResult$2(1, [[`Could not parse ${s_1}.`, new ErrorReason(0, [s_1, nil])]]);
            }
        }
    }
}

export function Data_genID(d) {
    const matchValue = d.ID;
    if (matchValue == null) {
        const matchValue_1 = d.Name;
        if (matchValue_1 == null) {
            return "#EmptyData";
        }
        else {
            return matchValue_1;
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Data_encoder(options, oa) {
    return object_10(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Data_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Data", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["Name"])), delay(() => append(singleton(tryInclude("type", (value_10) => DataFile_encoder(options, value_10), oa["DataType"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), oa["Comments"]))))))));
        }));
    }))));
}

export function Data_decoder(options, s, json) {
    if (hasUnknownFields(ofArray(["@id", "name", "type", "comments", "@type"]), json)) {
        return new FSharpResult$2(1, [["Unknown fields in Data", new ErrorReason(0, [s, nil])]]);
    }
    else {
        return object_11((get$) => {
            let objectArg, objectArg_1, objectArg_2, arg_7, decoder, objectArg_3;
            return new Data((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("type", (s_2, json_2) => DataFile_decoder(options, s_2, json_2))), (arg_7 = ((decoder = decoder_3(options), (path_1) => ((value_1) => list_1(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field("comments", uncurry2(arg_7)))));
        }, s, json);
    }
}

export function Data_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => Data_decoder(options, s_1, json)))), s);
}

export function Data_toString(m) {
    return toString(2, Data_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Data_toStringLD(d) {
    let returnVal;
    return toString(2, Data_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), d));
}

export function Source_genID(s) {
    const matchValue = s.ID;
    if (matchValue == null) {
        const matchValue_1 = s.Name;
        if (matchValue_1 == null) {
            return "#EmptySource";
        }
        else {
            return "#Source_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function Source_encoder(options, oa) {
    return object_10(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s_1;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Source_genID(oa), (typeof value === "string") ? ((s_1 = value, s_1)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_2;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_2 = value_3, s_2)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_3;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Source", (typeof value_5 === "string") ? ((s_3 = value_5, s_3)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_4;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_4 = value_8, s_4)) : nil;
            }, oa["Name"])), delay(() => singleton(tryInclude("characteristics", (oa_1) => MaterialAttributeValue_encoder(options, oa_1), oa["Characteristics"]))))));
        }));
    }))));
}

export function Source_decoder(options, s, json) {
    if (hasUnknownFields(ofArray(["@id", "name", "characteristics", "@type"]), json)) {
        return new FSharpResult$2(1, [["Unknown fields in Source", new ErrorReason(0, [s, nil])]]);
    }
    else {
        return object_11((get$) => {
            let objectArg, objectArg_1, arg_5, decoder, objectArg_2;
            return new Source((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (arg_5 = ((decoder = MaterialAttributeValue_decoder(options), (path_1) => ((value_1) => list_1(uncurry2(decoder), path_1, value_1)))), (objectArg_2 = get$.Optional, objectArg_2.Field("characteristics", uncurry2(arg_5)))));
        }, s, json);
    }
}

export function Source_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => Source_decoder(options, s_1, json)))), s);
}

export function Source_toString(m) {
    return toString(2, Source_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Source_toStringLD(s) {
    let returnVal;
    return toString(2, Source_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), s));
}

export function Sample_genID(s) {
    const matchValue = s.ID;
    if (matchValue == null) {
        const matchValue_1 = s.Name;
        if (matchValue_1 == null) {
            return "#EmptySample";
        }
        else {
            return "#Sample_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return matchValue;
    }
}

export function Sample_encoder(options, oa) {
    return object_10(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s_1;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Sample_genID(oa), (typeof value === "string") ? ((s_1 = value, s_1)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_2;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_2 = value_3, s_2)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_3;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Sample", (typeof value_5 === "string") ? ((s_3 = value_5, s_3)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_4;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_4 = value_8, s_4)) : nil;
            }, oa["Name"])), delay(() => append(singleton(tryInclude("characteristics", (oa_1) => MaterialAttributeValue_encoder(options, oa_1), oa["Characteristics"])), delay(() => append(singleton(tryInclude("factorValues", (oa_2) => FactorValue_encoder(options, oa_2), oa["FactorValues"])), delay(() => singleton(tryInclude("derivesFrom", (oa_3) => Source_encoder(options, oa_3), oa["DerivesFrom"]))))))))));
        }));
    }))));
}

export function Sample_decoder(options, s, json) {
    if (hasUnknownFields(ofArray(["@id", "name", "characteristics", "factorValues", "derivesFrom", "@type"]), json)) {
        return new FSharpResult$2(1, [["Unknown fields in Sample", new ErrorReason(0, [s, nil])]]);
    }
    else {
        return object_11((get$) => {
            let objectArg, objectArg_1, arg_5, decoder, objectArg_2, arg_7, decoder_1, objectArg_3, objectArg_4;
            return new Sample((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (arg_5 = ((decoder = MaterialAttributeValue_decoder(options), (path_1) => ((value_1) => list_1(uncurry2(decoder), path_1, value_1)))), (objectArg_2 = get$.Optional, objectArg_2.Field("characteristics", uncurry2(arg_5)))), (arg_7 = ((decoder_1 = FactorValue_decoder(options), (path_2) => ((value_2) => list_1(uncurry2(decoder_1), path_2, value_2)))), (objectArg_3 = get$.Optional, objectArg_3.Field("factorValues", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field("derivesFrom", (path_3, value_3) => list_1((s_2, json_2) => Source_decoder(options, s_2, json_2), path_3, value_3))));
        }, s, json);
    }
}

export function Sample_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => Sample_decoder(options, s_1, json)))), s);
}

export function Sample_toString(m) {
    return toString(2, Sample_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Sample_toStringLD(s) {
    let returnVal;
    return toString(2, Sample_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), s));
}

