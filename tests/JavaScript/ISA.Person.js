import { equal, deepEqual } from 'assert';
import { Person } from "./ARCtrl/ISA/ISA/JsonTypes/Person.js";
import { Comment$ as Comment } from "./ARCtrl/ISA/ISA/JsonTypes/Comment.js";

describe('ISA.Person', function () {
    describe('copy', function () {
        it('check mutable', function () {
            const person = Person.create(void 0, "Frey", "Kevin", void 0, "MyAwesomeMail@Awesome.mail");
            const copy = person.Copy()
            equal(person.FirstName, "Kevin");
            equal(copy.FirstName, "Kevin");
            person.FirstName = "NotKevin"
            equal(person.FirstName, "NotKevin");
            equal(copy.FirstName, "Kevin");
        });
        it('check nested mutable', function(){
            const comment = Comment.fromString("TestKey", "TestValue")
            const person = Person.create(void 0, "Frey", "Kevin", void 0, "MyAwesomeMail@Awesome.mail", void 0, void 0, void 0, void 0, void 0, [comment]);
            const copy = person.Copy()
            equal(person.Comments.length,1)
            deepEqual(person.Comments,copy.Comments, "Should be same")
            person.Comments[0].Name = "NewTestName"
            let expectedComment = Comment.fromString("NewTestName", "TestValue")
            deepEqual(person.Comments[0], expectedComment,"Should be new comment")
            deepEqual(copy.Comments[0], Comment.fromString("TestKey", "TestValue"), "should be original comment")
        })
    });
});