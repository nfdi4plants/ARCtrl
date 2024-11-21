import { tryFindIndex, tail, head, isEmpty, item, map } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { unwrap, map as map_1 } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { Data } from "../../Core/Data.js";
import { DataContext__set_GeneratedBy_6DFDD678, DataContext__set_Description_6DFDD678, DataContext__set_ObjectType_279AAFF2, DataContext__set_Unit_279AAFF2, DataContext__set_Explication_279AAFF2 } from "../../Core/DataContext.js";
import { Comment$ } from "../../Core/Comment.js";
import { ActivePatterns_$007CComment$007C_$007C } from "../../Core/Helper/Regex.js";
import { printf, toFail, join } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { map as map_2, append, delay, toList } from "../../fable_modules/fable-library-js.4.22.0/Seq.js";
import { FsCell } from "../../fable_modules/FsSpreadsheet.6.3.0-alpha.4/Cells/FsCell.fs.js";

export function ActivePattern_ontologyAnnotationFromFsCells(tsrCol, tanCol, cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    const tsr = map_1((i) => item(i, cellValues), tsrCol);
    const tan = map_1((i_1) => item(i_1, cellValues), tanCol);
    return new OntologyAnnotation(item(0, cellValues), unwrap(tsr), unwrap(tan));
}

export function ActivePattern_freeTextFromFsCells(cells) {
    return item(0, map((c) => c.ValueAsString(), cells));
}

export function ActivePattern_dataFromFsCells(format, selectorFormat, cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    const format_1 = map_1((i) => item(i, cellValues), format);
    const selectorFormat_1 = map_1((i_1) => item(i_1, cellValues), selectorFormat);
    return new Data(undefined, item(0, cellValues), undefined, unwrap(format_1), unwrap(selectorFormat_1));
}

