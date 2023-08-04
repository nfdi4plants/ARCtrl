import { equals } from "../../fable_modules/fable-library-ts/Util.js";
import { value, Option, some } from "../../fable_modules/fable-library-ts/Option.js";
import { map, equalsWith, fill, map3, fold } from "../../fable_modules/fable-library-ts/Array.js";
import { toText, printf, toFail } from "../../fable_modules/fable-library-ts/String.js";
import { int32 } from "../../fable_modules/fable-library-ts/Int32.js";
import { OntologyAnnotation } from "../ISA/JsonTypes/OntologyAnnotation.js";
import { Component_toString_Z609B8895, Component_fromString_Z61E08C1, Component } from "../ISA/JsonTypes/Component.js";
import { map as map_1, fold as fold_1, FSharpList, ofArray, empty } from "../../fable_modules/fable-library-ts/List.js";
import { Array_map4 } from "./CollectionAux.js";
import { ProtocolParameter_toString_Z3A4310A5, ProtocolParameter, ProtocolParameter_make } from "../ISA/JsonTypes/ProtocolParameter.js";

/**
 * If the value matches the default, a None is returned, else a Some is returned
 */
export function Option_fromValueWithDefault<$a>(d: $a, v: $a): Option<$a> {
    if (equals(d, v)) {
        return void 0;
    }
    else {
        return some(v);
    }
}

/**
 * Applies the function f on the value of the option if it exists, else applies it on the default value. If the result value matches the default, a None is returned
 */
export function Option_mapDefault<T>(d: T, f: ((arg0: T) => T), o: Option<T>): Option<T> {
    return Option_fromValueWithDefault<T>(d, (o == null) ? f(d) : f(value(o)));
}

/**
 * Returns the length of a subpropertylist from the aggregated strings
 * 
 * In ISATab format, some subproperties which are stored as lists in ISAJson are stored as semicolon delimited tables
 * 
 * These strings should either contain the same number of semicolon delimited elements or be empty.
 */
