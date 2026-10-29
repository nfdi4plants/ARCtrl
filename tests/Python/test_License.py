from src.ARCtrl import License


class TestPrimaryConstructor:
    def test_minimal(self):
        license_ = License("fulltext", "Permission is hereby granted.")
        assert isinstance(license_, License)
        assert license_.Type == "fulltext"
        assert license_.Content == "Permission is hereby granted."
        assert license_.Path == "LICENSE"

    def test_all_arguments(self):
        license_ = License("fulltext", "Custom license text", "COPYING")
        assert isinstance(license_, License)
        assert license_.Type == "fulltext"
        assert license_.Content == "Custom license text"
        assert license_.Path == "COPYING"
