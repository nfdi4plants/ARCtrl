import { equals } from "../../fable_modules/fable-library-js.4.22.0/Util.js";
import { defaultArg, value, some } from "../../fable_modules/fable-library-js.4.22.0/Option.js";
import { equalsWith, fill, map3, fold } from "../../fable_modules/fable-library-js.4.22.0/Array.js";
import { toText, printf, toFail } from "../../fable_modules/fable-library-js.4.22.0/String.js";
import { OntologyAnnotation } from "../../Core/OntologyAnnotation.js";
import { map, fold as fold_1, ofArray, empty } from "../../fable_modules/fable-library-js.4.22.0/List.js";
import { Array_map4 } from "../CollectionAux.js";
import { Component_toStringObject_Z685B8F25, Component_fromISAString_7C9A7CF8 } from "../../Core/Process/Component.js";
import { ResizeArray_map } from "../../Core/Helper/Collections.js";
import { ProtocolParameter } from "../../Core/Process/ProtocolParameter.js";

/**
 * If the value matches the default, a None is returned, else a Some is returned
 */
export function Option_fromValueWithDefault(d, v) {
    if (equals(d, v)) {
        return undefined;
    }
    else {
        return some(v);
    }
}

/**
 * Applies the function f on the value of the option if it exists, else applies it on the default value. If the result value matches the default, a None is returned
 */
export function Option_mapDefault(d, f, o) {
    return Option_fromValueWithDefault(d, (o == null) ? f(d) : f(value(o)));
}

/**
 * Returns the length of a subpropertylist from the aggregated strings
 * 
 * In ISATab format, some subproperties which are stored as lists in ISAJson are stored as semicolon delimited tables
 * 
 * These strings should either contain the same number of semicolon delimited elements or be empty.
 */
export function OntologyAnnotation_getLengthOfAggregatedStrings(separator, strings) {
    return fold((l, s) => {
        if (s === "") {
            return l | 0;
        }
        else if (l === 0) {
            return s.split(separator).length | 0;
        }
        else if (l === s.split(separator).length) {
            return l | 0;
        }
        else {
            return toFail(printf("The length of the aggregated string %s does not match the length of the others"))(s) | 0;
        }
    }, 0, strings);
}

/**
 * Returns a list of ISAJson OntologyAnnotation objects from ISATab aggregated strings
 */
export function OntologyAnnotation_fromAggregatedStrings(separator, terms, source, accessions) {
    const l = OntologyAnnotation_getLengthOfAggregatedStrings(separator, [terms, source, accessions]) | 0;
    if (l === 0) {
        return [];
    }
    else {
        const collection = map3((a, b, c) => OntologyAnnotation.create(a, b, c), (terms === "") ? fill(new Array(l), 0, l, "") : terms.split(separator), (source === "") ? fill(new Array(l), 0, l, "") : source.split(separator), (accessions === "") ? fill(new Array(l), 0, l, "") : accessions.split(separator));
        return Array.from(collection);
    }
}

/**
 * Returns the aggregated ISATab OntologyAnnotation Name, ontology source and Accession number from a list of ISAJson OntologyAnnotation objects
 */
export function OntologyAnnotation_toAggregatedStrings(separator, oas) {
    let first = true;
    if (equalsWith(equals, oas, [])) {
        return {
            TermAccessionNumberAgg: "",
            TermNameAgg: "",
            TermSourceREFAgg: "",
        };
    }
    else {
        const tupledArg_1 = fold((tupledArg, term) => {
            const matchValue = defaultArg(term.Name, "");
            const tsr = defaultArg(term.TermSourceREF, "");
            const tan = defaultArg(term.TermAccessionNumber, "");
            const name = matchValue;
            if (first) {
                first = false;
                return [name, tsr, tan];
            }
            else {
                return [toText(printf("%s%c%s"))(tupledArg[0])(separator)(name), toText(printf("%s%c%s"))(tupledArg[1])(separator)(tsr), toText(printf("%s%c%s"))(tupledArg[2])(separator)(tan)];
            }
        }, ["", "", ""], oas);
        return {
            TermAccessionNumberAgg: tupledArg_1[2],
            TermNameAgg: tupledArg_1[0],
            TermSourceREFAgg: tupledArg_1[1],
        };
    }
}

