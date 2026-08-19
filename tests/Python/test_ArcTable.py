from src.ARCtrl import ArcTable, CompositeCell, CompositeHeader, IOType

class TestAddColumn:

    def test_documented_example(self):
        """The flow from docs/scripts_python/EXAMPLE_CreateAssayFile.py."""
        table = ArcTable.init("growth")
        table.AddColumn(CompositeHeader.input(IOType.source()), [CompositeCell.free_text("Input1")])
        assert table.ColumnCount == 1
        assert table.RowCount == 1

    def test_long_free_text_cell(self):
        value = "x" * 512
        table = ArcTable.init("t")
        table.AddColumn(CompositeHeader.comment("c"), [CompositeCell.free_text(value)])
        assert table.GetCellAt(0, 0).AsFreeText == value
