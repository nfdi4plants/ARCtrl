import { ofArray, choose } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { unwrap, map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { decoder as decoder_1, encoder as encoder_1 } from "../Data.js";
import { tryInclude } from "../Encode.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "../OntologyAnnotation.js";
import { DataContext_$ctor_Z780A8A2A, DataContext__get_Label, DataContext__get_GeneratedBy, DataContext__get_Description, DataContext__get_ObjectType, DataContext__get_Unit, DataContext__get_Explication } from "../../Core/DataContext.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { string, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";

export function encoder(dc) {
    const values = choose((tupledArg) => map((v_1) => [tupledArg[0], v_1], tupledArg[1]), ofArray([["data", encoder_1(dc)], tryInclude("explication", OntologyAnnotation_encoder, DataContext__get_Explication(dc)), tryInclude("unit", OntologyAnnotation_encoder, DataContext__get_Unit(dc)), tryInclude("objectType", OntologyAnnotation_encoder, DataContext__get_ObjectType(dc)), tryInclude("description", (value) => ({
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    }), DataContext__get_Description(dc)), tryInclude("generatedBy", (value_2) => ({
        Encode(helpers_1) {
            return helpers_1.encodeString(value_2);
        },
    }), DataContext__get_GeneratedBy(dc)), tryInclude("label", (value_4) => ({
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    }), DataContext__get_Label(dc))]));
    return {
        Encode(helpers_3) {
            const arg = map_1((tupledArg_1) => [tupledArg_1[0], tupledArg_1[1].Encode(helpers_3)], values);
            return helpers_3.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6;
    let data;
    const objectArg = get$.Required;
    data = objectArg.Field("data", decoder_1);
    return DataContext_$ctor_Z780A8A2A(unwrap(data.ID), unwrap(data.Name), unwrap(data.DataType), unwrap(data.Format), unwrap(data.SelectorFormat), unwrap((objectArg_1 = get$.Optional, objectArg_1.Field("explication", OntologyAnnotation_decoder))), unwrap((objectArg_2 = get$.Optional, objectArg_2.Field("unit", OntologyAnnotation_decoder))), unwrap((objectArg_3 = get$.Optional, objectArg_3.Field("objectType", OntologyAnnotation_decoder))), unwrap((objectArg_4 = get$.Optional, objectArg_4.Field("label", string))), unwrap((objectArg_5 = get$.Optional, objectArg_5.Field("description", string))), unwrap((objectArg_6 = get$.Optional, objectArg_6.Field("generatedBy", string))), data.Comments);
});

