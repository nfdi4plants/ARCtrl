import { ISAJson_decoder as ISAJson_decoder_2, ISAJson_encoder as ISAJson_encoder_1, ROCrate_decoder as ROCrate_decoder_2, ROCrate_encoder as ROCrate_encoder_1 } from "../Data.js";
import { ISAJson_decoder as ISAJson_decoder_3, ISAJson_encoder as ISAJson_encoder_2, ROCrate_decoder as ROCrate_decoder_3, ROCrate_encoder as ROCrate_encoder_2 } from "./Material.js";
import { ISAJson_decoder as ISAJson_decoder_1, ISAJson_encoder as ISAJson_encoder_3, ROCrate_decoder as ROCrate_decoder_1, ROCrate_encoder as ROCrate_encoder_3 } from "./Sample.js";
import { map, oneOf } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { ProcessOutput } from "../../Core/Process/ProcessOutput.js";
import { ofArray } from "../../fable_modules/fable-library-js.4.22.0/List.js";

export function ROCrate_encoder(value) {
    switch (value.tag) {
        case 1:
            return ROCrate_encoder_1(value.fields[0]);
        case 2:
            return ROCrate_encoder_2(value.fields[0]);
        default:
            return ROCrate_encoder_3(value.fields[0]);
    }
}

export const ROCrate_decoder = oneOf(ofArray([map((Item) => (new ProcessOutput(0, [Item])), ROCrate_decoder_1), map((Item_1) => (new ProcessOutput(1, [Item_1])), ROCrate_decoder_2), map((Item_2) => (new ProcessOutput(2, [Item_2])), ROCrate_decoder_3)]));

export function ISAJson_encoder(idMap, value) {
    switch (value.tag) {
        case 1:
            return ISAJson_encoder_1(idMap, value.fields[0]);
        case 2:
            return ISAJson_encoder_2(idMap, value.fields[0]);
        default:
            return ISAJson_encoder_3(idMap, value.fields[0]);
    }
}

export const ISAJson_decoder = oneOf(ofArray([map((Item) => (new ProcessOutput(0, [Item])), ISAJson_decoder_1), map((Item_1) => (new ProcessOutput(1, [Item_1])), ISAJson_decoder_2), map((Item_2) => (new ProcessOutput(2, [Item_2])), ISAJson_decoder_3)]));

