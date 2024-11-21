import { printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { freeTextFromStringCells, dataFromStringCells, unitizedFromStringCells, termFromStringCells } from "./CompositeCell.js";
import { tryFindIndex, skip, item, equalsWith } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { defaultOf } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { ActivePatterns_$007CComment$007C_$007C, ActivePatterns_$007COutputColumnHeader$007C_$007C, ActivePatterns_$007CInputColumnHeader$007C_$007C, tryParseComponentColumnHeader, tryParseCharacteristicColumnHeader, tryParseFactorColumnHeader, tryParseParameterColumnHeader, ActivePatterns_$007CTSRColumnHeader$007C_$007C, ActivePatterns_$007CTANColumnHeader$007C_$007C, ActivePatterns_$007CUnitColumnHeader$007C_$007C } from "../../Core/Helper/Regex.js";
import { IOType, CompositeHeader } from "../../Core/Table/CompositeHeader.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { toString } from "../../fable_modules/fable-library-js.4.22.0/Types.js";
import { empty, singleton, append, delay, toArray } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";

export function ActivePattern_mergeIDInfo(idSpace1, localID1, idSpace2, localID2) {
    if (idSpace1 !== idSpace2) {
        toFail(printf("TermSourceRef %s and %s do not match"))(idSpace1)(idSpace2);
    }
    if (localID1 !== localID2) {
        toFail(printf("LocalID %s and %s do not match"))(localID1)(localID2);
    }
    return {
        TermAccessionNumber: `${idSpace1}:${localID1}`,
        TermSourceRef: idSpace1,
    };
}

export function ActivePattern_$007CTerm$007C_$007C(categoryParser, f, cellValues) {
    const $007CAC$007C_$007C = categoryParser;
    let matchResult, name, name_1, term1, term2;
    if (!equalsWith((x, y) => (x === y), cellValues, defaultOf()) && (cellValues.length === 1)) {
        const activePatternResult = $007CAC$007C_$007C(item(0, cellValues));
        if (activePatternResult != null) {
            matchResult = 0;
            name = activePatternResult;
        }
        else {
            matchResult = 2;
        }
    }
    else if (!equalsWith((x_1, y_1) => (x_1 === y_1), cellValues, defaultOf()) && (cellValues.length === 3)) {
        const activePatternResult_1 = $007CAC$007C_$007C(item(0, cellValues));
        if (activePatternResult_1 != null) {
            const activePatternResult_2 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(item(1, cellValues));
            if (activePatternResult_2 != null) {
                const activePatternResult_3 = ActivePatterns_$007CTANColumnHeader$007C_$007C(item(2, cellValues));
                if (activePatternResult_3 != null) {
                    matchResult = 1;
                    name_1 = activePatternResult_1;
                    term1 = activePatternResult_2;
                    term2 = activePatternResult_3;
                }
                else {
                    matchResult = 2;
                }
            }
            else {
                matchResult = 2;
            }
        }
        else {
            matchResult = 2;
        }
    }
    else {
        matchResult = 2;
    }
    switch (matchResult) {
        case 0:
            return [f(OntologyAnnotation.create(name)), (cellValues_1) => termFromStringCells(undefined, undefined, cellValues_1)];
        case 1: {
            const term = ActivePattern_mergeIDInfo(term1.IDSpace, term1.LocalID, term2.IDSpace, term2.LocalID);
            return [f(OntologyAnnotation.create(name_1, term.TermSourceRef, term.TermAccessionNumber)), (cellValues_2) => termFromStringCells(1, 2, cellValues_2)];
        }
        default: {
            let matchResult_1, name_2, term1_1, term2_1;
            if (!equalsWith((x_2, y_2) => (x_2 === y_2), cellValues, defaultOf()) && (cellValues.length === 3)) {
                const activePatternResult_4 = $007CAC$007C_$007C(item(0, cellValues));
                if (activePatternResult_4 != null) {
                    const activePatternResult_5 = ActivePatterns_$007CTANColumnHeader$007C_$007C(item(1, cellValues));
                    if (activePatternResult_5 != null) {
                        const activePatternResult_6 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(item(2, cellValues));
                        if (activePatternResult_6 != null) {
                            matchResult_1 = 0;
                            name_2 = activePatternResult_4;
                            term1_1 = activePatternResult_6;
                            term2_1 = activePatternResult_5;
                        }
                        else {
                            matchResult_1 = 1;
                        }
                    }
                    else {
                        matchResult_1 = 1;
                    }
                }
                else {
                    matchResult_1 = 1;
                }
            }
            else {
                matchResult_1 = 1;
            }
            switch (matchResult_1) {
                case 0: {
                    const term_1 = ActivePattern_mergeIDInfo(term1_1.IDSpace, term1_1.LocalID, term2_1.IDSpace, term2_1.LocalID);
                    return [f(OntologyAnnotation.create(name_2, term_1.TermSourceRef, term_1.TermAccessionNumber)), (cellValues_3) => termFromStringCells(2, 1, cellValues_3)];
                }
                default: {
                    let matchResult_2, name_3, term1_2, term2_2;
                    if (!equalsWith((x_3, y_3) => (x_3 === y_3), cellValues, defaultOf()) && (cellValues.length === 4)) {
                        const activePatternResult_7 = $007CAC$007C_$007C(item(0, cellValues));
                        if (activePatternResult_7 != null) {
                            if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(item(1, cellValues)) != null) {
                                const activePatternResult_9 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(item(2, cellValues));
                                if (activePatternResult_9 != null) {
                                    const activePatternResult_10 = ActivePatterns_$007CTANColumnHeader$007C_$007C(item(3, cellValues));
                                    if (activePatternResult_10 != null) {
                                        matchResult_2 = 0;
                                        name_3 = activePatternResult_7;
                                        term1_2 = activePatternResult_9;
                                        term2_2 = activePatternResult_10;
                                    }
                                    else {
                                        matchResult_2 = 1;
                                    }
                                }
                                else {
                                    matchResult_2 = 1;
                                }
                            }
                            else {
                                matchResult_2 = 1;
                            }
                        }
                        else {
                            matchResult_2 = 1;
                        }
                    }
                    else {
                        matchResult_2 = 1;
                    }
                    switch (matchResult_2) {
                        case 0: {
                            const term_2 = ActivePattern_mergeIDInfo(term1_2.IDSpace, term1_2.LocalID, term2_2.IDSpace, term2_2.LocalID);
                            return [f(OntologyAnnotation.create(name_3, term_2.TermSourceRef, term_2.TermAccessionNumber)), (cellValues_4) => unitizedFromStringCells(1, 2, 3, cellValues_4)];
                        }
                        default: {
                            let matchResult_3, name_4, term1_3, term2_3;
                            if (!equalsWith((x_4, y_4) => (x_4 === y_4), cellValues, defaultOf()) && (cellValues.length === 4)) {
                                const activePatternResult_11 = $007CAC$007C_$007C(item(0, cellValues));
                                if (activePatternResult_11 != null) {
                                    if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(item(1, cellValues)) != null) {
                                        const activePatternResult_13 = ActivePatterns_$007CTANColumnHeader$007C_$007C(item(2, cellValues));
                                        if (activePatternResult_13 != null) {
                                            const activePatternResult_14 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(item(3, cellValues));
                                            if (activePatternResult_14 != null) {
                                                matchResult_3 = 0;
                                                name_4 = activePatternResult_11;
                                                term1_3 = activePatternResult_14;
                                                term2_3 = activePatternResult_13;
                                            }
                                            else {
                                                matchResult_3 = 1;
                                            }
                                        }
                                        else {
                                            matchResult_3 = 1;
                                        }
                                    }
                                    else {
                                        matchResult_3 = 1;
                                    }
                                }
                                else {
                                    matchResult_3 = 1;
                                }
                            }
                            else {
                                matchResult_3 = 1;
                            }
                            switch (matchResult_3) {
                                case 0: {
                                    const term_3 = ActivePattern_mergeIDInfo(term1_3.IDSpace, term1_3.LocalID, term2_3.IDSpace, term2_3.LocalID);
                                    return [f(OntologyAnnotation.create(name_4, term_3.TermSourceRef, term_3.TermAccessionNumber)), (cellValues_5) => unitizedFromStringCells(1, 3, 2, cellValues_5)];
                                }
                                default:
                                    return undefined;
                            }
                        }
                    }
                }
            }
        }
    }
}

