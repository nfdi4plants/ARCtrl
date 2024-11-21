import { Record } from "../../../fable_modules/fable-library-js.4.22.0/Types.js";
import { record_type, string_type } from "../../../fable_modules/fable-library-js.4.22.0/Reflection.js";
import { map } from "../../../fable_modules/fable-library-js.4.22.0/Seq.js";

export class IContext extends Record {
    constructor(sdo, arc, Publication, pubMedID, doi, title, status, authorList, comments) {
        super();
        this.sdo = sdo;
        this.arc = arc;
        this.Publication = Publication;
        this.pubMedID = pubMedID;
        this.doi = doi;
        this.title = title;
        this.status = status;
        this.authorList = authorList;
        this.comments = comments;
    }
}

export function IContext_$reflection() {
    return record_type("ARCtrl.Json.ROCrateContext.Publication.IContext", [], IContext, () => [["sdo", string_type], ["arc", string_type], ["Publication", string_type], ["pubMedID", string_type], ["doi", string_type], ["title", string_type], ["status", string_type], ["authorList", string_type], ["comments", string_type]]);
}

export const context_jsonvalue = (() => {
    const values = [["sdo", {
        Encode(helpers) {
            return helpers.encodeString("http://schema.org/");
        },
    }], ["Publication", {
        Encode(helpers_1) {
            return helpers_1.encodeString("sdo:ScholarlyArticle");
        },
    }], ["pubMedID", {
        Encode(helpers_2) {
            return helpers_2.encodeString("sdo:url");
        },
    }], ["doi", {
        Encode(helpers_3) {
            return helpers_3.encodeString("sdo:sameAs");
        },
    }], ["title", {
        Encode(helpers_4) {
            return helpers_4.encodeString("sdo:headline");
        },
    }], ["status", {
        Encode(helpers_5) {
            return helpers_5.encodeString("sdo:creativeWorkStatus");
        },
    }], ["authorList", {
        Encode(helpers_6) {
            return helpers_6.encodeString("sdo:author");
        },
    }], ["comments", {
        Encode(helpers_7) {
            return helpers_7.encodeString("sdo:disambiguatingDescription");
        },
    }]];
    return {
        Encode(helpers_8) {
            const arg = map((tupledArg) => [tupledArg[0], tupledArg[1].Encode(helpers_8)], values);
            return helpers_8.encodeObject(arg);
        },
    };
})();

