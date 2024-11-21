import { string } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { FSharpResult$2 } from "../../fable_modules/fable-library-js.4.22.0/Result.js";
import { MaterialType } from "../../Core/Process/MaterialType.js";
import { ErrorReason$1 } from "../../fable_modules/Thoth.Json.Core.0.4.0/Types.fs.js";

export function ROCrate_encoder(value) {
    if (value.tag === 1) {
        return {
            Encode(helpers_1) {
                return helpers_1.encodeString("Labeled Extract Name");
            },
        };
    }
    else {
        return {
            Encode(helpers) {
                return helpers.encodeString("Extract Name");
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
                case "Extract Name":
                    return new FSharpResult$2(0, [new MaterialType(0, [])]);
                case "Labeled Extract Name":
                    return new FSharpResult$2(0, [new MaterialType(1, [])]);
                default: {
                    const s_1 = matchValue.fields[0];
                    return new FSharpResult$2(1, [[`Could not parse ${s_1}No other value than "Extract Name" or "Labeled Extract Name" allowed for materialtype`, new ErrorReason$1(0, [s_1, json])]]);
                }
            }
        }
    },
};

export const ISAJson_encoder = ROCrate_encoder;

export const ISAJson_decoder = ROCrate_decoder;

