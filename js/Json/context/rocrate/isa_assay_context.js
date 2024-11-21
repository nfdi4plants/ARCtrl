import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Assay, ArcAssay, measurementType, technologyType, technologyPlatform, dataFiles, materials, otherMaterials, samples, characteristicCategories, processSequence, unitCategories, comments, filename) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Assay = Assay;
        this.ArcAssay = ArcAssay;
        this.measurementType = measurementType;
        this.technologyType = technologyType;
        this.technologyPlatform = technologyPlatform;
        this.dataFiles = dataFiles;
        this.materials = materials;
        this.otherMaterials = otherMaterials;
        this.samples = samples;
        this.characteristicCategories = characteristicCategories;
        this.processSequence = processSequence;
        this.unitCategories = unitCategories;
        this.comments = comments;
        this.filename = filename;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Assay.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Assay", string_type], ["ArcAssay", string_type], ["measurementType", string_type], ["technologyType", string_type], ["technologyPlatform", string_type], ["dataFiles", string_type], ["materials", string_type], ["otherMaterials", string_type], ["samples", string_type], ["characteristicCategories", string_type], ["processSequence", string_type], ["unitCategories", string_type], ["comments", string_type], ["filename", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Assay", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Dataset");
        },
    }], ["identifier", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:identifier");
        },
    }], ["additionalType", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:additionalType");
        },
    }], ["measurementType", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:variableMeasured");
        },
    }], ["technologyType", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:measurementTechnique");
        },
    }], ["technologyPlatform", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:measurementMethod");
        },
    }], ["dataFiles", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:hasPart");
        },
    }], ["performers", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:creator");
        },
    }], ["processSequence", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:about");
        },
    }], ["comments", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:comment");
        },
    }], ["filename", {
        Encode(helpers_11) {
            return helpers_11.encodeString("sdo:url");
        },
    }]];
    return {
        Encode(helpers_12) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_12)], values);
            return helpers_12.encodeObject(arg);
        },
    };
})();

