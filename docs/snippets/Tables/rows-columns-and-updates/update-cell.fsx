#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let measurements = ArcTable.init("Measurements")

measurements.AddColumn(
    CompositeHeader.Input IOType.Source,
    ResizeArray [|
        CompositeCell.createFreeText "Source-001"
        CompositeCell.createFreeText "Source-002"
    |]
)

measurements.AddColumn(
    CompositeHeader.Output IOType.Sample,
    ResizeArray [|
        CompositeCell.createFreeText "Sample-001"
        CompositeCell.createFreeText "Sample-002"
    |]
)

measurements.AddRow(
    ResizeArray [|
        CompositeCell.createFreeText "Source-003"
        CompositeCell.createFreeText "Sample-003"
    |]
)

measurements.UpdateCellAt(
    1,
    2,
    CompositeCell.createFreeText "Sample-003-renamed"
)

let updatedSample =
    measurements.GetCellAt(1, 2).ToFreeTextCell().AsFreeText
// docs:end

// docs:assert
if measurements.RowCount <> 3 then
    failwithf "Expected 3 rows, got %i" measurements.RowCount

if measurements.ColumnCount <> 2 then
    failwithf "Expected 2 columns, got %i" measurements.ColumnCount

if updatedSample <> "Sample-003-renamed" then
    failwithf "Expected updated sample name, got %s" updatedSample
// docs:endassert
