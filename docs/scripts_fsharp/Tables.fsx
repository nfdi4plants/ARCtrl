#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl"

open ARCtrl.ISA

// create investigation
let myInvestigation = ArcInvestigation("BestInvestigation") 
// init study on investigation 
let myStudy = myInvestigation.InitStudy("BestStudy")
// init table on study
let growth = myStudy.InitTable("Growth")

// create ontology annotation for "species"
let oa_species =
    OntologyAnnotation.fromString(
        "species", "GO", "GO:0123456"
    )
// create ontology annotation for "chlamy"
let oa_chlamy = 
    OntologyAnnotation.fromString(
        "Chlamy", "NCBI", "NCBI:0123456"
    )

// append first column to table. 
// This will create an input column with one row cell.
// In xlsx this will be exactly 1 column.
growth.AddColumn(
    CompositeHeader.Input IOType.Source,
    [|CompositeCell.createFreeText "Input1"|]
)

// append second column to table. 
// this will create an Characteristic [species] column with one row cell.
// in xlsx this will be exactly 3 columns.
growth.AddColumn(
    CompositeHeader.Characteristic oa_species, 
    [|CompositeCell.createTerm oa_chlamy|]
)

myInvestigation