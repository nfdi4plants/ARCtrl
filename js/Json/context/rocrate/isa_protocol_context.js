import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Protocol, ArcProtocol, name, protocolType, description, version, components, parameters, uri, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Protocol = Protocol;
        this.ArcProtocol = ArcProtocol;
        this.name = name;
        this.protocolType = protocolType;
        this.description = description;
        this.version = version;
        this.components = components;
        this.parameters = parameters;
        this.uri = uri;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Protocol.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Protocol", string_type], ["ArcProtocol", string_type], ["name", string_type], ["protocolType", string_type], ["description", string_type], ["version", string_type], ["components", string_type], ["parameters", string_type], ["uri", string_type], ["comments", string_type]]);
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
    }], ["Protocol", {
        Encode(helpers_2) {
            return helpers_2.encodeString("bio:LabProtocol");
        },
    }], ["name", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["protocolType", {
        Encode(helpers_4) {
            return helpers_4.encodeString("bio:intendedUse");
        },
    }], ["description", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:description");
        },
    }], ["version", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:version");
        },
    }], ["components", {
        Encode(helpers_7) {
            return helpers_7.encodeString("bio:labEquipment");
        },
    }], ["reagents", {
        Encode(helpers_8) {
            return helpers_8.encodeString("bio:reagent");
        },
    }], ["computationalTools", {
        Encode(helpers_9) {
            return helpers_9.encodeString("bio:computationalTool");
        },
    }], ["uri", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:url");
        },
    }], ["comments", {
        Encode(helpers_11) {
            return helpers_11.encodeString("sdo:comment");
        },
    }]];
    return {
        Encode(helpers_12) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_12)], values);
            return helpers_12.encodeObject(arg);
        },
    };
})();

