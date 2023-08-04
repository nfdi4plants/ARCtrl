import { equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { value, some } from "../../fable_modules/fable-library.4.1.4/Option.js";

/**
 * If the value matches the default, a None is returned, else a Some is returned
 */
export function fromValueWithDefault(d, v) {
    if (equals(d, v)) {
        return void 0;
    }
    else {
        return some(v);
    }
}

/**
 * Applies the function f on the value of the option if it exists, else applies it on the default value. If the result value matches the default, a None is returned
 */
export function mapDefault(d, f, o) {
    return fromValueWithDefault(d, (o == null) ? f(d) : f(value(o)));
}

/**
 * Applies the function f on the value of the option if it exists, else returns the default value.
 */
export function mapOrDefault(d, f, o) {
    if (o == null) {
        return d;
    }
    else {
        return some(f(value(o)));
    }
}

