import { Dictionary } from "../../fable_modules/fable-library-ts/MutableMap.js";
import { isIterable, defaultOf, IMap, structuralHash, equals } from "../../fable_modules/fable-library-ts/Util.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { isEmpty, iterate } from "../../fable_modules/fable-library-ts/Seq.js";
import { tryGetValue, addToDict } from "../../fable_modules/fable-library-ts/MapUtil.js";
import { Union, toString, FSharpRef } from "../../fable_modules/fable-library-ts/Types.js";
import { value as value_1, Option, some } from "../../fable_modules/fable-library-ts/Option.js";
import { append_generic, distinct_generic, isList_generic, isMap_generic } from "./Fable.js";
import { union_type, TypeInfo, makeUnion, option_type, makeGenericType, getUnionCases, name, obj_type } from "../../fable_modules/fable-library-ts/Reflection.js";
import { exactlyOne, partition } from "../../fable_modules/fable-library-ts/Array.js";

export function Dict_ofSeq<Key, T>(s: Iterable<[Key, T]>): IMap<Key, T> {
    const dict: IMap<Key, T> = new Dictionary<Key, T>([], {
        Equals: equals,
        GetHashCode: structuralHash,
    });
    iterate<[Key, T]>((tupledArg: [Key, T]): void => {
        addToDict(dict, tupledArg[0], tupledArg[1]);
    }, s);
    return dict;
}

export function Dict_tryFind<Key, T>(key: Key, dict: IMap<Key, T>): Option<T> {
    let patternInput: [boolean, T];
    let outArg: T = defaultOf();
    patternInput = ([tryGetValue(dict, key, new FSharpRef<T>((): T => outArg, (v: T): void => {
        outArg = v;
    })), outArg] as [boolean, T]);
    if (patternInput[0]) {
        return some(patternInput[1]);
    }
    else {
        return void 0;
    }
}

export function Dict_ofSeqWithMerge<T, Key>(merge: ((arg0: T, arg1: T) => T), s: Iterable<[Key, T]>): IMap<Key, T> {
    const dict: IMap<Key, T> = new Dictionary<Key, T>([], {
        Equals: equals,
        GetHashCode: structuralHash,
    });
    iterate<[Key, T]>((tupledArg: [Key, T]): void => {
        const k: Key = tupledArg[0];
        const v: T = tupledArg[1];
        const matchValue: Option<T> = Dict_tryFind<Key, T>(k, dict);
        if (matchValue == null) {
            addToDict(dict, k, v);
        }
        else {
            const v$0027: T = value_1(matchValue);
            dict.delete(k);
            addToDict(dict, k, merge(v$0027, v));
        }
    }, s);
    return dict;
}

/**
 * Get the type of the IEnumerable elements. E.g. for Array<'T> it would be 'T
 */
export function Update_isMapType(v: any): boolean {
    return isMap_generic<any>(v);
}

export function Update_isListType(v: any): boolean {
    return isList_generic<any>(v);
}

/**
 * Get the type of the IEnumerable elements. E.g. for Array<'T> it would be 'T
 */
export function Update_enumGetInnerType(v: any): any {
    return obj_type;
}

/**
 * updates oldRT with newRT by replacing all values, but appending all lists.
 * 
 * newRTList@oldRTList
 */
export function Update_updateAppend(oldVal: any, newVal: any): any {
    let matchResult: int32, oldInternal: any, others: any;
    if (typeof oldVal === "string") {
        matchResult = 0;
    }
    else if (isIterable(oldVal)) {
        let activePatternResult: Option<any>;
        const a: any = oldVal;
        activePatternResult = ((a == null) ? void 0 : (undefined));
        if (activePatternResult != null) {
            matchResult = 1;
            oldInternal = value_1(activePatternResult);
        }
        else {
            matchResult = 2;
        }
    }
    else {
        let activePatternResult_1: Option<any>;
        const a_1: any = oldVal;
        activePatternResult_1 = ((a_1 == null) ? void 0 : (undefined));
        if (activePatternResult_1 != null) {
            matchResult = 1;
            oldInternal = value_1(activePatternResult_1);
        }
        else {
            matchResult = 3;
            others = oldVal;
        }
    }
    switch (matchResult) {
        case 0:
            return newVal;
        case 1:
            if (typeof oldInternal! === "string") {
                return newVal;
            }
            else if (isIterable(oldInternal!)) {
                let activePatternResult_2: Option<any>;
                const a_2: any = newVal;
                activePatternResult_2 = ((a_2 == null) ? void 0 : (undefined));
                if (activePatternResult_2 != null) {
                    const newInternal: any = value_1(activePatternResult_2);
                    const v: any = Update_updateAppend(oldInternal!, newInternal);
                    const cases_1: [any[], any[]] = partition<any>((x: any): boolean => (name(x) === "Some"), getUnionCases(makeGenericType(option_type(obj_type), [obj_type])));
                    const patternInput = [exactlyOne<any>(cases_1[0]), [v]] as [any, any[]];
                    return makeUnion(patternInput[0], patternInput[1]);
                }
                else {
                    return oldVal;
                }
            }
            else {
                return newVal;
            }
        case 2: {
            const oldSeq = oldVal as Iterable<any>;
            const newSeq = newVal as Iterable<any>;
            const innerType: any = Update_enumGetInnerType(oldVal);
            if (Update_isMapType(oldVal)) {
                return newVal;
            }
            else {
                return distinct_generic<Iterable<any>, any>(append_generic<Iterable<any>, Iterable<any>>(oldSeq, newSeq));
            }
        }
        default:
            return newVal;
    }
}

