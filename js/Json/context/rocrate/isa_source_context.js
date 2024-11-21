import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Source, ArcSource, identifier, characteristics, name) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Source = Source;
        this.ArcSource = ArcSource;
        this.identifier = identifier;
        this.characteristics = characteristics;
        this.name = name;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Source.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Source", string_type], ["ArcSource", string_type], ["identifier", string_type], ["characteristics", string_type], ["name", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["bio", {
        Encode(helpers_1) {
            return helpers_1.encodeString("https://bioschemas.org/");
        },
    }], ["Source", {
        Encode(helpers_2) {
            return helpers_2.encodeString("bio:Sample");
        },
    }], ["name", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["characteristics", {
        Encode(helpers_4) {
            return helpers_4.encodeString("bio:additionalProperty");
        },
    }]];
    return {
        Encode(helpers_5) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"Source\": \"sdo:Thing\",\r\n    \"ArcSource\": \"arc:ARC#ARC_00000071\",\r\n\r\n    \"identifier\": \"sdo:identifier\",\r\n\r\n    \"name\": \"arc:ARC#ARC_00000019\",\r\n    \"characteristics\": \"arc:ARC#ARC_00000080\"\r\n  }\r\n}\r\n    ";

