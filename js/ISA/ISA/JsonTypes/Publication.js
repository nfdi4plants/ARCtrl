import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, array_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Comment$_$reflection } from "./Comment.js";
import { filter, map, tryFind, exists, singleton, append } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { map as map_1 } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { mapDefault } from "../OptionExtensions.js";

export class Publication extends Record {
    constructor(PubMedID, DOI, Authors, Title, Status, Comments) {
        super();
        this.PubMedID = PubMedID;
        this.DOI = DOI;
        this.Authors = Authors;
        this.Title = Title;
        this.Status = Status;
        this.Comments = Comments;
    }
}

export function Publication_$reflection() {
    return record_type("ARCtrl.ISA.Publication", [], Publication, () => [["PubMedID", option_type(string_type)], ["DOI", option_type(string_type)], ["Authors", option_type(string_type)], ["Title", option_type(string_type)], ["Status", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

export function Publication_make(pubMedID, doi, authors, title, status, comments) {
    return new Publication(pubMedID, doi, authors, title, status, comments);
}

export function Publication_create_496BCAB8(PubMedID, Doi, Authors, Title, Status, Comments) {
    return Publication_make(PubMedID, Doi, Authors, Title, Status, Comments);
}

export function Publication_get_empty() {
    return Publication_create_496BCAB8();
}

/**
 * Adds the given publication to the publications
 */
export function Publication_add(publications, publication) {
    return append(publications, singleton(publication));
}

/**
 * Returns true, if a publication with the given doi exists in the investigation
 */
export function Publication_existsByDoi(doi, publications) {
    return exists((p) => equals(p.DOI, doi), publications);
}

/**
 * Returns true, if a publication with the given pubmedID exists in the investigation
 */
export function Publication_existsByPubMedID(pubMedID, publications) {
    return exists((p) => equals(p.PubMedID, pubMedID), publications);
}

/**
 * If a publication with the given doi exists in the investigation, returns it
 */
export function Publication_tryGetByDoi(doi, publications) {
    return tryFind((publication) => equals(publication.DOI, doi), publications);
}

/**
 * Updates all publications for which the predicate returns true with the given publication values
 */
export function Publication_updateBy(predicate, updateOption, publication, publications) {
    if (exists(predicate, publications)) {
        return map((p) => {
            if (predicate(p)) {
                const this$ = updateOption;
                const recordType_1 = p;
                const recordType_2 = publication;
                switch (this$.tag) {
                    case 2:
                        return makeRecord(Publication_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 1:
                        return makeRecord(Publication_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 3:
                        return makeRecord(Publication_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    default:
                        return recordType_2;
                }
            }
            else {
                return p;
            }
        }, publications);
    }
    else {
        return publications;
    }
}

/**
 * Updates all protocols with the same DOI as the given publication with its values
 */
export function Publication_updateByDOI(updateOption, publication, publications) {
    return Publication_updateBy((p) => equals(p.DOI, publication.DOI), updateOption, publication, publications);
}

/**
 * Updates all protocols with the same pubMedID as the given publication with its values
 */
export function Publication_updateByPubMedID(updateOption, publication, publications) {
    return Publication_updateBy((p) => equals(p.PubMedID, publication.PubMedID), updateOption, publication, publications);
}

/**
 * If a publication with the given doi exists in the investigation, removes it from the investigation
 */
export function Publication_removeByDoi(doi, publications) {
    return filter((p) => !equals(p.DOI, doi), publications);
}

/**
 * If a publication with the given pubMedID exists in the investigation, removes it
 */
export function Publication_removeByPubMedID(pubMedID, publications) {
    return filter((p) => !equals(p.PubMedID, pubMedID), publications);
}

/**
 * Status
 * Returns publication status of a publication
 */
export function Publication_getStatus_Z3279EA88(publication) {
    return publication.Status;
}

/**
 * Applies function f on publication status of a publication
 */
export function Publication_mapStatus(f, publication) {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, map_1(f, publication.Status), publication.Comments);
}

/**
 * Replaces publication status of a publication by given publication status
 */
export function Publication_setStatus(publication, status) {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, status, publication.Comments);
}

/**
 * Returns comments of a protocol
 */
export function Publication_getComments_Z3279EA88(publication) {
    return publication.Comments;
}

/**
 * Applies function f on comments of a protocol
 */
export function Publication_mapComments(f, publication) {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, mapDefault([], f, publication.Comments));
}

/**
 * Replaces comments of a protocol by given comment list
 */
export function Publication_setComments(publication, comments) {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, comments);
}

