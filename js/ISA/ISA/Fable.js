import { toString } from "../../fable_modules/fable-library.4.1.4/Types.js";
import { append } from "../../fable_modules/fable-library.4.1.4/List.js";
import { append as append_1 } from "../../fable_modules/fable-library.4.1.4/Array.js";
import { Array_distinct, List_distinct } from "../../fable_modules/fable-library.4.1.4/Seq2.js";
import { structuralHash, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";

export function isMap_generic(l1) {
    let copyOfStruct;
    return ((copyOfStruct = l1, toString(copyOfStruct))).indexOf("map [") === 0;
}

export function isList_generic(l1) {
    let s;
    let copyOfStruct = l1;
    s = toString(copyOfStruct);
    if (s.indexOf("[") === 0) {
        return !(s.indexOf("seq [") === 0);
    }
    else {
        return false;
    }
}

export function append_generic(l1, l2) {
    if (l2 == null) {
        return l1;
    }
    else if (isList_generic(l1)) {
        return append(l1, l2);
    }
    else {
        return append_1(l1, l2);
    }
}

export function distinct_generic(l1) {
    if (isList_generic(l1)) {
        return List_distinct(l1, {
            Equals: equals,
            GetHashCode: structuralHash,
        });
    }
    else {
        return Array_distinct(l1, {
            Equals: equals,
            GetHashCode: structuralHash,
        });
    }
}

