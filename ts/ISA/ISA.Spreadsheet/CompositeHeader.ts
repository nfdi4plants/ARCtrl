import { printf, toFail } from "../../fable_modules/fable-library-ts/String.js";
import { value, Option } from "../../fable_modules/fable-library-ts/Option.js";
import { singleton, tail, head, isEmpty, FSharpList, map } from "../../fable_modules/fable-library-ts/List.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.3.1.1/Cells/FsCell.fs.js";
import { OntologyAnnotation, OntologyAnnotation_fromString_Z7D8EB286 } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { ActivePatterns_$007COutputColumnHeader$007C_$007C, ActivePatterns_$007CInputColumnHeader$007C_$007C, tryParseCharacteristicColumnHeader, tryParseFactorColumnHeader, tryParseParameterColumnHeader, ActivePatterns_$007CUnitColumnHeader$007C_$007C, ActivePatterns_$007CTANColumnHeader$007C_$007C, ActivePatterns_$007CTSRColumnHeader$007C_$007C } from "../ISA/Regex.js";
import { CompositeHeader_FreeText, CompositeHeader_Date, CompositeHeader_Performer, CompositeHeader_ProtocolVersion, CompositeHeader_ProtocolUri, CompositeHeader_ProtocolDescription, CompositeHeader_ProtocolREF, CompositeHeader_ProtocolType, CompositeHeader_Output, CompositeHeader_Input, IOType, CompositeHeader_Characteristic, CompositeHeader_Factor, CompositeHeader_Parameter, CompositeHeader_$union } from "../ISA/ArcTypes/CompositeHeader.js";
import { toString } from "../../fable_modules/fable-library-ts/Types.js";
import { empty, singleton as singleton_1, append, delay, toList } from "../../fable_modules/fable-library-ts/Seq.js";

