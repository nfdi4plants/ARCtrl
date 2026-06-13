// docs:begin
import {
  ArcTable,
  CompositeCell,
  CompositeHeader,
  IOType,
} from "@nfdi4plants/arctrl";

const table = ArcTable.init("Smoke test");

table.AddColumn(
  CompositeHeader.input(IOType.source()),
  [CompositeCell.createFreeText("Source-001")],
);
// docs:end

// docs:assert
if (table.Name !== "Smoke test") {
  throw new Error("Expected the public ARCtrl import to expose ArcTable.");
}

if (table.ColumnCount !== 1) {
  throw new Error(`Expected one smoke-test column, got ${table.ColumnCount}`);
}
// docs:endassert
