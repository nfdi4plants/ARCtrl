import { second, minute, hour, day, month, year } from "../../fable_modules/fable-library-js.4.22.0/Date.js";
import { numberHash, identityHash } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { value as value_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { fold } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { fold as fold_1 } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";

export function mergeHashes(hash1, hash2) {
    return ((-1640531527 + hash2) + (hash1 << 6)) + (hash1 >> 2);
}

export function hashDateTime(dt) {
    let acc = 0;
    acc = (mergeHashes(acc, year(dt)) | 0);
    acc = (mergeHashes(acc, month(dt)) | 0);
    acc = (mergeHashes(acc, day(dt)) | 0);
    acc = (mergeHashes(acc, hour(dt)) | 0);
    acc = (mergeHashes(acc, minute(dt)) | 0);
    acc = (mergeHashes(acc, second(dt)) | 0);
    return acc | 0;
}

export function hash(obj) {
    let copyOfStruct = obj;
    return identityHash(copyOfStruct) | 0;
}

export function boxHashOption(a) {
    let copyOfStruct, copyOfStruct_1;
    return (a != null) ? ((copyOfStruct = value_1(a), identityHash(copyOfStruct))) : ((copyOfStruct_1 = 0, numberHash(copyOfStruct_1)));
}

export function boxHashArray(a) {
    return fold((acc, o) => mergeHashes(acc, hash(o)), 0, a);
}

export function boxHashSeq(a) {
    return fold_1((acc, o) => mergeHashes(acc, hash(o)), 0, a);
}

