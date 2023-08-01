import { tryFind, maxBy, skip } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { defaultOf, comparePrimitives, disposeSafe, getEnumerator } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { reverse, tryPick, empty, ofArrayWithTail, cons, head, tail, singleton, isEmpty } from "../../fable_modules/fable-library.4.1.4/List.js";
import { tryItem, skip as skip_1, initialize } from "../../fable_modules/fable-library.4.1.4/Array.js";
import { some, value } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { tryGetValue } from "../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { FSharpRef } from "../../fable_modules/fable-library.4.1.4/Types.js";

/**
 * If at least i values exist in seq a, builds a new array that contains the elements of the given seq, exluding the first i elements
 */
export function Seq_trySkip(i, s) {
    try {
        return skip(i, s);
    }
    catch (matchValue) {
        return void 0;
    }
}

function Seq_groupWhen(withOverlap, predicate, input) {
    let matchValue, t_4, h_4;
    const en = getEnumerator(input);
    try {
        const loop = (cont_mut) => {
            loop:
            while (true) {
                const cont = cont_mut;
                if (en["System.Collections.IEnumerator.MoveNext"]()) {
                    const temp = en["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    if (predicate(temp)) {
                        cont_mut = ((y) => cont(isEmpty(y) ? singleton(singleton(temp)) : (withOverlap ? ofArrayWithTail([singleton(temp), cons(temp, head(y))], tail(y)) : ofArrayWithTail([empty(), cons(temp, head(y))], tail(y)))));
                        continue loop;
                    }
                    else {
                        cont_mut = ((y_1) => cont(isEmpty(y_1) ? singleton(singleton(temp)) : cons(cons(temp, head(y_1)), tail(y_1))));
                        continue loop;
                    }
                }
                else {
                    return cont(empty());
                }
                break;
            }
        };
        return (matchValue = loop((x) => x), isEmpty(matchValue) ? empty() : ((t_4 = tail(matchValue), (h_4 = head(matchValue), isEmpty(h_4) ? t_4 : (isEmpty(tail(h_4)) ? ((predicate(head(h_4)) && withOverlap) ? t_4 : cons(h_4, t_4)) : cons(h_4, t_4))))));
    }
    finally {
        disposeSafe(en);
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
        return void 0;
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
    return initialize(aa.length, (i) => f(aa[i], ba[i], ca[i], da[i]));
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
        return void 0;
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
        return void 0;
    }
}

