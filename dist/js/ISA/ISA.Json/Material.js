import { MaterialType } from "../ISA/JsonTypes/MaterialType.js";
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
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "./Ontology.js";
import { hasUnknownFields, fromString, uri } from "./Decode.js";
import { MaterialAttribute } from "../ISA/JsonTypes/MaterialAttribute.js";
import { Value_decoder, Value_encoder } from "./Factor.js";
import { MaterialAttributeValue } from "../ISA/JsonTypes/MaterialAttributeValue.js";
import { replace } from "../../fable_modules/fable-library.4.1.4/String.js";
import { Material } from "../ISA/JsonTypes/Material.js";

export function MaterialType_encoder(options, value) {
    if (value instanceof MaterialType) {
        if (value.tag === 1) {
            return "Labeled Extract Name";
        }
        else {
            return "Extract Name";
        }
    }
    else {
        return nil;
    }
}

export function MaterialType_decoder(options, s, json) {
    const matchValue = string(s, json);
    if (matchValue.tag === 1) {
        return new FSharpResult$2(1, [matchValue.fields[0]]);
    }
    else {
        switch (matchValue.fields[0]) {
            case "Extract Name":
                return new FSharpResult$2(0, [new MaterialType(0, [])]);
            case "Labeled Extract Name":
                return new FSharpResult$2(0, [new MaterialType(1, [])]);
            default: {
                const s_1 = matchValue.fields[0];
                return new FSharpResult$2(1, [[`Could not parse ${s_1}No other value than "Extract Name" or "Labeled Extract Name" allowed for materialtype`, new ErrorReason(0, [s_1, nil])]]);
            }
        }
    }
}

export function MaterialAttribute_genID(m) {
    const matchValue = m.ID;
    if (matchValue == null) {
        return "#EmptyMaterialAttribute";
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function MaterialAttribute_encoder(options, oa) {
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
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = MaterialAttribute_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "MaterialAttribute", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => singleton(tryInclude("characteristicType", (oa_1) => OntologyAnnotation_encoder(options, oa_1), oa["CharacteristicType"]))));
        }));
    }))));
}

export function MaterialAttribute_decoder(options) {
    return (path) => ((v) => object_11((get$) => {
        let objectArg, arg_3, objectArg_1;
        return new MaterialAttribute((objectArg = get$.Optional, objectArg.Field("@id", uri)), (arg_3 = OntologyAnnotation_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field("characteristicType", uncurry2(arg_3)))));
    }, path, v));
}

export function MaterialAttribute_fromString(s) {
    return fromString(uncurry2(MaterialAttribute_decoder(ConverterOptions_$ctor())), s);
}

export function MaterialAttribute_toString(m) {
    return toString(2, MaterialAttribute_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function MaterialAttribute_toStringLD(m) {
    let returnVal;
    return toString(2, MaterialAttribute_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

export function MaterialAttributeValue_genID(m) {
    const matchValue = m.ID;
    if (matchValue == null) {
        return "#EmptyMaterialAttributeValue";
    }
    else {
        return URIModule_toString(matchValue);
    }
}

export function MaterialAttributeValue_encoder(options, oa) {
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
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = MaterialAttributeValue_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "MaterialAttributeValue", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("category", (oa_1) => MaterialAttribute_encoder(options, oa_1), oa["Category"])), delay(() => append(singleton(tryInclude("value", (value_7) => Value_encoder(options, value_7), oa["Value"])), delay(() => singleton(tryInclude("unit", (oa_2) => OntologyAnnotation_encoder(options, oa_2), oa["Unit"]))))))));
        }));
    }))));
}

export function MaterialAttributeValue_decoder(options) {
    return (path) => ((v) => object_11((get$) => {
        let objectArg, arg_3, objectArg_1, objectArg_2, arg_7, objectArg_3;
        return new MaterialAttributeValue((objectArg = get$.Optional, objectArg.Field("@id", uri)), (arg_3 = MaterialAttribute_decoder(options), (objectArg_1 = get$.Optional, objectArg_1.Field("category", uncurry2(arg_3)))), (objectArg_2 = get$.Optional, objectArg_2.Field("value", (s_1, json_1) => Value_decoder(options, s_1, json_1))), (arg_7 = OntologyAnnotation_decoder(options), (objectArg_3 = get$.Optional, objectArg_3.Field("unit", uncurry2(arg_7)))));
    }, path, v));
}

