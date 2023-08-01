import { printf, toFail } from "../../fable_modules/fable-library.4.1.4/String.js";
import { singleton, tail, head, isEmpty, map } from "../../fable_modules/fable-library.4.1.4/List.js";
import { OntologyAnnotation_fromString_Z7D8EB286 } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { ActivePatterns_$007COutputColumnHeader$007C_$007C, ActivePatterns_$007CInputColumnHeader$007C_$007C, tryParseCharacteristicColumnHeader, tryParseFactorColumnHeader, tryParseParameterColumnHeader, ActivePatterns_$007CUnitColumnHeader$007C_$007C, ActivePatterns_$007CTANColumnHeader$007C_$007C, ActivePatterns_$007CTSRColumnHeader$007C_$007C } from "../ISA/Regex.js";
import { IOType, CompositeHeader } from "../ISA/ArcTypes/CompositeHeader.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.3.1.1/Cells/FsCell.fs.js";
import { toString } from "../../fable_modules/fable-library.4.1.4/Types.js";
import { empty, singleton as singleton_1, append, delay, toList } from "../../fable_modules/fable-library.4.1.4/Seq.js";

export function ActivePattern_mergeTerms(tsr1, tan1, tsr2, tan2) {
    if (tsr1 !== tsr2) {
        toFail(printf("TermSourceRef %s and %s do not match"))(tsr1)(tsr2);
    }
    if (tan1 !== tan2) {
        toFail(printf("TermAccessionNumber %s and %s do not match"))(tan1)(tan2);
    }
    return {
        TermAccessionNumber: tan1,
        TermSourceRef: tsr1,
    };
}

export function ActivePattern_$007CTerm$007C_$007C(categoryParser, f, cells) {
    const $007CAC$007C_$007C = categoryParser;
    const cellValues = map((c) => c.Value, cells);
    let matchResult, name;
    if (!isEmpty(cellValues)) {
        const activePatternResult = $007CAC$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                name = activePatternResult;
            }
            else {
                matchResult = 1;
            }
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
            return f(OntologyAnnotation_fromString_Z7D8EB286(name));
        default: {
            let matchResult_1, name_1, term1, term2;
            if (!isEmpty(cellValues)) {
                const activePatternResult_1 = $007CAC$007C_$007C(head(cellValues));
                if (activePatternResult_1 != null) {
                    if (!isEmpty(tail(cellValues))) {
                        const activePatternResult_2 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(cellValues)));
                        if (activePatternResult_2 != null) {
                            if (!isEmpty(tail(tail(cellValues)))) {
                                const activePatternResult_3 = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                if (activePatternResult_3 != null) {
                                    if (!isEmpty(tail(tail(tail(cellValues))))) {
                                        const activePatternResult_4 = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(tail(cellValues)))));
                                        if (activePatternResult_4 != null) {
                                            if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                                                const activePatternResult_5 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                                if (activePatternResult_5 != null) {
                                                    if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(head(tail(cellValues))) != null) {
                                                        matchResult_1 = 0;
                                                        name_1 = activePatternResult_1;
                                                        term1 = activePatternResult_5;
                                                        term2 = activePatternResult_4;
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
                                    }
                                    else {
                                        matchResult_1 = 0;
                                        name_1 = activePatternResult_1;
                                        term1 = activePatternResult_2;
                                        term2 = activePatternResult_3;
                                    }
                                }
                                else {
                                    const activePatternResult_7 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                    if (activePatternResult_7 != null) {
                                        if (!isEmpty(tail(tail(tail(cellValues))))) {
                                            const activePatternResult_8 = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(tail(cellValues)))));
                                            if (activePatternResult_8 != null) {
                                                if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                                                    if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(head(tail(cellValues))) != null) {
                                                        matchResult_1 = 0;
                                                        name_1 = activePatternResult_1;
                                                        term1 = activePatternResult_7;
                                                        term2 = activePatternResult_8;
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
                                    }
                                    else {
                                        matchResult_1 = 1;
                                    }
                                }
                            }
                            else {
                                matchResult_1 = 1;
                            }
                        }
                        else if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(head(tail(cellValues))) != null) {
                            if (!isEmpty(tail(tail(cellValues)))) {
                                const activePatternResult_11 = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                if (activePatternResult_11 != null) {
                                    if (!isEmpty(tail(tail(tail(cellValues))))) {
                                        const activePatternResult_12 = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(tail(cellValues)))));
                                        if (activePatternResult_12 != null) {
                                            if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                                                matchResult_1 = 0;
                                                name_1 = activePatternResult_1;
                                                term1 = activePatternResult_11;
                                                term2 = activePatternResult_12;
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
            }
            else {
                matchResult_1 = 1;
            }
            switch (matchResult_1) {
                case 0: {
                    const term = ActivePattern_mergeTerms(term1.TermSourceREF, term1.TermAccessionNumber, term2.TermSourceREF, term2.TermAccessionNumber);
                    return f(OntologyAnnotation_fromString_Z7D8EB286(name_1, term.TermSourceRef, term.TermAccessionNumber));
                }
                default:
                    return void 0;
            }
        }
    }
}

export function ActivePattern_$007CParameter$007C_$007C(cells) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseParameterColumnHeader, (arg) => (new CompositeHeader(3, [arg])), cells);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return void 0;
    }
}

export function ActivePattern_$007CFactor$007C_$007C(cells) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseFactorColumnHeader, (arg) => (new CompositeHeader(2, [arg])), cells);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return void 0;
    }
}

export function ActivePattern_$007CCharacteristic$007C_$007C(cells) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C(tryParseCharacteristicColumnHeader, (arg) => (new CompositeHeader(1, [arg])), cells);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return void 0;
    }
}

