import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, ProtocolParamter, ArcProtocolParameter, parameterName) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.ProtocolParamter = ProtocolParamter;
        this.ArcProtocolParameter = ArcProtocolParameter;
        this.parameterName = parameterName;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.ProtocolParameter.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["ProtocolParamter", string_type], ["ArcProtocolParameter", string_type], ["parameterName", string_type]]);
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
    }], ["ProtocolParameter", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:Thing");
        },
    }], ["ArcProtocolParameter", {
        Encode(helpers_3) {
            return helpers_3.encodeString("arc:ARC#ARC_00000063");
        },
    }], ["parameterName", {
        Encode(helpers_4) {
            return helpers_4.encodeString("arc:ARC#ARC_00000100");
        },
    }]];
    return {
        Encode(helpers_5) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"ProtocolParameter\": \"sdo:Thing\",\r\n    \"ArcProtocolParameter\": \"arc:ARC#ARC_00000063\",\r\n\r\n    \"parameterName\": \"arc:ARC#ARC_00000100\"\r\n  }\r\n}\r\n    ";