export function ActivePattern_$007CParameter$007C_$007C(cellValues) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseParameterColumnHeader, (Item) => (new CompositeHeader(3, [Item])), cellValues);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CFactor$007C_$007C(cellValues) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseFactorColumnHeader, (Item) => (new CompositeHeader(2, [Item])), cellValues);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CCharacteristic$007C_$007C(cellValues) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseCharacteristicColumnHeader, (Item) => (new CompositeHeader(1, [Item])), cellValues);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CComponent$007C_$007C(cellValues) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseComponentColumnHeader, (Item) => (new CompositeHeader(0, [Item])), cellValues);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CInput$007C_$007C(cellValues) {
    if (cellValues.length === 0) {
        return undefined;
    }
    else {
        const matchValue = item(0, cellValues);
        const activePatternResult = ActivePatterns_$007CInputColumnHeader$007C_$007C(matchValue);
        if (activePatternResult != null) {
            const ioType = activePatternResult;
            const cols = skip(1, cellValues);
            const matchValue_1 = IOType.ofString(ioType);
            if (matchValue_1.tag === 2) {
                const format = map((y) => (1 + y), tryFindIndex((s) => s.startsWith("Data Format"), cols));
                const selectorFormat = map((y_1) => (1 + y_1), tryFindIndex((s_1) => s_1.startsWith("Data Selector Format"), cols));
                return [new CompositeHeader(11, [new IOType(2, [])]), (cellValues_1) => dataFromStringCells(format, selectorFormat, cellValues_1)];
            }
            else {
                return [new CompositeHeader(11, [matchValue_1]), freeTextFromStringCells];
            }
        }
        else {
            return undefined;
        }
    }
}

