import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { empty, filter, map, singleton, append, exists, tryFind, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { equals, IComparable, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

export class OntologySourceReference extends Record implements IEquatable<OntologySourceReference>, IComparable<OntologySourceReference> {
    readonly Description: Option<string>;
    readonly File: Option<string>;
    readonly Name: Option<string>;
    readonly Version: Option<string>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(Description: Option<string>, File: Option<string>, Name: Option<string>, Version: Option<string>, Comments: Option<FSharpList<Comment$>>) {
        super();
        this.Description = Description;
        this.File = File;
        this.Name = Name;
        this.Version = Version;
        this.Comments = Comments;
    }
}

export function OntologySourceReference_$reflection(): TypeInfo {
    return record_type("ISA.OntologySourceReference", [], OntologySourceReference, () => [["Description", option_type(string_type)], ["File", option_type(string_type)], ["Name", option_type(string_type)], ["Version", option_type(string_type)], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function OntologySourceReference_make(description: Option<string>, file: Option<string>, name: Option<string>, version: Option<string>, comments: Option<FSharpList<Comment$>>): OntologySourceReference {
    return new OntologySourceReference(description, file, name, version, comments);
}

export function OntologySourceReference_create_55205B02(Description?: string, File?: string, Name?: string, Version?: string, Comments?: FSharpList<Comment$>): OntologySourceReference {
    return OntologySourceReference_make(Description, File, Name, Version, Comments);
}

export function OntologySourceReference_get_empty(): OntologySourceReference {
    return OntologySourceReference_create_55205B02();
}

/**
 * If an ontology source reference with the given name exists in the list, returns it
 */
export function OntologySourceReference_tryGetByName(name: string, ontologies: FSharpList<OntologySourceReference>): Option<OntologySourceReference> {
    return tryFind<OntologySourceReference>((t: OntologySourceReference): boolean => equals(t.Name, name), ontologies);
}

/**
 * If an ontology source reference with the given name exists in the list, returns true
 */
export function OntologySourceReference_existsByName(name: string, ontologies: FSharpList<OntologySourceReference>): boolean {
    return exists<OntologySourceReference>((t: OntologySourceReference): boolean => equals(t.Name, name), ontologies);
}

/**
 * Adds the given ontology source reference to the investigation
 */
export function OntologySourceReference_add(ontologySourceReference: OntologySourceReference, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
    return append<OntologySourceReference>(ontologies, singleton(ontologySourceReference));
}

/**
 * Updates all ontology source references for which the predicate returns true with the given ontology source reference values
 */
export function OntologySourceReference_updateBy(predicate: ((arg0: OntologySourceReference) => boolean), updateOption: Update_UpdateOptions_$union, ontologySourceReference: OntologySourceReference, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
    if (exists<OntologySourceReference>(predicate, ontologies)) {
        return map<OntologySourceReference, OntologySourceReference>((t: OntologySourceReference): OntologySourceReference => {
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
        }, ontologies);
    }
    else {
        return ontologies;
    }
}

/**
 * If an ontology source reference with the same name as the given name exists in the investigation, updates it with the given ontology source reference
 */
export function OntologySourceReference_updateByName(updateOption: Update_UpdateOptions_$union, ontologySourceReference: OntologySourceReference, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
    return OntologySourceReference_updateBy((t: OntologySourceReference): boolean => equals(t.Name, ontologySourceReference.Name), updateOption, ontologySourceReference, ontologies);
}

/**
 * If a ontology source reference with the given name exists in the list, removes it
 */
export function OntologySourceReference_removeByName(name: string, ontologies: FSharpList<OntologySourceReference>): FSharpList<OntologySourceReference> {
    return filter<OntologySourceReference>((t: OntologySourceReference): boolean => !equals(t.Name, name), ontologies);
}

/**
 * Returns comments of ontology source ref
 */
export function OntologySourceReference_getComments_Z68D7813F(ontology: OntologySourceReference): Option<FSharpList<Comment$>> {
    return ontology.Comments;
}

/**
 * Applies function f on comments in ontology source ref
 */
export function OntologySourceReference_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), ontology: OntologySourceReference): OntologySourceReference {
    return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, mapDefault<FSharpList<Comment$>>(empty<Comment$>(), f, ontology.Comments));
}

/**
 * Replaces comments in ontology source ref by given comment list
 */
export function OntologySourceReference_setComments(ontology: OntologySourceReference, comments: FSharpList<Comment$>): OntologySourceReference {
    return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, comments);
}

