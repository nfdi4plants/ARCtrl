import { tryFind, maxBy, skip } from "../../fable_modules/fable-library-ts/Seq.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { some, value, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { IMap, defaultOf, comparePrimitives, IDisposable, disposeSafe, IEnumerator, getEnumerator } from "../../fable_modules/fable-library-ts/Util.js";
import { reverse, tryPick, FSharpList, empty, ofArrayWithTail, cons, head, tail, singleton, isEmpty } from "../../fable_modules/fable-library-ts/List.js";
import { tryItem, skip as skip_1, initialize } from "../../fable_modules/fable-library-ts/Array.js";
import { tryGetValue } from "../../fable_modules/fable-library-ts/MapUtil.js";
import { FSharpRef } from "../../fable_modules/fable-library-ts/Types.js";

/**
 * If at least i values exist in seq a, builds a new array that contains the elements of the given seq, exluding the first i elements
 */
export function Seq_trySkip<$a>(i: int32, s: Iterable<$a>): Option<Iterable<$a>> {
    try {
        return skip<$a>(i, s);
    }
    catch (matchValue: any) {
        return void 0;
    }
}

function Seq_groupWhen<a>(withOverlap: boolean, predicate: ((arg0: a) => boolean), input: Iterable<a>): Iterable<Iterable<a>> {
    let matchValue: FSharpList<FSharpList<a>>, t_4: FSharpList<FSharpList<a>>, h_4: FSharpList<a>;
    const en: IEnumerator<a> = getEnumerator(input);
    try {
        const loop = <$b>(cont_mut: ((arg0: FSharpList<FSharpList<a>>) => $b)): $b => {
            loop:
            while (true) {
                const cont: ((arg0: FSharpList<FSharpList<a>>) => $b) = cont_mut;
                if (en["System.Collections.IEnumerator.MoveNext"]()) {
                    const temp: a = en["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    if (predicate(temp)) {
                        cont_mut = ((y: FSharpList<FSharpList<a>>): $b => cont(isEmpty(y) ? singleton(singleton(temp)) : (withOverlap ? ofArrayWithTail([singleton(temp), cons(temp, head(y))], tail(y)) : ofArrayWithTail([empty<a>(), cons(temp, head(y))], tail(y)))));
                        continue loop;
                    }
                    else {
                        cont_mut = ((y_1: FSharpList<FSharpList<a>>): $b => cont(isEmpty(y_1) ? singleton(singleton(temp)) : cons(cons(temp, head(y_1)), tail(y_1))));
                        continue loop;
                    }
                }
                else {
                    return cont(empty<FSharpList<a>>());
                }
                break;
            }
        };
        return (matchValue = loop((x: FSharpList<FSharpList<a>>): FSharpList<FSharpList<a>> => x), isEmpty(matchValue) ? empty<FSharpList<a>>() : ((t_4 = tail(matchValue), (h_4 = head(matchValue), isEmpty(h_4) ? t_4 : (isEmpty(tail(h_4)) ? ((predicate(head(h_4)) && withOverlap) ? t_4 : cons(h_4, t_4)) : cons(h_4, t_4)))))) as Iterable<Iterable<a>>;
    }
    finally {
        disposeSafe(en as IDisposable);
    }
}

export function Array_ofIndexedSeq(s: Iterable<[int32, string]>): string[] {
    return initialize<string>(1 + maxBy<[int32, string], int32>((tuple: [int32, string]): int32 => tuple[0], s, {
        Compare: comparePrimitives,
    })[0], (i: int32): string => {
        const matchValue: Option<[int32, string]> = tryFind<[int32, string]>((arg: [int32, string]): boolean => (i === arg[0]), s);
        if (matchValue == null) {
            return "";
        }
        else {
            const i_1: int32 = value(matchValue)[0] | 0;
            return value(matchValue)[1];
        }
    });
}

/**
 * If at least i values exist in array a, builds a new array that contains the elements of the given array, exluding the first i elements
 */
export function Array_trySkip<$a>(i: int32, a: $a[]): Option<$a[]> {
    try {
        return skip_1<$a>(i, a);
    }
    catch (matchValue: any) {
        return void 0;
    }
}

/**
 * Returns Item of array at index i if existing, else returns default value
 */
export function Array_tryItemDefault<$a>(i: int32, d: $a, a: $a[]): $a {
    const matchValue: Option<$a> = tryItem<$a>(i, a);
    if (matchValue == null) {
        return d;
    }
    else {
        return value(matchValue);
    }
}

export function Array_map4<A, B, C, D, T>(f: ((arg0: A, arg1: B, arg2: C, arg3: D) => T), aa: A[], ba: B[], ca: C[], da: D[]): T[] {
    if (!(((aa.length === ba.length) && (ba.length === ca.length)) && (ca.length === da.length))) {
        throw new Error("");
    }
    return initialize<T>(aa.length, (i: int32): T => f(aa[i], ba[i], ca[i], da[i]));
}

export function List_tryPickDefault<T, U>(chooser: ((arg0: T) => Option<U>), d: U, list: FSharpList<T>): U {
    const matchValue: Option<U> = tryPick<T, U>(chooser, list);
    if (matchValue == null) {
        return d;
    }
    else {
        return value(matchValue);
    }
}

export function List_unzip4<A, B, C, D>(l: FSharpList<[A, B, C, D]>): [FSharpList<A>, FSharpList<B>, FSharpList<C>, FSharpList<D>] {
    const loop = <$a, $b, $c, $d>(la_mut: FSharpList<$a>, lb_mut: FSharpList<$b>, lc_mut: FSharpList<$c>, ld_mut: FSharpList<$d>, l_1_mut: FSharpList<[$a, $b, $c, $d]>): [FSharpList<$a>, FSharpList<$b>, FSharpList<$c>, FSharpList<$d>] => {
        loop:
        while (true) {
            const la: FSharpList<$a> = la_mut, lb: FSharpList<$b> = lb_mut, lc: FSharpList<$c> = lc_mut, ld: FSharpList<$d> = ld_mut, l_1: FSharpList<[$a, $b, $c, $d]> = l_1_mut;
            if (isEmpty(l_1)) {
                return [reverse<$a>(la), reverse<$b>(lb), reverse<$c>(lc), reverse<$d>(ld)] as [FSharpList<$a>, FSharpList<$b>, FSharpList<$c>, FSharpList<$d>];
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
    return loop(empty<A>(), empty<B>(), empty<C>(), empty<D>(), l);
}

export function Dictionary_tryGetValue<K, V>(k: K, dict: IMap<K, V>): Option<V> {
    let patternInput: [boolean, V];
    let outArg: V = defaultOf();
    patternInput = ([tryGetValue(dict, k, new FSharpRef<V>((): V => outArg, (v: V): void => {
        outArg = v;
    })), outArg] as [boolean, V]);
    if (patternInput[0]) {
        return some(patternInput[1]);
    }
    else {
        return void 0;
    }
}

export function Dictionary_tryGetString<K>(k: K, dict: IMap<K, string>): Option<string> {
    let patternInput: [boolean, string];
    let outArg: string = defaultOf();
    patternInput = ([tryGetValue(dict, k, new FSharpRef<string>((): string => outArg, (v: string): void => {
        outArg = v;
    })), outArg] as [boolean, string]);
    const v_1: string = patternInput[1];
    if (patternInput[0] && (v_1.trim() !== "")) {
        return v_1.trim();
    }
    else {
        return void 0;
    }
}

