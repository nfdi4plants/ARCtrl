import { create, match } from "../../fable_modules/fable-library-js.4.22.0/RegExp.js";
import { parse } from "../../fable_modules/fable-library-js.4.22.0/Int32.js";
import { map } from "../../fable_modules/fable-library-js.4.22.0/Option.js";

export function Pattern_handleGroupPatterns(pattern) {
    return pattern;
}

export const Pattern_ExcelNumberFormat = `"(?<${"numberFormat"}>(.*?))"`;

export const Pattern_TermAnnotationShortPattern = `(?<${"idspace"}>\\w+?):(?<${"localid"}>\\w+)`;

export const Pattern_TermAnnotationURIPattern = `http://purl.obolibrary.org/obo/(?<${"idspace"}>\\w+?)_(?<${"localid"}>\\w+)`;

export const Pattern_TermAnnotationURIPattern_lessRestrictive = `.*\\/(?<${"idspace"}>\\w+?)[:_](?<${"localid"}>\\w+)`;

export const Pattern_TermAnnotationURIPattern_MS_RO_PO = `.*252F(?<${"idspace"}>\\w+?)_(?<${"localid"}>\\w+)`;

export const Pattern_IOTypePattern = `(Input|Output)\\s*\\[(?<${"iotype"}>.+)\\]`;

export const Pattern_InputPattern = `Input\\s*\\[(?<${"iotype"}>.+)\\]`;

export const Pattern_OutputPattern = `Output\\s*\\[(?<${"iotype"}>.+)\\]`;

export const Pattern_CommentPattern = `Comment\\s*\\[(?<${"commentKey"}>.+)\\]`;

/**
 * Matches, if the input string matches the given regex pattern.
 */
export function ActivePatterns_$007CRegex$007C_$007C(pattern, input) {
    const m = match(create(Pattern_handleGroupPatterns(pattern)), input.trim());
    if (m != null) {
        return m;
    }
    else {
        return undefined;
    }
}

/**
 * Matches any column header starting with some text, followed by one whitespace and a term name inside squared brackets.
 */
export function ActivePatterns_$007CReferenceColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("(Term Source REF|Term Accession Number)\\s*\\((?<id>.*)\\)", input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return {
            Annotation: (r.groups && r.groups.id) || "",
        };
    }
    else {
        return undefined;
    }
}

/**
 * Matches any column header starting with some text, followed by one whitespace and a term name inside squared brackets.
 */
export function ActivePatterns_$007CTermColumn$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("(?<termcolumntype>.+?)\\s*\\[(?<termname>.+)\\]", input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return {
            TermColumnType: (r.groups && r.groups.termcolumntype) || "",
            TermName: (r.groups && r.groups.termname) || "",
        };
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Unit" column header.
 */
export function ActivePatterns_$007CUnitColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("Unit", input);
    if (activePatternResult != null) {
        const o = activePatternResult;
        return o[0];
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Parameter [Term]" or "Parameter Value [Term]" column header and returns the Term string.
 */
export function ActivePatterns_$007CParameterColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CTermColumn$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        const matchValue = r.TermColumnType;
        switch (matchValue) {
            case "Parameter":
            case "Parameter Value":
                return r.TermName;
            default:
                return undefined;
        }
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Factor [Term]" or "Factor Value [Term]" column header and returns the Term string.
 */
export function ActivePatterns_$007CFactorColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CTermColumn$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        const matchValue = r.TermColumnType;
        switch (matchValue) {
            case "Factor":
            case "Factor Value":
                return r.TermName;
            default:
                return undefined;
        }
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Characteristic [Term]" or "Characteristics [Term]" or "Characteristics Value [Term]" column header and returns the Term string.
 */
export function ActivePatterns_$007CCharacteristicColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CTermColumn$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        const matchValue = r.TermColumnType;
        switch (matchValue) {
            case "Characteristic":
            case "Characteristics":
            case "Characteristics Value":
                return r.TermName;
            default:
                return undefined;
        }
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Component [Term]" or "Component Value [Term]" column header and returns the Term string.
 */
export function ActivePatterns_$007CComponentColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CTermColumn$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        const matchValue = r.TermColumnType;
        switch (matchValue) {
            case "Component":
            case "Component Value":
                return r.TermName;
            default:
                return undefined;
        }
    }
    else {
        return undefined;
    }
}

/**
 * Matches a short term string and returns the term source ref and the annotation number strings.
 * 
 * Example: "MS:1003022" --> term source ref: "MS"; annotation number: "1003022"
 */
export function ActivePatterns_$007CTermAnnotationShort$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationShortPattern, input);
    if (activePatternResult != null) {
        const value = activePatternResult;
        return {
            IDSpace: (value.groups && value.groups.idspace) || "",
            LocalID: (value.groups && value.groups.localid) || "",
        };
    }
    else {
        return undefined;
    }
}