export function OntologyAnnotation_getLengthOfAggregatedStrings(separator: string, strings: string[]): int32 {
    return fold<string, int32>((l: int32, s: string): int32 => {
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
export function OntologyAnnotation_fromAggregatedStrings(separator: string, terms: string, source: string, accessions: string): OntologyAnnotation[] {
    const l: int32 = OntologyAnnotation_getLengthOfAggregatedStrings(separator, [terms, source, accessions]) | 0;
    if (l === 0) {
        return [];
    }
    else {
        return map3<string, string, string, OntologyAnnotation>((a: string, b: string, c: string): OntologyAnnotation => OntologyAnnotation.fromString(a, b, c), (terms === "") ? fill(new Array(l), 0, l, "") : terms.split(separator), (source === "") ? fill(new Array(l), 0, l, "") : source.split(separator), (accessions === "") ? fill(new Array(l), 0, l, "") : accessions.split(separator));
    }
}

/**
 * Returns the aggregated ISATab OntologyAnnotation Name, ontology source and Accession number from a list of ISAJson OntologyAnnotation objects
 */
export function OntologyAnnotation_toAggregatedStrings(separator: string, oas: OntologyAnnotation[]): { TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } {
    let first = true;
    if (equalsWith(equals, oas, [])) {
        return {
            TermAccessionNumberAgg: "",
            TermNameAgg: "",
            TermSourceREFAgg: "",
        };
    }
    else {
        const tupledArg_1: [string, string, string] = fold<{ TermAccessionNumber: string, TermName: string, TermSourceREF: string }, [string, string, string]>((tupledArg: [string, string, string], term: { TermAccessionNumber: string, TermName: string, TermSourceREF: string }): [string, string, string] => {
            if (first) {
                first = false;
                return [term.TermName, term.TermSourceREF, term.TermAccessionNumber] as [string, string, string];
            }
            else {
                return [toText(printf("%s%c%s"))(tupledArg[0])(separator)(term.TermName), toText(printf("%s%c%s"))(tupledArg[1])(separator)(term.TermSourceREF), toText(printf("%s%c%s"))(tupledArg[2])(separator)(term.TermAccessionNumber)] as [string, string, string];
            }
        }, ["", "", ""] as [string, string, string], map<OntologyAnnotation, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>((arg: OntologyAnnotation): { TermAccessionNumber: string, TermName: string, TermSourceREF: string } => OntologyAnnotation.toString(arg), oas));
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
export function Component_fromAggregatedStrings(separator: string, names: string, terms: string, source: string, accessions: string): FSharpList<Component> {
    const l: int32 = OntologyAnnotation_getLengthOfAggregatedStrings(separator, [names, terms, source, accessions]) | 0;
    if (l === 0) {
        return empty<Component>();
    }
    else {
        return ofArray<Component>(Array_map4<string, string, string, string, Component>(Component_fromString_Z61E08C1, (names === "") ? fill(new Array(l), 0, l, "") : names.split(separator), (terms === "") ? fill(new Array(l), 0, l, "") : terms.split(separator), (source === "") ? fill(new Array(l), 0, l, "") : source.split(separator), (accessions === "") ? fill(new Array(l), 0, l, "") : accessions.split(separator)));
    }
}

/**
 * Returns the aggregated ISATAb Component Name, Ontology Annotation value, Accession number and ontology source from a list of ISAJson Component objects
 */
export function Component_toAggregatedStrings(separator: string, cs: FSharpList<Component>): { NameAgg: string, TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } {
    let first = true;
    if (equals(cs, empty<Component>())) {
        return {
            NameAgg: "",
            TermAccessionNumberAgg: "",
            TermNameAgg: "",
            TermSourceREFAgg: "",
        };
    }
    else {
        const tupledArg_2: [string, string, string, string] = fold_1<[string, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }], [string, string, string, string]>((tupledArg: [string, string, string, string], tupledArg_1: [string, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }]): [string, string, string, string] => {
            const name: string = tupledArg_1[0];
            const term: { TermAccessionNumber: string, TermName: string, TermSourceREF: string } = tupledArg_1[1];
            if (first) {
                first = false;
                return [name, term.TermName, term.TermSourceREF, term.TermAccessionNumber] as [string, string, string, string];
            }
            else {
                return [toText(printf("%s%c%s"))(tupledArg[0])(separator)(name), toText(printf("%s%c%s"))(tupledArg[1])(separator)(term.TermName), toText(printf("%s%c%s"))(tupledArg[2])(separator)(term.TermSourceREF), toText(printf("%s%c%s"))(tupledArg[3])(separator)(term.TermAccessionNumber)] as [string, string, string, string];
            }
        }, ["", "", "", ""] as [string, string, string, string], map_1<Component, [string, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }]>(Component_toString_Z609B8895, cs));
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
export function ProtocolParameter_fromAggregatedStrings(separator: string, terms: string, source: string, accessions: string): ProtocolParameter[] {
    return map<OntologyAnnotation, ProtocolParameter>((arg_3: OntologyAnnotation): ProtocolParameter => ((arg_2: Option<OntologyAnnotation>): ProtocolParameter => ProtocolParameter_make(void 0, arg_2))(arg_3), OntologyAnnotation_fromAggregatedStrings(separator, terms, source, accessions));
}

/**
 * Returns the aggregated ISATAb Ontology Annotation value, Accession number and ontology source from a list of ISAJson ProtocolParameter objects
 */
export function ProtocolParameter_toAggregatedStrings(separator: string, oas: FSharpList<ProtocolParameter>): { TermAccessionNumberAgg: string, TermNameAgg: string, TermSourceREFAgg: string } {
    let first = true;
    if (equals(oas, empty<ProtocolParameter>())) {
        return {
            TermAccessionNumberAgg: "",
            TermNameAgg: "",
            TermSourceREFAgg: "",
        };
    }
    else {
        const tupledArg_1: [string, string, string] = fold_1<{ TermAccessionNumber: string, TermName: string, TermSourceREF: string }, [string, string, string]>((tupledArg: [string, string, string], term: { TermAccessionNumber: string, TermName: string, TermSourceREF: string }): [string, string, string] => {
            if (first) {
                first = false;
                return [term.TermName, term.TermSourceREF, term.TermAccessionNumber] as [string, string, string];
            }
            else {
                return [toText(printf("%s%c%s"))(tupledArg[0])(separator)(term.TermName), toText(printf("%s%c%s"))(tupledArg[1])(separator)(term.TermSourceREF), toText(printf("%s%c%s"))(tupledArg[2])(separator)(term.TermAccessionNumber)] as [string, string, string];
            }
        }, ["", "", ""] as [string, string, string], map_1<ProtocolParameter, { TermAccessionNumber: string, TermName: string, TermSourceREF: string }>(ProtocolParameter_toString_Z3A4310A5, oas));
        return {
            TermAccessionNumberAgg: tupledArg_1[2],
            TermNameAgg: tupledArg_1[0],
            TermSourceREFAgg: tupledArg_1[1],
        };
    }
}

