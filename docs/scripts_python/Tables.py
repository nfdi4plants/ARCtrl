from arctrl.arctrl import ArcInvestigation, ArcTable, OntologyAnnotation, CompositeHeader, CompositeCell, IOType

# Create investigation
my_investigation = ArcInvestigation.init("BestInvestigation")

# Init study on investigation
my_study = my_investigation.InitStudy("BestStudy")

# Init table on study
growth = my_study.InitTable("Growth")

# Create ontology annotation for "species"
oa_species = OntologyAnnotation.from_string("species", "GO", "GO:0123456")

# Create ontology annotation for "chlamy"
oa_chlamy = OntologyAnnotation.from_string("Chlamy", "NCBI", "NCBI:0123456")

# Add first column to table.
# This will create an input column with one row cell.
# In xlsx, this will be exactly 1 column.
# Syntax on CompositeHeader, IOType, and even CompositeCell will soon improve!
growth.AddColumn(
    CompositeHeader.input(IOType.source),
    [CompositeCell.create_free_text("Input1")]
)

# Append second column to table.
# This will create a Characteristic [species] column with one row cell.
# In xlsx, this will be exactly 3 columns.
growth.AddColumn(
    CompositeHeader.characteristic(oa_species),
    [CompositeCell.create_term(oa_chlamy)]
)

print(my_investigation)