/**
 * Matches a term string (either short or URI) and returns the term source ref and the annotation number strings.
 * 
 * Example 1: "MS:1003022" --> term source ref: "MS"; annotation number: "1003022"
 * 
 * Example 2: "http://purl.obolibrary.org/obo/MS_1003022" --> term source ref: "MS"; annotation number: "1003022"
 */
export function ActivePatterns_$007CTermAnnotation$007C_$007C(input) {
    let matchResult, value;
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationShortPattern, input);
    if (activePatternResult != null) {
        matchResult = 0;
        value = activePatternResult;
    }
    else {
        const activePatternResult_1 = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationURIPattern, input);
        if (activePatternResult_1 != null) {
            matchResult = 0;
            value = activePatternResult_1;
        }
        else {
            const activePatternResult_2 = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationURIPattern_lessRestrictive, input);
            if (activePatternResult_2 != null) {
                matchResult = 0;
                value = activePatternResult_2;
            }
            else {
                const activePatternResult_3 = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationURIPattern_MS_RO_PO, input);
                if (activePatternResult_3 != null) {
                    matchResult = 0;
                    value = activePatternResult_3;
                }
                else {
                    matchResult = 1;
                }
            }
        }
    }
    switch (matchResult) {
        case 0:
            return {
                IDSpace: (value.groups && value.groups.idspace) || "",
                LocalID: (value.groups && value.groups.localid) || "",
            };
        default:
            return undefined;
    }
}

/**
 * Matches a "Term Source REF (ShortTerm)" column header and returns the ShortTerm as Term Source Ref and Annotation Number.
 * 
 * Example: "Term Source REF (MS:1003022)" --> term source ref: "MS"; annotation number: "1003022"
 */
export function ActivePatterns_$007CTSRColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("Term Source REF\\s*\\((?<id>.*)\\)", input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        const matchValue = (r.groups && r.groups.id) || "";
        const activePatternResult_1 = ActivePatterns_$007CTermAnnotation$007C_$007C(matchValue);
        if (activePatternResult_1 != null) {
            const r_1 = activePatternResult_1;
            return r_1;
        }
        else {
            return {
                IDSpace: "",
                LocalID: "",
            };
        }
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Term Accession Number (ShortTerm)" column header and returns the ShortTerm as Term Source Ref and Annotation Number.
 * 
 * Example: "Term Accession Number (MS:1003022)" --> term source ref: "MS"; annotation number: "1003022"
 */
export function ActivePatterns_$007CTANColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("Term Accession Number\\s*\\((?<id>.*)\\)", input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        const matchValue = (r.groups && r.groups.id) || "";
        const activePatternResult_1 = ActivePatterns_$007CTermAnnotation$007C_$007C(matchValue);
        if (activePatternResult_1 != null) {
            const r_1 = activePatternResult_1;
            return r_1;
        }
        else {
            return {
                IDSpace: "",
                LocalID: "",
            };
        }
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Input [InputType]" column header and returns the InputType as string.
 */
export function ActivePatterns_$007CInputColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_InputPattern, input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (r.groups && r.groups.iotype) || "";
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Output [OutputType]" column header and returns the OutputType as string.
 */
export function ActivePatterns_$007COutputColumnHeader$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_OutputPattern, input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (r.groups && r.groups.iotype) || "";
    }
    else {
        return undefined;
    }
}

/**
 * Matches auto-generated readable table names. Mainly used in ArcAssay.addTable(). Default tables will get such a name.
 * 
 * Will match "New Table 10" and return the number `10`.
 */
export function ActivePatterns_$007CAutoGeneratedTableName$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C("^New\\sTable\\s(?<number>\\d+)$", input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return parse((r.groups && r.groups.number) || "", 511, false, 32);
    }
    else {
        return undefined;
    }
}

export function ActivePatterns_$007CComment$007C_$007C(input) {
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_CommentPattern, input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return (r.groups && r.groups.commentKey) || "";
    }
    else {
        return undefined;
    }
}

export function tryParseReferenceColumnHeader(str) {
    const matchValue = str.trim();
    const activePatternResult = ActivePatterns_$007CReferenceColumnHeader$007C_$007C(matchValue);
    if (activePatternResult != null) {
        const v = activePatternResult;
        return v;
    }
    else {
        return undefined;
    }
}

export function tryParseTermAnnotationShort(str) {
    const matchValue = str.trim();
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationShortPattern, matchValue);
    if (activePatternResult != null) {
        const value = activePatternResult;
        return {
            IDSpace: (value.groups && value.groups.idspace) || "",
            LocalID: (value.groups && value.groups.localid) || "",
        };
    }
    else {
        return undefined;
    }
}

/**
 * This function can be used to extract `IDSPACE:LOCALID` (or: `Term Accession`) from Swate header strings or obofoundry conform URI strings.
 * 
 * **Example 1:** "http://purl.obolibrary.org/obo/GO_000001" --> "GO:000001"
 * 
 * **Example 2:** "Term Source REF (NFDI4PSO:0000064)" --> "NFDI4PSO:0000064"
 */
