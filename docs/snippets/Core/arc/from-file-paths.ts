// docs:begin
import { ARC } from "@nfdi4plants/arctrl";

const filePaths = [
  "isa.investigation.xlsx",
  "studies/Study-001/isa.study.xlsx",
  "assays/Measurement/isa.assay.xlsx",
  "assays/Measurement/dataset/results.csv",
];

const arc = ARC.fromFilePaths(filePaths);

const readContracts = arc.GetReadContracts();
// docs:end

// docs:assert
if (readContracts.length !== 3) {
  throw new Error(`Expected 3 ISA read contracts, got ${readContracts.length}`);
}
// docs:endassert
