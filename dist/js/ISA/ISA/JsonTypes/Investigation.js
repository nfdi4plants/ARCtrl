import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologySourceReference_$reflection } from "./OntologySourceReference.js";
import { Publication_$reflection } from "./Publication.js";
import { Person_$reflection } from "./Person.js";
import { Study_update_7312BC8B, Study_$reflection } from "./Study.js";
import { Remark_$reflection, Comment$_$reflection } from "./Comment.js";
import { map, defaultArg } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { map as map_1, empty } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { mapDefault } from "../OptionExtensions.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { toFail } from "../../../fable_modules/fable-library.4.1.4/String.js";

export class Investigation extends Record {
    constructor(ID, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, OntologySourceReferences, Publications, Contacts, Studies, Comments, Remarks) {
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

export function Investigation_$reflection() {
    return record_type("ARCtrl.ISA.Investigation", [], Investigation, () => [["ID", option_type(string_type)], ["FileName", option_type(string_type)], ["Identifier", option_type(string_type)], ["Title", option_type(string_type)], ["Description", option_type(string_type)], ["SubmissionDate", option_type(string_type)], ["PublicReleaseDate", option_type(string_type)], ["OntologySourceReferences", option_type(list_type(OntologySourceReference_$reflection()))], ["Publications", option_type(list_type(Publication_$reflection()))], ["Contacts", option_type(list_type(Person_$reflection()))], ["Studies", option_type(list_type(Study_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))], ["Remarks", list_type(Remark_$reflection())]]);
}

export function Investigation_make(id, filename, identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReference, publications, contacts, studies, comments, remarks) {
    return new Investigation(id, filename, identifier, title, description, submissionDate, publicReleaseDate, ontologySourceReference, publications, contacts, studies, comments, remarks);
}

export function Investigation_create_4AD66BBE(Id, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, OntologySourceReferences, Publications, Contacts, Studies, Comments, Remarks) {
    return Investigation_make(Id, FileName, Identifier, Title, Description, SubmissionDate, PublicReleaseDate, OntologySourceReferences, Publications, Contacts, Studies, Comments, defaultArg(Remarks, empty()));
}

export function Investigation_get_empty() {
    return Investigation_create_4AD66BBE();
}

/**
 * Returns contacts of an investigation
 */
export function Investigation_getContacts_33B81164(investigation) {
    return investigation.Contacts;
}

/**
 * Applies function f on person of an investigation
 */
export function Investigation_mapContacts(f, investigation) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, mapDefault(empty(), f, investigation.Contacts), investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Replaces persons of an investigation with the given person list
 */
export function Investigation_setContacts(investigation, persons) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, persons, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns publications of an investigation
 */
export function Investigation_getPublications_33B81164(investigation) {
    return investigation.Publications;
}

/**
 * Applies function f on publications of an investigation
 */
export function Investigation_mapPublications(f, investigation) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, mapDefault(empty(), f, investigation.Publications), investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Replaces publications of an investigation with the given publication list
 */
export function Investigation_setPublications(investigation, publications) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, publications, investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns ontology source ref of an investigation
 */
export function Investigation_getOntologies_33B81164(investigation) {
    return investigation.OntologySourceReferences;
}

/**
 * Applies function f on ontology source ref of an investigation
 */
export function Investigation_mapOntologies(f, investigation) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, mapDefault(empty(), f, investigation.OntologySourceReferences), investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Replaces ontology source ref of an investigation with the given ontology source ref list
 */
export function Investigation_setOntologies(investigation, ontologies) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, ontologies, investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns studies of an investigation
 */
export function Investigation_getStudies_33B81164(investigation) {
    return defaultArg(investigation.Studies, empty());
}

/**
 * Applies function f on studies of an investigation
 */
export function Investigation_mapStudies(f, investigation) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, mapDefault(empty(), f, investigation.Studies), investigation.Comments, investigation.Remarks);
}

/**
 * Replaces studies of an investigation with the given study list
 */
export function Investigation_setStudies(investigation, studies) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, studies, investigation.Comments, investigation.Remarks);
}

/**
 * Returns comments of an investigation
 */
export function Investigation_getComments_33B81164(investigation) {
    return investigation.Comments;
}

/**
 * Applies function f on comments of an investigation
 */
export function Investigation_mapComments(f, investigation) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, mapDefault(empty(), f, investigation.Comments), investigation.Remarks);
}

/**
 * Replaces comments of an investigation with the given comment list
 */
export function Investigation_setComments(investigation, comments) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, comments, investigation.Remarks);
}

/**
 * Returns remarks of an investigation
 */
export function Investigation_getRemarks_33B81164(investigation) {
    return investigation.Remarks;
}

/**
 * Applies function f on remarks of an investigation
 */
export function Investigation_mapRemarks(f, investigation) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, f(investigation.Remarks));
}

/**
 * Replaces remarks of an investigation with the given remark list
 */
export function Investigation_setRemarks(investigation, remarks) {
    return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, investigation.Studies, investigation.Comments, remarks);
}

/**
 * Update the investigation with the values of the given newInvestigation
 */
export function Investigation_updateBy(updateOption, investigation, newInvestigation) {
    const this$ = updateOption;
    const recordType_1 = investigation;
    const recordType_2 = newInvestigation;
    switch (this$.tag) {
        case 2:
            return makeRecord(Investigation_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
        case 1:
            return makeRecord(Investigation_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
        case 3:
            return makeRecord(Investigation_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
        default:
            return recordType_2;
    }
}

export function Investigation_update_33B81164(investigation) {
    try {
        return new Investigation(investigation.ID, investigation.FileName, investigation.Identifier, investigation.Title, investigation.Description, investigation.SubmissionDate, investigation.PublicReleaseDate, investigation.OntologySourceReferences, investigation.Publications, investigation.Contacts, map((list) => map_1(Study_update_7312BC8B, list), investigation.Studies), investigation.Comments, investigation.Remarks);
    }
    catch (err) {
        return toFail(`Could not update investigation ${investigation.Identifier}: 
${err.message}`);
    }
}

