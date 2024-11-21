import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, OntologySourceReference, description, name, file, version, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.OntologySourceReference = OntologySourceReference;
        this.description = description;
        this.name = name;
        this.file = file;
        this.version = version;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.OntologySourceReference.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["OntologySourceReference", string_type], ["description", string_type], ["name", string_type], ["file", string_type], ["version", string_type], ["comments", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["OntologySourceReference", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:DefinedTermSet");
        },
    }], ["description", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:description");
        },
    }], ["name", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["file", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:url");
        },
    }], ["version", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:version");
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

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"OntologySourceReference\": \"sdo:DefinedTermSet\",\r\n    \r\n    \"description\": \"sdo:description\",\r\n    \"name\": \"sdo:name\",\r\n    \"file\": \"sdo:url\",\r\n    \"version\": \"sdo:version\",\r\n    \"comments\": \"sdo:disambiguatingDescription\"\r\n  }\r\n}\r\n    ";

