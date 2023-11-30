// This script presents the most straightforward way to create and write an assay file filled with tables
// In principle, there are three steps
// 1. Create and manipulate object in datamodel
// 2. Transform object to generic spreadsheet
// 3. Write spreadsheet to xlsx file (or bytes)

// Import ARCtrl
import * as arctrl from "@nfdi4plants/arctrl"
// Alternative import
// import { 
    //     Comment$ as Comment, OntologyAnnotation, Person, Publication, 
    //     IOType, CompositeHeader, CompositeCell, CompositeColumn, ArcTable, 
    //     ArcAssay, ArcStudy, ArcInvestigation,
    //     Template, Organisation, Templates, JsWeb,
    //     ARC } from "@nfdi4plants/arctrl";

// Import ARCtrl Assay to Spreadsheet transformation
import {toFsWorkbook,fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcAssay.js"
// Import ARCtrl Study to Spreadsheet transformation
// import {toFsWorkbook,fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcStudy.js"
// Import ARCtrl Investigation to Spreadsheet transformation
// import {toFsWorkbook,fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcInvestigation.js"

// Import Spreadsheet to XLSX reader/writer
import {Xlsx} from "@fslab/fsspreadsheet";

// -------- 1. Create and manipulate object in datamodel ----------

// Create assay
const myAssay = arctrl.ArcAssay.init("MyAssay");

// Create annotation table
const growth = arctrl.ArcTable.init("Growth");

// Add input column with one value to table
growth.AddColumn(new arctrl.CompositeHeader.input(arctrl.IOType.source), [arctrl.CompositeCell.createFreeText("Input1")]);

// Add characteristic column with one value
const oa_species = arctrl.OntologyAnnotation.fromString("species", "GO", "GO:0123456");
const oa_chlamy = arctrl.OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456");
growth.AddColumn(new arctrl.CompositeHeader.characteristic(oa_species), [arctrl.CompositeCell.createTerm(oa_chlamy)]);

// Add table to assay
myAssay.AddTable(growth);

// -------- 2. Transform object to generic spreadsheet ----------
let spreadsheet = toFsWorkbook(myAssay);

// -------- 3. Write spreadsheet to xlsx file (or bytes) ----------
const outPath = "./myFile.xlsx";

console.log(Xlsx.toBytes(spreadsheet));

await Xlsx.toFile(outPath,spreadsheet);
