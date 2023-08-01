import { toString } from "../../fable_modules/fable-library-ts/Types.js";
import { FSharpList, append } from "../../fable_modules/fable-library-ts/List.js";
import { append as append_1 } from "../../fable_modules/fable-library-ts/Array.js";
import { Array_distinct, List_distinct } from "../../fable_modules/fable-library-ts/Seq2.js";
import { structuralHash, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";

export function isMap_generic<$a>(l1: $a): boolean {
    let copyOfStruct: $a;
    return ((copyOfStruct = l1, toString(copyOfStruct))).indexOf("map [") === 0;
}

export function isList_generic<$a>(l1: $a): boolean {
    let s: string;
    let copyOfStruct: $a = l1;
    s = toString(copyOfStruct);
    if (s.indexOf("[") === 0) {
        return !(s.indexOf("seq [") === 0);
    }
    else {
        return false;
    }
}

export function append_generic<$a, $b>(l1: $a, l2: $b): $a {
    if (l2 == null) {
        return l1;
    }
    else if (isList_generic<$a>(l1)) {
        return append<any>(l1, l2);
    }
    else {
        return append_1<any>(l1, l2);
    }
}

export function distinct_generic<$a, $b>(l1: $a): FSharpList<any> {
    if (isList_generic<$a>(l1)) {
        return List_distinct<any>(l1, {
            Equals: equals,
            GetHashCode: structuralHash,
        });
    }
    else {
        return Array_distinct<any>(l1, {
            Equals: equals,
            GetHashCode: structuralHash,
        });
    }
}