export function ActivePattern_$007CTerm$007C_$007C(categoryString, cells) {
    const $007CAC$007C_$007C = (s) => {
        if (s === categoryString) {
            return 1;
        }
        else {
            return undefined;
        }
    };
    const $007CTSRColumnHeaderRaw$007C_$007C = (s_1) => {
        if (s_1.startsWith("Term Source REF")) {
            return s_1;
        }
        else {
            return undefined;
        }
    };
    const $007CTANColumnHeaderRaw$007C_$007C = (s_2) => {
        if (s_2.startsWith("Term Accession Number")) {
            return s_2;
        }
        else {
            return undefined;
        }
    };
    const cellValues = map((c) => c.ValueAsString(), cells);
    let matchResult, header;
    if (!isEmpty(cellValues)) {
        const activePatternResult = $007CAC$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
                header = activePatternResult;
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
            return (cells_1) => ActivePattern_ontologyAnnotationFromFsCells(undefined, undefined, cells_1);
        default: {
            let matchResult_1, header_1;
            if (!isEmpty(cellValues)) {
                const activePatternResult_1 = $007CAC$007C_$007C(head(cellValues));
                if (activePatternResult_1 != null) {
                    if (!isEmpty(tail(cellValues))) {
                        if ($007CTSRColumnHeaderRaw$007C_$007C(head(tail(cellValues))) != null) {
                            if (!isEmpty(tail(tail(cellValues)))) {
                                if ($007CTANColumnHeaderRaw$007C_$007C(head(tail(tail(cellValues)))) != null) {
                                    if (isEmpty(tail(tail(tail(cellValues))))) {
                                        matchResult_1 = 0;
                                        header_1 = activePatternResult_1;
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
                case 0:
                    return (cells_2) => ActivePattern_ontologyAnnotationFromFsCells(1, 2, cells_2);
                default: {
                    let matchResult_2, header_2;
                    if (!isEmpty(cellValues)) {
                        const activePatternResult_4 = $007CAC$007C_$007C(head(cellValues));
                        if (activePatternResult_4 != null) {
                            if (!isEmpty(tail(cellValues))) {
                                if ($007CTANColumnHeaderRaw$007C_$007C(head(tail(cellValues))) != null) {
                                    if (!isEmpty(tail(tail(cellValues)))) {
                                        if ($007CTSRColumnHeaderRaw$007C_$007C(head(tail(tail(cellValues)))) != null) {
                                            if (isEmpty(tail(tail(tail(cellValues))))) {
                                                matchResult_2 = 0;
                                                header_2 = activePatternResult_4;
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
                        }
                        else {
                            matchResult_2 = 1;
                        }
                    }
                    else {
                        matchResult_2 = 1;
                    }
                    switch (matchResult_2) {
                        case 0:
                            return (cells_3) => ActivePattern_ontologyAnnotationFromFsCells(2, 1, cells_3);
                        default:
                            return undefined;
                    }
                }
            }
        }
    }
}

export function ActivePattern_$007CExplication$007C_$007C(cells) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C("Explication", cells);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (dc) => ((cells_1) => {
            DataContext__set_Explication_279AAFF2(dc, r(cells_1));
            return dc;
        });
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CUnit$007C_$007C(cells) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C("Unit", cells);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (dc) => ((cells_1) => {
            DataContext__set_Unit_279AAFF2(dc, r(cells_1));
            return dc;
        });
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CObjectType$007C_$007C(cells) {
    const activePatternResult = ActivePattern_$007CTerm$007C_$007C("Object Type", cells);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (dc) => ((cells_1) => {
            DataContext__set_ObjectType_279AAFF2(dc, r(cells_1));
            return dc;
        });
    }
    else {
        return undefined;
    }
}

export function ActivePattern_$007CDescription$007C_$007C(cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    let matchResult;
    if (!isEmpty(cellValues)) {
        if (head(cellValues) === "Description") {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
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
            return (dc) => ((cells_1) => {
                DataContext__set_Description_6DFDD678(dc, ActivePattern_freeTextFromFsCells(cells_1));
                return dc;
            });
        default:
            return undefined;
    }
}

export function ActivePattern_$007CGeneratedBy$007C_$007C(cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    let matchResult;
    if (!isEmpty(cellValues)) {
        if (head(cellValues) === "Generated By") {
            if (isEmpty(tail(cellValues))) {
                matchResult = 0;
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
            return (dc) => ((cells_1) => {
                DataContext__set_GeneratedBy_6DFDD678(dc, ActivePattern_freeTextFromFsCells(cells_1));
                return dc;
            });
        default:
            return undefined;
    }
}

export function ActivePattern_$007CData$007C_$007C(cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    let matchResult, cols;
    if (!isEmpty(cellValues)) {
        if (head(cellValues) === "Data") {
            matchResult = 0;
            cols = tail(cellValues);
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
            return (dc) => ((cells_1) => {
                const d = ActivePattern_dataFromFsCells(map_1((y) => (1 + y), tryFindIndex((s) => s.startsWith("Data Format"), cols)), map_1((y_1) => (1 + y_1), tryFindIndex((s_1) => s_1.startsWith("Data Selector Format"), cols)), cells_1);
                dc.FilePath = d.FilePath;
                dc.Selector = d.Selector;
                dc.Format = d.Format;
                dc.SelectorFormat = d.SelectorFormat;
                return dc;
            });
        default:
            return undefined;
    }
}

export function ActivePattern_$007CComment$007C_$007C(cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    let matchResult, key;
    if (!isEmpty(cellValues)) {
        const activePatternResult = ActivePatterns_$007CComment$007C_$007C(head(cellValues));
        if (activePatternResult != null) {
            if (isEmpty(tail(cellValues))) {
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
    }
    else {
        matchResult = 1;
    }
    switch (matchResult) {
        case 0:
            return (dc) => ((cells_1) => {
                const comment = item(0, map((c_1) => c_1.ValueAsString(), cells_1));
                void (dc.Comments.push(Comment$.create(key, comment)));
                return dc;
            });
        default:
            return undefined;
    }
}

export function ActivePattern_$007CFreetext$007C_$007C(cells) {
    const cellValues = map((c) => c.ValueAsString(), cells);
    let matchResult, key;
    if (!isEmpty(cellValues)) {
        if (isEmpty(tail(cellValues))) {
            matchResult = 0;
            key = head(cellValues);
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
            return (dc) => ((cells_1) => {
                const comment = item(0, map((c_1) => c_1.ValueAsString(), cells_1));
                void (dc.Comments.push(Comment$.create(key, comment)));
                return dc;
            });
        default:
            return undefined;
    }
}

export function fromFsCells(cells) {
    let matchResult, r;
    const activePatternResult = ActivePattern_$007CExplication$007C_$007C(cells);
    if (activePatternResult != null) {
        matchResult = 0;
        r = activePatternResult;
    }
    else {
        const activePatternResult_1 = ActivePattern_$007CUnit$007C_$007C(cells);
        if (activePatternResult_1 != null) {
            matchResult = 0;
            r = activePatternResult_1;
        }
        else {
            const activePatternResult_2 = ActivePattern_$007CObjectType$007C_$007C(cells);
            if (activePatternResult_2 != null) {
                matchResult = 0;
                r = activePatternResult_2;
            }
            else {
                const activePatternResult_3 = ActivePattern_$007CDescription$007C_$007C(cells);
                if (activePatternResult_3 != null) {
                    matchResult = 0;
                    r = activePatternResult_3;
                }
                else {
                    const activePatternResult_4 = ActivePattern_$007CGeneratedBy$007C_$007C(cells);
                    if (activePatternResult_4 != null) {
                        matchResult = 0;
                        r = activePatternResult_4;
                    }
                    else {
                        const activePatternResult_5 = ActivePattern_$007CData$007C_$007C(cells);
                        if (activePatternResult_5 != null) {
                            matchResult = 0;
                            r = activePatternResult_5;
                        }
                        else {
                            const activePatternResult_6 = ActivePattern_$007CComment$007C_$007C(cells);
                            if (activePatternResult_6 != null) {
                                matchResult = 0;
                                r = activePatternResult_6;
                            }
                            else {
                                const activePatternResult_7 = ActivePattern_$007CFreetext$007C_$007C(cells);
                                if (activePatternResult_7 != null) {
                                    matchResult = 0;
                                    r = activePatternResult_7;
                                }
                                else {
                                    matchResult = 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    switch (matchResult) {
        case 0:
            return (dc) => ((cells_1) => r(dc)(cells_1));
        default: {
            const arg = join(", ", map((c) => c.ValueAsString(), cells));
            const clo_1 = toFail(printf("Could not parse data map column: %s"))(arg);
            return (arg_1) => {
                const clo_2 = clo_1(arg_1);
                return clo_2;
            };
        }
    }
}

export function toFsCells(commentKeys) {
    return toList(delay(() => append([new FsCell("Data"), new FsCell("Data Format"), new FsCell("Data Selector Format")], delay(() => append([new FsCell("Explication"), new FsCell("Term Source REF"), new FsCell("Term Accession Number")], delay(() => append([new FsCell("Unit"), new FsCell("Term Source REF"), new FsCell("Term Accession Number")], delay(() => append([new FsCell("Object Type"), new FsCell("Term Source REF"), new FsCell("Term Accession Number")], delay(() => append([new FsCell("Description")], delay(() => append([new FsCell("Generated By")], delay(() => map_2((ck) => (new FsCell(ck)), commentKeys)))))))))))))));
}

