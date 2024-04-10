import { ArcInvestigation, ArcTable, OntologyAnnotation, CompositeHeader, CompositeCell, IOType } from "@nfdi4plants/arctrl";

// create investigation
const myInvestigation = ArcInvestigation.init("BestInvestigation")
// init study on investigation 
const myStudy = myInvestigation.InitStudy("BestStudy")
// init table on study
const growth = myStudy.InitTable("Growth")

// create ontology annotation for "species"
const oa_species =
  new OntologyAnnotation(
    "species", "GO", "GO:0123456"
  )
// create ontology annotation for "chlamy"
const oa_chlamy =
  new OntologyAnnotation(
    "Chlamy", "NCBI", "NCBI:0123456"
  );

// add first column to table. 
// this will create an input column with one row cell.
// in xlsx this will be exactly 1 column.
// Syntax on CompositeHeader, IOType and even CompositeCell will soon improve!
growth.AddColumn(
  CompositeHeader.input(IOType.source()),
  [CompositeCell.createFreeText("Input1")]
);

// append second column to table. 
// this will create an Characteristic [species] column with one row cell.
// in xlsx this will be exactly 3 columns.
growth.AddColumn(
  CompositeHeader.characteristic(oa_species),
  [CompositeCell.createTerm(oa_chlamy)]
);

console.log(myInvestigation)