export function ActivePattern_mergeTerms(tsr1: string, tan1: string, tsr2: string, tan2: string): { TermAccessionNumber: string, TermSourceRef: string } {
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

export function ActivePattern_$007CTerm$007C_$007C(categoryParser: ((arg0: string) => Option<string>), f: ((arg0: OntologyAnnotation) => CompositeHeader_$union), cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const $007CAC$007C_$007C = categoryParser;
    const cellValues: FSharpList<string> = map<FsCell, string>((c: FsCell): string => c.Value, cells);
    let matchResult: int32, name: string;
    if (!isEmpty(cellValues)) {
        const activePatternResult: Option<string> = $007CAC$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                name = value(activePatternResult);
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
            return f(OntologyAnnotation_fromString_Z7D8EB286(name!));
        default: {
            let matchResult_1: int32, name_1: string, term1: { LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }, term2: { LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string };
            if (!isEmpty(cellValues)) {
                const activePatternResult_1: Option<string> = $007CAC$007C_$007C(head(cellValues));
                if (activePatternResult_1 != null) {
                    if (!isEmpty(tail(cellValues))) {
                        const activePatternResult_2: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(cellValues)));
                        if (activePatternResult_2 != null) {
                            if (!isEmpty(tail(tail(cellValues)))) {
                                const activePatternResult_3: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                if (activePatternResult_3 != null) {
                                    if (!isEmpty(tail(tail(tail(cellValues))))) {
                                        const activePatternResult_4: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(tail(cellValues)))));
                                        if (activePatternResult_4 != null) {
                                            if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                                                const activePatternResult_5: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                                if (activePatternResult_5 != null) {
                                                    if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(head(tail(cellValues))) != null) {
                                                        matchResult_1 = 0;
                                                        name_1 = value(activePatternResult_1);
                                                        term1 = value(activePatternResult_5);
                                                        term2 = value(activePatternResult_4);
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
                                        name_1 = value(activePatternResult_1);
                                        term1 = value(activePatternResult_2);
                                        term2 = value(activePatternResult_3);
                                    }
                                }
                                else {
                                    const activePatternResult_7: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                    if (activePatternResult_7 != null) {
                                        if (!isEmpty(tail(tail(tail(cellValues))))) {
                                            const activePatternResult_8: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(tail(cellValues)))));
                                            if (activePatternResult_8 != null) {
                                                if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                                                    if (ActivePatterns_$007CUnitColumnHeader$007C_$007C(head(tail(cellValues))) != null) {
                                                        matchResult_1 = 0;
                                                        name_1 = value(activePatternResult_1);
                                                        term1 = value(activePatternResult_7);
                                                        term2 = value(activePatternResult_8);
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
                                const activePatternResult_11: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTSRColumnHeader$007C_$007C(head(tail(tail(cellValues))));
                                if (activePatternResult_11 != null) {
                                    if (!isEmpty(tail(tail(tail(cellValues))))) {
                                        const activePatternResult_12: Option<{ LocalTAN: string, TermAccessionNumber: string, TermSourceREF: string }> = ActivePatterns_$007CTANColumnHeader$007C_$007C(head(tail(tail(tail(cellValues)))));
                                        if (activePatternResult_12 != null) {
                                            if (isEmpty(tail(tail(tail(tail(cellValues)))))) {
                                                matchResult_1 = 0;
                                                name_1 = value(activePatternResult_1);
                                                term1 = value(activePatternResult_11);
                                                term2 = value(activePatternResult_12);
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
                    const term: { TermAccessionNumber: string, TermSourceRef: string } = ActivePattern_mergeTerms(term1!.TermSourceREF, term1!.TermAccessionNumber, term2!.TermSourceREF, term2!.TermAccessionNumber);
                    return f(OntologyAnnotation_fromString_Z7D8EB286(name_1!, term.TermSourceRef, term.TermAccessionNumber));
                }
                default:
                    return void 0;
            }
        }
    }
}

export function ActivePattern_$007CParameter$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const activePatternResult: Option<CompositeHeader_$union> = ActivePattern_$007CTerm$007C_$007C(tryParseParameterColumnHeader, CompositeHeader_Parameter, cells);
    if (activePatternResult != null) {
        const r: CompositeHeader_$union = value(activePatternResult);
        return r;
    }
    else {
        return void 0;
    }
}

export function ActivePattern_$007CFactor$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const activePatternResult: Option<CompositeHeader_$union> = ActivePattern_$007CTerm$007C_$007C(tryParseFactorColumnHeader, CompositeHeader_Factor, cells);
    if (activePatternResult != null) {
        const r: CompositeHeader_$union = value(activePatternResult);
        return r;
    }
    else {
        return void 0;
    }
}

export function ActivePattern_$007CCharacteristic$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const activePatternResult: Option<CompositeHeader_$union> = ActivePattern_$007CTerm$007C_$007C(tryParseCharacteristicColumnHeader, CompositeHeader_Characteristic, cells);
    if (activePatternResult != null) {
        const r: CompositeHeader_$union = value(activePatternResult);
        return r;
    }
    else {
        return void 0;
    }
}

export function ActivePattern_$007CInput$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const cellValues: FSharpList<string> = map<FsCell, string>((c: FsCell): string => c.Value, cells);
    let matchResult: int32, ioType: string;
    if (!isEmpty(cellValues)) {
        const activePatternResult: Option<string> = ActivePatterns_$007CInputColumnHeader$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                ioType = value(activePatternResult);
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
            return CompositeHeader_Input(IOType.ofString(ioType!));
        default:
            return void 0;
    }
}

export function ActivePattern_$007COutput$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const cellValues: FSharpList<string> = map<FsCell, string>((c: FsCell): string => c.Value, cells);
    let matchResult: int32, ioType: string;
    if (!isEmpty(cellValues)) {
        const activePatternResult: Option<string> = ActivePatterns_$007COutputColumnHeader$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                ioType = value(activePatternResult);
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
            return CompositeHeader_Output(IOType.ofString(ioType!));
        default:
            return void 0;
    }
}

export function ActivePattern_$007CProtocolHeader$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const cellValues: FSharpList<string> = map<FsCell, string>((c: FsCell): string => c.Value, cells);
    let matchResult: int32;
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
            return CompositeHeader_ProtocolType();
        case 1:
            return CompositeHeader_ProtocolREF();
        case 2:
            return CompositeHeader_ProtocolDescription();
        case 3:
            return CompositeHeader_ProtocolUri();
        case 4:
            return CompositeHeader_ProtocolVersion();
        case 5:
            return CompositeHeader_Performer();
        case 6:
            return CompositeHeader_Date();
        default:
            return void 0;
    }
}

