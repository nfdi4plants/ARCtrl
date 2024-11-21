import { toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { string, succeed, andThen } from "../../fable_modules/Thoth.Json.Core.0.4.0/Decode.fs.js";
import { IOType } from "../../Core/Table/CompositeHeader.js";

export function encoder(io) {
    const value = toString(io);
    return {
        Encode(helpers) {
            return helpers.encodeString(value);
        },
    };
}

export const decoder = andThen((s) => succeed(IOType.ofString(s)), string);

