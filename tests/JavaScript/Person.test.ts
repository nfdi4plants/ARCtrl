import { equal, deepEqual } from 'assert';
import { Person, Comment, OntologyAnnotation } from "../../src/ARCtrl/index.ts";
import { expect, it, describe } from 'vitest';

describe('Person', function () {
    it('Primary Constructor Minimal', function() {
        const person = new Person();
        expect(person).toBeInstanceOf(Person);
        expect(person.ORCID).toBeUndefined();
        expect(person.LastName).toBeUndefined();
        expect(person.FirstName).toBeUndefined();
        expect(person.MidInitials).toBeUndefined();
        expect(person.EMail).toBeUndefined();
        expect(person.Phone).toBeUndefined();
        expect(person.Fax).toBeUndefined();
        expect(person.Address).toBeUndefined();
        expect(person.Affiliation).toBeUndefined();
        expect(person.Roles).toEqual([]);
        expect(person.Comments).toEqual([]);
    });
    it('Primary Constructor', function() {
        const person = new Person(
            "0000-0002-1825-0097",
            "Frey",
            "Kevin",
            "K. F.",
            "kevin@example.org",
            "+49 123 456",
            "+49 123 457",
            "Example Street 1",
            "Example University",
            [
                new OntologyAnnotation("investigator", "OBI", "OBI:0000202", [new Comment("role", "lead")]),
                new OntologyAnnotation("author", "CREDIT", "CREDIT:00000001")
            ],
            [new Comment("department", "biology"), new Comment("office", "42")]
        );
        expect(person).toBeInstanceOf(Person);
        expect(person.ORCID).toBe("0000-0002-1825-0097");
        expect(person.LastName).toBe("Frey");
        expect(person.FirstName).toBe("Kevin");
        expect(person.MidInitials).toBe("K. F.");
        expect(person.EMail).toBe("kevin@example.org");
        expect(person.Phone).toBe("+49 123 456");
        expect(person.Fax).toBe("+49 123 457");
        expect(person.Address).toBe("Example Street 1");
        expect(person.Affiliation).toBe("Example University");
        expect(person.Roles).toHaveLength(2);
        expect(person.Roles[0]).toBeInstanceOf(OntologyAnnotation);
        expect(person.Roles[0].Name).toBe("investigator");
        expect(person.Roles[0].TermSourceREF).toBe("OBI");
        expect(person.Roles[0].TermAccessionNumber).toBe("OBI:0000202");
        expect(person.Roles[0].Comments).toHaveLength(1);
        expect(person.Roles[0].Comments[0].Name).toBe("role");
        expect(person.Roles[0].Comments[0].Value).toBe("lead");
        expect(person.Roles[1].Name).toBe("author");
        expect(person.Roles[1].TermSourceREF).toBe("CREDIT");
        expect(person.Roles[1].TermAccessionNumber).toBe("CREDIT:00000001");
        expect(person.Roles[1].Comments).toEqual([]);
        expect(person.Comments).toHaveLength(2);
        expect(person.Comments[0].Name).toBe("department");
        expect(person.Comments[0].Value).toBe("biology");
        expect(person.Comments[1].Name).toBe("office");
        expect(person.Comments[1].Value).toBe("42");
    });
    it('Primary Constructor creates fresh roles and comments', function() {
        const first = new Person();
        const second = new Person();
        first.Roles.push(new OntologyAnnotation("investigator"));
        first.Comments.push(new Comment("department", "biology"));
        expect(second.Roles).toEqual([]);
        expect(second.Comments).toEqual([]);
    });
    describe('copy', function () {
        it('check mutable', function () {
            const person = Person.create("0000-0002-1825-0097", "Frey", "Kevin", void 0, "MyAwesomeMail@Awesome.mail");
            const copy = person.Copy()
            equal(person.ORCID, "0000-0002-1825-0097")
            equal(copy.ORCID, "0000-0002-1825-0097")
            equal(person.FirstName, "Kevin");
            equal(copy.FirstName, "Kevin");
            person.FirstName = "NotKevin"
            equal(person.FirstName, "NotKevin");
            equal(copy.FirstName, "Kevin");
        });
        it('check nested mutable', function(){
            const comment = new Comment("TestKey", "TestValue")
            const person = Person.create(void 0, "Frey", "Kevin", void 0, "MyAwesomeMail@Awesome.mail", void 0, void 0, void 0, void 0, void 0, [comment]);
            const copy = person.Copy()
            equal(person.Comments.length,1)
            deepEqual(person.Comments,copy.Comments, "Should be same")
            person.Comments[0].Name = "NewTestName"
            let expectedComment = new Comment("NewTestName", "TestValue")
            deepEqual(person.Comments[0], expectedComment,"Should be new comment")
            deepEqual(copy.Comments[0], new Comment("TestKey", "TestValue"), "should be original comment")
        })
    });
});