export function ActivePattern_$007COutput$007C_$007C(cellValues) {
    if (cellValues.length === 0) {
        return undefined;
    }
    else {
        const matchValue = item(0, cellValues);
        const activePatternResult = ActivePatterns_$007COutputColumnHeader$007C_$007C(matchValue);
        if (activePatternResult != null) {
            const ioType = activePatternResult;
            const cols = skip(1, cellValues);
            const matchValue_1 = IOType.ofString(ioType);
            if (matchValue_1.tag === 2) {
                const format = map((y) => (1 + y), tryFindIndex((s) => s.startsWith("Data Format"), cols));
                const selectorFormat = map((y_1) => (1 + y_1), tryFindIndex((s_1) => s_1.startsWith("Data Selector Format"), cols));
                return [new CompositeHeader(12, [new IOType(2, [])]), (cellValues_1) => dataFromStringCells(format, selectorFormat, cellValues_1)];
            }
            else {
                return [new CompositeHeader(12, [matchValue_1]), freeTextFromStringCells];
            }
        }
        else {
            return undefined;
        }
    }
}

export function ActivePattern_$007CComment$007C_$007C(cellValues) {
    let matchResult, key;
    if (!equalsWith((x, y) => (x === y), cellValues, defaultOf()) && (cellValues.length === 1)) {
        const activePatternResult = ActivePatterns_$007CComment$007C_$007C(item(0, cellValues));
        if (activePatternResult != null) {
            matchResult = 0;
            key = activePatternResult;
        }
        else {
            matchResult = 1;
        }
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return [new CompositeHeader(14, [key]), freeTextFromStringCells];
        default:
            return undefined;
    }
}

