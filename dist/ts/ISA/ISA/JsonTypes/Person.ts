import { map as map_1, Option } from "../../../fable_modules/fable-library-ts/Option.js";
import { OntologyAnnotation_$reflection, OntologyAnnotation } from "./OntologyAnnotation.js";
import { Comment$_$reflection, Comment$ } from "./Comment.js";
import { map2, map, tryFind } from "../../../fable_modules/fable-library-ts/Array.js";
import { IEquatable, equals } from "../../../fable_modules/fable-library-ts/Util.js";
import { FSharpList, singleton, append } from "../../../fable_modules/fable-library-ts/List.js";
import { Update_UpdateOptions_$union } from "../Update.js";
import { record_type, array_type, option_type, string_type, TypeInfo, getRecordFields, makeRecord } from "../../../fable_modules/fable-library-ts/Reflection.js";
import { Update_updateOnlyByExistingAppend, Update_updateOnlyByExisting, Update_updateAppend } from "../Update.js";
import { mapDefault } from "../OptionExtensions.js";
import { Record } from "../../../fable_modules/fable-library-ts/Types.js";

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
    readonly Roles: Option<OntologyAnnotation[]>;
    readonly Comments: Option<Comment$[]>;
    constructor(ID: Option<string>, LastName: Option<string>, FirstName: Option<string>, MidInitials: Option<string>, EMail: Option<string>, Phone: Option<string>, Fax: Option<string>, Address: Option<string>, Affiliation: Option<string>, Roles: Option<OntologyAnnotation[]>, Comments: Option<Comment$[]>) {
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
    static make(id: Option<string>, lastName: Option<string>, firstName: Option<string>, midInitials: Option<string>, email: Option<string>, phone: Option<string>, fax: Option<string>, address: Option<string>, affiliation: Option<string>, roles: Option<OntologyAnnotation[]>, comments: Option<Comment$[]>): Person {
        return new Person(id, lastName, firstName, midInitials, email, phone, fax, address, affiliation, roles, comments);
    }
    static create(Id?: string, LastName?: string, FirstName?: string, MidInitials?: string, Email?: string, Phone?: string, Fax?: string, Address?: string, Affiliation?: string, Roles?: OntologyAnnotation[], Comments?: Comment$[]): Person {
        return Person.make(Id, LastName, FirstName, MidInitials, Email, Phone, Fax, Address, Affiliation, Roles, Comments);
    }
    static get empty(): Person {
        return Person.create();
    }
    static tryGetByFullName(firstName: string, midInitials: string, lastName: string, persons: Person[]): Option<Person> {
        return tryFind<Person>((p: Person): boolean => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))), persons);
    }
    static existsByFullName(firstName: string, midInitials: string, lastName: string, persons: Person[]): boolean {
        return persons.some((p: Person): boolean => ((midInitials === "") ? (equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : ((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
    }
    static add(persons: FSharpList<Person>, person: Person): FSharpList<Person> {
        return append<Person>(persons, singleton(person));
    }
    static updateBy(predicate: ((arg0: Person) => boolean), updateOption: Update_UpdateOptions_$union, person: Person, persons: Person[]): Person[] {
        return persons.some(predicate) ? map<Person, Person>((p: Person): Person => {
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
        }, persons) : persons;
    }
    static updateByFullName(updateOption: Update_UpdateOptions_$union, person: Person, persons: Person[]): Person[] {
        return Person.updateBy((p: Person): boolean => {
            if (equals(p.FirstName, person.FirstName) && equals(p.MidInitials, person.MidInitials)) {
                return equals(p.LastName, person.LastName);
            }
            else {
                return false;
            }
        }, updateOption, person, persons);
    }
    static removeByFullName(firstName: string, midInitials: string, lastName: string, persons: Person[]): Person[] {
        return persons.filter((p: Person): boolean => ((midInitials === "") ? !(equals(p.FirstName, firstName) && equals(p.LastName, lastName)) : !((equals(p.FirstName, firstName) && equals(p.MidInitials, midInitials)) && equals(p.LastName, lastName))));
    }
    static getRoles(person: Person): Option<OntologyAnnotation[]> {
        return person.Roles;
    }
    static mapRoles(f: ((arg0: OntologyAnnotation[]) => OntologyAnnotation[]), person: Person): Person {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, mapDefault<OntologyAnnotation[]>([], f, person.Roles), person.Comments);
    }
    static setRoles(person: Person, roles: OntologyAnnotation[]): Person {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, roles, person.Comments);
    }
    static getComments(person: Person): Option<Comment$[]> {
        return person.Comments;
    }
    static mapComments(f: ((arg0: Comment$[]) => Comment$[]), person: Person): Person {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, mapDefault<Comment$[]>([], f, person.Comments));
    }
    static setComments(person: Person, comments: Comment$[]): Person {
        return new Person(person.ID, person.LastName, person.FirstName, person.MidInitials, person.EMail, person.Phone, person.Fax, person.Address, person.Affiliation, person.Roles, comments);
    }
    Copy(): Person {
        const this$: Person = this;
        const nextComments: Option<Comment$[]> = map_1<Comment$[], Comment$[]>((array: Comment$[]): Comment$[] => map<Comment$, Comment$>((c: Comment$): Comment$ => c.Copy(), array), this$.Comments);
        const arg_9: Option<OntologyAnnotation[]> = map_1<OntologyAnnotation[], OntologyAnnotation[]>((array_1: OntologyAnnotation[]): OntologyAnnotation[] => map<OntologyAnnotation, OntologyAnnotation>((c_1: OntologyAnnotation): OntologyAnnotation => c_1.Copy(), array_1), this$.Roles);
        return Person.make(this$.ID, this$.LastName, this$.FirstName, this$.MidInitials, this$.EMail, this$.Phone, this$.Fax, this$.Address, this$.Affiliation, arg_9, nextComments);
    }
}

export function Person_$reflection(): TypeInfo {
    return record_type("ARCtrl.ISA.Person", [], Person, () => [["ID", option_type(string_type)], ["LastName", option_type(string_type)], ["FirstName", option_type(string_type)], ["MidInitials", option_type(string_type)], ["EMail", option_type(string_type)], ["Phone", option_type(string_type)], ["Fax", option_type(string_type)], ["Address", option_type(string_type)], ["Affiliation", option_type(string_type)], ["Roles", option_type(array_type(OntologyAnnotation_$reflection()))], ["Comments", option_type(array_type(Comment$_$reflection()))]]);
}

