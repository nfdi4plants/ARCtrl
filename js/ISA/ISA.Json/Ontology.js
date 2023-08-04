import { AnnotationValue } from "../ISA/JsonTypes/AnnotationValue.js";
import { toString, object as object_10, nil } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { array, list as list_1, object as object_11, string, float, int } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library.4.1.4/Choice.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { decoder as decoder_1, encoder } from "./Comment.js";
import { fromString, uri } from "./Decode.js";
import { OntologySourceReference } from "../ISA/JsonTypes/OntologySourceReference.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { ActivePatterns_$007CTermAnnotation$007C_$007C } from "../ISA/Regex.js";
import { bind } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";

export function AnnotationValue_encoder(options, value) {
    if (value instanceof AnnotationValue) {
        if (value.tag === 2) {
            return value.fields[0];
        }
        else if (value.tag === 0) {
            return value.fields[0];
        }
        else {
            return value.fields[0];
        }
    }
    else {
        return nil;
    }
}

export function AnnotationValue_decoder(options, s, json) {
    const matchValue = int(s)(json);
    if (matchValue.tag === 1) {
        const matchValue_1 = float(s, json);
        if (matchValue_1.tag === 1) {
            const matchValue_2 = string(s, json);
            if (matchValue_2.tag === 1) {
                return new FSharpResult$2(1, [matchValue_2.fields[0]]);
            }
            else {
                return new FSharpResult$2(0, [new AnnotationValue(0, [matchValue_2.fields[0]])]);
            }
        }
        else {
            return new FSharpResult$2(0, [new AnnotationValue(1, [matchValue_1.fields[0]])]);
        }
    }
    else {
        return new FSharpResult$2(0, [new AnnotationValue(2, [matchValue.fields[0]])]);
    }
}

export function OntologySourceReference_genID(o) {
    const matchValue = o.File;
    if (matchValue == null) {
        const matchValue_1 = o.Name;
        if (matchValue_1 == null) {
            return "#DummyOntologySourceRef";
        }
        else {
            return "#OntologySourceRef_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return matchValue;
    }
}

export function OntologySourceReference_encoder(options, osr) {
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
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = OntologySourceReference_genID(osr), (typeof value === "string") ? ((s = value, s)) : nil)]) : empty(), delay(() => {
            let value_2, s_1;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_2 = "OntologySourceReference", (typeof value_2 === "string") ? ((s_1 = value_2, s_1)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("description", (value_4) => {
                let s_2;
                const value_5 = value_4;
                return (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil;
            }, osr["Description"])), delay(() => append(singleton(tryInclude("file", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, osr["File"])), delay(() => append(singleton(tryInclude("name", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, osr["Name"])), delay(() => append(singleton(tryInclude("version", (value_13) => {
                let s_5;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_5 = value_14, s_5)) : nil;
            }, osr["Version"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), osr["Comments"]))))))))))));
        }));
    }))));
}

export function OntologySourceReference_decoder(options) {
    return (path_4) => ((v) => object_11((get$) => {
        let objectArg, objectArg_1, objectArg_2, objectArg_3, arg_9, decoder, objectArg_4;
        return new OntologySourceReference((objectArg = get$.Optional, objectArg.Field("description", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("file", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("name", string)), (objectArg_3 = get$.Optional, objectArg_3.Field("version", string)), (arg_9 = ((decoder = decoder_1(options), (path_3) => ((value_3) => list_1(uncurry2(decoder), path_3, value_3)))), (objectArg_4 = get$.Optional, objectArg_4.Field("comments", uncurry2(arg_9)))));
    }, path_4, v));
}

export function OntologySourceReference_fromString(s) {
    return fromString(uncurry2(OntologySourceReference_decoder(ConverterOptions_$ctor())), s);
}

export function OntologySourceReference_toString(oa) {
    return toString(2, OntologySourceReference_encoder(ConverterOptions_$ctor(), oa));
}

/**
 * exports in json-ld format
 */
export function OntologySourceReference_toStringLD(oa) {
    let returnVal;
    return toString(2, OntologySourceReference_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), oa));
}

export function OntologyAnnotation_genID(o) {
    const matchValue = o.ID;
    if (matchValue == null) {
        const matchValue_1 = o.TermAccessionNumber;
        if (matchValue_1 == null) {
            const matchValue_2 = o.TermSourceREF;
            if (matchValue_2 == null) {
                return "#DummyOntologyAnnotation";
            }
            else {
                return "#" + replace(matchValue_2, " ", "_");
            }
        }
        else {
            return matchValue_1;
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function OntologyAnnotation_encoder(options, oa) {
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
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = OntologyAnnotation_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "OntologyAnnotation", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("annotationValue", (value_7) => AnnotationValue_encoder(options, value_7), oa["Name"])), delay(() => append(singleton(tryInclude("termSource", (value_8) => {
                let s_3;
                const value_9 = value_8;
                return (typeof value_9 === "string") ? ((s_3 = value_9, s_3)) : nil;
            }, oa["TermSourceREF"])), delay(() => append(singleton(tryInclude("termAccession", (value_11) => {
                let s_4;
                const value_12 = value_11;
                return (typeof value_12 === "string") ? ((s_4 = value_12, s_4)) : nil;
            }, oa["TermAccessionNumber"])), delay(() => singleton(tryInclude("comments", (comment) => encoder(options, comment), oa["Comments"]))))))))));
        }));
    }))));
}

export function OntologyAnnotation_localIDDecoder(s, json) {
    const matchValue = string(s, json);
    let matchResult, tan;
    if (matchValue.tag === 0) {
        const activePatternResult = ActivePatterns_$007CTermAnnotation$007C_$007C(matchValue.fields[0]);
        if (activePatternResult != null) {
            matchResult = 0;
            tan = activePatternResult;
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
            return new FSharpResult$2(0, [tan.TermSourceREF]);
        default:
            return new FSharpResult$2(0, [""]);
    }
}

export function OntologyAnnotation_decoder(options) {
    return (path_3) => ((v) => object_11((get$) => {
        let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, arg_11, decoder, objectArg_5;
        return new OntologyAnnotation((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("annotationValue", (s_1, json_1) => AnnotationValue_decoder(options, s_1, json_1))), (objectArg_2 = get$.Optional, objectArg_2.Field("termSource", string)), bind((s_3) => {
            if (s_3 === "") {
                return void 0;
            }
            else {
                return s_3;
            }
        }, (objectArg_3 = get$.Optional, objectArg_3.Field("termAccession", OntologyAnnotation_localIDDecoder))), (objectArg_4 = get$.Optional, objectArg_4.Field("termAccession", string)), (arg_11 = ((decoder = decoder_1(options), (path_2) => ((value_2) => array(uncurry2(decoder), path_2, value_2)))), (objectArg_5 = get$.Optional, objectArg_5.Field("comments", uncurry2(arg_11)))));
    }, path_3, v));
}

export function OntologyAnnotation_fromString(s) {
    return fromString(uncurry2(OntologyAnnotation_decoder(ConverterOptions_$ctor())), s);
}

export function OntologyAnnotation_toString(oa) {
    return toString(2, OntologyAnnotation_encoder(ConverterOptions_$ctor(), oa));
}

/**
 * exports in json-ld format
 */
export function OntologyAnnotation_toStringLD(oa) {
    let returnVal;
    return toString(2, OntologyAnnotation_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), oa));
}

