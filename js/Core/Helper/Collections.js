import { disposeSafe, getEnumerator, defaultOf, structuralHash, equals } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { value as value_1, some } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { empty, singleton, append, head, tail, isEmpty } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { Dictionary } from "../../fable_modules/fable-library-js.4.22.0/MutableMap.js";
import { iterate } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { tryGetValue, addToDict } from "../../fable_modules/fable-library-js.4.22.0/MapUtil.js";
import { FSharpRef } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { contains } from "../../fable_modules/fable-library-js.4.22.0/Array.js";

/**
 * If the value matches the default, a None is returned, else a Some is returned
 */
export function Option_fromValueWithDefault(d, v) {
    if (equals(d, v)) {
        return undefined;
    }
    else {
        return some(v);
    }
}

/**
 * Applies the function f on the value of the option if it exists, else applies it on the default value. If the result value matches the default, a None is returned
 */
export function Option_mapDefault(d, f, o) {
    return Option_fromValueWithDefault(d, (o == null) ? f(d) : f(value_1(o)));
}

/**
 * Applies the function f on the value of the option if it exists, else returns the default value.
 */
export function Option_mapOrDefault(d, f, o) {
    if (o == null) {
        return d;
    }
    else {
        return some(f(value_1(o)));
    }
}

export function List_tryPickAndRemove(f, lst) {
    const loop = (newList_mut, remainingList_mut) => {
        loop:
        while (true) {
            const newList = newList_mut, remainingList = remainingList_mut;
            if (!isEmpty(remainingList)) {
                const t = tail(remainingList);
                const h = head(remainingList);
                const matchValue = f(h);
                if (matchValue == null) {
                    newList_mut = append(newList, singleton(h));
                    remainingList_mut = t;
                    continue loop;
                }
                else {
                    return [some(value_1(matchValue)), append(newList, t)];
                }
            }
            else {
                return [undefined, newList];
            }
            break;
        }
    };
    return loop(empty(), lst);
}

export function Dictionary_ofSeq(s) {
    const dict = new Dictionary([], {
        Equals: equals,
        GetHashCode: structuralHash,
    });
    iterate((tupledArg) => {
        addToDict(dict, tupledArg[0], tupledArg[1]);
    }, s);
    return dict;
}

export function Dictionary_tryFind(key, dict) {
    let patternInput;
    let outArg = defaultOf();
    patternInput = [tryGetValue(dict, key, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (patternInput[0]) {
        return some(patternInput[1]);
    }
    else {
        return undefined;
    }
}

export function Dictionary_ofSeqWithMerge(merge, s) {
    const dict = new Dictionary([], {
        Equals: equals,
        GetHashCode: structuralHash,
    });
    iterate((tupledArg) => {
        const k = tupledArg[0];
        const v = tupledArg[1];
        const matchValue = Dictionary_tryFind(k, dict);
        if (matchValue == null) {
            addToDict(dict, k, v);
        }
        else {
            const v$0027 = value_1(matchValue);
            dict.delete(k);
            addToDict(dict, k, merge(v$0027, v));
        }
    }, s);
    return dict;
}

export function ResizeArray_map(f, a) {
    const b = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            void (b.push(f(i)));
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return b;
}

export function ResizeArray_choose(f, a) {
    const b = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const matchValue = f(enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]());
            if (matchValue == null) {
            }
            else {
                const x = value_1(matchValue);
                void (b.push(x));
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return b;
}

export function ResizeArray_filter(f, a) {
    const b = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            if (f(i)) {
                void (b.push(i));
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return b;
}

export function ResizeArray_fold(f, s, a) {
    let state = s;
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            state = f(state, i);
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return state;
}

export function ResizeArray_foldBack(f, a, s) {
    let state = s;
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            state = f(i, state);
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return state;
}

export function ResizeArray_iter(f, a) {
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            f(enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]());
        }
    }
    finally {
        disposeSafe(enumerator);
    }
}

export function ResizeArray_reduce(f, a) {
    switch (a.length) {
        case 0:
            throw new Error("ResizeArray.reduce: empty array");
        case 1:
            return a[0];
        default: {
            const a_5 = a;
            let state = a_5[0];
            for (let i = 1; i <= (a_5.length - 1); i++) {
                state = f(state, a_5[i]);
            }
            return state;
        }
    }
}

export function ResizeArray_collect(f, a) {
    const b = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const enumerator_1 = getEnumerator(f(enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]()));
            try {
                while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
                    const j = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
                    void (b.push(j));
                }
            }
            finally {
                disposeSafe(enumerator_1);
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return b;
}

export function ResizeArray_distinct(a) {
    const b = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            if (!contains(i, b, {
                Equals: equals,
                GetHashCode: structuralHash,
            })) {
                void (b.push(i));
            }
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    return b;
}

export function ResizeArray_isEmpty(a) {
    return a.length === 0;
}

/**
 * Immutable append
 */
export function ResizeArray_append(a, b) {
    const c = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            void (c.push(i));
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    let enumerator_1 = getEnumerator(b);
    try {
        while (enumerator_1["System.Collections.IEnumerator.MoveNext"]()) {
            const i_1 = enumerator_1["System.Collections.Generic.IEnumerator`1.get_Current"]();
            void (c.push(i_1));
        }
    }
    finally {
        disposeSafe(enumerator_1);
    }
    return c;
}

/**
 * append a single element
 */
export function ResizeArray_appendSingleton(b, a) {
    const c = [];
    let enumerator = getEnumerator(a);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const i = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            void (c.push(i));
        }
    }
    finally {
        disposeSafe(enumerator);
    }
    void (c.push(b));
    return c;
}

