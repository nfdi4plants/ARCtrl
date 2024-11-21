import { defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";

export const DefaultWhitespace = 2;

export function defaultWhitespace(spaces) {
    return defaultArg(spaces, DefaultWhitespace);
}

