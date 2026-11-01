from src.ARCtrl import Comment, OntologyAnnotation, Publication


class TestPrimaryConstructor:
    def test_minimal(self):
        publication = Publication()
        assert isinstance(publication, Publication)
        for name in ("PubMedID", "DOI", "Authors", "Title", "Status"):
            assert getattr(publication, name) is None
        assert publication.Comments == []

    def test_all_arguments(self):
        publication = Publication(
            "12345678", "10.1234/example", "Frey K.; Example A.", "An example publication",
            OntologyAnnotation("published", "OBI", "OBI:0000605", [Comment("status", "verified")]),
            [Comment("source", "journal"), Comment("year", "2026")],
        )
        assert isinstance(publication, Publication)
        assert publication.PubMedID == "12345678"
        assert publication.DOI == "10.1234/example"
        assert publication.Authors == "Frey K.; Example A."
        assert publication.Title == "An example publication"
        assert isinstance(publication.Status, OntologyAnnotation)
        assert (publication.Status.Name, publication.Status.TermSourceREF, publication.Status.TermAccessionNumber) == (
            "published", "OBI", "OBI:0000605",
        )
        assert [(c.Name, c.Value) for c in publication.Status.Comments] == [("status", "verified")]
        assert [(c.Name, c.Value) for c in publication.Comments] == [("source", "journal"), ("year", "2026")]

    def test_fresh_comments(self):
        first, second = Publication(), Publication()
        first.Comments.append(Comment("source", "journal"))
        assert second.Comments == []
