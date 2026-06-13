# docs:begin
from arctrl import ArcTable, CompositeCell, CompositeHeader, IOType

table = ArcTable.init("Smoke test")

table.AddColumn(
    CompositeHeader.input(IOType.source()),
    [CompositeCell.create_free_text("Source-001")],
)
# docs:end

# docs:assert
if table.Name != "Smoke test":
    raise Exception("Expected the public ARCtrl import to expose ArcTable.")

if table.ColumnCount != 1:
    raise Exception(f"Expected one smoke-test column, got {table.ColumnCount}")
# docs:endassert