export function tryParseTermAnnotation(str) {
    const matchValue = str.trim();
    let matchResult, value;
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationShortPattern, matchValue);
    if (activePatternResult != null) {
        matchResult = 0;
        value = activePatternResult;
    }
    else {
        const activePatternResult_1 = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationURIPattern, matchValue);
        if (activePatternResult_1 != null) {
            matchResult = 0;
            value = activePatternResult_1;
        }
        else {
            const activePatternResult_2 = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationURIPattern_lessRestrictive, matchValue);
            if (activePatternResult_2 != null) {
                matchResult = 0;
                value = activePatternResult_2;
            }
            else {
                const activePatternResult_3 = ActivePatterns_$007CRegex$007C_$007C(Pattern_TermAnnotationURIPattern_MS_RO_PO, matchValue);
                if (activePatternResult_3 != null) {
                    matchResult = 0;
                    value = activePatternResult_3;
                }
                else {
                    matchResult = 1;
                }
            }
        }
    }
    switch (matchResult) {
        case 0:
            return {
                IDSpace: (value.groups && value.groups.idspace) || "",
                LocalID: (value.groups && value.groups.localid) || "",
            };
        default:
            return undefined;
    }
}

/**
 * Tries to parse 'str' to term accession and returns it in the format `Some "termsourceref:localtan"`. Exmp.: `Some "MS:000001"`
 */
export function tryGetTermAnnotationShortString(str) {
    return map((r) => ((r.IDSpace + ":") + r.LocalID), tryParseTermAnnotation(str));
}

/**
 * Parses 'str' to term accession and returns it in the format "termsourceref:localtan". Exmp.: "MS:000001"
 */
export function getTermAnnotationShortString(str) {
    const matchValue = tryGetTermAnnotationShortString(str);
    if (matchValue == null) {
        throw new Error(`Unable to parse '${str}' to term accession.`);
    }
    else {
        return matchValue;
    }
}

/**
 * This function is used to parse Excel numberFormat string to term name.
 * 
 * **Example 1:** "0.00 "degree Celsius"" --> "degree Celsius"
 */
export function tryParseExcelNumberFormat(headerStr) {
    const matchValue = headerStr.trim();
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_ExcelNumberFormat, matchValue);
    if (activePatternResult != null) {
        const value = activePatternResult;
        return (value.groups && value.groups.numberFormat) || "";
    }
    else {
        return undefined;
    }
}

/**
 * This function is used to match both Input and Output columns and capture the IOType as `iotype` group.
 * 
 * **Example 1:** "Input [Sample]" --> "Sample"
 */
export function tryParseIOTypeHeader(headerStr) {
    const matchValue = headerStr.trim();
    const activePatternResult = ActivePatterns_$007CRegex$007C_$007C(Pattern_IOTypePattern, matchValue);
    if (activePatternResult != null) {
        const value = activePatternResult;
        return (value.groups && value.groups.iotype) || "";
    }
    else {
        return undefined;
    }
}

/**
 * Matches any column header starting with some text, followed by one whitespace and a term name inside squared brackets.
 */
export function tryParseTermColumn(input) {
    const activePatternResult = ActivePatterns_$007CTermColumn$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Unit" column header.
 */
export function tryParseUnitColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CUnitColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Parameter [Term]" or "Parameter Value [Term]" column header and returns the Term string.
 */
export function tryParseParameterColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CParameterColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Factor [Term]" or "Factor Value [Term]" column header and returns the Term string.
 */
export function tryParseFactorColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CFactorColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Characteristic [Term]" or "Characteristics [Term]" or "Characteristics Value [Term]" column header and returns the Term string.
 */
export function tryParseCharacteristicColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CCharacteristicColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Component [Term]" or "Characteristics [Term]" or "Component Value [Term]" column header and returns the Term string.
 */
export function tryParseComponentColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CComponentColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Term Source REF (ShortTerm)" column header and returns the ShortTerm as Term Source Ref and Annotation Number.
 * 
 * Example: "Term Source REF (MS:1003022)" --> term source ref: "MS"; annotation number: "1003022"
 */
export function tryParseTSRColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CTSRColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Term Accession Number (ShortTerm)" column header and returns the ShortTerm as Term Source Ref and Annotation Number.
 * 
 * Example: "Term Accession Number (MS:1003022)" --> term source ref: "MS"; annotation number: "1003022"
 */
export function tryParseTANColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CTANColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Input [InputType]" column header and returns the InputType as string.
 */
export function tryParseInputColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007CInputColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

/**
 * Matches a "Output [OutputType]" column header and returns the OutputType as string.
 */
export function tryParseOutputColumnHeader(input) {
    const activePatternResult = ActivePatterns_$007COutputColumnHeader$007C_$007C(input);
    if (activePatternResult != null) {
        const r = activePatternResult;
        return r;
    }
    else {
        return undefined;
    }
}

