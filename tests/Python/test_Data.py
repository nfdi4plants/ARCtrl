from src.ARCtrl import Comment, Data, DataFile


class TestPrimaryConstructor:
    def test_minimal(self):
        data = Data()
        for name in ("ID", "Name", "FilePath", "Selector", "DataType", "Format", "SelectorFormat"):
            assert getattr(data, name) is None
        assert data.Comments == []

    def test_all_arguments(self):
        data_type = DataFile.derived_data_file()
        comments = [Comment("source", "instrument")]
        data = Data("data-id", "data/results.csv#row=7", data_type, "text/csv", "RFC 7111", comments)
        assert data.ID == "data-id"
        assert data.Name == "data/results.csv#row=7"
        assert data.FilePath == "data/results.csv"
        assert data.Selector == "row=7"
        assert data.DataType is data_type
        assert data.DataType.IsDerivedData
        assert data.Format == "text/csv"
        assert data.SelectorFormat == "RFC 7111"
        assert data.Comments is comments
        assert data.Comments[0].Name == "source"
        assert data.Comments[0].Value == "instrument"
