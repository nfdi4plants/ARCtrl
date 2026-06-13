// docs:begin
import {
  ArcTable,
  CompositeCell,
  CompositeHeader,
  IOType,
} from "@nfdi4plants/arctrl";

const measurements = ArcTable.init("Measurements");

measurements.AddColumn(
  CompositeHeader.input(IOType.source()),
  [
    CompositeCell.createFreeText("Source-001"),
    CompositeCell.createFreeText("Source-002"),
  ],
);

measurements.AddColumn(
  CompositeHeader.output(IOType.sample()),
  [
    CompositeCell.createFreeText("Sample-001"),
    CompositeCell.createFreeText("Sample-002"),
  ],
);

measurements.AddRow([
  CompositeCell.createFreeText("Source-003"),
  CompositeCell.createFreeText("Sample-003"),
]);

measurements.UpdateCellAt(
  1,
  2,
  CompositeCell.createFreeText("Sample-003-renamed"),
);

const updatedSample = measurements.GetCellAt(1, 2).ToFreeTextCell().AsFreeText;
// docs:end

// docs:assert
if (measurements.RowCount !== 3) {
  throw new Error(`Expected 3 rows, got ${measurements.RowCount}`);
}

if (measurements.ColumnCount !== 2) {
  throw new Error(`Expected 2 columns, got ${measurements.ColumnCount}`);
}

if (updatedSample !== "Sample-003-renamed") {
  throw new Error(`Expected updated sample name, got ${updatedSample}`);
}
// docs:endassert
