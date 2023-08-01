import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { map, defaultArg, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { map as map_1, empty, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologySourceReference_$reflection, OntologySourceReference } from "./OntologySourceReference.js";
import { Publication_$reflection, Publication } from "./Publication.js";
import { Person_$reflection, Person } from "./Person.js";
import { Study_update_Z27CB2981, Study_$reflection, Study } from "./Study.js";
import { Remark_$reflection, Comment$_$reflection, Remark, Comment$ } from "./Comment.js";
import { IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { mapDefault } from "../OptionExtensions.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { toFail } from "../../../fable_modules/fable-library-ts/String.js";

export class Investigation extends Record implements IEquatable<Investigation> {
    readonly ID: Option<string>;
    readonly FileName: Option<string>;
    readonly Identifier: Option<string>;
    readonly Title: Option<string>;
    readonly Description: Option<string>;
    readonly SubmissionDate: Option<string>;
    readonly PublicReleaseDate: Option<string>;
    readonly OntologySourceReferences: Option<FSharpList<OntologySourceReference>>;
    readonly Publications: Option<FSharpList<Publication>>;
    readonly Contacts: Option<FSharpList<Person>>;
    readonly Studies: Option<FSharpList<Study>>;
    readonly Comments: Option<FSharpList<Comment$>>;
    readonly Remarks: FSharpList<Remark>;
    constructor(ID: Option<string>, FileName: Option<string>, Identifier: Option<string>, Title: Option<string>, Description: Option<string>, SubmissionDate: Option<string>, PublicReleaseDate: Option<string>, OntologySourceReferences: Option<FSharpList<OntologySourceReference>>, Publications: Option<FSharpList<Publication>>, Contacts: Option<FSharpList<Person>>, Studies: Option<FSharpList<Study>>, Comments: Option<FSharpList<Comment$>>, Remarks: FSharpList<Remark>) {
        super();
        this.ID = ID;
        this.FileName = FileName;
        this.Identifier = Identifier;
        this.Title = Title;
        this.Description = Description;
        this.SubmissionDate = SubmissionDate;
        this.PublicReleaseDate = PublicReleaseDate;
        this.OntologySourceReferences = OntologySourceReferences;
        this.Publications = Publications;
        this.Contacts = Contacts;
        this.Studies = Studies;
        this.Comments = Comments;
        this.Remarks = Remarks;
    }
}

export function Investigation_$reflection(): TypeInfo {
    return record_type("ISA.Investigation", [], Investigation, () => [["ID", option_type(string_type)], ["FileName", option_type(string_type)], ["Identifier", option_type(string_type)], ["Title", option_type(string_type)], ["Description", option_type(string_type)], ["SubmissionDate", option_type(string_type)], ["PublicReleaseDate", option_type(string_type)], ["OntologySourceReferences", option_type(list_type(OntologySourceReference_$reflection()))], ["Publications", option_type(list_type(Publication_$reflection()))], ["Contacts", option_type(list_type(Person_$reflection()))], ["Studies", option_type(list_type(Study_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))], ["Remarks", list_type(Remark_$reflection())]]);
}

export function Investigation_make(id: Option<string>, filename: Option<string>, identifier: Option<string>, title: Option<string>, description: Option<string>, submissionDate: Option<string>, publicReleaseDate: Option<string>, ontologySourceReference: Option<FSharpList<OntologySourceReference>>, publications: Option<FSharpList<Publication>>, contacts: Option<FSharpList<Person>>, studies: Option<FSharpList<Study>>, comments: Option<FSharpList<Comment$>>, remarks: FSharpList<Remark>): Investigation {
    return new Investigation(id, filename, identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReference, publications, contacts, studies, comments, remarks);
}

export function Investigation_create_ZB2B0942(Id?: string, FileName?: string, Identifier?: string, Title?: string, Description?: string, SubmissionDate?: string, PublicReleaseDate?: string, OntologySourceReferences?: FSharpList<OntologySourceReference>, Publications?: FSharpList<Publication>, Contacts?: FSharpList<Person>, Studies?: FSharpList<Study>, Comments?: FSharpList<Comment$>, Remarks?: FSharpList<Remark>): Investigation {
    return Investigation_make(Id, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, OntologySourceReferences, Publications, Contacts, Studies, Comments, defaultArg(Remarks, empty<Remark>()));
}

export function Investigation_get_empty(): Investigation {
    return Investigation_create_ZB2B0942();
}

/**
 * Returns contacts of an investigation
 */
export function Investigation_getContacts_5997CE50(investigation: Investigation): Option<FSharpList<Person>> {
    return investigation.Contacts;
}

/**
 * Applies function f on person of an investigation
 */
export function Investigation_mapContacts(f: ((arg0: FSharpList<Person>) => FSharpList<Person>), investigation: Investigation): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, mapDefault<FSharpList<Person>>(empty<Person>(), f, investigation.Contacts), investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Replaces persons of an investigation with the given person list
 */
export function Investigation_setContacts(investigation: Investigation, persons: FSharpList<Person>): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, persons, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns publications of an investigation
 */
export function Investigation_getPublications_5997CE50(investigation: Investigation): Option<FSharpList<Publication>> {
    return investigation.Publications;
}

/**
 * Applies function f on publications of an investigation
 */
export function Investigation_mapPublications(f: ((arg0: FSharpList<Publication>) => FSharpList<Publication>), investigation: Investigation): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, mapDefault<FSharpList<Publication>>(empty<Publication>(), f, investigation.Publications), investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Replaces publications of an investigation with the given publication list
 */
export function Investigation_setPublications(investigation: Investigation, publications: FSharpList<Publication>): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, publications, investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns ontology source ref of an investigation
 */
export function Investigation_getOntologies_5997CE50(investigation: Investigation): Option<FSharpList<OntologySourceReference>> {
    return investigation.OntologySourceReferences;
}

/**
 * Applies function f on ontology source ref of an investigation
 */
export function Investigation_mapOntologies(f: ((arg0: FSharpList<OntologySourceReference>) => FSharpList<OntologySourceReference>), investigation: Investigation): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, mapDefault<FSharpList<OntologySourceReference>>(empty<OntologySourceReference>(), f, investigation.OntologySourceReferences), investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Replaces ontology source ref of an investigation with the given ontology source ref list
 */
export function Investigation_setOntologies(investigation: Investigation, ontologies: FSharpList<OntologySourceReference>): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, ontologies, investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns studies of an investigation
 */
export function Investigation_getStudies_5997CE50(investigation: Investigation): FSharpList<Study> {
    return defaultArg(investigation.Studies, empty<Study>());
}

/**
 * Applies function f on studies of an investigation
 */
export function Investigation_mapStudies(f: ((arg0: FSharpList<Study>) => FSharpList<Study>), investigation: Investigation): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, mapDefault<FSharpList<Study>>(empty<Study>(), f, investigation.Studies), investigation.Comments, investigation.Remarks);
}

