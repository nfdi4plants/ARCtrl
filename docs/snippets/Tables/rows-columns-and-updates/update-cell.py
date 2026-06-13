# docs:begin
from arctrl import ArcTable, CompositeCell, CompositeHeader, IOType

measurements = ArcTable.init("Measurements")

measurements.AddColumn(
    CompositeHeader.input(IOType.source()),
    [
        CompositeCell.create_free_text("Source-001"),
        CompositeCell.create_free_text("Source-002"),
    ],
)

measurements.AddColumn(
    CompositeHeader.output(IOType.sample()),
    [
        CompositeCell.create_free_text("Sample-001"),
        CompositeCell.create_free_text("Sample-002"),
    ],
)

measurements.AddRow(
    [
        CompositeCell.create_free_text("Source-003"),
        CompositeCell.create_free_text("Sample-003"),
    ]
)

measurements.UpdateCellAt(
    1,
    2,
    CompositeCell.create_free_text("Sample-003-renamed"),
)

updated_sample = measurements.GetCellAt(1, 2).ToFreeTextCell().AsFreeText
# docs:end

# docs:assert
if measurements.RowCount != 3:
    raise Exception(f"Expected 3 rows, got {measurements.RowCount}")

if measurements.ColumnCount != 2:
    raise Exception(f"Expected 2 columns, got {measurements.ColumnCount}")

if updated_sample != "Sample-003-renamed":
    raise Exception(f"Expected updated sample name, got {updated_sample}")
# docs:endassert