export function ActivePattern_$007CFreeText$007C_$007C(cells: FSharpList<FsCell>): Option<CompositeHeader_$union> {
    const cellValues: FSharpList<string> = map<FsCell, string>((c: FsCell): string => c.Value, cells);
    let matchResult: int32, text: string;
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
            return CompositeHeader_FreeText(text!);
        default:
            return void 0;
    }
}

export function fromFsCells(cells: FSharpList<FsCell>): CompositeHeader_$union {
    const activePatternResult: Option<CompositeHeader_$union> = ActivePattern_$007CParameter$007C_$007C(cells);
    if (activePatternResult != null) {
        const p: CompositeHeader_$union = value(activePatternResult);
        return p;
    }
    else {
        const activePatternResult_1: Option<CompositeHeader_$union> = ActivePattern_$007CFactor$007C_$007C(cells);
        if (activePatternResult_1 != null) {
            const f: CompositeHeader_$union = value(activePatternResult_1);
            return f;
        }
        else {
            const activePatternResult_2: Option<CompositeHeader_$union> = ActivePattern_$007CCharacteristic$007C_$007C(cells);
            if (activePatternResult_2 != null) {
                const c: CompositeHeader_$union = value(activePatternResult_2);
                return c;
            }
            else {
                const activePatternResult_3: Option<CompositeHeader_$union> = ActivePattern_$007CInput$007C_$007C(cells);
                if (activePatternResult_3 != null) {
                    const i: CompositeHeader_$union = value(activePatternResult_3);
                    return i;
                }
                else {
                    const activePatternResult_4: Option<CompositeHeader_$union> = ActivePattern_$007COutput$007C_$007C(cells);
                    if (activePatternResult_4 != null) {
                        const o: CompositeHeader_$union = value(activePatternResult_4);
                        return o;
                    }
                    else {
                        const activePatternResult_5: Option<CompositeHeader_$union> = ActivePattern_$007CProtocolHeader$007C_$007C(cells);
                        if (activePatternResult_5 != null) {
                            const ph: CompositeHeader_$union = value(activePatternResult_5);
                            return ph;
                        }
                        else {
                            const activePatternResult_6: Option<CompositeHeader_$union> = ActivePattern_$007CFreeText$007C_$007C(cells);
                            if (activePatternResult_6 != null) {
                                const ft: CompositeHeader_$union = value(activePatternResult_6);
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

export function toFsCells(hasUnit: boolean, header: CompositeHeader_$union): FSharpList<FsCell> {
    if (header.IsSingleColumn) {
        return singleton(new FsCell(toString(header)));
    }
    else if (header.IsTermColumn) {
        return toList<FsCell>(delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton_1<FsCell>(new FsCell(toString(header))), delay<FsCell>((): Iterable<FsCell> => append<FsCell>(hasUnit ? singleton_1<FsCell>(new FsCell("Unit")) : empty<FsCell>(), delay<FsCell>((): Iterable<FsCell> => append<FsCell>(singleton_1<FsCell>(new FsCell(`Term Source REF (${header.GetColumnAccessionShort})`)), delay<FsCell>((): Iterable<FsCell> => singleton_1<FsCell>(new FsCell(`Term Accession Number (${header.GetColumnAccessionShort})`))))))))));
    }
    else {
        return toFail(printf("header %O is neither single nor term column"))(header);
    }
}

