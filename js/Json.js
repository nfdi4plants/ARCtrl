import { ARCtrl_OntologyAnnotation__OntologyAnnotation_toROCrateJsonString_Static_71136F3F, ARCtrl_OntologyAnnotation__OntologyAnnotation_toISAJsonString_Static_71136F3F, ARCtrl_OntologyAnnotation__OntologyAnnotation_toJsonString_Static_71136F3F, ARCtrl_OntologyAnnotation__OntologyAnnotation_fromROCrateJsonString_Static_Z721C83C5, ARCtrl_OntologyAnnotation__OntologyAnnotation_fromISAJsonString_Static_Z721C83C5, ARCtrl_OntologyAnnotation__OntologyAnnotation_fromJsonString_Static_Z721C83C5 } from "./JsonIO/OntologyAnnotation.js";
import { unwrap } from "./fable_modules/fable-library-js.4.22.0/Option.js";
import { class_type } from "./fable_modules/fable-library-js.4.22.0/Reflection.js";
import { ARCtrl_ArcAssay__ArcAssay_toROCrateJsonString_Static_5CABCA47, ARCtrl_ArcAssay__ArcAssay_toISAJsonString_Static_Z3B036AA, ARCtrl_ArcAssay__ArcAssay_toCompressedJsonString_Static_71136F3F, ARCtrl_ArcAssay__ArcAssay_toJsonString_Static_71136F3F, ARCtrl_ArcAssay__ArcAssay_fromROCrateJsonString_Static_Z721C83C5, ARCtrl_ArcAssay__ArcAssay_fromISAJsonString_Static_Z721C83C5, ARCtrl_ArcAssay__ArcAssay_fromCompressedJsonString_Static_Z721C83C5, ARCtrl_ArcAssay__ArcAssay_fromJsonString_Static_Z721C83C5 } from "./JsonIO/Assay.js";
import { ARCtrl_ArcStudy__ArcStudy_toROCrateJsonString_Static_3BA23086, ARCtrl_ArcStudy__ArcStudy_toISAJsonString_Static_Z3FD920F1, ARCtrl_ArcStudy__ArcStudy_toCompressedJsonString_Static_71136F3F, ARCtrl_ArcStudy__ArcStudy_toJsonString_Static_71136F3F, ARCtrl_ArcStudy__ArcStudy_fromROCrateJsonString_Static_Z721C83C5, ARCtrl_ArcStudy__ArcStudy_fromISAJsonString_Static_Z721C83C5, ARCtrl_ArcStudy__ArcStudy_fromCompressedJsonString_Static_Z721C83C5, ARCtrl_ArcStudy__ArcStudy_fromJsonString_Static_Z721C83C5 } from "./JsonIO/Study.js";
import { ARCtrl_ArcInvestigation__ArcInvestigation_toROCrateJsonString_Static_71136F3F, ARCtrl_ArcInvestigation__ArcInvestigation_toISAJsonString_Static_Z3B036AA, ARCtrl_ArcInvestigation__ArcInvestigation_toCompressedJsonString_Static_71136F3F, ARCtrl_ArcInvestigation__ArcInvestigation_toJsonString_Static_71136F3F, ARCtrl_ArcInvestigation__ArcInvestigation_fromROCrateJsonString_Static_Z721C83C5, ARCtrl_ArcInvestigation__ArcInvestigation_fromISAJsonString_Static_Z721C83C5, ARCtrl_ArcInvestigation__ArcInvestigation_fromCompressedJsonString_Static_Z721C83C5, ARCtrl_ArcInvestigation__ArcInvestigation_fromJsonString_Static_Z721C83C5 } from "./JsonIO/Investigation.js";
import { ARC } from "./ARC.js";

export class JsonHelper_OntologyAnnotationJson {
    constructor() {
    }
    fromJsonString(s) {
        return ARCtrl_OntologyAnnotation__OntologyAnnotation_fromJsonString_Static_Z721C83C5(s);
    }
    fromISAJsonString(s) {
        return ARCtrl_OntologyAnnotation__OntologyAnnotation_fromISAJsonString_Static_Z721C83C5(s);
    }
    fromROCrateJsonString(s) {
        return ARCtrl_OntologyAnnotation__OntologyAnnotation_fromROCrateJsonString_Static_Z721C83C5(s);
    }
    toJsonString(oa, spaces) {
        return ARCtrl_OntologyAnnotation__OntologyAnnotation_toJsonString_Static_71136F3F(unwrap(spaces))(oa);
    }
    toISAJsonString(oa, spaces) {
        return ARCtrl_OntologyAnnotation__OntologyAnnotation_toISAJsonString_Static_71136F3F(unwrap(spaces))(oa);
    }
    toROCrateJsonString(oa, spaces) {
        return ARCtrl_OntologyAnnotation__OntologyAnnotation_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(oa);
    }
}

export function JsonHelper_OntologyAnnotationJson_$reflection() {
    return class_type("ARCtrl.JsonHelper.OntologyAnnotationJson", undefined, JsonHelper_OntologyAnnotationJson);
}

export function JsonHelper_OntologyAnnotationJson_$ctor() {
    return new JsonHelper_OntologyAnnotationJson();
}

