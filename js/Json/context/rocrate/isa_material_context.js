import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Material, ArcMaterial, type, name, characteristics, derivesFrom) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Material = Material;
        this.ArcMaterial = ArcMaterial;
        this.type = type;
        this.name = name;
        this.characteristics = characteristics;
        this.derivesFrom = derivesFrom;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Material.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Material", string_type], ["ArcMaterial", string_type], ["type", string_type], ["name", string_type], ["characteristics", string_type], ["derivesFrom", string_type]]);
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
    }], ["Material", {
        Encode(helpers_2) {
            return helpers_2.encodeString("bio:Sample");
        },
    }], ["type", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:disambiguatingDescription");
        },
    }], ["name", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:name");
        },
    }], ["characteristics", {
        Encode(helpers_5) {
            return helpers_5.encodeString("bio:additionalProperty");
        },
    }]];
    return {
        Encode(helpers_6) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_6)], values);
            return helpers_6.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"ArcMaterial\": \"arc:ARC#ARC_00000108\",\r\n    \"Material\": \"sdo:Thing\",\r\n\r\n    \"type\": \"arc:ARC#ARC_00000085\",\r\n    \"name\": \"arc:ARC#ARC_00000019\",\r\n    \"characteristics\": \"arc:ARC#ARC_00000080\",\r\n    \"derivesFrom\": \"arc:ARC#ARC_00000082\"\r\n  }\r\n}\r\n    ";

