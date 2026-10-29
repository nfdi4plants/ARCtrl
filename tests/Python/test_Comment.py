from src.ARCtrl import Comment


class TestPrimaryConstructor:
    def test_minimal(self):
        comment = Comment()
        assert isinstance(comment, Comment)
        assert comment.Name is None
        assert comment.Value is None

    def test_all_arguments(self):
        comment = Comment("isa-column-order", "2")
        assert isinstance(comment, Comment)
        assert comment.Name == "isa-column-order"
        assert comment.Value == "2"
