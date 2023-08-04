import { tryPick } from "../../fable_modules/fable-library.4.1.4/Array.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { URIModule_toString } from "../ISA/JsonTypes/URI.js";
import { toString as toString_1, nil, object as object_22 } from "../../fable_modules/Thoth.Json.10.1.0/Encode.fs.js";
import { choose } from "../../fable_modules/fable-library.4.1.4/List.js";
import { uncurry2, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { empty, singleton, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { ConverterOptions__set_IncludeType_Z1FBCCD16, ConverterOptions__set_SetID_Z1FBCCD16, ConverterOptions_$ctor, ConverterOptions__get_IncludeType, ConverterOptions__get_SetID } from "./ConverterOptions.js";
import { tryInclude } from "./GEncode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { decoder as decoder_3, encoder as encoder_1 } from "./Comment.js";
import { array as array_1, string, object as object_23 } from "../../fable_modules/Thoth.Json.10.1.0/Decode.fs.js";
import { fromString as fromString_1, uri } from "./Decode.js";
import { Person } from "../ISA/JsonTypes/Person.js";

export function genID(p) {
    const matchValue = p.ID;
    if (matchValue == null) {
        let orcid;
        const matchValue_1 = p.Comments;
        orcid = ((matchValue_1 == null) ? void 0 : tryPick((c) => {
            const matchValue_2 = c.Name;
            const matchValue_3 = c.Value;
            let matchResult, n, v;
            if (matchValue_2 != null) {
                if (matchValue_3 != null) {
                    matchResult = 0;
                    n = matchValue_2;
                    v = matchValue_3;
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
                    if (((n === "orcid") ? true : (n === "Orcid")) ? true : (n === "ORCID")) {
                        return v;
                    }
                    else {
                        return void 0;
                    }
                default:
                    return void 0;
            }
        }, matchValue_1));
        if (orcid == null) {
            const matchValue_5 = p.EMail;
            if (matchValue_5 == null) {
                const matchValue_6 = p.FirstName;
                const matchValue_7 = p.MidInitials;
                const matchValue_8 = p.LastName;
                let matchResult_1, fn, ln, mn, fn_1, ln_1, ln_2, fn_2;
                if (matchValue_6 == null) {
                    if (matchValue_7 == null) {
                        if (matchValue_8 != null) {
                            matchResult_1 = 2;
                            ln_2 = matchValue_8;
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
                        fn_2 = matchValue_6;
                    }
                    else {
                        matchResult_1 = 1;
                        fn_1 = matchValue_6;
                        ln_1 = matchValue_8;
                    }
                }
                else if (matchValue_8 != null) {
                    matchResult_1 = 0;
                    fn = matchValue_6;
                    ln = matchValue_8;
                    mn = matchValue_7;
                }
                else {
                    matchResult_1 = 4;
                }
                switch (matchResult_1) {
                    case 0:
                        return (((("#" + replace(fn, " ", "_")) + "_") + replace(mn, " ", "_")) + "_") + replace(ln, " ", "_");
                    case 1:
                        return (("#" + replace(fn_1, " ", "_")) + "_") + replace(ln_1, " ", "_");
                    case 2:
                        return "#" + replace(ln_2, " ", "_");
                    case 3:
                        return "#" + replace(fn_2, " ", "_");
                    default:
                        return "#EmptyPerson";
                }
            }
            else {
                return matchValue_5;
            }
        }
        else {
            return orcid;
        }
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function encoder(options, oa) {
    return object_22(choose((tupledArg) => {
        const v = tupledArg[1];
        if (equals(v, nil)) {
            return void 0;
        }
        else {
            return [tupledArg[0], v];
        }
    }, toList(delay(() => {
        let value, s;
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Person", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("firstName", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["FirstName"])), delay(() => append(singleton(tryInclude("lastName", (value_10) => {
                let s_4;
                const value_11 = value_10;
                return (typeof value_11 === "string") ? ((s_4 = value_11, s_4)) : nil;
            }, oa["LastName"])), delay(() => append(singleton(tryInclude("midInitials", (value_13) => {
                let s_5;
                const value_14 = value_13;
                return (typeof value_14 === "string") ? ((s_5 = value_14, s_5)) : nil;
            }, oa["MidInitials"])), delay(() => append(singleton(tryInclude("email", (value_16) => {
                let s_6;
                const value_17 = value_16;
                return (typeof value_17 === "string") ? ((s_6 = value_17, s_6)) : nil;
            }, oa["EMail"])), delay(() => append(singleton(tryInclude("phone", (value_19) => {
                let s_7;
                const value_20 = value_19;
                return (typeof value_20 === "string") ? ((s_7 = value_20, s_7)) : nil;
            }, oa["Phone"])), delay(() => append(singleton(tryInclude("fax", (value_22) => {
                let s_8;
                const value_23 = value_22;
                return (typeof value_23 === "string") ? ((s_8 = value_23, s_8)) : nil;
            }, oa["Fax"])), delay(() => append(singleton(tryInclude("address", (value_25) => {
                let s_9;
                const value_26 = value_25;
                return (typeof value_26 === "string") ? ((s_9 = value_26, s_9)) : nil;
            }, oa["Address"])), delay(() => append(singleton(tryInclude("affiliation", (value_28) => {
                let s_10;
                const value_29 = value_28;
                return (typeof value_29 === "string") ? ((s_10 = value_29, s_10)) : nil;
            }, oa["Affiliation"])), delay(() => append(singleton(tryInclude("roles", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["Roles"])), delay(() => singleton(tryInclude("comments", (comment) => encoder_1(options, comment), oa["Comments"]))))))))))))))))))))));
        }));
    }))));
}

export function decoder(options) {
    return (path_10) => ((v) => object_23((get$) => {
        let objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7, objectArg_8, arg_19, decoder_1, objectArg_9, arg_21, decoder_2, objectArg_10;
        let ID;
        const objectArg = get$.Optional;
        ID = objectArg.Field("@id", uri);
        let FirstName;
        const objectArg_1 = get$.Optional;
        FirstName = objectArg_1.Field("firstName", string);
        return new Person(ID, (objectArg_2 = get$.Optional, objectArg_2.Field("lastName", string)), FirstName, (objectArg_3 = get$.Optional, objectArg_3.Field("midInitials", string)), (objectArg_4 = get$.Optional, objectArg_4.Field("email", string)), (objectArg_5 = get$.Optional, objectArg_5.Field("phone", string)), (objectArg_6 = get$.Optional, objectArg_6.Field("fax", string)), (objectArg_7 = get$.Optional, objectArg_7.Field("address", string)), (objectArg_8 = get$.Optional, objectArg_8.Field("affiliation", string)), (arg_19 = ((decoder_1 = OntologyAnnotation_decoder(options), (path_8) => ((value_8) => array_1(uncurry2(decoder_1), path_8, value_8)))), (objectArg_9 = get$.Optional, objectArg_9.Field("roles", uncurry2(arg_19)))), (arg_21 = ((decoder_2 = decoder_3(options), (path_9) => ((value_9) => array_1(uncurry2(decoder_2), path_9, value_9)))), (objectArg_10 = get$.Optional, objectArg_10.Field("comments", uncurry2(arg_21)))));
    }, path_10, v));
}

export function fromString(s) {
    return fromString_1(uncurry2(decoder(ConverterOptions_$ctor())), s);
}

export function toString(p) {
    return toString_1(2, encoder(ConverterOptions_$ctor(), p));
}

/**
 * exports in json-ld format
 */
export function toStringLD(p) {
    let returnVal;
    return toString_1(2, encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), p));
}

