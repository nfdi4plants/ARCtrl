import { filter, map, singleton, append, exists, tryFind } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { record_type, array_type, option_type, string_type, getRecordFields, makeRecord } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { map as map_2, map2 } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { map as map_1 } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { Comment$_$reflection } from "./Comment.js";

export class OntologySourceReference extends Record {
    constructor(Description, File, Name, Version, Comments) {
        super();
        this.Description = Description;
        this.File = File;
        this.Name = Name;
        this.Version = Version;
        this.Comments = Comments;
    }
    static make(description, file, name, version, comments) {
        return new OntologySourceReference(description, file, name, version, comments);
    }
    static create(Description, File, Name, Version, Comments) {
        return OntologySourceReference.make(Description, File, Name, Version, Comments);
    }
    static get empty() {
        return OntologySourceReference.create();
    }
    static tryGetByName(name, ontologies) {
        return tryFind((t) => equals(t.Name, name), ontologies);
    }
    static existsByName(name, ontologies) {
        return exists((t) => equals(t.Name, name), ontologies);
    }
    static add(ontologySourceReference, ontologies) {
        return append(ontologies, singleton(ontologySourceReference));
    }
    static updateBy(predicate, updateOption, ontologySourceReference, ontologies) {
        return exists(predicate, ontologies) ? map((t) => {
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
        }, ontologies) : ontologies;
    }
    static updateByName(updateOption, ontologySourceReference, ontologies) {
        return OntologySourceReference.updateBy((t) => equals(t.Name, ontologySourceReference.Name), updateOption, ontologySourceReference, ontologies);
    }
    static removeByName(name, ontologies) {
        return filter((t) => !equals(t.Name, name), ontologies);
    }
    static getComments(ontology) {
        return ontology.Comments;
    }
    static mapComments(f, ontology) {
        return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, mapDefault([], f, ontology.Comments));
    }
    static setComments(ontology, comments) {
        return new OntologySourceReference(ontology.Description, ontology.File, ontology.Name, ontology.Version, comments);
    }
    Copy() {
        const this$ = this;
        const arg_4 = map_1((array) => map_2((c) => c.Copy(), array), this$.Comments);
        return OntologySourceReference.make(this$.Description, this$.File, this$.Name, this$.Version, arg_4);
    }
}

export function OntologySourceReference_$reflection() {
    return record_type("ARCtrl.ISA.OntologySourceReference", [], OntologySourceReference, () => [["Description", option_type(string_type)], ["File", option_type(string_type)], ["Name", option_type(string_type)], ["Version", option_type(string_type)], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

