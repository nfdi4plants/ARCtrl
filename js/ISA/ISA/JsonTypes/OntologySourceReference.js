import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Comment$_$reflection } from "./Comment.js";
import { empty, filter, map, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

export class OntologySourceReference extends Record {
    constructor(Description, File, Name, Version, Comments) {
        super();
        this.Description = Description;
        this.File = File;
        this.Name = Name;
        this.Version = Version;
        this.Comments = Comments;
    }
}

export function OntologySourceReference_$reflection() {
    return record_type("ARCtrl.ISA.OntologySourceReference", [], OntologySourceReference, () => [["Description", option_type(string_type)], ["File", option_type(string_type)], ["Name", option_type(string_type)], ["Version", option_type(string_type)], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function OntologySourceReference_make(description, file, name, version, comments) {
    return new OntologySourceReference(description, file, name, version, comments);
}

export function OntologySourceReference_create_5CD5B036(Description, File, Name, Version, Comments) {
    return OntologySourceReference_make(Description, File, Name, Version, Comments);
}

export function OntologySourceReference_get_empty() {
    return OntologySourceReference_create_5CD5B036();
}

/**
 * If an ontology source reference with the given name exists in the list, returns it
 */
export function OntologySourceReference_tryGetByName(name, ontologies) {
    return tryFind((t) => equals(t.Name, name), ontologies);
}

/**
 * If an ontology source reference with the given name exists in the list, returns true
 */
export function OntologySourceReference_existsByName(name, ontologies) {
    return exists((t) => equals(t.Name, name), ontologies);
}

/**
 * Adds the given ontology source reference to the investigation
 */
export function OntologySourceReference_add(ontologySourceReference, ontologies) {
    return append(ontologies, singleton(ontologySourceReference));
}

/**
 * Updates all ontology source references for which the predicate returns true with the given ontology source reference values
 */
export function OntologySourceReference_updateBy(predicate, updateOption, ontologySourceReference, ontologies) {
    if (exists(predicate, ontologies)) {
        return map((t) => {
            if (predicate(t)) {
                const this$ = updateOption;
                const recordType_1 = t;
                const recordType_2 = ontologySourceReference;
                switch (this$.tag) {
                    case 2:
                        return makeRecord(OntologySourceReference_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 1:
                        return makeRecord(OntologySourceReference_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 3:
                        return makeRecord(OntologySourceReference_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
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
export function OntologySourceReference_updateByName(updateOption, ontologySourceReference, ontologies) {
    return OntologySourceReference_updateBy((t) => equals(t.Name, ontologySourceReference.Name), updateOption, ontologySourceReference, ontologies);
}

/**
 * If a ontology source reference with the given name exists in the list, removes it
 */
export function OntologySourceReference_removeByName(name, ontologies) {
    return filter((t) => !equals(t.Name, name), ontologies);
}

/**
 * Returns comments of ontology source ref
 */
export function OntologySourceReference_getComments_Z79C650B(ontology) {
    return ontology.Comments;
}

/**
 * Applies function f on comments in ontology source ref
 */
export function OntologySourceReference_mapComments(f, ontology) {
    return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, mapDefault(empty(), f, ontology.Comments));
}

/**
 * Replaces comments in ontology source ref by given comment list
 */
export function OntologySourceReference_setComments(ontology, comments) {
    return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, comments);
}

