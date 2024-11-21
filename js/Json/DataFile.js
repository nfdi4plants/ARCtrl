import { string } from "../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { FSharpResult$2 } from "../fable_modules/fable-library-js.4.22.0/Result.js";
import { DataFile } from "../Core/DataFile.js";
import { ErrorReason$1 } from "../fable_modules/Thoth.Json.Core.0.4.0/Types.fs.js";

export function ROCrate_encoder(value) {
    switch (value.tag) {
        case 1:
            return {
                Encode(helpers_1) {
                    return helpers_1.encodeString("Derived Data File");
                },
            };
        case 2:
            return {
                Encode(helpers_2) {
                    return helpers_2.encodeString("Image File");
                },
            };
        default:
            return {
                Encode(helpers) {
                    return helpers.encodeString("Raw Data File");
                },
            };
    }
}

export const ROCrate_decoder = {
    Decode(s, json) {
        const matchValue = string.Decode(s, json);
        if (matchValue.tag === 1) {
            return new FSharpResult$2(1, [matchValue.fields[0]]);
        }
        else {
            switch (matchValue.fields[0]) {
                case "Raw Data File":
                    return new FSharpResult$2(0, [new DataFile(0, [])]);
                case "Derived Data File":
                    return new FSharpResult$2(0, [new DataFile(1, [])]);
                case "Image File":
                    return new FSharpResult$2(0, [new DataFile(2, [])]);
                default: {
                    const s_1 = matchValue.fields[0];
                    return new FSharpResult$2(1, [[`Could not parse ${s_1}.`, new ErrorReason$1(0, [s_1, json])]]);
                }
            }
        }
    },
};

export const ISAJson_encoder = ROCrate_encoder;

export const ISAJson_decoder = ROCrate_decoder;

