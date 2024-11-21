import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, OntologyAnnotation, annotationValue, termSource, termAccession, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.OntologyAnnotation = OntologyAnnotation;
        this.annotationValue = annotationValue;
        this.termSource = termSource;
        this.termAccession = termAccession;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.OntologyAnnotation.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["OntologyAnnotation", string_type], ["annotationValue", string_type], ["termSource", string_type], ["termAccession", string_type], ["comments", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["OntologyAnnotation", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:DefinedTerm");
        },
    }], ["annotationValue", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:name");
        },
    }], ["termSource", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:inDefinedTermSet");
        },
    }], ["termAccession", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:termCode");
        },
    }], ["comments", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:disambiguatingDescription");
        },
    }]];
    return {
        Encode(helpers_6) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
})();