export function ActivePattern_$007CInput$007C_$007C(cells) {
    const cellValues = map((c) => c.Value, cells);
    let matchResult, ioType;
    if (!isEmpty(cellValues)) {
        const activePatternResult = ActivePatterns_$007CInputColumnHeader$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                ioType = activePatternResult;
            }
            else {
                matchResult = 1;
            }
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
            return new CompositeHeader(11, [IOType.ofString(ioType)]);
        default:
            return void 0;
    }
}

export function ActivePattern_$007COutput$007C_$007C(cells) {
    const cellValues = map((c) => c.Value, cells);
    let matchResult, ioType;
    if (!isEmpty(cellValues)) {
        const activePatternResult = ActivePatterns_$007COutputColumnHeader$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                ioType = activePatternResult;
            }
            else {
                matchResult = 1;
            }
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
            return new CompositeHeader(12, [IOType.ofString(ioType)]);
        default:
            return void 0;
    }
}

export function ActivePattern_$007CProtocolHeader$007C_$007C(cells) {
    const cellValues = map((c) => c.Value, cells);
    let matchResult;
    if (!isEmpty(cellValues)) {
        switch (head(cellValues)) {
            case "Protocol Type": {
                matchResult = 0;
                break;
            }
            case "Protocol REF": {
                if (isEmpty(tail(cellValues))) {
                    matchResult = 1;
                }
                else {
                    matchResult = 7;
                }
                break;
            }
            case "Protocol Description": {
                if (isEmpty(tail(cellValues))) {
                    matchResult = 2;
                }
                else {
                    matchResult = 7;
                }
                break;
            }
            case "Protocol Uri": {
                if (isEmpty(tail(cellValues))) {
                    matchResult = 3;
                }
                else {
                    matchResult = 7;
                }
                break;
            }
            case "Protocol Version": {
                if (isEmpty(tail(cellValues))) {
                    matchResult = 4;
                }
                else {
                    matchResult = 7;
                }
                break;
            }
            case "Performer": {
                if (isEmpty(tail(cellValues))) {
                    matchResult = 5;
                }
                else {
                    matchResult = 7;
                }
                break;
            }
            case "Date": {
                if (isEmpty(tail(cellValues))) {
                    matchResult = 6;
                }
                else {
                    matchResult = 7;
                }
                break;
            }
            default:
                matchResult = 7;
        }
    }
    else {
        matchResult = 7;
    }
    switch (matchResult) {
        case 0:
            return new CompositeHeader(4, []);
        case 1:
            return new CompositeHeader(8, []);
        case 2:
            return new CompositeHeader(5, []);
        case 3:
            return new CompositeHeader(6, []);
        case 4:
            return new CompositeHeader(7, []);
        case 5:
            return new CompositeHeader(9, []);
        case 6:
            return new CompositeHeader(10, []);
        default:
            return void 0;
    }
}

export function ActivePattern_$007CFreeText$007C_$007C(cells) {
    const cellValues = map((c) => c.Value, cells);
    let matchResult, text;
    if (!isEmpty(cellValues)) {
        if (isEmpty(tail(cellValues))) {
            matchResult = 0;
            text = head(cellValues);
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
            return new CompositeHeader(13, [text]);
        default:
            return void 0;
    }
}

export function fromFsCells(cells) {
    const activePatternResult = ActivePattern_$007CParameter$007C_$007C(cells);
    if (activePatternResult != null) {
        const p = activePatternResult;
        return p;
    }
    else {
        const activePatternResult_1 = ActivePattern_$007CFactor$007C_$007C(cells);
        if (activePatternResult_1 != null) {
            const f = activePatternResult_1;
            return f;
        }
        else {
            const activePatternResult_2 = ActivePattern_$007CCharacteristic$007C_$007C(cells);
            if (activePatternResult_2 != null) {
                const c = activePatternResult_2;
                return c;
            }
            else {
                const activePatternResult_3 = ActivePattern_$007CInput$007C_$007C(cells);
                if (activePatternResult_3 != null) {
                    const i = activePatternResult_3;
                    return i;
                }
                else {
                    const activePatternResult_4 = ActivePattern_$007COutput$007C_$007C(cells);
                    if (activePatternResult_4 != null) {
                        const o = activePatternResult_4;
                        return o;
                    }
                    else {
                        const activePatternResult_5 = ActivePattern_$007CProtocolHeader$007C_$007C(cells);
                        if (activePatternResult_5 != null) {
                            const ph = activePatternResult_5;
                            return ph;
                        }
                        else {
                            const activePatternResult_6 = ActivePattern_$007CFreeText$007C_$007C(cells);
                            if (activePatternResult_6 != null) {
                                const ft = activePatternResult_6;
                                return ft;
                            }
                            else {
                                throw new Error("parseCompositeHeader");
                            }
                        }
                    }
                }
            }
        }
    }
}

export function toFsCells(hasUnit, header) {
    if (header.IsSingleColumn) {
        return singleton(new FsCell(toString(header)));
    }
    else if (header.IsTermColumn) {
        return toList(delay(() => append(singleton_1(new FsCell(toString(header))), delay(() => append(hasUnit ? singleton_1(new FsCell("Unit")) : empty(), delay(() => append(singleton_1(new FsCell(`Term Source REF (${header.GetColumnAccessionShort})`)), delay(() => singleton_1(new FsCell(`Term Accession Number (${header.GetColumnAccessionShort})`))))))))));
    }
    else {
        return toFail(printf("header %O is neither single nor term column"))(header);
    }
}