/**
 * updates oldRT with newRT by replacing all values, but only if the new value is not empty.
 */
export function Update_updateOnlyByExisting(oldVal: any, newVal: any): any {
    if (equals(newVal, defaultOf())) {
        return oldVal;
    }
    else {
        let activePatternResult: Option<any>;
        const a: any = oldVal;
        activePatternResult = ((a == null) ? void 0 : (undefined));
        if (activePatternResult != null) {
            const oldInternal: any = value_1(activePatternResult);
            let activePatternResult_1: Option<any>;
            const a_1: any = newVal;
            activePatternResult_1 = ((a_1 == null) ? void 0 : (undefined));
            if (activePatternResult_1 != null) {
                const newInternal: any = value_1(activePatternResult_1);
                const v: any = Update_updateOnlyByExisting(oldInternal, newInternal);
                const cases_1: [any[], any[]] = partition<any>((x: any): boolean => (name(x) === "Some"), getUnionCases(makeGenericType(option_type(obj_type), [obj_type])));
                const patternInput = [exactlyOne<any>(cases_1[0]), [v]] as [any, any[]];
                return makeUnion(patternInput[0], patternInput[1]);
            }
            else {
                return oldVal;
            }
        }
        else if (typeof oldVal === "string") {
            const newStr: any = newVal;
            if (toString(newStr) === "") {
                return oldVal;
            }
            else {
                return newStr;
            }
        }
        else if (isIterable(oldVal)) {
            const newSeq: any = newVal;
            if (isEmpty<any>(newSeq as Iterable<any> as Iterable<any>)) {
                return oldVal;
            }
            else {
                return newSeq;
            }
        }
        else {
            return newVal;
        }
    }
}

/**
 * updates oldRT with newRT by replacing all values, but only if the new value is not empty.
 */
export function Update_updateOnlyByExistingAppend(oldVal: any, newVal: any): any {
    if (equals(newVal, defaultOf())) {
        return oldVal;
    }
    else {
        let activePatternResult: Option<any>;
        const a: any = oldVal;
        activePatternResult = ((a == null) ? void 0 : (undefined));
        if (activePatternResult != null) {
            const oldInternal: any = value_1(activePatternResult);
            let activePatternResult_1: Option<any>;
            const a_1: any = newVal;
            activePatternResult_1 = ((a_1 == null) ? void 0 : (undefined));
            if (activePatternResult_1 != null) {
                const newInternal: any = value_1(activePatternResult_1);
                const v: any = Update_updateOnlyByExistingAppend(oldInternal, newInternal);
                const cases_1: [any[], any[]] = partition<any>((x: any): boolean => (name(x) === "Some"), getUnionCases(makeGenericType(option_type(obj_type), [obj_type])));
                const patternInput = [exactlyOne<any>(cases_1[0]), [v]] as [any, any[]];
                return makeUnion(patternInput[0], patternInput[1]);
            }
            else {
                return oldVal;
            }
        }
        else if (typeof oldVal === "string") {
            const newStr: any = newVal;
            if (toString(newStr) === "") {
                return oldVal;
            }
            else {
                return newStr;
            }
        }
        else if (isIterable(oldVal)) {
            const innerType: any = Update_enumGetInnerType(oldVal);
            if (Update_isMapType(oldVal)) {
                return newVal;
            }
            else {
                return distinct_generic<any, any>(append_generic<any, any>(oldVal, newVal));
            }
        }
        else {
            return newVal;
        }
    }
}

export type Update_UpdateOptions_$union = 
    | Update_UpdateOptions<0>
    | Update_UpdateOptions<1>
    | Update_UpdateOptions<2>
    | Update_UpdateOptions<3>

export type Update_UpdateOptions_$cases = {
    0: ["UpdateAll", []],
    1: ["UpdateByExisting", []],
    2: ["UpdateAllAppendLists", []],
    3: ["UpdateByExistingAppendLists", []]
}

export function Update_UpdateOptions_UpdateAll() {
    return new Update_UpdateOptions<0>(0, []);
}

export function Update_UpdateOptions_UpdateByExisting() {
    return new Update_UpdateOptions<1>(1, []);
}

export function Update_UpdateOptions_UpdateAllAppendLists() {
    return new Update_UpdateOptions<2>(2, []);
}

export function Update_UpdateOptions_UpdateByExistingAppendLists() {
    return new Update_UpdateOptions<3>(3, []);
}

export class Update_UpdateOptions<Tag extends keyof Update_UpdateOptions_$cases> extends Union<Tag, Update_UpdateOptions_$cases[Tag][0]> {
    constructor(readonly tag: Tag, readonly fields: Update_UpdateOptions_$cases[Tag][1]) {
        super();
    }
    cases() {
        return ["UpdateAll", "UpdateByExisting", "UpdateAllAppendLists", "UpdateByExistingAppendLists"];
    }
}

export function Update_UpdateOptions_$reflection(): TypeInfo {
    return union_type("ISA.Aux.Update.UpdateOptions", [], Update_UpdateOptions, () => [[], [], [], []]);
}

