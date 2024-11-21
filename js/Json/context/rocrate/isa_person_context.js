import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Person, firstName, lastName, midInitials, email, address, phone, fax, comments, roles, affiliation) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Person = Person;
        this.firstName = firstName;
        this.lastName = lastName;
        this.midInitials = midInitials;
        this.email = email;
        this.address = address;
        this.phone = phone;
        this.fax = fax;
        this.comments = comments;
        this.roles = roles;
        this.affiliation = affiliation;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Person.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Person", string_type], ["firstName", string_type], ["lastName", string_type], ["midInitials", string_type], ["email", string_type], ["address", string_type], ["phone", string_type], ["fax", string_type], ["comments", string_type], ["roles", string_type], ["affiliation", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Person", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Person");
        },
    }], ["orcid", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:identifier");
        },
    }], ["firstName", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:givenName");
        },
    }], ["lastName", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:familyName");
        },
    }], ["midInitials", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:additionalName");
        },
    }], ["email", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:email");
        },
    }], ["address", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:address");
        },
    }], ["phone", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:telephone");
        },
    }], ["fax", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:faxNumber");
        },
    }], ["comments", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:disambiguatingDescription");
        },
    }], ["roles", {
        Encode(helpers_11) {
            return helpers_11.encodeString("sdo:jobTitle");
        },
    }], ["affiliation", {
        Encode(helpers_12) {
            return helpers_12.encodeString("sdo:affiliation");
        },
    }]];
    return {
        Encode(helpers_13) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_13)], values);
            return helpers_13.encodeObject(arg);
        },
    };
})();

export const contextMinimal_jsonValue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Person", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Person");
        },
    }], ["name", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:name");
        },
    }]];
    return {
        Encode(helpers_3) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_3)], values);
            return helpers_3.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"Person\": \"sdo:Person\",\r\n    \"firstName\": \"sdo:givenName\",\r\n    \"lastName\": \"sdo:familyName\",\r\n    \"midInitials\": \"sdo:additionalName\",\r\n    \"email\": \"sdo:email\",\r\n    \"address\": \"sdo:address\",\r\n    \"phone\": \"sdo:telephone\",\r\n    \"fax\": \"sdo:faxNumber\",\r\n    \"comments\": \"sdo:disambiguatingDescription\",\r\n    \"roles\": \"sdo:jobTitle\",\r\n    \"affiliation\": \"sdo:affiliation\"\r\n  }\r\n}\r\n    ";

