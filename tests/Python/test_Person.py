from src.ARCtrl import Comment, OntologyAnnotation, Person


class TestPrimaryConstructor:
    def test_minimal(self):
        person = Person()
        assert isinstance(person, Person)
        for name in ("ORCID", "LastName", "FirstName", "MidInitials", "EMail", "Phone", "Fax", "Address", "Affiliation"):
            assert getattr(person, name) is None
        assert person.Roles == []
        assert person.Comments == []

    def test_all_arguments(self):
        person = Person(
            "0000-0002-1825-0097", "Frey", "Kevin", "K. F.", "kevin@example.org",
            "+49 123 456", "+49 123 457", "Example Street 1", "Example University",
            [OntologyAnnotation("investigator", "OBI", "OBI:0000202", [Comment("role", "lead")]),
             OntologyAnnotation("author", "CREDIT", "CREDIT:00000001")],
            [Comment("department", "biology"), Comment("office", "42")],
        )
        assert isinstance(person, Person)
        expected = {
            "ORCID": "0000-0002-1825-0097", "LastName": "Frey", "FirstName": "Kevin",
            "MidInitials": "K. F.", "EMail": "kevin@example.org", "Phone": "+49 123 456",
            "Fax": "+49 123 457", "Address": "Example Street 1", "Affiliation": "Example University",
        }
        for name, value in expected.items():
            assert getattr(person, name) == value
        assert len(person.Roles) == 2
        investigator, author = person.Roles
        assert isinstance(investigator, OntologyAnnotation)
        assert (investigator.Name, investigator.TermSourceREF, investigator.TermAccessionNumber) == (
            "investigator", "OBI", "OBI:0000202",
        )
        assert [(c.Name, c.Value) for c in investigator.Comments] == [("role", "lead")]
        assert (author.Name, author.TermSourceREF, author.TermAccessionNumber) == (
            "author", "CREDIT", "CREDIT:00000001",
        )
        assert author.Comments == []
        assert [(c.Name, c.Value) for c in person.Comments] == [("department", "biology"), ("office", "42")]

    def test_fresh_roles_and_comments(self):
        first, second = Person(), Person()
        first.Roles.append(OntologyAnnotation("investigator"))
        first.Comments.append(Comment("department", "biology"))
        assert second.Roles == []
        assert second.Comments == []


class TestCopy:
    def test_mutable_properties(self):
        person = Person.create("0000-0002-1825-0097", "Frey", "Kevin", None, "MyAwesomeMail@Awesome.mail")
        copied = person.Copy()
        assert person.ORCID == copied.ORCID == "0000-0002-1825-0097"
        assert person.FirstName == copied.FirstName == "Kevin"
        person.FirstName = "NotKevin"
        assert person.FirstName == "NotKevin"
        assert copied.FirstName == "Kevin"

    def test_nested_mutable_properties(self):
        person = Person(None, "Frey", "Kevin", None, "MyAwesomeMail@Awesome.mail",
                        None, None, None, None, None, [Comment("TestKey", "TestValue")])
        copied = person.Copy()
        assert [(c.Name, c.Value) for c in person.Comments] == [(c.Name, c.Value) for c in copied.Comments]
        assert len(person.Comments) == 1
        person.Comments[0].Name = "NewTestName"
        assert (person.Comments[0].Name, person.Comments[0].Value) == ("NewTestName", "TestValue")
        assert (copied.Comments[0].Name, copied.Comments[0].Value) == ("TestKey", "TestValue")
