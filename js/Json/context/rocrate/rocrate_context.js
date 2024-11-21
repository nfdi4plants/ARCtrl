import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, CreativeWork, about, conformsTo) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.CreativeWork = CreativeWork;
        this.about = about;
        this.conformsTo = conformsTo;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.ROCrate.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["CreativeWork", string_type], ["about", string_type], ["conformsTo", string_type]]);
}

export const conformsTo_jsonvalue = (() => {
    const values = [["@id", {
        Encode(helpers) {
            return helpers.encodeString("https://w3id.org/ro/crate/1.1");
        },
    }]];
    return {
        Encode(helpers_1) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_1)], values);
            return helpers_1.encodeObject(arg);
        },
    };
})();

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["arc", {
        Encode(helpers_1) {
            return helpers_1.encodeString("http://purl.org/nfdi4plants/ontology/");
        },
    }], ["CreativeWork", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:CreativeWork");
        },
    }], ["about", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:about");
        },
    }], ["conformsTo", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:conformsTo");
        },
    }]];
    return {
        Encode(helpers_5) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_5)], values);
            return helpers_5.encodeObject(arg);
        },
    };
})();

