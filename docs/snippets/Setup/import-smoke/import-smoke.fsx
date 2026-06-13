#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let table = ArcTable.init("Smoke test")

table.AddColumn(
    CompositeHeader.Input IOType.Source,
    ResizeArray [| CompositeCell.createFreeText "Source-001" |]
)
// docs:end

// docs:assert
if table.Name <> "Smoke test" then
    failwith "Expected the public ARCtrl import to expose ArcTable."

if table.ColumnCount <> 1 then
    failwithf "Expected one smoke-test column, got %i" table.ColumnCount
// docs:endassert
