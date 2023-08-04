import { map2, map, tryFind } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { singleton, append } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { record_type, array_type, option_type, string_type, getRecordFields, makeRecord } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { map as map_1 } from "../../../fable_modules/fable-library.4.1.4/Option.js";
import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Comment$_$reflection } from "./Comment.js";

export class Person extends Record {
    constructor(ID, LastName, FirstName, MidInitials, EMail, Phone, Fax, Address, Affiliation, Roles, Comments) {
        super();
        this.ID = ID;
        this.LastName = LastName;
        this.FirstName = FirstName;
        this.MidInitials = MidInitials;
        this.EMail = EMail;
        this.Phone = Phone;
        this.Fax = Fax;
        this.Address = Address;
        this.Affiliation = Affiliation;
        this.Roles = Roles;
        this.Comments = Comments;
    }
    static make(id, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments) {
        return new Person(id, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments);
    }
    static create(Id, LastName, FirstName, MidInitials, Email, Phone, Fax, Address, Affiliation, Roles, Comments) {
        return Person.make(Id, LastName, FirstName, MidInitials, Email, Phone, Fax, Address, Affiliation, Roles, Comments);
    }
    static get empty() {
        return Person.create();
    }
    static tryGetByFullName(firstName, midInitials, lastName, persons) {
        return tryFind((p) => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))), persons);
    }
    static existsByFullName(firstName, midInitials, lastName, persons) {
        return persons.some((p) => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
    }
    static add(persons, person) {
        return append(persons, singleton(person));
    }
    static updateBy(predicate, updateOption, person, persons) {
        return persons.some(predicate) ? map((p) => {
            if (predicate(p)) {
                const this$ = updateOption;
                const recordType_1 = p;
                const recordType_2 = person;
                switch (this$.tag) {
                    case 2:
                        return makeRecord(Person_$reflection(), map2(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 1:
                        return makeRecord(Person_$reflection(), map2(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    case 3:
                        return makeRecord(Person_$reflection(), map2(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2)));
                    default:
                        return recordType_2;
                }
            }
            else {
                return p;
            }
        }, persons) : persons;
    }
    static updateByFullName(updateOption, person, persons) {
        return Person.updateBy((p) => {
            if (equals(p.FirstName, person.FirstName) && equals(p.MidInitials, person.MidInitials)) {
                return equals(p.LastName, person.LastName);
            }
            else {
                return false;
            }
        }, updateOption, person, persons);
    }
    static removeByFullName(firstName, midInitials, lastName, persons) {
        return persons.filter((p) => ((midInitials === "") ? !(equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : !((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
    }
    static getRoles(person) {
        return person.Roles;
    }
    static mapRoles(f, person) {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, mapDefault([], f, person.Roles), person.Comments);
    }
    static setRoles(person, roles) {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, roles, person.Comments);
    }
    static getComments(person) {
        return person.Comments;
    }
    static mapComments(f, person) {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, mapDefault([], f, person.Comments));
    }
    static setComments(person, comments) {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, comments);
    }
    Copy() {
        const this$ = this;
        const nextComments = map_1((array) => map((c) => c.Copy(), array), this$.Comments);
        const arg_9 = map_1((array_1) => map((c_1) => c_1.Copy(), array_1), this$.Roles);
        return Person.make(this$.ID, this$.LastName, this$.FirstName, this$.MidInitials, this$.EMail, this$.Phone, this$.Fax, this$.Address, this$.Affiliation, arg_9, nextComments);
    }
}

export function Person_$reflection() {
    return record_type("ARCtrl.ISA.Person", [], Person, () => [["ID", option_type(string_type)], ["LastName", option_type(string_type)], ["FirstName", option_type(string_type)], ["MidInitials", option_type(string_type)], ["EMail", option_type(string_type)], ["Phone", option_type(string_type)], ["Fax", option_type(string_type)], ["Address", option_type(string_type)], ["Affiliation", option_type(string_type)], ["Roles", option_type(array_type(OntologyAnnotation_$reflection()))], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

