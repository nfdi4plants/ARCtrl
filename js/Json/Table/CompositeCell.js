import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "../OntologyAnnotation.js";
import { ofArray, singleton } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { compressedDecoder, compressedEncoder, decoder as decoder_5, encoder as encoder_1 } from "../Data.js";
import { list } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { index, string, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { CompositeCell } from "../../Core/Table/CompositeCell.js";
import { printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { decodeOA, encodeOA } from "./OATable.js";
import { decodeString, encodeString } from "../StringTable.js";

export function encoder(cc) {
    const oaToJsonString = OntologyAnnotation_encoder;
    const patternInput = (cc.tag === 0) ? ["Term", singleton(oaToJsonString(cc.fields[0]))] : ((cc.tag === 2) ? ["Unitized", ofArray([{
        Encode(helpers_1) {
            return helpers_1.encodeString(cc.fields[0]);
        },
    }, oaToJsonString(cc.fields[1])])] : ((cc.tag === 3) ? ["Data", singleton(encoder_1(cc.fields[0]))] : ["FreeText", singleton({
        Encode(helpers) {
            return helpers.encodeString(cc.fields[0]);
        },
    })]));
    const values_1 = [["celltype", {
        Encode(helpers_2) {
            return helpers_2.encodeString(patternInput[0]);
        },
    }], ["values", list(patternInput[1])]];
    return {
        Encode(helpers_3) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_3)], values_1);
            return helpers_3.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let arg_3, objectArg_1, arg_5, objectArg_2, arg_7, objectArg_3, arg_9, objectArg_4, arg_11, objectArg_5;
    let matchValue;
    const objectArg = get$.Required;
    matchValue = objectArg.Field("celltype", string);
    return (matchValue === "FreeText") ? (new CompositeCell(1, [(arg_3 = index(0, string), (objectArg_1 = get$.Required, objectArg_1.Field("values", arg_3)))])) : ((matchValue === "Term") ? (new CompositeCell(0, [(arg_5 = index(0, OntologyAnnotation_decoder), (objectArg_2 = get$.Required, objectArg_2.Field("values", arg_5)))])) : ((matchValue === "Unitized") ? (new CompositeCell(2, [(arg_7 = index(0, string), (objectArg_3 = get$.Required, objectArg_3.Field("values", arg_7))), (arg_9 = index(1, OntologyAnnotation_decoder), (objectArg_4 = get$.Required, objectArg_4.Field("values", arg_9)))])) : ((matchValue === "Data") ? (new CompositeCell(3, [(arg_11 = index(0, decoder_5), (objectArg_5 = get$.Required, objectArg_5.Field("values", arg_11)))])) : toFail(printf("Error reading CompositeCell from json string: %A"))(matchValue))));
});

export function encoderCompressed(stringTable, oaTable, cc) {
    const patternInput = (cc.tag === 0) ? ["Term", singleton(encodeOA(oaTable, cc.fields[0]))] : ((cc.tag === 2) ? ["Unitized", ofArray([encodeString(stringTable, cc.fields[0]), encodeOA(oaTable, cc.fields[1])])] : ((cc.tag === 3) ? ["Data", singleton(compressedEncoder(stringTable, cc.fields[0]))] : ["FreeText", singleton(encodeString(stringTable, cc.fields[0]))]));
    const values_1 = [["t", encodeString(stringTable, patternInput[0])], ["v", list(patternInput[1])]];
    return {
        Encode(helpers) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers)], values_1);
            return helpers.encodeObject(arg);
        },
    };
}

export function decoderCompressed(stringTable, oaTable) {
    return object((get$) => {
        let arg_3, objectArg_1, arg_5, objectArg_2, arg_7, objectArg_3, arg_9, objectArg_4, arg_11, objectArg_5;
        let matchValue;
        const arg_1 = decodeString(stringTable);
        const objectArg = get$.Required;
        matchValue = objectArg.Field("t", arg_1);
        return (matchValue === "FreeText") ? (new CompositeCell(1, [(arg_3 = index(0, decodeString(stringTable)), (objectArg_1 = get$.Required, objectArg_1.Field("v", arg_3)))])) : ((matchValue === "Term") ? (new CompositeCell(0, [(arg_5 = index(0, decodeOA(oaTable)), (objectArg_2 = get$.Required, objectArg_2.Field("v", arg_5)))])) : ((matchValue === "Unitized") ? (new CompositeCell(2, [(arg_7 = index(0, decodeString(stringTable)), (objectArg_3 = get$.Required, objectArg_3.Field("v", arg_7))), (arg_9 = index(1, decodeOA(oaTable)), (objectArg_4 = get$.Required, objectArg_4.Field("v", arg_9)))])) : ((matchValue === "Data") ? (new CompositeCell(3, [(arg_11 = index(0, compressedDecoder(stringTable)), (objectArg_5 = get$.Required, objectArg_5.Field("v", arg_11)))])) : toFail(printf("Error reading CompositeCell from json string: %A"))(matchValue))));
    });
}

