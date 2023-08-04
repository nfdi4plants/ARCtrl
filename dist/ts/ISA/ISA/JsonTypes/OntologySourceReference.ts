import { map as map_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { filter, map, singleton, append, exists, FSharpList, tryFind } from "../../../fable_modules/fable-library-ts/List.js";
import { IComparable, IEquatable, equals } from "../../../fable_modules/fable-library-ts/Util.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { record_type, array_type, option_type, string_type, TypeInfo, getRecordFields, makeRecord } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { Record } from "../../../fable_modules/fable-library-ts/Types.js";

export class OntologySourceReference extends Record implements IEquatable<OntologySourceReference>, IComparable<OntologySourceReference> {
    readonly Description: Option<string>;
    readonly File: Option<string>;
    readonly Name: Option<string>;
    readonly Version: Option<string>;
    readonly Comments: Option<Comment$[]>;
    constructor(Description: Option<string>, File: Option<string>, Name: Option<string>, Version: Option<string>, Comments: Option<Comment$[]>) {
        super();
        this.Description = Description;
        this.File = File;
        this.Name = Name;
        this.Version = Version;
        this.Comments = Comments;
    }
    static make(description: Option<string>, file: Option<string>, name: Option<string>, version: Option<string>, comments: Option<Comment$[]>): OntologySourceReference {
        return new OntologySourceReference(description, file, name, version, comments);
    }
    static create(Description?: string, File?: string, Name?: string, Version?: string, Comments?: Comment$[]): OntologySourceReference {
        return OntologySourceReference.make(Description, File, Name, Version, Comments);
    }
    static get empty(): OntologySourceReference {
        return OntologySourceReference.create();
    }
    static tryGetByName(name: string, ontologies: FSharpList<OntologySourceReference>): Option<OntologySourceReference> {
        return tryFind<OntologySourceReference>((t: OntologySourceReference): boolean => equals(t.Name, name), ontologies);
    }
    static existsByName(name: string, ontologies: FSharpList<OntologySourceReference>): boolean {
        return exists<OntologySourceReference>((t: OntologySourceReference): boolean => equals(t.Name, name), ontologies);
    }
    static add(ontologySourceReference: OntologySourceReference, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
        return append<OntologySourceReference>(ontologies, singleton(ontologySourceReference));
    }
    static updateBy(predicate: ((arg0: OntologySourceReference) => boolean), updateOption: Update_UpdateOptions_$union, ontologySourceReference: OntologySourceReference, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
        return exists<OntologySourceReference>(predicate, ontologies) ? map<OntologySourceReference, OntologySourceReference>((t: OntologySourceReference): OntologySourceReference => {
            if (predicate(t)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: OntologySourceReference = t;
                const recordType_2: OntologySourceReference = ontologySourceReference;
                switch (this$.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(OntologySourceReference_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as OntologySourceReference;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(OntologySourceReference_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as OntologySourceReference;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(OntologySourceReference_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as OntologySourceReference;
                    default:
                        return recordType_2;
                }
            }
            else {
                return t;
            }
        }, ontologies) : ontologies;
    }
    static updateByName(updateOption: Update_UpdateOptions_$union, ontologySourceReference: OntologySourceReference, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
        return OntologySourceReference.updateBy((t: OntologySourceReference): boolean => equals(t.Name, ontologySourceReference.Name), updateOption, ontologySourceReference, ontologies);
    }
    static removeByName(name: string, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
        return filter<OntologySourceReference>((t: OntologySourceReference): boolean => !equals(t.Name, name), ontologies);
    }
    static getComments(ontology: OntologySourceReference): Option<Comment$[]> {
        return ontology.Comments;
    }
    static mapComments(f: ((arg0: Comment$[]) => Comment$[]), ontology: OntologySourceReference): OntologySourceReference {
        return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, mapDefault<Comment$[]>([], f, ontology.Comments));
    }
    static setComments(ontology: OntologySourceReference, comments: Comment$[]): OntologySourceReference {
        return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, comments);
    }
    Copy(): OntologySourceReference {
        const this$: OntologySourceReference = this;
        const arg_4: Option<Comment$[]> = map_1<Comment$[], Comment$[]>((array: Comment$[]): Comment$[] => map_2<Comment$, Comment$>((c: Comment$): Comment$ => c.Copy(), array), this$.Comments);
        return OntologySourceReference.make(this$.Description, this$.File, this$.Name, this$.Version, arg_4);
    }
}

export function OntologySourceReference_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.OntologySourceReference", [], OntologySourceReference, () => [["Description", option_type(string_type)], ["File", option_type(string_type)], ["Name", option_type(string_type)], ["Version", option_type(string_type)], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

