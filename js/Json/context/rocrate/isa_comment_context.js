import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Comment$, name, value) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Comment = Comment$;
        this.name = name;
        this.value = value;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Comment.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Comment", string_type], ["name", string_type], ["value", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Comment", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Comment");
        },
    }], ["name", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:name");
        },
    }], ["value", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:text");
        },
    }]];
    return {
        Encode(helpers_4) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_4)], values);
            return helpers_4.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \r\n    \"Comment\": \"sdo:Comment\",\r\n    \"name\": \"sdo:name\",\r\n    \"value\": \"sdo:text\"\r\n  }\r\n}\r\n    ";

