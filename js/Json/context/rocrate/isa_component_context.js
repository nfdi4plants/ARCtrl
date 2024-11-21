import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Component, ArcComponent, componentName, componentType) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Component = Component;
        this.ArcComponent = ArcComponent;
        this.componentName = componentName;
        this.componentType = componentType;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Component.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Component", string_type], ["ArcComponent", string_type], ["componentName", string_type], ["componentType", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Component", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:PropertyValue");
        },
    }], ["category", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:name");
        },
    }], ["categoryCode", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:propertyID");
        },
    }], ["value", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:value");
        },
    }], ["valueCode", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:valueReference");
        },
    }], ["unit", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:unitText");
        },
    }], ["unitCode", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:unitCode");
        },
    }]];
    return {
        Encode(helpers_8) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_8)], values);
            return helpers_8.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \r\n    \"Component\": \"sdo:PropertyValue\",\r\n\r\n    \"componentName\": \"sdo\",\r\n    \"componentType\": \"arc:ARC#ARC_00000102\"\r\n  }\r\n}\r\n    ";

