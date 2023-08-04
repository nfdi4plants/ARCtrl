import { filter, map, tryFind, exists, singleton, append } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { record_type, array_type, option_type, string_type, getRecordFields, makeRecord } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { map as map_1 } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { mapDefault } from "../OptionExtensions.js";
import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Comment$_$reflection } from "./Comment.js";

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
    static make(pubMedID, doi, authors, title, status, comments) {
        return new Publication(pubMedID, doi, authors, title, status, comments);
    }
    static create(PubMedID, Doi, Authors, Title, Status, Comments) {
        return Publication.make(PubMedID, Doi, Authors, Title, Status, Comments);
    }
    static get empty() {
        return Publication.create();
    }
    static add(publications, publication) {
        return append(publications, singleton(publication));
    }
    static existsByDoi(doi, publications) {
        return exists((p) => equals(p.DOI, doi), publications);
    }
    static existsByPubMedID(pubMedID, publications) {
        return exists((p) => equals(p.PubMedID, pubMedID), publications);
    }
    static tryGetByDoi(doi, publications) {
        return tryFind((publication) => equals(publication.DOI, doi), publications);
    }
    static updateBy(predicate, updateOption, publication, publications) {
        return exists(predicate, publications) ? map((p) => {
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
        }, publications) : publications;
    }
    static updateByDOI(updateOption, publication, publications) {
        return Publication.updateBy((p) => equals(p.DOI, publication.DOI), updateOption, publication, publications);
    }
    static updateByPubMedID(updateOption, publication, publications) {
        return Publication.updateBy((p) => equals(p.PubMedID, publication.PubMedID), updateOption, publication, publications);
    }
    static removeByDoi(doi, publications) {
        return filter((p) => !equals(p.DOI, doi), publications);
    }
    static removeByPubMedID(pubMedID, publications) {
        return filter((p) => !equals(p.PubMedID, pubMedID), publications);
    }
    static getStatus(publication) {
        return publication.Status;
    }
    static mapStatus(f, publication) {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, map_1(f, publication.Status), publication.Comments);
    }
    static setStatus(publication, status) {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, status, publication.Comments);
    }
    static getComments(publication) {
        return publication.Comments;
    }
    static mapComments(f, publication) {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, mapDefault([], f, publication.Comments));
    }
    static setComments(publication, comments) {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, comments);
    }
    Copy() {
        const this$ = this;
        const arg_5 = map_1((array) => map_2((c) => c.Copy(), array), this$.Comments);
        return Publication.make(this$.PubMedID, this$.DOI, this$.Authors, this$.Title, this$.Status, arg_5);
    }
}

export function Publication_$reflection() {
    return record_type("ARCtrl.ISA.Publication", [], Publication, () => [["PubMedID", option_type(string_type)], ["DOI", option_type(string_type)], ["Authors", option_type(string_type)], ["Title", option_type(string_type)], ["Status", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