export class JsonHelper_AssayJson {
    constructor() {
    }
    fromJsonString(s) {
        return ARCtrl_ArcAssay__ArcAssay_fromJsonString_Static_Z721C83C5(s);
    }
    fromCompressedJsonString(s) {
        return ARCtrl_ArcAssay__ArcAssay_fromCompressedJsonString_Static_Z721C83C5(s);
    }
    fromISAJsonString(s) {
        return ARCtrl_ArcAssay__ArcAssay_fromISAJsonString_Static_Z721C83C5(s);
    }
    fromROCrateJsonString(s) {
        return ARCtrl_ArcAssay__ArcAssay_fromROCrateJsonString_Static_Z721C83C5(s);
    }
    toJsonString(assay, spaces) {
        return ARCtrl_ArcAssay__ArcAssay_toJsonString_Static_71136F3F(unwrap(spaces))(assay);
    }
    toCompressedJsonString(assay, spaces) {
        return ARCtrl_ArcAssay__ArcAssay_toCompressedJsonString_Static_71136F3F(unwrap(spaces))(assay);
    }
    toISAJsonString(assay, spaces, useIDReferencing) {
        return ARCtrl_ArcAssay__ArcAssay_toISAJsonString_Static_Z3B036AA(unwrap(spaces), unwrap(useIDReferencing))(assay);
    }
    toROCrateJsonString(assay, studyName, spaces) {
        return ARCtrl_ArcAssay__ArcAssay_toROCrateJsonString_Static_5CABCA47(studyName, unwrap(spaces))(assay);
    }
}

export function JsonHelper_AssayJson_$reflection() {
    return class_type("ARCtrl.JsonHelper.AssayJson", undefined, JsonHelper_AssayJson);
}

export function JsonHelper_AssayJson_$ctor() {
    return new JsonHelper_AssayJson();
}

export class JsonHelper_StudyJson {
    constructor() {
    }
    fromJsonString(s) {
        return ARCtrl_ArcStudy__ArcStudy_fromJsonString_Static_Z721C83C5(s);
    }
    fromCompressedJsonString(s) {
        return ARCtrl_ArcStudy__ArcStudy_fromCompressedJsonString_Static_Z721C83C5(s);
    }
    fromISAJsonString(s) {
        return ARCtrl_ArcStudy__ArcStudy_fromISAJsonString_Static_Z721C83C5(s);
    }
    fromROCrateJsonString(s) {
        return ARCtrl_ArcStudy__ArcStudy_fromROCrateJsonString_Static_Z721C83C5(s);
    }
    toJsonString(study, spaces) {
        return ARCtrl_ArcStudy__ArcStudy_toJsonString_Static_71136F3F(unwrap(spaces))(study);
    }
    toCompressedJsonString(study, spaces) {
        return ARCtrl_ArcStudy__ArcStudy_toCompressedJsonString_Static_71136F3F(unwrap(spaces))(study);
    }
    toISAJsonString(study, assays, spaces, useIDReferencing) {
        return ARCtrl_ArcStudy__ArcStudy_toISAJsonString_Static_Z3FD920F1(unwrap(assays), unwrap(spaces), unwrap(useIDReferencing))(study);
    }
    toROCrateJsonString(study, assays, spaces) {
        return ARCtrl_ArcStudy__ArcStudy_toROCrateJsonString_Static_3BA23086(unwrap(assays), unwrap(spaces))(study);
    }
}

export function JsonHelper_StudyJson_$reflection() {
    return class_type("ARCtrl.JsonHelper.StudyJson", undefined, JsonHelper_StudyJson);
}

export function JsonHelper_StudyJson_$ctor() {
    return new JsonHelper_StudyJson();
}

export class JsonHelper_InvestigationJson {
    constructor() {
    }
    fromJsonString(s) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_fromJsonString_Static_Z721C83C5(s);
    }
    fromCompressedJsonString(s) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_fromCompressedJsonString_Static_Z721C83C5(s);
    }
    fromISAJsonString(s) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_fromISAJsonString_Static_Z721C83C5(s);
    }
    fromROCrateJsonString(s) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_fromROCrateJsonString_Static_Z721C83C5(s);
    }
    toJsonString(investigation, spaces) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_toJsonString_Static_71136F3F(unwrap(spaces))(investigation);
    }
    toCompressedJsonString(investigation, spaces) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_toCompressedJsonString_Static_71136F3F(unwrap(spaces))(investigation);
    }
    toISAJsonString(investigation, spaces, useIDReferencing) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_toISAJsonString_Static_Z3B036AA(unwrap(spaces), unwrap(useIDReferencing))(investigation);
    }
    toROCrateJsonString(investigation, spaces) {
        return ARCtrl_ArcInvestigation__ArcInvestigation_toROCrateJsonString_Static_71136F3F(unwrap(spaces))(investigation);
    }
}

export function JsonHelper_InvestigationJson_$reflection() {
    return class_type("ARCtrl.JsonHelper.InvestigationJson", undefined, JsonHelper_InvestigationJson);
}

export function JsonHelper_InvestigationJson_$ctor() {
    return new JsonHelper_InvestigationJson();
}

export class JsonHelper_ARCJson {
    constructor() {
    }
    fromROCrateJsonString(s) {
        return ARC.fromROCrateJsonString(s);
    }
    toROCrateJsonString(spaces) {
        return ARC.toROCrateJsonString(unwrap(spaces));
    }
}

export function JsonHelper_ARCJson_$reflection() {
    return class_type("ARCtrl.JsonHelper.ARCJson", undefined, JsonHelper_ARCJson);
}

export function JsonHelper_ARCJson_$ctor() {
    return new JsonHelper_ARCJson();
}

export class JsonController {
    constructor() {
    }
    static get OntologyAnnotation() {
        return new JsonHelper_OntologyAnnotationJson();
    }
    static get Assay() {
        return new JsonHelper_AssayJson();
    }
    static get Study() {
        return new JsonHelper_StudyJson();
    }
    static get Investigation() {
        return new JsonHelper_InvestigationJson();
    }
    static get ARC() {
        return new JsonHelper_ARCJson();
    }
}

export function JsonController_$reflection() {
    return class_type("ARCtrl.JsonController", undefined, JsonController);
}

