import { Getters$2__get_Errors, Getters$2_$ctor_Z4BE6C149, string } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { FSharpResult$2 } from "../fable_modules/fable-library-js.4.22.0/Result.js";
import { printf, toText } from "../fable_modules/fable-library-js.4.22.0/String.js";
import { ErrorReason$1 } from "../fable_modules/Thoth.Json.Core.0.4.0/Types.fs.js";
import { exists } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { ofSeq, contains } from "../fable_modules/fable-library-js.4.22.0/Set.js";
import { int32ToString, comparePrimitives } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { head, length, isEmpty } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { fold } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { tryParse, minValue } from "../fable_modules/fable-library-js.4.22.0/Date.js";
import { FSharpRef } from "../fable_modules/fable-library-js.4.22.0/Types.js";

export function Helpers_prependPath(path, err_, err__1) {
    const err = [err_, err__1];
    return [path + err[0], err[1]];
}

export function Decode_isURI(s) {
    return true;
}

export const Decode_uri = {
    Decode(s, json) {
        const matchValue = string.Decode(s, json);
        if (matchValue.tag === 1) {
            return new FSharpResult$2(1, [matchValue.fields[0]]);
        }
        else if (Decode_isURI(matchValue.fields[0])) {
            return new FSharpResult$2(0, [matchValue.fields[0]]);
        }
        else {
            const s_3 = matchValue.fields[0];
            return new FSharpResult$2(1, [[s_3, new ErrorReason$1(6, [toText(printf("Expected URI, got %s"))(s_3)])]]);
        }
    },
};

export function Decode_hasUnknownFields(helpers, knownFields, json) {
    return exists((x) => !contains(x, knownFields), helpers.getProperties(json));
}

export function Decode_objectNoAdditionalProperties(allowedProperties, builder) {
    const allowedProperties_1 = ofSeq(allowedProperties, {
        Compare: comparePrimitives,
    });
    return {
        Decode(helpers, value) {
            const getters = Getters$2_$ctor_Z4BE6C149(helpers, value);
            if (Decode_hasUnknownFields(helpers, allowedProperties_1, value)) {
                return new FSharpResult$2(1, [["Unknown fields in object", new ErrorReason$1(0, ["", value])]]);
            }
            else {
                const result = builder(getters);
                const matchValue = Getters$2__get_Errors(getters);
                if (!isEmpty(matchValue)) {
                    const errors = matchValue;
                    return (length(errors) > 1) ? (new FSharpResult$2(1, [["", new ErrorReason$1(7, [errors])]])) : (new FSharpResult$2(1, [head(matchValue)]));
                }
                else {
                    return new FSharpResult$2(0, [result]);
                }
            }
        },
    };
}

export function Decode_noAdditionalProperties(allowedProperties, decoder) {
    const allowedProperties_1 = ofSeq(allowedProperties, {
        Compare: comparePrimitives,
    });
    return {
        Decode(helpers, value) {
            const getters = Getters$2_$ctor_Z4BE6C149(helpers, value);
            return Decode_hasUnknownFields(helpers, allowedProperties_1, value) ? (new FSharpResult$2(1, [["Unknown fields in object", new ErrorReason$1(0, ["", value])]])) : decoder.Decode(helpers, value);
        },
    };
}

export function Decode_resizeArray(decoder) {
    return {
        Decode(helpers, value) {
            if (helpers.isArray(value)) {
                let i = -1;
                return fold((acc, value_1) => {
                    let tupledArg;
                    i = ((i + 1) | 0);
                    if (acc.tag === 0) {
                        const acc_1 = acc.fields[0];
                        const matchValue = decoder.Decode(helpers, value_1);
                        if (matchValue.tag === 0) {
                            void (acc_1.push(matchValue.fields[0]));
                            return new FSharpResult$2(0, [acc_1]);
                        }
                        else {
                            return new FSharpResult$2(1, [(tupledArg = matchValue.fields[0], Helpers_prependPath((".[" + int32ToString(i)) + "]", tupledArg[0], tupledArg[1]))]);
                        }
                    }
                    else {
                        return acc;
                    }
                }, new FSharpResult$2(0, [[]]), helpers.asArray(value));
            }
            else {
                return new FSharpResult$2(1, [["", new ErrorReason$1(0, ["an array", value])]]);
            }
        },
    };
}

export const Decode_datetime = {
    Decode(helpers, value) {
        if (helpers.isString(value)) {
            let matchValue;
            let outArg = minValue();
            matchValue = [tryParse(helpers.asString(value), new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            return matchValue[0] ? (new FSharpResult$2(0, [matchValue[1]])) : (new FSharpResult$2(1, [["", new ErrorReason$1(0, ["a datetime", value])]]));
        }
        else {
            return new FSharpResult$2(1, [["", new ErrorReason$1(0, ["a datetime", value])]]);
        }
    },
};

