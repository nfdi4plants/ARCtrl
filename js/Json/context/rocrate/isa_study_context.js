import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Study, ArcStudy, identifier, title, description, submissionDate, publicReleaseDate, publications, people, assays, filename, comments, protocols, materials, otherMaterials, sources, samples, processSequence, factors, characteristicCategories, unitCategories, studyDesignDescriptors) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Study = Study;
        this.ArcStudy = ArcStudy;
        this.identifier = identifier;
        this.title = title;
        this.description = description;
        this.submissionDate = submissionDate;
        this.publicReleaseDate = publicReleaseDate;
        this.publications = publications;
        this.people = people;
        this.assays = assays;
        this.filename = filename;
        this.comments = comments;
        this.protocols = protocols;
        this.materials = materials;
        this.otherMaterials = otherMaterials;
        this.sources = sources;
        this.samples = samples;
        this.processSequence = processSequence;
        this.factors = factors;
        this.characteristicCategories = characteristicCategories;
        this.unitCategories = unitCategories;
        this.studyDesignDescriptors = studyDesignDescriptors;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Study.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Study", string_type], ["ArcStudy", string_type], ["identifier", string_type], ["title", string_type], ["description", string_type], ["submissionDate", string_type], ["publicReleaseDate", string_type], ["publications", string_type], ["people", string_type], ["assays", string_type], ["filename", string_type], ["comments", string_type], ["protocols", string_type], ["materials", string_type], ["otherMaterials", string_type], ["sources", string_type], ["samples", string_type], ["processSequence", string_type], ["factors", string_type], ["characteristicCategories", string_type], ["unitCategories", string_type], ["studyDesignDescriptors", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Study", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:Dataset");
        },
    }], ["identifier", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:identifier");
        },
    }], ["title", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:headline");
        },
    }], ["additionalType", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:additionalType");
        },
    }], ["description", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:description");
        },
    }], ["submissionDate", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:dateCreated");
        },
    }], ["publicReleaseDate", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:datePublished");
        },
    }], ["publications", {
        Encode(helpers_8) {
            return helpers_8.encodeString("sdo:citation");
        },
    }], ["people", {
        Encode(helpers_9) {
            return helpers_9.encodeString("sdo:creator");
        },
    }], ["assays", {
        Encode(helpers_10) {
            return helpers_10.encodeString("sdo:hasPart");
        },
    }], ["filename", {
        Encode(helpers_11) {
            return helpers_11.encodeString("sdo:alternateName");
        },
    }], ["comments", {
        Encode(helpers_12) {
            return helpers_12.encodeString("sdo:comment");
        },
    }], ["processSequence", {
        Encode(helpers_13) {
            return helpers_13.encodeString("sdo:about");
        },
    }], ["studyDesignDescriptors", {
        Encode(helpers_14) {
            return helpers_14.encodeString("arc:ARC#ARC_00000037");
        },
    }]];
    return {
        Encode(helpers_15) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_15)], values);
            return helpers_15.encodeObject(arg);
        },
    };
})();

