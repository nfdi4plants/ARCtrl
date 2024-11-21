import { dateTime } from "./Encode.js";
import { LDObject } from "../ROCrate/LDObject.js";
import { int32ToString, disposeSafe, getEnumerator, isIterable, defaultOf, equals } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { list } from "../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { empty, singleton, append, choose, enumerateFromFunctions, map, delay, toList } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { value as value_10 } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { decimal, int, oneOf, map as map_1, Getters$2__get_Errors, string, Getters$2_$ctor_Z4BE6C149 } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { ofArray, head, length, isEmpty } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { ErrorReason$1 } from "../fable_modules/Thoth.Json.Core.0.4.0/Types.fs.js";
import { FSharpResult$2 } from "../fable_modules/fable-library-js.4.22.0/Result.js";
import { fold } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { Helpers_prependPath } from "./Decode.js";

export function genericEncoder(obj) {
    if (typeof obj === "string") {
        return {
            Encode(helpers) {
                return helpers.encodeString(obj);
            },
        };
    }
    else if (typeof obj === "number") {
        return {
            Encode(helpers_1) {
                return helpers_1.encodeSignedIntegralNumber(obj);
            },
        };
    }
    else if (typeof obj === "boolean") {
        return {
            Encode(helpers_2) {
                return helpers_2.encodeBool(obj);
            },
        };
    }
    else if (typeof obj === "number") {
        return {
            Encode(helpers_3) {
                return helpers_3.encodeDecimalNumber(obj);
            },
        };
    }
    else if (obj instanceof Date) {
        return dateTime(obj);
    }
    else if (obj instanceof LDObject) {
        return encoder(obj);
    }
    else if (equals(obj, defaultOf())) {
        return {
            Encode(helpers_4) {
                return helpers_4.encodeNull();
            },
        };
    }
    else if (isIterable(obj)) {
        const l = obj;
        return list(toList(delay(() => map(genericEncoder, enumerateFromFunctions(() => getEnumerator(l), (enumerator) => enumerator["System.Collections.IEnumerator.MoveNext"](), (enumerator_1) => enumerator_1["System.Collections.IEnumerator.get_Current"]())))));
    }
    else {
        throw new Error("Unknown type");
    }
}

export function encoder(obj) {
    let values;
    const source_2 = choose((kv) => {
        const l = kv[0].toLocaleLowerCase();
        if (((l !== "id") && (l !== "schematype")) && (l !== "additionaltype")) {
            return [kv[0], genericEncoder(kv[1])];
        }
        else {
            return undefined;
        }
    }, obj.GetProperties(true));
    values = append(toList(delay(() => {
        let value;
        return append(singleton(["@id", (value = obj.Id, {
            Encode(helpers) {
                return helpers.encodeString(value);
            },
        })]), delay(() => {
            let value_1;
            return append(singleton(["@type", (value_1 = obj.SchemaType, {
                Encode(helpers_1) {
                    return helpers_1.encodeString(value_1);
                },
            })]), delay(() => {
                let value_2;
                return (obj.AdditionalType != null) ? singleton(["additionalType", (value_2 = value_10(obj.AdditionalType), {
                    Encode(helpers_2) {
                        return helpers_2.encodeString(value_2);
                    },
                })]) : empty();
            }));
        }));
    })), source_2);
    return {
        Encode(helpers_3) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_3)], values);
            return helpers_3.encodeObject(arg);
        },
    };
}

/**
 * Returns a decoder
 * 
 * If expectObject is set to true, decoder fails if top-level value is not an ROCrate object
 */
export function getDecoder(expectObject) {
    const decode = (expectObject_1) => {
        const decodeObject = {
            Decode(helpers, value) {
                let objectArg_1, arg_5, objectArg_2;
                if (helpers.isObject(value)) {
                    const getters = Getters$2_$ctor_Z4BE6C149(helpers, value);
                    const properties = helpers.getProperties(value);
                    let result;
                    const get$ = getters;
                    let t;
                    const objectArg = get$.Required;
                    t = objectArg.Field("@type", string);
                    const o = new LDObject((objectArg_1 = get$.Required, objectArg_1.Field("@id", string)), t);
                    const enumerator = getEnumerator(properties);
                    try {
                        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                            const property = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                            if ((property !== "@id") && (property !== "@type")) {
                                o.SetProperty(property, (arg_5 = decode(false), (objectArg_2 = get$.Required, objectArg_2.Field(property, arg_5))));
                            }
                        }
                    }
                    finally {
                        disposeSafe(enumerator);
                    }
                    result = o;
                    const matchValue = Getters$2__get_Errors(getters);
                    if (!isEmpty(matchValue)) {
                        const errors = matchValue;
                        return (length(errors) > 1) ? (new FSharpResult$2(1, [["", new ErrorReason$1(7, [errors])]])) : (new FSharpResult$2(1, [head(matchValue)]));
                    }
                    else {
                        return new FSharpResult$2(0, [result]);
                    }
                }
                else {
                    return new FSharpResult$2(1, [["", new ErrorReason$1(0, ["an object", value])]]);
                }
            },
        };
        const resizeArray = {
            Decode(helpers_1, value_1) {
                if (helpers_1.isArray(value_1)) {
                    let i = -1;
                    return fold((acc, value_2) => {
                        let tupledArg;
                        i = ((i + 1) | 0);
                        if (acc.tag === 0) {
                            const acc_1 = acc.fields[0];
                            let matchValue_1;
                            let copyOfStruct = decode(false);
                            matchValue_1 = copyOfStruct.Decode(helpers_1, value_2);
                            if (matchValue_1.tag === 0) {
                                void (acc_1.push(matchValue_1.fields[0]));
                                return new FSharpResult$2(0, [acc_1]);
                            }
                            else {
                                return new FSharpResult$2(1, [(tupledArg = matchValue_1.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1]))]);
                            }
                        }
                        else {
                            return acc;
                        }
                    }, new FSharpResult$2(0, [[]]), helpers_1.asArray(value_1));
                }
                else {
                    return new FSharpResult$2(1, [["", new ErrorReason$1(0, ["an array", value_1])]]);
                }
            },
        };
        if (expectObject_1) {
            return map_1((value_4) => value_4, decodeObject);
        }
        else {
            return oneOf(ofArray([map_1((value_5) => value_5, decodeObject), map_1((value_6) => value_6, resizeArray), map_1((value_7) => value_7, string), map_1((value_8) => value_8, int), map_1((value_9) => value_9, decimal)]));
        }
    };
    return decode(expectObject);
}

export const decoder = map_1((value) => value, getDecoder(true));

export const genericDecoder = getDecoder(false);

