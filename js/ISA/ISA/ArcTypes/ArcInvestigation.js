import { map as map_2, defaultArg, unwrap } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { toList, removeAt, tryFindIndex, map } from "../../../fable_modules/fable-library.4.1.4/Seq.js";
import { ArcStudy } from "./ArcStudy.js";
import { tryFind, removeInPlace } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { disposeSafe, getEnumerator, defaultOf, safeHash, equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { ArcAssay } from "./ArcAssay.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { toArray, ofArray, map as map_1, empty } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { Investigation_create_4AD66BBE } from "../JsonTypes/Investigation.js";
import { createMissingIdentifier, isMissingIdentifier } from "./Identifier.js";
import { class_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";

export class ArcInvestigation {
    constructor(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, studies, comments, remarks) {
        const ontologySourceReferences_1 = defaultArg(ontologySourceReferences, []);
        const publications_1 = defaultArg(publications, []);
        const contacts_1 = defaultArg(contacts, []);
        const studies_1 = defaultArg(studies, []);
        const comments_1 = defaultArg(comments, []);
        const remarks_1 = defaultArg(remarks, []);
        this["identifier@25"] = identifier;
        this["Title@"] = title;
        this["Description@"] = description;
        this["SubmissionDate@"] = submissionDate;
        this["PublicReleaseDate@"] = publicReleaseDate;
        this["OntologySourceReferences@"] = ontologySourceReferences_1;
        this["Publications@"] = publications_1;
        this["Contacts@"] = contacts_1;
        this["Studies@"] = studies_1;
        this["Comments@"] = comments_1;
        this["Remarks@"] = remarks_1;
    }
    get Identifier() {
        const this$ = this;
        return this$["identifier@25"];
    }
    set Identifier(i) {
        const this$ = this;
        this$["identifier@25"] = i;
    }
    get Title() {
        const __ = this;
        return unwrap(__["Title@"]);
    }
    set Title(v) {
        const __ = this;
        __["Title@"] = v;
    }
    get Description() {
        const __ = this;
        return unwrap(__["Description@"]);
    }
    set Description(v) {
        const __ = this;
        __["Description@"] = v;
    }
    get SubmissionDate() {
        const __ = this;
        return unwrap(__["SubmissionDate@"]);
    }
    set SubmissionDate(v) {
        const __ = this;
        __["SubmissionDate@"] = v;
    }
    get PublicReleaseDate() {
        const __ = this;
        return unwrap(__["PublicReleaseDate@"]);
    }
    set PublicReleaseDate(v) {
        const __ = this;
        __["PublicReleaseDate@"] = v;
    }
    get OntologySourceReferences() {
        const __ = this;
        return __["OntologySourceReferences@"];
    }
    set OntologySourceReferences(v) {
        const __ = this;
        __["OntologySourceReferences@"] = v;
    }
    get Publications() {
        const __ = this;
        return __["Publications@"];
    }
    set Publications(v) {
        const __ = this;
        __["Publications@"] = v;
    }
    get Contacts() {
        const __ = this;
        return __["Contacts@"];
    }
    set Contacts(v) {
        const __ = this;
        __["Contacts@"] = v;
    }
    get Studies() {
        const __ = this;
        return __["Studies@"];
    }
    set Studies(v) {
        const __ = this;
        __["Studies@"] = v;
    }
    get Comments() {
        const __ = this;
        return __["Comments@"];
    }
    set Comments(v) {
        const __ = this;
        __["Comments@"] = v;
    }
    get Remarks() {
        const __ = this;
        return __["Remarks@"];
    }
    set Remarks(v) {
        const __ = this;
        __["Remarks@"] = v;
    }
    static get FileName() {
        return "isa.investigation.xlsx";
    }
    static init(identifier) {
        return new ArcInvestigation(identifier);
    }
    static create(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, studies, comments, remarks) {
        return new ArcInvestigation(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), unwrap(ontologySourceReferences), unwrap(publications), unwrap(contacts), unwrap(studies), unwrap(comments), unwrap(remarks));
    }
    static make(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, studies, comments, remarks) {
        return new ArcInvestigation(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), ontologySourceReferences, publications, contacts, studies, comments, remarks);
    }
    get StudyCount() {
        const this$ = this;
        return this$.Studies.length | 0;
    }
    get StudyIdentifiers() {
        const this$ = this;
        return map((x) => x.Identifier, this$.Studies);
    }
    AddStudy(study) {
        const this$ = this;
        const study_1 = study;
        const matchValue = tryFindIndex((x) => (x.Identifier === study_1.Identifier), this$.Studies);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${matchValue} has the same name.`);
        }
        void (this$.Studies.push(study));
    }
    static addStudy(study) {
        return (inv) => {
            const copy = inv.Copy();
            copy.AddStudy(study);
            return copy;
        };
    }
    InitStudy(studyName) {
        const this$ = this;
        const study = ArcStudy.init(studyName);
        this$.AddStudy(study);
        return study;
    }
    static initStudy(studyName) {
        return (inv) => {
            const copy = inv.Copy();
            return copy.InitStudy(studyName);
        };
    }
    RemoveStudyAt(index) {
        const this$ = this;
        this$.Studies.splice(index, 1);
    }
    static removeStudyAt(index) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.RemoveStudyAt(index);
            return newInv;
        };
    }
    RemoveStudy(studyIdentifier) {
        const this$ = this;
        removeInPlace(this$.GetStudy(studyIdentifier), this$.Studies, {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    static removeStudy(studyIdentifier) {
        return (inv) => {
            const copy = inv.Copy();
            copy.RemoveStudy(studyIdentifier);
            return copy;
        };
    }
    SetStudyAt(index, study) {
        const this$ = this;
        const study_1 = study;
        const matchValue = tryFindIndex((x) => (x.Identifier === study_1.Identifier), removeAt(index, this$.Studies));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${matchValue} has the same name.`);
        }
        this$.Studies[index] = study;
    }
    static setStudyAt(index, study) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.SetStudyAt(index, study);
            return newInv;
        };
    }
    SetStudy(studyIdentifier, study) {
        const this$ = this;
        const index = this$.GetStudyIndex(studyIdentifier) | 0;
        const study_1 = study;
        const matchValue = tryFindIndex((x) => (x.Identifier === study_1.Identifier), removeAt(index, this$.Studies));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${matchValue} has the same name.`);
        }
        this$.Studies[index] = study;
    }
    static setStudy(studyIdentifier, study) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.SetStudy(studyIdentifier, study);
            return newInv;
        };
    }
    GetStudyIndex(studyIdentifier) {
        const this$ = this;
        const index = this$.Studies.findIndex((s) => (s.Identifier === studyIdentifier)) | 0;
        if (index === -1) {
            throw new Error(`Unable to find study with specified identifier '${studyIdentifier}'!`);
        }
        return index | 0;
    }
    static getStudyIndex(studyIdentifier) {
        return (inv) => inv.GetStudyIndex(studyIdentifier);
    }
    GetStudyAt(index) {
        const this$ = this;
        return this$.Studies[index];
    }
    static getStudyAt(index) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.GetStudyAt(index);
        };
    }
    GetStudy(studyIdentifier) {
        const this$ = this;
        return defaultArg(tryFind((s) => (s.Identifier === studyIdentifier), this$.Studies), defaultOf());
    }
    static getStudy(studyIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.GetStudy(studyIdentifier);
        };
    }
    AddAssay(studyIdentifier, assay) {
        const this$ = this;
        const study = this$.GetStudy(studyIdentifier);
        const assay_1 = assay;
        const matchValue = tryFindIndex((x) => (x.Identifier === assay_1.Identifier), study.Assays);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${matchValue} has the same name.`);
        }
        study.AddAssay(assay);
    }
    static addAssay(studyIdentifier, assay) {
        return (inv) => {
            const copy = inv.Copy();
            copy.AddAssay(studyIdentifier, assay);
            return copy;
        };
    }
    AddAssayAt(studyIndex, assay) {
        const this$ = this;
        const study = this$.GetStudyAt(studyIndex);
        const assay_1 = assay;
        const matchValue = tryFindIndex((x) => (x.Identifier === assay_1.Identifier), study.Assays);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${matchValue} has the same name.`);
        }
        study.AddAssay(assay);
    }
    static addAssayAt(studyIndex, assay) {
        return (inv) => {
            const copy = inv.Copy();
            copy.AddAssayAt(studyIndex, assay);
            return copy;
        };
    }
    InitAssay(studyIdentifier, assayName) {
        const this$ = this;
        const assay = ArcAssay.init(assayName);
        this$.AddAssay(studyIdentifier, assay);
        return assay;
    }
    static initAssay(studyIdentifier, assayName) {
        return (inv) => {
            const copy = inv.Copy();
            return copy.InitAssay(studyIdentifier, assayName);
        };
    }
    RemoveAssayAt(studyIdentifier, index) {
        const this$ = this;
        const study = this$.GetStudy(studyIdentifier);
        study.Assays.splice(index, 1);
    }
    static removeAssayAt(studyIdentifier, index) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.RemoveAssayAt(studyIdentifier, index);
            return newInv;
        };
    }
    SetAssayAt(studyIdentifier, index, assay) {
        const this$ = this;
        const study = this$.GetStudy(studyIdentifier);
        study.SetAssayAt(index, assay);
        this$.Studies[index] = study;
    }
    static setAssayAt(studyIdentifier, index, assay) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.SetAssayAt(studyIdentifier, index, assay);
            return newInv;
        };
    }
    SetAssay(studyIdentifier, assayIdentifier, assay) {
        const this$ = this;
        const study = this$.GetStudy(studyIdentifier);
        const index = study.GetAssayIndex(assayIdentifier) | 0;
        study.SetAssayAt(index, assay);
    }
    static setAssay(studyIdentifier, assayIdentifier, assay) {
        return (inv) => {
            const newInv = inv.Copy();
            newInv.SetAssay(studyIdentifier, assayIdentifier, assay);
        };
    }
    GetAssayAt(studyIdentifier, index) {
        const this$ = this;
        const study = this$.GetStudy(studyIdentifier);
        return study.GetAssayAt(index);
    }
    static getAssayAt(studyIdentifier, index) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.GetAssayAt(studyIdentifier, index);
        };
    }
    GetAssay(studyIdentifier, assayIdentifier) {
        const this$ = this;
        const study = this$.GetStudy(studyIdentifier);
        const index = study.GetAssayIndex(assayIdentifier) | 0;
        return study.GetAssayAt(index);
    }
    static getAssay(studyIdentifier, assayIdentifier) {
        return (inv) => {
            const newInv = inv.Copy();
            return newInv.GetAssay(studyIdentifier, assayIdentifier);
        };
    }
    Copy() {
        const this$ = this;
        const newStudies = [];
        let enumerator = getEnumerator(this$.Studies);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const study = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy = study.Copy();
                void (newStudies.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        return new ArcInvestigation(this$.Identifier, unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.SubmissionDate), unwrap(this$.PublicReleaseDate), this$.OntologySourceReferences, this$.Publications, this$.Contacts, newStudies, this$.Comments, this$.Remarks);
    }
    ToInvestigation() {
        const this$ = this;
        const studies = fromValueWithDefault(empty(), map_1((a) => a.ToStudy(), toList(this$.Studies)));
        return Investigation_create_4AD66BBE(void 0, "isa.investigation.xlsx", isMissingIdentifier(this$.Identifier) ? void 0 : this$.Identifier, this$.Title, this$.Description, this$.SubmissionDate, this$.PublicReleaseDate, void 0, fromValueWithDefault(empty(), ofArray(this$.Publications)), fromValueWithDefault(empty(), ofArray(this$.Contacts)), studies, fromValueWithDefault(empty(), ofArray(this$.Comments)));
    }
    static fromInvestigation(i) {
        let identifer;
        const matchValue = i.Identifier;
        identifer = ((matchValue == null) ? createMissingIdentifier() : matchValue);
        const studies = map_2((arg_2) => {
            const arg_1 = map_1((arg) => ArcStudy.fromStudy(arg), arg_2);
            return Array.from(arg_1);
        }, i.Studies);
        return ArcInvestigation.create(identifer, i.Title, i.Description, i.SubmissionDate, i.PublicReleaseDate, void 0, map_2(toArray, i.Publications), map_2(toArray, i.Contacts), studies, map_2(toArray, i.Comments));
    }
}

export function ArcInvestigation_$reflection() {
    return class_type("ARCtrl.ISA.ArcInvestigation", void 0, ArcInvestigation);
}

export function ArcInvestigation_$ctor_12187E3F(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, studies, comments, remarks) {
    return new ArcInvestigation(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, studies, comments, remarks);
}