/**
 * Returns a list of ISAJson Component objects from ISATab aggregated strings
 */
export function Component_fromAggregatedStrings(separator, names, terms, source, accessions) {
    const l = OntologyAnnotation_getLengthOfAggregatedStrings(separator, [names, terms, source, accessions]) | 0;
    if (l === 0) {
        return empty();
    }
    else {
        return ofArray(Array_map4(Component_fromISAString_7C9A7CF8, (names === "") ? fill(new Array(l), 0, l, "") : names.split(separator), (terms === "") ? fill(new Array(l), 0, l, "") : terms.split(separator), (source === "") ? fill(new Array(l), 0, l, "") : source.split(separator), (accessions === "") ? fill(new Array(l), 0, l, "") : accessions.split(separator)));
    }
}

/**
 * Returns the aggregated ISATAb Component Name, Ontology Annotation value, Accession number and ontology source from a list of ISAJson Component objects
 */
export function Component_toAggregatedStrings(separator, cs) {
    let first = true;
    if (equals(cs, empty())) {
        return {
            NameAgg: "",
            TermAccessionNumberAgg: "",
            TermNameAgg: "",
            TermSourceREFAgg: "",
        };
    }
    else {
        const tupledArg_2 = fold_1((tupledArg, tupledArg_1) => {
            const name = tupledArg_1[0];
            const term = tupledArg_1[1];
            if (first) {
                first = false;
                return [name, term.TermName, term.TermSourceREF, term.TermAccessionNumber];
            }
            else {
                return [toText(printf("%s%c%s"))(tupledArg[0])(separator)(name), toText(printf("%s%c%s"))(tupledArg[1])(separator)(term.TermName), toText(printf("%s%c%s"))(tupledArg[2])(separator)(term.TermSourceREF), toText(printf("%s%c%s"))(tupledArg[3])(separator)(term.TermAccessionNumber)];
            }
        }, ["", "", "", ""], map(Component_toStringObject_Z685B8F25, cs));
        return {
            NameAgg: tupledArg_2[0],
            TermAccessionNumberAgg: tupledArg_2[3],
            TermNameAgg: tupledArg_2[1],
            TermSourceREFAgg: tupledArg_2[2],
        };
    }
}

/**
 * Returns a list of ISAJson ProtocolParameter objects from ISATab aggregated strings
 */
export function ProtocolParameter_fromAggregatedStrings(separator, terms, source, accessions) {
    return ResizeArray_map((arg) => ((parameterName) => ProtocolParameter.make(undefined, parameterName))(arg), OntologyAnnotation_fromAggregatedStrings(separator, terms, source, accessions));
}

/**
 * Returns the aggregated ISATAb Ontology Annotation value, Accession number and ontology source from a list of ISAJson ProtocolParameter objects
 */
export function ProtocolParameter_toAggregatedStrings(separator, oas) {
    let first = true;
    if (equals(oas, empty())) {
        return {
            TermAccessionNumberAgg: "",
            TermNameAgg: "",
            TermSourceREFAgg: "",
        };
    }
    else {
        const tupledArg_1 = fold_1((tupledArg, term) => {
            if (first) {
                first = false;
                return [term.TermName, term.TermSourceREF, term.TermAccessionNumber];
            }
            else {
                return [toText(printf("%s%c%s"))(tupledArg[0])(separator)(term.TermName), toText(printf("%s%c%s"))(tupledArg[1])(separator)(term.TermSourceREF), toText(printf("%s%c%s"))(tupledArg[2])(separator)(term.TermAccessionNumber)];
            }
        }, ["", "", ""], map((pp) => ProtocolParameter.toStringObject(pp), oas));
        return {
            TermAccessionNumberAgg: tupledArg_1[2],
            TermNameAgg: tupledArg_1[0],
            TermSourceREFAgg: tupledArg_1[1],
        };
    }
}