export function MaterialAttributeValue_fromString(s) {
    return fromString(uncurry2(MaterialAttributeValue_decoder(ConverterOptions_$ctor())), s);
}

export function MaterialAttributeValue_toString(m) {
    return toString(2, MaterialAttributeValue_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function MaterialAttributeValue_toStringLD(m) {
    let returnVal;
    return toString(2, MaterialAttributeValue_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

export function Material_genID(m) {
    const matchValue = m.ID;
    if (matchValue == null) {
        const matchValue_1 = m.Name;
        if (matchValue_1 == null) {
            return "#EmptyMaterial";
        }
        else {
            return "#Material_" + replace(matchValue_1, " ", "_");
        }
    }
    else {
        return matchValue;
    }
}

export function Material_encoder(options, oa) {
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
        return append(ConverterOptions__get_SetID(options) ? singleton(["@id", (value = Material_genID(oa), (typeof value === "string") ? ((s = value, s)) : nil)]) : singleton(tryInclude("@id", (value_2) => {
            let s_1;
            const value_3 = value_2;
            return (typeof value_3 === "string") ? ((s_1 = value_3, s_1)) : nil;
        }, oa["ID"])), delay(() => {
            let value_5, s_2;
            return append(ConverterOptions__get_IncludeType(options) ? singleton(["@type", (value_5 = "Material", (typeof value_5 === "string") ? ((s_2 = value_5, s_2)) : nil)]) : empty(), delay(() => append(singleton(tryInclude("name", (value_7) => {
                let s_3;
                const value_8 = value_7;
                return (typeof value_8 === "string") ? ((s_3 = value_8, s_3)) : nil;
            }, oa["Name"])), delay(() => append(singleton(tryInclude("type", (value_10) => MaterialType_encoder(options, value_10), oa["MaterialType"])), delay(() => append(singleton(tryInclude("characteristics", (oa_1) => MaterialAttributeValue_encoder(options, oa_1), oa["Characteristics"])), delay(() => singleton(tryInclude("derivesFrom", (oa_2) => Material_encoder(options, oa_2), oa["DerivesFrom"]))))))))));
        }));
    }))));
}

export function Material_decoder(options, s, json) {
    if (hasUnknownFields(ofArray(["@id", "@type", "name", "type", "characteristics", "derivesFrom"]), json)) {
        return new FSharpResult$2(1, [["Unknown fields in material", new ErrorReason(0, [s, nil])]]);
    }
    else {
        return object_11((get$) => {
            let objectArg, objectArg_1, objectArg_2, arg_7, decoder, objectArg_3, objectArg_4;
            return new Material((objectArg = get$.Optional, objectArg.Field("@id", uri)), (objectArg_1 = get$.Optional, objectArg_1.Field("name", string)), (objectArg_2 = get$.Optional, objectArg_2.Field("type", (s_2, json_2) => MaterialType_decoder(options, s_2, json_2))), (arg_7 = ((decoder = MaterialAttributeValue_decoder(options), (path_1) => ((value_1) => list_1(uncurry2(decoder), path_1, value_1)))), (objectArg_3 = get$.Optional, objectArg_3.Field("characteristics", uncurry2(arg_7)))), (objectArg_4 = get$.Optional, objectArg_4.Field("derivesFrom", (path_2, value_2) => list_1((s_3, json_3) => Material_decoder(options, s_3, json_3), path_2, value_2))));
        }, s, json);
    }
}

export function Material_fromString(s) {
    let options;
    return fromString(uncurry2((options = ConverterOptions_$ctor(), (s_1) => ((json) => Material_decoder(options, s_1, json)))), s);
}

export function Material_toString(m) {
    return toString(2, Material_encoder(ConverterOptions_$ctor(), m));
}

/**
 * exports in json-ld format
 */
export function Material_toStringLD(m) {
    let returnVal;
    return toString(2, Material_encoder((returnVal = ConverterOptions_$ctor(), ((ConverterOptions__set_SetID_Z1FBCCD16(returnVal, true), ConverterOptions__set_IncludeType_Z1FBCCD16(returnVal, true)), returnVal)), m));
}

