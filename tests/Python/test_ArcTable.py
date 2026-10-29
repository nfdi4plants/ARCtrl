from fable_library.array_ import Array

from src.ARCtrl import (
    ArcTable, Comment, CompositeCell, CompositeHeader, Data, DataFile, IOType,
    OntologyAnnotation,
)

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


def make_default_table():
    annotation = OntologyAnnotation("organism", "OBI", "OBI:0100026", [Comment("term", "curated")])
    unit = OntologyAnnotation("gram", "UO", "UO:0000021")
    data = Data(
        "table-data", "data/results.csv#row=4", DataFile.derived_data_file(),
        "text/csv", "RFC 7111", [Comment("quality", "checked")],
    )
    headers = [
        CompositeHeader.characteristic(annotation),
        CompositeHeader.parameter(OntologyAnnotation("mass")),
        CompositeHeader.input(IOType.data()),
        CompositeHeader.free_text("Notes"),
    ]
    columns = [
        [CompositeCell.term(annotation), CompositeCell.term(OntologyAnnotation("mouse"))],
        [CompositeCell.unitized("5", unit), CompositeCell.unitized("7", unit)],
        [CompositeCell.data(data), CompositeCell.data(Data(None, "data/second.csv"))],
        [CompositeCell.free_text("first row"), CompositeCell.free_text("second row")],
    ]
    return ArcTable("Filled Measurements", headers, columns)


class TestPrimaryConstructor:
    def test_name_only(self):
        table = ArcTable("Empty table")
        assert table.Name == "Empty table"
        assert table.Headers == []
        assert table.ColumnCount == 0
        assert table.RowCount == 0
        assert table.Columns == []

    def test_nested_headers_and_column_major_cells(self):
        annotation = OntologyAnnotation("organism", "OBI", "OBI:0100026", [Comment("term", "curated")])
        unit = OntologyAnnotation("gram", "UO", "UO:0000021")
        data = Data(
            "table-data", "data/results.csv#row=4", DataFile.derived_data_file(),
            "text/csv", "RFC 7111", [Comment("quality", "checked")],
        )
        headers = [
            CompositeHeader.characteristic(annotation),
            CompositeHeader.parameter(OntologyAnnotation("mass")),
            CompositeHeader.input(IOType.data()),
            CompositeHeader.free_text("Notes"),
        ]
        columns = [
            [CompositeCell.term(annotation), CompositeCell.term(OntologyAnnotation("mouse"))],
            [CompositeCell.unitized("5", unit), CompositeCell.unitized("7", unit)],
            [CompositeCell.data(data), CompositeCell.data(Data(None, "data/second.csv"))],
            [CompositeCell.free_text("first row"), CompositeCell.free_text("second row")],
        ]
        table = make_default_table()
        assert table.Name == "Filled Measurements"
        assert table.Headers == headers
        assert table.ColumnCount == 4
        assert table.RowCount == 2
        for column_index, column in enumerate(columns):
            for row_index, cell in enumerate(column):
                assert table.GetCellAt(column_index, row_index) == cell

        characteristic = table.Headers[0]
        assert characteristic.tag == 1
        assert characteristic.fields[0] == annotation
        assert characteristic.fields[0].Name == "organism"
        assert characteristic.fields[0].TermSourceREF == "OBI"
        assert characteristic.fields[0].TermAccessionNumber == "OBI:0100026"
        assert characteristic.fields[0].Comments[0].Name == "term"
        assert characteristic.fields[0].Comments[0].Value == "curated"
        parameter = table.Headers[1]
        assert parameter.tag == 3
        assert parameter.fields[0].Name == "mass"
        input_header = table.Headers[2]
        assert input_header.tag == 11
        assert input_header.fields[0].tag == 2
        assert list(input_header.fields[0].fields) == []
        assert list(table.Headers[3].fields) == ["Notes"]

        unitized = table.GetCellAt(1, 0)
        assert unitized.tag == 2
        assert unitized.fields[0] == "5"
        assert unitized.fields[1].Name == "gram"
        assert unitized.fields[1].TermSourceREF == "UO"
        assert unitized.fields[1].TermAccessionNumber == "UO:0000021"
        data_cell = table.GetCellAt(2, 0)
        assert data_cell.tag == 3
        payload = data_cell.fields[0]
        assert payload.ID == "table-data"
        assert payload.Name == "data/results.csv#row=4"
        assert payload.FilePath == "data/results.csv"
        assert payload.Selector == "row=4"
        assert payload.DataType.tag == 1
        assert payload.Format == "text/csv"
        assert payload.SelectorFormat == "RFC 7111"
        assert payload.Comments[0].Name == "quality"
        assert payload.Comments[0].Value == "checked"


def add_basic_columns(table):
    table.AddColumn(CompositeHeader.input(IOType.source()))
    table.AddColumn(CompositeHeader.protocol_ref())
    table.AddColumn(CompositeHeader.output(IOType.sample()))


class TestTableOperations:
    def test_add_columns(self):
        table = ArcTable("table-1")
        add_basic_columns(table)
        assert table.ColumnCount == 3
        assert len(table.Headers) == 3
        assert table.RowCount == 0

    def test_add_columns_then_rows(self):
        table = ArcTable("table-1")
        add_basic_columns(table)
        table.AddRow()
        assert table.RowCount == 1
        cell = table.GetCellAt(0, 0)
        assert cell is not None
        assert cell.is_free_text is True
        assert list(cell.fields) == [""]

    def test_add_filled_parameter_column(self):
        table = ArcTable("table-1")
        table.AddColumn(
            CompositeHeader.parameter(OntologyAnnotation("mass", "UO", "UO:0000021")),
            [
                CompositeCell.create_unitized_from_string(str(value), "kg", "UO", "UO:0000022")
                for value in range(5, 12)
            ],
        )
        assert table.ColumnCount == 1
        assert table.RowCount == 7
        cell = table.GetCellAt(0, 0)
        assert cell is not None
        assert cell.is_unitized is True
        assert isinstance(cell.fields, Array)
        assert cell.fields[0] == "5"
        assert cell.fields[1] is not None
        assert cell.fields[1].Name == "kg"
        assert cell.fields[1].TermSourceREF == "UO"
        assert cell.fields[1].TermAccessionNumber == "UO:0000022"
