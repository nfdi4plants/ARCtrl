import { unwrap, map } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ofArray, choose } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { tryInclude } from "./Encode.js";
import { context_jsonvalue } from "./context/rocrate/property_value_context.js";
import { map as map_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { string, object } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { OntologyAnnotation } from "../Core/OntologyAnnotation.js";
import { AnnotationValue_decoder } from "./OntologyAnnotation.js";
import { Value as Value_4 } from "../Core/Value.js";

export function genID(p) {
    const matchValue = p.GetCategory();
    const matchValue_1 = p.GetValue();
    const matchValue_2 = p.GetUnit();
    let matchResult, t, u, v, t_1, v_1;
    if (matchValue != null) {
        if (matchValue_1 != null) {
            if (matchValue_2 == null) {
                matchResult = 1;
                t_1 = matchValue;
                v_1 = matchValue_1;
            }
            else {
                matchResult = 0;
                t = matchValue;
                u = matchValue_2;
                v = matchValue_1;
            }
        }
        else {
            matchResult = 2;
        }
    }
    else {
        matchResult = 2;
    }
    switch (matchResult) {
        case 0:
            return `#${p.GetAdditionalType()}/${t.NameText}=${v.Text}${u.NameText}`;
        case 1:
            return `#${p.GetAdditionalType()}/${t_1.NameText}=${v_1.Text}`;
        default:
            return `#Empty${p.GetAdditionalType()}`;
    }
}

export function encoder(pv) {
    let value_8, value_10;
    let patternInput;
    const matchValue = pv.GetCategory();
    if (matchValue == null) {
        patternInput = [undefined, undefined];
    }
    else {
        const oa = matchValue;
        patternInput = [oa.Name, oa.TermAccessionNumber];
    }
    let patternInput_1;
    const matchValue_1 = pv.GetValue();
    if (matchValue_1 == null) {
        patternInput_1 = [undefined, undefined];
    }
    else {
        const v = matchValue_1;
        switch (v.tag) {
            case 1: {
                patternInput_1 = [{
                    Encode(helpers_1) {
                        return helpers_1.encodeSignedIntegralNumber(v.fields[0]);
                    },
                }, undefined];
                break;
            }
            case 2: {
                patternInput_1 = [{
                    Encode(helpers_2) {
                        return helpers_2.encodeDecimalNumber(v.fields[0]);
                    },
                }, undefined];
                break;
            }
            case 0: {
                const oa_1 = v.fields[0];
                patternInput_1 = [map((value_3) => ({
                    Encode(helpers_3) {
                        return helpers_3.encodeString(value_3);
                    },
                }), oa_1.Name), map((value_5) => ({
                    Encode(helpers_4) {
                        return helpers_4.encodeString(value_5);
                    },
                }), oa_1.TermAccessionNumber)];
                break;
            }
            default:
                patternInput_1 = [{
                    Encode(helpers) {
                        return helpers.encodeString(v.fields[0]);
                    },
                }, undefined];
        }
    }
    let patternInput_2;
    const matchValue_2 = pv.GetUnit();
    if (matchValue_2 == null) {
        patternInput_2 = [undefined, undefined];
    }
    else {
        const oa_2 = matchValue_2;
        patternInput_2 = [oa_2.Name, oa_2.TermAccessionNumber];
    }
    const values = choose((tupledArg) => map((v_1_1) => [tupledArg[0], v_1_1], tupledArg[1]), ofArray([["@id", (value_8 = genID(pv), {
        Encode(helpers_5) {
            return helpers_5.encodeString(value_8);
        },
    })], ["@type", {
        Encode(helpers_6) {
            return helpers_6.encodeString("PropertyValue");
        },
    }], ["additionalType", (value_10 = pv.GetAdditionalType(), {
        Encode(helpers_7) {
            return helpers_7.encodeString(value_10);
        },
    })], tryInclude("alternateName", (value_11) => ({
        Encode(helpers_8) {
            return helpers_8.encodeString(value_11);
        },
    }), pv.AlternateName()), tryInclude("measurementMethod", (value_13) => ({
        Encode(helpers_9) {
            return helpers_9.encodeString(value_13);
        },
    }), pv.MeasurementMethod()), tryInclude("description", (value_15) => ({
        Encode(helpers_10) {
            return helpers_10.encodeString(value_15);
        },
    }), pv.Description()), tryInclude("category", (value_17) => ({
        Encode(helpers_11) {
            return helpers_11.encodeString(value_17);
        },
    }), patternInput[0]), tryInclude("categoryCode", (value_19) => ({
        Encode(helpers_12) {
            return helpers_12.encodeString(value_19);
        },
    }), patternInput[1]), tryInclude("value", (x) => x, patternInput_1[0]), tryInclude("valueCode", (x_1) => x_1, patternInput_1[1]), tryInclude("unit", (value_21) => ({
        Encode(helpers_13) {
            return helpers_13.encodeString(value_21);
        },
    }), patternInput_2[0]), tryInclude("unitCode", (value_23) => ({
        Encode(helpers_14) {
            return helpers_14.encodeString(value_23);
        },
    }), patternInput_2[1]), ["@context", context_jsonvalue]]));
    return {
        Encode(helpers_15) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_15)], values);
            return helpers_15.encodeObject(arg);
        },
    };
}

