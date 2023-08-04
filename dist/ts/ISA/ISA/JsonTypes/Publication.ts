import { map as map_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { filter, map, tryFind, exists, FSharpList, singleton, append } from "../../../fable_modules/fable-library-ts/List.js";
import { IEquatable, equals } from "../../../fable_modules/fable-library-ts/Util.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { record_type, array_type, option_type, string_type, TypeInfo, getRecordFields, makeRecord } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { Record } from "../../../fable_modules/fable-library-ts/Types.js";

export class Publication extends Record implements IEquatable<Publication> {
    readonly PubMedID: Option<string>;
    readonly DOI: Option<string>;
    readonly Authors: Option<string>;
    readonly Title: Option<string>;
    readonly Status: Option<OntologyAnnotation>;
    readonly Comments: Option<Comment$[]>;
    constructor(PubMedID: Option<string>, DOI: Option<string>, Authors: Option<string>, Title: Option<string>, Status: Option<OntologyAnnotation>, Comments: Option<Comment$[]>) {
        super();
        this.PubMedID = PubMedID;
        this.DOI = DOI;
        this.Authors = Authors;
        this.Title = Title;
        this.Status = Status;
        this.Comments = Comments;
    }
    static make(pubMedID: Option<string>, doi: Option<string>, authors: Option<string>, title: Option<string>, status: Option<OntologyAnnotation>, comments: Option<Comment$[]>): Publication {
        return new Publication(pubMedID, doi, authors, title, status, comments);
    }
    static create(PubMedID?: string, Doi?: string, Authors?: string, Title?: string, Status?: OntologyAnnotation, Comments?: Comment$[]): Publication {
        return Publication.make(PubMedID, Doi, Authors, Title, Status, Comments);
    }
    static get empty(): Publication {
        return Publication.create();
    }
    static add(publications: FSharpList<Publication>, publication: Publication): FSharpList<Publication> {
        return append<Publication>(publications, singleton(publication));
    }
    static existsByDoi(doi: string, publications: FSharpList<Publication>): boolean {
        return exists<Publication>((p: Publication): boolean => equals(p.DOI, doi), publications);
    }
    static existsByPubMedID(pubMedID: string, publications: FSharpList<Publication>): boolean {
        return exists<Publication>((p: Publication): boolean => equals(p.PubMedID, pubMedID), publications);
    }
    static tryGetByDoi(doi: string, publications: FSharpList<Publication>): Option<Publication> {
        return tryFind<Publication>((publication: Publication): boolean => equals(publication.DOI, doi), publications);
    }
    static updateBy(predicate: ((arg0: Publication) => boolean), updateOption: Update_UpdateOptions_$union, publication: Publication, publications: FSharpList<Publication>): FSharpList<Publication> {
        return exists<Publication>(predicate, publications) ? map<Publication, Publication>((p: Publication): Publication => {
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
        }, publications) : publications;
    }
    static updateByDOI(updateOption: Update_UpdateOptions_$union, publication: Publication, publications: FSharpList<Publication>): FSharpList<Publication> {
        return Publication.updateBy((p: Publication): boolean => equals(p.DOI, publication.DOI), updateOption, publication, publications);
    }
    static updateByPubMedID(updateOption: Update_UpdateOptions_$union, publication: Publication, publications: FSharpList<Publication>): FSharpList<Publication> {
        return Publication.updateBy((p: Publication): boolean => equals(p.PubMedID, publication.PubMedID), updateOption, publication, publications);
    }
    static removeByDoi(doi: string, publications: FSharpList<Publication>): FSharpList<Publication> {
        return filter<Publication>((p: Publication): boolean => !equals(p.DOI, doi), publications);
    }
    static removeByPubMedID(pubMedID: string, publications: FSharpList<Publication>): FSharpList<Publication> {
        return filter<Publication>((p: Publication): boolean => !equals(p.PubMedID, pubMedID), publications);
    }
    static getStatus(publication: Publication): Option<OntologyAnnotation> {
        return publication.Status;
    }
    static mapStatus(f: ((arg0: OntologyAnnotation) => OntologyAnnotation), publication: Publication): Publication {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, map_1<OntologyAnnotation, OntologyAnnotation>(f, publication.Status), publication.Comments);
    }
    static setStatus(publication: Publication, status: OntologyAnnotation): Publication {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, status, publication.Comments);
    }
    static getComments(publication: Publication): Option<Comment$[]> {
        return publication.Comments;
    }
    static mapComments(f: ((arg0: Comment$[]) => Comment$[]), publication: Publication): Publication {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, mapDefault<Comment$[]>([], f, publication.Comments));
    }
    static setComments(publication: Publication, comments: Comment$[]): Publication {
        return new Publication(publication.PubMedID, publication.DOI, publication.Authors, publication.Title, publication.Status, comments);
    }
    Copy(): Publication {
        const this$: Publication = this;
        const arg_5: Option<Comment$[]> = map_1<Comment$[], Comment$[]>((array: Comment$[]): Comment$[] => map_2<Comment$, Comment$>((c: Comment$): Comment$ => c.Copy(), array), this$.Comments);
        return Publication.make(this$.PubMedID, this$.DOI, this$.Authors, this$.Title, this$.Status, arg_5);
    }
}

export function Publication_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Publication", [], Publication, () => [["PubMedID", option_type(string_type)], ["DOI", option_type(string_type)], ["Authors", option_type(string_type)], ["Title", option_type(string_type)], ["Status", option_type(OntologyAnnotation_$reflection())], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

