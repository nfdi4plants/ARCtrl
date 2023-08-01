import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { map as map_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { empty, filter, map, tryFind, exists, singleton, append, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

export class Publication extends Record implements IEquatable<Publication> {
    readonly PubMedID: Option<string>;
    readonly DOI: Option<string>;
    readonly Authors: Option<string>;
    readonly Title: Option<string>;
    readonly Status: Option<OntologyAnnotation>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(PubMedID: Option<string>, DOI: Option<string>, Authors: Option<string>, Title: Option<string>, Status: Option<OntologyAnnotation>, Comments: Option<FSharpList<Comment$>>) {
        super();
        this.PubMedID = PubMedID;
        this.DOI = DOI;
        this.Authors = Authors;
        this.Title = Title;
        this.Status = Status;
        this.Comments = Comments;
    }
}

export function Publication_$reflection(): TypeInfo {
    return record_type("ISA.Publication", [], Publication, () => [["PubMedID", option_type(string_type)], ["DOI", option_type(string_type)], ["Authors", option_type(string_type)], ["Title", option_type(string_type)], ["Status", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Publication_make(pubMedID: Option<string>, doi: Option<string>, authors: Option<string>, title: Option<string>, status: Option<OntologyAnnotation>, comments: Option<FSharpList<Comment$>>): Publication {
    return new Publication(pubMedID, doi, authors, title, status, comments);
}

export function Publication_create_Z3E55064F(PubMedID?: string, Doi?: string, Authors?: string, Title?: string, Status?: OntologyAnnotation, Comments?: FSharpList<Comment$>): Publication {
    return Publication_make(PubMedID, Doi, Authors, Title, Status, Comments);
}

export function Publication_get_empty(): Publication {
    return Publication_create_Z3E55064F();
}

/**
 * Adds the given publication to the publications
 */
export function Publication_add(publications: FSharpList<Publication>, publication: Publication): FSharpList<Publication> {
    return append<Publication>(publications, singleton(publication));
}

/**
 * Returns true, if a publication with the given doi exists in the investigation
 */
export function Publication_existsByDoi(doi: string, publications: FSharpList<Publication>): boolean {
    return exists<Publication>((p: Publication): boolean => equals(p.DOI, doi), publications);
}

/**
 * Returns true, if a publication with the given pubmedID exists in the investigation
 */
export function Publication_existsByPubMedID(pubMedID: string, publications: FSharpList<Publication>): boolean {
    return exists<Publication>((p: Publication): boolean => equals(p.PubMedID, pubMedID), publications);
}

/**
 * If a publication with the given doi exists in the investigation, returns it
 */
export function Publication_tryGetByDoi(doi: string, publications: FSharpList<Publication>): Option<Publication> {
    return tryFind<Publication>((publication: Publication): boolean => equals(publication.DOI, doi), publications);
}

/**
 * Updates all publications for which the predicate returns true with the given publication values
 */
export function Publication_updateBy(predicate: ((arg0: Publication) => boolean), updateOption: Update_UpdateOptions_$union, publication: Publication, publications: FSharpList<Publication>): FSharpList<Publication> {
    if (exists<Publication>(predicate, publications)) {
        return map<Publication, Publication>((p: Publication): Publication => {
            if (predicate(p)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Publication = p;
                const recordType_2: Publication = publication;
                switch (this$.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Publication_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Publication;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Publication_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Publication;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Publication_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Publication;
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
export function Publication_updateByDOI(updateOption: Update_UpdateOptions_$union, publication: Publication, publications: FSharpList<Publication>): FSharpList<Publication> {
    return Publication_updateBy((p: Publication): boolean => equals(p.DOI, publication.DOI), updateOption, publication, publications);
}

/**
 * Updates all protocols with the same pubMedID as the given publication with its values
 */
export function Publication_updateByPubMedID(updateOption: Update_UpdateOptions_$union, publication: Publication, publications: FSharpList<Publication>): FSharpList<Publication> {
    return Publication_updateBy((p: Publication): boolean => equals(p.PubMedID, publication.PubMedID), updateOption, publication, publications);
}

/**
 * If a publication with the given doi exists in the investigation, removes it from the investigation
 */
export function Publication_removeByDoi(doi: string, publications: FSharpList<Publication>): FSharpList<Publication> {
    return filter<Publication>((p: Publication): boolean => !equals(p.DOI, doi), publications);
}

/**
 * If a publication with the given pubMedID exists in the investigation, removes it
 */
export function Publication_removeByPubMedID(pubMedID: string, publications: FSharpList<Publication>): FSharpList<Publication> {
    return filter<Publication>((p: Publication): boolean => !equals(p.PubMedID, pubMedID), publications);
}

/**
 * Status
 * Returns publication status of a publication
 */
export function Publication_getStatus_Z3C2FFBB4(publication: Publication): Option<OntologyAnnotation> {
    return publication.Status;
}

/**
 * Applies function f on publication status of a publication
 */
export function Publication_mapStatus(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), publication: Publication): Publication {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, map_1<OntologyAnnotation, OntologyAnnotation>(f, publication.Status), publication.Comments);
}

/**
 * Replaces publication status of a publication by given publication status
 */
export function Publication_setStatus(publication: Publication, status: OntologyAnnotation): Publication {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, status, publication.Comments);
}

/**
 * Returns comments of a protocol
 */
export function Publication_getComments_Z3C2FFBB4(publication: Publication): Option<FSharpList<Comment$>> {
    return publication.Comments;
}

/**
 * Applies function f on comments of a protocol
 */
export function Publication_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), publication: Publication): Publication {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, mapDefault<FSharpList<Comment$>>(empty<Comment$>(), f, publication.Comments));
}

/**
 * Replaces comments of a protocol by given comment list
 */
export function Publication_setComments(publication: Publication, comments: FSharpList<Comment$>): Publication {
    return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, comments);
}

