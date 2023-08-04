import { Dictionary } from "../../fable_modules/fable-library.4.1.4/MutableMap.js";
import { isIterable, defaultOf, structuralHash, equals } from "../../fable_modules/fable-library.4.1.4/Util.js";
import { isEmpty, iterate } from "../../fable_modules/fable-library.4.1.4/Seq.js";
import { tryGetValue, addToDict } from "../../fable_modules/fable-library.4.1.4/MapUtil.js";
import { Union, toString, FSharpRef } from "../../fable_modules/fable-library.4.1.4/Types.js";
import { value as value_1, some } from "../../fable_modules/fable-library.4.1.4/Option.js";
import { append_generic, distinct_generic, isList_generic, isMap_generic } from "./Fable.js";
import { union_type, makeUnion, option_type, makeGenericType, getUnionCases, name, obj_type } from "../../fable_modules/fable-library.4.1.4/Reflection.js";
import { exactlyOne, partition } from "../../fable_modules/fable-library.4.1.4/Array.js";

export function Dict_ofSeq(s) {
    const dict = new Dictionary([], {
        Equals: equals,
        GetHashCode: structuralHash,
    });
    iterate((tupledArg) => {
        addToDict(dict, tupledArg[0], tupledArg[1]);
    }, s);
    return dict;
}

export function Dict_tryFind(key, dict) {
    let patternInput;
    let outArg = defaultOf();
    patternInput = [tryGetValue(dict, key, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (patternInput[0]) {
        return some(patternInput[1]);
    }
    else {
        return void 0;
    }
}

export function Dict_ofSeqWithMerge(merge, s) {
    const dict = new Dictionary([], {
        Equals: equals,
        GetHashCode: structuralHash,
    });
    iterate((tupledArg) => {
        const k = tupledArg[0];
        const v = tupledArg[1];
        const matchValue = Dict_tryFind(k, dict);
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

/**
 * Get the type of the IEnumerable elements. E.g. for Array<'T> it would be 'T
 */
export function Update_isMapType(v) {
    return isMap_generic(v);
}

export function Update_isListType(v) {
    return isList_generic(v);
}

/**
 * Get the type of the IEnumerable elements. E.g. for Array<'T> it would be 'T
 */
export function Update_enumGetInnerType(v) {
    return obj_type;
}

/**
 * updates oldRT with newRT by replacing all values, but appending all lists.
 * 
 * newRTList@oldRTList
 */
export function Update_updateAppend(oldVal, newVal) {
    let matchResult, oldInternal, others;
    if (typeof oldVal === "string") {
        matchResult = 0;
    }
    else if (isIterable(oldVal)) {
        let activePatternResult;
        const a = oldVal;
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
        let activePatternResult_1;
        const a_1 = oldVal;
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
            if (typeof oldInternal === "string") {
                return newVal;
            }
            else if (isIterable(oldInternal)) {
                let activePatternResult_2;
                const a_2 = newVal;
                activePatternResult_2 = ((a_2 == null) ? void 0 : (undefined));
                if (activePatternResult_2 != null) {
                    const newInternal = value_1(activePatternResult_2);
                    const v = Update_updateAppend(oldInternal, newInternal);
                    const cases_1 = partition((x) => (name(x) === "Some"), getUnionCases(makeGenericType(option_type(obj_type), [obj_type])));
                    const patternInput = [exactlyOne(cases_1[0]), [v]];
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
            const oldSeq = oldVal;
            const newSeq = newVal;
            const innerType = Update_enumGetInnerType(oldVal);
            if (Update_isMapType(oldVal)) {
                return newVal;
            }
            else {
                return distinct_generic(append_generic(oldSeq, newSeq));
            }
        }
        default:
            return newVal;
    }
}

/**
 * updates oldRT with newRT by replacing all values, but only if the new value is not empty.
 */
export function Update_updateOnlyByExisting(oldVal, newVal) {
    if (equals(newVal, defaultOf())) {
        return oldVal;
    }
    else {
        let activePatternResult;
        const a = oldVal;
        activePatternResult = ((a == null) ? void 0 : (undefined));
        if (activePatternResult != null) {
            const oldInternal = value_1(activePatternResult);
            let activePatternResult_1;
            const a_1 = newVal;
            activePatternResult_1 = ((a_1 == null) ? void 0 : (undefined));
            if (activePatternResult_1 != null) {
                const newInternal = value_1(activePatternResult_1);
                const v = Update_updateOnlyByExisting(oldInternal, newInternal);
                const cases_1 = partition((x) => (name(x) === "Some"), getUnionCases(makeGenericType(option_type(obj_type), [obj_type])));
                const patternInput = [exactlyOne(cases_1[0]), [v]];
                return makeUnion(patternInput[0], patternInput[1]);
            }
            else {
                return oldVal;
            }
        }
        else if (typeof oldVal === "string") {
            const newStr = newVal;
            if (toString(newStr) === "") {
                return oldVal;
            }
            else {
                return newStr;
            }
        }
        else if (isIterable(oldVal)) {
            const newSeq = newVal;
            if (isEmpty(newSeq)) {
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
export function Update_updateOnlyByExistingAppend(oldVal, newVal) {
    if (equals(newVal, defaultOf())) {
        return oldVal;
    }
    else {
        let activePatternResult;
        const a = oldVal;
        activePatternResult = ((a == null) ? void 0 : (undefined));
        if (activePatternResult != null) {
            const oldInternal = value_1(activePatternResult);
            let activePatternResult_1;
            const a_1 = newVal;
            activePatternResult_1 = ((a_1 == null) ? void 0 : (undefined));
            if (activePatternResult_1 != null) {
                const newInternal = value_1(activePatternResult_1);
                const v = Update_updateOnlyByExistingAppend(oldInternal, newInternal);
                const cases_1 = partition((x) => (name(x) === "Some"), getUnionCases(makeGenericType(option_type(obj_type), [obj_type])));
                const patternInput = [exactlyOne(cases_1[0]), [v]];
                return makeUnion(patternInput[0], patternInput[1]);
            }
            else {
                return oldVal;
            }
        }
        else if (typeof oldVal === "string") {
            const newStr = newVal;
            if (toString(newStr) === "") {
                return oldVal;
            }
            else {
                return newStr;
            }
        }
        else if (isIterable(oldVal)) {
            const innerType = Update_enumGetInnerType(oldVal);
            if (Update_isMapType(oldVal)) {
                return newVal;
            }
            else {
                return distinct_generic(append_generic(oldVal, newVal));
            }
        }
        else {
            return newVal;
        }
    }
}

export class Update_UpdateOptions extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["UpdateAll", "UpdateByExisting", "UpdateAllAppendLists", "UpdateByExistingAppendLists"];
    }
}

export function Update_UpdateOptions_$reflection() {
    return union_type("ARCtrl.ISA.Aux.Update.UpdateOptions", [], Update_UpdateOptions, () => [[], [], [], []]);
}

