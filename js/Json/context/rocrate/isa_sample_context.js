import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Sample, ArcSample, name, characteristics, factorValues, derivesFrom) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Sample = Sample;
        this.ArcSample = ArcSample;
        this.name = name;
        this.characteristics = characteristics;
        this.factorValues = factorValues;
        this.derivesFrom = derivesFrom;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Sample.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Sample", string_type], ["ArcSample", string_type], ["name", string_type], ["characteristics", string_type], ["factorValues", string_type], ["derivesFrom", string_type]]);
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
    }], ["Sample", {
        Encode(helpers_2) {
            return helpers_2.encodeString("bio:Sample");
        },
    }], ["name", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:name");
        },
    }], ["additionalProperties", {
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

