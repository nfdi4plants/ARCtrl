import { OntologyAnnotation_ISAJson_decoder, OntologyAnnotation_ISAJson_encoder } from "../OntologyAnnotation.js";
import { string, float, int, map, oneOf } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { Value } from "../../Core/Value.js";
import { ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";

export function encoder(idMap, value) {
    switch (value.tag) {
        case 1:
            return {
                Encode(helpers_1) {
                    return helpers_1.encodeSignedIntegralNumber(value.fields[0]);
                },
            };
        case 3:
            return {
                Encode(helpers_2) {
                    return helpers_2.encodeString(value.fields[0]);
                },
            };
        case 0:
            return OntologyAnnotation_ISAJson_encoder(idMap, value.fields[0]);
        default:
            return {
                Encode(helpers) {
                    return helpers.encodeDecimalNumber(value.fields[0]);
                },
            };
    }
}

export const decoder = oneOf(ofArray([map((Item) => (new Value(1, [Item])), int), map((Item_1) => (new Value(2, [Item_1])), float), map((Item_2) => (new Value(0, [Item_2])), OntologyAnnotation_ISAJson_decoder), map((Item_3) => (new Value(3, [Item_3])), string)]));

