import { map as map_2, defaultArg, value as value_1, Option, unwrap } from "../../../fable_modules/fable-library-ts/Option.js";
import { map as map_1, empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologySourceReference } from "../JsonTypes/OntologySourceReference.js";
import { Publication } from "../JsonTypes/Publication.js";
import { Person } from "../JsonTypes/Person.js";
import { ArcStudy } from "./ArcStudy.js";
import { Remark, Comment$ } from "../JsonTypes/Comment.js";
import { int32 } from "../../../fable_modules/fable-library-ts/Int32.js";
import { toList, removeAt, tryFindIndex, map } from "../../../fable_modules/fable-library-ts/Seq.js";
import { tryFind, removeInPlace } from "../../../fable_modules/fable-library-ts/Array.js";
import { disposeSafe, getEnumerator, defaultOf, safeHash, equals } from "../../../fable_modules/fable-library-ts/Util.js";
import { ArcAssay } from "./ArcAssay.js";
import { fromValueWithDefault } from "../OptionExtensions.js";
import { Study } from "../JsonTypes/Study.js";
import { Investigation, Investigation_create_ZB2B0942 } from "../JsonTypes/Investigation.js";
import { createMissingIdentifier, isMissingIdentifier } from "./Identifier.js";
import { class_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";

export class ArcInvestigation {
    "identifier@25": string;
    "Title@": Option<string>;
    "Description@": Option<string>;
    "SubmissionDate@": Option<string>;
    "PublicReleaseDate@": Option<string>;
    "OntologySourceReferences@": FSharpList<OntologySourceReference>;
    "Publications@": FSharpList<Publication>;
    "Contacts@": FSharpList<Person>;
    "Studies@": ArcStudy[];
    "Comments@": FSharpList<Comment$>;
    "Remarks@": FSharpList<Remark>;
    constructor(identifier: string, title?: string, description?: string, submissionDate?: string, publicReleaseDate?: string, ontologySourceReferences?: FSharpList<OntologySourceReference>, publications?: FSharpList<Publication>, contacts?: FSharpList<Person>, studies?: ArcStudy[], comments?: FSharpList<Comment$>, remarks?: FSharpList<Remark>) {
        const ontologySourceReferences_1: FSharpList<OntologySourceReference> = defaultArg<FSharpList<OntologySourceReference>>(ontologySourceReferences, empty<OntologySourceReference>());
        const publications_1: FSharpList<Publication> = defaultArg<FSharpList<Publication>>(publications, empty<Publication>());
        const contacts_1: FSharpList<Person> = defaultArg<FSharpList<Person>>(contacts, empty<Person>());
        const studies_1: ArcStudy[] = defaultArg<ArcStudy[]>(studies, []);
        const comments_1: FSharpList<Comment$> = defaultArg<FSharpList<Comment$>>(comments, empty<Comment$>());
        const remarks_1: FSharpList<Remark> = defaultArg<FSharpList<Remark>>(remarks, empty<Remark>());
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
    get Identifier(): string {
        const this$: ArcInvestigation = this;
        return this$["identifier@25"];
    }
    set Identifier(i: string) {
        const this$: ArcInvestigation = this;
        this$["identifier@25"] = i;
    }
    get Title(): string | undefined {
        const __: ArcInvestigation = this;
        return unwrap(__["Title@"]);
    }
    set Title(v: Option<string>) {
        const __: ArcInvestigation = this;
        __["Title@"] = v;
    }
    get Description(): string | undefined {
        const __: ArcInvestigation = this;
        return unwrap(__["Description@"]);
    }
    set Description(v: Option<string>) {
        const __: ArcInvestigation = this;
        __["Description@"] = v;
    }
    get SubmissionDate(): string | undefined {
        const __: ArcInvestigation = this;
        return unwrap(__["SubmissionDate@"]);
    }
    set SubmissionDate(v: Option<string>) {
        const __: ArcInvestigation = this;
        __["SubmissionDate@"] = v;
    }
    get PublicReleaseDate(): string | undefined {
        const __: ArcInvestigation = this;
        return unwrap(__["PublicReleaseDate@"]);
    }
    set PublicReleaseDate(v: Option<string>) {
        const __: ArcInvestigation = this;
        __["PublicReleaseDate@"] = v;
    }
    get OntologySourceReferences(): FSharpList<OntologySourceReference> {
        const __: ArcInvestigation = this;
        return __["OntologySourceReferences@"];
    }
    set OntologySourceReferences(v: FSharpList<OntologySourceReference>) {
        const __: ArcInvestigation = this;
        __["OntologySourceReferences@"] = v;
    }
    get Publications(): FSharpList<Publication> {
        const __: ArcInvestigation = this;
        return __["Publications@"];
    }
    set Publications(v: FSharpList<Publication>) {
        const __: ArcInvestigation = this;
        __["Publications@"] = v;
    }
    get Contacts(): FSharpList<Person> {
        const __: ArcInvestigation = this;
        return __["Contacts@"];
    }
    set Contacts(v: FSharpList<Person>) {
        const __: ArcInvestigation = this;
        __["Contacts@"] = v;
    }
    get Studies(): ArcStudy[] {
        const __: ArcInvestigation = this;
        return __["Studies@"];
    }
    set Studies(v: ArcStudy[]) {
        const __: ArcInvestigation = this;
        __["Studies@"] = v;
    }
    get Comments(): FSharpList<Comment$> {
        const __: ArcInvestigation = this;
        return __["Comments@"];
    }
    set Comments(v: FSharpList<Comment$>) {
        const __: ArcInvestigation = this;
        __["Comments@"] = v;
    }
    get Remarks(): FSharpList<Remark> {
        const __: ArcInvestigation = this;
        return __["Remarks@"];
    }
    set Remarks(v: FSharpList<Remark>) {
        const __: ArcInvestigation = this;
        __["Remarks@"] = v;
    }
    static get FileName(): string {
        return "isa.investigation.xlsx";
    }
    static init(identifier: string): ArcInvestigation {
        return new ArcInvestigation(identifier);
    }
    static create(identifier: string, title?: string, description?: string, submissionDate?: string, publicReleaseDate?: string, ontologySourceReferences?: FSharpList<OntologySourceReference>, publications?: FSharpList<Publication>, contacts?: FSharpList<Person>, studies?: ArcStudy[], comments?: FSharpList<Comment$>, remarks?: FSharpList<Remark>): ArcInvestigation {
        return new ArcInvestigation(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), unwrap(ontologySourceReferences), unwrap(publications), unwrap(contacts), unwrap(studies), unwrap(comments), unwrap(remarks));
    }
    static make(identifier: string, title: Option<string>, description: Option<string>, submissionDate: Option<string>, publicReleaseDate: Option<string>, ontologySourceReferences: FSharpList<OntologySourceReference>, publications: FSharpList<Publication>, contacts: FSharpList<Person>, studies: ArcStudy[], comments: FSharpList<Comment$>, remarks: FSharpList<Remark>): ArcInvestigation {
        return new ArcInvestigation(identifier, unwrap(title), unwrap(description), unwrap(submissionDate), unwrap(publicReleaseDate), ontologySourceReferences, publications, contacts, studies, comments, remarks);
    }
    get StudyCount(): int32 {
        const this$: ArcInvestigation = this;
        return this$.Studies.length | 0;
    }
    get StudyIdentifiers(): Iterable<string> {
        const this$: ArcInvestigation = this;
        return map<ArcStudy, string>((x: ArcStudy): string => x.Identifier, this$.Studies);
    }
    AddStudy(study: ArcStudy): void {
        const this$: ArcInvestigation = this;
        const study_1: ArcStudy = study;
        const matchValue: Option<int32> = tryFindIndex<ArcStudy>((x: ArcStudy): boolean => (x.Identifier === study_1.Identifier), this$.Studies);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${value_1(matchValue)} has the same name.`);
        }
        void (this$.Studies.push(study));
    }
    static addStudy(study: ArcStudy): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const copy: ArcInvestigation = inv.Copy();
            copy.AddStudy(study);
            return copy;
        };
    }
    InitStudy(studyName: string): void {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = ArcStudy.init(studyName);
        this$.AddStudy(study);
    }
    static initStudy(studyName: string): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const copy: ArcInvestigation = inv.Copy();
            copy.InitStudy(studyName);
            return copy;
        };
    }
    RemoveStudyAt(index: int32): void {
        const this$: ArcInvestigation = this;
        this$.Studies.splice(index, 1);
    }
    static removeStudyAt(index: int32): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const newInv: ArcInvestigation = inv.Copy();
            newInv.RemoveStudyAt(index);
            return newInv;
        };
    }
    RemoveStudy(studyIdentifier: string): void {
        const this$: ArcInvestigation = this;
        removeInPlace(this$.GetStudy(studyIdentifier), this$.Studies, {
            Equals: equals,
            GetHashCode: safeHash,
        });
    }
    static removeStudy(studyIdentifier: string): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const copy: ArcInvestigation = inv.Copy();
            copy.RemoveStudy(studyIdentifier);
            return copy;
        };
    }
    SetStudyAt(index: int32, study: ArcStudy): void {
        const this$: ArcInvestigation = this;
        const study_1: ArcStudy = study;
        const matchValue: Option<int32> = tryFindIndex<ArcStudy>((x: ArcStudy): boolean => (x.Identifier === study_1.Identifier), removeAt<ArcStudy>(index, this$.Studies));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${value_1(matchValue)} has the same name.`);
        }
        this$.Studies[index] = study;
    }
    static setStudyAt(index: int32, study: ArcStudy): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const newInv: ArcInvestigation = inv.Copy();
            newInv.SetStudyAt(index, study);
            return newInv;
        };
    }
    SetStudy(studyIdentifier: string, study: ArcStudy): void {
        const this$: ArcInvestigation = this;
        const index: int32 = this$.GetStudyIndex(studyIdentifier) | 0;
        const study_1: ArcStudy = study;
        const matchValue: Option<int32> = tryFindIndex<ArcStudy>((x: ArcStudy): boolean => (x.Identifier === study_1.Identifier), removeAt<ArcStudy>(index, this$.Studies));
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create study with name ${study_1.Identifier}, as study names must be unique and study at index ${value_1(matchValue)} has the same name.`);
        }
        this$.Studies[index] = study;
    }
    static setStudy(studyIdentifier: string, study: ArcStudy): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const newInv: ArcInvestigation = inv.Copy();
            newInv.SetStudy(studyIdentifier, study);
            return newInv;
        };
    }
    GetStudyIndex(studyIdentifier: string): int32 {
        const this$: ArcInvestigation = this;
        const index: int32 = this$.Studies.findIndex((s: ArcStudy): boolean => (s.Identifier === studyIdentifier)) | 0;
        if (index === -1) {
            throw new Error(`Unable to find study with specified identifier '${studyIdentifier}'!`);
        }
        return index | 0;
    }
    static getStudyIndex(studyIdentifier: string): ((arg0: ArcInvestigation) => int32) {
        return (inv: ArcInvestigation): int32 => inv.GetStudyIndex(studyIdentifier);
    }
    GetStudyAt(index: int32): ArcStudy {
        const this$: ArcInvestigation = this;
        return this$.Studies[index];
    }
    static getStudyAt(index: int32): ((arg0: ArcInvestigation) => ArcStudy) {
        return (inv: ArcInvestigation): ArcStudy => {
            const newInv: ArcInvestigation = inv.Copy();
            return newInv.GetStudyAt(index);
        };
    }
    GetStudy(studyIdentifier: string): ArcStudy {
        const this$: ArcInvestigation = this;
        return defaultArg(tryFind((s: ArcStudy): boolean => (s.Identifier === studyIdentifier), this$.Studies), defaultOf());
    }
    static getStudy(studyIdentifier: string): ((arg0: ArcInvestigation) => ArcStudy) {
        return (inv: ArcInvestigation): ArcStudy => {
            const newInv: ArcInvestigation = inv.Copy();
            return newInv.GetStudy(studyIdentifier);
        };
    }
    AddAssay(studyIdentifier: string, assay: ArcAssay): void {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudy(studyIdentifier);
        const assay_1: ArcAssay = assay;
        const matchValue: Option<int32> = tryFindIndex<ArcAssay>((x: ArcAssay): boolean => (x.Identifier === assay_1.Identifier), study.Assays);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${value_1(matchValue)} has the same name.`);
        }
        study.AddAssay(assay);
    }
    static addAssay(studyIdentifier: string, assay: ArcAssay): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const copy: ArcInvestigation = inv.Copy();
            copy.AddAssay(studyIdentifier, assay);
            return copy;
        };
    }
    AddAssayAt(studyIndex: int32, assay: ArcAssay): void {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudyAt(studyIndex);
        const assay_1: ArcAssay = assay;
        const matchValue: Option<int32> = tryFindIndex<ArcAssay>((x: ArcAssay): boolean => (x.Identifier === assay_1.Identifier), study.Assays);
        if (matchValue == null) {
        }
        else {
            throw new Error(`Cannot create assay with name ${assay_1.Identifier}, as assay names must be unique and assay at index ${value_1(matchValue)} has the same name.`);
        }
        study.AddAssay(assay);
    }
    static addAssayAt(studyIndex: int32, assay: ArcAssay): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const copy: ArcInvestigation = inv.Copy();
            copy.AddAssayAt(studyIndex, assay);
            return copy;
        };
    }
    InitAssay(studyIdentifier: string, assayName: string): void {
        const this$: ArcInvestigation = this;
        const assay: ArcAssay = ArcAssay.init(assayName);
        this$.AddAssay(studyIdentifier, assay);
    }
    static initAssay(studyIdentifier: string, assayName: string): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const copy: ArcInvestigation = inv.Copy();
            copy.InitAssay(studyIdentifier, assayName);
            return copy;
        };
    }
    RemoveAssayAt(studyIdentifier: string, index: int32): void {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudy(studyIdentifier);
        study.Assays.splice(index, 1);
    }
    static removeAssayAt(studyIdentifier: string, index: int32): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const newInv: ArcInvestigation = inv.Copy();
            newInv.RemoveAssayAt(studyIdentifier, index);
            return newInv;
        };
    }
    SetAssayAt(studyIdentifier: string, index: int32, assay: ArcAssay): void {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudy(studyIdentifier);
        study.SetAssayAt(index, assay);
        this$.Studies[index] = study;
    }
    static setAssayAt(studyIdentifier: string, index: int32, assay: ArcAssay): ((arg0: ArcInvestigation) => ArcInvestigation) {
        return (inv: ArcInvestigation): ArcInvestigation => {
            const newInv: ArcInvestigation = inv.Copy();
            newInv.SetAssayAt(studyIdentifier, index, assay);
            return newInv;
        };
    }
    SetAssay(studyIdentifier: string, assayIdentifier: string, assay: ArcAssay): void {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudy(studyIdentifier);
        const index: int32 = study.GetAssayIndex(assayIdentifier) | 0;
        study.SetAssayAt(index, assay);
    }
    static setAssay(studyIdentifier: string, assayIdentifier: string, assay: ArcAssay): ((arg0: ArcInvestigation) => void) {
        return (inv: ArcInvestigation): void => {
            const newInv: ArcInvestigation = inv.Copy();
            newInv.SetAssay(studyIdentifier, assayIdentifier, assay);
        };
    }
    GetAssayAt(studyIdentifier: string, index: int32): ArcAssay {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudy(studyIdentifier);
        return study.GetAssayAt(index);
    }
    static getAssayAt(studyIdentifier: string, index: int32): ((arg0: ArcInvestigation) => ArcAssay) {
        return (inv: ArcInvestigation): ArcAssay => {
            const newInv: ArcInvestigation = inv.Copy();
            return newInv.GetAssayAt(studyIdentifier, index);
        };
    }
    GetAssay(studyIdentifier: string, assayIdentifier: string): ArcAssay {
        const this$: ArcInvestigation = this;
        const study: ArcStudy = this$.GetStudy(studyIdentifier);
        const index: int32 = study.GetAssayIndex(assayIdentifier) | 0;
        return study.GetAssayAt(index);
    }
    static getAssay(studyIdentifier: string, assayIdentifier: string): ((arg0: ArcInvestigation) => ArcAssay) {
        return (inv: ArcInvestigation): ArcAssay => {
            const newInv: ArcInvestigation = inv.Copy();
            return newInv.GetAssay(studyIdentifier, assayIdentifier);
        };
    }
    Copy(): ArcInvestigation {
        const this$: ArcInvestigation = this;
        const newStudies: ArcStudy[] = [];
        let enumerator: any = getEnumerator(this$.Studies);
        try {
            while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
                const study: ArcStudy = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
                const copy: ArcStudy = study.Copy();
                void (newStudies.push(copy));
            }
        }
        finally {
            disposeSafe(enumerator);
        }
        return new ArcInvestigation(this$.Identifier, unwrap(this$.Title), unwrap(this$.Description), unwrap(this$.SubmissionDate), unwrap(this$.PublicReleaseDate), this$.OntologySourceReferences, this$.Publications, this$.Contacts, newStudies, this$.Comments, this$.Remarks);
    }
    ToInvestigation(): Investigation {
        const this$: ArcInvestigation = this;
        const studies: Option<FSharpList<Study>> = fromValueWithDefault<FSharpList<Study>>(empty<Study>(), map_1<ArcStudy, Study>((a: ArcStudy): Study => a.ToStudy(), toList<ArcStudy>(this$.Studies)));
        return Investigation_create_ZB2B0942(void 0, "isa.investigation.xlsx", isMissingIdentifier(this$.Identifier) ? void 0 : this$.Identifier, this$.Title, this$.Description, this$.SubmissionDate, this$.PublicReleaseDate, void 0, fromValueWithDefault<FSharpList<Publication>>(empty<Publication>(), this$.Publications), fromValueWithDefault<FSharpList<Person>>(empty<Person>(), this$.Contacts), studies, fromValueWithDefault<FSharpList<Comment$>>(empty<Comment$>(), this$.Comments));
    }
    static fromInvestigation(i: Investigation): ArcInvestigation {
        let identifer: string;
        const matchValue: Option<string> = i.Identifier;
        identifer = ((matchValue == null) ? createMissingIdentifier() : value_1(matchValue));
        const studies: Option<ArcStudy[]> = map_2<FSharpList<Study>, ArcStudy[]>((arg_2: FSharpList<Study>): ArcStudy[] => {
            const arg_1: FSharpList<ArcStudy> = map_1<Study, ArcStudy>((arg: Study): ArcStudy => ArcStudy.fromStudy(arg), arg_2);
            return Array.from(arg_1);
        }, i.Studies);
        return ArcInvestigation.create(identifer, i.Title, i.Description, i.SubmissionDate, i.PublicReleaseDate, void 0, i.Publications, i.Contacts, studies, i.Comments);
    }
}

export function ArcInvestigation_$reflection(): TypeInfo {
    return class_type("ISA.ArcInvestigation", void 0, ArcInvestigation);
}

export function ArcInvestigation_$ctor_Z8BBFD6A(identifier: string, title?: string, description?: string, submissionDate?: string, publicReleaseDate?: string, ontologySourceReferences?: FSharpList<OntologySourceReference>, publications?: FSharpList<Publication>, contacts?: FSharpList<Person>, studies?: ArcStudy[], comments?: FSharpList<Comment$>, remarks?: FSharpList<Remark>): ArcInvestigation {
    return new ArcInvestigation(identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReferences, publications, contacts, studies, comments, remarks);
}

