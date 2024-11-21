import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, FactorValue, ArcFactorValue, category, value, unit) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.FactorValue = FactorValue;
        this.ArcFactorValue = ArcFactorValue;
        this.category = category;
        this.value = value;
        this.unit = unit;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.FactorValue.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["FactorValue", string_type], ["ArcFactorValue", string_type], ["category", string_type], ["value", string_type], ["unit", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["FactorValue", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:PropertyValue");
        },
    }], ["additionalType", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:additionalType");
        },
    }], ["category", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["categoryName", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:alternateName");
        },
    }], ["categoryCode", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:propertyID");
        },
    }], ["value", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:value");
        },
    }], ["valueCode", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:valueReference");
        },
    }], ["unit", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:unitText");
        },
    }], ["unitCode", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:unitCode");
        },
    }]];
    return {
        Encode(helpers_10) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_10)], values);
            return helpers_10.encodeObject(arg);
        },
    };
})();

