#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let growth = ArcTable.init("Growth")

let oa_species =
    OntologyAnnotation(
        "species",
        "NCIT",
        "NCIT:C45293"
    )

let oa_chlamy =
    OntologyAnnotation(
        "Chlamydomonas reinhardtii",
        "NCBITaxon",
        "NCBITaxon:3055"
    )

let oa_time =
    OntologyAnnotation(
        "time",
        "EFO",
        "EFO:0000721"
    )

let oa_day =
    OntologyAnnotation(
        "day",
        "UO",
        "UO:0000033"
    )

growth.AddColumn(
    CompositeHeader.Input IOType.Source,
    ResizeArray [| CompositeCell.createFreeText "Input1" |]
)

growth.AddColumn(
    CompositeHeader.Characteristic oa_species,
    ResizeArray [| CompositeCell.createTerm oa_chlamy |]
)

growth.AddColumn(
    CompositeHeader.Parameter oa_time,
    ResizeArray [| CompositeCell.createUnitized("5", oa_day) |]
)

growth.AddColumn(
    CompositeHeader.Output IOType.Sample,
    ResizeArray [| CompositeCell.createFreeText "Output1" |]
)
// docs:end

// docs:assert
if growth.Name <> "Growth" then
    failwith "Expected table name to be Growth"

if growth.ColumnCount <> 4 then
    failwithf "Expected 4 columns, got %i" growth.ColumnCount
// docs:endassert
