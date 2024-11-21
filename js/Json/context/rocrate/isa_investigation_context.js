import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Investigation, identifier, title, description, submissionDate, publicReleaseDate, publications, people, studies, ontologySourceReferences, comments, publications$003F, filename) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Investigation = Investigation;
        this.identifier = identifier;
        this.title = title;
        this.description = description;
        this.submissionDate = submissionDate;
        this.publicReleaseDate = publicReleaseDate;
        this.publications = publications;
        this.people = people;
        this.studies = studies;
        this.ontologySourceReferences = ontologySourceReferences;
        this.comments = comments;
        this["publications?"] = publications$003F;
        this.filename = filename;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Investigation.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Investigation", string_type], ["identifier", string_type], ["title", string_type], ["description", string_type], ["submissionDate", string_type], ["publicReleaseDate", string_type], ["publications", string_type], ["people", string_type], ["studies", string_type], ["ontologySourceReferences", string_type], ["comments", string_type], ["publications?", string_type], ["filename", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Investigation", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Dataset");
        },
    }], ["identifier", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:identifier");
        },
    }], ["title", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:headline");
        },
    }], ["additionalType", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:additionalType");
        },
    }], ["description", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:description");
        },
    }], ["submissionDate", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:dateCreated");
        },
    }], ["publicReleaseDate", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:datePublished");
        },
    }], ["publications", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:citation");
        },
    }], ["people", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:creator");
        },
    }], ["studies", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:hasPart");
        },
    }], ["ontologySourceReferences", {
        Encode(helpers_11) {
            return helpers_11.encodeString("sdo:mentions");
        },
    }], ["comments", {
        Encode(helpers_12) {
            return helpers_12.encodeString("sdo:comment");
        },
    }], ["filename", {
        Encode(helpers_13) {
            return helpers_13.encodeString("sdo:alternateName");
        },
    }]];
    return {
        Encode(helpers_14) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_14)], values);
            return helpers_14.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"Investigation\": \"sdo:Dataset\",\r\n\r\n    \"identifier\" : \"sdo:identifier\",\r\n    \"title\": \"sdo:headline\",\r\n    \"description\": \"sdo:description\",\r\n    \"submissionDate\": \"sdo:dateCreated\",\r\n    \"publicReleaseDate\": \"sdo:datePublished\",\r\n    \"publications\": \"sdo:citation\",\r\n    \"people\": \"sdo:creator\",\r\n    \"studies\": \"sdo:hasPart\",\r\n    \"ontologySourceReferences\": \"sdo:mentions\",\r\n    \"comments\": \"sdo:disambiguatingDescription\",\r\n\r\n    \"publications?\": \"sdo:SubjectOf?\",\r\n    \"filename\": \"sdo:alternateName\"\r\n  }\r\n}\r\n    ";

