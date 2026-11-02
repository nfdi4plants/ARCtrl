from src.ARCtrl import Comment, OntologySourceReference


class TestPrimaryConstructor:
    def test_minimal(self):
        reference = OntologySourceReference()
        assert isinstance(reference, OntologySourceReference)
        for name in ("Description", "File", "Name", "Version"):
            assert getattr(reference, name) is None
        assert reference.Comments == []

    def test_all_arguments(self):
        reference = OntologySourceReference(
            "My cool description", "my/cool/filepath", "My cool file name",
            "2.0.0-alpha.01+meta.b6",
            [Comment("isa-column-order", "2"), Comment("lastUpdated", "2026-10-29")],
        )
        assert isinstance(reference, OntologySourceReference)
        assert reference.Description == "My cool description"
        assert reference.File == "my/cool/filepath"
        assert reference.Name == "My cool file name"
        assert reference.Version == "2.0.0-alpha.01+meta.b6"
        assert [(c.Name, c.Value) for c in reference.Comments] == [
            ("isa-column-order", "2"), ("lastUpdated", "2026-10-29"),
        ]

    def test_fresh_comments(self):
        first, second = OntologySourceReference(), OntologySourceReference()
        first.Comments.append(Comment("source", "manual"))
        assert second.Comments == []
