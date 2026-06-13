#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let filePaths =
    [|
        "isa.investigation.xlsx"
        "studies/Study-001/isa.study.xlsx"
        "assays/Measurement/isa.assay.xlsx"
        "assays/Measurement/dataset/results.csv"
    |]

let arc =
    ARC.fromFilePaths filePaths

let readContracts =
    arc.GetReadContracts()
// docs:end

// docs:assert
if readContracts.Length <> 3 then
    failwithf "Expected 3 ISA read contracts, got %i" readContracts.Length
// docs:endassert
