import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Data, ArcData, type, name, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Data = Data;
        this.ArcData = ArcData;
        this.type = type;
        this.name = name;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Data.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Data", string_type], ["ArcData", string_type], ["type", string_type], ["name", string_type], ["comments", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Data", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:MediaObject");
        },
    }], ["type", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:disambiguatingDescription");
        },
    }], ["encodingFormat", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:encodingFormat");
        },
    }], ["usageInfo", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:usageInfo");
        },
    }], ["name", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:name");
        },
    }], ["comments", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:comment");
        },
    }]];
    return {
        Encode(helpers_7) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_7)], values);
            return helpers_7.encodeObject(arg);
        },
    };
})();

export const context_str = "\r\n{\r\n  \"@context\": {\r\n    \"sdo\": \"http://schema.org/\",\r\n    \"arc\": \"http://purl.org/nfdi4plants/ontology/\",\r\n\r\n    \"Data\": \"sdo:MediaObject\",\r\n    \"ArcData\": \"arc:ARC#ARC_00000076\",\r\n\r\n    \"type\": \"arc:ARC#ARC_00000107\",\r\n\r\n    \"name\": \"sdo:name\",\r\n    \"comments\": \"sdo:disambiguatingDescription\"\r\n  }\r\n}\r\n    ";

