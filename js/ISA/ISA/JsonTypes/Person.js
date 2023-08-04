import { Record } from "../../../fable_modules/fable-library.4.1.4/Types.js";
import { getRecordFields, makeRecord, record_type, array_type, option_type, string_type } from "../../../fable_modules/fable-library.4.1.4/Reflection.js";
import { OntologyAnnotation_$reflection } from "./OntologyAnnotation.js";
import { Comment$_$reflection } from "./Comment.js";
import { map2, map, tryFind } from "../../../fable_modules/fable-library.4.1.4/Array.js";
import { equals } from "../../../fable_modules/fable-library.4.1.4/Util.js";
import { singleton, append } from "../../../fable_modules/fable-library.4.1.4/List.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

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
}

export function Person_$reflection() {
    return record_type("ARCtrl.ISA.Person", [], Person, () => [["ID", option_type(string_type)], ["LastName", option_type(string_type)], ["FirstName", option_type(string_type)], ["MidInitials", option_type(string_type)], ["EMail", option_type(string_type)], ["Phone", option_type(string_type)], ["Fax", option_type(string_type)], ["Address", option_type(string_type)], ["Affiliation", option_type(string_type)], ["Roles", option_type(array_type(OntologyAnnotation_$reflection()))], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

export function Person_make(id, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments) {
    return new Person(id, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments);
}

export function Person_create_4538B98B(Id, LastName, FirstName, MidInitials, Email, Phone, Fax, Address, Affiliation, Roles, Comments) {
    return Person_make(Id, LastName, FirstName, MidInitials, Email, Phone, Fax, Address, Affiliation, Roles, Comments);
}

export function Person_get_empty() {
    return Person_create_4538B98B();
}

export function Person_tryGetByFullName(firstName, midInitials, lastName, persons) {
    return tryFind((p) => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))), persons);
}

/**
 * If an person with the given FirstName, MidInitials and LastName exists in the list, returns true
 */
export function Person_existsByFullName(firstName, midInitials, lastName, persons) {
    return persons.some((p) => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
}

/**
 * adds the given person to the persons
 */
export function Person_add(persons, person) {
    return append(persons, singleton(person));
}

/**
 * Updates all persons for which the predicate returns true with the given person values
 */
export function Person_updateBy(predicate, updateOption, person, persons) {
    if (persons.some(predicate)) {
        return map((p) => {
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
        }, persons);
    }
    else {
        return persons;
    }
}

/**
 * Updates all persons with the same FirstName, MidInitials and LastName as the given person with its values
 */
export function Person_updateByFullName(updateOption, person, persons) {
    return Person_updateBy((p) => {
        if (equals(p.FirstName, person.FirstName) && equals(p.MidInitials, person.MidInitials)) {
            return equals(p.LastName, person.LastName);
        }
        else {
            return false;
        }
    }, updateOption, person, persons);
}

/**
 * If a person with the given FirstName, MidInitials and LastName exists in the list, removes it
 */
export function Person_removeByFullName(firstName, midInitials, lastName, persons) {
    return persons.filter((p) => ((midInitials === "") ? !(equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : !((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
}

/**
 * Returns roles of a person
 */
export function Person_getRoles_Z22779EAF(person) {
    return person.Roles;
}

/**
 * Applies function f on roles of a person
 */
export function Person_mapRoles(f, person) {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, mapDefault([], f, person.Roles), person.Comments);
}

/**
 * Replaces roles of a person with the given roles
 */
export function Person_setRoles(person, roles) {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, roles, person.Comments);
}

/**
 * Returns comments of a person
 */
export function Person_getComments_Z22779EAF(person) {
    return person.Comments;
}

/**
 * Applies function f on comments of a person
 */
export function Person_mapComments(f, person) {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, mapDefault([], f, person.Comments));
}

/**
 * Replaces comments of a person by given comment list
 */
export function Person_setComments(person, comments) {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, comments);
}

