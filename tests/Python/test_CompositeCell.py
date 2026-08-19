from src.ARCtrl import CompositeCell, OntologyAnnotation

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
