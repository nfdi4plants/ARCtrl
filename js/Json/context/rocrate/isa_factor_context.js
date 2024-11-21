import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Factor, ArcFactor, factorName, factorType, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Factor = Factor;
        this.ArcFactor = ArcFactor;
        this.factorName = factorName;
        this.factorType = factorType;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Factor.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Factor", string_type], ["ArcFactor", string_type], ["factorName", string_type], ["factorType", string_type], ["comments", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Factor", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:DefinedTerm");
        },
    }], ["factorName", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:name");
        },
    }], ["annotationValue", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["termSource", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:inDefinedTermSet");
        },
    }], ["termAccession", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:termCode");
        },
    }], ["comments", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:disambiguatingDescription");
        },
    }]];
    return {
        Encode(helpers_7) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_7)], values);
            return helpers_7.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"Factor\": \"sdo:Thing\",\r\n    \"ArcFactor\": \"arc:ARC#ARC_00000044\",\r\n\r\n    \"factorName\": \"arc:ARC#ARC_00000019\",\r\n    \"factorType\": \"arc:ARC#ARC_00000078\",\r\n\r\n    \"comments\": \"sdo:disambiguatingDescription\"\r\n  }\r\n}\r\n    ";

