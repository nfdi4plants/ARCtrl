# docs:begin
from arctrl import ARC

file_paths = [
    "isa.investigation.xlsx",
    "studies/Study-001/isa.study.xlsx",
    "assays/Measurement/isa.assay.xlsx",
    "assays/Measurement/dataset/results.csv",
]

arc = ARC.from_file_paths(file_paths)

read_contracts = arc.GetReadContracts()
# docs:end

# docs:assert
if len(read_contracts) != 3:
    raise Exception(f"Expected 3 ISA read contracts, got {len(read_contracts)}")
# docs:endassert
