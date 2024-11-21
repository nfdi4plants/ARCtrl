import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, MaterialAttribute, ArcMaterialAttribute, characteristicType) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.MaterialAttribute = MaterialAttribute;
        this.ArcMaterialAttribute = ArcMaterialAttribute;
        this.characteristicType = characteristicType;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.MaterialAttribute.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["MaterialAttribute", string_type], ["ArcMaterialAttribute", string_type], ["characteristicType", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["arc", {
        Encode(helpers_1) {
            return helpers_1.encodeString("http://purl.org/nfdi4plants/ontology/");
        },
    }], ["MaterialAttribute", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:Property");
        },
    }], ["ArcMaterialAttribute", {
        Encode(helpers_3) {
            return helpers_3.encodeString("arc:ARC#ARC_00000050");
        },
    }], ["characteristicType", {
        Encode(helpers_4) {
            return helpers_4.encodeString("arc:ARC#ARC_00000098");
        },
    }]];
    return {
        Encode(helpers_5) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"MaterialAttribute\": \"sdo:Property\",\r\n    \"ArcMaterialAttribute\": \"arc:ARC#ARC_00000050\",\r\n\r\n    \"characteristicType\": \"arc:ARC#ARC_00000098\"\r\n  }\r\n}\r\n    ";

