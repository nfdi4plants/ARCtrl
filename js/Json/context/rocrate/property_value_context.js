import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["additionalType", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:additionalType");
        },
    }], ["alternateName", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:alternateName");
        },
    }], ["measurementMethod", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:measurementMethod");
        },
    }], ["description", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:description");
        },
    }], ["category", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:name");
        },
    }], ["categoryCode", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:propertyID");
        },
    }], ["value", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:value");
        },
    }], ["valueCode", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:valueReference");
        },
    }], ["unit", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:unitText");
        },
    }], ["unitCode", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:unitCode");
        },
    }], ["comments", {
        Encode(helpers_11) {
            return helpers_11.encodeString("sdo:disambiguatingDescription");
        },
    }]];
    return {
        Encode(helpers_12) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_12)], values);
            return helpers_12.encodeObject(arg);
        },
    };
})();

