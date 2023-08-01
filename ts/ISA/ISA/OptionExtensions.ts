import { equals } from "../../fable_modules/fable-library-ts/Util.js";
import { value, Option, some } from "../../fable_modules/fable-library-ts/Option.js";

/**
 * If the value matches the default, a None is returned, else a Some is returned
 */
export function fromValueWithDefault<$a>(d: $a, v: $a): Option<$a> {
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
export function mapDefault<T>(d: T, f: ((arg0: T) => T), o: Option<T>): Option<T> {
    return fromValueWithDefault<T>(d, (o == null) ? f(d) : f(value(o)));
}

/**
 * Applies the function f on the value of the option if it exists, else returns the default value.
 */
export function mapOrDefault<T, U>(d: Option<T>, f: ((arg0: U) => T), o: Option<U>): Option<T> {
    if (o == null) {
        return d;
    }
    else {
        return some(f(value(o)));
    }
}

