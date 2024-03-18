# This script presents the most straightforward way to create and write an assay file filled with tables
# In principle, there are three steps
# 1. Create and manipulate object in datamodel
# 2. Transform object to generic spreadsheet
# 3. Write spreadsheet to xlsx file (or bytes)

# Import ARCtrl
from arctrl.arctrl import ArcAssay, ArcTable, CompositeHeader, CompositeCell, IOType, OntologyAnnotation
from fsspreadsheet.xlsx import Xlsx
from arctrl.ISA.ISA_Spreadsheet.arc_assay import to_fs_workbook

# Create and manipulate object in datamodel

# Create assay
my_assay = ArcAssay.init("MyAssay")

# Create annotation table
growth = ArcTable.init("Growth")

# Add input column with one value to table
growth.AddColumn(CompositeHeader.input(IOType.source()), [CompositeCell.create_free_text("Input1")])

# Add characteristic column with one value
oa_species = OntologyAnnotation.from_string("species", "GO", "GO:0123456")
oa_chlamy = OntologyAnnotation.from_string("Chlamy", "NCBI", "NCBI:0123456")
growth.AddColumn(CompositeHeader.characteristic(oa_species), [CompositeCell.create_term(oa_chlamy)])

# Add table to assay
my_assay.AddTable(growth)

# Transform object to generic spreadsheet
spreadsheet = to_fs_workbook(my_assay)

# Write spreadsheet to xlsx file (or bytes)
out_path = "./myFile.xlsx"

print(spreadsheet)

Xlsx.to_file(out_path, spreadsheet)