export function ActivePattern_$007CProtocolType$007C_$007C(cellValues) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C((s) => {
        if (s === "Protocol Type") {
            return s;
        }
        else {
            return undefined;
        }
    }, (_arg) => (new CompositeHeader(4, [])), cellValues);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CProtocolHeader$007C_$007C(cellValues) {
    let matchResult;
    if (!equalsWith((x, y) => (x === y), cellValues, defaultOf()) && (cellValues.length === 1)) {
        if (item(0, cellValues) === "Protocol REF") {
            matchResult = 0;
        }
        else if (item(0, cellValues) === "Protocol Description") {
            matchResult = 1;
        }
        else if (item(0, cellValues) === "Protocol Uri") {
            matchResult = 2;
        }
        else if (item(0, cellValues) === "Protocol Version") {
            matchResult = 3;
        }
        else if (item(0, cellValues) === "Performer") {
            matchResult = 4;
        }
        else if (item(0, cellValues) === "Date") {
            matchResult = 5;
        }
        else {
            matchResult = 6;
        }
    }
    else {
        matchResult = 6;
    }
    switch (matchResult) {
        case 0:
            return [new CompositeHeader(8, []), freeTextFromStringCells];
        case 1:
            return [new CompositeHeader(5, []), freeTextFromStringCells];
        case 2:
            return [new CompositeHeader(6, []), freeTextFromStringCells];
        case 3:
            return [new CompositeHeader(7, []), freeTextFromStringCells];
        case 4:
            return [new CompositeHeader(9, []), freeTextFromStringCells];
        case 5:
            return [new CompositeHeader(10, []), freeTextFromStringCells];
        default:
            return undefined;
    }
}

export function ActivePattern_$007CFreeText$007C_$007C(cellValues) {
    if (!equalsWith((x, y) => (x === y), cellValues, defaultOf()) && (cellValues.length === 1)) {
        return [new CompositeHeader(13, [item(0, cellValues)]), freeTextFromStringCells];
    }
    else {
        return undefined;
    }
}

export function fromStringCells(cellValues) {
    const activePatternResult = ActivePattern_$007CParameter$007C_$007C(cellValues);
    if (activePatternResult != null) {
        const p = activePatternResult;
        return p;
    }
    else {
        const activePatternResult_1 = ActivePattern_$007CFactor$007C_$007C(cellValues);
        if (activePatternResult_1 != null) {
            const f = activePatternResult_1;
            return f;
        }
        else {
            const activePatternResult_2 = ActivePattern_$007CCharacteristic$007C_$007C(cellValues);
            if (activePatternResult_2 != null) {
                const c = activePatternResult_2;
                return c;
            }
            else {
                const activePatternResult_3 = ActivePattern_$007CComponent$007C_$007C(cellValues);
                if (activePatternResult_3 != null) {
                    const c_1 = activePatternResult_3;
                    return c_1;
                }
                else {
                    const activePatternResult_4 = ActivePattern_$007CInput$007C_$007C(cellValues);
                    if (activePatternResult_4 != null) {
                        const i = activePatternResult_4;
                        return i;
                    }
                    else {
                        const activePatternResult_5 = ActivePattern_$007COutput$007C_$007C(cellValues);
                        if (activePatternResult_5 != null) {
                            const o = activePatternResult_5;
                            return o;
                        }
                        else {
                            const activePatternResult_6 = ActivePattern_$007CProtocolType$007C_$007C(cellValues);
                            if (activePatternResult_6 != null) {
                                const pt = activePatternResult_6;
                                return pt;
                            }
                            else {
                                const activePatternResult_7 = ActivePattern_$007CProtocolHeader$007C_$007C(cellValues);
                                if (activePatternResult_7 != null) {
                                    const ph = activePatternResult_7;
                                    return ph;
                                }
                                else {
                                    const activePatternResult_8 = ActivePattern_$007CComment$007C_$007C(cellValues);
                                    if (activePatternResult_8 != null) {
                                        const c_2 = activePatternResult_8;
                                        return c_2;
                                    }
                                    else {
                                        const activePatternResult_9 = ActivePattern_$007CFreeText$007C_$007C(cellValues);
                                        if (activePatternResult_9 != null) {
                                            const ft = activePatternResult_9;
                                            return ft;
                                        }
                                        else {
                                            return toFail(printf("Could not parse header group %O"))(cellValues);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

export function toStringCells(hasUnit, header) {
    if (header.IsDataColumn) {
        return [toString(header), "Data Format", "Data Selector Format"];
    }
    else if (header.IsSingleColumn) {
        return [toString(header)];
    }
    else if (header.IsTermColumn) {
        return toArray(delay(() => append(singleton(toString(header)), delay(() => append(hasUnit ? singleton("Unit") : empty(), delay(() => append(singleton(`Term Source REF (${header.GetColumnAccessionShort})`), delay(() => singleton(`Term Accession Number (${header.GetColumnAccessionShort})`)))))))));
    }
    else {
        return toFail(printf("header %O is neither single nor term column"))(header);
    }
}

