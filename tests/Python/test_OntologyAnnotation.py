from src.ARCtrl import Comment, OntologyAnnotation, JsonController, ArcInvestigation

class TestOntologyAnnotation:

    def test_create(self):
        oa = OntologyAnnotation("instrument model", "MS", "MS:1234567")
        assert oa.NameText == "instrument model"


class TestPrimaryConstructor:
    def test_minimal(self):
        annotation = OntologyAnnotation()
        assert isinstance(annotation, OntologyAnnotation)
        assert annotation.Name is None
        assert annotation.TermSourceREF is None
        assert annotation.TermAccessionNumber is None
        assert annotation.Comments == []

    def test_all_arguments(self):
        annotation = OntologyAnnotation(
            "Homo sapiens", "NCBITaxon", "NCBITaxon:9606",
            [Comment("source", "curated"), Comment("version", "2026")],
        )
        assert isinstance(annotation, OntologyAnnotation)
        assert annotation.Name == "Homo sapiens"
        assert annotation.TermSourceREF == "NCBITaxon"
        assert annotation.TermAccessionNumber == "NCBITaxon:9606"
        assert [(c.Name, c.Value) for c in annotation.Comments] == [("source", "curated"), ("version", "2026")]

    def test_fresh_comments(self):
        first, second = OntologyAnnotation(), OntologyAnnotation()
        first.Comments.append(Comment("source", "curated"))
        assert second.Comments == []
