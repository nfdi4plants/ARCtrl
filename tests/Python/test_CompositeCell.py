from src.ARCtrl import Comment, CompositeCell, Data, DataFile, OntologyAnnotation
from src.ARCtrl.py.Core.Table.composite_cell import (
    CompositeCell_Term, CompositeCell_FreeText, CompositeCell_Unitized, CompositeCell_Data,
)

class TestFactories:

    def test_free_text(self):
        cell = CompositeCell.free_text("Input1")
        assert cell.AsFreeText == "Input1"

    def test_term(self):
        oa = OntologyAnnotation("instrument model", "MS", "MS:1234567")
        cell = CompositeCell.term(oa)
        assert cell.AsTerm.NameText == "instrument model"

    def test_unitized(self):
        cell = CompositeCell.unitized("20", OntologyAnnotation("degree celsius"))
        value, unit = cell.AsUnitized
        assert value == "20"
        assert unit.NameText == "degree celsius"

    def test_empty_free_text(self):
        assert CompositeCell.empty_free_text.AsFreeText == ""

class TestHashCode:

    def test_long_free_text(self):
        value = "https://example.org/" + "segment/" * 64
        assert CompositeCell.free_text(value).GetHashCode() == CompositeCell.free_text(value).GetHashCode()


class TestPrimaryConstructor:
    def test_term(self):
        annotation = OntologyAnnotation("temperature", "PATO", "PATO:0000146", [Comment("term", "curated")])
        cell = CompositeCell_Term(annotation)
        assert cell == CompositeCell.term(annotation)
        assert cell.tag == 0
        assert cell.is_term
        assert len(cell.fields) == 1
        assert cell.AsTerm is annotation
        assert (cell.AsTerm.Name, cell.AsTerm.TermSourceREF, cell.AsTerm.TermAccessionNumber) == (
            "temperature", "PATO", "PATO:0000146",
        )
        assert [(c.Name, c.Value) for c in cell.AsTerm.Comments] == [("term", "curated")]

    def test_free_text(self):
        cell = CompositeCell_FreeText("measurement note")
        assert cell == CompositeCell.free_text("measurement note")
        assert cell.tag == 1
        assert cell.is_free_text
        assert cell.AsFreeText == "measurement note"
        assert list(cell.fields) == ["measurement note"]

    def test_unitized(self):
        unit = OntologyAnnotation("gram", "UO", "UO:0000021", [Comment("unit", "SI")])
        cell = CompositeCell_Unitized("5.25", unit)
        assert cell == CompositeCell.unitized("5.25", unit)
        assert cell.tag == 2
        assert cell.is_unitized
        assert len(cell.fields) == 2
        value, actual_unit = cell.AsUnitized
        assert value == "5.25"
        assert actual_unit is unit
        assert (actual_unit.Name, actual_unit.TermSourceREF, actual_unit.TermAccessionNumber) == (
            "gram", "UO", "UO:0000021",
        )
        assert [(c.Name, c.Value) for c in actual_unit.Comments] == [("unit", "SI")]

    def test_data(self):
        data = Data("cell-data", "data/raw.csv#row=3", DataFile.raw_data_file(), "text/csv", "RFC 7111", [Comment("source", "instrument")])
        cell = CompositeCell_Data(data)
        assert cell == CompositeCell.data(data)
        assert cell.tag == 3
        assert cell.is_data
        assert len(cell.fields) == 1
        assert cell.AsData is data
        assert data.ID == "cell-data"
        assert data.Name == "data/raw.csv#row=3"
        assert data.FilePath == "data/raw.csv"
        assert data.Selector == "row=3"
        assert data.DataType.IsRawData
        assert list(data.DataType.fields) == []
        assert data.Format == "text/csv"
        assert data.SelectorFormat == "RFC 7111"
        assert [(c.Name, c.Value) for c in data.Comments] == [("source", "instrument")]