/**
 * Replaces studies of an investigation with the given study list
 */
export function Investigation_setStudies(investigation: Investigation, studies: FSharpList<Study>): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns comments of an investigation
 */
export function Investigation_getComments_5997CE50(investigation: Investigation): Option<FSharpList<Comment$>> {
    return investigation.Comments;
}

/**
 * Applies function f on comments of an investigation
 */
export function Investigation_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), investigation: Investigation): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, mapDefault<FSharpList<Comment$>>(empty<Comment$>(), f, investigation.Comments), investigation.Remarks);
}

/**
 * Replaces comments of an investigation with the given comment list
 */
export function Investigation_setComments(investigation: Investigation, comments: FSharpList<Comment$>): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, comments, investigation.Remarks);
}

/**
 * Returns remarks of an investigation
 */
export function Investigation_getRemarks_5997CE50(investigation: Investigation): FSharpList<Remark> {
    return investigation.Remarks;
}

/**
 * Applies function f on remarks of an investigation
 */
export function Investigation_mapRemarks(f: ((arg0: FSharpList<Remark>) => FSharpList<Remark>), investigation: Investigation): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, f(investigation.Remarks));
}

/**
 * Replaces remarks of an investigation with the given remark list
 */
export function Investigation_setRemarks(investigation: Investigation, remarks: FSharpList<Remark>): Investigation {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, remarks);
}

/**
 * Update the investigation with the values of the given newInvestigation
 */
export function Investigation_updateBy(updateOption: Update_UpdateOptions_$union, investigation: Investigation, newInvestigation: Investigation): Investigation {
    const this$: Update_UpdateOptions_$union = updateOption;
    const recordType_1: Investigation = investigation;
    const recordType_2: Investigation = newInvestigation;
    switch (this$.tag) {
        case /* UpdateAllAppendLists */ 2:
            return makeRecord(Investigation_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Investigation;
        case /* UpdateByExisting */ 1:
            return makeRecord(Investigation_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Investigation;
        case /* UpdateByExistingAppendLists */ 3:
            return makeRecord(Investigation_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Investigation;
        default:
            return recordType_2;
    }
}

export function Investigation_update_5997CE50(investigation: Investigation): Investigation {
    try {
        return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, map<FSharpList<Study>, FSharpList<Study>>((list: FSharpList<Study>): FSharpList<Study> => map_1<Study, Study>(Study_update_Z27CB2981, list), investigation.Studies), investigation.Comments, investigation.Remarks);
    }
    catch (err: any) {
        return toFail(`Could not update investigation ${investigation.Identifier}: 
${err.message}`);
    }
}

