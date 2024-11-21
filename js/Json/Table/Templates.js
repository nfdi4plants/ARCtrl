import { toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { array as array_2, datetimeUtc, resizeArray, guid, object, string, succeed, andThen } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Template, Organisation } from "../../Core/Template.js";
import { decoderCompressed, encoderCompressed, decoder as decoder_1, encoder } from "./ArcTable.js";
import { seq } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { decoder as decoder_2, encoder as encoder_1 } from "../Person.js";
import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "../OntologyAnnotation.js";
import { dateTime } from "../Encode.js";
import { unwrap } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { Decode_datetime } from "../Decode.js";
import { toString as toString_1 } from "../../fable_modules/fable-library-js.4.22.0/Date.js";
import { map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Array.js";

export const Template_Organisation_encoder = (arg) => {
    const value = toString(arg);
    return {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    };
};

export const Template_Organisation_decoder = andThen((textValue) => succeed(Organisation.ofString(textValue)), string);

export function Template_encoder(template) {
    let value_1, copyOfStruct, value_3, value_4, value_5;
    const values_3 = [["id", (value_1 = ((copyOfStruct = template.Id, copyOfStruct)), {
        Encode(helpers) {
            return helpers.encodeString(value_1);
        },
    })], ["table", encoder(template.Table)], ["name", (value_3 = template.Name, {
        Encode(helpers_1) {
            return helpers_1.encodeString(value_3);
        },
    })], ["description", (value_4 = template.Description, {
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    })], ["organisation", Template_Organisation_encoder(template.Organisation)], ["version", (value_5 = template.Version, {
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    })], ["authors", seq(map(encoder_1, template.Authors))], ["endpoint_repositories", seq(map(OntologyAnnotation_encoder, template.EndpointRepositories))], ["tags", seq(map(OntologyAnnotation_encoder, template.Tags))], ["last_updated", dateTime(template.LastUpdated)]];
    return {
        Encode(helpers_4) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_4)], values_3);
            return helpers_4.encodeObject(arg);
        },
    };
}

export const Template_decoder = object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, objectArg_9;
    return Template.create((objectArg = get$.Required, objectArg.Field("id", guid)), (objectArg_1 = get$.Required, objectArg_1.Field("table", decoder_1)), (objectArg_2 = get$.Required, objectArg_2.Field("name", string)), (objectArg_3 = get$.Required, objectArg_3.Field("description", string)), (objectArg_4 = get$.Required, objectArg_4.Field("organisation", Template_Organisation_decoder)), (objectArg_5 = get$.Required, objectArg_5.Field("version", string)), unwrap((arg_13 = resizeArray(decoder_2), (objectArg_6 = get$.Optional, objectArg_6.Field("authors", arg_13)))), unwrap((arg_15 = resizeArray(OntologyAnnotation_decoder), (objectArg_7 = get$.Optional, objectArg_7.Field("endpoint_repositories", arg_15)))), unwrap((arg_17 = resizeArray(OntologyAnnotation_decoder), (objectArg_8 = get$.Optional, objectArg_8.Field("tags", arg_17)))), (objectArg_9 = get$.Required, objectArg_9.Field("last_updated", Decode_datetime)));
});

export function Template_encoderCompressed(stringTable, oaTable, cellTable, template) {
    let value_1, copyOfStruct, value_3, value_4, value_5, value_1_1;
    const values_3 = [["id", (value_1 = ((copyOfStruct = template.Id, copyOfStruct)), {
        Encode(helpers) {
            return helpers.encodeString(value_1);
        },
    })], ["table", encoderCompressed(stringTable, oaTable, cellTable, template.Table)], ["name", (value_3 = template.Name, {
        Encode(helpers_1) {
            return helpers_1.encodeString(value_3);
        },
    })], ["description", (value_4 = template.Description, {
        Encode(helpers_2) {
            return helpers_2.encodeString(value_4);
        },
    })], ["organisation", Template_Organisation_encoder(template.Organisation)], ["version", (value_5 = template.Version, {
        Encode(helpers_3) {
            return helpers_3.encodeString(value_5);
        },
    })], ["authors", seq(map(encoder_1, template.Authors))], ["endpoint_repositories", seq(map(OntologyAnnotation_encoder, template.EndpointRepositories))], ["tags", seq(map(OntologyAnnotation_encoder, template.Tags))], ["last_updated", (value_1_1 = toString_1(template.LastUpdated, "O", {}), {
        Encode(helpers_4) {
            return helpers_4.encodeString(value_1_1);
        },
    })]];
    return {
        Encode(helpers_5) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_5)], values_3);
            return helpers_5.encodeObject(arg);
        },
    };
}

export function Template_decoderCompressed(stringTable, oaTable, cellTable) {
    return object((get$) => {
        let objectArg, arg_3, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, arg_13, objectArg_6, arg_15, objectArg_7, arg_17, objectArg_8, objectArg_9;
        return Template.create((objectArg = get$.Required, objectArg.Field("id", guid)), (arg_3 = decoderCompressed(stringTable, oaTable, cellTable), (objectArg_1 = get$.Required, objectArg_1.Field("table", arg_3))), (objectArg_2 = get$.Required, objectArg_2.Field("name", string)), (objectArg_3 = get$.Required, objectArg_3.Field("description", string)), (objectArg_4 = get$.Required, objectArg_4.Field("organisation", Template_Organisation_decoder)), (objectArg_5 = get$.Required, objectArg_5.Field("version", string)), unwrap((arg_13 = resizeArray(decoder_2), (objectArg_6 = get$.Optional, objectArg_6.Field("authors", arg_13)))), unwrap((arg_15 = resizeArray(OntologyAnnotation_decoder), (objectArg_7 = get$.Optional, objectArg_7.Field("endpoint_repositories", arg_15)))), unwrap((arg_17 = resizeArray(OntologyAnnotation_decoder), (objectArg_8 = get$.Optional, objectArg_8.Field("tags", arg_17)))), (objectArg_9 = get$.Required, objectArg_9.Field("last_updated", datetimeUtc)));
    });
}

export function Templates_encoder(templates) {
    const values = map_1(Template_encoder, templates);
    return {
        Encode(helpers) {
            const arg = map_1((v) => v.Encode(helpers), values);
            return helpers.encodeArray(arg);
        },
    };
}

export const Templates_decoder = array_2(Template_decoder);

