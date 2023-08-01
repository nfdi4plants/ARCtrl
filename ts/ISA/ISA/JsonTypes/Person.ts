import { Record } from "../../../fable_modules/fable-library-ts/Types.js";
import { Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { empty, filter, map, singleton, append, exists, tryFind, FSharpList } from "../../../fable_modules/fable-library-ts/List.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { equals, IEquatable } from "../../../fable_modules/fable-library-ts/Util.js";
import { getRecordFields, makeRecord, record_type, list_type, option_type, string_type, TypeInfo } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { map2 } from "../../../fable_modules/fable-library-ts/Array.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";

export class Person extends Record implements IEquatable<Person> {
    readonly ID: Option<string>;
    readonly LastName: Option<string>;
    readonly FirstName: Option<string>;
    readonly MidInitials: Option<string>;
    readonly EMail: Option<string>;
    readonly Phone: Option<string>;
    readonly Fax: Option<string>;
    readonly Address: Option<string>;
    readonly Affiliation: Option<string>;
    readonly Roles: Option<FSharpList<OntologyAnnotation>>;
    readonly Comments: Option<FSharpList<Comment$>>;
    constructor(ID: Option<string>, LastName: Option<string>, FirstName: Option<string>, MidInitials: Option<string>, EMail: Option<string>, Phone: Option<string>, Fax: Option<string>, Address: Option<string>, Affiliation: Option<string>, Roles: Option<FSharpList<OntologyAnnotation>>, Comments: Option<FSharpList<Comment$>>) {
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

export function Person_$reflection(): TypeInfo {
    return record_type("ISA.Person", [], Person, () => [["ID", option_type(string_type)], ["LastName", option_type(string_type)], ["FirstName", option_type(string_type)], ["MidInitials", option_type(string_type)], ["EMail", option_type(string_type)], ["Phone", option_type(string_type)], ["Fax", option_type(string_type)], ["Address", option_type(string_type)], ["Affiliation", option_type(string_type)], ["Roles", option_type(list_type(OntologyAnnotation_$reflection()))], ["Comments", option_type(list_type(Comment$_$reflection()))]]);
}

export function Person_make(id: Option<string>, lastName: Option<string>, firstName: Option<string>, midInitials: Option<string>, email: Option<string>, phone: Option<string>, fax: Option<string>, address: Option<string>, affiliation: Option<string>, roles: Option<FSharpList<OntologyAnnotation>>, comments: Option<FSharpList<Comment$>>): Person {
    return new Person(id, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments);
}

export function Person_create_28E835CB(Id?: string, LastName?: string, FirstName?: string, MidInitials?: string, Email?: string, Phone?: string, Fax?: string, Address?: string, Affiliation?: string, Roles?: FSharpList<OntologyAnnotation>, Comments?: FSharpList<Comment$>): Person {
    return Person_make(Id, LastName, FirstName, MidInitials, Email, Phone, Fax, Address, Affiliation, Roles, Comments);
}

export function Person_get_empty(): Person {
    return Person_create_28E835CB();
}

export function Person_tryGetByFullName(firstName: string, midInitials: string, lastName: string, persons: FSharpList<Person>): Option<Person> {
    return tryFind<Person>((p: Person): boolean => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))), persons);
}

/**
 * If an person with the given FirstName, MidInitials and LastName exists in the list, returns true
 */
export function Person_existsByFullName(firstName: string, midInitials: string, lastName: string, persons: FSharpList<Person>): boolean {
    return exists<Person>((p: Person): boolean => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))), persons);
}

/**
 * adds the given person to the persons
 */
export function Person_add(persons: FSharpList<Person>, person: Person): FSharpList<Person> {
    return append<Person>(persons, singleton(person));
}

/**
 * Updates all persons for which the predicate returns true with the given person values
 */
export function Person_updateBy(predicate: ((arg0: Person) => boolean), updateOption: Update_UpdateOptions_$union, person: Person, persons: FSharpList<Person>): FSharpList<Person> {
    if (exists<Person>(predicate, persons)) {
        return map<Person, Person>((p: Person): Person => {
            if (predicate(p)) {
                const this$: Update_UpdateOptions_$union = updateOption;
                const recordType_1: Person = p;
                const recordType_2: Person = person;
                switch (this$.tag) {
                    case /* UpdateAllAppendLists */ 2:
                        return makeRecord(Person_$reflection(), map2<any, any, any>(Update_updateAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Person;
                    case /* UpdateByExisting */ 1:
                        return makeRecord(Person_$reflection(), map2<any, any, any>(Update_updateOnlyByExisting, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Person;
                    case /* UpdateByExistingAppendLists */ 3:
                        return makeRecord(Person_$reflection(), map2<any, any, any>(Update_updateOnlyByExistingAppend, getRecordFields(recordType_1), getRecordFields(recordType_2))) as Person;
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
export function Person_updateByFullName(updateOption: Update_UpdateOptions_$union, person: Person, persons: FSharpList<Person>): FSharpList<Person> {
    return Person_updateBy((p: Person): boolean => {
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
export function Person_removeByFullName(firstName: string, midInitials: string, lastName: string, persons: FSharpList<Person>): FSharpList<Person> {
    return filter<Person>((p: Person): boolean => ((midInitials === "") ? !(equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : !((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))), persons);
}

/**
 * Returns roles of a person
 */
export function Person_getRoles_Z2DD38D1B(person: Person): Option<FSharpList<OntologyAnnotation>> {
    return person.Roles;
}

/**
 * Applies function f on roles of a person
 */
export function Person_mapRoles(f: ((arg0: FSharpList<OntologyAnnotation>) => FSharpList<OntologyAnnotation>), person: Person): Person {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, mapDefault<FSharpList<OntologyAnnotation>>(empty<OntologyAnnotation>(), f, person.Roles), person.Comments);
}

/**
 * Replaces roles of a person with the given roles
 */
export function Person_setRoles(person: Person, roles: FSharpList<OntologyAnnotation>): Person {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, roles, person.Comments);
}

/**
 * Returns comments of a person
 */
export function Person_getComments_Z2DD38D1B(person: Person): Option<FSharpList<Comment$>> {
    return person.Comments;
}

/**
 * Applies function f on comments of a person
 */
export function Person_mapComments(f: ((arg0: FSharpList<Comment$>) => FSharpList<Comment$>), person: Person): Person {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, mapDefault<FSharpList<Comment$>>(empty<Comment$>(), f, person.Comments));
}

/**
 * Replaces comments of a person by given comment list
 */
export function Person_setComments(person: Person, comments: FSharpList<Comment$>): Person {
    return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, comments);
}

