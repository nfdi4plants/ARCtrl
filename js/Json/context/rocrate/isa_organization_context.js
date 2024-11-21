import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Organization, name) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Organization = Organization;
        this.name = name;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Organization.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Organization", string_type], ["name", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Organization", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Organization");
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

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"Organization\": \"sdo:Organization\",\r\n    \r\n    \"name\": \"sdo:name\"\r\n  }\r\n}\r\n    ";

