import { OntologyAnnotation_decoder, OntologyAnnotation_encoder } from "../OntologyAnnotation.js";
import { empty, singleton } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { decoder as decoder_1, encoder as encoder_1 } from "./IOType.js";
import { list } from "../../fable_modules/Thoth.Json.Core.0.4.0/Encode.fs.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { index, string, object } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { CompositeHeader } from "../../Core/Table/CompositeHeader.js";

export function encoder(ch) {
    const oaToJsonString = OntologyAnnotation_encoder;
    const patternInput = (ch.tag === 14) ? ["Comment", singleton({
        Encode(helpers) {
            return helpers.encodeString(ch.fields[0]);
        },
    })] : ((ch.tag === 3) ? ["Parameter", singleton(oaToJsonString(ch.fields[0]))] : ((ch.tag === 2) ? ["Factor", singleton(oaToJsonString(ch.fields[0]))] : ((ch.tag === 1) ? ["Characteristic", singleton(oaToJsonString(ch.fields[0]))] : ((ch.tag === 0) ? ["Component", singleton(oaToJsonString(ch.fields[0]))] : ((ch.tag === 4) ? ["ProtocolType", empty()] : ((ch.tag === 8) ? ["ProtocolREF", empty()] : ((ch.tag === 5) ? ["ProtocolDescription", empty()] : ((ch.tag === 6) ? ["ProtocolUri", empty()] : ((ch.tag === 7) ? ["ProtocolVersion", empty()] : ((ch.tag === 9) ? ["Performer", empty()] : ((ch.tag === 10) ? ["Date", empty()] : ((ch.tag === 11) ? ["Input", singleton(encoder_1(ch.fields[0]))] : ((ch.tag === 12) ? ["Output", singleton(encoder_1(ch.fields[0]))] : [ch.fields[0], empty()])))))))))))));
    const values_1 = [["headertype", {
        Encode(helpers_1) {
            return helpers_1.encodeString(patternInput[0]);
        },
    }], ["values", list(patternInput[1])]];
    return {
        Encode(helpers_2) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_2)], values_1);
            return helpers_2.encodeObject(arg);
        },
    };
}

export const decoder = object((get$) => {
    let arg_7, objectArg_3;
    let headerType;
    const objectArg = get$.Required;
    headerType = objectArg.Field("headertype", string);
    const oa = () => {
        const arg_3 = index(0, OntologyAnnotation_decoder);
        const objectArg_1 = get$.Required;
        return objectArg_1.Field("values", arg_3);
    };
    const io = () => {
        const arg_5 = index(0, decoder_1);
        const objectArg_2 = get$.Required;
        return objectArg_2.Field("values", arg_5);
    };
    return (headerType === "Characteristic") ? (new CompositeHeader(1, [oa()])) : ((headerType === "Parameter") ? (new CompositeHeader(3, [oa()])) : ((headerType === "Component") ? (new CompositeHeader(0, [oa()])) : ((headerType === "Factor") ? (new CompositeHeader(2, [oa()])) : ((headerType === "Input") ? (new CompositeHeader(11, [io()])) : ((headerType === "Output") ? (new CompositeHeader(12, [io()])) : ((headerType === "ProtocolType") ? (new CompositeHeader(4, [])) : ((headerType === "ProtocolREF") ? (new CompositeHeader(8, [])) : ((headerType === "ProtocolDescription") ? (new CompositeHeader(5, [])) : ((headerType === "ProtocolUri") ? (new CompositeHeader(6, [])) : ((headerType === "ProtocolVersion") ? (new CompositeHeader(7, [])) : ((headerType === "Performer") ? (new CompositeHeader(9, [])) : ((headerType === "Date") ? (new CompositeHeader(10, [])) : ((headerType === "Comment") ? (new CompositeHeader(14, [(arg_7 = index(0, string), (objectArg_3 = get$.Required, objectArg_3.Field("values", arg_7)))])) : (new CompositeHeader(13, [headerType])))))))))))))));
});

