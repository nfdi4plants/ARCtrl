import { tryFind, maxBy, skip } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { item, tryItem, skip as skip_1, initialize } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { defaultOf, comparePrimitives } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { some, value } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { empty, tail, cons, head, reverse, isEmpty, tryPick } from "../fable_modules/fable-library-js.4.22.0/List.js";
import { tryGetValue } from "../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { FSharpRef } from "../fable_modules/fable-library-js.4.22.0/Types.js";

/**
 * If at least i values exist in seq a, builds a new array that contains the elements of the given seq, exluding the first i elements
 */
export function Seq_trySkip(i, s) {
    try {
        return skip(i, s);
    }
    catch (matchValue) {
        return undefined;
    }
}

export function Array_ofIndexedSeq(s) {
    return initialize(1 + maxBy((tuple) => tuple[0], s, {
        Compare: comparePrimitives,
    })[0], (i) => {
        const matchValue = tryFind((arg) => (i === arg[0]), s);
        if (matchValue == null) {
            return "";
        }
        else {
            const i_1 = matchValue[0] | 0;
            return matchValue[1];
        }
    });
}

/**
 * If at least i values exist in array a, builds a new array that contains the elements of the given array, exluding the first i elements
 */
export function Array_trySkip(i, a) {
    try {
        return skip_1(i, a);
    }
    catch (matchValue) {
        return undefined;
    }
}

/**
 * Returns Item of array at index i if existing, else returns default value
 */
export function Array_tryItemDefault(i, d, a) {
    const matchValue = tryItem(i, a);
    if (matchValue == null) {
        return d;
    }
    else {
        return value(matchValue);
    }
}

export function Array_map4(f, aa, ba, ca, da) {
    if (!(((aa.length === ba.length) && (ba.length === ca.length)) && (ca.length === da.length))) {
        throw new Error("");
    }
    return initialize(aa.length, (i) => f(item(i, aa), item(i, ba), item(i, ca), item(i, da)));
}

export function List_tryPickDefault(chooser, d, list) {
    const matchValue = tryPick(chooser, list);
    if (matchValue == null) {
        return d;
    }
    else {
        return value(matchValue);
    }
}

export function List_unzip4(l) {
    const loop = (la_mut, lb_mut, lc_mut, ld_mut, l_1_mut) => {
        loop:
        while (true) {
            const la = la_mut, lb = lb_mut, lc = lc_mut, ld = ld_mut, l_1 = l_1_mut;
            if (isEmpty(l_1)) {
                return [reverse(la), reverse(lb), reverse(lc), reverse(ld)];
            }
            else {
                la_mut = cons(head(l_1)[0], la);
                lb_mut = cons(head(l_1)[1], lb);
                lc_mut = cons(head(l_1)[2], lc);
                ld_mut = cons(head(l_1)[3], ld);
                l_1_mut = tail(l_1);
                continue loop;
            }
            break;
        }
    };
    return loop(empty(), empty(), empty(), empty(), l);
}

export function Dictionary_tryGetValue(k, dict) {
    let patternInput;
    let outArg = defaultOf();
    patternInput = [tryGetValue(dict, k, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (patternInput[0]) {
        return some(patternInput[1]);
    }
    else {
        return undefined;
    }
}

export function Dictionary_tryGetString(k, dict) {
    let patternInput;
    let outArg = defaultOf();
    patternInput = [tryGetValue(dict, k, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    const v_1 = patternInput[1];
    if (patternInput[0] && (v_1.trim() !== "")) {
        return v_1.trim();
    }
    else {
        return undefined;
    }
}

