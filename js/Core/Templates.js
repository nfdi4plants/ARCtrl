import { defaultArg } from "../fable_modules/fable-library-js.4.22.0/Option.js";
import { ResizeArray_append, ResizeArray_collect, ResizeArray_distinct, ResizeArray_filter } from "./Helper/Collections.js";
import { uncurry2, safeHash, equals, disposeSafe, getEnumerator } from "../fable_modules/fable-library-js.4.22.0/Util.js";
import { contains as contains_1 } from "../fable_modules/fable-library-js.4.22.0/Seq.js";
import { addRangeInPlace, item } from "../fable_modules/fable-library-js.4.22.0/Array.js";
import { Array_distinct } from "../fable_modules/fable-library-js.4.22.0/Seq2.js";
import { class_type } from "../fable_modules/fable-library-js.4.22.0/Reflection.js";

export function TemplatesAux_getComparer(matchAll) {
    if (defaultArg(matchAll, false)) {
        return (e) => ((e_1) => (e && e_1));
    }
    else {
        return (e_2) => ((e_3) => (e_2 || e_3));
    }
}

export function TemplatesAux_filterOnTags(tagGetter, queryTags, comparer, templates) {
    return ResizeArray_filter((t) => {
        const templateTags = tagGetter(t);
        let isValid = undefined;
        let enumerator = getEnumerator(queryTags);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const contains = contains_1(enumerator["System.Collections.Generic.IEnumerator`1.get_Current"](), templateTags, {
                    Equals: equals,
                    GetHashCode: safeHash,
                });
                const isValid_1 = isValid;
                if (isValid_1 != null) {
                    const maybe = isValid_1;
                    isValid = comparer(maybe, contains);
                }
                else {
                    isValid = contains;
                }
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        return defaultArg(isValid, false);
    }, templates);
}

export class Templates {
    constructor() {
    }
    static getDistinctTags(templates) {
        return ResizeArray_distinct(ResizeArray_collect((t) => t.Tags, templates));
    }
    static getDistinctEndpointRepositories(templates) {
        return ResizeArray_distinct(ResizeArray_collect((t) => t.EndpointRepositories, templates));
    }
    static getDistinctOntologyAnnotations(templates) {
        const oas = [];
        for (let idx = 0; idx <= (templates.length - 1); idx++) {
            const t = item(idx, templates);
            addRangeInPlace(t.Tags, oas);
            addRangeInPlace(t.EndpointRepositories, oas);
        }
        return Array_distinct(Array.from(oas), {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    static filterByTags(queryTags, matchAll) {
        return (templates) => TemplatesAux_filterOnTags((t) => t.Tags, queryTags, uncurry2(TemplatesAux_getComparer(matchAll)), templates);
    }
    static filterByEndpointRepositories(queryTags, matchAll) {
        return (templates) => TemplatesAux_filterOnTags((t) => t.EndpointRepositories, queryTags, uncurry2(TemplatesAux_getComparer(matchAll)), templates);
    }
    static filterByOntologyAnnotation(queryTags, matchAll) {
        return (templates) => TemplatesAux_filterOnTags((t) => ResizeArray_append(t.Tags, t.EndpointRepositories), queryTags, uncurry2(TemplatesAux_getComparer(matchAll)), templates);
    }
    static filterByDataPLANT(templates) {
        return ResizeArray_filter((t) => t.Organisation.IsOfficial(), templates);
    }
}

export function Templates_$reflection() {
    return class_type("ARCtrl.Templates", undefined, Templates);
}