export function decoder(create) {
    return object((get$) => {
        let value, objectArg_7, code_4, objectArg_8;
        let alternateName;
        const objectArg = get$.Optional;
        alternateName = objectArg.Field("alternateName", string);
        let measurementMethod;
        const objectArg_1 = get$.Optional;
        measurementMethod = objectArg_1.Field("measurementMethod", string);
        let description;
        const objectArg_2 = get$.Optional;
        description = objectArg_2.Field("description", string);
        let category;
        let name;
        const objectArg_3 = get$.Optional;
        name = objectArg_3.Field("category", string);
        let code;
        const objectArg_4 = get$.Optional;
        code = objectArg_4.Field("categoryCode", string);
        let matchResult, code_1;
        if (name == null) {
            if (code != null) {
                if (code === "") {
                    matchResult = 0;
                }
                else {
                    matchResult = 2;
                    code_1 = code;
                }
            }
            else {
                matchResult = 0;
            }
        }
        else if (code != null) {
            if (code === "") {
                matchResult = 1;
            }
            else {
                matchResult = 2;
                code_1 = code;
            }
        }
        else {
            matchResult = 1;
        }
        switch (matchResult) {
            case 0: {
                category = undefined;
                break;
            }
            case 1: {
                try {
                    category = OntologyAnnotation.create(unwrap(name));
                }
                catch (err) {
                    throw new Error(`Error while decoding category (name:${name}): ${err}`);
                }
                break;
            }
            default:
                try {
                    category = OntologyAnnotation.fromTermAnnotation(code_1, unwrap(name));
                }
                catch (err_1) {
                    throw new Error(`Error while decoding category (name:${name}, code:${code_1}): ${err_1}`);
                }
        }
        let unit;
        let name_1;
        const objectArg_5 = get$.Optional;
        name_1 = objectArg_5.Field("unit", string);
        let code_2;
        const objectArg_6 = get$.Optional;
        code_2 = objectArg_6.Field("unitCode", string);
        let matchResult_1, code_3;
        if (name_1 == null) {
            if (code_2 != null) {
                if (code_2 === "") {
                    matchResult_1 = 0;
                }
                else {
                    matchResult_1 = 2;
                    code_3 = code_2;
                }
            }
            else {
                matchResult_1 = 0;
            }
        }
        else if (code_2 != null) {
            if (code_2 === "") {
                matchResult_1 = 1;
            }
            else {
                matchResult_1 = 2;
                code_3 = code_2;
            }
        }
        else {
            matchResult_1 = 1;
        }
        switch (matchResult_1) {
            case 0: {
                unit = undefined;
                break;
            }
            case 1: {
                try {
                    unit = OntologyAnnotation.create(unwrap(name_1));
                }
                catch (err_2) {
                    throw new Error(`Error while decoding unit (name:${name_1}): ${err_2}`);
                }
                break;
            }
            default:
                try {
                    unit = OntologyAnnotation.fromTermAnnotation(code_3, unwrap(name_1));
                }
                catch (err_3) {
                    throw new Error(`Error while decoding unit (name:${name_1}, code:${code_3}): ${err_3}`);
                }
        }
        return create(alternateName, measurementMethod, description, category, (value = ((objectArg_7 = get$.Optional, objectArg_7.Field("value", AnnotationValue_decoder))), (code_4 = ((objectArg_8 = get$.Optional, objectArg_8.Field("valueCode", string))), ((value == null) && (code_4 == null)) ? undefined : (() => {
            try {
                return Value_4.fromOptions(value, undefined, code_4);
            }
            catch (err_4) {
                throw new Error(`Error while decoding value ${value},${code_4}: ${err_4}`);
            }
        })())), unit);
    });
}

